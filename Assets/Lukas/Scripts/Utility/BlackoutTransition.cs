using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Scripts.Utility
{
    public class BlackoutTransition : MonoBehaviour
    {
        Volume volume;
        [SerializeField] Color startColor;
        [SerializeField] Color endColor;
        [SerializeField] float duration;

        public bool TransitionDone { get; private set; }
        
        void Awake()
        {
            volume = GetComponent<Volume>();
        }

        void OnDisable()
        {
            StopAllCoroutines();
        }
        

        public IEnumerator TransitionToBlackout()
        {
            TransitionDone = false;
            float timer = 0f;
            volume.profile.TryGet(out ColorAdjustments colorAdjustments);
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = timer / duration;
                colorAdjustments.colorFilter.Interp(startColor, endColor, t);
                yield return null;
            }
            TransitionDone = true;
        }

        public IEnumerator TransitionFromBlackout()
        {
            TransitionDone = false;
            float timer = 0f;
            volume.profile.TryGet(out ColorAdjustments colorAdjustments);
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = timer / duration;
                colorAdjustments.colorFilter.Interp(endColor, startColor, t);
                yield return null;
            }
            TransitionDone = true;
        }
    }
}