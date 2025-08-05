using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Scripts.UI.Subtitles
{
    public class SubtitleUI : MonoBehaviour
    {
        public static SubtitleUI Instance;

        [SerializeField] TextMeshProUGUI text;
        [SerializeField] CanvasGroup uiGroup;
        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void DisplaySubtitle(string _subtitle)
        {
            uiGroup.gameObject.SetActive(true);
            text.text = _subtitle;
        }

        void HideSubtitle()
        {
            uiGroup.gameObject.SetActive(false);
        }

        public void StartSubtitleTimer()
        {
            StartCoroutine(ResetSubtitleDisplay());
        }

        IEnumerator ResetSubtitleDisplay()
        {
            float timer = 0f;
            while (timer < 2f)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            HideSubtitle();
        }
    }
}