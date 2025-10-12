using System;
using System.Collections;
using System.Threading.Tasks;
using cfEngine.Logging;
using cfUnityEngine.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace cfUnityEngine.SceneManagement
{
    public class UnitySceneManager: MonoInstance<UnitySceneManager>, ISceneManager
    {
        public override bool dontDestroyOnLoad => true;

        public bool LoadScene(string sceneKey, LoadSceneMode mode = LoadSceneMode.Single)
        {
            try
            {
                SceneManager.LoadScene(sceneKey, mode);
                return true;
            }
            catch (Exception e)
            {
                Log.LogException(e);
                return false;
            }
        }

        public Task LoadSceneAsync(string sceneKey, LoadSceneMode mode = LoadSceneMode.Single, IProgress<float> progress = null)
        {
            TaskCompletionSource<AsyncOperation> loadTaskSource = new TaskCompletionSource<AsyncOperation>();
            
            StartCoroutine(_loadSceneAsync(loadTaskSource, sceneKey, mode, progress));
            
            return loadTaskSource.Task;
        }

        public Task UnloadSceneAsync(string sceneKey)
        {
            var aop = SceneManager.UnloadSceneAsync(sceneKey);
            return aop.ToTask();
        }

        private IEnumerator _loadSceneAsync(TaskCompletionSource<AsyncOperation> loadTaskSource, string sceneKey, LoadSceneMode mode = LoadSceneMode.Single, IProgress<float> progress = null)
        {
            var asyncOp = SceneManager.LoadSceneAsync(sceneKey, mode);
            asyncOp.allowSceneActivation = true;
            while (!asyncOp.isDone)
            {
                if (progress != null)
                {
                    progress.Report(asyncOp.progress);
                }

                yield return null;
            }
            
            if (asyncOp.progress >= 0.9f)
            {
                asyncOp.allowSceneActivation = true;
                loadTaskSource.SetResult(asyncOp);
            }
            else
            {
                loadTaskSource.SetException(new Exception($"[UnitySceneManager] Load scene {sceneKey} failed with progress: {asyncOp.progress}"));
            }
        }
        
        public Scene GetActiveScene()
        {
            return SceneManager.GetActiveScene();
        }

        public Scene GetScene(string sceneName)
        {
            return SceneManager.GetSceneByName(sceneName);
        }

        public void Dispose()
        {
        }
    }
}