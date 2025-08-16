using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Audio;
using Scripts.Audio.AudioManager;
using Scripts.UI.Subtitles;
using UnityEngine;

namespace Scripts.Utility
{
    public class GameStartScript : MonoBehaviour
    {
        BlackoutTransition blackoutTransition;
        [SerializeField] List<string> voiceLines = new();
        [SerializeField] bool isLevel;

        void Awake()
        {
            blackoutTransition = GetComponent<BlackoutTransition>();
        }

        void Start()
        {
            StartCoroutine(blackoutTransition.TransitionFromBlackout());
            if (isLevel) StartCoroutine(StartEntranceDialogue());
        }

        IEnumerator StartEntranceDialogue()
        {
            yield return new WaitForSeconds(0.5f);
            for (int i = 1; i <= 5; i++)
            {
                DialogueAudioScript.Instance.PlayDialogue("LE_" + i);
                int mySessionID = DialogueAudioScript.Instance.CurrentSessionID;
                SubtitleUI.Instance.DisplaySubtitle(voiceLines[i - 1], ESubtitleDisplayMode.Dynamic);
                yield return new WaitUntil(() => DialogueAudioScript.Instance.WaitUntilDialogueDone(mySessionID));
                if (DialogueAudioScript.Instance.WasCancelled) yield break;
            }
            BGMusicManager.Instance.PlayBGMusic(7);
        }
    }
}