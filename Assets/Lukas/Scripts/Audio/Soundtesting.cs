using System;
using FMODUnity;
using UnityEngine;

namespace Scripts.Audio
{
    public class Soundtesting : MonoBehaviour
    {
        DialogueAudioScript dialogueAudioScript;

        [SerializeField] string keyPath;
        [SerializeField][Range(1,100)] int dialogueVolume;

        void Awake()
        {
            dialogueAudioScript = FindObjectOfType<DialogueAudioScript>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                var bus = FMODUnity.RuntimeManager.GetBus("bus:/DIALOG");
                bus.setVolume(dialogueVolume / 150f);
                dialogueAudioScript.PlayDialogue(keyPath);
            }
        }
    }
}