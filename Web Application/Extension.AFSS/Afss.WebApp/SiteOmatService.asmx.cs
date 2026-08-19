using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

using FuelsManager.Afss.Module.Gasboy.BusinessObjects.SiteOmatObjects;

namespace FuelsManager.Afss.WebApp
{
	using System.Diagnostics;
	using System.Globalization;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.BusinessInterfaces;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.ChannelFactories;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.Constants;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;
	using FuelsManager.Afss.Module.Gasboy.WebApp;
	using FuelsManager.Afss.WebApp.Areas.AutomatedFuelStation.Controllers;
	using FuelsManager.Afss.WebApp.InternalClasses;
	using FuelsManager.FMReportWebMain;
	//using FuelsManager.Afss.WebApp.GasboyHO;

	/// <summary>
	/// Summary description for SiteOmatService
	/// </summary>
	/// <seealso cref="System.Web.Services.WebService" />
	[WebService(Namespace = "http://orpak.com/SiteOmatServices/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	public class SiteOmatService : System.Web.Services.WebService
	{
		/// <summary>
		/// Soes the login.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <param name="password">The password.</param>
		/// <param name="ROCode">The site code. </param>
		/// <returns>String.</returns>
		[WebMethod]
		public LoginRespond SOLogin(String user, String password, string ROCode)
		{
			LoginRespond response = new LoginRespond();
			var securityInstance = this.GetServiceSecurityInstance();
			GasboyStation station = new GasboyStation();

			if (ROCode != null)
			{
				station =
					GasboyChannelHelper.MakeCall<IGasboyStations, GasboyStation>(
						gasboyChannel => gasboyChannel.GetBySiteCode(securityInstance, ROCode));
			
				securityInstance.SiteGuid = station.SiteGuid;
				securityInstance.SiteID =
					FMChannelHelper.MakeCall<ISites, string>(site => site.GetIDNoRefresh(securityInstance, station.SiteGuid));

				station.LastConnectionAttempt = DateTimeOffset.Now;
				GasboyChannelHelper.MakeCall<IGasboyStations>(
					externalStationsService => externalStationsService.Modify(securityInstance, station));

			station.LastSuccessfulConnection = DateTimeOffset.Now;
			GasboyChannelHelper.MakeCall<IGasboyStations>(
				externalStationsService => externalStationsService.Modify(securityInstance, station));
			}



			response.rc = 0;
			response.rc_desc = "OK";
			response.SessionID = "NLPS3jb/.RO/2TMOVTajIogxjEVD62mGX0gae6wZDe0xNwqGeMZg";
			response.Version = "v1.0.0.0";



			return response;
		}

		/// <summary>
		/// Soes the authorize request.
		/// </summary>
		/// <param name="SessionID">The session identifier.</param>
		/// <param name="site_code">The site code.</param>
		/// <param name="amount">The amount.</param>
		/// <param name="track1">The track1.</param>
		/// <param name="track2">The track2.</param>
		/// <param name="card_num">The card number.</param>
		/// <param name="secondary_card_num">The secondary card number.</param>
		/// <returns>AuthorizeRequestRespond.</returns>
		[WebMethod]
		public AuthRequestRespond SOAuthRequest(string SessionID, Int32 site_code, Double amount, String track1, String track2, String card_num, String secondary_card_num)
		{
			AuthRequestRespond response = new AuthRequestRespond();

			var securityInstance = this.GetServiceSecurityInstance();

			string personName;
			string cardNumber;
			int cardExpirationYear;
			int cardExpirationMonth;

			Trace.WriteLine(string.Format("Incoming Request: {0}", track2));

			try
			{
				CardSwipeHelper.ParseMagneticStripe(
					track2,
					out personName,
					out cardNumber,
					out cardExpirationYear,
					out cardExpirationMonth);

				int year = 2000 + cardExpirationYear;
				int month = cardExpirationMonth;
				DateTime expirationDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

				Trace.WriteLine(string.Format("Parsed Card Number: {0}", cardNumber));

				var gasboyDevice =
					GasboyChannelHelper.MakeCall<IGasboyDevices, GasboyDevice>(
						gasboyDevicesSvc => gasboyDevicesSvc.GetByCardNumber(securityInstance, securityInstance.SiteGuid, cardNumber));

				if (null != gasboyDevice)
				{
					if (gasboyDevice.DepartmentIdentityGuid == GasboySpecialConstants.BlacklistDepartmentGuid)
					{
						Trace.WriteLine("Blacklisted Card");

						response.rc = 0;
						response.rc_desc = "Authorization Denied.  Specified Card is blocked.";
						response.auth_result = 76;
						response.fleet_code = GasboySpecialConstants.DefaultFleetCode;
						response.fleet_name = GasboySpecialConstants.DefaultFleetName;
					}
					else
					{
						Trace.WriteLine("Whitelisted Card");

						response.rc = 0;
						response.rc_desc = "OK";
						response.auth_result = 32;
						response.fleet_code = GasboySpecialConstants.DefaultFleetCode;
						response.fleet_name = GasboySpecialConstants.DefaultFleetName;
					}
				}
				else
				{
					Trace.WriteLine("Card Not Found - Default to Authorized.");

					response.rc = 0;
					response.rc_desc = "OK";
					response.auth_result = 32;
					response.fleet_code = GasboySpecialConstants.DefaultFleetCode;
					response.fleet_name = GasboySpecialConstants.DefaultFleetName;
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(string.Format("Exception: {0}.  Stack: {1}", ex.Message, ex.StackTrace));
			}

			return response;
		}

		/// <summary>
		/// Soes the authorize request.
		/// </summary>
		/// <param name="SessionID">The session identifier.</param>
		/// <param name="site_code">The site code.</param>
		/// <param name="amount">The amount.</param>
		/// <param name="odometer">Current Odometer as entered by the driver.</param>
		/// <param name="driver_name">The name of the driver.</param>
		/// <param name="vehicle_card_num">The card number.</param>
		/// <param name="ppv">Price of the selected product if available.</param>
		/// <param name="product_code">Selected product code if available. </param>
		/// <returns>AuthorizeRequestRespond.</returns>
		[WebMethod]
		public AuthorizeRequestRespond SOAuthorizeRequest(string SessionID, string site_code, string card_num, string driver_name, string odometer, float amount, double ppv, int product_code)
		{
			AuthorizeRequestRespond response = new AuthorizeRequestRespond();

			var securityInstance = this.GetServiceSecurityInstance();

			GasboyStation station =
				GasboyChannelHelper.MakeCall<IGasboyStations, GasboyStation>(
					gasboyChannel => gasboyChannel.GetBySiteCode(securityInstance, site_code));
			if (station != null)
			{
				securityInstance.SiteGuid = station.SiteGuid;
				securityInstance.SiteID =
					FMChannelHelper.MakeCall<ISites, string>(site => site.GetIDNoRefresh(securityInstance, station.SiteGuid));

				station.LastConnectionAttempt = DateTimeOffset.Now;
				GasboyChannelHelper.MakeCall<IGasboyStations>(
					externalStationsService => externalStationsService.Modify(securityInstance, station));
			}



			GasboyEvents gasboyEvents = new GasboyEvents();

			Trace.WriteLine(string.Format("Incoming Request: {0}", card_num));

			try
			{

				var gasboyDevice =
					GasboyChannelHelper.MakeCall<IGasboyDevices, GasboyDevice>(
						gasboyDevicesSvc => gasboyDevicesSvc.GetByCardNumber(securityInstance, securityInstance.SiteGuid, card_num));

				if (null != gasboyDevice)
				{
					if (gasboyDevice.DepartmentIdentityGuid == GasboySpecialConstants.BlacklistDepartmentGuid)
					{
						Trace.WriteLine("Blacklisted Card");

						response.rc = 0;
						response.rc_desc = "Ok";
						response.auth_result = 76;
						response.auth_result_msg = "Auth ok";
						response.limit_type = 0;
						response.limit = 99999999;
						response.credit = 99999999;
						response.any_product = 1;
						response.fuel_type = 1;
						response.num_products = 1;
						//productid[] myProducts = new productid[1];
						//myProducts[0] = new productid();
						//myProducts[0].value = "test"
						response.aProducts = new int[1];
						response.aProducts[0] = 0;
						response.num_dry_prod = 1;
						response.aDryProducts = new int[1];
						response.aDryProducts[0] = 0;
						response.driver_type_req = 0;
						response.drivers_type = 0;
						response.num_drivers = 1;
						//response.aDrivers = new driverid[1];
						//response.aDrivers[0].value = "test";
						response.aDrivers = new int[1];
						response.aDrivers[0] = 0;
						response.mean_type = 4;
						response.fleet_code = 1;
						response.fleet_name = "Default";
						response.dept_id = GasboySpecialConstants.DefaultBlackListDepartmentID;
						response.plate = 1;
						response.ref_num = 900000001;
						response.pressure_level = 0;
						response.fleet_id = GasboySpecialConstants.DefaultFleetID;
						response.mean_id = Convert.ToInt32(gasboyDevice.DeviceID);
						response.mean_name = "1";
						response.price_list_id = 0;
						response.prompt_odo = 0;
						response.prompt_plate = 0;
						response.use_pin_code = 0;
						response.pin_code = 0;
						response.ext_bank_rc = "";
						response.ext_bank_desc = "";
						response.volume_limit = 0;
						response.route_prompt = 0;

						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
						alarmAndEventChannel =>
						{
							alarmAndEventChannel.Add(
							securityInstance,
							gasboyEvents.GasboyOnlineAuthorizationDeniedEvent(gasboyDevice.ID, site_code));
						}
						);
					}
					else
					{
						Trace.WriteLine("Whitelisted Card");

						response.rc = 0;
						response.rc_desc = "Ok";
						response.auth_result = 32;
						response.auth_result_msg = "Auth ok";
						response.limit_type = 0;
						response.limit = 99999999;
						response.credit = 99999999;
						response.any_product = 1;
						response.fuel_type = 1;
						response.num_products = 1;
						//productid[] myProducts = new productid[1];
						//myProducts[0] = new productid();
						//myProducts[0].value = "test"
						response.aProducts = new int[1];
						response.aProducts[0] = 0;
						response.num_dry_prod = 1;
						response.aDryProducts = new int[1];
						response.aDryProducts[0] = 0;
						response.driver_type_req = 0;
						response.drivers_type = 0;
						response.num_drivers = 1;
						//response.aDrivers = new driverid[1];
						//response.aDrivers[0].value = "test";
						response.aDrivers = new int[1];
						response.aDrivers[0] = 0;
						response.mean_type = 4;
						response.fleet_code = 1;
						response.fleet_name = "Default";
						response.dept_id = GasboySpecialConstants.DefaultDepartmentID;
						response.plate = 1;
						response.ref_num = 900000001;
						response.pressure_level = 0;
						response.fleet_id = GasboySpecialConstants.DefaultFleetID;
						response.mean_id = 900000001;
						response.mean_name = "1";
						response.price_list_id = 0;
						response.prompt_odo = 0;
						response.prompt_plate = 0;
						response.use_pin_code = 0;
						response.pin_code = 0;
						response.ext_bank_rc = "";
						response.ext_bank_desc = "";
						response.volume_limit = 0;
						response.route_prompt = 0;

						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
						alarmAndEventChannel =>
						{
							alarmAndEventChannel.Add(
							securityInstance,
							gasboyEvents.GasboyOnlineAuthorizationApprovedEvent(gasboyDevice.ID, site_code));
						}
						);

					}
				}
				else
				{
					Trace.WriteLine("Card Not Found - Default to Authorized.");

					response.rc = 0;
					response.rc_desc = "Ok";
					response.auth_result = 32;
					response.auth_result_msg = "Auth ok";
					response.limit_type = 0;
					response.limit = 99999999;
					response.credit = 99999999;
					response.any_product = 1;
					response.fuel_type = 1;
					response.num_products = 1;
					//productid[] myProducts = new productid[1];
					//myProducts[0] = new productid();
					//myProducts[0].value = "test"
					response.aProducts = new int[1];
					response.aProducts[0] = 0;
					response.num_dry_prod = 1;
					response.aDryProducts = new int[1];
					response.aDryProducts[0] = 0;
					response.driver_type_req = 0;
					response.drivers_type = 0;
					response.num_drivers = 1;
					//response.aDrivers = new driverid[1];
					//response.aDrivers[0].value = "test";
					response.aDrivers = new int[1];
					response.aDrivers[0] = 0;
					response.mean_type = 4;
					response.fleet_code = 1;
					response.fleet_name = "Default";
					response.dept_id = GasboySpecialConstants.DefaultDepartmentID;
					response.plate = 1;
					response.ref_num = 900000001;
					response.pressure_level = 0;
					response.fleet_id = GasboySpecialConstants.DefaultFleetID;
					response.mean_id = Convert.ToInt32(gasboyDevice.DeviceID);
					response.mean_name = "1";
					response.price_list_id = 0;
					response.prompt_odo = 0;
					response.prompt_plate = 0;
					response.use_pin_code = 0;
					response.pin_code = 0;
					response.ext_bank_rc = "";
					response.ext_bank_desc = "";
					response.volume_limit = 0;
					response.route_prompt = 0;

					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
					alarmAndEventChannel =>
					{
						alarmAndEventChannel.Add(
						securityInstance,
						gasboyEvents.GasboyOnlineAuthorizationApprovedEvent(gasboyDevice.ID, site_code));
					}
					);

				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(string.Format("Exception: {0}.  Stack: {1}", ex.Message, ex.StackTrace));
			}

			return response;
		}

		/// <summary>
		/// Soes the logout.
		/// </summary>
		/// <param name="sessionID">The session identifier.</param>
		/// <returns>GetRespond.</returns>
		[WebMethod]
		public GetRespond SOLogout(String sessionID)
		{
			var response = new GetRespond() { rc = 0, rc_desc = string.Empty };

			return response;
		}

		/// <summary>
		/// Accepts Gasboy Transactions and sends them to processing
		/// </summary>
		/// <param name="sessionID">The session identifier.</param>
		/// <returns>GetRespond.</returns>
		[WebMethod]
		//public SOHOSendNewUpdatedTransactionsRespond SOHOSendNewUpdatedTransactions(string fleet_it, string fleet_name, string fleet_code, string product_name, string product_code, string mean_id, string mean_name, string plate, string driver_mean_id, string driver_plate, string driver_tag, string ext_auth_number,  string density, string temperature, string engine_hours, string pump_id, string pump, string nozzle_id, string nozzle, string hose_number, string tank_name, string shift_id, string odometer, string quantity, string ppv, string total_price, string proxy_id, string timestamp, string type, string track1, string track2, string tag, string cash_customer_id, string transID, string siteCode )
		public SOHOSendNewUpdatedTransactionsRespond SOHOSendNewUpdatedTransactions(int num_transactions, soTransaction[] a_soTransaction )
		{
			GasboyEvents gasboyEvents = new GasboyEvents();
			var securityInstance = this.GetServiceSecurityInstance();

			try
			{
				GasboyStation station = new GasboyStation();
				SOHOSendNewUpdatedTransactionsRespond response = new SOHOSendNewUpdatedTransactionsRespond();

			if (num_transactions == 0)
			{
				response.rc = 0;
				response.rc_desc = "OK";

				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
				alarmAndEventChannel =>
				{
					alarmAndEventChannel.Add(
					securityInstance,
					gasboyEvents.GasboyTransactionReceivedEvent("0", "None"));
				}
				);

				return response;
			}

			List<GasboyStationTransaction> downloadedTransactions = new List<GasboyStationTransaction>();
	
				foreach (soTransaction transaction in a_soTransaction)
				{
					var gasboyTransaction = new GasboyStationTransaction()
						                        {
							                        ID = transaction.id.ToString(CultureInfo.CurrentCulture),
							                        ExternalStationID = transaction.stn_id.ToString(),
							                        FleetID =
								                        transaction.fleet_id.ToString(CultureInfo.CurrentCulture),
							                        //FleetName = transaction.fleet,
							                        //FleetCode =transaction.fleet_code.ToString(CultureInfo.CurrentCulture),
							                        ProductName = transaction.product_name,
							                        ProductCode =
								                        transaction.product_code.ToString(
									                        CultureInfo.CurrentCulture),
							                        MeanID =
								                        transaction.mean_id.ToString(CultureInfo.CurrentCulture),
							                        MeanName = transaction.mean_name,
							                        FuelingVehiclePlate = transaction.plate,
							                        DriverMeanID =
								                        transaction.driver_mean_id.ToString(
									                        CultureInfo.CurrentCulture),
							                        DriverPlate = transaction.driver_mean_plate,
							                        DriverTag = transaction.driver_mean_tag,
							                        ExternalAuthorizationNumber = transaction.ext_auth_number,
							                        Density =
								                        transaction.density.ToString(CultureInfo.CurrentCulture),
							                        Temperature =
								                        transaction.temperature.ToString(
									                        CultureInfo.CurrentCulture),
							                        EngineHours =
								                        transaction.engine_hours.ToString(
									                        CultureInfo.CurrentCulture),
							                        PumpID =
								                        transaction.pump_id.ToString(CultureInfo.CurrentCulture),
							                        Pump =
								                        transaction.pump.ToString(CultureInfo.CurrentCulture),
							                        NozzleID =
								                        transaction.nozzle_id.ToString(
									                        CultureInfo.CurrentCulture),
							                        Nozzle =
								                        transaction.nozzle.ToString(CultureInfo.CurrentCulture),
							                        HoseNumber =
								                        transaction.hose_number.ToString(
									                        CultureInfo.CurrentCulture),
							                        TankName = transaction.tank_name,
							                        ShiftID =
								                        transaction.shift_id.ToString(CultureInfo.CurrentCulture),
							                        Odometer =
								                        transaction.odometer.ToString(CultureInfo.CurrentCulture),
							                        Quantity =
								                        transaction.quantity.ToString(CultureInfo.CurrentCulture),
							                        PricePerVolume =
								                        transaction.ppv.ToString(CultureInfo.CurrentCulture),
							                        TotalPrice =
								                        transaction.total_price.ToString(
									                        CultureInfo.CurrentCulture),
							                        ProxyDeviceID =
								                        transaction.proxy_id.ToString(CultureInfo.CurrentCulture),
							                        TransactionTimeStamp =
								                        transaction.timestamp.ToString(
									                        CultureInfo.CurrentCulture),
							                        TransactionType =
								                        transaction.type.ToString(CultureInfo.CurrentCulture),
							                        TrackData1 =
								                        transaction.track1.ToString(CultureInfo.CurrentCulture),
							                        TrackData2 =
								                        transaction.track2.ToString(CultureInfo.CurrentCulture),
							                        Tag = transaction.tag.ToString(CultureInfo.CurrentCulture),
							                        CashCustomerID =
								                        transaction.cash_customer_id.ToString(
									                        CultureInfo.CurrentCulture),
							                        DriverName = transaction.driver_name
						                        };

					station =
						GasboyChannelHelper.MakeCall<IGasboyStations, GasboyStation>(
							gasboyChannel => gasboyChannel.GetBySiteCode(securityInstance, transaction.stn_id.ToString()));
					if (station != null)
					{
						gasboyTransaction.ExternalStationGuid = station.IdentityGuid;
						securityInstance.SiteGuid = station.SiteGuid;
						//transactions should import into the site the station is assigned to, not site admin (as the security object comes from GetServiceSecurityInstance in GasboyServiceProcess.cs)
						securityInstance.SiteID =
							FMChannelHelper.MakeCall<ISites, string>(site => site.GetIDNoRefresh(securityInstance, station.SiteGuid));
					}
					else
					{
						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
						alarmAndEventChannel =>
						{
							alarmAndEventChannel.Add(
							securityInstance,
							gasboyEvents.GasboyTransactionReceivedErrorEvent("There is no matching station for id: "+ transaction.stn_id));
						}
						);
						response.rc = -1;
						response.rc_desc = "There is no matching station for id: " + transaction.stn_id;

						return response;
					}

					downloadedTransactions.Add(gasboyTransaction);
				}

				station.LastConnectionAttempt = DateTimeOffset.Now;
				GasboyChannelHelper.MakeCall<IGasboyStations>(
					externalStationsService => externalStationsService.Modify(securityInstance, station));

				var result =
					GasboyChannelHelper.MakeCall<IGasboyTransactionProcessor, string>(
						x => x.ImportTransactions(securityInstance, station, downloadedTransactions));

				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
				alarmAndEventChannel =>
				{
					alarmAndEventChannel.Add(
					securityInstance,
					gasboyEvents.GasboyTransactionReceivedEvent(downloadedTransactions.Count.ToString(), station.ID));
				}
				);

				station.LastSuccessfulConnection = DateTimeOffset.Now;
				GasboyChannelHelper.MakeCall<IGasboyStations>(
					externalStationsService => externalStationsService.Modify(securityInstance, station));

				response.rc = 0;
				response.rc_desc = "OK";

				return response;
			}
			catch (Exception ex)
			{
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
				alarmAndEventChannel =>
				{
					alarmAndEventChannel.Add(
					securityInstance,
					gasboyEvents.GasboyTransactionReceivedErrorEvent(ex.ToString()));
				}
				);

