using Generators;
using Infrastructure.GameControl;
using Infrastructure.SceneLoad;
using Infrastructure.States;
using UnityEngine;
using Zenject;

namespace Infrastructure.Bootstrapper
{
    /// <summary>
    /// Application entry point
    /// </summary>
    public class Bootstrap : MonoBehaviour, ICoroutineRunner
    {
        private ISceneLoader _sceneLoader;
        private IBallSet _ballSet;

        [Inject]
        private void Construct(ISceneLoader sceneLoader, IBallSet ballSet)
        {
            _sceneLoader = sceneLoader;
            _ballSet = ballSet;
        }

        private void Awake()
        {
            var gameStateMachine = new GameStateMachine(this, _sceneLoader, _ballSet);
            gameStateMachine.EnterState<BootstrapState>();
            
            DontDestroyOnLoad(this);
        }
    }
}