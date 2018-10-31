using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.ViewModels;
using Grpc.Core.Logging;
using CarHunters.Core.Common.Services;
using Grpc.Core;

namespace CarHunters.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();
            Mvx.IoCProvider.RegisterType<ILogger, DebugLogger>();

            GrpcEnvironment.SetLogger(Mvx.IoCProvider.Resolve<ILogger>());
            CreateDefaultViewModelLocator();
            RegisterCustomAppStart<AppStart>();
        }

        protected override IMvxViewModelLocator CreateDefaultViewModelLocator()
        {
            // register the instance
            if (!Mvx.IoCProvider.CanResolve<IMvxViewModelLocator>())
            {
                var locator = base.CreateDefaultViewModelLocator();
                Mvx.IoCProvider.RegisterSingleton(locator);
            }

            return Mvx.IoCProvider.Resolve<IMvxViewModelLocator>();
        }
    }
}
