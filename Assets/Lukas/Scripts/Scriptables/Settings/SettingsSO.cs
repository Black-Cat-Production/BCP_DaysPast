using UnityEngine;

namespace Scripts.Scriptables.Settings
{
    [CreateAssetMenu(fileName = "NewSettingsSO", menuName = "Scriptables/SettingsSO")]
    public class SettingsSO : ScriptableObject
    {
        public bool SubtitlesOn;
        public float MouseSensitivity;

        public float MasterVolume;
        public float MusicVolume;
        public float SfxVolume;
        public float DialogueVolume;
    }
}