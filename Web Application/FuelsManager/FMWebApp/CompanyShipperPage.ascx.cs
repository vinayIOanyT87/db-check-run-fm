// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanyShipperPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CompanyShipperPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMWebApp
{
	using System;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	/// <summary>
	///    Summary description for CompanyShipperPage.
	/// </summary>
	public partial class CompanyShipperPage : CompanyPageBase
	{
		#region Public Methods and Operators

		public void UpdateData()
		{
			if (!this.Company.HasRole(COMPANY_ROLE.SHIPPER))
			{
				return;
			}

			if (this.TypeDropDownList.SelectedIndex != -1)
			{
				this.Company.ShipperTypeApplicationStringGuid = Guid.Parse(this.TypeDropDownList.SelectedValue);
				this.Company.ShipperTypeID = this.TypeDropDownList.SelectedItem.Text;
			}
			else
			{
				this.Company.ShipperTypeApplicationStringGuid = Guid.Empty;
				this.Company.ShipperTypeID = "{None}";
			}

			this.Company.AdditiveAccounting = this.AdditiveAccountingCheckBox.Checked;
		}

		#endregion

		#region Methods

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (!this.Company.HasRole(COMPANY_ROLE.SHIPPER))
				{
					return;
				}

				if (!this.Page.IsPostBack)
				{
					// TypeDropDownList				

					ApplicationStringCollectionClass Types;
					Types = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
																	 x =>
																	 x.EnumerateByType(this.Security,STRING_TYPE.COMPANY_TYPE)
																);

                    this.TypeDropDownList.Items.Insert(0, new ListItem(this.GetTranslatedText("{None}"), Guid.Empty.ToString()));
                    for (int iItem = 0; iItem < Types.Count; iItem++)
					{
						ApplicationStringClass Type = Types[iItem];

						var NewTypeItem = new ListItem(Type.ID, Type.IdentityGuid.ToString());

						foreach (ListItem ExistingTypeItem in this.TypeDropDownList.Items)
						{
							if (ExistingTypeItem.Text.CompareTo(NewTypeItem.Text) > 0)
							{
								int Index = this.TypeDropDownList.Items.IndexOf(ExistingTypeItem);
								this.TypeDropDownList.Items.Insert(Index, NewTypeItem);

								if (Type.IdentityGuid == this.Company.ShipperTypeApplicationStringGuid)
								{
									this.TypeDropDownList.SelectedIndex = Index;
								}

								NewTypeItem = null;
								break;
							}
						}

						if (NewTypeItem != null)
						{
							this.TypeDropDownList.Items.Add(NewTypeItem);

							if (Type.IdentityGuid == this.Company.ShipperTypeApplicationStringGuid)
							{
								this.TypeDropDownList.SelectedIndex = this.TypeDropDownList.Items.Count - 1;
							}
						}
					}
                    
                    this.AdditiveAccountingCheckBox.Checked = this.Company.AdditiveAccounting;
                    SetFieldAccessibilityForChildRecordVersion();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}


        private void SetFieldAccessibilityForChildRecordVersion()
        {
            bool currentSiteOwnsRecordVersion = (this.Company.SiteGuid == this.Security.SiteGuid);
            if ((this.Company.IdentityGuid.Equals(Guid.Empty)
                 || (currentSiteOwnsRecordVersion && this.Company.IdentityGuid.Equals(this.Company.MasterRecordGuid))))
            {
                return;
            }
            else if (this.VersionSpecificFields != null)
            {
                this.TypeDropDownList.Enabled = (this.TypeDropDownList.Enabled 
                                      && this.VersionSpecificFields.Contains("ShipperTypeApplicationStringGuid"));
                this.AdditiveAccountingCheckBox.Enabled = (this.AdditiveAccountingCheckBox.Enabled 
                                          && this.VersionSpecificFields.Contains("AdditiveAccounting"));
            }
        }



		#endregion
	}
}