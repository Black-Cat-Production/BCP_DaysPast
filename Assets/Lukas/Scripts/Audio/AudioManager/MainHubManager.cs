using System.Collections;
using FMOD.Studio;
using Scripts.UI.Subtitles;
using UnityEngine;

namespace Scripts.Audio.AudioManager
{
    public class MainHubManager : MonoBehaviour
    {
        
        void Start()
        {
            StartCoroutine(StartHubDialogue());
        }

        IEnumerator StartHubDialogue()
        {
            yield return new WaitForSeconds(1.5f);
            DialogueAudioScript.Instance.PlayDialogue("HE_1");
            SubtitleUI.Instance.DisplaySubtitle("What the.. What is this?", ESubtitleDisplayMode.Dynamic);
            yield return new WaitUntil(DialogueAudioScript.Instance.WaitUntilDialogueDone);
            DialogueAudioScript.Instance.PlayDialogue("HE_2");
            SubtitleUI.Instance.DisplaySubtitle("This almost looks like.. My childhood bedroom. It’s been ages- it's just a storage room nowadays. I miss this place- everything was easier then.", ESubtitleDisplayMode.Dynamic);
            yield return new WaitUntil(DialogueAudioScript.Instance.WaitUntilDialogueDone);
            DialogueAudioScript.Instance.PlayDialogue("HE_3");
            SubtitleUI.Instance.DisplaySubtitle("The worst thing I had to worry about were monsters under the bed. I guess there are worse places to dream your way back to..", ESubtitleDisplayMode.Dynamic);
            yield return new WaitUntil(DialogueAudioScript.Instance.WaitUntilDialogueDone);

        }

    }
}