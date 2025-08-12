using System.Collections.Generic;
using System.Linq;
using Scripts.Scriptables.SceneLoader;
using UnityEngine;

namespace Scripts.InteractionSystem
{
    public class MainInteractable : MonoBehaviour, IInteractable
    {
        static readonly int revealAmount = Shader.PropertyToID("_RevealAmount");
        [SerializeField] protected List<Interactable> prerequisites;
        [SerializeField] [TextArea] protected string firstInteraction_01;
        [SerializeField] [TextArea] protected string firstInteraction_02;
        [SerializeField] [TextArea] protected string fullBlackoutVoiceLine;
        [SerializeField] [TextArea] protected string betweenStepsVoiceLine;
        [SerializeField] int fullStepCount = 8;
        [SerializeField] Canvas interactionIconCanvas;

        [SerializeField] SceneLoader mainHubLoader;
        
        int stepCount = 0;

        readonly List<string> voiceLines = new List<string>();

        Material shaderMaterial;

        protected virtual void Awake()
        {
            shaderMaterial = GetComponentInChildren<MeshRenderer>().material;
            voiceLines.Add(firstInteraction_01);
            voiceLines.Add(firstInteraction_02);
            voiceLines.Add(fullBlackoutVoiceLine);
            voiceLines.Add(betweenStepsVoiceLine);

            foreach (string line in voiceLines.Where(_line => _line == "").ToList())
            {
                voiceLines.Remove(line);
            }
            UpdateShader();
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
            if (stepCount == 0) stepCount = 2;
            else stepCount++;
            Debug.Log($"Step {stepCount}");
            UpdateShader();
            if (stepCount <= fullStepCount) return;
            Debug.LogError("MainItem steps got too high!");
            stepCount--;
        }

        public void Interact()
        {
            PlayVoiceLine(stepCount);
            if(stepCount == 0) stepCount = 1;
            if (stepCount == fullStepCount)
            {
                mainHubLoader.LoadScene();
            }
        }

        public void ShowInteractIcon()
        {
            interactionIconCanvas.gameObject.SetActive(true);
        }

        public void HideInteractIcon()
        {
            interactionIconCanvas.gameObject.SetActive(false);
        }

        void PlayVoiceLine(int _voiceLineIndex)
        {
            switch (_voiceLineIndex)
            {
                case 0:
                    Debug.Log(firstInteraction_01 + "___" + firstInteraction_02);
                    break;
                case >= 1 and < 7:
                    Debug.Log(voiceLines[2]);
                    break;
                case 7:
                    Debug.Log("RESOLVED");
                    break;
            }
        }

        void UpdateShader()
        {
            float calculatedStep = ((float)stepCount - 1)  / fullStepCount;
            calculatedStep = SmoothCalculatedStep(calculatedStep);
            Debug.Log(calculatedStep);
            shaderMaterial.SetFloat(revealAmount, calculatedStep);
        }
        
        float SmoothCalculatedStep(float _calculatedStep)
        {
            if (_calculatedStep <= 0f) return 0f;
            if (_calculatedStep >= 1f) return 1f;

            // Normalize middle range (0 → 0, 1 → 1)
            float t = Mathf.Clamp01(_calculatedStep);

            // Optional smoothing curve
            t = Mathf.SmoothStep(0f, 1f, t); 

            // Remap to 0.35 - 0.6
            return Mathf.Lerp(0.35f, 0.6f, t);
        }
    }
}