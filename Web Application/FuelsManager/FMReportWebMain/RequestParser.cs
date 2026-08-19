// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestParser.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the RequestParser type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using FMCore;

namespace FuelsManager.FMReportWebMain
{
	using System;
	using System.Collections;
	using System.Diagnostics;
	using System.Reflection;
	using System.Web;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	public class RequestParser
	{
		#region Constants and Fields

		private const string AliasList = "AliasList";
		private const string CarrierName = "CarrierName";
		private const string CarrierRepCont = "CarrierRepCont";
		private const string CarrierRepTel = "CarrierRepTel";
		private const string Consignor = "Consignor";
		private const string CostOfRepair = "CostOfRepair";
		private const string DateCarrNotified = "DateCarrNotified";
		private const string EmailAdd = "EmailAdd";
		private const string FacsmileNum = "FacsileNum";
		private const string FromStation = "FromStation";
		private const string NameOfPreparer = "NameOfPreparer";
		private const string PopupDisplay = "PopupDisplay";
		private const string RebateNumbers = "RebateNumbers";
		private const string ReportNameConst = "ReportName";
		private const string ReportTypeParmName = "ReportType";
		private const string SealNum = "SealNum";
		private const string ShipmentRcvReportType = "ShipmentReceiveReportType";
		private const string Shipper = "Shipper";
		private const string Telephone = "Telephone";
		private const string To = "To";
		private const string TransIDName = "TransID";
		private const string TranControlNum = "TranControlNum";
		private const string ViewName = "ViewName";
		private const string IsDWReportConst = "IsDWReport";

		private string aliasList;
		private bool fromStation;
		private bool isPopupDisplay;
		private Guid loginSiteGuid;
		private string rebateNumbers;
		private string reportName;
		private ReportTypesClass.ReportTypes reportType;
		private string sessionID;
		private string shipmentReceiveReportType;
		private Guid siteGuid;
		private string siteName;
		private string tdrCarrierName;
		private string tdrCarrierRepCont;
		private string tdrCarrierRepTel;
		private string tdrConsignor;
		private decimal tdrCostOfRepair;
		private DateTimeOffset tdrDateCarrNotified;
		private string tdrEmailAdd;
		private string tdrFascmileNum;
		private string tdrNameOfPreparer;
		private string tdrSealNum;
		private string tdrShipper;
		private string tdrTelephone;
		private string tdrTo;
		private string tdrTranControlNum;
		private string transID;
		private Guid userGuid;
		private string viewName;
		private bool isDWReport;

		#endregion

		#region Constructors and Destructors

		public RequestParser(ManageSecurity manageSecurity)
		{
			this.Initialize(manageSecurity);
		}

		#endregion

		#region Public Properties

		/// <summary>
		///    This property will return True if the report is going to a popup
		///    dialog.
		/// </summary>
		public bool IsPopupDisplay => this.isPopupDisplay;

		public ReportTypesClass.ReportTypes ReportType => this.reportType;

		/// <summary>
		///    This property will return the report name.
		/// </summary>
		public string ReportName => this.reportName;

		public bool IsDWReport => this.isDWReport;

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///    This method parses the request and returns a hash table of report parameters.
		///    If the parsing failed, then a null is returned.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public Hashtable ParseRequest(HttpRequest request)
		{
			this.SetReportType(request);
			this.SetReportName(request);
			this.SetIsDWReport(request);
			this.DeterminePopupRequest(request);
			Hashtable reportParms = null;

			if (this.ValidateRequest())
			{
				reportParms = new Hashtable();

				// Add the FuelsManager version number parameter
				Assembly assembly = Assembly.GetExecutingAssembly();
				FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
				reportParms.Add("VersionNumber", fvi.FileVersion);

				switch (this.reportType)
				{
					case ReportTypesClass.ReportTypes.AVIATION_RPT:
					case ReportTypesClass.ReportTypes.OIL_GAS_RPT:
						reportParms.Add("Site", this.siteName);
						reportParms.Add("SiteGuid", this.siteGuid.ToString());
						reportParms.Add("SiteGuidStr", this.siteGuid.ToString());
						reportParms.Add("LoginSiteGuid", this.loginSiteGuid.ToString());
						reportParms.Add("LoginSiteGuidStr", this.loginSiteGuid.ToString());
						reportParms.Add("UserGuid", this.userGuid.ToString());
						reportParms.Add("UserGuidStr", this.userGuid.ToString());
						reportParms.Add("SecurityToken", this.sessionID);

						if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey()))
						{
							// WI 4539 FM-613 JS add report path for every report
							reportParms.Add("ReportPath", this.reportName);
						}
						break;
					case ReportTypesClass.ReportTypes.QUERY_RPT:
						this.SetViewName(request);
						this.SetAliasList(request);
						reportParms.Add("LoginSiteGuid", this.loginSiteGuid.ToString());
						reportParms.Add("LoginSiteGuidStr", this.loginSiteGuid.ToString());
						reportParms.Add("SiteGuid", this.siteGuid.ToString());
						reportParms.Add("SiteGuidStr", this.siteGuid.ToString());
						reportParms.Add("UserGuid", this.userGuid.ToString());
						reportParms.Add("UserGuidStr", this.userGuid.ToString());
						reportParms.Add("ViewName", this.viewName);
						reportParms.Add("AliasList", this.aliasList);

						if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey()))
						{
							// WI 4539 FM-613 JS add report path for every report
							reportParms.Add("ReportPath", this.reportName);
						}
						break;
					case ReportTypesClass.ReportTypes.BOL_RPT:
						this.SetTransID(request);
						this.SetFromStation(request);
						reportParms.Add("SiteGuid", this.siteGuid.ToString());
						reportParms.Add("SiteGuidStr", this.siteGuid.ToString());
						reportParms.Add("TransID", this.transID);
						reportParms.Add("FromStation", this.fromStation);
						break;
					case ReportTypesClass.ReportTypes.SECURE_RPT:
						reportParms.Add("SessionID", this.sessionID);
						break;
					case ReportTypesClass.ReportTypes.FESS_RPT:
						this.SetRebateNumbers(request);
						reportParms.Add("SiteGuid", this.siteGuid.ToString());
						reportParms.Add("SiteGuidStr", this.siteGuid.ToString());
						reportParms.Add("LoginSiteGuid", this.loginSiteGuid.ToString());
						reportParms.Add("LoginSiteGuidStr", this.loginSiteGuid.ToString());
						reportParms.Add("UserGuid", this.userGuid.ToString());
						reportParms.Add("UserGuidStr", this.userGuid.ToString());
						reportParms.Add("RebateNumbers", this.rebateNumbers);
						break;
					case ReportTypesClass.ReportTypes.DOD_SHIPMENT_RCV_RPT:
						this.SetTransID(request);
						this.SetShipmentReceiveReportType(request);
						reportParms.Add("SiteGuid", this.siteGuid.ToString());
						reportParms.Add("SiteGuidStr", this.siteGuid.ToString());
						reportParms.Add("LoginSiteGuid", this.loginSiteGuid.ToString());
						reportParms.Add("LoginSiteGuidStr", this.loginSiteGuid.ToString());
						reportParms.Add("TransID", this.transID);
						reportParms.Add("Reporttype", this.shipmentReceiveReportType);
						break;
					case ReportTypesClass.ReportTypes.DOD_EOM_RPT:
						this.SetTransID(request);
						reportParms.Add("TransID", this.transID);
						reportParms.Add("SiteGuid", this.siteGuid.ToString());
						reportParms.Add("SiteGuidStr", this.siteGuid.ToString());
						reportParms.Add("UserGuid", this.userGuid.ToString());
						reportParms.Add("UserGuidStr", this.userGuid.ToString());
						reportParms.Add("LoginSiteGuid", this.loginSiteGuid.ToString());
						reportParms.Add("LoginSiteGuidStr", this.loginSiteGuid.ToString());
						break;
					case ReportTypesClass.ReportTypes.DOD_TDR_RPT:
						this.SetTransID(request);
						this.SetTo(request);
						this.SetConsignor(request);
						this.SetCarrierName(request);
						this.SetCarrierRepCont(request);
						this.SetSealNum(request);
						this.SetTranControlNum(request);
						this.SetNameOfPreparer(request);
						this.SetTelephone(request);
						this.SetShipper(request);
						this.SetDateCarrNotified(request);
						this.SetCarrierRepTel(request);
						this.SetCostOfRepair(request);
						this.SetEmailAdd(request);
						this.SetFascmileNum(request);
						reportParms.Add("TransID", this.transID);
						reportParms.Add("SiteGuid", this.siteGuid.ToString());
						reportParms.Add("SiteGuidStr", this.siteGuid.ToString());
						reportParms.Add("UserGuid", this.userGuid.ToString());
						reportParms.Add("UserGuidStr", this.userGuid.ToString());
						reportParms.Add("LoginSiteGuid", this.loginSiteGuid.ToString());
						reportParms.Add("LoginSiteGuidStr", this.loginSiteGuid.ToString());
						reportParms.Add("To", this.tdrTo);
						reportParms.Add("Consignor", this.tdrConsignor);
						reportParms.Add("CarrierName", this.tdrCarrierName);
						reportParms.Add("CarrierRepCont", this.tdrCarrierRepCont);
						reportParms.Add("SealNum", this.tdrSealNum);
						reportParms.Add("TranControlNum", this.tdrTranControlNum);
						reportParms.Add("NameofPreparer", this.tdrNameOfPreparer);
						reportParms.Add("Telephone", this.tdrTelephone);
						reportParms.Add("Shipper", this.tdrShipper);
						reportParms.Add("DateCarrNotified", this.tdrDateCarrNotified);
						reportParms.Add("CarrierRepTel", this.tdrCarrierRepTel);
						reportParms.Add("CostofRepair", this.tdrCostOfRepair);
						reportParms.Add("EmailAdd", this.tdrEmailAdd);
						reportParms.Add("FascmileNum", this.tdrFascmileNum);
						break;
					case ReportTypesClass.ReportTypes.VARIABLE_PARAMETERS:
						foreach (string paramName in request.Params.AllKeys)
						{
							if (paramName.Equals("StartDate") == false)
							{
								reportParms.Add(paramName, request.GetQueryOrFormValue(paramName));
							}
						}
						break;
					case ReportTypesClass.ReportTypes.OVRDUE_TST_RPRT:
						foreach (var paramName in request.Params.AllKeys)
						{
							reportParms.Add(paramName, request.GetQueryOrFormValue(paramName));
						}
						break;
					case ReportTypesClass.ReportTypes.ADF_BULK_RPT:
						this.SetTransID(request);
						reportParms.Add("SiteGuid", this.siteGuid.ToString());
						reportParms.Add("SiteGuidStr", this.siteGuid.ToString());
						reportParms.Add("BulkPaymentID", request.GetQueryOrFormValue("BulkPaymentID"));
						// WI 4539 FM-613 JS add report path for every report
						reportParms.Add("ReportPath", this.reportName);
						break;
				}
			}

			return reportParms;
		}

		#endregion

		#region Methods

		/// <summary>
		///    This method will determine if the report is to be displayed in a popup
		///    dialog. If so, the request will contain "PopupDisplay=True" entry and the
		///    flag will be set to true.
		/// </summary>
		/// <param name="request"></param>
		private void DeterminePopupRequest(HttpRequest request)
		{
			this.isPopupDisplay = false;
			string temp = request.GetQueryOrFormValue(PopupDisplay);

			if (!string.IsNullOrEmpty(temp))
			{
				if (temp.ToUpper().Equals("TRUE"))
				{
					this.isPopupDisplay = true;
				}
			}
		}

		/// <summary>
		///    This method initializes this object to its initial state.
		/// </summary>
		/// <param name="manageSecurity"></param>
		private void Initialize(ManageSecurity manageSecurity)
		{
			this.userGuid = Guids.UninitializedUserGuid;
			this.siteGuid = Guids.UninitializedSiteGuid;
			this.loginSiteGuid = Guids.UninitializedLoginSiteGuid;

			this.siteName = null;
			this.reportType = ReportTypesClass.ReportTypes.NONE_RPT;
			this.reportName = "";
			this.sessionID = "";
			this.isPopupDisplay = false;

			if (manageSecurity != null)
			{
				this.userGuid = manageSecurity.UserGuid;
				this.siteGuid = manageSecurity.SiteGuid;
				this.loginSiteGuid = manageSecurity.LoginSiteGuid;
				this.sessionID = manageSecurity.Security.Token.ToString();

				// JS20100809 WI-16639 will need to use the site name with proper case because SSRS will in some
				// cases complain about missing "Site" value if case do not match
				SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(manageSecurity.Security, this.siteGuid, false, false, false)
																);
				this.siteName = site.ID;
			}
		}

		/// <summary>
		///    This method will retrieve the alias list from the request and set the
		///    member alias list. This is used for query type reports.
		/// </summary>
		/// <param name="request"></param>
		private void SetAliasList(HttpRequest request)
		{
			string temp = request.GetQueryOrFormValue(AliasList);

			if (!string.IsNullOrEmpty(temp))
			{
				this.aliasList = temp;
			}
			else
			{
				this.aliasList = "";
			}
		}

		private void SetCarrierName(HttpRequest request)
		{
			string temp = request.GetQueryOrFormValue(CarrierName);

			if (string.IsNullOrEmpty(temp))
			{
				this.tdrCarrierName = "";
			}
			else
			{
				this.tdrCarrierName = temp;
			}
		}

		private void SetCarrierRepCont(HttpRequest request)
		{
			string temp = request.GetQueryOrFormValue(CarrierRepCont);

			if (string.IsNullOrEmpty(temp))
			{
				this.tdrCarrierRepCont = "";
			}
			else
			{
				this.tdrCarrierRepCont = temp;
			}
		}

		private void SetCarrierRepTel(HttpRequest request)
		{
			string temp = request.GetQueryOrFormValue(CarrierRepTel);

			if (string.IsNullOrEmpty(temp))
			{
				this.tdrCarrierRepTel = "";
			}
			else
			{
				this.tdrCarrierRepTel = temp;
			}
		}

		private void SetConsignor(HttpRequest request)
		{
			string temp = request.GetQueryOrFormValue(Consignor);

			if (string.IsNullOrEmpty(temp))
			{
				this.tdrConsignor = "";
			}
			else
			{
				this.tdrConsignor = temp;
			}
		}

		private void SetCostOfRepair(HttpRequest request)
		{
			string temp = request.GetQueryOrFormValue(CostOfRepair);

			if (string.IsNullOrEmpty(temp))
			{
				this.tdrCostOfRepair = 0.0M;
			}
			else
			{
				try
				{
					this.tdrCostOfRepair = Decimal.Parse(temp);
				}
				catch (FormatException)
				{
					this.tdrCostOfRepair = 0.0M;
				}
			}
		}

		private void SetDateCarrNotified(HttpRequest request)
		{
			string temp = request.GetQueryOrFormValue(DateCarrNotified);

			if (string.IsNullOrEmpty(temp))
			{
				this.tdrDateCarrNotified = TimeConverter.Today();
			}
			else
			{
				try
				{
					this.tdrDateCarrNotified = DateTimeOffset.Parse(temp);
				}
				catch (FormatException)
				{
					this.tdrDateCarrNotified = TimeConverter.Today();
				}
			}
		}

		private void SetEmailAdd(HttpRequest request)
		{
			string temp = request.GetQueryOrFormValue(EmailAdd);

			if (string.IsNullOrEmpty(temp))
			{
				this.tdrEmailAdd = "";
			}
			else
			{
				this.tdrEmailAdd = temp;
			}
		}

		private void SetFascmileNum(HttpRequest request)
		{
			string temp = request.GetQueryOrFormValue(FacsmileNum);

			if (string.IsNullOrEmpty(temp))
			{
				this.tdrFascmileNum = "";
			}
			else
			{
				this.tdrFascmileNum = temp;
			}
		}

		private void SetFromStation(HttpRequest request)
		{
			string temp = request.GetQueryOrFormValue(FromStation);

			if (temp != null)
			{
				this.fromStation = Convert.ToBoolean(temp);
			}
			else
			{
				this.fromStation = false;
			}
		}

		private void SetNameOfPreparer(HttpRequest request)
		{
			string temp = request.GetQueryOrFormValue(NameOfPreparer);

			if (string.IsNullOrEmpty(temp))
			{
				this.tdrNameOfPreparer = "";
			}
			else
			{
				this.tdrNameOfPreparer = temp;
			}
		}

		/// <summary>
		///    This method will retrieve the rebate numbers from the request and set the
		///    member rebate numbers. This is used for FESS type reports.
		/// </summary>
		/// <param name="request"></param>
		private void SetRebateNumbers(HttpRequest request)
		{
			string temp = request.GetQueryOrFormValue(RebateNumbers);

			if (!string.IsNullOrEmpty(temp))
			{
				this.rebateNumbers = temp;
			}
			else
			{
				this.rebateNumbers = "";
			}
		}

		/// <summary>
		///    This method will retrieve the report name from the request and set the
		///    member report name.
		/// </summary>
		/// <param name="request"></param>
		private void SetReportName(HttpRequest request)
		{
			string temp = request.GetQueryOrFormValue(ReportNameConst);

			if (!string.IsNullOrEmpty(temp))
			{
				this.reportName = temp;
			}
			else
			{
				this.reportName = "";
			}
		}


		/// <summary>
		///    This method will retrieve the IsDWReport flag from the request and set the
		///    member IsDWReport.
		/// </summary>
		/// <param name="request"></param>
		private void SetIsDWReport(HttpRequest request)
		{
			string temp = request.GetQueryOrFormValue(IsDWReportConst);
			this.isDWReport = false;

			if (!string.IsNullOrEmpty(temp))
			{
				bool flag = false;
				if (Boolean.TryParse(temp, out flag))
					this.isDWReport = flag;
			}
			else
			{
				this.isDWReport = false;
			}
		}


		/// <summary>
		///    This method will set the report type to either aviation, oil & gas,
		///    query, or none. None is the failure.
		/// </summary>
		/// <param name="request"></param>
		private void SetReportType(HttpRequest request)
		{
			string temp = request.GetQueryOrFormValue(ReportTypeParmName);

			if (!string.IsNullOrEmpty(temp))
			{
				try
				{
					int type = Convert.ToInt32(temp);

					this.reportType = (ReportTypesClass.ReportTypes)type;
				}
				catch (FormatException)
				{
					this.reportType = ReportTypesClass.ReportTypes.NONE_RPT;
				}
			}
			else
			{
				this.reportType = ReportTypesClass.ReportTypes.NONE_RPT;
			}
		}

		private void SetSealNum(HttpRequest request)
		{
			string temp = request.GetQueryOrFormValue(SealNum);

			if (string.IsNullOrEmpty(temp))
			{
				this.tdrSealNum = "";
			}
			else
			{
				this.tdrSealNum = temp;
			}
		}

		/// <summary>
		///    This method will retrieve the shipment/receive report type from the request and set the
		///    data member. This is used for DoD Shipment/Receive type reports.
		/// </summary>
		/// <param name="request"></param>
		private void SetShipmentReceiveReportType(HttpRequest request)
		{
			string temp = request.GetQueryOrFormValue(ShipmentRcvReportType);

			if (!string.IsNullOrEmpty(temp))
			{
				this.shipmentReceiveReportType = temp;
			}
			else
			{
				this.shipmentReceiveReportType = "DD FORM 1348-7";
			}
		}

		private void SetShipper(HttpRequest request)
		{
			string temp = request.GetQueryOrFormValue(Shipper);

			if (string.IsNullOrEmpty(temp))
			{
				this.tdrShipper = "";
			}
			else
			{
				this.tdrShipper = temp;
			}
		}

		private void SetTelephone(HttpRequest request)
		{
			string temp = request.GetQueryOrFormValue(Telephone);

			if (string.IsNullOrEmpty(temp))
			{
				this.tdrTelephone = "";
			}
			else
			{
				this.tdrTelephone = temp;
			}
		}

		private void SetTo(HttpRequest request)
		{
			string temp = request.GetQueryOrFormValue(To);

			if (string.IsNullOrEmpty(temp))
			{
				this.tdrTo = "";
			}
			else
			{
				this.tdrTo = temp;
			}
		}

		private void SetTranControlNum(HttpRequest request)
		{
			string temp = request.GetQueryOrFormValue(TranControlNum);

			if (string.IsNullOrEmpty(temp))
			{
				this.tdrTranControlNum = "";
			}
			else
			{
				this.tdrTranControlNum = temp;
			}
		}

		/// <summary>
		///    This method will retrieve the transaction ID from the request and set the
		///    member transaction ID. This is used for BOL type reports.
		/// </summary>
		/// <param name="request"></param>
		private void SetTransID(HttpRequest request)
		{
			string temp = request.GetQueryOrFormValue(TransIDName);

			if (!string.IsNullOrEmpty(temp))
			{
				this.transID = temp;
			}
			else
			{
				this.transID = "";
			}
		}

		/// <summary>
		///    This method will retrieve the view name from the request and set the
		///    member view name. This is used for query type reports.
		/// </summary>
		/// <param name="request"></param>
		private void SetViewName(HttpRequest request)
		{
			string temp = request.GetQueryOrFormValue(ViewName);

			if (!string.IsNullOrEmpty(temp))
			{
				this.viewName = temp;
			}
			else
			{
				this.viewName = "";
			}
		}

		/// <summary>
		///    This method ensures that the request was parsed successfully.
		///    It will return true on success.  Otherwise, it returns false.
		/// </summary>
		/// <returns></returns>
		private bool ValidateRequest()
		{
			if (this.userGuid == Guids.UninitializedUserGuid)
			{
				return false;
			}

			if (this.siteGuid == Guids.UninitializedSiteGuid)
			{
				return false;
			}

			if (this.loginSiteGuid == Guids.UninitializedLoginSiteGuid)
			{
				return false;
			}

			if (string.IsNullOrEmpty(this.siteName))
			{
				return false;
			}

			if (string.IsNullOrEmpty(this.reportName))
			{
				return false;
			}

			if (string.IsNullOrEmpty(this.sessionID))
			{
				return false;
			}

			if (this.reportType == ReportTypesClass.ReportTypes.NONE_RPT)
			{
				return false;
			}

			return true;
		}

		#endregion
	}
}