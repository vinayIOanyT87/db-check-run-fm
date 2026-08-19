namespace FuelsManager.Afss.ServiceProcess
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;

    using FuelsManager.Afss.Module.Gasboy.ServiceProcess;
    using FuelsManager.Afss.Module.Gasboy.ServiceProcess.ServiceClasses;

    public class ServiceManager
    {
        private readonly Dictionary<string, ServiceHost> ServiceHostList = new Dictionary<string, ServiceHost>();
  
        public void OpenAll()
        {                
            // Start each of the registered Processors
            GasboyServiceProcess.StartProcessThread();

            this.OpenHost<GasboyStationServices>(GasboyStationServices.ServiceName);
            // Register other services here
        }  
  
        public void CloseAll()
        {
            foreach (KeyValuePair<string, ServiceHost> serviceHost in this.ServiceHostList)
            {
                serviceHost.Value.Close();
            }

            this.ServiceHostList.Clear();

            GasboyServiceProcess.StopProcessThread();
        }  
  
        private void OpenHost<TService>(string serviceName)
        {
            Type type = typeof(TService);

            var serviceHost = new ServiceHost(type);  
            serviceHost.Open();
            this.ServiceHostList.Add(serviceName, serviceHost);
        }  
    }  
}
