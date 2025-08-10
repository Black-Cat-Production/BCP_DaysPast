using System;
using System.Collections;
using FMOD.Studio;
using Scripts.Audio;
using Scripts.Scriptables.SceneLoader;
using Scripts.UI.Subtitles;
using Scripts.Utility;
using UnityEngine;

namespace Scripts.InteractionSystem
{
    public class HubInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] SceneLoader levelLoader;

        bool interacted = false;
        BlackoutTransition blackoutTransition;

        void Awake()
        {
            blackoutTransition = FindObjectOfType<BlackoutTransition>();
        }

        public void Interact()
        {
            if (interacted) return;
            interacted = true;
            DialogueAudioScript.Instance.PlayDialogue("HICF_01");
            StartCoroutine(LoadLevel());
        }

        IEnumerator LoadLevel()
        {
            SubtitleUI.Instance.DisplaySubtitle("Strange... I can hardly make out what this is supposed to be, but it feels.. familiar?");
            SubtitleUI.Instance.StartSubtitleTimer(ESubtitleDisplayMode.Dynamic);
            yield return new WaitUntil(() =>
            {
                DialogueAudioScript.Instance.DialogueInstance.getPlaybackState(out var state);
                Debug.Log(state.ToString());
                return state == PLAYBACK_STATE.STOPPED;
            });
            DialogueAudioScript.Instance.PlayDialogue("HICF_03");
            SubtitleUI.Instance.DisplaySubtitle("Let's see what this is about!");
            SubtitleUI.Instance.StartSubtitleTimer(ESubtitleDisplayMode.Dynamic);
            yield return new WaitUntil(() =>
            {
                DialogueAudioScript.Instance.DialogueInstance.getPlaybackState(out var state);
                Debug.Log(state.ToString());
                return state == PLAYBACK_STATE.STOPPED;
            });
            yield return StartCoroutine(blackoutTransition.TransitionToBlackout());
            levelLoader.LoadScene();
        }

        public void ShowInteractIcon()
        {
            Debug.Log("No InteractIcon Logic! This might be okay!");
        }

        public void HideInteractIcon()
        {
            Debug.Log("No InteractIcon Logic! This might be okay!");
        }
    }
}