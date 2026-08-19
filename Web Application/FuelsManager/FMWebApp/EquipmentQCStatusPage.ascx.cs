namespace FuelsManager.FMWebApp
{
    using System;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.UtilityObjects;

    using FMCore;

    /// <summary>
	/// EquipmentQCStatusPage code behind class.
	/// </summary>
	public partial class EquipmentQcStatusPage : EquipmentPageBase
	{

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (!this.Page.IsPostBack)
				{
					this.InServiceCheckBox.Checked = this.Equipment.InServiceFlag;

					this.NotesTextbox.Text = this.Equipment.Notes;
					this.QCNoteTextbox.Text = this.Equipment.QCNote;
					this.MaintenanceNoteTextbox.Text = this.Equipment.MaintenanceNote;
					this.TagAssignmentTextbox.Text = this.Equipment.QualityTag.ID;

					if (this.Equipment._ReturnToServiceDate.Value != DateTimeOffset.MinValue &&
							this.Equipment.IdentityGuid != Guid.Empty)
					{
						this.ReturnToServiceFMDate.Text = this.Equipment.ReturnToServiceDate;
					}
					else
					{
						this.ReturnToServiceFMDate.Text = "";
					}

					this.ReturnToServiceFMDate.Enabled = false;

					if (this.Equipment._QCDate.Value != DateTimeOffset.MinValue &&
							this.Equipment.IdentityGuid != Guid.Empty)
					{
						this.QCDueDate.Text = this.Equipment.QCDate;
					}
					else
					{
						this.QCDueDate.Text = "";
					}

					// Work Item 11101 - QC Due Date is only editable when creating a new piece of equipment
					if (this.Equipment.IdentityGuid != Guid.Empty)
					{
						this.QCDueDate.Enabled = false;
					}

					if (this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) == false ||	this.Equipment.ManagedEquipmentFlag == false ||
                        this.Equipment.IdentityGuid == Guid.Empty || (this.Equipment.SiteGuid != this.Security.SiteGuid) || (!this.Equipment.IdentityGuid.Equals(this.Equipment.MasterRecordGuid)))
					{
						this.EditQCButton.Enabled = false;
					}

					// If new equipment, allow setting the In Service Flag
					if (this.Equipment.IdentityGuid == Guid.Empty && FMChannelHelper.MakeCall<IHardwareKey,bool>(x => x.IsDescKey()))
					{
						SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.GetBasic(this.Security, this.Security.SiteGuid)
																);
						
						this.InServiceCheckBox.Enabled = true;
						
						this.Equipment.InServiceFlag = (this.Session[EquipmentForm.EquipmentQcStatus] == null);
						this.Equipment.InUse = this.Equipment.InServiceFlag;
						this.InServiceCheckBox.Checked = this.Equipment.InServiceFlag;

						this.ReturnToServiceFMDate.Text = TimeConverter.Today(site).ToString("d");
						this.ReturnToServiceFMDate.Enabled = true;

						this.LoadMaintenanceReasons();
						this.StatusDescriptionDropDownList.Enabled = true;
					}
					else
					{
						this.StatusDescriptionDropDownList.Text = this.Equipment.StatusDescription;
						this.StatusDescriptionDropDownList.Enabled = false;

						var item = new ListItem(this.Equipment.StatusDescription, this.Equipment.StatusDescription);
						this.StatusDescriptionDropDownList.Items.Add(item);
						this.StatusDescriptionDropDownList.SelectByText(item.Text);
					}
                    this.SetFieldAccessibilityForChildRecordVersion();
				}

			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			this.LocalInitialize();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		protected void LoadMaintenanceReasons()
		{
			var oMaintenanceReasonCollectionClass =
				FMChannelHelper.MakeCall<IMaintenanceReasons, MaintenanceReasonCollectionClass>(x => x.EnumerateBySite(this.Security));

			// And add them to the droplist.
			this.StatusDescriptionDropDownList.Items.Clear();
			ListItem li;

			foreach (MaintenanceReasonClass oMaintenanceReason in oMaintenanceReasonCollectionClass)
			{
				if (oMaintenanceReason.Description != null && oMaintenanceReason.Description.Trim().Length > 0)
				{
					li = new ListItem(oMaintenanceReason.Description, oMaintenanceReason.IdentityGuid.ToString());
					this.StatusDescriptionDropDownList.Items.Add(li);
				}
			}

			li = new ListItem("", "0");
			this.StatusDescriptionDropDownList.Items.Add(li);
			this.StatusDescriptionDropDownList.SelectByText("");
		}

		private void LocalInitialize()
		{
			this.EditQCButton.Click += this.EditQcButtonClick;
			this.InServiceCheckBox.CheckedChanged += this.InServiceCheckBoxCheckedChanged;
		}

		void InServiceCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			try
			{
				this.Equipment.InServiceFlag = this.InServiceCheckBox.Checked;
				this.Equipment.InUse = this.InServiceCheckBox.Checked;

				this.ReturnToServiceFMDate.Enabled = (this.Equipment.InServiceFlag == false);
				this.StatusDescriptionDropDownList.Enabled = (this.Equipment.InServiceFlag == false);

				if (this.StatusDescriptionDropDownList.Enabled == false)
				{
					this.StatusDescriptionDropDownList.SelectByText(string.Empty);
				}

			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		void EditQcButtonClick(object sender, EventArgs e)
		{
			try
			{
				((EquipmentForm)this.Page).UpdateData();

				var oEquipmentMaintenanceLog =
					FMChannelHelper.MakeCall<IEquipmentMaintenanceLogs, EquipmentMaintenanceLogClass>(
						x => x.GetByEquipmentGuid(this.Security, this.Equipment.MasterRecordGuid));

				if (oEquipmentMaintenanceLog != null)
				{
					var managedEquipment =
						FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(x => x.Get(this.Security, this.Equipment.IdentityGuid));

					if (managedEquipment == null || managedEquipment.ManagedEquipmentFlag == false)
					{
						throw new ApplicationException("Must be managed equipment. Save as managed equipment first.");
					}
				}
				else
				{
					oEquipmentMaintenanceLog = new EquipmentMaintenanceLogClass
					                           {
						                           EquipmentGuid = this.Equipment.IdentityGuid,
						                           EquipmentID = this.Equipment.ID,
						                           EquipmentType = this.Equipment.EqTypeName
					                           };
				}

				this.Session["MaintenanceAddRecord.MaintenanceAddRecordForm.MaintenanceLog"] = oEquipmentMaintenanceLog;
				this.Session["EQUIPMENT_SESSION_KEY"] = this.Equipment;
				this.Session["ReturnPageFromMaintenanceAddRecordForm"] = "../FMWebApp/EquipmentForm.aspx?TAB=QCTab&QueryEdit=" +
																	this.Request.GetQueryOrFormValue("QueryEdit") + "&DispatchEdit=" + this.Request.GetQueryOrFormValue("DispatchEdit");

				this.Redirect("../MaintenanceWebApp/MaintenanceAddRecordForm.aspx?MODE=ADD");
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		public void UpdateData()
		{
			this.Equipment.Notes = this.NotesTextbox.Text;

			if (this.Equipment._QCDate == null)
			{
				this.Equipment._QCDate = new Date();
			}

			if (this.Equipment._QCDate != null && this.QCDueDate.Text != null && this.QCDueDate.Text.Length > 0)
			{
				this.Equipment._QCDate.Value = this.QCDueDate.CurrentValue;
			}
			else
			{
				this.Equipment._QCDate.Value = DateTimeOffset.MinValue;
			}

			if (this.InServiceCheckBox.Enabled)
			{
				this.Equipment.ReturnToServiceDate = this.ReturnToServiceFMDate.Text;
				this.Equipment.ReturnToServiceDateObject.Value = this.ReturnToServiceFMDate.CurrentValue;

				this.Equipment.StatusDescription = this.StatusDescriptionDropDownList.SelectedItem.Text;
				if (string.IsNullOrEmpty(this.StatusDescriptionDropDownList.SelectedItem.Text) == false)
				{
					this.Equipment.StatusDescriptionGuid = Guid.Parse(this.StatusDescriptionDropDownList.SelectedValue);
				}
			}

		}


        private void SetFieldAccessibilityForChildRecordVersion()
        {
            bool currentSiteOwnsRecordVersion = (this.Equipment.SiteGuid == this.Security.SiteGuid);
            if (this.Equipment.IdentityGuid.Equals(Guid.Empty)
                || (currentSiteOwnsRecordVersion && this.Equipment.IdentityGuid.Equals(this.Equipment.MasterRecordGuid))
                || (this.VersionSpecificFields == null))
            {
                return;
            }
            this.QCDueDate.Enabled = (this.QCDueDate.Enabled && this.VersionSpecificFields.Contains("QCDate"));
            this.NotesTextbox.Enabled = (this.NotesTextbox.Enabled && this.VersionSpecificFields.Contains("Notes"));
        }


	}
}
