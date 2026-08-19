namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Globalization;
	using System.Text;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.Areas.AssetTrackingArea.ViewModels;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	public partial class EquipmentHistoryTab : EquipmentPageBase
	{
		private List<AssetTrackingDeviceClass> assetTrackingDeviceCollection;

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
                if (!this.Security.HasRight(RIGHT.VIEW_ASSET_TRACKING_DEVICES) && !this.Security.HasRight(RIGHT.MODIFY_ASSET_TRACKING_DEVICES))
                {
                    return;
                }

                if (this.assetTrackingDeviceCollection == null || this.assetTrackingDeviceCollection.Count == 0)
				{
					this.assetTrackingDeviceCollection = FMChannelHelper.MakeCall<IAssetTrackingDevices, List<AssetTrackingDeviceClass>>(
																x => x.EnumerateAllDevicesLinkedToEquipment(this.Security));
				}

				if (this.Page.IsPostBack == false)
				{
					this.PopulatePeriodDropdown();
					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method will handle the equipment history Refresh button on click
		/// event.  It will update the view with the new data.
		/// </summary>
		/// <param name="sender">The sending object.</param>
		/// <param name="e">The event arguments.</param>
		protected void EquipmentHistoryRefreshOnClick(object sender, EventArgs e)
		{
			this.UpdateView();
		}

		/// <summary>
		/// This method will handle the equipment history grid paging size.
		/// </summary>
		/// <param name="source">The sending object.</param>
		/// <param name="e">The event arguments.</param>
		protected void PageSizeDropDownSelectedIndexChanged(object source, EventArgs e)
		{
			this.UpdateView();
		}

		protected void EquipmentHistoryGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				this.EquipmentHistoryGrid.CurrentPageIndex = e.NewPageIndex;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method initializes the page on the OnInit event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e)
		{
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //           
            this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// This method handles the item data bound event. It will set the row color
		/// to red if the item Contamination is set to true.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected void EquipmentHistoryGridItemDataBound(object source, DataGridItemEventArgs e)
		{
			var messageStateLabel = (Label)e.Item.FindControl("MessageStateLabel");

			if (messageStateLabel != null)
			{
				int messageStateInt;

				if (int.TryParse(messageStateLabel.Text, out messageStateInt) == false)
				{
					return;
				}

				var messageState = (AssetTrackingDetailClass.MessageStates) messageStateInt;

				if (messageState == AssetTrackingDetailClass.MessageStates.None)
				{
					return;
				}

				if (messageState == AssetTrackingDetailClass.MessageStates.Contaminated)
				{
					e.Item.BackColor = Color.Orange;
					this.SetLabelColor(e, Color.Black);
				}

				if (messageState == AssetTrackingDetailClass.MessageStates.Investigate)
				{
					e.Item.BackColor = Color.Yellow;
					this.SetLabelColor(e, Color.Black);
				}

				if (messageState == AssetTrackingDetailClass.MessageStates.InvestigateCompletedFailed)
				{
					e.Item.BackColor = Color.Red;
					this.SetLabelColor(e, Color.White);
				}

				if (messageState == AssetTrackingDetailClass.MessageStates.InvestigateCompletedPassed)
				{
					e.Item.BackColor = Color.ForestGreen;
					this.SetLabelColor(e, Color.White);
				}
			}
		}

		#region Private Methods
		/// <summary>
		/// This method will set the label color on the rows.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="labelColor"></param>
		private void SetLabelColor(DataGridItemEventArgs e, Color labelColor)
		{
					var deviceIdLabel	= (Label) e.Item.FindControl("DeviceIdLabel");
					var gpsLabel		= (Label) e.Item.FindControl("GpsLabel");
					var timestampLabel	= (Label) e.Item.FindControl("TimestampLabel");
					var productLabel	= (Label) e.Item.FindControl("ProductLabel");
					var volumLabel		= (Label) e.Item.FindControl("VolumLabel");
					var waterLabel		= (Label) e.Item.FindControl("WaterLabel");
					var densityLabel	= (Label) e.Item.FindControl("DensityLabel");
					var dielectricLabel = (Label) e.Item.FindControl("DielectricLabel");
					var remarksLabel	= (Label) e.Item.FindControl("RemarksLabel");
					var expandLabel		= (Label) e.Item.FindControl("ExpandLabel");

			if (deviceIdLabel != null)		deviceIdLabel.ForeColor = labelColor;
			if (gpsLabel != null)			gpsLabel.ForeColor = labelColor;
			if (timestampLabel != null)		timestampLabel.ForeColor = labelColor;
			if (productLabel != null)		productLabel.ForeColor = labelColor;
			if (volumLabel != null)			volumLabel.ForeColor = labelColor;
			if (waterLabel != null)			waterLabel.ForeColor = labelColor;
			if (densityLabel != null)		densityLabel.ForeColor = labelColor;
			if (dielectricLabel != null)	dielectricLabel.ForeColor = labelColor;
			if (remarksLabel != null)		remarksLabel.ForeColor = labelColor;
			if (expandLabel != null)		expandLabel.ForeColor = labelColor;
				}

		/// <summary>
		/// This method will update the equipment history data in the grid based on the filtering information.
		/// </summary>
		private void UpdateView()
		{
			var equipmentHistoryModel = new AssetEquipmentHistoryModel();

			if (this.VerifyDates() == false)
			{
				return;
			}

			var equipList = this.Session["EquipmentArrayList"] as ArrayList;
			EquipmentClass equipment = null;

			if (equipList != null && equipList.Count > 0)
			{
			    var equipmentTuple = equipList[0] as Tuple<EquipmentClass, bool>;
			    if (equipmentTuple != null)
			    {
			        equipment = equipmentTuple.Item1;
			    }
			    else
			    {
                    equipment = (EquipmentClass)equipList[0];
                }
            }

			// Check to see if there is an asset tracking device associated to the equipment.
			// If not, then return an empty list.
			if (equipment == null || equipment.IdentityGuid == Guid.Empty || equipment.AssetTrackingDeviceGuid == Guid.Empty)
			{
				this.EquipmentHistoryPageSizeDropDown.SetPageSize(this.EquipmentHistoryGrid, 0);

				// Bind the data to the grid.
				this.EquipmentHistoryGrid.DataSource = new List<AssetEquipmentHistoryRecordModel>();
				this.EquipmentHistoryGrid.DataBind();
				return;
			}

			try
			{
				var localSite = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

				string fromDateStr		= this.EquipHistoryStartDateHidden.Text;
				string endDateStr		= this.EquipHistoryEndDateHidden.Text;
				string breadcrumbState	= this.GetBreadcrumbSelection();

				List<AssetTrackingDetailClass> assetTrackingDetailList = this.GetOneDetailEquipment(equipment.IdentityGuid.ToString(), breadcrumbState, fromDateStr, endDateStr);

				if (assetTrackingDetailList != null && assetTrackingDetailList.Count > 0)
				{
					int siteVolumeUnitIndex = (int)localSite.VolumeUnits;
					int siteDensityUnitIndex = (int)localSite.DensityUnits;

					foreach (AssetTrackingDetailClass detailRecord in assetTrackingDetailList)
					{
						if (detailRecord.Latitude != null && detailRecord.Longitude != null)
						{
							string productId			= string.Empty;
							string equipmentId			= string.Empty;
							string productDensityStr	= string.Empty;

							EngineeringUnit volumeUnitIndex	= localSite.VolumeUnits;
							int siteVolumeDecimalPlaces			= int.Parse(localSite.VolumeDecimalPlaces);
							int siteDensityDecimalPlaces		= int.Parse(localSite.DensityDecimalPlaces);

							AssetTrackingDeviceClass oneDevice = this.assetTrackingDeviceCollection.Find(x => x.DeviceId == detailRecord.AssetTrackingDeviceId);

							if (oneDevice != null)
							{
								if (oneDevice.ProductDensity != null)
								{
									EngineeringUnit densityUnitIndex = this.GetAppropriateUnitIndex(siteDensityUnitIndex, oneDevice.EquipmentDensityUnitIndex, oneDevice.ProductDensityUnitIndex);

									double productDensity = EngineeringUnits.Convert(oneDevice.ProductDensity.Value, EngineeringUnit.FmdKgM3, densityUnitIndex, 15);
									productDensity = Math.Round(productDensity, siteDensityDecimalPlaces, MidpointRounding.AwayFromZero);
									productDensityStr = productDensity.ToString(CultureInfo.InvariantCulture);
								}

								equipmentId = oneDevice.EquipmentId;
								productId	= oneDevice.ProductId;

								volumeUnitIndex	= this.GetAppropriateUnitIndex(siteVolumeUnitIndex, oneDevice.EquipmentVolumeUnitIndex, oneDevice.ProductVolumeUnitIndex);
							}

							equipmentHistoryModel.EquipmentID = equipmentId;
							string deviceId = detailRecord.AssetTrackingDeviceId;

							// Add radio active symbol to the Device ID to indicate contamination.
							if (detailRecord.Contaminated)
							{
								deviceId = deviceId + " " + this.GetRadioActiveSymbol();
							}

							var historyRecordModel = new AssetEquipmentHistoryRecordModel
												{
													AssetTrackingDetailGuidStr	= detailRecord.AssetTrackingDetailGuid.ToString(),
													ProductId					= productId,
													AssetTrackingDeviceId		= deviceId,
													GpsCoordinatesStr			= Math.Round(detailRecord.Latitude.Value, 4, MidpointRounding.AwayFromZero) + ", " + Math.Round(detailRecord.Longitude.Value, 4, MidpointRounding.AwayFromZero),
													SessionDatetimeStr			= this.FormatSessionDateTime(detailRecord.AssetSessionDateTime, localSite),
													VolumeStr					= "",
													WaterStr					= "NO",
													DensityStr					= productDensityStr,
													IsCompartment				= false,
													IsContaminated				= detailRecord.Contaminated,
													Remarks						= detailRecord.Remarks,
													MessageState				= (int) detailRecord.MessageState
							};

							equipmentHistoryModel.EquipmentHistoryRecordList.Add(historyRecordModel);

							if (detailRecord.TrackingTanks != null)
							{
								int tankCount = 1;
								double totalVolume = 0.0;
								int totalCompartments = detailRecord.TrackingTanks.Count;
								string saveDielectricStr = string.Empty;

								foreach (AssetTrackingTankClass wrdcuTank in detailRecord.TrackingTanks)
								{
									double volume = 0;
									string waterContent = "NO";
									string dielectricStr = string.Empty;

									if (wrdcuTank.Dielectric != null)
									{
										double dielectricRounded = Math.Round(wrdcuTank.Dielectric.Value, 4, MidpointRounding.AwayFromZero);
										dielectricStr = dielectricRounded.ToString(CultureInfo.InvariantCulture);

										// Only consider values to the 1000th place to determine zero.
										int dielectricInt = (int)wrdcuTank.Dielectric.Value * 1000;

										if (dielectricInt == 0)
										{
											waterContent = "YES";
											historyRecordModel.WaterStr = waterContent;
										}

										// Save off the dielectric in case there is only one tank.
										if (tankCount == 1)
										{
											saveDielectricStr = dielectricStr;
										}
									}

									if (wrdcuTank.Volume != null)
									{
										volume = EngineeringUnits.Convert(wrdcuTank.Volume.Value, EngineeringUnit.FmvMeter3, volumeUnitIndex, 15);
									}

									totalVolume = totalVolume + volume;
									volume = Math.Round(volume, siteVolumeDecimalPlaces, MidpointRounding.AwayFromZero);

									string compartmentName = tankCount.ToString();
									if (wrdcuTank.Contaminated)
									{
										compartmentName = compartmentName + " " + this.GetRadioActiveSymbol();
									}

									var historyCompartmentRecordModel = new AssetEquipmentHistoryRecordModel
																	{
																		ProductId				= string.Empty,
																		AssetTrackingDeviceId	= string.Empty,
																		GpsCoordinatesStr		= string.Empty,
																		SessionDatetimeStr		= string.Empty,
																		VolumeStr				= volume.ToString(CultureInfo.InvariantCulture),
																		WaterStr				= waterContent,
																		DensityStr				= string.Empty,
																		IsCompartment			= true,
																		CompartmentName			= compartmentName,
																		IsContaminated			= wrdcuTank.Contaminated,
																		DielectricStr			= dielectricStr,
																		MessageState			= (int) detailRecord.MessageState
									};

									if (totalCompartments > 1 && this.CompartmentCheckbox.Checked)
									{
										equipmentHistoryModel.EquipmentHistoryRecordList.Add(historyCompartmentRecordModel);
									}

									tankCount++;
								}

								totalVolume = Math.Round(totalVolume, siteVolumeDecimalPlaces, MidpointRounding.AwayFromZero);
								historyRecordModel.VolumeStr = totalVolume.ToString(CultureInfo.InvariantCulture);

								// A count of two means there was only one tank. In this case, we want to
								// display the dielectric value on main row since the compartment row will
								// not be displayed.
								if (tankCount == 2)
								{
									historyRecordModel.DielectricStr = saveDielectricStr;
								}
							}
						}
				}
				}

				this.EquipmentHistoryPageSizeDropDown.SetPageSize(this.EquipmentHistoryGrid, equipmentHistoryModel.EquipmentHistoryRecordList.Count);

				// Bind the data to the grid.
				this.EquipmentHistoryGrid.DataSource = equipmentHistoryModel.EquipmentHistoryRecordList;
				this.EquipmentHistoryGrid.DataBind();
			}
			catch (Exception)
			{
				const string ErrMsg = "Error retrieving equipment history data.";
				this.ErrorHandler(new Exception(ErrMsg));
			}
		}

		/// <summary>
		/// This method will return a Radio Active Symbol as a string.
		/// </summary>
		/// <returns>Returns a radio active symbol in string format.</returns>
		private string GetRadioActiveSymbol()
		{
			// Radio Active unicode hex value: 0x2622.
			byte[] array = { 0x22, 0x26 };
			var enc = Encoding.Unicode;
			var radioActiveChar = enc.GetChars(array);

			return new string(radioActiveChar);
		}

		/// <summary>
		/// This method will return the appropriate units based on the unit index passed into
		/// the methods. The heirarchy is to return the unit index from Equipement if
		/// present, then product, then site.
		/// </summary>
		/// <param name="siteUnit">The site unit index.</param>
		/// <param name="equipmentUnit">The equipment unit index.</param>
		/// <param name="productUnit">The product unit index.</param>
		/// <returns>Returns the appropriate unit abbreviation.</returns>
		private EngineeringUnit GetAppropriateUnitIndex(int siteUnit, int? equipmentUnit, int? productUnit)
		{
			EngineeringUnit unitIndex = (EngineeringUnit)siteUnit;

			if (productUnit != null)
			{
				unitIndex = (EngineeringUnit)productUnit;
			}

			if (equipmentUnit != null)
			{
				unitIndex = (EngineeringUnit)equipmentUnit;
			}

			return unitIndex;
		}

		/// <summary>
		/// This method will convert the date time to a string format
		/// of yyyy/mm/dd hh:mm:ss.
		/// </summary>
		/// <param name="sessionDateTime">The Iridium session date time.</param>
		/// <param name="site">The current site.</param>
		/// <returns>Returns a string representing the date time.</returns>
		private string FormatSessionDateTime(DateTime? sessionDateTime, SiteClass site)
		{
			if (sessionDateTime == null)
			{
				return string.Empty;
			}

			string dateSeparator = site == null ? "/" : site.DateSeparator;
			string timeSeparator = site == null ? ":" : site.TimeSeparator;

			string monthStr		= sessionDateTime.Value.Month < 10 ? "0" + sessionDateTime.Value.Month : sessionDateTime.Value.Month.ToString();
			string dayStr		= sessionDateTime.Value.Day < 10 ? "0" + sessionDateTime.Value.Day : sessionDateTime.Value.Day.ToString();
			string hourStr		= sessionDateTime.Value.Hour < 10 ? "0" + sessionDateTime.Value.Hour : sessionDateTime.Value.Hour.ToString();
			string minuteStr	= sessionDateTime.Value.Minute < 10 ? "0" + sessionDateTime.Value.Minute : sessionDateTime.Value.Minute.ToString();
			string secondStr	= sessionDateTime.Value.Second < 10 ? "0" + sessionDateTime.Value.Second : sessionDateTime.Value.Second.ToString();

			string sessionDateTimeStr = sessionDateTime.Value.Year + dateSeparator
										+ monthStr + dateSeparator
										+ dayStr + " "
										+ hourStr + timeSeparator
										+ minuteStr + timeSeparator
										+ secondStr;

			return sessionDateTimeStr;
		}

		/// <summary>
		/// This method will get a list of asset tracking detail information base on the
		/// device ID.
		/// </summary>
		/// <param name="selectedEquipmentGuid">The selected equipment that is linked to a device.</param>
		/// <param name="inBreadcrumbState">Breadcrumb state.</param>
		/// <param name="fromDateStr">From date string, most current date.</param>
		/// <param name="endDateStr">End date string, date in the past.</param>
		/// <returns>Returns a collection of device ID in the asset tracking detail table.</returns>
		private List<AssetTrackingDetailClass> GetOneDetailEquipment(string selectedEquipmentGuid, string inBreadcrumbState, string fromDateStr, string endDateStr)
		{
			string breadcrumbState = inBreadcrumbState;
			bool topOne = false;

			// This start date is the date 60 days in the past. That is were we want to start retrieving
			// the data.
			var currentDateTime = DateTime.Now;
			var startDate = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, 0, 0, 0);
			startDate = startDate.AddDays(-60);


			DateTime filterStartingDateTime = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, 0, 0, 0);
			DateTime filterEndingDateTime = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, 23, 59, 59);

			if (string.IsNullOrEmpty(inBreadcrumbState) == false)
			{
				if (inBreadcrumbState == "Current")
				{
					topOne = true;
				}

				if (inBreadcrumbState == "All")
				{
					// Get all the detail information for the last 60 days.
					breadcrumbState = "0";
				}

				if (topOne == false)
				{
					DateTime pastDate;
					DateTime currentDate;
					this.CalculateDateRange(breadcrumbState, out pastDate, out currentDate);
					filterStartingDateTime = pastDate;
					filterEndingDateTime = currentDate;
				}
			}

			// Only get asset tracking detail records if there are from and end dates.
			if (string.IsNullOrEmpty(fromDateStr) == false && string.IsNullOrEmpty(endDateStr) == false)
			{
				filterStartingDateTime = this.ConvertToDateTime(endDateStr, true);
				filterEndingDateTime = this.ConvertToDateTime(fromDateStr, false);
			}

			AssetTrackingDeviceClass assetTrackingDevice = this.assetTrackingDeviceCollection.Find(x => x.EquipmentGuidStr == selectedEquipmentGuid);

			var assetTrackingDetailList = FMChannelHelper.MakeCall<IAssetTrackingDetails, List<AssetTrackingDetailClass>>(
											x => x.GetLast60DaysByDevice(this.Security, assetTrackingDevice.DeviceId, startDate, filterStartingDateTime, filterEndingDateTime, topOne));

			return assetTrackingDetailList;
		}

		/// <summary>
		/// This method will return the appropriate breadcrumb value.
		/// </summary>
		/// <returns></returns>
		private string GetBreadcrumbSelection()
		{
			if (this.PeriodDropdown.Value == "-99")
			{
				return string.Empty;
			}

			if (this.PeriodDropdown.Value == "-88")
			{
				return "All";
			}

			if (this.PeriodDropdown.Value == "0")
			{
				return "Current";
			}

			return this.PeriodDropdown.Value;
		}

		/// <summary>
		/// This method will populate the time period dropdown.
		/// </summary>
		private void PopulatePeriodDropdown()
		{
			var newItem = new ListItem { Text = " ", Value = "-99" };
			this.PeriodDropdown.Items.Add(newItem);

			newItem = new ListItem { Text = "All", Value = "-88" };
			this.PeriodDropdown.Items.Add(newItem);

			newItem = new ListItem { Text = "Current", Value = "0" };
			this.PeriodDropdown.Items.Add(newItem);

			for (int nextPeriod = 1; nextPeriod <= 60; nextPeriod++)
			{
				newItem = new ListItem { Text = nextPeriod.ToString(), Value = nextPeriod.ToString() };
				this.PeriodDropdown.Items.Add(newItem);
			}

			// Set the default to be no selection (current selection).
			this.PeriodDropdown.SelectedIndex = 2;
		}

		/// <summary>
		/// This method will validate the dates entered.
		/// </summary>
		// ReSharper disable once UnusedMethodReturnValue.Local
		private bool VerifyDates()
		{
			string fromDateStr	= this.EquipHistoryStartDateHidden.Text;
			string endDateStr	= this.EquipHistoryEndDateHidden.Text;

			if (string.IsNullOrEmpty(fromDateStr) && string.IsNullOrEmpty(endDateStr))
			{
				return true;
			}

			if ((string.IsNullOrEmpty(fromDateStr) && string.IsNullOrEmpty(endDateStr) == false)
				|| (string.IsNullOrEmpty(endDateStr) && string.IsNullOrEmpty(fromDateStr) == false))
			{
				const string ErrMsg = "Must enter both dates.";
				this.ErrorHandler(new Exception(ErrMsg));
				return false;
			}

			string[] parts = fromDateStr?.Split('/');
			if ((parts?.Length ?? 0) < 3)
			{
				const string ErrMsg = "Invalid From date.";
				this.ErrorHandler(new Exception(ErrMsg));
				return false;
			}

		    // ReSharper disable once PossibleNullReferenceException
            // This is protected by the return above
			int year		= int.Parse(parts[0]);
			int month		= int.Parse(parts[1]);
			int day			= int.Parse(parts[2]);
			var startDateOut = new DateTime(year, month, day, 0, 0, 0);

			parts = endDateStr?.Split('/');
			if ((parts?.Length ?? 0) < 3)
			{
				const string ErrMsg = "Invalid From date.";
				this.ErrorHandler(new Exception(ErrMsg));
				return false;
			}

            // ReSharper disable once PossibleNullReferenceException
            // This is protected by the return above
            year = int.Parse(parts[0]);
			month		= int.Parse(parts[1]);
			day			= int.Parse(parts[2]);
			var endDateOut = new DateTime(year, month, day, 23, 59, 59);

			if (endDateOut > startDateOut)
			{
				const string ErrMsg = "From date must be more current than the end date.";
				this.ErrorHandler(new Exception(ErrMsg));
				return false;
			}

			return true;
		}

		/// <summary>
		/// This method will convert a date string formatted like yyyy/mm/dd
		/// to a DateTime object.
		/// </summary>
		/// <param name="inDate">The date string to be converted</param>
		/// <param name="startingDate">Flag indicating whether the inDate is a starting date.</param>
		/// <returns></returns>
		private DateTime ConvertToDateTime(string inDate, bool startingDate)
		{
			string[] parts = inDate.Split('/');

			if (parts.Length >= 3)
			{
				int yyyy;
				int mm;
				int dd;

				if (int.TryParse(parts[0], out yyyy) == false)
				{
					return DateTime.Today;
				}

				if (int.TryParse(parts[1], out mm) == false)
				{
					return DateTime.Today;
				}

				if (int.TryParse(parts[2], out dd) == false)
				{
					return DateTime.Today;
				}

				DateTime convertedDateTime = new DateTime(yyyy, mm, dd, 0, 0, 0);

				if (startingDate == false)
				{
					convertedDateTime = new DateTime(yyyy, mm, dd, 23, 59, 59);
				}

				return convertedDateTime;
			}

			return DateTime.Today;
		}

		/// <summary>
		/// This method will calculate the bread crumb date range. If the selection is current postion,
		/// then the range will be for one day and will return use current position only.
		/// </summary>
		/// <param name="breadcrumbRange">Breadcrumb range.</param>
		/// <param name="startDate">Returns the starting date range.</param>
		/// <param name="endDate">Returns the ending date range.</param>
		/// <returns>Returns Current Position string or empty string.</returns>
		// ReSharper disable once UnusedMethodReturnValue.Local
		private string CalculateDateRange(string breadcrumbRange, out DateTime startDate, out DateTime endDate)
		{
			var currentDateTime = DateTime.Now;
			endDate				= new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, 23, 59, 59);
			startDate			= new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, 0, 0, 0);

			// Zero indicates the most current position of the equipment. With that,
			// the date range needs to be set to 60 days, since we do not know
			// if the vehicle stopped for any duration.
			if (string.IsNullOrEmpty(breadcrumbRange) || breadcrumbRange.Equals("0"))
			{
				startDate = startDate.AddDays(-60);
				return AssetMapsBreadcrumbModel.CurrentPosition;
			}

			int day;

			if (int.TryParse(breadcrumbRange, out day))
			{
				startDate = startDate.AddDays(day * -1);
			}

			return string.Empty;
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.EquipmentHistoryGrid.Height = new Unit(320, UnitType.Pixel);
			this.EquipmentHistoryGrid.FixedHeight = new Unit(320, UnitType.Pixel);

			this.EquipmentHistoryGrid.PageIndexChanged += this.EquipmentHistoryGridPageIndexChanged;
			this.EquipmentHistoryGrid.ItemDataBound += this.EquipmentHistoryGridItemDataBound;
		}
		#endregion
	}
}