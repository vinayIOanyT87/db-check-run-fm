namespace FMBusinessServices.ServiceClasses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;
    using FMBusinessObjects.ServiceRequests;
    using FMBusinessObjects.UtilityObjects;
    using System.Text;
    using FMWebAPIBusinessLogic.Interfaces.FMProxy;
    using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
    using FMDepedencyManager;
    using Unity;
    using FMCore;
    using FMWebAPIBusinessLogic;
    using Varec.CommonComponents.EngineeringUnitsLibrary;
    using Varec.CommonComponents.VolumeCorrection;
    using System.Text.RegularExpressions;

    //using FMCommon;
    //using ConsolidatedBLL;
    //using ConsolidatedDataObjects;
    //using Interop.ConvertEngUnits;

    public class IntoPlaneImport : FMServiceBase, IIntoPlaneImport
    {
        #region Private attributes
        private SecurityClass security;
        private List<TransactionDO> transactionList;
        private Dictionary<string, Guid> siteTable;
        private Dictionary<string, SiteClass> siteLookupTable;
        private Dictionary<string, Dictionary<string, CompanyClass>> companyTable;
        private Dictionary<string, Dictionary<string, ProductClass>> productTable;
        private Dictionary<string, Dictionary<string, EquipmentClass>> equipmentTable;
        private Dictionary<string, Dictionary<string, MeterClass>> meterTable;
        private Dictionary<string, Dictionary<string, Guid>> tankTable;
        private Dictionary<string, Dictionary<string, Guid>> transactionAliasTable;
        private Dictionary<string, Dictionary<string, TransactionTypes>> transactionAliasTypeTable;
        private Dictionary<string, Dictionary<Guid, Dictionary<string, string>>> transactionAliasUserFieldsTable;
        //private List<string> defaultUserDataFieldNames;
        private Dictionary<string, Dictionary<string, Guid>> personnelTable;
        private Dictionary<string, Dictionary<string, Guid>> stationTable;
        private Dictionary<string, Guid> companyTranslationTable;
        private Dictionary<string, Guid> productTranslationTable;
        private Dictionary<string, Dictionary<string, GateClass>> gateTable;
        private Dictionary<string, GeneralConfigDO> forceCloseoutTable;
        private Dictionary<string, List<CloseoutDO>> closeoutTable;

        // Date Format: HH:MM (HH optional leading 0)
        private static Regex TIME_FORMAT = new Regex("^(2[0-3]|[01]?[0-9])[:][0-5][0-9]$");

        // Date Format: MM/DD/YYYY 
        private static Regex DATE_FORMAT = new Regex("^(0[1-9]|1[0-2])[/](0[1-9]|[1-2][0-9]|3[0-1])[/][0-9]{4}");

        // Date time Format: YYYY-mm-dd HH:MM:SS (HH optional leading 0)
        private static Regex DATE_TIME_FORMAT = new Regex("^[0-9]{4}-(0[1-9]|1[0-2])-(0[1-9]|[1-2][0-9]|3[0-1]) (2[0-3]|[01]?[0-9]):[0-5][0-9]:[0-5][0-9]$");

        private const int MAX_USER_DATA_LENGTH = 20;
        private readonly ITransactionAliasesProxy _transactionAliasProxy;
        private readonly ITransactionPipeline _transactionPipeline;
        private readonly ICurrentRequestContext _currentUserSecurity;
        private readonly IMetersProxy _metersProxy;
        #endregion

        #region Private Functions
        private TransactionDO CreateTransaction(IntoPlaneImportFields fields, IntoPlaneImportParametersDO parameters, SiteClass site, out string warnings)
        {
            TransactionDO trans = null;
            switch((IntoPlaneImportFields.TransTypes)fields.TransactionNumber)
            {
                case IntoPlaneImportFields.TransTypes.Defuel_Primary:
                case IntoPlaneImportFields.TransTypes.Defuel_Secondary:
                    trans = CreateDefuelTransaction(fields,parameters,site, out warnings);
                    break;
                case IntoPlaneImportFields.TransTypes.Issue_Primary:
                case IntoPlaneImportFields.TransTypes.Issue_Secondary:
                    trans = CreateIssueTransaction(fields, parameters, site, out warnings);
                    break;
                case IntoPlaneImportFields.TransTypes.Load_Rack:
                    trans = CreateLoadRackTransaction(fields, parameters,site, out warnings);
                    break;
                case IntoPlaneImportFields.TransTypes.Notation_Rotation:
                    trans = CreateNotationTransaction(fields, parameters,site, out warnings);
                    break;
                default:
                    throw new IntoPlaneImportGeneralException("Invalid Transaction Type Number");

            }
            return trans;
        }

        private TransactionDO CreateBaseTransactionObject(IntoPlaneImportFields fields, IntoPlaneImportParametersDO parameters, SiteClass site, out string warnings)
        {
            warnings = string.Empty;
            string siteID = security.SiteID;
            SiteTimeConverter converter = new SiteTimeConverter(site);
            TransactionDO trans = new TransactionDO();
            bool bErrorCreatingTransaction = false;
            string strErrorMessage = "";
            double? vcf, temp, grav;
            double dGross;
            double dNet = 0.0;
            double? dMeterStart;
            double? dMeterStop;

            #region Set Main HeaderInfo
            trans.Site = site.SiteID;
            trans.SiteGuid = site.IdentityGuid;
            trans.TransID = "FMIP-" + fields.TransactionDate.Date.ToString("ddMMyy") + "-" + fields.ID;
            trans.Alias = fields.TransactionAlias; // TranslateAliasName(fields.TransactionAlias);
            trans.TransactionAliasGuid = GetAliasGuid(siteID, trans.Alias);
            
            if (trans.TransactionAliasGuid == Guid.Empty)
            {
                bErrorCreatingTransaction = true;
                strErrorMessage += IntoPlaneImportFieldNames.TransactionAlias + " [" + fields.TransactionAlias + "] not found." + Environment.NewLine;
            }
            var type = GetAliasType(siteID, trans.Alias);
            if ((int)type != fields.TransactionNumber)
            {
                bErrorCreatingTransaction = true;
                strErrorMessage += IntoPlaneImportFieldNames.TransactionAlias + " [" + fields.TransactionAlias + "] does not match Transaction Number." + Environment.NewLine;
            }
            trans.InventoryDate = fields.TransactionDate.Date;
            trans.TransactionDateTime = converter.ConvertFromSiteTime(fields.TransactionDate);
            trans.Notes += fields.Notes;
            if(trans.Notes.Length > 255 )
            {
                bErrorCreatingTransaction = true;
                strErrorMessage += IntoPlaneImportFieldNames.Notes + " exceeds max allowed 255 characters" + Environment.NewLine;
            }
            trans.Notes += Environment.NewLine + "INTOPLANE_NOTES:" + Environment.NewLine;
            trans.Notes += fields.IPRemarks;

            CompanyClass manager = GetCompanyByID(siteID, fields.Manager);
            CompanyClass owner = GetCompanyByID(siteID, fields.Owner);
            CompanyClass vendor = GetCompanyByID(siteID, fields.Vendor);
            CompanyClass customer = GetCompanyByID(siteID, fields.Customer);

            //If gate not found, should we reject?  Warn?
            GateClass gate = GetGate(siteID, fields.Gate);
            if (gate != null)
            {
                trans.GateID = gate.ID;
                trans.GateGuid = gate.IdentityGuid;
            }
            else
            {
                trans.GateID = fields.Gate;
                trans.GateGuid = Guid.Empty;
            }

            if (manager != null)
            {
                trans.ManagerID = manager.ID;
                trans.ManagerCode = manager.Code;
                trans.ManagerCompanyGuid = manager.MasterRecordGuid;
            }
            else
            {
                bErrorCreatingTransaction = true;
                strErrorMessage += IntoPlaneImportFieldNames.Manager + " [" + fields.Manager + "] not found." + Environment.NewLine;
            }
            if (owner != null)
            {
                trans.OwnerID = owner.ID;
                trans.OwnerCode = owner.Code;
                trans.OwnerCompanyGuid = owner.MasterRecordGuid;
            }
            else
            {
                bErrorCreatingTransaction = true;
                strErrorMessage += IntoPlaneImportFieldNames.Owner + " [" + fields.Owner + "] not found." + Environment.NewLine;

            }
            if (vendor != null)
            {
                trans.CarrierID = vendor.ID;
                trans.CarrierCode = vendor.Code;
                trans.CarrierCompanyGuid = vendor.MasterRecordGuid;
            }
            else
            {
                bErrorCreatingTransaction = true;
                strErrorMessage += IntoPlaneImportFieldNames.Vendor + " [" + fields.Vendor + "] not found." + Environment.NewLine;
            }
            if (customer != null)
            {
                trans.ShipToID = customer.ID;
                trans.ShipToCode = customer.Code;
                trans.ShipToCompanyGuid = customer.MasterRecordGuid;
            }
            else
            {
                bErrorCreatingTransaction = true;
                strErrorMessage += IntoPlaneImportFieldNames.Customer + " [" + fields.Customer + "] not found." + Environment.NewLine;
            }
            #endregion

            #region LineItems Info
            LineItemDO li = new LineItemDO();



            //fields.Product is ProductID not ProductCode
            ProductClass product = GetProductByID(siteID, fields.Product);
            if (product == null)
            {
                bErrorCreatingTransaction = true;
                strErrorMessage += IntoPlaneImportFieldNames.Product + " [" + fields.Product + "] not found." + Environment.NewLine;
            }
            else
            {
                li.Product = product.ID;
                li.ProductCode = product.Code;
                li.ProductGuid = product.MasterRecordGuid;
            }

            li.DocumentNumber = fields.TicketNumber;
            if (string.IsNullOrEmpty(li.DocumentNumber))
            {
                bErrorCreatingTransaction = true;
                strErrorMessage += IntoPlaneImportFieldNames.TicketNumber + "not found." + Environment.NewLine;
            }
            li.AcknowledgedDateTime = (fields.AckTime == null ? fields.AckTime : (DateTimeOffset?)converter.ConvertFromSiteTime(fields.AckTime.Value));
            li.OnLocationTime = (fields.ArrivalTime == null ? fields.ArrivalTime : (DateTimeOffset?)converter.ConvertFromSiteTime(fields.ArrivalTime.Value));

            //fields.CartRegistrationID is meterID, not equipmentId
            li.MeterID = fields.CartRegistrationID;
     
            //If destination is defined in system then use it.
            EquipmentClass destEQ = GetEquipment(siteID, fields.DestinationRegistrationID);
            if (destEQ != null)
            {
                li.DestinationEQ = new EquipmentDO(destEQ);
            }
            else
            {
                //If transaction is a load rack then report an error because the destination must be defined.
                if ((IntoPlaneImportFields.TransTypes)fields.TransactionNumber == IntoPlaneImportFields.TransTypes.Load_Rack)
                {
                    bErrorCreatingTransaction = true;
                    strErrorMessage += IntoPlaneImportFieldNames.DestinationRegistrationID + " [" + fields.DestinationRegistrationID + "] not found.  Load Rack Transactions Require Destination Reg ID to be defined in system" + Environment.NewLine;
                }
                else //Otherwise attempt to create destination using just ID provided in data file.
                {
                    EquipmentDO edo = new EquipmentDO();
                    li.DestinationEQ = edo;
                    edo.RegistrationID = fields.DestinationRegistrationID;
                    if (!fields.IsGSE)
                    {
                        edo.EquipmentModel = fields.UserData9;
                    }
                    else
                    {
                        edo.EquipmentModel = fields.GseCategoryID;
                    }
                }
            }

            li.DifferentialPressure = fields.DiffPressure;
            li.DualFuelingModeFlag = fields.DualFueling;
            li.MobileDeviceID = fields.DeviceID;
            li.HydrantPressure = fields.HydrantPressure;
            li.FlowRate = fields.FlowRate;
            li.FreezePoint = fields.FreezePoint;

            dMeterStart = fields.MeterStart;
            dMeterStop = fields.MeterStop;


            //Logic for VCF/Temp/Gravity & Quantity
            var tempGravityVcfParam = parameters.GetTempGravityVCFParam(fields.Product);
            if (parameters.UseTempGravVCFParam && tempGravityVcfParam != null)
            {
                //use values entered in form.  
                // We should only get VCF or (Temperture and Gravity)
                vcf = tempGravityVcfParam.VCF;
                temp = tempGravityVcfParam.Temperature;
                grav = tempGravityVcfParam.Gravity;
            }
            else if (fields.Temperature.HasValue && fields.Gravity.HasValue && fields.Gravity.Value != 0)
            {
                //use temp and gravity values from file
                vcf = null;
                temp = fields.Temperature;
                grav = fields.Gravity;
            }
            else if (fields.VCF.HasValue && fields.VCF != 0)
            {
                //use VCF value from file
                vcf = fields.VCF;
                temp = null;
                grav = null;
            }
            else
            {
                // No values set but may be "Net Volume Indicator" set in file
                vcf = null;
                temp = null;
                grav = null;
            }

            bool hasCalculateVcfError = false;

            if ((!vcf.HasValue || vcf.Value == 0) && temp.HasValue && grav.HasValue && grav.Value != 0.0 && product != null)
            {
                try
                {
                    double convertedTemp = temp.Value;
                    double convertedDensity = grav.Value;
                    var tempUnits = EngineeringUnit.FmtDegC;
                    var densityUnits = EngineeringUnit.FmdKgM3;
                    var standardPressureUnits = product.PressureUnits == EngineeringUnit.FmSiteUnits ? site.PressureUnits : product.PressureUnits;
                    var stdTemp = FMBusinessObjects.DataObjects.VcfModuleSettings.GetStandardTemperature(product._VcfModuleSettings.CorrectionMethodType, product._VcfModuleSettings.CorrectionMethodSpecific);

                    if (!stdTemp.Contains("°C"))
                    {
                        tempUnits = EngineeringUnit.FmtDegF;
                        densityUnits = EngineeringUnit.FmdUsLbGal;
                        // Import temp is Deg C and density KgM3  as defined in import specification but need to be in units match products std temp
                        convertedTemp = EngineeringUnits.Convert(temp.Value, EngineeringUnit.FmtDegC, tempUnits, 0);
                        convertedDensity = EngineeringUnits.Convert(grav.Value, EngineeringUnit.FmdKgM3, densityUnits, 0);
                    }

                    //calculate Vcf from temp/grav entered in file or form
                    vcf = Vcf.CalculateVcf((ECorrectionTypeMajor)Convert.ToInt32(product._VcfModuleSettings.CorrectionMethodType),
                                            (ECorrectionTypeMinor)Convert.ToInt32(product._VcfModuleSettings.CorrectionMethodSpecific),
                                            convertedTemp, // measured temp from UI or csv file
                                            tempUnits,
                                            product._VcfModuleSettings.BaseTemperature.Value,
                                            tempUnits,
                                            convertedDensity, // standard Gravity from UI or csv file
                                            densityUnits,
                                            0.0,
                                            standardPressureUnits,
                                            0.0,
                                            tempUnits,
                                            0.0,
                                            standardPressureUnits,
                                            new[] { product.CorrectionFactor0, product.CorrectionFactor1, product.CorrectionFactor2, product.CorrectionFactor3, product.CorrectionFactor4 });
                }
                catch (Exception ex) {
                    bErrorCreatingTransaction = true;
                    strErrorMessage += ex.Message+Environment.NewLine;
                    hasCalculateVcfError = true;
                }

            }


            //------------------------------------------------------------------------------------------------------------
            //The gross and net volume fields in the Daily Fuel Transaction File are in metric units (liters) 
            //regardless of what the site's units are in 7.1. If you need to calculate volume from meter start/meter stop 
            //values you will need to take into account the site's volume units.
            //------------------------------------------------------------------------------------------------------------

            ////Check for meter rollover and calculate Net and Gross.  These values may be overridden by the cases below
            //double dGrossTemp = 0.0;
            //dGrossTemp = dMeterStop.Value - dMeterStart.Value;
            //if (dGrossTemp < 0)
            //{
            //    dGrossTemp = (GetMaxNumber(meter?.NumberOfDigits ?? 0) - dMeterStart.Value) + dMeterStop.Value + 1;
            //}

            dGross = fields.GrossVolume / 1000;


            //Net Volume
            if (fields.NetVolumeIndicator)
            {
                if (fields.NetVolume.HasValue)
                {
                    dNet = (double)fields.NetVolume / 1000;
                }
                else
                {
                    bErrorCreatingTransaction = true;
                    strErrorMessage += "Net Volume Indicator set without Net Volume" + Environment.NewLine;
                }

            }
            else
            {
                if (vcf.HasValue)
                {
                    dNet = dGross * vcf.Value * (fields.FuelCp ?? 1.0) * (fields.MeterFactor ?? 1.0);
                }
                else if(!hasCalculateVcfError) // dont add second error messages for same issue
                {
                    bErrorCreatingTransaction = true;
                    strErrorMessage += "VCF not set or calculated from Temperature/Density" + Environment.NewLine;
                }

            }

            //Create new GA Defuel import to Accounting if there is a value in UserData23
            if (!string.IsNullOrEmpty(fields.UserData23) && vcf.HasValue)
            {
                double dTempVol;
                if (double.TryParse(fields.UserData23, out dTempVol))
                {
                    li.UserData23 = dGross.ToString();
                    dGross = dTempVol / 1000;

                    //Calculate Net
                    dNet = dGross * vcf.Value * (fields.FuelCp ?? 1.0) * (fields.MeterFactor ?? 1.0);
                }
            }

            //DeIce Transactions
            //recalculate some values for deice transactions
            if (fields.TransactionSubtypeCode2 == "De-Ice") //Do we have SubTypeCodes defined anywhere?
            {
                int nTransactionType = 5;  //default value.  Do we have de-ice transaction type defined per site anywhere?

                //	this is an issue transaction for deice
                //	for Deice transactions UserData6 is also the percentage of blend
                double dDeIceBlendRatio = 0.0;
                double.TryParse(fields.UserData6, out dDeIceBlendRatio);

                dDeIceBlendRatio = dDeIceBlendRatio / 100.0;
                // for deice/issue/blended set meter start/stop to zero
                // gross and net includes water for blend
                double dRequired = dGross;
                double dArrival = dNet;
                if (5 == nTransactionType && product?.ProductType == ProductType.BlendProduct)
                {
                    dMeterStart = 0.0;
                    dMeterStop = 0.0;
                }
                // for water mixed products, gross and net are only the product portion
                dGross *= dDeIceBlendRatio;
                dNet *= dDeIceBlendRatio;
            }

            //Net Accounting - Check for configuration mismatch
            //Display a error in the output from the import process that explains that there is a mismatch between the Net Indicator and the Product's Net Accounting configuration
            //Write a note into the Notes field that explains the same information

            //TO DO: Update product configuration UI and Class for Net Accounting configuration
            //if (!fields.NetVolumeIndicator && product.NetAccounting)
            //{
            //    csaProductNetAccountingConfig.Add(csticketnumber);
            //    CString csNetAccounting;
            //    csNetAccounting.LoadString(IDS_PRODUCT_NET_ACCOUNTING_NOTE);
            //    if (csnotes != _T(""))
            //        csnotes += "\r\n";
            //    csnotes += csNetAccounting;
            //}

            li.Density = grav;
            li.DensityUnits = fields.DensityUnits;
            li.Temperature = temp;
            li.TemperatureUnits = EngineeringUnit.FmtDegC; // Defined in import specification
            li.TemperatureQualityStatus = fields.TempQualityStatus;
            li.VCF = vcf;

            li.Quantity = new QuantityDO(dGross, dNet, 0.0, 0.0);

            li.VolumeUnits = EngineeringUnit.FmvLitre; // Defined in import specification
            li.PartialFill = fields.PartialFill;
            li.MeterReading = new MeterReadingDO();
            li.MeterReading.MeterFactor = fields.MeterFactor;
            li.MeterReading.MeterStart = dMeterStart;
            li.MeterReading.MeterStop = dMeterStop;
            li.MeterReading.StartDateTime = (fields.FuelingStartTime == null ? fields.FuelingStartTime : (DateTimeOffset?)converter.ConvertFromSiteTime(fields.FuelingStartTime.Value)); //converter.ConvertFromSiteTime(fields.FuelingStartTime);
            li.MeterReading.StopDateTime = (fields.FuelingStopTime == null ? fields.FuelingStopTime : (DateTimeOffset?)converter.ConvertFromSiteTime(fields.FuelingStopTime.Value)); //converter.ConvertFromSiteTime(fields.FuelingStopTime);
            li.MeterStartObtainedAutomaticallyFlag = fields.MeterStartReadFromDCU;
            li.MeterStopObtainedAutomaticallyFlag = fields.MeterStopReadFromDCU;
            li.FuelCompressionFactor = fields.FuelCp;
            li.OperatorID = fields.Operator;
            li.OperatorPersonnelGuid = GetPersonMasterRecordGuid(siteID, fields.Operator);

            li.NetVolumeIndicator = fields.NetVolumeIndicator;

            if (!ValidateAndSetUserData(fields, trans, out string warning, out string errors))
            {
                bErrorCreatingTransaction = true;
                strErrorMessage += errors;
            }
            else
            {
                warnings += warning;
            }
            trans.LineItems.Add(li);


            #endregion

            #region Set RoutInfoDO
            RouteInfoDO route = new RouteInfoDO();
            trans.RouteInfo = route;
            route.FinalStationIATAID = fields.UserData1_DestinationID;
            route.NextStationIATAID = fields.UserData1_DestinationID;
            route.PreviousRoutingID = fields.OriginID;
            route.OriginStationIATAID = fields.OriginID;
            route.RouteOriginationDate = fields.OriginTime;
            route.RoutingID = fields.SerialNumber_FlightNumber;
            route.PreviousRoutingID = fields.ArrivalFlightID;
            route.InternationalRouteIndicator = fields.FTZ;
            #endregion

            #region Set RoutScheduleDO
            RouteScheduleDO schDO = new RouteScheduleDO();
            trans.RouteSchedule = schDO;
            schDO.ETA = (fields.ETA == null ? fields.ETA : (DateTimeOffset?)converter.ConvertFromSiteTime(fields.ETA.Value)); //converter.ConvertFromSiteTime(fields.ETA);
            schDO.ETD = (fields.ETD == null ? fields.ETD : (DateTimeOffset?)converter.ConvertFromSiteTime(fields.ETD.Value)); //converter.ConvertFromSiteTime(fields.ETD);
            schDO.STD = (fields.STD == null ? fields.STD : (DateTimeOffset?)converter.ConvertFromSiteTime(fields.STD.Value)); //converter.ConvertFromSiteTime(fields.STD);
            schDO.STA = (fields.STA == null ? fields.STA : (DateTimeOffset?)converter.ConvertFromSiteTime(fields.STA.Value)); //converter.ConvertFromSiteTime(fields.STA);
            schDO.SFT = (fields.SFT == null ? fields.SFT : (DateTimeOffset?)converter.ConvertFromSiteTime(fields.SFT.Value)); //converter.ConvertFromSiteTime(fields.SFT);
            #endregion

            #region Set WeightReadingDO
            for (int iIndex = 1; iIndex <= fields.TankCount; iIndex++)
            {
                WeightReadingDO wrdo = new WeightReadingDO();
                wrdo.CompartmentName = fields.ArrivalTankName(iIndex);
                wrdo.BeginQuantity = fields.ArrivalTankValue(iIndex);
                wrdo.RequestedQuantity = fields.DesiredTankValue(iIndex);
                wrdo.FinalQuantity = fields.FinalTankValue(iIndex);
                wrdo.VolumetricTopOffFlag = fields.FinalTankShutoffInd(iIndex);
                trans.WeightReadings.Add(wrdo);
            }

            #endregion


            if (bErrorCreatingTransaction)
            {
                throw new IntoPlaneImportGeneralException(strErrorMessage);
            }

            return trans;
        }

        private static string ValidateUserData(string ud,string  ID)
        {
            if (ud.Length > MAX_USER_DATA_LENGTH)
            {
                return ID + " exceeds max length of " + MAX_USER_DATA_LENGTH + " charaters"+ Environment.NewLine;
            }
            return string.Empty;
        }

        private static bool ValidateAndSetUserData(IntoPlaneImportFields fields, TransactionDO trans, out string warnings, out string errors)
        {
            warnings = string.Empty;
            errors = string.Empty;
            bool isDefuelOrIssue = false;
            switch ((IntoPlaneImportFields.TransTypes)fields.TransactionNumber)
            {
                case IntoPlaneImportFields.TransTypes.Defuel_Primary:
                case IntoPlaneImportFields.TransTypes.Defuel_Secondary:
                case IntoPlaneImportFields.TransTypes.Issue_Primary:
                case IntoPlaneImportFields.TransTypes.Issue_Secondary:
                    isDefuelOrIssue = true;
                    break;
            }

            // Transaction UserData properties setter adds items to dictionary and we dont want keys for empty values
            // as if there are no keys we dont create row in DB table [tblTransactionUserData]
            // Should we be validating UserData formats?
            var ud = fields.UserData1_DestinationID;
            if (!string.IsNullOrWhiteSpace(ud))
            {
                errors += ValidateUserData(ud, IntoPlaneImportFieldNames.UserData1_DestinationID);
                trans.UserData1 = ud;
            }

            ud = fields.UserData2_GateID;
            if (!string.IsNullOrWhiteSpace(ud))
            {
                errors += ValidateUserData(ud, IntoPlaneImportFieldNames.UserData2_GateID);
                trans.UserData2 = ud;
            }

            ud = fields.UserData3_Operator;
            if (!string.IsNullOrWhiteSpace(ud))
            {
                errors += ValidateUserData(ud, IntoPlaneImportFieldNames.UserData3_Operator);
                trans.UserData3 = ud;
            }

            ud = fields.UserData4;
            if (!string.IsNullOrWhiteSpace(ud))
            {
                errors += ValidateUserData(ud, IntoPlaneImportFieldNames.UserData4);
                trans.UserData4 = ud;
            }

            ud = fields.UserData5;
            if (!string.IsNullOrWhiteSpace(ud))
            {
                errors += ValidateUserData(ud, IntoPlaneImportFieldNames.UserData5);
                trans.UserData5 = ud;
            }
      
            ud = fields.UserData6;
            if (!string.IsNullOrWhiteSpace(ud))
            {
                errors += ValidateUserData(ud, IntoPlaneImportFieldNames.UserData6);
                trans.UserData6 = ud;
            }
   
            ud = fields.UserData7;
            if (!string.IsNullOrWhiteSpace(ud))
            {
                errors += ValidateUserData(ud, IntoPlaneImportFieldNames.UserData7);
                trans.UserData7 = ud;
                if(isDefuelOrIssue && !TIME_FORMAT.IsMatch(ud))
                { 
                    warnings += IntoPlaneImportFieldNames.UserData7 + " does not match format HH:MM" + Environment.NewLine;
                }
            }

            ud = fields.UserData8;
            if (!string.IsNullOrWhiteSpace(ud))
            {
                errors += ValidateUserData(ud, IntoPlaneImportFieldNames.UserData8);
                trans.UserData8 = ud;
                if (isDefuelOrIssue && !TIME_FORMAT.IsMatch(ud))
                {
                    warnings += IntoPlaneImportFieldNames.UserData8 + " does not match format HH:MM" + Environment.NewLine;
                }
            }
           
            ud = fields.UserData9;
            if (!string.IsNullOrWhiteSpace(ud))
            {
                errors += ValidateUserData(ud, IntoPlaneImportFieldNames.UserData9);
                trans.UserData9 = ud;
            }

            ud = fields.UserData10;
            if (!string.IsNullOrWhiteSpace(ud))
            {
                errors += ValidateUserData(ud, IntoPlaneImportFieldNames.UserData10);
                trans.UserData10 = ud;
            }

            ud = fields.UserData11;
            if (!string.IsNullOrWhiteSpace(ud))
            {
                errors += ValidateUserData(ud, IntoPlaneImportFieldNames.UserData11);
                trans.UserData11 = ud;
            }

            ud = fields.UserData12;
            if (!string.IsNullOrWhiteSpace(ud))
            {
                errors += ValidateUserData(ud, IntoPlaneImportFieldNames.UserData12);
                trans.UserData12 = ud;
            }

            ud = fields.UserData13;
            if (!string.IsNullOrWhiteSpace(ud))
            {
                errors += ValidateUserData(ud, IntoPlaneImportFieldNames.UserData13);
                trans.UserData13 = ud;
            }

            ud = fields.UserData14;
            if (!string.IsNullOrWhiteSpace(ud))
            {
                errors += ValidateUserData(ud, IntoPlaneImportFieldNames.UserData14);
                trans.UserData14 = ud;
            }
    
            ud = fields.UserData15;
            if (!string.IsNullOrWhiteSpace(ud))
            {
                errors += ValidateUserData(ud, IntoPlaneImportFieldNames.UserData15);
                trans.UserData15 = ud;
                if (isDefuelOrIssue && !DATE_FORMAT.IsMatch(ud))
                {
                    warnings += IntoPlaneImportFieldNames.UserData15 + " does not match format MM/DD/YYYY"+Environment.NewLine;
                }
            }

            ud = fields.UserData16;
            if (!string.IsNullOrWhiteSpace(ud))
            {
                errors += ValidateUserData(ud, IntoPlaneImportFieldNames.UserData16);
                trans.UserData16 = ud;
            }

            ud = fields.UserData17;
            if (!string.IsNullOrWhiteSpace(ud))
            {
                errors += ValidateUserData(ud, IntoPlaneImportFieldNames.UserData17);
                trans.UserData17 = ud;
            }

            ud = fields.UserData18;
            if (!string.IsNullOrWhiteSpace(ud))
            {
                errors += ValidateUserData(ud, IntoPlaneImportFieldNames.UserData18);
                trans.UserData18 = ud;
            }

            ud = fields.UserData19;
            if (!string.IsNullOrWhiteSpace(ud))
            {
                errors += ValidateUserData(ud, IntoPlaneImportFieldNames.UserData19);
                trans.UserData19 = ud;
            }

            ud = fields.UserData20;
            if (!string.IsNullOrWhiteSpace(ud))
            {
                errors += ValidateUserData(ud, IntoPlaneImportFieldNames.UserData20);
                trans.UserData20 = ud;
            }

            ud = fields.UserData21;
            if (!string.IsNullOrWhiteSpace(ud))
            {
                errors += ValidateUserData(ud, IntoPlaneImportFieldNames.UserData21);
                trans.UserData21 = ud;
            }

            ud = fields.UserData22_OriginID;
            if (!string.IsNullOrWhiteSpace(ud))
            {
                errors += ValidateUserData(ud, IntoPlaneImportFieldNames.UserData22_OriginID);
                trans.UserData22 = ud;
            }

            ud = fields.UserData23;
            if (!string.IsNullOrWhiteSpace(ud))
            {
                errors += ValidateUserData(ud, IntoPlaneImportFieldNames.UserData23);
                trans.UserData23 = ud;
            }

            ud = fields.UserData24;
            if (!string.IsNullOrWhiteSpace(ud))
            {
                errors += ValidateUserData(ud, IntoPlaneImportFieldNames.UserData24);
                trans.UserData24 = ud;
            }
            return errors.Length == 0;
        }

        private TransactionDO CreateDefuelTransaction(IntoPlaneImportFields fields, IntoPlaneImportParametersDO parameters, SiteClass site, out string warnings)
        {
            TransactionDO trans = CreateBaseTransactionObject(fields,parameters,site, out warnings);
            return trans;
        }
        private TransactionDO CreateIssueTransaction(IntoPlaneImportFields fields, IntoPlaneImportParametersDO parameters, SiteClass site, out string warnings)
        {
            TransactionDO trans = CreateBaseTransactionObject(fields, parameters, site, out warnings);
            return trans;
        }
        private TransactionDO CreateLoadRackTransaction(IntoPlaneImportFields fields, IntoPlaneImportParametersDO parameters, SiteClass site, out string warnings)
        {
            TransactionDO trans = CreateBaseTransactionObject(fields, parameters, site, out warnings);
            return trans;
        }
        private TransactionDO CreateNotationTransaction(IntoPlaneImportFields fields, IntoPlaneImportParametersDO parameters, SiteClass site, out string warnings)
        {
            TransactionDO trans = CreateBaseTransactionObject(fields, parameters, site,out warnings);
            return trans;
        }
        private SaveTransactionsResultDO SaveTransactions(SecurityClass security)
        {
            if (transactionList.Count == 0)
                return new SaveTransactionsResultDO();
            // The save transactions processor relies on guids being present to determine whether to insert, update, or delete. We must populate the guids
            // before we save the transactions, or we'll end up always inserting new records.
            List<TransactionDO> transactionsWithPrimaryKeys = FMChannelHelper.MakeCall<ITransactionImportProcessor, List<TransactionDO>>(
                                                                     importProcessor => importProcessor.PopulateKeyTransactionGuids(security, transactionList));

            SaveTransactionsSR sr = new SaveTransactionsSR
            {
                Security = security,
                CurrentSiteGuid = security.SiteGuid,
                ConvertUnits = false,
                UseAutoComplete = true,
                Transactions = transactionsWithPrimaryKeys,
                BypassValidation = true
            };

            SaveTransactionsResultDO result = FMChannelHelper.MakeCall<ISaveTransactionsProcessor, SaveTransactionsResultDO>(
                                                        saveTransactionsProcessor =>
                                                        saveTransactionsProcessor.SaveTransactions(sr));

            return result;
        }
        private void GetLookupTables()
        {
            this.gateTable = new Dictionary<string, Dictionary<string, GateClass>>();
            this.companyTable = new Dictionary<string, Dictionary<string, CompanyClass>>();
            this.productTable = new Dictionary<string, Dictionary<string, ProductClass>>();
            this.equipmentTable = new Dictionary<string, Dictionary<string, EquipmentClass>>();
            this.meterTable = new Dictionary<string, Dictionary<string, MeterClass>>();
            this.tankTable = new Dictionary<string, Dictionary<string, Guid>>();
            this.transactionAliasTable = new Dictionary<string, Dictionary<string, Guid>>();
            this.transactionAliasTypeTable = new Dictionary<string, Dictionary<string, TransactionTypes>>();
            this.personnelTable = new Dictionary<string, Dictionary<string, Guid>>();
            this.stationTable = new Dictionary<string, Dictionary<string, Guid>>();
            this.transactionAliasUserFieldsTable = new Dictionary<string, Dictionary<Guid, Dictionary<string, string>>>();
            this.closeoutTable = new Dictionary<string, List<CloseoutDO>>();
            this.forceCloseoutTable = new Dictionary<string, GeneralConfigDO>();

            // Get a list of strings that are "UserData" + x where x is a number 1 through the maximum number of user data fields
            // This is used when populating user data on the transaction record.
            //this.defaultUserDataFieldNames = Enumerable.Range(1, 24).Select(number => DefaultUserDataFieldPrefix + number.ToString(CultureInfo.InvariantCulture)).ToList();

            Guid originalSiteGuid = this.security.SiteGuid;

            SiteClass parentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(sites => sites.Get(this.security, originalSiteGuid, true, false, false));
            SiteCollectionClass siteCollection = FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(sites => sites.EnumerateByParentSite(security, parentSite.IdentityGuid));
            this.siteLookupTable = new Dictionary<string, SiteClass>() 
            { 
                { parentSite.SiteID, parentSite }
            };

            this.siteTable = new Dictionary<string, Guid>
				                                    {
					                                    { parentSite.SiteID, parentSite.SiteGuid }
				                                    };

            foreach (SiteToSiteMapClass siteToSiteMap in parentSite.SiteToSiteMapCollection)
            {
                if (!this.siteTable.ContainsKey(siteToSiteMap.ChildSiteID))
                {
                    this.siteTable.Add(siteToSiteMap.ChildSiteID, siteToSiteMap.ChildSiteGuid);
                }
                
            }

            foreach(SiteClass site in siteCollection)
            {
                if (!siteLookupTable.ContainsKey(site.SiteID))
                {
                    this.siteLookupTable.Add(site.SiteID, site);
                }
            }


            foreach (KeyValuePair<string, Guid> site in this.siteTable)
            {
                this.security.SiteGuid = site.Value;

                // Get Stations (loading locations) for the site.
                StationCollectionClass stationList = EnumerateStations(this.security);
                Dictionary<string, Guid> siteStations = stationList.ToDictionary(station => station.ID, station => station.IdentityGuid);

                this.stationTable.Add(site.Key, siteStations);

                // Get personnel for the site.
                IEnumerable<PersonClass> personnelList = EnumeratePersonnel(this.security);
                Dictionary<string, Guid> sitePersonnel = personnelList.ToDictionary(person => person.ID, person => person.MasterRecordGuid);

                this.personnelTable.Add(site.Key, sitePersonnel);

                // Get gates for the site
                GateCollectionClass gates = EnumerateGates(security);
                Dictionary<string, GateClass> siteGates = gates.ToDictionary(gate => gate.ID, gate => gate);
                gateTable.Add(site.Key, siteGates);

                // Get aliases for the site.
                TransactionAliasNameCollectionClass aliasList = EnumerateTransactionAliases(this.security);
                Dictionary<string, Guid> siteTransactionAliases = aliasList.ToDictionary(alias => alias.AliasName, alias => alias.MasterRecordGuid);

                this.transactionAliasTable.Add(site.Key, siteTransactionAliases);
                Dictionary<string, TransactionTypes> aliasTypes = aliasList.ToDictionary(alias => alias.AliasName, alias => alias.TransTypeID);
                transactionAliasTypeTable.Add(site.Key, aliasTypes);
                // Get alias fields for the site
                List<Guid> transactionAliasGuids = aliasList.Select(transactionAliasName => transactionAliasName.IdentityGuid).ToList();

                Dictionary<Guid, Dictionary<string, string>> userDataFields = EnumerateTransactionAliasesUserDataFields(this.security, transactionAliasGuids);

                this.transactionAliasUserFieldsTable.Add(site.Key, userDataFields);

                // Get Companies for the Site. Lookups for companies are case-insensitive to reduce the number of FMAE Translations required.
                CompanyCollectionClass companyList = EnumerateCompanies(this.security);
                Dictionary<string, CompanyClass> siteCompanies = companyList.ToDictionary(company => company.ID, StringComparer.OrdinalIgnoreCase);

                this.companyTable.Add(site.Key, siteCompanies);

                // Get Products for the Site. Lookups for products are case-insensitive to reduce the number of FMAE Translations required.
                ProductCollectionClass productList = EnumerateProducts(this.security);
                Dictionary<string, ProductClass> siteProducts = productList.ToDictionary(product => product.ID, StringComparer.OrdinalIgnoreCase);

                this.productTable.Add(site.Key, siteProducts);

                // Get Equipment for the Site
                List<EquipmentClass> equipmentList = EnumerateEquipment(this.security);

                Dictionary<string, EquipmentClass> siteEquipment = equipmentList.ToDictionary(equipment => equipment.ID, equipment => equipment);

                this.equipmentTable.Add(site.Key, siteEquipment);

                //Get Meters for the Site
                List<MeterClass> meterList = _metersProxy.Enumerate().ToList<MeterClass>();

                Dictionary<string, MeterClass> siteMeters = meterList.ToDictionary(meter => meter.ID, meter => meter);

                this.meterTable.Add(site.Key, siteMeters);

                // Get Tanks for the Site
                TankCollectionClass tankList = EnumerateTanks(this.security);
                Dictionary<string, Guid> siteTanks = tankList.ToDictionary(tank => tank.ID, tank => tank.IdentityGuid);

                this.tankTable.Add(site.Key, siteTanks);

                GeneralConfigDO accountingConfig =
                    FMChannelHelper.MakeCall<ITransactionValidator, GeneralConfigDO>(
                        validator => validator.GetForcedCloseout(this.security, this.security.SiteID, this.security.SiteGuid));

                this.forceCloseoutTable.Add(site.Key, accountingConfig);

                List<Guid> productNamelist = productList.Select(product => product.MasterRecordGuid).ToList();

                List<CloseoutDO> siteCloseouts =
                    FMChannelHelper.MakeCall<ITransactionValidator, List<CloseoutDO>>(
                        validator => validator.GetCloseoutDates(this.security, this.security.SiteID, this.security.SiteGuid, Guid.Empty, productNamelist));

                this.closeoutTable.Add(site.Key, siteCloseouts);
            }

            this.security.SiteGuid = originalSiteGuid;

            // Translations do not depend on a particular site so we only need to get them once
            this.companyTranslationTable = EnumerateTranslationValues(this.security, FMAETranslationType.Company);

            this.productTranslationTable = EnumerateTranslationValues(this.security, FMAETranslationType.Product);
        }

        #region Methods to Enumerate Reference Data
        private static GateCollectionClass EnumerateGates(SecurityClass securityClass)
        {
            return FMChannelHelper.MakeCall<IGates, GateCollectionClass>(gates => gates.Enumerate(securityClass));
        }

        private static StationCollectionClass EnumerateStations(SecurityClass securityClass)
        {
            return FMChannelHelper.MakeCall<IStations, StationCollectionClass>(stations => stations.Enumerate(securityClass));
        }

        private static IEnumerable<PersonClass> EnumeratePersonnel(SecurityClass securityClass)
        {
            return FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(personnel => personnel.EnumerateBasicInformationOnly(securityClass));
        }

        private static TransactionAliasNameCollectionClass EnumerateTransactionAliases(SecurityClass securityClass)
        {
            return FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasNameCollectionClass>(aliasNames => aliasNames.EnumerateNamesOnly(securityClass, false));
        }
        /// <summary>
        /// Get transaction alias user data fields. These are sometimes needed if the aviation client provides user data
        /// </summary>
        /// <param name="securityClass">Contains Security Information</param>
        /// <param name="transactionAliasGuids">A list of alias guids to retrieve user data fields for</param>
        /// <returns>User data fields for the provided site</returns>
        private static Dictionary<Guid, Dictionary<string, string>> EnumerateTransactionAliasesUserDataFields(SecurityClass securityClass, List<Guid> transactionAliasGuids)
        {
            Dictionary<Guid, Dictionary<string, string>> siteUserDataFields = new Dictionary<Guid, Dictionary<string, string>>();

            // For every alias guid provided, get the user data fields
            foreach (Guid transactionAliasGuid in transactionAliasGuids)
            {
                Guid localTransactionAliasGuid = transactionAliasGuid;

                UserDataFieldCollectionClass transactionAliasUserDataFields = FMChannelHelper.MakeCall<IUserDataFields, UserDataFieldCollectionClass>(
                    userDataFields =>
                    userDataFields.EnumerateByEntityType(securityClass, ENTITY_TYPE.TRANSACTION_ALIAS, localTransactionAliasGuid, false, false));

                // Add each user data field to a collection for the alias, 
                // and then add that collection to a collection of aliases for the site
                Dictionary<string, string> userDataFieldValues = new Dictionary<string, string>();
                foreach (UserDataFieldClass userDataField in transactionAliasUserDataFields)
                {

                    if (!userDataFieldValues.ContainsKey(userDataField.DisplayName))
                    {
                        userDataFieldValues.Add(userDataField.DisplayName, userDataField.DbName);
                    }

                }
                siteUserDataFields.Add(localTransactionAliasGuid, userDataFieldValues);
            }

            return siteUserDataFields;
        }

        private static TankCollectionClass EnumerateTanks(SecurityClass securityClass)
        {
            return FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(tanks => tanks.EnumerateBasicInformation(securityClass));
        }

        private static List<EquipmentClass> EnumerateEquipment(SecurityClass securityClass)
        {
            return FMChannelHelper.MakeCall<IEquipments, EquipmentCollectionClass>(equipments => equipments.Enumerate(securityClass)).ToList();
        }

        private static List<MeterClass> EnumerateMeters(SecurityClass securityClass)
        {
            //To get this to work correctly the various Enumerate methods in the MeterClass will have to be refactored.
            return FMChannelHelper.MakeCall<IMeters, List<MeterClass>>(meters => meters.Enumerate(securityClass)).ToList();
        }
            

        private static ProductCollectionClass EnumerateProducts(SecurityClass securityClass)
        {
            return FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(products => products.EnumerateByFilterAndLocalize(securityClass, string.Empty, false));
        }

        private static CompanyCollectionClass EnumerateCompanies(SecurityClass securityClass)
        {
            return FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(companies => companies.EnumerateByRoleGetIDCodeTypesIdentityGuidOnly(securityClass, null));
        }
        
        /// <summary>
        /// Return a dictionary of translation values that have been defined for the specified type of entity
        /// </summary>
        /// <param name="securityClass">Contains security information</param>
        /// <param name="translationType">The type of translations to retrieve, e.g. company or product</param>
        /// <returns>A dictionary of translation values that have been defined for the specified type of entity</returns>
        private static Dictionary<string, Guid> EnumerateTranslationValues(SecurityClass securityClass, FMAETranslationType translationType)
        {
            List<FMAETranslation> translations = FMChannelHelper.MakeCall<IFMAETranslations, List<FMAETranslation>>(translationsClient => translationsClient.Enumerate(securityClass, translationType));

            Dictionary<string, Guid> translationTable = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

            foreach (FMAETranslation translation in translations)
            {
                if (!translationTable.ContainsKey(translation.ID))
                {
                    translationTable.Add(translation.ID, translation.EntityGuid);
                }
            }

            return translationTable;
        }
        #endregion

        #region Methods to Look up values in Reference Data

        /// <summary>
        /// Is the field name provided one of the default user data field names like "UserData" + "1" through "24"?
        /// </summary>
#pragma warning disable 1584,1711,1572,1581,1580
        /// <param name="fieldName">The field name to check</param>
#pragma warning restore 1584,1711,1572,1581,1580
        /// <returns>True if the field name provided is one of the default user data field names</returns>
        //public bool IsDefaultUserDataFieldName(string fieldName)
        //{
        //    return this.defaultUserDataFieldNames.Contains(fieldName);
        //}

        /// <summary>
        /// This method will return the station guid for the given site and
        /// station ID (loading location).
        /// </summary>
        /// <param name="siteID"></param>
        /// <param name="stationID"></param>
        /// <returns></returns>
        public Guid GetStationGuid(string siteID, string stationID)
        {
            if (this.stationTable != null && !string.IsNullOrEmpty(siteID) && !string.IsNullOrEmpty(stationID))
            {
                Dictionary<string, Guid> siteStations;
                Guid stationGuid;

                if (this.stationTable.TryGetValue(siteID, out siteStations) && siteStations.TryGetValue(stationID, out stationGuid))
                {
                    return stationGuid;
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// This method will return the person guid for the given site and
        /// person ID.
        /// </summary>
        /// <param name="siteID"></param>
        /// <param name="personID"></param>
        /// <returns></returns>
        public Guid GetPersonMasterRecordGuid(string siteID, string personID)
        {
            if (this.personnelTable != null && !string.IsNullOrEmpty(siteID) && !string.IsNullOrEmpty(personID))
            {
                Dictionary<string, Guid> sitePersonnel;
                Guid personnelMasterRecordGuid;

                if (this.personnelTable.TryGetValue(siteID, out sitePersonnel) && sitePersonnel.TryGetValue(personID, out personnelMasterRecordGuid))
                {
                    return personnelMasterRecordGuid;
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Return the site guid for the provided site ID
        /// </summary>
        /// <param name="siteID">The siteID to get the SiteGuid for</param>
        /// <returns>The siteGuid corresponding to the provided site ID</returns>
        public Guid GetSiteGuid(string siteID)
        {
            if (this.siteTable != null && !string.IsNullOrEmpty(siteID))
            {
                Guid siteGuid;

                if (this.siteTable.TryGetValue(siteID, out siteGuid) && siteGuid != Guid.Empty)
                {
                    return siteGuid;
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Return the transaction alias guid for the provided transaction alias ID
        /// </summary>
        /// <param name="siteID">The site that the transaction belongs to</param>
        /// <param name="aliasID">The transaction alias ID to look up</param>
        /// <returns>The TransactionAliasGuid corresponding to the provided transaction alias ID</returns>
        public Guid GetAliasGuid(string siteID, string aliasID)
        {
            if (this.transactionAliasTable != null && !string.IsNullOrEmpty(siteID) && !string.IsNullOrEmpty(aliasID))
            {
                Dictionary<string, Guid> siteTransactionAliasNames;
                Guid transactionAliasMasterRecordGuid;

                if (this.transactionAliasTable.TryGetValue(siteID, out siteTransactionAliasNames) && siteTransactionAliasNames.TryGetValue(aliasID, out transactionAliasMasterRecordGuid)
                    && transactionAliasMasterRecordGuid != Guid.Empty)
                {
                    return transactionAliasMasterRecordGuid;
                }
            }
            return Guid.Empty;
        }

        /// <summary>
        /// Return the transaction alias type for the provided transaction alias ID
        /// </summary>
        /// <param name="siteID">The site that the transaction belongs to</param>
        /// <param name="aliasID">The transaction alias ID to look up</param>
        /// <returns>The TransactionAliasGuid corresponding to the provided transaction alias ID</returns>
        public TransactionTypes GetAliasType(string siteID, string aliasID)
        {
            if (this.transactionAliasTypeTable != null && !string.IsNullOrEmpty(siteID) && !string.IsNullOrEmpty(aliasID))
            {
                Dictionary<string, TransactionTypes> siteTransactionAliasType;
                TransactionTypes transactionAliasType;

                if (this.transactionAliasTypeTable.TryGetValue(siteID, out siteTransactionAliasType) && siteTransactionAliasType.TryGetValue(aliasID, out transactionAliasType)
                    && transactionAliasType != TransactionTypes.TransactionType_None)
                {
                    return transactionAliasType;
                }
            }
            return TransactionTypes.TransactionType_None;
        }

        /// <summary>
        /// Get transaction alias user data fields for the site provided
        /// </summary>
        /// <param name="siteID">The site to get user data fields for</param>
        /// <param name="transactionAliasGuid">The transaction alias to get user data fields for</param>
        /// <returns>User data fields corresponding to the site provided, or null if no user data fields are found</returns>
        public Dictionary<string, string> GetTransactionAliasUserDataFields(string siteID, Guid transactionAliasGuid)
        {
            Dictionary<Guid, Dictionary<string, string>> siteUserDataFields;
            Dictionary<string, string> userDataFields;

            if (!string.IsNullOrEmpty(siteID)
                && this.transactionAliasUserFieldsTable != null
                && transactionAliasGuid != Guid.Empty
                && this.transactionAliasUserFieldsTable.TryGetValue(siteID, out siteUserDataFields)
                && siteUserDataFields.TryGetValue(transactionAliasGuid, out userDataFields))
            {
                return userDataFields;
            }

            return null;
        }

        /// <summary>
        /// Retrieve closeouts for the given site that correspond to the provided manager
        /// </summary>
        /// <param name="siteID">The site to get closeouts for</param>
        /// <param name="managerID">The manager to get closeouts for</param>
        /// <returns>A list of closeouts for the given site and manager</returns>
        public List<CloseoutDO> GetSiteCloseoutsForManager(string siteID, string managerID)
        {
            List<CloseoutDO> siteCloseouts = new List<CloseoutDO>();

            if (!string.IsNullOrEmpty(siteID)
                && this.closeoutTable != null
                && this.closeoutTable.TryGetValue(siteID, out siteCloseouts))
            {
                return siteCloseouts.FindAll(closeout => string.Compare(closeout.ManagerName, managerID, StringComparison.OrdinalIgnoreCase) == 0);
            }

            return siteCloseouts;
        }

        /// <summary>
        /// Retrieve the accounting general configuration for the given site. We use this information
        /// when checking the inventory date
        /// </summary>
        /// <param name="siteID">The site to get the general configuration for</param>
        /// <returns>The accounting general configuration for the given site</returns>
        public GeneralConfigDO GetSiteAccountingConfiguration(string siteID)
        {
            GeneralConfigDO siteAccountingConfiguration;

            if (!string.IsNullOrEmpty(siteID)
                && this.forceCloseoutTable != null
                && this.forceCloseoutTable.TryGetValue(siteID, out siteAccountingConfiguration))
            {
                return siteAccountingConfiguration;
            }

            return null;
        }

        /// <summary>
        /// Get the company in the enterprise system identified by the provided guid
        /// </summary>
        /// <param name="siteID">The site to search for the company in</param>
        /// <param name="companyGuid">Identifies the company to retrieve</param>
        /// <returns>The company matching the provided guid, or null if none was found</returns>
        public CompanyClass GetCompanyByGuid(string siteID, Guid companyGuid)
        {
            if (this.companyTable != null && !string.IsNullOrEmpty(siteID) && companyGuid != Guid.Empty)
            {
                Dictionary<string, CompanyClass> siteCompanies;

                if (this.companyTable.TryGetValue(siteID, out siteCompanies))
                {
                    return siteCompanies.Values.FirstOrDefault(company => company.MasterRecordGuid == companyGuid);
                }
            }

            return null;
        }

        /// <summary>
        /// Get the company in the enterprise system matching the provided code
        /// </summary>
        /// <param name="siteID">The site to search for the company in</param>
        /// <param name="companyCode">The company code to search for in the enterprise system</param>
        /// <returns>The company matching the provided code, or null if none was found</returns>
        public CompanyClass GetCompanyByCode(string siteID, string companyCode)
        {
            if (this.companyTable != null && !string.IsNullOrEmpty(siteID) && !string.IsNullOrEmpty(companyCode))
            {
                Dictionary<string, CompanyClass> siteCompanies;

                if (this.companyTable.TryGetValue(siteID, out siteCompanies))
                {
                    List<CompanyClass> matchingCompanies = siteCompanies.Values.Where(company => company.Code == companyCode).ToList();

                    // Only return a company if there is one and only one match on the code
                    if (matchingCompanies.Count == 1)
                    {
                        return matchingCompanies[0];
                    }
                }
                else
                {
                    Guid companyGuid = GetTranslatedEntityGuid(companyCode, FMAETranslationType.Company);
                    return GetCompanyByGuid(siteID, companyGuid);
                }
            }

            return null;
        }

        /// <summary>
        /// Get the company in the enterprise system matching the provided ID
        /// </summary>
        /// <param name="siteID">The site to search for the company in</param>
        /// <param name="companyID">The company ID to search for in the enterprise system</param>
        /// <returns>The company matching the provided ID, or null if none was found</returns>
        public CompanyClass GetCompanyByID(string siteID, string companyID)
        {
            CompanyClass company = null;
            Guid companyGuid = GetTranslatedEntityGuid(companyID, FMAETranslationType.Company);
            if(companyGuid != Guid.Empty)
            {
                company = GetCompanyByGuid(siteID, companyGuid);
            }
            else
            {
                if (this.companyTable != null && !string.IsNullOrEmpty(siteID) && !string.IsNullOrEmpty(companyID))
                {
                    Dictionary<string, CompanyClass> siteCompanies;
                    if(this.companyTable.TryGetValue(siteID, out siteCompanies))
                    {
                        if (siteCompanies.TryGetValue(companyID, out company))
                        {
                            return company;
                        }
                        else
                        {
                            string companyIDWithoutCode;
                            int index = companyID.IndexOf('-', 0);
                            companyIDWithoutCode = (index != -1 && index < companyID.Length - 3) ? companyID.Substring(index + 1).Trim() : companyID;
                            if(siteCompanies.TryGetValue(companyIDWithoutCode, out company))
                            {
                                return company;
                            }
                        }
                    }
                    
                }
            }
            return company;
        }

        /// <summary>
        /// Get the product in the enterprise system identified by the provided guid
        /// </summary>
        /// <param name="siteID">The site to search for the product in</param>
        /// <param name="productGuid">Identifies the product to retrieve</param>
        /// <returns>The product matching the provided guid, or null if none was found</returns>
        public ProductClass GetProductByGuid(string siteID, Guid productGuid)
        {
            if (this.productTable != null && !string.IsNullOrEmpty(siteID) && productGuid != Guid.Empty)
            {
                Dictionary<string, ProductClass> siteProducts;

                if (this.productTable.TryGetValue(siteID, out siteProducts))
                {
                    return siteProducts.Values.FirstOrDefault(product => product.MasterRecordGuid == productGuid);
                }
            }

            return null;
        }

        /// <summary>
        /// Get the product in the enterprise system matching the provided ID
        /// </summary>
        /// <param name="siteID">The site to search for the product in</param>
        /// <param name="productID">The product ID to search for a match for in the enterprise system</param>
        /// <returns>The product matching the provided ID, or null if none was found</returns>
        public ProductClass GetProductByID(string siteID, string productID)
        {
            if (this.productTable != null && !string.IsNullOrEmpty(siteID) && !string.IsNullOrEmpty(productID))
            {
                Dictionary<string, ProductClass> siteProducts;
                ProductClass product;

                if (this.productTable.TryGetValue(siteID, out siteProducts) && siteProducts.TryGetValue(productID, out product))
                {
                    return product;
                }
                else
                {
                    Guid productGuid = GetTranslatedEntityGuid(productID, FMAETranslationType.Product);
                    return GetProductByGuid(siteID, productGuid);
                }
            }

            return null;
        }

        /// <summary>
        /// Get the product in the enterprise system matching the provided Code
        /// </summary>
        /// <param name="siteID">The site to search for the product in</param>
        /// <param name="productCode">The product Code to search for a match for in the enterprise system</param>
        /// <returns>The product matching the provided Code, or null if none was found</returns>
        public ProductClass GetProductByCode(string siteID, string productCode)
        {
            if (this.productTable != null && !string.IsNullOrEmpty(siteID) && !string.IsNullOrEmpty(productCode))
            {
                Dictionary<string, ProductClass> siteProducts;

                if (this.productTable.TryGetValue(siteID, out siteProducts))
                {
                    List<ProductClass> matchingProducts = siteProducts.Values.Where(product => product.Code == productCode).ToList();

                    // Only return a product if there is one and only one match on the code
                    if (matchingProducts.Count == 1)
                    {
                        return matchingProducts[0];
                    }
                }
                else
                {
                    Guid productGuid = GetTranslatedEntityGuid(productCode, FMAETranslationType.Product);
                    return GetProductByGuid(siteID, productGuid);
                }
            }

            return null;
        }
        
        /// <summary>
        /// Retrieve the equipment guid corresponding to the provided equipment ID.
        /// </summary>
        /// <param name="siteID">The site we're looking up equipment for</param>
        /// <param name="equipmentID">The ID of an equipment record to use to search for a match</param>
        /// <returns>The EquipmentGuid of the record matching the provided equipment ID</returns>
        public EquipmentClass GetEquipment(string siteID, string equipmentID)
        {
            if (this.equipmentTable != null && !string.IsNullOrEmpty(siteID) && !string.IsNullOrEmpty(equipmentID))
            {
                Dictionary<string, EquipmentClass> siteEquipment;
                EquipmentClass equipment;

                if (this.equipmentTable.TryGetValue(siteID, out siteEquipment) && siteEquipment.TryGetValue(equipmentID, out equipment))
                {
                    return equipment;
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieve the meter corresponding to the provided meter ID 
        /// </summary>
        /// <param name="siteID">The site we're looking up meters for</param>
        /// <param name="meterID">The ID of a meter record to use to search for a match</param>
        /// <returns>MeterClass matching the provided meter ID</returns>
        public MeterClass GetMeter(string siteID, string meterID)
        {
            if (this.meterTable != null && !string.IsNullOrEmpty(siteID) && !string.IsNullOrEmpty(meterID))
            {
                Dictionary<string, MeterClass> siteMeters;
                MeterClass meter;

                if (this.meterTable.TryGetValue(siteID, out siteMeters) && siteMeters.TryGetValue(meterID, out meter))
                {
                    return meter;
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieve the gate corresponding to the provided gate ID.
        /// </summary>
        /// <param name="siteID">The site we're looking up equipment for</param>
        /// <param name="gateID">The ID of an equipment record to use to search for a match</param>
        /// <returns>The EquipmentGuid of the record matching the provided equipment ID</returns>
        public GateClass GetGate(string siteID, string gateID)
        {
            if (this.gateTable != null && !string.IsNullOrEmpty(siteID) && !string.IsNullOrEmpty(gateID))
            {
                Dictionary<string, GateClass> siteGates;
                GateClass gate;

                if (this.gateTable.TryGetValue(siteID, out siteGates) && siteGates.TryGetValue(gateID, out gate))
                {
                    return gate;
                }
            }

            return null;
        }

        public Guid GetTankGuid(string siteID, string tankID)
        {
            if (this.tankTable != null && !string.IsNullOrEmpty(siteID) && !string.IsNullOrEmpty(tankID))
            {
                Dictionary<string, Guid> siteTanks;
                Guid tankGuid;

                if (this.tankTable.TryGetValue(siteID, out siteTanks) && siteTanks.TryGetValue(tankID, out tankGuid))
                {
                    return tankGuid;
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Get the Enterprise (FuelsManager) record guid defined as a translation for the 
        /// legacy entity ID provided.
        /// </summary>
        /// <param name="legacyEntityID">The ID from the legacy transaction record</param>
        /// <param name="translationType">The type of translation, e.g. product or company</param>
        /// <returns>The Guid of the entity the legacy ID should translate to, or Guid.Empty if no translation was found</returns>
        public Guid GetTranslatedEntityGuid(string legacyEntityID, FMAETranslationType translationType)
        {
            Guid translatedEntityGuid;

            if (translationType == FMAETranslationType.Company)
            {
                this.companyTranslationTable.TryGetValue(legacyEntityID, out translatedEntityGuid);
            }
            else if (translationType == FMAETranslationType.Product)
            {
                this.productTranslationTable.TryGetValue(legacyEntityID, out translatedEntityGuid);
            }
            else
            {
                throw new Exception("Unknown entity translation type: " + translationType);
            }

            return translatedEntityGuid;
        }

        #endregion
        #endregion

        #region Construction
        public IntoPlaneImport()
        {
            transactionList = new List<TransactionDO>();
            FMServiceLocator.Container = new UnityContainer();
            FMServiceLocator.Container.RegisterFMCoreServices();
            FMServiceLocator.Container.RegisterFuelManagerWebAPIBusinessServices();
            this._transactionAliasProxy = FMServiceLocator.Container.Resolve<ITransactionAliasesProxy>();
            this._transactionPipeline = FMServiceLocator.Container.Resolve<ITransactionPipeline>();
            this._currentUserSecurity = FMServiceLocator.Container.Resolve<ICurrentRequestContext>();
            this._metersProxy = FMServiceLocator.Container.Resolve<IMetersProxy>();
        }
        #endregion

        #region IIntoPlaneImport Implementation
        public string ImportData(SecurityClass sec, string data, IntoPlaneImportParametersDO parameters)
        {

            if (sec == null)
            {
                throw new ArgumentNullException("sec");
            }
            security = sec;

            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            string strMessage = "";
            string[] lines = data.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

            if(lines.Length < 2)
            {
                //throw IntoPlaneImportNoDataException
                return "";
            }

            _currentUserSecurity.SetCurrentSecurityContext(sec);
            GetLookupTables();
            SiteClass site = siteLookupTable[sec.SiteID];
            this._currentUserSecurity.SetCurrentSite(site);

            IntoPlaneImportFields header = new IntoPlaneImportFields(new SortedList<string, int>());
            header.ParseValues(lines[0]);
            if (!header.ValidateHeader(out string message, out SortedList<string, int> headers))
                return message;

            //Set i = 1 to skip first line since this is always the header
            for (int i = 1; i < lines.Length; i++ )
            {
                try
                {
                    //skip empty lines
                    if (!string.IsNullOrEmpty(lines[i]) && !lines[i].All(c => c == ','))
                    {
                        StringBuilder sbErr = new StringBuilder();
                        IntoPlaneImportFields record = new IntoPlaneImportFields(site, headers);
                        record.ParseValues(lines[i]);

                        if (string.IsNullOrWhiteSpace(record.TicketNumber))
                        {
                            strMessage += "Row " + i + " skipped Ticket Number is required"+Environment.NewLine+Environment.NewLine;
                            continue;
                        }
                        if(!DATE_TIME_FORMAT.IsMatch(record.TransactionDateString))
                        {
                            strMessage += GenerateIntoPlaneTransactionErrorMessage(record, new IntoPlaneImportGeneralException("Transaction Date is required in format : YYYY-mm-dd HH:MM:SS"));
                            continue;
                        }
                        if (string.IsNullOrEmpty(record.Manager))
                        {
                            strMessage += GenerateIntoPlaneTransactionErrorMessage(record, new IntoPlaneImportGeneralException("Manager is a required field"));
                            continue;
                        }
                        if(record.Manager != parameters.ManagerFilter)
                        {
                            strMessage += GenerateIntoPlaneTransactionErrorMessage(record, new IntoPlaneImportGeneralException("Transaction has been skipped due to not being the selected manager."));
                            continue;
                        }
                        if (record.TransactionDate.Date < parameters.StartDateFilter || record.TransactionDate.Date > parameters.EndDateFilter)
                        {
                            strMessage += GenerateIntoPlaneTransactionErrorMessage(record, new IntoPlaneImportGeneralException("Transaction has been skipped due a transaction date outside the selected range."));
                            continue;
                        }

                        try
                        {
                            TransactionDO trans = CreateTransaction(record, parameters, site, out string warnings);
                            //get the transaction alias class of the inbound transaction
                            var transactionAlias = this._transactionAliasProxy.Get(trans.TransactionAliasGuid, false);
                            //pass each transaction through inbound pipeline
                            var inboundPipeline = this._transactionPipeline.Inbound();
                            foreach (var pipe in inboundPipeline)
                            {
                                pipe.Execute(trans, transactionAlias);
                            }
                            transactionList.Add(trans);
                            strMessage += GenerateIntoPlaneTransactionWarningMessage(record, warnings);
                        }
                        catch (IntoPlaneImportGeneralException ex)
                        {
                            strMessage += GenerateIntoPlaneTransactionErrorMessage(record, ex);
                        }
                    }
                }
                catch(Exception ex)
                {
                    strMessage += "Row "+(i+1)+" not imported: " +ex.Message+ Environment.NewLine + Environment.NewLine;
                }
            }
            try
            {
                var saveResult = SaveTransactions(security);
                if (saveResult.Results.Count() > 0)
                {
                    strMessage += "Errors saving transactions" + Environment.NewLine + Environment.NewLine;
                }
                else
                {
                    strMessage += transactionList.Count + " Transactions Saved" + Environment.NewLine + Environment.NewLine;
                }
            }
            catch(Exception e)
            {
                strMessage += "Errors saving transactions" + Environment.NewLine + Environment.NewLine;
                strMessage += e.Message;
            }




            return strMessage;
        }

        private string GenerateIntoPlaneTransactionWarningMessage(IntoPlaneImportFields fields, string warning)
        {
            string strReturn = string.Empty;
            if (!string.IsNullOrWhiteSpace(warning))
            {
                strReturn = "Ticket Number " + fields.TicketNumber + Environment.NewLine + " Warnings:" + Environment.NewLine;
                strReturn += warning;
                strReturn += Environment.NewLine + Environment.NewLine;
            }
            return strReturn;
        }

        private string GenerateIntoPlaneTransactionErrorMessage(IntoPlaneImportFields fields, IntoPlaneImportGeneralException ex)
        {
            string strReturn = "Ticket Number: " + fields.TicketNumber + " not imported for the following reasons:" + Environment.NewLine;
            strReturn += ex.Message;
            strReturn += Environment.NewLine + Environment.NewLine;
            return strReturn;
        }

        private int GetMaxNumber(int digits)
        {
            string tempNumber = string.Empty;
            int result; 
            for(int i = 0; i < digits; i++)
            {
                tempNumber += "9";
            }

            bool success = int.TryParse(tempNumber, out result);
            return (success ? result : 0);
        }

        /// <summary>
        /// AliasId passed from 7.x does not match the name used in tblTransactionAliases in 9.7. 
        /// This method does the translation
        /// </summary>
        /// <param name="AliasId">7.x alias name</param>
        /// <returns>9.7 alias name</returns>
        private string TranslateAliasName(string aliasId)
        {
            //The following transaction types aren't included in Daily Fuel Transaction Files
            //"Adjustment"
            //"Bulk Issue" 
            //"LR Receipt"
            //"Physical Inventory"
            //"Receipt"
            //"Transfer"

            switch(aliasId.ToUpper())
            {
                case "24 HR":
                    return "24 Hour Closeout";
                case "DEFUEL":
                    return "Defuel";
                case "ISSUE":
                    return "Issue";
                case "LOAD RACK":
                    return "Load Rack";
                case "ROTATION":
                    return "Rotation";
                default:
                    return string.Empty;
            }
        }
        #endregion

        #region Public Properties
        public SecurityClass Security
        {
            get { return security; }
            set { security = value; }
        }
        #endregion



    }
}