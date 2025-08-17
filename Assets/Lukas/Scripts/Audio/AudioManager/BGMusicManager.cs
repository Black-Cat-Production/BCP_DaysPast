using System.Collections;
using FMODUnity;
using UnityEngine;

namespace Scripts.Audio.AudioManager
{
    public class BGMusicManager : MonoBehaviour
    {
        StudioEventEmitter eventEmitter;

        string theme01 = "event:/Music/THEME_01";
        string theme04 = "event:/Music/THEME_04";
        string theme05 = "event:/Music/THEME_05";
        string theme06 = "event:/Music/THEME_06";
        string theme07 = "event:/Music/THEME_07";

        public static BGMusicManager Instance;

        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);
        }

        void Start()
        {
            eventEmitter = GetComponent<StudioEventEmitter>();
        }

        public void PlayBGMusic(int _trackNumber)
        {
            //StartCoroutine(FadeOutBGMusic(_trackNumber));
            //return;
            eventEmitter.AllowFadeout = true;
            eventEmitter.Stop();

            var tempRef = _trackNumber switch
            {
                1 => FMODUnity.RuntimeManager.PathToEventReference(theme01),
                4 => FMODUnity.RuntimeManager.PathToEventReference(theme04),
                5 => FMODUnity.RuntimeManager.PathToEventReference(theme05),
                6 => FMODUnity.RuntimeManager.PathToEventReference(theme06),
                7 => FMODUnity.RuntimeManager.PathToEventReference(theme07),
                _ => eventEmitter.EventReference
            };
            
            Destroy(eventEmitter);
            eventEmitter = gameObject.AddComponent<StudioEventEmitter>();
            eventEmitter.EventReference = tempRef;
            eventEmitter.Play();
        }

        IEnumerator FadeOutBGMusic(int _trackNumber)
        {
            eventEmitter.AllowFadeout = true;
            eventEmitter.Stop();
            yield return new WaitWhile(eventEmitter.IsPlaying);
            var tempRef = _trackNumber switch
            {
                1 => FMODUnity.RuntimeManager.PathToEventReference(theme01),
                4 => FMODUnity.RuntimeManager.PathToEventReference(theme04),
                5 => FMODUnity.RuntimeManager.PathToEventReference(theme05),
                6 => FMODUnity.RuntimeManager.PathToEventReference(theme06),
                7 => FMODUnity.RuntimeManager.PathToEventReference(theme07),
                _ => eventEmitter.EventReference
            };
            
            Destroy(eventEmitter);
            eventEmitter = gameObject.AddComponent<StudioEventEmitter>();
            eventEmitter.EventReference = tempRef;
            eventEmitter.Play();
        }

        public void StopBGMusic()
        {
            eventEmitter.Stop();
        }
    }
}