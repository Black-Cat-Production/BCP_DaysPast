using System.Collections;
using System.Collections.Generic;
using Scripts.Audio;
using Scripts.Scriptables.SceneLoader;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Program
{
    public class IntroManager : MonoBehaviour
    {
        [SerializeField] List<Sprite> introSprites = new List<Sprite>();
        [SerializeField] Image introSlide;
        [SerializeField] SceneLoader hubAreaLoader;

        void Start()
        {
            StartCoroutine(IntroRoutine());
        }

        IEnumerator IntroRoutine()
        {
            Debug.Log(introSprites.Count);
            for (int i = 0; i < introSprites.Count; i++)
            {
                introSlide.sprite = (i + 1) switch
                {
                    < 8 => introSprites[i],
                    8 or 9 or 10 or 11 => introSprites[i - 1],
                    _ => introSlide.sprite
                };
                DialogueAudioScript.Instance.PlayDialogue("OPEN_" + (i + 1));
                Debug.Log("OPEN_" + (i+1));
                yield return new WaitUntil(DialogueAudioScript.Instance.WaitUntilDialogueDone);
            }

            introSlide.sprite = introSprites[^1];
            DialogueAudioScript.Instance.PlayDialogue("OPEN_12");
            yield return new WaitUntil(DialogueAudioScript.Instance.WaitUntilDialogueDone);
            yield return new WaitForSeconds(0.5f);
            hubAreaLoader.LoadScene();
        }
    }
}