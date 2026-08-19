using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

using FMBusinessObjects.DataObjects;

using FMPointCommon;
using FuelsManager.Areas.Controllers;
using Newtonsoft.Json;

namespace FuelsManager.Areas.AccountingArea.ViewModels
{
	[Serializable]
	public class TransactionAliasDetailModel : FMBaseModel
    {
        public bool ModifyEnabled = true;
        public SiteClass Site { get; set; }

        public TransactionAliasClass TransactionAlias { get; set; }

        public Guid TransactionAliasGuid { get; set; }
        public Guid IdentityGuid { get; set; }

		[Required(ErrorMessage = "TransactionAliasEditor|Name is required.")]
		public string Name { get; set; }
		public Guid SiteGuid { get; set; }
		public string TransTypeID { get; set; }
		public string ShowCompanyName { get; set; }
        public string AssociatedReport { get; set; }
        public string AssociatedPreloadReport { get; set; }
        public bool EnableAutoCompleteControls { get; set; }
        public List<ExcludedProductMap> ExcludedProductCollection { get; set; }
        public List<UserGroupMap> GroupTransactionAliasMapCollection { get; set; }
        public List<string> AssignedStatuses { get; set; }
        public string DefaultStatus { get; set; }
        public bool UseTransactionDetailWithLayout { get; set; }
        public string LevelDecimalPlaces { get; set; }
        public int LevelUnitsInt { get; set; }
        public string VolumeDecimalPlaces { get; set; }
        public int VolumeUnitsInt { get; set; }
        public string AdditiveVolumeDecimalPlaces { get; set; }
        public int AdditiveVolumeUnitsInt { get; set; }
        public List<AliasEditorFieldClass> FieldCollection { get; set; }
        public List<Tuple<string, string>> AllFieldTypes { get; set; }
        public string PlacementInfo { get; set; }
        public bool EnableQtyToleranceExceededWarning { get; set; }
        public bool EnableTotalQtyExceededWarning { get; set; }
        public bool EnableTotalValueExceededWarning { get; set; }
        public bool EnableValueToleranceExceededWarning { get; set; }
        public bool IncludeInDispatch { get; set; }
        public bool LimitSelectionsBasedOnHierarchy { get; set; }
        public bool MeterCloseout { get; set; }
        public bool MultipleLineItems { get; set; }
        public bool MultipleTransportLineItems { get; set; }
        public bool MultipleWeightReadings { get; set; }
        public List<TransactionTypeList> transactionTypeList { get; set; }
		public List<ShowCompanyOptions> showCompanyOptions { get; set; }
        public bool PermitNonReferenceData { get; set; }
        public bool UseComboxControls { get; set; }
        public bool DistributedImpact { get; set; }
        public bool BulkShipment { get; set; }
        public List<UserGroupMap> AllUserGroupTransactionAliasMapCollection { get; set; }
        public string TemperatureDecimalPlaces { get; set; }
        public int TemperatureUnitsInt { get; set; }
        public string DensityDecimalPlaces { get; set; }
        public int DensityUnitsInt { get; set; }
        public string MassDecimalPlaces { get; set; }
        public int MassUnitsInt { get; set; }
        public string FlowDecimalPlaces { get; set; }
        public int FlowUnitsInt { get; set; }
        public string PressureDecimalPlaces { get; set; }
        public int PressureUnitsInt { get; set; }
        public string AssociatedAlias { get; set; }
        public List<AliasEditorTransactionAliasList> AssociatedAliases { get; set; }
        public List<Tuple<string, string>> LevelUnits { get; internal set; }
        public List<Tuple<string, string>> AdditiveVolumeUnits { get; internal set; }
        public List<Tuple<string, string>> VolumeUnits { get; internal set; }
        public List<Tuple<string, string>> TemperatureUnits { get; internal set; }
        public List<Tuple<string, string>> DensityUnits { get; internal set; }
        public List<Tuple<string, string>> MassUnits { get; internal set; }
        public List<Tuple<string, string>> FlowUnits { get; internal set; }
        public List<Tuple<string, string>> PressureUnits { get; internal set; }
        public List<Tuple<string, string>> ExcludedProductList { get; set; }
        public bool RememberMeterEndForMeterID { get; set; }
        public bool PopulateCompaniesFromEquipment { get; set; }
        public bool PopulateGrossVolumeFromMeterValues { get; set; }
        public bool UseMeterAndCompressionFactorFromMeter { get; set; }
		public bool DefaultMeterToEquipmentID { get; set; }
		public bool LimitSourceEquipmentByProduct { get; set; }
		public TransactionAliasDetailModel() {
			this.TransactionAlias = new TransactionAliasClass();
		}

