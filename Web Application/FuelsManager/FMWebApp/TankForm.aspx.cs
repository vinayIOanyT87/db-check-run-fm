// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TankForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TankForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
    using System;
    using System.Net.Sockets;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Interfaces;

    using FMControls;

    /// <summary>
	///    Summary description for TankForm.
	/// </summary>
	public partial class TankForm : FMAutoSubmitFormBase
	{
		#region Constants and Fields
		protected FMLabel Label5;
		public const string SessionGeneralPageUiError = "TankForm.GeneralPageUiError";
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

		protected void Page_Init(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (this.IsPostBack == false)
				{
					if (this.IsFromQueryWriter)
					{
						TankClass tank = FMChannelHelper.MakeCall<ITanks, TankClass>(x => x.Get(this.Security, this.QueryEntityGuid));

						this.Session["Tank"] = tank;
					}
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
                    if (!this.Security.HasRight(RIGHT.MODIFY_TANK_DATA))
                    {
						this.OK.Enabled = false;
					}

					var tank = (TankClass)this.Session["Tank"];

					if (tank == null)
					{
						// Get IdentityGuid
						if (this.Session["IdentityGuid"] != null)
						{
                            // Get Tank
                            // ReSharper disable once AssignNullToNotNullAttribute
                            tank = FMChannelHelper.MakeCall<ITanks, TankClass>(
												x =>
												x.Get(this.Security, Guid.Parse(this.Session["IdentityGuid"] as string))
										);
						}
						else
						{
							tank = new TankClass();
						}

						this.Session["Tank"] = tank;
					}

					//Set the title label with a key field from the bound object appended
					if (tank != null)
					{
						this.TankTitleLabel.Text = this.GetTitleLabelText(this.TankTitleLabel.Text, tank.ID);
					}

                    if (tank != null && (tank.DeviceTankType == DeviceTankTypes.Satellite
                                         && !(this.Security.HasRight(RIGHT.MODIFY_ASSET_TRACKING_DEVICES) || this.Security.HasRight(RIGHT.VIEW_ASSET_TRACKING_DEVICES))))
                        throw new Exception("Access Denied"); //sijuan: in case user views the page
                }
                
				this.tpGeneralPage.HeaderText = this.GetTranslatedText("General");

				if (this.Security.HasRight(RIGHT.VIEW_METERS) || this.Security.HasRight(RIGHT.MODIFY_METERS))
				{
					this.tpMetersPage.HeaderText = this.GetTranslatedText("Meters");
				}
				else
				{
					this.tpMetersPage.Visible = false;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.Session.Remove("Tank");
			}
		}

		private void CancelCommand(object sender, CommandEventArgs e)
		{
			this.Session.Remove("Tank");
			this.Session.Remove(SessionGeneralPageUiError);

			if (this.IsFromQueryWriter)
			{
				this.Redirect("..\\QueryWriterWebApp\\QueryResultsForm.aspx?Mode=Returning");
			}
			else
			{
				this.Redirect("TanksForm.aspx");
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.OK.Command += this.OkCommand;
			this.Cancel.Command += this.CancelCommand;
		}

		private void OkCommand(object sender, CommandEventArgs e)
		{
			try
			{
				// Do not perform the OK if there is a UI error on the
				// Tank General page.
				if (this.Session[SessionGeneralPageUiError] != null)
				{
					this.Session.Remove(SessionGeneralPageUiError);
					return;
				}

				var tank = (TankClass)this.Session["Tank"];

				Guid identityGuid = tank.IdentityGuid;
				if (tank.IdentityGuid != Guid.Empty)
				{
					FMChannelHelper.MakeCall<ITanks>(
																	 x =>
																	 x.Modify(this.Security, tank)
																);
				}
				else
				{
					tank.IdentityGuid = FMChannelHelper.MakeCall<ITanks, Guid>(
																	 x =>
																	 x.Add(this.Security, tank)
																);

				}

				try
				{
					ILoadRackManager loadRackManager = this.GetLoadRackManager();
					if (identityGuid != Guid.Empty)
					{
						loadRackManager.Modify(this.Security, typeof(TankClass), tank.IdentityGuid);
					}
					else
					{
						loadRackManager.Add(this.Security, typeof(TankClass), tank.IdentityGuid);
					}

                    // we need to save the changes to the loadrack database for disabled stations so the components match upon startup
				    this.UpdateLoadArmComponentConfiguration(this.Security, tank.IdentityGuid, true);
                }
                catch (SocketException socketExcept)
				{
                    if (socketExcept.ErrorCode == 10061)	// load rack not running
                    {
                        // we need to save the changes to the loadrack database so the components match upon startup
                        this.UpdateLoadArmComponentConfiguration(this.Security, tank.IdentityGuid, false);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Session.Remove("Tank");

			if (this.IsFromQueryWriter)
			{
				this.Redirect("..\\QueryWriterWebApp\\QueryResultsForm.aspx?Mode=Returning");
			}
			else
			{
				this.Redirect("TanksForm.aspx");
			}
		}

	    private void UpdateLoadArmComponentConfiguration(SecurityClass security, Guid identityGuid, bool skipEnabled)
	    {
            // we need to get all of the stations and then check the components assigned to each arm and make the required change
            StationCollectionClass stationCollection = FMChannelHelper.MakeCall<IStations, StationCollectionClass>(stations => stations.Enumerate(security));
            TankClass tank = FMChannelHelper.MakeCall<ITanks, TankClass>(tanks => tanks.Get(security, identityGuid));
            foreach (StationClass station in stationCollection)
            {
                if (station.Type == STATION_TYPE.LOAD_RACK || station.Type == STATION_TYPE.OFF_LOADING)
                {
                    var updateStation = false;
                    StationClass localStation = FMChannelHelper.MakeCall<IStations, StationClass>(stations => stations.Get(security, station.IdentityGuid));
                    if (skipEnabled && localStation.Enabled)
                    {
                        continue;
                    }

                    foreach (LoadArmClass loadArm in localStation.LoadArmCollection)
                    {
                        foreach (ProductMapClass component in loadArm.ComponentCollection)
                        {
                            if (component.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP
                                && component.TankOrGroupGuid == identityGuid)
                            {
                                component.TankOrGroupID = tank.ID;
                                component.AssignedGuid = tank.ProductGuid;
                                component.AssignedID = tank.ProductID;

                                component.AssignedDescription = tank.ProductID;
                                component.AssignedCode = tank.ProductCode;
                                updateStation = true;
                            }
                        }

                        foreach (ProductMapClass additive in loadArm.AdditiveInjectorCollection)
                        {
                            if (additive.Type == PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP
                                && additive.TankOrGroupGuid == identityGuid)
                            {
                                additive.TankOrGroupID = tank.ID;
                                additive.AssignedGuid = tank.ProductGuid;
                                additive.AssignedID = tank.ProductID;

                                additive.AssignedDescription = tank.ProductID;
                                additive.AssignedCode = tank.ProductCode;
                                updateStation = true;
                            }
                        }

                        foreach (ProductMapClass externalComponent in loadArm.ExternalComponentCollection)
                        {
                            if (externalComponent.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP
                                && externalComponent.TankOrGroupGuid == identityGuid)
                            {
                                externalComponent.TankOrGroupID = tank.ID;
                                externalComponent.AssignedGuid = tank.ProductGuid;
                                externalComponent.AssignedID = tank.ProductID;

                                externalComponent.AssignedDescription = tank.ProductID;
                                externalComponent.AssignedCode = tank.ProductCode;
                                updateStation = true;
                            }
                        }

                        foreach (ProductMapClass flowControlledAdditive in loadArm.FlowControlledAdditiveCollection)
                        {
                            if (flowControlledAdditive.Type == PRODUCT_MAP_TYPE.PRESET_FLOW_CONTROLLED_ADDITIVE_MAP
                                && flowControlledAdditive.TankOrGroupGuid == identityGuid)
                            {
                                flowControlledAdditive.TankOrGroupID = tank.ID;
                                flowControlledAdditive.AssignedGuid = tank.ProductGuid;
                                flowControlledAdditive.AssignedID = tank.ProductID;

                                flowControlledAdditive.AssignedDescription = tank.ProductID;
                                flowControlledAdditive.AssignedCode = tank.ProductCode;
                                updateStation = true;
                            }
                        }

                        foreach (ProductMapClass offloadProduct in loadArm.OffloadExternalProductCollection)
                        {
                            if (offloadProduct.Type == PRODUCT_MAP_TYPE.OFFLOAD_EXTERNAL_METER_MAP
                                && offloadProduct.TankOrGroupGuid == identityGuid)
                            {
                                offloadProduct.TankOrGroupID = tank.ID;
                                offloadProduct.AssignedGuid = tank.ProductGuid;
                                offloadProduct.AssignedID = tank.ProductID;

                                offloadProduct.AssignedDescription = tank.ProductID;
                                offloadProduct.AssignedCode = tank.ProductCode;
                                updateStation = true;
                            }
                        }
                    }
                    if (updateStation)
                    {
                        FMChannelHelper.MakeCall<IStations>(stations => stations.Modify(security, localStation));
                    }
                }
            }
        }

        #endregion
    }
}