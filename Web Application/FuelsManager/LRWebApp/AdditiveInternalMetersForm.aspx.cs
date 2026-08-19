// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdditiveInternalMetersForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LoadRackWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.UtilityObjects;

	using FMWebApp;

	using FuelsManager.FMWebApp;

	public partial class AdditiveInternalMetersForm : FMFormBase, IMenuDiscovery
	{
		#region Constants and Fields

		protected int AdditiveIndex = 0;

		protected int ArmIndex = 0;

		protected StationCollectionClass InternalMeterStationCollection;

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
		/// <param name="options">
		/// Hardware key options
		/// </param>
		/// <returns>
		/// List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
            if (useNewLicenseKey == 1)
            {
                if ((word2 & 0x01) != 0x01)
				{
					return null;
				}
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
						MenuItemType = FMMenuItemType.OPERATIONS_LOAD_RACK_ADDITIVE_INTERNAL_METERS, 
						RootMenuName = "Operations", 
						CategoryName = "Load Rack", 
						ItemName = "Additive Internal Meters", 
						NavigateUrl = "..\\LRWebApp\\AdditiveInternalMetersForm.aspx", 
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});

			return items;
		}

		#endregion

		#region Methods

		protected override void OnInit(EventArgs e)
		{
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
					this.PopulateStationFilterDropDown();
					this.UpdateView();

					// Enable/Disable buttons on page load
					if (this.Security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
					{
						this.EditButton.Enabled = true;
						this.ApplyButton.Enabled = false;
						this.CancelButton.Enabled = false;
					}
					else
					{
						this.EditButton.Enabled = false;
						this.ApplyButton.Enabled = false;
						this.CancelButton.Enabled = false;
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void ApplyButton_Click(object sender, EventArgs e)
		{
			this.EditButton.Enabled = true;
			this.ApplyButton.Enabled = false;
			this.CancelButton.Enabled = false;
			this.ApplyChanges();
			this.UpdateView();
		}

		private void ApplyChanges()
		{
			try
			{
				var Station = new StationClass();

				foreach (DataGridItem item in this.InternalMetersDataGrid.Items)
				{
					var stationGuidLabel = (Label)item.FindControl("StationGuidLabel");
					var armlabel = (Label)item.FindControl("ArmLabel");
					var armGuidLabel = (Label)item.FindControl("ArmGuidLabel");
					var additivelabel = (Label)item.FindControl("AdditiveLabel");
					var componentindexlabel = (Label)item.FindControl("ComponentIndexLabel");
					var productGuidLabel = (Label)item.FindControl("ProductGuidLabel");
					var metervaluetextbox = (TextBox)item.FindControl("MeterValueTextBox");

					if ((null != stationGuidLabel) && (null != armlabel) && (null != armGuidLabel) && (null != additivelabel)
					    && (null != componentindexlabel) && (null != productGuidLabel) && (null != metervaluetextbox))
					{
						// Get the current station
						if (Station.IdentityGuid != Guid.Parse(stationGuidLabel.Text))
						{
							Station =
								FMChannelHelper.MakeCall<IStations, StationClass>(
									stations => stations.Get(this.Security, Guid.Parse(stationGuidLabel.Text)));
						}

						// Don't apply changes unless the station exists.
						if (Station.IdentityGuid.IsEmpty())
						{
							continue;
						}

						// Apply changes through Load Rack Service when Station is Enabled
						if (Station.Enabled)
						{
							try
							{
								ILoadRackManager loadRackManager = this.GetLoadRackManager();
								loadRackManager.SetAdditiveMeterTotalizer(
									this.Security, 
									Guid.Parse(stationGuidLabel.Text), 
									Guid.Parse(armGuidLabel.Text), 
									Guid.Parse(productGuidLabel.Text), 
									Convert.ToDouble(metervaluetextbox.Text));
							}
							catch (Exception except)
							{
								if (!except.Message.Contains("No connection could be made because the target machine actively refused it"))
								{
									throw;
								}

								ProcessVariableClass internalPv =
									Station.LoadArmCollection[Convert.ToInt32(armlabel.Text) - 1].AdditiveInjectorCollection[
										Convert.ToInt32(componentindexlabel.Text)].ProcessVariableCollection[
											PROCESS_VARIABLE_TYPE.ADDITIVE_METER_FLOW_TOTAL_PV];
								if (internalPv == null)
								{
									throw new Exception("LoadRack|Additive Injector Not Internal");
								}

								internalPv.ServerValue = Convert.ToDouble(metervaluetextbox.Text);
								internalPv.DateTimeStamp = DateTimeOffset.Now;

								FMChannelHelper.MakeCall<IProcessVariables>(x => x.Modify(this.Security, DATA_TYPE.DYNAMIC, internalPv));
							}
						}
						else
						{
							ProcessVariableClass internalPv =
								Station.LoadArmCollection[Convert.ToInt32(armlabel.Text) - 1].AdditiveInjectorCollection[
									Convert.ToInt32(componentindexlabel.Text)].ProcessVariableCollection[
										PROCESS_VARIABLE_TYPE.ADDITIVE_METER_FLOW_TOTAL_PV];
							if (internalPv == null)
							{
								throw new Exception("LoadRack|Additive Injector Not Internal");
							}

							internalPv.ServerValue = Convert.ToDouble(metervaluetextbox.Text);
							internalPv.DateTimeStamp = DateTimeOffset.Now;

							FMChannelHelper.MakeCall<IProcessVariables>(x => x.Modify(this.Security, DATA_TYPE.DYNAMIC, internalPv));
						}

						// Set the alarm and event
						StationClass station = Station;
						var stationValue = station.SetAdditiveMeterTotalizerEvent(
							Convert.ToInt32(armlabel.Text), additivelabel.Text, Convert.ToDouble(metervaluetextbox.Text));

						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, stationValue));
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void CancelButton_Click(object sender, EventArgs e)
		{
			this.EditButton.Enabled = true;
			this.ApplyButton.Enabled = false;
			this.CancelButton.Enabled = false;
			this.UpdateView();
		}

		private void EditButton_Click(object sender, EventArgs e)
		{
			this.EditButton.Enabled = false;
			this.ApplyButton.Enabled = true;
			this.CancelButton.Enabled = true;
			this.UpdateView();
		}

		private ICollection EnumerateInternalMeters()
		{
			this.InternalMeterStationCollection = (StationCollectionClass)this.Session["InternalMeterStationCollection"];

			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(
																				this.Security, 
																				this.Security.SiteGuid, 
																				getMemberSites: true, 
																				getSchedulesAndProcessVariables: true, 
																				bGetAssociatedAliases: true)
						);


			NumberFormatInfo AdditiveFormatInfo = site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.ADDITIVE_VOLUME);

			var InternalMeterDataTable = new DataTable();
			DataRow InternalMeterDataRow;

			InternalMeterDataTable.Columns.Add("StationID", typeof(string));
			InternalMeterDataTable.Columns.Add("StationGuid", typeof(string));
			InternalMeterDataTable.Columns.Add("Arm", typeof(string));
			InternalMeterDataTable.Columns.Add("ArmGuid", typeof(string));
			InternalMeterDataTable.Columns.Add("ComponentIndex", typeof(string));
			InternalMeterDataTable.Columns.Add("Meter", typeof(string));
			InternalMeterDataTable.Columns.Add("ProductID", typeof(string));
			InternalMeterDataTable.Columns.Add("ProductGuid", typeof(string));
			InternalMeterDataTable.Columns.Add("MeterValue", typeof(string));

			// Get the selected station guid, Guids.AllFilterGuid equals {All} stations
			ListItem li = this.StationFilterDropDown.SelectedItem;
			if (Guids.AllFilterGuid.ToString() == li.Value)
			{
				FMChannelHelper.MakeCall<IStations>(
					stations =>
						{
							foreach (StationClass Station in this.InternalMeterStationCollection)
							{
								Station.Load(stations.Get(this.Security, Station.IdentityGuid));

								// Don't add the station unless the station exists.
								if (!Station.IdentityGuid.IsEmpty())
								{
									this.ArmIndex = 0;
									foreach (LoadArmClass LoadArm in Station.LoadArmCollection)
									{
										this.AdditiveIndex = 0;
										foreach (ProductMapClass Additive in LoadArm.AdditiveInjectorCollection)
										{
											// only use station with arm that internal additive meter is set
											ProcessVariableClass InternalPV =
												Additive.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.ADDITIVE_METER_FLOW_TOTAL_PV];
											if (InternalPV != null)
											{
												InternalMeterDataRow = InternalMeterDataTable.NewRow();

												InternalMeterDataRow["StationID"] = Station.ID;
												InternalMeterDataRow["StationGuid"] = Station.IdentityGuid.ToString();
												InternalMeterDataRow["Arm"] = (this.ArmIndex + 1).ToString();
												InternalMeterDataRow["ArmGuid"] = LoadArm.IdentityGuid.ToString();
												InternalMeterDataRow["ComponentIndex"] = this.AdditiveIndex.ToString();
												InternalMeterDataRow["Meter"] = (Additive.Meter != null) ? Additive.Meter.ID : string.Empty;
												InternalMeterDataRow["ProductID"] = Additive.AssignedID;
												InternalMeterDataRow["ProductGuid"] = Additive.AssignedGuid.ToString();

												// convert process varible to double
												double MeterValue = Convert.ToDouble(InternalPV.ServerValue);
												InternalMeterDataRow["MeterValue"] = MeterValue.ToString("N", AdditiveFormatInfo);

												InternalMeterDataTable.Rows.Add(InternalMeterDataRow);
											}

											this.AdditiveIndex++;
										}

										this.ArmIndex++;
									}
								}
							}
						});
			}
			else
			{
				// retrieve the selected station from the filtered selection
				StationClass Station =
					FMChannelHelper.MakeCall<IStations, StationClass>(stations => stations.Get(this.Security, Guid.Parse(li.Value)));

				// Don't add the station unless the station exists.
				if (!Station.IdentityGuid.IsEmpty())
				{
					this.ArmIndex = 0;
					foreach (LoadArmClass LoadArm in Station.LoadArmCollection)
					{
						this.AdditiveIndex = 0;
						foreach (ProductMapClass Additive in LoadArm.AdditiveInjectorCollection)
						{
							// only use station with arm that internal additive meter is set
							ProcessVariableClass InternalPV =
								Additive.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.ADDITIVE_METER_FLOW_TOTAL_PV];
							if (InternalPV != null)
							{
								InternalMeterDataRow = InternalMeterDataTable.NewRow();

								InternalMeterDataRow["StationID"] = Station.ID;
								InternalMeterDataRow["StationGuid"] = Station.IdentityGuid.ToString();
								InternalMeterDataRow["Arm"] = (this.ArmIndex + 1).ToString();
								InternalMeterDataRow["ArmGuid"] = LoadArm.IdentityGuid.ToString();
								InternalMeterDataRow["ComponentIndex"] = this.AdditiveIndex.ToString();
								InternalMeterDataRow["Meter"] = (Additive.Meter != null) ? Additive.Meter.ID : string.Empty;
								InternalMeterDataRow["ProductID"] = Additive.AssignedID;
								InternalMeterDataRow["ProductGuid"] = Additive.AssignedGuid.ToString();

								// convert process varible to double
								double MeterValue = Convert.ToDouble(InternalPV.ServerValue);
								InternalMeterDataRow["MeterValue"] = MeterValue.ToString("N", AdditiveFormatInfo);

								InternalMeterDataTable.Rows.Add(InternalMeterDataRow);
							}

							this.AdditiveIndex++;
						}

						this.ArmIndex++;
					}
				}
			}

			var InternalMeterDataView = new DataView(InternalMeterDataTable)
			{
				// Add sorting by Station, Arm, Meter (IGO 20-Jan-2009)
				Sort = "StationID asc, Arm asc, Meter asc"
			};

			return InternalMeterDataView;
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.InternalMetersFormPageSizeDropDown.SelectedIndexChanged +=
				new EventHandler(this.PageSizeDropDown_SelectedIndexChanged);
			this.StationFilterDropDown.SelectedIndexChanged +=
				new EventHandler(this.StationFilterDropDown_SelectedIndexChanged);
			this.InternalMetersDataGrid.ItemDataBound +=
				new DataGridItemEventHandler(this.InternalMetersDataGrid_ItemDataBound);
			this.EditButton.Click += new EventHandler(this.EditButton_Click);
			this.ApplyButton.Click += new EventHandler(this.ApplyButton_Click);
			this.CancelButton.Click += new EventHandler(this.CancelButton_Click);
		}

		private void InternalMetersDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var metervaluelabel = (Label)e.Item.FindControl("MeterValueLabel");
			var metervaluetextbox = (TextBox)e.Item.FindControl("MeterValueTextBox");

			// show text box control when in edit mode
			if ((false == this.EditButton.Enabled) && this.ApplyButton.Enabled && this.CancelButton.Enabled)
			{
				if (null != metervaluelabel)
				{
					metervaluelabel.Visible = false;
				}

				if (null != metervaluetextbox)
				{
					metervaluetextbox.Visible = true;
				}
			}
			else
			{
				if (null != metervaluelabel)
				{
					metervaluelabel.Visible = true;
				}

				if (null != metervaluetextbox)
				{
					metervaluetextbox.Visible = false;
				}
			}
		}

		private void PageSizeDropDown_SelectedIndexChanged(object source, EventArgs e)
		{
			this.UpdateView();
		}

		private void PopulateStationFilterDropDown()
		{
			this.InternalMeterStationCollection = new StationCollectionClass();

			this.StationFilterDropDown.Items.Clear();
			this.StationFilterDropDown.Items.Add(new ListItem("{All}", Guids.AllFilterGuid.ToString()));

			// Enumerate stations with internal meters
			FMChannelHelper.MakeCall<IStations>(
				stations =>
					{
						StationCollectionClass loadRackStationCollection = stations.EnumerateByType(this.Security, STATION_TYPE.LOAD_RACK);

						foreach (StationClass station in loadRackStationCollection)
						{
							bool bInternalMeterFound = false;

							station.Load(stations.Get(this.Security, station.IdentityGuid));

							// Don't add the station unless the station exists.
							if (!station.IdentityGuid.IsEmpty())
							{
								foreach (LoadArmClass loadArm in station.LoadArmCollection)
								{
									foreach (ProductMapClass additive in loadArm.AdditiveInjectorCollection)
									{
										// only use station with arm that internal additive meter is set
										ProcessVariableClass internalPv =
											additive.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.ADDITIVE_METER_FLOW_TOTAL_PV];
										if (internalPv != null)
										{
											bInternalMeterFound = true;
											this.InternalMeterStationCollection.Add(station);
											break;
										}
									}

									// don't search any other arm if internal meter found
									if (bInternalMeterFound)
									{
										break;
									}
								}
							}
						}
					});

			// Add internal meter station collection to session
			this.Session.Add("InternalMeterStationCollection", this.InternalMeterStationCollection);

			// Add internal meter station to drop down list
			foreach (StationClass station in this.InternalMeterStationCollection)
			{
				this.StationFilterDropDown.Items.Add(new ListItem(station.ID, station.IdentityGuid.ToString()));
			}

			// Set the current selection to ALL
			this.StationFilterDropDown.SelectedIndex = 0;
		}

		private void StationFilterDropDown_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.UpdateView();
		}

		private void UpdateView()
		{
			ICollection internalMeters = this.EnumerateInternalMeters();

			this.InternalMetersFormPageSizeDropDown.SetPageSize(this.InternalMetersDataGrid, internalMeters.Count);

			this.InternalMetersDataGrid.DataSource = internalMeters;
			this.InternalMetersDataGrid.DataBind();
		}

		#endregion
	}
}