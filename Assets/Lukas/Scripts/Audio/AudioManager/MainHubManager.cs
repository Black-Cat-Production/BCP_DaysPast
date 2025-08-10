using System;
using System.Collections;
using Scripts.UI.Subtitles;
using UnityEngine;

namespace Scripts.Audio.AudioManager
{
    public class MainHubManager : MonoBehaviour
    {
        void Start()
        {
            DialogueAudioScript.Instance.PlayDialogue("HE_01");
        }

    }
}