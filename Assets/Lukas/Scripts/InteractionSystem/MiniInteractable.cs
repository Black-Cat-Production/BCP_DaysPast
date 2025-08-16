using Scripts.Audio;
using Scripts.DialogueSystem;
using Scripts.UI.Subtitles;
using UnityEngine;

namespace Scripts.InteractionSystem
{
    public class MiniInteractable : Interactable
    {
        [SerializeField] string keyPrefix;
        [SerializeField] int startIndex;
        [SerializeField] int maxIndex;
        int voiceLineCounter = 1;

        protected override void Awake()
        {
            base.Awake();

            voiceLineCounter = startIndex;
        }

        public override void Interact()
        {
            PlayVoiceLine(voiceLineCounter);
            voiceLineCounter++;
            if (voiceLineCounter > maxIndex) voiceLineCounter--;
        }

        public override bool CheckPrerequisites()
        {
            throw new System.NotImplementedException();
        }

        public override void PlayVoiceLine(EVoiceLineType _voiceLineType)
        {
            throw new System.NotImplementedException();
        }

        void PlayVoiceLine(int _voiceLineIndex)
        {
            string keyString = keyPrefix + "_" + _voiceLineIndex;
            Debug.Log(keyString);
            DialogueAudioScript.Instance.PlayDialogue(keyString);
            if (SubtitleUI.Instance == null) return;
            SubtitleUI.Instance.DisplaySubtitle(voiceLines[_voiceLineIndex - 1], ESubtitleDisplayMode.Dynamic);
        }

        public override void ContinueMainItem()
        {
            throw new System.NotImplementedException();
        }
    }
}