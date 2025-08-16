using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Audio;
using Scripts.Audio.AudioManager;
using Scripts.InteractionSystem;
using Scripts.MinigameSystem.Flowerbook;
using Scripts.UI.Subtitles;
using Scripts.Utility;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Scripts.MinigameSystem.Swipe
{
    public class SwipeGame : Minigame
    {
        [SerializeField] CanvasGroup swipeGameUI;
        [SerializeField] List<SwipeableLeaf> leafs = new();
        [SerializeField] PlayerInput playerInput;
        [SerializeField] InteractionIconDetector iconDetector;

        const float FlyOffDistance = 300f;

        void OnEnable()
        {
            foreach (var leaf in leafs)
            {
                leaf.OnSwipe += Swipe;
            }
        }

        public override void Play()
        {
            if (gameIsDone) return;
            base.Play();
            iconDetector.ChangeIconDisplayStatus(EIconDisplayState.HIDE);
            playerInput.enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            StartCoroutine(StartSwipeGame());
        }

        IEnumerator StartSwipeGame()
        {
            yield return StartGameRoutine();
            yield return new WaitUntil(() => DialogueAudioScript.Instance.WaitUntilDialogueDone(DialogueAudioScript.Instance.CurrentSessionID));
            DialogueAudioScript.Instance.PlayDialogue("ITPT_1");
            SubtitleUI.Instance.DisplaySubtitle("There is something under the leaves here..", ESubtitleDisplayMode.Dynamic);
        }

        protected override void EndGame()
        {
            if (gameIsDone) return;
            base.EndGame();
            playerInput.enabled = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            gameIsDone = true;
            StartCoroutine(EndSwipeGame());
        }

        IEnumerator EndSwipeGame()
        {
            yield return EndGameRoutine();
            DialogueAudioScript.Instance.PlayDialogue("ITPT_2");
            SubtitleUI.Instance.DisplaySubtitle("I remember! This was Mrs. Porter's last trip before going into maternity leave! ", ESubtitleDisplayMode.Dynamic);
            int mySessionID = DialogueAudioScript.Instance.CurrentSessionID;
            yield return new WaitUntil(() => DialogueAudioScript.Instance.WaitUntilDialogueDone(mySessionID));
            if (DialogueAudioScript.Instance.WasCancelled) yield break;
            DialogueAudioScript.Instance.PlayDialogue("ITPT_3");
            SubtitleUI.Instance.DisplaySubtitle("We used this field day to send her off and I made the card for her! (with some parental help..)", ESubtitleDisplayMode.Dynamic);
            mySessionID = DialogueAudioScript.Instance.CurrentSessionID;
            yield return new WaitUntil(() => DialogueAudioScript.Instance.WaitUntilDialogueDone(mySessionID));
            if (DialogueAudioScript.Instance.WasCancelled) yield break;
            DialogueAudioScript.Instance.PlayDialogue("ITPT_4");
            SubtitleUI.Instance.DisplaySubtitle("We hid the card in the play tunnel so all students could write our names on it without her seeing.. I must have been here when my sketchbook got swiped..", ESubtitleDisplayMode.Dynamic);
            mySessionID = DialogueAudioScript.Instance.CurrentSessionID;
            yield return new WaitUntil(() => DialogueAudioScript.Instance.WaitUntilDialogueDone(mySessionID));
            if (DialogueAudioScript.Instance.WasCancelled) yield break;
            DialogueAudioScript.Instance.PlayDialogue("ITPT_5");
            SubtitleUI.Instance.DisplaySubtitle("Mrs. Porter was a really beloved teacher.. We even crafted something together for her to remember us by.. ", ESubtitleDisplayMode.Dynamic);
            mySessionID = DialogueAudioScript.Instance.CurrentSessionID;
            yield return new WaitUntil(() => DialogueAudioScript.Instance.WaitUntilDialogueDone(mySessionID));
            if (DialogueAudioScript.Instance.WasCancelled) yield break;
            BGMusicManager.Instance.StopBGMusic();
            DialogueAudioScript.Instance.PlayDialogue("ITPT_6");
            SubtitleUI.Instance.DisplaySubtitle("Wait. That item that brought me here..!", ESubtitleDisplayMode.Dynamic);
            mySessionID = DialogueAudioScript.Instance.CurrentSessionID;
            yield return new WaitUntil(() => DialogueAudioScript.Instance.WaitUntilDialogueDone(mySessionID));
            if (DialogueAudioScript.Instance.WasCancelled) yield break;
        }

        protected override void OpenUI()
        {
            swipeGameUI.gameObject.SetActive(true);
        }

        protected override void CloseUI()
        {
            swipeGameUI.gameObject.SetActive(false);
            iconDetector.ChangeIconDisplayStatus(EIconDisplayState.DISPLAY);
        }

        void Swipe(SwipeableLeaf _leaf)
        {
            if (!leafs.Contains(_leaf)) return;
            StartCoroutine(RemoveLeaf(_leaf));
        }

        IEnumerator RemoveLeaf(SwipeableLeaf _leaf)
        {
            leafs.Remove(_leaf);
            _leaf.OnSwipe -= Swipe;

            var direction = _leaf.SwipedDirection.normalized;
            if (direction == Vector2.right) direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            var startPos = _leaf.transform.position;
            float screenDiagonal = new Vector2(Screen.width, Screen.height).magnitude;
            var targetPos = startPos + (Vector3)(direction * (screenDiagonal * 2f));


            yield return OverTimeMovement.MoveOverTime(startPos, targetPos, 2f, _pos => _leaf.transform.position = _pos);

            Destroy(_leaf.gameObject);
            yield return null;

            if (leafs.Count == 0 && _leaf == null) EndGame();
        }
    }
}