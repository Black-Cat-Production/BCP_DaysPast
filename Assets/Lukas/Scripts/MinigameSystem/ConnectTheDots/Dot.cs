using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.MinigameSystem.ConnectTheDots
{
    public class Dot : MonoBehaviour, IPointerClickHandler
    {
        public Action<Dot> OnClick;
        
        public bool IsConnected { get; private set; }
        
        public void OnPointerClick(PointerEventData _eventData)
        {
            OnClick.Invoke(this);
        }

        public void Connect()
        {
            Debug.Log("I got connected!" + name);
            IsConnected = true;
        }

        public void Disconnect()
        {
            IsConnected = false;
        }
    }
}