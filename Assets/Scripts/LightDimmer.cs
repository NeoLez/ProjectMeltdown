using System.Collections.Generic;
using Root.Controller;
using Unity.Mathematics;
using UnityEngine;

namespace Root
{
    public class LightDimmer : Interactable
    {
        [SerializeField] private LightSwitch lightSwitch;
        [SerializeField] private List<Light> lights;

        [SerializeField] private float sensitivity;
        [SerializeField] private float maxSwitchSpeed;

        [SerializeField] private float minIntensity;
        [SerializeField] private float maxIntensity;
        [SerializeField] private float minRange;
        [SerializeField] private float maxRange;

        [SerializeField] private float minRotation;
        [SerializeField] private float maxRotation;
        [SerializeField] private Transform visuals;

        [SerializeField] private float percentage;

        private bool _active;

        private void Start()
        {
            foreach (var light in lights)
            {
                light.intensity = minIntensity;
                light.range = minRange;
            }
            visuals.localRotation = Quaternion.Euler(0, math.lerp(minRotation, maxRotation, percentage), 0);
        }

        public override void StartInteraction()
        {
            _active = true;
            MouseHandler.RequestControl(CursorLockMode.Locked, false, this);
        }

        public override void EndInteraction()
        {
            _active = false;
            MouseHandler.RelinquishControl(this);
        }

        private void Update()
        {
            if (_active)
            {
                float xMovement = GameManager.Input.CameraMovement.MouseX.ReadValue<float>() * sensitivity;
                xMovement = math.clamp(xMovement, -maxSwitchSpeed, maxSwitchSpeed);
                percentage += xMovement;
                percentage = math.clamp(percentage, 0, 1);
                visuals.localRotation = Quaternion.Euler(0, math.lerp(minRotation, maxRotation, percentage), 0);
            }

            foreach (var light in lights)
            {
                light.intensity = lightSwitch.IsOn() ? math.lerp(minIntensity, maxIntensity, percentage) : 0;
                light.range = lightSwitch.IsOn() ? math.lerp(minRange, maxRange, percentage) : 0;
            }
        }
    }
}