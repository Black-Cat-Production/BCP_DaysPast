using System;
using Scripts.Movement;
using Scripts.Scriptables.SceneLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.UI.PauseMenu
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] SceneLoader mainMenuLoader;

        PlayerController playerController;
        void OnEnable()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            playerController = FindObjectOfType<PlayerController>();
        }

        void OnDisable()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            playerController.IsPaused = false;
        }

        public void Resume()
        {
            SceneManager.UnloadSceneAsync("PauseScene");
        }

        public void ReturnToMainMenu()
        {
            mainMenuLoader.LoadScene();
            SceneManager.UnloadSceneAsync("PauseScene");
        }
    }
}