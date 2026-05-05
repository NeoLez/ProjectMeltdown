using System;
using System.Collections.Generic;
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
        [Serializable] public class BrakeLevels {
            public float maxDamage;
            public float maxBraking;
        }

        [SerializeField] public List<BrakeLevels> brakeLevels;
        [SerializeField] public List<GameObject> ledIndicators;

        private void Awake()
        {
            _initialPosition = transform.localPosition;
        }

        public override void Interact(bool state)
        {
            active = state;
            if (active)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
            }
        }

        private void Update() {
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

        private void UpdateBrakeState() {
            if (currentDamage > brakeLevels[currentBrakeLevel].maxDamage) {
                ledIndicators[currentBrakeLevel].SetActive(true);
                currentBrakeLevel++;
                BrakeDegradeSound.Play();
                Debug.Log("Brakes Degraded");
            }
        }

        public float UseBrakeGetAmount()
        {
            return percentage * brakeLevels[currentBrakeLevel].maxBraking;
        }

        public void Damage(float damage) {
            currentDamage += damage;
        }
    }
}