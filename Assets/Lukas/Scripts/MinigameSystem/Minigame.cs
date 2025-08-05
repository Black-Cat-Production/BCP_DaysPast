using System;
using System.Collections;
using Scripts.Utility;
using UnityEngine;

namespace Scripts.MinigameSystem
{
    public abstract class Minigame : MonoBehaviour
    {
        protected bool gameIsDone;

        protected BlackoutTransition blackoutTransition;

        protected void Awake()
        {
            blackoutTransition = FindObjectOfType<BlackoutTransition>();
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
            yield return StartCoroutine(blackoutTransition.TransitionToBlackout());
            OpenUI();
            yield return StartCoroutine(blackoutTransition.TransitionFromBlackout());
        }

        protected IEnumerator EndGameRoutine()
        {
            if (blackoutTransition == null)
            {
                CloseUI();
                yield break;
            }
            yield return StartCoroutine(blackoutTransition.TransitionToBlackout());
            CloseUI();
            yield return StartCoroutine(blackoutTransition.TransitionFromBlackout());
        }
    }
}