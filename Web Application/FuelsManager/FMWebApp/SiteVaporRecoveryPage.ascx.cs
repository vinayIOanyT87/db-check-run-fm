/******************************************************************************
	FILE NAME:		SiteVaporRecoveryPage.ascx.cs
	PURPOSE:		Implementation of SiteVaporRecoveryPage

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	W. Gray
	VERSION:	1.0.0  Current version



	MODIFICATION HISTORY:
		Date:		By:					Reason:
		---------	-----------------	-------------------------------------------
		2006-10-24	Richard Panachida	Fixed data dictionary. Some labels are not inhieriting
										from FMControls (CSI 3405).
										
		2007-07-30	I.Orndorff			1.0.0.2 - Changed the following controls from 
												  FMControls.FMLabel to System.Web.UI.WebControls.Label: 
												  RateUnitsLabel, HourlyUnitsLabel, DailyUnitsLabel, 
												  YearlyUnitsLabel and CurrentYearUnitsLabel. 
												  This fixes CSI #4670.
 
*******************************************************************************/

namespace FuelsManager.FMWebApp
{
    using System;
    using System.Security;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.DataObjects;

    using Opc.Da;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    /// <summary>
	/// Summary description for SiteVaporRecoveryPage.
	/// </summary>
	public partial class SiteVaporRecoveryPage : FMUserControlBase
	{
		protected TextBox VruCurrentYearTextBox;

		[SecurityCritical]
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{

				SiteClass	site=(SiteClass) this.Session["Site"];

				if (! this.Page.IsPostBack) 
				{
					this.VRURateLimitEnabledCheckBox.Checked			= site.VRURateLimitEnabled;
					this.VRUHourlyLimitEnabledCheckBox.Checked		= site.VRUHourlyLimitEnabled;
					this.VRUDailyLimitEnabledCheckBox.Checked		= site.VRUDailyLimitEnabled;
					this.VRUYearlyLimitEnabledCheckBox.Checked		= site.VRUYearlyLimitEnabled;
					this.VRUCurrentYearLimitEnabledCheckBox.Checked= site.VRUCurrentYearLimitEnabled;

					ProcessVariableClass vruSetpointPv=site.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.VRU_SETPOINT_PV];
					if(vruSetpointPv != null)
					{
						EngineeringUnit units=site.GetSiteUnits(vruSetpointPv.SiteVariableType);
						byte decimalPlaces=site.GetSiteDecimalPlaces(vruSetpointPv.SiteVariableType);
						this.SetpointTextBox.Text=vruSetpointPv.Encode(	vruSetpointPv.GetValue(units,decimalPlaces),
																					new Quality(vruSetpointPv.OPCQuality),
																					units,
																					site.GetNumberFormatInfo(vruSetpointPv.SiteVariableType));
					}

					ProcessVariableClass vruDeadbandPv=site.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.VRU_DEADBAND_PV];
					if(vruDeadbandPv != null)
					{
						EngineeringUnit units=site.GetSiteUnits(vruDeadbandPv.SiteVariableType);
						byte decimalPlaces=site.GetSiteDecimalPlaces(vruDeadbandPv.SiteVariableType);
						this.DeadbandTextBox.Text=vruDeadbandPv.Encode(	vruDeadbandPv.GetValue(units,decimalPlaces),
																					new Quality(vruDeadbandPv.OPCQuality),
																					units,
																					site.GetNumberFormatInfo(vruDeadbandPv.SiteVariableType));
					}
				}
				else
					this.UpdateData();

				// "Actual" text boxes are for display only
				this.VRURateActualTextBox.ReadOnly = true;
				this.VRUCurrentYearActualTextBox.ReadOnly = true;
				this.VRUDailyActualTextBox.ReadOnly = true;
				this.VRUHourlyActualTextBox.ReadOnly = true;
				this.VRUYearlyActualTextBox.ReadOnly = true;

