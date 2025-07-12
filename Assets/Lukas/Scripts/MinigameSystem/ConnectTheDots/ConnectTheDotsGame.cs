using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.MinigameSystem.ConnectTheDots
{
    public class ConnectTheDotsGame : Minigame
    {
        [SerializeField] CanvasGroup minigameUIGroup;
        [SerializeField] PlayerInput playerInput;
        [SerializeField] List<Dot> dots = new List<Dot>();
        UILineDrawer lineDrawer;
        Dot selectedDot;

        Dot startDot;

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

        void Awake()
        {
            lineDrawer = GetComponent<UILineDrawer>();
        }

        void OnEnable()
        {
            foreach (var dot in dots)
            {
                dot.OnClick += DotSelect;
            }
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
            OpenUI();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            playerInput.enabled = false;
        }

        protected override void EndGame()
        {
            lineDrawer.ClearLinesFromScene();
            lineSegments.Clear();
            CloseUI();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            playerInput.enabled = true;
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
            minigameUIGroup.gameObject.SetActive(true);
        }

        protected override void CloseUI()
        {
            minigameUIGroup.gameObject.SetActive(false);
        }
    }
}