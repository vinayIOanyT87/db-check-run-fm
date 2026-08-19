/******************************************************************************

	FILE NAME:		SiteUnitsPage.ascx.cs


	PURPOSE:			Implementation of SiteUnitsPage


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		06/16/2008	W.Gray		7.4.5.0 - Added AdditiveProfile Units and Decimal Places (CSI 5960)

*******************************************************************************/

namespace FuelsManager.FMWebApp
{
    using System;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.DataObjects;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    /// <summary>
	/// Summary description for SiteUnitsPage.
	/// </summary>
	public partial class SiteUnitsPage : FMUserControlBase
	{

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				SiteClass	site=(SiteClass) this.Session["Site"];

				if (! this.Page.IsPostBack) 
				{
					this.InitializeUnitsDropDownList(this.LevelUnitsDropDownList,EngineeringUnit.FmlFtIn8Th,EngineeringUnit.FmlMile,site.LevelUnits);
					this.InitializeUnitsDropDownList(this.VolumeUnitsDropDownList,EngineeringUnit.FmvCm3,EngineeringUnit.FmvKl,site.VolumeUnits);
					this.InitializeUnitsDropDownList(this.AdditiveVolumeUnitsDropDownList,EngineeringUnit.FmvCm3,EngineeringUnit.FmvKl,site.AdditiveVolumeUnits);
					this.InitializeUnitsDropDownList(this.TemperatureUnitsDropDownList,EngineeringUnit.FmtDegC,EngineeringUnit.FmtDegR,site.TemperatureUnits);
					this.InitializeUnitsDropDownList(this.DensityUnitsDropDownList,EngineeringUnit.FmdGcm3,EngineeringUnit.FmdSTnYd3,site.DensityUnits);
					this.InitializeUnitsDropDownList(this.MassUnitsDropDownList,EngineeringUnit.FmmGram,EngineeringUnit.FmmMlbs,site.MassUnits);
					this.InitializeUnitsDropDownList(this.FlowUnitsDropDownList,EngineeringUnit.FmvfCcMin,EngineeringUnit.FmvfKlDay,site.FlowUnits);
					this.InitializeUnitsDropDownList(this.PressureUnitsDropDownList,EngineeringUnit.FmpPa,EngineeringUnit.FmpAtm,site.PressureUnits);
					this.InitializeUnitsDropDownList(this.AdditiveProfileCycleAmountUnitsDropDownList,EngineeringUnit.FmvCm3,EngineeringUnit.FmvKl,site.AdditiveProfileCycleAmountUnits);
					this.InitializeUnitsDropDownList(this.AdditiveProfileRateUnitsDropDownList,EngineeringUnit.FmvCm3,EngineeringUnit.FmvKl,site.AdditiveProfileRateUnits);

					this.LevelDecimalPlacesTextbox.Text=site.LevelDecimalPlaces;
					this.VolumeDecimalPlacesTextbox.Text=site.VolumeDecimalPlaces;
					this.AdditiveVolumeDecimalPlacesTextbox.Text=site.AdditiveVolumeDecimalPlaces;
					this.TemperatureDecimalPlacesTextbox.Text=site.TemperatureDecimalPlaces;
					this.DensityDecimalPlacesTextbox.Text=site.DensityDecimalPlaces;
					this.MassDecimalPlacesTextbox.Text=site.MassDecimalPlaces;
					this.FlowDecimalPlacesTextbox.Text=site.FlowDecimalPlaces;
					this.PressureDecimalPlacesTextbox.Text=site.PressureDecimalPlaces;
					this.AdditiveProfileCycleAmountDecimalPlacesTextbox.Text=site.AdditiveProfileCycleAmountDecimalPlaces;
					this.AdditiveProfileRateDecimalPlacesTextbox.Text=site.AdditiveProfileRateDecimalPlaces;

					this.InitializeQuantityDisplayDefault();
				}

