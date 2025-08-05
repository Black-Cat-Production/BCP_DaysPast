using UnityEngine;

namespace Scripts.Utility
{
    public class BillboardImage : MonoBehaviour
    {
        void LateUpdate()
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}