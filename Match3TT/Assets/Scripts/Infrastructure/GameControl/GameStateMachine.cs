using System;
using System.Collections.Generic;
using Generators;
using Infrastructure.Bootstrapper;
using Infrastructure.SceneLoad;
using Infrastructure.States;

namespace Infrastructure.GameControl
{
    public class GameStateMachine
    {
        private Dictionary<Type, IState> _states;
        private IState activeState;

        public GameStateMachine(ICoroutineRunner coroutineRunner, ISceneLoader sceneLoader, IBallSet ballSet)
        {
            _states = new Dictionary<Type, IState>
            {
                [typeof(BootstrapState)] = new BootstrapState(this, coroutineRunner, sceneLoader),
                [typeof(MenuState)] = new MenuState(this, coroutineRunner, sceneLoader),
                [typeof(LevelState)] = new LevelState(this, coroutineRunner, sceneLoader, ballSet)
            };
        }

        public void EnterState<T>() where T : IState
        {
            activeState?.Exit();

            var state = GetState<T>();
            state.Enter();

            activeState = state;
        }

        private IState GetState<T>() where T : IState => 
            _states[typeof(T)];
    }
}