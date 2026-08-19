using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FMBusinessObjects.Exceptions;
using System.Xml;

namespace FMBusinessServices.InternalClasses.EntityImportExport
{
    public abstract class WorksheetBase
    {
        #region Public data members - Worksheets
        public const string WORKSHEET_COMPANIES = "Companies";
        public const string WORKSHEET_COMPANY_ROLES = "CompanyRole";
        public const string WORKSHEET_COMPANY_AUTHORIZED_CARRIERS = "CompanyAuthorizedCarriers";
        public const string WORKSHEET_COMPANY_AUTHORIZED_PRODUCTS = "CompanyAuthorizedProducts";
        public const string WORKSHEET_COMPANY_CERTIFICATES_AND_PERMITS = "CompanyCertificatesAndPermits";
        public const string WORKSHEET_COMPANY_ACCESS_SCHEDULE = "CompanyAccessSchedule";
        public const string WORKSHEET_COMPANY_HIERARCHY = "CompanyHierarchy";
        public const string WORKSHEET_COMPANY_DRIVERS = "CompanyDrivers";
        public const string WORKSHEET_COMPANY_EQUIPMENT = "CompanyEquipment";
        public const string WORKSHEET_EQUIPMENT = "Equipment";
        public const string WORKSHEET_EQUIPMENT_COMPARTMENTS = "EquipCompartments";
        public const string WORKSHEET_EQUIPMENT_TEST_AND_INSPECTIONS = "EquipTestAndInspections";
        public const string WORKSHEET_EQUIPMENT_TAGS_AND_LICENSES = "EquipTagsAndLicenses";
        public const string WORKSHEET_EQUIPMENT_TYPES = "EquipTypes";
        public const string WORKSHEET_EQUIPMENT_TYPE_QUALIFICATIONS = "EquipTypeQualifications";
        public const string WORKSHEET_EQUIPMENT_TYPE_TRAINING = "EquipTypeTraining";
        public const string WORKSHEET_PERSONNEL = "Personnel";
        public const string WORKSHEET_PERSONNEL_ROLES = "PersonnelRoles";
        public const string WORKSHEET_PERSONNEL_QUALIFICATIONS = "PersonnelQualifications";
        public const string WORKSHEET_PERSONNEL_LICENSES = "PersonnelLicenses";
        public const string WORKSHEET_PERSONNEL_TRAINING = "PersonnelTraining";
        public const string WORKSHEET_PERSONNEL_ACCESS_SCHEDULE = "PersonnelAccessSchedule";
        public const string WORKSHEET_PRODUCTS = "Products";
        public const string WORKSHEET_PRODUCT_AUTHORIZED_CUSTOMERS = "ProductAuthorizedCustomers";
        public const string WORKSHEET_PRODUCT_MESSAGES = "ProductMessages";
        public const string WORKSHEET_PRODUCT_HAZARDOUS_MATERIAL_MESSAGES = "ProdHazardousMaterialMessages";
        public const string WORKSHEET_PRODUCT_AUTHORIZED_CUSTOMER_GROUPS = "ProdAuthorizedCustomerGroups";
        public const string WORKSHEET_PRODUCT_ADDITIVE_PROFILES = "ProdAdditiveProfiles";
        public const string WORKSHEET_PRODUCT_BLEND_COMPONENTS = "ProductBlendComponents";
        public const string WORKSHEET_STANDING_OFFERS = "StandingOffers";
        public const string WORKSHEET_FUEL_CARD = "FuelCard";
        public const string WORKSHEET_FUEL_CARD_EQUIPMENT_ASSIGNMENTS = "FuelCardEquipmentAssignments";
        public const string WORKSHEET_IATA_CODES = "IATACodes";
        #endregion

        #region Public data members - Engineering Units
        // Mass Units
        public const string UNIT_GRAMS = "GRAMS";
        public const string UNIT_KG = "KG";
        public const string UNIT_MTON = "MTON";
        public const string UNIT_OZ = "OZ";
        public const string UNIT_LB = "LB";
        public const string UNIT_ETON = "ETON";
        public const string UNIT_STON = "STON";
        public const string UNIT_LTON = "LTON";
        public const string UNIT_MLBS = "MLBS";

