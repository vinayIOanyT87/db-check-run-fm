// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchingView.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the DispatchingView type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.DispatchWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

    using FMCore;

	/// <summary>
	///    Code behind for Dispatching View page.
	/// </summary>
	public partial class DispatchingView : FMDispatchFormBase
	{
		#region Enums

		/// <summary>
		///    For use in prompting for alias names and re-entering business logic flow on return from client.
		/// </summary>
		private enum AliasPromptMode
		{
			FillStand,

			ReturnToBulk
		}

		/// <summary>
		///    Warning state designator for use in returning after warning dialog
		/// </summary>
		private enum WarningState
		{
			NoWarnings,

			UnitOutOfService,

			PersonnelOut,

			EquipmentQualitCheckOverdue,

			PersonnelAlreadyAssignedToDifferentVehicle,

			CheckingQualificationExists,

			CheckingQualifications,

			CheckingTrainingExists,

			CheckingTraining,

			CheckingTagsAndLicenses,

			CheckingTestsAndInspections,

			CheckingQualityTags,

			CheckAdditiveFlag,

			CheckDefuelStatus,

			CheckRefuelStatus,

			CheckGrade
		}

		#endregion

		#region Public Properties

		/// <summary>
		///    Gets or sets the StandByStatusValues value
		/// </summary>
		/// <value>
		///    The EquipmentID and PersonnelId of what's selected on the StandByStatus Board.
		/// </value>
		public string StandByStatusValues { get; set; }

		#endregion

		#region Properties

		/// <summary>
		///    Gets or sets the reference trans id.
		/// </summary>
		/// <value>
		///    The reference trans id.
		/// </value>
		protected string ReferenceTransId { get; set; }

		/// <summary>
		///    Gets or sets the configuration settings.
		/// </summary>
		/// <value>
		///    The configuration settings.
		/// </value>
		private DispatchConfigurationClass ConfigurationSettings { get; set; }

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///    Radio button click event handler.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="eventArgs">
		///    The <see cref="System.EventArgs" /> instance containing the event data.
		/// </param>
		public void RadioButtonOnServerClick(object sender, EventArgs eventArgs)
		{
			try
			{
				// Get the transaction id
				string transId = this.RequestGridSelection.Value;

				// Get the radio value and register a start function to show the dialog
				if (string.IsNullOrEmpty(transId))
				{
					return;
				}

				// Look up the transaction
				TransactionDO transaction = this.GetTransaction(transId);
				if (transaction == null)
				{
					return;
				}

				string radioText = transaction.RadioNumber;

				// Get the postback script name
				string postBackString = this.Page.ClientScript.GetPostBackEventReference(this, "RadioDialogOkButtonClick");

				// Create script block - use delay to have dialog display after entire page rendered (otherwise the page is blank).
				string scriptBlock = @"
						function endDialogGetResponse()
						{
							$('#RadioTextValue').val($('#RadioTextBox').val());
							$(this).dialog('close');
							" + postBackString + @";
						}

						function RadioFunction() {
							$('#RadioTextBox').val('" + radioText + @"');

							$('#RadioDialog').dialog(
							{
								autoOpen: false,
								modal: true,
								width: 275,
								height: 200,
								buttons: {
									'OK': endDialogGetResponse,
									'Cancel' : function() {$(this).dialog('close'); }
								}
							});

							$('#RadioDialog').dialog('open');
						}

						window.setTimeout('RadioFunction()', 100);
					";

				this.ClientScript.RegisterClientScriptBlock(this.GetType(), "ButtonClickEvent", scriptBlock, true);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		#endregion

		#region Methods

		/// <summary>
		///    Home button click event handler.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="eventArgs">
		///    The <see cref="System.EventArgs" /> instance containing the event data.
		/// </param>
		protected void HomeButtonOnServerClick(object sender, EventArgs eventArgs)
		{
			try
			{
				if (string.IsNullOrEmpty(this.OperatorGridSelection.Value) == false)
				{
					FMChannelHelper.MakeCall<IPersonnel>(this.SetPersonToHomeStatus);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Raises the <see cref="OnInit" /> event.
		/// </summary>
		/// <param name="e">
		///    The <see cref="System.EventArgs" /> instance containing the event data.
		/// </param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.InitializeComponents();
		}

		/// <summary>
		///    Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">
		///    The <see cref="System.EventArgs" /> instance containing the event data.
		/// </param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				this.GenerateEquipmentGridEditPostBack();
				this.GeneratePersonnelGridEditPostBack();

				// Save the transId of the transaction passed from tabular view that is selected.
				if (string.IsNullOrEmpty(this.Request.GetQueryOrFormValue("transId")) == false)
				{
					this.ReferenceTransId = this.Request.GetQueryOrFormValue("transId");
				}

				if (this.IsPostBack == false)
				{
					this.InitializeCommonProperties(this.hiddenFields, PageType.DispatchingView);
					this.CloseButton.Attributes.Add("onclick", "return DispatchingViewLib.CloseButtonOnClick();");

					if (!string.IsNullOrEmpty(this.Request.GetQueryOrFormValue("StandByStatusValues")))
					{
						this.StandByStatusValues = this.Request.GetQueryOrFormValue("StandByStatusValues");
					}
				}
				else
				{
					this.RestoreCommonProperties(this.hiddenFields);

					if (this.ParseCustomEventArguments())
					{
						return;
					}

					this.SetGridSelectionCall();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Alias OK dialog button click event
		/// </summary>
		private void AliasOkButtonClick()
		{
			try
			{
				this.ConfigurationSettings = this.GetConfigurationSettings();

				var mode = (AliasPromptMode)Enum.Parse(typeof(AliasPromptMode), this.WarningLoopValue.Value);

				switch (mode)
				{
					case AliasPromptMode.FillStand:
						this.CreateFillStand();
						break;

					case AliasPromptMode.ReturnToBulk:
						this.CreateReturnToBulk();
						break;

					default:
						throw new ApplicationException("Unknown alias selection mode");
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Checks the additive flag.  Once a user clears a transaction for this warning it is assumed to be cleared
		///    for any additional transactions.
		/// </summary>
		/// <param name="equipment">The equipment.</param>
		/// <param name="transactionArray">The transaction array.</param>
		/// <returns>True if all is well</returns>
		private bool CheckAdditiveFlag(EquipmentClass equipment, IEnumerable<TransactionDO> transactionArray)
		{
			foreach (TransactionDO transaction in transactionArray)
			{
				if (equipment.FuelAdditiveFlag != transaction.FuelAdditiveFlag)
				{
					this.ShowWarningDialog(
						"Attention, either the Servicing Unit or the Fuel Request is missing the Fuel Additive Flag.  Dispatch anyway?",
						WarningState.CheckAdditiveFlag);

					return false;
				}
			}

			return true;
		}

		/// <summary>
		///    Checks the defuel status.
		/// </summary>
		/// <param name="equipment">The equipment.</param>
		/// <param name="transactionArray">The transaction array.</param>
		/// <returns>True if defuel status checks out.</returns>
		private bool CheckDefuelStatus(EquipmentClass equipment, IEnumerable<TransactionDO> transactionArray)
		{
			foreach (TransactionDO transaction in transactionArray)
			{
				if (transaction.TransTypeID == TransactionTypes.T4_SecondaryDefuel
					&& equipment.FuelingType != FUELING_TYPES.DEFUELER)
				{
					const string Message =
						"The fueling status of the unit dispatched does not match the type of request.  Dispatch anyway?";

					this.ShowWarningDialog(Message, WarningState.CheckDefuelStatus);
					return false;
				}
			}

			return true;
		}

		/// <summary>
		///    Checks the equipment quality checkup date.
		/// </summary>
		/// <param name="equipment">The equipment.</param>
		/// <param name="currentSiteTime">The current site time.</param>
		/// <returns>True if the equipment quality date is ok.</returns>
		private bool CheckEquipmentQualityCheckupDate(EquipmentClass equipment, DateTimeOffset currentSiteTime)
		{
			if (equipment._QCDate.Value != DateTimeOffset.MinValue && equipment._QCDate.Value.Date < currentSiteTime.Date)
			{
				this.ShowWarningDialog(
					"Equipment is overdue QC Checkup.  Dispatch Anyway?", WarningState.EquipmentQualitCheckOverdue);

				return false;
			}

			return true;
		}

		/// <summary>
		///    Checks the type of the equipment.
		/// </summary>
		/// <param name="transactionAlias">The transaction alias.</param>
		/// <param name="equipment">The equipment.</param>
		/// <returns>True if the equipment is valid.</returns>
		private bool CheckEquipmentType(TransactionAliasClass transactionAlias, EquipmentClass equipment)
		{
			EQUIPMENT_TYPE[] types = transactionAlias.GetEquipmentTypes(true, 1);

			foreach (EQUIPMENT_TYPE type in types)
			{
				if (type == equipment.Type)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		///    Checks for initial dispatching errors.
		/// </summary>
		/// <param name="equipment">The equipment.</param>
		/// <param name="person">The person.</param>
		private void CheckForInitialDispatchingErrors(EquipmentClass equipment, PersonClass person)
		{
			if (this.ConfigurationSettings.EquipmentRequired)
			{
				if (equipment.LockedOut)
				{
					throw new ApplicationException("Servicing unit is locked-out.  Cannot dispatch this vehicle.");
				}

				if (string.IsNullOrEmpty(equipment.IssPtNum))
				{
					throw new ApplicationException("IssPtNum for Vehicle is blank.  Cannot dispatch this vehicle.");
				}
			}

			if (this.ConfigurationSettings.PersonnelRequired)
			{
				if (person.LockedOut)
				{
					string message = string.Format(
						"[{0},{1}] is locked-out.  Cannot dispatch this agent.", person.LastName, person.FirstName);

					throw new ApplicationException(message);
				}

				// Don't dispatch equipment if it is already assigned to another operator
				var personCollection = FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(
										x => x.EnumerateByRole(this.Security, PERSON_ROLE.LOADER_ROLE));

				foreach (PersonClass personClass in personCollection)
				{
					if (personClass.MasterRecordGuid != person.MasterRecordGuid)
					{
						if (equipment.MasterRecordGuid == personClass.AssignedEquipmentGuid)
						{
							const string Message =
								"Vehicle is already associated with another operator and may not be assigned to this operator.\n"
								+ "Please choose a different vehicle or different operator and try again.";

							throw new ApplicationException(Message);
						}
					}
				}
			}
		}

		/// <summary>
		///    Checks the grade.
		/// </summary>
		/// <param name="equipment">The equipment.</param>
		/// <param name="transactionArray">The transaction array.</param>
		/// <returns>True if the grade checks out.</returns>
		private bool CheckGrade(EquipmentClass equipment, IEnumerable<TransactionDO> transactionArray)
		{
			foreach (TransactionDO transaction in transactionArray)
			{
				foreach (LineItemDO lineItem in transaction.LineItems)
				{
					if (transaction.LineItems[0].ProductGuid == Guid.Empty || equipment.ProductGuid != lineItem.ProductGuid)
					{
						const string Message = "Grade in selected servicing unit and selected Request conflict.  Dispatch anyway?";

						this.ShowWarningDialog(Message, WarningState.CheckGrade);

						return false;
					}
				}
			}

			return true;
		}

		/// <summary>
		///    Checks if personnel already assigned to different vehicle.
		/// </summary>
		/// <param name="person">The person.</param>
		/// <param name="equipment">The equipment.</param>
		/// <returns>True if the vehicle assignment checks out ok.</returns>
		private bool CheckIfPersonnelAlreadyAssignedToDifferentVehicle(PersonClass person, EquipmentClass equipment)
		{
			if (person.AssignedEquipmentGuid.IsNotEmptyAndNotEqualTo(equipment.MasterRecordGuid))
			{
				string message = string.Format(
					"{0},{1} is currently assigned to vehicle {2}.  Reassign {0},{1} to {3}?",
					person.LastName,
					person.FirstName,
					person.AssignedEquipmentID,
					equipment.ID);

				this.ShowWarningDialog(message, WarningState.PersonnelAlreadyAssignedToDifferentVehicle);
				return false;
			}

			return true;
		}

		/// <summary>
		///    Checks if the personnel status is set to out.
		/// </summary>
		/// <param name="person">The person to check.</param>
		/// <returns>True if the person is not out.</returns>
		private bool CheckPersonnelOut(PersonClass person)
		{
			// Give warning if operator is OUT
			if (person.Status == PersonClass.STATUS.Out)
			{
				this.ShowWarningDialog(
					"The operator is currently OUT.  Dispatch anyway and update status?", WarningState.PersonnelOut);
				return false;
			}

			return true;
		}

		/// <summary>
		///    Checks the qualifications.
		/// </summary>
		/// <param name="person">The person.</param>
		/// <param name="equipment">The equipment.</param>
		/// <param name="currentSiteTime">The current site time.</param>
		/// <param name="warningState">State of the warning.</param>
		/// <param name="loopStart">The loop start.</param>
		/// <returns>True if the qualifications check out.</returns>
		private bool CheckQualifications(
			PersonClass person,
			EquipmentClass equipment,
			DateTimeOffset currentSiteTime,
			WarningState warningState,
			int loopStart)
		{
			EquipmentTypeClass equipmentType =
				FMChannelHelper.MakeCall<IEquipmentTypes, EquipmentTypeClass>(
					x => x.Get(this.Security, equipment.EquipmentTypeGuid));

			if (equipmentType.ReqQualificationsCollection.Count > 0
				&& loopStart < equipmentType.ReqQualificationsCollection.Count)
			{
				for (int index = loopStart; index < equipmentType.ReqQualificationsCollection.Count; ++index)
				{
					QualificationMapClass qualification = equipmentType.ReqQualificationsCollection[index];

					QualificationMapClass qualificationRecord = this.FindQualificationRecord(
						qualification, person.QualificationCollection);

					this.WarningLoopValue.Value = index.ToString(CultureInfo.InvariantCulture);

					if (warningState < WarningState.CheckingQualificationExists && qualificationRecord == null)
					{
						string message = string.Format(
							"{0},{1} does not have the \"{2}\" required qualification.  Dispatch anyway?",
							person.LastName,
							person.FirstName,
							qualification.ID);

						this.ShowWarningDialog(message, WarningState.CheckingQualificationExists);
						return false;
					}

					if (qualificationRecord != null && qualificationRecord.ExpirationDate.Value < currentSiteTime.Date)
					{
						string message =
							string.Format(
								"{0},{1} has the \"{2}\" required qualification but it has expired.  Dispatch anyway?",
								person.LastName,
								person.FirstName,
								qualification.ID);

						this.ShowWarningDialog(message, WarningState.CheckingQualifications);
						return false;
					}

					warningState = WarningState.NoWarnings;
				}
			}

			// Indicate we are done with the loop
			this.WarningLoopValue.Value = string.Empty;

			return true;
		}

		/// <summary>
		///    Checks the quality tag log.
		/// </summary>
		/// <param name="equipment">The equipment.</param>
		/// <returns>Returns true if the quality tag log checks out ok.</returns>
		private bool CheckQualityTagLog(EquipmentClass equipment)
		{
			// If the equiment is in service, the last quality tag should not be taken into account here
			EquipmentQualityTagLogClass tag =
				FMChannelHelper.MakeCall<IEquipmentQualityTagLogs, EquipmentQualityTagLogClass>(
					x => x.GetMostRecentByEquipmentID(this.Security, equipment.ID));

			if (tag != null && tag.IdentityGuid != Guid.Empty && string.IsNullOrEmpty(tag.RemovedBy))
			{
				if (tag.QualityTagGuid != Guid.Empty)
				{
					QualityTagClass qualityTag = FMChannelHelper.MakeCall<IQualityTags, QualityTagClass>(
																	 x => x.Get(this.Security, tag.QualityTagGuid));

					if (qualityTag.Severity == QUALITY_SEVERITY_LEVELS.CAUTION
						|| qualityTag.Severity == QUALITY_SEVERITY_LEVELS.WARNING)
					{
						string message = string.Format(
							"The servicing unit has a {0} tag.  Do you still wish to send this servicing unit?",
							Enum.GetName(typeof(QUALITY_SEVERITY_LEVELS), equipment.QualityTag.Severity));

						this.ShowWarningDialog(message, WarningState.CheckingQualityTags);
						return false;
					}

					if (qualityTag.Severity == QUALITY_SEVERITY_LEVELS.DANGER)
					{
						const string Message = "The servicing unit has a DANGER tag.  This service unit cannot be dispatched.";
						throw new ApplicationException(Message);
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Checks the refuel status.
		/// </summary>
		/// <param name="equipment">The equipment.</param>
		/// <param name="transactionArray">The transaction array.</param>
		/// <returns>True if the refuel status checks out.</returns>
		private bool CheckRefuelStatus(EquipmentClass equipment, IEnumerable<TransactionDO> transactionArray)
		{
			foreach (TransactionDO transaction in transactionArray)
			{
				if (transaction.TransTypeID == TransactionTypes.T6_SecondaryDisbursement
					&& equipment.FuelingType != FUELING_TYPES.REFUELER)
				{
					const string Message =
						"The fueling status of the unit dispatched does not match the type of request.  Dispatch anyway?";

					this.ShowWarningDialog(Message, WarningState.CheckRefuelStatus);

					return false;
				}
			}

			return true;
		}

		/// <summary>
		///    Validates the tag and licenses.
		/// </summary>
		/// <param name="equipment">The equipment.</param>
		/// <param name="currentSiteTime">The current site time.</param>
		/// <param name="loopStart">The loop start.</param>
		/// <returns>Returns true if the tags and license records check out ok.</returns>
		private bool CheckTagAndLicenses(EquipmentClass equipment, DateTimeOffset currentSiteTime, int loopStart)
		{
			if (equipment.TagAndLicenseCollection.Count > 0)
			{
				for (int index = loopStart; index < equipment.TagAndLicenseCollection.Count; ++index)
				{
					this.WarningLoopValue.Value = (index + 1).ToString(CultureInfo.InvariantCulture);

					QualificationMapClass tagLicense = equipment.TagAndLicenseCollection[index];

					if (tagLicense.ExpirationDate.Value < currentSiteTime.Date)
					{
						string message = string.Format(
							"Tag/License [{0}] for servicing unit has expired.  Dispatch anyway?", tagLicense.ID);

						this.ShowWarningDialog(message, WarningState.CheckingTagsAndLicenses);
						return false;
					}
				}
			}

			return true;
		}

		/// <summary>
		///    Checks the tests and inspections.
		/// </summary>
		/// <param name="equipment">The equipment.</param>
		/// <param name="currentSiteTime">The current site time.</param>
		/// <param name="loopStart">The loop start.</param>
		/// <returns>Returns true if the tests and inspections records check out ok.</returns>
		private bool CheckTestsAndInspections(EquipmentClass equipment, DateTimeOffset currentSiteTime, int loopStart)
		{
			if (equipment.TestAndInspectionCollection.Count > 0)
			{
				for (int index = loopStart; index < equipment.TestAndInspectionCollection.Count; ++index)
				{
					this.WarningLoopValue.Value = (index + 1).ToString(CultureInfo.InvariantCulture);

					QualificationMapClass testInspection = equipment.TestAndInspectionCollection[index];

					if (testInspection.ExpirationDate.Value < currentSiteTime.Date)
					{
						string message = string.Format(
							"Test/Inspection [{0}] for servicing unit has expired.  Dispatch anyway?", testInspection.ID);

						this.ShowWarningDialog(message, WarningState.CheckingTestsAndInspections);
						return false;
					}
				}
			}

			return true;
		}

		/// <summary>
		///    Checks the training records.
		/// </summary>
		/// <param name="person">The person.</param>
		/// <param name="equipment">The equipment.</param>
		/// <param name="currentSiteTime">The current site time.</param>
		/// <param name="warningState">State of the warning.</param>
		/// <param name="loopStart">The loop value.</param>
		/// <returns>Returns true if the training records check out ok</returns>
		private bool CheckTraining(
			PersonClass person,
			EquipmentClass equipment,
			DateTimeOffset currentSiteTime,
			WarningState warningState,
			int loopStart)
		{
			EquipmentTypeClass equipmentType =
				FMChannelHelper.MakeCall<IEquipmentTypes, EquipmentTypeClass>(
					x => x.Get(this.Security, equipment.EquipmentTypeGuid));

			if (equipmentType.ReqTrainingCollection.Count > 0)
			{
				for (int index = loopStart; index < equipmentType.ReqTrainingCollection.Count; ++index)
				{
					QualificationMapClass training = equipmentType.ReqTrainingCollection[index];

					QualificationMapClass trainingRecord = this.FindQualificationRecord(training, person.TrainingCollection);

					this.WarningLoopValue.Value = index.ToString(CultureInfo.InvariantCulture);

					if (warningState < WarningState.CheckingTrainingExists && trainingRecord == null)
					{
						string message = string.Format(
							"{0},{1} does not have the \"{2}\" required training.  Dispatch anyway?",
							person.LastName,
							person.FirstName,
							training.ID);

						this.ShowWarningDialog(message, WarningState.CheckingTrainingExists);
						return false;
					}

					if (trainingRecord != null && trainingRecord.ExpirationDate.Value < currentSiteTime.Date)
					{
						string message = string.Format(
							"{0},{1} has the \"{2}\" required training but it has expired.  Dispatch anyway?",
							person.LastName,
							person.FirstName,
							training.ID);

						this.ShowWarningDialog(message, WarningState.CheckingTraining);
						return false;
					}

					warningState = WarningState.NoWarnings;
				}
			}

			// Indicate we are done with the loop
			return true;
		}

		/// <summary>
		///    Checks if the unit is out of service.
		/// </summary>
		/// <param name="equipment">The equipment to check.</param>
		/// <returns>True if the unit is in-service.</returns>
		private bool CheckUnitOutOfService(EquipmentClass equipment)
		{
			// If selected equipment is not in service, give a warning
			if (equipment.InServiceFlag == false)
			{
				this.ShowWarningDialog("This servicing unit is out of service.  Dispatch anyway?", WarningState.UnitOutOfService);
				return false;
			}

			return true;
		}

		/// <summary>
		///    Creates the fill stand.
		/// </summary>
		private void CreateFillStand()
		{
			try
			{
				string equipmentGuid = this.EquipmentGridSelection.Value;
				string personnelGuid = this.OperatorGridSelection.Value;

				if (string.IsNullOrEmpty(equipmentGuid) || string.IsNullOrEmpty(personnelGuid)
					|| string.IsNullOrEmpty(this.AliasSelectValue.Value))
				{
					return;
				}

				// Get the transaction alias
				TransactionAliasClass transactionAlias =
					FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
						x => x.Get(this.Security, new Guid(this.AliasSelectValue.Value), false));
				if (transactionAlias.IdentityGuid == Guid.Empty)
				{
					throw new ApplicationException("Transaction alias cannot be identified for use.");
				}

				EquipmentClass equipment =
					FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(x => x.Get(this.Security, new Guid(equipmentGuid)));
				if (equipment.IdentityGuid == Guid.Empty)
				{
					throw new ApplicationException("Servicing unit cannot be identified for use.");
				}

				PersonClass person =
					FMChannelHelper.MakeCall<IPersonnel, PersonClass>(x => x.Get(this.Security, new Guid(personnelGuid)));
				if (person.IdentityGuid == Guid.Empty)
				{
					throw new ApplicationException("Operator cannot be identified for use.");
				}

				// Equipment must be of a type authorized for the Fillstand alias
				if (this.CheckEquipmentType(transactionAlias, equipment) == false)
				{
					throw new ApplicationException("Servicing unit is not authorized for Fillstand transaction.  Cannot create Fillstand.");
				}

				// Get the current site time
				SiteClass site =
					FMChannelHelper.MakeCall<ISites, SiteClass>(
						x =>
						x.Get(
							this.Security,
							this.Security.SiteGuid,
							getMemberSites: false,
							bGetAssociatedAliases: false,
							getSchedulesAndProcessVariables: false));

				var currentSiteTime = TimeConverter.Now(site);

				// Create a fillstand transaction
				var documentNumberGenerator = new DocumentNumberGenerator(this.Security);
				var lineItem = new LineItemDO();
				var transaction = new TransactionDO
				                  {
					                  TransactionDateTime	= currentSiteTime,
					                  SubmittedToAccounting = false,
					                  TransID				= FuelsManagerId.NewId(),
					                  Site					= this.Security.SiteID,
					                  SiteGuid				= this.Security.SiteGuid,
					                  Alias					= transactionAlias.ID,
					                  TransTypeID			= transactionAlias.TransTypeID,
					                  TransactionAliasGuid	= transactionAlias.MasterRecordGuid,
									  DocumentNumber		= documentNumberGenerator.GetNextDocumentNumber(transactionAlias.TransTypeID)
				                  };


				transaction.LineItems.Add(lineItem);

				var inventoryDateSr = new InventoryDateSR { Security = this.Security, CurrentSiteGuid = this.Security.SiteGuid };

				InventoryDateDO inventoryDateDO =
					FMChannelHelper.MakeCall<IInventoryDateProcessor, InventoryDateDO>(x => x.Process(inventoryDateSr));

				transaction.InventoryDate = inventoryDateDO.InventoryDate;
				transaction.OriginApplication = this.DetermineOriginApplication();
				transaction.Status = TransactionStatus.Requested;
				transaction.RequestedDateTime = currentSiteTime;

				lineItem.RequestedDateTime = transaction.RequestedDateTime;
				lineItem.Status = TransactionStatus.Dispatched;

				var unitsHelper = new UnitsHelperClass(this.Security, site, transactionAlias, null);
				unitsHelper.SetUnits(transaction, 0);

				List<CompanyClass> managerCollection =
					FMChannelHelper.MakeCall<ICompanies, List<CompanyClass>>(
						x => x.EnumerateByRole(this.Security, COMPANY_ROLE.MANAGER, byGroupCompanies: false, bLocalize: true));

				if (managerCollection.Count == 0)
				{
					throw new Exception("No Manager");
				}

				if (managerCollection.Count > 1)
				{
					throw new Exception("Multiple Managers");
				}

				transaction.ManagerID = managerCollection[0].ID;
				transaction.ManagerCode = managerCollection[0].Code;
				transaction.ManagerCompanyGuid = managerCollection[0].MasterRecordGuid;

				List<CompanyClass> ownerCollection =
					FMChannelHelper.MakeCall<ICompanies, List<CompanyClass>>(
						x => x.EnumerateByRole(this.Security, COMPANY_ROLE.OWNER, byGroupCompanies: false, bLocalize: true));

				if (ownerCollection.Count == 0)
				{
					throw new Exception("No Owner");
				}

				if (ownerCollection.Count > 1)
				{
					throw new Exception("Multiple Owners");
				}

				transaction.OwnerID = ownerCollection[0].ID;
				transaction.OwnerCode = ownerCollection[0].Code;
				transaction.OwnerCompanyGuid = ownerCollection[0].MasterRecordGuid;
				transaction.Number02 = Convert.ToDouble(FuelRequestType.FillStand);

				transaction.Notes = string.Empty;

				if (equipment.ProductGuid != Guid.Empty)
				{
					ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
						x =>
						x.GetByInfoAuthorizedCompanies(
							this.Security, equipment.ProductGuid, getMinimalInfo: true, getAuthorizedCompanies: false));

					lineItem.Product = product.ID;
					lineItem.ProductCode = ProductClass.ProductTypeID( product.ProductType );
					lineItem.ProductGuid = product.MasterRecordGuid;
					unitsHelper.Product = product;
					unitsHelper.SetUnits(lineItem, defaultProductType: 0, product: product);
				}
				else
				{
					throw new ApplicationException("Selected servicing unit has no product assignment");
				}

				lineItem.Quantity = new QuantityDO( 0, 0, 0, 0 );

				// Registration ID
				var equipmentDO = new EquipmentDO(equipment);
				transaction.DestinationEQ1 = equipmentDO;
				transaction.DestinationEQ1.RegistrationID = equipment.ID;
				lineItem.DestinationEQ = new EquipmentDO( equipment );

				lineItem.PartialFill = false;

				// Dispatch the transaction
				this.PerformDispatchTransaction(person, equipment, transaction, currentSiteTime);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Creates the return to bulk transaction.
		/// </summary>
		private void CreateReturnToBulk()
		{
			try
			{
				string equipmentGuid = this.EquipmentGridSelection.Value;
				string personnelGuid = this.OperatorGridSelection.Value;

				if (string.IsNullOrEmpty(equipmentGuid) || string.IsNullOrEmpty(personnelGuid)
					|| string.IsNullOrEmpty(this.AliasSelectValue.Value))
				{
					return;
				}

				// Get the transaction alias
				TransactionAliasClass transactionAlias =
					FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
						x => x.Get(this.Security, new Guid(this.AliasSelectValue.Value), false));
				if (transactionAlias.IdentityGuid == Guid.Empty)
				{
					throw new ApplicationException("Transaction alias cannot be identified for use.");
				}

				EquipmentClass equipment =
					FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(x => x.Get(this.Security, new Guid(equipmentGuid)));
				if (equipment.IdentityGuid == Guid.Empty)
				{
					throw new ApplicationException("Servicing unit cannot be identified for use.");
				}

				PersonClass person =
					FMChannelHelper.MakeCall<IPersonnel, PersonClass>(x => x.Get(this.Security, new Guid(personnelGuid)));
				if (person.IdentityGuid == Guid.Empty)
				{
					throw new ApplicationException("Operator cannot be identified for use.");
				}

				// Equipment must be of a type authorized for the Return to Bulk alias
				if (this.CheckEquipmentType(transactionAlias, equipment) == false)
				{
					throw new ApplicationException(
						"Servicing unit is not authorized for Return to Bulk transaction.  Cannot create Return to Bulk.");
				}

				// Get the current site time
				SiteClass site =
					FMChannelHelper.MakeCall<ISites, SiteClass>(
						x =>
						x.Get(
							this.Security,
							this.Security.SiteGuid,
							getMemberSites: false,
							bGetAssociatedAliases: false,
							getSchedulesAndProcessVariables: false));

				var currentSiteTime = TimeConverter.Now(site);

				// Create a return to bulk transaction
				var documentNumberGenerator = new DocumentNumberGenerator(this.Security);
				var lineItem = new LineItemDO();
				var transaction = new TransactionDO
				                  {
					                  TransactionDateTime	= currentSiteTime,
					                  SubmittedToAccounting = false,
					                  TransID				= FuelsManagerId.NewId(),
					                  Site					= this.Security.SiteID,
					                  SiteGuid				= this.Security.SiteGuid,
					                  Alias					= transactionAlias.ID,
					                  TransTypeID			= transactionAlias.TransTypeID,
					                  TransactionAliasGuid	= transactionAlias.MasterRecordGuid,
									  DocumentNumber		= documentNumberGenerator.GetNextDocumentNumber(transactionAlias.TransTypeID)
				                  };


				transaction.LineItems.Add(lineItem);

				var inventoryDateSr = new InventoryDateSR { Security = this.Security, CurrentSiteGuid = this.Security.SiteGuid };

				InventoryDateDO inventoryDateDO =
					FMChannelHelper.MakeCall<IInventoryDateProcessor, InventoryDateDO>(x => x.Process(inventoryDateSr));

				transaction.InventoryDate = inventoryDateDO.InventoryDate;
				transaction.OriginApplication = this.DetermineOriginApplication();
				transaction.Status = TransactionStatus.Requested;
				transaction.RequestedDateTime = currentSiteTime;

				lineItem.RequestedDateTime = transaction.RequestedDateTime;
				lineItem.Status = TransactionStatus.Dispatched;

				var unitsHelper = new UnitsHelperClass(this.Security, site, transactionAlias, null);
				unitsHelper.SetUnits(transaction, 0);

				List<CompanyClass> managerCollection =
					FMChannelHelper.MakeCall<ICompanies, List<CompanyClass>>(
						x => x.EnumerateByRole(this.Security, COMPANY_ROLE.MANAGER, byGroupCompanies: false, bLocalize: true));

				if (managerCollection.Count == 0)
				{
					throw new Exception("No Manager");
				}

				if (managerCollection.Count > 1)
				{
					throw new Exception("Multiple Managers");
				}

				transaction.ManagerID = managerCollection[0].ID;
				transaction.ManagerCode = managerCollection[0].Code;
				transaction.ManagerCompanyGuid = managerCollection[0].MasterRecordGuid;

				List<CompanyClass> ownerCollection =
					FMChannelHelper.MakeCall<ICompanies, List<CompanyClass>>(
						x => x.EnumerateByRole(this.Security, COMPANY_ROLE.OWNER, byGroupCompanies: false, bLocalize: true));

				if (ownerCollection.Count == 0)
				{
					throw new Exception("No Owner");
				}

				if (ownerCollection.Count > 1)
				{
					throw new Exception("Multiple Owners");
				}

				transaction.OwnerID = ownerCollection[0].ID;
				transaction.OwnerCode = ownerCollection[0].Code;
				transaction.OwnerCompanyGuid = ownerCollection[0].MasterRecordGuid;

				// TODO: Make this a core field.
				// Return to bulk is a fillstand type request.
				transaction.Number02 = Convert.ToDouble(FuelRequestType.FillStand);

				transaction.Notes = string.Empty;

				if (equipment.ProductGuid != Guid.Empty)
				{
					ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
						x =>
						x.GetByInfoAuthorizedCompanies(
							this.Security, equipment.ProductGuid, getMinimalInfo: true, getAuthorizedCompanies: false));

					lineItem.Product = product.ID;
					lineItem.ProductCode = ProductClass.ProductTypeID(product.ProductType);
					lineItem.ProductGuid = product.MasterRecordGuid;
					unitsHelper.Product = product;
					unitsHelper.SetUnits(lineItem, defaultProductType: 0, product: product);
				}
				else
				{
					throw new ApplicationException("Selected servicing unit has no product assignment");
				}

				lineItem.Quantity = new QuantityDO(0, 0, 0, 0);

				// Registration ID
				var equipmentDO = new EquipmentDO(equipment);
				transaction.SourceEQ1 = equipmentDO;
				transaction.SourceEQ1.RegistrationID = equipment.ID;
				lineItem.SourceEQ = new EquipmentDO(equipment);

				// Dispatch the transaction
				this.PerformDispatchTransaction(person, equipment, transaction, currentSiteTime);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Dispatches the button on server click.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">
		///    The <see cref="System.EventArgs" /> instance containing the event data.
		/// </param>
		private void DispatchButtonOnServerClick(object sender, EventArgs e)
		{
			try
			{
				this.WarningLoopValue.Value = string.Empty;
				this.DispatchProcess(WarningState.NoWarnings);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Governs the dispatching process including restart after a warning dialog response.
		/// </summary>
		/// <param name="warningState">Specifies the current warning being processed.</param>
		private void DispatchProcess(WarningState warningState)
		{
			// Get the current site time
			SiteClass site =
				FMChannelHelper.MakeCall<ISites, SiteClass>(
					x =>
					x.Get(
						this.Security,
						this.Security.SiteGuid,
						getMemberSites: false,
						bGetAssociatedAliases: false,
						getSchedulesAndProcessVariables: false));

			var currentSiteTime = TimeConverter.Now(site);

			string equipmentGuid = this.EquipmentGridSelection.Value;
			string personnelGuid = this.OperatorGridSelection.Value;
			string transactionIds = this.RequestGridSelection.Value;

			// Get the equipment record
			EquipmentClass equipment = new EquipmentClass();
			if (!string.IsNullOrEmpty(equipmentGuid))
			{
				equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
					x => x.Get(this.Security, new Guid(equipmentGuid)));
			}

			// Get the personnel record
			PersonClass person = new PersonClass();
			if (!string.IsNullOrEmpty(personnelGuid))
			{
				person = FMChannelHelper.MakeCall<IPersonnel, PersonClass>(x => x.Get(this.Security, new Guid(personnelGuid)));
			}

			this.ConfigurationSettings = this.GetConfigurationSettings();

			if (this.ConfigurationSettings.EquipmentRequired && equipment.IdentityGuid == Guid.Empty)
			{
				throw new ApplicationException("Equipment selection is required for dispatching.");
			}

			if (this.ConfigurationSettings.PersonnelRequired && person.IdentityGuid == Guid.Empty)
			{
				throw new ApplicationException("Operator selection is required for dispatching.");
			}

			if (string.IsNullOrEmpty(transactionIds))
			{
				throw new ApplicationException("No transaction is selected for dispatching.");
			}

			if (warningState == WarningState.NoWarnings)
			{
				this.CheckForInitialDispatchingErrors(equipment, person);
			}

			// If selected equipment is not in service, give a warning
			if (warningState < WarningState.UnitOutOfService && this.ConfigurationSettings.EquipmentRequired
				&& this.CheckUnitOutOfService(equipment) == false)
			{
				return;
			}

			// Check if the personnel status is set to Out
			if (warningState < WarningState.PersonnelOut && this.ConfigurationSettings.PersonnelRequired
				&& this.CheckPersonnelOut(person) == false)
			{
				return;
			}

			// Check if the equipment quality check date is overdue
			if (warningState < WarningState.EquipmentQualitCheckOverdue && this.ConfigurationSettings.EquipmentRequired
				&& this.CheckEquipmentQualityCheckupDate(equipment, currentSiteTime) == false)
			{
				return;
			}

			// Check if the personnel is already assigned to a different equipment
			if (warningState < WarningState.PersonnelAlreadyAssignedToDifferentVehicle
				&& this.ConfigurationSettings.EquipmentRequired
				&& this.CheckIfPersonnelAlreadyAssignedToDifferentVehicle(person, equipment) == false)
			{
				return;
			}

			if (warningState < WarningState.CheckingTrainingExists && this.ConfigurationSettings.EquipmentRequired)
			{
				int loopValue = 0;

				if (string.IsNullOrEmpty(this.WarningLoopValue.Value) == false)
				{
					loopValue = Convert.ToInt32(this.WarningLoopValue.Value);
				}

				if (this.CheckQualifications(person, equipment, currentSiteTime, warningState, loopValue) == false)
				{
					return;
				}

				this.WarningLoopValue.Value = string.Empty;
			}

			if (warningState < WarningState.CheckingTagsAndLicenses && this.ConfigurationSettings.EquipmentRequired)
			{
				int loopValue = 0;

				if (string.IsNullOrEmpty(this.WarningLoopValue.Value) == false)
				{
					loopValue = Convert.ToInt32(this.WarningLoopValue.Value);
				}

				if (this.CheckTraining(person, equipment, currentSiteTime, warningState, loopValue) == false)
				{
					return;
				}

				this.WarningLoopValue.Value = string.Empty;
			}

			if (warningState < WarningState.CheckingTestsAndInspections && this.ConfigurationSettings.EquipmentRequired)
			{
				int loopValue = 0;

				if (string.IsNullOrEmpty(this.WarningLoopValue.Value) == false)
				{
					loopValue = Convert.ToInt32(this.WarningLoopValue.Value);
				}

				if (this.CheckTagAndLicenses(equipment, currentSiteTime, loopValue) == false)
				{
					return;
				}

				this.WarningLoopValue.Value = string.Empty;
			}

			if (warningState < WarningState.CheckingQualityTags && this.ConfigurationSettings.EquipmentRequired)
			{
				int loopValue = 0;

				if (string.IsNullOrEmpty(this.WarningLoopValue.Value) == false)
				{
					loopValue = Convert.ToInt32(this.WarningLoopValue.Value);
				}

				if (this.CheckTestsAndInspections(equipment, currentSiteTime, loopValue) == false)
				{
					return;
				}

				this.WarningLoopValue.Value = string.Empty;
			}

			if (warningState < WarningState.CheckingQualityTags && this.ConfigurationSettings.EquipmentRequired
				&& this.CheckQualityTagLog(equipment) == false)
			{
				return;
			}

			// Parse the transaction IDs - delayed doing it to here for performance
			List<TransactionDO> transactionArray = this.ParseTransactionIds(transactionIds);

			if (warningState < WarningState.CheckAdditiveFlag && this.ConfigurationSettings.EquipmentRequired
				&& this.CheckAdditiveFlag(equipment, transactionArray) == false)
			{
				return;
			}

			if (warningState < WarningState.CheckDefuelStatus && this.ConfigurationSettings.EquipmentRequired
				&& this.CheckDefuelStatus(equipment, transactionArray) == false)
			{
				return;
			}

			if (warningState < WarningState.CheckRefuelStatus && this.ConfigurationSettings.EquipmentRequired
				&& this.CheckRefuelStatus(equipment, transactionArray) == false)
			{
				return;
			}

			if (warningState < WarningState.CheckGrade && this.ConfigurationSettings.EquipmentRequired
				&& this.CheckGrade(equipment, transactionArray) == false)
			{
				return;
			}

			foreach (TransactionDO transaction in transactionArray)
			{
				this.PerformDispatchTransaction(person, equipment, transaction, currentSiteTime);
			}
		}

		/// <summary>
		///    Fills the stand button on server click.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">
		///    The <see cref="System.EventArgs" /> instance containing the event data.
		/// </param>
		private void FillStandButtonOnServerClick(object sender, EventArgs e)
		{
			try
			{
				this.ConfigurationSettings = this.GetConfigurationSettings();

				string equipmentGuid = this.EquipmentGridSelection.Value;
				string personnelGuid = this.OperatorGridSelection.Value;

				if (this.ConfigurationSettings.EquipmentRequired && string.IsNullOrEmpty(equipmentGuid))
				{
					throw new ApplicationException("Equipment selection is required for dispatching.");
				}

				if (this.ConfigurationSettings.PersonnelRequired && string.IsNullOrEmpty(personnelGuid))
				{
					throw new ApplicationException("Operator selection is required for dispatching.");
				}

				// Get the list of transaction aliases
				TransactionAliasNameCollectionClass aliasNames =
					FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasNameCollectionClass>(
						x => x.EnumerateDispatchAliasNames(this.Security));

				int count = aliasNames.Count(x => x.TransTypeID == TransactionTypes.T7_FillStand);

				IEnumerable<TransactionAliasNameClass> fillStandNames = from aliasName in aliasNames
																		where aliasName.TransTypeID == TransactionTypes.T7_FillStand
																		select aliasName;

				if (count == 0)
				{
					throw new ApplicationException("Cannot create transaction.  No fill stand transaction aliases found.");
				}

				if (count > 1)
				{
					// Prompt for alias.
					this.ShowAliasSelectionDialog(
						fillStandNames, "Select the fill stand transaction type to create:", AliasPromptMode.FillStand);
					return;
				}

				if (count == 1)
				{
					this.AliasSelectValue.Value = fillStandNames.ElementAt(0).IdentityGuid.ToString();
					this.CreateFillStand();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Finds the qualification record.
		/// </summary>
		/// <param name="qualification">The qualification.</param>
		/// <param name="collection">The collection.</param>
		/// <returns>The qualification record if found otherwise null.</returns>
		private QualificationMapClass FindQualificationRecord(
			QualificationMapClass qualification, IEnumerable<QualificationMapClass> collection)
		{
			foreach (QualificationMapClass personQualification in collection)
			{
				if (personQualification.AssignedGuid == qualification.AssignedGuid)
				{
					return personQualification;
				}
			}

			return null;
		}

		/// <summary>
		///    The purpose of this method is to generate a function that can be called
		///    by the double-click event on the equipment SlickGrid to allow editing of
		///    selected items.
		/// </summary>
		private void GenerateEquipmentGridEditPostBack()
		{
			// Get the postback script name
			string postBackString = this.Page.ClientScript.GetPostBackEventReference(this, "EquipmentGridDoubleClick");

			string scriptBlock = @"
				DispatchingViewLib.equipmentGridDoubleClick = function()
				{
					if (DispatchingViewLib.equipmentGridSettings.selectedRows != undefined && DispatchingViewLib.equipmentGridSettings.selectedRows.length > 0)
					{
						var rowNum = DispatchingViewLib.equipmentGridSettings.selectedRows[0];
						Form1.EquipmentGridSelection.value = DispatchingViewLib.equipmentData[rowNum].IdentityGuid;
					}

					" + postBackString + @"
				}
			";

			this.ClientScript.RegisterClientScriptBlock(this.GetType(), "DispatchingViewLib.equipmentGridDoubleClick", scriptBlock, true);
		}

		/// <summary>
		///    The purpose of this method is to generate a function that can be called
		///    by the double-click event on the personnel SlickGrid to allow editing of
		///    selected items.
		/// </summary>
		private void GeneratePersonnelGridEditPostBack()
		{
			// Get the postback script name
			string postBackString = this.Page.ClientScript.GetPostBackEventReference(this, "PersonnelGridDoubleClick");

			string scriptBlock = @"
				DispatchingViewLib.personnelGridDoubleClick = function()
				{
					if (DispatchingViewLib.personnelGridSettings.selectedRows != undefined && DispatchingViewLib.personnelGridSettings.selectedRows.length > 0)
					{
						var rowNum = DispatchingViewLib.personnelGridSettings.selectedRows[0];
						Form1.OperatorGridSelection.value = DispatchingViewLib.personnelData[rowNum].IdentityGuid;
					}

					" + postBackString + @"
				}
			";

			this.ClientScript.RegisterClientScriptBlock(this.GetType(), "DispatchingViewLib.personnelGridDoubleClick", scriptBlock, true);
		}

		/// <summary>
		///    Initializes the components of the page.
		/// </summary>
		private void InitializeComponents()
		{
			this.HomeButton.ServerClick += this.HomeButtonOnServerClick;
			this.OutButton.ServerClick += this.OutButtonOnServerClick;
			this.StandbyButton.ServerClick += this.StandbyButtonOnServerClick;
			this.StandbyButton2.ServerClick += this.StandbyButtonOnServerClick;
			this.RadioButton.ServerClick += this.RadioButtonOnServerClick;
			this.DispatchButton.ServerClick += this.DispatchButtonOnServerClick;
			this.UnDispatchButton.ServerClick += this.UndispatchButtonOnServerClick;
			this.FillStandButton.ServerClick += this.FillStandButtonOnServerClick;
			this.ReturnToBulkButton.ServerClick += this.ReturnToBulkButtonOnServerClick;
		}

		/// <summary>
		///    Out button click event handler
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="eventArgs">
		///    The <see cref="System.EventArgs" /> instance containing the event data.
		/// </param>
		private void OutButtonOnServerClick(object sender, EventArgs eventArgs)
		{
			try
			{
				if (string.IsNullOrEmpty(this.OperatorGridSelection.Value) == false)
				{
					FMChannelHelper.MakeCall<IPersonnel>(this.SetPersonToOutStatus);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Parses custom event arguments and dispatches the messages to handler routines as appropriate.
		/// </summary>
		/// <returns>True if valid event argument found to indicate it has been handled.</returns>
		private bool ParseCustomEventArguments()
		{
			string arguments = this.Request.GetQueryOrFormValue("__EVENTARGUMENT");
			if (arguments != null)
			{
				if (arguments.Equals("WarningYesButtonClick", StringComparison.InvariantCultureIgnoreCase))
				{
					this.RestartDispatchProcess();
					return true;
				}

				if (arguments.Equals("RadioDialogOkButtonClick", StringComparison.InvariantCultureIgnoreCase))
				{
					this.RadioDialogOkButtonClick();
					return true;
				}

				if (arguments.Equals("EquipmentGridDoubleClick", StringComparison.InvariantCultureIgnoreCase))
				{
					this.TransferForEquipmentEdit();
					return true;
				}

				if (arguments.Equals("PersonnelGridDoubleClick", StringComparison.InvariantCultureIgnoreCase))
				{
					this.TransferForPersonnelEdit();
					return true;
				}

				if (arguments.Equals("StandbyOkButtonClick", StringComparison.InvariantCultureIgnoreCase))
				{
					this.StandbyOkButtonClick();
					return true;
				}

				if (arguments.Equals("AliasOkButtonClick", StringComparison.InvariantCultureIgnoreCase))
				{
					this.AliasOkButtonClick();
				}
			}

			return false;
		}

		/// <summary>
		///    Performs the transaction dispatch.
		/// </summary>
		/// <param name="person">The person.</param>
		/// <param name="equipment">The equipment.</param>
		/// <param name="transaction">The transaction to dispatch.</param>
		/// <param name="dispatchDate">The dispatch date time.</param>
		private void PerformDispatchTransaction(
			PersonClass person,
			EquipmentClass equipment,
			TransactionDO transaction,
			DateTimeOffset dispatchDate)
		{
			// Dispatch
			transaction.OperatorPersonnelGuid = person.MasterRecordGuid;
			transaction.OperatorID = person.ID;
			transaction.OperatorName = person.FullName;

			transaction.Status = TransactionStatus.Dispatched;
			transaction.DispatchedDateTime = dispatchDate;

			foreach (LineItemDO lineItem in transaction.LineItems)
			{
				lineItem.Status = TransactionStatus.Dispatched;
				lineItem.DispatchedDateTime = transaction.DispatchedDateTime;
				switch (transaction.TransTypeID)
				{
					case TransactionTypes.T3_PrimaryDefuel:
					case TransactionTypes.T4_SecondaryDefuel:
					case TransactionTypes.T7_FillStand:
						lineItem.DestinationEQ = new EquipmentDO(equipment) { RegistrationID = equipment.ID };
						transaction.DestinationEQ1 = lineItem.DestinationEQ;
						break;

					case TransactionTypes.T5_PrimaryDisbursement:
					case TransactionTypes.T6_SecondaryDisbursement:
					case TransactionTypes.T10_Unload:
					case TransactionTypes.T12_InventoryNotAffected:
						lineItem.SourceEQ = new EquipmentDO(equipment) { RegistrationID = equipment.ID };
						transaction.SourceEQ1 = lineItem.SourceEQ;
						break;

					default:
						throw new ApplicationException(
							"Unhandled transaction type passed to dispatch: " + transaction.TransTypeID.ToString());
				}

				foreach (SubLineItemDO sublineitem in lineItem.SubLineItems)
				{
					sublineitem.Status = TransactionStatus.Dispatched;
				}
			}

			person.AssignedEquipmentID = equipment.ID;
			person.AssignedEquipmentGuid = equipment.MasterRecordGuid;
			person.Status = PersonClass.STATUS.In;

			transaction.IssuePoint = equipment.IssPt;
			transaction.IssuePointNumber = equipment.IssPtNum;

			this.SaveTransaction(transaction, person);
		}

		/// <summary>
		/// Event handler for ok button on radio dialog
		/// </summary>
		private void RadioDialogOkButtonClick()
		{
			try
			{
				string transId = this.RequestGridSelection.Value;

				string radioValue = this.RadioTextValue.Value;

				TransactionDO transaction = this.GetTransaction(transId);

				if (transaction == null)
				{
					throw new ApplicationException("Transaction not found.");
				}

				transaction.RadioNumber = radioValue;

				this.SaveTransaction(transaction);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Restarts the dispatch process after a warning dialog.
		/// </summary>
		private void RestartDispatchProcess()
		{
			// Parse warning state
			WarningState warningState;
			if (Enum.TryParse(this.RadioTextValue.Value, out warningState))
			{
				this.DispatchProcess(warningState);
			}
			else
			{
				throw new ApplicationException("Invalid warning state encountered.");
			}
		}

		/// <summary>
		///    Returns to bulk button on server click.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">
		///    The <see cref="System.EventArgs" /> instance containing the event data.
		/// </param>
		private void ReturnToBulkButtonOnServerClick(object sender, EventArgs e)
		{
			try
			{
				this.ConfigurationSettings = this.GetConfigurationSettings();

				string equipmentGuid = this.EquipmentGridSelection.Value;
				string personnelGuid = this.OperatorGridSelection.Value;

				if (this.ConfigurationSettings.EquipmentRequired && string.IsNullOrEmpty(equipmentGuid))
				{
					throw new ApplicationException("Equipment selection is required for dispatching.");
				}

				if (this.ConfigurationSettings.PersonnelRequired && string.IsNullOrEmpty(personnelGuid))
				{
					throw new ApplicationException("Operator selection is required for dispatching.");
				}

				// Get the list of transaction aliases
				TransactionAliasNameCollectionClass aliasNames =
					FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasNameCollectionClass>(
						x => x.EnumerateDispatchAliasNames(this.Security));

				int count = aliasNames.Count(x => x.TransTypeID == TransactionTypes.T10_Unload);

				IEnumerable<TransactionAliasNameClass> returnNames = from aliasName in aliasNames
																	 where
																		aliasName.TransTypeID == TransactionTypes.T10_Unload
																	 select aliasName;

				if (count == 0)
				{
					throw new ApplicationException("Cannot create transaction.  No return to bulk transaction aliases found.");
				}

				if (count > 1)
				{
					// Prompt for alias.
					this.ShowAliasSelectionDialog(
						returnNames, "Select the return-to-bulk transaction type to create:", AliasPromptMode.ReturnToBulk);
					return;
				}

				if (count == 1)
				{
					this.AliasSelectValue.Value = returnNames.ElementAt(0).IdentityGuid.ToString();
					this.CreateReturnToBulk();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    The set grid selection call.
		/// </summary>
		private void SetGridSelectionCall()
		{
			const string ScriptBlock = "DispatchingViewLib.setGridSelectionsFlag();";
			this.ClientScript.RegisterClientScriptBlock(this.GetType(), "setGridSelections", ScriptBlock, true);
		}

		/// <summary>
		///    Sets the person to home status.
		/// </summary>
		/// <param name="personnel">The personnel.</param>
		private void SetPersonToHomeStatus(IPersonnel personnel)
		{
			PersonClass person = personnel.Get(this.Security, new Guid(this.OperatorGridSelection.Value));

			person.Status = PersonClass.STATUS.In;
			person.AssignedEquipmentGuid = Guid.Empty;
			person.AssignedEquipmentID = string.Empty;

			personnel.Modify(this.Security, DATA_TYPE.DYNAMIC, person);
		}

		/// <summary>
		///    Sets the person to out status.
		/// </summary>
		/// <param name="personnel">The personnel.</param>
		private void SetPersonToOutStatus(IPersonnel personnel)
		{
			PersonClass person = personnel.Get(this.Security, new Guid(this.OperatorGridSelection.Value));
			person.Status = PersonClass.STATUS.Out;
			personnel.Modify(this.Security, DATA_TYPE.DYNAMIC, person);
		}

		/// <summary>
		///    Sets the person to standby status.
		/// </summary>
		/// <param name="proxyPersonnel">The proxy personnel.</param>
		/// <param name="equipment">The equipment.</param>
		/// <param name="selectedPersonGuid">The selected person GUID.</param>
		private void SetPersonToStandbyStatus(IPersonnel proxyPersonnel, EquipmentClass equipment, string selectedPersonGuid)
		{
			PersonClass person = proxyPersonnel.Get(this.Security, new Guid(selectedPersonGuid));
			person.Status = PersonClass.STATUS.STB;
			person.AssignedEquipmentGuid = equipment.MasterRecordGuid;
			proxyPersonnel.Modify(this.Security, DATA_TYPE.DYNAMIC, person);
		}

		/// <summary>
		///    Shows the alias selection dialog.
		/// </summary>
		/// <param name="aliasNames">The fill stand names.</param>
		/// <param name="promptMessage">The prompt message.</param>
		/// <param name="aliasPromptMode">The mode of the alias selection.</param>
		private void ShowAliasSelectionDialog(
			IEnumerable<TransactionAliasNameClass> aliasNames, string promptMessage, AliasPromptMode aliasPromptMode)
		{
			try
			{
				// Get the postback script name
				string postBackString = this.Page.ClientScript.GetPostBackEventReference(this, "AliasOkButtonClick");

				string appendOptions = string.Empty;

				foreach (TransactionAliasNameClass name in aliasNames)
				{
					appendOptions += string.Format(
						"$('#AliasSelect').append(new Option('{0}', '{1}', true, true));", name.AliasName, name.IdentityGuid);
				}

				// Create script block - use delay to have dialog display after entire page rendered (otherwise the page is blank).
				// We are reusing the RadioTextValue hidden control to communicate the selected equipment value.
				string scriptBlock = @"
						function endDialogGetResponse()
						{
							$('#AliasSelectValue').val($('#AliasSelect').val());
							$('#WarningLoopValue').val('" + aliasPromptMode + @"');

							$(this).dialog('close');
							" + postBackString + @";
						}

						function AliasDialogFunction() {
							$('#AliasPrompt').text('" + promptMessage + @"');

							" + appendOptions + @"
							
							$('#AliasSelectionDialog').dialog(
							{
								autoOpen: false,
								modal: true,
								width: 350,
								height: 225,
								buttons: {
									'OK': endDialogGetResponse,
									'Cancel' : function() {$(this).dialog('close'); }
								}
							});

							$('#AliasSelectionDialog').dialog('open');
						}

						window.setTimeout('AliasDialogFunction()', 100);
					";

				this.ClientScript.RegisterClientScriptBlock(this.GetType(), "ButtonClickEvent3", scriptBlock, true);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Shows the warning dialog.
		/// </summary>
		/// <param name="question">The question to ask.</param>
		/// <param name="warningState">The stage of dispatch where the warning occurs.</param>
		private void ShowWarningDialog(string question, WarningState warningState)
		{
			// Get the postback script name
			string postBackString = this.Page.ClientScript.GetPostBackEventReference(this, "WarningYesButtonClick");

			// Save the warning state for use on postback
			this.RadioTextValue.Value = ((int)warningState).ToString(CultureInfo.InvariantCulture);

			// Create script block - use delay to have dialog display after entire page rendered (otherwise the page is blank).
			// We are reusing the RadioTextValue hidden control to communicate the selected equipment value.
			string scriptBlock = @"
						function endDialogGetResponse()
						{
							$(this).dialog('close');
							" + postBackString + @";
						}

						function WarningDialogFunction() {
							$('#WarningTextLabel').text('" + question + @"');

							$('#WarningDialog').dialog(
							{
								autoOpen: false,
								modal: true,
								width: 400,
								height: 225,
								buttons: {
									'Yes': endDialogGetResponse,
									'No' : function() {$(this).dialog('close'); }
								}
							});

							$('#WarningDialog').dialog('open');
						}

						window.setTimeout('WarningDialogFunction()', 100);
					";

			this.ClientScript.RegisterClientScriptBlock(this.GetType(), "ButtonClickEvent3", scriptBlock, true);
		}

		/// <summary>
		///    Standby button click event handler
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="eventArgs">
		///    The <see cref="System.EventArgs" /> instance containing the event data.
		/// </param>
		private void StandbyButtonOnServerClick(object sender, EventArgs eventArgs)
		{
			try
			{
				// Get the person record
				PersonClass person =
					FMChannelHelper.MakeCall<IPersonnel, PersonClass>(
						x => x.Get(this.Security, new Guid(this.OperatorGridSelection.Value)));

				// Get the postback script name
				string postBackString = this.Page.ClientScript.GetPostBackEventReference(this, "StandbyOkButtonClick");

				// Get the list of equipment to offer for selection
				EquipmentCollectionClass equipmentList =
					FMChannelHelper.MakeCall<IEquipments, EquipmentCollectionClass>(x => x.EnumerateBySource(this.Security));

				string appendOptions = string.Empty;

				foreach (EquipmentClass equipment in equipmentList)
				{
					// Add equipment to the registration selection control only if it has not been assigned to a personnel record
					if (!equipment.IsAssignedToPersonnel)
					{
						appendOptions += string.Format(
							"$('#RegistrationSelect').append(new Option('{0}', '{1}', true, true));", equipment.Xref, equipment.IdentityGuid);
					}
				}

				string message = string.Format(
					"{0},{1} is currently assigned to {2}.  Do you wish to reassign {0},{1} to vehicle ",
					person.LastName,
					person.FirstName,
					person.AssignedEquipmentID);

				string assignedEquipmentGuid = string.Empty;
				if (person.AssignedEquipmentGuid != Guid.Empty)
				{
					assignedEquipmentGuid = person.AssignedEquipmentGuid.ToString();
				}

				// Create script block - use delay to have dialog display after entire page rendered (otherwise the page is blank).
				// We are reusing the RadioTextValue hidden control to communicate the selected equipment value.
				string scriptBlock = @"
						function endDialogGetResponse()
						{
							$('#RadioTextValue').val($('#RegistrationSelect').val());

							var emptyGuid = '" + Guid.Empty.ToString() + @"';
							var assignedEquipment = '" + person.AssignedEquipmentGuid.ToString() + @"';
							var selectedEquipment = $('#RegistrationSelect').val();
							var selectedEquipmentName = $('#RegistrationSelect :selected').text();

							var answer = true;
							if ( assignedEquipment != emptyGuid && assignedEquipment != selectedEquipment )
							{
								answer = confirm('" + message + @"' + selectedEquipmentName);
							}

							if (answer)
							{
								$(this).dialog('close');
								" + postBackString + @";
							}
						}

						function StandbyDialogFunction() {
							$('#OperatorNameText').val('" + person.FullName + @"');
							$('#EmployeeIdText').val('" + person.ID + @"');

							" + appendOptions + @"

							$('#RegistrationSelect').val('" + assignedEquipmentGuid + @"');

							$('#StandbyRegistrationSelectionForm').dialog(
							{
								autoOpen: false,
								modal: true,
								width: 450,
								height: 300,
								resizable: false,
								buttons: {
									'OK': endDialogGetResponse,
									'Cancel' : function() {$(this).dialog('close'); }
								}
							});

							$('#StandbyRegistrationSelectionForm').dialog('open');
						}

						window.setTimeout('StandbyDialogFunction()', 100);
					";

				this.ClientScript.RegisterClientScriptBlock(this.GetType(), "ButtonClickEvent2", scriptBlock, true);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Event handler for the ok button on the Standby registration dialog
		/// </summary>
		private void StandbyOkButtonClick()
		{
			try
			{
				string equipmentSelected = this.RadioTextValue.Value;

				if (string.IsNullOrEmpty(equipmentSelected))
				{
					throw new ApplicationException(
						"The Reference Number is empty or is not in the list.  Please select a Reference Number from the list.");
				}

				// Load the equipment record
				EquipmentClass equipment =
					FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
						equipments => equipments.Get(this.Security, new Guid(equipmentSelected)));

				string selectedPersonGuid = this.OperatorGridSelection.Value;

				if (string.IsNullOrEmpty(selectedPersonGuid))
				{
					throw new ApplicationException("Could not find selected person.");
				}

				// If everything ok, set the value
				FMChannelHelper.MakeCall<IPersonnel>(
					proxyPersonnel => this.SetPersonToStandbyStatus(proxyPersonnel, equipment, selectedPersonGuid));
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Transfers for equipment edit.
		/// </summary>
		private void TransferForEquipmentEdit()
		{
			// Get the equipment number
			string equipmentGuid = this.EquipmentGridSelection.Value;

			// Transfer for editing
			this.Redirect("../FMWebApp/EquipmentForm.aspx?DispatchEdit=" + equipmentGuid);
		}

		/// <summary>
		///    The transfer for personnel edit.
		/// </summary>
		private void TransferForPersonnelEdit()
		{
			// Get the personnel number
			string personnelGuid = this.OperatorGridSelection.Value;

			// Transfer for editing
			this.Redirect("../FMWebApp/PersonForm.aspx?DispatchEdit=" + personnelGuid);
		}

		/// <summary>
		///    Un-dispatch button click handler.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">
		///    The <see cref="System.EventArgs" /> instance containing the event data.
		/// </param>
		private void UndispatchButtonOnServerClick(object sender, EventArgs e)
		{
			try
			{
				string transactionIds = this.RequestGridSelection.Value;
				List<TransactionDO> transactionArray = this.ParseTransactionIds(transactionIds);

				foreach (TransactionDO transaction in transactionArray)
				{
					this.UndispatchTransaction(transaction);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private TransactionOrigin DetermineOriginApplication()
		{
			return this.IsEnterprise ? TransactionOrigin.DispatchEnterprise : TransactionOrigin.Dispatch;
		}

		#endregion
	}
}