using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.MinigameSystem.Flowerbook;
using Scripts.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.MinigameSystem.Swipe
{
    public class SwipeGame : Minigame
    {
        [SerializeField] CanvasGroup swipeGameUI;
        [SerializeField] List<SwipeableLeaf> leafs = new();
        [SerializeField] PlayerInput playerInput;

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
            OpenUI();
            playerInput.enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        protected override void EndGame()
        {
            CloseUI();
            playerInput.enabled = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            gameIsDone = true;
        }

        protected override void OpenUI()
        {
            swipeGameUI.gameObject.SetActive(true);
        }

        protected override void CloseUI()
        {
            swipeGameUI.gameObject.SetActive(false);
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
            var direction = _leaf.swipedDirection.normalized;
            var startPos = _leaf.transform.position;

            float screenDiagonal = new Vector2(Screen.width, Screen.height).magnitude;
            var targetPos = startPos + (Vector3)(direction * (screenDiagonal * 1.5f));
            
            yield return OverTimeMovement.MoveOverTime(startPos, targetPos, 2f, _pos => _leaf.transform.position = _pos);
            Destroy(_leaf.gameObject);
            if (leafs.Count == 0) EndGame();
        }
    }
}