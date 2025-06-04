using UnityEngine;

namespace Scripts.MinigameSystem
{
    public abstract class Minigame : MonoBehaviour
    {
        public abstract void Play();
        protected abstract void EndGame();
        protected abstract void OpenUI();
    }
}