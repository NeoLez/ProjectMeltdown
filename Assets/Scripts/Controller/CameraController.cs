using Root;
using Root.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using Root.Managers;
using Timers;
using Unity.Mathematics;
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
    [SerializeField] public float cameraSmoothing = 0.1f;

    [SerializeField] private float sideSwayAngle;
    [SerializeField] private float swaySpeed;
    [SerializeField] private float horizontalAmount;
    [SerializeField] private float verticalAmount;
    [SerializeField] private float runningVerticalAmount;
    [SerializeField] private float frequency;
    [SerializeField] private float runningFrequency;
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

    [SerializeField] private GameObject crosshair;
    [SerializeField] private List<Sprite> _crosshairSprite;
    private Image _crosshairImage;

    public Timer walkCancelTimer = new Timer();
    public float stepSoundTime = 0;

    private bool _limitRotation;
    private Vector3 _currentEuler;
    private InteractableNormalCamera _currentInteractable;

    private void Awake()
    {
        GameManager.CameraController = this;
    }
    
    private void Start()
    {        
        _crosshairImage = crosshair.GetComponent<Image>();
        _input = GameManager.Input;
        _input.CameraMovement.Enable();
        _input.Interaction.Interact.performed += HandleInteraction;
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
        _input.Interaction.Interact.performed -= HandleInteraction;
        DialogueManager.Instance.OnDialogueEnded -= EnableNormalMovement;
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

    private void Update() {
        TryFindInteractableObject(out var newInteractable);

        if (_currentInteractable != newInteractable || newInteractable == null)
        {
            if (_currentInteractable != null)
            {
                _currentInteractable.ShowFeedback(false);
            }

            _currentInteractable = newInteractable;

            if (_currentInteractable != null)
            {
                _currentInteractable.ShowFeedback(true);
                _crosshairImage.sprite = _crosshairSprite[1];
            }
            else
            {
                _crosshairImage.sprite = _crosshairSprite[0];
            }
        }
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
        cam.position = new Vector3(cameraPosition.position.x,
            math.lerp(cam.position.y, cameraPosition.position.y, cameraSmoothing), cameraPosition.position.z) + viewBobVector;
        cam.localPosition += CameraShakeManager.Instance.GetShakeOffset();

        if(!_limitRotation)
        {
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
    }

    public float NormalizeAngle(float angle)
    {
        angle %= 360;
        if (angle > 180) angle -= 360;
        return angle;
    }

    private void HeadBob()
    {
        float s = Mathf.Sin((Time.time - startedWalk) * (_input.Movement.Run.IsPressed() ? runningFrequency : frequency) + (float)Math.PI / 2);
        cameraBobbingOffset.y = Mathf.Lerp(cameraBobbingOffset.y, s * (_input.Movement.Run.IsPressed()? runningVerticalAmount : verticalAmount), smooth * Time.deltaTime);
        cameraBobbingOffset.x = Mathf.Lerp(cameraBobbingOffset.x, Mathf.Cos((Time.time - startedWalk) * frequency / 2 + (float)Math.PI / 2) * horizontalAmount, smooth * Time.deltaTime);
    }

    public void FocusCamera(Transform newPivot)
    {
        DialogueManager.Instance.OnDialogueEnded += EnableNormalMovement;

        _limitRotation = true;
        StartCoroutine(RotateCamerTowardsNPC(newPivot));
    }

    private IEnumerator RotateCamerTowardsNPC(Transform newPivot)
    {
        float journeyProgress = 0f;
        Quaternion targetRot = newPivot.rotation;

        while (Quaternion.Angle(cam.rotation, targetRot) > 0.01f)
        {
            journeyProgress += Time.deltaTime * 0.5f;

            float percentage = Mathf.Clamp01(journeyProgress);
            cam.rotation = Quaternion.Slerp(cam.rotation, targetRot, percentage);

            if (percentage >= 1f) break;

            yield return null;
        }

        cam.rotation = targetRot;
        _currentEuler = cam.localEulerAngles;
    }

    private void EnableNormalMovement()
    {
        _limitRotation = false;
        pitch = -NormalizeAngle(_currentEuler.x);
        yaw = NormalizeAngle(_currentEuler.y);
    }


    
    
    public void SyncToRotation(Quaternion targetWorldRotation)
    {
        Quaternion targetLocalRotation = targetWorldRotation;
        Vector3 euler = targetLocalRotation.eulerAngles;
        pitch = Mathf.Clamp(-NormalizeAngle(euler.x), -89f, 89f);
        yaw = NormalizeAngle(euler.y);
        cam.localRotation = Quaternion.Euler(-pitch, yaw, currentSideSwayAngle);
        cam.position = cameraPosition.position;
    }

    public void LockCamera()
    {
        MouseHandler.RequestControl(CursorLockMode.Locked, false, this, true);
        cam.position = cameraPosition.position;
        if (crosshair != null) crosshair.SetActive(true);
    }

    public void UnlockCamera()
    {
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