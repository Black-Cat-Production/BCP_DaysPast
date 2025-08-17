using System;
using FMOD.Studio;
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
            FMODUnity.RuntimeManager.PauseAllEvents(true);
        }

        void OnDisable()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            playerController.Unpause();
            FMODUnity.RuntimeManager.PauseAllEvents(false);
        }

        public void Resume()
        {
            SceneManager.UnloadSceneAsync("PauseScene");
        }

        public void ReturnToMainMenu()
        {
            var masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
            masterBus.stopAllEvents(STOP_MODE.IMMEDIATE);
            FMODUnity.RuntimeManager.CoreSystem.update();
            FMODUnity.RuntimeManager.StudioSystem.update();
            mainMenuLoader.LoadScene();
            SceneManager.UnloadSceneAsync("PauseScene");
        }
    }
}