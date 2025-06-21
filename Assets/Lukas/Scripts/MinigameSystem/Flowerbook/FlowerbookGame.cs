using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.MinigameSystem.Flowerbook
{
    public class FlowerbookGame : Minigame
    {
        [SerializeField] CanvasGroup flowerbookUI;
        [SerializeField] List<Leaf> leafs = new List<Leaf>();
        [SerializeField] List<LeafSpot> leafSpots = new List<LeafSpot>();
        [SerializeField] PlayerInput playerInput;
        public override void Play()
        {
            OpenUI();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            playerInput.enabled = false;
        }

        void OnEnable()
        {
            foreach (var leaf in leafs)
            {
                leaf.OnPlace += PlaceLeaf;
            }
        }

        void PlaceLeaf(Leaf _leaf)
        {
            foreach (var leafSpot in leafSpots.Where(_leafSpot => _leafSpot.MatchesLeaf(_leaf)))
            {
                _leaf.Place(leafSpot);
                leafSpot.ActivateFilled();
                CheckWin();
                return;
            }
            _leaf.Return();
        }

        void CheckWin()
        {
            if (!leafSpots.All(_spot => _spot.IsFull)) return;
            Debug.Log("You won the minigame!");
            EndGame();
        }

        protected override void EndGame()
        {
            CloseUI();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            playerInput.enabled = true;
        }

        protected override void OpenUI()
        {
            flowerbookUI.gameObject.SetActive(true);
        }

        protected override void CloseUI()
        {
            flowerbookUI.gameObject.SetActive(false);
        }
    }
}