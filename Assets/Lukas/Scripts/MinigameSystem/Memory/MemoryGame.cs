using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
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
        [SerializeField] Vector3 removedPosition;
        [SerializeField] float turnWaitTime;
        [SerializeField] float rotationMin;
        [SerializeField] float rotationMax;
        [SerializeField] float liftDuration;
        [SerializeField] float flipDuration;

        MemoryCard firstSelectedCard;
        Coroutine waitRoutine;
        Coroutine turnRoutine;

        bool isGameOver;


        void OnEnable()
        {
            foreach (var card in cards)
            {
                card.OnFaceUp += CardSelect;
                float randomRota = Random.Range(rotationMin, rotationMax);
                card.transform.rotation = Quaternion.Euler(card.transform.rotation.x, randomRota, -180);
            }
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
            miniGameCam.gameObject.SetActive(true);
            miniGameCam.MoveToTopOfPrioritySubqueue();
            playerInput.SwitchCurrentActionMap("MemoryGame");
            Debug.Log(playerInput.currentActionMap.name);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        protected override void EndGame()
        {
            miniGameCam.gameObject.SetActive(false);
            playerInput.SwitchCurrentActionMap("Player");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            playerInput.enabled = true;
            playerInput.currentActionMap.Enable();
            isGameOver = true;
        }

        void CardSelect(MemoryCard _card)
        {
            if (turnRoutine != null || waitRoutine != null) return;
            if (firstSelectedCard == null) firstSelectedCard = _card;
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
            var startRot = _card.transform.rotation;
            var endRot = Quaternion.Euler(0, _card.transform.eulerAngles.y, 0);
            var startPos = _card.transform.position;
            var liftedPos = startPos + Vector3.up / 4;

            if (_reverse)
            {
                endRot = Quaternion.Euler(0, _card.transform.eulerAngles.y, 180);
            }

            yield return OverTimeMovement.MoveOverTime(startPos, liftedPos, liftDuration, _pos => _card.transform.position = _pos);

            yield return OverTimeMovement.MoveOverTime(startRot, endRot, flipDuration, _rot => _card.transform.rotation = _rot);

            yield return OverTimeMovement.MoveOverTime(liftedPos, startPos, liftDuration, _pos => _card.transform.position = _pos);

            turnRoutine = null;
        }

        protected override void OpenUI()
        {
            throw new System.NotImplementedException();
        }

        protected override void CloseUI()
        {
            throw new System.NotImplementedException();
        }
    }
}