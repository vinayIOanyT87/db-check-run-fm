// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportLandingPage.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ReportLandingPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMReportWebMain
{
	using System;
	using System.Collections;
	using System.Configuration;
	using System.Linq;
	using System.Net;
	using System.Security.Principal;
	using System.Web.UI.WebControls;
	using System.Xml.Linq;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using FMWebApp;

	using Microsoft.Reporting.WebForms;

	public partial class ReportLandingPage : FMFormBase
	{
		#region Constants and Fields

		private ManageSecurity manageSecurity;

		private ParameterParser parmParser;

		private RequestParser requestParser;

		#endregion

		#region Methods

		/// <summary>
		/// Override onInit so we can tell the application to not disable buttons
		/// during a postback. In some versions of IE (10 or earlier) the "View Report"
		/// button was staying disabled after doing something like exporting the report.
		/// Microsoft seems to handle the enabling and disabling themselves, so we don't need to use
		/// the FuelsManager script which disables buttons.
		/// </summary>
		/// <param name="e">EventArgs containing event data</param>
		protected override void OnInit(EventArgs e)
        {
			this.IgnoreInputDisable = true;
			base.OnInit(e);
        }

		protected void Page_Load(object sender, EventArgs e)
		{
         this.ErrorLabel.Visible = true;
			this.RptViewer.Visible = true;

			if (FMChannelHelper.MakeCall<IHardwareKey, Boolean>(x => x.IsADFKey()))
			{
				this.RptViewer.ShowPrintButton = false;
			}

			if (this.Page.IsPostBack == false)
			{
				// Build the security object to ensure that the request is coming
				// from the FuelsManager application.
				this.manageSecurity = new ManageSecurity();
				var security = this.Session["Security"] as SecurityClass;
				this.manageSecurity.BuildSecurity(security);

				// If the request is valid, then parse the request and create the report viewer parameters.
				if (this.manageSecurity.IsSecurityValid)
				{
					this.requestParser = new RequestParser(this.manageSecurity);
					Hashtable rptParms = this.requestParser.ParseRequest(this.Request);

					if (this.requestParser.ReportType == ReportTypesClass.ReportTypes.BOL_RPT)
					{
						this.ucFMMenuBar.ShowInDialog = true;
					}

					// If the report parameters were parsable and the request information was
					// correct, then build the report viewer parameters.
					if (rptParms != null)
					{
						var rptLocation = new ReportLocation(this.manageSecurity);
						this.RptViewer.ProcessingMode = ProcessingMode.Remote;

						if (this.requestParser.IsPopupDisplay)
						{
							var width = new Unit(1100, UnitType.Pixel);
							var height = new Unit(950, UnitType.Pixel);

							this.RptViewer.SizeToReportContent = true;
							ucFMMenuBar.HideEvenIfNotDialog = true;
						}
						else
						{
							// Get the configured report viewer size. 
							Hashtable reportViewSizeHshTbl = this.GetViewAreaSize();
							var configWidth = reportViewSizeHshTbl["width"] as int?;
							var configHeight = reportViewSizeHshTbl["height"] as int?;

                     // JS20100722 WI-16051 Changes default unit to a 100% value
                     var width = new Unit();
                     var height = new Unit();

							if (configWidth.HasValue)
							{
								if (configWidth.Value <= 0)
								{
									width = new Unit(100, UnitType.Percentage);
								}
							}
                     if (configHeight.HasValue)
                     {
                        if (configHeight.Value <= 0)
                        {
                           height = new Unit(100, UnitType.Percentage);
                        }
                     }
                     if (width.IsEmpty && configWidth != null)
							{
								width = new Unit(configWidth.Value, UnitType.Pixel);
							}
							if (height.IsEmpty && configHeight != null)
							{
								height = new Unit(configHeight.Value, UnitType.Pixel);
							}

							this.RptViewer.Width = width;
							this.RptViewer.Height = height;
						}

						try
						{
							this.RptViewer.ServerReport.ReportServerUrl = new Uri(rptLocation.ReportServerUri);
							//replace // with / if necessary.  ReportPath in db may or may not have preceeding /
							string rptDir =
								FMChannelHelper.MakeCall<ISites, string>(x => x.GetReportDirectory(security, this.requestParser.ReportName));

							this.RptViewer.ServerReport.ReportPath = (rptDir + "/" + this.requestParser.ReportName).Replace("//", "/");
							this.RptViewer.ShowParameterPrompts = true;
							this.RptViewer.ShowPromptAreaButton = true;
							this.RptViewer.ZoomMode = ZoomMode.Percent;
							this.RptViewer.ZoomPercent = 100;


							this.RptViewer.ServerReport.ReportServerCredentials = new ReportServerCredentials(security);

							ReportParameterInfoCollection rptParmCollection = this.RptViewer.ServerReport.GetParameters();
							this.parmParser = new ParameterParser(rptParmCollection);

							ReportParameter[] finalReportParam = this.parmParser.ParseParameters(rptParms);
							if (finalReportParam != null)
							{
								this.RptViewer.ServerReport.SetParameters(finalReportParam);
							}
						}
						catch (Exception ex)
						{
							this.RptViewer.Visible = false;
							this.ErrorLabel.Visible = true;
							this.ErrorLabel.Text = "Error in rendering report.";
							this.ErrorHandler(ex);
						}
					}
				}
				else
				{
					this.RptViewer.Visible = false;
					this.ErrorLabel.Visible = true;
					this.ErrorLabel.Text = "Error in render report.";
				}
			}
		}

		/// <summary>
		///    This method will return a hash table with the width and height of the configured
		///    report viewer. The default width = 900 and height = 600.
		/// </summary>
		/// <returns></returns>
		private Hashtable GetViewAreaSize()
		{
			int defaultHeight = 0;
			int defaultWidth = 0;

			if (ConfigurationManager.AppSettings["TransactionFieldConfiguration"] != null)
			{
				string configFileName = ConfigurationManager.AppSettings["TransactionFieldConfiguration"];

				if (string.IsNullOrEmpty(configFileName) == false)
				{
					try
					{
						string fileAndPath = this.Page.Server.MapPath(configFileName);

						// The configuration file should be in the FuelsManager directory and
						// not in the FMReporting directory.
						if (string.IsNullOrEmpty(fileAndPath) == false)
						{
                            fileAndPath = fileAndPath.Replace("\\FMReporting", "").Replace("\\FMReportWebMain", "");
						}

						XDocument reportViewerSizeXML = XDocument.Load(fileAndPath);

						var reportViewerSizes = from reportSize in reportViewerSizeXML.Descendants("ReportViewSize").DefaultIfEmpty(null) where reportSize != null
							select new { width = reportSize.Attribute("width").Value, height = reportSize.Attribute("height").Value };

						try
						{
							var reportViewerSize = reportViewerSizes.FirstOrDefault();

							if (reportViewerSize != null)
							{
								defaultWidth = Convert.ToInt32(reportViewerSize.width);
								defaultHeight = Convert.ToInt32(reportViewerSize.height);
							}
						}
							// ReSharper disable once EmptyGeneralCatchClause
						catch
						{
						}

					}
						// ReSharper disable once EmptyGeneralCatchClause
					catch
					{
					}
				}
			}

			var reportViewerSizeHshTbl = new Hashtable { { "width", defaultWidth }, { "height", defaultHeight } };

			return reportViewerSizeHshTbl;
		}

		#endregion
	}

	/// <summary>
	///    Implementation of IReportServerCredentials to supply forms credentials to SQL Reporting using GetFormsCredentials()
	/// </summary>
	[Serializable]
	public class ReportServerCredentials : IReportServerCredentials
	{
		#region Constants and Fields

		private readonly SecurityClass security;

		#endregion

		#region Constructors and Destructors

		public ReportServerCredentials()
		{
		}

		public ReportServerCredentials(SecurityClass security)
		{
			this.security = security;
		}

		#endregion

		#region Public Properties

		public WindowsIdentity ImpersonationUser
		{
			get
			{
				return null;
			}
		}

		public ICredentials NetworkCredentials
		{
			get
			{
				//get credentials from systemsetting
				SystemSettingClass systemSetting =
					FMChannelHelper.MakeCall<ISystemSettings, SystemSettingClass>(x => x.Get(this.security));

				if (!string.IsNullOrEmpty(systemSetting.ReportServerUserName))
				{
					string[] userName = systemSetting.ReportServerUserName.Split('\\');
					if (userName.Length > 1)
					{
						return new NetworkCredential(userName[1], systemSetting.ReportServerPassword, userName[0]);
					}

					return new NetworkCredential(userName[0], systemSetting.ReportServerPassword, ".");
				}

				return CredentialCache.DefaultCredentials;
			}
		}

		#endregion

		#region Public Methods and Operators

		public bool GetFormsCredentials(out Cookie authCookie, out string userName, out string password, out string authority)
		{
			authCookie = null;
			userName = null;
			password = null;
			authority = null;

			// Not using form credentials
			return false;
		}

		#endregion
	}
}