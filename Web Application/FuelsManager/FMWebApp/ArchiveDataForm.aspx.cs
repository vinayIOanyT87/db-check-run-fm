// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArchiveDataForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ArchiveDataForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.LogClient;
	using FMBusinessObjects.ServiceRequests;

	using global::FMWebApp;

	public partial class ArchiveDataForm : FMFormBase, IMenuDiscovery
	{
		#region Constants and Fields

		private readonly Logger logger = new Logger("TransactionArchiving");

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///    Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <param name="options">Hardware key options</param>
		/// <returns>
		///    List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
            if (useNewLicenseKey == 1)
            {

            }
            else
            {
                // Depends upon Shared Components
                if ((options & 0x4000) == 0)
                {
                    return null;
                }
            }
            // Only show the node if the user has the Modify System Settings right
            if (security.HasRight(RIGHT.MODIFY_SYSTEM_SETTINGS) == false)
			{
				return null;
			}

			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
				{
					MenuItemType = FMMenuItemType.CONFIG_SYSTEM_ARCHIVE_DATA,
					RootMenuName = "Configuration",
					CategoryName = "System",
					ItemName = "Archive Data",
					NavigateUrl = "ArchiveDataForm.aspx",
					ApplyDataDictionary = ApplyDataDictionary.Apply
				};

			menuItems.Add(menuItem);

			return menuItems;
		}

		#endregion

		#region Methods

		protected void ArchiveButton_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.StartDate.Text == "")
				{
					throw new Exception("Please enter Start Date.");
				}

				if (this.EndDate.Text == "")
				{
					throw new Exception("Please enter End Date.");
				}

				if (this.StartDate.CurrentValue.Subtract(this.EndDate.CurrentValue).TotalDays > 0)
				{
					throw new Exception("Start Date should be earlier than End Date.");
				}

				if (this.EndDate.CurrentValue.Subtract(this.StartDate.CurrentValue).TotalDays > 365)
				{
					throw new Exception("Please choose time duration less than a year.");
				}

				if ((this.chkAccounting.Checked == false) && (this.chkQC.Checked == false) && (this.chkMaintenance.Checked == false)
				    && (this.chkAlarm.Checked == false) && (this.chkAudit.Checked == false))
				{
					throw new Exception("Please choose Data to Archive");
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
				this.logger.Error("Archiving Database failed. " + ex.Message);
				System.Diagnostics.Trace.WriteLine(String.Format("Archiving Database failed. {0}", ex.Message));
				return;
			}

			var archiveDataSR = new ArchiveDataSR();
			archiveDataSR.StartDate = this.StartDate.CurrentValue;
			archiveDataSR.EndDate = this.EndDate.CurrentValue;
			archiveDataSR.CheckAccounting = this.chkAccounting.Checked;
			archiveDataSR.CheckQC = this.chkQC.Checked;
			archiveDataSR.CheckMaintenance = this.chkMaintenance.Checked;
			archiveDataSR.CheckAlarm = this.chkAlarm.Checked;
			archiveDataSR.CheckAudit = this.chkAudit.Checked;
			archiveDataSR.Security = this.Security;

			this.ResultsTextBox.Text = FMChannelHelper.MakeCall<IArchiveDataProcessor, string>(
														x =>
														x.Process(archiveDataSR)
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
				this.GetSecurity();
				this.ArchiveButton.Enabled = true;

				this.ArchiveButton.Attributes.Add(
					"onClick",
					"return confirm('All archived records will be permanently removed from the database. Are you sure that you want to archive the records?');");

				if (!this.Page.IsPostBack)
				{
					this.StartDate.CurrentValue = DateTimeOffset.Now.AddYears(-1);
					this.EndDate.CurrentValue = DateTimeOffset.Now;
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

		#endregion
	}
}