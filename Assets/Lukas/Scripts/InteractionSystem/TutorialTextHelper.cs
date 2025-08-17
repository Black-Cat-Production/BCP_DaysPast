using System.Collections;
using Scripts.Audio;
using UnityEngine;

namespace Scripts.InteractionSystem
{
    public class TutorialTextHelper : MonoBehaviour
    {
        [SerializeField] CanvasGroup tutorialGroup;

        public void DisplayTutorial(int _forcedDelay = 0, bool _delayUntilVoiceLine = false)
        {
            StartCoroutine(TutorialDisplayRoutine(_forcedDelay, _delayUntilVoiceLine));
        }

        public void DestroyTutorial()
        {
            StopAllCoroutines();
            tutorialGroup.gameObject.SetActive(false);
        }

        IEnumerator TutorialDisplayRoutine(int _forcedDelay = 0,bool _delayUntilVoiceLine = false)
        {
            int mySessionID = DialogueAudioScript.Instance.CurrentSessionID;
            if(_delayUntilVoiceLine)yield return new WaitUntil(() => DialogueAudioScript.Instance.WaitUntilDialogueDone(mySessionID));
            if(_forcedDelay != 0) yield return new WaitForSeconds(_forcedDelay);
            tutorialGroup.gameObject.SetActive(true);
            yield return new WaitForSeconds(8f);
            tutorialGroup.gameObject.SetActive(false);
        }
    }
}