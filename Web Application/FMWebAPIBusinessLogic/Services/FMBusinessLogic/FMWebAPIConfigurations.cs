using FMCore.Interfaces;
using FMWebAPIBusinessLogic.DTO;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;

namespace FMWebAPIBusinessLogic.Services.FMBusinessLogic
{
    public class FMWebAPIConfigurationFactory : IFMWebAPIConfigurationFactory
    {
        private readonly IFMConfigurationManager _configManager;
        public FMWebAPIConfigurationFactory(IFMConfigurationManager configManager)
        {
            _configManager = configManager;
        }

        public FMWebAPIConfiguration GetConfig()
        {
            var result = new FMWebAPIConfiguration();
            int parsedTimeoutResult;
            if (int.TryParse(_configManager.Get("FuelsManagerSessionTimeoutMinutes"), out parsedTimeoutResult))
            {
                result.FuelsManagerSessionTimeoutMinutes = parsedTimeoutResult;
            }
            else
            {
                result.FuelsManagerSessionTimeoutMinutes = 20;
            }

            bool parsedAJAXResult;
            if (bool.TryParse(_configManager.Get("EnableAJAXTransactionEntry"), out parsedAJAXResult))
            {
                result.EnableAJAXTransactionEntry = parsedAJAXResult;
            }
            else
            {
                result.EnableAJAXTransactionEntry = false;
            }
            return result;
        }
    }
}
