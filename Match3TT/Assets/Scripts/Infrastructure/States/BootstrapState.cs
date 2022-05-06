using System.Collections;
using Infrastructure.Bootstrapper;
using Infrastructure.GameControl;
using Infrastructure.SceneLoad;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Infrastructure.States
{
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

        public void Enter()
        {
            sceneLoader.LoadScene(SceneNames.MenuScene);

            coroutineRunner.StartCoroutine(EnterMenuState());
        }

        public void Exit() { }

        private IEnumerator EnterMenuState()
        {
            while (SceneManager.GetActiveScene().name != SceneNames.MenuScene)
                yield return null;
            
            gameStateMachine.EnterState<MenuState>();
        }
    }
}