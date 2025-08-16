using System.Collections;
using Scripts.Audio;
using UnityEngine;

namespace Scripts.InteractionSystem
{
    public class TutorialTextHelper : MonoBehaviour
    {
        [SerializeField] CanvasGroup tutorialGroup;

        public void DisplayTutorial(bool _delayUntilVoiceLine = false)
        {
            StartCoroutine(TutorialDisplayRoutine());
        }

        public void DestroyTutorial()
        {
            StopAllCoroutines();
            tutorialGroup.gameObject.SetActive(false);
        }

        IEnumerator TutorialDisplayRoutine(bool _delayUntilVoiceLine = false)
        {
            int mySessionID = DialogueAudioScript.Instance.CurrentSessionID;
            if(_delayUntilVoiceLine)yield return new WaitUntil(() => DialogueAudioScript.Instance.WaitUntilDialogueDone(mySessionID));
            tutorialGroup.gameObject.SetActive(true);
            yield return new WaitForSeconds(10f);
            tutorialGroup.gameObject.SetActive(false);
        }
    }
}