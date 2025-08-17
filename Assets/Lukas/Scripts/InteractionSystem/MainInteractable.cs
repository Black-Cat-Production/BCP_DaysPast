using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Audio;
using Scripts.DialogueSystem;
using Scripts.Scriptables.SceneLoader;
using Scripts.Scriptables.Settings;
using Scripts.UI.Subtitles;
using Scripts.Utility;
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
        [SerializeField] EndGameDialogue endGameDialogue;
        [SerializeField] SceneLoader mainHubLoader;
        [SerializeField] SettingsSO settings;
        
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
        }

        IEnumerator LoadMainMenu()
        {
            var blackoutTransition = FindObjectOfType<BlackoutTransition>();
            yield return blackoutTransition.TransitionToBlackout();
            mainHubLoader.LoadScene();
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
                    StartCoroutine(PlayStartDialogueChain());
                    break;
                case 1:
                    DialogueAudioScript.Instance.PlayDialogue("CFS_1");
                    SubtitleUI.Instance.DisplaySubtitle(fullBlackoutVoiceLine, ESubtitleDisplayMode.Dynamic);
                    break;
                case >= 2 and < 8:
                    DialogueAudioScript.Instance.PlayDialogue("CFS_2");
                    SubtitleUI.Instance.DisplaySubtitle(voiceLines[3], ESubtitleDisplayMode.Fixed);
                    break;
                case 8:
                    StartCoroutine(EndGame());
                    break;
            }
        }

        IEnumerator EndGame()
        {
            yield return endGameDialogue.StartEndGameDialogue();
            StartCoroutine(LoadMainMenu());
        }

        IEnumerator PlayStartDialogueChain()
        {
            DialogueAudioScript.Instance.PlayDialogue("LICF_1");
            SubtitleUI.Instance.DisplaySubtitle(firstInteraction_01, ESubtitleDisplayMode.Dynamic);
            int mySessionID = DialogueAudioScript.Instance.CurrentSessionID;
            yield return new WaitUntil(() => DialogueAudioScript.Instance.WaitUntilDialogueDone(mySessionID));
            if (DialogueAudioScript.Instance.WasCancelled) yield break;
            DialogueAudioScript.Instance.PlayDialogue("LICF_2");
            SubtitleUI.Instance.DisplaySubtitle(firstInteraction_02, ESubtitleDisplayMode.Fixed);
            yield return new WaitUntil(() => DialogueAudioScript.Instance.WaitUntilDialogueDone(mySessionID));
        }

        void UpdateShader()
        {
            float calculatedStep = ((float)stepCount)  / fullStepCount;
            calculatedStep = SmoothCalculatedStep(calculatedStep);
            Debug.Log(calculatedStep);
            shaderMaterial.SetFloat(revealAmount, calculatedStep);
        }
        
        float SmoothCalculatedStep(float _calculatedStep)
        {
            if (_calculatedStep <= 0f) return 0f;
            if (_calculatedStep >= 1f) return 1f;

            float t = Mathf.Clamp01(_calculatedStep);

            t = Mathf.SmoothStep(0f, 1f, t); 

            return Mathf.Lerp(0.35f, 0.6f, t);
        }
    }
}