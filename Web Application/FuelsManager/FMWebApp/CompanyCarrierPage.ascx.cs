// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanyCarrierPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CompanyCarrierPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    /// <summary>
	///    Summary description for CompanyCarrierPage.
	/// </summary>
	public partial class CompanyCarrierPage : CompanyPageBase
	{
		#region Constants and Fields

		public DateTimeFormatInfo DateFormat = DateTimeFormatInfo.CurrentInfo;

		protected EquipmentCollectionClass EquipmentCollection;

		#endregion

		#region Enums

		public enum AssignmentType
		{
			AuthorizedShipTo = 0,
			Driver = 1,
		};

		#endregion

		#region Properties

		private string JavascriptStartup
		{
			get
			{
				string script = @"
				<script type='text/javascript'>
				<!--
					// Set Assign and Unassign Button values according to Data Dictionary
					var AssignButton=document.getElementById('CompanyCarrierPage_AssignButton');
					if(AssignButton != null)
						AssignButton.value='" + ((FMFormBase)this.Page).GetTranslatedText("Assign") + @"';
					var UnassignButton=document.getElementById('CompanyCarrierPage_UnassignButton');
					if(UnassignButton != null)
						UnassignButton.value='" + ((FMFormBase)this.Page).GetTranslatedText("Unassign") + @"';
				//-->
				</script>
				";
				return script;
			}
		}

		#endregion

		#region Public Methods and Operators

		public void AddSelf()
		{
			var companyMap = CompanyMapClass.CreateCompanyMap(COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP);
			companyMap.AssignedToID = this.Company.ID;
			companyMap.AssignedToGuid = this.Company.IdentityGuid;
			companyMap.AssignedToName = this.Company.Name;
			companyMap.AssignedToAddress = this.Company.Address1;
			companyMap.AssignedToCity = this.Company.City;
			companyMap.AssignedToState = this.Company.State;
			this.Company.CarrierCustomerShipToCollection.Add(companyMap);

			this.UpdateView();
		}

		public void RemoveSelf()
		{
			int index = 0;
			foreach (CompanyMapClass companyMap in this.Company.CarrierCustomerShipToCollection)
			{
				if (companyMap.AssignedToID == this.Company.ID)
				{
					this.Company.CarrierCustomerShipToCollection.Remove(index);
					break;
				}
				index++;
			}

			this.UpdateView();
		}

		public void UpdateData()
		{
			if (!this.Company.HasRole(COMPANY_ROLE.CARRIER))
			{
				return;
			}

			this.Company.SCACCode = this.SCACCodeTextbox.Text;
			this.Company.LicenseNumber = this.LicenseNumberTextbox.Text;
			this.Company.LicenseExpiration = this.LicenseExpirationDate.Text;
			this.Company.InsuranceCompany = this.InsuranceCompanyTextbox.Text;
			this.Company.InsurancePolicy = this.InsurancePolicyTextbox.Text;
			this.Company.LiabilityAmount = this.LiabilityAmountTextbox.Text;
			this.Company.HazardousMaterialExclusion = this.HazardousMaterialExclusionCheckBox.Checked;
			this.Company.InsuranceExpiration = this.InsuranceExpirationDate.Text;
			this.Company.FlushPermitted = this.FlushPermittedCheckBox.Checked;
			this.Company.PumpOffPermitted = this.PumpOffPermittedCheckBox.Checked;
			this.Company.DeliveryToTerminalPermitted = this.DeliveryToTerminalPermittedCheckBox.Checked;
			this.Company.AllowDriverEntry = this.AllowDriverEntryCheckBox.Checked;
			this.Company.PINRequired = this.PINRequiredCheckBox.Checked;
            this.Company.ScullyRequired = this.ScullyRequiredCheckBox.Checked;
		}

		#endregion

		#region Methods

		protected void AssignEntitiesTextBoxTextChanged(object sender, EventArgs e)
		{
			try
			{
				string[] ids = this.AssignEntitiesTextBox.Text.Split('|');
				this.AssignEntitiesTextBox.Text = "";

				var type = (AssignmentType)Convert.ToInt32(this.TypeDropDownList.SelectedValue);

				if (type == AssignmentType.AuthorizedShipTo)
				{
					foreach (string id in ids)
					{ 
						if (id == "|")
						{
							continue;
						}

						Guid identityGuid = this.GetIdentityGuid(id);

						CompanyClass shipTo = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 companies => companies.Get(this.Security, identityGuid, false));

						var companyMap = CompanyMapClass.CreateCompanyMap(COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP);
						companyMap.AssignedToID = shipTo.ID;
						companyMap.AssignedToGuid = shipTo.IdentityGuid;
						companyMap.AssignedToName = shipTo.Name;
						companyMap.AssignedToAddress = shipTo.Address1;
						companyMap.AssignedToCity = shipTo.City;
						companyMap.AssignedToState = shipTo.State;
						this.Company.CarrierCustomerShipToCollection.Add(companyMap);

						if (shipTo.IdentityGuid == this.Company.IdentityGuid)
						{
							var shipToPage =
								(CompanyCustomerShipToPage)
								this.Page.FindControl("tcCompanyTabs")
								    .FindControl("tpCustomerShipToPage")
								    .FindControl("CompanyCustomerShipToPage");
							shipToPage.AddSelf();
						}
					}

					this.Company.CarrierCustomerShipToCollection.Sort(COMPANY_MAP_SORT_CRITERIA.ASSIGNEDTO);
				}

				else if (type == AssignmentType.Driver)
				{
					foreach (string id in ids)
					{
						if (id == "|")
						{
							continue;
						}

						PersonClass driver =
							FMChannelHelper.MakeCall<IPersonnel, PersonClass>(x => x.Get(this.Security, x.GetGuidByID(this.Security, id)));
                        CompanyMapClass assignedDriver = CompanyMapClass.CreateCompanyMap(COMPANY_MAP_TYPE.PERSON_ASSIGNED_COMPANY);
                        assignedDriver.AssignedGuid = this.Company.IdentityGuid;
                        assignedDriver.AssignedID = this.Company.ID;
                        assignedDriver.AssignedToGuid = driver.IdentityGuid;
                        assignedDriver.AssignedToID = driver.ID;
                        assignedDriver.AssignedToFirstName = driver.FirstName;
                        assignedDriver.AssignedToMiddleName = driver.MiddleName;
                        assignedDriver.AssignedToLastName = driver.LastName;
					    this.Company.AssignedPersonnelCollection.Add(assignedDriver);
					}

				    this.Company.AssignedPersonnelCollection.Sort(COMPANY_MAP_SORT_CRITERIA.ASSIGNEDTO);
				}

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private Guid GetIdentityGuid(string id)
		{
			return FMChannelHelper.MakeCall<ICompanies, Guid>(
								x =>
								x.GetIdentityGuid(this.Security, id)
						);
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
				if (!this.Company.HasRole(COMPANY_ROLE.CARRIER))
				{
					return;
				}

				if (this.ViewState["DATE_FORMAT"] != null)
				{
					this.DateFormat = this.ViewState["DATE_FORMAT"] as DateTimeFormatInfo;
				}
				else
				{
					if (this.Security != null)
					{
						Guid siteGuid = (this.Company.SiteGuid == Guid.Empty) ? this.Security.SiteGuid : this.Company.SiteGuid;

						SiteClass site =
							FMChannelHelper.MakeCall<ISites, SiteClass>(
								sites =>
								sites.GetBasic(
									this.Security,
									siteGuid));

					    var d = site?.GetDateTimeFormatInfo();
					    if (d != null)
					    {
					        this.DateFormat = d;
					    }
					}
					this.ViewState["DATE_FORMAT"] = this.DateFormat;
				}

				if (this.Page.IsPostBack == false)
				{
					this.SCACCodeTextbox.Text = this.Company.SCACCode;
					this.LicenseNumberTextbox.Text = this.Company.LicenseNumber;

					this.Company._LicenseExpiration.Format = this.DateFormat;
					this.LicenseExpirationDate.Text = this.Company.LicenseExpiration;

					this.InsuranceCompanyTextbox.Text = this.Company.InsuranceCompany;
					this.InsurancePolicyTextbox.Text = this.Company.InsurancePolicy;
					this.LiabilityAmountTextbox.Text = this.Company.LiabilityAmount;
					this.HazardousMaterialExclusionCheckBox.Checked = this.Company.HazardousMaterialExclusion;

					this.Company._InsuranceExpiration.Format = this.DateFormat;
					this.InsuranceExpirationDate.Text = this.Company.InsuranceExpiration;

					this.FlushPermittedCheckBox.Checked = this.Company.FlushPermitted;
					this.PumpOffPermittedCheckBox.Checked = this.Company.PumpOffPermitted;
					this.DeliveryToTerminalPermittedCheckBox.Checked = this.Company.DeliveryToTerminalPermitted;
					this.AllowDriverEntryCheckBox.Checked = this.Company.AllowDriverEntry;
					this.PINRequiredCheckBox.Checked = this.Company.PINRequired;
                    this.ScullyRequiredCheckBox.Checked = this.Company.ScullyRequired;

					// Populate the TypeDropDownList
					AssignmentType[] types =
						{
							AssignmentType.AuthorizedShipTo,
							AssignmentType.Driver
						};

					foreach (AssignmentType type in types)
					{
						var item = new ListItem(this.AssignmentTypeID(type), ((int)type).ToString(CultureInfo.InvariantCulture));
						this.TypeDropDownList.Items.Add(item);
					}

					this.UpdateView();
                    this.SetFieldAccessibilityForChildRecordVersion();
				}

				this.Page.ClientScript.RegisterStartupScript(
					this.GetType(), "CompanyCarrierPageScriptBlock", this.JavascriptStartup);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void TypeDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			this.UpdateView();
		}

		protected void UnassignEntitiesTextBoxTextChanged(object sender, EventArgs e)
		{
			try
			{
				string[] ids = this.UnassignEntitiesTextBox.Text.Split('|');
				this.UnassignEntitiesTextBox.Text = "";

				var type = (AssignmentType)Convert.ToInt32(this.TypeDropDownList.SelectedValue);

				if (type == AssignmentType.AuthorizedShipTo)
				{
					foreach (CompanyMapClass companyMap in this.Company.CarrierCustomerShipToCollection)
					{
						foreach (string id in ids)
						{
							if (id == "|")
							{
								continue;
							}

							if ((companyMap.AssignedToID == id) && (companyMap.AssignedGuid == Guid.Empty))
							{
								throw new ApplicationException("Cannot remove company from ALL company configuration of ShipTo company " + companyMap.AssignedToID);
							}
						}
					}

					foreach (string id in ids)
					{
						if (id == "|")
						{
							continue;
						}

						int index = 0;
						foreach (CompanyMapClass companyMap in this.Company.CarrierCustomerShipToCollection)
						{
							if (companyMap.AssignedToID == id)
							{
								this.Company.CarrierCustomerShipToCollection.Remove(index);

								if (companyMap.AssignedID == this.Company.ID)
								{
									var shipToPage =
										(CompanyCustomerShipToPage)
										this.Page.FindControl("tcCompanyTabs")
											.FindControl("tpCustomerShipToPage")
											.FindControl("CompanyCustomerShipToPage");
									shipToPage.RemoveSelf();
								}
								break;
							}
							index++;
						}
					}
				}

				else if (type == AssignmentType.Driver)
				{
					foreach (string id in ids)
					{
						if (id == "|")
						{
							continue;
						}

						int index = 0;
                        foreach (CompanyMapClass assignedDriver in this.Company.AssignedPersonnelCollection)
                        {
                            if (assignedDriver.AssignedToID == id)
                            {
                                this.Company.AssignedPersonnelCollection.RemoveAt(index);
                                break;
                            }
                            index++;
                        }
					}
				}

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AssignedEntitiesDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex != -1)
			{
				var type = (AssignmentType)Convert.ToInt32(this.TypeDropDownList.SelectedValue);

				var idLabel = (Label)e.Item.FindControl("IDLabel");

				if (type == AssignmentType.AuthorizedShipTo)
				{
					CompanyMapClass shipTo = this.Company.CarrierCustomerShipToCollection[e.Item.DataSetIndex];
					idLabel.Text = shipTo.AssignedToID;
					idLabel.ToolTip = shipTo.AssignedToToolTip;
				}

				else if (type == AssignmentType.Driver)
				{
                    CompanyMapClass assignedPerson = this.Company.AssignedPersonnelCollection[e.Item.DataSetIndex];
                    idLabel.Text = assignedPerson.AssignedToID;
                    idLabel.ToolTip = assignedPerson.AssignedToToolTip;
				}
			}
		}

		private void AssignedEntitiesDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.AssignedEntitiesDataGrid.EditItemIndex > -1)
			{
				return;
			}
			this.AssignedEntitiesDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		private string AssignmentTypeID(AssignmentType type)
		{
			switch (type)
			{
				case AssignmentType.AuthorizedShipTo:
					return "Ship To";

				case AssignmentType.Driver:
					return "Drivers";

				default:
					return "";
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AssignedEntitiesDataGrid.PageIndexChanged += this.AssignedEntitiesDataGridPageIndexChanged;
			this.AssignedEntitiesDataGrid.ItemDataBound += this.AssignedEntitiesDataGridItemDataBound;
		}

		private void UpdateView()
		{
			if (this.TypeDropDownList.SelectedValue == "")
			{
				return;
			}

			var type = (AssignmentType)Convert.ToInt32(this.TypeDropDownList.SelectedValue);

			if (type == AssignmentType.AuthorizedShipTo)
			{
                this.AssignedEntitiesDataGrid.DataSource = this.Company.CarrierCustomerShipToCollection;
                if ((!this.Company.IdentityGuid.Equals(Guid.Empty))
                     && (!this.Company.IdentityGuid.Equals(this.Company.MasterRecordGuid)) && (this.VersionSpecificFields != null))
                {
                    this.CompanyCarrierPage_AssignButton.Disabled = (!this.VersionSpecificFields.Contains("AuthorizedShipTo"));
                    this.CompanyCarrierPage_UnassignButton.Disabled = (!this.VersionSpecificFields.Contains("AuthorizedShipTo"));
                }
			}

			else if (type == AssignmentType.Driver)
			{
				this.AssignedEntitiesDataGrid.DataSource = this.Company.AssignedPersonnelCollection;
                if ((!this.Company.IdentityGuid.Equals(Guid.Empty))
                     && (!this.Company.IdentityGuid.Equals(this.Company.MasterRecordGuid)) && (this.VersionSpecificFields != null))
                {
                    this.CompanyCarrierPage_AssignButton.Disabled = (!this.VersionSpecificFields.Contains("Drivers"));
                    this.CompanyCarrierPage_UnassignButton.Disabled = (!this.VersionSpecificFields.Contains("Drivers"));
                }
			}

			int count = 0;

			//sometimes the dataSource is a list, sometimes it's a CollectionBase
		    var list = this.AssignedEntitiesDataGrid.DataSource as List<PersonClass>;
		    if (list != null)
			{
                count = list.Count;
			}
            else
		    {
		        var source = this.AssignedEntitiesDataGrid.DataSource as List<EquipmentClass>;
		        if (source != null)
		        {
		            count = source.Count;
		        }
		        else
		        {
		            var classes = this.AssignedEntitiesDataGrid.DataSource as List<CompanyMapClass>;
		            if (classes != null)
		            {
		                count = classes.Count;
		            }
		        }
		    }

		    if ((count - 1) / this.AssignedEntitiesDataGrid.PageSize < this.AssignedEntitiesDataGrid.CurrentPageIndex)
			{
				this.AssignedEntitiesDataGrid.CurrentPageIndex = (count - 1) / this.AssignedEntitiesDataGrid.PageSize;
			}

			this.AssignedEntitiesDataGrid.DataBind();
		}



        private void SetFieldAccessibilityForChildRecordVersion()
        {
            bool currentSiteOwnsRecordVersion = (this.Company.SiteGuid == this.Security.SiteGuid);
            if ((this.Company.IdentityGuid.Equals(Guid.Empty)
                 || (currentSiteOwnsRecordVersion && this.Company.IdentityGuid.Equals(this.Company.MasterRecordGuid))
                 || (this.VersionSpecificFields == null)))
            {
                return;
            }

            this.SCACCodeTextbox.Enabled = (this.SCACCodeTextbox.Enabled && this.VersionSpecificFields.Contains("SCACCode"));
            this.LicenseNumberTextbox.Enabled = (this.LicenseNumberTextbox.Enabled && this.VersionSpecificFields.Contains("LicenseNumber"));
            this.LicenseExpirationDate.Enabled = (this.LicenseExpirationDate.Enabled && this.VersionSpecificFields.Contains("LicenseExpiration"));
            this.InsuranceCompanyTextbox.Enabled = (this.InsuranceCompanyTextbox.Enabled && this.VersionSpecificFields.Contains("InsuranceCompany"));
            this.InsurancePolicyTextbox.Enabled = (this.InsurancePolicyTextbox.Enabled && this.VersionSpecificFields.Contains("InsurancePolicy"));
            this.LiabilityAmountTextbox.Enabled = (this.LiabilityAmountTextbox.Enabled && this.VersionSpecificFields.Contains("LiabilityAmount"));
            this.HazardousMaterialExclusionCheckBox.Enabled = (this.HazardousMaterialExclusionCheckBox.Enabled && this.VersionSpecificFields.Contains("HazardousMaterialExclusion"));
            this.InsuranceExpirationDate.Enabled = (this.InsuranceExpirationDate.Enabled && this.VersionSpecificFields.Contains("InsuranceExpiration"));
            this.FlushPermittedCheckBox.Enabled = (this.FlushPermittedCheckBox.Enabled && this.VersionSpecificFields.Contains("FlushPermitted"));
            this.PumpOffPermittedCheckBox.Enabled = (this.PumpOffPermittedCheckBox.Enabled && this.VersionSpecificFields.Contains("PumpOffPermitted"));
            this.AllowDriverEntryCheckBox.Enabled = (this.AllowDriverEntryCheckBox.Enabled && this.VersionSpecificFields.Contains("AllowDriverEntry"));
            this.DeliveryToTerminalPermittedCheckBox.Enabled = (this.DeliveryToTerminalPermittedCheckBox.Enabled && this.VersionSpecificFields.Contains("DeliveryToTerminalPermitted"));
            this.PINRequiredCheckBox.Enabled = (this.PINRequiredCheckBox.Enabled && this.VersionSpecificFields.Contains("PINRequired"));     
            this.ScullyRequiredCheckBox.Enabled= (this.ScullyRequiredCheckBox.Enabled && this.VersionSpecificFields.Contains("ScullyRequired"));
            this.CompanyCarrierPage_AssignButton.Disabled = (this.CompanyCarrierPage_AssignButton.Disabled
                                                             || (VersionSpecificFields.Count == 0));
            this.CompanyCarrierPage_UnassignButton.Disabled = (this.CompanyCarrierPage_UnassignButton.Disabled 
                                                             || (VersionSpecificFields.Count == 0));
        }



		#endregion
	}
}