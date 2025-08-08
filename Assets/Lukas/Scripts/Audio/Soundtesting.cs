using System;
using FMODUnity;
using UnityEngine;

namespace Scripts.Audio
{
    public class Soundtesting : MonoBehaviour
    {
        DialogueAudioScript dialogueAudioScript;
        
        void Awake()
        {
            dialogueAudioScript = FindObjectOfType<DialogueAudioScript>();
            
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
               dialogueAudioScript.PlayDialogue("00 Character Sounds/CSE_01");
            }
        }
    }
}