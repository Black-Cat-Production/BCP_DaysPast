using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.MinigameSystem.Memory
{
    public class MemoryCard : MonoBehaviour
    {
        [field: SerializeField] public int Id { get; private set; }
        public bool IsRemoved { get; private set; }
        bool isFaceUp;

        public Action<MemoryCard> OnFaceUp;

        void TurnFaceUp()
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);
            OnFaceUp.Invoke(this);
        }

        public void TurnFaceDown()
        {
            transform.rotation = Quaternion.Euler(0, 0, -180);
            transform.position = new Vector3(transform.position.x, 1.515f, transform.position.z);
        }

        public void RemoveFromBoard(Vector3 _targetPosition)
        {
            transform.position = _targetPosition;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            IsRemoved = true;
        }

        public void Select()
        {
            Debug.Log("I got clicked! " + name);
            if (IsRemoved) return;
            if (!isFaceUp) TurnFaceUp();
        }
    }
}