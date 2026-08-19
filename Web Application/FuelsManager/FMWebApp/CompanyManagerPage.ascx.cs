/******************************************************************************

	FILE NAME:		CompanyManagerPage.ascx.cs


	PURPOSE:			Implementation of CompanyManagerPage


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------

*******************************************************************************/
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using FMBusinessObjects.DataObjects;

namespace FMWebApp
{
	using FuelsManager.FMWebApp;

	/// <summary>
	/// Summary description for CompanyManagerPage.
	/// </summary>
	public partial class CompanyManagerPage : CompanyPageBase
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				if(!Company.HasRole(COMPANY_ROLE.MANAGER))
					return;

				if (! Page.IsPostBack) 
				{
				}
			}	
			catch (Exception except)
			{
				ErrorHandler(except);
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion

		public void UpdateData()
		{
			if(!Company.HasRole(COMPANY_ROLE.MANAGER))
				return;
		}
	}
}
