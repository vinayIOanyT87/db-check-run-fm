// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TankLoadArmAssignmentForm.aspx.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LoadRackWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using System.Net.Sockets;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using FMControls;

	using FuelsManager.FMWebApp;

	/// <summary>
	///	Summary description for TankLoadArmAssignmentForm.
	/// </summary>
	public partial class TankLoadArmAssignmentForm : FMFormBase, IMenuDiscovery
	{
		#region Constants and Fields

		protected int AdditiveIndex;

		protected int ArmIndex;

		protected int ComponentIndex;

		protected bool DisplayArms;

		protected bool DisplayInjectors;

		protected bool DisplayMeters;

		protected Guid LocationGuid = Guid.Empty;

		protected Guid ProductGuid = Guid.Empty;

		protected Guid StationGuid;

		protected string Type;

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">
		/// The security object of the current session
		/// </param>
		/// <param name="siteGroup">
		/// Whether the current logged-in site is a site group
		/// </param>
		/// <param name="useNewLicenseKey"></param>
		/// <param name="options">
		/// Hardware key options
		/// </param>
		/// <param name="word1"></param>
		/// <param name="word2"></param>
		/// <returns>
		/// List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
				if (useNewLicenseKey == 1)
				{
					if ((word2 & 0x01) != 0x01)
						return null;
				}
				else
				{
					// Depends Upon Load Rack
					if ((options & 0x8000) == 0)
					{
						return null;
					}
				}

				var items = new List<FMMenuItem>();

			if (siteGroup)
			{
				return null;
			}

			if (!security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA) && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
			{
				return null;
			}

			items.Add(
				new FMMenuItem
					{
						MenuItemType = FMMenuItemType.OPERATIONS_LOAD_RACK_TANK_ASSIGNMENT, 
						RootMenuName = "Operations", 
						CategoryName = "Load Rack", 
						ItemName = "Tank Assignment", 
						NavigateUrl = "..\\LRWebApp\\TankLoadArmAssignmentForm.aspx", 
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});

			return items;
		}

		public void TypeDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			var typeDropDownList = (DropDownList)sender;
			this.Type = typeDropDownList.SelectedValue;
			this.LocationGuid = Guid.Empty;

			DataGridItem item = this.LocationAssignmentDataGrid.Items[this.LocationAssignmentDataGrid.EditItemIndex];
			var productGuidLabel = (Label)item.FindControl("ProductGuidLabel");
			this.ProductGuid = FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetMasterRecordGuid(this.Security, Guid.Parse(productGuidLabel.Text)));

			this.UpdateView();
		}

		#endregion

		#region Methods

		protected ListItemCollection EnumerateLocationTypes()
		{
			var typeItems = new ListItemCollection
				{
					new ListItem(this.GetTranslatedText("Tank"), "Tank"),
					new ListItem(this.GetTranslatedText("Group"), "Group")
				};

			return typeItems;
		}

		protected ListItemCollection EnumerateLocations()
		{
			var locationItems = new ListItemCollection();

			try
			{
					// Tanks map to products by product master record guid
				Guid productMasterGuid =
					FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetMasterRecordGuid(this.Security, this.ProductGuid));

				if (this.Type == "Tank")
				{
					StationClass station = FMChannelHelper.MakeCall<IStations, StationClass>(
						stations => stations.Get(this.Security, this.StationGuid));

					var collection = new List<TankClass>();

					FMChannelHelper.MakeCall<ITanks>(
						tanks =>
						{
							// enumerate all tanks for a meter station (23-Apr-2008)
							if (STATION_TYPE.METER == station.Type)
							{
								collection = tanks.Enumerate(this.Security);
							}
							else
							{
								collection = tanks.EnumerateByProduct(this.Security, productMasterGuid);
							}
						});

					foreach (TankClass o in collection)
					{
						BaseDataObject cdo = o;

						var newLocationItem = new ListItem(cdo.ID, cdo.IdentityGuid.ToString());
						foreach (ListItem existingLocationItem in locationItems)
						{
							if (string.Compare(existingLocationItem.Text, newLocationItem.Text, StringComparison.Ordinal) > 0)
							{
								int index = locationItems.IndexOf(existingLocationItem);
								locationItems.Insert(index, newLocationItem);
								newLocationItem = null;
								break;
							}
						}

						if (newLocationItem != null)
						{
							locationItems.Add(newLocationItem);
						}
					}
				}
				else
				{
					List<TankGroupClass> collection = FMChannelHelper.MakeCall<ITankGroups, List<TankGroupClass>>(
						tankGroups => tankGroups.EnumerateByProduct(this.Security, productMasterGuid));

					foreach (TankGroupClass o in collection)
					{
						BaseDataObject cdo = o;

						var newLocationItem = new ListItem(cdo.ID, cdo.IdentityGuid.ToString());
						foreach (ListItem existingLocationItem in locationItems)
						{
							if (string.Compare(existingLocationItem.Text, newLocationItem.Text, StringComparison.Ordinal) > 0)
							{
								int index = locationItems.IndexOf(existingLocationItem);
								locationItems.Insert(index, newLocationItem);
								newLocationItem = null;
								break;
							}
						}

						if (newLocationItem != null)
						{
							locationItems.Add(newLocationItem);
						}
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			if (locationItems.Count == 0)
			{
				if (this.Type == "Tank")
				{
					this.ErrorHandler(new Exception("No Tanks Available."));
				}
				else
				{
					this.ErrorHandler(new Exception("No Groups Available."));
				}
			}

			return locationItems;
		}

		protected void LocationAssignmentDataGridCancelCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				this.LocationAssignmentDataGrid.EditItemIndex = -1;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void LocationAssignmentDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			this.LocationAssignmentDataGrid.EditItemIndex = e.Item.ItemIndex;
			try
			{
				var productGuidLabel = (Label)e.Item.FindControl("ProductGuidLabel");
					this.ProductGuid = FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetMasterRecordGuid(this.Security, Guid.Parse(productGuidLabel.Text)));
				var typeLabel = (Label)e.Item.FindControl("LocationTypeLabel");
				if (this.GetTranslatedText("Tank") == typeLabel.Text)
				{
					this.Type = "Tank";
				}
				else
				{
					this.Type = "Group";
				}

				var locationGuidLabel = (Label)e.Item.FindControl("LocationGuidLabel");
				this.LocationGuid = Guid.Parse(locationGuidLabel.Text);

				// Update the station guid. (IGO 23-Apr-2008)
				var stationGuidLabel = (Label)e.Item.FindControl("StationGuidLabel");
				this.StationGuid = Guid.Parse(stationGuidLabel.Text);

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.LocationAssignmentDataGrid.EditItemIndex = -1;
				this.UpdateView();
			}
		}

		protected void LocationAssignmentDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.LocationAssignmentDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.LocationAssignmentDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		protected void LocationAssignmentDataGridUpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var typeDropDownList = (FMDropDownList)e.Item.FindControl("TypeDropDownList");
				var locationDropDownList = (DropDownList)e.Item.FindControl("LocationDropDownList");
				var stationGuidLabel = (Label)e.Item.FindControl("StationGuidLabel");
				var armLabel = (Label)e.Item.FindControl("ArmLabel");
				var componentIndexLabel = (Label)e.Item.FindControl("ComponentIndexLabel");
				var productGuidLabel = (Label)e.Item.FindControl("ProductGuidLabel");
				var productTypeLabel = (Label)e.Item.FindControl("ProductTypeLabel");

				if (typeDropDownList != null && locationDropDownList != null && locationDropDownList.SelectedIndex != -1
					&& stationGuidLabel != null && armLabel != null && componentIndexLabel != null && productGuidLabel != null
					&& productTypeLabel != null)
				{
					this.Type = typeDropDownList.SelectedValue;
					this.StationGuid = Guid.Parse(stationGuidLabel.Text);
					this.ProductGuid = FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetMasterRecordGuid(this.Security, Guid.Parse(productGuidLabel.Text)));

					FMChannelHelper.MakeCall<IStations>(stations =>
					{
						StationClass station = stations.Get(this.Security, this.StationGuid);

						if (station.IdentityGuid != this.StationGuid)
						{
							throw new Exception("[Invalid] [Station]");
						}

						switch (station.Type)
						{
							case STATION_TYPE.METER:
							{
								station.AssociatedTankGuid = new Guid(locationDropDownList.SelectedValue);
								station.AssociatedTankId = locationDropDownList.SelectedItem.Text;

								try
								{
									ILoadRackManager loadRackManager = this.GetLoadRackManager();
									loadRackManager.Purge(this.Security, typeof(StationClass), station.IdentityGuid);
								}
								catch (SocketException)
								{
								}

								stations.Modify(this.Security, station);

								try
								{
									ILoadRackManager loadRackManager = this.GetLoadRackManager();
									loadRackManager.Add(this.Security, typeof(StationClass), station.IdentityGuid);
								}
								catch (SocketException)
								{
								}

								break;
							}

							case STATION_TYPE.LOAD_RACK:
							{
								// Update the load rack station
								this.ArmIndex = Convert.ToInt32(armLabel.Text) - 1;
								this.ComponentIndex = Convert.ToInt32(componentIndexLabel.Text);

								if (this.ArmIndex >= station.LoadArmCollection.Count)
								{
									throw new Exception("[Invalid] [Arm]");
								}

								LoadArmClass loadArm = station.LoadArmCollection[this.ArmIndex];

								// Check product type, this allows updating of the Component Collection or the 
								// Additive Injector Collection. (IGO 22-Apr-2008)
								if (this.GetTranslatedText(ProductClass.ProductTypeID(ProductType.AdditiveProduct)) == productTypeLabel.Text)
								{
									if (this.ComponentIndex >= loadArm.AdditiveInjectorCollection.Count)
									{
										throw new Exception("[Invalid] [Additive]");
									}

									ProductMapClass additive = loadArm.AdditiveInjectorCollection[this.ComponentIndex];

									additive.TankOrGroupGuid = Guid.Parse(locationDropDownList.SelectedValue);
									additive.TankOrGroupID = locationDropDownList.SelectedItem.Text;

									try
									{
										ILoadRackManager loadRackManager = this.GetLoadRackManager();
										loadRackManager.Purge(this.Security, typeof(StationClass), station.IdentityGuid);
									}
									catch (SocketException)
									{
									}

									stations.Modify(this.Security, station);

									try
									{
										ILoadRackManager loadRackManager = this.GetLoadRackManager();
										loadRackManager.Add(this.Security, typeof(StationClass), station.IdentityGuid);
									}
									catch (SocketException)
									{
									}
								}
								else if (this.GetTranslatedText(ProductClass.ProductTypeID(ProductType.ComponentProduct)) == productTypeLabel.Text)
								{
									ProductMapClass component;
									if (this.ComponentIndex < loadArm.ExternalComponentCollection.Count)
									{
										component = loadArm.ExternalComponentCollection[this.ComponentIndex];
									}
									else if ((this.ComponentIndex -= loadArm.ExternalComponentCollection.Count) < loadArm.ComponentCollection.Count)
									{
										component = loadArm.ComponentCollection[this.ComponentIndex];
									}
									else
									{
										throw new Exception("[Invalid] [Component]");
									}

									if (component.TankOrGroupGuid != Guid.Parse(locationDropDownList.SelectedValue)
											|| (this.Type == "Tank" && (component.Type != PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP && component.Type != PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP))
											|| (this.Type != "Group" && (component.Type != PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP && component.Type != PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP)))
									{
										switch (this.Type)
										{
											case "Tank":
												if (component.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP)
												{
													component.Type = PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP;
												}
												else if (component.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP)
												{
													component.Type = PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP;
												}
												break;
											case "Group":
												if (component.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP)
												{
													component.Type = PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP;
												}
												else if (component.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP)
												{
													component.Type = PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP;
												}
												break;
											default:
												Console.WriteLine("The type returned was not a tank or group.");
												break;
										}
									}

									component.TankOrGroupGuid = Guid.Parse(locationDropDownList.SelectedValue);
									component.TankOrGroupID = locationDropDownList.SelectedItem.Text;

									try
									{
										ILoadRackManager loadRackManager = this.GetLoadRackManager();
										loadRackManager.Purge(this.Security, typeof(StationClass), station.IdentityGuid);
									}
									catch (SocketException)
									{
									}

									stations.Modify(this.Security, station);

									try
									{
										ILoadRackManager loadRackManager = this.GetLoadRackManager();
										loadRackManager.Add(this.Security, typeof(StationClass), station.IdentityGuid);
									}
									catch (SocketException)
									{
									}
								}
								break;
							}

							case STATION_TYPE.OFF_LOADING:
							{
								this.ArmIndex = Convert.ToInt32(armLabel.Text) - 1;
								this.ComponentIndex = Convert.ToInt32(componentIndexLabel.Text);

								if (this.ArmIndex >= station.LoadArmCollection.Count)
								{
									throw new Exception("[Invalid] [Arm]");
								}

								LoadArmClass loadArm = station.LoadArmCollection[this.ArmIndex];


								if (this.GetTranslatedText(ProductClass.ProductTypeID(ProductType.ComponentProduct)) == productTypeLabel.Text)
								{
									ProductMapClass component = null;
									if (this.ComponentIndex < loadArm.OffloadExternalProductCollection.Count)
									{
										component = loadArm.OffloadExternalProductCollection[this.ComponentIndex];
									}
									else
									{
										throw new Exception("[Invalid] [Component]");
									}

									if (component.TankOrGroupGuid != Guid.Parse(locationDropDownList.SelectedValue))
									{
										component.TankOrGroupGuid = Guid.Parse(locationDropDownList.SelectedValue);
										component.TankOrGroupID = locationDropDownList.SelectedItem.Text;

										try
										{
											ILoadRackManager loadRackManager = this.GetLoadRackManager();
											loadRackManager.Purge(Security, typeof(StationClass), station.IdentityGuid);
										}
										catch (System.Net.Sockets.SocketException ex)
										{
											if (ex.ErrorCode != 10061)
											{
												this.ErrorHandler(ex);
											}
										}

										stations.Modify(Security, station);

										try
										{
											ILoadRackManager loadRackManager = this.GetLoadRackManager();
											loadRackManager.Add(Security, typeof(StationClass), station.IdentityGuid);
										}
										catch (System.Net.Sockets.SocketException ex)
										{
											if (ex.ErrorCode != 10061)
											{
												this.ErrorHandler(ex);
											}
										}
									}
								}
								break;
							}
						}
					});
				}

				this.LocationAssignmentDataGrid.EditItemIndex = -1;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Raises the <see cref="OnInit" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
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
					// Load Filter Drop Down List on initial page load (IGO 23-Apr-2008)
					this.PopulateStationFilterDropDown();
					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void StationFilterDropDownSelectedIndexChanged(object sender, EventArgs e)
		{
			this.UpdateView();
		}

		private ICollection EnumerateLocationAssignments()
		{
			var locationAssignmentDataTable = new DataTable();
			FMChannelHelper.MakeCall<IStations>(
				stations =>
				{
					StationCollectionClass loadRackStationCollection = stations.EnumerateByType(this.Security, STATION_TYPE.LOAD_RACK);
					StationCollectionClass meterStationCollection = stations.EnumerateByType(this.Security, STATION_TYPE.METER);
					StationCollectionClass offLoadStationCollection = stations.EnumerateByType(this.Security, STATION_TYPE.OFF_LOADING);

					DataRow locationAssignmentDataRow;

					locationAssignmentDataTable.Columns.Add("StationID", typeof(string));
					locationAssignmentDataTable.Columns.Add("StationGuid", typeof(string));
					locationAssignmentDataTable.Columns.Add("Arm", typeof(string));
					locationAssignmentDataTable.Columns.Add("ComponentIndex", typeof(string));
					locationAssignmentDataTable.Columns.Add("ProductType", typeof(string));
					locationAssignmentDataTable.Columns.Add("ProductID", typeof(string));
					locationAssignmentDataTable.Columns.Add("ProductGuid", typeof(string));
					locationAssignmentDataTable.Columns.Add("LocationType", typeof(string));
					locationAssignmentDataTable.Columns.Add("LocationID", typeof(string));
					locationAssignmentDataTable.Columns.Add("LocationGuid", typeof(string));

					if (this.DisplayArms)
					{
						foreach (StationClass station in loadRackStationCollection)
						{
							station.Load(stations.Get(this.Security, station.IdentityGuid));

							this.ArmIndex = 0;
							foreach (LoadArmClass loadArm in station.LoadArmCollection)
							{
								this.ComponentIndex = 0;

								foreach (ProductMapClass component in loadArm.ExternalComponentCollection)
								{
									locationAssignmentDataRow = locationAssignmentDataTable.NewRow();

									locationAssignmentDataRow["StationID"] = station.ID;
									locationAssignmentDataRow["StationGuid"] = station.IdentityGuid.ToString();
									locationAssignmentDataRow["Arm"] = (this.ArmIndex + 1).ToString(CultureInfo.InvariantCulture);
									locationAssignmentDataRow["ComponentIndex"] = this.ComponentIndex.ToString(CultureInfo.InvariantCulture);
									locationAssignmentDataRow["ProductType"] = this.GetTranslatedText(ProductClass.ProductTypeID(component.AssignedProductType));
									locationAssignmentDataRow["ProductID"] = component.AssignedID;
									locationAssignmentDataRow["ProductGuid"] = component.AssignedGuid.ToString();

									if (component.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP)
									{
										locationAssignmentDataRow["LocationType"] = this.GetTranslatedText("Tank");
									}
									else
									{
										locationAssignmentDataRow["LocationType"] = this.GetTranslatedText("Group");
									}

									locationAssignmentDataRow["LocationID"] = component.TankOrGroupID;
									locationAssignmentDataRow["LocationGuid"] = component.TankOrGroupGuid;
									locationAssignmentDataTable.Rows.Add(locationAssignmentDataRow);
									this.ComponentIndex++;
								}

								foreach (ProductMapClass component in loadArm.ComponentCollection)
								{
									locationAssignmentDataRow = locationAssignmentDataTable.NewRow();

									locationAssignmentDataRow["StationID"] = station.ID;
									locationAssignmentDataRow["StationGuid"] = station.IdentityGuid.ToString();
									locationAssignmentDataRow["Arm"] = (this.ArmIndex + 1).ToString(CultureInfo.InvariantCulture);
									locationAssignmentDataRow["ComponentIndex"] = this.ComponentIndex.ToString(CultureInfo.InvariantCulture);
									locationAssignmentDataRow["ProductType"] = this.GetTranslatedText(ProductClass.ProductTypeID(component.AssignedProductType));
									locationAssignmentDataRow["ProductID"] = component.AssignedID;
									locationAssignmentDataRow["ProductGuid"] = component.AssignedGuid.ToString();

									if (component.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP)
									{
										locationAssignmentDataRow["LocationType"] = this.GetTranslatedText("Tank");
									}
									else
									{
										locationAssignmentDataRow["LocationType"] = this.GetTranslatedText("Group");
									}

									locationAssignmentDataRow["LocationID"] = component.TankOrGroupID;
									locationAssignmentDataRow["LocationGuid"] = component.TankOrGroupGuid;
									locationAssignmentDataTable.Rows.Add(locationAssignmentDataRow);
									this.ComponentIndex++;
								}

								if (this.DisplayInjectors)
								{
									this.AdditiveIndex = 0;
									foreach (ProductMapClass additive in loadArm.AdditiveInjectorCollection)
									{
										locationAssignmentDataRow = locationAssignmentDataTable.NewRow();

										locationAssignmentDataRow["StationID"] = station.ID;
										locationAssignmentDataRow["StationGuid"] = station.IdentityGuid.ToString();
										locationAssignmentDataRow["Arm"] = (this.ArmIndex + 1).ToString(CultureInfo.InvariantCulture);
										locationAssignmentDataRow["ComponentIndex"] = this.AdditiveIndex.ToString(CultureInfo.InvariantCulture);
										locationAssignmentDataRow["ProductType"] = this.GetTranslatedText(ProductClass.ProductTypeID(additive.AssignedProductType));
										locationAssignmentDataRow["ProductID"] = additive.AssignedID;
										locationAssignmentDataRow["ProductGuid"] = additive.AssignedGuid.ToString();
										locationAssignmentDataRow["LocationType"] = this.GetTranslatedText("Tank");
										locationAssignmentDataRow["LocationID"] = additive.TankOrGroupID;
										locationAssignmentDataRow["LocationGuid"] = additive.TankOrGroupGuid;
										locationAssignmentDataTable.Rows.Add(locationAssignmentDataRow);
										this.AdditiveIndex++;
									}
								}

								this.ArmIndex++;
							}
						}

						foreach (StationClass station in offLoadStationCollection)
						{
							station.Load(stations.Get(this.Security, station.IdentityGuid));

							this.ArmIndex = 0;
							foreach (LoadArmClass loadArm in station.LoadArmCollection)
							{
								this.ComponentIndex = 0;

								foreach (ProductMapClass component in loadArm.OffloadExternalProductCollection)
								{
									locationAssignmentDataRow = locationAssignmentDataTable.NewRow();

									locationAssignmentDataRow["StationID"] = station.ID;
									locationAssignmentDataRow["StationGuid"] = station.IdentityGuid.ToString();
									locationAssignmentDataRow["Arm"] = (this.ArmIndex + 1).ToString(CultureInfo.InvariantCulture);
									locationAssignmentDataRow["ComponentIndex"] = this.ComponentIndex.ToString(CultureInfo.InvariantCulture);
									locationAssignmentDataRow["ProductType"] = this.GetTranslatedText(ProductClass.ProductTypeID(component.AssignedProductType));
									locationAssignmentDataRow["ProductID"] = component.AssignedID;
									locationAssignmentDataRow["ProductGuid"] = component.AssignedGuid.ToString();
									locationAssignmentDataRow["LocationType"] = this.GetTranslatedText("Tank");
									locationAssignmentDataRow["LocationID"] = component.TankOrGroupID;
									locationAssignmentDataRow["LocationGuid"] = component.TankOrGroupGuid;
									locationAssignmentDataTable.Rows.Add(locationAssignmentDataRow);
									this.ComponentIndex++;
								}

								this.ArmIndex++;
							}
						}
					}

					// Meter Station
					if (this.DisplayMeters)
					{
						foreach (StationClass meterStation in meterStationCollection)
						{
							meterStation.Load(stations.Get(this.Security, meterStation.IdentityGuid));

							locationAssignmentDataRow = locationAssignmentDataTable.NewRow();

							try
							{
								// Get the product information for the associated tank
								StationClass tempMeterStation = meterStation;
								TankClass tank = FMChannelHelper.MakeCall<ITanks, TankClass>(
									tanks => tanks.Get(this.Security, tempMeterStation.AssociatedTankGuid));
								if (null != tank)
								{
									ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
										products => products.GetByInfoAuthorizedCompanies(this.Security, tank.ProductGuid, false, true));
									if (null != product)
									{
										locationAssignmentDataRow["StationID"] = meterStation.ID;
										locationAssignmentDataRow["StationGuid"] = meterStation.IdentityGuid.ToString();
										locationAssignmentDataRow["Arm"] = "N\\A";
										locationAssignmentDataRow["ComponentIndex"] = string.Empty;
										locationAssignmentDataRow["ProductType"] = this.GetTranslatedText(ProductClass.ProductTypeID(product.ProductType));
										locationAssignmentDataRow["ProductID"] = product.ID;
										locationAssignmentDataRow["ProductGuid"] = product.MasterRecordGuid.ToString();
										locationAssignmentDataRow["LocationType"] = this.GetTranslatedText("Tank");
										locationAssignmentDataRow["LocationID"] = meterStation.AssociatedTankId;
										locationAssignmentDataRow["LocationGuid"] = meterStation.AssociatedTankGuid;
										locationAssignmentDataTable.Rows.Add(locationAssignmentDataRow);
									}
								}
							}
							catch (Exception except)
							{
								this.ErrorHandler(except);
							}
						}
					}
				});

			var locationAssignmentDataView = new DataView(locationAssignmentDataTable);
			return locationAssignmentDataView;
		}

		/// <summary>
		///	Required method for Designer support - do not modify
		///	the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.LocationAssignmentDataGrid.EditCommand +=
				this.LocationAssignmentDataGridEditCommand;
			this.LocationAssignmentDataGrid.PageIndexChanged +=
				this.LocationAssignmentDataGridPageIndexChanged;
			this.LocationAssignmentDataGrid.CancelCommand +=
				this.LocationAssignmentDataGridCancelCommand;
			this.LocationAssignmentDataGrid.UpdateCommand +=
				this.LocationAssignmentDataGridUpdateCommand;
			this.LocationAssignmentDataGrid.ItemDataBound +=
				this.LocationAssignmentDataGridItemDataBound;
		}

		private void LocationAssignmentDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			try
			{
				if (this.LocationAssignmentDataGrid.EditItemIndex == e.Item.ItemIndex)
				{
					var typeDropDownList = (FMDropDownList)e.Item.FindControl("TypeDropDownList");
					var locationDropDownList = (DropDownList)e.Item.FindControl("LocationDropDownList");
					var stationGuidLabel = (Label)e.Item.FindControl("StationGuidLabel");
					var armLabel = (Label)e.Item.FindControl("ArmLabel");
					var componentIndexLabel = (Label)e.Item.FindControl("ComponentIndexLabel");
					var productGuidLabel = (Label)e.Item.FindControl("ProductGuidLabel");
					var productTypeLabel = (Label)e.Item.FindControl("ProductTypeLabel");

					if (typeDropDownList != null && locationDropDownList != null && stationGuidLabel != null && armLabel != null
						&& componentIndexLabel != null && productGuidLabel != null && productTypeLabel != null)
					{
						ListItemCollection itemCollection = locationDropDownList.Items;
						int index = itemCollection.IndexOf(itemCollection.FindByValue(this.LocationGuid.ToString()));
						locationDropDownList.SelectedIndex = index;

						typeDropDownList.SelectedValue = this.Type;

						// Disable type drop down list if the product type is additive. (IGO 22-Apr-2008)
						if (this.GetTranslatedText(ProductClass.ProductTypeID(ProductType.AdditiveProduct)) == productTypeLabel.Text)
						{
							typeDropDownList.Enabled = false;
						}
						else
						{
							this.StationGuid = Guid.Parse(stationGuidLabel.Text);
							StationClass station = FMChannelHelper.MakeCall<IStations, StationClass>(
								stations => stations.Get(this.Security, this.StationGuid));

							// Disable type drop down list if station type is meter (IGO 23-Apr-2008)
							if (STATION_TYPE.METER == station.Type)
							{
								typeDropDownList.Enabled = false;
							}
							else if (STATION_TYPE.LOAD_RACK == station.Type)
							{
								this.ArmIndex = Convert.ToInt32(armLabel.Text) - 1;
								this.ComponentIndex = Convert.ToInt32(componentIndexLabel.Text);

								if (this.ArmIndex >= station.LoadArmCollection.Count)
								{
									throw new Exception("[Invalid] [Arm]");
								}

								LoadArmClass loadArm = station.LoadArmCollection[this.ArmIndex];

								// Disable type drop down list if component is PRESET_EXTERNAL_COMPONENT
								if (this.ComponentIndex < loadArm.ExternalComponentCollection.Count)
								{
									typeDropDownList.Enabled = false;
								}
							}

							else if (STATION_TYPE.OFF_LOADING == station.Type)
							{
								typeDropDownList.Enabled = false;
							}
						}
					}
				}

				var editButton = (LinkButton)e.Item.FindControl("EditButton");
				if (editButton != null)
				{
					if (!this.Security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
					{
						editButton.Enabled = false;
						editButton.Text = "<img src=../FMWebApp/Images/Edit_un.gif border=0 align=absmiddle alt='Edit this item'>";
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void PopulateStationFilterDropDown()
		{
			this.StationFilterDropDown.Items.Clear();
			this.StationFilterDropDown.Items.Add("All");
			this.StationFilterDropDown.Items.Add("Arms");
			this.StationFilterDropDown.Items.Add("Arms and Injectors");
			this.StationFilterDropDown.Items.Add("Meters");

			// Set the current selection to ALL
			this.StationFilterDropDown.SelectedIndex = 0;
		}

		private void SetStationDisplayFlags()
		{
			// All
			if (0 == this.StationFilterDropDown.SelectedIndex)
			{
				this.DisplayArms = true;
				this.DisplayInjectors = true;
				this.DisplayMeters = true;
			}
			else if (1 == this.StationFilterDropDown.SelectedIndex)
			{
				// Arms
				this.DisplayArms = true;
				this.DisplayInjectors = false;
				this.DisplayMeters = false;
			}
			else if (2 == this.StationFilterDropDown.SelectedIndex)
			{
				// Arms and Injectors
				this.DisplayArms = true;
				this.DisplayInjectors = true;
				this.DisplayMeters = false;
			}
			else if (3 == this.StationFilterDropDown.SelectedIndex)
			{
				// Meters
				this.DisplayArms = false;
				this.DisplayInjectors = false;
				this.DisplayMeters = true;
			}
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		private void UpdateView()
		{
			this.SetStationDisplayFlags();

			ICollection locations = this.EnumerateLocationAssignments();

			this.ArmAssignFormPageSizeDropDown.SetPageSize(this.LocationAssignmentDataGrid, locations.Count);

			this.LocationAssignmentDataGrid.DataSource = locations;
			this.LocationAssignmentDataGrid.DataBind();
		}

		#endregion
	}
}