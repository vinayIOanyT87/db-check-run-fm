// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TankGeneralPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TankGeneralPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using System.Security;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using Areas.AssetTrackingArea.Controllers;

	using Opc;
	using Opc.Da;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	using Convert = System.Convert;

    /// <summary>
	///    This page displays general tank information, including process variables
	/// </summary>
	public partial class TankGeneralPage : FMUserControlBase
	{
	    private bool coordinateValidationError;
	    private const string SessionPreviousDeviceGuid = "TankGeneralPage.PreviousDeviceGuid";

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
				this.Session.Remove(TankForm.SessionGeneralPageUiError);

				if (this.Page.IsPostBack == false)
				{
					this.Session.Remove(SessionPreviousDeviceGuid);
					var tank = (TankClass)this.Session["Tank"];

					// Set the coordinate info for the calculate coordinate popup.
					this.SetCoordinateInfoInSession(tank);

					this.PopulateTankTypeDropdown(tank.DeviceTankType);
					this.TankTypeDropdownSelectedIndexChanged(null, null);

					this.PopulateTankConfigurationNumberDropdown(tank.TankConfigurationNumber);

					this.TankID.Text			= tank.ID;
					this.LatitudeTextBox.Text	= tank.Latitude.ToString();
					this.LongitudeTextBox.Text	= tank.Longitude.ToString();
					this.ZoomTextBox.Text		= tank.Zoom.ToString();

				    this.HiddenCheckBox.Checked = tank.HiddenDate.HasValue;

                    // Get asset tracking devices and load dropdown.
                    if (tank.DeviceTankType == DeviceTankTypes.Satellite)
                    {
                        this.Session.Add(SessionPreviousDeviceGuid, tank.AssetTrackingDeviceGuid);
                        this.LoadTrackingDeviceDropdown(tank);
                    }

					// Get Product
					var product = new ProductClass();

					if (tank.ProductGuid != Guid.Empty)
					{
						product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
																	 x =>
																	 x.GetByProductAuthorizedCompanies(this.Security, tank.ProductGuid, false)
																);
					}

					// Populate the ProductTypeDropDownList
					for (var productType = ProductType.ComponentProduct; productType < ProductType.MaxProduct; productType++)
					{
						if (productType == ProductType.AdditizedProduct)
						{
							continue;
						}

						var newTypeItem = new ListItem(ProductClass.ProductTypeID(productType), ((int)productType).ToString("G", CultureInfo.InvariantCulture));
						this.ProductTypeDropDownList.Items.Add(newTypeItem);

						if (product.IdentityGuid != Guid.Empty && product.ProductType == productType)
						{
							this.ProductTypeDropDownList.SelectedIndex = this.ProductTypeDropDownList.Items.Count - 1;
						}
					}

					this.Session["ProductType"] = (ProductType)Convert.ToInt32(this.ProductTypeDropDownList.SelectedItem.Value, CultureInfo.InvariantCulture);

					// Populate VesselTypeDropDownList
					VESSEL_TYPE[] vesselType =
						{
							VESSEL_TYPE.SPHERICAL_VESSEL, VESSEL_TYPE.CYLINDRICAL_VESSEL,
							VESSEL_TYPE.BULLET_VESSEL, VESSEL_TYPE.PROPANE_VESSEL, VESSEL_TYPE.UNDERGROUND_VESSEL, VESSEL_TYPE.TANKER_VESSEL,
							VESSEL_TYPE.PIPELINE_VESSEL, VESSEL_TYPE.OTHER_VESSEL, VESSEL_TYPE.COLLAPSIBLE_STORAGE_TANK, VESSEL_TYPE.UNDEFINED_VESSEL
						};

					int iVesselType = 0;

					while (vesselType[iVesselType] != VESSEL_TYPE.UNDEFINED_VESSEL)
					{
						this.VesselTypeDropDownList.Items.Add(
							new ListItem(TankClass.VesselTypeID(vesselType[iVesselType]), ((int)vesselType[iVesselType]).ToString("G", CultureInfo.InvariantCulture)));

						if (tank.VesselType == vesselType[iVesselType])
						{
							this.VesselTypeDropDownList.SelectedIndex = this.VesselTypeDropDownList.Items.Count - 1;
						}

						iVesselType++;
					}

					this.ProductTypeDropDownListSelectedIndexChanged(null, null);								

					CompanyCollectionClass companyCollection = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
						x =>
						x.EnumerateByRole(this.Security, COMPANY_ROLE.MANAGER, byGroupCompanies: false, bLocalize: true)
					);

					foreach (CompanyClass company in companyCollection)
					{
						var newCompanyItem = new ListItem(company.ID, company.MasterRecordGuid.ToString());

						foreach (ListItem existingCompanyItem in this.ManagersDropDownList.Items)
						{
							if (string.Compare(existingCompanyItem.Text, newCompanyItem.Text, StringComparison.Ordinal) > 0)
							{
								int index = this.ManagersDropDownList.Items.IndexOf(existingCompanyItem);
								this.ManagersDropDownList.Items.Insert(index, newCompanyItem);

								if (tank.ManagerGuid == company.MasterRecordGuid)
								{
									this.ManagersDropDownList.SelectedIndex = index;
								}

								newCompanyItem = null;
								break;
							}
						}

						if (newCompanyItem != null)
						{
							this.ManagersDropDownList.Items.Add(newCompanyItem);

							if (tank.ManagerGuid == company.MasterRecordGuid)
							{
								this.ManagersDropDownList.SelectedIndex = this.ManagersDropDownList.Items.Count - 1;
							}
						}
					}

                    var item = new ListItem(this.GetTranslatedText("{None}"), Guid.Empty.ToString());
                    this.ManagersDropDownList.Items.Insert(0, item);

                    CompanyCollectionClass ownerCollection = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
                        x =>
                        x.EnumerateByRole(this.Security, COMPANY_ROLE.OWNER, byGroupCompanies: false, bLocalize: true)
                    );

                    foreach (CompanyClass owner in ownerCollection)
                    {                        
                        var newOwnerItem = new ListItem(owner.ID, owner.MasterRecordGuid.ToString());

                        foreach (ListItem existingOwnerItem in this.OwnersDownlist.Items)
                        {
                            if (string.Compare(existingOwnerItem.Text, newOwnerItem.Text, StringComparison.Ordinal) > 0)
                            {
                                int index = this.OwnersDownlist.Items.IndexOf(existingOwnerItem);
                                this.OwnersDownlist.Items.Insert(index, newOwnerItem);
                                if (tank.OwnerGuid == owner.MasterRecordGuid)
                                {
                                    this.OwnersDownlist.SelectedIndex = index;
                                }
                                newOwnerItem = null;
                                break;
                            }
                        }

                        if (newOwnerItem != null)
                        {
                            this.OwnersDownlist.Items.Add(newOwnerItem);
                            if (tank.OwnerGuid == owner.MasterRecordGuid)
                                this.OwnersDownlist.SelectedIndex = this.OwnersDownlist.Items.Count - 1;
                        }
                    }
                    this.OwnersDownlist.Items.Insert(0, item);
                    this.UpdateProcessVariablesView();
				}
				else
				{
					var tank = (TankClass)this.Session["Tank"];

					tank.ID = this.TankID.Text;
					tank.VesselType = (VESSEL_TYPE)Convert.ToInt32(this.VesselTypeDropDownList.SelectedValue, CultureInfo.InvariantCulture);

					if (this.ProductsDropDownList.SelectedItem.Text == this.GetTranslatedText("{None}"))
					{
						tank.ProductGuid = Guid.Empty;
						tank.ProductID = "{None}";
					}
					else
					{
						tank.ProductGuid = Guid.Parse(this.ProductsDropDownList.SelectedValue);
						tank.ProductID = this.ProductsDropDownList.SelectedItem.Text;
					}

					if (this.ManagersDropDownList.SelectedItem.Text == this.GetTranslatedText("{None}"))
					{
						tank.ManagerGuid = Guid.Empty;
						tank.ManagerID = "{None}";
					}
					else
					{
						tank.ManagerGuid = Guid.Parse(this.ManagersDropDownList.SelectedValue);
						tank.ManagerID = this.ManagersDropDownList.SelectedItem.Text;
					}

                    if (this.OwnersDownlist.SelectedItem.Text == this.GetTranslatedText("<None>"))
                    {
                        tank.OwnerGuid = Guid.Empty;
                        tank.OwnerID = "<None>";
                    }
                    else
                    {
                        tank.OwnerGuid = Guid.Parse(this.OwnersDownlist.SelectedValue);
                        tank.OwnerID = this.OwnersDownlist.SelectedItem.Text;
                    }

                    // Only set the hidden date if the hidden check box is checked and there isn't already a value
                    if (this.HiddenCheckBox.Checked && !tank.HiddenDate.HasValue)
                    {
                        tank.HiddenDate = DateTimeOffset.Now;
                    }
                    else if (!this.HiddenCheckBox.Checked)
                    {
                        tank.HiddenDate = null;
                    }

					var selectedTankType = (DeviceTankTypes)int.Parse(this.TankTypeDropdown.SelectedValue);
					tank.DeviceTankType = selectedTankType;

					if (selectedTankType == DeviceTankTypes.Opc)
					{
						tank.AssetTrackingDeviceGuid	= Guid.Empty;
						tank.TrackingDeviceId			= "{None}";
						tank.Latitude					= null;
						tank.Longitude					= null;
						tank.Zoom						= null;
						tank.TankConfigurationNumber	= null;
					}
					else
					{
						tank.AssetTrackingDeviceGuid = Guid.Parse(this.TrackingDeviceDropdown.SelectedValue);
						tank.TrackingDeviceId = this.TrackingDeviceDropdown.SelectedItem.Text;

						if (string.IsNullOrEmpty(this.LatitudeTextBox.Text)
						    && string.IsNullOrEmpty(this.LongitudeTextBox.Text)
							&& string.IsNullOrEmpty(this.ZoomTextBox.Text))
						{
							tank.Latitude	= null;
							tank.Longitude	= null;
							tank.Zoom		= null;
						}
						else
						{
							this.ValidateCoordinates();
							tank.Latitude	= double.Parse(this.LatitudeTextBox.Text);
							tank.Longitude	= double.Parse(this.LongitudeTextBox.Text);
							tank.Zoom		= int.Parse(this.ZoomTextBox.Text);
						}

						tank.TankConfigurationNumber = null;
						int selectedTankNumber = int.Parse(this.TankConfigNumberDropdown.SelectedValue);

						if (selectedTankNumber > 0)
						{
							tank.TankConfigurationNumber = selectedTankNumber;
						}
					}
                    this.Session["Tank"] = tank;
                    this.SetCoordinateInfoInSession();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);

				if (this.coordinateValidationError == false)
				{
					this.Session.Remove("Tank");
				}
				else
				{
					this.Session.Add(TankForm.SessionGeneralPageUiError, true);
				}
			}
		}

		/// <summary>
		/// This method will set the coordinate info into session for the popup.
		/// </summary>
	    private void SetCoordinateInfoInSession()
	    {
		    if (string.IsNullOrEmpty(this.LatitudeTextBox.Text) == false)
		    {
			    this.Session[AssetCalculateCoordinatesController.SessionCalculateCoordinateLatitude] = this.LatitudeTextBox.Text;
		    }

			if (string.IsNullOrEmpty(this.LongitudeTextBox.Text) == false)
			{
				this.Session[AssetCalculateCoordinatesController.SessionCalculateCoordinateLongitude] = this.LongitudeTextBox.Text;
			}

			if (string.IsNullOrEmpty(this.ZoomTextBox.Text) == false)
			{
				this.Session[AssetCalculateCoordinatesController.SessionCalculateCoordinateZoom] = this.ZoomTextBox.Text;
			}
		}

		/// <summary>
		/// This method will set the coordinate info into session for the popup.
		/// </summary>
		private void SetCoordinateInfoInSession(TankClass inTank)
		{
			if (this.Session[AssetCalculateCoordinatesController.SessionCalculateCoordinateZoom] == null)
			{
				this.Session.Add(AssetCalculateCoordinatesController.SessionCalculateCoordinateZoom, "1");
			}

			if (inTank.Latitude != null)
			{
				this.Session.Add(AssetCalculateCoordinatesController.SessionCalculateCoordinateLatitude, inTank.Latitude.ToString());
			}

			if (inTank.Longitude != null)
			{
				this.Session.Add(AssetCalculateCoordinatesController.SessionCalculateCoordinateLongitude, inTank.Longitude.ToString());
			}

			if (inTank.Zoom != null)
			{
				this.Session.Add(AssetCalculateCoordinatesController.SessionCalculateCoordinateZoom, inTank.Zoom.ToString());
			}
		}

		/// <summary>
		/// This method will validate the latitude and longitude. If one is present, then
		/// must have both. Must be numeric values and be in the correct range.
		/// </summary>
		private void ValidateCoordinates()
		{
			this.coordinateValidationError = false;

		    if ((string.IsNullOrEmpty(this.LatitudeTextBox.Text) == false && string.IsNullOrEmpty(this.LongitudeTextBox.Text))
				|| (string.IsNullOrEmpty(this.LongitudeTextBox.Text) == false && string.IsNullOrEmpty(this.LatitudeTextBox.Text)))
		    {
			    this.coordinateValidationError = true;
			    throw new Exception("Must have both Latitude and Longitude.");
		    }

			if ((string.IsNullOrEmpty(this.LatitudeTextBox.Text) == false || string.IsNullOrEmpty(this.LongitudeTextBox.Text) == false)
				&& string.IsNullOrEmpty(this.ZoomTextBox.Text))
			{
				this.coordinateValidationError = true;
				throw new Exception("Must have a zoom value.");
			}

		    double latOut;
			double longOut;
			int zoomOut;

			if (double.TryParse(this.LatitudeTextBox.Text, out latOut) == false)
		    {
				this.coordinateValidationError = true;
				throw new Exception("Must be numeric.");
		    }

			if (double.TryParse(this.LongitudeTextBox.Text, out longOut) == false)
			{
				this.coordinateValidationError = true;
				throw new Exception("Must be numeric.");
			}

			if (int.TryParse(this.ZoomTextBox.Text, out zoomOut) == false)
			{
				this.coordinateValidationError = true;
				throw new Exception("Must be numeric.");
			}

			if (latOut < -90 || latOut > 90)
			{
				this.coordinateValidationError = true;
				throw new Exception("Must be between -90 and 90 degrees.");
			}

			if (longOut < -180 || longOut > 180)
			{
				this.coordinateValidationError = true;
				throw new Exception("Must be between -180 and 180 degrees.");
			}

			if (zoomOut < 0 || zoomOut > 25)
			{
				this.coordinateValidationError = true;
				throw new Exception("Must be between 0 and 25.");
			}
		}

		/// <summary>
		/// This method will load the asset tracking device dropdown.
		/// </summary>
		/// <param name="tank">The current selected tank.</param>
	    private void LoadTrackingDeviceDropdown(TankClass tank)
	    {
		    int selectedCount = 0;
			int selectedIndex = selectedCount;
			var items = new List<ListItem>();
			this.TrackingDeviceDropdown.Items.Clear();

			var item = new ListItem { Text = "{None}", Value = Guid.Empty.ToString() };
			items.Add(item);
			selectedCount++;

			if (tank != null)
			{
				var trackingDevices =
					FMChannelHelper.MakeCall<IAssetTrackingDevices, List<AssetTrackingDeviceClass>>(
						x => x.EnumerateAllSatelliteDevices(this.Security));

				if (trackingDevices != null && trackingDevices.Count > 0)
				{
					foreach (AssetTrackingDeviceClass device in trackingDevices)
					{
						item = new ListItem { Text = device.DeviceId, Value = device.AssetTrackingDeviceGuidStr };
						items.Add(item);

						if (device.AssetTrackingDeviceGuid == tank.AssetTrackingDeviceGuid)
						{
							selectedIndex = selectedCount;
						}

						selectedCount++;
					}
				}
			}

			this.TrackingDeviceDropdown.DataSource = items;
			this.TrackingDeviceDropdown.DataTextField = "Text";
			this.TrackingDeviceDropdown.DataValueField = "Value";
			this.TrackingDeviceDropdown.DataBind();
			this.TrackingDeviceDropdown.SelectedIndex = selectedIndex;
		}

		/// <summary>
		/// This method will handle the tank type dropdown event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void TankTypeDropdownSelectedIndexChanged(object sender, EventArgs e)
	    {
			DeviceTankTypes selectedTankType = (DeviceTankTypes)int.Parse(this.TankTypeDropdown.SelectedValue);
			var tank = (TankClass)this.Session["Tank"];

			if (selectedTankType == DeviceTankTypes.Opc)
		    {
			    this.LatitudeTextBox.Text	= string.Empty;
			    this.LongitudeTextBox.Text	= string.Empty;
			    this.ZoomTextBox.Text		= string.Empty;

			    this.LatitudeTextBox.Enabled	= false;
			    this.LongitudeTextBox.Enabled	= false;
			    this.ZoomTextBox.Enabled		= false;

			    this.LatitudeTextBox.ReadOnly	= true;
			    this.LongitudeTextBox.ReadOnly	= true;
			    this.ZoomTextBox.ReadOnly		= true;

				this.LoadTrackingDeviceDropdown(null);
			    this.TrackingDeviceDropdown.Enabled = false;

			    this.TankConfigNumberDropdown.SelectedIndex = 0;
			    this.TankConfigNumberDropdown.Enabled = false;

			    if (tank.AssetTrackingDeviceGuid != Guid.Empty)
			    {
				    this.Session.Add(SessionPreviousDeviceGuid, tank.AssetTrackingDeviceGuid);
			    }
		    }
			else
			{
				this.LatitudeTextBox.Enabled	= true;
				this.LongitudeTextBox.Enabled	= true;
				this.ZoomTextBox.Enabled		= true;

				this.LatitudeTextBox.ReadOnly	= false;
				this.LongitudeTextBox.ReadOnly	= false;
				this.ZoomTextBox.ReadOnly		= false;

				this.TrackingDeviceDropdown.Enabled		= true;

				this.TankConfigNumberDropdown.Enabled = true;

				if (tank.TankConfigurationNumber == null)
				{
					this.TankConfigNumberDropdown.SelectedIndex = 0;
				}
				else
				{
					this.TankConfigNumberDropdown.SelectedIndex = tank.TankConfigurationNumber.Value;
				}

				if (this.Session[SessionPreviousDeviceGuid] != null)
				{
					var previousDeviceGuid = (Guid)this.Session[SessionPreviousDeviceGuid];
					tank.AssetTrackingDeviceGuid = previousDeviceGuid;
					this.Session.Remove(SessionPreviousDeviceGuid);
				}

				this.LoadTrackingDeviceDropdown(tank);
			}

			this.UpdateProcessVariablesView();
		}

		/// <summary>
		/// This method will populate the Tank Type Dropdown with OPC or
		/// Satellite.
		/// </summary>
		private void PopulateTankTypeDropdown(DeviceTankTypes selectedTankType)
	    {
			var items = new List<ListItem>();

			var item = new ListItem { Text = "OPC", Value = ((int)DeviceTankTypes.Opc).ToString() };
			items.Add(item);

			item = new ListItem { Text = "Satellite", Value = ((int)DeviceTankTypes.Satellite).ToString() };
			items.Add(item);

			this.TankTypeDropdown.DataSource		= items;
			this.TankTypeDropdown.DataTextField		= "Text";
			this.TankTypeDropdown.DataValueField	= "Value";
			this.TankTypeDropdown.SelectedIndex		= 0;

            if (!(this.Security.HasRight(RIGHT.MODIFY_ASSET_TRACKING_DEVICES) || this.Security.HasRight(RIGHT.VIEW_ASSET_TRACKING_DEVICES)))
            {
                this.TankTypeDropdown.Enabled = false;
            }
            else if (selectedTankType == DeviceTankTypes.Satellite)
			{
				this.TankTypeDropdown.SelectedIndex = 1;
			}
            
			this.TankTypeDropdown.DataBind();

            

        }

		/// <summary>
		/// This method will populate the Tank Type Dropdown with OPC or
		/// Satellite.
		/// </summary>
		private void PopulateTankConfigurationNumberDropdown(int? selectedTankNumber)
		{
			var items = new List<ListItem>();

			var item = new ListItem { Text = "--Select--", Value = "0" };
			items.Add(item);

			for (int nextTankNumber = 1; nextTankNumber < 9; nextTankNumber++)
			{
				item = new ListItem { Text = nextTankNumber.ToString(), Value = nextTankNumber.ToString() };
				items.Add(item);
			}

			this.TankConfigNumberDropdown.DataSource		= items;
			this.TankConfigNumberDropdown.DataTextField		= "Text";
			this.TankConfigNumberDropdown.DataValueField	= "Value";

			if (selectedTankNumber != null)
			{
				this.TankConfigNumberDropdown.SelectedIndex	= selectedTankNumber.Value;
			}

			this.TankConfigNumberDropdown.DataBind();
		}

		/// <summary>
		/// This method handles the tank configuration number dropdown change event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void TankConfigurationNumberSelectedIndexChanged(object sender, EventArgs e)
		{
			var tankConfigNum = this.TankConfigNumberDropdown.SelectedItem.Value;
			int tankConfigurationNumber;

			if (int.TryParse(tankConfigNum, out tankConfigurationNumber) == false)
			{
				return;
			}

			var tank = (TankClass) this.Session["Tank"];

			// Force the user to select an asset tracking device first.
			// This is in order to verify if the tank config number is being used.
			if (tank.AssetTrackingDeviceGuid == Guid.Empty)
			{
				this.TankConfigNumberDropdown.SelectedIndex = 0;
				const string ErrMsg = "Must select a Tracking Device first.";
				this.ErrorHandler(new Exception(ErrMsg));
			}

			var numberBeingUsed = FMChannelHelper.MakeCall<ITanks, int>(
									x => x.TankConfigurationNumberBeingUsed(this.Security, tank.IdentityGuid, tank.AssetTrackingDeviceGuid, tankConfigurationNumber));

			if (numberBeingUsed > 0)
			{
				this.TankConfigNumberDropdown.SelectedIndex = 0;
				string errMsg = "Tank Configuration Number '" + tankConfigNum + "' is already being used by another Tank.";
				this.ErrorHandler(new Exception(errMsg));
			}
		}

		protected void ProductTypeDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				var productType = (ProductType)Convert.ToInt32(this.ProductTypeDropDownList.SelectedItem.Value, CultureInfo.InvariantCulture);
				this.Session["ProductType"] = productType;
				var tank = (TankClass)this.Session["Tank"];

				this.ProductsDropDownList.Items.Clear();				               

                ProductCollectionClass productCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
																	 x =>
																	 x.EnumerateByType(this.Security, productType)
																);

				foreach (ProductClass product in productCollection)
				{
					var newProductItem = new ListItem(product.ID, product.MasterRecordGuid.ToString());

					foreach (ListItem existingProductItem in this.ProductsDropDownList.Items)
					{
						if (string.Compare(existingProductItem.Text, newProductItem.Text, StringComparison.Ordinal) > 0)
						{
							int index = this.ProductsDropDownList.Items.IndexOf(existingProductItem);
							this.ProductsDropDownList.Items.Insert(index, newProductItem);

							if (tank.ProductGuid == product.MasterRecordGuid)
							{
								this.ProductsDropDownList.SelectedIndex = index;
							}

							newProductItem = null;
							break;
						}
					}

					if (newProductItem != null)
					{
						this.ProductsDropDownList.Items.Add(newProductItem);

						if (tank.ProductGuid == product.MasterRecordGuid)
						{
							this.ProductsDropDownList.SelectedIndex = this.ProductsDropDownList.Items.Count - 1;
						}
					}
				}

                var item = new ListItem(this.GetTranslatedText("{None}"), Guid.Empty.ToString());
                this.ProductsDropDownList.Items.Insert(0, item);
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
			this.ProcessVariablesDataGrid.EditCommand += this.ProcessVariablesDataGridEditCommand;
			this.ProcessVariablesDataGrid.PageIndexChanged += this.ProcessVariablesDataGridPageIndexChanged;
            this.ProcessVariablesDataGrid.ItemDataBound += this.ProcessVariablesDataGridItemDataBound;
		}

		private void ProcessVariablesDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			this.Session["UnitForm"] = "TankForm.aspx";
			var tank = (TankClass)this.Session["Tank"];
			this.Session["ProcessVariable"] = tank.ProcessVariableCollection[e.Item.DataSetIndex];
			this.Redirect("OPCConnectionForm.aspx");
		}

		private void ProcessVariablesDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.ProcessVariablesDataGrid.EditItemIndex > -1)
				{
					return;
				}

				this.ProcessVariablesDataGrid.CurrentPageIndex = e.NewPageIndex;
				this.UpdateProcessVariablesView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.Session.Remove("Tank");
			}
		}

		private void ProcessVariablesDataGridItemDataBound(object source, DataGridItemEventArgs e)
		{
			var editButton = (LinkButton)e.Item.FindControl("FMEditLinkButton1");

			if(editButton != null)
			{
				var selectedTankType = (DeviceTankTypes)int.Parse(this.TankTypeDropdown.SelectedValue);

				editButton.Enabled = true;

				if (selectedTankType == DeviceTankTypes.Satellite)
				if (selectedTankType == DeviceTankTypes.Satellite)
				{
					editButton.Enabled = false;
				}
			}
		}

		[SecurityCritical]
		private ICollection ProcessVariablesView()
		{
			var pvDataTable = new DataTable();

			pvDataTable.Columns.Add("Index", typeof(Int32));
			pvDataTable.Columns.Add("TypeID", typeof(string));
			pvDataTable.Columns.Add("EngineeringUnits", typeof(string));
			pvDataTable.Columns.Add("Maximum", typeof(string));
			pvDataTable.Columns.Add("Minimum", typeof(string));
			pvDataTable.Columns.Add("Host", typeof(string));
			pvDataTable.Columns.Add("OPCServerID", typeof(string));
			pvDataTable.Columns.Add("OPCItemID", typeof(string));

			var tank = (TankClass)this.Session["Tank"];
			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(
																			this.Security,
																			this.Security.SiteGuid,
																			getMemberSites: true,
																			getSchedulesAndProcessVariables: true,
																			bGetAssociatedAliases: true)
																	);
			if (tank.ProcessVariableCollection != null)
			{
				var selectedTankType = (DeviceTankTypes)int.Parse(this.TankTypeDropdown.SelectedValue);

				foreach (ProcessVariableClass processVariable in tank.ProcessVariableCollection)
				{
					// If the tank type is set to satellite, then we only want the 
					// following process variables.
					if (selectedTankType == DeviceTankTypes.Satellite)
					{
						if (processVariable.ProcessVariableType != PROCESS_VARIABLE_TYPE.LEVEL_PV &&
							processVariable.ProcessVariableType != PROCESS_VARIABLE_TYPE.TEMPERATURE_PV &&
							processVariable.ProcessVariableType != PROCESS_VARIABLE_TYPE.GROSS_VOLUME_PV &&
							processVariable.ProcessVariableType != PROCESS_VARIABLE_TYPE.NET_VOLUME_PV &&
							processVariable.ProcessVariableType != PROCESS_VARIABLE_TYPE.DENSITY_PV &&
							processVariable.ProcessVariableType != PROCESS_VARIABLE_TYPE.VAPOR_PRESSURE_PV)
						{
							continue;
						}
					}

				    var pv = this.Session["ProcessVariable"] as ProcessVariableClass;
				    if (pv != null
					&& pv.ProcessVariableType == processVariable.ProcessVariableType
					&& pv.InstanceNumber == processVariable.InstanceNumber)
					{
						var editedProcessVariable = pv;
						processVariable.Load(editedProcessVariable);
						this.Session.Remove("ProcessVariable");
					}

					var pvDataRow = pvDataTable.NewRow();

					pvDataRow["Index"] = processVariable.ProcessVariableType;
					pvDataRow["TypeID"] =
						this.GetTranslatedText(ProcessVariableClass.ProcessVariableTypeID(processVariable.ProcessVariableType));

					EngineeringUnit units = site.GetSiteUnits(processVariable.SiteVariableType);
					byte decimalPlaces = site.GetSiteDecimalPlaces(processVariable.SiteVariableType);

					if (processVariable.UnitsEnabled)
					{
						string abbrevString = EngineeringUnits.GetUnitAbbreviation(units);
						pvDataRow["EngineeringUnits"] = abbrevString;
					}

					pvDataRow["Maximum"] = processVariable.Encode(
						processVariable.GetMaximum(units, decimalPlaces),
						Quality.Good,
						units,
						site.GetNumberFormatInfo(processVariable.SiteVariableType));
					pvDataRow["Minimum"] = processVariable.Encode(
						processVariable.GetMinimum(units, decimalPlaces),
						Quality.Good,
						units,
						site.GetNumberFormatInfo(processVariable.SiteVariableType));
					var url = new URL(processVariable.URL);
					pvDataRow["Host"] = url.HostName;
					pvDataRow["OPCServerID"] = processVariable.ProgID;
					pvDataRow["OPCItemID"] = processVariable.OPCItemID;
					pvDataTable.Rows.Add(pvDataRow);
				}
			}

			var pvDataView = new DataView(pvDataTable);
			return pvDataView;
		}

		private void UpdateProcessVariablesView()
		{
			this.ProcessVariablesDataGrid.DataSource = this.ProcessVariablesView();
			this.ProcessVariablesDataGrid.DataBind();
		}
		#endregion
	}
}