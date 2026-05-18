using System;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Root
{
    public class Train : MonoBehaviour
    {
        [SerializeField] private TrainSpeedController speedController;
        [SerializeField] private TrainBrakeController brakeController;
        [SerializeField] private BatterySlot batterySlot;
        [SerializeField] private EmergencyStopButton emergencyStopButton;
        [SerializeField] private SpeedometerHorizontal speedometerHorizontal;
        [SerializeField] private List<Button> externalDoorButtons;
        [SerializeField] private List<Animator> externalDoors;
        
        [SerializeField] private List<Button> cabDoorButton;
        [SerializeField] private List<Animator> cabDoor;
        
        [SerializeField] private List<TrainPathWaypoint> _waypoints;

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
        [SerializeField] private Transform trainPosition;

        [SerializeField] private List<VisualContainer> containers;
        [SerializeField] private List<VisualContainer> objectsInsideTrain;
        [SerializeField] private List<ItemInsideArea> itemInsideAreas;

        private void Awake() {
            previousDirection = previousDirection == Vector3.zero ? trainPosition.forward : previousDirection;
            mapGenerator.OnAddedPiece += section => _waypoints.AddRange(section.Waypoints);
            GameManager.Input.Interaction.Reset.performed += context => {
                if (_descarrilado)
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            };
            GameManager.Train = this;

            foreach (var button in externalDoorButtons) {
                button.OnClicked += HandleExternalDoorButton;
            }

            foreach (var button in cabDoorButton) {
                button.OnClicked += HandleCabDoorButton;
            }
        }

        public bool IsStopped() {
            return isStopped;
        }

        
        private bool isStopped = true;
        private void Update()
        {
            if (_descarrilado) {
                ui_descarrilado.SetActive(true);
                return;
            }

            float targetSpeed = speedController.GetTargetSpeed();
            float speedDifference = targetSpeed - _currentSpeed;
            float speedChange = CalculateSpeed(targetSpeed, speedDifference);
            
            _currentSpeed += speedChange;
            _currentSpeed = math.clamp(_currentSpeed, 0, speedController.maxTrainSpeed);

            ConsumeBattery();
            
            speedometerHorizontal.SetSpeed(_currentSpeed);

            UpdateSounds(targetSpeed, speedDifference);
            
            MoveTrain();
            
            if (_currentSpeed == 0) {
                if (!isStopped) {
                    TrainStopped();
                }
            }
            else {
                LockExternalDoorButtons();
                if (isStopped) {
                    TrainStarted();
                }
            }
        }

        private void MoveTrain() {
            currentDistanceBetweenPathpoints = Vector3.Distance(_waypoints[0].transform.position, _waypoints[1].transform.position);
            
            var distanceToTravel = _currentSpeed * Time.deltaTime;
            var m =  currentDistanceBetweenPathpoints - currentDistanceTraveledToNextPathpoint;
            while (distanceToTravel > currentDistanceBetweenPathpoints - currentDistanceTraveledToNextPathpoint ) {
                distanceToTravel -= m;
                previousDirection = (_waypoints[1].transform.position - _waypoints[0].transform.position).normalized;
                currentDistanceTraveledToNextPathpoint = 0;
                _waypoints[0].TrainReached();
                _waypoints.RemoveAt(0);
                if (_waypoints[0].maxSpeed < _currentSpeed && !emergencyStopButton.IsBreaking()) {
                    if (_descarriladoTimer >= tiempoDescarrilamiento) {
                        _descarrilado = true;
                        _descarriladoTimer = 0;
                        return;
                    }

                    _descarriladoTimer += (_currentSpeed - _waypoints[0].maxSpeed) * Time.deltaTime;
                }
                else {
                    _descarriladoTimer = 0;
                }
                currentDistanceBetweenPathpoints = Vector3.Distance(_waypoints[0].transform.position, _waypoints[1].transform.position);
                m = currentDistanceBetweenPathpoints - currentDistanceTraveledToNextPathpoint;
            }
            currentDistanceTraveledToNextPathpoint += distanceToTravel;

            var currentDirection = (_waypoints[1].transform.position - _waypoints[0].transform.position).normalized;
            if (_currentSpeed != 0) {
                trainPosition.position = _waypoints[0].transform.position + currentDirection * currentDistanceTraveledToNextPathpoint;
                trainPosition.forward = Vector3.Slerp(previousDirection, currentDirection, currentDistanceTraveledToNextPathpoint / currentDistanceBetweenPathpoints);
            }
        }

        private void TrainStopped() {
            isStopped = true;
            
            if (emergencyStopButton.IsBreaking()) {
                emergencyStopButton.FinishBraking();
            }
            UnlockExternalDoorButtons();
            MovePhysicalTrainToMap();
            SetContainerVisualsToTheirOwnPosition();
        }

        private void TrainStarted() {
            isStopped = false;
            
            if (externalDoorsOpened) {
                CloseExternalDoors(false);
            }
            MovePhysicalTrainToSimulationArea();
            SetContainerVisualsToTrainRelative();
        }

        private float CalculateSpeed(float targetSpeed, float speedDifference) {
            var braking = brakeController.UseBrakeGetAmount() * Time.deltaTime;
            float speedChange = - braking;
            brakeController.Damage(braking * (targetSpeed + _currentSpeed) / (2 * speedController.maxTrainSpeed));

            if (emergencyStopButton.IsBreaking()) {
                speedChange -= emergencyStopButton.brakeSpeed * Time.deltaTime;
            }
            
            if (_currentSpeed < targetSpeed) {
                speedChange += _engineAccelerationRate * math.clamp(targetSpeed / _currentSpeed, 0.5f, 2) * Time.deltaTime;
            }
            else {
                speedChange -= _frictionDecelerationRate * _currentSpeed * Time.deltaTime;
            }

            return speedChange;
        }

        private bool ConsumeBattery() {
            var battery = batterySlot?.GetBattery();
            if (battery != null)
            {
                battery.energy -= Time.deltaTime; //Cambiar logica aca
            }
            //TODO: A lo mejor hacer que si no puede consumir la bateria correctamente retorne falso y de ahi manejar que se apague todo
            return true;
        }

        private void UpdateSounds(float targetSpeed, float speedDifference) {
            engineSound.pitch = math.lerp(engineSoundPitchLow, engineSoundPitchHigh, _currentSpeed / speedController.maxTrainSpeed);
            engineSound.volume = math.lerp(0.1f, 1, targetSpeed / speedController.maxTrainSpeed);
            rattleSound.volume = math.lerp(0, 1, _currentSpeed / speedController.maxTrainSpeed);
            rattleSound.pitch = math.lerp(0.5f, 3, _currentSpeed / speedController.maxTrainSpeed);
            strainSound.volume = math.lerp(strainSoundVolumeLow, strainSoundVolumeHigh, speedDifference / _maxEngineStrain);
        }
        
        private void MovePhysicalTrainToMap() {
            foreach (var objectInsideTrain in objectsInsideTrain) {
                objectInsideTrain.transform.position = trainPosition.TransformPoint(movementTeleport.InverseTransformPoint(objectInsideTrain.transform.position));
                objectInsideTrain.transform.forward = trainPosition.TransformDirection(movementTeleport.InverseTransformDirection(objectInsideTrain.transform.forward));
            }
            transform.position = trainPosition.position;
            transform.rotation = trainPosition.rotation;
        }
        
        private void MovePhysicalTrainToSimulationArea() {
            HashSet<VisualContainer> itemsInside = new();
            foreach (var area in itemInsideAreas) {
                itemsInside.UnionWith(area._containers);
            }
            objectsInsideTrain = itemsInside.ToList();
            
            
            foreach (var objectInsideTrain in objectsInsideTrain) {
                objectInsideTrain.transform.position = movementTeleport.TransformPoint(transform.InverseTransformPoint(objectInsideTrain.transform.position));
                objectInsideTrain.transform.forward = movementTeleport.TransformDirection(transform.InverseTransformDirection(objectInsideTrain.transform.forward));
            }
            transform.position = movementTeleport.position;
            transform.rotation = movementTeleport.rotation;
        }

        private void SetContainerVisualsToTrainRelative() {
            foreach (var container in containers) {
                container.goal = trainPosition;
            }
            foreach (var container in objectsInsideTrain) {
                container.goal = trainPosition;
            }
        }

        private void SetContainerVisualsToTheirOwnPosition() {
            foreach (var container in containers) {
                container.goal = null;
            }
            foreach (var container in objectsInsideTrain) {
                container.goal = null;
            }
        }

        public bool cabDoorOpened = true;
        public void HandleCabDoorButton() {
            if (cabDoorOpened) {
                CloseCabDoors();
            }
            else {
                OpenCabDoors();
            }
        }
        
        private void OpenCabDoors() {
            cabDoorOpened = true;
            LockCabDoorButtons();
            foreach (var door in externalDoors) {
                
            }
            Invoke(nameof(UnlockCabDoorButtons), 2f);
        }

        private void CloseCabDoors() {
            cabDoorOpened = false;
            LockCabDoorButtons();
            foreach (var door in externalDoors) {
                
            }
            Invoke(nameof(UnlockCabDoorButtons), 2f);
        }
        
        private void LockCabDoorButtons() {
            foreach (var button in cabDoorButton) {
                button.Lock();
            }
        }
        
        private void UnlockCabDoorButtons() {
            foreach (var button in cabDoorButton) {
                button.Unlock();
            }   
        }
        
        public bool externalDoorsOpened = true;
        public void HandleExternalDoorButton() {
            if (externalDoorsOpened) {
                CloseExternalDoors(true);
            }
            else {
                OpenExternalDoors();
            }
        }
        
        private void OpenExternalDoors() {
            externalDoorsOpened = true;
            LockExternalDoorButtons();
            foreach (var door in externalDoors) {
                Tween.Custom(0.99f, 0f, new TweenSettings(1f), f => {
                    Debug.Log(f);
                    door.SetFloat("Time", f);
                });
            }
            Invoke(nameof(UnlockExternalDoorButtons), 2f);
        }

        private void CloseExternalDoors(bool unlockOnceClosed) {
            externalDoorsOpened = false;
            LockExternalDoorButtons();
            foreach (var door in externalDoors) {
                Tween.Custom(0f, 0.99f, new TweenSettings(1f), f => {
                    door.SetFloat("Time", f);
                });
            }
            if(unlockOnceClosed)
                Invoke(nameof(UnlockExternalDoorButtons), 2f);
        }
        
        private void LockExternalDoorButtons() {
            foreach (var button in externalDoorButtons) {
                button.Lock();
            }
        }
        
        private void UnlockExternalDoorButtons() {
            foreach (var button in externalDoorButtons) {
                button.Unlock();
            }
        }
        
        public float currentDistanceBetweenPathpoints;
        public float currentDistanceTraveledToNextPathpoint;
        public Vector3 previousDirection;

        public bool EstaDescarrilando() {
            return _descarriladoTimer > 0;
        }

        public float GetCurrentMaxSpeed() 
        {
            return _waypoints.Count > 0 ? _waypoints[0].maxSpeed : 0f;
        }
    }

}