		public TransactionAliasDetailModel(TransactionAliasClass transactionAlias,
                                                TransactionAliasFieldPlacementInformationClass placementInfo,
                                                GroupCollectionClass groupCollection,
                                                SiteClass site)
		{
			var numberFormatInfo = new NumberFormatInfo
			{
				NumberGroupSizes = site.GetNumberGroupSizes(),
				NumberGroupSeparator = site.NumberGroupSeparator,
				NumberDecimalSeparator = site.NumberDecimalSeparator
			};

			this.TransactionAlias = transactionAlias;
			this.Site = site;

			this.TransactionAliasGuid = transactionAlias.IdentityGuid;
			this.IdentityGuid = transactionAlias.IdentityGuid;

			this.Name = transactionAlias.ID;
			this.SiteGuid = transactionAlias.SiteGuid;
			this.TransTypeID = transactionAlias.TransTypeID.ToString();
			this.TransactionAlias = transactionAlias;
			this.ShowCompanyName = transactionAlias.ShowCompanyName.ToString();
			this.AssociatedReport = transactionAlias.AssociatedReport;
			this.AssociatedPreloadReport = transactionAlias.AssociatedPreloadReport;
			this.EnableAutoCompleteControls = transactionAlias.EnableAutoCompleteControls;
			this.EnableQtyToleranceExceededWarning = transactionAlias.EnableQtyToleranceExceededWarning;
			this.EnableTotalQtyExceededWarning = transactionAlias.EnableTotalQtyExceededWarning;
			this.EnableTotalValueExceededWarning = transactionAlias.EnableTotalValueExceededWarning;
			this.EnableValueToleranceExceededWarning = transactionAlias.EnableValueToleranceExceededWarning;
			this.IncludeInDispatch = transactionAlias.IncludeInDispatch;
			this.LimitSelectionsBasedOnHierarchy = transactionAlias.LimitSelectionsBasedOnHierarchy;
			this.MeterCloseout = transactionAlias.MeterCloseout;
			this.MultipleLineItems = transactionAlias.MultipleLineItems;
			this.MultipleTransportLineItems = transactionAlias.MultipleTransportLineItems;
			this.MultipleWeightReadings = transactionAlias.MultipleWeightReadings;
			this.PermitNonReferenceData = transactionAlias.PermitNonReferenceData;
			this.UseComboxControls = transactionAlias.UseComboxControls;
			this.DistributedImpact = transactionAlias.DistributedImpact;
			this.BulkShipment = transactionAlias.BulkShipment;
			this.EnableAutoCompleteControls = transactionAlias.EnableAutoCompleteControls;
			this.ExcludedProductCollection = transactionAlias.ExcludedProductCollection.ToList().Select( x => new ExcludedProductMap( x.AssignedID, x.AssignedGuid)).ToList();
			this.GroupTransactionAliasMapCollection = transactionAlias.GroupTransactionAliasMapCollection.ToList().Select(x => new UserGroupMap(x.ID, x.GroupGuid, (int)x.Right)).ToList();
            this.AssignedStatuses = transactionAlias.AssignedStatuses.ToArray().Select(x => Enum.GetName(typeof(TransactionStatus), x)).ToList();
			this.DefaultStatus = transactionAlias.LookupDefaultStatusIndex != -1 ? Enum.GetName(typeof(TransactionStatus), transactionAlias.LookupDefaultStatusIndex): "" ;
			this.UseTransactionDetailWithLayout = transactionAlias.UseTransactionDetailWithLayout;
			this.DefaultMeterToEquipmentID = transactionAlias.DefaultMeterToEquipmentID;
			this.LimitSourceEquipmentByProduct = transactionAlias.LimitSourceEquipmentByProduct;
			this.RememberMeterEndForMeterID = transactionAlias.RememberMeterEndForMeterID;
			this.PopulateCompaniesFromEquipment = transactionAlias.PopulateCompaniesFromEquipment;
			this.PopulateGrossVolumeFromMeterValues = transactionAlias.PopulateGrossVolumeFromMeterValues;
			this.UseMeterAndCompressionFactorFromMeter = transactionAlias.UseMeterAndCompressionFactorFromMeter;
			this.LevelDecimalPlaces = transactionAlias.LevelDecimalPlaces;
			this.LevelUnitsInt = transactionAlias.LevelUnitsInt;
			this.VolumeDecimalPlaces = transactionAlias.VolumeDecimalPlaces;
			this.VolumeUnitsInt = transactionAlias.VolumeUnitsInt;
			this.AdditiveVolumeDecimalPlaces = transactionAlias.AdditiveVolumeDecimalPlaces;
			this.AdditiveVolumeUnitsInt = transactionAlias.AdditiveVolumeUnitsInt;
			this.TemperatureDecimalPlaces = transactionAlias.TemperatureDecimalPlaces;
			this.TemperatureUnitsInt = transactionAlias.TemperatureUnitsInt;
			this.DensityDecimalPlaces = transactionAlias.DensityDecimalPlaces;
			this.DensityUnitsInt = transactionAlias.DensityUnitsInt;
			this.MassDecimalPlaces = transactionAlias.MassDecimalPlaces;
			this.MassUnitsInt = transactionAlias.MassUnitsInt;
			this.FlowDecimalPlaces = transactionAlias.FlowDecimalPlaces;
			this.FlowUnitsInt = transactionAlias.FlowUnitsInt;
			this.PressureDecimalPlaces = transactionAlias.PressureDecimalPlaces;
			this.PressureUnitsInt = transactionAlias.PressureUnitsInt;
			this.AssociatedAlias = transactionAlias.AssociatedAlias;
			this.AssociatedAliases = new List<AliasEditorTransactionAliasList>();

			this.LevelUnits = new List<Tuple<string, string>>();
			this.VolumeUnits = new List<Tuple<string, string>>();
			this.AdditiveVolumeUnits = new List<Tuple<string, string>>();
			this.TemperatureUnits = new List<Tuple<string, string>>();
			this.DensityUnits = new List<Tuple<string, string>>();
			this.MassUnits = new List<Tuple<string, string>>();
			this.FlowUnits = new List<Tuple<string, string>>();
			this.PressureUnits = new List<Tuple<string, string>>();
			this.LevelUnits = new List<Tuple<string, string>>();
			this.LevelUnits = new List<Tuple<string, string>>();

			var fieldCollection = transactionAlias.TransactionFieldCollection.ToList().Select(x => new AliasEditorFieldClass(x.IdentityGuid, x.DbName, x.DisplayName, x.FieldRequired, x.VirtualField, "Transaction", 1, x.DisplayOrder, x.ClearOnNew, x.UserGroupGuid, x.UserGroupID, (int)x.Visibility, x.ReadOnly, x.DefaultValue)).ToList();
			fieldCollection.AddRange(transactionAlias.LineItemFieldCollection.ToList().Select(x => new AliasEditorFieldClass(x.IdentityGuid, x.DbName, x.DisplayName, x.FieldRequired, x.VirtualField, "Line Item", 2, x.DisplayOrder, x.ClearOnNew, x.UserGroupGuid, x.UserGroupID, (int)x.Visibility, x.ReadOnly, x.DefaultValue)).ToList());
			fieldCollection.AddRange(transactionAlias.NoteFieldCollection.ToList().Select(x => new AliasEditorFieldClass(x.IdentityGuid, x.DbName, x.DisplayName, x.FieldRequired, x.VirtualField, "Note", 3, x.DisplayOrder, x.ClearOnNew, x.UserGroupGuid, x.UserGroupID, (int)x.Visibility, x.ReadOnly, x.DefaultValue)).ToList());
			fieldCollection.AddRange(transactionAlias.TransportLineItemFieldCollection.ToList().Select(x => new AliasEditorFieldClass(x.IdentityGuid, x.DbName, x.DisplayName, x.FieldRequired, x.VirtualField, "Transport Line Item", 4, x.DisplayOrder, x.ClearOnNew, x.UserGroupGuid, x.UserGroupID, (int)x.Visibility, x.ReadOnly, x.DefaultValue)).ToList());
			fieldCollection.AddRange(transactionAlias.WeightReadingFieldCollection.ToList().Select(x => new AliasEditorFieldClass(x.IdentityGuid, x.DbName, x.DisplayName, x.FieldRequired, x.VirtualField, "Weight Reading", 5, x.DisplayOrder, x.ClearOnNew, x.UserGroupGuid, x.UserGroupID, (int)x.Visibility, x.ReadOnly, x.DefaultValue)).ToList());
			fieldCollection.AddRange(transactionAlias.ExportResultDetailFieldCollection.ToList().Select(x => new AliasEditorFieldClass(x.IdentityGuid, x.DbName, x.DisplayName, x.FieldRequired, x.VirtualField, "Export Result", 6, x.DisplayOrder, x.ClearOnNew, x.UserGroupGuid, x.UserGroupID, (int)x.Visibility, x.ReadOnly, x.DefaultValue)).ToList());
			this.FieldCollection = fieldCollection;

			var fieldTypes = new List<Tuple<string,string>>();
			foreach (TransactionFieldType transactionFieldType in Enum.GetValues(typeof(TransactionFieldType)))
			{
				if (transactionFieldType != TransactionFieldType.TransactionFieldTypeMax)
				{
					fieldTypes.Add(new Tuple<string, string>(transactionFieldType.ToString(), TransactionAliasFieldClass.TransactionFieldTypeID(transactionFieldType)));
				}
			}
			fieldTypes.Add(new Tuple<string, string>("TransactionUserData", "Transaction User Data"));
			fieldTypes.Add(new Tuple<string, string>("LineItemUserData", "Line Item User Data"));
			this.AllFieldTypes = fieldTypes;
			if (placementInfo == null) {
				this.PlacementInfo = "{\"fieldMap\":[],\"numberOfColumns\":3,\"numberOfRows\":12,\"retrieveMeterStartOnMeterIDSet\":false}";
			} else {
				this.PlacementInfo = placementInfo.PlacementInformation;
            }

			this.AllUserGroupTransactionAliasMapCollection = groupCollection.ToList().Select( x => new UserGroupMap(x.ID, x.IdentityGuid, 0)).OrderBy(x => x.ID).ToList();
			this.AllUserGroupTransactionAliasMapCollection = this.AllUserGroupTransactionAliasMapCollection.Prepend(new UserGroupMap( "{All}", Guid.Empty, 0)).ToList();
		}
	}

