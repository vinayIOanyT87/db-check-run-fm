// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TanksForm.aspx.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Defines the TanksForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Runtime.InteropServices;
	using System.Net;
	using System.Net.Sockets;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;



	using OpcCom;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	using TankClass = FMBusinessObjects.DataObjects.TankClass;
	using ITankCollection = Interop.DataObjects.ITankCollection;



	/// <summary>
	///	Summary description for TanksForm.
	/// </summary>
	public partial class TanksForm : FMFormBase, IEntityDiscovery, IMenuDiscovery
	{
		/// <summary>
		/// Retain the state of the Show Hidden checkbox
		/// </summary>
		private bool SessionTankSummaryShowHiddenChecked
		{
			get
			{
				if (this.Session["TankSummaryShowHiddenChecked"] is bool)
				{
					return (bool)this.Session["TankSummaryShowHiddenChecked"];
				}
				else
				{
					return false;
				}
			}

			set
			{
				this.Session.Add("TankSummaryShowHiddenChecked", value);
			}
		}

		#region Explicit Interface Properties

		bool IEntityDiscovery.EntityAssignable => false;

		Type IEntityDiscovery.EntityEngineType => typeof(ITanks);

		ENTITY_TYPE IEntityDiscovery.EntityType => ENTITY_TYPE.TANK;

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///	Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <param name="options">Hardware key options</param>
		/// <returns>
		///	List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
				if (useNewLicenseKey == 1)
				{

				}
				else
				{
					// Depends Upon WEB Inventory
					if ((options & 0x28000) == 0)
					{
						return null;
					}

					// Depends Upon Accounting
					if ((options & 0x80100) == 0)
					{
						return null;
					}
				}
				var items = new List<FMMenuItem>();

			//// Site Groups don't have Tanks
			if (siteGroup)
			{
				return null;
			}

			if (!security.HasRight(RIGHT.VIEW_TANK_DATA) && !security.HasRight(RIGHT.MODIFY_TANK_DATA))
			{
				return null;
			}


			items.Add(
				new FMMenuItem
					{
						MenuItemType = FMMenuItemType.ASSETS_EQUIPMENT_TANKS,
						RootMenuName = "Assets",
						CategoryName = "Equipment",
						ItemName = "Tanks",
						NavigateUrl = "TanksForm.aspx",
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});

			return items;
		}

		#endregion

		#region Explicit Interface Methods

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(
			SecurityClass securityParam, ENTITY_ASSIGNMENT_TYPE type)
		{
			var tankCollection = FMChannelHelper.MakeCall<ITanks, List<TankClass>>(
				x =>
						x.Enumerate(securityParam)
				);

			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (TankClass tank in tankCollection)
			{
				if (type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (securityParam.SiteGuid == tank.SiteGuid)
					{
						continue;
					}

					if (securityParam.LoginSiteGuid != tank.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (securityParam.SiteGuid != tank.SiteGuid)
					{
						continue;
					}
				}

				var entityToSiteMap = new EntityToSiteMapClass(tank);
				entityToSiteMapCollection.Add(entityToSiteMap);
			}

			return entityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string idParam)
		{
			return Guid.Empty;
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid siteGuid)
		{
			TankClass tank = FMChannelHelper.MakeCall<ITanks, TankClass>(
																	x =>
																	x.Get(security, guid)
																);

			tank.SiteGuid = siteGuid;
			FMChannelHelper.MakeCall<ITanks>(
																	x =>
																	x.Modify(security,tank)
																);
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

		protected void PageSizeDropDownSelectedIndexChanged(object source, EventArgs e)
		{
			this.UpdateView();
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
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
						this.AutoCreateButton.Enabled = false;
					}

					this.ShowHiddenCheckBox.Checked = this.SessionTankSummaryShowHiddenChecked;

					if (this.Session["TanksPage"] != null)
					{
						this.TanksDataGrid.CurrentPageIndex = (int)this.Session["TanksPage"];
						this.Session.Remove("TanksPage");
					}

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}


		/// <summary>
		/// When the user checks or unchecks the Show Hidden checkbox, update the view
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		protected void ShowHiddenCheckBox_OnCheckedChanged(object sender, EventArgs e)
		{
				try
				{
					this.SessionTankSummaryShowHiddenChecked = this.ShowHiddenCheckBox.Checked;
					this.UpdateView();
				}
				catch (Exception ex)
				{
					this.ErrorHandler(ex);
				}
		}

		private void AddButtonCommand(object sender, CommandEventArgs e)
		{
			this.Session.Remove("Tank");
			this.Session.Remove("IdentityGuid");
			this.Session["TanksPage"] = this.TanksDataGrid.CurrentPageIndex;
			this.Redirect("TankForm.aspx");
		}

		private void AutoCreateButtonCommand(object sender, CommandEventArgs e)
		{
			try
			{

				var tankDictionary = FMChannelHelper.MakeCall<IPoints, Dictionary<string, Dictionary<Guid, Guid>>>(
																  x =>
																  x.EnumerateTerminalAutomationTankTags(this.Security, this.Security.SiteGuid)
															  );



				foreach (var tankID in tankDictionary.Keys)
				{

					if (this.GetIdentityGuidForTank(this.Security, tankID) == Guid.Empty)
					{
						var tank = new TankClass { ID = tankID };

						var tagDictionary = tankDictionary[tankID];

						foreach (ProcessVariableClass processVariable in tank.ProcessVariableCollection)
						{
							processVariable.URL = string.Empty;
							processVariable.ProgID = string.Empty;

							switch (processVariable.ProcessVariableType)
							{
								case PROCESS_VARIABLE_TYPE.LEVEL_PV:
									if (tagDictionary.ContainsKey(Guids.LevelProductGuid))
									{
										processVariable.OPCItemID = tagDictionary[Guids.LevelProductGuid].ToString();
									}
									break;
								case PROCESS_VARIABLE_TYPE.TEMPERATURE_PV:
									if (tagDictionary.ContainsKey(Guids.TemperatureProductGuid))
									{
										processVariable.OPCItemID = tagDictionary[Guids.TemperatureProductGuid].ToString();
									}
									break;
								case PROCESS_VARIABLE_TYPE.GROSS_VOLUME_PV:
									if (tagDictionary.ContainsKey(Guids.VolumeGrossObservedGuid))
									{
										processVariable.OPCItemID = tagDictionary[Guids.VolumeGrossObservedGuid].ToString();
									}
									break;
								case PROCESS_VARIABLE_TYPE.AVAILABLE_GROSS_VOLUME_PV:
									if (tagDictionary.ContainsKey(Guids.VolumeGrossObservedAvailableGuid))
									{
										processVariable.OPCItemID = tagDictionary[Guids.VolumeGrossObservedAvailableGuid].ToString();
									}
									break;
								case PROCESS_VARIABLE_TYPE.REMAINING_GROSS_VOLUME_PV:
									if (tagDictionary.ContainsKey(Guids.VolumeGrossObservedRemainingGuid))
									{
										processVariable.OPCItemID = tagDictionary[Guids.VolumeGrossObservedRemainingGuid].ToString();
									}
									break;
								case PROCESS_VARIABLE_TYPE.TANK_OPERATION_PV:
									if (tagDictionary.ContainsKey(Guids.OperationalModeGuid))
									{
										processVariable.OPCItemID = tagDictionary[Guids.OperationalModeGuid].ToString();
									}
									break;
								case PROCESS_VARIABLE_TYPE.NET_VOLUME_PV:
									if (tagDictionary.ContainsKey(Guids.VolumeNetStandardGuid))
									{
										processVariable.OPCItemID = tagDictionary[Guids.VolumeNetStandardGuid].ToString();
									}
									break;
								case PROCESS_VARIABLE_TYPE.DENSITY_PV:
									if (tagDictionary.ContainsKey(Guids.DensityProductObservedGuid))
									{
										processVariable.OPCItemID = tagDictionary[Guids.DensityProductObservedGuid].ToString();
									}
									break;
								case PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV:
									if (tagDictionary.ContainsKey(Guids.DensityProductStandardGuid))
									{
										processVariable.OPCItemID = tagDictionary[Guids.DensityProductStandardGuid].ToString();
									}
									break;
								case PROCESS_VARIABLE_TYPE.MASS_PV:
									if (tagDictionary.ContainsKey(Guids.MassLiquidGuid))
									{
										processVariable.OPCItemID = tagDictionary[Guids.MassLiquidGuid].ToString();
									}
									break;
								case PROCESS_VARIABLE_TYPE.VCF_PV:
									if (tagDictionary.ContainsKey(Guids.VolumeCorrectionFactorGuid))
									{
										processVariable.OPCItemID = tagDictionary[Guids.VolumeCorrectionFactorGuid].ToString();
									}
									break;
								case PROCESS_VARIABLE_TYPE.VAPOR_PRESSURE_PV:
									if (tagDictionary.ContainsKey(Guids.PressureVaporGuid))
									{
										processVariable.OPCItemID = tagDictionary[Guids.PressureVaporGuid].ToString();
									}
									break;
								case PROCESS_VARIABLE_TYPE.AVAILABLE_NET_VOLUME_PV:
									if (tagDictionary.ContainsKey(Guids.VolumeNetStandardAvailableGuid))
									{
										processVariable.OPCItemID = tagDictionary[Guids.VolumeNetStandardAvailableGuid].ToString();
									}
									break;
								case PROCESS_VARIABLE_TYPE.REMAINING_NET_VOLUME_PV:
									if (tagDictionary.ContainsKey(Guids.VolumeNetStandardRemainingGuid))
									{
										processVariable.OPCItemID = tagDictionary[Guids.VolumeNetStandardRemainingGuid].ToString();
									}
									break;
								case PROCESS_VARIABLE_TYPE.TANK_STATUS_PV:
									if (tagDictionary.ContainsKey(Guids.TankStatusGuid))
									{
										processVariable.OPCItemID = tagDictionary[Guids.TankStatusGuid].ToString();
									}
									break;
								default:
									processVariable.OPCItemID = Guid.Empty.ToString();
									break;
							}
						}
/*
						switch (fmTank.VesselType)
						{
							case 0: // SHELL_CYL
								tank.VesselType = VESSEL_TYPE.CYLINDRICAL_VESSEL;
								break;
							case 1: // SHELL_HORIZ
								tank.VesselType = VESSEL_TYPE.BULLET_VESSEL;
								break;
							case 2: // SHELL_HORIZ_FLAT
								tank.VesselType = VESSEL_TYPE.BULLET_VESSEL;
								break;
							case 3: // SHELL_STANDARD_SPHERE
								tank.VesselType = VESSEL_TYPE.SPHERICAL_VESSEL;
								break;
							case 4: // SHELL_POLYNOMIAL_SPHERE
								tank.VesselType = VESSEL_TYPE.SPHERICAL_VESSEL;
								break;
							case 5: // SHELL_UGHORIZ_ROUND
								tank.VesselType = VESSEL_TYPE.UNDERGROUND_VESSEL;
								break;
							case 6: // SHELL_UGHORIZ_FLAT
								tank.VesselType = VESSEL_TYPE.UNDERGROUND_VESSEL;
								break;
							case 7:	// TANK_SHAPE_FUELBAG
								tank.VesselType = VESSEL_TYPE.COLLAPSIBLE_STORAGE_TANK;
								break;
							default:
								tank.VesselType = VESSEL_TYPE.CYLINDRICAL_VESSEL;	// default to a cylindrical tank
								break;
						}


						tank.ProductGuid = FMChannelHelper.MakeCall<IProducts, Guid>(
																	x =>
																	x.GetIdentityGuid(this.Security, fmTank.ProductID)
																);
*/
						tank.IdentityGuid = FMChannelHelper.MakeCall<ITanks, Guid>(
																	x =>
																	x.Add(this.Security,tank)
																);

						try
						{
							ILoadRackManager loadRackManager = this.GetLoadRackManager();
							loadRackManager.Add(this.Security, typeof(TankClass), tank.IdentityGuid);
						}
						catch (SocketException socketExcept)
						{
							if (socketExcept.ErrorCode != 10061)
							{
								throw;
							}
						}
					}
				}


				this.UpdateView();
			}
			catch (Exception except)
			{
				if (except.Message.ToUpper().Contains("COCREATEINSTANCEEX"))
				{
					this.ErrorHandler(new Exception("SCADA is not available."));
				}
				else
				{
					this.ErrorHandler(except);
				}
			}



		}

		private Guid GetIdentityGuidForTank(SecurityClass securityClass, string key)
		{
			return FMChannelHelper.MakeCall<ITanks, Guid>(
																	x =>
																	x.GetIdentityGuid(securityClass, key)
																);
		}

		/// <summary>
		///	Required method for Designer support - do not modify
		///	the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton2.Command += this.AddButtonCommand;
			this.TanksDataGrid.EditCommand +=
				this.TanksDataGridEditCommand;
			this.TanksDataGrid.PageIndexChanged +=
				this.TanksDataGridPageIndexChanged;
			this.TanksDataGrid.DeleteCommand +=
				this.TanksDataGridDeleteCommand;
			this.TanksDataGrid.ItemDataBound +=
				this.TanksDataGridItemDataBound;
			this.AddButton.Command += this.AddButtonCommand;
			this.AutoCreateButton.Command += this.AutoCreateButtonCommand;
		}


		private void TanksDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Get identityGuid
				TableCell identityGuidCell = e.Item.Cells[1];//bds
				Guid identityGuid = Guid.Parse(identityGuidCell.Text);

				try
				{
					ILoadRackManager loadRackManager = this.GetLoadRackManager();
					loadRackManager.Purge(this.Security, typeof(TankClass), identityGuid);
				}
				catch (SocketException socketExcept)
				{
					if (socketExcept.ErrorCode != 10061)
					{
						throw;
					}
				}

				FMChannelHelper.MakeCall<ITanks>(
																	x =>
																	x.Purge(this.Security, identityGuid)
															);

				this.TanksDataGrid.SelectedIndex = -1;
				this.Session.Remove("IdentityGuid");
				if (this.TanksDataGrid.Items.Count == 1 && this.TanksDataGrid.CurrentPageIndex > 0)
				{
					this.TanksDataGrid.CurrentPageIndex--;
				}
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void TanksDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			this.Session.Remove("Tank");
			TableCell identityGuidCell = e.Item.Cells[1];//bds
			this.Session["IdentityGuid"] = identityGuidCell.Text;
			this.Session["TanksPage"] = this.TanksDataGrid.CurrentPageIndex;
			this.Redirect("TankForm.aspx");
		}

		private void TanksDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			try
			{
				if (e.Item.ItemIndex >= 0)
				{
						var deleteButton = (LinkButton)e.Item.FindControl("DeleteButton");
						if (deleteButton != null)
						{
							if (!this.Security.HasRight(RIGHT.MODIFY_TANK_DATA))
							{
								deleteButton.Enabled = false;
								deleteButton.Text =
										"<img src=Images/Delete_un.gif border=0 align=absmiddle alt='Delete this item'>";
							}
						}

						// Product and Manager may contain {None}
						e.Item.Cells[3].Text = e.Item.Cells[3].Text.Replace(">", "&gt");//bds
						e.Item.Cells[3].Text = e.Item.Cells[3].Text.Replace("<", "&lt");//bds
						e.Item.Cells[4].Text = e.Item.Cells[4].Text.Replace(">", "&gt");//bds
						e.Item.Cells[4].Text = e.Item.Cells[4].Text.Replace("<", "&lt");//bds
				}

				// Change the color of the text of hidden tanks to give the user a visual indication that the tank is hidden.
				TankClass tank = e.Item.DataItem as TankClass;

				if (tank?.HiddenDate != null)
				{
						e.Item.ForeColor = Color.Red;
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		private void TanksDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.TanksDataGrid.EditItemIndex > -1)
			{
				return;
			}
			this.TanksDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		private void UpdateView()
		{
			List<TankClass> tankCollection = FMChannelHelper.MakeCall<ITanks, List<TankClass>>(
																	x =>
																	x.Enumerate(this.Security, hideHiddenTanks: !this.ShowHiddenCheckBox.Checked)
																);

			this.TanksFormPageSizeDropDown.SetPageSize(this.TanksDataGrid, tankCollection.Count);

			this.TanksDataGrid.DataSource = tankCollection;
			this.TanksDataGrid.DataBind();
		}

		#endregion
	}
}