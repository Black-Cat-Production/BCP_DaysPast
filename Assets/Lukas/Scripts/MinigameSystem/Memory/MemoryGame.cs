using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using FMODUnity;
using Scripts.InteractionSystem;
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
            isGameOver = true;
            iconDetector.ChangeIconDisplayStatus(EIconDisplayState.DISPLAY);
            yield return StartCoroutine(blackoutTransition.TransitionFromBlackout(fakeVolumeImage));
        }

        protected override void EndGame()
        {
            base.EndGame();
            StartCoroutine(EndGameRoutine());
            
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
                yield return TurnAroundRoutine(firstSelectedCard, true);
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

        IEnumerator TurnAroundRoutine(MemoryCard _card, bool _reverse)
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

            turnEventEmitter.Play();
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