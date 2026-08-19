namespace FMBusinessServices.InternalClasses
{
    using System;
    using System.Configuration;

    using FMBusinessObjects.Exceptions;

    using InternalInterfaces;

    internal class PointServiceInfoGetter : IPointServiceInfoGetter
    {
        private static Lazy<PointServiceInfo> pointServiceInfo 
            = new Lazy<PointServiceInfo>(CreateInfo);

        public PointServiceInfo Info { get { return pointServiceInfo.Value; } }

        private static PointServiceInfo CreateInfo()
        { 
            var info = new PointServiceInfo();

            info.PointServiceBindingType = ConfigurationManager.AppSettings["FMPointServiceBindingType"];
            if (string.IsNullOrEmpty(info.PointServiceBindingType))
            {
                throw new ConfigurationErrorException("FMPointServiceBindingType not found in configuration");
            }

            info.PointServiceBindingConfiguration = ConfigurationManager.AppSettings["FMPointServiceBindingConfiguration"];
            if (string.IsNullOrEmpty(info.PointServiceBindingConfiguration))
            {
                throw new ConfigurationErrorException("FMPointServiceBindingConfiguration not found in configuration");
            }

            info.PointServiceBindingEndPointAddress = ConfigurationManager.AppSettings["FMPointServiceBindingEndPointAddress"];
            if (string.IsNullOrEmpty(info.PointServiceBindingEndPointAddress))
            {
                throw new ConfigurationErrorException("FMPointServiceBindingEndPointAddress not found in configuration");
            }

            return info;
        }

        public void Refresh()
        {
            pointServiceInfo = new Lazy<PointServiceInfo>(CreateInfo);
        }
    }
}
