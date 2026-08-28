using System.Collections.Generic;
using Root.Controller;
using Unity.Mathematics;
using UnityEngine;

namespace Root
{
    public class LightDimmer : InteractableDraggable
    {
        [SerializeField] private LightSwitchButton lightSwitch;
        [SerializeField] private List<Light> lights;

        [SerializeField] private List<Renderer> emissiveMat;

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

        [SerializeField] private float _emissiveMaxIntensity;
        private float _initialEmissive;

        private void Start()
        {
            SetCamera(GameManager.Camera);
            
            foreach (var light in lights)
            {
                light.intensity = minIntensity;
                light.range = minRange;
            }
            visuals.localRotation = Quaternion.Euler(0, math.lerp(minRotation, maxRotation, percentage), 0);

            foreach (var mat in emissiveMat) //fijarme si no me agarra el primero que esta ahi
            {
                Material[] mats = mat.materials;
                _initialEmissive = mats[1].GetFloat("_LightIntensity");
            }
        }

        private void Update()
        {
            if (active)
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

            foreach (var mat in emissiveMat)
            {
                Material[] mats = mat.materials;
                _initialEmissive = mats[1].GetFloat("_LightIntensity");
                _initialEmissive = lightSwitch.IsOn() ? math.lerp(0, _emissiveMaxIntensity, percentage) : 0;
            }
            UpdateMousePosition();
        }
    }
} 