				// Need to UpdateData on each post back becuse if Site is Login Site
				// other controls reformat based upon new settings
				else
					this.UpdateData();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void InitializeQuantityDisplayDefault()
		{
			SiteClass site = (SiteClass) this.Session["Site"];
			
			this.QuantityDisplayDefaultDropDown.Items.Clear();

			this.QuantityDisplayDefaultDropDown.Items.Add(new ListItem("Gross and Net",((int)QuantityDisplay.GROSS_AND_NET).ToString()));
			this.QuantityDisplayDefaultDropDown.Items.Add(new ListItem("Gross",((int)QuantityDisplay.GROSS).ToString()));
			this.QuantityDisplayDefaultDropDown.Items.Add(new ListItem("Net",((int)QuantityDisplay.NET).ToString()));
			this.QuantityDisplayDefaultDropDown.Items.Add(new ListItem("Mass",((int)QuantityDisplay.MASS).ToString()));
			this.QuantityDisplayDefaultDropDown.Items.Add(new ListItem("Package",((int)QuantityDisplay.PACKAGE).ToString()));

			this.QuantityDisplayDefaultDropDown.SelectedIndex = (int)site.QuantityDisplayDefault;

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

			site.LevelUnits=(EngineeringUnit) Convert.ToInt32(this.LevelUnitsDropDownList.SelectedValue);
			site.VolumeUnits=(EngineeringUnit) Convert.ToInt32(this.VolumeUnitsDropDownList.SelectedValue);
			site.AdditiveVolumeUnits=(EngineeringUnit) Convert.ToInt32(this.AdditiveVolumeUnitsDropDownList.SelectedValue);
			site.TemperatureUnits=(EngineeringUnit) Convert.ToInt32(this.TemperatureUnitsDropDownList.SelectedValue);
			site.DensityUnits=(EngineeringUnit) Convert.ToInt32(this.DensityUnitsDropDownList.SelectedValue);
			site.MassUnits=(EngineeringUnit) Convert.ToInt32(this.MassUnitsDropDownList.SelectedValue);
			site.FlowUnits=(EngineeringUnit) Convert.ToInt32(this.FlowUnitsDropDownList.SelectedValue);
			site.PressureUnits=(EngineeringUnit) Convert.ToInt32(this.PressureUnitsDropDownList.SelectedValue);
			site.AdditiveProfileCycleAmountUnits=(EngineeringUnit) Convert.ToInt32(this.AdditiveProfileCycleAmountUnitsDropDownList.SelectedValue);
			site.AdditiveProfileRateUnits=(EngineeringUnit) Convert.ToInt32(this.AdditiveProfileRateUnitsDropDownList.SelectedValue);
			site.LevelDecimalPlaces=this.LevelDecimalPlacesTextbox.Text;
			site.VolumeDecimalPlaces=this.VolumeDecimalPlacesTextbox.Text;
			site.AdditiveVolumeDecimalPlaces=this.AdditiveVolumeDecimalPlacesTextbox.Text;
			site.TemperatureDecimalPlaces=this.TemperatureDecimalPlacesTextbox.Text;
			site.DensityDecimalPlaces=this.DensityDecimalPlacesTextbox.Text;
			site.MassDecimalPlaces=this.MassDecimalPlacesTextbox.Text;
			site.FlowDecimalPlaces=this.FlowDecimalPlacesTextbox.Text;
			site.PressureDecimalPlaces=this.PressureDecimalPlacesTextbox.Text;
			site.AdditiveProfileCycleAmountDecimalPlaces=this.AdditiveProfileCycleAmountDecimalPlacesTextbox.Text;
			site.AdditiveProfileRateDecimalPlaces=this.AdditiveProfileRateDecimalPlacesTextbox.Text;
		}

		protected void QuantityDisplayDefaultDropDownSelectedIndexChanged(object sender,EventArgs e)
		{
			SiteClass site = (SiteClass)this.Session["Site"];

			site.QuantityDisplayDefault = (QuantityDisplay) Convert.ToInt32(this.QuantityDisplayDefaultDropDown.SelectedItem.Value);
		}
	}
}
