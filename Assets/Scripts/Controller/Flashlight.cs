using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Audio;
using Root.Managers; 

public class Flashlight : MonoBehaviour
{
    [SerializeField] private Light flashlight;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private AudioClip toggleOnClip;
    [SerializeField] private AudioClip toggleOffClip;

    private bool isOn = false;

    private void Awake()
    {
        if (audioSource != null && sfxMixerGroup != null)
        {
            audioSource.outputAudioMixerGroup = sfxMixerGroup;
        }
    }

    private void OnEnable()
    {
        GameManager.Input.Interaction.Flashlight.performed += ToggleFlashlight;
    }

    private void OnDisable()
    {
        GameManager.Input.Interaction.Flashlight.performed -= ToggleFlashlight;
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