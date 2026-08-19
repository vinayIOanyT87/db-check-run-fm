/******************************************************************************

	FILE NAME:		SiteReportsPage.ascx.cs


	PURPOSE:			Implementation of SiteReportsPage


	COMMENTS:

		Copyright (C) Varec, Inc.  All rights reserved.

		This file shall not be copied or reproduced in any form without
				the express written consent of Varec.


	AUTHOR(S):	P Reynolds


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------


*******************************************************************************/

namespace FuelsManager.FMWebApp
{
   using FMBusinessObjects.BusinessInterfaces;
   using FMBusinessObjects.ChannelFactories;
   using FMBusinessObjects.DataObjects;
   using FMBusinessObjects.Exceptions;
    using FMBusinessObjects.UtilityObjects;
   using FMBusinessServices.ServiceClasses;
   using System;
   using System.Collections.Generic;
   using System.Globalization;
    using System.Linq;
    using System.Net.Sockets;
   using System.Web.UI.WebControls;

   /// <summary>
   ///  Leak Detection Module set site setttings
   /// </summary>
	public partial class SiteReportsPage : FMUserControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                SiteClass site = (SiteClass)this.Session["Site"];

                if (!this.Page.IsPostBack)
                {

                    PopulatePrinterDropDowns(site);

                    this.ReportDirectoryTextBox.Text = site.ReportDirectory;
                    this.ManageReportsCheckBox.Checked = site.ManageReports;
                    this.ManagedReportDirectoryTextBox.Text = site.ManagedReportDirectory;
                    this.ManagedReportDirectoryTextBox.Enabled = site.ManageReports;
					     this.EnableAutomaticMovementTicketPrintingCheckBox.Checked = site.EnableAutomaticMovementTicketPrinting;
                    this.EnableMovementTicketArchivingCheckBox.Checked = site.EnableMovementTicketPDFArchiving;

                    this.Mvmt_Ticket_Archive_Directory.Text = site.MovementTicketFileExportDirectory;
                    this.Mvmt_Ticket_Export_FileName.Text = site.MovementTicketExportFileName;

                    this.PG_Export_Archive_Directory.Text = site.PointGroupFileExportDirectory;
                    this.PG_Export_Default_FileName.Text = site.PointGroupDefaultFileName;

					     var availabeReports = ReportService.GetReportsList(Security, site).ToArray();

                     if (site.SiteGroup == true || Security.HasRight(RIGHT.MODIFY_SITE_CLOSEOUT_TIME) == false)
                     {
                        this.CloseoutTimeControl.Enabled = false;
                     }
                     else if (site.CloseoutTime != null)
                     {
                        this.CloseoutTimeControl.Enabled = true;
                     }

                     if (site.SiteGroup == false 

                           && Security.HasRight(RIGHT.VIEW_ONLY_SITE_CLOSEOUT_TIME))
                     {
                        if (site.CloseoutTime == null)
                        {
                           site.CloseoutTime = new TimeSpan(23, 59, 59);
                        }
                        TimeSpan closeoutTime = site.CloseoutTime.Value;
                        DateTime t = new DateTime(2000,1,1,0,0,0,0);
                        this.CloseoutTimeControl.Text = t.Add(closeoutTime).ToString("T", site.GetDateTimeFormatInfo());
                     }

                     PopulateDropDown(this.MeterRecReportDropDownList, availabeReports, site.MeterReconciliationReportName, true);
                     PopulateDropDown(this.MovementTicketReportList, availabeReports, site.MovementTicketReportName, true);
                     PopulateDropDown(this.LeakReportList, availabeReports, site.LeakDetectionReport, true);
                }
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }

        private void PopulatePrinterDropDowns(SiteClass site)
        {
            string[] installedPrinters;
            try
            {
                installedPrinters = ReportServicePrintService.EnumeratePrinters("Site Reports");
                installedPrinters = installedPrinters.Distinct().OrderBy(p=>p).ToArray();
                PopulateDropDown(this.MovementTicketPrinter, installedPrinters, site.MovementTicketPrinter, true);
                PopulateDropDown(this.LeakReportPrinterDropDownList, installedPrinters, site.LeakDetectionPrinter, true);
            }
            catch (SocketException socketExcept)
            {
                if (socketExcept.ErrorCode != 10061)
                {
                    throw;
                }
                installedPrinters = new string[] { site.MovementTicketPrinter };
                PopulateDropDown(this.MovementTicketPrinter, installedPrinters, site.MovementTicketPrinter, true);
                installedPrinters = new string[] { site.LeakDetectionPrinter };
                PopulateDropDown(this.LeakReportPrinterDropDownList, installedPrinters, site.LeakDetectionPrinter, true);
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

            site.ReportDirectory = this.ReportDirectoryTextBox.Text;
            site.EnableAutomaticMovementTicketPrinting = this.EnableAutomaticMovementTicketPrintingCheckBox.Checked;
            site.EnableMovementTicketPDFArchiving = this.EnableMovementTicketArchivingCheckBox.Checked;
            site.ManageReports = this.ManageReportsCheckBox.Checked;
            site.ManagedReportDirectory = this.ManagedReportDirectoryTextBox.Text;
         
            DateTimeFormatInfo formatInfo = site.GetDateTimeFormatInfo();
            string dayOne = TimeConverter.MinFMDate.ToString("d", formatInfo);

            if (site.SiteGroup == false && this.CloseoutTimeControl.Enabled && Security.HasRight(RIGHT.MODIFY_SITE_CLOSEOUT_TIME))
            {

               TimeSpan t = TimeSpan.Zero;
               DateTime dt;
               if (this.CloseoutTimeControl.Text.Trim() == string.Empty)
               {
                  site.CloseoutTime = new TimeSpan(23, 59, 59);
               }
               else
               {
                  try
                  {
                     dt = DateTime.Parse(dayOne + " " + this.CloseoutTimeControl.Text);
                     site.CloseoutTime = dt.TimeOfDay;
                  }
                  catch
                  {
                     throw new Exception("Invalid closeout time.");
                  }
               }

            }
   
            site.PointGroupFileExportDirectory = this.PG_Export_Archive_Directory.Text;
            site.PointGroupDefaultFileName = this.PG_Export_Default_FileName.Text;
            site.MovementTicketFileExportDirectory = this.Mvmt_Ticket_Archive_Directory.Text;
            site.MovementTicketExportFileName = this.Mvmt_Ticket_Export_FileName.Text;

            if (this.MeterRecReportDropDownList.SelectedItem != null)
            {
                var selected = this.MeterRecReportDropDownList.SelectedItem.Text;
                site.MeterReconciliationReportName = DROP_DOWN_NONE.Equals(this.GetTranslatedText(selected)) ? string.Empty : selected;
            }

            if (this.MovementTicketReportList.SelectedItem != null)
            {
                var selected = this.MovementTicketReportList.SelectedItem.Text;
                site.MovementTicketReportName = DROP_DOWN_NONE.Equals(this.GetTranslatedText(selected)) ? string.Empty : selected;
            }

            if (this.MovementTicketPrinter.SelectedItem != null)
            {
                var selected = this.MovementTicketPrinter.SelectedItem.Text;
                site.MovementTicketPrinter = DROP_DOWN_NONE.Equals(this.GetTranslatedText(selected)) ? string.Empty : selected;
            }

            if (this.LeakReportPrinterDropDownList.SelectedItem != null)
            {
                var selected = this.LeakReportPrinterDropDownList.SelectedItem.Text;
                site.LeakDetectionPrinter = DROP_DOWN_NONE.Equals(this.GetTranslatedText(selected)) ? string.Empty : selected;
            }

            if (this.LeakReportList.SelectedItem != null)
            {
                var selected = this.LeakReportList.SelectedItem.Text;
                site.LeakDetectionReport = DROP_DOWN_NONE.Equals(this.GetTranslatedText(selected)) ? string.Empty : selected;
            }

        }
	}
}
