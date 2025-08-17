using System;
using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.MinigameSystem.Swipe
{
    public class SwipeableLeaf : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] public Vector2 SwipedDirection = Vector2.right;
        public Action<SwipeableLeaf> OnSwipe;

        StudioEventEmitter eventEmitter;
        void Awake()
        {
            eventEmitter = GetComponent<StudioEventEmitter>();
        }

        public void OnPointerClick(PointerEventData _eventData)
        {
            eventEmitter.Play();
            OnSwipe?.Invoke(this);
        }
    }
}