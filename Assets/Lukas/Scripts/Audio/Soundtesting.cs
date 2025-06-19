using System;
using FMODUnity;
using UnityEngine;

namespace Scripts.Audio
{
    public class Soundtesting : MonoBehaviour
    {
        StudioEventEmitter emitter;

        void Awake()
        {
            emitter = GetComponent<StudioEventEmitter>();
        }

        void FixedUpdate()
        {
            if (!emitter.IsPlaying()) emitter.Play();
        }
    }
}