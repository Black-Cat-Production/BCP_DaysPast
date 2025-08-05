using System.Linq;
using Scripts.DialogueSystem;
using Scripts.MinigameSystem;
using Scripts.UI.Subtitles;
using UnityEngine;

namespace Scripts.InteractionSystem
{
    public class StoryInteractable : Interactable
    {
        bool hasContinuedMain = false;
        protected override void Awake()
        {
            base.Awake();
            interactionMap.Add((false, false), () => PlayVoiceLine(EVoiceLineType.Unknown));
            interactionMap.Add((false, true), () => PlayVoiceLine(EVoiceLineType.Known));
            interactionMap.Add((true, false), () => PlayVoiceLine(EVoiceLineType.Visited));
            interactionMap.Add((true, true), () =>
            {
                PlayVoiceLine(EVoiceLineType.Understood);
                resolved = true;
            });
        }

        public override void Interact()
        {
            bool hasPrerequisites = CheckPrerequisites();

            if (interactionMap.TryGetValue((interacted, hasPrerequisites), out var action))
            {
                action.Invoke();
            }
            else if (resolved)
            {
                PlayVoiceLine(EVoiceLineType.Done);
            }
            interacted = true;

            if (!hasPrerequisites) return;
            if(TryGetComponent(out Minigame minigame)) minigame.Play();
            if (hasContinuedMain) return;
            Debug.Log("Starting Mini-game or Story goes forward etc.");
            ContinueMainItem();
            hasContinuedMain = true;
        }

        public override bool CheckPrerequisites()
        {
            return prerequisites.Any(_interactable => _interactable.Interacted) || prerequisites.Count == 0;
        }

        public override void PlayVoiceLine(EVoiceLineType _voiceLineType)
        {
            Debug.Log(voiceLines[(int)_voiceLineType]);
            if (SubtitleUI.Instance == null) return;
            SubtitleUI.Instance.DisplaySubtitle(voiceLines[(int)_voiceLineType]);
            SubtitleUI.Instance.StartSubtitleTimer();
        }

        public override void ContinueMainItem()
        {
            OnInteracted?.Invoke();
        }
    }
}