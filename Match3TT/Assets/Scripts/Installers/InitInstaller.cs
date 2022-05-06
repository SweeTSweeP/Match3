using Balls;
using Generators;
using Infrastructure.SceneLoad;
using Zenject;

namespace Installers
{
    public class InitInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
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
    }
}