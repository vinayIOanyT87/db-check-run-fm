/******************************************************************************
	FILE NAME:		SiteSystemPage.ascx.cs
	PURPOSE:		Implementation of SiteSystemPage

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	W. Gray
	VERSION:	1.0.0  Current version

	MODIFICATION HISTORY:
		Date:		By:					Reason:
		----------	-----------------	-------------------------------------------
		07/10/2006	Richard Panachida	Set max field lengths. CSI # 3078
		2006-10-24	Richard Panachida	Fix CSI 3406 (counter and connection mode dropdown
										did not function correctly). Missing event handler
										link.
		2006-10-24	Richard Panachida	Fixed data dictionary. Some labels are not inhieriting
										from FMControls (CSI 3405).
		2006-11-14	W.Gray				Added SystemSelectModeDropDownList to address CSI 2743
		2007-04-23	W.Gray				Added InhibitTankScan (CSI 4458)
		2007-04-25	W.Gray				Set Visiblity = false for Inhibit Automatic Adjustment Distribution
												and Inhibit Automatic Closeout

		2007-07-30	I.Orndorff			1.0.0.2 - Changed AlarmAndEventPrinterDropDownList from 
												  System.Web.UI.WebControls.DropDownList to 
												  FMControls.FMDropDownList. This fixes CSI #4670.
												  
		2007-11-28	Richard Panachida	7.3.0.4	Added code to handle the new field to enforce single owner (CSI 5246).
		
		2008-04-22	I.Orndorff			7.4.0.0 - Modified "Page_Load()" to set the default alarm and event printer 
												  even if it can't be enumerated. This fixes CSI #5777.

		2008-05-15	W.Gray				7.4.3.0 - Added Mail From (CSI 5894)
		
		2008-05-22	I.Orndorff			7.4.5.0 - Removed "EnumerateHosts()" for "Page_Load()".
												- Added "PopulateSystemDropDownList()", which
												  is only called the the SystemDropDownList is 
												  visible. This fixes CSI #5907.
 
		2009-03-20	W.Gray				7.5.0.0 - Revised such that MailPassword is not set on the form
												and only retrieved from the form when it is not string.Empty.
												Also fixed form to properly set Password from initial Password (CSI 2061)

		2012-04-24  B. Main				8.0.5.0 - Cannot discover local printers when running in Azure.  When in Azure 
												AlarmAndEventPrinter drop down list will contain one option, {None}
 		2012-08-31  S. Marlin			8.0.5.14 - WI 33371 Removed ReportDirectory, ManagedReports Checkbox, and ManagedReportDirectory
*******************************************************************************/
namespace FuelsManager.FMWebApp
{
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;
    using FMBusinessObjects.UtilityObjects;
    using FMCore;
    using OpcCom;
    using System;
    using System.Drawing;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using System.Web.UI.WebControls;


    /// <summary>
    ///		Summary description for SiteSystemPage.
    /// </summary>
    public partial class SiteSystemPage : FMUserControlBase
	{
		#region Protected data members
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				SiteClass site = (SiteClass)this.Session["Site"];

