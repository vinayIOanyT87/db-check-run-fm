// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EquipmentTypeDetailsForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the EquipmentTypeDetailsForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	public partial class EquipmentTypeDetailsForm : FMFormBase
	{
		#region Constants and Fields

		public EquipmentTypeClass EquipmentType;
		bool canSaveQualificationsAndTrainingsOnly = false;

		private const string EQUIPMENT_TYPE_TAB_SELECTION = "EquipmentTypeTabSelection";

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///    This method will either enable or disable controls.  It is called by
		///    the individual tabs associated to the equipment form.
		/// </summary>
		/// <param name="enable"></param>
		public void EnableControls(bool enable)
		{
			if (this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
			{
				this.OK.Enabled = enable;
				this.New.Enabled = enable && (this.IsFromDispatch == false);
				
				if (!enable || (this.Security.SiteGuid != this.EquipmentType.SiteGuid && this.EquipmentType.SiteGuid != Guid.Empty))
				{
					this.EquipmentTypeAircraftGeneralPage.SetReadOnly();
					this.EquipmentTypeAircraftTanksPage.SetReadOnly();
					this.EquipmentTypeGeneralPage.SetReadOnly();
				}
			}

			this.Cancel.Enabled = enable;

			this.tcEquipmentTypeDetails.HeaderEnabled = enable;
		}

		public void UpdateData()
		{
			if (this.EquipmentType.Attribute == EQUIPMENT_TYPE.AIRCRAFT_TYPE)
			{
				this.EquipmentTypeAircraftGeneralPage.UpdateData();
			}
			else
			{
				this.EquipmentTypeGeneralPage.UpdateData();
			}
		}

		#endregion

		#region Methods

		protected void Cancel_Command(object sender, EventArgs e)
		{
			this.Session.Remove("SelectedEquipmentType");
			this.Redirect("EquipmentTypesForm.aspx");
		}

		protected void New_Command(object sender, EventArgs e)
		{
			if (this.CommitData())
			{
				this.Session.Remove("SelectedEquipmentType");
				this.Redirect("EquipmentTypeDetailsForm.aspx");
			}
		}

		protected void OK_Command(object sender, EventArgs e)
		{
			if (canSaveQualificationsAndTrainingsOnly)
			{
				if (this.CommitQualificationsAndTrainings())
				{
					this.Session.Remove("SelectedEquipmentType");
					this.Redirect("EquipmentTypesForm.aspx");
				}
			}
			else if (this.CommitData())
			{
				this.Session.Remove("SelectedEquipmentType");
				this.Redirect("EquipmentTypesForm.aspx");
			}
		}

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				base.GetSecurity();

				if (!this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) && !this.Security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA))
				{
					throw new Exception("Access denied.");
				}

				this.EquipmentType = this.Session["SelectedEquipmentType"] as EquipmentTypeClass;
				if (this.EquipmentType == null)
				{
					SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.GetByMemberAndProcessVariables(this.Security, this.Security.SiteGuid, false, false)
																);
					this.Session["SelectedEquipmentType"] = new EquipmentTypeClass(site);
					this.EquipmentType = this.Session["SelectedEquipmentType"] as EquipmentTypeClass;
				}

				if (!this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
				    || (this.EquipmentType != null && this.Security.SiteGuid != this.EquipmentType.SiteGuid))
				{
					canSaveQualificationsAndTrainingsOnly = this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
							&& this.Security.SiteGuid != this.EquipmentType.SiteGuid;

					this.New.Enabled = false;
					this.OK.Enabled = canSaveQualificationsAndTrainingsOnly;

					this.EquipmentTypeAircraftGeneralPage.SetReadOnly();
					this.EquipmentTypeAircraftTanksPage.SetReadOnly();
					this.EquipmentTypeGeneralPage.SetReadOnly();
				}

				if (this.Page.IsPostBack == false)
				{
					//Set the title label with a key field from the bound object appended
					if (this.EquipmentType != null)
					{
						this.EquipmentTypeTitleLabel.Text = this.GetTitleLabelText(
							this.EquipmentTypeTitleLabel.Text, this.EquipmentType.ID);
					}

					if (this.EquipmentType != null && this.EquipmentType.Attribute == EQUIPMENT_TYPE.AIRCRAFT_TYPE)
					{
						this.tpTanks.Visible = true;
						this.tpTanks.HeaderText = this.GetTranslatedText("Tanks");
						this.tpGeneral.Visible = false;
						this.tpAircraftGeneral.Visible = true;
						this.tpAircraftGeneral.HeaderText = this.GetTranslatedText("General");
					}
					else
					{
						this.tpAircraftGeneral.Visible = false;
						this.tpTanks.Visible = false;
						this.tpGeneral.Visible = true;
					}

					// When the page loads and it's not a post back, we set the previous active tab index to the first tab.
					// This is to overcome and issue with the tab control related to hiding the first tab.
					// The behavior observed was that for aircraft types causing multiple post backs by changing the Fuel Service tolerance type
					// would cause the second tab to be shown. 
					// This appears to only happen when the first tab (tpGeneral) is hidden
					// Because this setting in our tab control keeps track of visible tabs only, the value we use 0 no matter whether we are showing the regular general tab
					// or the general tab for aircraft.
					this.tcEquipmentTypeDetails.PreviousClientActiveTabIndex = "0";
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.Response.End();
			}
		}

		private bool CommitQualificationsAndTrainings()
		{
			try
			{
				//canSaveQualificationsOnly
				FMChannelHelper.MakeCall<IEquipmentTypes>(
							x =>
							x.ModifyOnlyQualificationsAndTrainings(this.Security, this.EquipmentType)
					);

				return true;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return false;
			}
		}

		private bool CommitData()
		{
			try
			{
				this.UpdateData();
				if (this.EquipmentType.ID == "")
				{
					throw new Exception("Equipment Type ID is a required field.");
				}
				if (this.EquipmentType.IsTanksPaired() == false)
				{
					this.ErrorHandler("FuelsManager: ", "Tanks Off Centerline Are Not Paired!");
					return false;
				}
				if (this.EquipmentType.IdentityGuid == Guid.Empty)
				{
					this.EquipmentType.SiteGuid = this.Security.SiteGuid;
					FMChannelHelper.MakeCall<IEquipmentTypes, Guid>(
																	 x =>
																	 x.Add(this.Security, this.EquipmentType)
																);
				}
				else
				{
					FMChannelHelper.MakeCall<IEquipmentTypes>(
																	 x =>
																	 x.Modify(this.Security, this.EquipmentType)
																);
				}

				return true;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return false;
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.New.Command += new System.Web.UI.WebControls.CommandEventHandler(this.New_Command);
			this.OK.Command += new System.Web.UI.WebControls.CommandEventHandler(this.OK_Command);
			this.Cancel.Command += new System.Web.UI.WebControls.CommandEventHandler(this.Cancel_Command);
		}

		#endregion
	}
}
