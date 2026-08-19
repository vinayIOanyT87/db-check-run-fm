// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchConfigurationClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the DispatchConfigurationClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	#region Enums

	/// <summary>
	/// FillToActualOrStandardType Enumeration for Tabularview on Dispatch.
	/// </summary>
	public enum FillToActualOrStandardType
	{
		FillToActual,
		Standard
	}

	#endregion

	/// <summary>
	/// Definition of the DispatchConfigurationClass.
	/// </summary>
	[DataContract]
	[Serializable]
	public class DispatchConfigurationClass : BaseDataObject
	{
		/// <summary>
		/// The default Dispatch Configuration ID
		/// </summary>
		public const string DefaultId = "Dispatch Configuration";

		/// <summary>
		/// The default FuelsManager Report URL
		/// </summary>
		public const string DefaultFuelsManagerReportURL = @"../FMReportWebMain/ReportLandingPage.aspx";

		/// <summary>
		/// The default refresh period in seconds.
		/// </summary>
		public const int DefaultDataRefreshPeriod = 5;

		/// <summary>
		/// The default automatic restart delay in seconds.
		/// </summary>
		public const int DefaultAutomaticRestartDelay = 30;

		/// <summary>
		/// The default number of hours in the past of the operational window.
		/// </summary>
		public const int DefaultOperationalWindowPastHours = 8;

		/// <summary>
		/// The default number of hours in the future of the operational window.
		/// </summary>
		public const int DefaultOperationalWindowFutureHours = 16;

		/// <summary>
		/// Initializes a new instance of the <see cref="DispatchConfigurationClass"/> class.
		/// </summary>
		public DispatchConfigurationClass()
		{
			this.Reset();
		}

		#region Properties

		/// <summary>
		/// Gets the entity type.
		/// </summary>
		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.DISPATCH_CONFIGURATION; }
		}

		/// <summary>
		/// Gets the parent entity type.
		/// </summary>
		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to enable dispatch service request calls.
		/// </summary>
		[DataMember]
		public bool EnableServiceRequests
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the dispatch data refresh period.
		/// </summary>
		[DataMember]
		public int DispatchDataRefreshPeriod
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the automatic restart delay.
		/// </summary>
		[DataMember]
		public int AutomaticRestartDelay
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to display the current time on applicable dispatch pages.
		/// </summary>
		[DataMember]
		public bool DisplayCurrentTime
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the FuelsManager Report URL
		/// </summary>
		[DataMember]
		public string FuelsManagerReportURL
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to display military date on the tabular view page.
		/// </summary>
		[DataMember]
		public bool TabularViewDisplayMilitaryDate
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value for Fill to Actual or Fill to Standard in Dispatch.
		/// </summary>
		[DataMember]
		public FillToActualOrStandardType FillToActualOrStandard
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to use Arrival Time.
		/// </summary>
		[DataMember]
		public bool UseArrivalTime
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to use Start Time.
		/// </summary>
		[DataMember]
		public bool UseStartTime
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to use Stop Time.
		/// </summary>
		[DataMember]
		public bool UseStopTime
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the operational window past hours.
		/// </summary>
		[DataMember]
		public int OperationalWindowPastHours
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the operational window future hours.
		/// </summary>
		[DataMember]
		public int OperationalWindowFutureHours
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to show Grid Lines.
		/// </summary>
		[DataMember]
		public bool ShowGridLines
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to use Static Time Display.
		/// </summary>
		[DataMember]
		public bool StaticTimeDisplay
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to perform quantity not zero check.
		/// </summary>
		[DataMember]
		public bool QuantityNotZeroCheck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to perform exactly one manager check.
		/// </summary>
		[DataMember]
		public bool ExactlyOneManagerCheck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to perform exactly one owner check.
		/// </summary>
		[DataMember]
		public bool ExactlyOneOwnerCheck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to perform dispatch fuel additive flag check.
		/// </summary>
		[DataMember]
		public bool DispatchFuelAdditiveFlagCheck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to perform fast log fuel additive flag check.
		/// </summary>
		[DataMember]
		public bool FastLogFuelAdditiveFlagCheck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to perform fillstand volume within tolerance check.
		/// </summary>
		[DataMember]
		public bool FillstandVolumeWithinToleranceCheck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to perform return to bulk volume within tolerance check.
		/// </summary>
		[DataMember]
		public bool ReturnToBulkVolumeWithinToleranceCheck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to perform recirculation volumes greater than zero check.
		/// </summary>
		[DataMember]
		public bool RecirculationVolumesGreaterThanZeroCheck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to perform operator is in check.
		/// </summary>
		[DataMember]
		public bool OperatorIsInCheck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to perform operator not assigned check.
		/// </summary>
		[DataMember]
		public bool OperatorNotAssignedCheck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to perform operator has required training check.
		/// </summary>
		[DataMember]
		public bool OperatorHasRequiredTrainingCheck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to perform operator training not expired check.
		/// </summary>
		[DataMember]
		public bool OperatorTrainingNotExpiredCheck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to perform operator not locked out check.
		/// </summary>
		[DataMember]
		public bool OperatorNotLockedOutCheck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to perform operator has required qualifications check.
		/// </summary>
		[DataMember]
		public bool OperatorHasRequiredQualificationsCheck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to perform operator qualifications not expired check.
		/// </summary>
		[DataMember]
		public bool OperatorQualificationsNotExpiredCheck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to perform defuel status check.
		/// </summary>
		[DataMember]
		public bool DefuelStatusCheck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to perform refuel status check.
		/// </summary>
		[DataMember]
		public bool RefuelStatusCheck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to perform equipment fuel grade check.
		/// </summary>
		[DataMember]
		public bool EquipmentFuelGradeCheck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to perform equipment not locked out check.
		/// </summary>
		[DataMember]
		public bool EquipmentNotLockedOutCheck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to perform equipment not assigned check.
		/// </summary>
		[DataMember]
		public bool EquipmentNotAssignedCheck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to perform equipment in service check.
		/// </summary>
		[DataMember]
		public bool EquipmentInServiceCheck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to perform tag license not expired check.
		/// </summary>
		[DataMember]
		public bool TagLicenseNotExpiredCheck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to perform test inspection not expired check.
		/// </summary>
		[DataMember]
		public bool TestInspectionNotExpiredCheck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to perform quality control checkup date check.
		/// </summary>
		[DataMember]
		public bool QualityControlCheckupDateCheck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to perform caution quality tag check.
		/// </summary>
		[DataMember]
		public bool CautionQualityTagCheck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to perform warning quality tag check.
		/// </summary>
		[DataMember]
		public bool WarningQualityTagCheck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to perform danger quality tag check.
		/// </summary>
		[DataMember]
		public bool DangerQualityTagCheck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to require Equipment in Dispatch.
		/// </summary>
		[DataMember]
		public bool EquipmentRequired
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to require Personnel in Dispatch.
		/// </summary>
		[DataMember]
		public bool PersonnelRequired
		{
			get;
			set;
		}

		#endregion

		/// <summary>
		/// Resets the Dispatch Configuration object to its initial state.
		/// </summary>
		public override sealed void Reset()
		{
			base.Reset();
			this._ID = DefaultId;

			// General settings
			this.EnableServiceRequests = true;
			this.DispatchDataRefreshPeriod = DefaultDataRefreshPeriod;
			this.AutomaticRestartDelay = DefaultAutomaticRestartDelay;
			this.DisplayCurrentTime = false;
			this.FuelsManagerReportURL = DefaultFuelsManagerReportURL;

			// Tabular View settings
			this.TabularViewDisplayMilitaryDate = false;
			this.FillToActualOrStandard = FillToActualOrStandardType.Standard;
			this.UseArrivalTime = false;
			this.UseStartTime = false;
			this.UseStopTime = false;

			// Graphical View settings
			this.OperationalWindowPastHours = DefaultOperationalWindowPastHours;
			this.OperationalWindowFutureHours = DefaultOperationalWindowFutureHours;
			this.ShowGridLines = false;
			this.StaticTimeDisplay = false;

			// Validation settings
			this.QuantityNotZeroCheck = false;
			this.ExactlyOneManagerCheck = false;
			this.ExactlyOneOwnerCheck = false;
			this.DispatchFuelAdditiveFlagCheck = false;
			this.FastLogFuelAdditiveFlagCheck = false;
			this.FillstandVolumeWithinToleranceCheck = false;
			this.ReturnToBulkVolumeWithinToleranceCheck = false;
			this.RecirculationVolumesGreaterThanZeroCheck = false;
			this.OperatorIsInCheck = false;
			this.OperatorNotAssignedCheck = false;
			this.OperatorHasRequiredTrainingCheck = false;
			this.OperatorTrainingNotExpiredCheck = false;
			this.OperatorNotLockedOutCheck = false;
			this.OperatorHasRequiredQualificationsCheck = false;
			this.OperatorQualificationsNotExpiredCheck = false;
			this.DefuelStatusCheck = false;
			this.RefuelStatusCheck = false;
			this.EquipmentFuelGradeCheck = false;
			this.EquipmentNotLockedOutCheck = false;
			this.EquipmentNotAssignedCheck = false;
			this.EquipmentInServiceCheck = false;
			this.TagLicenseNotExpiredCheck = false;
			this.TestInspectionNotExpiredCheck = false;
			this.QualityControlCheckupDateCheck = false;
			this.CautionQualityTagCheck = false;
			this.WarningQualityTagCheck = false;
			this.PersonnelRequired = false;
			this.EquipmentRequired = false;
		}

		/// <summary>
		/// Loads the Dispatch Configuration data retrieved from the database.
		/// </summary>
		/// <param name="set">The DataSet retrieved from the database</param>
		public void Load(DataSet set)
		{
			if (set == null)
			{
				throw new ArgumentNullException("set");
			}

			this.Reset();

			DataTable table = set.Tables[0];

			if (table.Rows.Count == 0)
			{
				return;
			}

			DataRow row = table.Rows[0];

			this._IdentityGuid = DataObject.getValue<Guid>(row["DispatchConfigurationGuid"], Guid.Empty);
			this._SiteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
			this._ID = DataObject.getValue<string>(row["ID"], DefaultId);

			this.EnableServiceRequests = DataObject.getValue<bool>(row["EnableServiceRequests"], true);
			this.AutomaticRestartDelay = DataObject.getValue<int>(row["AutomaticRestartDelay"], DefaultAutomaticRestartDelay);
			this.DisplayCurrentTime = DataObject.getValue<bool>(row["DisplayCurrentTime"], false);
			this.FuelsManagerReportURL = DataObject.getValue<string>(row["FuelsManagerReportURL"], DefaultFuelsManagerReportURL);
			this.DispatchDataRefreshPeriod = DataObject.getValue<int>(row["DispatchDataRefreshPeriod"], DefaultDataRefreshPeriod);
			this.TabularViewDisplayMilitaryDate = DataObject.getValue<bool>(row["TabularViewDisplayMilitaryDate"], false);
			this.FillToActualOrStandard = (FillToActualOrStandardType)DataObject.getValue<int>(row["FillToActualOrStandard"], (int)FillToActualOrStandardType.Standard);
			this.UseArrivalTime = DataObject.getValue<bool>(row["UseArrivalTime"], false);
			this.UseStartTime = DataObject.getValue<bool>(row["UseStartTime"], false);
			this.UseStopTime = DataObject.getValue<bool>(row["UseStopTime"], false);
			this.OperationalWindowPastHours = DataObject.getValue<int>(row["OperationalWindowPastHours"], DefaultOperationalWindowPastHours);
			this.OperationalWindowFutureHours = DataObject.getValue<int>(row["OperationalWindowFutureHours"], DefaultOperationalWindowFutureHours);
			this.ShowGridLines = DataObject.getValue<bool>(row["ShowGridLines"], false);
			this.StaticTimeDisplay = DataObject.getValue<bool>(row["StaticTimeDisplay"], false);

			this.QuantityNotZeroCheck = DataObject.getValue<bool>(row["QuantityNotZeroCheck"], false);
			this.ExactlyOneManagerCheck = DataObject.getValue<bool>(row["ExactlyOneManagerCheck"], false);
			this.ExactlyOneOwnerCheck = DataObject.getValue<bool>(row["ExactlyOneOwnerCheck"], false);
			this.DispatchFuelAdditiveFlagCheck = DataObject.getValue<bool>(row["DispatchFuelAdditiveFlagCheck"], false);
			this.FastLogFuelAdditiveFlagCheck = DataObject.getValue<bool>(row["FastLogFuelAdditiveFlagCheck"], false);
			this.FillstandVolumeWithinToleranceCheck = DataObject.getValue<bool>(row["FillstandVolumeWithinToleranceCheck"], false);
			this.ReturnToBulkVolumeWithinToleranceCheck = DataObject.getValue<bool>(row["ReturnToBulkVolumeWithinToleranceCheck"], false);
			this.RecirculationVolumesGreaterThanZeroCheck = DataObject.getValue<bool>(row["RecirculationVolumesGreaterThanZeroCheck"], false);
			this.OperatorIsInCheck = DataObject.getValue<bool>(row["OperatorIsInCheck"], false);
			this.OperatorNotAssignedCheck = DataObject.getValue<bool>(row["OperatorNotAssignedCheck"], false);
			this.OperatorHasRequiredTrainingCheck = DataObject.getValue<bool>(row["OperatorHasRequiredTrainingCheck"], false);
			this.OperatorTrainingNotExpiredCheck = DataObject.getValue<bool>(row["OperatorTrainingNotExpiredCheck"], false);
			this.OperatorNotLockedOutCheck = DataObject.getValue<bool>(row["OperatorNotLockedOutCheck"], false);
			this.OperatorHasRequiredQualificationsCheck = DataObject.getValue<bool>(row["OperatorHasRequiredQualificationsCheck"], false);
			this.OperatorQualificationsNotExpiredCheck = DataObject.getValue<bool>(row["OperatorQualificationsNotExpiredCheck"], false);
			this.DefuelStatusCheck = DataObject.getValue<bool>(row["DefuelStatusCheck"], false);
			this.RefuelStatusCheck = DataObject.getValue<bool>(row["RefuelStatusCheck"], false);
			this.EquipmentFuelGradeCheck = DataObject.getValue<bool>(row["EquipmentFuelGradeCheck"], false);
			this.EquipmentNotLockedOutCheck = DataObject.getValue<bool>(row["EquipmentNotLockedOutCheck"], false);
			this.EquipmentNotAssignedCheck = DataObject.getValue<bool>(row["EquipmentNotAssignedCheck"], false);
			this.EquipmentInServiceCheck = DataObject.getValue<bool>(row["EquipmentInServiceCheck"], false);
			this.TagLicenseNotExpiredCheck = DataObject.getValue<bool>(row["TagLicenseNotExpiredCheck"], false);
			this.TestInspectionNotExpiredCheck = DataObject.getValue<bool>(row["TestInspectionNotExpiredCheck"], false);
			this.QualityControlCheckupDateCheck = DataObject.getValue<bool>(row["QualityControlCheckupDateCheck"], false);
			this.CautionQualityTagCheck = DataObject.getValue<bool>(row["CautionQualityTagCheck"], false);
			this.WarningQualityTagCheck = DataObject.getValue<bool>(row["WarningQualityTagCheck"], false);
			this.DangerQualityTagCheck = DataObject.getValue<bool>(row["DangerQualityTagCheck"], false);
			this.PersonnelRequired = DataObject.getValue<bool>(row["PersonnelRequired"], false);
			this.EquipmentRequired = DataObject.getValue<bool>(row["EquipmentRequired"], false);

			this._CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
			this._CreatedBy = DataObject.getValue<string>(row["CreatedBy"], BaseDataObject.ADMIN);
			this._UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this._CreatedDate);
			this._UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], BaseDataObject.ADMIN);
		}

		#region paramaterized SQL

		/// <summary>
		/// Generates the dynamic SQL to insert a DispatchConfigurationClass object into the database
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		public void InsertSql(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblDispatchConfiguration " +
				"(SiteGuid," +
				"ID," +

				"EnableServiceRequests," +
				"AutomaticRestartDelay," +
				"DisplayCurrentTime," +
				"FuelsManagerReportURL," +
				"DispatchDataRefreshPeriod," +
				"TabularViewDisplayMilitaryDate," +
				"FillToActualOrStandard," +
				"UseArrivalTime," +
				"UseStartTime," +
				"UseStopTime," +
				"OperationalWindowPastHours," +
				"OperationalWindowFutureHours," +
				"ShowGridLines," +
				"StaticTimeDisplay," +

				"QuantityNotZeroCheck," +
				"ExactlyOneManagerCheck," +
				"ExactlyOneOwnerCheck," +
				"DispatchFuelAdditiveFlagCheck," +
				"FastLogFuelAdditiveFlagCheck," +
				"FillstandVolumeWithinToleranceCheck," +
				"ReturnToBulkVolumeWithinToleranceCheck," +
				"RecirculationVolumesGreaterThanZeroCheck," +
				"OperatorIsInCheck," +
				"OperatorNotAssignedCheck," +
				"OperatorHasRequiredTrainingCheck," +
				"OperatorTrainingNotExpiredCheck," +
				"OperatorNotLockedOutCheck," +
				"OperatorHasRequiredQualificationsCheck," +
				"OperatorQualificationsNotExpiredCheck," +
				"DefuelStatusCheck," +
				"RefuelStatusCheck," +
				"EquipmentFuelGradeCheck," +
				"EquipmentNotLockedOutCheck," +
				"EquipmentNotAssignedCheck," +
				"EquipmentInServiceCheck," +
				"TagLicenseNotExpiredCheck," +
				"TestInspectionNotExpiredCheck," +
				"QualityControlCheckupDateCheck," +
				"CautionQualityTagCheck," +
				"WarningQualityTagCheck," +
				"DangerQualityTagCheck," +
				"PersonnelRequired," +
				"EquipmentRequired," +

				"CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy," +
				"DispatchConfigurationGuid" +
				") VALUES (" +
				"@SiteGuid," +
				"@ID," +

				"@EnableServiceRequests," +
				"@AutomaticRestartDelay," +
				"@DisplayCurrentTime," +
				"@FuelsManagerReportURL," +
				"@DispatchDataRefreshPeriod," +
				"@TabularViewDisplayMilitaryDate," +
				"@FillToActualOrStandard," +
				"@UseArrivalTime," +
				"@UseStartTime," +
				"@UseStopTime," +
				"@OperationalWindowPastHours," +
				"@OperationalWindowFutureHours," +
				"@ShowGridLines," +
				"@StaticTimeDisplay," +

				"@QuantityNotZeroCheck," +
				"@ExactlyOneManagerCheck," +
				"@ExactlyOneOwnerCheck," +
				"@DispatchFuelAdditiveFlagCheck," +
				"@FastLogFuelAdditiveFlagCheck," +
				"@FillstandVolumeWithinToleranceCheck," +
				"@ReturnToBulkVolumeWithinToleranceCheck," +
				"@RecirculationVolumesGreaterThanZeroCheck," +
				"@OperatorIsInCheck," +
				"@OperatorNotAssignedCheck," +
				"@OperatorHasRequiredTrainingCheck," +
				"@OperatorTrainingNotExpiredCheck," +
				"@OperatorNotLockedOutCheck," +
				"@OperatorHasRequiredQualificationsCheck," +
				"@OperatorQualificationsNotExpiredCheck," +
				"@DefuelStatusCheck," +
				"@RefuelStatusCheck," +
				"@EquipmentFuelGradeCheck," +
				"@EquipmentNotLockedOutCheck," +
				"@EquipmentNotAssignedCheck," +
				"@EquipmentInServiceCheck," +
				"@TagLicenseNotExpiredCheck," +
				"@TestInspectionNotExpiredCheck," +
				"@QualityControlCheckupDateCheck," +
				"@CautionQualityTagCheck," +
				"@WarningQualityTagCheck," +
				"@DangerQualityTagCheck," +
				"@PersonnelRequired," +
				"@EquipmentRequired," +

				"@CreatedDate," +
				"@CreatedBy," +
				"@UpdatedDate," +
				"@UpdatedBy," +
				"@DispatchConfigurationGuid)";

			cmd.Parameters.AddWithValue("@SiteGuid", this._SiteGuid);
			cmd.Parameters.AddWithValue("@ID", this._ID);

			cmd.Parameters.AddWithValue("@EnableServiceRequests", this.EnableServiceRequests);
			cmd.Parameters.AddWithValue("@AutomaticRestartDelay", this.AutomaticRestartDelay);
			cmd.Parameters.AddWithValue("@DisplayCurrentTime", this.DisplayCurrentTime);
			cmd.Parameters.AddWithValue("@FuelsManagerReportURL", this.FuelsManagerReportURL);
			cmd.Parameters.AddWithValue("@DispatchDataRefreshPeriod", this.DispatchDataRefreshPeriod);
			cmd.Parameters.AddWithValue("@TabularViewDisplayMilitaryDate", this.TabularViewDisplayMilitaryDate);
			cmd.Parameters.AddWithValue("@FillToActualOrStandard", this.FillToActualOrStandard);
			cmd.Parameters.AddWithValue("@UseArrivalTime", this.UseArrivalTime);
			cmd.Parameters.AddWithValue("@UseStartTime", this.UseStartTime);
			cmd.Parameters.AddWithValue("@UseStopTime", this.UseStopTime);
			cmd.Parameters.AddWithValue("@OperationalWindowPastHours", this.OperationalWindowPastHours);
			cmd.Parameters.AddWithValue("@OperationalWindowFutureHours", this.OperationalWindowFutureHours);
			cmd.Parameters.AddWithValue("@ShowGridLines", this.ShowGridLines);
			cmd.Parameters.AddWithValue("@StaticTimeDisplay", this.StaticTimeDisplay);

			cmd.Parameters.AddWithValue("@QuantityNotZeroCheck", this.QuantityNotZeroCheck);
			cmd.Parameters.AddWithValue("@ExactlyOneManagerCheck", this.ExactlyOneManagerCheck);
			cmd.Parameters.AddWithValue("@ExactlyOneOwnerCheck", this.ExactlyOneOwnerCheck);
			cmd.Parameters.AddWithValue("@DispatchFuelAdditiveFlagCheck", this.DispatchFuelAdditiveFlagCheck);
			cmd.Parameters.AddWithValue("@FastLogFuelAdditiveFlagCheck", this.FastLogFuelAdditiveFlagCheck);
			cmd.Parameters.AddWithValue("@FillstandVolumeWithinToleranceCheck", this.FillstandVolumeWithinToleranceCheck);
			cmd.Parameters.AddWithValue("@ReturnToBulkVolumeWithinToleranceCheck", this.ReturnToBulkVolumeWithinToleranceCheck);
			cmd.Parameters.AddWithValue("@RecirculationVolumesGreaterThanZeroCheck", this.RecirculationVolumesGreaterThanZeroCheck);
			cmd.Parameters.AddWithValue("@OperatorIsInCheck", this.OperatorIsInCheck);
			cmd.Parameters.AddWithValue("@OperatorNotAssignedCheck", this.OperatorNotAssignedCheck);
			cmd.Parameters.AddWithValue("@OperatorHasRequiredTrainingCheck", this.OperatorHasRequiredTrainingCheck);
			cmd.Parameters.AddWithValue("@OperatorTrainingNotExpiredCheck", this.OperatorTrainingNotExpiredCheck);
			cmd.Parameters.AddWithValue("@OperatorNotLockedOutCheck", this.OperatorNotLockedOutCheck);
			cmd.Parameters.AddWithValue("@OperatorHasRequiredQualificationsCheck", this.OperatorHasRequiredQualificationsCheck);
			cmd.Parameters.AddWithValue("@OperatorQualificationsNotExpiredCheck", this.OperatorQualificationsNotExpiredCheck);
			cmd.Parameters.AddWithValue("@DefuelStatusCheck", this.DefuelStatusCheck);
			cmd.Parameters.AddWithValue("@RefuelStatusCheck", this.RefuelStatusCheck);
			cmd.Parameters.AddWithValue("@EquipmentFuelGradeCheck", this.EquipmentFuelGradeCheck);
			cmd.Parameters.AddWithValue("@EquipmentNotLockedOutCheck", this.EquipmentNotLockedOutCheck);
			cmd.Parameters.AddWithValue("@EquipmentNotAssignedCheck", this.EquipmentNotAssignedCheck);
			cmd.Parameters.AddWithValue("@EquipmentInServiceCheck", this.EquipmentInServiceCheck);
			cmd.Parameters.AddWithValue("@TagLicenseNotExpiredCheck", this.TagLicenseNotExpiredCheck);
			cmd.Parameters.AddWithValue("@TestInspectionNotExpiredCheck", this.TestInspectionNotExpiredCheck);
			cmd.Parameters.AddWithValue("@QualityControlCheckupDateCheck", this.QualityControlCheckupDateCheck);
			cmd.Parameters.AddWithValue("@CautionQualityTagCheck", this.CautionQualityTagCheck);
			cmd.Parameters.AddWithValue("@WarningQualityTagCheck", this.WarningQualityTagCheck);
			cmd.Parameters.AddWithValue("@DangerQualityTagCheck", this.DangerQualityTagCheck);
			cmd.Parameters.AddWithValue("@PersonnelRequired", this.PersonnelRequired);
			cmd.Parameters.AddWithValue("@EquipmentRequired", this.EquipmentRequired);

			cmd.Parameters.AddWithValue("@CreatedDate", this._CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", this._CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", this._UpdatedBy);
			cmd.Parameters.AddWithValue("@DispatchConfigurationGuid", this._IdentityGuid);

		}

		/// <summary>
		/// Generates the dynamic SQL to update a DispatchConfigurationClass object in the database
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		public void UpdateSql(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblDispatchConfiguration SET " +
					"SiteGuid = @SiteGuid," +
					"ID = @ID," +

					"EnableServiceRequests = @EnableServiceRequests," +
					"AutomaticRestartDelay = @AutomaticRestartDelay," +
					"DisplayCurrentTime = @DisplayCurrentTime," +
					"FuelsManagerReportURL = @FuelsManagerReportURL," +
					"DispatchDataRefreshPeriod = @DispatchDataRefreshPeriod," +
					"TabularViewDisplayMilitaryDate = @TabularViewDisplayMilitaryDate," +
					"FillToActualOrStandard = @FillToActualOrStandard," +
					"UseArrivalTime = @UseArrivalTime," +
					"UseStartTime = @UseStartTime," +
					"UseStopTime = @UseStopTime," +
					"OperationalWindowPastHours = @OperationalWindowPastHours," +
					"OperationalWindowFutureHours = @OperationalWindowFutureHours," +
					"ShowGridLines = @ShowGridLines," +
					"StaticTimeDisplay = @StaticTimeDisplay," +

					"QuantityNotZeroCheck = @QuantityNotZeroCheck," +
					"ExactlyOneManagerCheck = @ExactlyOneManagerCheck," +
					"ExactlyOneOwnerCheck = @ExactlyOneOwnerCheck," +
					"DispatchFuelAdditiveFlagCheck = @DispatchFuelAdditiveFlagCheck," +
					"FastLogFuelAdditiveFlagCheck = @FastLogFuelAdditiveFlagCheck," +
					"FillstandVolumeWithinToleranceCheck = @FillstandVolumeWithinToleranceCheck," +
					"ReturnToBulkVolumeWithinToleranceCheck = @ReturnToBulkVolumeWithinToleranceCheck," +
					"RecirculationVolumesGreaterThanZeroCheck = @RecirculationVolumesGreaterThanZeroCheck," +
					"OperatorIsInCheck = @OperatorIsInCheck," +
					"OperatorNotAssignedCheck = @OperatorNotAssignedCheck," +
					"OperatorHasRequiredTrainingCheck = @OperatorHasRequiredTrainingCheck," +
					"OperatorTrainingNotExpiredCheck = @OperatorTrainingNotExpiredCheck," +
					"OperatorNotLockedOutCheck = @OperatorNotLockedOutCheck," +
					"OperatorHasRequiredQualificationsCheck = @OperatorHasRequiredQualificationsCheck," +
					"OperatorQualificationsNotExpiredCheck = @OperatorQualificationsNotExpiredCheck," +
					"DefuelStatusCheck = @DefuelStatusCheck," +
					"RefuelStatusCheck = @RefuelStatusCheck," +
					"EquipmentFuelGradeCheck = @EquipmentFuelGradeCheck," +
					"EquipmentNotLockedOutCheck = @EquipmentNotLockedOutCheck," +
					"EquipmentNotAssignedCheck = @EquipmentNotAssignedCheck," +
					"EquipmentInServiceCheck = @EquipmentInServiceCheck," +
					"TagLicenseNotExpiredCheck = @TagLicenseNotExpiredCheck," +
					"TestInspectionNotExpiredCheck = @TestInspectionNotExpiredCheck," +
					"QualityControlCheckupDateCheck = @QualityControlCheckupDateCheck," +
					"CautionQualityTagCheck = @CautionQualityTagCheck," +
					"WarningQualityTagCheck = @WarningQualityTagCheck," +
					"DangerQualityTagCheck = @DangerQualityTagCheck," +
					"PersonnelRequired = @PersonnelRequired," +
					"EquipmentRequired = @EquipmentRequired," +

					"UpdatedDate = @UpdatedDate," +
					"UpdatedBy = @UpdatedBy" +
					" WHERE DispatchConfigurationGuid = @DispatchConfigurationGuid";

			cmd.Parameters.AddWithValue("@SiteGuid", this._SiteGuid);
			cmd.Parameters.AddWithValue("@ID", this._ID);

			cmd.Parameters.AddWithValue("@EnableServiceRequests", this.EnableServiceRequests);
			cmd.Parameters.AddWithValue("@AutomaticRestartDelay", this.AutomaticRestartDelay);
			cmd.Parameters.AddWithValue("@DisplayCurrentTime", this.DisplayCurrentTime);
			cmd.Parameters.AddWithValue("@FuelsManagerReportURL", this.FuelsManagerReportURL);
			cmd.Parameters.AddWithValue("@DispatchDataRefreshPeriod", this.DispatchDataRefreshPeriod);
			cmd.Parameters.AddWithValue("@TabularViewDisplayMilitaryDate", this.TabularViewDisplayMilitaryDate);
			cmd.Parameters.AddWithValue("@FillToActualOrStandard", this.FillToActualOrStandard);
			cmd.Parameters.AddWithValue("@UseArrivalTime", this.UseArrivalTime);
			cmd.Parameters.AddWithValue("@UseStartTime", this.UseStartTime);
			cmd.Parameters.AddWithValue("@UseStopTime", this.UseStopTime);
			cmd.Parameters.AddWithValue("@OperationalWindowPastHours", this.OperationalWindowPastHours);
			cmd.Parameters.AddWithValue("@OperationalWindowFutureHours", this.OperationalWindowFutureHours);
			cmd.Parameters.AddWithValue("@ShowGridLines", this.ShowGridLines);
			cmd.Parameters.AddWithValue("@StaticTimeDisplay", this.StaticTimeDisplay);

			cmd.Parameters.AddWithValue("@QuantityNotZeroCheck", this.QuantityNotZeroCheck);
			cmd.Parameters.AddWithValue("@ExactlyOneManagerCheck", this.ExactlyOneManagerCheck);
			cmd.Parameters.AddWithValue("@ExactlyOneOwnerCheck", this.ExactlyOneOwnerCheck);
			cmd.Parameters.AddWithValue("@DispatchFuelAdditiveFlagCheck", this.DispatchFuelAdditiveFlagCheck);
			cmd.Parameters.AddWithValue("@FastLogFuelAdditiveFlagCheck", this.FastLogFuelAdditiveFlagCheck);
			cmd.Parameters.AddWithValue("@FillstandVolumeWithinToleranceCheck", this.FillstandVolumeWithinToleranceCheck);
			cmd.Parameters.AddWithValue("@ReturnToBulkVolumeWithinToleranceCheck", this.ReturnToBulkVolumeWithinToleranceCheck);
			cmd.Parameters.AddWithValue("@RecirculationVolumesGreaterThanZeroCheck", this.RecirculationVolumesGreaterThanZeroCheck);
			cmd.Parameters.AddWithValue("@OperatorIsInCheck", this.OperatorIsInCheck);
			cmd.Parameters.AddWithValue("@OperatorNotAssignedCheck", this.OperatorNotAssignedCheck);
			cmd.Parameters.AddWithValue("@OperatorHasRequiredTrainingCheck", this.OperatorHasRequiredTrainingCheck);
			cmd.Parameters.AddWithValue("@OperatorTrainingNotExpiredCheck", this.OperatorTrainingNotExpiredCheck);
			cmd.Parameters.AddWithValue("@OperatorNotLockedOutCheck", this.OperatorNotLockedOutCheck);
			cmd.Parameters.AddWithValue("@OperatorHasRequiredQualificationsCheck", this.OperatorHasRequiredQualificationsCheck);
			cmd.Parameters.AddWithValue("@OperatorQualificationsNotExpiredCheck", this.OperatorQualificationsNotExpiredCheck);
			cmd.Parameters.AddWithValue("@DefuelStatusCheck", this.DefuelStatusCheck);
			cmd.Parameters.AddWithValue("@RefuelStatusCheck", this.RefuelStatusCheck);
			cmd.Parameters.AddWithValue("@EquipmentFuelGradeCheck", this.EquipmentFuelGradeCheck);
			cmd.Parameters.AddWithValue("@EquipmentNotLockedOutCheck", this.EquipmentNotLockedOutCheck);
			cmd.Parameters.AddWithValue("@EquipmentNotAssignedCheck", this.EquipmentNotAssignedCheck);
			cmd.Parameters.AddWithValue("@EquipmentInServiceCheck", this.EquipmentInServiceCheck);
			cmd.Parameters.AddWithValue("@TagLicenseNotExpiredCheck", this.TagLicenseNotExpiredCheck);
			cmd.Parameters.AddWithValue("@TestInspectionNotExpiredCheck", this.TestInspectionNotExpiredCheck);
			cmd.Parameters.AddWithValue("@QualityControlCheckupDateCheck", this.QualityControlCheckupDateCheck);
			cmd.Parameters.AddWithValue("@CautionQualityTagCheck", this.CautionQualityTagCheck);
			cmd.Parameters.AddWithValue("@WarningQualityTagCheck", this.WarningQualityTagCheck);
			cmd.Parameters.AddWithValue("@DangerQualityTagCheck", this.DangerQualityTagCheck);
			cmd.Parameters.AddWithValue("@PersonnelRequired", this.PersonnelRequired);
			cmd.Parameters.AddWithValue("@EquipmentRequired", this.EquipmentRequired);

			cmd.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", this._UpdatedBy);
			cmd.Parameters.AddWithValue("@DispatchConfigurationGuid", this._IdentityGuid);
		}

		/// <summary>
		/// Generates the dynamic SQL to delete a DispatchConfigurationClass object from the database
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		public void PurgeSql(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblDispatchConfiguration" +
				" WHERE DispatchConfigurationGuid = @DispatchConfigurationGuid";

			cmd.Parameters.AddWithValue("@DispatchConfigurationGuid", this._IdentityGuid);
		}

		/// <summary>
		/// Generates the dynamic SQL to delete linked EntityToSiteMapClass objects from the database
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		public void PurgeFromEntityToSiteMapSql(SqlCommand cmd)
		{
			cmd.CommandText = "EXEC map.gsp_EntityDispatchConfigurationToSiteDeleteByDispatchConfigurationGuid @DispatchConfigurationGuid";

			cmd.Parameters.AddWithValue("@DispatchConfigurationGuid", this._IdentityGuid);
		}

		/// <summary>
		/// The dynamic SQL SELECT prefix
		/// </summary>
		private const string SelectClause = "SELECT tblDispatchConfiguration.*";

		/// <summary>
		/// Generates the dynamic SQL to select a DispatchConfigurationClass object from the database
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		/// <param name="inTransaction">Flag indicating if in a transaction</param>
		public void SelectSql(SqlCommand cmd, bool inTransaction)
		{
			cmd.CommandText = SelectClause +
				" FROM tblDispatchConfiguration " + BaseDataObject.SQLUpdateLock(inTransaction) +
				" WHERE DispatchConfigurationGuid = @DispatchConfigurationGuid";

			cmd.Parameters.AddWithValue("@DispatchConfigurationGuid", this._IdentityGuid);
		}

		/// <summary>
		/// Generates the dynamic SQL to select a DispatchConfigurationClass object from the database by ID
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		/// <param name="inTransaction">Flag indicating if in a transaction</param>
		public void SelectByIdSql(SqlCommand cmd, bool inTransaction)
		{
			cmd.CommandText = SelectClause +
				" FROM tblDispatchConfiguration, " + EntityToSiteMapClass.GetMappingTableName(this.EntityType) + " " + BaseDataObject.SQLUpdateLock(inTransaction) +
				" WHERE " + EntityToSiteMapClass.GetMappingTableName(this.EntityType) + ".SiteGuid = @SiteGuid" +
				" AND " + EntityToSiteMapClass.GetMappingTableName(this.EntityType) + ".DispatchConfigurationGuid = tblDispatchConfiguration.DispatchConfigurationGuid" +
				" AND tblDispatchConfiguration.ID = @ID";

			cmd.Parameters.AddWithValue("@SiteGuid", this._SiteGuid);
			cmd.Parameters.AddWithValue("@ID", this._ID);
		}

		/// <summary>
		/// Generates the dynamic SQL to select a list of DispatchConfigurationClass objects from the database
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="cmd">The SqlCommand object</param>
		public void EnumerateSql(SecurityClass security, SqlCommand cmd)
		{
			cmd.CommandText = SelectClause +
				" FROM tblDispatchConfiguration" +
				" WHERE" + this.AppendSiteWhereClauseParameters(cmd, security, "tblDispatchConfiguration", "DispatchConfigurationGuid") +
				" ORDER BY ID";
		}

		#endregion

	}

	/// <summary>
	/// Defines a list of DispatchConfigurationClass objects.
	/// </summary>
	[Serializable]
	[CollectionDataContract]
	[KnownType(typeof(DispatchConfigurationClass))]
	public class DispatchConfigurationCollectionClass : List<DispatchConfigurationClass>
	{
	}
}
