using System;
using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{

    public class ConvertEngUnits
    {
        public static bool ConvEngrUnits(ref double ToDataPtr, double FromData, EngineeringUnit ToUnit, EngineeringUnit FromUnit, double SpecialParam)
        {
            try
            {
                EngineeringUnits.Convert(FromData,FromUnit, ref ToDataPtr, ToUnit, SpecialParam);
            }
            catch(Exception)
            {
                return false;
            }
            return true;                                 
        }
    }
}
