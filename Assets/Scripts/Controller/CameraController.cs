using System;
using System.Collections.Generic;
using Root;
using Root.Controller;
using Timers;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private Transform cam;
    [SerializeField] public float sensitivity = 1;
    [SerializeField] public float interactDistance = 2;

    [SerializeField] private float sideSwayAngle;
    [SerializeField] private float swaySpeed;
    [SerializeField] private float horizontalAmount;
    [SerializeField] private float verticalAmount;
    [SerializeField] private float frequency;
    [SerializeField] private float smooth;
    private Vector2 cameraBobbingOffset;
    private float currentSideSwayAngle;
    
    [SerializeField] private LayerMask raycastLayerMask;

    [SerializeField] private float waitTimeForFootstepStateChange = 0.05f;

    [SerializeField] private float yaw;
    [SerializeField] private float pitch;
    private bool walkedPreviousFrame;
    private float startedWalk;

    private PlayerInputActions _input;
    private MovementController _movementController;
    private PlayerItemHolder _playerItemHolder;

    // MODIFICADO: referencia al crosshair que se muestra cuando el cursor esta bloqueado
    [SerializeField] private GameObject crosshair;
    [SerializeField] private List<Sprite> _crosshairSprite; //System collections Generic
    private Image _crosshairImage;

    public Timer walkCancelTimer = new Timer();
    public float stepSoundTime = 0;
    
    private void Start()
    {
        _crosshairImage = crosshair.GetComponent<Image>();
        _input = GameManager.Input;
        _input.Interaction.Interact.started += HandleInteraction;
        _movementController = GetComponent<MovementController>();
        _playerItemHolder = GetComponent<PlayerItemHolder>();
        Assert.IsNotNull(_playerItemHolder);
    }

    private void OnEnable()
    {
        LockCamera();
    }

    private void OnDestroy()
    {
        _input.Interaction.Interact.started -= HandleInteraction;
    }

    private void HandleInteraction(InputAction.CallbackContext _)
    {
        if (_playerItemHolder.HasItem) {
            if (!TryFindInteractableObject(out var component)) 
                _playerItemHolder.Drop();
            else
                component.Interact();
            return;
        }

        if (!TryFindInteractableObject(out var interactable))
            return;
        
        interactable.Interact();
    }

    private bool TryFindInteractableObject(out InteractableNormalCamera interactable) {
        interactable = null;
        return Physics.Raycast(cam.position, cam.forward, out var hit, interactDistance, raycastLayerMask) && hit.collider.gameObject.TryGetComponent(out interactable);
    }

    private void Update()
    {
        if (!TryFindInteractableObject(out _))
        {
            _crosshairImage.sprite = _crosshairSprite[0];
        }
        else _crosshairImage.sprite = _crosshairSprite[1];
    }

    private void LateUpdate()
    {
        Vector2 moveDir = _input.Movement.MoveDir.ReadValue<Vector2>();

        if (moveDir.magnitude > 0 && _movementController.GetState() != CharacterState.Air)
        {
            if (!walkedPreviousFrame)
            {
                walkedPreviousFrame = true;
                startedWalk = Time.time;
                stepSoundTime = (float)Math.PI / frequency;
            }
            walkCancelTimer.Reset(waitTimeForFootstepStateChange);
            HeadBob();
        }
        else
        {
            if (walkedPreviousFrame)
            {
                if (walkCancelTimer.IsCompleted())
                {
                    walkedPreviousFrame = false;
                    cameraBobbingOffset = Vector2.Lerp(cameraBobbingOffset, Vector2.zero, smooth * Time.deltaTime);
                }
            }
        }

        if (walkedPreviousFrame)
        {
            stepSoundTime -= Time.deltaTime;
            if (stepSoundTime <= 0)
            {
                stepSoundTime += (float)Math.PI * 2 / frequency;
            }
        }

        Vector3 viewBobVector = GetHorizontalDirectionRightVector().Swizzle_x0y() * cameraBobbingOffset.x + Vector3.up * cameraBobbingOffset.y;
        cam.position = cameraPosition.position + viewBobVector;
        CalculateShakeOffset();
        cam.localPosition += GetShakeOffset();

        yaw += _input.CameraMovement.MouseX.ReadValue<float>() * sensitivity;
        pitch += _input.CameraMovement.MouseY.ReadValue<float>() * sensitivity;

        pitch = Mathf.Clamp(pitch, -89f, 89f);
        if (yaw > 360)
            yaw -= 360;
        else if (yaw < 0)
            yaw += 360;

        float target = -moveDir.x * sideSwayAngle;
        currentSideSwayAngle = (target - currentSideSwayAngle) * swaySpeed + currentSideSwayAngle;
        cam.localRotation = Quaternion.Euler(-pitch, yaw, currentSideSwayAngle);
    }

    private void HeadBob()
    {
        float s = Mathf.Sin((Time.time - startedWalk) * frequency + (float)Math.PI / 2);
        cameraBobbingOffset.y = Mathf.Lerp(cameraBobbingOffset.y, s * verticalAmount * 1.4f, smooth * Time.deltaTime);
        cameraBobbingOffset.x = Mathf.Lerp(cameraBobbingOffset.x, Mathf.Cos((Time.time - startedWalk) * frequency / 2 + (float)Math.PI / 2) * horizontalAmount * 1.6f, smooth * Time.deltaTime);
    }
    
    public float shakeIntensity;
    public float targetShakeIntensity;
    public float shakeTime;
    public float shakeIntensityLerp;
    public float shakeLerpBetweenRandomVectors;
    private Vector3 GetShakeOffset()
    {
        return new Vector3(
            Random.Range(-1f, 1f) * shakeIntensity,
            Random.Range(-1f, 1f) * shakeIntensity,
            0f
        );
    }

    public void Shake(float intensity, float time) {
        targetShakeIntensity = intensity;
        shakeTime = time + Time.time;
    }

    private void CalculateShakeOffset() {
        if (shakeTime < Time.time) {
            shakeIntensity = Mathf.Lerp(shakeIntensity, 0, shakeIntensityLerp);
            return;
        }
        shakeIntensity = Mathf.Lerp(shakeIntensity, targetShakeIntensity, shakeIntensityLerp);
    }

    public void AddPitch(float n)
    {
        pitch += n;
    }

    public void AddYaw(float n)
    {
        yaw += n;
    }

    public void LockCamera()
    {
        // MODIFICADO: pasa true para mostrar el crosshair cuando el cursor esta bloqueado
        MouseHandler.RequestControl(CursorLockMode.Locked, false, this, true);
        if (crosshair != null) crosshair.SetActive(true);
    }

    public void UnlockCamera()
    {
        // MODIFICADO: apaga el crosshair al desbloquear la camara
        if (crosshair != null) crosshair.SetActive(false);
        MouseHandler.RelinquishControl(this);
    }

    public Vector2 GetHorizontalDirectionForwardVector()
    {
        return new Vector2(Mathf.Sin(yaw * Mathf.Deg2Rad), Mathf.Cos(yaw * Mathf.Deg2Rad));
    }

    public Vector2 GetHorizontalDirectionRightVector()
    {
        return new Vector2(Mathf.Cos(yaw * Mathf.Deg2Rad), -Mathf.Sin(yaw * Mathf.Deg2Rad));
    }
}