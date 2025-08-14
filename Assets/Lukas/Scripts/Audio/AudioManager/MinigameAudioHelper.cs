using FMODUnity;
using UnityEngine;

namespace Scripts.Audio.AudioManager
{
    public class MinigameAudioHelper : MonoBehaviour
    {
        [SerializeField] StudioEventEmitter startEmitter;
        [SerializeField] StudioEventEmitter endEmitter;
        public void PlayStartAudio()
        {
            startEmitter.Play();
        }

        public void PlayEndAudio()
        {
            endEmitter.Play();
        }

        public bool EndAudioIsPlaying()
        {
            return endEmitter.IsPlaying();
        }
    }
}