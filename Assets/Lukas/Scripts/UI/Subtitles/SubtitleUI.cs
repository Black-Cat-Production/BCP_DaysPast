using System;
using System.Collections;
using FMOD;
using FMOD.Studio;
using Scripts.Audio;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Scripts.UI.Subtitles
{
    public class SubtitleUI : MonoBehaviour
    {
        public static SubtitleUI Instance;

        [SerializeField] TextMeshProUGUI text;
        [SerializeField] CanvasGroup uiGroup;

        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void DisplaySubtitle(string _subtitle)
        {
            if (uiGroup.gameObject.activeSelf)
            {
                StopAllCoroutines();
                uiGroup.gameObject.SetActive(false);
            }

            uiGroup.gameObject.SetActive(true);
            text.text = _subtitle;
        }


        void HideSubtitle()
        {
            uiGroup.gameObject.SetActive(false);
        }

        public void StartSubtitleTimer(ESubtitleDisplayMode _subtitleDisplayMode, float _fixedDurationValue = 2f)
        {
            StartCoroutine(ResetSubtitleDisplay(_subtitleDisplayMode, _fixedDurationValue));
        }

        IEnumerator ResetSubtitleDisplay(ESubtitleDisplayMode _subtitleDisplayMode, float _duration)
        {
            switch (_subtitleDisplayMode)
            {
                case ESubtitleDisplayMode.Fixed:
                    float timer = 0f;
                    while (timer < _duration)
                    {
                        timer += Time.deltaTime;
                        yield return null;
                    }

                    break;
                case ESubtitleDisplayMode.Dynamic:
                    yield return new WaitUntil(() =>
                    {
                        DialogueAudioScript.Instance.DialogueInstance.getPlaybackState(out var state);
                        Debug.Log(state.ToString());
                        return state == PLAYBACK_STATE.STOPPED;
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_subtitleDisplayMode), _subtitleDisplayMode, null);
            }

            HideSubtitle();
        }
    }
}