        // Density Units
        public const string FMD_GCM3 = "FMD_GCM3";       // Grams/cubic cm
        public const string FMD_GMl3 = "FMD_GMl3";       // Grams/cubic millilitre
        public const string FMD_GL3 = "FMD_GL3";         // Grams/cubic litre
        public const string FMD_KGM3 = "FMD_KGM3";       // Kilograms/cubic meter
        public const string FMD_KGL3 = "FMD_KGL3";       // Kilograms/cubic litre
        public const string FMD_LBIN3 = "FMD_LBIN3";        // Pounds/cubic inch
        public const string FMD_LBFT3 = "FMD_LBFT3";        // Pounds/cubic feet
        public const string FMD_USLBGAL = "FMD_USLBGAL";    // Pounds/gallon
        public const string FMD_IMPLBGL = "FMD_IMPLBGL";    // Pounds/gallon (imperial)
        public const string FMD_LBBLOIL = "FMD_LBBLOIL";    // Pounds/barrel (oil)
        public const string FMD_LBBLLIQ = "FMD_LBBLLIQ";    // Pounds/barrel (liquid)
        public const string FMD_DEGAPI = "FMD_DEGAPI";         // Degrees API
        public const string FMD_SPGRAV = "FMD_SPGRAV";         // Specific gravity
        public const string FMD_PRPLATO = "FMD_PRPLATO";    // % Plato
        public const string FMD_DEGBRIX = "FMD_DEGBRIX";    // Degrees BRIX
        public const string FMD_DEGBMLT = "FMD_DEGBMLT";    // Degrees Baum (light)
        public const string FMD_DEGBMHY = "FMD_DEGBMHY";    // Degrees Baum (heavy)
        public const string FMD_DEGTWAD = "FMD_DEGTWAD";    // Degrees Twaddell
        public const string FMD_DEGBAL = "FMD_DEGBAL";         // Degrees Balling
        public const string FMD_STNYD3 = "FMD_STNYD3";         // Short tons/cubic yard

        // Temperature Units
        public const string FMT_DEGC = "FMT_DEGC";          // Degrees Celcius
        public const string FMT_DEGF = "FMT_DEGF";          // Degrees Farenheit
        public const string FMT_DEGK = "FMT_DEGK";          // Degrees Kelvin
        public const string FMT_DEGR = "FMT_DEGR";          // Degrees Rankine

        // Volume units
        public const string FMV_CM3 = "FMV_CM3";               // Cubic centimeters
        public const string FMV_Meter3 = "FMV_METER3";               // Cubic meters
        public const string FMV_Litre = "FMV_LITRE";              // Litres
        public const string FMV_Inch3 = "FMV_INCH3";              // Cubic inches
        public const string FMV_Feet3 = "FMV_FEET3";              // Cubic feet
        public const string FMV_Yard3 = "FMV_YARD3";              // Cubic yards
        public const string FMV_USGal = "FMV_USGAL";              // US Gallons
        public const string FMV_ImpGal = "FMV_IMPGAL";               // Imp Gallons
        public const string FMV_BlOil = "FMV_BLOIL";              // Barrels Oil
        public const string FMV_BlLiq = "FMV_BLLIQ";              // Barrels Liquid
        public const string FMV_KL = "FMV_KL";                 // Kilolitres
        public const string FMV_MsFt3 = "FMV_MsFt3";              // 1000 standard cubic feet 
        #endregion

        #region Public data members - String Formats
        public const string YYYYMMDD_FORMAT = "yyyy-MM-dd";
        public static List<string> standardformatType;
        #endregion

        #region Private data members
        private string worksheetName;
        #endregion

