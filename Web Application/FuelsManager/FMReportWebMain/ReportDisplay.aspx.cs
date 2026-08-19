// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportDisplay.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ReportDisplay type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMReportWebMain
{
	using System;
	using System.Net;
	using System.Security.Principal;
	using System.Web.UI;

	using Microsoft.Reporting.WebForms;

	public partial class ReportDisplay : Page
	{
		//protected void Page_Load(object sender, EventArgs e)
		//{
		//    if (Page.IsPostBack == false)
		//    {

		#region Methods

		protected void Page_Init(object sender, EventArgs e)
		{
			this.ReportViewer1.ProcessingMode = ProcessingMode.Remote;
			//ReportViewer1.ServerReport.ReportServerUrl = new Uri(String.Format("https://{0}/reportserver", ConfigurationManager.AppSettings["SERVER_NAME"]));
			this.ReportViewer1.ServerReport.ReportServerUrl = new Uri("https://zwsplvbqgv.reporting.windows.net/reportserver");
			this.ReportViewer1.ServerReport.ReportPath = "/TestReport.rdl";
			this.ReportViewer1.ServerReport.ReportServerCredentials = new ReportServerCredentialsDisplay();
			this.ReportViewer1.Visible = true;

			this.ReportViewer1.SizeToReportContent = true;

			this.ReportViewer1.ShowParameterPrompts = true;
			this.ReportViewer1.ShowPromptAreaButton = true;
			this.ReportViewer1.ZoomMode = ZoomMode.Percent;
			this.ReportViewer1.ZoomPercent = 100;
		}

		#endregion
	}

	/// <summary>
	///    Implementation of IReportServerCredentials to supply forms credentials to SQL Reporting using GetFormsCredentials()
	/// </summary>
	[Serializable]
	public class ReportServerCredentialsDisplay : IReportServerCredentials
	{
		//private SecurityClass security;

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
				return null;
			}
		}

		#endregion

		#region Public Methods and Operators

		public bool GetFormsCredentials(out Cookie authCookie, out string user, out string password, out string authority)
		{
			authCookie = null;
			user = "Brian";
			password = "!Qaz2Wsx";
			authority = "https://zwsplvbqgv.reporting.windows.net/reportserver";
			return true;
		}

		#endregion
	}
}