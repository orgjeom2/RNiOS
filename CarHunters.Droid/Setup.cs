using System;
using CarHunters.Core;
using MvvmCross.Converters;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.IoC;

namespace CarHunters.Droid
{
    public class Setup : MvxAppCompatSetup<App>
    {
        protected override void InitializePlatformServices()
        {
            base.InitializePlatformServices();

            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();
        }

        protected override void FillValueConverters(IMvxValueConverterRegistry registry)
        {
            base.FillValueConverters(registry);
            foreach (var assembly in ValueConverterAssemblies)
                foreach (var item in assembly.CreatableTypes().EndingWith("Converter"))
                    registry.AddOrOverwrite(item.Name, (IMvxValueConverter)Activator.CreateInstance(item));
        }
    }
}
