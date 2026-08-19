// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportConfigurationAssignmentDirectoriesPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ReportConfigurationAssignmentDirectoriesPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMReportWebMain
{
	using System;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FMWebApp;

	/// <summary>
	/// The report configuration assignment directories page.
	/// </summary>
	public partial class ReportConfigurationAssignmentDirectoriesPage : FMUserControlBase
	{
		#region Constants and Fields
		/// <summary>
		/// The error message 001.
		/// </summary>
		private string errorMsg001 = "Invalid entry";

		/// <summary>
		/// The error message 002.
		/// </summary>
		private string errorMsg002 = "Insufficient Privledges To View Site Parameters";
		#endregion

		#region Public Methods and Operators
		/// <summary>
		/// The update data.
		/// </summary>
		public void UpdateData()
		{
			try
			{
				SiteClass site =
					FMChannelHelper.MakeCall<ISites, SiteClass>(
																x => x.Get(
																	this.Security,
																	this.Security.SiteGuid,
																	getMemberSites: true,
																	getSchedulesAndProcessVariables: true,
																	bGetAssociatedAliases: true));

				site.ReportDirectory = this.ReportDirectoryTextBox.Text;
				site.ManageReports = this.ManageReportsCheckBox.Checked;
				site.ManagedReportDirectory = this.ManagedReportDirectoryTextBox.Text;
				FMChannelHelper.MakeCall<ISites>( x => x.Modify(this.Security, DATA_TYPE.CONFIG, site, updateDocumentNumbers: true));
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
			}
		}

		#endregion

		#region Methods
		/// <summary>
		/// The load page data.
		/// </summary>
		protected void LoadPageData()
		{
			try
			{
				if (this.Security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS))
				{
					SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																x => x.Get(
																	this.Security,
																	this.Security.SiteGuid,
																	getMemberSites: true,
																	getSchedulesAndProcessVariables: true,
																	bGetAssociatedAliases: true));

					this.ReportDirectoryTextBox.Text = site.ReportDirectory;
					this.ManageReportsCheckBox.Checked = site.ManageReports;
					this.ManagedReportDirectoryTextBox.Text = site.ManagedReportDirectory;
				}
				else
				{
					this.ReportDirectoryTextBox.Enabled = false;
					this.ManageReportsCheckBox.Enabled = false;
					this.ManagedReportDirectoryTextBox.Enabled = false;
					this.HandleErrorCondition(this.errorMsg002);
				}

				if (!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
				{
					this.OK.Enabled = false;
				}

				// TODO: Temporary disable the Manage Reports checkbox and directory for QA testing of this feature.
				this.ManageReportsCheckBox.Visible = false;
				this.ManagedReportDirectoryTextBox.Visible = false;
				this.ManagedReportDirectoryLabel.Visible = false;
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
			}
		}

		/// <summary>
		/// The on initialize.
		/// </summary>
		/// <param name="e">
		/// The e.
		/// </param>
		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// The page_ load.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (this.Page.IsPostBack)
			{
				// Apply the data dictionary to the page.
				this.ApplyDataDictionary();
			}
			else
			{
				try
				{
					this.LoadPageData();
				}
				catch (Exception exception)
				{
					this.ErrorHandler(exception);
				}
			}
		}

		/// <summary>
		///    This method will apply the data dictionary to this page.  If the data dictionary
		///    use flag is set to true, then it will apply data dictionary.
		/// </summary>
		private void ApplyDataDictionary()
		{
			string newText = this.GetTranslatedText(this.errorMsg001);
			this.errorMsg001 = newText;
			newText = this.GetTranslatedText(this.errorMsg002);
			this.errorMsg002 = newText;
		}

		/// <summary>
		/// The cancel command.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		private void CancelCommand(object sender, CommandEventArgs e)
		{
			this.LoadPageData();
		}

		/// <summary>
		/// The handle error condition.
		/// </summary>
		/// <param name="errMsg">
		/// The error message.
		/// </param>
		private void HandleErrorCondition(string errMsg)
		{
			if (string.IsNullOrEmpty(errMsg) == false)
			{
				((FMFormBase)this.Page).ErrorHandler("FuelsManager", errMsg);
			}
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
		/// The ok command.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		private void OkCommand(object sender, CommandEventArgs e)
		{
			this.UpdateData();
		}
		#endregion
	}
}