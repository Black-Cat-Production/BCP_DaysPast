using System;
using System.Collections;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Scripts.MinigameSystem.Memory
{
    public class MemoryCard : MonoBehaviour
    {
        [field: SerializeField] public int Id { get; private set; }
        public bool IsRemoved { get; private set; }

        float timer;

        public Action<MemoryCard> OnFaceUp;

        void TurnFaceUp()
        {
            OnFaceUp.Invoke(this);
        }

        public void Select()
        {
            Debug.Log("I got clicked! " + name);
            if (IsRemoved) return;
            TurnFaceUp();
        }


        public IEnumerator RemoveFromBoard(float _duration, Vector3 _targetPosition)
        {
            IsRemoved = true;
            float elapsedTime = 0;
            var currentPos = transform.position;
            while (elapsedTime < _duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / _duration;
                transform.position = Vector3.Lerp(currentPos, _targetPosition, t);
                yield return null;
            }
        }
    }
}