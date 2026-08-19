// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanyCustomerBillToPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CompanyCustomerBillToPage type.
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
	///    Summary description for CustomerBillToPage.
	/// </summary>
	public partial class CompanyCustomerBillToPage : CompanyPageBase
	{
		#region Public Methods and Operators

		public void UpdateData()
		{
			if (!this.Company.HasRole(COMPANY_ROLE.CUSTOMER_BILLTO))
			{
				return;
			}

			if (this.TypeDropDownList.SelectedIndex != -1)
			{
				this.Company.CustomerBillToTypeApplicationStringGuid = Guid.Parse(this.TypeDropDownList.SelectedValue);
				this.Company.CustomerBillToTypeID = this.TypeDropDownList.SelectedItem.Text;
			}
			else
			{
				this.Company.CustomerBillToTypeApplicationStringGuid = Guid.Empty;
				this.Company.CustomerBillToTypeID = "{None}";
			}
		}

		#endregion

		#region Methods

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
				if (!this.Company.HasRole(COMPANY_ROLE.CUSTOMER_BILLTO))
				{
					return;
				}

				if (!this.Page.IsPostBack)
				{
					// TypeDropDownList				

					ApplicationStringCollectionClass Types =
						FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
							x => x.EnumerateByType(this.Security, STRING_TYPE.COMPANY_TYPE));

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
								if (Type.IdentityGuid == this.Company.CustomerBillToTypeApplicationStringGuid)
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
							if (Type.IdentityGuid == this.Company.CustomerBillToTypeApplicationStringGuid)
							{
								this.TypeDropDownList.SelectedIndex = this.TypeDropDownList.Items.Count - 1;
							}
						}
					}
                    this.TypeDropDownList.Items.Insert(0, new ListItem(this.GetTranslatedText("{None}"), Guid.Empty.ToString()));
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
                 || (currentSiteOwnsRecordVersion && this.Company.IdentityGuid.Equals(this.Company.MasterRecordGuid))
                 || (this.VersionSpecificFields == null)))
            {
                return;
            }
            this.TypeDropDownList.Enabled = (this.TypeDropDownList.Enabled 
                                                && this.VersionSpecificFields.Contains("CustomerBillToTypeApplicationStringGuid"));            
        }


		#endregion
	}
}