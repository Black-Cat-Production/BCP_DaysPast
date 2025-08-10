using System;
using System.Collections;
using Scripts.Audio.AudioManager;
using Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.MinigameSystem
{
    public abstract class Minigame : MonoBehaviour
    {
        protected bool gameIsDone;

        protected BlackoutTransition blackoutTransition;
        protected MinigameAudioHelper audioHelper;

        [SerializeField] Image fakeVolumeImage;

        protected virtual void Awake()
        {
            blackoutTransition = FindObjectOfType<BlackoutTransition>();
            audioHelper = FindObjectOfType<MinigameAudioHelper>();
        }

        public abstract void Play();
        protected abstract void EndGame();
        protected abstract void OpenUI();
        protected abstract void CloseUI();
        
        protected IEnumerator StartGameRoutine()
        {
            if (blackoutTransition == null)
            {
                OpenUI();
                yield break;
            }
            yield return StartCoroutine(blackoutTransition.TransitionToBlackout(fakeVolumeImage));
            OpenUI();
            audioHelper.PlayStartAudio();
            yield return StartCoroutine(blackoutTransition.TransitionFromBlackout(fakeVolumeImage));
        }

        protected IEnumerator EndGameRoutine()
        {
            if (blackoutTransition == null)
            {
                CloseUI();
                yield break;
            }
            audioHelper.PlayEndAudio();
            yield return new WaitWhile(audioHelper.EndAudioIsPlaying);
            yield return StartCoroutine(blackoutTransition.TransitionToBlackout(fakeVolumeImage));
            CloseUI();
            yield return StartCoroutine(blackoutTransition.TransitionFromBlackout(fakeVolumeImage));
        }
    }
}