using UnityEngine;

namespace Lukas.Scripts
{
    public class SwapBlocker
    {
        public bool GetSwapBlocked(Transform _transform, LayerMask _blockingLayer)
        {
            
            var colliders = Physics.OverlapSphere(_transform.position, 1f, _blockingLayer);
            foreach (var collider in colliders)
            {
                Debug.Log("Found hidden collider: " + collider.name);
            }
            return colliders.Length > 0;
        }
    }
}