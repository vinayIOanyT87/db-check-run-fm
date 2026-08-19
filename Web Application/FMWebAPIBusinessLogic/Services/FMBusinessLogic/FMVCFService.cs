using FMCore.Interfaces;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using System.Linq;
using Varec.CommonComponents.EngineeringUnitsLibrary;
using Varec.CommonComponents.VolumeCorrection;

namespace FMWebAPIBusinessLogic.Services.FMBusinessLogic
{
    public class FMVCFService : IFMVCFService
    {
        private readonly IFMCustomLogger _logger;
        private readonly IProductsProxy _productProxy;
        private readonly ICurrentRequestContext _currentRequestContext;

        public FMVCFService(IFMCustomLogger logger,
            IProductsProxy productProxy,
            ICurrentRequestContext currentRequestContext)
        {
            this._logger = logger;
            this._productProxy = productProxy;
            this._currentRequestContext = currentRequestContext;
        }

        public double CalculateStandardDensityFromObserved(ECorrectionTypeMajor majorMethod,
            ECorrectionTypeMinor minorMethod, double measuredTemperature,
            EngineeringUnit measuredTemperatureUnits, double standardTemperature,
            EngineeringUnit standardTemperatureUnits, double measuredDensity,
            EngineeringUnit densityUnits, double measuredPressure,
            EngineeringUnit pressureUnits, double alternateTemperature,
            EngineeringUnit alternateTemperatureUnits, double alternatePressure,
            EngineeringUnit alternatePressureUnits, double[] kFactors)
        {
            this._logger.Debug("Passed in parameters for caculating for VCF from observed: {@PassedInParameters}",
                new
                {
                    majorMethod,
                    minorMethod,
                    measuredTemperature,
                    measuredTemperatureUnits,
                    standardTemperature,
                    standardTemperatureUnits,
                    measuredDensity,
                    densityUnits,
                    measuredPressure,
                    pressureUnits,
                    alternateTemperature,
                    alternateTemperatureUnits,
                    alternatePressure,
                    alternatePressureUnits,
                    kFactors
                });
            var result = Vcf
                .CalculateStandardDensityFromObserved(
                majorMethod: majorMethod,
                minorMethod: minorMethod,
                measuredTemperature: measuredTemperature,
                measuredTemperatureUnits: measuredTemperatureUnits,
                standardTemperature: standardTemperature,
                standardTemperatureUnits: standardTemperatureUnits,
                measuredDensity: measuredDensity,
                densityUnits: densityUnits,
                measuredPressure: measuredPressure,
                pressureUnits: pressureUnits,
                alternateTemperature: alternateTemperature,
                alternateTemperatureUnits: alternateTemperatureUnits,
                alternatePressure: alternatePressure,
                alternatePressureUnits: alternatePressureUnits,
                kFactors: kFactors
                );
            return result;
        }

        public double CalculateVcf(ECorrectionTypeMajor majorMethod, ECorrectionTypeMinor minorMethod,
            double measuredTemperature, EngineeringUnit measuredTemperatureUnits,
            double standardTemperature, EngineeringUnit standardTemperatureUnits,
            double standardDensity, EngineeringUnit standardDensityUnits,
            double measuredPressure, EngineeringUnit pressureUnits,
            double alternateTemperature, EngineeringUnit alternateTemperatureUnits,
            double alternatePressure, EngineeringUnit alternatePressureUnits,
            double[] kFactors)
        {
            this._logger.Debug("Passed in parameters for calculating for VCF: {@PassedInParameters}",
                new
                {
                    majorMethod,
                    minorMethod,
                    measuredTemperature,
                    measuredTemperatureUnits,
                    standardTemperature,
                    standardTemperatureUnits,
                    standardDensity,
                    standardDensityUnits,
                    measuredPressure,
                    pressureUnits,
                    alternateTemperature,
                    alternateTemperatureUnits,
                    alternatePressure,
                    alternatePressureUnits,
                    kFactors
                });
            var result = Vcf
                   .CalculateVcf(
                   majorMethod: majorMethod,
                   minorMethod: minorMethod,
                   measuredTemperature: measuredTemperature,
                   measuredTemperatureUnits: measuredTemperatureUnits,
                   standardTemperature: standardTemperature,
                   standardTemperatureUnits: standardTemperatureUnits,
                   standardDensity: standardDensity,
                   standardDensityUnits: standardDensityUnits,
                   measuredPressure: measuredPressure,
                   pressureUnits: pressureUnits,
                   alternateTemperature: alternateTemperature,
                   alternateTemperatureUnits: alternateTemperatureUnits,
                   alternatePressure: alternatePressure,
                   alternatePressureUnits: alternatePressureUnits,
                   kFactors: kFactors
                   );
            return result;
        }

        public double GetVCFForProductBasedOnUserForAviation(string productId, double temperature, double density)
        {
            this._logger.Debug("GetVCFForProductBasedOnUserForAviation passed in parameter: {@PassedInParameters}", new { productId, temperature, density });
            var userSecurity = this._currentRequestContext.GetCurrentSecurityContext();
            var site = this._currentRequestContext.GetCurrentSite();
            var products = this._productProxy.Enumerate();
            var product = products.First(x => x.ID.ToLower() == productId.ToLower());

            var result = this.CalculateVcf(
                majorMethod: (ECorrectionTypeMajor)System.Convert.ToInt32(product._VcfModuleSettings.CorrectionMethodType),
                minorMethod: (ECorrectionTypeMinor)System.Convert.ToInt32(product._VcfModuleSettings.CorrectionMethodSpecific),
                measuredTemperature: temperature,
                measuredTemperatureUnits: site.TemperatureUnits,
                standardTemperature: product._VcfModuleSettings.BaseTemperature.Value,
                standardTemperatureUnits: (EngineeringUnit)product._VcfModuleSettings.BaseTemperature.EngineeringUnitsType,
                standardDensity: density,
                standardDensityUnits: site.DensityUnits,
                measuredPressure: 0.0,
                pressureUnits: site.PressureUnits,
                alternateTemperature: product._VcfModuleSettings.AlternateTemperature.Value,
                alternateTemperatureUnits: (EngineeringUnit)product._VcfModuleSettings.AlternateTemperature.EngineeringUnitsType,
                alternatePressure: product._VcfModuleSettings.AlternateBasePressure.Value,
                alternatePressureUnits: (EngineeringUnit)product._VcfModuleSettings.AlternateBasePressure.EngineeringUnitsType,
                kFactors: new[] { product._VcfModuleSettings.K[0], product._VcfModuleSettings.K[1], product._VcfModuleSettings.K[2], product._VcfModuleSettings.K[3], product._VcfModuleSettings.K[4] }
            );

            return result;
        }
    }
}
