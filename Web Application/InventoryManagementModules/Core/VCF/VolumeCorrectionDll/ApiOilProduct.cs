using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{
    public class ApiOilProduct
    {
        public enum EProductNumber
        {
            /** The symbolic number for the Crude Oil commodity type */
            API_CRUDE_OIL_NAME = 0,

            /** The symbolic number for the Lubrication Oil commodity type */
            API_LUBRICATING_OIL_NAME = 1,

            /** The symbolic number for the Refined Products commodity type */
            API_REFINED_PRODUCTS_NAME = 2,

            /** The symbolic number for the Fuel Oil commodity type */
            API_FUEL_OIL_NAME = 3,

            /** The symbolic number for the Jet Oil commodity type */
            API_JET_FUEL_NAME = 4,

            /** The symbolic number for the Transition Zone commodity type */
            API_TRANSITION_ZONE_NAME = 5,

            /** The symbolic number for the Gasoline  commodity type */
            API_GASOLINE_NAME = 6,

            /** The symbolic number for the commodity if alpha 60is supplied  */
            API_COMMODITY_NOT_GIVEN = -1,

            API_OIL_PRODUCT_MAX_NUMBER = 7
        };

        public EProductNumber name;                 /** The name of the ApiOilProduct */
        public ApiDensity[] limits = new ApiDensity[2];     /** The density limits for this ApiOilProduct */
        public ApiDensity[] rhoLimits = new ApiDensity[2];  /** The density limits for 11.1.6.2 calculations */
        public double[] KValues = new double[3];         /** The K-Values, used in 11.1.6.1 calculations */
        public double Dalpha;             /** The D_alpha value used in 11.1.6.2 calculations */
        public bool hasRhoLimits;         /** Does this ApiOilProduct have density limits */
        /** for 11.1.6.2 calculations */
        public bool hasKValues;           /** Does this product have K-Values */
        public bool isRefined;            /** Is this a refined product */



        /** The available ApiOilProducts */
        protected static ApiOilProduct[] API_OIL_PRODUCT = new ApiOilProduct[(int)EProductNumber.API_OIL_PRODUCT_MAX_NUMBER];

        public ApiOilProduct()
        {
            //Error errCode = Initalize();
            //if(errCode != Error.NO_ERROR)
            //{
            //    throw new ApplicationException("ApiOilProduct.Initizialize : errCode = " + errCode.ToString());
            //}
        }

        public static ApiOilProduct Init(EProductNumber name,
                           double[] limits,
                           double[] rhoLimits,
                           double[] kvalues,
                           double dalpha,
                           bool hasKValues,
                           bool hasRhoLimits,
                           bool isRefined)
        {
            ApiOilProduct product = new ApiOilProduct();
            Error errorCode = Error.NO_ERROR;
            int i;

            if (product == null)
            {
                return product;
            }

            /** Set the name for the product */
            product.name = name;

            /** Set the density limits */
            for (i = 0; i < 2; i++)
            {
                product.limits[i] = ApiDensity.Init(limits[i],
                                  ApiUnit.ApiUnit_Density_KGM3(),
                                  false,
                                  out errorCode);
                if (errorCode != Error.NO_ERROR || product.limits[i] == null)
                {
                    product = null;
                    return product;
                }
                product.rhoLimits[i] = ApiDensity.Init(rhoLimits[i],
                                     ApiUnit.ApiUnit_Density_KGM3(),
                                     false,
                                     out errorCode);
                if (errorCode != Error.NO_ERROR || product.rhoLimits[i] == null)
                {
                    product = null;
                    return product;
                }
            }


            /** Set the K-Values */
            for (i = 0; i < 3; i++)
            {
                product.KValues[i] = kvalues[i];
            }

            /** Set the D_alpha value */
            product.Dalpha = dalpha;


            /** Set whether we have rho limits  and k values*/
            product.hasRhoLimits = hasRhoLimits;
            product.hasKValues = hasKValues;

            /** Set the isRefined value */
            product.isRefined = isRefined;

            return product;
        }


        /**
        * The initialization routine
*/
        public static Error Initalize()
        {
            Error errorCode = Error.NO_ERROR;

            /** Crude Oil */
            if (API_OIL_PRODUCT[(int)EProductNumber.API_CRUDE_OIL_NAME] == null)
            {
                double [] limits = Constants.API_CRUDE_OIL_DENSITY_LIMITS;
                double [] rhoLimits = Constants.API_CRUDE_OIL_RHO_LIMITS;
                double [] kvalue = Constants.API_CRUDE_OIL_KVALUES;


                API_OIL_PRODUCT[(int)EProductNumber.API_CRUDE_OIL_NAME] = Init(EProductNumber.API_CRUDE_OIL_NAME,
                                             limits,
                                             rhoLimits,
                                             kvalue,
                                             Constants.API_CRUDE_OIL_D_ALPHA,
                                             true,
                                             true,
                                             false);
                if (API_OIL_PRODUCT[(int)EProductNumber.API_CRUDE_OIL_NAME] == null)
                {
                    errorCode = Error.INITIALIZE_FAILED;
                    return errorCode;
                }
            }


            /** Fuel Oil */
            if (API_OIL_PRODUCT[(int)EProductNumber.API_FUEL_OIL_NAME] == null)
            {
                double [] limits = Constants.API_FUEL_OIL_DENSITY_LIMITS;
                double [] rhoLimits = { 0, 0 };
                double [] kvalue = Constants.API_FUEL_OIL_KVALUES;

                API_OIL_PRODUCT[(int)EProductNumber.API_FUEL_OIL_NAME] = Init(EProductNumber.API_FUEL_OIL_NAME,
                                            limits,
                                            rhoLimits,
                                            kvalue,
                                            Constants.API_FUEL_OIL_D_ALPHA,
                                            true,
                                            false,
                                            true);

                if (API_OIL_PRODUCT[(int)EProductNumber.API_FUEL_OIL_NAME] == null)
                {
                    errorCode = Error.INITIALIZE_FAILED;
                    return errorCode;
                }
            }


            /** Jet Fuel */
            if (API_OIL_PRODUCT[(int)EProductNumber.API_JET_FUEL_NAME] == null)
            {
                double [] limits = Constants.API_JET_FUEL_DENSITY_LIMITS;
                double [] rhoLimits = { 0, 0 };
                double [] kvalue = Constants.API_JET_FUEL_KVALUES;

                API_OIL_PRODUCT[(int)EProductNumber.API_JET_FUEL_NAME] = Init(EProductNumber.API_JET_FUEL_NAME,
                                             limits,
                                             rhoLimits,
                                             kvalue,
                                             Constants.API_JET_FUEL_D_ALPHA,
                                             true,
                                             false,
                                             true);

                if (API_OIL_PRODUCT[(int)EProductNumber.API_JET_FUEL_NAME] == null)
                {
                    errorCode = Error.INITIALIZE_FAILED;
                    return errorCode;
                }
            }

            /** Transition Zone */
            if (API_OIL_PRODUCT[(int)EProductNumber.API_TRANSITION_ZONE_NAME] == null)
            {
                double [] limits = Constants.API_TRANSITION_ZONE_DENSITY_LIMITS;
                double [] rhoLimits = { 0, 0 };
                double [] kvalue = Constants.API_TRANSITION_ZONE_KVALUES;

                API_OIL_PRODUCT[(int)EProductNumber.API_TRANSITION_ZONE_NAME] =
                                   Init(EProductNumber.API_TRANSITION_ZONE_NAME,
                                 limits,
                                 rhoLimits,
                                 kvalue,
                                 Constants.API_TRANSITION_ZONE_D_ALPHA,
                                 true,
                                 false,
                                 true);
                if (API_OIL_PRODUCT[(int)EProductNumber.API_TRANSITION_ZONE_NAME] == null)
                {
                    errorCode = Error.INITIALIZE_FAILED;
                    return errorCode;
                }
            }

            /* Gasoline  */
            if (API_OIL_PRODUCT[(int)EProductNumber.API_GASOLINE_NAME] == null)
            {
                double [] limits = Constants.API_GASOLINE_DENSITY_LIMITS;
                double [] rhoLimits = { 0, 0 };
                double [] kvalue = Constants.API_GASOLINE_KVALUES;

                API_OIL_PRODUCT[(int)EProductNumber.API_GASOLINE_NAME] = Init(EProductNumber.API_GASOLINE_NAME,
                                            limits,
                                            rhoLimits,
                                            kvalue,
                                            Constants.API_GASOLINE_D_ALPHA,
                                            true,
                                            false,
                                            true);
                if (API_OIL_PRODUCT[(int)EProductNumber.API_GASOLINE_NAME] == null)
                {
                    errorCode = Error.INITIALIZE_FAILED;
                    return errorCode;
                }
            }


            /** Lubrication  Oil */
            if (API_OIL_PRODUCT[(int)EProductNumber.API_LUBRICATING_OIL_NAME] == null)
            {
                double [] limits = Constants.API_LUBRICATION_OIL_DENSITY_LIMITS;
                double [] rhoLimits = Constants.API_LUBRICATION_OIL_RHO_LIMITS;
                double [] kvalue = Constants.API_LUBRICATION_OIL_KVALUES;

                API_OIL_PRODUCT[(int)EProductNumber.API_LUBRICATING_OIL_NAME] =
                                                 Init(EProductNumber.API_LUBRICATING_OIL_NAME,
                                       limits,
                                       rhoLimits,
                                       kvalue,
                                       Constants.API_LUBRICATION_OIL_D_ALPHA,
                                       true,
                                       true,
                                       false);
                if (API_OIL_PRODUCT[(int)EProductNumber.API_LUBRICATING_OIL_NAME] == null)
                {
                    errorCode = Error.INITIALIZE_FAILED;
                    return errorCode;
                }
            }


            /** Refined Products */
            if (API_OIL_PRODUCT[(int)EProductNumber.API_REFINED_PRODUCTS_NAME] == null)
            {
                double [] limits = Constants.API_REFINED_PRODUCTS_DENSITY_LIMITS;
                double [] rhoLimits = Constants.API_REFINED_PRODUCTS_RHO_LIMITS;
                double [] kvalue = { 0, 0, 0 };

                API_OIL_PRODUCT[(int)EProductNumber.API_REFINED_PRODUCTS_NAME] =
                                                  Init(EProductNumber.API_REFINED_PRODUCTS_NAME,
                                        limits,
                                        rhoLimits,
                                        kvalue,
                                        0,
                                        false,
                                        true,
                                        false);
                if (API_OIL_PRODUCT[(int)EProductNumber.API_REFINED_PRODUCTS_NAME] == null)
                {
                    errorCode = Error.INITIALIZE_FAILED;
                    return errorCode;
                }
            }

            return errorCode;
        }


        public EProductNumber ProductName( out Error errorCode)
        {
            errorCode = Error.NO_ERROR;
            return name;    
        }

        public Error GetKValues(out double[] values)
        {
            int i;
            Error errorCode = Error.NO_ERROR;

            values = new double[3];

 
                if (hasKValues)
                {
                    for (i = 0; i < 3; i++) values[i] = KValues[i];
                }
                else
                {
                    for (i = 0; i < 3; i++) values[i] = 0;
                    errorCode = Error.UNDEFINED_K_VALUES;
                }         

            return errorCode;
        }

        public double GetDalpha(out Error errorCode)
        {
            errorCode = Error.NO_ERROR;

            return Dalpha;
        }

        public ApiDensity[] GetLimits(out Error errorCode)
        {
            errorCode = Error.NO_ERROR;

            return limits;
        }

        public ApiDensity[] GetRhoRange(out Error errorCode)
        {
            errorCode = Error.NO_ERROR;
            if (hasRhoLimits)
            {
                return rhoLimits;
            }
            else
            {
                errorCode = Error.UNDEFINED_RHO_LIMITS;
            }
            return null;
        }

        public bool IsInRange(ApiDensity density,out Error errorCode)
        {
            bool result = true;
            int comp;

            errorCode = Error.NO_ERROR;

            if ( density == null)
            {
                result = false;
                errorCode = Error.NULL_POINTER_EXCEPTION;
                return result;
            }

            comp = limits[0].CompareTo(density,out errorCode);
            if (errorCode != Error.NO_ERROR)
            {
                result = false;
                return result;
            }
            if (comp > 0)
            {
                result = false;
            }

            comp = limits[1].CompareTo(density,out errorCode);
            if (errorCode != Error.NO_ERROR)
            {
                result = false;
                return result;
            }
            if (comp < 0)
            {
                result = false;
            }

            return result;
        }

        public bool IsInRhoRange(ApiDensity density, out Error errorCode)
        {
            bool result = true;
            int comp;

            errorCode = Error.NO_ERROR;

            if (density == null)
            {
                result = false;
                errorCode = Error.NULL_POINTER_EXCEPTION;
                return result;
            }

            if (!hasRhoLimits)
            {
                errorCode = Error.UNDEFINED_RHO_LIMITS;
                result = false;
                return result;
            }

            comp = rhoLimits[0].CompareTo(density, out errorCode);
            if (errorCode != Error.NO_ERROR)
            {
                result = false;
                return result;
            }
            if (comp > 0)
            {
                result = false;
            }

            comp = rhoLimits[1].CompareTo(density, out errorCode);
            if (errorCode != Error.NO_ERROR)
            {
                result = false;
                return result;
            }
            if (comp < 0)
            {
                result = false;
            }

            return result;
        }

        public static ApiOilProduct GetProductByName(EProductNumber name, out Error errorCode)
        {
            ApiOilProduct product = null;

            errorCode = Error.NO_ERROR;

            if (name == EProductNumber.API_CRUDE_OIL_NAME)
            {
                product = API_OIL_PRODUCT[(int)EProductNumber.API_CRUDE_OIL_NAME];
            }
            else if (name == EProductNumber.API_LUBRICATING_OIL_NAME)
            {
                product = API_OIL_PRODUCT[(int)EProductNumber.API_LUBRICATING_OIL_NAME];
            }
            else if (name == EProductNumber.API_REFINED_PRODUCTS_NAME)
            {
                product = API_OIL_PRODUCT[(int)EProductNumber.API_REFINED_PRODUCTS_NAME];
            }

            if (product == null) errorCode = Error.UNDEFINED_OIL_PRODUCT;

            return product;
        }


        public static ApiOilProduct GetProductByDensity(EProductNumber name, ApiDensity density, out Error errorCode)
        {
            ApiOilProduct product = GetProductByName(name, out errorCode);
            int i;

            if (product == null || errorCode != Error.NO_ERROR)
            {
                errorCode = Error.UNDEFINED_OIL_PRODUCT;
                return product;
            }

            errorCode = Error.NO_ERROR;

            if (product == API_OIL_PRODUCT[(int)EProductNumber.API_REFINED_PRODUCTS_NAME])
            {
                bool result;

                for (i = 0; i < (int)EProductNumber.API_OIL_PRODUCT_MAX_NUMBER; i++)
                {
                    if (API_OIL_PRODUCT[i] == null)
                    {
                        errorCode = Error.NULL_POINTER_EXCEPTION;
                        return null;
                    }
                    if (API_OIL_PRODUCT[i].isRefined == true)
                    {
                        result = API_OIL_PRODUCT[i].IsInRange(density, out errorCode);
                        if (errorCode != Error.NO_ERROR)
                        {
                            product = null;
                            return product;
                        }

                        if (result)
                        {
                            product = API_OIL_PRODUCT[i];
                            return product;
                        }
                    }
                }
            }

            return product;
        }


    }
}