				if (!this.Page.IsPostBack)
                {
                    this.MaximumDaysToRetainLogsTextBox.Text = site.MaximumDaysToRetainLogs;
                    this.MaximumDaysToRetainArchiveTextBox.Text = site.MaximumDaysToRetainArchive;
                    this.EnableDebugLoggingCheckBox.Checked = site.EnableDebugLogging;
                    this.EnableAuditLoggingCheckBox.Checked = site.EnableAuditLogging;
                    this.AutomaticallyPrintAlarmsAndEventsCheckBox.Checked = site.AutomaticallyPrintAlarmsAndEvents;

					PopulatePrinterDropDowns(site);

					this.InhibitTemplateGraphicsCheckBox.Checked = site.InhibitTemplateGraphics;
                    this.InhibitEndOfDayCheckBox.Checked = site.InhibitEndOfDayOperations;
                    this.InhibitEndOfMonthCheckBox.Checked = site.InhibitEndOfMonthOperations;
                    this.EndOfDayWarningPeriodTextBox.Text = site.EndOfDayWarningPeriod;
                    this.InhibitAutomaticPhysicalInventoryCheckBox.Checked = site.InhibitAutomaticPhysicalInventory;
                    this.InhibitAutomaticMeterCloseoutCheckBox.Checked = site.InhibitAutomaticMeterCloseout;
                    this.InhibitAutomaticReportGenerationCheckBox.Checked = site.InhibitAutomaticReportGeneration;
                    this.InhibitAutomaticCloseoutCheckBox.Checked = site.InhibitAutomaticCloseout;
                    this.InhibitTankScanCheckBox.Checked = site.InhibitTankScan;
                    this.InhibitCloseoutOnUnpostedBol.Checked = site.BlockCloseOnUnpostedBol;

                    this.InhibitBOLSummaryAutoSelection.Checked = site.InhibitBOLSummaryAutoPopulate;
                    this.InhibitOrderSummaryAutoSelection.Checked = site.InhibitOrderSummaryAutoPopulate;
                    this.InhibitSupplyOrderSummaryAutoSelection.Checked = site.InhibitSupplyOrderSummaryAutoPopulate;

                    this.MailServerTextBox.Text = site.MailServer;
                    this.MailFromTextBox.Text = site.MailFrom;
                    this.MailUserNameTextBox.Text = site.MailUserName;

                    // Populate ConnectionModesDropDownList
					MAIL_SERVER_CONNECT_MODE[] mailConnectModes ={	MAIL_SERVER_CONNECT_MODE.LAN,
                                                                                    MAIL_SERVER_CONNECT_MODE.DIALUP
                                                                                };

                    foreach (MAIL_SERVER_CONNECT_MODE mailConnectMode in mailConnectModes)
                    {
                        ListItem newConnectItem = new ListItem(mailConnectMode.ToString(), ((int)mailConnectMode).ToString());
                        this.ConnectionModeDropDownList.Items.Add(newConnectItem);
                        if (mailConnectMode == site.MailConnectMode) this.ConnectionModeDropDownList.SelectedIndex = this.ConnectionModeDropDownList.Items.Count - 1;
                    }

                    this.ConnectionModeDropDownList_SelectedIndexChanged(null, null);

                    this.RefreshIntervalTextBox.Text = site.RefreshInterval;

                    // Populate SelectSystemModeDropDownList
                    ListItem newItem = new ListItem("List", "0");
                    this.SelectSystemModeDropDownList.Items.Add(newItem);
                    newItem = new ListItem("Text", "1");
                    this.SelectSystemModeDropDownList.Items.Add(newItem);
                    this.SelectSystemModeDropDownList.SelectedIndex = 1;
                    this.SelectSystemModeDropDownList_SelectedIndexChanged(null, null);

                    this.ScadaSystemTextBox.Text = site.SCADASystem;
            
					//Enterprise Query Credentials
                    this.EnterpriseQueryUserNameTextbox.Text = site.EnterpriseUserId;
                    if (!string.IsNullOrEmpty(site.EnterprisePassword))
                    {
                        this.EnterpriseQueryPasswordTextbox.Attributes.Add("value", "**********");
                    }
                    else
                    {
                        this.EnterpriseQueryPasswordTextbox.Text = "";
                    }

                    this.EnterpriseQuerySiteGroupTextbox.Text = site.EnterpriseSite;
                }

