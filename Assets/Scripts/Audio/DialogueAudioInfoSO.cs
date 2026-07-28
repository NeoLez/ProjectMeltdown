using UnityEngine;

namespace Root
{
    [CreateAssetMenu(fileName = "DialogueAudio", menuName = "SO/DialogueAudioInfo")]
    public class DialogueAudioInfoSO : ScriptableObject
    {
        public string id;

        public AudioClip[] dialogueTypingSounds;

        [Range(1, 5)]
        public int frecuencyLevel;

        [Range(-3,3)]
        public float minPitch = 0.5f;

        [Range(-3, 3)]
        public float maxPitch = 2f;

        //considerar cambiar los sonidos basados en el humor de los personajes??
    }
}
