using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.DialogueSystem;
using Scripts.Scriptables.SceneLoader;
using Unity.VisualScripting;
using UnityEngine;

namespace Scripts.InteractionSystem
{
    public class MainInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] protected List<Interactable> prerequisites;
        [SerializeField] [TextArea] protected string voiceLine1;
        [SerializeField] [TextArea] protected string voiceLine2;
        [SerializeField] [TextArea] protected string voiceLine3;
        [SerializeField] [TextArea] protected string voiceLine4;
        [SerializeField] [TextArea] protected string voiceLine5;
        [SerializeField] [TextArea] protected string voiceLine6;
        [SerializeField] [TextArea] protected string voiceLineResolved;

        [SerializeField] SceneLoader mainHubLoader;
        
        int stepCount = 0;

        readonly List<string> voiceLines = new List<string>();

        protected virtual void Awake()
        {
            voiceLines.Add(voiceLine1);
            voiceLines.Add(voiceLine2);
            voiceLines.Add(voiceLine3);
            voiceLines.Add(voiceLine4);
            voiceLines.Add(voiceLine5);
            voiceLines.Add(voiceLine6);
            voiceLines.Add(voiceLineResolved);

            foreach (string line in voiceLines.Where(_line => _line == "").ToList())
            {
                voiceLines.Remove(line);
            }
        }

        void OnEnable()
        {
            foreach (var interactable in prerequisites)
            {
                interactable.OnInteracted += ContinueMainItem;
            }
        }

        void OnDisable()
        {
            foreach (var interactable in prerequisites)
            {
                interactable.OnInteracted -= ContinueMainItem;
            }
        }

        void ContinueMainItem()
        {
            stepCount++;
            Debug.Log($"Step {stepCount}");
            if (stepCount <= voiceLines.Count - 1) return;
            Debug.LogError("MainItem steps got too high!");
            stepCount--;
        }

        public void Interact()
        {
            PlayVoiceLine(stepCount);
            if (stepCount == voiceLines.Count - 1)
            {
                mainHubLoader.LoadScene();
            }
        }

        void PlayVoiceLine(int _voiceLineIndex)
        {
            Debug.Log(voiceLines[_voiceLineIndex]);
        }
    }
}