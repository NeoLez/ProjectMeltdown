using System.Collections.Generic;
using UnityEngine;

namespace SoundSystem
{
    [CreateAssetMenu(fileName = "FootstepsMaterial", menuName = "SO/Sound/FootstepMaterial")]
    public class FootstepMaterialSO : ScriptableObject
    {
        public List<AudioClip> sounds;

        public void PlaySound()
        {
            GameManager.AudioSystem.PlaySound(sounds[Random.Range(0, sounds.Count - 1)], GameManager.AudioSystem.VFX);
        }
    }
}