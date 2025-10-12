using System;
using System.Threading.Tasks;
using cfEngine.Service;
using UnityEngine.SceneManagement;

namespace cfUnityEngine.SceneManagement
{
    public interface ISceneManager: IService
    {
        public bool LoadScene(string sceneKey, LoadSceneMode mode = LoadSceneMode.Single);
        public Task LoadSceneAsync(string sceneKey, LoadSceneMode mode = LoadSceneMode.Single, IProgress<float> progress = null);
        public Task UnloadSceneAsync(string sceneKey);
        public Scene GetActiveScene();
        public Scene GetScene(string sceneName);
    }
}