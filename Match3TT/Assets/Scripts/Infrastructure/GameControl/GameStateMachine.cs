using System;
using System.Collections.Generic;
using Generators;
using Infrastructure.Bootstrapper;
using Infrastructure.SceneLoad;
using Infrastructure.States;

namespace Infrastructure.GameControl
{
    /// <summary>
    /// Controller of states to rule the application 
    /// </summary>
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

        /// <summary>
        /// Enter to new state
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void EnterState<T>() where T : IState
        {
            activeState?.Exit();

            var state = GetState<T>();
            state.Enter();

            activeState = state;
        }

        /// <summary>
        /// Find state in dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Return found state</returns>
        private IState GetState<T>() where T : IState => 
            _states[typeof(T)];
    }
}