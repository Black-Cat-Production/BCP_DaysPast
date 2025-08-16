using System;
using System.Collections.Generic;
using Scripts.DialogueSystem;
using Scripts.Movement;
using Scripts.Scriptables.Settings;
using UnityEngine;

namespace Scripts.InteractionSystem
{
    public abstract class Interactable : MonoBehaviour, IInteractable
    {
        [SerializeField] protected List<Interactable> prerequisites;
        [SerializeField][TextArea] protected string voiceLine1;
        [SerializeField][TextArea] protected string voiceLine2;
        [SerializeField][TextArea] protected string voiceLine3;
        [SerializeField][TextArea] protected string voiceLine4;
        [SerializeField][TextArea] protected string voiceLineResolved;
        [SerializeField][TextArea] protected string voiceLineWrongCamera;
        [SerializeField] Canvas interactionIconCanvas;
        [SerializeField] protected SettingsSO settings;
        [SerializeField] protected ECameraState targetCameraState;

        protected bool interacted;
        public bool Interacted => interacted;
        
        protected bool resolved;
        public bool Resolved => resolved;
        
        protected readonly Dictionary<(bool, bool), Action> interactionMap = new Dictionary<(bool interacted, bool hasPrerequisites), Action>();
        
        protected readonly List<string> voiceLines = new List<string>();

        public Action OnInteracted;

        protected PlayerController playerController;

        protected virtual void Awake()
        {
            playerController = FindObjectOfType<PlayerController>();
            voiceLines.Add(voiceLine1);
            voiceLines.Add(voiceLine2);
            voiceLines.Add(voiceLine3);
            voiceLines.Add(voiceLine4);
            voiceLines.Add(voiceLineResolved);
            voiceLines.Add(voiceLineWrongCamera);
        }

        public abstract void Interact();
        public abstract bool CheckPrerequisites();
        public abstract void PlayVoiceLine(EVoiceLineType _voiceLineType);

        public abstract void ContinueMainItem();

        public void ShowInteractIcon()
        {
            if (interactionIconCanvas == null) return;
            interactionIconCanvas.gameObject.SetActive(true);
        }

        public void HideInteractIcon()
        {
            if (interactionIconCanvas == null) return;
            interactionIconCanvas.gameObject.SetActive(false);
        }
    }
}