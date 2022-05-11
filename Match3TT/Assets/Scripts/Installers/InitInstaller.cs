using Balls;
using Generators;
using Infrastructure.Bootstrapper;
using Infrastructure.SceneLoad;
using UnityEngine;
using Zenject;

namespace Installers
{
    /// <summary>
    /// Zenject dependencies installer
    /// </summary>
    public class InitInstaller : MonoInstaller
    {
        [SerializeField] private Bootstrap coroutineRunner;
        
        public override void InstallBindings()
        {
            BindCoroutineRunner();
            BindSceneLoader();
            BindBallSwapper();
            BindBallSet();
        }

        /// <summary>
        /// Register scene loader
        /// </summary>
        private void BindSceneLoader() => 
            Container.Bind<ISceneLoader>().To<SceneLoader>().AsSingle();

        /// <summary>
        /// Register ball set
        /// </summary>
        private void BindBallSet() =>
            Container.Bind<IBallSet>().To<BallSet>().AsSingle();

        /// <summary>
        /// Register ball swapper
        /// </summary>
        private void BindBallSwapper() => 
            Container.Bind<IBallSwapper>().To<BallSwapper>().AsSingle();

        /// <summary>
        /// Register coroutine runner
        /// </summary>
        private void BindCoroutineRunner() => 
            Container.Bind<ICoroutineRunner>().FromInstance(coroutineRunner).AsSingle();
    }
}