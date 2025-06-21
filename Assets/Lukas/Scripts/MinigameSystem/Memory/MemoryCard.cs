using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.MinigameSystem.Memory
{
    public class MemoryCard : MonoBehaviour, IPointerClickHandler
    {
        [field: SerializeField] public int Id {get; private set;}

        public void TurnFaceUp()
        {
            
        }
        public void TurnFaceDown()
        {
            
        }

        public void RemoveFromBoard()
        {
            
        }

        public void OnPointerClick(PointerEventData _eventData)
        {
            throw new System.NotImplementedException();
        }
    }
}