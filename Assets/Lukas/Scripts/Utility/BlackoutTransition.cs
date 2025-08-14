using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Scripts.Utility
{
    public class BlackoutTransition : MonoBehaviour
    {
        Volume volume;
        [SerializeField] Color startColor;
        [SerializeField] Color endColor;
        [SerializeField] Color startSpriteColor;
        [SerializeField] Color endSpriteColor;
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
        

        public IEnumerator TransitionToBlackout(Image _screenSpaceFakeImage = null)
        {
            TransitionDone = false;
            float timer = 0f;
            volume.profile.TryGet(out ColorAdjustments colorAdjustments);
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = timer / duration;
                colorAdjustments.colorFilter.Interp(startColor, endColor, t);
                if (_screenSpaceFakeImage != null)
                {
                    _screenSpaceFakeImage.color = Color.Lerp(startSpriteColor, endSpriteColor, t);
                }
                yield return null;
            }
            TransitionDone = true;
        }

        public IEnumerator TransitionFromBlackout(Image _screenSpaceFakeImage = null)
        {
            TransitionDone = false;
            float timer = 0f;
            volume.profile.TryGet(out ColorAdjustments colorAdjustments);
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = timer / duration;
                colorAdjustments.colorFilter.Interp(endColor, startColor, t);
                if (_screenSpaceFakeImage != null)
                {
                    _screenSpaceFakeImage.color = Color.Lerp(endSpriteColor, startSpriteColor, t);
                }
                yield return null;
            }
            TransitionDone = true;
        }
    }
}