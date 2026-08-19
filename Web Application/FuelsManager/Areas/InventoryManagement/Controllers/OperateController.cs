namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using AjaxControlToolkit.HTMLEditor.ToolbarButton;
	using Areas.Controllers;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.DataObjects.CodedVariables;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;
	using Microsoft.Ajax.Utilities;
	using Opc.Ua;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Drawing.Printing;
	using System.Globalization;
	using System.Linq;
	using System.Security.Policy;
	using System.ServiceModel;
	using System.Text.RegularExpressions;
	using System.Web.Mvc;
	using System.Web.Script.Serialization;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	using ViewModels;

	[SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
	public class OperateController : FMBaseControllerEx
	{
		protected const string OperatorScreenID = "Operator";
		protected const string OperatorMonitorCountID = "OperatorMonitorCount";
		protected const string OperatorAdditionalScreensEnabledID = "OperatorScreensEnabled";
		protected const int MaxOperateScreenSettingsToScan = 32;

		[HttpGet]
		public ActionResult OperateIndex(string id)
		{

			var model = new OperateModel();
			try
			{
				// Verify that we don't have too many operate sessions open
				int isEnterprise;
				int currentOperateSessionCount;
				int maxOperateSessionCount;
				string maxOperateSessionCountKey;

				try
				{
					model.OperateTagRefreshInterval = Convert.ToInt32(FMChannelHelper.MakeCall<IConfigurationSettings, string>(x => x.GetKeyValueByKey(this.Security, "OperateTagRefreshInterval")));
				}
				catch (Exception ex) when (ex is OverflowException || ex is FormatException)
				{
					_ = ex;
				}

				try
				{
					model.OperateAlarmRefreshInterval = Convert.ToInt32(FMChannelHelper.MakeCall<IConfigurationSettings, string>(x => x.GetKeyValueByKey(this.Security, "OperateAlarmRefreshInterval")));
				}
				catch (Exception ex) when (ex is OverflowException || ex is FormatException)
				{
					_ = ex;
				}

				try
				{
					isEnterprise = Convert.ToInt32(FMChannelHelper.MakeCall<IConfigurationSettings, string>(x => x.GetKeyValueByKey(this.Security, "IsEnterprise")));
				}
				catch (Exception ex) when (ex is OverflowException || ex is FormatException)
				{
					_ = ex;
					isEnterprise = 0;
				}

				try
				{
					isEnterprise = Convert.ToInt32(FMChannelHelper.MakeCall<IConfigurationSettings, string>(x => x.GetKeyValueByKey(this.Security, "IsEnterprise")));
				}
				catch (Exception ex) when (ex is OverflowException || ex is FormatException)
				{
					_ = ex;
					isEnterprise = 0;
				}

				maxOperateSessionCountKey = isEnterprise > 0 ? "MaximumOperateSessions_Enterprise" : "MaximumOperateSessions_Terminal";
				try
				{
					maxOperateSessionCount = Convert.ToInt32(FMChannelHelper.MakeCall<IConfigurationSettings, string>(x => x.GetKeyValueByKey(this.Security, maxOperateSessionCountKey)));
				}
				catch (Exception ex) when (ex is OverflowException || ex is FormatException)
				{
					_ = ex;
					maxOperateSessionCount = 2;
				}

				currentOperateSessionCount = FMChannelHelper.MakeCall<ISessions, int>(x => x.GetCountActiveOperateScreens(this.Security));

				if (maxOperateSessionCount <= currentOperateSessionCount)
				{
					return this.View("OperateUnavailable", model);
				}
				var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

				model.Format = new NumberFormatInfo
				{
					NumberGroupSizes = site.GetNumberGroupSizes(),
					NumberGroupSeparator = site.NumberGroupSeparator,
					NumberDecimalSeparator = site.NumberDecimalSeparator,
				};

				// check if we want to open the alarm summary when opening the screen (its opened from the alarm bell in the menu)
				if (id == "alarmsummary")
				{
					model.OpenAlarmSummary = true;
				}
				model.DisplayCUIDataMark = Global.IsFdsIM || AppSettingsHelper.GetKeyValue<bool>("DisplayCUIDataMark", false);
				model.ShortDatePattern = site.ShortDatePattern;
				model.TimePattern = site.TimePattern;
				model.TimeZone = site.TimeZone;
            TimeZoneInfo sitesTimezone = TimeZoneInfo.FindSystemTimeZoneById(site.TimeZone);
            double timezoneOffset = sitesTimezone.GetUtcOffset(DateTimeOffset.Now).TotalMinutes;
				DateTime currentSiteTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, sitesTimezone);
				model.TimeZoneOffset = timezoneOffset;
            model.DatepickerTimezoneString = String.Format("{0:D4}", (sitesTimezone.GetUtcOffset(DateTimeOffset.Now).Hours * 100) + sitesTimezone.GetUtcOffset(DateTimeOffset.Now).Minutes); //must be formatted "-0500" for jquery datepicker
            if (timezoneOffset >= 0)
            {
                model.DatepickerTimezoneString = model.DatepickerTimezoneString.PadLeft(5, '+'); // + sign needed for UTC +1 or more
            }
            model.SiteGuid = this.Security.SiteGuid;
				model.UserGuid = this.Security.UserGuid;
				model.IsTabGroupEnabled = site.OperateTabGroups;
				model.DateTimeFormatInfo = site.GetDateTimeFormatInfo();
				model.MaxOperateTabsAllowed = site.MaxOperateTabsAllowed;

				string pointgroupreportgeneration = this.Session["pointgroupreportgeneration"] as string;
				if (!string.IsNullOrEmpty(pointgroupreportgeneration))
				{
					model.pointgroupreportgeneration = pointgroupreportgeneration;
				}

				string viewOperateOnlystring = this.Session["ViewOperateOnly"] as string;
				model.IsOperateViewOnlyMode = !string.IsNullOrEmpty(viewOperateOnlystring);

				var snfi = new FMNumberFormatInfo
				{
					NegativeSign = "-",
					NumberDecimalDigits = 0,
					NumberDecimalSeparator = site.NumberDecimalSeparator,
					NumberGroupSeparator = site.NumberGroupSeparator,
					NumberGroupSizes = site.GetNumberGroupSizes()[0],
					NumberNegativePattern = 1,
					ShortDatePattern = site.ShortDatePattern
				};

				model.SiteNumFormatInfo = snfi;

				model.ViewPointsRight				= this.Security.HasRight(RIGHT.OPERATE_VIEW_POINTS);
				model.ViewGraphicsRight				= this.Security.HasRight(RIGHT.OPERATE_VIEW_GRAPHICS);
                model.ViewPointHistoryRight			= this.Security.HasRight(RIGHT.OPERATE_VIEW_POINT_HISTORY);
                model.ViewPointGroupsRight			= this.Security.HasRight(RIGHT.OPERATE_VIEW_POINT_GROUPS) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_POINT_GROUP);
                model.ModifyPointGroupsRight		= this.Security.HasRight(RIGHT.OPERATE_MODIFY_POINT_GROUPS) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_POINT_GROUP);
				model.CreatePublicPointGroupsRight	= this.Security.HasRight(RIGHT.OPERATE_CREATE_PUBLIC_POINT_GROUPS);
				model.ModifyPublicPointGroupsRight	= this.Security.HasRight(RIGHT.OPERATE_MODIFY_PUBLIC_POINT_GROUPS) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_POINT_GROUP);
				model.CreateSharedPointGroupsRight	= this.Security.HasRight(RIGHT.OPERATE_CREATE_SHARED_POINT_GROUPS);
				model.ModifySharedPointGroupsRight	= this.Security.HasRight(RIGHT.OPERATE_MODIFY_SHARED_POINT_GROUPS) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_POINT_GROUP);
                model.PointCalculatorRight = this.Security.HasRight(RIGHT.OPERATE_USE_POINT_CALCULATOR);

                bool isLeakDetectionKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsLeakDetectionKey());

				model.LeakAnalysisRight				= this.Security.HasRight(RIGHT.OPERATE_PERFORM_LEAK_DETECTION) && isLeakDetectionKey;
				model.ViewTrendsRight				= this.Security.HasRight(RIGHT.OPERATE_VIEW_TRENDS);
				model.ViewIMReportsRight			= this.Security.HasRight(RIGHT.OPERATE_VIEW_IM_REPORTS);
				model.ModifyTrendsRight				= this.Security.HasRight(RIGHT.OPERATE_MODIFY_TRENDS);
				model.ViewAlarmHistoryRight			= this.Security.HasRight(RIGHT.OPERATE_VIEW_ALARM_HISTORY);
				model.ViewAlarmSummaryRight			= this.Security.HasRight(RIGHT.OPERATE_VIEW_ALARM_SUMMARY);

				bool isMovementKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsMovementKey());

				model.ViewMovementSummaryRight			= (this.Security.HasRight(RIGHT.OPERATE_VIEW_MOVEMENT_SUMMARY) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_MOVEMENT_SUMMARY)) && isMovementKey;
				model.ModifyMovementSummaryRight		= (this.Security.HasRight(RIGHT.OPERATE_MODIFY_MOVEMENT_SUMMARY) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_MOVEMENT_SUMMARY)) && isMovementKey;
				model.CreatePublicMovementSummaryRight	= this.Security.HasRight(RIGHT.OPERATE_CREATE_PUBLIC_MOVEMENT_SUMMARY) && isMovementKey;
				model.ModifyPublicMovementSummaryRight	= (this.Security.HasRight(RIGHT.OPERATE_MODIFY_PUBLIC_MOVEMENT_SUMMARY) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_MOVEMENT_SUMMARY)) && isMovementKey;
				model.CreateSharedMovementSummaryRight	= this.Security.HasRight(RIGHT.OPERATE_CREATE_SHARED_MOVEMENT_SUMMARY) && isMovementKey;
				model.ModifySharedMovementSummaryRight	= (this.Security.HasRight(RIGHT.OPERATE_MODIFY_SHARED_MOVEMENT_SUMMARY) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_MOVEMENT_SUMMARY)) && isMovementKey;

				model.ModifyMovementHistoryRight	= this.Security.HasRight(RIGHT.OPERATE_MODIFY_MOVEMENT_HISTORY) && isMovementKey;
				model.ViewMovementHistoryRight		= this.Security.HasRight(RIGHT.OPERATE_VIEW_MOVEMENT_HISTORY) && isMovementKey;

				// only check the hardware key if they have permissions
				if (model.ViewIMReportsRight == true)
				{
					ushort word1Value = FMChannelHelper.MakeCall<IHardwareKey, ushort>(x => x.GetWord1ValueLIN());

					if ((word1Value & 0x04) != 0x04)
					{
						model.ViewIMReportsRight = false;
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			return this.View(model);
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult GetTrendArchiveData(List<Guid> tagGuids, DateTimeOffset start, DateTimeOffset end)
		{
			try
			{
				var trendArchiveData = FMChannelHelper.MakeCall<IPointTagArchive, List<List<TrendArchiveDataElement>>>(x => x.GetTrendArchiveData(this.Security, tagGuids, start, end));

				return this.JsonWithErrorMessages(trendArchiveData);

			}
			catch (Exception e)
			{
				if (!Global.IsFdsIM)
            {
                this.OnError(e);
            }
				else
				{
					 // redact the IP address and port, if there is one in e.message
                Regex ip = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}:\d{1,5}\b");
                Match result = ip.Match(e.Message);
					 if (result.Success)
					 {
						  Exception modifiedException = new Exception(e.Message.Replace(result.ToString(), string.Empty));
						  this.OnError(modifiedException);
					 }
					 else
					 {
						  this.OnError(e);
					 }
            }
				return this.JsonWithErrorMessages(null);
			}
		}


		[HttpPost]
		public ActionResult UpdateTags(List<Guid> tagGuids, string siteTimeZone)
		{
			try
			{
				var pointTags = FMChannelHelper.MakeCall<IPointServiceManager, List<PointTag>>(x => x.GetPointTagData(this.Security, tagGuids));

				if (pointTags == null)
				{
					pointTags = new List<PointTag>(tagGuids.Count);
					foreach (var tagGuid in tagGuids)
					{
						pointTags.Add(new PointTag() { PointTagGuid = tagGuid });
					}
				}
				// we don't need to return all the data, only the fields we will use
				var pointTagList = from p in pointTags select new { p.Value, p.ValueTypeString, p.PointTagGuid, p.ID, p.ServerTimeStamp, p.Units, p.DecimalPlaces, p.Maximum, p.Minimum, p.QualityAbbreviation, p.EngineeringUnitsType, p.Acknowledged, p.AlarmPriorityGuid, p.AlarmState, p.WellKnownIdentityGuid };

				return this.JsonWithErrorMessages(pointTagList);
			}
			catch (Exception e)
			{
				this.OnError(e);
				return this.JsonWithErrorMessages(null);
			}
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult UpdateTagsForDynamicGroup(PointGroupFilterRules filter, List<string> tagList, string siteTimeZone)
		{
			try
			{
				var points = FMChannelHelper.MakeCall<IPoints, PointCollection>(x => x.EnumerateBySiteFiltered(this.Security, this.Security.SiteGuid, filter, tagList));

				// we need to create a list of tags to request the values
				var returnPointNameList = new List<PointValue>();
				var pointValueIdentifierPropertiesList = new List<PointValueIdentifier>();
				var includeProductName = (tagList.FirstOrDefault(stringToCheck => stringToCheck.Contains("ProductID")) != null);
				var includeProductDescription = (tagList.FirstOrDefault(stringToCheck => stringToCheck.Contains("ProductDescription")) != null);

				foreach (var point in points)
				{
					// add the point name as a tag since it's display in the grid
					var pointValue = new PointValue
					{
						PointValueIdentifier = new PointValueIdentifier(point.PointGuid, PointValueType.Point, "point"),
						PointGuid = point.PointGuid,
						ID = "point",
						Value = point.ID,
						ValueTypeString = "System.String"
					};
					returnPointNameList.Add(pointValue);

					if (includeProductName)
					{
						pointValueIdentifierPropertiesList.Add(new PointValueIdentifier(point.PointGuid, PointValueType.Point, "ProductID"));
					}

					if (includeProductDescription)
					{
						pointValueIdentifierPropertiesList.Add(new PointValueIdentifier(point.PointGuid, PointValueType.Point, "ProductDescription"));
					}
				}

				// get the list of tag values
				var pointValueIdentifierList = points.SelectMany(point => point.Tags).Select(x => new PointValueIdentifier(x.Value.PointTagGuid, PointValueType.Tag, null, x.Value.WellKnownIdentityGuid)).ToList();

				var allPointValueIdentifiers = pointValueIdentifierPropertiesList.Union(pointValueIdentifierList).ToList();

				var pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, allPointValueIdentifiers));

				if (pointValues == null)
				{
					pointValues = new List<PointValue>(allPointValueIdentifiers.Count);
					foreach (var pointValueIdentifier in allPointValueIdentifiers)
					{
						pointValues.Add(new PointValue() { PointValueIdentifier = pointValueIdentifier });
					}
				}
				else
				{
					foreach (var pointValue in pointValues)
					{
						if (pointValue.Value != null)
						{

							if (pointValue.ValueTypeString.IndexOf("FMBusinessObjects.DataObjects.CodedVariables") != -1)
							{
								pointValue.Value = this.GetTranslatedText(FMBusinessObjects.DataObjects.CodedVariables.SelectList.CreateUIString((Enum)pointValue.Value));
							}

							else if (pointValue.ValueTypeString == "System.Boolean")
							{
								if (pointValue.Value is bool)
								{
									pointValue.Value = ((bool)pointValue.Value) ? this.GetTranslatedText("True") : this.GetTranslatedText("False");
								}
							}

							else if (pointValue.ValueTypeString == "System.DateTimeOffset")
							{
								var siteTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(siteTimeZone);
								pointValue.Value = TimeZoneInfo.ConvertTime((DateTimeOffset)pointValue.Value, siteTimeZoneInfo);
							}

							else if ((pointValue.Value is double || pointValue.Value is float) && Double.IsNaN(Convert.ToDouble(pointValue.Value)))
							{
								pointValue.Value = "NaN";
							}
						}
					}
				}

				// merge the list of tag values with the point names
				var returnList = returnPointNameList.Union(pointValues);
				bool CommunicationsFailure = false;
				var pointTagList = from p in returnList select new { PointValueIdentifier_IdentityGuid = p.PointValueIdentifier.IdentityGuid, PointValueIdentifier_PointValueType = p.PointValueIdentifier.PointValueType, PointValueIdentifier_PropertyID = p.PointValueIdentifier.PropertyID, PointValueIdentifier_UtcTicks = 0, p.PointGuid, p.Value, p.ValueTypeString, PointTagGuid = p.PointValueIdentifier.IdentityGuid, ID = (p.PointValueIdentifier.PointValueType == PointValueType.Point ? p.PointValueIdentifier.PropertyID : p.ID), p.ServerTimeStamp, p.Units, p.DecimalPlaces, p.Maximum, p.Minimum, p.QualityAbbreviation, p.EngineeringUnitsType, p.Acknowledged, p.AlarmPriorityGuid, p.AlarmState, p.WellKnownIdentityGuid, p.Access, p.InputOutputType, p.InhibitOverride, p.Status, CommunicationsFailure };

				return this.JsonWithErrorMessages(pointTagList, JsonRequestBehavior.AllowGet);

			}
			catch (Exception e)
			{
				this.OnError(e);
				return this.JsonWithErrorMessages(null);
			}
		}

		public void FormatValue(PointValue pointValue, string siteTimeZone)
		{
			if (pointValue.Value != null)
			{

				if (pointValue.ValueTypeString.IndexOf("FMBusinessObjects.DataObjects.CodedVariables") != -1)
				{
					if(pointValue.ValueTypeString == "FMBusinessObjects.DataObjects.CodedVariables.MovementType")
					{ 
                        pointValue.Value = FMBusinessObjects.DataObjects.CodedVariables.SelectList.CreateUIString((Enum)pointValue.Value);
					}
					else
					{ 
						pointValue.Value = this.GetTranslatedText(FMBusinessObjects.DataObjects.CodedVariables.SelectList.CreateUIString((Enum)pointValue.Value));
					}
                }
                else if (pointValue.ValueTypeString == "System.Boolean")
				{
					pointValue.Value = ((bool)pointValue.Value) ? this.GetTranslatedText("True") : this.GetTranslatedText("False");
				}
				else if (pointValue.ValueTypeString == "System.DateTimeOffset")
				{
					var siteTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(siteTimeZone);
					pointValue.Value = TimeZoneInfo.ConvertTime((DateTimeOffset)pointValue.Value, siteTimeZoneInfo);
				}
				else if ((pointValue.Value is double || pointValue.Value is float) && Double.IsNaN(Convert.ToDouble(pointValue.Value)))
				{
					pointValue.Value = "NaN";
				}
			}
		}


		[HttpPost]
		public ActionResult UpdateValues(List<PointValueIdentifier> pointValueIdentifiers, string siteTimeZone)
		{
			try
			{
				// we don't need to return all the data, only the fields we will use
				List<OperatePointValue> pointValueList = new List<OperatePointValue>(pointValueIdentifiers.Count);

				var pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueDataChanges(this.Security, pointValueIdentifiers));

				if (pointValues == null)
				{
					foreach (var pointValueIdentifier in pointValueIdentifiers)
					{
						var pointValue = new PointValue() { PointValueIdentifier = pointValueIdentifier };
						var pv = new OperatePointValue(pointValue);
						pointValueList.Add(pv);
					}
				}
				else
				{
					foreach (var pointValue in pointValues)
					{
						if (pointValue.PointValueIdentifier.IdentityGuid == SystemDataPointVirtualPoint.TagSiteDataGuid
						|| pointValue.PointValueIdentifier.IdentityGuid == SystemDataPointVirtualPoint.TagUserDataGuid
						|| pointValue.PointValueIdentifier.IdentityGuid == SystemDataPointVirtualPoint.TagDateTimeDataGuid
						|| pointValue.PointValueIdentifier.IdentityGuid == SystemDataPointVirtualPoint.TaglicenseExpiryDataGuid)
						{
							continue;
						}


						if (pointValue.Value != null)
						{
							if (pointValue.Value is List<PointValue>)
							{
								foreach (var value in (pointValue.Value as List<PointValue>))
								{
									if(value == null)
									{
										continue;
									}

									this.FormatValue(value, siteTimeZone);
								}
							}
							else
							{
								this.FormatValue(pointValue, siteTimeZone);
							}
						}

						var pv = new OperatePointValue(pointValue);
						pointValueList.Add(pv);
					}

					// First three or four are the Virtual Points
					for(var pointValueIdentifierIndex = 0; pointValueIdentifierIndex < 4; pointValueIdentifierIndex++)
					{
						if(pointValueIdentifierIndex >= pointValueIdentifiers.Count)
						{
							break;
						}

						if(pointValueIdentifiers[pointValueIdentifierIndex].IdentityGuid == SystemDataPointVirtualPoint.TagSiteDataGuid
						|| pointValueIdentifiers[pointValueIdentifierIndex].IdentityGuid == SystemDataPointVirtualPoint.TagUserDataGuid
						|| pointValueIdentifiers[pointValueIdentifierIndex].IdentityGuid == SystemDataPointVirtualPoint.TagDateTimeDataGuid
						|| pointValueIdentifiers[pointValueIdentifierIndex].IdentityGuid == SystemDataPointVirtualPoint.TaglicenseExpiryDataGuid)
						{
							var pointValue = new PointValue() { PointValueIdentifier = pointValueIdentifiers[pointValueIdentifierIndex] };
							this.UpdateVirtualSystemDataPointPointValue(pointValue, pointValue.PointValueIdentifier);
							var pv = new OperatePointValue(pointValue);
							pointValueList.Add(pv);
						}
					}
				}

				return this.JsonWithErrorMessages(pointValueList);
			}
			catch (Exception e)
			{
				this.OnError(e);
				return this.JsonWithErrorMessages(null);
			}
		}

		/// <summary>
		/// This method handles populating the System Data Point virtual point value.
		/// </summary>
		/// <param name="pointValue">The point value to be updated.</param>
		/// <param name="pointValueIdentifier">The point value identifies associated to the point value.</param>
		private void UpdateVirtualSystemDataPointPointValue(PointValue pointValue, PointValueIdentifier pointValueIdentifier)
		{
			if (pointValueIdentifier.IdentityGuid == SystemDataPointVirtualPoint.TagSiteDataGuid)
			{
				pointValue.PointGuid = SystemDataPointVirtualPoint.PointGuid;
				pointValue.PointID = SystemDataPointVirtualPoint.PointId;
				pointValue.PointTemplateTagGuid = SystemDataPointVirtualPoint.TagSiteDataGuid;
				pointValue.PointValueIdentifier = pointValueIdentifier;
				pointValue.ID = SystemDataPointVirtualPoint.TagSiteDataId;
				pointValue.ValueTypeString = "System.String";
				pointValue.Value = this.Security.SiteID;
			}

			if (pointValueIdentifier.IdentityGuid == SystemDataPointVirtualPoint.TagUserDataGuid)
			{
				pointValue.PointGuid = SystemDataPointVirtualPoint.PointGuid;
				pointValue.PointID = SystemDataPointVirtualPoint.PointId;
				pointValue.PointTemplateTagGuid = SystemDataPointVirtualPoint.TagUserDataGuid;
				pointValue.PointValueIdentifier = pointValueIdentifier;
				pointValue.ID = SystemDataPointVirtualPoint.TagUserDataId;
				pointValue.ValueTypeString = "System.String";
				pointValue.Value = this.Security.UserID;
			}

			if (pointValueIdentifier.IdentityGuid == SystemDataPointVirtualPoint.TagDateTimeDataGuid)
			{
				string dateTimeStr = this.GetSiteFormattedDateTime();

				pointValue.PointGuid = SystemDataPointVirtualPoint.PointGuid;
				pointValue.PointID = SystemDataPointVirtualPoint.PointId;
				pointValue.PointTemplateTagGuid = SystemDataPointVirtualPoint.TagDateTimeDataGuid;
				pointValue.PointValueIdentifier = pointValueIdentifier;
				pointValue.ID = SystemDataPointVirtualPoint.TagDateTimeDataId;
				pointValue.ValueTypeString = "System.String";
				pointValue.Value = dateTimeStr;
			}


			if (pointValueIdentifier.IdentityGuid == SystemDataPointVirtualPoint.TaglicenseExpiryDataGuid)
			{
					string licenseExpiryMessageInfo = this.GetLicenseExpiryMessageInfo();

					pointValue.PointGuid = SystemDataPointVirtualPoint.PointGuid;
					pointValue.PointID = SystemDataPointVirtualPoint.PointId;
					pointValue.PointTemplateTagGuid = SystemDataPointVirtualPoint.TaglicenseExpiryDataGuid;
					pointValue.PointValueIdentifier = pointValueIdentifier;
					pointValue.ID = SystemDataPointVirtualPoint.TagLicenseExpiryDataId;
					pointValue.ValueTypeString = "System.String";
					pointValue.Value = licenseExpiryMessageInfo;
			}
		}

		/// <summary>
		/// This method shall return the Site formatted date time as a string.
		/// </summary>
		/// <returns>Returns the site's formatted date time as a string.</returns>
		private string GetSiteFormattedDateTime()
		{
			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(Security, Security.SiteGuid, false, false, false));
			var siteTimeConverter = new SiteTimeConverter(site);


			DateTime siteDateTime = siteTimeConverter.Now().DateTime;
			var siteDateTimeStr = siteDateTime.ToString(site.GetDateTimeFormatInfo());

			return siteDateTimeStr;
		}

        /// <summary>
        /// This method shall return the Site formatted date time as a string.
        /// </summary>
        /// <returns>Returns the site's formatted date time as a string.</returns>
        private string GetLicenseExpiryMessageInfo()
        {
            if (Session["LicenseExpiryFullScreenEnabled"] == null)
            {
                this.Session["LicenseExpiryFullScreenEnabled"] = true;
                string value = FMChannelHelper.MakeCall<IConfigurationSettings, string>(s => s.GetKeyValueByKey(Session["Security"] as SecurityClass, "LicenseExpiryFullScreenEnabled"));
                bool enabled = true;
                if (Boolean.TryParse(value, out enabled))
                {
                    this.Session["LicenseExpiryFullScreenEnabled"] = enabled;
                }
            }

            string licenseStatusText;
            string licenseStatusStyle;
            FMHelpers.GetLicenseStatusInfo( this.Security, Session["LicenseDaysLeftToExpire"], Session["LicenseExpirationDate"], out licenseStatusText, out licenseStatusStyle);
            return string.Concat(licenseStatusText,"|", licenseStatusStyle, "|", this.Session["LicenseExpiryFullScreenEnabled"]);
        }

        /// <summary>
        /// This method is called by the UI to retrieve product graphic information that is associated to Points.
        /// </summary>
        /// <returns>Returns a list of point values that contains the point and associated product graphic info.</returns>
        [HttpPost]
		public ActionResult RetrieveProductGraphicInfo()
		{
			try
			{
				var pointValueList = new List<PointValue>();

				// Retreive product graphic info that is associated to points.
				var pointProductGraphicInfoList =
						FMChannelHelper.MakeCall<IPoints, List<Tuple<Guid, string, string, int>>>(x => x.EnumeratePointProductGraphicInfo(this.Security));

				if (pointProductGraphicInfoList != null && pointProductGraphicInfoList.Count > 0)
				{
					foreach (var pointProductGraphicRecord in pointProductGraphicInfoList)
					{
						Guid pointGuid = pointProductGraphicRecord.Item1;
						string productColor = pointProductGraphicRecord.Item2;
						string patternColor = pointProductGraphicRecord.Item3;
						int patternNumber = pointProductGraphicRecord.Item4;

						if (string.IsNullOrEmpty(productColor) == false && string.IsNullOrEmpty(patternColor) == false)
						{
							var pointValue = new PointValue
							{
								PointGuid = pointGuid,
								ProductColor = productColor,
								PatternColor = patternColor,
								PatternNumber = patternNumber,
								HasProductGraphicInfo = true
							};

							pointValueList.Add(pointValue);
						}
					}
				}

				return this.JsonWithErrorMessages(pointValueList);
			}
			catch (Exception e)
			{
				this.OnError(e);
				return this.JsonWithErrorMessages(null);
			}
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GetAlarmPriorities()
		{

			try
			{
				var alarmPriorityCollection = FMChannelHelper.MakeCall<IAlarmPriorities, AlarmPriorityCollectionClass>(alarmPriorities => alarmPriorities.Enumerate(this.Security));
				// we don't need to return all fields, only the ones we will use
				var alarmPriorityReduced = from p in alarmPriorityCollection select new { p.IdentityGuid, p.BackgroundSteady, p.BackgroundAlternate, p.TextSteady, p.TextAlternate };
				return this.JsonWithErrorMessages(alarmPriorityReduced, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				this.OnError(e);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GetTankStatusColors()	//bds
		{
			try
			{
				var tankstatusColorsCollection = FMChannelHelper.MakeCall<ITankStatusColors, TankStatusColorsCollectionClass>(tankstatusColors => tankstatusColors.Enumerate(this.Security));
				return this.JsonWithErrorMessages(tankstatusColorsCollection, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				this.OnError(e);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public ActionResult GetScreenConfiguration(string windowName, int? monitorCount)
		{
			try
			{
				windowName = NormalizeOperateWindowName(windowName);

				string screenConfiguration = "[]";
				var userSettings = FMChannelHelper.MakeCall<IUserViewStateSettings, UserViewStateSettingCollection>(x => x.EnumerateBySiteUserClientIpAddressWindowNameAndViewID(this.Security, this.Security.SiteGuid, this.Security.UserGuid, windowName, OperatorScreenID));
				if (userSettings != null && userSettings.Count > 0)
				{
					var userSetting = userSettings[0];
					screenConfiguration = (string)userSetting.Value;
				}
				return this.JsonWithErrorMessages(screenConfiguration, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				this.OnError(e);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

		}

		[HttpPost]
		public ActionResult SetScreenConfiguration(string configuration, string windowName)
		{
			try
			{
				windowName = NormalizeOperateWindowName(windowName);
				if (configuration.IsNullOrWhiteSpace())
				{
					configuration = "[]";
				}

				var userSettings = FMChannelHelper.MakeCall<IUserViewStateSettings, UserViewStateSettingCollection>(x => x.EnumerateBySiteUserClientIpAddressWindowNameAndViewID(this.Security, this.Security.SiteGuid, this.Security.UserGuid, windowName, OperatorScreenID));

				if (userSettings == null || userSettings.Count <= 0)
				{
					var userSetting = new UserViewStateSetting(this.Security) { Value = configuration, WindowName = windowName, ViewID = OperatorScreenID };

					FMChannelHelper.MakeCall<IUserViewStateSettings>(x => x.Add(this.Security, userSetting));
				}
				else
				{
					var userSetting = userSettings[0];
					userSetting.Value = configuration;
					userSetting.WindowName = windowName;
					FMChannelHelper.MakeCall<IUserViewStateSettings>(x => x.Modify(this.Security, userSetting));
				}
				return this.JsonWithErrorMessages(null);
			}
			catch (Exception e)
			{
				this.OnError(e);
				return this.JsonWithErrorMessages(null);
			}
		}

		[HttpGet]
		public ActionResult GetOperateMonitorState()
		{
			try
			{
				var savedScreenMask = this.GetOperateScreenConfigurationMaskSetting();
				var screenMask = NormalizeOperateScreenMask(savedScreenMask ?? this.GetDefaultOperateScreenConfigurationMask());
				var savedMonitorCount = this.GetOperateMonitorCountSetting();
				return this.JsonWithErrorMessages(
					new
					{
						MonitorCount = savedMonitorCount,
						OpenAdditionalScreens = HasAdditionalAssignedScreens(screenMask),
						ScreenMask = screenMask,
						HasOperateScreenConfiguration = savedScreenMask.HasValue,
						MaxConfiguredMonitorIndex = this.GetMaxConfiguredOperateScreenIndex()
					},
					JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				this.OnError(e);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost]
		public ActionResult SetOperateMonitorState(int monitorCount, bool openAdditionalScreens, bool mergeUnavailableScreens = false, long? screenMask = null)
		{
			try
			{
				if (monitorCount < 1)
				{
					monitorCount = 1;
				}

				if (mergeUnavailableScreens)
				{
					this.MergeUnavailableOperateScreenConfigurations(monitorCount);
				}

				var operateScreenMask = NormalizeOperateScreenMask(screenMask ?? GetScreenMaskFromMonitorState(monitorCount, openAdditionalScreens));
				this.SetOperateScreenConfigurationMask(operateScreenMask);
				this.SetOperateViewStateSetting(OperatorMonitorCountID, monitorCount);
				this.SetOperateViewStateSetting(OperatorAdditionalScreensEnabledID, HasAdditionalAssignedScreens(operateScreenMask));

				return this.JsonWithErrorMessages(null);
			}
			catch (Exception e)
			{
				this.OnError(e);
				return this.JsonWithErrorMessages(null);
			}
		}

		private int? GetOperateMonitorCountSetting()
		{
			var userSetting = this.GetOperateViewStateSetting(OperatorMonitorCountID);
			if (userSetting == null || userSetting.Value == null)
			{
				return null;
			}

			int monitorCount;
			if (int.TryParse(Convert.ToString(userSetting.Value, CultureInfo.InvariantCulture), NumberStyles.Integer, CultureInfo.InvariantCulture, out monitorCount))
			{
				return monitorCount;
			}

			return null;
		}

		private bool? GetOperateAdditionalScreensEnabledSetting()
		{
			var userSetting = this.GetOperateViewStateSetting(OperatorAdditionalScreensEnabledID);
			if (userSetting == null || userSetting.Value == null)
			{
				return null;
			}

			bool openAdditionalScreens;
			if (bool.TryParse(Convert.ToString(userSetting.Value, CultureInfo.InvariantCulture), out openAdditionalScreens))
			{
				return openAdditionalScreens;
			}

			int openAdditionalScreensAsInt;
			if (int.TryParse(Convert.ToString(userSetting.Value, CultureInfo.InvariantCulture), NumberStyles.Integer, CultureInfo.InvariantCulture, out openAdditionalScreensAsInt))
			{
				return openAdditionalScreensAsInt != 0;
			}

			return null;
		}

		private UserViewStateSetting GetOperateViewStateSetting(string viewID)
		{
			var userSettings = FMChannelHelper.MakeCall<IUserViewStateSettings, UserViewStateSettingCollection>(x => x.EnumerateBySiteUserClientIpAddressWindowNameAndViewID(this.Security, this.Security.SiteGuid, this.Security.UserGuid, string.Empty, viewID));
			if (userSettings == null || userSettings.Count <= 0)
			{
				return null;
			}

			return userSettings[0];
		}

		private void SetOperateViewStateSetting(string viewID, object value)
		{
			var userSetting = this.GetOperateViewStateSetting(viewID);
			if (userSetting == null)
			{
				userSetting = new UserViewStateSetting(this.Security) { Value = value, WindowName = string.Empty, ViewID = viewID };
				FMChannelHelper.MakeCall<IUserViewStateSettings>(x => x.Add(this.Security, userSetting));
				return;
			}

			userSetting.Value = value;
			userSetting.WindowName = string.Empty;
			userSetting.ViewID = viewID;
			FMChannelHelper.MakeCall<IUserViewStateSettings>(x => x.Modify(this.Security, userSetting));
		}

		private long? GetOperateScreenConfigurationMaskSetting()
		{
			var configuration = FMChannelHelper.MakeCall<IOperateScreenConfigurations, OperateScreenConfiguration>(
				x => x.GetBySiteUserClientIpAddress(this.Security, this.Security.SiteGuid, this.Security.UserGuid, this.Security.ClientIpAddress ?? string.Empty));
			return configuration == null ? (long?)null : configuration.ScreenMask;
		}

		private void SetOperateScreenConfigurationMask(long screenMask)
		{
			screenMask = NormalizeOperateScreenMask(screenMask);
			FMChannelHelper.MakeCall<IOperateScreenConfigurations>(
				x => x.SetScreenMask(this.Security, this.Security.SiteGuid, this.Security.UserGuid, this.Security.ClientIpAddress ?? string.Empty, screenMask));
		}

		private long GetDefaultOperateScreenConfigurationMask()
		{
			var maxConfiguredMonitorIndex = this.GetMaxConfiguredOperateScreenIndex();
			var savedMonitorCount = this.GetOperateMonitorCountSetting();
			var savedOpenAdditionalScreens = this.GetOperateAdditionalScreensEnabledSetting();
			var monitorCount = Math.Max(maxConfiguredMonitorIndex, savedMonitorCount ?? 1);
			if (savedOpenAdditionalScreens == false && maxConfiguredMonitorIndex <= 1)
			{
				monitorCount = 1;
			}

			return GetScreenMaskForMonitorCount(monitorCount);
		}

		private static long GetScreenMaskFromMonitorState(int monitorCount, bool openAdditionalScreens)
		{
			return openAdditionalScreens ? GetScreenMaskForMonitorCount(monitorCount) : 1L;
		}

		private static long GetScreenMaskForMonitorCount(int monitorCount)
		{
			var screenMask = 0L;
			for (var screenIndex = 1; screenIndex <= Math.Min(MaxOperateScreenSettingsToScan, Math.Max(1, monitorCount)); screenIndex++)
			{
				screenMask += 1L << (screenIndex - 1);
			}

			return NormalizeOperateScreenMask(screenMask);
		}

		private static long NormalizeOperateScreenMask(long screenMask)
		{
			return (screenMask < 1L ? 1L : screenMask) | 1L;
		}

		private static int GetHighestAssignedScreenNumber(long screenMask)
		{
			screenMask = NormalizeOperateScreenMask(screenMask);
			var highestScreenNumber = 1;
			for (var screenIndex = 1; screenIndex <= MaxOperateScreenSettingsToScan; screenIndex++)
			{
				if ((screenMask & (1L << (screenIndex - 1))) != 0)
				{
					highestScreenNumber = screenIndex;
				}
			}

			return highestScreenNumber;
		}

		private static bool HasAdditionalAssignedScreens(long screenMask)
		{
			return (NormalizeOperateScreenMask(screenMask) & ~1L) != 0;
		}

		private void MergeUnavailableOperateScreenConfigurations(int currentMonitorCount)
		{
			var unavailableScreenSettings = this.GetUnavailableOperateScreenConfigurationSettings(currentMonitorCount);

			var screen1Settings = this.GetOperateScreenConfigurationSettings("Screen1");
			var screen1Setting = screen1Settings == null || screen1Settings.Count <= 0 ? null : screen1Settings[0];
			var mergedControls = DeserializeOperateScreenConfiguration(screen1Setting == null ? "[]" : Convert.ToString(screen1Setting.Value, CultureInfo.InvariantCulture));
			var settingsToPurge = new List<UserViewStateSetting>();

			foreach (var unavailableScreenSetting in unavailableScreenSettings)
			{
				var unavailableControls = DeserializeOperateScreenConfiguration(Convert.ToString(unavailableScreenSetting.Value, CultureInfo.InvariantCulture));
				foreach (var unavailableControl in unavailableControls)
				{
					ClearOperateControlActiveState(unavailableControl);
					mergedControls.Add(unavailableControl);
				}

				settingsToPurge.Add(unavailableScreenSetting);
			}

			if (settingsToPurge.Count <= 0)
			{
				return;
			}

			this.SetOperateScreenConfigurationSetting("Screen1", SerializeOperateScreenConfiguration(mergedControls), screen1Setting);

			foreach (var settingToPurge in settingsToPurge)
			{
				FMChannelHelper.MakeCall<IUserViewStateSettings>(x => x.Purge(this.Security, settingToPurge.UserViewStateSettingGuid));
			}
		}

		private void ReconcileUnavailableOperateScreenConfigurations(int currentMonitorCount)
		{
			if (currentMonitorCount < 1)
			{
				currentMonitorCount = 1;
			}

			var savedMonitorCount = this.GetOperateMonitorCountSetting();
			var maxConfiguredMonitorIndex = this.GetMaxConfiguredOperateScreenIndex();
			if ((!savedMonitorCount.HasValue || savedMonitorCount.Value <= currentMonitorCount) && maxConfiguredMonitorIndex <= currentMonitorCount)
			{
				return;
			}

			this.MergeUnavailableOperateScreenConfigurations(currentMonitorCount);
			this.SetOperateViewStateSetting(OperatorMonitorCountID, currentMonitorCount);
			if (currentMonitorCount == 1)
			{
				this.SetOperateViewStateSetting(OperatorAdditionalScreensEnabledID, false);
			}
		}

		private int GetMaxConfiguredOperateScreenIndex()
		{
			var maxConfiguredMonitorIndex = 1;
			for (var screenIndex = 1; screenIndex <= MaxOperateScreenSettingsToScan; screenIndex++)
			{
				var settings = this.GetOperateScreenConfigurationSettings("Screen" + screenIndex);
				if (settings != null && settings.Count > 0)
				{
					maxConfiguredMonitorIndex = Math.Max(maxConfiguredMonitorIndex, screenIndex);
				}
			}

			return maxConfiguredMonitorIndex;
		}

		private List<UserViewStateSetting> GetUnavailableOperateScreenConfigurationSettings(int currentMonitorCount)
		{
			var unavailableScreenSettings = new List<UserViewStateSetting>();
			for (var screenIndex = currentMonitorCount + 1; screenIndex <= MaxOperateScreenSettingsToScan; screenIndex++)
			{
				var settings = this.GetOperateScreenConfigurationSettings("Screen" + screenIndex);
				if (settings != null && settings.Count > 0)
				{
					unavailableScreenSettings.AddRange(settings.Cast<UserViewStateSetting>());
				}
			}

			return unavailableScreenSettings;
		}

		private UserViewStateSettingCollection GetOperateScreenConfigurationSettings(string windowName)
		{
			windowName = NormalizeOperateWindowName(windowName);
			return FMChannelHelper.MakeCall<IUserViewStateSettings, UserViewStateSettingCollection>(x => x.EnumerateBySiteUserClientIpAddressWindowNameAndViewID(this.Security, this.Security.SiteGuid, this.Security.UserGuid, windowName, OperatorScreenID));
		}

		private void SetOperateScreenConfigurationSetting(string windowName, string configuration, UserViewStateSetting existingSetting)
		{
			windowName = NormalizeOperateWindowName(windowName);
			if (existingSetting == null)
			{
				var userSetting = new UserViewStateSetting(this.Security) { Value = configuration, WindowName = windowName, ViewID = OperatorScreenID };
				FMChannelHelper.MakeCall<IUserViewStateSettings>(x => x.Add(this.Security, userSetting));
				return;
			}

			existingSetting.Value = configuration;
			existingSetting.WindowName = windowName;
			existingSetting.ViewID = OperatorScreenID;
			FMChannelHelper.MakeCall<IUserViewStateSettings>(x => x.Modify(this.Security, existingSetting));
		}

		private static string NormalizeOperateWindowName(string windowName)
		{
			if (windowName.IsNullOrWhiteSpace())
			{
				return "Screen1";
			}

			var screenMatch = Regex.Match(windowName.Trim(), @"^Screen(?<screenIndex>\d+)$", RegexOptions.IgnoreCase);
			int screenIndex;
			if (screenMatch.Success
				&& int.TryParse(screenMatch.Groups["screenIndex"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out screenIndex)
				&& screenIndex > 0)
			{
				return "Screen" + screenIndex.ToString(CultureInfo.InvariantCulture);
			}

			return "Screen1";
		}

		private static List<object> DeserializeOperateScreenConfiguration(string configuration)
		{
			if (configuration.IsNullOrWhiteSpace())
			{
				return new List<object>();
			}

			var serializer = CreateOperateScreenConfigurationSerializer();
			return serializer.Deserialize<List<object>>(configuration) ?? new List<object>();
		}

		private static string SerializeOperateScreenConfiguration(List<object> controls)
		{
			var serializer = CreateOperateScreenConfigurationSerializer();
			return serializer.Serialize(controls ?? new List<object>());
		}

		private static JavaScriptSerializer CreateOperateScreenConfigurationSerializer()
		{
			return new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
		}

		private static void ClearOperateControlActiveState(object control)
		{
			var dictionary = control as IDictionary<string, object>;
			if (dictionary != null)
			{
				if (dictionary.ContainsKey("active"))
				{
					dictionary["active"] = false;
				}

				foreach (var value in dictionary.Values)
				{
					ClearOperateControlActiveState(value);
				}

				return;
			}

			var enumerable = control as IEnumerable;
			if (enumerable == null || control is string)
			{
				return;
			}

			foreach (var value in enumerable)
			{
				ClearOperateControlActiveState(value);
			}
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public PartialViewResult GetListOfGraphicsPartialView()
		{
			try
			{
				var model = new DrawingSummaryModel
				{
					Names = FMChannelHelper.MakeCall<IDrawings, List<DrawingName>>(
													x => x.EnumerateAvailableDrawingNamesByPublished(this.Security))
				};
				return this.PartialView("GraphicMenuSelection", model);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				throw;
			}
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public PartialViewResult GetListOfPointGroupPartialView()
		{
			try
			{
				var pointGroupList = new PointGroupCollection();
				pointGroupList = FMChannelHelper.MakeCall<IPointGroups, PointGroupCollection>(x => x.EnumerateByUserSite(this.Security, this.Security.UserGuid, this.Security.SiteGuid));
				var ownerDisplayNames = new Dictionary<Guid, string>();

				var model = new PointGroupsFilterModel();
				foreach (var pointGroup in pointGroupList)
				{
					model.pointGroups.Add(new PointGroupModel
					{
						PointGroupGuid = pointGroup.PointGroupGuid,
						ID = pointGroup.ID,
						Description = pointGroup.Description,
						PointGroupType = pointGroup.PointGroupType,
						OwnerUserGuid = pointGroup.OwnerUserGuid,
						Owner = this.GetSavedViewOwnerDisplayName(pointGroup.OwnerUserGuid, ownerDisplayNames, false),
						IsEditable = (pointGroup.OwnerUserGuid == this.Security.UserGuid || pointGroup.PointGroupType == PointGroup.PointGroupVisibilityType.Public || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_POINT_GROUP)),
						ViewPointGroupsRight = this.Security.HasRight(RIGHT.OPERATE_VIEW_POINT_GROUPS) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_POINT_GROUP),
                  ModifyPointGroupsRight = this.Security.HasRight(RIGHT.OPERATE_MODIFY_POINT_GROUPS) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_POINT_GROUP),
                  AdministerPointGroupsRight = this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_POINT_GROUP),
                  CreatePublicPointGroupsRight = this.Security.HasRight(RIGHT.OPERATE_CREATE_PUBLIC_POINT_GROUPS),
						ModifyPublicPointGroupsRight = this.Security.HasRight(RIGHT.OPERATE_MODIFY_PUBLIC_POINT_GROUPS) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_POINT_GROUP),
						CreateSharedPointGroupsRight = this.Security.HasRight(RIGHT.OPERATE_CREATE_SHARED_POINT_GROUPS),
						ModifySharedPointGroupsRight = this.Security.HasRight(RIGHT.OPERATE_MODIFY_SHARED_POINT_GROUPS) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_POINT_GROUP)
               });
				}
				model.OwnerOptions = BuildSavedViewOwnerOptions(
					model.pointGroups.Select(x => new KeyValuePair<Guid, string>(x.OwnerUserGuid, x.Owner)),
					false);
            model.AdministerPointGroupsRight = this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_POINT_GROUP);
            model.ModifyPointGroupsRight = this.Security.HasRight(RIGHT.OPERATE_MODIFY_POINT_GROUPS);
				return this.PartialView("PointGroupSelection", model);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				throw;
			}
		}

		/// <summary>
		/// This method handles the ajax call to retrieve the list of movement summary menu items.
		/// </summary>
		/// <returns>Return the movement summary selection partial view object.</returns>
		[HttpGet, ValidateJsonAntiForgeryToken]
		public PartialViewResult GetListOfMovementSummaryPartialView()
		{
			try
			{
				var model = new MovementSummaryFilterModel();
				var movementSummaryList = FMChannelHelper.MakeCall<IMovementSummaries, MovementSummaryCollection>(
													x => x.EnumerateByUserSite(this.Security, this.Security.UserGuid, this.Security.SiteGuid));
				var ownerDisplayNames = new Dictionary<Guid, string>();

				bool isMovementKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsMovementKey());

				if (movementSummaryList == null || movementSummaryList.Count == 0)
				{
					model.OwnerOptions = BuildSavedViewOwnerOptions(
						Enumerable.Empty<KeyValuePair<Guid, string>>(),
						true
						);
					model.ModifyMovementSummaryRight = (this.Security.HasRight(RIGHT.OPERATE_MODIFY_MOVEMENT_SUMMARY) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_MOVEMENT_SUMMARY)) && isMovementKey;
					return this.PartialView("MovementSummarySelectionView", model);
				}

				foreach (MovementSummary movementSummary in movementSummaryList)
				{
					var movementSummaryMenuModel = new MovementSummaryMenuModel
					{
						MovementSummaryGuid = movementSummary.MovementSummaryGuid,
						ID = movementSummary.ID,
						Description = movementSummary.Description,
						MovementSummaryType = movementSummary.MovementSummaryType,
						OwnerUserGuid = movementSummary.OwnerUserGuid,
						Owner = this.GetSavedViewOwnerDisplayName(
							movementSummary.OwnerUserGuid,
							ownerDisplayNames,
							movementSummary.MovementSummaryType == MovementSummary.MovementSummaryVisibilityType.Public),
						IsEditable = (movementSummary.OwnerUserGuid == this.Security.UserGuid ||
																movementSummary.MovementSummaryType == MovementSummary.MovementSummaryVisibilityType.Public
                                                || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_MOVEMENT_SUMMARY)),
						ViewMovementSummaryRight = (this.Security.HasRight(RIGHT.OPERATE_VIEW_MOVEMENT_SUMMARY) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_MOVEMENT_SUMMARY)) && isMovementKey,
                  ModifyMovementSummaryRight = (this.Security.HasRight(RIGHT.OPERATE_MODIFY_MOVEMENT_SUMMARY) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_MOVEMENT_SUMMARY)) && isMovementKey,
                  AdministerMovementSummaryRight = this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_MOVEMENT_SUMMARY) && isMovementKey,
                  CreatePublicMovementSummaryRight = this.Security.HasRight(RIGHT.OPERATE_CREATE_PUBLIC_MOVEMENT_SUMMARY) && isMovementKey,
						ModifyPublicMovementSummaryRight = (this.Security.HasRight(RIGHT.OPERATE_MODIFY_PUBLIC_MOVEMENT_SUMMARY) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_MOVEMENT_SUMMARY)) && isMovementKey,
						CreateSharedMovementSummaryRight = this.Security.HasRight(RIGHT.OPERATE_CREATE_SHARED_MOVEMENT_SUMMARY) && isMovementKey,
						ModifySharedMovementSummaryRight = (this.Security.HasRight(RIGHT.OPERATE_MODIFY_SHARED_MOVEMENT_SUMMARY) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_MOVEMENT_SUMMARY)) && isMovementKey,
						RowVersion = Convert.ToBase64String(movementSummary.RowVersion)
					};

					model.MovementSummaries.Add(movementSummaryMenuModel);
				}

				model.OwnerOptions = BuildSavedViewOwnerOptions(
					model.MovementSummaries
						.Select(x => new KeyValuePair<Guid, string>(x.OwnerUserGuid, x.Owner)),true
					);
            model.ModifyMovementSummaryRight = (this.Security.HasRight(RIGHT.OPERATE_MODIFY_MOVEMENT_SUMMARY) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_MOVEMENT_SUMMARY)) && isMovementKey;
            model.AdministerMovementSummaryRight = this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_MOVEMENT_SUMMARY) && isMovementKey;
            return this.PartialView("MovementSummarySelectionView", model);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				throw;
			}
		}

		/// <summary>
		/// Creates the distinct owner choices shown by a saved-view submenu.
		/// Owners that cannot be resolved from tblUsers are represented by one
		/// Unavailable/Deleted choice while their entities retain their owner GUID.
		/// </summary>
		private static List<SavedViewOwnerOptionModel> BuildSavedViewOwnerOptions(
			IEnumerable<KeyValuePair<Guid, string>> owners,
			bool includeSystemOwner)
		{
			var ownerList = owners == null
				? new List<KeyValuePair<Guid, string>>()
				: owners.ToList();
			var options = new List<SavedViewOwnerOptionModel>
			{
				new SavedViewOwnerOptionModel(string.Empty, "All owners")
			};

			options.AddRange(
				ownerList
					.Where(x => x.Key != Guid.Empty && !string.Equals(x.Value, "Unavailable", StringComparison.Ordinal))
					.GroupBy(x => x.Key)
					.Select(x => x.First())
					.OrderBy(x => x.Value, StringComparer.CurrentCultureIgnoreCase)
					.Select(x => new SavedViewOwnerOptionModel(x.Key.ToString(), x.Value)));

			if (includeSystemOwner)
			{
				options.Add(new SavedViewOwnerOptionModel("__system__", "System (Public)"));
			}

			if (ownerList.Any(x => string.Equals(x.Value, "Unavailable", StringComparison.Ordinal)))
			{
				options.Add(new SavedViewOwnerOptionModel("__unavailable__", "Unavailable/Deleted"));
			}

			return options;
		}

		/// <summary>
		/// Resolves a saved-view owner for presentation in the Operate submenu demo.
		/// </summary>
		private string GetSavedViewOwnerDisplayName(
			Guid ownerUserGuid,
			IDictionary<Guid, string> ownerDisplayNames,
			bool isSystemOwned)
		{
			if (isSystemOwned && ownerUserGuid == Guid.Empty)
			{
				return "System (Public)";
			}

			if (ownerUserGuid == this.Security.UserGuid)
			{
				return this.Security.UserID;
			}

			if (ownerUserGuid == Guid.Empty)
			{
				return "Unavailable";
			}

			string ownerDisplayName;
			if (ownerDisplayNames.TryGetValue(ownerUserGuid, out ownerDisplayName))
			{
				return ownerDisplayName;
			}

			try
			{
				UserClass owner = FMChannelHelper.MakeCall<IUsers, UserClass>(
					x => x.Get(this.Security, ownerUserGuid));
				ownerDisplayName = owner == null || owner.IdentityGuid == Guid.Empty
					? "Unavailable"
					: owner.ID;
			}
			catch
			{
				ownerDisplayName = "Unavailable";
			}

			ownerDisplayNames[ownerUserGuid] = ownerDisplayName;
			return ownerDisplayName;
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult GetPointMetadataForMovementSummary(string pointguid, List<string> tags)
		{
			try
			{
				if (tags.Count > 0)
				{
					Guid pointGuid = new Guid(pointguid);
					var pointGuidList = new List<Guid>();

					var movementDataGuid = FMChannelHelper.MakeCall<IPointProperties, Guid>(x => x.GetPointPropertyGuid(this.Security, pointGuid, "Movement Data"));

					var movementyDataPropertyList = new List<PointTag>();

					foreach (var tagname in tags)
					{
						if(tagname == "empty")
						{
							continue;
						}

						// all tags will be read from the MovementData
						if(movementDataGuid != null && movementDataGuid != Guid.Empty)
						{
							// create the point value identifier for the column
							var movementDataProperty = new PointTag { PointGuid = movementDataGuid, ID = tagname.Replace(" ",""), PointTagGuid = Guid.Empty };
							movementyDataPropertyList.Add(movementDataProperty);
						}
					}

					bool CommunicationsFailure = false;
					var propertyList = from p in movementyDataPropertyList select new { PointValueIdentifier_IdentityGuid = p.PointGuid, PointValueIdentifier_PointValueType = PointValueType.Setting, PointValueIdentifier_PropertyID = p.ID, PointValueIdentifier_UtcTicks = 0, p.PointGuid, p.PointTagGuid, p.ID, p.Units, p.Maximum, p.Minimum, p.DecimalPlaces, p.EngineeringUnitsType, p.InhibitOverride, p.WellKnownIdentityGuid, p.InputOutputType, p.Status, CommunicationsFailure };
					return this.JsonWithErrorMessages(propertyList, JsonRequestBehavior.AllowGet);
				}

				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult GetTagMetadataForMovementSummaryPointList(List<Guid> pointGuids, string tagName)
		{
			try
			{
				bool CommunicationsFailure = false;

				if (pointGuids.Count > 0)
				{
					var movementyDataPropertyList = new List<PointTag>();

					foreach (var pointGuid in pointGuids)
					{ 
						var pointGuidList = new List<Guid>();

						var movementDataGuid = FMChannelHelper.MakeCall<IPointProperties, Guid>(x => x.GetPointPropertyGuid(this.Security, pointGuid, "Movement Data"));


						// all tags will be read from the MovementData except the point & empty column
						if (tagName != "point"
						&& tagName != "State"
						&& tagName != "empty"
						&& movementDataGuid != null
						&& movementDataGuid != Guid.Empty)
						{
							// create the point value identifier for the column
							var movementDataProperty = new PointTag { PointGuid = movementDataGuid, ID = tagName.Replace(" ", ""), PointTagGuid = Guid.Empty };
							movementyDataPropertyList.Add(movementDataProperty);
						}
					}
					var propertyList = from p in movementyDataPropertyList select new { PointValueIdentifier_IdentityGuid = p.PointGuid, PointValueIdentifier_PointValueType = PointValueType.Setting, PointValueIdentifier_PropertyID = p.ID, PointValueIdentifier_UtcTicks = 0, p.PointGuid, p.PointTagGuid, p.ID, p.Units, p.Maximum, p.Minimum, p.DecimalPlaces, p.EngineeringUnitsType, p.InhibitOverride, p.WellKnownIdentityGuid, p.InputOutputType, p.Status, CommunicationsFailure };
					return this.JsonWithErrorMessages(propertyList, JsonRequestBehavior.AllowGet);
				}

				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult GetPointMetadataForPointGroup(string pointguid, List<string> tags)
		{
			try
			{
				if (tags.Count > 0)
				{
					Guid checkPointGuid = new Guid(pointguid);
					var pointGuidList = new List<Guid>();
					pointGuidList.Add(checkPointGuid);

					var pointTagDictionary = FMChannelHelper.MakeCall<IPointTags, Dictionary<Guid, Dictionary<Guid, PointTag>>>(x => x.EnumerateByPointList(this.Security, pointGuidList, tags));

					var tagList = new List<PointTag>();
					var propertyList = new List<PointTag>();

					var allTags = (pointTagDictionary.ContainsKey(checkPointGuid)) ? pointTagDictionary[checkPointGuid].Values.ToList() : new List<PointTag>();
					foreach (var tagname in tags)
					{
						// check if tag name is actually a point property
						// the tagName may actually be a setting ( we allow for name, product, product description, site name, and site number )
						if (tagname == "ID" || tagname == "ProductID" || tagname == "ProductDescription" || tagname == "SiteID" || tagname == "SiteNumber")
						{
							// we need to create the point value identifier for the column
							var pointPropery = new PointTag { PointGuid = checkPointGuid, ID = tagname, PointTagGuid = Guid.Empty };
							propertyList.Add(pointPropery);
							continue;
						}

						int index = allTags.FindIndex(x => x.ID.ToLower(CultureInfo.InvariantCulture) == tagname.ToLower(CultureInfo.InvariantCulture) && !x.Deleted);
						if (index >= 0)
						{
							var newTag = allTags[index];

							tagList.Add(newTag);
						}
					}

					bool CommunicationsFailure = false;
					// we don't need to return all the data, only the fields we will use
					var pointTagList = from p in tagList select new { PointValueIdentifier_IdentityGuid = p.PointTagGuid, PointValueIdentifier_PointValueType = PointValueType.Tag, PointValueIdentifier_PropertyID = (string)null, PointValueIdentifier_UtcTicks = 0, p.PointGuid, p.PointTagGuid, p.ID, p.Units, p.Maximum, p.Minimum, p.DecimalPlaces, p.EngineeringUnitsType, p.InhibitOverride, p.WellKnownIdentityGuid, p.InputOutputType, p.Status, CommunicationsFailure };
					var pointpropertyList = from p in propertyList select new { PointValueIdentifier_IdentityGuid = p.PointGuid, PointValueIdentifier_PointValueType = PointValueType.Point, PointValueIdentifier_PropertyID = p.ID, PointValueIdentifier_UtcTicks = 0, p.PointGuid, p.PointTagGuid, p.ID, p.Units, p.Maximum, p.Minimum, p.DecimalPlaces, p.EngineeringUnitsType, p.InhibitOverride, p.WellKnownIdentityGuid, p.InputOutputType, p.Status, CommunicationsFailure };
					var result = pointTagList.Concat(pointpropertyList);
					return this.JsonWithErrorMessages(result, JsonRequestBehavior.AllowGet);
				}

				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult GetAllPointsMetadataForPointGroup(List<string> pointguids, List<string> tags)
		{
			try
			{
				if (tags.Count > 0 && (pointguids?.Count ?? 0) > 0)
				{
					var pointGuidList = (from checkPointGuid in pointguids select new Guid(checkPointGuid)).ToList();

					var pointTagDictionary = FMChannelHelper.MakeCall<IPointTags, Dictionary<Guid, Dictionary<Guid, PointTag>>>(x => x.EnumerateByPointList(this.Security, pointGuidList, tags));

					Dictionary<string, List<PointGroupRowMetadata>> result = new Dictionary<string, List<PointGroupRowMetadata>>();

					foreach (string pointguid in pointguids)
					{
						var tagList = new List<PointTag>();
						var propertyList = new List<PointTag>();

						Guid checkPointGuid = new Guid(pointguid);
						var allTags = pointTagDictionary.ContainsKey(checkPointGuid) ? pointTagDictionary[checkPointGuid].Values.ToList() : new List<PointTag>();
						foreach (var tagname in tags)
						{
							// check if tag name is actually a point property
							// the tagName may actually be a setting ( we allow for name, product, product description, site name, and site number )
							if (tagname == "ID" || tagname == "ProductID" || tagname == "ProductDescription" || tagname == "SiteID" || tagname == "SiteNumber")
							{
								// we need to create the point value identifier for the column
								var pointPropery = new PointTag { PointGuid = checkPointGuid, ID = tagname, PointTagGuid = Guid.Empty };
								propertyList.Add(pointPropery);
								continue;
							}

							int index = allTags.FindIndex(x => x.ID.ToLower(CultureInfo.InvariantCulture) == tagname.ToLower(CultureInfo.InvariantCulture) && !x.Deleted);
							if (index >= 0)
							{
								var newTag = allTags[index];

								tagList.Add(newTag);
							}
						}

						bool CommunicationsFailure = false;
						// we don't need to return all the data, only the fields we will use
						var pointTagList = from p in tagList select new PointGroupRowMetadata(p.PointTagGuid, PointValueType.Tag, (string)null, p.ServerTimeStamp.UtcTicks, p.PointGuid, p.PointTagGuid, p.ID, p.Units, p.Maximum, p.Minimum, p.DecimalPlaces, p.EngineeringUnitsType, p.InhibitOverride, p.WellKnownIdentityGuid, p.InputOutputType, p.Status, CommunicationsFailure);
						var pointpropertyList = from p in propertyList select new PointGroupRowMetadata(p.PointGuid, PointValueType.Point, p.ID, p.ServerTimeStamp.UtcTicks, p.PointGuid, p.PointTagGuid, p.ID, p.Units, p.Maximum, p.Minimum, p.DecimalPlaces, p.EngineeringUnitsType, p.InhibitOverride, p.WellKnownIdentityGuid, p.InputOutputType, p.Status, CommunicationsFailure);
						result[pointguid] = pointTagList.Concat(pointpropertyList).ToList();

					}		
					
					return this.JsonWithErrorMessages(result, JsonRequestBehavior.AllowGet);
				}

				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult GetTagMetadataForPointList(List<Guid> points, string tagName)
		{
			try
			{
				bool CommunicationsFailure = false;

				if (points.Count > 0)
				{

					// the tagName may actually be a setting ( we allow for name, product, product description, site name, and site number )
					if (tagName == "ID" || tagName == "ProductID" || tagName == "ProductDescription" || tagName == "SiteID" || tagName == "SiteNumber")
					{
						// we need to create the point value identifier for the column
						var pointList = new List<PointTag>();

						for (int i = 0; i < points.Count; i++)
						{
							var pointTag = new PointTag { PointGuid = points[i], ID = tagName, PointTagGuid = Guid.Empty };
							pointList.Add(pointTag);
						}

						// we don't need to return all the data, only the fields we will use
						var pointTagList = from p in pointList select new { PointValueIdentifier_IdentityGuid = p.PointGuid, PointValueIdentifier_PointValueType = PointValueType.Point, PointValueIdentifier_PropertyID = p.ID, PointValueIdentifier_UtcTicks = p.ServerTimeStamp.UtcTicks, p.PointGuid, p.PointTagGuid, p.ID, p.Units, p.Maximum, p.Minimum, p.DecimalPlaces, p.EngineeringUnitsType, p.InhibitOverride, p.WellKnownIdentityGuid, p.Status, CommunicationsFailure };
						return this.JsonWithErrorMessages(pointTagList, JsonRequestBehavior.AllowGet);

					}
					else
					{
						var tagList = FMChannelHelper.MakeCall<IPointTags, List<PointTag>>(x => x.EnumerateTagsByPointList(this.Security, points, tagName));

						// we don't need to return all the data, only the fields we will use
						var pointTagList = from p in tagList select new { PointValueIdentifier_IdentityGuid = p.PointTagGuid, PointValueIdentifier_PointValueType = PointValueType.Tag, PointValueIdentifier_PropertyID = (string)null, PointValueIdentifier_UtcTicks = p.ServerTimeStamp.UtcTicks, p.PointGuid, p.PointTagGuid, p.ID, p.Units, p.Maximum, p.Minimum, p.DecimalPlaces, p.EngineeringUnitsType, p.InhibitOverride, p.WellKnownIdentityGuid, p.Status, CommunicationsFailure };

						return this.JsonWithErrorMessages(pointTagList, JsonRequestBehavior.AllowGet);
					}
				}

				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult GetAllPointsMetadataForPointHistory(List<string> pointguids, List<string> tags)
        {
            try
            {
                if (tags.Count > 0 && (pointguids?.Count ?? 0) > 0)
                {
                    var pointGuidList = (from checkPointGuid in pointguids select new Guid(checkPointGuid)).ToList();

                    var pointTagDictionary = FMChannelHelper.MakeCall<IPointTags, Dictionary<Guid, Dictionary<Guid, PointTag>>>(x => x.EnumerateByPointList(this.Security, pointGuidList, tags));

                    Dictionary<string, List<PointHistoryRowMetadata>> result = new Dictionary<string, List<PointHistoryRowMetadata>>();

                    foreach (string pointguid in pointguids)
                    {
                        var tagList = new List<PointTag>();
                        var propertyList = new List<PointTag>();

                        Guid checkPointGuid = new Guid(pointguid);
                        var allTags = pointTagDictionary.ContainsKey(checkPointGuid) ? pointTagDictionary[checkPointGuid].Values.ToList() : new List<PointTag>();
                        foreach (var tagname in tags)
                        {
                            int index = allTags.FindIndex(x => x.ID.ToLower(CultureInfo.InvariantCulture) == tagname.ToLower(CultureInfo.InvariantCulture) && !x.Deleted);
                            if (index >= 0)
                            {
                                var newTag = allTags[index];

                                tagList.Add(newTag);
                            }
                        }

                        bool CommunicationsFailure = false;
                        // we don't need to return all the data, only the fields we will use
                        var pointTagList = from p in tagList select new PointHistoryRowMetadata(p.PointTagGuid, PointValueType.Tag, (string)null, p.ServerTimeStamp.UtcTicks, p.PointGuid, p.PointTagGuid, p.ID, p.Units, p.Maximum, p.Minimum, p.DecimalPlaces, p.EngineeringUnitsType, p.InhibitOverride, p.WellKnownIdentityGuid, p.InputOutputType, p.Status, CommunicationsFailure, p.ValueTypeString);
                        var pointpropertyList = from p in propertyList select new PointHistoryRowMetadata(p.PointGuid, PointValueType.Point, p.ID, p.ServerTimeStamp.UtcTicks, p.PointGuid, p.PointTagGuid, p.ID, p.Units, p.Maximum, p.Minimum, p.DecimalPlaces, p.EngineeringUnitsType, p.InhibitOverride, p.WellKnownIdentityGuid, p.InputOutputType, p.Status, CommunicationsFailure, p.ValueTypeString);
                        result[pointguid] = pointTagList.Concat(pointpropertyList).ToList();

                    }

                    return this.JsonWithErrorMessages(result, JsonRequestBehavior.AllowGet);
                }

                return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception except)
            {
                this.OnError(except);
                return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult GetTagMetadataForPointHistoryList(List<Guid> points, string tagName)
        {
            try
            {
                bool CommunicationsFailure = false;

                if (points.Count > 0)
                {
                    var tagList = FMChannelHelper.MakeCall<IPointTags, List<PointTag>>(x => x.EnumerateTagsByPointList(this.Security, points, tagName));

                    // we don't need to return all the data, only the fields we will use
                    var pointTagList = from p in tagList select new PointHistoryRowMetadata(p.PointTagGuid, PointValueType.Tag, (string)null, p.ServerTimeStamp.UtcTicks, p.PointGuid, p.PointTagGuid, p.ID, p.Units, p.Maximum, p.Minimum, p.DecimalPlaces, p.EngineeringUnitsType, p.InhibitOverride, p.WellKnownIdentityGuid, p.InputOutputType, p.Status, CommunicationsFailure, p.ValueTypeString);

                    return this.JsonWithErrorMessages(pointTagList, JsonRequestBehavior.AllowGet);
                }

                return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception except)
            {
                this.OnError(except);
                return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
		[ValidateJsonAntiForgeryToken]
		public ActionResult AddTrend(string id, string description)
		{

			// Check to see if we are trying to insert a duplicate trend 
			var duplicateTrendGuid = FMChannelHelper.MakeCall<ITrends, Guid?>(x => x.GetIdentityGuid(this.Security, id));
			if (duplicateTrendGuid == Guid.Empty)
			{
				Trend trend = new Trend
				{
					TrendGuid = Guid.NewGuid(),
					ID = id,
					Description = description,
					Mode = FMBusinessObjects.DataObjects.CodedVariables.TrendModeEnum.Realtime,
					PeriodType = FMBusinessObjects.DataObjects.CodedVariables.TrendPeriodType.Minutes,
					Period = 10
				};

				var trendGuid = FMChannelHelper.MakeCall<ITrends, Guid>(x => x.Add(this.Security, trend)).ToString();

				this.ModelState.Clear();
				this.AddSuccess(this.GetTranslatedText("Save Successful"));
				return this.JsonWithErrorMessages(new { trendGuid = trendGuid, duplicateFound = false }, JsonRequestBehavior.AllowGet);
			}

			this.ModelState.Clear();
			return this.JsonWithErrorMessages(new { trendGuid = duplicateTrendGuid, duplicateFound = true }, JsonRequestBehavior.AllowGet);
		}



		[HttpPost]
		[ValidateJsonAntiForgeryToken]
		public ActionResult AddMovementSummary(string id, string description, MovementSummary.MovementSummaryVisibilityType movementSummaryType)
		{

			// Check to see if we are trying to insert a duplicate Movement Summary 
			var duplicateMovementSummaryGuid = FMChannelHelper.MakeCall<IMovementSummaries, Guid?>
								(x => x.GetDuplicate(this.Security, id, (int)movementSummaryType, (movementSummaryType == MovementSummary.MovementSummaryVisibilityType.Public) ? Guid.Empty : this.Security.UserGuid, this.Security.SiteGuid));
			
			if (duplicateMovementSummaryGuid == null || duplicateMovementSummaryGuid == new Guid())
			{
				string defaultRows = "{\"id\":\"" + Guid.NewGuid().ToString() + "\",\"type\":\"blank\",\"parentRowId\":\"null\",\"indent\":1},";
				defaultRows = "[" + defaultRows.TrimEnd(',') + "]";

				// default list of columns 

				MovementSummary movementSummary = new MovementSummary();
				movementSummary.MovementSummaryGuid = Guid.NewGuid();
				movementSummary.ID = id;
				movementSummary.Description = description;
				movementSummary.OwnerUserGuid = (movementSummaryType == MovementSummary.MovementSummaryVisibilityType.Public) ? Guid.Empty : this.Security.UserGuid;
				movementSummary.SiteGuid = this.Security.SiteGuid;
				movementSummary.MovementSummaryType = movementSummaryType;
				movementSummary.ColumnsDefinition = MovementSummary.DefaultColumns;
				movementSummary.FontSize = 14;
				movementSummary.RowsDefinition = defaultRows;  // always add an empty row, even if dynamic movement summary

				var movementSummaryGuid = FMChannelHelper.MakeCall<IMovementSummaries, Guid>(x => x.Add(this.Security, movementSummary)).ToString();

				this.ModelState.Clear();
				this.AddSuccess(this.GetTranslatedText("Save Successful"));
				return this.JsonWithErrorMessages(new { movementSummaryGuid = movementSummaryGuid, duplicateFound = false }, JsonRequestBehavior.AllowGet);

			}

			this.ModelState.Clear();
			return this.JsonWithErrorMessages(new { movementSummaryGuid = duplicateMovementSummaryGuid, duplicateFound = true }, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// This method initial a movement.
		/// </summary>
		/// <param name="movementPointGuidStr">The movement point Guid string.</param>
		/// <returns>Returns back success or error.</returns>
		[HttpPost]
		[ValidateJsonAntiForgeryToken]
		public ActionResult InitiateMovement(string movementPointGuidString)
		{
            if (!this.Security.HasRight(RIGHT.OPERATE_MODIFY_MOVEMENT_SUMMARY))
            {
                this.OnError("User does not have right (Operate Modify Movement Summary).");
                return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
            }

            Guid movementPointGuid;
			bool validGuid = Guid.TryParse(movementPointGuidString, out movementPointGuid);

            if (validGuid == false)
			{
				this.OnError("Invalid Movement Point Guid.");
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

			var movementWellKnownTagGuidList = new Guid[] {
				Guids.MovementInitiateIdentityGuid
			};

			var pointGuidList = new List<Guid>();
			pointGuidList.Add(movementPointGuid);

			var movementPointValueIdentifierList = FMChannelHelper.MakeCall < IPointTags, List<PointValueIdentifier>> (x => x.EnumeratePointValueIdentifersByPointAndTagLists(this.Security, pointGuidList, movementWellKnownTagGuidList.ToList()));

			var movementPointValueList = FMChannelHelper.MakeCall < IPointServiceManager, List< PointValue>> (x => x.GetPointValueData(this.Security, movementPointValueIdentifierList));

			movementPointValueList[0].Value = movementPointGuidString;
			movementPointValueList[0].Status = StatusCodes.Good;
			movementPointValueList[0].ServerTimeStamp = DateTimeOffset.UtcNow;
			movementPointValueList[0].SourceTimeStamp = DateTimeOffset.UtcNow;

			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

			try
			{
				SetPointValues(this.Security, site, movementPointValueList);
			}
			catch (Exception e)
			{
				this.OnError(new Exception(this.GetTranslatedText("Error Initiating Movement : " + e.Message)));
			}

			return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// This method initial a movement.
		/// </summary>
		/// <param name="movementPointGuidStr">The movement point Guid string.</param>
		/// <param name="movementNodePointGuidStr">The movement node point Guid string.</param>
		/// <returns>Returns back success or error.</returns>
		[HttpPost]
		[ValidateJsonAntiForgeryToken]
		public ActionResult InitiateMovementNode(string movementPointGuidString, string movementNodePointGuidString)
		{
            if (!this.Security.HasRight(RIGHT.OPERATE_MODIFY_MOVEMENT_SUMMARY))
            {
                this.OnError("User does not have right (Operate Modify Movement Summary).");
                return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
            }

            Guid movementPointGuid;
			bool validGuid = Guid.TryParse(movementPointGuidString, out movementPointGuid);

			if (validGuid == false)
			{
				this.OnError("Invalid Movement Point Guid.");
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

			Guid movementNodePointGuid;
			validGuid = Guid.TryParse(movementNodePointGuidString, out movementNodePointGuid);

			if (validGuid == false)
			{
				this.OnError("Invalid Movement NodePoint Guid.");
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			var movementWellKnownTagGuidList = new Guid[] {
				Guids.MovementInitiateIdentityGuid
			};

			var pointGuidList = new List<Guid>();
			pointGuidList.Add(movementPointGuid);

			var movementPointValueIdentifierList = FMChannelHelper.MakeCall<IPointTags, List<PointValueIdentifier>>(x => x.EnumeratePointValueIdentifersByPointAndTagLists(this.Security, pointGuidList, movementWellKnownTagGuidList.ToList()));

			var movementPointValueList = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, movementPointValueIdentifierList));

			movementPointValueList[0].Value = movementNodePointGuidString;
			movementPointValueList[0].Status = StatusCodes.Good;
			movementPointValueList[0].ServerTimeStamp = DateTimeOffset.UtcNow;
			movementPointValueList[0].SourceTimeStamp = DateTimeOffset.UtcNow;

			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

			try
			{
				SetPointValues(this.Security, site, movementPointValueList);
			}
			catch (Exception e)
			{
				this.OnError(new Exception(this.GetTranslatedText("Error Initiating Movement Node: " + e.Message)));
			}

			return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
		}


		/// <summary>
		/// This method will set the movement hold for hand gauge command.
		/// </summary>
		/// <param name="movementOrNodePointGuidStr">The movement point Guid string.</param>
		/// <returns>Returns back success or error.</returns>
		[HttpPost]
		[ValidateJsonAntiForgeryToken]
		public ActionResult SetMovementHandGaugeCommand(string movementOrNodePointGuidStr)
		{
			Guid movementOrNodePointGuid;
			bool validGuid = Guid.TryParse(movementOrNodePointGuidStr, out movementOrNodePointGuid);

			if (validGuid == false)
			{
				this.OnError("Invalid Movement Point Guid.");
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

			// Get the point tag Guid for the movement tag "Command".
			var commandTagPointGuid = FMChannelHelper.MakeCall<IPointTags, Guid>(x => x.GetIdentityGuid(this.Security, "Command", movementOrNodePointGuid));
			var pointValueIdentifier = new PointValueIdentifier(commandTagPointGuid, PointValueType.Tag, null);

			List<PointValueIdentifier> pointValueIdentifiers = new List<PointValueIdentifier>(1) { pointValueIdentifier };
			List<PointValue> pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, pointValueIdentifiers));

			if (pointValues == null || pointValues.Count != 1 || pointValues[0] == null)
			{
				throw new Exception("OperateController InitiateMovement: error reading point value");
			}

			var pointValue = pointValues[0];
			pointValue.Value = MovementCommand.HoldForHandgaugeData;

			pointValues = new List<PointValue> { pointValue };
			FMChannelHelper.MakeCall<IPointServiceManager>(x => x.SetPointValueData(this.Security, pointValues, false));

			return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// This method delete a movement.
		/// </summary>
		/// <param name="movementPointGuidString">The movement point Guid string.</param>
		/// <param name="actionType">Contains Movement or MovementNode action.</param>
		/// <returns>Returns back success or error.</returns>
		[HttpPost]
		[ValidateJsonAntiForgeryToken]
		public ActionResult DeleteMovement(string movementPointGuidString)
		{
            if (!(this.Security.HasRight(RIGHT.OPERATE_MODIFY_MOVEMENT_SUMMARY) 
				|| this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_MOVEMENT_SUMMARY)))
            {
                this.OnError("User does not have right (Operate Modify Movement Summary).");
                return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
            }

            Guid movementPointGuid;
			bool validGuid = Guid.TryParse(movementPointGuidString, out movementPointGuid);

			if (validGuid == false)
			{
				this.OnError("Invalid Movement Point Guid.");
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

			try
			{
				FMChannelHelper.MakeCall<IPoints>(x => x.Purge(this.Security, movementPointGuid));

				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			catch (CommunicationException e)
			{
				this.OnError(new Exception(this.GetTranslatedText(e.Message)));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null);
			}
		}


		/// <summary>
		/// This method stop a movement or movement node.
		/// </summary>
		/// <param name="movementPointGuidString">The movement point Guid string.</param>
		/// <param name="actionType">Contains Movement or MovementNode action.</param>
		/// <returns>Returns back success or error.</returns>
		[HttpPost]
		[ValidateJsonAntiForgeryToken]
		public ActionResult StopMovement(string movementPointGuidString)
		{
            if (!(this.Security.HasRight(RIGHT.OPERATE_MODIFY_MOVEMENT_SUMMARY) 
				|| this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_MOVEMENT_SUMMARY)))
            {
                this.OnError("User does not have right (Operate Modify Movement Summary).");
                return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
            }
            
			Guid movementPointGuid;
			bool validGuid = Guid.TryParse(movementPointGuidString, out movementPointGuid);

			if (validGuid == false)
			{
				this.OnError("Invalid Movement Point Guid.");
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}


			var movementWellKnownTagGuidList = new Guid[] {
				Guids.MovementStopIdentityGuid
			};

			var pointGuidList = new List<Guid>();
			pointGuidList.Add(movementPointGuid);

			var movementPointValueIdentifierList = FMChannelHelper.MakeCall<IPointTags, List<PointValueIdentifier>>(x => x.EnumeratePointValueIdentifersByPointAndTagLists(this.Security, pointGuidList, movementWellKnownTagGuidList.ToList()));

			var movementPointValueList = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, movementPointValueIdentifierList));

			movementPointValueList[0].Value = movementPointGuidString;
			movementPointValueList[0].Status = StatusCodes.Good;
			movementPointValueList[0].ServerTimeStamp = DateTimeOffset.UtcNow;
			movementPointValueList[0].SourceTimeStamp = DateTimeOffset.UtcNow;

			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

			try
			{
				SetPointValues(this.Security, site, movementPointValueList);
			}
			catch (Exception e)
			{
				this.OnError(new Exception(this.GetTranslatedText("Error Stopping Movement : " + e.Message)));
			}

			return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// This method stop a movement or movement node.
		/// </summary>
		/// <param name="movementNodePointGuidString">The movement node point Guid string.</param>
		/// <param name="actionType">Contains Movement or MovementNode action.</param>
		/// <returns>Returns back success or error.</returns>
		[HttpPost]
		[ValidateJsonAntiForgeryToken]
		public ActionResult StopMovementNode(string movementPointGuidString, string movementNodePointGuidString)
		{
            if (!(this.Security.HasRight(RIGHT.OPERATE_MODIFY_MOVEMENT_SUMMARY)
             || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_MOVEMENT_SUMMARY)))
            {
                this.OnError("User does not have right (Operate Modify Movement Summary).");
                return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
            }

            Guid movementPointGuid;
			bool validGuid = Guid.TryParse(movementPointGuidString, out movementPointGuid);

			if (validGuid == false)
			{
				this.OnError("Invalid Movement Point Guid.");
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

			Guid movementNodePointGuid;
			validGuid = Guid.TryParse(movementNodePointGuidString, out movementNodePointGuid);

			if (validGuid == false)
			{
				this.OnError("Invalid Movement Node Point Guid.");
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}


			var movementWellKnownTagGuidList = new Guid[] {
				Guids.MovementStopIdentityGuid
			};

			var pointGuidList = new List<Guid>();
			pointGuidList.Add(movementPointGuid);

			var movementPointValueIdentifierList = FMChannelHelper.MakeCall<IPointTags, List<PointValueIdentifier>>(x => x.EnumeratePointValueIdentifersByPointAndTagLists(this.Security, pointGuidList, movementWellKnownTagGuidList.ToList()));

			var movementPointValueList = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, movementPointValueIdentifierList));

			movementPointValueList[0].Value = movementNodePointGuidString;
			movementPointValueList[0].Status = StatusCodes.Good;
			movementPointValueList[0].ServerTimeStamp = DateTimeOffset.UtcNow;
			movementPointValueList[0].SourceTimeStamp = DateTimeOffset.UtcNow;

			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

			try
			{
				SetPointValues(this.Security, site, movementPointValueList);
			}
			catch (Exception e)
			{
				this.OnError(new Exception(this.GetTranslatedText("Error Stopping Movement Node : " + e.Message)));
			}

			return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
		}


		/// <summary>
		/// This method will get the associated movement node data and the movement point.
		/// </summary>
		/// <param name="pointGuidStr">The movement point Guid string.</param>
		/// <returns>Returns a list of movement node data and movement node point information</returns>
		[HttpPost]
		[ValidateJsonAntiForgeryToken]
		public ActionResult GetAssociatedMovementNodes(string pointGuidStr)
		{
			try
			{
				Guid movementPointGuid;
				bool validGuid = Guid.TryParse(pointGuidStr, out movementPointGuid);

				if(validGuid == false)
					{
					this.OnError("Invalid Movement Point Guid.");
					//return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
					return this.Json(null);
				}

				var movementPointProperties = FMChannelHelper.MakeCall<IPointProperties, Dictionary<Guid, PointProperty>>
																		(x => x.EnumerateByPoint(this.Security, movementPointGuid));

				var movementPoint = FMChannelHelper.MakeCall<IPoints, Point>(x => x.Get(this.Security, movementPointGuid));

				var movementInfoList = new List<MovementNodeInfoModel>();

				if (movementPointProperties == null || movementPointProperties.Count == 0)
				{
					//return this.JsonWithErrorMessages(movementInfoList, JsonRequestBehavior.AllowGet);
					return this.Json(movementInfoList);
				}

				// There should only be one movement point property associated to the movement point.
				foreach (KeyValuePair<Guid, PointProperty> item in movementPointProperties)
				{
					Guid movementPointPropertyGuid = item.Key;
					PointProperty movementPointProperty = item.Value;

					var movementModelSetting = movementPointProperty.Value as MovementModuleSettings;

					if (movementModelSetting != null && movementModelSetting.MovementNodeDataList != null && movementModelSetting.MovementNodeDataList.Count > 0)
					{
						foreach(MovementNodeData movementNodeData in movementModelSetting.MovementNodeDataList)
						{
							var movementNodePoint = FMChannelHelper.MakeCall<IPoints, Point>(x => x.Get(this.Security, movementNodeData.MovementNodeGuid, false));
							string movementNodePointId = "Unknown";

							if(movementNodePoint != null && string.IsNullOrEmpty(movementNodePoint.ID) == false)
							{
								movementNodePointId = movementNodePoint.ID;
							}

							var model = new MovementNodeInfoModel
							{
								MovementPointId				= movementPoint.ID,
								MovementPointGuid				= movementPointGuid,
								MovementNodeGuid				= movementNodeData.MovementNodeGuid,
								TransferTarget					= movementNodeData.TransferTarget,
								MovementNodeId					= movementNodePointId,
								IndividualNodeControl		= movementNodeData.IndividualNodeControl
							};

							switch(movementNodeData.TransferMode)
							{
								case TransferModes.Level:
									model.TransferMode = "Level";
									break;
								case TransferModes.Batch:
									model.TransferMode = "Batch";
									break;
								default:
									model.TransferMode = "None";
									break;
							}

							switch(movementNodeData.TransferDirection)
							{
								case TransferDirection.Destination:
									model.TransferDirection = "Destination";
									break;
								case TransferDirection.Source:
									model.TransferDirection = "Source";
									break;
								default:
									model.TransferDirection = "None";
									break;
							}

							movementInfoList.Add(model);
						}
					}
				}

				//return this.JsonWithErrorMessages(movementInfoList, JsonRequestBehavior.AllowGet);
				return this.Json(movementInfoList);
			}
			catch (Exception except)
			{
				this.OnError(except);
				//return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
				return this.Json(null);
			}
		}

		[HttpPost]
		[ValidateJsonAntiForgeryToken]
		public ActionResult AddPointGroup(string id, string description, PointGroup.PointGroupVisibilityType pointGroupType)
		{

			// Check to see if we are trying to insert a duplicate point group 
			var duplicatePointGroupGuid = FMChannelHelper.MakeCall<IPointGroups, Guid?>(x => x.GetDuplicate(this.Security, id, (int)pointGroupType, this.Security.UserGuid, this.Security.SiteGuid));
			if (duplicatePointGroupGuid == null || duplicatePointGroupGuid == new Guid())
			{

				var defaultRows = "";
				for (int i = 0; i < 5; i++)
				{
					defaultRows += "{\"id\":\"" + Guid.NewGuid().ToString() + "\", \"type\":\"blank\"},";
				}
				defaultRows = "[" + defaultRows.TrimEnd(',') + "]";

				// default list of columns 
				var defaultColumns = "[{\"name\":\"Point\",\"resizable\":true,\"sortable\":false,\"minWidth\":30,\"rerenderOnResize\":false,\"headerCssClass\":\"text-center grid-font-14\",\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":80,\"id\":\"point\",\"field\":\"point\",\"behavior\":\"selectAndMove\",\"cssClass\":\"ui-state-default text-center grid-font-14\",\"header\":null,\"previousWidth\":80,\"totalizerValue\":null},"
									+ "{\"name\":\"\",\"resizable\":true,\"sortable\":false,\"minWidth\":30,\"rerenderOnResize\":false,\"headerCssClass\":null,\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":80,\"id\":\"empty5\",\"header\":null,\"previousWidth\":80,\"totalizerValue\":null},"
									+ "{\"name\":\"\",\"resizable\":true,\"sortable\":false,\"minWidth\":30,\"rerenderOnResize\":false,\"headerCssClass\":null,\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":80,\"id\":\"empty6\",\"header\":null,\"previousWidth\":80,\"totalizerValue\":null},"
									+ "{\"name\":\"\",\"resizable\":true,\"sortable\":false,\"minWidth\":30,\"rerenderOnResize\":false,\"headerCssClass\":null,\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":80,\"id\":\"empty7\",\"header\":null,\"previousWidth\":80,\"totalizerValue\":null},"
									+ "{\"name\":\"\",\"resizable\":true,\"sortable\":false,\"minWidth\":30,\"rerenderOnResize\":false,\"headerCssClass\":null,\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":80,\"id\":\"empty8\",\"header\":null,\"previousWidth\":80,\"totalizerValue\":null},"
									+ "{\"name\":\"\",\"resizable\":true,\"sortable\":false,\"minWidth\":30,\"rerenderOnResize\":false,\"headerCssClass\":null,\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":80,\"id\":\"empty9\",\"header\":null,\"previousWidth\":80,\"totalizerValue\":null}]";

				PointGroup pointGroup = new PointGroup();
				pointGroup.PointGroupGuid = Guid.NewGuid();
				pointGroup.ID = id;
				pointGroup.Description = description;
				pointGroup.OwnerUserGuid = this.Security.UserGuid;
				pointGroup.SiteGuid = this.Security.SiteGuid;
				pointGroup.PointGroupType = pointGroupType;
				pointGroup.PointGroupColumn.PointGroupGuid = pointGroup.PointGroupGuid;
				pointGroup.PointGroupColumn.PointGroupColumnsGuid = Guid.NewGuid();
				pointGroup.PointGroupColumn.ColumnsDefinition = defaultColumns;
				pointGroup.PointGroupColumn.FontSize = 14;
				pointGroup.PointGroupColumn.OwnerUserGuid = this.Security.UserGuid;
				pointGroup.PointGroupColumn.SiteGuid = this.Security.SiteGuid;
				pointGroup.PointGroupRow.PointGroupGuid = pointGroup.PointGroupGuid;
				pointGroup.PointGroupRow.PointGroupRowsGuid = Guid.NewGuid();
				pointGroup.PointGroupRow.RowsDefinition = defaultRows;  // always add 5 empty rows, even if dynamic point group
				pointGroup.PointGroupRow.OwnerUserGuid = this.Security.UserGuid;
				pointGroup.PointGroupRow.SiteGuid = this.Security.SiteGuid;

				var pointGroupGuid = FMChannelHelper.MakeCall<IPointGroups, Guid>(x => x.Add(this.Security, pointGroup)).ToString();

				this.ModelState.Clear();
				this.AddSuccess(this.GetTranslatedText("Save Successful"));
				return this.JsonWithErrorMessages(new { pointGroupGuid = pointGroupGuid, duplicateFound = false }, JsonRequestBehavior.AllowGet);

			}

			this.ModelState.Clear();
			return this.JsonWithErrorMessages(new { pointGroupGuid = duplicatePointGroupGuid, duplicateFound = true }, JsonRequestBehavior.AllowGet);
			/*
			JsonSerializerSettings serSettings = new JsonSerializerSettings();
			serSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
			FilterRule outObject = JsonConvert.DeserializeObject<FilterRule>(dynamicPointGroupFilterRule, serSettings);
			return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
*/
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GetDynamicPointGroupFilterOptions()
		{
			try
			{
				var options = new Dictionary<string, List<Tuple<string, string>>>();

				// get the list of Point Types
				var pointTypes = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(x => x.EnumerateByType(this.Security, STRING_TYPE.POINT_TEMPLATE_TYPE));

				var pointTypeList = new List<Tuple<string, string>>();
				foreach (var pointType in pointTypes)
				{
					pointTypeList.Add(new Tuple<string, string>(pointType.IdentityGuid.ToString(), pointType.ID));
				}
				options.Add("point_type", pointTypeList);

				// get the list of Categories
				var categories = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(x => x.EnumerateByType(this.Security, STRING_TYPE.POINT_CATEGORY));

				var categoryList = new List<Tuple<string, string>>();
				foreach (var category in categories.OrderBy(x => x.ID))
				{
					categoryList.Add(new Tuple<string, string>(category.IdentityGuid.ToString(), category.ID));
				}
				options.Add("category", categoryList);

				// get the list of product Groups
				var productGroups = FMChannelHelper.MakeCall<IProductGroups, ProductGroupCollectionClass>(x => x.Enumerate(this.Security));

				var productGroupList = new List<Tuple<string, string>>();
				foreach (var productGroup in productGroups.OrderBy(x => x.ID))
				{
					productGroupList.Add(new Tuple<string, string>(productGroup.IdentityGuid.ToString(), productGroup.ID));
				}
				options.Add("product_group", productGroupList);

				return this.JsonWithErrorMessages(options, JsonRequestBehavior.AllowGet);

			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		/// <summary>
		/// This method will presist the movement summary columns and rows.
		/// </summary>
		/// <param name="movementSummaryGuid">The movement summary Guid to update.</param>
		/// <param name="id">The ID of the movement</param>
		/// <param name="description">The description of the movement</param>
		/// <param name="columns">The column definition</param>
		/// <param name="rows">The row definition</param>
		/// <param name="fontSize">The font size.</param>
		/// <param name="movementSummaryType">The movement type.</param>
		/// <returns>Returns the movement summary model</returns>
		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult SaveMovementSummary(string movementSummaryGuid, string id, string description, string columns, string rows, int fontSize, string jsRowVersion, MovementSummary.MovementSummaryVisibilityType movementSummaryType)
		{
			try
			{
				if (this.ModelState.IsValid)
				{
					// if new Movement Summary
					if (string.IsNullOrWhiteSpace(movementSummaryGuid) || movementSummaryGuid == "00000000-0000-0000-0000-000000000000")
					{
						// Check to see if we are trying to insert a duplicate Movement Summary 
						var duplicateMovementSummaryGuid = FMChannelHelper.MakeCall<IMovementSummaries, Guid?>(x => x.GetDuplicate(this.Security, id, (int)movementSummaryType, (movementSummaryType == MovementSummary.MovementSummaryVisibilityType.Public) ? Guid.Empty : this.Security.UserGuid, this.Security.SiteGuid));

						if (duplicateMovementSummaryGuid == null || duplicateMovementSummaryGuid == new Guid())
						{

							MovementSummary movementSummary = new MovementSummary
							{
								MovementSummaryGuid = Guid.NewGuid(),
								ID					= id,
								Description		= description,
								OwnerUserGuid	= (movementSummaryType == MovementSummary.MovementSummaryVisibilityType.Public) ? Guid.Empty : this.Security.UserGuid,
								SiteGuid			= this.Security.SiteGuid,
								MovementSummaryType = movementSummaryType
							};

							movementSummary.ColumnsDefinition				= columns;
							movementSummary.FontSize							= fontSize;
							movementSummary.RowsDefinition					= rows;
							movementSummaryGuid = FMChannelHelper.MakeCall<IMovementSummaries, Guid>(x => x.Add(this.Security, movementSummary)).ToString();

							this.ModelState.Clear();
							this.AddSuccess(this.GetTranslatedText("Save Successful"));
							return this.JsonWithErrorMessages(new { movementSummaryGuid = movementSummaryGuid, duplicateFound = false }, JsonRequestBehavior.AllowGet);

						}

						this.ModelState.Clear();
						return this.JsonWithErrorMessages(new { movementSummaryGuid = duplicateMovementSummaryGuid, duplicateFound = true }, JsonRequestBehavior.AllowGet);
					}
					else
					{
						var movementSummary = FMChannelHelper.MakeCall<IMovementSummaries, MovementSummary>(x => x.Get(this.Security, Guid.Parse(movementSummaryGuid), this.Security.UserGuid, this.Security.SiteGuid));
	
						// check if we can update the Movement Summary
						if (movementSummary.OwnerUserGuid == this.Security.UserGuid || movementSummary.MovementSummaryType == MovementSummary.MovementSummaryVisibilityType.Public || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_POINT_GROUP))
						{
							// check to see if we find a Movement Summary that matches the Movement Summary that we are saving
							var duplicateMovementSummaryGuid = FMChannelHelper.MakeCall<IMovementSummaries, Guid?>(
												x => x.GetDuplicate(this.Security, id, (int)movementSummaryType, movementSummary.OwnerUserGuid, movementSummary.SiteGuid));
	
							if (!(duplicateMovementSummaryGuid == null || duplicateMovementSummaryGuid == new Guid()) && duplicateMovementSummaryGuid != Guid.Parse(movementSummaryGuid))
							{
								this.ModelState.AddModelError("Movement Summary", TranslateText("Duplicate Movement Summary."));
								return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
							}

							movementSummary.ID = id;
							movementSummary.Description = description;

							// I can change the visibility type only if I own the Movement
							if (movementSummary.OwnerUserGuid == this.Security.UserGuid)
							{
								movementSummary.MovementSummaryType = movementSummaryType;
							}

							movementSummary.FontSize = fontSize;
							movementSummary.ColumnsDefinition = columns;

							movementSummary.RowsDefinition = rows;

						  if (jsRowVersion != null)
						  {
								byte[] rowVersionBytes = Convert.FromBase64String(jsRowVersion);

								if (rowVersionBytes != null && rowVersionBytes.Length > 0)
									 movementSummary.RowVersion = rowVersionBytes;
						  }

							byte[] rowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };

							FMChannelHelper.MakeCall<IMovementSummaries>(x => x.Modify(this.Security, movementSummary, out rowVersion));
							this.ModelState.Clear();
							return this.JsonWithErrorMessages(new { movementSummaryGuid = movementSummaryGuid, duplicateFound = false, rowVersion = Convert.ToBase64String(rowVersion) }, JsonRequestBehavior.AllowGet);
						}

						this.ModelState.AddModelError("Movement Summary", TranslateText("You have no permission to update the specified Movement Summary."));

						return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
					}
				}

				return this.JsonWithErrorMessages(new { movementSummaryGuid = movementSummaryGuid, duplicateFound = false }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				// Suppress this particular error. It just means the movementSummary was edited elsewhere (example, Galahad interface)
				// View will be updated on next refresh
				if (e.Message.Contains("Attempted to modify a stale copy of the record")) 
					 return this.JsonWithErrorMessages(new { movementSummaryGuid = movementSummaryGuid, duplicateFound = false, rowVersion = Convert.ToBase64String(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }) }, JsonRequestBehavior.AllowGet);
				this.OnError(new Exception(this.GetTranslatedText("Error Saving Movement Summary : " + e.Message)));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}	

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult SavePointGroup(string pointGroupGuid, string id, string description, string columns, string rows, int fontSize, PointGroup.PointGroupVisibilityType pointGroupType)
		{
			try
			{
				if (this.ModelState.IsValid)
				{
					// if new Point Group
					if (string.IsNullOrWhiteSpace(pointGroupGuid) || pointGroupGuid == "00000000-0000-0000-0000-000000000000")
					{
						// Check to see if we are trying to insert a duplicate point group 
						var duplicatePointGroupGuid = FMChannelHelper.MakeCall<IPointGroups, Guid?>(x => x.GetDuplicate(this.Security, id, (int)pointGroupType, this.Security.UserGuid, this.Security.SiteGuid));

						if (duplicatePointGroupGuid == null || duplicatePointGroupGuid == new Guid())
						{

							PointGroup pointGroup = new PointGroup
							{
								PointGroupGuid = Guid.NewGuid(),
								ID = id,
								Description = description,
								OwnerUserGuid = this.Security.UserGuid,
								SiteGuid = this.Security.SiteGuid,
								PointGroupType = pointGroupType
							};

							pointGroup.PointGroupColumn.PointGroupGuid = pointGroup.PointGroupGuid;
							pointGroup.PointGroupColumn.PointGroupColumnsGuid = Guid.NewGuid();
							pointGroup.PointGroupColumn.ColumnsDefinition = columns;
							pointGroup.PointGroupColumn.FontSize = fontSize;
							pointGroup.PointGroupColumn.OwnerUserGuid = this.Security.UserGuid;
							pointGroup.PointGroupColumn.SiteGuid = this.Security.SiteGuid;
							pointGroup.PointGroupRow.PointGroupGuid = pointGroup.PointGroupGuid;
							pointGroup.PointGroupRow.PointGroupRowsGuid = Guid.NewGuid();
							pointGroup.PointGroupRow.RowsDefinition = rows;
							pointGroup.PointGroupRow.OwnerUserGuid = this.Security.UserGuid;
							pointGroup.PointGroupRow.SiteGuid = this.Security.SiteGuid;

							pointGroupGuid = FMChannelHelper.MakeCall<IPointGroups, Guid>(x => x.Add(this.Security, pointGroup)).ToString();

							this.ModelState.Clear();
							this.AddSuccess(this.GetTranslatedText("Save Successful"));
							return this.JsonWithErrorMessages(new { pointGroupGuid = pointGroupGuid, duplicateFound = false }, JsonRequestBehavior.AllowGet);

						}

						this.ModelState.Clear();
						return this.JsonWithErrorMessages(new { pointGroupGuid = duplicatePointGroupGuid, duplicateFound = true }, JsonRequestBehavior.AllowGet);
					}
					else
					{
						var pointGroup = FMChannelHelper.MakeCall<IPointGroups, PointGroup>(x => x.Get(this.Security, Guid.Parse(pointGroupGuid), this.Security.UserGuid, this.Security.SiteGuid));
						// check if we can update the point group
						if (pointGroup.OwnerUserGuid == this.Security.UserGuid
							|| pointGroup.PointGroupType == PointGroup.PointGroupVisibilityType.Public
                      || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_POINT_GROUP))
						{

							// check to see if we find a point group that matches the point group that we are saving
							var duplicatePointGroupGuid = FMChannelHelper.MakeCall<IPointGroups, Guid?>(x => x.GetDuplicate(this.Security, id, (int)pointGroupType, pointGroup.OwnerUserGuid, pointGroup.SiteGuid));
							if (!(duplicatePointGroupGuid == null || duplicatePointGroupGuid == new Guid()) && duplicatePointGroupGuid != Guid.Parse(pointGroupGuid))
							{
								this.ModelState.AddModelError("Point Group", TranslateText("Duplicate Point Group."));
								return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
							}

							pointGroup.ID = id;
							pointGroup.Description = description;
							// I can change the visibility type only if I own the point
							if (pointGroup.OwnerUserGuid == this.Security.UserGuid)
							{
								pointGroup.PointGroupType = pointGroupType;
							}
							if (pointGroup.PointGroupColumn != null)
							{
								pointGroup.PointGroupColumn.FontSize = fontSize;
								pointGroup.PointGroupColumn.ColumnsDefinition = columns;
							}
							if (pointGroup.PointGroupRow != null)
							{
								pointGroup.PointGroupRow.RowsDefinition = rows;
							}

							FMChannelHelper.MakeCall<IPointGroups>(x => x.Modify(this.Security, pointGroup));
							this.ModelState.Clear();
							return this.JsonWithErrorMessages(new { pointGroupGuid = pointGroupGuid, duplicateFound = false }, JsonRequestBehavior.AllowGet);
						}

						this.ModelState.AddModelError("Point Group", TranslateText("You have no permission to update the specified Point Group."));

						return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
					}
				}

				return this.JsonWithErrorMessages(new { pointGroupGuid = pointGroupGuid, duplicateFound = false }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				this.OnError(new Exception(this.GetTranslatedText("Error Saving Point Group : " + e.Message)));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GetOperatePointGroup(string id, string pointName)
		{
			try
			{
				var pointGroupObject = new PointGroupModel();
				//TODO eventually we will have a template to help create blank point groups
				// if we are creating a new point group we return the defaults 
				if (string.IsNullOrEmpty(id) || id == "00000000-0000-0000-0000-000000000000")
				{
					pointGroupObject.ID = pointName;
					pointGroupObject.Description = "";
					// 5 empty rows 
					pointGroupObject.Rows = "[{},{},{},{},{}]";
					// default list of columns 
					pointGroupObject.Columns =
						"[{\"name\":\"Point\",\"resizable\":true,\"sortable\":false,\"minWidth\":30,\"rerenderOnResize\":false,\"headerCssClass\":\"text-center grid-font-14\",\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":80,\"id\":\"point\",\"field\":\"point\",\"behavior\":\"selectAndMove\",\"cssClass\":\"ui-state-default text-center grid-font-14\",\"header\":null,\"previousWidth\":80,\"totalizerValue\":null},"
						+ "{\"name\":\"Level%20Product\",\"resizable\":true,\"sortable\":false,\"minWidth\":30,\"rerenderOnResize\":false,\"headerCssClass\":\"text-center grid-font-14\",\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":115,\"id\":\"levelProduct\",\"field\":\"Level Product\",\"cssClass\":\"text-center grid-font-14\",\"header\":null,\"previousWidth\":115,\"totalizerValue\":null},"
						+ "{\"name\":\"Level%20Water\",\"resizable\":true,\"sortable\":false,\"minWidth\":30,\"rerenderOnResize\":false,\"headerCssClass\":\"text-center grid-font-14\",\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":104,\"id\":\"levelWater\",\"field\":\"Level Water\",\"cssClass\":\"text - center grid - font - 14\",\"header\":null,\"previousWidth\":104,\"totalizerValue\":null},"
						+ "{\"name\":\"Volume%20Total%20Observed\",\"resizable\":true,\"sortable\":false,\"minWidth\":30,\"rerenderOnResize\":false,\"headerCssClass\":\"text-center grid-font-14\",\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":178,\"id\":\"volumeProduct\",\"field\":\"Volume Total Observed\",\"cssClass\":\"text - center grid - font - 14\",\"header\":null,\"previousWidth\":164,\"totalizerValue\":null},"
						+ "{\"name\":\"Volume%20Water\",\"resizable\":true,\"sortable\":false,\"minWidth\":30,\"rerenderOnResize\":false,\"headerCssClass\":\"text-center grid-font-14\",\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":102,\"id\":\"volumeWater\",\"field\":\"Volume Water\",\"cssClass\":\"text - center grid - font - 14\",\"header\":null,\"previousWidth\":102,\"totalizerValue\":null},"
						+ "{\"name\":\"\",\"resizable\":true,\"sortable\":false,\"minWidth\":30,\"rerenderOnResize\":false,\"headerCssClass\":null,\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":80,\"id\":\"empty5\",\"header\":null,\"previousWidth\":80,\"totalizerValue\":null},"
						+ "{\"name\":\"\",\"resizable\":true,\"sortable\":false,\"minWidth\":30,\"rerenderOnResize\":false,\"headerCssClass\":null,\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":80,\"id\":\"empty6\",\"header\":null,\"previousWidth\":80,\"totalizerValue\":null},"
						+ "{\"name\":\"\",\"resizable\":true,\"sortable\":false,\"minWidth\":30,\"rerenderOnResize\":false,\"headerCssClass\":null,\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":80,\"id\":\"empty7\",\"header\":null,\"previousWidth\":80,\"totalizerValue\":null},"
						+ "{\"name\":\"\",\"resizable\":true,\"sortable\":false,\"minWidth\":30,\"rerenderOnResize\":false,\"headerCssClass\":null,\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":80,\"id\":\"empty8\",\"header\":null,\"previousWidth\":80,\"totalizerValue\":null},"
						+ "{\"name\":\"\",\"resizable\":true,\"sortable\":false,\"minWidth\":30,\"rerenderOnResize\":false,\"headerCssClass\":null,\"defaultSortAsc\":true,\"focusable\":true,\"selectable\":true,\"width\":80,\"id\":\"empty9\",\"header\":null,\"previousWidth\":80,\"totalizerValue\":null}]";
					pointGroupObject.FontSize = 14;
					pointGroupObject.PointGroupType = PointGroup.PointGroupVisibilityType.Public;

					UserClass user = FMChannelHelper.MakeCall<IUsers, UserClass>(x => x.Get(this.Security, this.Security.UserGuid));
					pointGroupObject.Owner = user.Name;
					pointGroupObject.IsOwnedByMe = true;
					pointGroupObject.ViewPointGroupsRight = this.Security.HasRight(RIGHT.OPERATE_VIEW_POINT_GROUPS) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_POINT_GROUP);
					pointGroupObject.ModifyPointGroupsRight = this.Security.HasRight(RIGHT.OPERATE_MODIFY_POINT_GROUPS) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_POINT_GROUP);
					pointGroupObject.CreatePublicPointGroupsRight = this.Security.HasRight(RIGHT.OPERATE_CREATE_PUBLIC_POINT_GROUPS);
					pointGroupObject.ModifyPublicPointGroupsRight = this.Security.HasRight(RIGHT.OPERATE_MODIFY_PUBLIC_POINT_GROUPS) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_POINT_GROUP);
					pointGroupObject.CreateSharedPointGroupsRight = this.Security.HasRight(RIGHT.OPERATE_CREATE_SHARED_POINT_GROUPS);
					pointGroupObject.ModifySharedPointGroupsRight = this.Security.HasRight(RIGHT.OPERATE_MODIFY_SHARED_POINT_GROUPS) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_POINT_GROUP);

					// Save the new point Group
					var newPointGroup = new PointGroup
					{
						ID = pointGroupObject.ID,
						Description = pointGroupObject.Description,
						OwnerUserGuid = this.Security.UserGuid,
						PointGroupType = PointGroup.PointGroupVisibilityType.Public,
						SiteGuid = this.Security.SiteGuid,
						PointGroupColumn =
															{
																FontSize = pointGroupObject.FontSize,
																ColumnsDefinition = pointGroupObject.Columns,
																OwnerUserGuid = this.Security.UserGuid,
																SiteGuid = this.Security.SiteGuid
															},
						PointGroupRow =
															{
																RowsDefinition = pointGroupObject.Rows,
																OwnerUserGuid = this.Security.UserGuid,
																SiteGuid = this.Security.SiteGuid
															}
					};

					pointGroupObject.PointGroupGuid =
						FMChannelHelper.MakeCall<IPointGroups, Guid>(x => x.Add(this.Security, newPointGroup));
				}
				else
				{

					var pointGroupGuid = new Guid(id);
					var pointGroup =
						FMChannelHelper.MakeCall<IPointGroups, PointGroup>(
							x => x.Get(this.Security, pointGroupGuid, this.Security.UserGuid, this.Security.SiteGuid));

					if (pointGroup.IdentityGuid == Guid.Empty)
					{
						this.ModelState.AddModelError("Point Group", TranslateText("Point Group not found."));
					}
					else if ( // check if we have permission to access the point group
						!(pointGroup.OwnerUserGuid == this.Security.UserGuid
						|| pointGroup.PointGroupType == PointGroup.PointGroupVisibilityType.Public
						|| pointGroup.PointGroupType == PointGroup.PointGroupVisibilityType.Shared
						|| this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_POINT_GROUP)))
					{
						this.ModelState.AddModelError(
							"Point Group",
							TranslateText("You have no permission to access the specified Point Group."));
					}
					else
					{
						pointGroupObject.PointGroupGuid = pointGroup.PointGroupGuid;
						pointGroupObject.ID = pointGroup.ID;
						pointGroupObject.Description = pointGroup.Description;
						pointGroupObject.Rows = pointGroup.PointGroupRow.RowsDefinition;
						pointGroupObject.Columns = pointGroup.PointGroupColumn.ColumnsDefinition;
						pointGroupObject.FontSize = pointGroup.PointGroupColumn.FontSize;
						pointGroupObject.PointGroupType = pointGroup.PointGroupType;
						pointGroupObject.IsEditable = ((pointGroup.OwnerUserGuid == this.Security.UserGuid
																&& (pointGroup.PointGroupType == PointGroup.PointGroupVisibilityType.Private
																	|| (pointGroup.PointGroupType == PointGroup.PointGroupVisibilityType.Public
																	&& this.Security.HasRight(RIGHT.OPERATE_MODIFY_PUBLIC_POINT_GROUPS))
																	|| (pointGroup.PointGroupType == PointGroup.PointGroupVisibilityType.Shared
																	&& this.Security.HasRight(RIGHT.OPERATE_MODIFY_SHARED_POINT_GROUPS))
																	))
																|| (pointGroup.PointGroupType == PointGroup.PointGroupVisibilityType.Public
																	&& this.Security.HasRight(RIGHT.OPERATE_MODIFY_PUBLIC_POINT_GROUPS))
                                                 ||  this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_POINT_GROUP));
						pointGroupObject.ViewPointGroupsRight = this.Security.HasRight(RIGHT.OPERATE_VIEW_POINT_GROUPS) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_POINT_GROUP);
						pointGroupObject.ModifyPointGroupsRight = this.Security.HasRight(RIGHT.OPERATE_MODIFY_POINT_GROUPS) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_POINT_GROUP);
						pointGroupObject.CreatePublicPointGroupsRight = this.Security.HasRight(RIGHT.OPERATE_CREATE_PUBLIC_POINT_GROUPS);
						pointGroupObject.ModifyPublicPointGroupsRight = this.Security.HasRight(RIGHT.OPERATE_MODIFY_PUBLIC_POINT_GROUPS) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_POINT_GROUP);
						pointGroupObject.CreateSharedPointGroupsRight = this.Security.HasRight(RIGHT.OPERATE_CREATE_SHARED_POINT_GROUPS);
						pointGroupObject.ModifySharedPointGroupsRight = this.Security.HasRight(RIGHT.OPERATE_MODIFY_SHARED_POINT_GROUPS) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_POINT_GROUP);

						pointGroupObject.IsOwnedByMe = (pointGroup.OwnerUserGuid == this.Security.UserGuid);
						try
						{
							UserClass ownerUser =
								FMChannelHelper.MakeCall<IUsers, UserClass>(x => x.Get(this.Security, pointGroup.OwnerUserGuid));
							pointGroupObject.Owner = ownerUser.Name;
						}
						catch (Exception)
						{
							pointGroupObject.Owner = "Unavailable";
						}
					}
				}
				return this.JsonWithErrorMessages(pointGroupObject, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				this.OnError(e);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

        [HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GetOperateMovementSummary(string id, string movementName)
		{
			try
			{
				bool isMovementKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsMovementKey());
				var movementSummaryObject = new MovementSummaryMenuModel();

				//TODO eventually we will have a template to help create blank point groups
				// if we are creating a new point group we return the defaults 
				if (string.IsNullOrEmpty(id) || id == "00000000-0000-0000-0000-000000000000")
				{
					movementSummaryObject.ID = movementName;
					movementSummaryObject.Description = "";
					// 5 empty rows 
					movementSummaryObject.Rows = "[{},{},{},{},{}]";
					// default list of columns 
					movementSummaryObject.Columns = MovementSummary.DefaultColumns;
					movementSummaryObject.FontSize = 14;
					movementSummaryObject.MovementSummaryType =  MovementSummary.MovementSummaryVisibilityType.Public;

					UserClass user = FMChannelHelper.MakeCall<IUsers, UserClass>(x => x.Get(this.Security, this.Security.UserGuid));

					movementSummaryObject.Owner = user.Name;
					movementSummaryObject.IsOwnedByMe = true;
					movementSummaryObject.ViewMovementSummaryRight = (this.Security.HasRight(RIGHT.OPERATE_VIEW_MOVEMENT_SUMMARY) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_MOVEMENT_SUMMARY)) && isMovementKey;
					movementSummaryObject.ModifyMovementSummaryRight = (this.Security.HasRight(RIGHT.OPERATE_MODIFY_MOVEMENT_SUMMARY) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_MOVEMENT_SUMMARY)) && isMovementKey;
					movementSummaryObject.CreatePublicMovementSummaryRight = this.Security.HasRight(RIGHT.OPERATE_CREATE_PUBLIC_MOVEMENT_SUMMARY) && isMovementKey;
					movementSummaryObject.ModifyPublicMovementSummaryRight = (this.Security.HasRight(RIGHT.OPERATE_MODIFY_PUBLIC_MOVEMENT_SUMMARY) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_MOVEMENT_SUMMARY)) && isMovementKey;
					movementSummaryObject.CreateSharedMovementSummaryRight = this.Security.HasRight(RIGHT.OPERATE_CREATE_SHARED_MOVEMENT_SUMMARY) && isMovementKey;
					movementSummaryObject.ModifySharedMovementSummaryRight = (this.Security.HasRight(RIGHT.OPERATE_MODIFY_SHARED_MOVEMENT_SUMMARY) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_MOVEMENT_SUMMARY)) && isMovementKey;

					// Save the new point Group
					var newMovementSummary = new MovementSummary
					{
						ID = movementSummaryObject.ID,
						Description = movementSummaryObject.Description,
						OwnerUserGuid = this.Security.UserGuid,
						MovementSummaryType = MovementSummary.MovementSummaryVisibilityType.Public,
						SiteGuid = this.Security.SiteGuid,
						FontSize = movementSummaryObject.FontSize,
						ColumnsDefinition = movementSummaryObject.Columns,
						RowsDefinition = movementSummaryObject.Rows,
					};

					movementSummaryObject.MovementSummaryGuid =
						FMChannelHelper.MakeCall<IMovementSummaries, Guid>(x => x.Add(this.Security, newMovementSummary));
				}
				else
				{

					var movementSummaryGuid = new Guid(id);
					var movementSummary =
						FMChannelHelper.MakeCall<IMovementSummaries, MovementSummary>(
							x => x.Get(this.Security, movementSummaryGuid, this.Security.UserGuid, this.Security.SiteGuid));

					if (movementSummary.IdentityGuid == Guid.Empty)
					{
						this.ModelState.AddModelError("Movement Summary", TranslateText("Movement Summary not found."));
					}
					else if ( // check if we have permission to access the point group
						!(movementSummary.OwnerUserGuid == this.Security.UserGuid
						|| movementSummary.MovementSummaryType == MovementSummary.MovementSummaryVisibilityType.Public
						|| movementSummary.MovementSummaryType == MovementSummary.MovementSummaryVisibilityType.Shared
                  || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_MOVEMENT_SUMMARY)))
					{
						this.ModelState.AddModelError(
							"Movement Summary",
							TranslateText("You have no permission to access the specified Movement Summary."));
					}
					else
					{
						movementSummaryObject.MovementSummaryGuid = movementSummary.MovementSummaryGuid;
						movementSummaryObject.ID = movementSummary.ID;
						movementSummaryObject.Description = movementSummary.Description;
						movementSummaryObject.Rows = movementSummary.RowsDefinition;
						movementSummaryObject.Columns = movementSummary.ColumnsDefinition;
						movementSummaryObject.FontSize = movementSummary.FontSize;
						movementSummaryObject.MovementSummaryType = movementSummary.MovementSummaryType;
						movementSummaryObject.IsEditable = true;
						movementSummaryObject.ViewMovementSummaryRight = this.Security.HasRight(RIGHT.OPERATE_VIEW_MOVEMENT_SUMMARY) && isMovementKey;
						movementSummaryObject.ModifyMovementSummaryRight = this.Security.HasRight(RIGHT.OPERATE_MODIFY_MOVEMENT_SUMMARY) && isMovementKey;
						movementSummaryObject.CreatePublicMovementSummaryRight = this.Security.HasRight(RIGHT.OPERATE_CREATE_PUBLIC_MOVEMENT_SUMMARY) && isMovementKey;
						movementSummaryObject.ModifyPublicMovementSummaryRight = this.Security.HasRight(RIGHT.OPERATE_MODIFY_PUBLIC_MOVEMENT_SUMMARY) && isMovementKey;
						movementSummaryObject.CreateSharedMovementSummaryRight = this.Security.HasRight(RIGHT.OPERATE_CREATE_SHARED_MOVEMENT_SUMMARY) && isMovementKey;
						movementSummaryObject.ModifySharedMovementSummaryRight = this.Security.HasRight(RIGHT.OPERATE_MODIFY_SHARED_MOVEMENT_SUMMARY) && isMovementKey;
						movementSummaryObject.RowVersion = Convert.ToBase64String(movementSummary.RowVersion);

						movementSummaryObject.IsOwnedByMe = (movementSummary.OwnerUserGuid == this.Security.UserGuid);
						try
						{
							UserClass ownerUser =
								FMChannelHelper.MakeCall<IUsers, UserClass>(x => x.Get(this.Security, movementSummary.OwnerUserGuid));
							movementSummaryObject.Owner = ownerUser.Name;
						}
						catch (Exception)
						{
							movementSummaryObject.Owner = "Unavailable";
						}
					}
				}
				return this.JsonWithErrorMessages(movementSummaryObject, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				this.OnError(e);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GetMovementSummaryIfNewer(string movementSummaryGuidStr, string prevRowVersion)
		{
			MovementSummary movementSummary = null;

				FMChannelHelper.MakeCall<IMovementSummaries>(
					x => x.GetMovementSummaryIfNewer(this.Security, new Guid(movementSummaryGuidStr), Convert.FromBase64String(prevRowVersion), out movementSummary));
			
			if(movementSummary != null)
			{
				bool isMovementKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsMovementKey());
				var movementSummaryObject = new MovementSummaryMenuModel();
				if (movementSummary.IdentityGuid == Guid.Empty)
				{
					this.ModelState.AddModelError("Movement Summary", TranslateText("Movement Summary not found."));
				}
				else if ( // check if we have permission to access the point group
					!(movementSummary.OwnerUserGuid == this.Security.UserGuid
					|| movementSummary.MovementSummaryType == MovementSummary.MovementSummaryVisibilityType.Public
					|| movementSummary.MovementSummaryType == MovementSummary.MovementSummaryVisibilityType.Shared
               || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_MOVEMENT_SUMMARY)))
				{
					this.ModelState.AddModelError(
						"Movement Summary",
						TranslateText("You have no permission to access the specified Movement Summary."));
				}
				else
				{
					movementSummaryObject.MovementSummaryGuid = movementSummary.MovementSummaryGuid;
					movementSummaryObject.ID = movementSummary.ID;
					movementSummaryObject.Description = movementSummary.Description;
					movementSummaryObject.Rows = movementSummary.RowsDefinition;
					movementSummaryObject.Columns = movementSummary.ColumnsDefinition;
					movementSummaryObject.FontSize = movementSummary.FontSize;
					movementSummaryObject.MovementSummaryType = movementSummary.MovementSummaryType;
					movementSummaryObject.IsEditable = true;
					movementSummaryObject.ViewMovementSummaryRight = (this.Security.HasRight(RIGHT.OPERATE_VIEW_MOVEMENT_SUMMARY) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_MOVEMENT_SUMMARY)) && isMovementKey;
					movementSummaryObject.ModifyMovementSummaryRight = (this.Security.HasRight(RIGHT.OPERATE_MODIFY_MOVEMENT_SUMMARY) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_MOVEMENT_SUMMARY)) && isMovementKey;
					movementSummaryObject.CreatePublicMovementSummaryRight = this.Security.HasRight(RIGHT.OPERATE_CREATE_PUBLIC_MOVEMENT_SUMMARY) && isMovementKey;
					movementSummaryObject.ModifyPublicMovementSummaryRight = (this.Security.HasRight(RIGHT.OPERATE_MODIFY_PUBLIC_MOVEMENT_SUMMARY) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_MOVEMENT_SUMMARY)) && isMovementKey;
					movementSummaryObject.CreateSharedMovementSummaryRight = this.Security.HasRight(RIGHT.OPERATE_CREATE_SHARED_MOVEMENT_SUMMARY) && isMovementKey;
					movementSummaryObject.ModifySharedMovementSummaryRight = (this.Security.HasRight(RIGHT.OPERATE_MODIFY_SHARED_MOVEMENT_SUMMARY) || this.Security.HasRight(RIGHT.OPERATE_ADMINISTER_MOVEMENT_SUMMARY)) && isMovementKey;
					movementSummaryObject.RowVersion = Convert.ToBase64String(movementSummary.RowVersion);

					movementSummaryObject.IsOwnedByMe = (movementSummary.OwnerUserGuid == this.Security.UserGuid);
					try
					{
						UserClass ownerUser =
							FMChannelHelper.MakeCall<IUsers, UserClass>(x => x.Get(this.Security, movementSummary.OwnerUserGuid));
						movementSummaryObject.Owner = ownerUser.Name;
					}
					catch (Exception)
					{
						movementSummaryObject.Owner = "Unavailable";
					}
				}
				return this.JsonWithErrorMessages(movementSummaryObject, JsonRequestBehavior.AllowGet);
			}

			return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
		}


		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult DeleteMovementSummary(string id)
		{
			try
			{
				var movementSummaryGuid = new Guid(id);
				FMChannelHelper.MakeCall<IMovementSummaries>(x => x.Purge(this.Security, movementSummaryGuid));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				this.OnError(e);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}
      [HttpPost, ValidateJsonAntiForgeryToken]
      public ActionResult DeleteMultipleMovementSummary(List<string> guidList)
      {
         try
         {
            foreach (var guid in guidList)
				{
					//Global.WriteToEventLog("DeleteMultipleMovementSummary guid: " + guid, System.Diagnostics.EventLogEntryType.Information);
					var movementSummaryGuid = new Guid(guid);
					FMChannelHelper.MakeCall<IMovementSummaries>(x => x.Purge(this.Security, movementSummaryGuid));
				}
            return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
         }
         catch (Exception e)
         {
            this.OnError(e);
            return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
         }
      }

      [HttpPost, ValidateJsonAntiForgeryToken]
      public ActionResult DeleteMultiplePointGroups(List<string> guidList)
      {
         try
         {
				foreach (var guid in guidList)
				{
					//Global.WriteToEventLog("DeleteMultiplePointGroups guid: " + guid, System.Diagnostics.EventLogEntryType.Information);
					var pointGroupGuid = new Guid(guid);
					FMChannelHelper.MakeCall<IPointGroups>(x => x.Purge(this.Security, pointGroupGuid));
            }
            return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
         }
         catch (Exception e)
         {
            this.OnError(e);
            return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
         }
      }

      [HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult DeletePointGroup(string id)
		{
			try
			{
				var pointGroupGuid = new Guid(id);
				FMChannelHelper.MakeCall<IPointGroups>(x => x.Purge(this.Security, pointGroupGuid));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				this.OnError(e);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

        [HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult SaveOperatePointHistory(string startDateString, int intervalQuantity, int intervalType, int rangeQuantity, int rangeType, string columnsDefinition) {
			try 
			{
                if (this.ModelState.IsValid)
                {
                    var existingPointHistory = FMChannelHelper.MakeCall<IPointHistories, PointHistory>(x => x.Get(this.Security, this.Security.UserGuid, this.Security.SiteGuid));

					DateTimeOffset startDate = DateTimeOffset.Parse(startDateString);

                    var pointHistory = new PointHistory();
			        pointHistory.UserGuid = this.Security.UserGuid;
				    pointHistory.SiteGuid = this.Security.SiteGuid;
					pointHistory.StartDate = startDate;
				    pointHistory.IntervalQuantity = intervalQuantity;
					pointHistory.IntervalType = intervalType;
				    pointHistory.RangeQuantity	= rangeQuantity;
					pointHistory.RangeType = rangeType;	
					pointHistory.ColumnsDefinition = columnsDefinition;

                    if (existingPointHistory.PointHistoryGuid == new Guid("00000000-0000-0000-0000-000000000000"))
                    {
                        pointHistory.PointHistoryGuid = Guid.NewGuid();
                        FMChannelHelper.MakeCall<IPointHistories>(x => x.Add(this.Security, pointHistory));
                    }
                    else
                    {
                        pointHistory.PointHistoryGuid = existingPointHistory.PointHistoryGuid;
                        FMChannelHelper.MakeCall<IPointHistories>(x => x.Modify(this.Security, pointHistory));
                    }
                }

                return this.JsonWithErrorMessages(new { isSaved = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e) {
                this.OnError(e);
                return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GetOperatePointHistory() 
		{
            try
            {
                var pointHistoryObject = new PointHistoryTabModel();
                var pointHistory = FMChannelHelper.MakeCall<IPointHistories, PointHistory>(x => x.Get(this.Security, this.Security.UserGuid, this.Security.SiteGuid));

				if (pointHistory.ColumnsDefinition != null) 
				{ //return what was in the database
					pointHistoryObject.Start = pointHistory.StartDate.ToString("MM/d/yyyy hh:mm tt");
					pointHistoryObject.IntervalQuantity = pointHistory.IntervalQuantity;
					pointHistoryObject.Interval = (PointHistoryInterval)pointHistory.IntervalType;
					pointHistoryObject.RangeQuantity = pointHistory.RangeQuantity;
					pointHistoryObject.Range = (PointHistoryRange)pointHistory.RangeType;
					pointHistoryObject.Columns = pointHistory.ColumnsDefinition;
				}
				else
				{ //return default values
                    pointHistoryObject.Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).ToString("MM/d/yyyy hh:mm tt");
                    pointHistoryObject.IntervalQuantity = 1;
                    pointHistoryObject.Interval = PointHistoryInterval.Hour;
                    pointHistoryObject.RangeQuantity = 1;
                    pointHistoryObject.Range = PointHistoryRange.Day;
                    pointHistoryObject.Columns = "";
                }
                return this.JsonWithErrorMessages(pointHistoryObject, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                this.OnError(e);
                return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
            }
        }


		/// <summary>
		/// This method is called by the Movement Summary Tab JS to get a list of movements that the user
		/// can selected from. If a movement has already been add to the movement summary, then it will
		/// exclude that movement.
		/// </summary>
		/// <param name="ID">The list of existing movement names that have been added on the UI.</param>
		/// <param name="parentControl">The parent control which is defaulted to empty.</param>
		/// <param name="persistChanges">The flag to persist changes which is defaulted to false.</param>
		/// <returns>Return a partial view of a movement list dialog.</returns>
		[HttpGet, ValidateJsonAntiForgeryToken]
		public PartialViewResult GetListOfMovementSummaryPointsPartialView(List<string> ID, string parentControl = "", bool persistChanges = false)
		{
			try
			{
				List<Point> allPoints = FMChannelHelper.MakeCall<IPoints, List<Point>>(
									x => x.EnumerateForSummaryWithCategories(this.Security, this.Security.SiteGuid, includeDictionaries: false, applyPointAccess: true));

				var model = new MovementSummaryFilterModel
				{
					PersistChanges = persistChanges,
					ParentControl = parentControl
				};

				model.MovementPoints = new List<Point>();

				// Retrieve the all the movement points.
                foreach (Point point in allPoints)
                {
                    if (point.PointType.ToLower() == "movement")
                    {
                        if (ID == null || ID.Count == 0)
                        {
                            model.MovementPoints.Add(point);
                        }
                        else
                        {
                            // Only add if the movement name does not already exist based on the list
                            // from the UI.
                            if (ID.Contains(point.ID) == false)
                            {
                                model.MovementPoints.Add(point);
                            }
                        }
                    }
                }

                // Populate the model
                var allCategoryList = new List<SelectListItem>();
				var pointTypeList = new List<SelectListItem>();

				// get list of categories for filter
				ApplicationStringCollectionClass categories = new ApplicationStringCollectionClass();

				if (model.ActionListCategories.Count == 0)
				{
					categories = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
																x => x.EnumerateByType(this.Security, STRING_TYPE.POINT_CATEGORY));
				}

				var categorySelectItem = new SelectListItem { Value = "-99", Text = "Select Category" };
				allCategoryList.Add(categorySelectItem);

				foreach (var category in categories)
				{
					categorySelectItem = new SelectListItem { Value = category.ID, Text = category.ID };
					allCategoryList.Add(categorySelectItem);
				}

				model.ActionListCategories = allCategoryList;

				// get list of point types
				if (model.ActionListPointTypes.Count == 0)
				{
					var pointTypes = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
														x => x.EnumerateByType(this.Security, STRING_TYPE.POINT_TEMPLATE_TYPE));

					SelectListItem pointTypeSelectItem;

					// Only populate the movement type.
					foreach (var pointType in pointTypes)
					{
						if (pointType.ID.ToUpper() == "MOVEMENT")
						{
							pointTypeSelectItem = new SelectListItem { Value = pointType.ID, Text = pointType.ID };
							pointTypeList.Add(pointTypeSelectItem);
							break;
						}
					}

					if(pointTypeList.Count == 0)
                    {
						pointTypeSelectItem = new SelectListItem { Value = "-99", Text = "Select Point Type" };
						pointTypeList.Add(pointTypeSelectItem);
					}

					model.ActionListPointTypes = pointTypeList;
				}


				return this.PartialView("MovementSummaryMenuPointSelection", model);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				throw;
			}
		}


		[HttpGet, ValidateJsonAntiForgeryToken]
		public PartialViewResult GetListOfPointsPartialView(bool persistChanges = false, string parentControl = "")
		{
			try
			{
				var model = new PointsFilterModel
				{
					PersistChanges = persistChanges,
					ParentControl = parentControl,
					Points = FMChannelHelper.MakeCall<IPoints, List<Point>>(
										x => x.EnumerateForSummaryWithCategories(this.Security, this.Security.SiteGuid, includeDictionaries: false, applyPointAccess: true))
				};

				// Populate the model

				var allCategoryList = new List<SelectListItem>();
				var pointTypeList = new List<SelectListItem>();
				// get list of categories for filter
				ApplicationStringCollectionClass categories = new ApplicationStringCollectionClass();

				if (model.ActionListCategories.Count == 0)
				{
					categories = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
																x => x.EnumerateByType(this.Security, STRING_TYPE.POINT_CATEGORY));
				}

				var categorySelectItem = new SelectListItem { Value = "-99", Text = "Select Category" };
				allCategoryList.Add(categorySelectItem);

				foreach (var category in categories)
				{
					categorySelectItem = new SelectListItem { Value = category.ID, Text = category.ID };
					allCategoryList.Add(categorySelectItem);
				}

				model.ActionListCategories = allCategoryList;
				// get list of point types
				if (model.ActionListPointTypes.Count == 0)
				{
					var pointTypes = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
														x => x.EnumerateByType(this.Security, STRING_TYPE.POINT_TEMPLATE_TYPE));

					var pointTypeSelectItem = new SelectListItem { Value = "-99", Text = "Select Point Type" };
					pointTypeList.Add(pointTypeSelectItem);

					foreach (var pointType in pointTypes)
					{
						pointTypeSelectItem = new SelectListItem { Value = pointType.ID, Text = pointType.ID };
						pointTypeList.Add(pointTypeSelectItem);
					}

					model.ActionListPointTypes = pointTypeList;
				}


				return this.PartialView("PointsMenuSelection", model);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				throw;
			}
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GetListOfTagNamesPartialView()
		{
			try
			{

				// Populate the model
				var model = new TagNameSelectionModel
				{
					TagNames = FMChannelHelper.MakeCall<IPointTemplateTags, List<KeyValuePair<string, string>>>(
														x => x.EnumerateAllUniqueTagNames(this.Security))
				};


				return this.PartialViewWithErrorMessages("TagNameSelection", model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GetListOfMovementTagNamesPartialView()
		{
			try
			{

				// Populate the model
				var model = new TagNameSelectionModel
				{
					TagNames = FMChannelHelper.MakeCall<IPointTemplateTags, List<KeyValuePair<string, string>>>(
													x => x.EnumerateMovementSummaryColumnNames(this.Security))
				};


				return this.PartialViewWithErrorMessages("TagNameSelection", model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GetListOfPointTagsPartialView(string pointGuid)
		{
			try
			{
				// Populate the model
				var model = new PointTagSelectionModel
				{
					PointTags = FMChannelHelper.MakeCall<IPointTags, Dictionary<Guid, PointTag>>(
														x => x.EnumerateByPointGuid(this.Security, new Guid(pointGuid)))
				};


				return this.PartialViewWithErrorMessages("PointTagSelection", model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GetPointTagGuid(string pointTagID, string pointGuid)
		{
			try
			{
				var pointTagGuid = FMChannelHelper.MakeCall<IPointTags, Guid>(x => x.GetIdentityGuid(this.Security, pointTagID, new Guid(pointGuid)));

				return this.JsonWithErrorMessages(pointTagGuid, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public PartialViewResult GetListOfTrendsPartialView()
		{
			try
			{
				var model = new TrendSummaryModel
				{
					Names = FMChannelHelper.MakeCall<ITrends, List<TrendName>>(x => x.EnumerateAvailableTrendNames(this.Security)),
					ModifyTrendsRight = this.Security.HasRight(RIGHT.OPERATE_MODIFY_TRENDS)
				};


				return this.PartialView("TrendMenuSelection", model);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				throw;
			}
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public PartialViewResult GetListOfReportsPartialView()
		{
			try
			{
				var model = new ReportSummaryModel();

				var serviceRequest = new ReportConfigurationDetailSR
				{
					CurrentSiteGuid = this.Security.SiteGuid,
					Security = this.Security
				};

				var reportDetailDo = FMChannelHelper.MakeCall<IReportConfigurationDetailProcessor, ReportConfigurationDetailListDO>
																		(x => x.GetAllNonPrint(serviceRequest));

				foreach (var reportDetail in reportDetailDo.ReportDetailDOList)
				{
					var reportDetailModel = new ReportDetailModel
					{
						Name = reportDetail.ReportName,
						Description = reportDetail.ReportDescription,
						ReportGuid = reportDetail.ReportGuid
					};

					model.ReportDetailList.Add(reportDetailModel);
				}

				return this.PartialView("ReportMenuSelection", model);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				throw;
			}
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GetOperateDrawing(string id, string pointGuidStr)
		{
			try
			{
				var graphicObject = new OperateGraphic();
				Guid drawingGuid;
				Guid pointGuid;

				if (Guid.TryParse(pointGuidStr, out pointGuid) && pointGuid != Guid.Empty)
				{
					graphicObject.PointInformation = this.GetOperatePoint(pointGuid);
					drawingGuid = graphicObject.PointInformation?.PointDetailDrawingGuid ?? Guid.Empty;
				}
				else
				{
					drawingGuid = new Guid(id);
				}
				var drawing = FMChannelHelper.MakeCall<IDrawings, Drawing>(x => x.Get(this.Security, drawingGuid));
				graphicObject.Drawing = drawing.Image;

				if (drawing.IdentityGuid == Guid.Empty)
				{
					this.ModelState.AddModelError("Drawing", TranslateText("Graphic not found."));
				}

				if(drawing.Published.HasValue
				&& !drawing.Published.Value
				&& !this.Security.HasRight(RIGHT.OPERATE_VIEW_UNPUBLISHED))
				{
					this.ModelState.AddModelError("Drawing", TranslateText("Graphic " + drawing.ID + " not published."));
				}

				var drawingGuidList = new List<Guid>();
				drawingGuidList.Add(drawingGuid);
				var drawingToAnimationMaps = FMChannelHelper.MakeCall<IAnimationDrawingMaps, Dictionary<Guid, AnimationToDrawingMapClass>>(x => x.EnumerateByDrawingGuids(this.Security, drawingGuidList));
				var animationGuidList = new List<Guid>();
				foreach (var animationToDrawing in drawingToAnimationMaps.Values)
				{
					animationGuidList.Add(animationToDrawing.AnimationGuid);
				}
				var animationDictionary = FMChannelHelper.MakeCall<IAnimations, Dictionary<Guid, AnimationClass>>(x => x.EnumerateByAnimationGuids(this.Security, animationGuidList));
				graphicObject.Animations = animationDictionary.Values.ToList();

				return this.JsonWithErrorMessages(graphicObject, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				this.OnError(e);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GetOperateTrend(bool pointTrend, string guidString)
		{
			try
			{
				if (!pointTrend)
				{
					var trendGuid = new Guid(guidString);
					var trend = FMChannelHelper.MakeCall<ITrends, Trend>(x => x.Get(this.Security, trendGuid));
					if (trend.TrendGuid == Guid.Empty)
					{
						this.ModelState.AddModelError("Trend", TranslateText("Trend not found."));

					}
					return this.JsonWithErrorMessages(trend, JsonRequestBehavior.AllowGet);
				}
				else
				{
					var pointGuid = new Guid(guidString);
					var trend = FMChannelHelper.MakeCall<ITrends, Trend>(x => x.GetPointTrend(this.Security, pointGuid));
					if (trend == null)
					{
						this.ModelState.AddModelError("Trend", TranslateText("Point Detail Trend has to be initially opened/created at the client site."));
					}
					else if (trend.PointTemplateGuid == Guid.Empty)
					{
						this.ModelState.AddModelError("Trend", TranslateText("Trend not found."));

					}
					return this.JsonWithErrorMessages(trend, JsonRequestBehavior.AllowGet);
				}
			}
			catch (Exception e)
			{
				this.OnError(e);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		/// <summary>
		/// This method will return report detail information.
		/// </summary>
		/// <param name="reportName"></param>
		/// <param name="reportGuidStr"></param>
		/// <returns></returns>
		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GetOperateReport(string reportName, string reportGuidStr)
		{
			try
			{
				if (string.IsNullOrEmpty(reportGuidStr))
				{
					this.ModelState.AddModelError("Report", TranslateText("Report not found") + " (" + reportName + ").");
					return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
				}

				Guid reportGuid;

				if (Guid.TryParse(reportGuidStr, out reportGuid) == false)
				{
					this.ModelState.AddModelError("Report", TranslateText("Report not found" + " (" + reportName + ")."));
					return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
				}

				var serviceRequest = new ReportConfigurationDetailSR
				{
					CurrentSiteGuid = this.Security.SiteGuid,
					Security = this.Security,
					ReportConfigurationDetailDO = new ReportConfigurationDetailDO { ReportGuid = reportGuid }
				};

				var reportConfigDo = FMChannelHelper.MakeCall<IReportConfigurationDetailProcessor, ReportConfigurationDetailDO>
																		(x => x.GetConfiguration(serviceRequest));

				if (string.IsNullOrEmpty(reportConfigDo.ReportPath))
				{
					this.ModelState.AddModelError("Report", TranslateText("Report not found") + " (" + reportName + ").");
					return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
				}

				string actualReportName = reportConfigDo.ReportPath;
				string csrfToken = this.Security.CSRFToken;
				string reportInfo = actualReportName + "|" + csrfToken;

				return this.JsonWithErrorMessages(reportInfo, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				this.OnError(e);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

        [HttpGet, ValidateJsonAntiForgeryToken]
        public PartialViewResult GetListOfPointHistoryPartialView(bool persistChanges = false, string parentControl = "")
        {
            try
            {
                var model = new PointsFilterModel
                {
                    PersistChanges = persistChanges,
                    ParentControl = parentControl,
                    Points = FMChannelHelper.MakeCall<IPoints, List<Point>>(
                                        x => x.EnumerateForSummaryWithCategories(this.Security, this.Security.SiteGuid, includeDictionaries: false, applyPointAccess: true))
                };

                // Populate the model

                var allCategoryList = new List<SelectListItem>();
                var pointTypeList = new List<SelectListItem>();
                // get list of categories for filter
                ApplicationStringCollectionClass categories = new ApplicationStringCollectionClass();

                if (model.ActionListCategories.Count == 0)
                {
                    categories = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
                                                                x => x.EnumerateByType(this.Security, STRING_TYPE.POINT_CATEGORY));
                }

                var categorySelectItem = new SelectListItem { Value = "-99", Text = "Select Category" };
                allCategoryList.Add(categorySelectItem);

                foreach (var category in categories)
                {
                    categorySelectItem = new SelectListItem { Value = category.ID, Text = category.ID };
                    allCategoryList.Add(categorySelectItem);
                }

                model.ActionListCategories = allCategoryList;
                // get list of point types
                if (model.ActionListPointTypes.Count == 0)
                {
                    var pointTypes = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
                                                        x => x.EnumerateByType(this.Security, STRING_TYPE.POINT_TEMPLATE_TYPE));

                    var pointTypeSelectItem = new SelectListItem { Value = "-99", Text = "Select Point Type" };
                    pointTypeList.Add(pointTypeSelectItem);

                    foreach (var pointType in pointTypes)
                    {
                        pointTypeSelectItem = new SelectListItem { Value = pointType.ID, Text = pointType.ID };
                        pointTypeList.Add(pointTypeSelectItem);
                    }

                    model.ActionListPointTypes = pointTypeList;
                }


                return this.PartialView("PointHistoryMenuSelection", model);
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
                throw;
            }
        }

        [HttpGet, ValidateJsonAntiForgeryToken]
		public PartialViewResult GetListOfPointsCalculatorPartialView(bool persistChanges = false, string parentControl = "")
		{
			try
            {
                // Populate the model
                PointsFilterModel model = PopulateModelWithTankPoints(persistChanges, parentControl, new string[] { "StandardTankCalculator.FMStandardTankCalculator" });

                return this.PartialView("PointsCalculatorMenu", model);
            }
            catch (Exception except)
			{
				this.ErrorHandler(except);
				throw;
			}
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public PartialViewResult GetListOfPointsLeakAnalsisPartialView(bool persistChanges = false, string parentControl = "")
		{
			try
			{
				// Populate the model
				PointsFilterModel model = PopulateModelWithTankPoints(persistChanges, parentControl, new string[] { "LeakDetection.FMLeakDetection" } );


				return this.PartialView("PointsLeakAnalysisMenu", model);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				throw;
			}
		}

		private PointsFilterModel PopulateModelWithTankPoints(bool persistChanges, string parentControl, string[] moduleTypeNames)
        {

			var pointTemplateGuidList = FMChannelHelper.MakeCall<IModules, List<Guid>>(x => x.EnumeratePointTemplatesByAnyModuleTypeNames(this.Security, moduleTypeNames));

			PointCollection points = new PointCollection();

			if (pointTemplateGuidList.Count > 0)
			{
				points = FMChannelHelper.MakeCall<IPoints, PointCollection>(x => x.EnumerateByPointTemplateGuids(this.Security, pointTemplateGuidList.ToArray()));
			}

			var model = new PointsFilterModel
            {
                PersistChanges = persistChanges,
                ParentControl = parentControl,
                Points = points
            };


            var allCategoryList = new List<SelectListItem>();
            var pointTypeList = new List<SelectListItem>();
            // get list of categories for filter
            ApplicationStringCollectionClass categories = new ApplicationStringCollectionClass();

            if (model.ActionListCategories.Count == 0)
            {
                categories = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
                                                            x => x.EnumerateByType(this.Security, STRING_TYPE.POINT_CATEGORY));
            }

            var categorySelectItem = new SelectListItem { Value = "-99", Text = "Select Category" };
            allCategoryList.Add(categorySelectItem);

            foreach (var category in categories)
            {
                categorySelectItem = new SelectListItem { Value = category.ID, Text = category.ID };
                allCategoryList.Add(categorySelectItem);
            }

            model.ActionListCategories = allCategoryList;
            // get list of point types
            if (model.ActionListPointTypes.Count == 0)
            {
                var pointTypes = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
                                                    x => x.EnumerateByType(this.Security, STRING_TYPE.POINT_TEMPLATE_TYPE));

                // this is being left in because we may want to allow calculator function on different point types
                // if not then I will delete it
                var pointTypeSelectItem = new SelectListItem { Value = "-99", Text = "Select Point Type" };
                //pointTypeList.Add(pointTypeSelectItem);

                foreach (var pointType in pointTypes)
                {
                    if (pointType.ID == "Tank")
                    {
                        pointTypeSelectItem = new SelectListItem { Value = pointType.ID, Text = pointType.ID };
                        pointTypeList.Add(pointTypeSelectItem);
                    }
                }

                model.ActionListPointTypes = pointTypeList;
            }

            return model;
        }



		public OperatePoint GetOperatePoint(Guid pointGuid)
		{
			var point = FMChannelHelper.MakeCall<IPoints, Point>(x => x.Get(this.Security, pointGuid));
			if (point.PointGuid == Guid.Empty)
			{
				this.ModelState.AddModelError("Point", TranslateText("Point not found."));

			}


			var pointTemplate = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.Get(this.Security, point.PointTemplateGuid));
			if (pointTemplate.PointTemplateGuid == Guid.Empty)
			{
				this.ModelState.AddModelError("PointTemplate", TranslateText("Point Template not found."));

			}


			var operatePoint = new OperatePoint { PointID = point.ID, PointGuid = point.PointGuid, PointTemplateGuid = point.PointTemplateGuid, PointDetailDrawingGuid = point.PointDetailDrawingGuid };
			var pointValues = new List<PointValue>();

			foreach (var tag in point.Tags.Values)
			{
				if ((tag.Value is double || tag.Value is float) && Double.IsNaN(Convert.ToDouble(tag.Value)))
				{
					tag.Value = "NaN";
				}
				pointValues.Add(new PointValue(tag));
			}

			foreach (var value in point.GetExposedSettings())
			{
				pointValues.Add(value);
			}

			foreach (var pointProperty in point.Properties.Values)
			{
				foreach (var pointValueIdentifier in pointProperty.GetExposedSettingPointValueIdentifiers())
				{
					pointValues.Add(new PointValue(pointValueIdentifier, pointProperty, point));
				}
			}
			operatePoint.Values = pointValues;
			return operatePoint;
		}


		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult DeleteOperateTrend(string id)
		{
			try
			{
				var trendGuid = new Guid(id);
				FMChannelHelper.MakeCall<ITrends>(x => x.Purge(this.Security, trendGuid));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				this.OnError(e);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}



		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult SaveTrend(string trendString)
		{
			try
			{
				if (!string.IsNullOrEmpty(trendString))
				{
					JavaScriptSerializer jss = new JavaScriptSerializer();
					var trend = jss.Deserialize<Trend>(trendString);

					FMChannelHelper.MakeCall<ITrends>(x => x.Modify(this.Security, trend));
				}
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				this.OnError(new Exception(this.GetTranslatedText("Error Saving Trend : " + e.Message)));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}


		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GetUnitsByUnitType(int unitType)
		{
			List<TagUnitToUnitType> tagUnitToUnitTypeList = new List<TagUnitToUnitType>();
			try
			{
				EngineeringUnitType unitTypeValue = (EngineeringUnitType)unitType;

				var unitList = EngineeringUnits.GetUnitsByType(unitTypeValue);
				foreach (var unit in unitList)
				{
					tagUnitToUnitTypeList.Add(
						new TagUnitToUnitType
						{
							Unit = (int)unit,
							UnitStr = this.GetTranslatedText(unit.ToString()),
							UnitDescription = this.GetTranslatedText(EngineeringUnits.GetUnitString(unit)),
							UnitAbbreviation = this.GetTranslatedText(EngineeringUnits.GetUnitAbbreviation(unit))
						});
				}

				return this.JsonWithErrorMessages(tagUnitToUnitTypeList, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GetOptionsForEnumValueType(string valueType)
		{

			try
			{
				if (string.IsNullOrEmpty(valueType))
				{
					throw new ArgumentNullException(valueType, "Value Type is missing");
				}
				var enumList = FMBaseController.GetEnumSelectList(valueType);
				return this.JsonWithErrorMessages(enumList, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}


		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult SavePointGroupSchedule(string pointGroupGuid, string cronschedule, string startschedule, string endschedule, string printer, string emailto, PointGroupSchedule.LayoutType layout, PointGroupSchedule.ExportFileType fileType, bool exportOptions, bool fitToPage)
		{
			try
			{
				if (this.ModelState.IsValid)
				{
					Boolean addSchedule = false;
					var pointGroupSchedule = FMChannelHelper.MakeCall<IPointGroupSchedules, PointGroupSchedule>(x => x.Get(this.Security, Guid.Parse(pointGroupGuid), this.Security.UserGuid, this.Security.SiteGuid));
                    var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));
                    if (pointGroupSchedule.PointGroupScheduleGuid == Guid.Empty)
					{
						addSchedule = true;
						pointGroupSchedule = new PointGroupSchedule();
						pointGroupSchedule.PointGroupScheduleGuid = Guid.NewGuid();
						pointGroupSchedule.PointGroupGuid = Guid.Parse(pointGroupGuid);
						pointGroupSchedule.SiteGuid = this.Security.SiteGuid;
						pointGroupSchedule.UserGuid = this.Security.UserGuid;
					}
					pointGroupSchedule.CronSchedule = cronschedule;

					 TimeZoneInfo sitesTimezone = TimeZoneInfo.FindSystemTimeZoneById(site.TimeZone);
					 double systemTimezoneOffset = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.Now).TotalMinutes;
					 double timezoneOffset = sitesTimezone.GetUtcOffset(DateTimeOffset.Now).TotalMinutes;
					 DateTime scheduleTime = DateTime.Parse(startschedule);

					 // Use the difference in offset between the site time and system time to get the server timezone
					 scheduleTime = scheduleTime.AddMinutes(-(timezoneOffset - systemTimezoneOffset));
					 pointGroupSchedule.StartSchedule = scheduleTime;
					pointGroupSchedule.EndSchedule = endschedule;
					pointGroupSchedule.Printer = printer;
					pointGroupSchedule.EmailTo = emailto;
					pointGroupSchedule.Layout = layout;
					pointGroupSchedule.ExportFileFormat = fileType;
					pointGroupSchedule.CreateNewExportFile = !exportOptions;
					pointGroupSchedule.FitToPage = fitToPage;

					if (addSchedule)
					{
						FMChannelHelper.MakeCall<IPointGroupSchedules, Guid>(x => x.Add(this.Security, pointGroupSchedule));
					}
					else
					{
						FMChannelHelper.MakeCall<IPointGroupSchedules>(x => x.Modify(this.Security, pointGroupSchedule));
					}

					this.ModelState.Clear();
					this.AddSuccess(this.GetTranslatedText("Save Successful"));
					return this.JsonWithErrorMessages(new { pointGroupGuid = pointGroupGuid }, JsonRequestBehavior.AllowGet);
				}
				return this.JsonWithErrorMessages(new { pointGroupGuid = pointGroupGuid }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				this.OnError(new Exception(this.GetTranslatedText("Error Saving Point Group Schedule: " + e.Message)));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult DeletePointGroupSchedule(string pointGroupGuid)
		{
			try
			{
				if (this.ModelState.IsValid)
				{

					FMChannelHelper.MakeCall<IPointGroupSchedules>(x => x.Purge(this.Security, Guid.Parse(pointGroupGuid), this.Security.UserGuid, this.Security.SiteGuid));

					this.ModelState.Clear();
					this.AddSuccess(this.GetTranslatedText("Delete Successful"));
					return this.JsonWithErrorMessages(new { pointGroupGuid = pointGroupGuid }, JsonRequestBehavior.AllowGet);
				}
				return this.JsonWithErrorMessages(new { pointGroupGuid = pointGroupGuid }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				this.OnError(new Exception(this.GetTranslatedText("Error Deleting Point Group Schedule: " + e.Message)));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult GetPointGroupSchedule(string pointGroupGuid)
		{
			try
			{
				if (this.ModelState.IsValid)
				{
                var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

                var pointGroupSchedule = FMChannelHelper.MakeCall<IPointGroupSchedules, PointGroupSchedule>(x => x.Get(this.Security, Guid.Parse(pointGroupGuid), this.Security.UserGuid, this.Security.SiteGuid));

                // IPointGroupSchedules.Get creates a new empty pointGroupSchedule if one is not found in the db. Don't try to parse pointGroupSchedule.StartSchedule in this case
                if (pointGroupSchedule.IdentityGuid != Guid.Empty)
					 {
						  TimeZoneInfo sitesTimezone = TimeZoneInfo.FindSystemTimeZoneById(site.TimeZone);
						  double systemTimezoneOffset = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.Now).TotalMinutes;
						  double timezoneOffset = sitesTimezone.GetUtcOffset(DateTimeOffset.Now).TotalMinutes;

						  // Use the difference in offset between system time and site time to get the site timezone
						  pointGroupSchedule.StartSchedule = pointGroupSchedule.StartSchedule.AddMinutes(-(systemTimezoneOffset - timezoneOffset));
					 }

                this.ModelState.Clear();

					var pointGroupScheduleModel = new OperatePointGroupScheduleModel();
					pointGroupScheduleModel.PointGroupSchedule = pointGroupSchedule;
					pointGroupScheduleModel.Printers = FMBusinessObjects.UtilityObjects.ReportServicePrintService.EnumeratePrinters("Point Group Schedule Report").ToList();

					return this.JsonWithErrorMessages(pointGroupScheduleModel, JsonRequestBehavior.AllowGet);
				}
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				this.OnError(new Exception(this.GetTranslatedText("Error Getting Point Group Schedule: " + e.Message)));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult StopOperateStatistics(string windowName)
		{
			try
			{
				FMChannelHelper.MakeCall<ISessions>(x => x.MarkScreenUsingOperate(this.Security, windowName, false));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				this.OnError(new Exception(this.GetTranslatedText("Error stopping operate statistics : " + e.Message)));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult SaveStatistics(OperateStatistics statistics)
		{
			try
			{
				FMChannelHelper.MakeCall<ISessions>(x => x.SaveSessionOperateStatistics(this.Security, statistics));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				this.OnError(new Exception(this.GetTranslatedText("Error saving operate statistics : " + e.Message)));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}
   }

	internal class PointGroupRowMetadata
	{
		public Guid PointValueIdentifier_IdentityGuid { get; }
		public PointValueType PointValueIdentifier_PointValueType { get; }
		public string PointValueIdentifier_PropertyID { get; }
		public long PointValueIdentifier_UtcTicks { get; }
		public Guid PointGuid { get; }
		public Guid PointTagGuid { get; }
		public string ID { get; }
		public EngineeringUnit Units { get; }
		public double Maximum { get; }
		public double Minimum { get; }
		public byte DecimalPlaces { get; }
		public EngineeringUnitType EngineeringUnitsType { get; }
		public bool InhibitOverride { get; }
		public Guid WellKnownIdentityGuid { get; }
		public PointTemplateTag.PointTagInputOutputType InputOutputType { get; }
		public long Status { get; }
		public bool CommunicationsFailure { get; }

		public PointGroupRowMetadata(Guid pointValueIdentifier_IdentityGuid, PointValueType pointValueIdentifier_PointValueType, string pointValueIdentifier_PropertyID, long pointValueIdentifer_UtcTicks, Guid pointGuid, Guid pointTagGuid, string iD, EngineeringUnit units, double maximum, double minimum, byte decimalPlaces, EngineeringUnitType engineeringUnitsType, bool inhibitOverride, Guid wellKnownIdentityGuid, PointTemplateTag.PointTagInputOutputType inputOutputType, long status, bool communicationsFailure)
		{
			this.PointValueIdentifier_IdentityGuid = pointValueIdentifier_IdentityGuid;
			this.PointValueIdentifier_PointValueType = pointValueIdentifier_PointValueType;
			this.PointValueIdentifier_PropertyID = pointValueIdentifier_PropertyID;
			this.PointValueIdentifier_UtcTicks = pointValueIdentifer_UtcTicks;
			this.PointGuid = pointGuid;
			this.PointTagGuid = pointTagGuid;
			this.ID = iD;
			this.Units = units;
			this.Maximum = maximum;
			this.Minimum = minimum;
			this.DecimalPlaces = decimalPlaces;
			this.EngineeringUnitsType = engineeringUnitsType;
			this.InhibitOverride = inhibitOverride;
			this.WellKnownIdentityGuid = wellKnownIdentityGuid;
			this.InputOutputType = inputOutputType;
			this.Status = status;
			this.CommunicationsFailure = communicationsFailure;
		}

		public override bool Equals(object obj)
		{
			return obj is PointGroupRowMetadata other &&
					 this.PointValueIdentifier_IdentityGuid.Equals(other.PointValueIdentifier_IdentityGuid) &&
					 this.PointValueIdentifier_PointValueType == other.PointValueIdentifier_PointValueType &&
					 this.PointValueIdentifier_PropertyID == other.PointValueIdentifier_PropertyID &&
					 this.PointValueIdentifier_UtcTicks == other.PointValueIdentifier_UtcTicks &&
					 this.PointGuid.Equals(other.PointGuid) &&
					 this.PointTagGuid.Equals(other.PointTagGuid) &&
					 this.ID == other.ID &&
					 this.Units == other.Units &&
					 this.Maximum == other.Maximum &&
					 this.Minimum == other.Minimum &&
					 this.DecimalPlaces == other.DecimalPlaces &&
					 this.EngineeringUnitsType == other.EngineeringUnitsType &&
					 this.InhibitOverride == other.InhibitOverride &&
					 this.WellKnownIdentityGuid.Equals(other.WellKnownIdentityGuid) &&
					 this.InputOutputType == other.InputOutputType &&
					 this.Status == other.Status &&
					 this.CommunicationsFailure == other.CommunicationsFailure;
		}

		public override int GetHashCode()
		{
			int hashCode = 663868232;
			hashCode = hashCode * -1521134295 + this.PointValueIdentifier_IdentityGuid.GetHashCode();
			hashCode = hashCode * -1521134295 + this.PointValueIdentifier_PointValueType.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.PointValueIdentifier_PropertyID);
			hashCode = hashCode * -1521134295 + this.PointValueIdentifier_UtcTicks.GetHashCode();
			hashCode = hashCode * -1521134295 + this.PointGuid.GetHashCode();
			hashCode = hashCode * -1521134295 + this.PointTagGuid.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.ID);
			hashCode = hashCode * -1521134295 + this.Units.GetHashCode();
			hashCode = hashCode * -1521134295 + this.Maximum.GetHashCode();
			hashCode = hashCode * -1521134295 + this.Minimum.GetHashCode();
			hashCode = hashCode * -1521134295 + this.DecimalPlaces.GetHashCode();
			hashCode = hashCode * -1521134295 + this.EngineeringUnitsType.GetHashCode();
			hashCode = hashCode * -1521134295 + this.InhibitOverride.GetHashCode();
			hashCode = hashCode * -1521134295 + this.WellKnownIdentityGuid.GetHashCode();
			hashCode = hashCode * -1521134295 + this.InputOutputType.GetHashCode();
			hashCode = hashCode * -1521134295 + this.Status.GetHashCode();
			hashCode = hashCode * -1521134295 + this.CommunicationsFailure.GetHashCode();
			return hashCode;
		}
	}

    internal class PointHistoryRowMetadata
    {
        public Guid PointValueIdentifier_IdentityGuid { get; }
        public PointValueType PointValueIdentifier_PointValueType { get; }
        public string PointValueIdentifier_PropertyID { get; }
        public long PointValueIdentifier_UtcTicks { get; }
        public Guid PointGuid { get; }
        public Guid PointTagGuid { get; }
        public string ID { get; }
        public EngineeringUnit Units { get; }
        public double Maximum { get; }
        public double Minimum { get; }
        public byte DecimalPlaces { get; }
        public EngineeringUnitType EngineeringUnitsType { get; }
        public bool InhibitOverride { get; }
        public Guid WellKnownIdentityGuid { get; }
        public PointTemplateTag.PointTagInputOutputType InputOutputType { get; }
        public long Status { get; }
        public bool CommunicationsFailure { get; }
		public string ValueTypeString { get;}

        public PointHistoryRowMetadata(Guid pointValueIdentifier_IdentityGuid, PointValueType pointValueIdentifier_PointValueType, string pointValueIdentifier_PropertyID, long pointValueIdentifer_UtcTicks, Guid pointGuid, Guid pointTagGuid, string iD, EngineeringUnit units, double maximum, double minimum, byte decimalPlaces, EngineeringUnitType engineeringUnitsType, bool inhibitOverride, Guid wellKnownIdentityGuid, PointTemplateTag.PointTagInputOutputType inputOutputType, long status, bool communicationsFailure, string valueTypeString)
        {
            this.PointValueIdentifier_IdentityGuid = pointValueIdentifier_IdentityGuid;
            this.PointValueIdentifier_PointValueType = pointValueIdentifier_PointValueType;
            this.PointValueIdentifier_PropertyID = pointValueIdentifier_PropertyID;
            this.PointValueIdentifier_UtcTicks = pointValueIdentifer_UtcTicks;
            this.PointGuid = pointGuid;
            this.PointTagGuid = pointTagGuid;
            this.ID = iD;
            this.Units = units;
            this.Maximum = maximum;
            this.Minimum = minimum;
            this.DecimalPlaces = decimalPlaces;
            this.EngineeringUnitsType = engineeringUnitsType;
            this.InhibitOverride = inhibitOverride;
            this.WellKnownIdentityGuid = wellKnownIdentityGuid;
            this.InputOutputType = inputOutputType;
            this.Status = status;
            this.CommunicationsFailure = communicationsFailure;
            this.ValueTypeString = valueTypeString;
        }

        public override bool Equals(object obj)
        {
            return obj is PointHistoryRowMetadata other &&
                     this.PointValueIdentifier_IdentityGuid.Equals(other.PointValueIdentifier_IdentityGuid) &&
                     this.PointValueIdentifier_PointValueType == other.PointValueIdentifier_PointValueType &&
                     this.PointValueIdentifier_PropertyID == other.PointValueIdentifier_PropertyID &&
                     this.PointValueIdentifier_UtcTicks == other.PointValueIdentifier_UtcTicks &&
                     this.PointGuid.Equals(other.PointGuid) &&
                     this.PointTagGuid.Equals(other.PointTagGuid) &&
                     this.ID == other.ID &&
                     this.Units == other.Units &&
                     this.Maximum == other.Maximum &&
                     this.Minimum == other.Minimum &&
                     this.DecimalPlaces == other.DecimalPlaces &&
                     this.EngineeringUnitsType == other.EngineeringUnitsType &&
                     this.InhibitOverride == other.InhibitOverride &&
                     this.WellKnownIdentityGuid.Equals(other.WellKnownIdentityGuid) &&
                     this.InputOutputType == other.InputOutputType &&
                     this.Status == other.Status &&
                     this.CommunicationsFailure == other.CommunicationsFailure &&
		             this.ValueTypeString == other.ValueTypeString;
        }

        public override int GetHashCode()
        {
            int hashCode = 663868232;
            hashCode = hashCode * -1521134295 + this.PointValueIdentifier_IdentityGuid.GetHashCode();
            hashCode = hashCode * -1521134295 + this.PointValueIdentifier_PointValueType.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.PointValueIdentifier_PropertyID);
            hashCode = hashCode * -1521134295 + this.PointValueIdentifier_UtcTicks.GetHashCode();
            hashCode = hashCode * -1521134295 + this.PointGuid.GetHashCode();
            hashCode = hashCode * -1521134295 + this.PointTagGuid.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.ID);
            hashCode = hashCode * -1521134295 + this.Units.GetHashCode();
            hashCode = hashCode * -1521134295 + this.Maximum.GetHashCode();
            hashCode = hashCode * -1521134295 + this.Minimum.GetHashCode();
            hashCode = hashCode * -1521134295 + this.DecimalPlaces.GetHashCode();
            hashCode = hashCode * -1521134295 + this.EngineeringUnitsType.GetHashCode();
            hashCode = hashCode * -1521134295 + this.InhibitOverride.GetHashCode();
            hashCode = hashCode * -1521134295 + this.WellKnownIdentityGuid.GetHashCode();
            hashCode = hashCode * -1521134295 + this.InputOutputType.GetHashCode();
            hashCode = hashCode * -1521134295 + this.Status.GetHashCode();
            hashCode = hashCode * -1521134295 + this.CommunicationsFailure.GetHashCode();
            hashCode = hashCode * -1521134295 + this.ValueTypeString.GetHashCode();
            return hashCode;
        }
    }
}
