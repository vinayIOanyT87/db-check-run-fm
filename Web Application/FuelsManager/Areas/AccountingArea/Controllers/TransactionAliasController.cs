using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.Interfaces;
using FMBusinessObjects.ReportSvr2005;
using FMBusinessObjects.ServiceRequests;
using FuelsManager.Areas.AccountingArea.ViewModels;
using FuelsManager.Areas.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace FuelsManager.Areas.AccountingArea.Controllers
{
	 [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
	public class TransactionAliasController : FMBaseControllerEx
	{
        //private string TranslatedNone = FMBaseController.TranslateText("None");

        // GET: AccountingArea/TransactionAlias
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult TransactionAliasDetail(string id)
        {
            var model = new TransactionAliasDetailModel();
            try
            {
                var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));
				bool orderEntryHardwareKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsAnOrderEntryKey());

                if (string.IsNullOrEmpty(id) || id.Equals("PointIndex", StringComparison.InvariantCultureIgnoreCase))
                {
                }
                else
                {
					var TransactionAliasGuid = new Guid(id);
                    var transactionAlias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(x => x.Get(this.Security, TransactionAliasGuid, false));
					var placementInfo = FMChannelHelper.MakeCall<ITransactionAliasFieldPlacementInformation, TransactionAliasFieldPlacementInformationClass>(x => x.GetByTransactionAlias(this.Security, TransactionAliasGuid));
					GroupCollectionClass groupCollection = FMChannelHelper.MakeCall<IGroups, GroupCollectionClass>(groups => groups.Enumerate(this.Security));
					// Create a new service request
					var sr = new TransactionAliasListSR();

					// Initialize service reqeust
					sr.Security = this.Security;
					// Get the list of aliases
					TransactionAliasListDO aliasListDO = FMChannelHelper.MakeCall<ITransactionAliasListProcessor, TransactionAliasListDO>(x => x.Process(sr));

					model = new TransactionAliasDetailModel(transactionAlias, placementInfo, groupCollection, site);

					// populate unit dropdowns
					model.LevelUnits = this.GetUnitsDropDownList(EngineeringUnit.FmlFtIn8Th, EngineeringUnit.FmlMile);
					model.VolumeUnits = this.GetUnitsDropDownList(EngineeringUnit.FmvCm3, EngineeringUnit.FmvKl);
					model.AdditiveVolumeUnits = this.GetUnitsDropDownList(EngineeringUnit.FmvCm3,EngineeringUnit.FmvKl);
					model.TemperatureUnits = this.GetUnitsDropDownList(EngineeringUnit.FmtDegC,EngineeringUnit.FmtDegR);
					model.DensityUnits = this.GetUnitsDropDownList(EngineeringUnit.FmdGcm3,EngineeringUnit.FmdSTnYd3);
					model.MassUnits = this.GetUnitsDropDownList(EngineeringUnit.FmmGram,EngineeringUnit.FmmMlbs);
					model.FlowUnits = this.GetUnitsDropDownList(EngineeringUnit.FmvfCcMin,EngineeringUnit.FmvfKlDay);
					model.PressureUnits = this.GetUnitsDropDownList(EngineeringUnit.FmpPa,EngineeringUnit.FmpAtm);

					model.ExcludedProductList = transactionAlias.ExcludedProductCollection.Select( x => new Tuple<string, string>( x.IdentityGuid.ToString(), x.ID)).ToList();

					model.ModifyEnabled = this.Security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES) && (this.Security.SiteGuid == model.SiteGuid || model.SiteGuid == Guid.Empty);

					// Populate TransactionTypeDropDownList
					var transactionTypeList = new List<TransactionTypeList>();
					for (var type = TransactionTypes.T1_PrimaryAdjustment; type < TransactionTypes.T_Maximum; type++)
					{
						if (type == TransactionTypes.T21_AccountPayableInvoice || type == TransactionTypes.T22_AccountReceivableInvoice)
						{
							continue;
						}

						if (type == TransactionTypes.T19_EndOfDay || type == TransactionTypes.T20_EndOfMonth
							|| type == TransactionTypes.T24_Aggregate)
						{
							continue;
						}

						// If the hardware key is not set, do not let Order alias be an option
						if ((type == TransactionTypes.T17_Order || type == TransactionTypes.T18_SupplyOrder)
							&& orderEntryHardwareKey == false)
						{
							continue;
						}

						var item = new TransactionTypeList(type.ToString(), TransactionAliasClass.TransactionTypeID(type));
						transactionTypeList.Add(item);

					}
					model.transactionTypeList = transactionTypeList;

					// Populate TransactionTypeDropDownList
					var showCompanyOptionsList = new List<ShowCompanyOptions>();

					for (var type = TRANSACTION_SHOW_COMPANY_NAME.SHOW_ID_ONLY;
						 type <= TRANSACTION_SHOW_COMPANY_NAME.SHOW_NAME_AND_ID;
						 type++)
					{
						var item = new ShowCompanyOptions(type.ToString(), TransactionAliasClass.GetShowCompanyDisplayName(type) );
						showCompanyOptionsList.Add(item);
					}
					model.showCompanyOptions = showCompanyOptionsList;

					// get the list of aliases that we can associate for Orders and Supply Orders
					var associatedAliasList = new List<AliasEditorTransactionAliasList>();
					if (aliasListDO.aliasList.Keys.Count > 0)
					{
						associatedAliasList.Add(new AliasEditorTransactionAliasList {
								type="All",
								key= this.GetTranslatedText("{None}"), 
								value= Guid.Empty});

						foreach (TransactionAliasClass alias in aliasListDO.aliasList.Values)
						{
							if (alias.TransTypeID != TransactionTypes.T5_PrimaryDisbursement
									&& alias.TransTypeID != TransactionTypes.T6_SecondaryDisbursement
									&& alias.TransTypeID != TransactionTypes.T3_PrimaryDefuel
									&& alias.TransTypeID != TransactionTypes.T4_SecondaryDefuel
									&& alias.TransTypeID != TransactionTypes.T25_Shipment 
									&& alias.TransTypeID != TransactionTypes.T8_Receipt)
							{
								continue;
							}

							// for Orders we want T5_PrimaryDisbursement, T6_SecondaryDisbursement,  T3_PrimaryDefuel, T4_SecondaryDefuel, T25_Shipment, T8_Receipt
							associatedAliasList.Add(new AliasEditorTransactionAliasList
							{
								type = alias.TransTypeID == TransactionTypes.T8_Receipt ? "SupplyOrder" : "Order",
								key = alias.ID,
								value = alias.IdentityGuid
							});

						}
					}

					model.AssociatedAliases = associatedAliasList;

				}
            }
            catch (Exception except)
            {
                this.OnError(except);
            }

            return this.View(model);
        }

		[HttpGet]
		public ActionResult TransactionAliasAdd()
		{
			var model = new TransactionAliasDetailModel();
			try
			{
				bool orderEntryHardwareKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsAnOrderEntryKey());
				// Populate TransactionTypeDropDownList
				var transactionTypeList = new List<TransactionTypeList>();
				for (var type = TransactionTypes.T1_PrimaryAdjustment; type < TransactionTypes.T_Maximum; type++)
				{
					if (type == TransactionTypes.T21_AccountPayableInvoice || type == TransactionTypes.T22_AccountReceivableInvoice)
					{
						continue;
					}

					if (type == TransactionTypes.T19_EndOfDay || type == TransactionTypes.T20_EndOfMonth
						|| type == TransactionTypes.T24_Aggregate)
					{
						continue;
					}

					// If the hardware key is not set, do not let Order alias be an option
					if ((type == TransactionTypes.T17_Order || type == TransactionTypes.T18_SupplyOrder)
						&& orderEntryHardwareKey == false)
					{
						continue;
					}

					var item = new TransactionTypeList(type.ToString(), TransactionAliasClass.TransactionTypeID(type));
					transactionTypeList.Add(item);

				}
				model.transactionTypeList = transactionTypeList;
			}
			catch (Exception except)
			{
				this.OnError(except);
			}

			return this.View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult TransactionAliasEdit(TransactionAliasDetailModel model, string userGroups, string excludedProductGuidList, string statusList, string defaultStatus, string fieldGrid)
		{
			try
			{
				// Revalidate the model.  If the validation fails with the data annotations (we are checking for require fields there) it will
				// not call the IValidatableObject.Validate method so we may be missing error messages.
				// By forcing a call to the validation we may get duplicate error messages so we need to remove them in the client.
				this.TryValidateModel(model);

				fieldGrid = Server.UrlDecode(fieldGrid);
				userGroups = Server.UrlDecode(userGroups);
				excludedProductGuidList = Server.UrlDecode(excludedProductGuidList);
				statusList = Server.UrlDecode(statusList);

				// convert the editor entries into a list (had problems with the default MVC binder automatically doing it so I need to do it manually )
				JavaScriptSerializer jss = new JavaScriptSerializer();
				if (!string.IsNullOrEmpty(userGroups))
				{
					model.GroupTransactionAliasMapCollection = jss.Deserialize<List<UserGroupMap>>(userGroups);
				}

				var excludedProducts = new List<AliasEditorKeyGuiValue>();
				if (!string.IsNullOrEmpty(excludedProductGuidList))
				{
					excludedProducts = jss.Deserialize< List<AliasEditorKeyGuiValue> >(excludedProductGuidList);
				}

				var selectedStatusList = new List<string>();
				selectedStatusList = jss.Deserialize<List<string>>(statusList);

				// convert the editor entries into a list (had problems with the default MVC binder automatically doing it so I need to do it manually )
				if (!string.IsNullOrEmpty(fieldGrid))
				{
					model.FieldCollection = jss.Deserialize<List<AliasEditorFieldClass>>(fieldGrid);
				}

				var transactionAlias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(x => x.Get(this.Security, model.IdentityGuid, true));

				var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

				var numberFormatInfo = new NumberFormatInfo
				{
					NumberGroupSizes = site.GetNumberGroupSizes(),
					NumberGroupSeparator = site.NumberGroupSeparator,
					NumberDecimalSeparator = site.NumberDecimalSeparator
				};

				Guid tempGuid = Guid.Empty;
				model.Site = site;

				// modify the alias
				if (this.ModelState.IsValid)
				{
					if (model.IdentityGuid == Guid.Empty)
					{
						throw new Exception("GUID cannot be blank.");
					}
					// changing the transaction alias resets fields
					TransactionTypes transType;
					if (Enum.TryParse<TransactionTypes>(model.TransTypeID, out transType))
					{
						if (transactionAlias.TransTypeID != transType)
						{
							transactionAlias.TransTypeID = transType;
						}
					}
					TRANSACTION_SHOW_COMPANY_NAME ShowCompanyName;
					if (Enum.TryParse<TRANSACTION_SHOW_COMPANY_NAME>(model.ShowCompanyName, out ShowCompanyName))
					{
						transactionAlias.ShowCompanyName = ShowCompanyName;
					}
					transactionAlias.ID = model.Name;

					// update checkboxes
					transactionAlias.MeterCloseout = model.MeterCloseout;
					transactionAlias.UseTransactionDetailWithLayout = model.UseTransactionDetailWithLayout;
					transactionAlias.LimitSelectionsBasedOnHierarchy = model.LimitSelectionsBasedOnHierarchy;
					transactionAlias.PermitNonReferenceData = model.PermitNonReferenceData;
					transactionAlias.DistributedImpact = model.DistributedImpact;
					transactionAlias.BulkShipment = model.BulkShipment;
					transactionAlias.UseComboxControls = model.UseComboxControls;
					transactionAlias.MultipleWeightReadings = model.MultipleWeightReadings;
					transactionAlias.MultipleLineItems = model.MultipleLineItems;
					transactionAlias.EnableAutoCompleteControls = model.EnableAutoCompleteControls;
					transactionAlias.MultipleTransportLineItems = model.MultipleTransportLineItems;
					transactionAlias.DefaultMeterToEquipmentID = model.DefaultMeterToEquipmentID;
					transactionAlias.RememberMeterEndForMeterID = model.RememberMeterEndForMeterID;
					transactionAlias.PopulateCompaniesFromEquipment = model.PopulateCompaniesFromEquipment;
					transactionAlias.PopulateGrossVolumeFromMeterValues = model.PopulateGrossVolumeFromMeterValues;
					transactionAlias.LimitSourceEquipmentByProduct = model.LimitSourceEquipmentByProduct;
					transactionAlias.UseMeterAndCompressionFactorFromMeter = model.UseMeterAndCompressionFactorFromMeter;
					transactionAlias.LevelDecimalPlaces = model.LevelDecimalPlaces;
					if (model.LevelUnitsInt == 0)
					{
						transactionAlias.LevelUnits = EngineeringUnit.FmSiteUnits;
					}
					else { // I can't directly set the unitInt to siteunit
						transactionAlias.LevelUnitsInt =  model.LevelUnitsInt;
                    }
					transactionAlias.VolumeDecimalPlaces = model.VolumeDecimalPlaces;
					if (model.VolumeUnitsInt == 0)
					{
						transactionAlias.VolumeUnits = EngineeringUnit.FmSiteUnits;
					}
					else
					{ // I can't directly set the unitInt to siteunit
						transactionAlias.VolumeUnitsInt = model.VolumeUnitsInt;
					}
					transactionAlias.AdditiveVolumeDecimalPlaces = model.AdditiveVolumeDecimalPlaces;
					if (model.AdditiveVolumeUnitsInt == 0)
					{
						transactionAlias.AdditiveVolumeUnits = EngineeringUnit.FmSiteUnits;
					}
					else
					{ // I can't directly set the unitInt to siteunit
						transactionAlias.AdditiveVolumeUnitsInt = model.AdditiveVolumeUnitsInt;
					}
					transactionAlias.TemperatureDecimalPlaces = model.TemperatureDecimalPlaces;
					if (model.TemperatureUnitsInt == 0)
					{
						transactionAlias.TemperatureUnits = EngineeringUnit.FmSiteUnits;
					}
					else
					{ // I can't directly set the unitInt to siteunit
						transactionAlias.TemperatureUnitsInt = model.TemperatureUnitsInt;
					}
					transactionAlias.DensityDecimalPlaces = model.DensityDecimalPlaces;
					if (model.DensityUnitsInt == 0)
					{
						transactionAlias.DensityUnits = EngineeringUnit.FmSiteUnits;
					}
					else
					{ // I can't directly set the unitInt to siteunit
						transactionAlias.DensityUnitsInt = model.DensityUnitsInt;
					}
					transactionAlias.MassDecimalPlaces = model.MassDecimalPlaces;
					if (model.MassUnitsInt == 0)
					{
						transactionAlias.MassUnits = EngineeringUnit.FmSiteUnits;
					}
					else
					{ // I can't directly set the unitInt to siteunit
						transactionAlias.MassUnitsInt = model.MassUnitsInt;
					}
					transactionAlias.FlowDecimalPlaces = model.FlowDecimalPlaces;
					if (model.FlowUnitsInt == 0)
					{
						transactionAlias.FlowUnits = EngineeringUnit.FmSiteUnits;
					}
					else
					{ // I can't directly set the unitInt to siteunit
						transactionAlias.FlowUnitsInt = model.FlowUnitsInt;
					}
					transactionAlias.PressureDecimalPlaces = model.PressureDecimalPlaces;
					if (model.PressureUnitsInt == 0)
					{
						transactionAlias.PressureUnits = EngineeringUnit.FmSiteUnits;
					}
					else
					{ // I can't directly set the unitInt to siteunit
						transactionAlias.PressureUnitsInt = model.PressureUnitsInt;
					}
					transactionAlias.AssociatedReport = model.AssociatedReport == null ? "": model.AssociatedReport;
					transactionAlias.AssociatedPreloadReport = model.AssociatedPreloadReport == null ? "": model.AssociatedPreloadReport;

					// process user groups
					// remove the user groups that need to be deleted
					GroupTransactionAliasMapCollectionClass newGroupList = new GroupTransactionAliasMapCollectionClass();
					foreach (var group in transactionAlias.GroupTransactionAliasMapCollection)
					{
						if (model.GroupTransactionAliasMapCollection.Any(p2 => p2.Guid == group.GroupGuid))
						{
							newGroupList.Add(group);
						}
					}
					transactionAlias.GroupTransactionAliasMapCollection = newGroupList;

					// add missing user groups
					foreach ( var usergroup in model.GroupTransactionAliasMapCollection) {
						var existingGroup = transactionAlias.GroupTransactionAliasMapCollection.Find( x => x.GroupGuid == usergroup.Guid);
						if (existingGroup != null ) { 
							if(existingGroup.Right != (GroupTransactionAliasMapClass.RIGHT)usergroup.Right) {
								existingGroup.Right = (GroupTransactionAliasMapClass.RIGHT)usergroup.Right;
							}
						} else {
							transactionAlias.GroupTransactionAliasMapCollection.Add(
								new GroupTransactionAliasMapClass()
								{
									GroupGuid = (Guid)usergroup.Guid,
									Right = (GroupTransactionAliasMapClass.RIGHT)usergroup.Right,
									SiteGuid = model.Site.SiteGuid,
									TransactionAliasGuid = model.IdentityGuid

								}) ;

						}
					}

					// process excluded products
					// remove the products that need to be deleted
					ProductMapCollectionClass newProductList = new ProductMapCollectionClass();
					foreach (var product in transactionAlias.ExcludedProductCollection)
					{
						if (excludedProducts.Any(p2 => p2.value == product.AssignedGuid))
						{
							newProductList.Add(product);
						}
					}
					transactionAlias.ExcludedProductCollection = newProductList;

					// add missing products
					foreach (var product in excludedProducts)
					{
						var existingProduct = transactionAlias.ExcludedProductCollection.Find(x => x.AssignedGuid == product.value);
						if (existingProduct == null) {
							transactionAlias.ExcludedProductCollection.Add(
								new ProductMapClass()
								{
									AssignedGuid = product.value,
									AssignedID = product.key,
									Type = PRODUCT_MAP_TYPE.TRANSACTION_ALIAS_EXCLUSION_MAP,
									SiteGuid = model.Site.SiteGuid,
									AssignedToGuid = model.IdentityGuid
								}) ; 
						}

					}

					// process statuses
					ArrayList selectedStatusArrayList = new ArrayList();
					foreach( var status in selectedStatusList) {
						selectedStatusArrayList.Add((int)(TransactionStatus)Enum.Parse(typeof(TransactionStatus), status));

					}
					transactionAlias.AssignedStatuses = selectedStatusArrayList;
					transactionAlias.LookupDefaultStatusIndex = string.IsNullOrEmpty(defaultStatus) ? -1 : (int)(TransactionStatus)Enum.Parse(typeof(TransactionStatus), defaultStatus);
				}

				// process fields
				foreach( var field in model.FieldCollection)
				{
                    switch (field.typeName)
                    {
						case "Transaction":
							updateField(transactionAlias.TransactionFieldCollection, field, model.Site.SiteGuid, model.IdentityGuid);
							break;
						case "Line Item":
							updateField(transactionAlias.LineItemFieldCollection, field, model.Site.SiteGuid, model.IdentityGuid);
							break;
						case "Note":
							updateField(transactionAlias.NoteFieldCollection, field, model.Site.SiteGuid, model.IdentityGuid);
							break;
						case "Transport Line Item":
							updateField(transactionAlias.TransactionFieldCollection, field, model.Site.SiteGuid, model.IdentityGuid);
							break;
						case "Weight Reading":
							updateField(transactionAlias.WeightReadingFieldCollection, field, model.Site.SiteGuid, model.IdentityGuid);
							break;
						case "Export Result":
							updateField(transactionAlias.ExportResultDetailFieldCollection, field, model.Site.SiteGuid, model.IdentityGuid);
							break;
						default:
                            break;
                    }

                }


				if (this.ModelState.IsValid)
				{
					FMChannelHelper.MakeCall<ITransactionAliases>(x => x.Modify(this.Security, transactionAlias));

					this.AddSuccess("Save Successful");
				}
				else // if failed validation
				{
					return this.JsonWithErrorMessages(null);
				}
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null);

			}

			return this.JsonWithErrorMessages(null);
		}

		// Add/update fields in the alias class
		private TransactionAliasFieldCollectionClass updateField ( TransactionAliasFieldCollectionClass fieldCollection, AliasEditorFieldClass field, Guid siteGuid, Guid transactionALiasGuid )
		{
			var existingField = fieldCollection.ToList().Find(x => x.DbName == field.dbName);
			if (existingField == null)
			{
				fieldCollection.Add(
					new TransactionAliasFieldClass()
					{
						SiteGuid = siteGuid,
						DbName = field.dbName,
						TransactionAliasGuid = transactionALiasGuid,
						DisplayOrder = field.displayOrder,
						DisplayName = field.displayName,
						UserGroupID = field.userGroupID == "{All}" ? "" : field.userGroupID,
						UserGroupGuid = field.userGroupGuid,
						EntityType = ENTITY_TYPE.TRANSACTION_ALIAS_FIELD,
						ReadOnly = field.readOnly,
						FieldRequired = field.fieldRequired,
						Visibility = (TransactionFieldVisibility)field.visibility,
						Type = TransactionFieldType.ExportResult,
						DefaultValue = field.defaultValue,
					});
			}
			else
			{
				existingField.DisplayOrder = field.displayOrder;
				existingField.DisplayName = field.displayName;
				existingField.UserGroupID = field.userGroupID == "{All}" ? "" : field.userGroupID;
				existingField.UserGroupGuid = field.userGroupGuid;
				existingField.ReadOnly = field.readOnly;
				existingField.DefaultValue = field.defaultValue;
				existingField.FieldRequired = field.fieldRequired;
				existingField.Visibility = (TransactionFieldVisibility)field.visibility;
			}

			return fieldCollection;
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult TransactionAliasAdd(string name, string TransTypeID)
		{
			Guid newTransactionAliasGuid = new Guid();
			try
			{
				name = Server.UrlDecode(name);

				if (string.IsNullOrEmpty(name)) {
					throw new Exception("Name is required");
                }
				TransTypeID = Server.UrlDecode(TransTypeID);

				var transactionAlias = new TransactionAliasClass();
				transactionAlias.SiteGuid = this.Security.SiteGuid;
				transactionAlias.ID = name;
				var convertedTransactionType = (TransactionTypes)Enum.Parse(typeof(TransactionTypes), TransTypeID);
				transactionAlias.TransTypeID = convertedTransactionType;

				newTransactionAliasGuid = FMChannelHelper.MakeCall<ITransactionAliases, Guid>(x => x.Add(this.Security, transactionAlias));
				this.AddSuccess("Save Successful");
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null);

			}

			return this.JsonWithErrorMessages(newTransactionAliasGuid);
		}
		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GetAllFields(string transactionType) {

			try
			{

				var allFields = new List<Tuple<string, string>>();
				var convertedTransactionType = (TransactionTypes)Enum.Parse(typeof(TransactionTypes), transactionType);
				foreach (TransactionFieldType transactionFielType in Enum.GetValues( typeof(TransactionFieldType) ) ) { 
					if (transactionFielType != TransactionFieldType.TransactionFieldTypeMax) { 
						List<string> fieldNames = FMChannelHelper.MakeCall<ITransactionAliasFields, List<string>>(
							fields => fields.EnumerateFields(
								this.Security,
								fieldType: transactionFielType,
								transType: convertedTransactionType));

							foreach (var field in fieldNames)
							{
								allFields.Add(new Tuple<string, string>(TransactionAliasFieldClass.TransactionFieldTypeID(transactionFielType ), field));
							}
					}

				}
				return this.JsonWithErrorMessages(allFields, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				this.OnError(e);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GetAllUserGroups()
		{

			try
			{

				var allUserGroups = new List<Tuple<string, string>>();
				var groupCollection = FMChannelHelper.MakeCall<IGroups, GroupCollectionClass>(groups => groups.Enumerate(this.Security));

				foreach (var usergroup in groupCollection)
				{
					allUserGroups.Add(new Tuple<string, string>(usergroup.IdentityGuid.ToString(), usergroup.ID));
				}

				return this.JsonWithErrorMessages(allUserGroups, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				this.OnError(e);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GetAllStatuses()
		{

			try
			{

				var allUserStatuses = new List<Tuple<string, string>>();
				var Statuses = (int[])Enum.GetValues(typeof(TransactionStatus));
				foreach (int Status in Statuses)
				{
					allUserStatuses.Add(new Tuple<string, string>(Status.ToString(), Enum.GetName(typeof(TransactionStatus), Status)));
				}
				return this.JsonWithErrorMessages(allUserStatuses, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				this.OnError(e);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GetAllProducts()
		{
			try
			{
				var allProducts = new List<Tuple<string, string>>();
				var productCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(x => x.Enumerate(this.Security));
				foreach (var product in productCollection)
				{
					allProducts.Add(new Tuple<string, string>(product.IdentityGuid.ToString(), product.ID));
				}
				return this.JsonWithErrorMessages(allProducts, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				this.OnError(e);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GetAllReports()
		{

			try
			{

				var allReports = new List<string>();

				SystemSettingClass systemSetting =
					FMChannelHelper.MakeCall<ISystemSettings, SystemSettingClass>(x => x.Get(this.Security));

				SiteClass siteClass = FMChannelHelper.MakeCall<ISites, SiteClass>(
					sites => sites.GetBasic(this.Security, this.Security.SiteGuid));

				////**** Use ReportServerCredentials when running in azure. Use dbAccessClient when not Azure *******
				var reportingService = new ReportingService2005
				{
					Url = systemSetting.ReportServerUrl + "/ReportService2005.asmx",
					CookieContainer = new CookieContainer()
				};

				if (!string.IsNullOrEmpty(systemSetting.ReportServerUserName))
				{
					string[] userName = systemSetting.ReportServerUserName.Split('\\');
					if (userName.Length > 1)
					{
						reportingService.Credentials = new NetworkCredential(userName[1], systemSetting.ReportServerPassword, userName[0]);
					}
					else
					{
						reportingService.Credentials = new NetworkCredential(userName[0], systemSetting.ReportServerPassword, ".");
					}
				}
				else
				{
					reportingService.Credentials = CredentialCache.DefaultCredentials;
				}

				//replace // with / if necessary.  ReportPath in db may or may not have preceeding /
				string tempPath = ("/" + siteClass.ReportDirectory).Replace("//", "/");

				//remove trailing / if necessary
				if (tempPath.Substring(tempPath.Length - 1) == "/")
				{
					tempPath = tempPath.Substring(0, tempPath.Length - 1);
				}

				CatalogItem[] items = reportingService.ListChildren(tempPath, false);

				foreach (CatalogItem item in items)
				{
					if (item.Type != ItemTypeEnum.Report && item.Type != ItemTypeEnum.LinkedReport)
					{
						continue;
					}

					// Create a new item
					allReports.Add(item.Name);
				}
				return this.JsonWithErrorMessages(allReports, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				this.OnError(e);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		protected List<Tuple<string, string>> GetUnitsDropDownList( EngineeringUnit beginningUnits, EngineeringUnit endingUnits)
		{
			var unitsList = new List<Tuple<string, string>>();
			unitsList.Add(new Tuple<string, string>("<Site>", "0"));

			for (EngineeringUnit index = beginningUnits; index <= endingUnits; index++)
			{
				if (Enum.IsDefined(typeof(EngineeringUnit), index) == false)
				{
					continue;
				}

				string abbrevString;
				try
				{
					abbrevString = EngineeringUnits.GetUnitAbbreviation(index);
				}
				catch
				{
					continue;
				}

				unitsList.Add(new Tuple<string, string> (abbrevString, ((int)index).ToString(CultureInfo.InvariantCulture)));
			}
			return unitsList;
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GetAllEquipmentTypes()
		{
			try
			{

				var allEquipmentTypes = new List<Tuple<string, string>>();

				for (var type = EQUIPMENT_TYPE.TRAILER_TYPE; type < EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE; type++)
				{
					allEquipmentTypes.Add(new Tuple<string, string>(((int)type).ToString(CultureInfo.InvariantCulture), EquipmentTypeClass.TypeID(type)));
				}
				return this.JsonWithErrorMessages(allEquipmentTypes, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				this.OnError(e);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

	}
}
