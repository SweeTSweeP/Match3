using System.Collections;
using Generators;
using Infrastructure.Bootstrapper;
using Infrastructure.GameControl;
using Infrastructure.SceneLoad;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Infrastructure.States
{
    /// <summary>
    /// Main state to load game field
    /// </summary>
    public class LevelState : IState
    {
        private readonly GameStateMachine gameStateMachine;
        private readonly ICoroutineRunner coroutineRunner;
        private readonly ISceneLoader sceneLoader;
        private readonly IBallSet ballSet;

        public LevelState(
            GameStateMachine gameStateMachine, 
            ICoroutineRunner coroutineRunner,
            ISceneLoader sceneLoader,
            IBallSet ballSet)
        {
            this.gameStateMachine = gameStateMachine;
            this.coroutineRunner = coroutineRunner;
            this.sceneLoader = sceneLoader;
            this.ballSet = ballSet;
        }

        /// <summary>
        /// Enter to state
        /// </summary>
        public void Enter() => 
            coroutineRunner.StartCoroutine(StateEnter());

        public void Exit() { }
        
        /// <summary>
        /// Entering to state
        /// </summary>
        /// <returns></returns>
        private IEnumerator StateEnter()
        {
            while (SceneManager.GetActiveScene().name != SceneNames.MainScene)
                yield return null;
            
            ballSet.SetBalls();
            OnClickMenuButton();
        }

        /// <summary>
        /// Add button listener to enter other state
        /// </summary>
        private void OnClickMenuButton()
        {
            var playButton = GameObject.FindWithTag(Constants.MenuButton)?.GetComponent<Button>();
            
            if (playButton != null)
                playButton.onClick.AddListener(() =>
                {
                    gameStateMachine.EnterState<MenuState>();
                    sceneLoader.LoadScene(SceneNames.MenuScene);
                });
        }
    }
}