using System;
using FMOD.Studio;
using Scripts.Scriptables.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI.MainMenu
{
    public class OptionsMenu : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] SettingsSO settings;
        
        [Header("Subtitles")]
        [SerializeField] Sprite toggleButtonOff;
        [SerializeField] Sprite toggleButtonOn;
        [SerializeField] Button subtitlesButton;
        [SerializeField] TextMeshProUGUI subtitlesText;
        
        [Header("MouseSense")]
        [SerializeField] Slider mouseSense;
        
        [Header("VolumeOptions")]
        [SerializeField] Slider volumeMasterSlider;
        [SerializeField] Slider volumeMusicSlider;
        [SerializeField] Slider volumeSFXSlider;
        [SerializeField] Slider volumeDialogueSlider;


        bool isOpening;
        void OnEnable()
        {
            isOpening = true;
            mouseSense.value = settings.MouseSensitivity * 100f;
            volumeMasterSlider.value = settings.MasterVolume;
            volumeMusicSlider.value = settings.MusicVolume;
            volumeSFXSlider.value = settings.SfxVolume;
            volumeDialogueSlider.value = settings.DialogueVolume;
            isOpening = false;
        }

        public void ToggleSubtitles()
        {
            subtitlesButton.image.sprite = settings.SubtitlesOn ? toggleButtonOff : toggleButtonOn;
            settings.SubtitlesOn = !settings.SubtitlesOn;
            subtitlesText.gameObject.SetActive(settings.SubtitlesOn);
        }

        public void UpdateMouseSense()
        {
            if (isOpening) return;
            float calculatedMouseSensitivity = mouseSense.value / 100f;
            settings.MouseSensitivity = calculatedMouseSensitivity;
        }

        public void UpdateVolume()
        {
            if (isOpening) return;
            settings.MasterVolume = volumeMasterSlider.value;
            settings.MusicVolume = volumeMusicSlider.value;
            settings.SfxVolume = volumeSFXSlider.value;
            settings.DialogueVolume = volumeDialogueSlider.value;
            UpdateFMODVolume();
        }

        void UpdateFMODVolume()
        {
            var musicBus = FMODUnity.RuntimeManager.GetBus("bus:/MUSIC");
            musicBus.setVolume(settings.MusicVolume / 200f);
            var sfxBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
            sfxBus.setVolume( settings.SfxVolume / 110f);
            var dialogueBus = FMODUnity.RuntimeManager.GetBus("bus:/DIALOG");
            dialogueBus.setVolume(settings.DialogueVolume / 150f);
            var masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
            masterBus.setVolume(settings.MasterVolume / 100f);
            
            dialogueBus.getVolume(out var dialogueVolume);
            sfxBus.getVolume(out var sfxVolume);
            musicBus.getVolume(out var musicVolume);
            Debug.Log("DialogVolume: " + dialogueVolume);
            Debug.Log("SFXVolume: " + sfxVolume);
            Debug.Log("MusicVolume: " + musicVolume);
            
            //FMODUnity.RuntimeManager.StudioSystem.getBankList(out var bankList);
            //foreach (var bank in bankList)
            //{
            //    bank.getBusCount(out int busCount);
            //    bank.getPath(out string path);
            //    if(busCount == 0) continue;
            //    Debug.Log("Bank: " + path + " " + busCount);
            //    bank.getBusList(out var busList);
            //    foreach (var bus in busList)
            //    {
            //        bus.getPath(out string busPath);
            //        //Debug.Log("Bus: " + busPath);
            //        FMODUnity.RuntimeManager.StudioSystem.getBus(busPath, out Bus _bus);
            //        Debug.Log("Got BUS: " + _bus.isValid());
            //        if (_bus.isValid())
            //        {
            //            _bus.getVolume(out float volume);
            //            Debug.Log("Volume: " + volume + "%");
            //        }
            //    }
            //}
        }
    }
}