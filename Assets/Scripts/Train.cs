using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Root
{
    public class Train : MonoBehaviour
    {
        [SerializeField] private TrainSpeedController speedController;
        [SerializeField] private TrainBrakeController brakeController;
        [SerializeField] private BatterySlot batterySlot;
        [SerializeField] private Speedometer speedometer;
        [SerializeField] private SpeedometerHorizontal speedometerHorizontal;
        [SerializeField] private List<TrainPathWaypoint> _waypoints;
        
        [SerializeField] private float batteryDrain;

        [SerializeField] private float _maxEngineStrain;
        [SerializeField] private float _currentSpeed;
        [SerializeField] private float _accelerationRate;
        [SerializeField] private float _decelerationRate;
        [SerializeField] private AudioSource engineSound;
        [SerializeField] private float engineSoundPitchLow;
        [SerializeField] private float engineSoundPitchHigh;
        [SerializeField] private AudioSource rattleSound;
        [SerializeField] private AudioSource strainSound;
        [SerializeField] private float strainSoundVolumeLow;
        [SerializeField] private float strainSoundVolumeHigh;
        
        [SerializeField] private GameObject ui_descarrilado;

        private bool _descarrilado;

        private void Awake() {
            previousDirection = previousDirection == Vector3.zero ? transform.forward : previousDirection;
        }

        private void Update()
        {
            if (_descarrilado) {
                currentSpeed += Vector3.up;
                transform.position = currentSpeed * _currentSpeed * Time.deltaTime;
                ui_descarrilado.SetActive(true);
                return;
            }
            
            float targetSpeed = speedController.GetTargetSpeed() * (1 - brakeController.GetBrakeAmount());
            float speedDifference = targetSpeed - _currentSpeed;

            if (speedDifference > _maxEngineStrain)
            {
                Debug.Log("Straining Engine: " + targetSpeed + " " + _currentSpeed);
            }

            if (_currentSpeed > targetSpeed)
            {
                _currentSpeed -= _decelerationRate * (brakeController.GetBrakeAmount() * 40 + 1) * Time.deltaTime;
                if (_currentSpeed < targetSpeed)
                {
                    _currentSpeed = targetSpeed;
                }
            }
            else if (_currentSpeed < targetSpeed)
            {
                _currentSpeed += _accelerationRate * Time.deltaTime;
                if (_currentSpeed > targetSpeed)
                {
                    _currentSpeed = targetSpeed;
                }
            }

            var battery = batterySlot.GetBattery();
            if (battery != null)
            {
                battery.energy -= batteryDrain * Time.deltaTime;
            }

            speedometer.maxSpeed = speedController.maxTrainSpeed;
            speedometer.SetSpeed(_currentSpeed);
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
                _waypoints.RemoveAt(0);
                if (_waypoints[0].maxSpeed < _currentSpeed) {
                    _descarrilado = true;
                    return;
                }
                currentDistanceBetweenPathpoints = Vector3.Distance(_waypoints[0].transform.position, _waypoints[1].transform.position);
                //dirVector = (_waypoints[0].transform.position - _waypoints[1].transform.position).normalized;
                m = currentDistanceBetweenPathpoints - currentDistanceTraveledToNextPathpoint;
            }
            currentDistanceTraveledToNextPathpoint += distanceToTravel;

            var currentDirection = (_waypoints[1].transform.position - _waypoints[0].transform.position).normalized;
            
            currentSpeed += currentDirection * distanceToTravel;
            Vector3 dirVector = (_waypoints[1].transform.position - _waypoints[0].transform.position).normalized;
            transform.position = _waypoints[0].transform.position + dirVector * currentDistanceTraveledToNextPathpoint;
            transform.forward = Vector3.Slerp(previousDirection, currentDirection, currentDistanceTraveledToNextPathpoint / currentDistanceBetweenPathpoints);
        }
        
        public float currentDistanceBetweenPathpoints;
        public float currentDistanceTraveledToNextPathpoint;
        public Vector3 currentSpeed = Vector3.zero;
        public Vector3 previousDirection;
        
        public Vector3 GetSpeed()
        {
            return _currentSpeed * currentSpeed;
        }
    }
}