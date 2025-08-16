using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Scripts.Audio;
using Scripts.Audio.AudioManager;
using Scripts.DialogueSystem;
using Scripts.MinigameSystem;
using Scripts.UI.Subtitles;
using TMPro;
using UnityEngine;

namespace Scripts.InteractionSystem
{
    public class StoryInteractable : Interactable
    {
        bool hasContinuedMain = false;
        [SerializeField] TutorialTextHelper tutorialTextHelper;

        [SerializeField] int startIndexDialogueChain;
        [SerializeField] int endIndexDialogueChain;

        [SerializedDictionary("VoiceLineIndex", "Key")] [SerializeField]
        SerializedDictionary<int, string> chainLinks = new();

        [SerializedDictionary("VoiceLineType", "Key")] [SerializeField]
        SerializedDictionary<EVoiceLineType, string> normalLinks = new();

        protected override void Awake()
        {
            base.Awake();
            interactionMap.Add((false, false), () => PlayVoiceLine(EVoiceLineType.Unknown));
            interactionMap.Add((false, true), () => PlayVoiceLine(EVoiceLineType.Known));
            interactionMap.Add((true, false), () => PlayVoiceLine(EVoiceLineType.Visited));
            interactionMap.Add((true, true), () =>
            {
                if (resolved) return;
                PlayVoiceLine(EVoiceLineType.Understood);
                resolved = true;
            });
        }

        public override void Interact()
        {
            bool hasPrerequisites = CheckPrerequisites();
            if (playerController.CurrentCameraState != targetCameraState && targetCameraState != ECameraState.Either && !resolved && hasPrerequisites)
            {
                if (SubtitleUI.Instance == null) return;
                PlayVoiceLine(EVoiceLineType.CameraError);
                if (tutorialTextHelper != null) tutorialTextHelper.DisplayTutorial(true);
                return;
            }

            if (resolved)
            {
                PlayVoiceLine(EVoiceLineType.Done);
            }
            else if (interactionMap.TryGetValue((interacted, hasPrerequisites), out var action))
            {
                action.Invoke();
            }

            interacted = true;

            if (!hasPrerequisites) return;
            resolved = true;
            if (TryGetComponent(out Minigame minigame)) minigame.Play();
            if (hasContinuedMain) return;
            Debug.Log("Starting Mini-game or Story goes forward etc.");
            ContinueMainItem();
            hasContinuedMain = true;
        }

        public override bool CheckPrerequisites()
        {
            return prerequisites.All(_interactable => _interactable.Resolved) || prerequisites.Count == 0;
        }

        public override void PlayVoiceLine(EVoiceLineType _voiceLineType)
        {
            Debug.Log(voiceLines[(int)_voiceLineType]);
            if (voiceLines[(int)_voiceLineType].Contains("LONG DYNAMIC", StringComparison.Ordinal))
            {
                Debug.Log("Dynamic dialogue detected!");
                StartCoroutine(PlayDialogueChain());
                return;
            }

            if (!normalLinks.ContainsKey(_voiceLineType)) return;
            string key = normalLinks[_voiceLineType];
            DialogueAudioScript.Instance.PlayDialogue(key);
            if (SubtitleUI.Instance == null) return;
            SubtitleUI.Instance.DisplaySubtitle(voiceLines[(int)_voiceLineType], ESubtitleDisplayMode.Dynamic);
        }

        IEnumerator PlayDialogueChain()
        {
            if(this.gameObject.name == "SM_Sketchbook") BGMusicManager.Instance.PlayBGMusic(5);
            for (int i = startIndexDialogueChain; i <= endIndexDialogueChain; i++)
            {
                string key = chainLinks[i];
                DialogueAudioScript.Instance.PlayDialogue(key);
                int mySessionID = DialogueAudioScript.Instance.CurrentSessionID;
                SubtitleUI.Instance.DisplaySubtitle(DialogueTextHelper.Instance.GetSubtitle(key), ESubtitleDisplayMode.Dynamic);
                yield return new WaitUntil(() => DialogueAudioScript.Instance.WaitUntilDialogueDone(mySessionID));
                if (DialogueAudioScript.Instance.WasCancelled) yield break;
            }

            yield return null;
        }

        public override void ContinueMainItem()
        {
            OnInteracted?.Invoke();
        }
    }
}