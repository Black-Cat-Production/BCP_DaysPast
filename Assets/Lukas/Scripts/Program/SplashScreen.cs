using System;
using Scripts.Scriptables.SceneLoader;
using UnityEngine;
using UnityEngine.Video;

namespace Scripts.Program
{
    public class SplashScreen : MonoBehaviour
    {
        
        VideoPlayer videoPlayer;
        [SerializeField]SceneLoader mainMenuLoader;


        void Awake()
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        void Start()
        {
            videoPlayer.loopPointReached += _source => mainMenuLoader.LoadScene();
            videoPlayer.Play();
        }
        
    }
}