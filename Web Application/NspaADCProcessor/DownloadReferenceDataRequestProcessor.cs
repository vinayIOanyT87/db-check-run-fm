// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DownloadReferenceDataRequestProcessor.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the DownloadReferenceDataRequestProcessor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Nspa
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.IO;
	using System.IO.Compression;
	using System.Linq;

	using ADC.Nspa.General;

	using Crypt;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using Nspa.Exchange;
    using System.Diagnostics;

	using Varec.CommonComponents.VolumeCorrection;

    public class DownloadReferenceDataRequestProcessor : RequestProcessorGenericBase<DownloadReferenceDataRequest, DownloadReferenceDataResponse>
	{
		private const TransactionTypes CustomsTransferAliasType = TransactionTypes.T9_Request;
		private static readonly byte[] Seed = (new Guid("1488AE9C-6813-49AE-AF08-155A53D99CE6")).ToByteArray();
		private static readonly byte[] DummyData = (new Guid("4BE74006-F456-4399-86C5-03613D7FB234")).ToByteArray();
		private static AESCrypt encryptor = new AESCrypt();

		private ulong aliasEquipmentTypes = 0;

		private SiteClass site = null;

		private TransactionAliasCollectionClass aliasListInternal = null;

		//  This is a cache list to be used in clearing the FuelCard field in the equipment list
		private Guid[] fuelCardGuidList = null; 


		internal DownloadReferenceDataRequestProcessor()
			: base("downloading reference data")
		{
			
		}

		private TransactionAliasCollectionClass AliasList
		{
			get
			{
				if (aliasListInternal == null)
				{
					aliasListInternal = this.EnumerateAliases();
				}
				return aliasListInternal;
			}
		}

		

		
		/// <summary>
		/// Adds the new data table as compressed bytes to response object
		/// </summary>
		/// <param name="newDataTable">The new data table.</param>
		private void AddNewDataTable(DataTable newDataTable)
		{
			if (this.generateFileOnly)
			{
				this.sqlCeWorker.SaveTable(newDataTable);
			}
			else
			{
				var compressedTable = CompressDataTable(newDataTable);
				var entityId = newDataTable.TableName;
				var newEntity = new DownloadReferenceDataResponse.EntityData() { Name = entityId, Binary = compressedTable };
				this.Response.ExchangeData.Add(newEntity);
			}
		}

		/// <summary>
		/// Processes download reference for the given entity.
		/// </summary>
		/// <param name="type">The type.</param>
		private void ProcessEntity(EntityTypes type)
		{
			switch (type)
			{
				case EntityTypes.Site:
					this.ProcessSite();
					break;
				case EntityTypes.User:
					this.ProcessUser();					
					break;
				case EntityTypes.Product:
					this.ProcessProduct();
					break;
				case EntityTypes.ApplicationString:
					this.ProcessApplicationString();
					break;
				case EntityTypes.Company:
					this.ProcessCompany();
					break;
				case EntityTypes.CompanyRoleMap:
					this.ProcessCompanyRoles();
					break;
				case EntityTypes.TransactionAlias:
				case EntityTypes.UserDataFieldTransactionAlias:
				case EntityTypes.UserDataListValueTransactionAlias:
				case EntityTypes.ProductToTransactionAliasExclusion:
					this.ProcessTransactionAlias();
					break;
				case EntityTypes.EquipmentType:
				case EntityTypes.EquipmentTypeClass:
					this.ProcessEquipmentTypeClasses();
					this.ProcessEquipmentTypes();
					break;
				case EntityTypes.Equipment:
					this.ProcessEquipment();
					break;
				case EntityTypes.Personnel:
					this.ProcessPersonnel();
					break;
				case EntityTypes.IATA:
					this.ProcessIATA();
					break;
				case EntityTypes.FuelCard:
					this.ProcessFuelCards();
					break;
                case EntityTypes.FuelCardLimit:
                    this.ProcessFuelCardLimits();
                    break;
                case EntityTypes.Gate:
					this.ProcessGates(); 
					break;
				case EntityTypes.Meter:
					this.ProcessMeters();
					break;
				case EntityTypes.Tank:
					this.ProcessTanks();
					break;
				case EntityTypes.DocumentNumber:
					this.ProcessAssociatedDocuments();
					break;
			}
		}

		private SqlCeHelper sqlCeWorker;

		/// <summary>
		/// Gets a value indicating whether to generate file only.
		/// </summary>
		/// <value>
		///   <c>true</c> if [generate file only]; otherwise, <c>false</c>.
		/// </value>
		private bool generateFileOnly;
		
		protected override void ProcessCore()
		{
		    try
		    {
		        ValidateExchangeUserId(this.Security.UserID);

				site = FMChannelHelper.MakeCall<ISites, SiteClass>(
		                siteService => siteService.Get(this.Security, this.Security.SiteGuid, false, false, false));
		        this.generateFileOnly = this.Request.GenerateFileOnly;
		        sqlCeWorker = this.generateFileOnly ? new SqlCeHelper(site.IdentityGuid) : null;
		        this.Response.ExchangeData = new List<DownloadReferenceDataResponse.EntityData>();
		        if (!string.IsNullOrEmpty(this.Request.EntityNames))
		        {
		            var entityNameList = this.Request.EntityNames.Split(EntityTypesExtension.Separator);

		            foreach (var entityTypeString in entityNameList)
		            {
		                EntityTypes entityType;
		                if (Enum.TryParse(entityTypeString, true, out entityType))
		                {
		                    ProcessEntity(entityType);
		                }
		            }

		        }

		        if (this.generateFileOnly)
		        {
		            // We need to close the connection to avoid issue to get the hash from the file.
		            this.sqlCeWorker.Cleanup();
		            this.Response.DownloadFile.FileHash = this.sqlCeWorker.GetFileHash();
		            this.Response.DownloadFile.FileId = this.sqlCeWorker.IdString;
		            this.sqlCeWorker = null;
		        }
		    }
		    catch (Exception ex)
		    {
                string exceptionMessage = "Error processing Entity:";
                string eventLogMessage = exceptionMessage + Environment.NewLine + ex.Message;
		        if (ex.InnerException != null)
		        {
                    eventLogMessage += Environment.NewLine + ex.InnerException.Message + Environment.NewLine;
                }
		        eventLogMessage += ex.StackTrace;

                Helper.NspaADCEventLog.WriteEntry(eventLogMessage, EventLogEntryType.Error);
		        throw;
		    }
            finally
			{
				if (sqlCeWorker != null)
				{
					sqlCeWorker.Cleanup();
				}
			}
		}

		private void ProcessSite()
		{
            try
            {
                var returnDataTable = new DataTable(EntityTypes.Site.TableName());
                returnDataTable.Columns.Add("Id", typeof(string));
                returnDataTable.Columns.Add("SiteGuid", typeof(Guid));
                AddMeasurementColumns(returnDataTable.Columns);
                AddAdditiveVolumeMeasurementColumns(returnDataTable.Columns);

                var newRow = returnDataTable.NewRow();
                newRow["Id"] = site.ID;
                newRow["SiteGuid"] = site.IdentityGuid;
                AddMeasurementValues(newRow,
                            site.VolumeUnits, Convert.ToByte(site.VolumeDecimalPlaces),
                            site.DensityUnits, Convert.ToByte(site.DensityDecimalPlaces),
                            site.MassUnits, Convert.ToByte(site.MassDecimalPlaces),
                            site.TemperatureUnits, Convert.ToByte(site.TemperatureDecimalPlaces)); // true means including additive				
                AddAdditiveVolumeMeasurementValues(newRow,
                    site.AdditiveVolumeUnits, Convert.ToByte(site.AdditiveVolumeDecimalPlaces));

                returnDataTable.Rows.Add(newRow);
                AddNewDataTable(returnDataTable);
            }
            catch (Exception ex)
            {
                string exceptionMessage = "Error processing site data";
                string eventLogMessage = exceptionMessage + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                Helper.NspaADCEventLog.WriteEntry(eventLogMessage, EventLogEntryType.Error);
                throw new Exception(exceptionMessage, ex);
            }
		}

		private static void AddAdditiveVolumeMeasurementColumns(DataColumnCollection columns)
		{
			columns.Add("AdditiveVolumeUnitIndex", typeof(int));
			columns.Add("AdditiveVolumeDecimalPlaces", typeof(byte));
		}

		private static void AddPressureMeasurementColumns(DataColumnCollection columns)
		{
			columns.Add("PressureUnitIndex", typeof(int));
			columns.Add("PressureDecimalPlaces", typeof(byte));
		}

		private static void AddMeasurementColumns(DataColumnCollection columns)
		{
			columns.Add("VolumeUnitIndex", typeof(int));
			columns.Add("VolumeDecimalPlaces", typeof(byte));

			columns.Add("DensityUnitIndex", typeof(int));
			columns.Add("DensityDecimalPlaces", typeof(byte));

			columns.Add("MassUnitIndex", typeof(int));
			columns.Add("MassDecimalPlaces", typeof(byte));

			columns.Add("TemperatureUnitIndex", typeof(int));
			columns.Add("TemperatureDecimalPlaces", typeof(byte));

		}

		private static void AddAdditiveVolumeMeasurementValues(DataRow newRow, EngineeringUnit additiveVolumeUnitIndex,byte additiveVolumeDecimalPlaces)
		{
			newRow["AdditiveVolumeUnitIndex"] = additiveVolumeUnitIndex;
			newRow["AdditiveVolumeDecimalPlaces"] = additiveVolumeDecimalPlaces;
		}

		private static void AddPressureMeasurementValues(DataRow newRow, EngineeringUnit pressureUnitIndex, byte pressureDecimalPlaces)
		{
			newRow["PressureUnitIndex"] = pressureUnitIndex;
			newRow["PressureDecimalPlaces"] = pressureDecimalPlaces;
		}

		private static void AddMeasurementValues(DataRow newRow,
			EngineeringUnit volumeUnitIndex, byte volumeDecimalPlaces,
			EngineeringUnit densityUnitIndex, byte densityDecimalPlaces,
			EngineeringUnit massUnitIndex, byte massDecimalPlaces,
			EngineeringUnit temperatureUnitIndex, byte temperatureDecimalPlaces)
		{

			newRow["VolumeUnitIndex"] = volumeUnitIndex;
			newRow["VolumeDecimalPlaces"] = volumeDecimalPlaces;

			newRow["DensityUnitIndex"] = densityUnitIndex;
			newRow["DensityDecimalPlaces"] = densityDecimalPlaces;

			newRow["MassUnitIndex"] = massUnitIndex;
			newRow["MassDecimalPlaces"] = massDecimalPlaces;

			newRow["TemperatureUnitIndex"] = temperatureUnitIndex;
			newRow["TemperatureDecimalPlaces"] = temperatureDecimalPlaces;

		}

		private string GetSiteId(Dictionary<Guid, string> siteIdDictionary, Guid siteGuid)
		{
			string siteId;
			if (siteIdDictionary.ContainsKey(siteGuid))
			{
				siteId = siteIdDictionary[siteGuid];
			}
			else
			{
				var site = FMChannelHelper.MakeCall<ISites, SiteClass>(
				siteService => siteService.Get(this.Security, siteGuid, false, false, false));

				siteId = site.ID;
				siteIdDictionary.Add(siteGuid, siteId);
			}
			return siteId;
		}

		/// <summary>
		/// Sets the length of the given field.
		/// </summary>
		/// <param name="theTable">The table.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <param name="fieldLength">Length of the field.</param>
		private static void SetFieldLength(DataTable theTable, string columnName, int fieldLength)
		{
            if (theTable == null)
            {
                Helper.NspaADCEventLog.WriteEntry("Cannot set field length on empty/null table", EventLogEntryType.Error);
            }

            if (theTable.Columns.Contains(columnName) == false)
            {
                string columnMessage = "Cannot set field length non-existent column:" + columnName;

                columnMessage += Environment.NewLine + "Current columns: ";
                foreach (DataColumn c in theTable.Columns)
                {
                    columnMessage += Environment.NewLine + c.ColumnName;
                }
                Helper.NspaADCEventLog.WriteEntry(columnMessage, EventLogEntryType.Warning);
                return;
            }

            try
            {
                theTable.Columns[columnName].MaxLength = fieldLength;
            }
            catch (Exception ex)
            {
                string exceptionMessage = "Error setting field length";
                string eventLogMessage = exceptionMessage + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                Helper.NspaADCEventLog.WriteEntry(eventLogMessage, EventLogEntryType.Error);
                throw new Exception(exceptionMessage, ex);
            }
		}

		private void ProcessApplicationString()
		{
            try
            {
                var applicationstringCollection = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
                    applicationStringService => applicationStringService.EnumerateByType(this.Security, STRING_TYPE.FUEL_CARD_TYPE));

                var dataRows = from applicationString in applicationstringCollection
                               select
                                   new
                                   {
                                       applicationString.ID,
                                       LookupApplicationStringTypeIndex = (int)applicationString.Type,
                                       ApplicationStringGuid = applicationString.IdentityGuid
                                   };

                var returnDataTable = ListToDataTable(null, dataRows, EntityTypes.ApplicationString.TableName());
                SetFieldLength(returnDataTable, "ID", 250);

                applicationstringCollection = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
                    applicationStringService => applicationStringService.EnumerateByType(this.Security, STRING_TYPE.COMPANY_TYPE));

                dataRows = from applicationString in applicationstringCollection
                           select
                               new
                               {
                                   applicationString.ID,
                                   LookupApplicationStringTypeIndex = (int)applicationString.Type,
                                   ApplicationStringGuid = applicationString.IdentityGuid
                               };

                returnDataTable = ListToDataTable(returnDataTable, dataRows, EntityTypes.ApplicationString.TableName());


                AddNewDataTable(returnDataTable);
            }
            catch (Exception ex)
            {
                string exceptionMessage = "Error processing application string data";
                string eventLogMessage = exceptionMessage + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                Helper.NspaADCEventLog.WriteEntry(eventLogMessage, EventLogEntryType.Error);
                throw new Exception(exceptionMessage, ex);
            }
		}

		private void ProcessProduct()
		{
            try
            {
                var siteIdDictionary = new Dictionary<Guid, string>();

                // get a list of products
                var productDataSet = FMChannelHelper.MakeCall<IProducts, DataSet>(
                    productService => productService.EnumerateByType1(this.Security, ProductType.ComponentProduct));

                var returnDataTable = new DataTable(EntityTypes.Product.TableName());
                returnDataTable.Columns.Add("ProductId", typeof(string));
                returnDataTable.Columns.Add("StandardDensity", typeof(double));
                returnDataTable.Columns.Add("StandardTemperature", typeof(double));
                returnDataTable.Columns.Add("Description", typeof(string));
                returnDataTable.Columns.Add("ProductGuid", typeof(Guid));
                returnDataTable.Columns.Add("AlternatePressure", typeof(double));
                returnDataTable.Columns.Add("AlternateTemperature", typeof(double));
                returnDataTable.Columns.Add("ApplyVolumeCorrection", typeof(bool));
                returnDataTable.Columns.Add("CorrectionFactor0", typeof(double));
                returnDataTable.Columns.Add("CorrectionFactor1", typeof(double));
                returnDataTable.Columns.Add("CorrectionFactor2", typeof(double));
                returnDataTable.Columns.Add("CorrectionFactor3", typeof(double));
                returnDataTable.Columns.Add("CorrectionFactor4", typeof(double));
                returnDataTable.Columns.Add("LookupMajorCorrectionMethodIndex", typeof(int));
                returnDataTable.Columns.Add("MinorCorrectionMethod", typeof(int));
                returnDataTable.Columns.Add("HiddenDate", typeof(DateTime));

                SetFieldLength(returnDataTable, "ProductID", 30);
                SetFieldLength(returnDataTable, "Description", 50);

                AddMeasurementColumns(returnDataTable.Columns);
                AddPressureMeasurementColumns(returnDataTable.Columns);

                var productCollection = new ProductCollectionClass();

                DataTable table = productDataSet.Tables[0];
                while (table.Rows.Count != 0)
                {
                    var product = new ProductClass();
                    product.Load(productDataSet);
                    productCollection.Add(product);

                    table.Rows.RemoveAt(0);
                }

                foreach (ProductClass p in productCollection)
                {
                    var siteGuid = p.SiteGuid;
                    var siteId = p.SiteID;

                    var newRow = returnDataTable.NewRow();
                    newRow["ProductId"] = p.ID;
                    newRow["StandardDensity"] = p.StandardDensity;
                    newRow["StandardTemperature"] = p.StandardTemperature;
                    newRow["Description"] = p.Description;
                    newRow["ProductGuid"] = p.MasterRecordGuid;
                    newRow["AlternatePressure"] = p.AlternateBasePressure;
                    newRow["AlternateTemperature"] = p.AlternateTemperature;
                    newRow["ApplyVolumeCorrection"] = p.ApplyVolumeCorrection;
                    newRow["CorrectionFactor0"] = p.CorrectionFactor0;
                    newRow["CorrectionFactor1"] = p.CorrectionFactor1;
                    newRow["CorrectionFactor2"] = p.CorrectionFactor2;
                    newRow["CorrectionFactor3"] = p.CorrectionFactor3;
                    newRow["CorrectionFactor4"] = p.CorrectionFactor4;
                    newRow["LookupMajorCorrectionMethodIndex"] = (ECorrectionTypeMajor)Convert.ToInt32(p._VcfModuleSettings.CorrectionMethodType);
                    newRow["MinorCorrectionMethod"] = (ECorrectionTypeMinor)(Convert.ToInt32(p._VcfModuleSettings.CorrectionMethodSpecific) - 1);

                    if (p.HiddenDate != null)
                    {
                        newRow["HiddenDate"] = ConvertDateTimeOffsetToDateTime(p.HiddenDate);
                    }

                    AddMeasurementValues(
                        newRow,
                        p.VolumeUnits,
                        p.VolumeDecimalPlaces,
                        p.DensityUnits,
                        p.DensityDecimalPlaces,
                        p.MassUnits,
                        p.MassDecimalPlaces,
                        p.TemperatureUnits,
                        p.TemperatureDecimalPlaces);
                    AddPressureMeasurementValues(
                        newRow,
                        p.PressureUnits,
                        p.PressureDecimalPlaces);

                    returnDataTable.Rows.Add(newRow);
                }

                AddNewDataTable(returnDataTable);
            }
            catch (Exception ex)
            {
                string exceptionMessage = "Error processing product data";
                string eventLogMessage = exceptionMessage + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                Helper.NspaADCEventLog.WriteEntry(eventLogMessage, EventLogEntryType.Error);
                throw new Exception(exceptionMessage, ex);
            }
		}

		private void ProcessEquipmentTypes()
		{
            try
            {
                var equipmentTypeCollection =
                    FMChannelHelper.MakeCall<IEquipmentTypes, EquipmentTypeCollectionClass>(
                        equipmentTypesService => equipmentTypesService.Enumerate(this.Security, "", ""));


                var dataRows = from equipmentType in equipmentTypeCollection
                               select
                                   new
                                   {
                                       EqTypeName = equipmentType.ID,
                                       LookupEquipmentTypeIndex = (int)equipmentType.Attribute,
                                       EquipmentTypeGuid = equipmentType.IdentityGuid
                                   };

                var returnDataTable = ListToDataTable(null, dataRows, EntityTypes.EquipmentType.TableName());
                SetFieldLength(returnDataTable, "EqTypeName", 50);
                AddNewDataTable(returnDataTable);
            }
            catch (Exception ex)
            {
                string exceptionMessage = "Error processing equipment type data";
                string eventLogMessage = exceptionMessage + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                Helper.NspaADCEventLog.WriteEntry(eventLogMessage, EventLogEntryType.Error);
                throw new Exception(exceptionMessage, ex);
            }
		}

		/// <summary>
		/// Makes the equipment type class unique identifier.
		/// Equipment Type Class doesn't have a guid. We make one up that is easy to make up.
		/// </summary>
		/// <param name="equipmentTypeClassIndex">Index of the equipment type class.</param>
		/// <returns></returns>
		private static Guid MakeEquipmentTypeClassGuid(EQUIPMENT_TYPE equipmentTypeClassIndex)
		{
			// Random Guid {2DCDE25F-6393-426a-A0FF-8BDBF1D0E37A}
			// substitute the last byte with classIndex to generate unique but consistent Guids

			var predefinedGuid = new Guid(0x2dcde25f, 0x6393, 0x426a, 0xa0, (byte)equipmentTypeClassIndex, 0x8b, 0xdb, 0xf1, 0xd0, 0xe3, 0x7a);
			return predefinedGuid;
		}

		private void ProcessEquipmentTypeClasses()
		{
            try
            {
                var equipmentTypeClassList =
                    Enum.GetValues(typeof(EQUIPMENT_TYPE))
                        .Cast<EQUIPMENT_TYPE>()
                        .Where(type => type != EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE)
                        .ToList();

                var dataRows = from equipmentTypeClassIndex in equipmentTypeClassList
                               select
                                   new
                                   {
                                       EquipmentTypeClassGuid = MakeEquipmentTypeClassGuid(equipmentTypeClassIndex),
                                       EquipmentTypeClassIndex = (int)equipmentTypeClassIndex,
                                       EquipmentTypeClassName =
                                       FMChannelHelper.MakeCall<IDataDictionariesClass, string>(dataDictionaryService =>
                                           dataDictionaryService.Get(this.Security.SiteGuid, EquipmentTypeClass.TypeID(equipmentTypeClassIndex))),
                                   };

                var returnDataTable = ListToDataTable(null, dataRows, EntityTypes.EquipmentTypeClass.TableName());

                AddNewDataTable(returnDataTable);
            }
            catch (Exception ex)
            {
                string exceptionMessage = "Error processing equipment type class data";
                string eventLogMessage = exceptionMessage + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                Helper.NspaADCEventLog.WriteEntry(eventLogMessage, EventLogEntryType.Error);
                throw new Exception(exceptionMessage, ex);
            }
		}

		private void ProcessEquipment()
		{
            try
            {
                List<EQUIPMENT_TYPE> equpmentTypeList = new List<EQUIPMENT_TYPE>();

                // if this.aliasEquipmentTypes !=0, that means ProcessTransactionAlias is called already.
                // It is a little redundant in the logic to provide individual table download funcationality but acceptable
                if (this.aliasEquipmentTypes == 0)
                {
                    var aliasCollection = this.AliasList;
                    foreach (var alias in aliasCollection)
                    {
                        this.aliasEquipmentTypes |= alias.InternalDestinationTypes[0];
                        this.aliasEquipmentTypes |= alias.InternalSourceTypes[0];
                    }
                }

                for (int item = 0; item < (int)EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE; item++)
                {
                    if ((aliasEquipmentTypes & (ulong)(0x000000001 << item)) != 0)
                    {
                        equpmentTypeList.Add((EQUIPMENT_TYPE)item);
                    }
                }

                var returnDataTable = new DataTable(EntityTypes.Equipment.TableName());

                returnDataTable.Columns.Add("ID", typeof(string));
                returnDataTable.Columns.Add("SecondaryStorageFlag", typeof(bool));
                returnDataTable.Columns.Add("ManagedEquipmentFlag", typeof(bool));
                returnDataTable.Columns.Add("TruckCardNumber", typeof(string));
                returnDataTable.Columns.Add("ProductGuid", typeof(Guid));
                returnDataTable.Columns.Add("FuelCardGuid", typeof(Guid));
                returnDataTable.Columns.Add("EquipmentTypeGuid", typeof(Guid));
                returnDataTable.Columns.Add("EquipmentGuid", typeof(Guid));
                returnDataTable.Columns.Add("HiddenDate", typeof(DateTime));
                SetFieldLength(returnDataTable, "ID", 100);
                SetFieldLength(returnDataTable, "TruckCardNumber", 32);

                // get a list of equipment types

                DataSet ds =
                    FMChannelHelper.MakeCall<IEquipments, DataSet>(
                        equipmentService =>
                        equipmentService.EnumerateByTypesCompanyFuelCardProductAndSecondaryStorage1(
                            this.Security,
                            equpmentTypeList.ToArray(),
                            null,
                            null,
                            null,
                            null));

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var newRow = returnDataTable.NewRow();
                    newRow["Id"] = row["ID"];
                    newRow["SecondaryStorageFlag"] = row["SecondaryStorageFlag"];
                    newRow["ManagedEquipmentFlag"] = row["ManagedEquipmentFlag"];
                    newRow["TruckCardNumber"] = row["TruckCardNumber"];

                    newRow["ProductGuid"] = row["ProductGuid"];

                    // We have to erase the fuel card value if it is not in the fuel card list.
                    // Otherwise, when we insert, the database may throw foreign key error.
                    if (row["FuelCardGuid"] != DBNull.Value)
                    {
                        var fuelCardGuid = (Guid)(row["FuelCardGuid"]);
                        if ((this.fuelCardGuidList != null) &&
                            this.fuelCardGuidList.Contains(fuelCardGuid))
                        {
                            newRow["FuelCardGuid"] = fuelCardGuid;
                        }
                    }
                    newRow["EquipmentTypeGuid"] = row["EquipmentTypeGuid"];
                    newRow["EquipmentGuid"] = row["_MasterRecordGuid"];

                    try
                    {
                        var theDate = ConvertDbDateTimeOffsetToDateTime(row, "HiddenDate");

                        DateTime? hiddenDate = (DateTime?)theDate;

                        if (hiddenDate != null)
                        {
                            newRow["HiddenDate"] = hiddenDate;
                        }
                    }
                    catch
                    {
                    }

                    returnDataTable.Rows.Add(newRow);
                }

                AddNewDataTable(returnDataTable);
            }
            catch (Exception ex)
            {
                string exceptionMessage = "Error processing equipment data";
                string eventLogMessage = exceptionMessage + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                Helper.NspaADCEventLog.WriteEntry(eventLogMessage, EventLogEntryType.Error);
                throw new Exception(exceptionMessage, ex);
            }
		}

		private void ProcessCompanyRoles()
		{
            try
            {
                var companyRoleList = FMChannelHelper.MakeCall<ICompanyRoleMaps, List<CompanyRoleMapClass>>(
                        companyRoleMapsService => companyRoleMapsService.EnumerateBySiteForRoleMapping(this.Security, this.Security.SiteGuid));

                var dataRows = from companyRoleMap in companyRoleList
                               where
                                   companyRoleMap.Role == COMPANY_ROLE.CARRIER
                                   || companyRoleMap.Role == COMPANY_ROLE.CUSTOMER_BILLTO
                                   || companyRoleMap.Role == COMPANY_ROLE.CUSTOMER_SHIPTO
                                   || companyRoleMap.Role == COMPANY_ROLE.SUPPLIER
                               select new
                               {
                                   companyRoleMap.CompanyGuid,
                                   LookUpCompanyRoleIndex = (int)companyRoleMap.Role,
                                   CompanyToRoleGuid = companyRoleMap.IdentityGuid
                               };

                var returnDataTable = ListToDataTable(null, dataRows, EntityTypes.CompanyRoleMap.TableName());

                AddNewDataTable(returnDataTable);
            }
            catch (Exception ex)
            {
                string exceptionMessage = "Error processing company role data";
                string eventLogMessage = exceptionMessage + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                Helper.NspaADCEventLog.WriteEntry(eventLogMessage, EventLogEntryType.Error);
                throw new Exception(exceptionMessage, ex);
            }
		}

		/// <summary>
		/// Converts DateTimeOffset to DateTime type (mobile DB doesn't support DateTimeOffset
		/// </summary>
		/// <param name="sourceDate"></param>
		/// <returns></returns>
		private static DateTime? ConvertDateTimeOffsetToDateTime(DateTimeOffset? sourceDate)
		{
			DateTime? newDate = null;
			if (sourceDate != null)
			{
				newDate = sourceDate.Value.LocalDateTime;
			}
			return newDate;
		}

		/// <summary>
		/// Converts DateTimeOffset to DateTime type (mobile DB doesn't support DateTimeOffset
		/// </summary>
		/// <param name="theRow"></param>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		private static object ConvertDbDateTimeOffsetToDateTime(DataRow theRow, string fieldName)
		{

			object newDate = null;
		    try
		    {
                if (theRow.Table.Columns.Contains(fieldName))
                //if (!theRow.IsNull(fieldName))
                {
                    newDate = ((DateTimeOffset)theRow[fieldName]).LocalDateTime;
                }
            }
            catch (Exception)
            {
                newDate = null;
            }
			return newDate;
		}

		private void ProcessCompany()
		{
            try
            {
                var companyList = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
                    companyService => companyService.EnumerateByRoleGetIDCodeTypesIdentityGuidOnly(this.Security,
                        new COMPANY_ROLE[] { COMPANY_ROLE.SUPPLIER, COMPANY_ROLE.CARRIER, COMPANY_ROLE.CUSTOMER_BILLTO, COMPANY_ROLE.CUSTOMER_SHIPTO }));

                var dataRows = from company in companyList
                               select new
                               {
                                   company.ID,
                                   company.Code,
                                   CompanyGuid = company.MasterRecordGuid,
                                   company.CustomerBillToTypeApplicationStringGuid,
                                   HiddenDate = ConvertDateTimeOffsetToDateTime(company.HiddenDate)

                               };

                var returnDataTable = ListToDataTable(null, dataRows, EntityTypes.Company.TableName());
                SetFieldLength(returnDataTable, "ID", 100);
                SetFieldLength(returnDataTable, "Code", 10);
                AddNewDataTable(returnDataTable);
            }
            catch (Exception ex)
            {
                string exceptionMessage = "Error processing company data";
                string eventLogMessage = exceptionMessage + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                Helper.NspaADCEventLog.WriteEntry(eventLogMessage, EventLogEntryType.Error);
                throw new Exception(exceptionMessage, ex);
            }
		}

		private void ProcessUser()
		{
            try
            {
                var groupGuid = FMChannelHelper.MakeCall<IGroups, Guid>(
                   groupService => groupService.GetIdentityGuid(this.Security, HandheldGroupId));

                if (groupGuid != Guid.Empty)
                {
                    byte[] newSeed = new byte[Seed.Length + DummyData.Length];
                    Buffer.BlockCopy(Seed, 0, newSeed, 0, Seed.Length);
                    Buffer.BlockCopy(DummyData, 0, newSeed, Seed.Length, DummyData.Length);
                    using (var key = new AESKey(newSeed, this.Security.SiteGuid.ToByteArray()))
                    {
                        var userList =
                            FMChannelHelper.MakeCall<IUsers, UserCollectionClass>(
                                usersService => usersService.EnumerateByGroup(this.Security, groupGuid));

                        var userRows = from user in userList
                                       select
                                           new
                                           {
                                               UserID = user.ID,
                                               Password = encryptor.Encrypt(user.Password, key),
                                               user.Name,
                                               UserGuid = user.IdentityGuid
                                           };

                        var returnDataTable = ListToDataTable(null, userRows, EntityTypes.User.TableName());
                        SetFieldLength(returnDataTable, "UserID", 100);
                        SetFieldLength(returnDataTable, "Name", 50);

                        AddNewDataTable(returnDataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                string exceptionMessage = "Error processing user data";
                string eventLogMessage = exceptionMessage + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                Helper.NspaADCEventLog.WriteEntry(eventLogMessage, EventLogEntryType.Error);
                throw new Exception(exceptionMessage, ex);
            }
		}

		private TransactionAliasCollectionClass EnumerateAliases()
		{
			return FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasCollectionClass>(
					aliasService => aliasService.Enumerate(this.Security));			
		}

        private void ProcessTransactionAlias()
		{
            try
            {
                var aliasCollection = this.AliasList;
                var aliasDataTable = new DataTable(EntityTypes.TransactionAlias.TableName());
                aliasDataTable.Columns.Add("AliasName", typeof(string));
                aliasDataTable.Columns.Add("DestinationEquipmentTypes1", typeof(ulong));
                aliasDataTable.Columns.Add("SourceEquipmentTypes1", typeof(ulong));
                SetFieldLength(aliasDataTable, "AliasName", 32);

                AddMeasurementColumns(aliasDataTable.Columns);
                AddAdditiveVolumeMeasurementColumns(aliasDataTable.Columns);

                aliasDataTable.Columns.Add("TransactionAliasGuid", typeof(Guid));

                var userDataFieldTable = new DataTable(EntityTypes.UserDataFieldTransactionAlias.TableName());
                userDataFieldTable.Columns.Add("Number", typeof(byte));
                userDataFieldTable.Columns.Add("TransactionAliasGuid", typeof(Guid));
                userDataFieldTable.Columns.Add("UserDataFieldTransactionAliasGuid", typeof(Guid));


                var userDataFieldListValueTable = new DataTable(EntityTypes.UserDataListValueTransactionAlias.TableName());
                userDataFieldListValueTable.Columns.Add("Value", typeof(string));
                userDataFieldListValueTable.Columns.Add("UserDataFieldTransactionAliasGuid", typeof(Guid));
                userDataFieldListValueTable.Columns.Add("UserDataListValueTransactionAliasGuid", typeof(Guid));

                var productToTransactionAliasExclusionTable = new DataTable(EntityTypes.ProductToTransactionAliasExclusion.TableName());
                productToTransactionAliasExclusionTable.Columns.Add("ProductGuid", typeof(Guid));
                productToTransactionAliasExclusionTable.Columns.Add("AssignedToTransactionAliasGuid", typeof(Guid));
                productToTransactionAliasExclusionTable.Columns.Add("ProductToTransactionAliasExclusionGuid", typeof(Guid));


                foreach (var alias in aliasCollection)
                {
                    this.aliasEquipmentTypes |= alias.InternalDestinationTypes[0];
                    this.aliasEquipmentTypes |= alias.InternalSourceTypes[0];

                    var newRow = aliasDataTable.NewRow();
                    newRow["AliasName"] = alias.ID;
                    newRow["DestinationEquipmentTypes1"] = alias.InternalDestinationTypes[0];
                    newRow["SourceEquipmentTypes1"] = alias.InternalSourceTypes[0];
                    AddMeasurementValues(
                        newRow,
                        alias.VolumeUnits,
                        Convert.ToByte(alias.VolumeDecimalPlaces),
                        alias.DensityUnits,
                        Convert.ToByte(alias.DensityDecimalPlaces),
                        alias.MassUnits,
                        Convert.ToByte(alias.MassDecimalPlaces),
                        alias.TemperatureUnits,
                        Convert.ToByte(alias.TemperatureDecimalPlaces));

                    AddAdditiveVolumeMeasurementValues(
                        newRow,
                        alias.AdditiveVolumeUnits,
                        Convert.ToByte(alias.AdditiveVolumeDecimalPlaces));

                    newRow["TransactionAliasGuid"] = alias.MasterRecordGuid;
                    aliasDataTable.Rows.Add(newRow);

                    Guid savedSiteGuid = this.Security.SiteGuid;
                    this.Security.SiteGuid = alias.SiteGuid;


                    var userDataFieldCollection =
                        FMChannelHelper.MakeCall<IUserDataFields, UserDataFieldCollectionClass>(
                            userDataFieldService => userDataFieldService.EnumerateByEntityType(this.Security, ENTITY_TYPE.TRANSACTION_ALIAS, alias.MasterRecordGuid, false, false));

                    this.Security.SiteGuid = savedSiteGuid;

                    foreach (UserDataFieldClass userDataField in userDataFieldCollection)
                    {
                        if (userDataField.UserDataType != USER_DATA_TYPE.LIST)
                        {
                            continue;
                        }

                        newRow = userDataFieldTable.NewRow();
                        newRow["Number"] = userDataField.Number;
                        newRow["TransactionAliasGuid"] = userDataField.TransactionAliasGuid;
                        newRow["UserDataFieldTransactionAliasGuid"] = userDataField.IdentityGuid;
                        userDataFieldTable.Rows.Add(newRow);

                        var userDataListValueCollection =
                            FMChannelHelper.MakeCall<IUserDataListValues, UserDataListValueCollectionClass>(
                                userDataFieldService =>
                                userDataFieldService.Enumerate(this.Security, userDataField.IdentityGuid, ENTITY_TYPE.TRANSACTION_ALIAS));

                        foreach (UserDataListValueClass userDataListValue in userDataListValueCollection)
                        {
                            newRow = userDataFieldListValueTable.NewRow();
                            newRow["Value"] = userDataListValue.ID;
                            newRow["UserDataFieldTransactionAliasGuid"] = userDataListValue.UserDataFieldGuid;
                            newRow["UserDataListValueTransactionAliasGuid"] = userDataListValue.IdentityGuid;
                            userDataFieldListValueTable.Rows.Add(newRow);
                        }
                    }

                    var productMapCollection =
                        FMChannelHelper.MakeCall<IProductMaps, ProductMapCollectionClass>(
                            productMapService => productMapService.EnumerateByAssignedToGuidAndType(this.Security, alias.MasterRecordGuid, PRODUCT_MAP_TYPE.TRANSACTION_ALIAS_EXCLUSION_MAP));

                    foreach (ProductMapClass productMap in productMapCollection)
                    {
                        newRow = productToTransactionAliasExclusionTable.NewRow();
                        newRow["ProductGuid"] = productMap.AssignedGuid;
                        newRow["AssignedToTransactionAliasGuid"] = productMap.AssignedToGuid;
                        newRow["ProductToTransactionAliasExclusionGuid"] = productMap.IdentityGuid;
                        productToTransactionAliasExclusionTable.Rows.Add(newRow);
                    }

                }

                AddNewDataTable(aliasDataTable);
                AddNewDataTable(userDataFieldTable);
                AddNewDataTable(userDataFieldListValueTable);
                AddNewDataTable(productToTransactionAliasExclusionTable);
            }
            catch (Exception ex)
            {
                string exceptionMessage = "Error processing transaction alias data";
                string eventLogMessage = exceptionMessage + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                Helper.NspaADCEventLog.WriteEntry(eventLogMessage, EventLogEntryType.Error);
                throw new Exception(exceptionMessage, ex);
            }
		}

		private void ProcessPersonnel()
		{
            try
            {
                var personList = FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(
                personnelService => personnelService.Enumerate(this.Security, true)); // true means hide inactive entities

                byte[] newSeed = new byte[Seed.Length + DummyData.Length];
                Buffer.BlockCopy(Seed, 0, newSeed, 0, Seed.Length);
                Buffer.BlockCopy(DummyData, 0, newSeed, Seed.Length, DummyData.Length);
                using (var key = new AESKey(newSeed, this.Security.SiteGuid.ToByteArray()))
                {

                    var personnelRows = from person in personList
                                        where person.LockedOut == false
                                        select
                                            new
                                            {
                                                PersonID = person.ID,
                                                PIN = encryptor.Encrypt(person.PINNumber, key),
                                                person.PINRequired,
                                                PersonnelGuid = person.MasterRecordGuid,
                                                UserGuid = person.UserGuid
                                            };

                    var returnDataTable = ListToDataTable(null, personnelRows, EntityTypes.Personnel.TableName());
                    SetFieldLength(returnDataTable, "PersonID", 50);

                    AddNewDataTable(returnDataTable);
                }
            }
            catch (Exception ex)
            {
                string exceptionMessage = "Error processing personnel data";
                string eventLogMessage = exceptionMessage + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                Helper.NspaADCEventLog.WriteEntry(eventLogMessage, EventLogEntryType.Error);
                throw new Exception(exceptionMessage, ex);
            }
		}

		private void ProcessIATA()
		{
            try
            {
                var IATACodeList = FMChannelHelper.MakeCall<IIATACodes, IATACodeCollectionClass>(
                IATACodeService => IATACodeService.Enumerate(this.Security));

                var IATACodeRows = from IATACode in IATACodeList
                                   select
                                       new
                                       {
                                           IATAID = IATACode.ID,
                                           IATAGuid = IATACode.IdentityGuid
                                       };

                var returnDataTable = ListToDataTable(null, IATACodeRows, EntityTypes.IATA.TableName());
                SetFieldLength(returnDataTable, "IATAID", 50);

                AddNewDataTable(returnDataTable);
            }
            catch (Exception ex)
            {
                string exceptionMessage = "Error processing iata data";
                string eventLogMessage = exceptionMessage + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                Helper.NspaADCEventLog.WriteEntry(eventLogMessage, EventLogEntryType.Error);
                throw new Exception(exceptionMessage, ex);
            }
		}

		private void ProcessGates()
		{
            try
            {
                var gateList = FMChannelHelper.MakeCall<IGates, GateCollectionClass>(
                gateService => gateService.Enumerate(this.Security));

                var gateRows = from gate in gateList
                               select
                                   new
                                   {
                                       gate.ID,
                                       GateGuid = gate.IdentityGuid
                                   };

                var returnDataTable = ListToDataTable(null, gateRows, EntityTypes.Gate.TableName());
                SetFieldLength(returnDataTable, "ID", 10);

                AddNewDataTable(returnDataTable);
            }
            catch (Exception ex)
            {
                string exceptionMessage = "Error processing gate data";
                string eventLogMessage = exceptionMessage + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                Helper.NspaADCEventLog.WriteEntry(eventLogMessage, EventLogEntryType.Error);
                throw new Exception(exceptionMessage, ex);
            }
		}

		private void ProcessMeters()
		{
            try
            {
                var meterList = FMChannelHelper.MakeCall<IMeters, List<MeterClass>>(
                                                                         meterService =>
                                                                         meterService.Enumerate(this.Security)
                                                                    );

                var meterRows = from meter in meterList
                                select
                                    new  { MeterID = meter.ID, MeterGuid = meter.IdentityGuid };
                var returnDataTable = ListToDataTable(null, meterRows, EntityTypes.Meter.TableName());
                SetFieldLength(returnDataTable, "MeterID", 30);

                AddNewDataTable(returnDataTable);
            }
            catch (Exception ex)
            {
                string exceptionMessage = "Error processing meter data";
                string eventLogMessage = exceptionMessage + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                Helper.NspaADCEventLog.WriteEntry(eventLogMessage, EventLogEntryType.Error);
                throw new Exception(exceptionMessage, ex);
            }
		}

		private void ProcessTanks()
		{
            try
            {
                var tankList = FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(
                                                                         tankService =>
                                                                         tankService.Enumerate(this.Security)
                                                                    );

                var managerCompany = NspaADCProcessor.FindCompanyWithDefault(this.Security, COMPANY_ROLE.MANAGER);
                var tankRows = from tank in tankList
                               where tank.ManagerGuid.Equals(managerCompany.IdentityGuid)
                               select
                                   new
                                   {
                                       TankID = tank.ID,
                                       TankGuid = tank.IdentityGuid,
                                       tank.ProductGuid,
                                       HiddenDate = ConvertDateTimeOffsetToDateTime(tank.HiddenDate)
                                   };

                int tankCount = tankRows.ToList().Count;
                if (tankCount == 0)
                {
                    Helper.NspaADCEventLog.WriteEntry("Warning: no tanks found", EventLogEntryType.Warning);
                    return;
                }

                var returnDataTable = ListToDataTable(null, tankRows, EntityTypes.Tank.TableName());
                SetFieldLength(returnDataTable, "TankID", 50);

                AddNewDataTable(returnDataTable);
            }
            catch (Exception ex)
            {
                string exceptionMessage = "Error processing tank data";
                string eventLogMessage = exceptionMessage + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                Helper.NspaADCEventLog.WriteEntry(eventLogMessage, EventLogEntryType.Error);
                throw new Exception(exceptionMessage, ex);
            }
		}

		private AssociatedParentTxListDO GetAssociatedParentTxList(Guid targetAliasGuid)
		{
			var associatedParentTxSR = new GetAssociatedParentTxSR
			{
				TransactionAliasGuid = targetAliasGuid,
				SubTypeRequest = GetAssociatedParentTxSR.AssociatedParentTxRequest.GET_ASSOCIATED_PARENT_TX,
				Security = this.Security,
				CurrentSiteGuid = this.Security.SiteGuid,
			};

			var associatedParentTxListDO =
				FMChannelHelper.MakeCall<IGetAssociatedParentTxProcessor, AssociatedParentTxListDO>(
																	 x =>
																	 x.Process(associatedParentTxSR));

			return associatedParentTxListDO;
		}

		private static Guid GetAssociatedDocGuid(string transId)
		{
			Guid docGuid;
			if (!Guid.TryParse(transId, out docGuid))
			{
				docGuid = Guid.NewGuid();
			}
			return docGuid;
		}

		private void ProcessAssociatedDocuments()
		{
            try
            {
                var aliasCollection = this.AliasList;

                var customsTransferAliasList = from alias in aliasCollection
                                               where alias.TransTypeID == CustomsTransferAliasType
                                               select new { alias.IdentityGuid };
                foreach (var alias in customsTransferAliasList)
                {
                    var associatedDocumentArray = GetAssociatedParentTxList(alias.IdentityGuid);
                    if (associatedDocumentArray != null && associatedDocumentArray.List != null)
                    {
                        var associatedDocumentList = associatedDocumentArray.List.Cast<AssociatedParentTxDO>().ToList();
                        var associatedDocumentRows = from associatedDocument in associatedDocumentList
                                                     select new { associatedDocument.DocumentNumber, DocumentNumberGuid = GetAssociatedDocGuid(associatedDocument.TransID) };

                        var returnDataTable = ListToDataTable(null, associatedDocumentRows, EntityTypes.DocumentNumber.TableName());
                        AddNewDataTable(returnDataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                string exceptionMessage = "Error processing associated document data";
                string eventLogMessage = exceptionMessage + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                Helper.NspaADCEventLog.WriteEntry(eventLogMessage, EventLogEntryType.Error);
                throw new Exception(exceptionMessage, ex);
            }
		}
		/// <summary>
		/// Compresses the data table.
		/// </summary>
		/// <param name="newDataTable">The new data table.</param>
		/// <returns></returns>
		private static byte[] CompressDataTable(DataTable newDataTable)
		{
			byte[] compressedBytes;
			using (var compressedStream = new MemoryStream())
			using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
			{
				newDataTable.WriteXml(zipStream, XmlWriteMode.WriteSchema);
				zipStream.Close();
				compressedBytes = compressedStream.ToArray();
				compressedStream.Close();					
			}
			return compressedBytes;
		}       

		private void ProcessFuelCards()
		{
            try
            {
                var fuelCardList = FMChannelHelper.MakeCall<IFuelCards, FuelCardCollectionClass>(
                fuelCardService => fuelCardService.EnumerateFuelCards(this.Security));

                Byte[] PinNumber = { 0x55, 0xFF };

                byte[] newSeed = new byte[Seed.Length + DummyData.Length];
                Buffer.BlockCopy(Seed, 0, newSeed, 0, Seed.Length);
                Buffer.BlockCopy(DummyData, 0, newSeed, Seed.Length, DummyData.Length);

                SiteTimeConverter timeConverter = new SiteTimeConverter(this.site);
                DateTimeOffset localNow = timeConverter.ConvertToSiteTime(DateTimeOffset.Now);

                using (var key = new AESKey(newSeed, this.Security.SiteGuid.ToByteArray()))
                {

                    var fuelCardRows = from fuelCard in fuelCardList
                                       where fuelCard.Status == FuelCardClass.Statuses.ACTIVE && (fuelCard.ExpirationDate == null || fuelCard.ExpirationDate > localNow)
                                       select new
                                       {
                                           fuelCard.ID,
                                           PIN = encryptor.Encrypt(fuelCard.PIN, key),
                                           BillToCompanyGuid = fuelCard.BillToGuid,
                                           ShipToCompanyGuid = fuelCard.ShipToGuid,
                                           FuelCardGuid = fuelCard.IdentityGuid,
                                           ExpirationDate = (fuelCard.ExpirationDate == null) ? localNow.DateTime : ((DateTimeOffset)fuelCard.ExpirationDate).DateTime,
                                           fuelCard.TransientCardFlag,
                                           fuelCard.UserData3,
                                           fuelCard.FuelCardTypeApplicationStringGuid,
                                           HiddenDate = ConvertDateTimeOffsetToDateTime(fuelCard.HiddenDate)
                                       };
                    var returnDataTable = ListToDataTable(null, fuelCardRows.ToList(), EntityTypes.FuelCard.TableName());
                    SetFieldLength(returnDataTable, "ID", 100);
                    SetFieldLength(returnDataTable, "UserData3", 60);

                    var fuelCardGuidList = from fuelCard in fuelCardRows select fuelCard.FuelCardGuid;
                    this.fuelCardGuidList = fuelCardGuidList.ToArray();

                    AddNewDataTable(returnDataTable);
                }
            }
            catch (Exception ex)
            {
                string exceptionMessage = "Error processing fuel card data";
                string eventLogMessage = exceptionMessage + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                Helper.NspaADCEventLog.WriteEntry(eventLogMessage, EventLogEntryType.Error);
                throw new Exception(exceptionMessage, ex);
            }
		}

        private void ProcessFuelCardLimits()
        {
            try
            {
                var fuelCardLimitTable =
                    FMChannelHelper.MakeCall<IFuelCardLimits, DataTable>(
                        fuelCardLimitService => fuelCardLimitService.EnumerateForMobile(this.Security));

                var fuelCardLimitRows = fuelCardLimitTable.Select();
                var fuelCardLimits = from fuelCardLimitRow in fuelCardLimitRows
                    select
                        (new
                         {
                             FuelCardLimitId = fuelCardLimitRow["FuelCardLimitId"].ToString(),
                             FuelCardLimitGuid = fuelCardLimitRow["FuelCardLimitGuid"].ToString(),
                             LimitAmount = fuelCardLimitRow["LimitAmount"].ToString(),
                             ProductID = fuelCardLimitRow["ProductID"].ToString(),
                             LimitPeriod = fuelCardLimitRow["LimitPeriod"].ToString(),
                             FuelCardId = fuelCardLimitRow["FuelCardId"].ToString()
                         });

                if (fuelCardLimitTable != null && fuelCardLimitTable.Rows.Count > 0)
                {
                    var returnDataTable = ListToDataTable(null, fuelCardLimits, EntityTypes.FuelCardLimit.TableName());
                    //SetFieldLength(returnDataTable, "FuelCardLimitId", 30);
                    AddNewDataTable(returnDataTable);
                }
            }
            catch (Exception ex)
            {
                string exceptionMessage = "Error processing iata data";
                string eventLogMessage = exceptionMessage + Environment.NewLine + ex.Message + Environment.NewLine
                                         + ex.StackTrace;
                Helper.NspaADCEventLog.WriteEntry(eventLogMessage, EventLogEntryType.Error);
                throw new Exception(exceptionMessage, ex);
            }
        }
        
        public static TField GetRowField<TField>(DataRow row, string fieldName, TField defaultValue)
		{
			TField newValue;

			var isNull = row.IsNull(fieldName);

			if (isNull)
			{
				newValue = defaultValue;
			}
			else
			{
				newValue = (TField)row[fieldName];
			}

			return newValue;
		}
	}
}
