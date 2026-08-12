using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Audio;

public class Flashlight : MonoBehaviour
{
    [SerializeField] private Light flashlight;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private AudioClip toggleOnClip;
    [SerializeField] private AudioClip toggleOffClip;

    private PlayerInputActions inputActions;
    private bool isOn = false;

    private void Awake()
    {
        inputActions = new PlayerInputActions();

        if (audioSource != null && sfxMixerGroup != null)
        {
            audioSource.outputAudioMixerGroup = sfxMixerGroup;
        }
    }

    private void OnEnable()
    {
        inputActions.Interaction.Enable();
        inputActions.Interaction.Flashlight.performed += ToggleFlashlight;
    }

    private void OnDisable()
    {
        inputActions.Interaction.Flashlight.performed -= ToggleFlashlight;
        inputActions.Interaction.Disable();
    }

    private void ToggleFlashlight(InputAction.CallbackContext context)
    {
        isOn = !isOn;
        flashlight.enabled = isOn;

        AudioClip clipToPlay = isOn ? toggleOnClip : toggleOffClip;
        if (audioSource != null && clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay);
        }
    }
}