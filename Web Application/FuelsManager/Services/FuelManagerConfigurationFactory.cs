using FMCore.Interfaces;
using FuelsManager.DTO;
using FuelsManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FuelsManager.Services
{
    public class FuelManagerConfigurationFactory : IFuelManagerConfigurationFactory
    {
        IFMConfigurationManager _configManager;
        public FuelManagerConfigurationFactory(IFMConfigurationManager configManager)
        {
            this._configManager = configManager;
        }

        public FuelManagerConfiguration GetConfig()
        {
            var result = new FuelManagerConfiguration();
            bool parsedAjaxScreen;
            if (bool.TryParse(_configManager.Get("UseNewTransactionEntryScreen"),out parsedAjaxScreen))
            {
                result.EnableAjaxTransactionScreen = parsedAjaxScreen;
            }
            return result;
        }
    }
}