using UnityEngine.SceneManagement;

namespace Infrastructure.SceneLoad
{
    /// <summary>
    /// Class for loading scene
    /// </summary>
    public class SceneLoader : ISceneLoader
    {
        /// <summary>
        /// Load scene by name
        /// </summary>
        /// <param name="sceneName"></param>
        public void LoadScene(string sceneName) => 
            SceneManager.LoadSceneAsync(sceneName);
    }
}