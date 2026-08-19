using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace FMDepedencyManager
{
    public static class FMServiceLocator
    {
        static public IUnityContainer Container { get; set; }

        static public T GetInstance<T>()
        {
            if (FMServiceLocator.Container == null)
            {
                throw new ApplicationException("The service locator has not initialized");
            }
            return FMServiceLocator.Container.Resolve<T>();
        }
    }
}
