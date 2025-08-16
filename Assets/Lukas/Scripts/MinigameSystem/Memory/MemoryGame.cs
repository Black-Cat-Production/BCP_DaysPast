using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using FMODUnity;
using Scripts.Audio;
using Scripts.Audio.AudioManager;
using Scripts.InteractionSystem;
using Scripts.UI.Subtitles;
using Scripts.Utility;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Scripts.MinigameSystem.Memory
{
    public class MemoryGame : Minigame
    {
        [SerializeField] List<MemoryCard> cards = new List<MemoryCard>();
        [SerializeField] CinemachineVirtualCamera miniGameCam;
        [SerializeField] PlayerInput playerInput;
        [SerializeField] InteractionIconDetector iconDetector;
        [SerializeField] Vector3 removedPosition;
        [SerializeField] float turnWaitTime;
        [SerializeField] float rotationMin;
        [SerializeField] float rotationMax;
        [SerializeField] float liftDuration;
        [SerializeField] float flipDuration;

        [Header("Audio")]
        [SerializeField] StudioEventEmitter turnEventEmitter;
        [SerializeField] StudioEventEmitter clickEventEmitter;
        [SerializeField] StudioEventEmitter pairEventEmitter;

        [Header("Scene Logic")]
        [SerializeField] GameObject originalMemoryObject;

        [SerializeField] GameObject minigameMemoryObject;

        MemoryCard firstSelectedCard;
        Coroutine waitRoutine;
        Coroutine turnRoutine;

        bool isGameOver;


        void OnEnable()
        {
            int subscribedCards = 0;
            foreach (var card in cards)
            {
                card.OnFaceUp += CardSelect;
                subscribedCards++;
                float randomRota = Random.Range(rotationMin, rotationMax);
                card.transform.rotation = Quaternion.Euler(card.transform.rotation.x, randomRota + -184, 180);
            }

            Debug.Log(subscribedCards);
        }

        void OnDisable()
        {
            foreach (var card in cards)
            {
                card.OnFaceUp -= CardSelect;
            }
        }

        void FixedUpdate()
        {
            if (isGameOver) return;
            if (cards.All((_card => _card.IsRemoved)))
            {
                EndGame();
            }
        }

        public override void Play()
        {
            if (isGameOver) return;
            base.Play();
            iconDetector.ChangeIconDisplayStatus(EIconDisplayState.HIDE);
            StartCoroutine(StartGameRoutine());
            
        }

        protected override IEnumerator StartGameRoutine()
        {
            yield return StartCoroutine(blackoutTransition.TransitionToBlackout(fakeVolumeImage));
            miniGameCam.gameObject.SetActive(true);
            miniGameCam.MoveToTopOfPrioritySubqueue();
            playerInput.SwitchCurrentActionMap("MemoryGame");
            Debug.Log(playerInput.currentActionMap.name);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            originalMemoryObject.SetActive(false);
            minigameMemoryObject.SetActive(true);
            audioHelper.PlayStartAudio();
            yield return StartCoroutine(blackoutTransition.TransitionFromBlackout(fakeVolumeImage));
        }

        protected override IEnumerator EndGameRoutine()
        {
            audioHelper.PlayEndAudio();
            yield return new WaitWhile(audioHelper.EndAudioIsPlaying);
            yield return StartCoroutine(blackoutTransition.TransitionToBlackout(fakeVolumeImage));
            miniGameCam.gameObject.SetActive(false);
            playerInput.SwitchCurrentActionMap("Player");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            playerInput.enabled = true;
            playerInput.currentActionMap.Enable();
            iconDetector.ChangeIconDisplayStatus(EIconDisplayState.DISPLAY);
            yield return StartCoroutine(blackoutTransition.TransitionFromBlackout(fakeVolumeImage));
        }

        protected override void EndGame()
        {
            isGameOver = true;
            base.EndGame();
            DialogueAudioScript.Instance.PlayDialogue("MEMO_4");
            SubtitleUI.Instance.DisplaySubtitle("Well.. This definitely used to be harder. Not sure how this kept us entertained so long.", ESubtitleDisplayMode.Dynamic);
            StartCoroutine(EndMemory());
        }

        IEnumerator EndMemory()
        {
            yield return EndGameRoutine();
            BGMusicManager.Instance.PlayBGMusic(4);
            DialogueAudioScript.Instance.PlayDialogue("MEMO_5");
            SubtitleUI.Instance.DisplaySubtitle("I like to think I have more refined tastes nowadays..With my pencils here and the card in the sketchbook.. ",ESubtitleDisplayMode.Dynamic);
            int mySessionID = DialogueAudioScript.Instance.CurrentSessionID;
            yield return new WaitUntil(() => DialogueAudioScript.Instance.WaitUntilDialogueDone(mySessionID));
            if (DialogueAudioScript.Instance.WasCancelled) yield break;
            DialogueAudioScript.Instance.PlayDialogue("MEMO_6");
            SubtitleUI.Instance.DisplaySubtitle("That kid did´nt take my sketchbook from my backpack at all! But why would I leave it laying open like this? ", ESubtitleDisplayMode.Dynamic);
            mySessionID = DialogueAudioScript.Instance.CurrentSessionID;
            yield return new WaitUntil(() => DialogueAudioScript.Instance.WaitUntilDialogueDone(mySessionID));
            if (DialogueAudioScript.Instance.WasCancelled) yield break;
            DialogueAudioScript.Instance.PlayDialogue("TRUTH_1");
            SubtitleUI.Instance.DisplaySubtitle("Wait. I think I had to go and sign on some kind of birthday card for our teacher..I was always the one to make gifts like this as the designated crafty kid.",ESubtitleDisplayMode.Dynamic);
            mySessionID = DialogueAudioScript.Instance.CurrentSessionID;
            yield return new WaitUntil(() => DialogueAudioScript.Instance.WaitUntilDialogueDone(mySessionID));
            if (DialogueAudioScript.Instance.WasCancelled) yield break;
            DialogueAudioScript.Instance.PlayDialogue("TRUTH_2");
            SubtitleUI.Instance.DisplaySubtitle("That’s why I took all of my art things and my pressed flowers on this trip!",ESubtitleDisplayMode.Dynamic);
            mySessionID = DialogueAudioScript.Instance.CurrentSessionID;
            yield return new WaitUntil(() => DialogueAudioScript.Instance.WaitUntilDialogueDone(mySessionID));
            if (DialogueAudioScript.Instance.WasCancelled) yield break;
            DialogueAudioScript.Instance.PlayDialogue("TRUTH_3");
            SubtitleUI.Instance.DisplaySubtitle("All of this- and the reason something about this day is off.. It has to have something to do with Mrs. Porter.",ESubtitleDisplayMode.Dynamic);
            mySessionID = DialogueAudioScript.Instance.CurrentSessionID;
            yield return new WaitUntil(() => DialogueAudioScript.Instance.WaitUntilDialogueDone(mySessionID));
            if (DialogueAudioScript.Instance.WasCancelled) yield break;
            DialogueAudioScript.Instance.PlayDialogue("TRUTH_4");
            SubtitleUI.Instance.DisplaySubtitle("We must have signed the card in secret somewhere, where she would’nt see it.. I know I’m close to the answer, I have to find it!",ESubtitleDisplayMode.Dynamic);
            mySessionID = DialogueAudioScript.Instance.CurrentSessionID;
            yield return new WaitUntil(() => DialogueAudioScript.Instance.WaitUntilDialogueDone(mySessionID));
            if (DialogueAudioScript.Instance.WasCancelled) yield break;
        }

        void CardSelect(MemoryCard _card)
        {
            if (turnRoutine != null || waitRoutine != null) return;
            if (firstSelectedCard == null) firstSelectedCard = _card;
            clickEventEmitter.Play();
            turnRoutine = StartCoroutine(TurnAroundRoutine(_card, false));
            if (firstSelectedCard == _card) return;


            waitRoutine = StartCoroutine(firstSelectedCard.Id == _card.Id ? WaitRoutine(_card, true) : WaitRoutine(_card, false));
        }

        IEnumerator WaitRoutine(MemoryCard _card, bool _isCorrect)
        {
            while (turnRoutine != null) yield return null;
            yield return new WaitForSeconds(turnWaitTime);
            if (_isCorrect)
            {
                pairEventEmitter.Play();
                StartCoroutine(firstSelectedCard.RemoveFromBoard(0.4f, CalculateYCoord()));
                yield return _card.RemoveFromBoard(0.7f, CalculateYCoord());
            }
            else
            {
                StartCoroutine(TurnAroundRoutine(_card, true));
                yield return TurnAroundRoutine(firstSelectedCard, true, true);
            }

            waitRoutine = null;
            firstSelectedCard = null;
        }

        Vector3 CalculateYCoord()
        {
            int removedCards = cards.Count((_card) => _card.IsRemoved);
            if (removedCards == 0) return removedPosition;
            return new Vector3(removedPosition.x, removedPosition.y + removedCards * 0.015f, removedPosition.z);
        }

        IEnumerator TurnAroundRoutine(MemoryCard _card, bool _reverse, bool _secondCard = false)
        {
            if (turnRoutine != null) yield break;
            var startRot = _card.transform.rotation;
            var endRot = Quaternion.Euler(0, _card.transform.eulerAngles.y, 0);
            var startPos = _card.transform.localPosition;
            Debug.Log(startPos);
            var liftedPos = startPos + Vector3.up / 4;

            if (_reverse)
            {
                endRot = Quaternion.Euler(0, _card.transform.eulerAngles.y, 180);
            }

            yield return OverTimeMovement.MoveOverTime(startPos, liftedPos, liftDuration, _pos => _card.transform.localPosition = _pos);

            if(!_secondCard)turnEventEmitter.Play();
            yield return OverTimeMovement.MoveOverTime(startRot, endRot, flipDuration, _rot => _card.transform.rotation = _rot);

            yield return OverTimeMovement.MoveOverTime(liftedPos, startPos, liftDuration, _pos => _card.transform.localPosition = _pos);

            turnRoutine = null;
        }

        protected override void OpenUI()
        {
           
        }

        protected override void CloseUI()
        {
            
        }
    }
}