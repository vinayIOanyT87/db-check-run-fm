// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanyGroupForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CompanyGroupForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using global::FMWebApp;

	/// <summary>
	/// Code behind for CompanyGroupForm.
	/// </summary>
	public partial class CompanyGroupForm : FMAutoSubmitFormBase
	{
		#region Public Methods and Operators

		/// <summary>
		///    This method will either enable or disable controls.  It is called by
		///    the individual tabs associated to the company form.
		/// </summary>
		/// <param name="enable">True if controls should be enabled</param>
		public void EnableControls(bool enable)
		{
			CompanyGroupClass companyGroup;

			var companyGuid = this.Session["IdentityGuid"] as string;

			// Get IdentityGuid
			if (string.IsNullOrEmpty(companyGuid) == false)
			{
				// Get CompanyGroup
				companyGroup =
					FMChannelHelper.MakeCall<ICompanyGroups, CompanyGroupClass>(x => x.Get(this.Security, Guid.Parse(companyGuid)));
			}
			else
			{
				companyGroup = new CompanyGroupClass();
			}

			if (this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
			    && (this.Security.SiteGuid == companyGroup.SiteGuid || companyGroup.IdentityGuid == Guid.Empty))
			{
				this.OK.Enabled = enable;
			}

			this.Cancel.Enabled = enable;

			this.tcCompanyGroupTabs.HeaderEnabled = enable;
		}

		/// <summary>
		/// Updates the data.
		/// </summary>
		public void UpdateData()
		{
			this.CompanyGroupGeneralPage.UpdateData();
			this.CompanyGroupProductsPage.UpdateData();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Raises the <see cref="OnInit" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					CompanyGroupClass companyGroup = null;

					var companyGuid = this.Session["IdentityGuid"] as string;

					// Get IdentityGuid
					if (string.IsNullOrEmpty(companyGuid) == false)
					{
						if (string.IsNullOrEmpty(companyGuid) == false)
						{
							// Get CompanyGroup
							companyGroup =
								FMChannelHelper.MakeCall<ICompanyGroups, CompanyGroupClass>(
									x => x.Get(this.Security, Guid.Parse(companyGuid)));
						}
					}
					else
					{
						companyGroup = new CompanyGroupClass();
					}

					this.Session["CompanyGroup"] = companyGroup;

					if (!this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
					    || (this.Security.SiteGuid != companyGroup.SiteGuid && companyGroup.IdentityGuid != Guid.Empty))
					{
						this.OK.Enabled = false;
					}

					// Set the title label with a key field from the bound object appended
					if (companyGroup != null)
					{
						this.CompanyGroupTitleLabel.Text = this.GetTitleLabelText(this.CompanyGroupTitleLabel.Text, companyGroup.ID);
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the Command event of the Cancel control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="CommandEventArgs" /> instance containing the event data.</param>
		private void CancelCommand(object sender, CommandEventArgs e)
		{
			this.Redirect("CompanyGroupsForm.aspx");
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.OK.Command += this.OkCommand;
			this.Cancel.Command += this.CancelCommand;
		}

		/// <summary>
		/// Handles the Command event of the OK control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="CommandEventArgs" /> instance containing the event data.</param>
		private void OkCommand(object sender, CommandEventArgs e)
		{
			try
			{
				var companyGroup = (CompanyGroupClass)this.Session["CompanyGroup"];

				this.UpdateData();

				if (companyGroup.IdentityGuid != Guid.Empty)
				{
					FMChannelHelper.MakeCall<ICompanyGroups>(x => x.Modify(this.Security, companyGroup));
				}
				else
				{
					FMChannelHelper.MakeCall<ICompanyGroups>(x => x.Add(this.Security, companyGroup));
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Redirect("CompanyGroupsForm.aspx");
		}

		#endregion
	}
}