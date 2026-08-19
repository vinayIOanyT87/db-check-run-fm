using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{
    public class ApiUnit
    {

        public string unitName;

        public static Error ApiUnit_unitName(ApiUnit unit, ref string buffer)
        {
            buffer += unit.unitName;
            return Error.NO_ERROR;
        }
        public static ApiUnit ApiUnit_init(string value)
        {
            var ret = new ApiUnit();
            ret.unitName = value;
            return ret;
        }

        public static void ApiUnit_cleanUp(ApiUnit unit)
        {
            unit.unitName = string.Empty;
        }

        protected static ApiUnit DENSITY_API = null;
        protected static ApiUnit DENSITY_RELATIVE = null;
        protected static ApiUnit DENSITY_KGM3 = null;

        public static ApiUnit ApiUnit_Density_API()
        {
            return DENSITY_API;
        }

        public static ApiUnit ApiUnit_Density_RELATIVE()
        {
            return DENSITY_RELATIVE;
        }

        public static ApiUnit ApiUnit_Density_KGM3()
        {
            return DENSITY_KGM3;
        }

        protected static ApiUnit PRESSURE_PSI = null;
        protected static ApiUnit PRESSURE_KPA = null;
        protected static ApiUnit PRESSURE_BAR = null;

        public static ApiUnit ApiUnit_Pressure_PSI()
        {
            return PRESSURE_PSI;
        }

        public static ApiUnit ApiUnit_Pressure_KPA()
        {
            return PRESSURE_KPA;
        }

        public static ApiUnit ApiUnit_Pressure_BAR()
        {
            return PRESSURE_BAR;
        }

        protected static ApiUnit TEMPERATURE_F = null;
        protected static ApiUnit TEMPERATURE_C = null;

        public static ApiUnit ApiUnit_Temperature_F()
        {
            return TEMPERATURE_F;
        }

        public static ApiUnit ApiUnit_Temperature_C()
        {
            return TEMPERATURE_C;
        }

        protected static ApiUnit VOLUME_BARREL = null;
        protected static ApiUnit VOLUME_LITER = null;
        protected static ApiUnit VOLUME_M3 = null;

        public static ApiUnit ApiUnit_Volume_BARREL()
        {
            return VOLUME_BARREL;
        }

        public static ApiUnit ApiUnit_Volume_LITER()
        {
            return VOLUME_LITER;
        }

        public static ApiUnit ApiUnit_Volume_M3()
        {
            return VOLUME_M3;
        }

        protected static ApiUnit EXPANSION_DIMLESS = null;

        public static ApiUnit ApiUnit_Expansion_DIMLESS()
        {
            return EXPANSION_DIMLESS;
        }

        protected static ApiUnit SCALED_COMP_REV_PSI = null;
        protected static ApiUnit SCALED_COMP_REV_BAR = null;
        protected static ApiUnit SCALED_COMP_REV_KPA = null;

        public static ApiUnit ApiUnit_ScaledComp_REV_PSI()
        {
            return SCALED_COMP_REV_PSI;
        }

        public static ApiUnit ApiUnit_ScaledComp_REV_BAR()
        {
            return SCALED_COMP_REV_BAR;
        }

        public static ApiUnit ApiUnit_ScaledComp_REV_KPA()
        {
            return SCALED_COMP_REV_KPA;
        }
        protected static ApiUnit THERMAL_EXP_REV_C = null;
        protected static ApiUnit THERMAL_EXP_REV_F = null;

        public static ApiUnit ApiUnit_ThermalExp_REV_C()
        {
            return THERMAL_EXP_REV_C;
        }

        public static ApiUnit ApiUnit_ThermalExp_REV_F()
        {
            return THERMAL_EXP_REV_F;
        }

        public static Error ApiUnit_initalize()
        {
            Error errorCode = Error.NO_ERROR;

            /** Density unit objects */
            if (DENSITY_API == null)
            {
                DENSITY_API = ApiUnit_init(" API");
                if (DENSITY_API == null) errorCode = Error.INITIALIZE_FAILED;
            }
            if (DENSITY_RELATIVE == null)
            {
                DENSITY_RELATIVE = ApiUnit_init(" Rel. Dens.");
                if (DENSITY_RELATIVE == null) errorCode = Error.INITIALIZE_FAILED;
            }
            if (DENSITY_KGM3 == null)
            {
                DENSITY_KGM3 = ApiUnit_init(" Kg/m^3");
                if (DENSITY_KGM3 == null) errorCode = Error.INITIALIZE_FAILED;
            }

            /** Pressure unit objects */
            if (PRESSURE_PSI == null)
            {
                PRESSURE_PSI = ApiUnit_init(" psi");
                if (PRESSURE_PSI == null) errorCode = Error.INITIALIZE_FAILED;
            }
            if (PRESSURE_KPA == null)
            {
                PRESSURE_KPA = ApiUnit_init(" kPa");
                if (PRESSURE_KPA == null) errorCode = Error.INITIALIZE_FAILED;
            }
            if (PRESSURE_BAR == null)
            {
                PRESSURE_BAR = ApiUnit_init(" bar");
                if (PRESSURE_BAR == null) errorCode = Error.INITIALIZE_FAILED;
            }

            /** Temperature unit objects */
            if (TEMPERATURE_F == null)
            {
                TEMPERATURE_F = ApiUnit_init(" F");
                if (TEMPERATURE_F == null) errorCode = Error.INITIALIZE_FAILED;
            }
            if (TEMPERATURE_C == null)
            {
                TEMPERATURE_C = ApiUnit_init(" C");
                if (TEMPERATURE_C == null) errorCode = Error.INITIALIZE_FAILED;
            }

            /** Volume unit objects */
            if (VOLUME_BARREL == null)
            {
                VOLUME_BARREL = ApiUnit_init(" barrel");
                if (VOLUME_BARREL == null) errorCode = Error.INITIALIZE_FAILED;
            }
            if (VOLUME_LITER == null)
            {
                VOLUME_LITER = ApiUnit_init(" l");
                if (VOLUME_LITER == null) errorCode = Error.INITIALIZE_FAILED;
            }
            if (VOLUME_M3 == null)
            {
                VOLUME_M3 = ApiUnit_init(" m^3");
                if (VOLUME_M3 == null) errorCode = Error.INITIALIZE_FAILED;
            }

            /** Dimensionless unit object */
            if (EXPANSION_DIMLESS == null)
            {
                EXPANSION_DIMLESS = ApiUnit_init(" ");
                if (EXPANSION_DIMLESS == null) errorCode = Error.INITIALIZE_FAILED;
            }

            /** Scaled Comp Factor unit object */
            if (SCALED_COMP_REV_PSI == null)
            {
                SCALED_COMP_REV_PSI = ApiUnit_init(" 1/psi");
                if (SCALED_COMP_REV_PSI == null) errorCode = Error.INITIALIZE_FAILED;
            }
            if (SCALED_COMP_REV_BAR == null)
            {
                SCALED_COMP_REV_BAR = ApiUnit_init(" 1/bar");
                if (SCALED_COMP_REV_BAR == null) errorCode = Error.INITIALIZE_FAILED;
            }
            if (SCALED_COMP_REV_KPA == null)
            {
                SCALED_COMP_REV_KPA = ApiUnit_init(" 1/kPa");
                if (SCALED_COMP_REV_KPA == null) errorCode = Error.INITIALIZE_FAILED;
            }

            /** Thermal expansion factor  unit object */
            if (THERMAL_EXP_REV_C == null)
            {
                THERMAL_EXP_REV_C = ApiUnit_init(" 1/C");
                if (THERMAL_EXP_REV_C == null) errorCode = Error.INITIALIZE_FAILED;
            }
            if (THERMAL_EXP_REV_F == null)
            {
                THERMAL_EXP_REV_F = ApiUnit_init(" 1/F");
                if (THERMAL_EXP_REV_F == null) errorCode = Error.INITIALIZE_FAILED;
            }

            return errorCode;
        }

        public static void ApiUnit_destroy()
        {
            /** Density unit objects */
            if (DENSITY_API != null)
            {
                ApiUnit_cleanUp(DENSITY_API);
                DENSITY_API = null;
            }
            if (DENSITY_RELATIVE != null)
            {
                ApiUnit_cleanUp(DENSITY_RELATIVE);
                DENSITY_RELATIVE = null;
            }
            if (DENSITY_KGM3 != null)
            {
                ApiUnit_cleanUp(DENSITY_KGM3);
                DENSITY_KGM3 = null;
            }

            /** Pressure unit objects */
            if (PRESSURE_PSI != null)
            {
                ApiUnit_cleanUp(PRESSURE_PSI);
                PRESSURE_PSI = null;
            }
            if (PRESSURE_KPA != null)
            {
                ApiUnit_cleanUp(PRESSURE_KPA);
                PRESSURE_KPA = null;
            }
            if (PRESSURE_KPA != null)
            {
                ApiUnit_cleanUp(PRESSURE_KPA);
                PRESSURE_KPA = null;
            }

            /** Temperature unit objects */
            if (TEMPERATURE_F != null)
            {
                ApiUnit_cleanUp(TEMPERATURE_F);
                TEMPERATURE_F = null;
            }
            if (TEMPERATURE_C != null)
            {
                ApiUnit_cleanUp(TEMPERATURE_C);
                TEMPERATURE_C = null;
            }

            /** Volume unit objects */
            if (VOLUME_BARREL != null)
            {
                ApiUnit_cleanUp(VOLUME_BARREL);
                VOLUME_BARREL = null;
            }
            if (VOLUME_LITER != null)
            {
                ApiUnit_cleanUp(VOLUME_LITER);
                VOLUME_LITER = null;
            }
            if (VOLUME_M3 != null)
            {
                ApiUnit_cleanUp(VOLUME_M3);
                VOLUME_M3 = null;
            }

            /** Dimensionless unit objects */
            if (EXPANSION_DIMLESS != null)
            {
                ApiUnit_cleanUp(EXPANSION_DIMLESS);
                EXPANSION_DIMLESS = null;
            }

            /** Scaled Comp Factor unit objects */
            if (SCALED_COMP_REV_PSI != null)
            {
                ApiUnit_cleanUp(SCALED_COMP_REV_PSI);
                SCALED_COMP_REV_PSI = null;
            }
            if (SCALED_COMP_REV_BAR != null)
            {
                ApiUnit_cleanUp(SCALED_COMP_REV_BAR);
                SCALED_COMP_REV_BAR = null;
            }
            if (SCALED_COMP_REV_KPA != null)
            {
                ApiUnit_cleanUp(SCALED_COMP_REV_KPA);
                SCALED_COMP_REV_KPA = null;
            }


            /** Thermal Expansion factor unit objects */
            if (THERMAL_EXP_REV_C != null)
            {
                ApiUnit_cleanUp(THERMAL_EXP_REV_C);
                THERMAL_EXP_REV_C = null;
            }
            if (THERMAL_EXP_REV_F != null)
            {
                ApiUnit_cleanUp(THERMAL_EXP_REV_F);
                THERMAL_EXP_REV_F = null;
            }
        }
    }
}
