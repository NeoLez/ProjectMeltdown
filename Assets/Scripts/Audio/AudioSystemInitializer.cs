using UnityEngine;

public class AudioSystemInitializer : MonoBehaviour
{
    [SerializeField] private AudioSource nonPositionalSource;

    private void Awake()
    {
        GameManager.AudioSystem.NonPositionAudioSource = nonPositionalSource;
    }
}