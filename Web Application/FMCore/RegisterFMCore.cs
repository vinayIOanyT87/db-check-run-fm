using FMCore.Interfaces;
using FMCore.Services;
using Unity;

namespace FMCore
{
    public static class RegisterFMCore
    {
        public static void RegisterFMCoreServices(this IUnityContainer fmService)
        {
            fmService.RegisterType<IFMConfigurationManager, FMConfigurationManager>();
            fmService.RegisterType<IFMCustomLogger, FakeLogger>();
        }
    }
}
