using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.InteractionSystem
{
    public class InteractionModule : MonoBehaviour
    {
        IInteractable interactable;
        
        void OnTriggerEnter(Collider _other)
        {
            interactable = _other.GetComponent<IInteractable>();
        }

        public void Interact(InputAction.CallbackContext _callbackContext)
        {
            if (_callbackContext.phase != InputActionPhase.Started) return;
            interactable?.Interact();
        }

        void OnTriggerExit(Collider _other)
        {
            var exitedCollider = _other.GetComponent<IInteractable>();
            if(exitedCollider == interactable) interactable = null;
        }
    }
}