using Scripts.Scriptables.SceneLoader;
using UnityEngine;

namespace Scripts.InteractionSystem
{
    public class HubInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] SceneLoader levelLoader;
        public void Interact()
        {
            levelLoader.LoadScene();
        }
    }
}