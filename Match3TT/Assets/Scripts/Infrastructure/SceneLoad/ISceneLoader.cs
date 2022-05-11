namespace Infrastructure.SceneLoad
{
    /// <summary>
    /// SceneLoader abstraction
    /// </summary>
    public interface ISceneLoader
    {
        void LoadScene(string sceneName);
    }
}