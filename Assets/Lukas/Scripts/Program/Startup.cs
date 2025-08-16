using System;
using System.Xml;
using Scripts.Scriptables.Settings;
using UnityEngine;

namespace Scripts.Program
{
    public class Startup : MonoBehaviour
    {
        [SerializeField] SettingsSO settings;
        void Start()
        {
            Screen.SetResolution(1920, 1080, true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
            var musicBus = FMODUnity.RuntimeManager.GetBus("bus:/MUSIC");
            musicBus.setVolume(settings.MusicVolume / 200f);
            var sfxBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
            sfxBus.setVolume( settings.SfxVolume / 110f);
            var dialogueBus = FMODUnity.RuntimeManager.GetBus("bus:/DIALOG");
            dialogueBus.setVolume(settings.DialogueVolume / 150f);
            var masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
            masterBus.setVolume(settings.MasterVolume / 100f);

        }
    }
}