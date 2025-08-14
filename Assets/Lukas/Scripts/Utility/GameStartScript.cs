using System;
using UnityEngine;

namespace Scripts.Utility
{
    public class GameStartScript : MonoBehaviour
    {
        BlackoutTransition blackoutTransition;

        void Awake()
        {
            blackoutTransition = GetComponent<BlackoutTransition>();
        }

        void Start()
        {
            StartCoroutine(blackoutTransition.TransitionFromBlackout());
        }
    }
}