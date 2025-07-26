using System;
using UnityEngine;

namespace Scripts.Program
{
    public class Startup : MonoBehaviour
    {
        void Start()
        {
            Screen.SetResolution(1920, 1080, true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}