using UnityEngine;

namespace Scripts.MinigameSystem.Flowerbook
{
    public class LeafSpot : MonoBehaviour
    {
        [field: SerializeField] public int LeafID { get; private set; }
        [SerializeField] float snappingDistance = 1f;
        public bool IsFull { get; private set; }
        
        public bool MatchesLeaf(Leaf _leaf)
        {
            return _leaf.Id == LeafID && Vector3.Distance(_leaf.transform.position, transform.position) < snappingDistance;
        }

        public void ActivateFilled()
        {
            IsFull = true;
        }
    }
}