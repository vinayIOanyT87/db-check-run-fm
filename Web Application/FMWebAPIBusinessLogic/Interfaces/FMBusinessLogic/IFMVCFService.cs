using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Varec.CommonComponents.EngineeringUnitsLibrary;
using Varec.CommonComponents.VolumeCorrection;

namespace FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic
{
    public interface IFMVCFService
    {
        double CalculateStandardDensityFromObserved(ECorrectionTypeMajor majorMethod, ECorrectionTypeMinor minorMethod, double measuredTemperature, EngineeringUnit measuredTemperatureUnits, double standardTemperature, EngineeringUnit standardTemperatureUnits, double measuredDensity, EngineeringUnit densityUnits, double measuredPressure, EngineeringUnit pressureUnits, double alternateTemperature, EngineeringUnit alternateTemperatureUnits, double alternatePressure, EngineeringUnit alternatePressureUnits, double[] kFactors);
        double CalculateVcf(ECorrectionTypeMajor majorMethod, ECorrectionTypeMinor minorMethod, double measuredTemperature, EngineeringUnit measuredTemperatureUnits, double standardTemperature, EngineeringUnit standardTemperatureUnits, double standardDensity, EngineeringUnit standardDensityUnits, double measuredPressure, EngineeringUnit pressureUnits, double alternateTemperature, EngineeringUnit alternateTemperatureUnits, double alternatePressure, EngineeringUnit alternatePressureUnits, double[] kFactors);
        double GetVCFForProductBasedOnUserForAviation(string productId, double temperature, double density);
    }
}