                this.InitialMailPasswordTextBox.Text = this.MailPasswordTextBox.Text;
               
			}
			catch (Exception except)
			{
			    this.ErrorHandler(except);
			}
		}

		private void PopulatePrinterDropDowns(SiteClass site)
        {
			string[] installedPrinters= new string[0];
			try
            {
				installedPrinters = ReportServicePrintService.EnumeratePrinters("Site Alarm and Event");
				PopulateDropDown(this.AlarmAndEventPrinterDropDownList, installedPrinters, site.AlarmAndEventPrinter, true);
			}
            catch (SocketException socketExcept)
			{
				if (socketExcept.ErrorCode != 10061)
					throw;
				installedPrinters = new string[] { site.AlarmAndEventPrinter };
				PopulateDropDown(this.AlarmAndEventPrinterDropDownList, installedPrinters, site.AlarmAndEventPrinter, true);				
			}
		}

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		public void UpdateData()
		{
			SiteClass site = (SiteClass)this.Session["Site"];

			site.MaximumDaysToRetainLogs = this.MaximumDaysToRetainLogsTextBox.Text;
			site.MaximumDaysToRetainArchive = this.MaximumDaysToRetainArchiveTextBox.Text;
			site.EnableDebugLogging = this.EnableDebugLoggingCheckBox.Checked;
			site.EnableAuditLogging = this.EnableAuditLoggingCheckBox.Checked;
			site.AutomaticallyPrintAlarmsAndEvents = this.AutomaticallyPrintAlarmsAndEventsCheckBox.Checked;
			site.InhibitBOLSummaryAutoPopulate = this.InhibitBOLSummaryAutoSelection.Checked;
			site.InhibitOrderSummaryAutoPopulate = this.InhibitOrderSummaryAutoSelection.Checked;
			site.InhibitSupplyOrderSummaryAutoPopulate = this.InhibitSupplyOrderSummaryAutoSelection.Checked;

			if (this.AlarmAndEventPrinterDropDownList.SelectedIndex != -1)
				site.AlarmAndEventPrinter = this.AlarmAndEventPrinterDropDownList.SelectedItem.Text;

			site.InhibitTemplateGraphics = this.InhibitTemplateGraphicsCheckBox.Checked;
			site.InhibitEndOfDayOperations = this.InhibitEndOfDayCheckBox.Checked;
			site.InhibitEndOfMonthOperations = this.InhibitEndOfMonthCheckBox.Checked;
			site.EndOfDayWarningPeriod = this.EndOfDayWarningPeriodTextBox.Text;
			site.InhibitAutomaticPhysicalInventory = this.InhibitAutomaticPhysicalInventoryCheckBox.Checked;
			site.InhibitAutomaticMeterCloseout = this.InhibitAutomaticMeterCloseoutCheckBox.Checked;
			site.InhibitAutomaticReportGeneration = this.InhibitAutomaticReportGenerationCheckBox.Checked;
			site.InhibitAutomaticCloseout = this.InhibitAutomaticCloseoutCheckBox.Checked;
			site.InhibitTankScan = this.InhibitTankScanCheckBox.Checked;
            site.BlockCloseOnUnpostedBol = this.InhibitCloseoutOnUnpostedBol.Checked;
			site.MailServer = this.MailServerTextBox.Text;

            // Validate the email address provided in the Mail From field.
		    if (!this.MailFromTextBox.Text.IsValidEmailAddressSyntax())
		    {
                throw new FMEmailFormatException();
		    }

			site.MailFrom = this.MailFromTextBox.Text;
			site.MailUserName = this.MailUserNameTextBox.Text;
			if (this.MailPasswordTextBox.Text != string.Empty)
				site.MailPassword = this.MailPasswordTextBox.Text;

			if (this.ConnectionModeDropDownList.SelectedIndex != -1)
				site.MailConnectMode = (MAIL_SERVER_CONNECT_MODE)Convert.ToInt32(this.ConnectionModeDropDownList.SelectedValue);

			if (this.DialupNameDropDownList.SelectedIndex != -1)
				site.DialupName = this.DialupNameDropDownList.SelectedItem.Text;

			if (this.ScadaSystemDropDownList.Visible
			&& this.ScadaSystemDropDownList.SelectedIndex != -1)
				site.SCADASystem = this.ScadaSystemDropDownList.SelectedItem.Text;
			else
				site.SCADASystem = this.ScadaSystemTextBox.Text;

			site.RefreshInterval = this.RefreshIntervalTextBox.Text;
     
			//Enterprise Query Credentials
			site.EnterpriseUserId = this.EnterpriseQueryUserNameTextbox.Text;
		    if (this.EnterpriseQueryPasswordTextbox.Text != "**********")
		    {
                site.EnterprisePassword = this.EnterpriseQueryPasswordTextbox.Text;
            }             
		    site.EnterpriseSite = this.EnterpriseQuerySiteGroupTextbox.Text;
		}

        // ReSharper disable once InconsistentNaming
		protected void ConnectionModeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((MAIL_SERVER_CONNECT_MODE)Convert.ToInt32(this.ConnectionModeDropDownList.SelectedValue) == MAIL_SERVER_CONNECT_MODE.LAN)
			{
			    this.DialupNameDropDownList.Items.Clear();
			    this.DialupNameDropDownList.Enabled = false;
			    this.DialupNameDropDownList.BackColor = Color.LightGray;
			}
			else
			{
				// Populate DialupNameDropDownList
				SiteClass site = (SiteClass)this.Session["Site"];
			    this.DialupNameDropDownList.Enabled = true;
			    this.DialupNameDropDownList.BackColor = Color.White;

				uint size = 520;
				uint number;
				RASENTRYNAME[] rasEntryName = new RASENTRYNAME[1];
				rasEntryName[0].Size = 520;
				uint result;
				while (603 == (result = RasApi.RasEnumEntries(null, null, rasEntryName, ref size, out number)))
				{
					rasEntryName = new RASENTRYNAME[number];
					rasEntryName[0].Size = 520;
				}

				if (result == 0)
				{
					foreach (RASENTRYNAME entry in rasEntryName)
					{
						ListItem newDialupNameItem = new ListItem(entry.EntryName, this.DialupNameDropDownList.Items.Count.ToString());
						foreach (ListItem existingDialupNameItem in this.DialupNameDropDownList.Items)
						{
							if (string.Compare(existingDialupNameItem.Text, newDialupNameItem.Text, StringComparison.Ordinal) > 0)
							{
								int insert = this.DialupNameDropDownList.Items.IndexOf(existingDialupNameItem);
							    this.DialupNameDropDownList.Items.Insert(insert, newDialupNameItem);
								if (site.DialupName == newDialupNameItem.Text) this.DialupNameDropDownList.SelectedIndex = insert;
								newDialupNameItem = null;
								break;
							}
						}

						if (newDialupNameItem != null)
						{
						    this.DialupNameDropDownList.Items.Add(newDialupNameItem);
							if (site.DialupName == newDialupNameItem.Text) this.DialupNameDropDownList.SelectedIndex = this.DialupNameDropDownList.Items.Count - 1;
						}
					}
				}
			}
		}

        // ReSharper disable once InconsistentNaming
		protected void SelectSystemModeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.ScadaSystemDropDownList.Visible
			&& this.ScadaSystemDropDownList.SelectedIndex != -1)
			{
				if (this.ScadaSystemDropDownList.SelectedItem.Text == this.GetTranslatedText("{None}")) this.ScadaSystemTextBox.Text = "{None}";
				else this.ScadaSystemTextBox.Text = this.ScadaSystemDropDownList.SelectedItem.Text;
			}

		    this.ScadaSystemDropDownList.Visible = (this.SelectSystemModeDropDownList.SelectedIndex != 1);
		    this.ScadaSystemTextBox.Visible = !this.ScadaSystemDropDownList.Visible;

			// Only popluate the system drop down list when visible. 
			if (this.ScadaSystemDropDownList.Visible)
			{
			    this.PopulateSystemDropDownList();
			}
		}

		private void PopulateSystemDropDownList()
		{
			SiteClass site = (SiteClass)this.Session["Site"];

			// Populate SystemDropDownList
		    this.ScadaSystemDropDownList.Items.Clear();
			ListItem newItem = new ListItem(this.GetTranslatedText("{None}"), "0");
		    this.ScadaSystemDropDownList.Items.Add(newItem);
			newItem = new ListItem("localhost", "1");
		    this.ScadaSystemDropDownList.Items.Add(newItem);
			if ("localhost" == site.SCADASystem) this.ScadaSystemDropDownList.SelectedIndex = this.ScadaSystemDropDownList.Items.Count - 1;

			ServerEnumerator enumerator = new ServerEnumerator();
			string[] systems = enumerator.EnumerateHosts();
			int item = 2;
			foreach (string system in systems)
			{
				newItem = new ListItem(system, item.ToString());
			    this.ScadaSystemDropDownList.Items.Add(newItem);
				if (system == site.SCADASystem) this.ScadaSystemDropDownList.SelectedIndex = this.ScadaSystemDropDownList.Items.Count - 1;
				item++;
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	// ReSharper disable once InconsistentNaming
	public struct RASENTRYNAME
	{
		public uint Size;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 257)]
		public string EntryName;
	}

	public class RasApi
	{
		[DllImport("rasapi32.dll", CharSet = CharSet.Auto)]
		public static extern uint RasEnumEntries(string reserved,
			string szPhoneBook,
		// ReSharper disable InconsistentNaming
			[In, Out] RASENTRYNAME[] RasEntryName,
			ref uint Size,
			out uint Number);
        // ReSharper restore InconsistentNaming
    }


}
