// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TankDetailForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TankDetailForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using Opc.Da;

	/// <summary>
	///    Summary description for TankDetailForm.
	/// </summary>
	public partial class TankDetailForm : FMFormBase
	{
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
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					SiteClass CurrentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(this.Security, this.Security.SiteGuid, false, false, true)
																);
					// Get IdentityGuid
					if (this.Session["IdentityGuid"] != null)
					{
						// Get Tank
						TankClass Tank;
						Tank = FMChannelHelper.MakeCall<ITanks, TankClass>(
																	 x =>
																	 x.Get(this.Security, Guid.Parse(this.Session["IdentityGuid"] as string))
																);


						ProcessVariableCollectionClass PVs = Tank.ProcessVariableCollection;
						ProcessVariableClass StatusPV = PVs[PROCESS_VARIABLE_TYPE.TANK_STATUS_PV];
						ProcessVariableClass LevelPV = PVs[PROCESS_VARIABLE_TYPE.LEVEL_PV];
						ProcessVariableClass TemperaturePV = PVs[PROCESS_VARIABLE_TYPE.TEMPERATURE_PV];
						ProcessVariableClass GrossVolumePV = PVs[PROCESS_VARIABLE_TYPE.GROSS_VOLUME_PV];
						ProcessVariableClass NetVolumePV = PVs[PROCESS_VARIABLE_TYPE.NET_VOLUME_PV];
						ProcessVariableClass DensityPV = PVs[PROCESS_VARIABLE_TYPE.DENSITY_PV];
						ProcessVariableClass StandardDensityPV = PVs[PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV];
						ProcessVariableClass MassPV = PVs[PROCESS_VARIABLE_TYPE.MASS_PV];

						// Fill in the page label
						string InventoryText = "Inventory Information";
						if (this.Session["UseDataDictionary"] == null || (bool)this.Session["UseDataDictionary"])
						{
							InventoryText = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(CurrentSite.SiteGuid, "Inventory Information")
																);
						}

						this.InventoryInfo.Text = InventoryText + " - " + Tank.ID;

						// Put user code to initialize the page here
						this.ProductText.Text = Tank.ProductID;

						this.StatusText.Text = StatusPV.Encode(
							StatusPV.ServerValue,
							(CurrentSite.UseLastKnownGoodTankData) ? new Quality(Quality.Good.GetCode()) : new Quality(StatusPV.OPCQuality),
							0,
							null);

						this.LastUpdateText.Text = LevelPV.DateTimeStamp.ToString(CurrentSite.GetDateTimeFormatInfo());

						this.LevelText.Text = LevelPV.Encode(
							LevelPV.GetValue(CurrentSite.LevelUnits, CurrentSite._LevelDecimalPlaces),
							(CurrentSite.UseLastKnownGoodTankData) ? new Quality(Quality.Good.GetCode()) : new Quality(LevelPV.OPCQuality),
							CurrentSite.LevelUnits,
							CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.LENGTH));

						this.MaxVolumeText.Text =
							NetVolumePV.Encode(
								NetVolumePV.GetMaximum(CurrentSite.VolumeUnits, CurrentSite._VolumeDecimalPlaces),
								Quality.Good,
								CurrentSite.VolumeUnits,
								CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));

						this.MinVolumeText.Text =
							NetVolumePV.Encode(
								NetVolumePV.GetMinimum(CurrentSite.VolumeUnits, CurrentSite._VolumeDecimalPlaces),
								Quality.Good,
								CurrentSite.VolumeUnits,
								CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));

						this.GrossVolumeText.Text =
							GrossVolumePV.Encode(
								GrossVolumePV.GetValue(CurrentSite.VolumeUnits, CurrentSite._VolumeDecimalPlaces),
								(CurrentSite.UseLastKnownGoodTankData)
									? new Quality(Quality.Good.GetCode())
									: new Quality(GrossVolumePV.OPCQuality),
								CurrentSite.VolumeUnits,
								CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));

						this.NetVolumeText.Text =
							NetVolumePV.Encode(
								NetVolumePV.GetValue(CurrentSite.VolumeUnits, CurrentSite._VolumeDecimalPlaces),
								(CurrentSite.UseLastKnownGoodTankData)
									? new Quality(Quality.Good.GetCode())
									: new Quality(NetVolumePV.OPCQuality),
								CurrentSite.VolumeUnits,
								CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));

						this.MassText.Text = MassPV.Encode(
							MassPV.GetValue(CurrentSite.MassUnits, CurrentSite._MassDecimalPlaces),
							(CurrentSite.UseLastKnownGoodTankData) ? new Quality(Quality.Good.GetCode()) : new Quality(MassPV.OPCQuality),
							CurrentSite.MassUnits,
							CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.MASS));

						this.TempText.Text =
							TemperaturePV.Encode(
								TemperaturePV.GetValue(CurrentSite.TemperatureUnits, CurrentSite._TemperatureDecimalPlaces),
								(CurrentSite.UseLastKnownGoodTankData)
									? new Quality(Quality.Good.GetCode())
									: new Quality(TemperaturePV.OPCQuality),
								CurrentSite.TemperatureUnits,
								CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.TEMPERATURE));

						this.DensityText.Text =
							DensityPV.Encode(
								DensityPV.GetValue(CurrentSite.DensityUnits, CurrentSite._DensityDecimalPlaces),
								(CurrentSite.UseLastKnownGoodTankData) ? new Quality(Quality.Good.GetCode()) : new Quality(DensityPV.OPCQuality),
								CurrentSite.DensityUnits,
								CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DENSITY));

						double MaxVolume;
						double MinVolume;
						double NetVolume;
						double Percentage;

						try
						{
							MaxVolume = (double)NetVolumePV.GetMaximum(CurrentSite.VolumeUnits, CurrentSite._VolumeDecimalPlaces);
						}
						catch
						{
							MaxVolume = 100.0;
						}

						try
						{
							MinVolume = (double)NetVolumePV.GetMinimum(CurrentSite.VolumeUnits, CurrentSite._VolumeDecimalPlaces);
						}
						catch
						{
							MinVolume = 0.0;
						}

						try
						{
							NetVolume = (double)NetVolumePV.GetValue(CurrentSite.VolumeUnits, CurrentSite._VolumeDecimalPlaces);
						}
						catch
						{
							NetVolume = 0.0;
						}

						// check if this is a valid span
						if ((MaxVolume - MinVolume) > 0)
						{
							if (NetVolume <= 0.0)
							{
								Percentage = 0;
							}
							else
							{
								// calculate the percentage and display
								if (NetVolume > MaxVolume)
								{
									NetVolume = MaxVolume;
								}
								Percentage = ((NetVolume - MinVolume) / (MaxVolume - MinVolume)) * 100.0;
							}
						}
						else
						{
							Percentage = 0;
						}

						// note:
						// display the percentage
						// the graphic bar is opposite from what we want
						// ie top = 0; bottom = 144
						// we are manipulating the black background not the
						// product bar
						this.DetailBar.Visible = false;
						this.DetailBarbackground.Visible = false;
						this.DetailBar1.Visible = false;
						this.DetailBarbackground1.Visible = false;
						this.DetailBar3.Visible = false;
						this.DetailBarbackground3.Visible = false;
						this.DetailBar4.Visible = false;
						this.DetailBarbackground4.Visible = false;
						this.DetailBar5.Visible = false;
						this.DetailBarbackground5.Visible = false;
						this.DetailBar6.Visible = false;
						this.DetailBarbackground6.Visible = false;
						this.DetailBar7.Visible = false;
						this.DetailBarbackground7.Visible = false;

						// set the correct tank shape
						switch (Tank.VesselType)
						{
							case VESSEL_TYPE.PROPANE_VESSEL:
								{
									this.TankImage.ImageUrl = "images\\rbt_07_0240_g.gif";
									this.DetailBar1.Visible = true;
									this.DetailBarbackground1.Visible = true;
									this.DetailBar1.Height = (int)(56 - (.56 * Percentage));
									break;
								}

							case VESSEL_TYPE.BULLET_VESSEL:
								{
									this.TankImage.ImageUrl = "images\\sbt_27_0240_g.gif";
									this.DetailBar1.Visible = true;
									this.DetailBarbackground1.Visible = true;
									this.DetailBar1.Height = (int)(56 - (.56 * Percentage));
									break;
								}

							case VESSEL_TYPE.SPHERICAL_VESSEL:
								{
									this.TankImage.ImageUrl = "images\\gsp_07_0240_g.gif";
									this.DetailBar5.Visible = true;
									this.DetailBarbackground5.Visible = true;
									this.DetailBar5.Height = Convert.ToInt32(138.0 - (1.38 * Percentage));
									break;
								}

							case VESSEL_TYPE.CYLINDRICAL_VESSEL:
								{
									this.TankImage.ImageUrl = "images\\mcr_07_0240_g.gif";
									this.DetailBar3.Visible = true;
									this.DetailBarbackground3.Visible = true;
									this.DetailBar3.Height = (int)(138 - (1.38 * Percentage));
									break;
								}

							case VESSEL_TYPE.UNDERGROUND_VESSEL:
								{
									this.TankImage.ImageUrl = "images\\urb_07_0240_g.gif";
									this.DetailBar4.Visible = true;
									this.DetailBarbackground4.Visible = true;
									this.DetailBar4.Height = (int)(56 - (.56 * Percentage));
									break;
								}

							case VESSEL_TYPE.TANKER_VESSEL:
								{
									this.TankImage.ImageUrl = "images\\sfts_07_0180_g.gif";
									this.DetailBar6.Visible = true;
									this.DetailBarbackground6.Visible = true;
									this.DetailBar6.Height = (int)(56 - (.56 * Percentage));
									break;
								}

							case VESSEL_TYPE.COLLAPSIBLE_STORAGE_TANK:
								{
									this.TankImage.ImageUrl = "images\\ffs_0x_0240_g.gif";
									this.DetailBar7.Visible = true;
									this.DetailBarbackground7.Visible = true;
									this.DetailBar7.Height = (int)(96 - (.96 * Percentage));
									break;
								}

							default:
								{
									this.TankImage.ImageUrl = "images\\mcr_07_0240_g.gif";
									this.DetailBar.Visible = true;
									this.DetailBarbackground.Visible = true;
									this.DetailBar.Height = (int)(144 - (1.44 * Percentage));
									break;
								}
						}
					}
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