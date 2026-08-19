// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemDates.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.Accounting
{
	using System;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Code behind for system dates form
	/// </summary>
	public partial class SystemDates : AccountingAutoSubmitWebFormView
	{
		#region Constants and Fields
		/// <summary>
		/// The site to use for the form.
		/// </summary>
		private SiteClass site;
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
			this.Initialize();
			this.GetSecurity();

			this.site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(
																			this.security, 
																			this.security.SiteGuid, 
																			getMemberSites: true, 
																			getSchedulesAndProcessVariables: true, 
																			bGetAssociatedAliases: true)
																);
			if (this.IsPostBack == false)
			{
				this.Operations.Text = this.site.OperationalLockDate;
				this.Accounting.Text = this.site.AdministrativeLockDate;

				this.ApplyDataDictionary();
			}
		}

		/// <summary>
		/// Handles the Click event of the SaveButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void SaveButtonClick(object sender, EventArgs e)
		{
			try
			{
				this.SetDates();
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
			}
		}

		/// <summary>
		/// Applies the data dictionary.
		/// </summary>
		private void ApplyDataDictionary()
		{
			this.TitleLabel.Text = GetDataDictionaryValueByKey(this.Security.SiteGuid, "Lockout Dates Configuration");
			this.OperationsLabel.Text = GetDataDictionaryValueByKey(this.Security.SiteGuid, "Operational Lock Date");
			this.AccountingLabel.Text = GetDataDictionaryValueByKey(this.Security.SiteGuid, "Administrative Lock Date");
			this.SaveButton.Text = GetDataDictionaryValueByKey(this.Security.SiteGuid, "Apply");
		}
	

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Sets the dates.
		/// </summary>
		private void SetDates()
		{
			// Try setting the values based on the user input strings.
			// if it's not successful, reset the dates to their current valid values.
			// Ideally we'd be using TryParse() instead of throwing and catching exceptions
			try
			{
				this.site.OperationalLockDate = this.Operations.Text;
			}
			catch (Exception)
			{
				this.Operations.Text = this.site.OperationalLockDate;
				throw;
			}

			try
			{
				this.site.AdministrativeLockDate = this.Accounting.Text;
			}
			catch (Exception)
			{
				this.Accounting.Text = this.site.AdministrativeLockDate;
				throw;
			}
			
			FMChannelHelper.MakeCall<ISites>(
				x =>
				x.Modify(this.security, DATA_TYPE.CONFIG, this.site, updateDocumentNumbers: true)
			);
		}

		#endregion
	}
}