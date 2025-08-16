using System;
using System.ComponentModel.Design.Serialization;
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
            eventEmitter.AllowFadeout = true;
            eventEmitter.Stop();

            var tempRef = _trackNumber switch
            {
                1 => EventReference.Find(theme01),
                4 => EventReference.Find(theme04),
                5 => EventReference.Find(theme05),
                6 => EventReference.Find(theme06),
                7 => EventReference.Find(theme07),
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