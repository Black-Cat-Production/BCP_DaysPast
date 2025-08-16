using System.Collections;
using System.Collections.Generic;
using Scripts.Audio;
using Scripts.Audio.AudioManager;
using Scripts.UI.Subtitles;
using UnityEngine;

namespace Scripts.DialogueSystem
{
    public class EndGameDialogue : MonoBehaviour
    {
        [SerializeField] List<string> voiceLines = new();


        public IEnumerator StartEndGameDialogue()
        {
            DialogueAudioScript.Instance.PlayDialogue("CFF_1");
            SubtitleUI.Instance.DisplaySubtitle("Now it all makes sense to me..These little figures were at the center of it all along.",ESubtitleDisplayMode.Dynamic);
            int mySessionID = DialogueAudioScript.Instance.CurrentSessionID;
            yield return new WaitUntil(() => DialogueAudioScript.Instance.WaitUntilDialogueDone(mySessionID));
            BGMusicManager.Instance.PlayBGMusic(1);
            if (DialogueAudioScript.Instance.WasCancelled) yield break;
            for (int i = 0; i < voiceLines.Count; i++)
            {
                DialogueAudioScript.Instance.PlayDialogue("CFF_" + (i+1));
                SubtitleUI.Instance.DisplaySubtitle(voiceLines[i], ESubtitleDisplayMode.Dynamic);
                mySessionID = DialogueAudioScript.Instance.CurrentSessionID;
                yield return new WaitUntil(() => DialogueAudioScript.Instance.WaitUntilDialogueDone(mySessionID));
                if (DialogueAudioScript.Instance.WasCancelled) yield break;
            }

            yield return new WaitForSeconds(1f);
            
        }
        
    }
}