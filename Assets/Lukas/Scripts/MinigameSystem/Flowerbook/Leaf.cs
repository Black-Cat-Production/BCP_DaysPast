using System;
using Cinemachine;
using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Scripts.MinigameSystem.Flowerbook
{
    public class Leaf : MonoBehaviour, IPointerClickHandler
    {
        [field: SerializeField] public int Id { get; private set; }
        bool pickedUp;
        bool gotPlaced;
        Vector3 startPosition;

        public Action<Leaf> OnPlace;
        public Action<Leaf> OnPickup;
        StudioEventEmitter eventEmitter;
        
        void Awake()
        {
            startPosition = transform.position;
            eventEmitter = GetComponent<StudioEventEmitter>();
        }


        public void OnPointerClick(PointerEventData _eventData)
        {
            if (gotPlaced) return;
            pickedUp = !pickedUp;
            eventEmitter.Play();
            OnPickup?.Invoke(this);
            if (pickedUp) return;
            OnPlace?.Invoke(this);
        }

        void FixedUpdate()
        {
            if (!pickedUp) return;
            transform.position = Input.mousePosition;
        }

        public void Place(LeafSpot _matchedLeafSpot)
        {
            transform.position = _matchedLeafSpot.transform.position;
            transform.rotation = _matchedLeafSpot.transform.rotation;
            gotPlaced = true;
            pickedUp = false;
            Debug.Log("GotPlacedCorrectly!");
        }

        public void Return()
        {
            transform.position = startPosition;
        }
    }
}