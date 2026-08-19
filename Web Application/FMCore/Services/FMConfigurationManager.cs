using FMCore.Interfaces;
using System.Configuration;

namespace FMCore.Services
{
    public class FMConfigurationManager : IFMConfigurationManager
    {
        public string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
