using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts_v2.Scene
{
    public class SceneTransitionManager : MonoBehaviour
    {
        public void LoadSceneByIndex(int sceneIndex)
        {
            StartCoroutine(LoadSceneByIndexRoutine(sceneIndex));
        }
        
        public void AutomaticallyLoadNextScene()
        {
            StartCoroutine(LoadNextSceneRoutine());
        }

        private IEnumerator LoadSceneByIndexRoutine(int sceneIndex)
        {
            var operation = SceneManager.LoadSceneAsync(sceneIndex);
            if (operation == null) yield break;
            operation.allowSceneActivation = false;

            yield return operation.isDone;

            operation.allowSceneActivation = true;
        }
        
        private IEnumerator LoadNextSceneRoutine()
        {
            var operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
            if (operation == null) yield break;
            operation.allowSceneActivation = false;

            yield return operation.isDone;

            operation.allowSceneActivation = true;
        }
    }
}