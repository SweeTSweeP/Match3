﻿using System.Collections;
using Infrastructure.Bootstrapper;
using Infrastructure.GameControl;
using Infrastructure.SceneLoad;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Infrastructure.States
{
    public class MenuState : IState
    {
        private readonly GameStateMachine gameStateMachine;
        private readonly ICoroutineRunner coroutineRunner;
        private readonly ISceneLoader sceneLoader;

        public MenuState(GameStateMachine gameStateMachine, ICoroutineRunner coroutineRunner, ISceneLoader sceneLoader)
        {
            this.gameStateMachine = gameStateMachine;
            this.coroutineRunner = coroutineRunner;
            this.sceneLoader = sceneLoader;
        }

        public void Enter() => 
            coroutineRunner.StartCoroutine(OnClickPlayButton());

        public void Exit() { }

        private IEnumerator OnClickPlayButton()
        {
            while (SceneManager.GetActiveScene().name != SceneNames.MenuScene)
                yield return null;

            var playButton = GameObject.FindWithTag(Constants.PlayButton)?.GetComponent<Button>();
            
            if (playButton != null)
                playButton.onClick.AddListener(() =>
                {
                    gameStateMachine.EnterState<LevelState>();
                    sceneLoader.LoadScene(SceneNames.MainScene);
                });
        }
    }
}