	[Serializable]
	public class TransactionTypeList : FMBaseModel
	{
        public TransactionTypeList(string ID, string Name)
        {
            this.ID = ID;
            this.Name = Name;
        }

        public string ID { get; set; }
		public string Name { get; set; }
	}

	[Serializable]
	public class ShowCompanyOptions : FMBaseModel
	{
		public ShowCompanyOptions(string ID, string Name)
		{
			this.ID = ID;
			this.Name = Name;
		}
		public string ID { get; set; }
		public string Name { get; set; }
	}

	[Serializable]
	public class ExcludedProductMap : FMBaseModel
	{
		public ExcludedProductMap(string ID, Guid guid)
		{
			this.ID = ID;
			this.Guid = guid;
		}
		public string ID { get; set; }
		public Guid Guid { get; set; }
	}

	[Serializable]
	public class UserGroupMap : FMBaseModel
	{
		public UserGroupMap()
		{
		}
		public UserGroupMap(string ID, Guid? guid, int right)
		{
			this.ID = ID;
			this.Guid = guid;
			this.Right = right;
		}
		public string ID { get; set; }
		public Guid? Guid { get; set; }
		public int Right { get; set; }
	}

	[Serializable]
	public class AliasEditorFieldClass : FMBaseModel
	{
		public Guid identityGuid { get; set; }
		public string dbName { get; set; }
		public string displayName { get; set; }
		public bool fieldRequired { get; set; }
		public bool virtualField { get; set; }
		public string typeName { get; set; }
		public int type { get; set; }
		public int displayOrder { get; set; }
		public bool clearOnNew { get; set; }
		public Guid userGroupGuid { get; set; }
		public string userGroupID { get; set; }
        public int visibility { get; set; }
        public bool readOnly { get; set; }
        public string defaultValue { get; set; }

