using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Resources.Scripts
{
    public class SceneTransition : MonoBehaviour
    {
        private bool _isLoading;

        private static SceneTransition _instance;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        public void Restart()
        {
            if (!_isLoading)
                StartCoroutine(LoadSceneRoutine(SceneManager.GetActiveScene().name));
        }

        public void LoadScene(string sceneName)
        {
            if (!_isLoading)
                StartCoroutine(LoadSceneRoutine(sceneName));
        }

        private IEnumerator LoadSceneRoutine(string sceneName)
        {
            _isLoading = true;

            var waitFading = true;
            Fader.Instance.FadeIn(() => waitFading = false);
            
            while (waitFading) 
                yield return null;
            
            var async = SceneManager.LoadSceneAsync(sceneName);
            async.allowSceneActivation = false;

            while (async.progress < 0.9f)
            {
                Debug.Log(async.progress);
                yield return null;
            }
            
            async.allowSceneActivation = true;

            waitFading = true;
            Fader.Instance.FadeOut(() => waitFading = false);

            while (waitFading)
                yield return null;
            Debug.Log(42);
            _isLoading = false;
        }
    }
}
