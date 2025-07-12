using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Scriptables.SceneLoader
{
    [CreateAssetMenu(fileName = "NewSceneLoader", menuName = "Scriptables/SceneLoader")]
    public class SceneLoader : ScriptableObject
    {
        [SerializeField] EScenes sceneToLoad;
        public void LoadScene()
        {
            SceneManager.LoadScene((int)sceneToLoad);
        }
    }
}