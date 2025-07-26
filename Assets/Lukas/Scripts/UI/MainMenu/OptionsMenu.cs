using System;
using Scripts.Scriptables.Settings;
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
        }
    }
}