using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using Scripts.Movement;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using Random = UnityEngine.Random;

namespace Scripts.MinigameSystem.Flowerbook
{
    public class FlowerbookGame : Minigame
    {
        [SerializeField] CanvasGroup flowerbookUI;
        [SerializeField] List<Leaf> leafs = new List<Leaf>();
        [SerializeField] List<LeafSpot> leafSpots = new List<LeafSpot>();
        [SerializeField] PlayerInput playerInput;

        [SerializeField] float randomFlowerRotationMin;
        [SerializeField] float randomFlowerRotationMax;

        RectTransform selectedLeaf;
        PlayerController playerController;
        StudioEventEmitter placementEventEmitter;

        public override void Play()
        {
            if (gameIsDone) return;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            playerInput.SwitchCurrentActionMap("FlowerbookGame");
            StartCoroutine(StartGameRoutine());
            
        }

        void OnEnable()
        {
            placementEventEmitter = flowerbookUI.GetComponent<StudioEventEmitter>();
            playerController = playerInput.gameObject.GetComponent<PlayerController>();
            foreach (var leaf in leafs)
            {
                leaf.OnPlace += PlaceLeaf;
                leaf.OnPickup += SelectLeaf;
                //float randomRota = Random.Range(randomFlowerRotationMin, randomFlowerRotationMax);
                //leaf.transform.rotation = Quaternion.Euler(0,0,randomRota);
            }
            playerController.OnScroll += RotateFlower;
        }

        void OnDisable()
        {
            foreach (var leaf in leafs)
            {
                leaf.OnPlace -= PlaceLeaf;
                leaf.OnPickup -= SelectLeaf;
            }
            playerController.OnScroll -= RotateFlower;
        }

        void SelectLeaf(Leaf _leaf)
        {
            UpdateSelectedLeaf(true, _leaf);
        }

        void UpdateSelectedLeaf(bool _selected, Leaf _leaf = null)
        {
            selectedLeaf = _selected && _leaf != null ? _leaf.gameObject.GetComponent<RectTransform>() : null;
        }

        void PlaceLeaf(Leaf _leaf)
        {
            foreach (var leafSpot in leafSpots.Where(_leafSpot => _leafSpot.MatchesLeaf(_leaf)))
            {
                _leaf.Place(leafSpot);
                leafSpot.ActivateFilled();
                placementEventEmitter.Play();
                CheckWin();
                return;
            }

            _leaf.Return();
            UpdateSelectedLeaf(false, _leaf);
        }

        void CheckWin()
        {
            if (!leafSpots.All(_spot => _spot.IsFull)) return;
            Debug.Log("You won the minigame!");
            EndGame();
        }

        protected override void EndGame()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            playerInput.SwitchCurrentActionMap("Player");
            gameIsDone = true;
            StartCoroutine(EndGameRoutine());
        }

        protected override void OpenUI()
        {
            flowerbookUI.gameObject.SetActive(true);
        }

        protected override void CloseUI()
        {
            flowerbookUI.gameObject.SetActive(false);
        }

        void RotateFlower(float _yDelta)
        {
            switch (_yDelta)
            {
                case > 0:
                    selectedLeaf.Rotate(Vector3.back, 5f);
                    break;
                case < 0:
                    selectedLeaf.Rotate(Vector3.forward, 5f);
                    break;
            }
        }
    }
}