        #region Protected data members
        protected XmlNode worksheetNode;
        protected EntityImportExportException EntityImportExportException;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor for the Worksheet base class.
        /// </summary>
        /// <param name="wrkshtName"></param>
        public WorksheetBase(string wrkshtName)
        {
            this.worksheetName = "Worksheet1";

            if ((wrkshtName != null) && (wrkshtName.Length > 0))
            {
                this.worksheetName = wrkshtName;
            }

            standardformatType = new List<string>();
            standardformatType.Add("General");
            standardformatType.Add("General Number");
            standardformatType.Add("General Date");
            standardformatType.Add("Long Date");
            standardformatType.Add("Medium Date");
            standardformatType.Add("Short Date");
            standardformatType.Add("Long Time");
            standardformatType.Add("Medium Time");
            standardformatType.Add("Short Time");
            standardformatType.Add("Currency");
            standardformatType.Add("Euro Currency");
            standardformatType.Add("Fixed");
            standardformatType.Add("Standard");
            standardformatType.Add("Percent");
            standardformatType.Add("Scientific");
            standardformatType.Add("Yes/No");
            standardformatType.Add("True/False");
            standardformatType.Add("On/Off");

            this.EntityImportExportException = new EntityImportExportException(null, EntityImportExportException.EXCEPTION_TYPES.NONE);
        }
        #endregion

        #region Properties
        protected string WorksheetName
        {
            get { return this.worksheetName; }
        }

        public EntityImportExportException ImportException
        {
            get { return this.EntityImportExportException; }
            set { this.EntityImportExportException = value; }
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// This method returns a boolean true or false.  The default
        /// is set to false.
        /// </summary>
        /// <param name="inStr"></param>
        /// <returns></returns>
        protected bool ConvertToBool(string inStr)
        {
            bool convertedValue = false;

            if ((inStr != null) && (inStr.Length > 0))
            {
                if ((inStr.ToUpper().Equals("TRUE") == true) ||
                   (inStr.Equals("1") == true))
                {
                    convertedValue = true;
                }
            }

            return convertedValue;
        }

        /// <summary>
        /// This method returns a db null or the converted double value.
        /// </summary>
        /// <param name="inStr"></param>
        /// <returns></returns>
        protected double ConvertToDouble(string inStr)
        {
            double convertedValue = 0.0;

            if ((inStr != null) && (inStr.Length > 0))
            {
                try
                {
                    convertedValue = System.Convert.ToDouble(inStr);
                }
                catch (Exception)
                {
                }
            }

            return convertedValue;
        }

        /// <summary>
        /// This method returns an integer value that was converted from the given string.
        /// </summary>
        /// <param name="inStr"></param>
        /// <returns></returns>
        protected int ConvertToInteger(string inStr)
        {
            int convertedValue = 0;

            if ((inStr != null) && (inStr.Length > 0))
            {
                try
                {
                    convertedValue = System.Convert.ToInt32(inStr);
                }
                catch (Exception)
                {
                }
            }

            return convertedValue;
        }

        /// <summary>
        /// This method returns an short value that was converted from the given string.
        /// </summary>
        /// <param name="inStr"></param>
        /// <returns></returns>
        protected short ConvertToShort(string inStr)
        {
            short convertedValue = 0;

            if ((inStr != null) && (inStr.Length > 0))
            {
                try
                {
                    convertedValue = System.Convert.ToInt16(inStr);
                }
                catch (Exception)
                {
                }
            }

            return convertedValue;
        }


        /// This method returns an equipment type value that was converted from the given string.
        /// </summary>
        /// <param name="inStr"></param>
        /// <returns></returns>
        /// <summary>
        /// This method will either return an empty string or the actual
        /// string value.
        /// </summary>
        /// <param name="inStr"></param>
        /// <returns></returns>
        protected string GetStringValue(string inStr)
        {
            string strValue = "";

            if ((inStr != null) && (inStr.Length > 0))
            {
                strValue = inStr;
            }

            return strValue;
        }

        /// <summary>
        /// This method will convert the unit name string into a CU_UNIT type.
        /// The default is pounds.
        /// </summary>
        /// <param name="inStr"></param>
        /// <returns>CU_UNIT</returns>


        /// This method returns a personnel role value that was converted from the given string.
        /// </summary>
        /// <param name="inStr"></param>
        /// <returns>PERSON_ROLE</returns>


        /// This method returns a product type value that was converted from the given string.
        /// </summary>
        /// <param name="inStr"></param>
        /// <returns>PRODUCT_TYPE</returns>
        #endregion
    }
}