				if (this.Security.LoginSiteGuid == site.IdentityGuid)
				{
					site._VRURateLimit.Format.NumberDecimalDigits=site._VolumeDecimalPlaces;
					site._VRUHourlyLimit.Format.NumberDecimalDigits=site._VolumeDecimalPlaces;
					site._VRUDailyLimit.Format.NumberDecimalDigits=site._VolumeDecimalPlaces;
					site._VRUYearlyLimit.Format.NumberDecimalDigits=site._VolumeDecimalPlaces;
					site._VRUCurrentYearLimit.Format.NumberDecimalDigits=site._VolumeDecimalPlaces;
					site._VRURateActual.Format.NumberDecimalDigits=site._VolumeDecimalPlaces;
					site._VRUHourlyActual.Format.NumberDecimalDigits=site._VolumeDecimalPlaces;
					site._VRUDailyActual.Format.NumberDecimalDigits=site._VolumeDecimalPlaces;
					site._VRUYearlyActual.Format.NumberDecimalDigits=site._VolumeDecimalPlaces;
					site._VRUCurrentYearActual.Format.NumberDecimalDigits=site._VolumeDecimalPlaces;

					site._VRURateLimit.Units=site.VolumeUnits;
					site._VRUHourlyLimit.Units=site.VolumeUnits;
					site._VRUDailyLimit.Units=site.VolumeUnits;
					site._VRUYearlyLimit.Units=site.VolumeUnits;
					site._VRUCurrentYearLimit.Units=site.VolumeUnits;
					site._VRURateActual.Units=site.VolumeUnits;
					site._VRUHourlyActual.Units=site.VolumeUnits;
					site._VRUDailyActual.Units=site.VolumeUnits;
					site._VRUYearlyActual.Units=site.VolumeUnits;
					site._VRUCurrentYearActual.Units=site.VolumeUnits;
				}

				this.VRURateLimitTextBox.Text						= site.VRURateLimit;
				this.VRUHourlyLimitTextBox.Text						= site.VRUHourlyLimit;
				this.VRUDailyLimitTextBox.Text						= site.VRUDailyLimit;
				this.VRUYearlyLimitTextBox.Text						= site.VRUYearlyLimit;
				this.VRUCurrentYearLimitTextBox.Text				= site.VRUCurrentYearLimit;

				this.VRURateActualTextBox.Text						= site.VRURateActual;
				this.VRUHourlyActualTextBox.Text					= site.VRUHourlyActual;
				this.VRUDailyActualTextBox.Text						= site.VRUDailyActual;
				this.VRUYearlyActualTextBox.Text					= site.VRUYearlyActual;
				this.VRUCurrentYearActualTextBox.Text				= site.VRUCurrentYearActual;



				string abbrevString=EngineeringUnits.GetUnitAbbreviation(site.VolumeUnits);
				this.RateUnitsLabel.Text=abbrevString;
				this.HourlyUnitsLabel.Text=abbrevString;
				this.DailyUnitsLabel.Text=abbrevString;
				this.YearlyUnitsLabel.Text=abbrevString;
				this.CurrentYearUnitsLabel.Text=abbrevString;
			}	
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
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
			SiteClass	site=(SiteClass) this.Session["Site"];

			site.VRURateLimit				= this.VRURateLimitTextBox.Text;
			site.VRUHourlyLimit			= this.VRUHourlyLimitTextBox.Text;
			site.VRUDailyLimit			= this.VRUDailyLimitTextBox.Text;
			site.VRUYearlyLimit			= this.VRUYearlyLimitTextBox.Text;
			site.VRUCurrentYearLimit	= this.VRUCurrentYearLimitTextBox.Text;

			site.VRURateLimitEnabled			= this.VRURateLimitEnabledCheckBox.Checked;
			site.VRUHourlyLimitEnabled			= this.VRUHourlyLimitEnabledCheckBox.Checked;
			site.VRUDailyLimitEnabled			= this.VRUDailyLimitEnabledCheckBox.Checked;
			site.VRUYearlyLimitEnabled			= this.VRUYearlyLimitEnabledCheckBox.Checked;
			site.VRUCurrentYearLimitEnabled	= this.VRUCurrentYearLimitEnabledCheckBox.Checked;

			ProcessVariableClass vruSetpointPv=site.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.VRU_SETPOINT_PV];
			if(vruSetpointPv != null)
			{
                EngineeringUnit units = site.GetSiteUnits(vruSetpointPv.SiteVariableType);
                vruSetpointPv.SetValue(this.SetpointTextBox.Text, units, site.GetNumberFormatInfo(vruSetpointPv.SiteVariableType));
			}

			ProcessVariableClass vruDeadbandPv=site.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.VRU_DEADBAND_PV];
			if(vruDeadbandPv != null)
			{
                EngineeringUnit units = site.GetSiteUnits(vruDeadbandPv.SiteVariableType);
                vruDeadbandPv.SetValue(this.DeadbandTextBox.Text, units, site.GetNumberFormatInfo(vruDeadbandPv.SiteVariableType));
			}
		}
	}
}
