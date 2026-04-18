using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomsSim.Core
{
    /// <summary>Thin wrapper around SceneManager with a completion callback.</summary>
    public static class SceneLoader
    {
        public static void Load(string sceneName, Action onLoaded = null)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                throw new ArgumentException("Scene name must not be empty", nameof(sceneName));
            }

            var runner = CoroutineHost.Ensure();
            runner.StartCoroutine(LoadRoutine(sceneName, onLoaded));
        }

        private static IEnumerator LoadRoutine(string sceneName, Action onLoaded)
        {
            var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            if (op == null)
            {
                Debug.LogError($"SceneLoader: scene '{sceneName}' not found in build settings.");
                yield break;
            }

            while (!op.isDone)
            {
                yield return null;
            }

            onLoaded?.Invoke();
        }

        private sealed class CoroutineHost : MonoBehaviour
        {
            private static CoroutineHost _instance;

            public static CoroutineHost Ensure()
            {
                if (_instance != null) return _instance;
                var go = new GameObject("[SceneLoaderHost]");
                _instance = go.AddComponent<CoroutineHost>();
                UnityEngine.Object.DontDestroyOnLoad(go);
                return _instance;
            }
        }
    }
}
