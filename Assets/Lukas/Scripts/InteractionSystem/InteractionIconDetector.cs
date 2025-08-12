using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.InteractionSystem
{
    [RequireComponent(typeof(SphereCollider))]
    public class InteractionIconDetector : MonoBehaviour
    {
        List<IInteractable> interactables = new List<IInteractable>();
        
        void OnTriggerEnter(Collider _other)
        {
            if (!_other.TryGetComponent(out IInteractable component)) return;
            ChangeIconDisplayStatus(component, EIconDisplayState.DISPLAY);
            interactables.Add(component);
        }

        void OnTriggerExit(Collider _other)
        {
            if (!_other.TryGetComponent(out IInteractable component)) return;
            ChangeIconDisplayStatus(component,EIconDisplayState.HIDE);
            interactables.Remove(component);
        }

        public void ChangeIconDisplayStatus(IInteractable _interactable,EIconDisplayState _iconDisplayState)
        {
            switch (_iconDisplayState)
            {
                case EIconDisplayState.DISPLAY:
                    _interactable?.ShowInteractIcon();
                    break;
                case EIconDisplayState.HIDE:
                    _interactable?.HideInteractIcon();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_iconDisplayState), _iconDisplayState, null);
            }
        }

        public void ChangeIconDisplayStatus(EIconDisplayState _iconDisplayState)
        {
            foreach (var interactable in interactables)
            {
                ChangeIconDisplayStatus(interactable, _iconDisplayState);
            }
        }
    }

    public enum EIconDisplayState
    {
        DISPLAY,
        HIDE
    }
}