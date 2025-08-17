using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using Scripts.Audio;
using Scripts.InteractionSystem;
using Scripts.UI.Subtitles;
using Scripts.Utility;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Scripts.MinigameSystem.ConnectTheDots
{
    [RequireComponent(typeof(UILineDrawer))]
    public class ConnectTheDotsGame : Minigame
    {
        [SerializeField] CanvasGroup connectUIGroup;
        [SerializeField] PlayerInput playerInput;
        [SerializeField] List<Dot> dots = new List<Dot>();
        [SerializeField] TutorialTextHelper tutorialTextHelper;
        UILineDrawer lineDrawer;
        Dot selectedDot;

        Dot startDot;

        StudioEventEmitter lineEventEmitter;

        public struct LineSegment
        {
            public Vector2 start;
            public Vector2 end;

            public LineSegment(Vector2 _start, Vector2 _end)
            {
                start = _start;
                end = _end;
            }
        }

        readonly List<LineSegment> lineSegments = new List<LineSegment>();

        protected override void Awake()
        {
            base.Awake();
            lineDrawer = GetComponent<UILineDrawer>();
        }

        void OnEnable()
        {
            foreach (var dot in dots)
            {
                dot.OnClick += DotSelect;
            }

            lineEventEmitter = connectUIGroup.GetComponent<StudioEventEmitter>();
        }

        void OnDisable()
        {
            foreach (var dot in dots)
            {
                dot.OnClick -= DotSelect;
            }
        }

        void DotSelect(Dot _dot)
        {
            if (selectedDot == null)
            {
                selectedDot = _dot;
                Debug.Log("Selected Dot: " + selectedDot.name);
                return;
            }

            Debug.Log("Selected Dot: " + selectedDot.name);

            if (IsInvalidSelection(_dot)) return;

            var start = selectedDot.GetComponent<RectTransform>().anchoredPosition;
            var end = _dot.GetComponent<RectTransform>().anchoredPosition;
            var newLineSegment = new LineSegment(start, end);

            if (CreatesIntersection(newLineSegment))
            {
                Debug.Log("Intersection Detected!");
                return;
            }

            if (IsTryingToReconnectToStart(_dot))
            {
                HandleWinningConnection(start, end);
                return;
            }

            ConnectDot(start, end, newLineSegment, _dot);
        }

        bool IsInvalidSelection(Dot _dot)
        {
            if (selectedDot.IsConnected) return true;
            if (selectedDot == _dot) return true;

            return _dot.IsConnected && !IsTryingToReconnectToStart(_dot);
        }

        bool IsTryingToReconnectToStart(Dot _dot)
        {
            return _dot == startDot && AllowConnectToFirstDot();
        }

        bool CreatesIntersection(LineSegment _newSegment)
        {
            return IntersectionUtility.IntersectAny(_newSegment, lineSegments);
        }

        void HandleWinningConnection(Vector2 _start, Vector2 _end)
        {
            lineDrawer.DrawLine(_start, _end);
            Debug.Log("You won!");
            EndGame();
        }

        void ConnectDot(Vector2 _start, Vector2 _end, LineSegment _segment, Dot _nextDot)
        {
            lineDrawer.DrawLine(_start, _end);
            lineSegments.Add(_segment);
            selectedDot.Connect();
            lineEventEmitter.Play();

            if (startDot == null)
                startDot = selectedDot;

            selectedDot = _nextDot;
        }

        bool AllowConnectToFirstDot()
        {
            int unconnectedDots = dots.Count(_dot => _dot.IsConnected == false);
            return unconnectedDots <= 1;
        }

        public override void Play()
        {
            base.Play();
            if (gameIsDone) return;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            playerInput.enabled = false;
            StartCoroutine(StartConnectTheDots());
        }

        IEnumerator StartConnectTheDots()
        {
            yield return StartGameRoutine();
            tutorialTextHelper.DisplayTutorial(_delayUntilVoiceLine: true);
            yield return new WaitUntil(() => DialogueAudioScript.Instance.WaitUntilDialogueDone(DialogueAudioScript.Instance.CurrentSessionID));
            if (DialogueAudioScript.Instance.WasCancelled) yield break;
            DialogueAudioScript.Instance.PlayDialogue("CTD_1");
            SubtitleUI.Instance.DisplaySubtitle("My friend Tom sometimes came up with puzzles. He was the only other kid allowed in here- he drew them in chalk he snatched from our classroom!", ESubtitleDisplayMode.Dynamic);
        }


        protected override void EndGame()
        {
            base.EndGame();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            playerInput.enabled = true;
            gameIsDone = true;
            DialogueAudioScript.Instance.PlayDialogue("CTD_2");
            SubtitleUI.Instance.DisplaySubtitle("I still got it! ",ESubtitleDisplayMode.Dynamic);
            StartCoroutine(EndConnectTheDots());
        }

        IEnumerator EndConnectTheDots()
        {
            yield return EndGameRoutine();
            tutorialTextHelper.DestroyTutorial();
            DialogueAudioScript.Instance.PlayDialogue("SH_5");
            SubtitleUI.Instance.DisplaySubtitle("Tom and I lost contact after school, but we used to get in trouble all the time together.. ",ESubtitleDisplayMode.Dynamic);
            int mySessionID = DialogueAudioScript.Instance.CurrentSessionID;
            yield return new WaitUntil(() => DialogueAudioScript.Instance.WaitUntilDialogueDone(mySessionID));
            if (DialogueAudioScript.Instance.WasCancelled) yield break;
            DialogueAudioScript.Instance.PlayDialogue("SH_6");
            SubtitleUI.Instance.DisplaySubtitle("Hardly a class trip someone wouldn’t get a bloody nose or scratched up knees at least! Thinking of.. Maybe someone got hurt on this trip!", ESubtitleDisplayMode.Dynamic);
            mySessionID = DialogueAudioScript.Instance.CurrentSessionID;
            yield return new WaitUntil(() => DialogueAudioScript.Instance.WaitUntilDialogueDone(mySessionID));
            if (DialogueAudioScript.Instance.WasCancelled) yield break;
            DialogueAudioScript.Instance.PlayDialogue("SH_7");
            SubtitleUI.Instance.DisplaySubtitle("And that’s why it wont leave my mind.. But how? Mrs. Porter always kept an eye out from the seating area..",ESubtitleDisplayMode.Dynamic);
        }


        //Function is used by unity button.
        public void ResetGame()
        {
            lineDrawer.ClearLinesFromScene();
            lineSegments.Clear();
            startDot = null;
            selectedDot = null;
            foreach (var dot in dots)
            {
                dot.Disconnect();
            }
        }

        protected override void OpenUI()
        {
            connectUIGroup.gameObject.SetActive(true);
        }

        protected override void CloseUI()
        {
            lineDrawer.ClearLinesFromScene();
            lineSegments.Clear();
            connectUIGroup.gameObject.SetActive(false);
        }
    }
}