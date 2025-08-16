using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using Scripts.Audio;
using Scripts.Movement;
using Scripts.UI.Subtitles;
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
        StudioEventEmitter placementEventEmitter;

        public override void Play()
        {
            if (gameIsDone) return;
            base.Play();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            playerInput.SwitchCurrentActionMap("FlowerbookGame");
            StartCoroutine(StartFlowerbook());
            
        }

        IEnumerator StartFlowerbook()
        {
            yield return StartGameRoutine();
            DialogueAudioScript.Instance.PlayDialogue("FBMINI_2");
            SubtitleUI.Instance.DisplaySubtitle("Oh no, all of the tape came loose, I have no idea where everything belonged anymore.", ESubtitleDisplayMode.Dynamic);
        }

        void OnEnable()
        {
            placementEventEmitter = flowerbookUI.GetComponent<StudioEventEmitter>();
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
                UpdateSelectedLeaf(false);
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
            base.EndGame();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            playerInput.SwitchCurrentActionMap("Player");
            gameIsDone = true;
            StartCoroutine(EndFlowerbook());
        }

        IEnumerator EndFlowerbook()
        {
            DialogueAudioScript.Instance.PlayDialogue("FBMINI_3");
            SubtitleUI.Instance.DisplaySubtitle("Yes, this is it!", ESubtitleDisplayMode.Dynamic);
            yield return StartCoroutine(EndGameRoutine());
            DialogueAudioScript.Instance.PlayDialogue("FB_4");
            int mySessionID = DialogueAudioScript.Instance.CurrentSessionID;
            SubtitleUI.Instance.DisplaySubtitle("Much better! This was one of my favorite things to do back in the day. I don't really have time for stuff like this anymore.", ESubtitleDisplayMode.Dynamic);
            yield return new WaitUntil(() => DialogueAudioScript.Instance.WaitUntilDialogueDone(mySessionID));
            if (DialogueAudioScript.Instance.WasCancelled) yield break;
            DialogueAudioScript.Instance.PlayDialogue("FB_5");
            mySessionID = DialogueAudioScript.Instance.CurrentSessionID;
            SubtitleUI.Instance.DisplaySubtitle("Besides- I live in the city now! not exactly many flowers around... But at the same time kid me would be so jealous if she knew I live so close to her favorite toy store now.", ESubtitleDisplayMode.Dynamic);
            yield return new WaitUntil(() => DialogueAudioScript.Instance.WaitUntilDialogueDone(mySessionID));
            if (DialogueAudioScript.Instance.WasCancelled) yield break;
            DialogueAudioScript.Instance.PlayDialogue("FB_6");
            mySessionID = DialogueAudioScript.Instance.CurrentSessionID;
            SubtitleUI.Instance.DisplaySubtitle("My world sure got a little bigger over time.",ESubtitleDisplayMode.Dynamic);
            yield return new WaitUntil(() => DialogueAudioScript.Instance.WaitUntilDialogueDone(mySessionID));
            if (DialogueAudioScript.Instance.WasCancelled) yield break;
            DialogueAudioScript.Instance.PlayDialogue("SH_1");
            mySessionID = DialogueAudioScript.Instance.CurrentSessionID;
            SubtitleUI.Instance.DisplaySubtitle("With this whole “changing” thing- I used to climb underneath all of the playground I could fit! Both to hide from teachers, but also to make a little secret hideout!", ESubtitleDisplayMode.Dynamic);
            yield return new WaitUntil(() => DialogueAudioScript.Instance.WaitUntilDialogueDone(mySessionID));
            if (DialogueAudioScript.Instance.WasCancelled) yield break;
            DialogueAudioScript.Instance.PlayDialogue("SH_2");
            mySessionID = DialogueAudioScript.Instance.CurrentSessionID;
            SubtitleUI.Instance.DisplaySubtitle("I’m pretty sure at this playground I also had one. I can probably fit it now!", ESubtitleDisplayMode.Dynamic);
            yield return new WaitUntil(() => DialogueAudioScript.Instance.WaitUntilDialogueDone(mySessionID));
            if (DialogueAudioScript.Instance.WasCancelled) yield break;

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
            if (selectedLeaf == null) return;
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