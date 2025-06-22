using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.MinigameSystem.Memory
{
    public class MemoryGame : Minigame
    {
        [SerializeField] List<MemoryCard> cards = new List<MemoryCard>();
        [SerializeField] CinemachineVirtualCamera miniGameCam;
        [SerializeField] PlayerInput playerInput;
        [SerializeField] Vector3 removedPosition;
        [SerializeField] float turnWaitTime;

        MemoryCard firstSelectedCard;
        Coroutine waitRoutine;

        bool isGameOver;


        void OnEnable()
        {
            foreach (var card in cards)
            {
                card.OnFaceUp += CardSelect;
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
            if(isGameOver) return;
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
            if (firstSelectedCard == null) firstSelectedCard = _card;
            if (firstSelectedCard == _card) return;
            if (waitRoutine != null)
            {
                _card.TurnFaceDown();
                return;
            }

            waitRoutine = StartCoroutine(firstSelectedCard.Id == _card.Id ? WaitRoutine(_card, true) : WaitRoutine(_card, false));
        }

        IEnumerator WaitRoutine(MemoryCard _card, bool _isCorrect)
        {
            yield return new WaitForSeconds(turnWaitTime);
            if (_isCorrect)
            {
                firstSelectedCard.RemoveFromBoard(CalculateYCoord());
                _card.RemoveFromBoard(CalculateYCoord());
            }
            else
            {
                _card.TurnFaceDown();
                firstSelectedCard.TurnFaceDown();
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