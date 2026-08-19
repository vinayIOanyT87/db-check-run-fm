// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionAliasUnitsPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TransactionAliasUnitsPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

    /// <summary>
	///    Summary description for TransactionAliasUnitsPage.
	/// </summary>
	public partial class TransactionAliasUnitsPage : FMUserControlBase
	{
		#region Constants and Fields
		protected SiteClass CurrentSite;
		#endregion

		#region Public Methods and Operators
		public void UpdateData()
		{
			var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];

			transactionAlias.LevelUnits = (EngineeringUnit)Convert.ToInt32(this.LevelUnitsDropDownList.SelectedValue);
			transactionAlias.VolumeUnits = (EngineeringUnit)Convert.ToInt32(this.VolumeUnitsDropDownList.SelectedValue);
			transactionAlias.AdditiveVolumeUnits = (EngineeringUnit)Convert.ToInt32(this.AdditiveVolumeUnitsDropDownList.SelectedValue);
			transactionAlias.TemperatureUnits = (EngineeringUnit)Convert.ToInt32(this.TemperatureUnitsDropDownList.SelectedValue);
			transactionAlias.DensityUnits = (EngineeringUnit)Convert.ToInt32(this.DensityUnitsDropDownList.SelectedValue);
			transactionAlias.MassUnits = (EngineeringUnit)Convert.ToInt32(this.MassUnitsDropDownList.SelectedValue);
			transactionAlias.FlowUnits = (EngineeringUnit)Convert.ToInt32(this.FlowUnitsDropDownList.SelectedValue);
			transactionAlias.PressureUnits = (EngineeringUnit)Convert.ToInt32(this.PressureUnitsDropDownList.SelectedValue);
			transactionAlias.LevelDecimalPlaces = this.LevelDecimalPlacesTextbox.Text;
			transactionAlias.VolumeDecimalPlaces = this.VolumeDecimalPlacesTextbox.Text;
			transactionAlias.AdditiveVolumeDecimalPlaces = this.AdditiveVolumeDecimalPlacesTextbox.Text;
			transactionAlias.TemperatureDecimalPlaces = this.TemperatureDecimalPlacesTextbox.Text;
			transactionAlias.DensityDecimalPlaces = this.DensityDecimalPlacesTextbox.Text;
			transactionAlias.MassDecimalPlaces = this.MassDecimalPlacesTextbox.Text;
			transactionAlias.FlowDecimalPlaces = this.FlowDecimalPlacesTextbox.Text;
			transactionAlias.PressureDecimalPlaces = this.PressureDecimalPlacesTextbox.Text;
		}
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

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];

				this.CurrentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(sites => sites.GetBasic(this.Security, this.Security.SiteGuid));

				if (! this.Page.IsPostBack)
				{
					this.InitializeUnitsDropDownList(this.LevelUnitsDropDownList, 
													EngineeringUnit.FmlFtIn8Th, 
													EngineeringUnit.FmlMile, 
													transactionAlias.LevelUnits);
					this.InitializeUnitsDropDownList(this.VolumeUnitsDropDownList, 
													EngineeringUnit.FmvCm3, 
													EngineeringUnit.FmvKl, 
													transactionAlias.VolumeUnits);
					this.InitializeUnitsDropDownList(this.AdditiveVolumeUnitsDropDownList,
													EngineeringUnit.FmvCm3,
													EngineeringUnit.FmvKl,
													transactionAlias.AdditiveVolumeUnits);
					this.InitializeUnitsDropDownList(this.TemperatureUnitsDropDownList,
													EngineeringUnit.FmtDegC,
													EngineeringUnit.FmtDegR,
													transactionAlias.TemperatureUnits);
					this.InitializeUnitsDropDownList(this.DensityUnitsDropDownList,
													EngineeringUnit.FmdGcm3,
													EngineeringUnit.FmdSTnYd3,
													transactionAlias.DensityUnits);
					this.InitializeUnitsDropDownList(this.MassUnitsDropDownList, 
													EngineeringUnit.FmmGram, 
													EngineeringUnit.FmmMlbs, 
													transactionAlias.MassUnits);
					this.InitializeUnitsDropDownList(this.FlowUnitsDropDownList, 
													EngineeringUnit.FmvfCcMin, 
													EngineeringUnit.FmvfKlDay, 
													transactionAlias.FlowUnits);
					this.InitializeUnitsDropDownList(this.PressureUnitsDropDownList, 
													EngineeringUnit.FmpPa, 
													EngineeringUnit.FmpAtm, 
													transactionAlias.PressureUnits);

					this.LevelUnitsDropDownList.Items.Insert(0, new ListItem("<Site>", "0"));
					this.VolumeUnitsDropDownList.Items.Insert(0, new ListItem("<Site>", "0"));
					this.AdditiveVolumeUnitsDropDownList.Items.Insert(0, new ListItem("<Site>", "0"));
					this.TemperatureUnitsDropDownList.Items.Insert(0, new ListItem("<Site>", "0"));
					this.DensityUnitsDropDownList.Items.Insert(0, new ListItem("<Site>", "0"));
					this.MassUnitsDropDownList.Items.Insert(0, new ListItem("<Site>", "0"));
					this.FlowUnitsDropDownList.Items.Insert(0, new ListItem("<Site>", "0"));
					this.PressureUnitsDropDownList.Items.Insert(0, new ListItem("<Site>", "0"));

					this.LevelDecimalPlacesTextbox.Text = transactionAlias.LevelDecimalPlaces;
					this.VolumeDecimalPlacesTextbox.Text = transactionAlias.VolumeDecimalPlaces;
					this.AdditiveVolumeDecimalPlacesTextbox.Text = transactionAlias.AdditiveVolumeDecimalPlaces;
					this.TemperatureDecimalPlacesTextbox.Text = transactionAlias.TemperatureDecimalPlaces;
					this.DensityDecimalPlacesTextbox.Text = transactionAlias.DensityDecimalPlaces;
					this.MassDecimalPlacesTextbox.Text = transactionAlias.MassDecimalPlaces;
					this.FlowDecimalPlacesTextbox.Text = transactionAlias.FlowDecimalPlaces;
					this.PressureDecimalPlacesTextbox.Text = transactionAlias.PressureDecimalPlaces;

					this.LevelUnitsDropDownListSelectedIndexChanged(null, null);
					this.VolumeUnitsDropDownListSelectedIndexChanged(null, null);
					this.AdditiveVolumeUnitsDropDownListSelectedIndexChanged(null, null);
					this.TemperatureUnitsDropDownListSelectedIndexChanged(null, null);
					this.DensityUnitsDropDownListSelectedIndexChanged(null, null);
					this.MassUnitsDropDownListSelectedIndexChanged(null, null);
					this.FlowUnitsDropDownListSelectedIndexChanged(null, null);
					this.PressureUnitsDropDownListSelectedIndexChanged(null, null);
                    this.SetFieldAccessibilityForChildRecordVersion();
				}
				else
				{
					// Need to UpdateData on each post back becuse if TransactionAlias is Login TransactionAlias
					// other controls reformat based upon new settings
					this.UpdateData();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AdditiveVolumeUnitsDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
            var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
            var versionSpecificFields = this.Session[PageSessionKeyConstants.TRANS_ALIAS_VERSION_SPECIFIC_FIELDS] as List<string>;
            this.AdditiveVolumeDecimalPlacesTextbox.Enabled = false;
            bool currentSiteOwnsRecordVersion = (transactionAlias.SiteGuid == this.Security.SiteGuid);

            if (this.AdditiveVolumeUnitsDropDownList.SelectedIndex != 0)
            {
                if (transactionAlias == null 
					|| transactionAlias.IdentityGuid.Equals(Guid.Empty)
                    || (currentSiteOwnsRecordVersion 
                        && transactionAlias.IdentityGuid.Equals(transactionAlias.MasterRecordGuid)))
                {
                    this.AdditiveVolumeDecimalPlacesTextbox.Enabled = true;
                }
                else if (versionSpecificFields != null)
                {
                    this.AdditiveVolumeDecimalPlacesTextbox.Enabled = versionSpecificFields.Contains("AdditiveVolumeDecimalPlaces");
                }
            }

			if (!this.AdditiveVolumeDecimalPlacesTextbox.Enabled)
			{
				this.AdditiveVolumeDecimalPlacesTextbox.Text = this.CurrentSite.AdditiveVolumeDecimalPlaces;
			}
		}

		private void DensityUnitsDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
            var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
            var versionSpecificFields = this.Session[PageSessionKeyConstants.TRANS_ALIAS_VERSION_SPECIFIC_FIELDS] as List<string>;
            this.DensityDecimalPlacesTextbox.Enabled = false;
            bool currentSiteOwnsRecordVersion = (transactionAlias.SiteGuid == this.Security.SiteGuid);

            if (this.DensityUnitsDropDownList.SelectedIndex != 0)
            {
                if (transactionAlias == null 
					|| transactionAlias.IdentityGuid.Equals(Guid.Empty)
                    || (currentSiteOwnsRecordVersion && transactionAlias.IdentityGuid.Equals(transactionAlias.MasterRecordGuid)))
                {
                    this.DensityDecimalPlacesTextbox.Enabled = true;
                }
                else if (versionSpecificFields != null)
                {
                    this.DensityDecimalPlacesTextbox.Enabled = versionSpecificFields.Contains("DensityDecimalPlaces");
                }
            }

			if (!this.DensityDecimalPlacesTextbox.Enabled)
			{
				this.DensityDecimalPlacesTextbox.Text = this.CurrentSite.DensityDecimalPlaces;
			}
		}

		private void FlowUnitsDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
            var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
            var versionSpecificFields = this.Session[PageSessionKeyConstants.TRANS_ALIAS_VERSION_SPECIFIC_FIELDS] as List<string>;
            this.FlowDecimalPlacesTextbox.Enabled = false;
            bool currentSiteOwnsRecordVersion = (transactionAlias.SiteGuid == this.Security.SiteGuid);

            if (this.FlowUnitsDropDownList.SelectedIndex != 0)
            {
                if (transactionAlias == null
					|| transactionAlias.IdentityGuid.Equals(Guid.Empty)
                    || (currentSiteOwnsRecordVersion && transactionAlias.IdentityGuid.Equals(transactionAlias.MasterRecordGuid)))
                {
                    this.FlowDecimalPlacesTextbox.Enabled = true;
                }
                else if (versionSpecificFields != null)
                {
                    this.FlowDecimalPlacesTextbox.Enabled = versionSpecificFields.Contains("FlowDecimalPlaces");
                }
            }

			if (!this.FlowDecimalPlacesTextbox.Enabled)
			{
				this.FlowDecimalPlacesTextbox.Text = this.CurrentSite.FlowDecimalPlaces;
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.PressureUnitsDropDownList.SelectedIndexChanged			+= this.PressureUnitsDropDownListSelectedIndexChanged;
			this.FlowUnitsDropDownList.SelectedIndexChanged				+= this.FlowUnitsDropDownListSelectedIndexChanged;
			this.LevelUnitsDropDownList.SelectedIndexChanged			+= this.LevelUnitsDropDownListSelectedIndexChanged;
			this.MassUnitsDropDownList.SelectedIndexChanged				+= this.MassUnitsDropDownListSelectedIndexChanged;
			this.AdditiveVolumeUnitsDropDownList.SelectedIndexChanged	+= this.AdditiveVolumeUnitsDropDownListSelectedIndexChanged;
			this.VolumeUnitsDropDownList.SelectedIndexChanged			+= this.VolumeUnitsDropDownListSelectedIndexChanged;
			this.TemperatureUnitsDropDownList.SelectedIndexChanged		+= this.TemperatureUnitsDropDownListSelectedIndexChanged;
			this.DensityUnitsDropDownList.SelectedIndexChanged			+= this.DensityUnitsDropDownListSelectedIndexChanged;
		}

		private void LevelUnitsDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
            var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
            var versionSpecificFields = this.Session[PageSessionKeyConstants.TRANS_ALIAS_VERSION_SPECIFIC_FIELDS] as List<string>;
            this.LevelDecimalPlacesTextbox.Enabled = false;
            bool currentSiteOwnsRecordVersion = (transactionAlias.SiteGuid == this.Security.SiteGuid);

            if (this.LevelUnitsDropDownList.SelectedIndex != 0)
            {
                if (transactionAlias == null 
					|| transactionAlias.IdentityGuid.Equals(Guid.Empty)
                    || (currentSiteOwnsRecordVersion && transactionAlias.IdentityGuid.Equals(transactionAlias.MasterRecordGuid)))
                {
                    this.LevelDecimalPlacesTextbox.Enabled = true;
                }
                else if (versionSpecificFields != null)
                {
                    this.LevelDecimalPlacesTextbox.Enabled = versionSpecificFields.Contains("LevelDecimalPlaces");
                }
            }

			if (!this.LevelDecimalPlacesTextbox.Enabled)
			{
				this.LevelDecimalPlacesTextbox.Text = this.CurrentSite.LevelDecimalPlaces;
			}
		}

		private void MassUnitsDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
            var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
            var versionSpecificFields = this.Session[PageSessionKeyConstants.TRANS_ALIAS_VERSION_SPECIFIC_FIELDS] as List<string>;            
            this.MassDecimalPlacesTextbox.Enabled = false;
            bool currentSiteOwnsRecordVersion = (transactionAlias.SiteGuid == this.Security.SiteGuid);

            if (this.MassUnitsDropDownList.SelectedIndex != 0)
            {
                if (transactionAlias == null
					|| transactionAlias.IdentityGuid.Equals(Guid.Empty)
                    || (currentSiteOwnsRecordVersion && transactionAlias.IdentityGuid.Equals(transactionAlias.MasterRecordGuid)))
                {
                    this.MassDecimalPlacesTextbox.Enabled = true;
                }
                else if (versionSpecificFields != null)
                {
                    this.MassDecimalPlacesTextbox.Enabled = versionSpecificFields.Contains("MassDecimalPlaces");
                }
            }

			if (!this.MassDecimalPlacesTextbox.Enabled)
			{
				this.MassDecimalPlacesTextbox.Text = this.CurrentSite.MassDecimalPlaces;
			}
		}

		private void PressureUnitsDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
            var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
            var versionSpecificFields = this.Session[PageSessionKeyConstants.TRANS_ALIAS_VERSION_SPECIFIC_FIELDS] as List<string>;
            this.PressureDecimalPlacesTextbox.Enabled = false;
            bool currentSiteOwnsRecordVersion = (transactionAlias.SiteGuid == this.Security.SiteGuid);

            if (this.PressureUnitsDropDownList.SelectedIndex != 0)
            {
                if (transactionAlias == null
					|| transactionAlias.IdentityGuid.Equals(Guid.Empty)
                    || (currentSiteOwnsRecordVersion && transactionAlias.IdentityGuid.Equals(transactionAlias.MasterRecordGuid)))
                {
                    this.PressureDecimalPlacesTextbox.Enabled = true;
                }
                else if (versionSpecificFields != null)
                {
                    this.PressureDecimalPlacesTextbox.Enabled = versionSpecificFields.Contains("PressureDecimalPlaces");
                }
            }
			
			if (!this.PressureDecimalPlacesTextbox.Enabled)
			{
				this.PressureDecimalPlacesTextbox.Text = this.CurrentSite.PressureDecimalPlaces;
			}
		}

		private void TemperatureUnitsDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
            var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
            var versionSpecificFields = this.Session[PageSessionKeyConstants.TRANS_ALIAS_VERSION_SPECIFIC_FIELDS] as List<string>;
            this.TemperatureDecimalPlacesTextbox.Enabled = false;
            bool currentSiteOwnsRecordVersion = (transactionAlias.SiteGuid == this.Security.SiteGuid);

            if (this.TemperatureUnitsDropDownList.SelectedIndex != 0)
            {
                if (transactionAlias == null
					|| transactionAlias.IdentityGuid.Equals(Guid.Empty)
                    || (currentSiteOwnsRecordVersion && transactionAlias.IdentityGuid.Equals(transactionAlias.MasterRecordGuid)))
                {
                    this.TemperatureDecimalPlacesTextbox.Enabled = true;
                }
                else if (versionSpecificFields != null)
                {
                    this.TemperatureDecimalPlacesTextbox.Enabled = versionSpecificFields.Contains("TemperatureDecimalPlaces");
                }
            }
			
			if (!this.TemperatureDecimalPlacesTextbox.Enabled)
			{
				this.TemperatureDecimalPlacesTextbox.Text = this.CurrentSite.TemperatureDecimalPlaces;
			}
		}

		private void VolumeUnitsDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
            var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
            var versionSpecificFields = this.Session[PageSessionKeyConstants.TRANS_ALIAS_VERSION_SPECIFIC_FIELDS] as List<string>;
            this.VolumeDecimalPlacesTextbox.Enabled = false;
            bool currentSiteOwnsRecordVersion = (transactionAlias.SiteGuid == this.Security.SiteGuid);

            if (this.VolumeUnitsDropDownList.SelectedIndex != 0)
            {
                if (transactionAlias == null
					|| transactionAlias.IdentityGuid.Equals(Guid.Empty)
                    || (currentSiteOwnsRecordVersion && transactionAlias.IdentityGuid.Equals(transactionAlias.MasterRecordGuid)))
                {
                    this.VolumeDecimalPlacesTextbox.Enabled = true;
                }
                else if (versionSpecificFields != null)
                {
                    this.VolumeDecimalPlacesTextbox.Enabled = versionSpecificFields.Contains("VolumeDecimalPlaces");
                }
            }
			
			if (!this.VolumeDecimalPlacesTextbox.Enabled)
			{
				this.VolumeDecimalPlacesTextbox.Text = this.CurrentSite.VolumeDecimalPlaces;
			}
		}


        private void SetFieldAccessibilityForChildRecordVersion()
        {
            var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
            var versionSpecificFields = this.Session[PageSessionKeyConstants.TRANS_ALIAS_VERSION_SPECIFIC_FIELDS] as List<string>;
            bool currentSiteOwnsRecordVersion = (transactionAlias.SiteGuid == this.Security.SiteGuid);

            if (versionSpecificFields != null && (transactionAlias.IdentityGuid.Equals(Guid.Empty)
                                              || (currentSiteOwnsRecordVersion && transactionAlias.IdentityGuid.Equals(transactionAlias.MasterRecordGuid))))
            {
                return;
            }

            if (versionSpecificFields != null)
            {
                this.LevelUnitsDropDownList.Enabled = (this.LevelUnitsDropDownList.Enabled && versionSpecificFields.Contains("LevelUnitIndex"));
                this.LevelDecimalPlacesTextbox.Enabled = (this.LevelDecimalPlacesTextbox.Enabled && versionSpecificFields.Contains("LevelDecimalPlaces"));
                this.VolumeUnitsDropDownList.Enabled = (this.VolumeUnitsDropDownList.Enabled && versionSpecificFields.Contains("VolumeUnitIndex"));
                this.VolumeDecimalPlacesTextbox.Enabled = (this.VolumeDecimalPlacesTextbox.Enabled && versionSpecificFields.Contains("VolumeDecimalPlaces"));
                this.AdditiveVolumeUnitsDropDownList.Enabled = (this.AdditiveVolumeUnitsDropDownList.Enabled && versionSpecificFields.Contains("AdditiveVolumeUnitIndex"));
                this.AdditiveVolumeDecimalPlacesTextbox.Enabled = (this.AdditiveVolumeDecimalPlacesTextbox.Enabled && versionSpecificFields.Contains("AdditiveVolumeDecimalPlaces"));
                this.TemperatureUnitsDropDownList.Enabled = (this.TemperatureUnitsDropDownList.Enabled && versionSpecificFields.Contains("TemperatureUnitIndex"));
                this.TemperatureDecimalPlacesTextbox.Enabled = (this.TemperatureDecimalPlacesTextbox.Enabled && versionSpecificFields.Contains("TemperatureDecimalPlaces"));
                this.DensityUnitsDropDownList.Enabled = (this.DensityUnitsDropDownList.Enabled && versionSpecificFields.Contains("DensityUnitIndex"));
                this.DensityDecimalPlacesTextbox.Enabled = (this.DensityDecimalPlacesTextbox.Enabled && versionSpecificFields.Contains("DensityDecimalPlaces"));
                this.MassUnitsDropDownList.Enabled = (this.MassUnitsDropDownList.Enabled && versionSpecificFields.Contains("MassUnitIndex"));
                this.MassDecimalPlacesTextbox.Enabled = (this.MassDecimalPlacesTextbox.Enabled && versionSpecificFields.Contains("MassDecimalPlaces"));
                this.FlowUnitsDropDownList.Enabled = (this.FlowUnitsDropDownList.Enabled && versionSpecificFields.Contains("FlowUnitIndex"));
                this.FlowDecimalPlacesTextbox.Enabled = (this.FlowDecimalPlacesTextbox.Enabled && versionSpecificFields.Contains("FlowDecimalPlaces"));
                this.PressureUnitsDropDownList.Enabled = (this.PressureUnitsDropDownList.Enabled && versionSpecificFields.Contains("PressureUnitIndex"));
                this.PressureDecimalPlacesTextbox.Enabled = (this.PressureDecimalPlacesTextbox.Enabled && versionSpecificFields.Contains("PressureDecimalPlaces"));
            }
        }
		#endregion
	}
}