				SOHOSendNewUpdatedTransactionsRespond response = new SOHOSendNewUpdatedTransactionsRespond();
				response.rc = -1;
				response.rc_desc = ex.ToString();

				return response;
			}



		}

		/// <summary>
		/// Returns the fleet configuration to the gasboy
		/// </summary>
		/// <param name="sessionID">The session identifier.</param>
		/// <returns>GetRespond.</returns>
		[WebMethod]
		public SOGetNewUpdatedFleetsRespond SOGetNewUpdatedFleets(
			int stn_id)
		{

			GasboyEvents gasboyEvents = new GasboyEvents();
			var securityInstance = this.GetServiceSecurityInstance();
			SOGetNewUpdatedFleetsRespond response = new SOGetNewUpdatedFleetsRespond();

			try
			{
				response.num_fleets = 1;
				response.a_soFleet = new soFleet[1];

				response.a_soFleet[0].id = GasboySpecialConstants.DefaultFleetID;
				response.a_soFleet[0].name = GasboySpecialConstants.DefaultFleetName;
				response.a_soFleet[0].status = 2;
				response.a_soFleet[0].code = GasboySpecialConstants.DefaultFleetCode;
				response.a_soFleet[0].default_rule = 200000000;

				response.a_soFleet[0].address = " ";
				response.a_soFleet[0].phone = " ";
				response.a_soFleet[0].fax = " ";
				response.a_soFleet[0].email = " ";
				response.a_soFleet[0].contact = " ";
				response.a_soFleet[0].acctyp = 0;
				response.a_soFleet[0].available_amount = 0;
				response.a_soFleet[0].min_allowed = 0;
				response.a_soFleet[0].use_pin_code = 0;
				response.a_soFleet[0].auth_pin_from = 2;
				response.a_soFleet[0].nr_pin_retries = 0;
				response.a_soFleet[0].block_if_pin_retries_fail = 0;
				response.a_soFleet[0].opos_prompt_for_plate = 0;
				response.a_soFleet[0].opos_prompt_for_odometer = 0;
				response.a_soFleet[0].do_odo_reasonability_check = 0;
				response.a_soFleet[0].max_eh_delta_allowed = 0;
				response.a_soFleet[0].nr_odo_retries = 0;
				response.a_soFleet[0].price_list_id = 0;
				response.a_soFleet[0].use_rule_limit = 0;
				response.a_soFleet[0].max_rules = 0;
				response.a_soFleet[0].max_group_rules = 0;
				response.a_soFleet[0].eft_id = 0;
				response.a_soFleet[0].wex_renewal_fee = 0;
				response.a_soFleet[0].wex_billing_fee_56 = 0;
				response.a_soFleet[0].on_line_fee_68 = 0;
				response.a_soFleet[0].line_of_credit = 0;
				response.a_soFleet[0].opos_prompt_for_engine_hours = 0;
				response.a_soFleet[0].prompt_always_for_viu = 1;
				response.a_soFleet[0].do_eh_reasonability_check = 1;
				response.a_soFleet[0].max_eh_delta_allowed = 0;
				response.a_soFleet[0].nr_eh_retries = 0;
				response.a_soFleet[0].reject_if_eh_check_fails = 0;

				response.rc = 0;
				response.rc_desc = "OK";

				GasboyStation station =
					GasboyChannelHelper.MakeCall<IGasboyStations, GasboyStation>(
						gasboyChannel => gasboyChannel.GetBySiteCode(securityInstance, Convert.ToString(stn_id)));
				if (station != null)
				{
					securityInstance.SiteGuid = station.SiteGuid;
					securityInstance.SiteID =
						FMChannelHelper.MakeCall<ISites, string>(site => site.GetIDNoRefresh(securityInstance, station.SiteGuid));

					station.LastConnectionAttempt = DateTimeOffset.Now;
					station.LastSuccessfulConnection = DateTimeOffset.Now;
					GasboyChannelHelper.MakeCall<IGasboyStations>(
						externalStationsService => externalStationsService.Modify(securityInstance, station));
				}

				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
				alarmAndEventChannel =>
				{
					alarmAndEventChannel.Add(
					securityInstance,
					gasboyEvents.GasboyFleetDataTransferredEvent(stn_id.ToString()));
				}
				);
				return response;
			}
			catch (Exception ex)
			{
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
				alarmAndEventChannel =>
				{
					alarmAndEventChannel.Add(
					securityInstance,
					gasboyEvents.GasboyDataTransferErrorEvent(stn_id.ToString(),"Fleet",ex.ToString()));
				}
				);
				response.rc = -1;
				response.rc_desc = ex.ToString();
				return response;
			}
		}

		/// <summary>
		/// Returns the Department configuration to the gasboy
		/// </summary>
		/// <param name="sessionID">The session identifier.</param>
		/// <returns>GetRespond.</returns>
		[WebMethod]
		public SOGetNewUpdatedDeptsRespond SOGetNewUpdatedDepts(int stn_id)
		{
			var securityInstance = this.GetServiceSecurityInstance();
			GasboyEvents gasboyEvents = new GasboyEvents();
			SOGetNewUpdatedDeptsRespond response = new SOGetNewUpdatedDeptsRespond();
			try
			{




				response.num_dept = 2;
				response.a_soDept = new soDept[2];

				//configure the default department
				response.a_soDept[0].id = GasboySpecialConstants.DefaultDepartmentID;
				response.a_soDept[0].fleet_id = GasboySpecialConstants.DefaultFleetID;
				response.a_soDept[0].name = GasboySpecialConstants.DefaultDepartmentName;
				response.a_soDept[0].status = 2;
				response.a_soDept[0].code = GasboySpecialConstants.DefaultDepartmentCode;
				response.a_soDept[0].default_rule = 200000000;
				response.a_soDept[0].address = " ";
				response.a_soDept[0].phone = " ";
				response.a_soDept[0].fax = " ";
				response.a_soDept[0].email = " ";
				response.a_soDept[0].contact = " ";
				response.a_soDept[0].use_pin_code = 0;
				response.a_soDept[0].auth_pin_from = 2;
				response.a_soDept[0].nr_pin_retries = 0;
				response.a_soDept[0].block_if_pin_retries_fail = 0;
				response.a_soDept[0].opos_prompt_for_plate = 0;
				response.a_soDept[0].opos_prompt_for_odometer = 0;
				response.a_soDept[0].do_odo_reasonability_check = 1;
				response.a_soDept[0].max_odo_delta_allowed = 0;
				response.a_soDept[0].nr_odo_retries = 0;
				response.a_soDept[0].price_list_id = 0;
				response.a_soDept[0].black_white_type = 1;
				response.a_soDept[0].opos_prompt_for_engine_hours = 0;
				response.a_soDept[0].prompt_always_for_viu = 1;
				response.a_soDept[0].do_eh_reasonability_check = 1;
				response.a_soDept[0].max_eh_delta_allowed = 0;
				response.a_soDept[0].nr_eh_retries = 0;
				response.a_soDept[0].reject_if_eh_check_fails = 0;
				response.a_soDept[1].reject_if_odm_check_fails = 0;

				//configure the blacklist department
				response.a_soDept[1].id = GasboySpecialConstants.DefaultBlackListDepartmentID;
				response.a_soDept[1].fleet_id = GasboySpecialConstants.DefaultFleetID;
				response.a_soDept[1].name = GasboySpecialConstants.DefaultBlackListDepartmentName;
				response.a_soDept[1].status = 2;
				response.a_soDept[1].code = GasboySpecialConstants.DefaultBlackListDepartmentCode;
				response.a_soDept[1].default_rule = 200000000;
				response.a_soDept[1].address = " ";
				response.a_soDept[1].phone = " ";
				response.a_soDept[1].fax = " ";
				response.a_soDept[1].email = " ";
				response.a_soDept[1].contact = " ";
				response.a_soDept[1].use_pin_code = 0;
				response.a_soDept[1].auth_pin_from = 2;
				response.a_soDept[1].nr_pin_retries = 0;
				response.a_soDept[1].block_if_pin_retries_fail = 0;
				response.a_soDept[1].opos_prompt_for_plate = 0;
				response.a_soDept[1].opos_prompt_for_odometer = 0;
				response.a_soDept[1].do_odo_reasonability_check = 1;
				response.a_soDept[1].max_odo_delta_allowed = 0;
				response.a_soDept[1].nr_odo_retries = 0;
				response.a_soDept[1].price_list_id = 0;
				response.a_soDept[1].black_white_type = 2;
				response.a_soDept[1].opos_prompt_for_engine_hours = 0;
				response.a_soDept[1].prompt_always_for_viu = 1;
				response.a_soDept[1].do_eh_reasonability_check = 1;
				response.a_soDept[1].max_eh_delta_allowed = 0;
				response.a_soDept[1].nr_eh_retries = 0;
				response.a_soDept[1].reject_if_eh_check_fails = 0;
				response.a_soDept[1].reject_if_odm_check_fails = 0;

				response.rc = 0;
				response.rc_desc = "OK";



				GasboyStation station =
					GasboyChannelHelper.MakeCall<IGasboyStations, GasboyStation>(
						gasboyChannel => gasboyChannel.GetBySiteCode(securityInstance, Convert.ToString(stn_id)));
				if (station != null)
				{
					securityInstance.SiteGuid = station.SiteGuid;
					securityInstance.SiteID =
						FMChannelHelper.MakeCall<ISites, string>(site => site.GetIDNoRefresh(securityInstance, station.SiteGuid));

					station.LastConnectionAttempt = DateTimeOffset.Now;
					GasboyChannelHelper.MakeCall<IGasboyStations>(
						externalStationsService => externalStationsService.Modify(securityInstance, station));
					station.LastSuccessfulConnection = DateTimeOffset.Now;
					GasboyChannelHelper.MakeCall<IGasboyStations>(
						externalStationsService => externalStationsService.Modify(securityInstance, station));
				}
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
				alarmAndEventChannel =>
				{
					alarmAndEventChannel.Add(
					securityInstance,
					gasboyEvents.GasboyDepartmentDataTransferredEvent(stn_id.ToString()));
				}
				);
				return response;
			}
			catch (Exception ex)

			{
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
				alarmAndEventChannel =>
				{
					alarmAndEventChannel.Add(
					securityInstance,
					gasboyEvents.GasboyDataTransferErrorEvent(stn_id.ToString(), "Department", ex.ToString()));
				}
				);
				response.rc = -1;
				response.rc_desc = ex.ToString();
				return response;

			}		
}

