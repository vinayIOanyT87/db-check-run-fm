using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{
    class Constants
    {
        public const double API_EPSILON = 1e-7;
        public const double API_TEMP_C_TO_F_SLOPE = 1.8;
        public const double API_TEMP_C_TO_F_INTERCEPT = 32.0;
        public const double API_TEMP_C_ROUNDING_INCREMENT = 0.05;
        public const double API_TEMP_F_ROUNDING_INCREMENT = 0.1;
        public static readonly double[] API_ALLOWED_TEMPERATURE_RANGE = new double[] { -58.0, 302.0 };
        public static readonly double[] API_TEMP_IPTS68_CONSTANTS = new double[] { -0.148759, -0.267408, 1.080760, 1.269056, -4.089591, -1.871251, 7.438081, -3.536296 };
        public const double API_TEMP_IPTS68_SCALER = 630.0;

        public const double API_RESSURE_PSI_ROUNDING_INCREMENT = 1.0;
        public const double API_PRESSURE_KPA_ROUNDING_INCREMENT = 5.0;
        public const double API_PRESSURE_BAR_ROUNDING_INCREMENT = 0.05;
        public const double API_PRESSURE_KPA_TO_PSI_FACTOR = 6.894757;
        public const double API_PRESSURE_BAR_TO_PSI_FACTOR = 0.06894757;
        public const double API_DENSITY_KGM3_ROUNDING_INCREMENT = 0.1;
        public const double API_DENSITY_API_ROUNDING_INCREMENT = 0.1;
        public const double API_DENSITY_RELATIVE_ROUNDING_INCREMENT = 0.0001;
        public const double API_DENSITY_RELATIVE_TO_KGM3_FACTOR = 999.016;
        public const double API_DENSITY_API_TO_KGM3_UPPER = 141.5;
        public const double API_DENSITY_API_TO_KGM3_LOWER = 131.5;
        public const double API_THERMAL_EXPANSION_F_ROUNDING_INCREMENT = 0.1e-6;
        public const double API_THERMAL_EXPANSION_C_ROUNDING_INCREMENT = 0.2e-6;
        public const double API_THERMAL_EXPANSION_REVF_TO_REVC = 1.8;
        public const double API_EXPANSION_FACTOR_ROUNDING_INCREMENT = 0.00001;
        public const double API_SCALED_COMP_REV_PSI_ROUNDING_INCREMENT = 0.001;
        public const double API_SCALED_COMP_REV_KPA_ROUNDING_INCREMENT = 0.0001;
        public const double API_SCALED_COMP_REV_BAR_ROUNDING_INCREMENT = 0.01;
        public static readonly double[] API_CRUDE_OIL_KVALUES = new double[] { 341.0957, 0.0, 0.0 };
        public static readonly double[] API_FUEL_OIL_KVALUES = new double[] { 103.8720, 0.2701, 0.0 };
        public static readonly double[] API_JET_FUEL_KVALUES = new double[] { 330.3010, 0.0, 0.0 };
        public static readonly double[] API_TRANSITION_ZONE_KVALUES = new double[] { 1489.0670, 0.0, -0.00186840 };
        public static readonly double[] API_GASOLINE_KVALUES = new double[] { 192.4571, 0.2438, 0.0 };
        public static readonly double[] API_LUBRICATION_OIL_KVALUES = new double[] { 0.0, 0.34878, 0.0 };
        public const double API_CRUDE_OIL_D_ALPHA = 2.0;
        public const double API_FUEL_OIL_D_ALPHA = 1.3;
        public const double API_JET_FUEL_D_ALPHA = 2.0;
        public const double API_TRANSITION_ZONE_D_ALPHA = 8.5;
        public const double API_GASOLINE_D_ALPHA = 1.5;
        public const double API_LUBRICATION_OIL_D_ALPHA = 1.0;
        public const double API_DELTA_60 = 0.01374979547;
        public const double API_IPTS_68_BASE = 60.0068749;
        public static readonly double[] API_FP_STEP_6_FACTOR = new double[] { -1.9947, 0.00013427, 793920.0, 2326.0 };
        public static readonly double[] API_DP_STEP_5_FACTOR = new double[] { 7.93920, 0.02326 };
        public const int API_ITERATION_STEPS = 15;
        public const int API_THERMAL_REGRESSION_ITER = 6;
        public const int API_BASE_TEMP = 60;
        public const int API_BASE_PRES = 0;
        public static readonly double[] API_CRUDE_OIL_DENSITY_LIMITS = new double[] { 610.6, 1163.5 };
        public static readonly double[] API_FUEL_OIL_DENSITY_LIMITS = new double[] { 838.3127, 1163.5 };
        public static readonly double[] API_JET_FUEL_DENSITY_LIMITS = new double[] { 787.5195, 838.3127 };
        public static readonly double[] API_TRANSITION_ZONE_DENSITY_LIMITS = new double[] { 770.3520, 787.5195 };
        public static readonly double[] API_GASOLINE_DENSITY_LIMITS = new double[] { 610.6, 770.3520 };
        public static readonly double[] API_LUBRICATION_OIL_DENSITY_LIMITS = new double[] { 800.9, 1163.5 };
        public static readonly double[] API_REFINED_PRODUCTS_DENSITY_LIMITS = new double[] { 610.6, 1163.5 };
        public static readonly double[] API_CRUDE_OIL_RHO_LIMITS = new double[] { 470.5, 1201.8 };
        public static readonly double[] API_REFINED_PRODUCTS_RHO_LIMITS = new double[] { 470.4, 1209.5 };
        public static readonly double[] API_LUBRICATION_OIL_RHO_LIMITS = new double[] { 714.3, 1208.3 };
        public static readonly double[] API_ALLOWED_PRESSURE_RANGE = new double[] { 0, 1500 };
        public static readonly double[] API_ALLOWED_THERMAL_EXPANSION_RANGE = new double[] { 230.0e-6, 930.0e-6 };
        public const string API_CONVERGENCE_NOTE_11162 = "Convergence failed. Last iteration value is accepted as final value.";
        public const int API_CONVERSION_NOTE_11161_CUTOFF = 1;

        public const string API_EOL = "\n";
        public const double API_CONVERSION_CRITERIA = 0.000001;


//#define API_CRUDE_OIL_D_ALPHA  2.0
//#define API_FUEL_OIL_D_ALPHA  1.3
//#define API_JET_FUEL_D_ALPHA  2.0
//#define API_TRANSITION_ZONE_D_ALPHA  8.5
//#define API_GASOLINE_D_ALPHA  1.5
//#define API_LUBRICATION_OIL_D_ALPHA  1.0
//#define API_DELTA_60  0.01374979547
//#define API_IPTS_68_BASE  60.0068749
//#define API_FP_STEP_6_FACTOR  { -1.9947, 0.00013427, 793920.0 , 2326.0 }
//#define API_DP_STEP_5_FACTOR  {7.93920, 0.02326}
//#define API_ITERATION_STEPS  15
//#define API_THERMAL_REGRESSION_ITER  6
//#define API_BASE_TEMP 60
//#define API_BASE_PRES 0
//#define API_CRUDE_OIL_DENSITY_LIMITS  {610.6,1163.5}
//#define API_FUEL_OIL_DENSITY_LIMITS {838.3127,1163.5}
//#define API_JET_FUEL_DENSITY_LIMITS {787.5195, 838.3127}
//#define API_TRANSITION_ZONE_DENSITY_LIMITS {770.3520, 787.5195}
//#define API_GASOLINE_DENSITY_LIMITS {610.6, 770.3520}
//#define API_LUBRICATION_OIL_DENSITY_LIMITS {800.9,1163.5}
//#define API_REFINED_PRODUCTS_DENSITY_LIMITS {610.6, 1163.5}
//#define API_CRUDE_OIL_RHO_LIMITS {470.5,1201.8}
//#define API_REFINED_PRODUCTS_RHO_LIMITS { 470.4, 1209.5}
//#define API_LUBRICATION_OIL_RHO_LIMITS  {714.3, 1208.3}



    }
}
