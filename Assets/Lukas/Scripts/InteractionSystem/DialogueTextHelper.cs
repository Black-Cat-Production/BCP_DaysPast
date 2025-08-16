using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Scripts.InteractionSystem
{
    public class DialogueTextHelper : MonoBehaviour
    {
        public static DialogueTextHelper Instance;

        [SerializedDictionary("Key", "Subtitle")] [SerializeField]
        SerializedDictionary<string, string> textLinks = new();
        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);
        }

        public string GetSubtitle(string _key)
        {
            if(!textLinks.ContainsKey(_key)) return "Error! Key not found: " + _key;
            return textLinks[_key];
        }
    }
}