using System;
using System.Collections.Generic;
using Root.Controller;
using Unity.Mathematics;
using UnityEngine;
namespace Root
{
    public class TrainBrakeController : Interactable
    {
        private bool active;
        private Vector3 _initialPosition;
        [SerializeField] private float resetSpeed = 1f;
        [SerializeField] private float sensitivity;
        [SerializeField] private float maxSwitchSpeed;
        [SerializeField] private float percentage;
        [SerializeField] private float maxTransformY;
        [SerializeField] private Transform visuals;
        [SerializeField] private AudioSource BrakeDegradeSound;
        private int currentBrakeLevel;
        [SerializeField] private float currentDamage;
        [Serializable]
        public class BrakeLevels
        {
            public float maxDamage;
            public float maxBraking;
        }
        [SerializeField] public List<BrakeLevels> brakeLevels;
        private void Awake()
        {
            _initialPosition = transform.localPosition;
        }
        public override void StartInteraction()
        {
            active = true;
            MouseHandler.RequestControl(CursorLockMode.Locked, false, this);
        }

        public override void EndInteraction()
        {
            active = false;
            MouseHandler.RelinquishControl(this);
        }
        
        private void Update()
        {
            UpdateBrakeState();
            if (active)
            {
                float yMovement = GameManager.Input.CameraMovement.MouseY.ReadValue<float>() * sensitivity;
                if (yMovement > maxSwitchSpeed)
                    yMovement = maxSwitchSpeed;
                else if (yMovement < -maxSwitchSpeed)
                    yMovement = -maxSwitchSpeed;
                percentage -= yMovement;
                percentage = math.clamp(percentage, 0, 1);
                visuals.localPosition = _initialPosition - maxTransformY * percentage * Vector3.forward;
            }
            else
            {
                percentage = math.max(0, percentage - resetSpeed * Time.deltaTime);
                visuals.localPosition = _initialPosition - maxTransformY * percentage * Vector3.forward;
            }
        }
        private void UpdateBrakeState()
        {
            if (currentBrakeLevel >= brakeLevels.Count - 1) return;
            if (currentDamage > brakeLevels[currentBrakeLevel].maxDamage)
            {
                currentBrakeLevel++;
                BrakeDegradeSound.Play();
            }
        }
        public float UseBrakeGetAmount()
        {
            return percentage * brakeLevels[currentBrakeLevel].maxBraking;
        }
        public void Damage(float damage)
        {
            currentDamage += damage;
        }
        public void Repair(float amount)
        {
            currentDamage = Mathf.Max(0, currentDamage - amount);
            while (currentBrakeLevel > 0 && currentDamage <= brakeLevels[currentBrakeLevel - 1].maxDamage)
            {
                currentBrakeLevel--;
            }
        }
        public int GetBrakeLevel() => currentBrakeLevel;
    }
}