		public AliasEditorFieldClass()
		{
		}

		public AliasEditorFieldClass(Guid identityGuid, string dbName, string displayName, bool fieldRequired, bool virtualField, string typeName, int type, int displayOrder, bool clearOnNew, Guid userGroupGuid, string userGroupID, int visibility, bool readOnly, object defaultValue)
        {
            this.identityGuid = identityGuid;
            this.dbName = dbName;
            this.displayName = displayName;
            this.fieldRequired = fieldRequired;
            this.virtualField = virtualField;
            this.typeName = typeName;
			this.type = type;
			this.displayOrder = displayOrder;
			this.clearOnNew = clearOnNew;
			this.userGroupGuid = userGroupGuid;
			this.userGroupID = userGroupID;
			this.visibility = visibility;
			this.readOnly = readOnly;
			this.defaultValue = (defaultValue != null) ? defaultValue.ToString() : String.Empty;
		}
	}

	[Serializable]
	public class AliasEditorKeyGuiValue : FMBaseModel
	{
		// class used for deserialization of data passed from the editor to the controller
		public string key { get; set; }
		public Guid value { get; set; }
		public AliasEditorKeyGuiValue(){
        }
	}

	[Serializable]
	public class AliasEditorTransactionAliasList : FMBaseModel
	{
		// class used for deserialization of data passed from the editor to the controller
		public string type { get; set; }
		public string key { get; set; }
		public Guid value { get; set; }
		public AliasEditorTransactionAliasList()
		{
		}
	}

}