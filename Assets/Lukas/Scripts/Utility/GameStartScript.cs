using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Audio;
using Scripts.Audio.AudioManager;
using Scripts.InteractionSystem;
using Scripts.Scriptables.Settings;
using Scripts.UI.Subtitles;
using UnityEngine;

namespace Scripts.Utility
{
    public class GameStartScript : MonoBehaviour
    {
        BlackoutTransition blackoutTransition;
        [SerializeField] List<string> voiceLines = new();
        [SerializeField] TutorialTextHelper tutorialTextHelper;
        [SerializeField] bool isLevel;
        [SerializeField] bool isDev;
        [SerializeField] SettingsSO devSettings;

        void Awake()
        {
            blackoutTransition = GetComponent<BlackoutTransition>();
        }

        void Start()
        {
            StartCoroutine(AwakeArea());
            if (isLevel) StartCoroutine(StartEntranceDialogue());
            if (!isDev) return;
            var musicBus = FMODUnity.RuntimeManager.GetBus("bus:/MUSIC");
            musicBus.setVolume(devSettings.MusicVolume / 150);
            var sfxBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
            sfxBus.setVolume( devSettings.SfxVolume / 110f);
            var dialogueBus = FMODUnity.RuntimeManager.GetBus("bus:/DIALOG");
            dialogueBus.setVolume(devSettings.DialogueVolume / 180);
            var masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
            masterBus.setVolume(devSettings.MasterVolume / 100f);
        }

        IEnumerator AwakeArea()
        {
            yield return StartCoroutine(blackoutTransition.TransitionFromBlackout());
            if (isLevel) yield break;
            tutorialTextHelper.DisplayTutorial();
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
                if (DialogueAudioScript.Instance.WasCancelled) break;
            }
            BGMusicManager.Instance.PlayBGMusic(7);
        }
    }
}