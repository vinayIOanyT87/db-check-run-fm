// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StationForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the StationForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.UtilityObjects;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net.Sockets;
	using System.Web;
	using System.Web.Http;
	using System.Web.UI.WebControls;

	 /// <summary>
	///    Summary description for StationForm.
	/// </summary>
	public partial class StationForm : FMAutoSubmitFormBase
	{
		#region Constants and Fields

		protected Label Label1;

		protected Label Label2;

		protected Label Label5;

		protected Label Label8;

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///    This method is called by the OK and New buttons to save the data from
		///    the associated tab pages. It will call an update method for each of the
		///    tab pages.
		/// </summary>
		public void UpdateData()
		{
			var Station = (StationClass)this.Session["Station"];

			if (Station.Type == STATION_TYPE.LOAD_RACK)
			{
				this.StationLoadRackPage.UpdateData();
			}

			if (Station.Type == STATION_TYPE.ENTRY_GATE)
			{
				this.StationEntryGatePage.UpdateData();
			}

			if (Station.Type == STATION_TYPE.EXIT_GATE)
			{
				this.StationExitGatePage.UpdateData();
			}

			if (Station.Type == STATION_TYPE.WEIGHT_SCALE)
			{
				this.StationWeightScalePage.UpdateData();
			}

			if (Station.Type == STATION_TYPE.PRELOAD)
			{
				this.StationPreloadPage.UpdateData();
			}

			if (Station.Type == STATION_TYPE.BOL)
			{
				this.StationBillOfLadingPage.UpdateData();
			}

			if (Station.Type == STATION_TYPE.SIGNATURE)
			{
				this.StationSignatureStationPage.UpdateData();
			}

			if (Station.Type == STATION_TYPE.METER)
			{
				this.StationMeterPage.UpdateData();
			}

			if (Station.Type == STATION_TYPE.OFF_LOADING)
			{
				this.Stationdefuelpage.UpdateData();
			}
		}

		#endregion

		#region Methods

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		public void EnableControls(bool enable)
		{
			if (Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
					// can not be configured at SiteGroup
					this.OK.Enabled = enable;
					this.New.Enabled = enable;
			}

			this.Cancel.Enabled = enable;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.Session.Remove("Status");

				this.GetSecurity();

				StationClass Station;

				if (!this.Page.IsPostBack)
				{
					SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																		x =>
																		x.Get(
																			this.Security,
																			this.Security.SiteGuid,
																			getMemberSites: false,
																			getSchedulesAndProcessVariables: false,
																			bGetAssociatedAliases: true)
																	);

					if (!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
					{
						this.OK.Enabled = false;
						this.New.Enabled = false;
						this.Apply.Enabled = false;
					}

					if (!site.DeferStationChanges)
					{
						this.Apply.Visible = false;
					}

					Station = (StationClass)this.Session["Station"];

					if (Station == null)
					{
						// Get Guid
						if (this.Session["IdentityGuid"] != null)
						{
							// Get Station
							Station = FMChannelHelper.MakeCall<IStations, StationClass>(
																		x =>
																		x.Get(this.Security, (Guid)this.Session["IdentityGuid"])
																);

							//if the station type is Meter Station, load meter
							if (Station.Type == STATION_TYPE.METER && Station.Meter.IdentityGuid != Guid.Empty)
							{
								MeterClass meter = FMChannelHelper.MakeCall<IMeters, MeterClass>(x => x.Get(this.Security, Station.Meter.IdentityGuid));
								if (meter != null)
								{
									Station.Meter = new MeterClass(meter);
								}
							}
						}
						else
						{
							Station = new StationClass();
						}

						this.Session["Station"] = Station;
					}

					//Set the title label with a key field from the bound object appended
					if (Station != null)
					{
						this.StationTitleLabel.Text = this.GetTitleLabelText(this.StationTitleLabel.Text, Station.ID);
					}
				}

				else
				{
					if (this.Session["Station"] == null)
					{
						throw new Exception("Station not in Session");
					}

					Station = (StationClass)this.Session["Station"];
				}

				// Set up which tabs are visible and the proper text on the tab header
				// Hide these now, they will be made visible below if user has permission
				this.tpBillOfLadingPage.Visible = false;
				this.tpDeFuelPage.Visible = false;
				this.tpEntryGatePage.Visible = false;
				this.tpExitGatePage.Visible = false;
				this.tpLoadArmsPage.Visible = false;
				this.tpLoadRackPage.Visible = false;
				this.tpMeterPage.Visible = false;
				this.tpPreloadPage.Visible = false;
				this.tpReqQualificationsPage.Visible = false;
				this.tpReqStationDeFuelMeterPage.Visible = false;
				this.tpReqTestsandInspectionsPage.Visible = false;
				this.tpReqTrainingPage.Visible = false;
				this.tpSignatureStationPage.Visible = false;
				this.tpWeightScalePage.Visible = false;
				this.tpReqLicensePage.Visible = false;
				this.tpReqEquipmentLicensePage.Visible = false;
				this.tpReqOffLoadingProductPage.Visible = false;

				// Set up the tcStation based upon Type
				int selectedIndex = this.tcStation.ActiveTabIndex;

				this.tpGeneralPage.HeaderText = this.GetTranslatedText("General");

				switch (Station.Type)
				{
					case STATION_TYPE.ENTRY_GATE:
					{
						this.tpEntryGatePage.Visible = true;
						this.tpEntryGatePage.HeaderText = this.GetTranslatedText(StationClass.TypeID(STATION_TYPE.ENTRY_GATE));
									this.tpReqLicensePage.Visible = true;
									this.tpReqLicensePage.HeaderText = this.GetTranslatedText("Personnel Licenses");
						break;
					}

					case STATION_TYPE.LOAD_RACK:
					{
						this.tpLoadRackPage.Visible = true;
						this.tpLoadRackPage.HeaderText = this.GetTranslatedText(StationClass.TypeID(STATION_TYPE.LOAD_RACK));

						this.tpLoadArmsPage.Visible = true;
						this.tpLoadArmsPage.HeaderText = this.GetTranslatedText("Load Arms");

						this.tpReqQualificationsPage.Visible = true;
						this.tpReqQualificationsPage.HeaderText = this.GetTranslatedText("Required Qualifications");

						this.tpReqTrainingPage.Visible = true;
						this.tpReqTrainingPage.HeaderText = this.GetTranslatedText("Required Training");

						this.tpReqLicensePage.Visible = true;
						this.tpReqLicensePage.HeaderText = this.GetTranslatedText("Personnel Licenses");

						this.tpReqEquipmentLicensePage.Visible = true;
						this.tpReqEquipmentLicensePage.HeaderText = this.GetTranslatedText("Equipment Licenses");

						this.tpReqTestsandInspectionsPage.Visible = true;
						this.tpReqTestsandInspectionsPage.HeaderText = this.GetTranslatedText("Tests & Inspections");

						break;
					}

					case STATION_TYPE.EXIT_GATE:
					{
						this.tpExitGatePage.Visible = true;
						this.tpExitGatePage.HeaderText = this.GetTranslatedText(StationClass.TypeID(STATION_TYPE.EXIT_GATE));
						break;
					}

					case STATION_TYPE.WEIGHT_SCALE:
					{
						this.tpWeightScalePage.Visible = true;
						this.tpWeightScalePage.HeaderText = this.GetTranslatedText(StationClass.TypeID(STATION_TYPE.WEIGHT_SCALE));
						break;
					}

					case STATION_TYPE.BOL:
					{
						this.tpBillOfLadingPage.Visible = true;
						this.tpBillOfLadingPage.HeaderText = this.GetTranslatedText(StationClass.TypeID(STATION_TYPE.BOL));
						break;
					}

					case STATION_TYPE.PRELOAD:
					{
						this.tpPreloadPage.Visible = true;
						this.tpPreloadPage.HeaderText = this.GetTranslatedText(StationClass.TypeID(STATION_TYPE.PRELOAD));
						break;
					}

					case STATION_TYPE.SIGNATURE:
					{
						this.tpSignatureStationPage.Visible = true;
						this.tpSignatureStationPage.HeaderText = this.GetTranslatedText(StationClass.TypeID(STATION_TYPE.SIGNATURE));
						break;
					}

					case STATION_TYPE.METER:
					{
						this.tpMeterPage.Visible = true;
						this.tpMeterPage.HeaderText = this.GetTranslatedText(StationClass.TypeID(STATION_TYPE.METER));
						break;
					}

					case STATION_TYPE.OFF_LOADING:
					{
						this.tpDeFuelPage.Visible = true;
						this.tpDeFuelPage.HeaderText = this.GetTranslatedText(StationClass.TypeID(STATION_TYPE.OFF_LOADING));

						if (Station.InterfaceType == STATION_INTERFACE_TYPE.VAREC_DET)
						{
							this.tpReqOffLoadingProductPage.Visible = true;
							this.tpReqOffLoadingProductPage.HeaderText = this.GetTranslatedText("Products");
						}
						else
						{
							this.tpLoadArmsPage.Visible = true;
							this.tpLoadArmsPage.HeaderText = this.GetTranslatedText("Load Arms");
						}

						this.tpReqQualificationsPage.Visible = true;
						this.tpReqQualificationsPage.HeaderText = this.GetTranslatedText("Required Qualifications");

						this.tpReqTrainingPage.Visible = true;
						this.tpReqTrainingPage.HeaderText = this.GetTranslatedText("Required Training");

						this.tpReqLicensePage.Visible = true;
						this.tpReqLicensePage.HeaderText = this.GetTranslatedText("Personnel Licenses");

						break;
					}

					default:
						break;
				}

				if (this.Session["TabIndex"] != null)
				{
					selectedIndex = (int)Session["TabIndex"];
					Session.Remove("TabIndex");
				}

				this.tcStation.ActiveTabIndex = selectedIndex;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.Response.End();
			}
		}

		private void Apply_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ((this.Session["Status"] != null) && ((string)this.Session["Status"] == "Error"))
				{
					return;
				}

				this.Save(false);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}
		}

		private void Cancel_Command(object sender, CommandEventArgs e)
		{
			this.Redirect("StationsForm.aspx");
			//this.Session.Remove("Station");// randomly getting FuelsManager : Station not in Session; so moving it to StationsForm page_load
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.OK.Command += new System.Web.UI.WebControls.CommandEventHandler(this.OK_Command);
			this.New.Command += new System.Web.UI.WebControls.CommandEventHandler(this.New_Command);
			this.Cancel.Command += new System.Web.UI.WebControls.CommandEventHandler(this.Cancel_Command);
			this.Apply.Command += new System.Web.UI.WebControls.CommandEventHandler(this.Apply_Command);
		}

		private void New_Command(object sender, CommandEventArgs e)
		{
			try
			{
				SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(
																			this.Security,
																			this.Security.SiteGuid,
																			getMemberSites: false,
																			getSchedulesAndProcessVariables: false,
																			bGetAssociatedAliases: true)
																	);
				if ((this.Session["Status"] != null) && ((string)this.Session["Status"] == "Error"))
				{
					return;
				}

				this.Save(site.DeferStationChanges);

				var Station = (StationClass)this.Session["Station"];

				Station.ID = "";
				Station.IdentityGuid = Guid.Empty;
				foreach (ProcessVariableClass PV in Station.ProcessVariableCollection)
				{
					PV.IdentityGuid = Guid.Empty;
				}

				foreach (LoadArmClass LoadArm in Station.LoadArmCollection)
				{
					LoadArm.IdentityGuid = Guid.Empty;
					foreach (ProcessVariableClass PV in LoadArm.ProcessVariableCollection)
					{
						PV.IdentityGuid = Guid.Empty;
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}
			this.Redirect("StationForm.aspx");
		}

		private void OK_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ((this.Session["Status"] != null) && ((string)this.Session["Status"] == "Error"))
				{
					return;
				}

				SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(this.Security, this.Security.SiteGuid, false, false, true)
																);
				this.Save(site.DeferStationChanges);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Redirect("StationsForm.aspx");
			//this.Session.Remove("Station"); // randomly getting FuelsManager : Station not in Session; so moving it to StationsForm page_load
		}

		private void Save(bool InhibitApplyStationChanges)
		{
			// Update the data from the tab pages.
			this.UpdateData();

			var Station = (StationClass)this.Session["Station"];

			if (Station.Type == STATION_TYPE.LOAD_RACK && UndoDynamicRecipeChanges.Value == "true")
			{
				Station.EnableDynamicRecipes = !Station.EnableDynamicRecipes;
			}

			//Ensure the preload printer is blank for other types. The user may switch type from preload to loadrack
			if (Station.Type != STATION_TYPE.WEIGHT_SCALE && Station.Type != STATION_TYPE.PRELOAD)
			{
				Station.PreloadPrinter = "";
				Station.NumberOfPreloadCopies = 1;
			}
			// For existing Station try to purge from Load Rack Manager
			// to insure that no operation is started on the station
			// while being modified
			if (!InhibitApplyStationChanges && !(Station.IdentityGuid.IsEmpty()) && Station.Enabled && UsingLoadRack)
			{
				try
				{
					ILoadRackManager LoadRackManager = this.GetLoadRackManager();
					LoadRackManager.Purge(this.Security, typeof(StationClass), Station.IdentityGuid);
				}
				catch (SocketException socketExcept)
				{
					if (socketExcept.ErrorCode != 10061)
					{
						throw socketExcept;
					}
				}
			}

			if (!Station.IdentityGuid.IsEmpty())
			{
				if (Station.Type == STATION_TYPE.METER && Station.Meter.IdentityGuid != Guid.Empty)
				{
					FMChannelHelper.MakeCall<IMeters>(x => x.Modify(this.Security, Station.Meter));
				}
				else if (Station.Type == STATION_TYPE.METER)
				{
					Station.Meter.IdentityGuid = FMChannelHelper.MakeCall<IMeters, Guid>(x => x.Add(this.Security, Station.Meter));
				}

				FMChannelHelper.MakeCall<IStations>(
													x =>
													x.Modify(this.Security, Station)
												);
			}
			else
			{
				if (Station.Type == STATION_TYPE.METER)
				{
					Station.Meter.IdentityGuid = FMChannelHelper.MakeCall<IMeters, Guid>(x => x.Add(this.Security, Station.Meter));
				}

				Station.IdentityGuid = FMChannelHelper.MakeCall<IStations, Guid>(
																x =>
																x.Add(this.Security, Station)
															);
			}

			if (!InhibitApplyStationChanges && Station.Enabled && UsingLoadRack)
			{
				try
				{
					ILoadRackManager LoadRackManager = this.GetLoadRackManager();
					LoadRackManager.Add(this.Security, typeof(StationClass), Station.IdentityGuid);
				}
				catch (SocketException socketExcept)
				{
					if (socketExcept.ErrorCode != 10061)
					{
						throw socketExcept;
					}
				}
			}
		}

		#endregion
	}
}