[WebMethod]
		public SOGetNewUpdatedMeansRespond SOGetNewUpdatedMeans(int stn_id)
		{
			GasboyStation station = new GasboyStation();
			var securityInstance = this.GetServiceSecurityInstance();
			GasboyEvents gasboyEvents = new GasboyEvents();
			SOGetNewUpdatedMeansRespond response = new SOGetNewUpdatedMeansRespond();


			try
	{
		station =
			GasboyChannelHelper.MakeCall<IGasboyStations, GasboyStation>(
				gasboyChannel => gasboyChannel.GetBySiteCode(securityInstance, stn_id.ToString()));

		if (station != null)
		{
			securityInstance.SiteGuid = station.SiteGuid;
			securityInstance.SiteID =
				FMChannelHelper.MakeCall<ISites, string>(site => site.GetIDNoRefresh(securityInstance, station.SiteGuid));

			station.LastConnectionAttempt = DateTimeOffset.Now;
			GasboyChannelHelper.MakeCall<IGasboyStations>(
				externalStationsService => externalStationsService.Modify(securityInstance, station));

		}




		var gasboyDeviceList =
			GasboyChannelHelper.MakeCall<IGasboyDevices, List<GasboyDevice>>(
				gasboyDevicesSvc => gasboyDevicesSvc.EnumerateWithDeleted(securityInstance));

		response.num_of_means = gasboyDeviceList.Count;

		response.a_soMean = new soMean[response.num_of_means];
		int count = 0;
		foreach (GasboyDevice device in gasboyDeviceList)
		{
			response.a_soMean[count] = new soMean();
			response.a_soMean[count].id = device.DeviceID;
			response.a_soMean[count].fleet_id = device.FleetID ?? GasboySpecialConstants.DefaultFleetID;
			response.a_soMean[count].dept_id = device.DepartmentID ?? GasboySpecialConstants.DefaultDepartmentID;
			response.a_soMean[count].plate = device.CardNumber;
			response.a_soMean[count].name = device.DeviceName;
			response.a_soMean[count].rule = GasboySpecialConstants.NoRestrictionGroupRuleCode;
			response.a_soMean[count].employee_type = (int)device.EmployeeType;
			response.a_soMean[count].plate = device.VehiclePlate;

			response.a_soMean[count].string1 = device.CardNumber;
			response.a_soMean[count].type = (int)device.DeviceType;
			response.a_soMean[count].hardware_type = (int)device.HardwareType;
			response.a_soMean[count].auttyp = (int)device.AuthorizationType;
			response.a_soMean[count].use_pin_code = Convert.ToInt32(device.UsePINCode);
			response.a_soMean[count].pin_code = device.PINCode;
			response.a_soMean[count].auth_pin_from = 0;
			response.a_soMean[count].opos_plate_check_type = (int)device.VehiclePlateCheckType;
			response.a_soMean[count].prompt_always_for_viu = Convert.ToInt32(device.AlwaysPromptForAdditionalValidation);
			response.a_soMean[count].opos_prompt_for_odometer = 1;
			if (device.RecordStatus == GasboyRecordStatus.Deleted)
			{
				response.a_soMean[count].status = 0;
			}
			else
			{
				response.a_soMean[count].status = 2;
			}


			count++;
		}

		response.rc = 0;
		response.rc_desc = "OK";
		if (station != null) station.LastSuccessfulConnection = DateTimeOffset.Now;
		GasboyChannelHelper.MakeCall<IGasboyStations>(
			externalStationsService => externalStationsService.Modify(securityInstance, station));

				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
				alarmAndEventChannel =>
				{
					alarmAndEventChannel.Add(
					securityInstance,
					gasboyEvents.GasboyMeantDataTransferredEvent(stn_id.ToString()));
				}
				);

				return response;
	}
	catch (Exception ex)
	{

				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
				alarmAndEventChannel =>
				{
					alarmAndEventChannel.Add(
					securityInstance,
					gasboyEvents.GasboyDataTransferErrorEvent(stn_id.ToString(), "Mean", ex.ToString()));
				}
				);

				response.rc = -1;
				response.rc_desc = ex.ToString();
				return response;

	}
		}

		[WebMethod]
		public SOResponse SOHOSendNewUpdatedEvents(int site_code, int num, soEventLog[] a_soEventLog)
		{
			var securityInstance = this.GetServiceSecurityInstance();
			List<GasboyStationLog> uploadedEvents = new List<GasboyStationLog>();
			SOResponse response = new SOResponse();
			GasboyEvents gasboyEvents = new GasboyEvents();
			GasboyStation station;

			try
			{
				station =
					GasboyChannelHelper.MakeCall<IGasboyStations, GasboyStation>(
						gasboyChannel => gasboyChannel.GetBySiteCode(securityInstance, site_code.ToString()));

				if (station != null)
				{
					securityInstance.SiteGuid = station.SiteGuid;
					securityInstance.SiteID =
						FMChannelHelper.MakeCall<ISites, string>(site => site.GetIDNoRefresh(securityInstance, station.SiteGuid));

					station.LastConnectionAttempt = DateTimeOffset.Now;
					GasboyChannelHelper.MakeCall<IGasboyStations>(
						externalStationsService => externalStationsService.Modify(securityInstance, station));




					foreach (soEventLog uploadedEvent in a_soEventLog)
					{
						var gasboyStationEvent = new GasboyStationEvent()
							                         {
								                         ExternalStationGuid = station.IdentityGuid,
								                         ID = uploadedEvent.error_code.ToString(),
								                         LogType = ExternalStationLogType.StationEvent,
								                         LogDate = Convert.ToDateTime(uploadedEvent.error_date),
								                         ErrorClassCode =
									                         (GasboyEventErrorClassCode)uploadedEvent.errcls_code,
								                         ErrorCode = (ErrorCode)uploadedEvent.error_code,
								                         EventID = uploadedEvent.id,
								                         FleetID = uploadedEvent.fleet_id,
								                         ObjectID = uploadedEvent.object_id,
								                         EventObjectType =
									                         (GasboyEventObjectType)uploadedEvent.object_type,
								                         DeviceName = uploadedEvent.device_name,
								                         Field1 = uploadedEvent.field1,
								                         Field2 = uploadedEvent.field2,
								                         Field3 = uploadedEvent.field3,
								                         Field4 = uploadedEvent.field4,
								                         Field5 = uploadedEvent.field5,
								                         Field6 = uploadedEvent.field6,
								                         Field7 = uploadedEvent.field7,
								                         Field8 = uploadedEvent.field8,
							                         };
						uploadedEvents.Add(gasboyStationEvent);
					}

					GasboyChannelHelper.MakeCall<IGasboyStations>(
						gasboyStationsChannel => gasboyStationsChannel.AddExternalStationLogs(securityInstance, uploadedEvents));

					response.rc = 0;
					response.rc_desc = "Ok";

					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
					alarmAndEventChannel =>
					{
						alarmAndEventChannel.Add(
						securityInstance,
						gasboyEvents.GasboyEventDataTransferredEvent(station.ID.ToString()));
					}
					);

					station.LastSuccessfulConnection = DateTimeOffset.Now;
					GasboyChannelHelper.MakeCall<IGasboyStations>(
						externalStationsService => externalStationsService.Modify(securityInstance, station));
					return response;
				}
				else
				{
					response.rc = -1;
					response.rc_desc = "Station could not be parsed";
						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
						alarmAndEventChannel =>
					{
						alarmAndEventChannel.Add(
						securityInstance,
						gasboyEvents.GasboyDataTransferErrorEvent(site_code.ToString(), "Event", "Station could not be parsed."));
					}
				);

					return response;
				}


			}
			catch (Exception ex)
			{

				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
				alarmAndEventChannel =>
				{
					alarmAndEventChannel.Add(
					securityInstance,
					gasboyEvents.GasboyDataTransferErrorEvent(site_code.ToString(), "Event", ex.ToString()));
				}
				);

				response.rc = -1;
				response.rc_desc = ex.ToString();
				return response;

			}

		}

		[WebMethod]
		public SOHOClockSynchRespond SOHOClockSynch(int stn_id)
		{
			SOHOClockSynchRespond response = new SOHOClockSynchRespond();
			DateTimeOffset sd = new DateTimeOffset();
			sd = DateTimeOffset.Now;

			string format = "yyyy-MM-dd HH:mm:ss";

			response.GMTDateTimeString = sd.ToString(format);
			response.rc = 0;
			response.rc_desc = "Ok";
			return response;
		}

		/// <summary>
		/// The get service security instance.
		/// </summary>
		/// <returns>
		/// The <see cref="SecurityClass"/>.
		/// </returns>
		private SecurityClass GetServiceSecurityInstance()
		{
			SecurityClass serviceProcessSecurity = new SecurityClass();
			serviceProcessSecurity.LoginSiteGuid = Guids.SiteAdminGuid;
			serviceProcessSecurity.LoginSiteID = "SiteAdmin";
			serviceProcessSecurity.SiteGuid = Guids.SiteAdminGuid;
			serviceProcessSecurity.SiteID = "SiteAdmin";
			serviceProcessSecurity.UserGuid = Guids.UserAdminGuid;
			serviceProcessSecurity.UserID = "GasboyService";
			serviceProcessSecurity.AddRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION);
			serviceProcessSecurity.AddRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION);

			serviceProcessSecurity.AddRight(RIGHT.BASE_EXPORT);
			serviceProcessSecurity.AddRight(RIGHT.INTERFACE_IMPORT);

			serviceProcessSecurity.AddRight(RIGHT.MODIFY_TRANSACTION_DATA);

			return serviceProcessSecurity;
		}
	}
}
