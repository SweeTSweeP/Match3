using UnityEngine.SceneManagement;

namespace Infrastructure.SceneLoad
{
    public class SceneLoader : ISceneLoader
    {
        public void LoadScene(string sceneName) => 
            SceneManager.LoadSceneAsync(sceneName);
    }
}