namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ReportSvr2005;
	using FMBusinessObjects.UtilityObjects;

	using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.InventoryManagement.ViewModels;

	using System;
	using System.Diagnostics;
	using System.Globalization;
	using System.Linq;
	using System.Web;
	using System.Web.Mvc;
	using System.Web.UI;

	public class PointLeakAnalysisController : FMBaseControllerEx
	{
		// GET: InventoryManagement/PointLeakAnalysis
		[HttpPost]
		public ActionResult PointLeakAnalysisView(string pointIdString, string pointGuidString)
		{
			var model = new PointLeakAnalysisModel();
			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));
			model.Format = new NumberFormatInfo
			{
				NumberGroupSizes = site.GetNumberGroupSizes(),
				NumberGroupSeparator = site.NumberGroupSeparator,
				NumberDecimalSeparator = site.NumberDecimalSeparator,
			};


			model.selectedBasePoint = pointIdString;
			model.selectedBasePointGuid = new Guid(pointGuidString);
			model.siteGuid = site.SiteGuid;
			model.siteId = site.ID;

			var propertyGuid = FMChannelHelper.MakeCall<IPointProperties, Guid>(x => x.GetPointPropertyGuid(this.Security, model.selectedBasePointGuid, "Leak Detection Settings"));
			PointProperty pointProperty = FMChannelHelper.MakeCall<IPointProperties, PointProperty>(x => x.Get(this.Security, propertyGuid));
			if (pointProperty != null)
			{
				model.leakDetectionSettings = pointProperty.Value as LeakDetectionSettings;
			}
			GaugeTypeClass gaugeType = FMChannelHelper.MakeCall<IGaugeTypes, GaugeTypeClass>(x => x.GetByID(this.Security, model.leakDetectionSettings.GaugeType));
			model.gaugeTypeName = gaugeType?.Name ?? "";

			model.volumeDecimalPlaces = 0;
			model.temperatureDecimalPlaces = 1;
			model.flowDecimalPlaces = 2;

			model.LeakReportName = site.LeakDetectionReport;

            TimeZoneInfo sitesTimezone = TimeZoneInfo.FindSystemTimeZoneById(site.TimeZone);
            var currentTime = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, sitesTimezone);

         currentTime = currentTime.AddSeconds(-currentTime.Second);
			var minTestMinutes = site.LeakDetectionMinQuietTime;
            var minGaugeTestHours = (ushort)gaugeType.MinHours.GetValueOrDefault();
			if (minGaugeTestHours > 0)
			{
				minTestMinutes = minGaugeTestHours * 60;
			}
			model.startTime = currentTime.AddMinutes(-minTestMinutes);
			model.endTime = currentTime;
			model.dateTimeFormat = site.ShortDatePattern + " " + site.TimePattern;

			int number;

			if (int.TryParse(site.VolumeDecimalPlaces, out number))
			{
				model.volumeDecimalPlaces = number;
			}
			if (int.TryParse(site.TemperatureDecimalPlaces, out number))
			{
				model.temperatureDecimalPlaces = number;
			}
			if (int.TryParse(site.FlowDecimalPlaces, out number))
			{
				model.flowDecimalPlaces = number;
			}

			return this.PartialViewWithErrorMessages("PointLeakAnalysisView", model);
		}



        [HttpPost]
        public ActionResult PointLeakAnalysisRun(string pointIdString, string pointGuidString, string startTimeString, string endTimeString)
        {
			LeakAnalysisResult leakAnalysisResult= new LeakAnalysisResult();

			try
            {
                var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));
                EventLog eventLog = new EventLog("Application", ".", "FuelsManager"); ;

                string dateTimeFormat = site.ShortDatePattern + " " + site.TimePattern;

                DateTimeOffset startTime;
                if (!DateTimeOffset.TryParseExact(startTimeString, dateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime))
                {
                    throw new Exception("Start Time has invalid format");
                }
                TimeZoneInfo sitesTimezone = TimeZoneInfo.FindSystemTimeZoneById(site.TimeZone);
                double systemTimezoneOffset = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.Now).TotalMinutes;
                double timezoneOffset = sitesTimezone.GetUtcOffset(DateTimeOffset.Now).TotalMinutes;

                // Use the difference in offset between the site time and system time to get the server timezone
                startTime = startTime.AddMinutes(-(timezoneOffset - systemTimezoneOffset));

                DateTimeOffset endTime;
                if (!DateTimeOffset.TryParseExact(endTimeString, dateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out endTime))
                {
                    throw new Exception("End Time has invalid format");
                }
                // Use the difference in offset between the site time and system time to get the server timezone
                endTime = endTime.AddMinutes(-(timezoneOffset - systemTimezoneOffset));

                if (startTime >= endTime)
                {
                    throw new Exception("Start Time must before End Time");
                }

                Point point = FMChannelHelper.MakeCall<IPoints, Point>(x => x.Get(this.Security, new Guid(pointGuidString)));
                if (point == null)
                {
                    throw new Exception("Point not found " + pointGuidString);
                }

                LeakDetectionSettings settings;
                var propertyGuid = FMChannelHelper.MakeCall<IPointProperties, Guid>(x => x.GetPointPropertyGuid(this.Security, point.IdentityGuid, "Leak Detection Settings"));
                PointProperty pointProperty = FMChannelHelper.MakeCall<IPointProperties, PointProperty>(x => x.Get(this.Security, propertyGuid));
                settings = pointProperty != null
                    ? pointProperty.Value as LeakDetectionSettings
                    : throw new Exception("Setting not found for point ID:" + pointIdString);

                GaugeTypeClass gaugeType = FMChannelHelper.MakeCall<IGaugeTypes, GaugeTypeClass>(x => x.GetByID(this.Security, settings.GaugeType));
                if (gaugeType == null)
                {
                    throw new Exception($"Gauge Type {settings.GaugeType} not found.");
                }

                leakAnalysisResult = new LeakAnalysisResult
                {
                    StartTime = startTime,
                    StopTime = endTime,
                    GaugeType = gaugeType.IdentityGuid,
                    CertRate = gaugeType.CertificationLeakRate.GetValueOrDefault(),
                    LeakThreshold = gaugeType.Threshold.GetValueOrDefault(),
                    MaxTemperature = 200,
                    MinTemperature = -25,
                    DeltaTemperature = gaugeType.DeltaTemp.GetValueOrDefault(),
                    MinValue = point.Tags.Values.FirstOrDefault(u => u.WellKnownIdentityGuid == Guids.VolumeNetStandardGuid).GetMinimum(point),
                    MaxValue = point.Tags.Values.FirstOrDefault(u => u.WellKnownIdentityGuid == Guids.VolumeNetStandardGuid).GetMaximum(point),
                    MinimumFillPercentage = settings.MinimumFillPercentage,
                    MinGaugeTestTime = (ushort)gaugeType.MinHours.GetValueOrDefault()
                };


                LeakDetectionError leakError = FMChannelHelper.MakeCall<ILeakTests, LeakDetectionError>(x => x.Run(this.Security, point, settings.AnalysisType, settings.AnalysisMethod, startTime, endTime, ref leakAnalysisResult));

                if (
                    (leakError & LeakDetectionError.InvalidIndex) == LeakDetectionError.InvalidIndex ||
                    (leakError & LeakDetectionError.ArchiveAccessError) == LeakDetectionError.ArchiveAccessError ||
                    (leakError & LeakDetectionError.SqlError) == LeakDetectionError.SqlError ||
                    (leakError & LeakDetectionError.NotEnoughMemory) == LeakDetectionError.NotEnoughMemory ||
                    (leakError & LeakDetectionError.ConnectionFailed) == LeakDetectionError.ConnectionFailed
                    )
                {
                    // We have errors for system
                    OnError("Error calculating leak rate: " + LeakAnalysisResult.GetDispalyMessgae(leakError));
                }
                var availabeReports = ReportService.GetReportsList(Security, site);
                leakAnalysisResult.EnableReportPrint = availabeReports.Contains(site.LeakDetectionReport);
			}
            catch (Exception ex)
            {
                leakAnalysisResult.AnalysisStatus = LeakDetectionError.TestFailed;
                OnError(ex);
            }
            return this.JsonWithErrorMessages(leakAnalysisResult);
        }



        /// <summary>
        ///	This method handles the Print Preview button being pressed 
        /// </summary>
        [HttpPost]
		public ActionResult PointLeakAnalysisPrintPreview(string leakReportGuid)
		{
			//	string bolRptType = ((int)ReportTypesClass.ReportTypes.BOL_RPT).ToString(CultureInfo.InvariantCulture);

			//	//string rptURL = "../FMReporting/ReportLandingPage.aspx?ReportType=" + bolRptType;
			//	string rptUrl = "../FMReportWebMain/PopupReportLandingPage.aspx?ReportType=" + bolRptType;

			//	string reportName = this.TransContext.aliasClass.AssociatedReport.Replace(" ", "+");
			//	rptUrl = rptUrl + "&ReportName=" + reportName;
			//	rptUrl = rptUrl + "&SiteGuidStr=" + this.Trans.SiteGuid;
			//	rptUrl = rptUrl + "&TransID=" + this.Trans.TransID;
			//	rptUrl += "&" + this.security.CSRFTokenWithParamName;

			//	string javascriptPopupReport = "<script type='text/javascript'>\n<!-- \n" + "window.open('" + rptUrl + "', "
			//											+ "'Reports', "
			//											+ "'status=0, toolbar=0, menubar=1, resizable=1, scrollbars=1, height=950, width=850'"
			//											+ "); \n" + "-->\n</script>";

			//	this.Response.Cookies.Add(new HttpCookie("Token", this.Session["Token"] as string));
			//	ScriptManager.RegisterClientScriptBlock(
			//		this.Page, this.GetType(), "RPT_POPUP_NEW_BROWSER", javascriptPopupReport, false);
			return this.JsonWithErrorMessages(true);
		}


		[HttpPost]
		public ActionResult ReportCleanUp(string leakReportIdString)
        {
			Guid LeakReportId;
			if (Guid.TryParse(leakReportIdString, out LeakReportId))
            {
				FMChannelHelper.MakeCall<ILeakTests, bool>(x => x.CleanupLeakReportData(this.Security, LeakReportId));
			}
			return this.JsonWithErrorMessages(true);
		}
	}
}