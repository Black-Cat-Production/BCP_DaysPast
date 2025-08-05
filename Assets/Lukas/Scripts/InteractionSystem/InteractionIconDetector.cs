using UnityEngine;

namespace Scripts.InteractionSystem
{
    [RequireComponent(typeof(SphereCollider))]
    public class InteractionIconDetector : MonoBehaviour
    {
        IInteractable interactable;
        
        void OnTriggerEnter(Collider _other)
        {
            interactable = _other.GetComponent<IInteractable>();
            interactable?.ShowInteractIcon();
        }

        void OnTriggerExit(Collider _other)
        {
            var exitedCollider = _other.GetComponent<IInteractable>();
            exitedCollider?.HideInteractIcon();
            if(exitedCollider == interactable) interactable = null;
        }
    }
}