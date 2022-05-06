using Balls;
using Generators;
using Infrastructure.Bootstrapper;
using Infrastructure.SceneLoad;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Installers
{
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

        private void BindSceneLoader() => 
            Container.Bind<ISceneLoader>().To<SceneLoader>().AsSingle();

        private void BindBallSet() =>
            Container.Bind<IBallSet>().To<BallSet>().AsSingle();

        private void BindBallSwapper() => 
            Container.Bind<IBallSwapper>().To<BallSwapper>().AsSingle();

        private void BindCoroutineRunner() => 
            Container.Bind<ICoroutineRunner>().FromInstance(coroutineRunner).AsSingle();
    }
}