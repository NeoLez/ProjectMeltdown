using System;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace Root
{
    public class Train : MonoBehaviour
    {
        public bool godmode;
        [SerializeField] private TrainSpeedController speedController;
        [SerializeField] private TrainBrakeController brakeController;
        [SerializeField] private BatterySlot batterySlot;
        [SerializeField] private EmergencyStopButton emergencyStopButton;
        [SerializeField] private SpeedometerHorizontal speedometerHorizontal;
        [SerializeField] public ForkDecisionSwitch forkDecisionSwitch;
        [SerializeField] private List<Button> externalDoorButtons;
        [SerializeField] private List<Animator> externalDoors;

        [SerializeField] private float _maxEngineStrain;
        [SerializeField] private float _currentSpeed;
        [SerializeField] private float _engineAccelerationRate;
        [SerializeField] private float _frictionDecelerationRate;
        [SerializeField] private AudioSource engineSound;
        [SerializeField] private float engineSoundPitchLow;
        [SerializeField] private float engineSoundPitchHigh;
        [SerializeField] private AudioSource rattleSound;
        [SerializeField] private AudioSource strainSound;
        [SerializeField] private float strainSoundVolumeLow;
        [SerializeField] private float strainSoundVolumeHigh;

        [SerializeField] private GameObject ui_descarrilado;
        [SerializeField] private MapGeneration mapGenerator;

        [SerializeField] private float tiempoDescarrilamiento;
        [SerializeField] private float _descarriladoTimer;
        private bool _descarrilado;

        [SerializeField] private Transform movementTeleport;
        [SerializeField] public Transform trainPosition;

        [SerializeField] private List<VisualContainer> containers;
        [SerializeField] private List<VisualContainer> objectsInsideTrain;
        [SerializeField] private List<ItemInsideArea> itemInsideAreas;

        // MODIFICADO: campos de consumo de bateria + desaceleracion cuando se agota la bateria, independiente del freno normal
        [SerializeField] private float batteryDrain = 1f;
        [SerializeField] private float strainMultiplier = 0.5f;
        [SerializeField] private float batteryOutDecelerationRate = 0.5f;
        [SerializeField] private float batteryStopTime = 0.1f;

        public TrainAlertSystem AlertSystem;
        [SerializeField] private AudioClip _descarriladoAudio;
        [SerializeField] private AudioClip _trainReleaseAudio;

        public event Action OnPowerLost;
        public event Action OnPowerRestored;
        public event Action OnTrainStartedMoving;
        public event Action OnTrainStoppedMoving;
        private bool _powerLost;
        private bool _engineEnabled = true;

        private void Awake()
        {
            _powerLost = true;
            previousDirection = previousDirection == Vector3.zero ? trainPosition.forward : previousDirection;
            GameManager.Input.Interaction.Reset.performed += HandleResetPerformed;
            GameManager.Train = this;

            foreach (var button in externalDoorButtons){
                button.OnClicked += HandleExternalDoorButton;
            }
        }

        private void HandleResetPerformed(InputAction.CallbackContext context)
        {
            if (_descarrilado)
                SceneManager.LoadScene("Menu");
        }

        private void OnDestroy()
        {
            GameManager.Input.Interaction.Reset.performed -= HandleResetPerformed;
        }

        public bool IsStopped()
        {
            return isStopped;
        }

        private void Start()
        {
            GameManager.AudioSystem.AssignOutputMixerGroup(engineSound, GameManager.AudioSystem.VFX);
            GameManager.AudioSystem.AssignOutputMixerGroup(rattleSound, GameManager.AudioSystem.VFX);
            GameManager.AudioSystem.AssignOutputMixerGroup(strainSound, GameManager.AudioSystem.VFX);
        }

        private bool isStopped = true;
        private void Update()
        {
            if (_descarrilado)
            {
                if (!ui_descarrilado.activeInHierarchy)
                {
                    StopTrainSounds(); 
                    GameManager.AudioSystem.PlaySound(_descarriladoAudio, GameManager.AudioSystem.VFX);
                }
                ui_descarrilado.SetActive(true);
                return;
            }

            float targetSpeed = _powerLost ? 0 : speedController.GetTargetSpeed();
            float speedDifference = targetSpeed - _currentSpeed;
            
            if (!_engineEnabled || !ConsumeBattery(speedDifference))
            {
                if (!_powerLost)
                {
                    _powerLost = true;
                    OnPowerLost?.Invoke();
                }
            }
            else
            {
                if (_powerLost)
                {
                    _powerLost = false;
                    OnPowerRestored?.Invoke();
                }
            }

            float speedChange = CalculateSpeedVariation(targetSpeed, speedDifference);

            _currentSpeed += speedChange;
            _currentSpeed = math.clamp(_currentSpeed, 0, speedController.maxTrainSpeed);

            speedometerHorizontal.SetSpeed(_currentSpeed);

            MoveTrain();

            if (_currentSpeed == 0)
            {
                if (!isStopped)
                {
                    Debug.Log("c");
                    TrainStopped();
                }
            }
            else
            {
                if (!_powerLost) {
                    UpdateSounds(targetSpeed, speedDifference);
                    LockExternalDoorButtons();
                    if (isStopped)
                    {
                        Debug.Log("b");
                        TrainStarted();
                    }
                }
                else {
                    UpdateSounds(0, 0);
                }
            }
        }
        
        public float currentDistanceBetweenPathpoints;
        public float currentDistanceTraveledToNextPathpoint;
        public int waypointIndex;
        public TrainPathWaypoint currentWaypoint;
        public TrainPathWaypoint nextWaypoint;
        public Vector3 previousDirection;

        private void MoveTrain()
        {
            if (currentWaypoint == null)
            {
                waypointIndex = 1;
                currentWaypoint = mapGenerator.GetWaypoint(0);
                nextWaypoint = mapGenerator.GetWaypoint(waypointIndex);
            }

            currentDistanceBetweenPathpoints = Vector3.Distance(currentWaypoint.transform.position, nextWaypoint.transform.position);

            var distanceToTravel = _currentSpeed * Time.deltaTime;
            var m = currentDistanceBetweenPathpoints - currentDistanceTraveledToNextPathpoint;
            while (distanceToTravel > currentDistanceBetweenPathpoints - currentDistanceTraveledToNextPathpoint)
            {
                distanceToTravel -= m;
                previousDirection = (nextWaypoint.transform.position - currentWaypoint.transform.position).normalized;
                currentDistanceTraveledToNextPathpoint = 0;
                nextWaypoint.TrainReached();
                currentWaypoint = nextWaypoint;
                waypointIndex++;
                nextWaypoint = mapGenerator.GetWaypoint(waypointIndex);

                if (currentWaypoint.maxSpeed < _currentSpeed && !emergencyStopButton.IsBreaking())
                {
                    if (_descarriladoTimer >= tiempoDescarrilamiento && !godmode)
                    {
                        _descarrilado = true;
                        _descarriladoTimer = 0;
                        return;
                    }

                    _descarriladoTimer += (_currentSpeed - currentWaypoint.maxSpeed) * Time.deltaTime;
                }
                else
                {
                    _descarriladoTimer = 0;
                }
                currentDistanceBetweenPathpoints = Vector3.Distance(currentWaypoint.transform.position, nextWaypoint.transform.position);
                m = currentDistanceBetweenPathpoints - currentDistanceTraveledToNextPathpoint;
            }
            currentDistanceTraveledToNextPathpoint += distanceToTravel;

            var currentDirection = (nextWaypoint.transform.position - currentWaypoint.transform.position).normalized;

            trainPosition.position = currentWaypoint.transform.position + currentDirection * currentDistanceTraveledToNextPathpoint;
            trainPosition.forward = Vector3.Slerp(previousDirection, currentDirection, currentDistanceTraveledToNextPathpoint / currentDistanceBetweenPathpoints);

            if (_currentSpeed == 0 && transform.position != trainPosition.position)
            {
                MovePhysicalTrainToMap();
            }
        }

        private void TrainStopped()
        {
            isStopped = true;
            OnTrainStoppedMoving?.Invoke();
            GameManager.AudioSystem.PlaySound(_trainReleaseAudio, GameManager.AudioSystem.VFX);
            if (emergencyStopButton.IsBreaking())
            {
                emergencyStopButton.FinishBraking();
            }
            UnlockExternalDoorButtons();
            MovePhysicalTrainToMap();
            SetContainerVisualsToTheirOwnPosition();
        }

        private void TrainStarted()
        {
            isStopped = false;
            OnTrainStartedMoving?.Invoke();
            if (externalDoorsOpened)
            {
                CloseExternalDoors(false);
            }
            MovePhysicalTrainToSimulationArea();
            SetContainerVisualsToTrainRelative();
        }

        private float CalculateSpeedVariation(float targetSpeed, float speedDifference)
        {
            var braking = brakeController.UseBrakeGetAmount() * Time.deltaTime;
            float speedChange = -braking;
            brakeController.Damage(braking * (targetSpeed + _currentSpeed) / (2 * speedController.maxTrainSpeed));

            if (emergencyStopButton.IsBreaking())
            {
                speedChange -= emergencyStopButton.brakeSpeed * Time.deltaTime;
            }

            if (_currentSpeed < targetSpeed)
            {
                speedChange += _engineAccelerationRate * math.clamp(targetSpeed / _currentSpeed, 0.5f, 2) * Time.deltaTime;
            }
            else
            {
                speedChange -= _frictionDecelerationRate * math.clamp(_currentSpeed / speedController.maxTrainSpeed, 0.5f, 1f) * Time.deltaTime;
            }

            return speedChange;
        }

        // permite prender y apagar el motor manualmente 
        public void SetEnginePower(bool on)
        {
            _engineEnabled = on;

            if (!on)
            {
                OnPowerLost?.Invoke();
            }
            else
            {
                OnPowerRestored?.Invoke();
            }
        }

        // MODIFICADO: consumo variable segun el esfuerzo del motor + corta si el motor est� apagado
        private bool ConsumeBattery(float speedDifference)
        {
            var battery = batterySlot?.GetBattery();
            if (battery == null) return false;
            if (battery.energy <= 0) return false;
            battery.energy -= math.max(0, batteryDrain + speedDifference * strainMultiplier) * Time.deltaTime;
            return true;
        }

        private void UpdateSounds(float targetSpeed, float speedDifference)
        {
            engineSound.pitch = math.lerp(engineSoundPitchLow, engineSoundPitchHigh, _currentSpeed / speedController.maxTrainSpeed);
            engineSound.volume = math.lerp(0.1f, 1, targetSpeed / speedController.maxTrainSpeed);
            rattleSound.volume = math.lerp(0, 1, _currentSpeed / speedController.maxTrainSpeed);
            rattleSound.pitch = math.lerp(0.5f, 3, _currentSpeed / speedController.maxTrainSpeed);
            strainSound.volume = math.lerp(strainSoundVolumeLow, strainSoundVolumeHigh, speedDifference / _maxEngineStrain);
        }

        private void StopTrainSounds() // apaga sonidos
        {
            if (engineSound != null) engineSound.Stop();
            if (rattleSound != null) rattleSound.Stop();
            if (strainSound != null) strainSound.Stop();
        }

        private void MovePhysicalTrainToMap()
        {
            if (!isStopped)
            {
                foreach (var objectInsideTrain in objectsInsideTrain)
                {
                    if (objectInsideTrain == null) continue;
                    objectInsideTrain.transform.position = trainPosition.TransformPoint(movementTeleport.InverseTransformPoint(objectInsideTrain.transform.position));
                    objectInsideTrain.transform.forward = trainPosition.TransformDirection(movementTeleport.InverseTransformDirection(objectInsideTrain.transform.forward));
                }
            }
            else
            {
                foreach (var objectInsideTrain in objectsInsideTrain)
                {
                    if (objectInsideTrain == null) continue;
                    objectInsideTrain.transform.position = trainPosition.TransformPoint(transform.InverseTransformPoint(objectInsideTrain.transform.position));
                    objectInsideTrain.transform.forward = trainPosition.TransformDirection(transform.InverseTransformDirection(objectInsideTrain.transform.forward));
                }
            }

            transform.position = trainPosition.position;
            transform.rotation = trainPosition.rotation;
        }

        private void MovePhysicalTrainToSimulationArea()
        {
            HashSet<VisualContainer> itemsInside = new();
            foreach (var area in itemInsideAreas)
            {
                itemsInside.UnionWith(area._containers);
            }
            objectsInsideTrain = itemsInside.ToList();

            foreach (var objectInsideTrain in objectsInsideTrain)
            {
                if (objectInsideTrain == null) continue;
                objectInsideTrain.transform.position = movementTeleport.TransformPoint(transform.InverseTransformPoint(objectInsideTrain.transform.position));
                objectInsideTrain.transform.forward = movementTeleport.TransformDirection(transform.InverseTransformDirection(objectInsideTrain.transform.forward));
            }
            transform.position = movementTeleport.position;
            transform.rotation = movementTeleport.rotation;
        }

        private void SetContainerVisualsToTrainRelative()
        {
            foreach (var container in containers)
            {
                container.goal = trainPosition;
            }
            foreach (var container in objectsInsideTrain)
            {
                container.goal = trainPosition;
            }
        }

        private void SetContainerVisualsToTheirOwnPosition()
        {
            foreach (var container in containers)
            {
                container.goal = null;
            }
            foreach (var container in objectsInsideTrain)
            {
                container.goal = null;
            }
        }

        public bool externalDoorsOpened = true;
        public void HandleExternalDoorButton()
        {
            if (externalDoorsOpened)
            {
                CloseExternalDoors(true);
            }
            else
            {
                OpenExternalDoors();
            }
        }

        private void OpenExternalDoors()
        {
            foreach (var door in externalDoors)
            {
                Tween.Custom(0.99f, 0f, new TweenSettings(1f), f => {
                    door.SetFloat("Time", f);
                });
            }
            externalDoorsOpened = true;
            
            LockExternalDoorButtons(2f);
        }

        private void CloseExternalDoors(bool unlockOnceClosed)
        {
            foreach (var door in externalDoors)
            {
                Tween.Custom(0f, 0.99f, new TweenSettings(1f), f => {
                    door.SetFloat("Time", f);
                });
            }
            externalDoorsOpened = false;

            if (unlockOnceClosed) {
                LockExternalDoorButtons(2f);
            }
            else {
                LockExternalDoorButtons();
            }
        }

        private void LockExternalDoorButtons()
        {
            foreach (var button in externalDoorButtons)
            {
                button.Lock();
            }
        }
        
        private void LockExternalDoorButtons(float duration)
        {
            foreach (var button in externalDoorButtons)
            {
                button.LockForSeconds(duration);
            }
        }

        private void UnlockExternalDoorButtons()
        {
            foreach (var button in externalDoorButtons)
            {
                button.Unlock();
            }
        }

        public bool EstaDescarrilando()
        {
            return _descarriladoTimer > 0;
        }

        public float GetCurrentMaxSpeed()
        {
            return currentWaypoint != null ? currentWaypoint.maxSpeed : 0.0f;
        }

        public Transform GetTrainPosition()
        {
            return trainPosition;
        }
    }
}