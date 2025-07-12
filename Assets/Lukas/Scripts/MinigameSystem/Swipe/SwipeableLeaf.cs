using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.MinigameSystem.Swipe
{
    public class SwipeableLeaf : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] public Vector2 swipedDirection = Vector2.right;
        public Action<SwipeableLeaf> OnSwipe;
        public void OnPointerClick(PointerEventData _eventData)
        {
            OnSwipe.Invoke(this);
        }
    }
}