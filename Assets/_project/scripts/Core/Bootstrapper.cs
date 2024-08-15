using System;
using _project.scripts.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _project.scripts.Core
{
    public class Bootstrapper : MonoBehaviour
    {
        
        private void Start()
        {
#if UNITY_EDITOR
            Application.runInBackground = true;
            
            if (SceneManager.loadedSceneCount == 1)
            {
                SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
            }

            GetComponent<NetworkButtons>().enabled = SceneManager.GetSceneAt(1).name == "Gameplay";
#endif
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
#if UNITY_EDITOR
            var currentlyLoadedEditorScene = SceneManager.GetActiveScene();
#endif
            if (!SceneManager.GetSceneByName("Bootstrapper").isLoaded)
            {
                SceneManager.LoadScene("Bootstrapper");
            }

#if UNITY_EDITOR
            if (currentlyLoadedEditorScene.IsValid())
                SceneManager.LoadSceneAsync(currentlyLoadedEditorScene.name, LoadSceneMode.Additive);
#else
            SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
#endif
        }
    }
}
