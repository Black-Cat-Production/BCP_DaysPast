using System;
using Scripts.MinigameSystem.Memory;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.InteractionSystem
{
    public class InteractionModule : MonoBehaviour
    {
        IInteractable interactable;
        
        void OnTriggerEnter(Collider _other)
        {
            if (_other.TryGetComponent(out MemoryCard _)) return;
            if (!_other.TryGetComponent(out IInteractable component)) return;
            interactable = component;
            Debug.Log("Entered: " + _other.gameObject.name);
        }

        public void Interact(InputAction.CallbackContext _callbackContext)
        {
            if (_callbackContext.phase != InputActionPhase.Started) return;
            interactable?.Interact();
        }

        void OnTriggerExit(Collider _other)
        {
            if (_other.TryGetComponent(out MemoryCard _)) return;
            var exitedCollider = _other.GetComponent<IInteractable>();
            if (exitedCollider != interactable || exitedCollider == null || interactable == null) return;
            Debug.Log("Interactable: " + interactable);
            Debug.Log("Exited: " + _other.gameObject.name);
            interactable = null;
        }
    }
}