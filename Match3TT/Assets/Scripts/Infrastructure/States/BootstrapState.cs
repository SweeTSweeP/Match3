using System.Collections;
using Infrastructure.Bootstrapper;
using Infrastructure.GameControl;
using Infrastructure.SceneLoad;
using UnityEngine.SceneManagement;

namespace Infrastructure.States
{
    /// <summary>
    /// First state to setup dependencies before game
    /// </summary>
    public class BootstrapState : IState
    {
        private readonly GameStateMachine gameStateMachine;
        private readonly ISceneLoader sceneLoader;
        private readonly ICoroutineRunner coroutineRunner;

        public BootstrapState(
            GameStateMachine gameStateMachine, 
            ICoroutineRunner coroutineRunner, 
            ISceneLoader sceneLoader)
        {
            this.gameStateMachine = gameStateMachine;
            this.sceneLoader = sceneLoader;
            this.coroutineRunner = coroutineRunner;
        }

        /// <summary>
        /// Enter to state
        /// </summary>
        public void Enter()
        {
            sceneLoader.LoadScene(SceneNames.MenuScene);

            coroutineRunner.StartCoroutine(EnterMenuState());
        }

        public void Exit() { }

        /// <summary>
        /// Enter to next state 
        /// </summary>
        /// <returns></returns>
        private IEnumerator EnterMenuState()
        {
            while (SceneManager.GetActiveScene().name != SceneNames.MenuScene)
                yield return null;
            
            gameStateMachine.EnterState<MenuState>();
        }
    }
}