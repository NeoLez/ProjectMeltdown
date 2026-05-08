using System;
using System.Collections.Generic;
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
        
        [SerializeField] private float batteryDrain;

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
        [SerializeField] private Transform visualPosition;
        [SerializeField] private Transform trainPosition;

        [SerializeField] private List<VisualContainer> containers;
        [SerializeField] private List<Transform> objectsInsideTrain;

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
                currentSpeed += Vector3.up;
                trainPosition.position = currentSpeed * _currentSpeed * Time.deltaTime;
                ui_descarrilado.SetActive(true);
                return;
            }

            if (emergencyStopButton.ShouldBreak() && _currentSpeed == 0) {
                emergencyStopButton.ReachedStop();
            }
            
            float targetSpeed = speedController.GetTargetSpeed();
            float speedDifference = targetSpeed - _currentSpeed;

            if (speedDifference > _maxEngineStrain)
            {
                //Debug.Log("Straining Engine: " + targetSpeed + " " + _currentSpeed);
            }

            var braking = brakeController.UseBrakeGetAmount() * Time.deltaTime;
            float speedChange = - braking;
            brakeController.Damage(braking * (targetSpeed + _currentSpeed) / (2 * speedController.maxTrainSpeed));

            if (emergencyStopButton.ShouldBreak()) {
                speedChange -= emergencyStopButton.brakeSpeed * Time.deltaTime;
            }
            

            if (_currentSpeed < targetSpeed) {
                speedChange += _engineAccelerationRate * math.clamp(targetSpeed / _currentSpeed, 0.5f, 2) * Time.deltaTime;
            }
            else {
                speedChange -= _frictionDecelerationRate * _currentSpeed * Time.deltaTime;
            }

            _currentSpeed += speedChange;

            _currentSpeed = math.clamp(_currentSpeed, 0, speedController.maxTrainSpeed);

            var battery = batterySlot?.GetBattery();
            if (battery != null)
            {
                battery.energy -= batteryDrain * Time.deltaTime;
            }
            
            speedometerHorizontal.maxSpeed = speedController.maxTrainSpeed;
            speedometerHorizontal.SetSpeed(_currentSpeed);

            engineSound.pitch = math.lerp(engineSoundPitchLow, engineSoundPitchHigh, _currentSpeed / speedController.maxTrainSpeed);
            engineSound.volume = math.lerp(0.1f, 1, targetSpeed / speedController.maxTrainSpeed);
            rattleSound.volume = math.lerp(0, 1, _currentSpeed / speedController.maxTrainSpeed);
            rattleSound.pitch = math.lerp(0.5f, 3, _currentSpeed / speedController.maxTrainSpeed);
            strainSound.volume = math.lerp(strainSoundVolumeLow, strainSoundVolumeHigh, speedDifference / _maxEngineStrain);

            
            
            currentDistanceBetweenPathpoints = Vector3.Distance(_waypoints[0].transform.position, _waypoints[1].transform.position);
            
            var distanceToTravel = _currentSpeed * Time.deltaTime;
            var m =  currentDistanceBetweenPathpoints - currentDistanceTraveledToNextPathpoint;
            currentSpeed = Vector3.zero;
            while (distanceToTravel > currentDistanceBetweenPathpoints - currentDistanceTraveledToNextPathpoint ) {
                distanceToTravel -= m;
                previousDirection = (_waypoints[1].transform.position - _waypoints[0].transform.position).normalized;
                currentSpeed += previousDirection * m;
                currentDistanceTraveledToNextPathpoint = 0;
                _waypoints[0].TrainReached();
                _waypoints.RemoveAt(0);
                if (_waypoints[0].maxSpeed < _currentSpeed && !emergencyStopButton.ShouldBreak()) {
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
                //dirVector = (_waypoints[0].transform.position - _waypoints[1].transform.position).normalized;
                m = currentDistanceBetweenPathpoints - currentDistanceTraveledToNextPathpoint;
            }
            currentDistanceTraveledToNextPathpoint += distanceToTravel;

            var currentDirection = (_waypoints[1].transform.position - _waypoints[0].transform.position).normalized;
            
            currentSpeed += currentDirection * distanceToTravel;
            Vector3 dirVector = (_waypoints[1].transform.position - _waypoints[0].transform.position).normalized;

            
            if (_currentSpeed == 0) {
                if (!isStopped) {
                    isStopped = true;
                    Debug.Log("A");
                    UnlockExternalDoorButtons();
                    trainPosition.position = _waypoints[0].transform.position + dirVector * currentDistanceTraveledToNextPathpoint;
                    trainPosition.forward = Vector3.Slerp(previousDirection, currentDirection, currentDistanceTraveledToNextPathpoint / currentDistanceBetweenPathpoints);
                    MovePhysicalTrainToTrainPosition();
                    ResetVisualThingies();
                }
            }
            else {
                trainPosition.position = _waypoints[0].transform.position + dirVector * currentDistanceTraveledToNextPathpoint;
                trainPosition.forward = Vector3.Slerp(previousDirection, currentDirection, currentDistanceTraveledToNextPathpoint / currentDistanceBetweenPathpoints);
                
                if (isStopped) {
                    isStopped = false;
                    Debug.Log("B");
                    if (externalDoorsOpened) {
                        CloseExternalDoors(false);
                        Debug.Log("E");
                    }
                    
                    MovePhysicalTrainToStaticArea();
                    SetVisualThingies();
                }
            }
        }

        private void MovePhysicalTrainToTrainPosition() {
            Debug.Log("Moving Train to actual position");
            foreach (var objectInsideTrain in objectsInsideTrain) {
                objectInsideTrain.position = trainPosition.TransformPoint(movementTeleport.InverseTransformPoint(objectInsideTrain.position));
                objectInsideTrain.forward = trainPosition.TransformDirection(movementTeleport.InverseTransformDirection(objectInsideTrain.forward));
            }
            transform.position = trainPosition.position;
            transform.rotation = trainPosition.rotation;
        }
        
        private void MovePhysicalTrainToStaticArea() {
            Debug.Log("Moving Train to static simulation area");
            foreach (var objectInsideTrain in objectsInsideTrain) {
                objectInsideTrain.position = movementTeleport.TransformPoint(transform.InverseTransformPoint(objectInsideTrain.position));
                objectInsideTrain.forward = movementTeleport.TransformDirection(transform.InverseTransformDirection(objectInsideTrain.forward));
            }
            transform.position = movementTeleport.position;
            transform.rotation = movementTeleport.rotation;
        }

        private void SetVisualThingies() {
            foreach (var container in containers) {
                container.goal = visualPosition;
            }
        }

        private void ResetVisualThingies() {
            foreach (var container in containers) {
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
                    Debug.Log(f);
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
        public Vector3 currentSpeed = Vector3.zero;
        public Vector3 previousDirection;
        
        public Vector3 GetSpeed()
        {
            return _currentSpeed * currentSpeed;
        }

        public bool EstaDescarrilando() {
            return _descarriladoTimer > 0;
        }
    }
}