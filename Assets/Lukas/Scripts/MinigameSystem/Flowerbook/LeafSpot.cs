using UnityEngine;

namespace Scripts.MinigameSystem.Flowerbook
{
    public class LeafSpot : MonoBehaviour
    {
        [field: SerializeField] public int LeafID { get; private set; }
        [SerializeField] float snappingDistance = 1f;
        [SerializeField] float snappingAngle = 10f;
        public bool IsFull { get; private set; }
        
        public bool MatchesLeaf(Leaf _leaf)
        {
            return _leaf.Id == LeafID && Vector3.Distance(_leaf.transform.position, transform.position) < snappingDistance && AreRotationalClose(_leaf.transform.rotation, this.transform.rotation, snappingAngle);
        }

        bool AreRotationalClose(Quaternion _a, Quaternion _b, float _thresholdDegrees)
        {
            float angleA = _a.eulerAngles.z;
            float angleB = _b.eulerAngles.z;
            
            float delta = Mathf.Abs(Mathf.DeltaAngle(angleA, angleB));
            return delta <= _thresholdDegrees;
        }

        public void ActivateFilled()
        {
            IsFull = true;
        }
    }
}