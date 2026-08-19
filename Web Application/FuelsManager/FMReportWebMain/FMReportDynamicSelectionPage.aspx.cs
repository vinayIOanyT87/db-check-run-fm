// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMReportDynamicSelectionPage.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMReportDynamicSelectionPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMReportWebMain
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.ServiceRequests;

    using FMCore;

	using FMWebApp;

	public partial class FMReportDynamicSelectionPage : FMFormBase
	{
		#region Constants and Fields

		private const int EMPTY_STRING = 0;

		private Guid currentSiteGuid;
		private ReportConfigurationDetailListDO detailListDO;
		private ReportConfigurationGroupListDO groupListDO;
		private SecurityClass security;
		private string site;

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

		/// <summary>
		///    This is the main entry point to the report dynamic selection page. It is called by
		///    the IIS server.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (this.SetupSecuritySite())
			{
				// Retrieve the report data and build the dynamic report selection page.
				this.RetrieveReportSelectionData();
				this.BuildDynamicPage();

				// Apply data dictionary to this page.
				this.ApplyDataDictionary();
			}
		}

		/// <summary>
		///    This method will apply the data dictionary to this page.  If the data dictionary
		///    use flag is set to true, then it will apply data dictionary.
		/// </summary>
		private void ApplyDataDictionary()
		{
			if (this.Page.Session["UseDataDictionary"] == null || (bool)this.Page.Session["UseDataDictionary"])
			{
				string newText = this.GetTranslatedText(this.ReportPageTitle.Text);
				this.ReportPageTitle.Text = newText;
			}
		}

		/// <summary>
		///    This method will create the report dynamic selection page using the report configuration
		///    data from the database.
		/// </summary>
		private void BuildDynamicPage()
		{
			// Ensure that our report configuration data exists.
			if ((this.groupListDO != null) && (this.detailListDO != null))
			{
				List<ReportConfigurationGroupDO> groupList = this.groupListDO.ReportGroupDOList;
				List<ReportConfigurationDetailDO> detailList = this.detailListDO.ReportDetailDOList;

				bool firstGroup = true;

				// For each group, find the report detail records and group under the current
				// group.
				foreach (ReportConfigurationGroupDO groupDO in groupList)
				{
					// Find each report detail record associated to the current group and
					// create an entry under the group.

					bool firstReport = true;

					foreach (ReportConfigurationDetailDO detailDO in detailList)
					{
						if (detailDO.ReportGroupGuid != groupDO.ReportGroupGuid)
						{
							continue;
						}

						if (firstReport)
						{
							// Create the group section.
							if (firstGroup == false)
							{
								this.MainPanel.Controls.Add(this.CreateLiteral("<BR><BR>"));
							}

							firstGroup = false;
							this.MainPanel.Controls.Add(this.CreateGroupLabel(groupDO.GroupName, groupDO.ReportGroupGuid));
							this.MainPanel.Controls.Add(this.CreateLiteral("<BR>"));

							firstReport = false;
						}

						this.MainPanel.Controls.Add(this.CreateLinkButton(detailDO.ReportName, detailDO.ReportGuid));
						this.MainPanel.Controls.Add(this.CreateLiteral("<BR>"));
						this.MainPanel.Controls.Add(this.CreateDescriptionLabel(detailDO.ReportDescription, detailDO.ReportGuid));
						this.MainPanel.Controls.Add(this.CreateLiteral("<BR><BR>"));
					}
				}

				// Find each report that is not associated with a group and create a report
				// entry.
				foreach (ReportConfigurationDetailDO detailDO in detailList)
				{
					if (detailDO.ReportGroupGuid == Guid.Empty)
					{
						this.MainPanel.Controls.Add(this.CreateLinkButton(detailDO.ReportName, detailDO.ReportGuid));
						this.MainPanel.Controls.Add(this.CreateLiteral("<BR>"));
						this.MainPanel.Controls.Add(this.CreateDescriptionLabel(detailDO.ReportDescription, detailDO.ReportGuid));
						this.MainPanel.Controls.Add(this.CreateLiteral("<BR><BR>"));
					}
				}
			}
		}

		/// <summary>
		///    This method will create a label control for a report description. It will return the a label
		///    control object.
		/// </summary>
		/// <param name="reportDescription"></param>
		/// <param name="reportGuid"></param>
		/// <returns></returns>
		private Label CreateDescriptionLabel(string reportDescription, Guid reportGuid)
		{
			var descLabel = new Label { ID = "ReportDescGuid" + reportGuid, Text = reportDescription, CssClass = "paratext" };

			return descLabel;
		}

		/// <summary>
		///    This method will create a label control for a group. It will return the a label
		///    control object.
		/// </summary>
		/// <param name="groupName"></param>
		/// <param name="groupGuid"></param>
		/// <returns></returns>
		private Label CreateGroupLabel(string groupName, Guid groupGuid)
		{
			var groupLabel = new Label();
			var fontUnit = new FontUnit("12pt");

			groupLabel.ID = "GroupGuid" + groupGuid;
			groupLabel.Text = groupName;
			groupLabel.CssClass = "parasubheadline";
			groupLabel.Font.Size = fontUnit;

			return groupLabel;
		}

		/// <summary>
		///    This method will create a new link button with the report name and add the callback
		///    event to the event list. It will return a new link button.
		/// </summary>
		/// <param name="reportName"></param>
		/// <param name="reportGuid"></param>
		/// <returns></returns>
		private LinkButton CreateLinkButton(string reportName, Guid reportGuid)
		{
			var linkBtn = new LinkButton();
			var fontUnit = new FontUnit("11pt");

			linkBtn.ID = "ReportGuid" + reportGuid;
			linkBtn.Text = reportName;
			linkBtn.CommandName = "Command" + reportGuid;
			linkBtn.CssClass = "paralink";
			linkBtn.Font.Size = fontUnit;

			linkBtn.Command += this.LinkBtnCommand;

			return linkBtn;
		}

		/// <summary>
		///    This method will create and return a literal control of the string that was passed
		///    into the method. The string shall be of HTML type.
		/// </summary>
		/// <param name="literalStr"></param>
		/// <returns></returns>
		private Literal CreateLiteral(string literalStr)
		{
			var literalCntl = new Literal { Text = literalStr };

			return literalCntl;
		}

		/// <summary>
		///    This method will determine the product solution.  It will return true if the product is
		///    aviation.  Otherwise, it will return false.
		/// </summary>
		/// <returns></returns>
		private bool GetIndustrySolution()
		{
			return FMChannelHelper.MakeCall<IHardwareKey, Boolean>(x => x.IsAviationProduct());
		}

		/// <summary>
		///    This method will return the report Guid of the report that was selected from the
		///    report selection page. It will return -1 if there is a parse error.
		/// </summary>
		/// <param name="commandStr"></param>
		/// <returns></returns>
		private Guid GetReportGuid(string commandStr)
		{
			Guid reportGuid = Guid.Empty;

			if (string.IsNullOrEmpty(commandStr) == false)
			{
				if ((commandStr.IndexOf("Command", 0, StringComparison.Ordinal) > -1) && (commandStr.Length > 7))
				{
					int length = commandStr.Length - 7;
					string guidStr = commandStr.Substring(7, length);
					reportGuid = Guid.Parse(guidStr);
				}
			}

			return reportGuid;
		}

		/// <summary>
		///    This method will check to see if there is an error, if so, then it will display an
		///    error dialog and transfer control to the error page.
		/// </summary>
		/// <param name="errMsg"></param>
		private void HandleErrorCondition(string errMsg)
		{
			if (string.IsNullOrEmpty(errMsg) == false)
			{
				this.ErrorHandler(new Exception(errMsg));
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		///    This method will retrieve the report configuration data in order to display the
		///    report selection page.
		/// </summary>
		private void RetrieveReportSelectionData()
		{
			var groupSR = new ReportConfigurationGroupSR();
			var detailSR = new ReportConfigurationDetailSR();

			if (!string.IsNullOrEmpty(this.Request.GetQueryOrFormValue("GROUP")))
			{
				groupSR.RequestType = ReportConfigurationGroupSR.RequestTypes.GET_BY_NAME;
				groupSR.ReportConfigurationGroupDO = new ReportConfigurationGroupDO
				                                     {
					                                     GroupName =
						                                     this.Request.GetQueryOrFormValue("GROUP")
				                                     };
				groupSR.Site = this.site;
				groupSR.CurrentSiteGuid = this.currentSiteGuid;
				groupSR.Security = this.security;

				try
				{
					ReportConfigurationGroupDO groupDO =
						FMChannelHelper.MakeCall<IReportConfigurationGroupProcessor, ReportConfigurationGroupDO>(
							x => x.GetByName(groupSR));

					this.groupListDO = new ReportConfigurationGroupListDO();
					this.groupListDO.ReportGroupDOList.Add(groupDO);
				}
				catch (Exception ex)
				{
					this.HandleErrorCondition(ex.Message);
				}
			}
			else
			{
				groupSR.RequestType = ReportConfigurationGroupSR.RequestTypes.GET_ALL;
				groupSR.Site = this.site;
				groupSR.CurrentSiteGuid = this.currentSiteGuid;
				groupSR.Security = this.security;

				try
				{
					this.groupListDO =
						FMChannelHelper.MakeCall<IReportConfigurationGroupProcessor, ReportConfigurationGroupListDO>(
							x => x.GetAll(groupSR));
				}
				catch (Exception ex)
				{
					this.HandleErrorCondition(ex.Message);
				}
			}

			detailSR.RequestType = ReportConfigurationDetailSR.RequestTypes.GET_ALL_NON_PRINT;
			detailSR.Site = this.site;
			detailSR.CurrentSiteGuid = this.currentSiteGuid;
			detailSR.Security = this.security;

			try
			{
				this.detailListDO =
					FMChannelHelper.MakeCall<IReportConfigurationDetailProcessor, ReportConfigurationDetailListDO>(
						x => x.GetAllNonPrint(detailSR));
			}
			catch (Exception ex)
			{
				this.HandleErrorCondition(ex.Message);
			}
		}

		/// <summary>
		///    This method will initialize the security and data dictionary classes.  It is called by
		///    the OnInit method.
		/// </summary>
		private bool SetupSecuritySite()
		{
			this.currentSiteGuid = Guids.SiteAdminGuid;
			bool isValid = false;

			// Use the token retrieved from the cookie in order to retrieve 
			// the security class for a given site.  Add the security class 
			// to the session.
			this.security = (SecurityClass)this.Session["Security"];

			// Setup a default security object since the real one
			// could be found.
			if (this.security == null)
			{
				this.ErrorHandler(new Exception(FMSessionInvalidException.SessionNotFoundExceptionMessage));
			}
			else
			{
				isValid = true;
				this.currentSiteGuid = this.security.SiteGuid;
				this.site = this.security.SiteID;
			}

			return isValid;
		}

		/// <summary>
		///    This method is called by selecting a report from the current page. It will determine which
		///    report was selected from the command event arguments parameter and retrieve the corresponding
		///    report URL for the selected report. It will then transfer control to the reports landing page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="commandEventArgs"></param>
		private void LinkBtnCommand(object sender, CommandEventArgs commandEventArgs)
		{
			Guid reportGuid = this.GetReportGuid(commandEventArgs.CommandName);

			if (reportGuid == Guid.Empty)
			{
				this.ErrorHandler(new Exception("Invalid Command for selected report: " + commandEventArgs.CommandName));
			}
			else
			{
				var detailSR = new ReportConfigurationDetailSR();
				var detailDO = new ReportConfigurationDetailDO();

				detailSR.RequestType = ReportConfigurationDetailSR.RequestTypes.GET;
				detailSR.Site = this.site;
				detailSR.CurrentSiteGuid = this.currentSiteGuid;
				detailDO.ReportGuid = reportGuid;
				detailDO.SiteGuid = this.currentSiteGuid;
				detailSR.ReportConfigurationDetailDO = detailDO;
				detailSR.Security = this.security;

				try
				{
					detailDO =
						FMChannelHelper.MakeCall<IReportConfigurationDetailProcessor, ReportConfigurationDetailDO>(
							x => x.GetConfiguration(detailSR));
				}
				catch (Exception ex)
				{
					this.HandleErrorCondition(ex.Message);
				}


				// Retrieve the system report URL and directory information from the BLL.
				try
				{
					// Concatenate the URL, directory, and report name.
					string reportName = detailDO.ReportPath.Replace(" ", "+");
					string rptUrl = "ReportLandingPage.aspx?ReportType=";

					if (this.GetIndustrySolution())
					{
						string aviation = ((int)ReportTypesClass.ReportTypes.AVIATION_RPT).ToString(CultureInfo.InvariantCulture);
						rptUrl = rptUrl + aviation;
					}
					else
					{
						string oilAndGas = ((int)ReportTypesClass.ReportTypes.OIL_GAS_RPT).ToString(CultureInfo.InvariantCulture);
						rptUrl = rptUrl + oilAndGas;
					}

					rptUrl = rptUrl + "&ReportName=" + reportName;
					this.Redirect(rptUrl);
				}
				catch (Exception ex)
				{
					this.HandleErrorCondition(ex.Message);
				}
			}
		}

		#endregion
	}
}