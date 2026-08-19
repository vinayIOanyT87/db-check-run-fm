// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QualityTagAddRecordForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;

namespace FuelsManager.QualityControlWebApp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Web;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using FMCore;
    using FMWebApp;

    using QualityControlWebApp;

    public partial class QualityTagAddRecordForm : FMFormBaseAjax
	{
		#region Constants and Fields

		public static string QualityTagLogSessionKey = "QualityTagAddRecord.QualityTagAddRecordForm.QualityTagLogObject";

		#endregion

		#region Public Properties

		public string DateFormat { get; set; }

		public EquipmentQualityTagLogClass EquipmentQualityTagLog
		{
			get
			{
				var equipmentQualityTagLog = this.Session["QualityTagLogGuid"] as EquipmentQualityTagLogClass;
				return equipmentQualityTagLog;
			}

			set
			{
				this.Session["QualityTagLogGuid"] = value;
			}
		}

		public TankQualityTagLogClass TankQualityTagLog
		{
			get
			{
				var tankQualityTagLog = this.Session["QualityTagLogGuid"] as TankQualityTagLogClass;
				return tankQualityTagLog;
			}

			set
			{
				this.Session["QualityTagLogGuid"] = value;
			}
		}

		#endregion

		#region Properties

		protected string Mode
		{
			get
			{
				return (string)this.ViewState["Mode"];
			}

			set
			{
				this.ViewState["Mode"] = value;
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///     Override to distinguish how the page is being used
		/// </summary>
		/// <returns>Key for lookup into tblHelpMapping</returns>
		public override string GetHelpContextKey()
		{
			return base.GetHelpContextKey() + "|" + this.Mode;
		}

      public override List<string> GetHelpContextKeys()
      {
          List<string> list = new List<string>() { base.GetHelpContextKey() + "|" + this.Mode };
          return list;
      }

        #endregion

        // Get labels' text from m_Dictionaries.
        #region Methods

        protected void ApplyDataDictionary()
		{
			this.AssetTypeLabel.Text = this.GetTranslatedText("Type") + ":";
			this.QualityTagNameLabel.Text = this.GetTranslatedText("Quality Tag") + ":";
			this.MemoLabel.Text = this.GetTranslatedText("Memo") + ":";
		}

		protected void ApplyOrRemoveButtonClick(object sender, EventArgs e)
		{
			try
			{
				this.WriteToDatabase();
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
				return;
			}

			this.Session.Remove(QualityTagLogSessionKey);
			this.Session.Remove("QualityTagLogGuid");
			var returnTo = this.Session["ReturnPageFromQualityTagAddRecordForm"] as string;

			this.Session.Remove("ReturnPageFromQualityTagAddRecordForm");

			// Because this control is in an update panel, we must add the CSRF token manually
			if (this.IsFromQueryWriter)
			{
				this.Redirect("..\\QueryWriterWebApp\\QueryResultsForm.aspx?Mode=Returning");
			}
			else if (returnTo != null)
			{
				this.Session.Remove("ReturnPageFromQualityTagAddRecordForm");

				this.Redirect(returnTo);
			}
			else if (this.Mode == "ADD")
			{
				this.Redirect("QualityTagAddRecordForm.aspx?MODE=ADD");
			}
			else
			{
				this.Redirect("QualityTagLogForm.aspx");
			}
		}

		/// <summary>
		///     Event handler for when the tank select combobox is changed.
		/// </summary>
		protected void AssetIDComboBoxTankSelectedIndexChanged()
		{
			try
			{
				TankQualityTagLogClass tankQualityTagLog =
					FMChannelHelper.MakeCall<ITankQualityTagLogs, TankQualityTagLogClass>(
						logs => logs.GetMostRecentByTankID(this.Security, this.AssetIDComboBox.SelectedItem.Text));

				if (this.Mode == "ADD")
				{
					tankQualityTagLog.IdentityGuid = Guid.Empty;
					tankQualityTagLog.ID = string.Empty;
					tankQualityTagLog.Memo = string.Empty;
					tankQualityTagLog.RemovedBy = string.Empty;
					tankQualityTagLog.RemovedDate = DateTimeOffset.Now;
					tankQualityTagLog.QualityTagName = string.Empty;
					tankQualityTagLog.QualityTagGuid = Guid.Empty;
					tankQualityTagLog.TaggedBy = string.Empty;
					tankQualityTagLog.TaggedDate = DateTimeOffset.Now;
					tankQualityTagLog.TankID = this.AssetIDComboBox.SelectedItem.Text;

					FMChannelHelper.MakeCall<ITanks>(
						tanks =>
							{
								tankQualityTagLog.TankGuid = tanks.GetIdentityGuid(this.Security, tankQualityTagLog.TankID);
								TankClass tank = tanks.Get(this.Security, tankQualityTagLog.TankGuid);

								tankQualityTagLog.VesselType = TankClass.VesselTypeID(tank.VesselType);
							});

					this.ValidTagNumber();
					tankQualityTagLog.TagNumber = Convert.ToInt32(this.TagTextBox.Text);
				}

				this.TankQualityTagLog = tankQualityTagLog;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void AssetIDComboBoxEquipmentSelectedIndexChanged()
		{
			try
			{
				EquipmentQualityTagLogClass equipmentQualityTagLog =
					FMChannelHelper.MakeCall<IEquipmentQualityTagLogs, EquipmentQualityTagLogClass>(
						logs => logs.GetMostRecentByEquipmentID(this.Security, this.AssetIDComboBox.SelectedItem.Text));

				if (this.Mode == "ADD")
				{
					equipmentQualityTagLog.IdentityGuid = Guid.Empty;
					equipmentQualityTagLog.ID = string.Empty;
					equipmentQualityTagLog.Memo = string.Empty;
					equipmentQualityTagLog.RemovedBy = string.Empty;
					equipmentQualityTagLog.RemovedDate = DateTimeOffset.Now;
					equipmentQualityTagLog.QualityTagName = string.Empty;
					equipmentQualityTagLog.QualityTagGuid = Guid.Empty;
					equipmentQualityTagLog.TaggedBy = string.Empty;
					equipmentQualityTagLog.TaggedDate = DateTimeOffset.Now;
					equipmentQualityTagLog.EquipmentID = this.AssetIDComboBox.SelectedItem.Text;

					FMChannelHelper.MakeCall<IEquipments>(
						equipments =>
							{
								equipmentQualityTagLog.EquipmentGuid = equipments.GetIdentityGuid(
									this.Security, equipmentQualityTagLog.EquipmentID);
								EquipmentClass equipment = equipments.Get(this.Security, equipmentQualityTagLog.EquipmentGuid);
								equipmentQualityTagLog.EquipmentType = equipment.EqTypeName;
							});

					this.ValidTagNumber();
					equipmentQualityTagLog.TagNumber = Convert.ToInt32(this.TagTextBox.Text);
				}

				this.EquipmentQualityTagLog = equipmentQualityTagLog;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void AssetIDComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				if (this.AssetTypeDropdown.SelectedItem.Text == "EQUIPMENT")
				{
					this.AssetIDComboBoxEquipmentSelectedIndexChanged();
				}
				else
				{
					this.AssetIDComboBoxTankSelectedIndexChanged();
				}

				this.LoadQualityTags();

				this.AssetTypeTextBox.Text = string.Empty;
				this.QualityTagNameFMCombobox.SelectByText(string.Empty);
				this.MemoTextBox.Text = string.Empty;
				this.TaggedDateValue.Text = string.Empty;
				this.TaggedByValue.Text = string.Empty;
				this.RemovedByValue.Text = string.Empty;
				this.RemovedByValue.Text = string.Empty;

				this.UpdateView();

				this.UpdatePanelQualityTag.Update();
				this.UpdatePanelMemo.Update();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void AssetTypeDropdownSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.AssetIDLabel.Text = this.GetTranslatedText(this.AssetTypeDropdown.SelectedItem.Value);
				if (this.AssetTypeDropdown.SelectedItem.Text == "EQUIPMENT")
				{
					this.EquipmentQualityTagLog = new EquipmentQualityTagLogClass();
				}
				else
				{
					this.TankQualityTagLog = new TankQualityTagLogClass();
				}

				this.LoadEquipmentIdsOrTankIds();
				this.LoadQualityTags();
				this.AssetIDComboBox.SelectByText(string.Empty);
				this.AssetTypeTextBox.Text = string.Empty;
				this.QualityTagNameFMCombobox.SelectByText(string.Empty);
				this.MemoTextBox.Text = string.Empty;
				this.TaggedDateValue.Text = string.Empty;
				this.TaggedByValue.Text = string.Empty;
				this.RemovedByValue.Text = string.Empty;
				this.RemovedByValue.Text = string.Empty;

				this.SetTagNumberTextBox();

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void CancelButtonClick(object sender, EventArgs e)
		{
			this.Session.Remove(QualityTagLogSessionKey);
			var returnTo = this.Session["ReturnPageFromQualityTagAddRecordForm"] as string;
			this.Session.Remove("ReturnPageFromQualityTagAddRecordForm");

			if (this.IsFromQueryWriter)
			{
				this.Redirect("..\\QueryWriterWebApp\\QueryResultsForm.aspx?Mode=Returning");
			}
			else if (this.Mode == "ADD" && returnTo != null)
			{
				this.Redirect(returnTo);
			}
			else
			{
				this.Redirect("QualityTagLogForm.aspx");
			}
		}

		/// <summary>
		/// Raises the <see cref="OnInit"/> event.
		/// </summary>
		/// <param name="e">
		/// The <see cref="EventArgs"/> instance containing the event data.
		/// </param>
		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.GetSecurity();

			// Want to ignore the disabling of inputs on post backs.
			this.IgnoreInputDisable = true;
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				var site =
					FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

				this.DateFormat = site.ShortDatePattern + " " + site.TimePattern;

				// This is the first time through.
				if (!this.IsPostBack)
				{
					this.Title = this.GetTranslatedText(this.Title);
					this.MemoTextBox_TextBoxWatermarkExtender.WatermarkText =
						this.GetTranslatedText(this.MemoTextBox_TextBoxWatermarkExtender.WatermarkText);

					this.ResetControls();
					if (string.IsNullOrEmpty(this.Request.GetQueryOrFormValue("QUERYEDIT")) == false)
					{
						Guid entityIdentity = Guid.Parse(this.Request.GetQueryOrFormValue("QUERYEDIT").Substring(1));
						char entityType = this.Request.GetQueryOrFormValue("QUERYEDIT")[0];

						if (entityType == 'E')
						{
							this.EquipmentQualityTagLog = FMChannelHelper.MakeCall<IEquipmentQualityTagLogs, EquipmentQualityTagLogClass>
								(x => x.Get(this.Security, entityIdentity));
						}
						else
						{
							this.TankQualityTagLog = FMChannelHelper.MakeCall<ITankQualityTagLogs,TankQualityTagLogClass>
								(x => x.Get(this.Security, entityIdentity));
						}
					}

					this.Mode = this.Request.GetQueryOrFormValue("MODE");
					if (this.IsFromQueryWriter)
					{
						this.Mode = "VIEW";
					}

					if (this.Mode == "ADD")
					{
						this.Session.Remove("QualityTagLogGuid");
					}

					if (this.EquipmentQualityTagLog == null && this.TankQualityTagLog == null)
					{
						this.EquipmentQualityTagLog = new EquipmentQualityTagLogClass();
					}

					// Load the controls that use the View State.
					this.LoadAssetTypes();

					// If an AJAX control, also check for callback.
					if (!this.IsCallback)
					{
						this.LoadEquipmentIdsOrTankIds();
					}

					this.LoadQualityTags();

					this.SetTagNumberTextBox();

					this.UpdateView();
				}

				this.ApplyOrRemoveButton.Attributes.Add("onClick", string.Empty);
				if (this.AssetTypeDropdown.SelectedItem.Text == "EQUIPMENT")
				{
					this.SetConfirmationForEquipment();
				}
				else
				{
					this.SetConfirmationForTank();
				}

				this.UpdatePanelOKButton.Update();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void QualityTagNameFMComboboxOnSelectedIndexChanged(object sender, EventArgs e)
		{
			//this is here for postback
		}

		// Used for coming from Equipment QualityTag Log Form.
		protected void ResetControls()
		{
			// Clear controls' text. They want the AssetType combobox to have a value.
			this.AssetTypeDropdown.SelectByText("EQUIPMENT");
			this.AssetIDComboBox.SelectByText(string.Empty);
			this.AssetTypeTextBox.Text = string.Empty;
			this.QualityTagNameFMCombobox.SelectByText(string.Empty);
			this.MemoTextBox.Text = string.Empty;
			this.TaggedDateValue.Text = string.Empty;
			this.TaggedByValue.Text = string.Empty;
			this.RemovedByValue.Text = string.Empty;
			this.RemovedByValue.Text = string.Empty;
			this.TagTextBox.Text = string.Empty;
		}

		// Saving from controls to objects, thence to database.
		protected void WriteEquipmentQualityTagLogToDatabase(bool createMaintenanceRecord)
		{
			EquipmentQualityTagLogClass oEquipmentQualityTagLog = this.EquipmentQualityTagLog;
			Debug.Assert(null != oEquipmentQualityTagLog);

			ListItem li = this.AssetIDComboBox.SelectedItem;

			EquipmentClass equipment = null;

			FMChannelHelper.MakeCall<IEquipments>(
				equipments =>
					{
						Guid equipmentGuid = equipments.GetIdentityGuid(this.Security, li.Text);
						equipment = equipments.Get(this.Security, equipmentGuid);
					});

			oEquipmentQualityTagLog.EquipmentID = li.Text;
			oEquipmentQualityTagLog.EquipmentType = equipment.EqTypeName;

			if (this.Mode == "ADD") // if mode is edit then do not overwrite the orginal tagged date. 
			{
				oEquipmentQualityTagLog.TaggedDate = DateTimeOffset.Now;
				oEquipmentQualityTagLog.TaggedBy = this.Security.UserID;
			}

			oEquipmentQualityTagLog.Memo = this.MemoTextBox.Text;

			this.ValidTagNumber();

			oEquipmentQualityTagLog.TagNumber = Convert.ToInt32(this.TagTextBox.Text);

			oEquipmentQualityTagLog.QualityTagGuid = Guid.Parse(this.QualityTagNameFMCombobox.SelectedItem.Value);
			oEquipmentQualityTagLog.QualityTagName = this.QualityTagNameFMCombobox.SelectedItem.Text;

			if (string.IsNullOrEmpty(oEquipmentQualityTagLog.EquipmentID))
			{
				throw new ApplicationException("Equipment ID required.");
			}

			if (string.IsNullOrEmpty(oEquipmentQualityTagLog.QualityTagName))
			{
				throw new ApplicationException("Quality Tag required.");
			}

			if (this.Mode == "EDIT")
			{
				FMChannelHelper.MakeCall<IEquipmentQualityTagLogs>(logs => logs.Modify(this.Security, oEquipmentQualityTagLog));
				if (createMaintenanceRecord)
				{
					EquipmentMaintenanceLogClass equipmentMaintenanceLog =
						FMChannelHelper.MakeCall<IEquipmentMaintenanceLogs, EquipmentMaintenanceLogClass>(
							eLogs => eLogs.GetByEquipmentGuid(this.Security, oEquipmentQualityTagLog.EquipmentGuid));

					if (equipmentMaintenanceLog != null
					    && (equipmentMaintenanceLog.IdentityGuid != Guid.Empty && equipmentMaintenanceLog.InServiceFlag == 0))
					{
						equipmentMaintenanceLog.InServiceFlag = 1;
						this.Session.Remove(QualityTagLogSessionKey);
						this.Session["MaintenanceAddRecord.MaintenanceAddRecordForm.MaintenanceLog"] = equipmentMaintenanceLog;
						this.Session["EQUIPMENT_SESSION_KEY"] = equipment;
						this.Session["ReturnPageFromMaintenanceAddRecordForm"] = "../QualityControlWebApp/QualityTagLogForm.aspx";
						this.Session["ReturnPageFromQualityTagAddRecordForm"] =
							"../MaintenanceWebApp/MaintenanceAddRecordForm.aspx?MODE=ADD";
					}
				}
			}
			else if (this.Mode == "ADD")
			{
				try
				{
					QualityTagClass qualityTag = FMChannelHelper.MakeCall<IQualityTags, QualityTagClass>(
						tags => tags.Get(this.Security, oEquipmentQualityTagLog.QualityTagGuid));

					if (!createMaintenanceRecord)
					{
						createMaintenanceRecord = qualityTag.Severity == QUALITY_SEVERITY_LEVELS.DANGER;
					}

					if (equipment.InServiceFlag == false && createMaintenanceRecord)
					{
						createMaintenanceRecord = false;
					}
				}
					// ReSharper disable once EmptyGeneralCatchClause
				catch
				{
				}

				FMChannelHelper.MakeCall<IEquipmentQualityTagLogs>(logs => logs.Add(this.Security, oEquipmentQualityTagLog));
				if (createMaintenanceRecord)
				{
					// create maintenance record
					EquipmentMaintenanceLogClass equipmentMaintenanceLog =
						FMChannelHelper.MakeCall<IEquipmentMaintenanceLogs, EquipmentMaintenanceLogClass>(
							eLogs => eLogs.GetByEquipmentGuid(this.Security, oEquipmentQualityTagLog.EquipmentGuid));

					if (equipmentMaintenanceLog == null)
					{
						equipmentMaintenanceLog = new EquipmentMaintenanceLogClass
						                          {
							                          EquipmentID = oEquipmentQualityTagLog.EquipmentID,
							                          EquipmentGuid = oEquipmentQualityTagLog.EquipmentGuid,
							                          EquipmentType = oEquipmentQualityTagLog.EquipmentType
						                          };
					}

					if (equipmentMaintenanceLog.IdentityGuid == Guid.Empty || equipmentMaintenanceLog.InServiceFlag == 1)
					{
						MaintenanceReasonClass maintenanceReason = null;

						FMChannelHelper.MakeCall<IMaintenanceReasons>(
							reasons =>
								{
									Guid identityGuid = reasons.GetIdentityGuid(this.Security, "QUALITY_TAG_ASSIGNEMENT");
									if (identityGuid == Guid.Empty)
									{
										maintenanceReason = new MaintenanceReasonClass
										                    {
											                    CreatedBy = this.Security.UserID,
											                    UpdatedBy = this.Security.UserID,
											                    CreatedDate = DateTimeOffset.Now,
											                    Description = "Triggered by Quality Tag assignment.",
											                    ID = "QUALITY_TAG_ASSIGNEMENT",
											                    SiteGuid = Guids.SiteAdminGuid,
											                    SiteID = "SiteAdmin"
										                    };
										maintenanceReason.IdentityGuid = reasons.Add(this.Security, maintenanceReason);
										maintenanceReason.UpdatedDate = maintenanceReason.CreatedDate;
									}
									else
									{
										maintenanceReason = reasons.Get(this.Security, identityGuid);
									}
								});

						equipmentMaintenanceLog.InServiceFlag = 0;
						equipmentMaintenanceLog.MaintenanceReasonGuid = maintenanceReason.IdentityGuid;
						equipmentMaintenanceLog.MaintenanceReason = maintenanceReason.Description;
						equipmentMaintenanceLog.EstReturnToServiceDate = DateTimeOffset.Now;
						equipmentMaintenanceLog.Memo = string.Empty;

						this.Session.Remove(QualityTagLogSessionKey);
						this.Session["MaintenanceAddRecord.MaintenanceAddRecordForm.MaintenanceLog"] = equipmentMaintenanceLog;
						this.Session["EQUIPMENT_SESSION_KEY"] = equipment;
						if (this.Session["ReturnPageFromQualityTagAddRecordForm"] != null)
						{
							this.Session["ReturnPageFromMaintenanceAddRecordForm"] = this.Session["ReturnPageFromQualityTagAddRecordForm"];
							this.Session.Remove("ReturnPageFromQualityTagAddRecordForm");
						}
						else
						{
							this.Session["ReturnPageFromMaintenanceAddRecordForm"] =
								"../QualityControlWebApp/QualityTagAddRecordForm.aspx?MODE=ADD";
						}

						this.Session.Remove("QualityTagLogGuid");
						this.Session["ReturnPageFromQualityTagAddRecordForm"] =
							"../MaintenanceWebApp/MaintenanceAddRecordForm.aspx?MODE=ADD";
					}
				}
			}
		}

		protected void WriteTankQualityTagLogToDatabase(bool createMaintenanceRecord)
		{
			TankQualityTagLogClass oTankQualityTagLog = this.TankQualityTagLog;
			Debug.Assert(null != oTankQualityTagLog);

			ListItem li = this.AssetIDComboBox.SelectedItem;

			TankClass tank = null;
			FMChannelHelper.MakeCall<ITanks>(
				tanks =>
					{
						Guid eqGuid = tanks.GetIdentityGuid(this.Security, li.Text);
						tank = tanks.Get(this.Security, eqGuid);
					});

			oTankQualityTagLog.TankID = li.Text;
			oTankQualityTagLog.VesselType = TankClass.VesselTypeID(tank.VesselType);
			oTankQualityTagLog.TaggedDate = DateTimeOffset.Now;
			oTankQualityTagLog.TaggedBy = this.Security.UserID;
			oTankQualityTagLog.Memo = this.MemoTextBox.Text;

			oTankQualityTagLog.QualityTagGuid = Guid.Parse(this.QualityTagNameFMCombobox.SelectedItem.Value);
			oTankQualityTagLog.QualityTagName = this.QualityTagNameFMCombobox.SelectedItem.Text;

			if (string.IsNullOrEmpty(oTankQualityTagLog.TankID))
			{
				throw new ApplicationException("Tank ID required.");
			}

			if (string.IsNullOrEmpty(oTankQualityTagLog.QualityTagName))
			{
				throw new ApplicationException("Quality Tag required.");
			}

			if (this.Mode == "EDIT")
			{
				FMChannelHelper.MakeCall<ITankQualityTagLogs>(logs => logs.Modify(this.Security, oTankQualityTagLog));
				if (createMaintenanceRecord)
				{
					TankMaintenanceLogClass tankMaintenanceLog = FMChannelHelper.MakeCall<ITankMaintenanceLogs, TankMaintenanceLogClass>(
						tankMaintenanceLogs => tankMaintenanceLogs.GetByTankGuid(this.Security, oTankQualityTagLog.TankGuid));

					if (tankMaintenanceLog != null
					    && tankMaintenanceLog.IdentityGuid != Guid.Empty 
						&& tankMaintenanceLog.InServiceFlag == 0)
					{
						tankMaintenanceLog.InServiceFlag = 1;
						this.Session.Remove(QualityTagLogSessionKey);
						this.Session["MaintenanceAddRecord.MaintenanceAddRecordForm.MaintenanceLog"] = tankMaintenanceLog;
						this.Session["Tank_SESSION_KEY"] = tank;
						this.Session["ReturnPageFromMaintenanceAddRecordForm"] = "../QualityControlWebApp/QualityTagLogForm.aspx";
						this.Session["ReturnPageFromQualityTagAddRecordForm"] =
							"../MaintenanceWebApp/MaintenanceAddRecordForm.aspx?MODE=ADD";
					}
				}
			}
			else if (this.Mode == "ADD")
			{
				try
				{
					QualityTagClass qualityTag =
						FMChannelHelper.MakeCall<IQualityTags, QualityTagClass>(
							tags => tags.Get(this.Security, oTankQualityTagLog.QualityTagGuid));

					if (!createMaintenanceRecord)
					{
						createMaintenanceRecord = qualityTag.Severity == QUALITY_SEVERITY_LEVELS.DANGER;
					}

					if (tank.InServiceFlag == false && createMaintenanceRecord)
					{
						createMaintenanceRecord = false;
					}
				}
					// ReSharper disable once EmptyGeneralCatchClause
				catch
				{
				}

				FMChannelHelper.MakeCall<ITankQualityTagLogs>(logs => logs.Add(this.Security, oTankQualityTagLog));

				if (createMaintenanceRecord)
				{
					// Create maintenance record
					TankMaintenanceLogClass tankMaintenanceLog =
						FMChannelHelper.MakeCall<ITankMaintenanceLogs, TankMaintenanceLogClass>(
							maintLogs => maintLogs.GetByTankGuid(this.Security, oTankQualityTagLog.TankGuid));

					if (tankMaintenanceLog == null)
					{
						tankMaintenanceLog = new TankMaintenanceLogClass
							{
								TankID = oTankQualityTagLog.TankID, 
								TankGuid = oTankQualityTagLog.TankGuid, 
								VesselType = oTankQualityTagLog.VesselType
							};
					}

					if (tankMaintenanceLog.IdentityGuid == Guid.Empty || tankMaintenanceLog.InServiceFlag == 1)
					{
						MaintenanceReasonClass maintenanceReason = null;
						FMChannelHelper.MakeCall<IMaintenanceReasons>(
							reasons =>
								{
									Guid identityGuid = reasons.GetIdentityGuid(this.Security, MaintenanceReasonClass.QUALITY_TAG_ASSIGNMENT);
									if (identityGuid == Guid.Empty)
									{
										maintenanceReason = new MaintenanceReasonClass
										                    {
											                    CreatedBy = this.Security.UserID,
											                    UpdatedBy = this.Security.UserID,
											                    CreatedDate = DateTimeOffset.Now,
											                    Description = MaintenanceReasonClass.QUALITY_TAG_DESCRIPTION,
											                    ID = MaintenanceReasonClass.QUALITY_TAG_ASSIGNMENT,
											                    SiteGuid = Guids.SiteAdminGuid,
											                    SiteID = "SiteAdmin"
										                    };

										maintenanceReason.UpdatedDate = maintenanceReason.CreatedDate;
										maintenanceReason.IdentityGuid = reasons.Add(this.Security, maintenanceReason);
									}
									else
									{
										maintenanceReason = reasons.Get(this.Security, identityGuid);
									}
								});

						tankMaintenanceLog.InServiceFlag = 0;
						tankMaintenanceLog.MaintenanceReasonGuid = maintenanceReason.IdentityGuid;
						tankMaintenanceLog.MaintenanceReason = maintenanceReason.Description;
						tankMaintenanceLog.EstReturnToServiceDate = DateTimeOffset.Now;
						tankMaintenanceLog.Memo = string.Empty;

						this.Session.Remove(QualityTagLogSessionKey);
						this.Session["MaintenanceAddRecord.MaintenanceAddRecordForm.MaintenanceLog"] = tankMaintenanceLog;

						if (this.Session["ReturnPageFromQualityTagAddRecordForm"] != null)
						{
							this.Session["ReturnPageFromMaintenanceAddRecordForm"] = this.Session["ReturnPageFromQualityTagAddRecordForm"];
							this.Session.Remove("ReturnPageFromQualityTagAddRecordForm");
						}
						else
						{
							this.Session["ReturnPageFromMaintenanceAddRecordForm"] =
								"../QualityControlWebApp/QualityTagAddRecordForm.aspx?MODE=ADD";
						}

						this.Session.Remove("QualityTagLogGuid");
						this.Session["ReturnPageFromQualityTagAddRecordForm"] =
							"../MaintenanceWebApp/MaintenanceAddRecordForm.aspx?MODE=ADD";
					}
				}
			}
		}

		protected void WriteToDatabase()
		{
			string sAssetTypeSelected = this.AssetTypeDropdown.SelectedItem.Text;
			string confirm = string.Empty;
			string confirmEvent = this.Request.GetQueryOrFormValue("__MYEVENTTARGET");
			if (confirmEvent == "Confirm")
			{
				confirm = this.Request.GetQueryOrFormValue("__MYEVENTARGUMENT");
			}

			bool createMaintenanceRecord = confirm == "OK";

			if (sAssetTypeSelected == "EQUIPMENT")
			{
				this.WriteEquipmentQualityTagLogToDatabase(createMaintenanceRecord);
			}
			else
			{
				this.WriteTankQualityTagLogToDatabase(createMaintenanceRecord);
			}
		}

		protected void LoadAssetTypes()
		{
			var assetTypes = new ArrayList { new ListItem("EQUIPMENT", this.GetTranslatedText("Equipment ID") + ":"), new ListItem("TANK", this.GetTranslatedText("Tank ID") + ":") };

			this.AssetTypeDropdown.Items.Clear();
			this.AssetTypeDropdown.DataSource = assetTypes;
			this.AssetTypeDropdown.DataTextField = "Text";
			this.AssetTypeDropdown.DataValueField = "Value";
			this.AssetTypeDropdown.DataBind();

			if (this.TankQualityTagLog == null)
			{
				this.AssetTypeDropdown.SelectByText("EQUIPMENT");
			}
			else
			{
				this.AssetTypeDropdown.SelectByText("TANK");
			}
		}

		// Changed from "EQUIPMENT" to "TANK", or vv.
		protected void LoadEquipmentIds()
		{
			this.AssetIDComboBox.Clear();

			if (this.Mode == "ADD")
			{
				EquipmentCollectionClass equipmentCollectionClass =
					FMChannelHelper.MakeCall<IEquipments, EquipmentCollectionClass>(
						equipments => equipments.EnumerateManagedEquipmentWithoutQualityTag(this.Security));

				var li = new ListItem(string.Empty, string.Empty);
				this.AssetIDComboBox.Items.Add(li);

				// And add them to the droplist.
				foreach (EquipmentClass equipment in equipmentCollectionClass)
				{
					li = new ListItem(equipment.ID, equipment.MasterRecordGuid.ToString());
					this.AssetIDComboBox.Items.Add(li);
				}
			}
			else
			{
				EquipmentClass equipment = null;

				FMChannelHelper.MakeCall<IEquipments>(
					equipments =>
						{
							Guid equipmentGuid = equipments.GetIdentityGuid(this.Security, this.EquipmentQualityTagLog.EquipmentID);
							equipment = equipments.Get(this.Security, equipmentGuid);
						});

				ListItem li;
				if (equipment.IdentityGuid == Guid.Empty)
				{
					li = new ListItem(this.EquipmentQualityTagLog.EquipmentID, Guid.Empty.ToString());
				}
				else
				{
					li = new ListItem(equipment.ID, equipment.MasterRecordGuid.ToString());
				}

				this.AssetIDComboBox.Items.Add(li);
			}

			this.AssetIDComboBox.SelectByText(this.EquipmentQualityTagLog.EquipmentID);
		}

		protected void LoadEquipmentIdsOrTankIds()
		{
			// Create the collection of Equipments or Tanks.
			if (this.AssetTypeDropdown.SelectedItem.Text == "EQUIPMENT")
			{
				this.LoadEquipmentIds();
			}
			else
			{
				this.LoadTankIds();
			}
		}

		// Load the combobox from the database table tblQualityTagReasons.
		protected void LoadQualityTags()
		{
			QualityTagCollectionClass qualityTagCollectionClass =
				FMChannelHelper.MakeCall<IQualityTags, QualityTagCollectionClass>(
					qualityTags => qualityTags.Enumerate(this.Security, null, null, false));

			// And add them to the droplist.
			this.QualityTagNameFMCombobox.Clear();
			var li = new ListItem(string.Empty, Guid.Empty.ToString());
			this.QualityTagNameFMCombobox.Items.Add(li);

			foreach (QualityTagClass qualityTagReason in qualityTagCollectionClass)
			{
				if (qualityTagReason.Active)
				{
					li = new ListItem(qualityTagReason.ID, qualityTagReason.IdentityGuid.ToString());
					this.QualityTagNameFMCombobox.Items.Add(li);
				}
			}

			this.QualityTagNameFMCombobox.SelectByText(string.Empty);
		}

		protected void LoadTaggedAndRemovedValues()
		{
			// Create the collection of Equipments or Tanks.
			if (this.AssetTypeDropdown.SelectedItem.Text == "EQUIPMENT")
			{
				EquipmentQualityTagLogClass equipmentQualityTagLog = this.EquipmentQualityTagLog;

				this.TaggedByValue.Text = equipmentQualityTagLog.TaggedBy;
				this.TaggedDateValue.Text = string.Empty;

				if (string.IsNullOrEmpty(equipmentQualityTagLog.TaggedBy) == false && 
					equipmentQualityTagLog.TaggedDate != null)
				{
					this.TaggedDateValue.Text = equipmentQualityTagLog.TaggedDate.Value.ToString(this.DateFormat);
				}

				this.RemovedByValue.Text = equipmentQualityTagLog.RemovedBy;
				this.RemovedDateValue.Text = string.Empty;

				if (string.IsNullOrEmpty(equipmentQualityTagLog.RemovedBy) == false &&
					equipmentQualityTagLog.RemovedDate != null)
				{
					this.RemovedDateValue.Text = equipmentQualityTagLog.RemovedDate.Value.ToString(this.DateFormat);
				}
			}
			else
			{
				TankQualityTagLogClass tankQualityTagLog = this.TankQualityTagLog;

				this.TaggedDateValue.Text = string.IsNullOrEmpty(tankQualityTagLog.TaggedBy)
					                            ? string.Empty
					                            : tankQualityTagLog.TaggedDate.ToString(this.DateFormat);
				this.TaggedByValue.Text = tankQualityTagLog.TaggedBy;

				this.RemovedDateValue.Text = string.IsNullOrEmpty(tankQualityTagLog.RemovedBy)
					                             ? string.Empty
					                             : tankQualityTagLog.RemovedDate.ToString(this.DateFormat);
				this.RemovedByValue.Text = tankQualityTagLog.RemovedBy;
			}
		}

		protected void LoadTankIds()
		{
			this.AssetIDComboBox.Clear();

			if (this.Mode == "ADD")
			{
				TankCollectionClass tankCollectionClass =
					FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(
						tanks => tanks.EnumerateTanksWithoutQualityTag(this.Security));

				var li = new ListItem(string.Empty, string.Empty);
				this.AssetIDComboBox.Items.Add(li);

				// And add them to the droplist.
				foreach (TankClass tank in tankCollectionClass)
				{
					li = new ListItem(tank.ID, tank.IdentityGuid.ToString());
					this.AssetIDComboBox.Items.Add(li);
				}
			}
			else
			{
				TankClass tank = null;

				FMChannelHelper.MakeCall<ITanks>(
					tanks =>
						{
							Guid identityGuid = tanks.GetIdentityGuid(this.Security, this.TankQualityTagLog.TankID);
							tank = tanks.Get(this.Security, identityGuid);
						});

				ListItem li;
				if (tank.IdentityGuid == Guid.Empty)
				{
					li = new ListItem(this.TankQualityTagLog.TankID, Guid.Empty.ToString());
				}
				else
				{
					li = new ListItem(tank.ID, tank.IdentityGuid.ToString());
				}

				this.AssetIDComboBox.Items.Add(li);
			}

			this.AssetIDComboBox.SelectByText(this.TankQualityTagLog.TankID);
		}

		private int GetNextSampleNumber(int previousSampleNumber, DateTimeOffset? previousDate)
		{
			int samplenumber = previousSampleNumber;

			DateTimeOffset currentdate = DateTimeOffset.Now;
			int year = currentdate.Year;

			year = year % 100;

			if (previousDate != null)
			{
				// If current year is greater than the year of the previous date, reset 
				// the sample number to the default value
				if ((currentdate.Year > previousDate.Value.Year)
				    && ((previousSampleNumber / TestSetResultGeneralPage.SampleNumMultiplier) != year))
				{
					samplenumber = (year * TestSetResultGeneralPage.SampleNumMultiplier) + 1;
				}
				else
				{
					samplenumber++;
				}

				if (samplenumber <= previousSampleNumber)
				{
					samplenumber = previousSampleNumber + 1;
				}
			}

			return samplenumber;
		}

		private void SetConfirmationForEquipment()
		{
			EquipmentQualityTagLogClass oEquipmentQualityTagLog = this.EquipmentQualityTagLog;
			if (oEquipmentQualityTagLog != null && oEquipmentQualityTagLog.EquipmentGuid != Guid.Empty)
			{
				EquipmentClass equipment =
					FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
						x => x.Get(this.Security, oEquipmentQualityTagLog.EquipmentGuid));

				if (this.Mode == "ADD")
				{
					try
					{
						QualityTagClass qualityTag =
							FMChannelHelper.MakeCall<IQualityTags, QualityTagClass>(
								x => x.Get(this.Security, Guid.Parse(this.QualityTagNameFMCombobox.SelectedItem.Value)));

						if (qualityTag.Severity != QUALITY_SEVERITY_LEVELS.DANGER && equipment.InServiceFlag)
						{
							string sEquipmentID = this.AssetIDComboBox.SelectedItem.Text;
							string confirmText =
								HttpUtility.JavaScriptStringEncode("Do you want to take equipment " + sEquipmentID + " out of service?\r\nOK will put the item out of service.\r\nCancel will leave it in service.");

							this.ApplyOrRemoveButton.Attributes.Add(
							"onClick", 
								"if ( confirm(\"" + confirmText
								+ "\")) __mydoPostBack('Confirm', 'OK');else __mydoPostBack('Confirm', 'Cancel');");
						}
					}
						// ReSharper disable once EmptyGeneralCatchClause
					catch
					{
					}
				}
				else if (this.Mode == "EDIT")
				{
					if (equipment.InServiceFlag == false)
					{
						string confirmText;
						string sEquipmentID = this.AssetIDComboBox.SelectedItem.Text;

						EquipmentMaintenanceLogClass equipmentMaintenanceLog =
							FMChannelHelper.MakeCall<IEquipmentMaintenanceLogs, EquipmentMaintenanceLogClass>(
								logs => logs.GetByEquipmentGuid(this.Security, equipment.MasterRecordGuid));

						MaintenanceReasonClass reason = null;

						if (equipmentMaintenanceLog != null)
						{
							reason =
								FMChannelHelper.MakeCall<IMaintenanceReasons, MaintenanceReasonClass>(
									reasons => reasons.Get(this.Security, equipmentMaintenanceLog.MaintenanceReasonGuid));
						}

						if (reason != null && reason.ID.Equals(MaintenanceReasonClass.QUALITY_TAG_ASSIGNMENT))
						{
							confirmText =
								HttpUtility.JavaScriptStringEncode("Do you want to put the equipment " + sEquipmentID + " back into service?\r\nOK will put the item in service.\r\nCancel will leave it out of service.");
						}
						else
						{
							confirmText =
								HttpUtility.JavaScriptStringEncode(
									string.Format(
										"Equipment {0} put out of service for maintenance.  Would you like to override and place the equipment back into service?\r\nOK will put the item in service.\r\nCancel will leave it out of service.",  
										sEquipmentID));
						}

						this.ApplyOrRemoveButton.Attributes.Add(
							"onClick", 
							"if ( confirm(\"" + confirmText
							+ "\")) __mydoPostBack('Confirm', 'OK');else __mydoPostBack('Confirm', 'Cancel');");
					}
				}
			}
		}

		private void SetConfirmationForTank()
		{
			TankQualityTagLogClass oTankQualityTagLog = this.TankQualityTagLog;
			if (oTankQualityTagLog != null && oTankQualityTagLog.TankGuid != Guid.Empty)
			{
				TankClass tank =
					FMChannelHelper.MakeCall<ITanks, TankClass>(tanks => tanks.Get(this.Security, oTankQualityTagLog.TankGuid));

				if (this.Mode == "ADD")
				{
					try
					{
						QualityTagClass qualityTag =
							FMChannelHelper.MakeCall<IQualityTags, QualityTagClass>(
								qualityTags => qualityTags.Get(this.Security, Guid.Parse(this.QualityTagNameFMCombobox.SelectedItem.Value)));

						if (qualityTag.Severity != QUALITY_SEVERITY_LEVELS.DANGER && tank.InServiceFlag)
						{
							string sTankID = this.AssetIDComboBox.SelectedItem.Text;
							string confirmText =
								HttpUtility.JavaScriptStringEncode("Do you want to take tank " + sTankID + " out of service?\r\nOK will put the item out of service.\r\nCancel will leave it in service.");
							this.ApplyOrRemoveButton.Attributes.Add(
								"onClick", 
								"if ( confirm(\"" + confirmText
								+ "\")) __mydoPostBack('Confirm', 'OK');else __mydoPostBack('Confirm', 'Cancel');");
						}
					}
						// ReSharper disable once EmptyGeneralCatchClause
					catch
					{
					}
				}
				else if (this.Mode == "EDIT")
				{
					if (tank.InServiceFlag == false)
					{
						string sTankID = this.AssetIDComboBox.SelectedItem.Text;
						string confirmText =
							HttpUtility.JavaScriptStringEncode("Do you want to put the tank " + sTankID + " back into service?\r\nOK will put the item out of service.\r\nCancel will leave it in service.");
						this.ApplyOrRemoveButton.Attributes.Add(
							"onClick", 
							"if ( confirm(\"" + confirmText
							+ "\")) __mydoPostBack('Confirm', 'OK');else __mydoPostBack('Confirm', 'Cancel');");
					}
				}
			}
		}

		private void SetTagNumberTextBox()
		{
			int sampleNumber = 0;

			if (this.Mode == "ADD")
			{
				int previousSampleNumber;
				DateTimeOffset? taggedDate = null;

				if (this.AssetTypeDropdown.SelectedItem.Text.Equals("EQUIPMENT"))
				{
					EquipmentQualityTagLogClass log =
						FMChannelHelper.MakeCall<IEquipmentQualityTagLogs, EquipmentQualityTagLogClass>(
							x => x.GetPreviousTagNumber(this.Security));

					previousSampleNumber = log.TagNumber;

					if (log.TaggedDate != null)
					{
						taggedDate = log.TaggedDate.Value;
					}
				}
				else
				{
					// Tank
					TankQualityTagLogClass log =
						FMChannelHelper.MakeCall<ITankQualityTagLogs, TankQualityTagLogClass>(x => x.GetPreviousTagNumber(this.Security));

					previousSampleNumber = log.TagNumber;
					taggedDate = log.TaggedDate;
				}

				if (previousSampleNumber == 0)
				{
					DateTimeOffset currentdate = DateTimeOffset.Now;
					int year = currentdate.Year;
					year = year % 100;
					sampleNumber = (year * TestSetResultGeneralPage.SampleNumMultiplier) + 1;
				}
				else
				{
					sampleNumber = this.GetNextSampleNumber(previousSampleNumber, taggedDate);
				}
			}

			if (this.AssetTypeDropdown.SelectedItem.Text.Equals("EQUIPMENT"))
			{
				if (this.Mode.Equals("ADD"))
				{
					this.EquipmentQualityTagLog.TagNumber = sampleNumber;
				}
				else
				{
					sampleNumber = this.EquipmentQualityTagLog.TagNumber;
				}
			}
			else
			{
				if (this.Mode.Equals("ADD"))
				{
					this.TankQualityTagLog.TagNumber = sampleNumber;
				}
				else
				{
					sampleNumber = this.TankQualityTagLog.TagNumber;
				}
			}

			this.TagTextBox.Text = sampleNumber.ToString(CultureInfo.InvariantCulture);
		}

		private void UpdateView()
		{
			if (this.AssetTypeDropdown.SelectedItem.Text == "EQUIPMENT")
			{
				this.PopulateControlsForEquipment();
			}
			else
			{
				this.PopulateControlsForTank();
			}

			this.LoadTaggedAndRemovedValues();

			if (this.Security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD) || this.Security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD))
			{
				// Set controls' enable states.
				this.AssetTypeDropdown.Enabled = this.Mode == "ADD";
				this.AssetIDComboBox.Enabled = this.Mode == "ADD";
				this.QualityTagNameFMCombobox.Enabled = this.Mode == "ADD";
				this.TagTextBox.Enabled = this.Mode == "ADD";

				this.MemoTextBox.Enabled = this.Mode == "ADD";
				this.ApplyOrRemoveButton.Enabled = (this.Mode == "ADD") && this.Security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD);
				this.ApplyOrRemoveButton.Text = this.Mode == "ADD" ? this.GetTranslatedText("Apply") : this.GetTranslatedText("Remove");
				this.TitleLabel.Text = "View QC Tag";
				this.AssetIDLabel.Text = this.AssetTypeDropdown.SelectedValue;

				if (this.Mode == "ADD")
				{
					this.TitleLabel.Text = "Assign QC Tag";
				}
				else if (this.Mode == "EDIT")
				{
					if (this.AssetTypeDropdown.SelectedItem.Text == "EQUIPMENT")
					{
						Guid equipmentGuid =
							FMChannelHelper.MakeCall<IEquipments, Guid>(
								equipments => equipments.GetIdentityGuid(this.Security, this.EquipmentQualityTagLog.EquipmentID));

						if (equipmentGuid == Guid.Empty)
						{
							// deleted equipment. Disable controls.
							this.Mode = "VIEW";
							this.AssetTypeDropdown.Enabled = false;
							this.AssetIDComboBox.Enabled = false;
							this.QualityTagNameFMCombobox.Enabled = false;
							this.ApplyOrRemoveButton.Enabled = false;
							this.MemoTextBox.Enabled = false;
							this.TagTextBox.Enabled = false;
							this.TitleLabel.Text = "View QC Tag";
						}
						else if (string.IsNullOrEmpty(this.EquipmentQualityTagLog.RemovedBy))
						{
							this.ApplyOrRemoveButton.Enabled = this.Security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD);
							this.MemoTextBox.Enabled = true;
							this.TitleLabel.Text = "Remove QC Tag";
						}
					}
					else
					{
						Guid identityGuid =
							FMChannelHelper.MakeCall<ITanks, Guid>(x => x.GetIdentityGuid(this.Security, this.TankQualityTagLog.TankID));

						if (identityGuid == Guid.Empty)
						{
							// deleted equipment. Disable controls.
							this.Mode = "VIEW";
							this.AssetTypeDropdown.Enabled = false;
							this.AssetIDComboBox.Enabled = false;
							this.QualityTagNameFMCombobox.Enabled = false;
							this.ApplyOrRemoveButton.Enabled = false;
							this.MemoTextBox.Enabled = false;
							this.TagTextBox.Enabled = false;
							this.TitleLabel.Text = "View QC Tag";
						}
						else if (string.IsNullOrEmpty(this.TankQualityTagLog.RemovedBy))
						{
							this.ApplyOrRemoveButton.Enabled = this.Security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD);
							this.MemoTextBox.Enabled = true;
							this.TitleLabel.Text = "Remove QC Tag";
						}
					}
				}
			}
			else
			{
				this.Mode = "VIEW";
				this.AssetTypeDropdown.Enabled = false;
				this.AssetIDComboBox.Enabled = false;
				this.QualityTagNameFMCombobox.Enabled = false;
				this.ApplyOrRemoveButton.Enabled = false;
				this.MemoTextBox.Enabled = false;
				this.TagTextBox.Enabled = false;
				this.TitleLabel.Text = "View QC Tag";
			}

			this.ApplyDataDictionary();
		}

		private void ValidTagNumber()
		{
			// sample number cannot be empty
			if (string.Empty == this.TagTextBox.Text)
			{
				this.TagTextBox.Focus();
				string message = this.GetTranslatedText("The following field is required:") + " "
				                 + this.GetTranslatedText("Tag Number") + "!";
				throw new ApplicationException(message);
			}

			// sample number must be an integer
			int samplenumber;
			if (false == int.TryParse(this.TagTextBox.Text, out samplenumber))
			{
				this.TagTextBox.Focus();
				string message = this.GetTranslatedText("Tag Number must be an integer value") + "!";
				throw new ApplicationException(message);
			}

			int tagNumber = int.Parse(this.TagTextBox.Text);

			EquipmentQualityTagLogClass checkEquipment =
				FMChannelHelper.MakeCall<IEquipmentQualityTagLogs, EquipmentQualityTagLogClass>(
					eLogs => eLogs.GetByTagNumber(this.Security, tagNumber));

			TankQualityTagLogClass checkTank =
				FMChannelHelper.MakeCall<ITankQualityTagLogs, TankQualityTagLogClass>(
					tLogs => tLogs.GetByTagNumber(this.Security, tagNumber));

			if (this.EquipmentQualityTagLog == null)
			{
				// Check tank
				if (checkTank != null && checkTank.IdentityGuid != this.TankQualityTagLog.IdentityGuid)
				{
					throw new ApplicationException("Tag number must be unique.");
				}
			}
			else
			{
				if (checkEquipment != null && checkEquipment.IdentityGuid != this.EquipmentQualityTagLog.IdentityGuid)
				{
					throw new ApplicationException("Tag number must be unique.");
				}
			}
		}

		private void PopulateControlsForEquipment()
		{
			EquipmentQualityTagLogClass oEquipmentQualityTagLog = this.EquipmentQualityTagLog;

			if (oEquipmentQualityTagLog == null)
			{
				oEquipmentQualityTagLog = new EquipmentQualityTagLogClass();
				this.EquipmentQualityTagLog = oEquipmentQualityTagLog;
			}

			if (oEquipmentQualityTagLog.QualityTagName != null)
			{
				this.QualityTagNameFMCombobox.SelectByText(oEquipmentQualityTagLog.QualityTagName);
			}

			this.MemoTextBox.Text = oEquipmentQualityTagLog.Memo ?? string.Empty;

			this.LoadTaggedAndRemovedValues();
			this.AssetIDComboBox.SelectByText(oEquipmentQualityTagLog.EquipmentID);
			this.AssetTypeTextBox.Text = oEquipmentQualityTagLog.EquipmentType;

			this.TagTextBox.Text = oEquipmentQualityTagLog.TagNumber.ToString(CultureInfo.InvariantCulture);
		}

		// Used for coming from Tank QualityTag Log Form.
		private void PopulateControlsForTank()
		{
			TankQualityTagLogClass oTankQualityTagLog = this.TankQualityTagLog;

			if (oTankQualityTagLog == null)
			{
				oTankQualityTagLog = new TankQualityTagLogClass();
				this.TankQualityTagLog = oTankQualityTagLog;
			}

			if (oTankQualityTagLog.QualityTagName != null)
			{
				this.QualityTagNameFMCombobox.SelectByText(oTankQualityTagLog.QualityTagName);
			}

			this.MemoTextBox.Text = oTankQualityTagLog.Memo ?? string.Empty;

			this.AssetIDComboBox.SelectByText(oTankQualityTagLog.TankID);
			this.AssetTypeTextBox.Text = oTankQualityTagLog.VesselType;

			this.LoadTaggedAndRemovedValues();

			this.TagTextBox.Text = oTankQualityTagLog.TagNumber.ToString(CultureInfo.InvariantCulture);
		}
		#endregion
	}
}