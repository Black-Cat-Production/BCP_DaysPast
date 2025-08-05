using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.MinigameSystem.Flowerbook;
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
        
        const float FlyOffDistance = 3f;

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
            playerInput.enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            StartCoroutine(StartGameRoutine());
        }

        protected override void EndGame()
        {
            if(gameIsDone) return;
            playerInput.enabled = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            gameIsDone = true;
            StartCoroutine(EndGameRoutine());
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
    
            var direction = _leaf.SwipedDirection.normalized;
            if (direction == Vector2.right)
                direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

            var startPos = _leaf.transform.position;
            
            var targetPos = startPos + (Vector3)(direction * FlyOffDistance);

            yield return OverTimeMovement.MoveOverTime(startPos, targetPos, 2f, _pos => _leaf.transform.position = _pos);

            Destroy(_leaf.gameObject);
            yield return null;

            if (leafs.Count == 0 && _leaf == null)
                EndGame();
        }
    }
}