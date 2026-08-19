using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Unity;

namespace FuelsManager.Services
{

    public static class FMWebAPIServiceLocator
    {
        public static IUnityContainer Container => ((UnityResolver)DependencyResolver.Current).Container;

        static public T GetInstance<T>()
        {
            return (T)System.Web.Mvc.DependencyResolver.Current.GetService(typeof(T));
        }
    }

    public class UnityResolver : IDependencyResolver
    {
        public IUnityContainer Container { get; set; }

        public UnityResolver(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            this.Container = container;
        }

        public object GetService(Type serviceType)
        {
            if (!Container.IsRegistered(serviceType))
            {
                if (serviceType.IsAbstract || serviceType.IsInterface)
                {
                    return null;
                }

                // Check to see if the controller is valid.  Return null if it is not.
                if (serviceType.Name.Equals("FileNotFoundController"))
                {
                    return null;
                }
            }

            return this.Container.Resolve(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return this.Container.ResolveAll(serviceType);
        }

        public void Dispose()
        {
            this.Container.Dispose();
        }
    }
}