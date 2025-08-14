using Scripts.Scriptables.SceneLoader;
using UnityEngine;

namespace Scripts.UI.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        [Header("MenuGroups")]
        [SerializeField] CanvasGroup mainMenuGroup;
        [SerializeField] CanvasGroup optionsMenuGroup;
        
        [SerializeField] SceneLoader gameLoader;

        public void OpenOptions()
        {
            mainMenuGroup.gameObject.SetActive(false);
            optionsMenuGroup.gameObject.SetActive(true);
        }

        public void OpenMainMenu()
        {
            mainMenuGroup.gameObject.SetActive(true);
            optionsMenuGroup.gameObject.SetActive(false);
        }

        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        public void Play()
        {
            gameLoader.LoadScene();
        }
    }
}