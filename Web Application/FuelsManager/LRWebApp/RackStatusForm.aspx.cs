// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RackStatusForm.aspx.cs" company="Varec, Inc.">
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
	using System.Net.Sockets;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using LoadRackLibrary;


	using FuelsManager.FMWebApp;

	/// <summary>
	///     Summary description for RackStatusForm.
	/// </summary>
	public partial class RackStatusForm : FMFormBase, IMenuDiscovery
	{
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
						MenuItemType = FMMenuItemType.OPERATIONS_LOAD_RACK_RACK_STATUS, 
						RootMenuName = "Operations", 
						CategoryName = "Load Rack", 
						ItemName = "Rack Status", 
						NavigateUrl = "..\\LRWebApp\\RackStatusForm.aspx", 
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});

			return items;
		}

		#endregion

		#region Methods

		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			InitializeComponent();
			base.OnInit(e);
		}

		protected void PageSizeDropDown_SelectedIndexChanged(object source, EventArgs e)
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
					SiteClass Site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(
																			this.Security, 
																			this.Security.SiteGuid, 
																			getMemberSites: false, 
																			getSchedulesAndProcessVariables: false, 
																			bGetAssociatedAliases: true)
																	);
					if (!Site.PromptForTractorOrTanker)
					{
						this.RackStatusDataGrid.Columns[6].Visible = false;
						if (Site.PromptForSecondTrailer)
						{
							this.RackStatusDataGrid.Columns[4].HeaderText = this.GetTranslatedText("Trailer 1");
							this.RackStatusDataGrid.Columns[5].HeaderText = this.GetTranslatedText("Trailer 2");
						}
						else
						{
							this.RackStatusDataGrid.Columns[4].HeaderText = this.GetTranslatedText("Trailer");
						}
					}
					else if (!Site.PromptForSecondTrailer)
					{
						this.RackStatusDataGrid.Columns[6].Visible = false;
						this.RackStatusDataGrid.Columns[5].HeaderText = this.GetTranslatedText("Trailer");
					}

					if (!Site.PromptForFirstTrailer)
					{
						this.RackStatusDataGrid.Columns[5].Visible = false;
					}

					if (!Site.PromptForTractorOrTanker && !Site.PromptForSecondTrailer)
					{
						this.RackStatusDataGrid.Columns[5].Visible = false;
					}

					if (!Site.PromptForSecondTrailer && !Site.PromptForFirstTrailer && !Site.PromptForTractorOrTanker)
					{
						this.RackStatusDataGrid.Columns[4].Visible = false;
					}
				}

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private ICollection EnumerateRackStatus()
		{
			StationCollectionClass stationCollection = FMChannelHelper.MakeCall<IStations, StationCollectionClass>(stations => stations.EnumerateByType(this.Security, STATION_TYPE.LOAD_RACK));
			StationCollectionClass offloadStationCollection = FMChannelHelper.MakeCall<IStations, StationCollectionClass>(stations => stations.EnumerateByType(this.Security, STATION_TYPE.OFF_LOADING));
			foreach (StationClass offloadStation in offloadStationCollection)
			{
				stationCollection.Add(offloadStation);
			}

			var rackStatusDataTable = new DataTable();
			DataRow rackStatusDataRow;

			rackStatusDataTable.Columns.Add("RackID", typeof(string));
			rackStatusDataTable.Columns.Add("Status", typeof(string));
			rackStatusDataTable.Columns.Add("DriverID", typeof(string));
			rackStatusDataTable.Columns.Add("CarrierID", typeof(string));
			rackStatusDataTable.Columns.Add("Equipment1ID", typeof(string));
			rackStatusDataTable.Columns.Add("Equipment2ID", typeof(string));
			rackStatusDataTable.Columns.Add("Equipment3ID", typeof(string));
			rackStatusDataTable.Columns.Add("ShipToID/SupplierID", typeof(string));
			rackStatusDataTable.Columns.Add("LoadID", typeof(string));
			rackStatusDataTable.Columns.Add("PermissivesClick");

			ILoadRackManager loadRackManager = this.GetLoadRackManager();

			foreach (StationClass station in stationCollection)
			{
				if (station.InterfaceType == STATION_INTERFACE_TYPE.MANUAL)
				{
					continue;
				}

				rackStatusDataRow = rackStatusDataTable.NewRow();

				rackStatusDataRow["RackID"] = station.ID;

				if (station.Enabled == false)
				{
					rackStatusDataRow["Status"] = this.GetTranslatedText("Disabled");
				}
				else
				{
					bool communicationsFailure = true;
					try
					{
						communicationsFailure = loadRackManager.GetStationCommunicationsStatus(Security.SiteGuid, station.IdentityGuid);
					}
					catch (SocketException se)
					{
						// Show the case where we can't reach the load rack, instead of an opaque error message
						if (se.ErrorCode != 10061)
						{
							throw;
						}
						rackStatusDataRow["Status"] = this.GetTranslatedText("Load Rack Unavailable");
					}

					if (communicationsFailure)
					{
						rackStatusDataRow["Status"] = this.GetTranslatedText("CommFail");
					}
					else
					{

						TransactionDO transaction = null;

						try
						{
							transaction = loadRackManager.GetStationTransaction(this.Security, station.IdentityGuid);
						}

						catch (System.Net.Sockets.SocketException socketExcept)
						{
							// vthompson 10/15/2008
							// Changed to catch the specific exception instead of checking the exception message
							if (socketExcept.ErrorCode != 10061)
							{
								throw;
							}
						}

						if (transaction != null)
						{
							rackStatusDataRow["Status"] = this.GetTranslatedText("Active");
							if (transaction.OperatorID != null)
							{
								rackStatusDataRow["DriverID"] = transaction.OperatorID;
							}

							if (transaction.CarrierID != null)
							{
								rackStatusDataRow["CarrierID"] = transaction.CarrierID;
							}

							if (transaction.DestinationEQ1.RegistrationID != null)
							{
								rackStatusDataRow["Equipment1ID"] = transaction.DestinationEQ1.RegistrationID;
							}

							if (transaction.DestinationEQ2.RegistrationID != null)
							{
								rackStatusDataRow["Equipment2ID"] = transaction.DestinationEQ2.RegistrationID;
							}

							if (transaction.DestinationEQ3.RegistrationID != null)
							{
								rackStatusDataRow["Equipment3ID"] = transaction.DestinationEQ3.RegistrationID;
							}


							if (station.Type == STATION_TYPE.LOAD_RACK)
							{
								if (transaction.ShipToID != null)
								{
									rackStatusDataRow["ShipToID/SupplierID"] = transaction.ShipToID;
								}
							}
							else
							{
								if (transaction.SupplierID != null)
								{
									rackStatusDataRow["ShipToID/SupplierID"] = transaction.SupplierID;
								}
							}

							if (transaction.LoadID != null)
							{
								rackStatusDataRow["LoadID"] = transaction.LoadID;
							}
						}
						else
						{
							rackStatusDataRow["Status"] = this.GetTranslatedText("Idle");
						}
					}
				}

				rackStatusDataRow["PermissivesClick"] = "PermissivesButton_Click('" + station.IdentityGuid.ToString() + "')";

				rackStatusDataTable.Rows.Add(rackStatusDataRow);
			}

			var RackStatusDataView = new DataView(rackStatusDataTable);
			return RackStatusDataView;
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		private void UpdateView()
		{
			ICollection Status = this.EnumerateRackStatus();

			this.RackStatusFormPageSizeDropDown.SetPageSize(this.RackStatusDataGrid, Status.Count);

			this.RackStatusDataGrid.DataSource = Status;
			this.RackStatusDataGrid.DataBind();
		}

		#endregion
	}
}