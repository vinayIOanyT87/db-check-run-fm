

namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.InventoryManagement.ViewModels;

	using Newtonsoft.Json;

	public class AlarmHistoryTabController : FMBaseControllerEx
	{
		protected const string AlarmHistoryTabID = "AlarmHistoryTab";

		/// <summary>
		/// Identifies the data dictionary keys needed for this item.
		/// </summary>
		/// <param name="model">The model to serialize</param>
		/// <returns>An array of data dictionary keys.</returns>
		[NonAction]
		public static string SerializeModel(AlarmHistoryTabModel model)
		{
			return JsonConvert.SerializeObject(model);
		}

		/// <summary>
		/// Identifies the data dictionary keys needed for this item.
		/// </summary>
		/// <param name="modelStr">The model to serialize</param>
		/// <returns>An array of data dictionary keys.</returns>
		[NonAction]
		public static AlarmHistoryTabModel DeserializeModel(string modelStr)
		{
			var jsonSerializerSettings = new JsonSerializerSettings
			{
				MissingMemberHandling = MissingMemberHandling.Ignore
			};

			var obj = JsonConvert.DeserializeObject<AlarmHistoryTabModel>(modelStr, jsonSerializerSettings);
			return obj;
		}

		/// <summary>
		/// This method will return alarm history items in a model.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="columnFilterInfoList"></param>
		/// <returns>Returns the alarm history model.</returns>
		[NonAction]
		public static AlarmHistoryTabModel GetModel(SecurityClass security, List<AlarmHistoryTabColumnFilterInfo> columnFilterInfoList, int recordTypeFilter)
		{
			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(security, security.SiteGuid, false, false, false));
			TimeZoneInfo sitesTimezone = TimeZoneInfo.FindSystemTimeZoneById(site.TimeZone);
			var model = new AlarmHistoryTabModel { AlarmHistories = new List<AlarmHistoryTabRow>(), Site = site };

			// read the data from Cassandra
			List<AandEDataElement> archiveDataList = FMChannelHelper.MakeCall<IAandEArchive, List<AandEDataElement>>
														(x => x.GetAandEArchiveData(security, columnFilterInfoList, recordTypeFilter));

			foreach(var archivedata in archiveDataList)
			{
				var alarmAndEventRecordGuid = archivedata.RecordGuid.ToString();
				var row = new AlarmHistoryTabRow
				{
					DT_RowId				= "Row_" + alarmAndEventRecordGuid,
					AlarmAndEventRecordGuid = alarmAndEventRecordGuid,
					AlarmOrTagGuid				= archivedata.AlarmOrTagGuid.ToString(),
					AlarmTestGuid			= archivedata.AlarmTestGuid.ToString(),
					AlarmState				= archivedata.AlarmState,
					Action					= archivedata.Action,
					Comments				= archivedata.Comments,
					DateAndTime				= ConvertArchiveDateTimeToLocalTime(archivedata.DateAndTime.DateTime, site),
					Point					= archivedata.Point,
					PointDescription		= archivedata.PointDescription,
					Priority				= archivedata.Priority,
					PointType				= archivedata.PointType,
					Site					= archivedata.Site,
					Units					= GetEngineeringUnitsAbbreviation(archivedata.Units),
					User					= archivedata.User,
					Value					= ConvertLevelValue(archivedata.Units, archivedata.Value),
					Variable				= archivedata.Variable,
					CommentDateTime			= ConvertArchiveDateTimeToLocalTime(archivedata.CommentDateTime.DateTime, site),
					CommentUserName			= archivedata.CommentUser,
					DateAndTimeTicks		= archivedata.DateAndTime.Ticks,
					CommentDateTimeTicks	= archivedata.CommentDateTime.Ticks
				};

				if (String.IsNullOrWhiteSpace(row.Comments) || row.CommentDateTime.Contains("1/1/0001"))
				{
					row.CommentDateTime = "";
				}

				model.AlarmHistories.Add(row);
			}

			return model;
		}

		[NonAction]
		protected static void SaveViewStateSettings(SecurityClass security, AlarmHistoryUserViewStateSettings alarmHistoryViewStateSettings)
		{
			if (alarmHistoryViewStateSettings != null)
			{
				var userSettings = FMChannelHelper.MakeCall<IUserViewStateSettings, UserViewStateSettingCollection>(x => x.EnumerateBySiteUserClientIpAddressWindowNameAndViewID(security, security.SiteGuid, security.UserGuid, "", AlarmHistoryTabID));
				if (userSettings == null || userSettings.Count <= 0)
				{
					var userSetting = new UserViewStateSetting(security);
					userSetting.Value = alarmHistoryViewStateSettings;
					userSetting.ViewID = AlarmHistoryTabID;
					FMChannelHelper.MakeCall<IUserViewStateSettings>(x => x.Add(security, userSetting));
				}
				else
				{
					var userSetting = userSettings[0];
					userSetting.Value = alarmHistoryViewStateSettings;
					FMChannelHelper.MakeCall<IUserViewStateSettings>(x => x.Modify(security, userSetting));
				}
			}
		}

		[NonAction]
		protected static AlarmHistoryUserViewStateSettings GetViewStateSettings(SecurityClass security)
		{
			var userSettings = FMChannelHelper.MakeCall<IUserViewStateSettings, UserViewStateSettingCollection>(x => x.EnumerateBySiteUserClientIpAddressWindowNameAndViewID(security, security.SiteGuid, security.UserGuid, "", AlarmHistoryTabID));
			AlarmHistoryUserViewStateSettings userSetting = null;
			if (userSettings != null && userSettings.Count > 0)
			{
				userSetting = (AlarmHistoryUserViewStateSettings)userSettings[0].Value;
			}
			return userSetting;
		}

		[NonAction]
		public static AlarmHistoryTabModel GetBlankModel(SecurityClass security)
		{
			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(security, security.SiteGuid, false, false, false));

			var userSetting = GetViewStateSettings(security);

			var model = new AlarmHistoryTabModel { AlarmHistories = new List<AlarmHistoryTabRow>(), Site = site, ViewStateSettings = userSetting};

			return model;
		}


		// GET: InventoryManagement/AlarmHistoryTab
		[HttpGet]
		public ActionResult AlarmHistoryTabView()
		{
			var model = GetBlankModel(this.Security);
			return this.View(model);
		}

		[HttpGet]
		public ActionResult AlarmHistoryView()
		{
			if (this.Security.HasRight(RIGHT.OPERATE_VIEW_ALARM_HISTORY))
			{
				var model = GetBlankModel(this.Security);
				return this.PartialViewWithErrorMessages("AlarmHistoryTabView", model, JsonRequestBehavior.AllowGet);
			}
			else
			{
				this.OnError(this.GetTranslatedText("You have no rights to access this screen."));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

		}

		/// <summary>
		/// This method is called by the UI to retrieve the alarm history data.
		/// </summary>
		/// <param name="orderDir"></param>
		/// <param name="columnFilterInfoList"></param>
		/// <param name="originalColumnOrderIndex">The column index to order on.</param>
		/// <param name="draw"></param>
		/// <param name="start"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult GetData(string draw, int start, int length, string orderDir, List<AlarmHistoryTabColumnFilterInfo> columnFilterInfoList, int originalColumnOrderIndex, int recordTypeFilter)
		{
			// Initialization.   
			JsonResult result;

			try
			{
				// Loading.   
				var model = GetModel(this.Security, columnFilterInfoList, recordTypeFilter);
				var data = model.AlarmHistories;

				// Total record count.   
				int totalRecords = data.Count;

				// Sorting. 
				string order = originalColumnOrderIndex.ToString();  
				data = this.SortByColumnWithOrder(order, orderDir, data);

				// Filter record count.   
				int recFilter = data.Count;

				// Apply pagination.   
				data = data.Skip(start).Take(length).ToList();
				// Loading drop down lists.   
				result = this.Json(new
				{
					draw = Convert.ToInt32(draw),
					recordsTotal = totalRecords,
					recordsFiltered = recFilter,
					data = data
				}, JsonRequestBehavior.AllowGet);
			}
			catch (Exception)
			{
				//Console.Write(ex.Message);

				result = this.Json(new
				{
					draw = 0,
					recordsTotal = 0,
					recordsFiltered = 0,
					data = 0
				}, JsonRequestBehavior.AllowGet);
			}

			// Return info.   
			return result;
		}

		/// <summary>
		/// This method will convert the archive unit name which is actually the engineering
		/// unit enum name and return the appropriate unit abbreviation.  If not found it 
		/// will return the input name.
		/// </summary>
		/// <param name="archiveUnitName">This is the enum to string value.</param>
		/// <returns>Returns the real unit name.</returns>
		private static string GetEngineeringUnitsAbbreviation(string archiveUnitName)
		{
			if (string.IsNullOrEmpty(archiveUnitName))
			{
				return archiveUnitName;
			}

			EngineeringUnit unitEnumIndex;

			if (Enum.TryParse(archiveUnitName, out unitEnumIndex))
			{
				return EngineeringUnits.GetUnitAbbreviation(unitEnumIndex);
			}

			return archiveUnitName;
		}

		/// <summary>
		/// This method will convert a double to a level in feet/inches/16th or 8th.
		/// </summary>
		/// <param name="archiveUnitName"></param>
		/// <param name="archiveValue"></param>
		/// <returns>Returns a string containing the level.</returns>
		private static string ConvertLevelValue(string archiveUnitName, string archiveValue)
		{
			if (string.IsNullOrEmpty(archiveUnitName) || string.IsNullOrEmpty(archiveValue))
			{
				return archiveValue;
			}

			double archiveValueDouble;

			if (double.TryParse(archiveValue, out archiveValueDouble) == false)
			{
				return archiveValue;
			}

			EngineeringUnit unitEnumIndex;

			if (Enum.TryParse(archiveUnitName, out unitEnumIndex) == false)
			{
				return archiveValue;
			}

			double feet;
			double inches;
			int inchesInt;
			string level;

			switch (unitEnumIndex)
			{
				case EngineeringUnit.FmlFtIn8Th:
					feet			= Math.Floor(archiveValueDouble);
					inches			= (archiveValueDouble - feet) * 12;
					double eighth   = (inches - Math.Floor(inches)) / 0.125;
					int eightInt	= (int) Math.Round(eighth, MidpointRounding.AwayFromZero);
					inchesInt		= (int) inches;

					level = (int) feet + "-" + (inchesInt >= 10 ? inchesInt.ToString() : "0" + inchesInt) + "-" + (eightInt >= 10 ? eightInt.ToString() : "0" + eightInt);
					return level;

				case EngineeringUnit.FmlFtIn16Th:
					feet				= Math.Floor(archiveValueDouble);
					inches				= (archiveValueDouble - feet) * 12;
					double sixtenths	= (inches - Math.Floor(inches)) / 0.0625;
					int sixteenthsInt	= (int) Math.Round(sixtenths, MidpointRounding.AwayFromZero);
					inchesInt			= (int)inches;

					level = (int)feet + "-" + (inchesInt >= 10 ? inchesInt.ToString() : "0" + inchesInt) + "-" + (sixteenthsInt >= 10 ? sixteenthsInt.ToString() : "0" + sixteenthsInt);
					return level;
			}

			return archiveValue;
		}

		/// <summary>
		/// This method will convert the archive date time from an UTC format to the site
		/// configured format for local time.
		/// </summary>
		/// <param name="utcTime"></param>
		/// <param name="site"></param>
		/// <returns></returns>
		private static string ConvertArchiveDateTimeToLocalTime(DateTime utcTime, SiteClass site)
		{
			string localTimeStr = utcTime.ToString(site.TimePattern);
			string localDateStr = utcTime.ToString(site.ShortDatePattern);
			string localDateTimeStr = localDateStr + " " + localTimeStr;


			try
			{
				TimeZoneInfo localTimeZone = TimeZoneInfo.FindSystemTimeZoneById(site.TimeZone);
				DateTimeOffset localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, localTimeZone);

				localTimeStr = localDateTime.ToString(site.TimePattern);
				localDateStr = localDateTime.ToString(site.ShortDatePattern);
				localDateTimeStr = localDateStr + " " + localTimeStr;
				return localDateTimeStr;
			}
			catch (TimeZoneNotFoundException )
			{
				//Console.Write(timeZoneNotFoundExcept.Message);
			}
			catch (InvalidTimeZoneException )
			{
				//Console.Write(invalidTimeZoneExcept.Message);
			}

			return localDateTimeStr;
		}

		[HttpPost]
		public ActionResult AlarmHistoryTabColumnFilterGetFilter(int selectedColumn, List<AlarmHistoryTabColumnFilterInfo> filterInfo)
		{
			// call the wcf interface and read the filters from cassandra AandE table

			List<string> filterList = FMChannelHelper.MakeCall<IAandEArchive, List<string>>(x => x.GetColumnFilterData(this.Security,
																													   selectedColumn,
																													   filterInfo));

			return this.Json(filterList);
		}

		[HttpPost]
		public ActionResult UpdateComment(string timeStampTicks, string alarmAndEventRecordGuid, string comment)
		{
			var returnVal = FMChannelHelper.MakeCall<IAandEArchive, Tuple<string, DateTimeOffset>>(x => x.UpdateAandEComment(this.Security,
																											new DateTimeOffset(Convert.ToInt64(timeStampTicks), new TimeSpan(0)),
																										   new Guid(alarmAndEventRecordGuid),
																										   comment));
			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));
			var timeStr = ConvertArchiveDateTimeToLocalTime(returnVal.Item2.DateTime, site);
			var ret = new Tuple<string, string>(returnVal.Item1, timeStr);
			return this.Json(ret);
		}


		private List<AlarmHistoryTabRow> SortByColumnWithOrder(string order, string orderDir, List<AlarmHistoryTabRow> data)
		{
			// Initialization.   
			List<AlarmHistoryTabRow> lst = new List<AlarmHistoryTabRow>();
			try
			{
				// Sorting   
				switch (order)
				{
					case "0":
						// DateAndTime ordering.   
						lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.DateAndTimeTicks).ToList() : data.OrderBy(p => p.DateAndTimeTicks).ToList();
						break;
					case "1":
						// Site ordering.   
						lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Site).ToList() : data.OrderBy(p => p.Site).ToList();
						break;
					case "2":
						// Point type ordering.   
						lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.PointType).ToList() : data.OrderBy(p => p.PointType).ToList();
						break;
					case "3":
						// Point ordering.   
						lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Point).ToList() : data.OrderBy(p => p.Point).ToList();
						break;
					case "4":
						// Point description description.   
						lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.PointDescription).ToList() : data.OrderBy(p => p.PointDescription).ToList();
						break;
					case "5":
						// Variable ordering.   
						lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Variable).ToList() : data.OrderBy(p => p.Variable).ToList();
						break;
					case "6":
						// Value ordering.   
						lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Value).ToList() : data.OrderBy(p => p.Value).ToList();
						break;
					case "7":
						// Units ordering.   
						lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Units).ToList() : data.OrderBy(p => p.Units).ToList();
						break;
					case "8":
						// Alarm state ordering.   
						lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.AlarmState).ToList() : data.OrderBy(p => p.AlarmState).ToList();
						break;
					case "9":
						// Priority ordering.   
						lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Priority).ToList() : data.OrderBy(p => p.Priority).ToList();
						break;
					case "10":
						// Action ordering.   
						lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Action).ToList() : data.OrderBy(p => p.Action).ToList();
						break;
					case "11":
						// User ordering.   
						lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.User).ToList() : data.OrderBy(p => p.User).ToList();
						break;
					case "12":
						// Comment ordering.   
						lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Comments).ToList() : data.OrderBy(p => p.Comments).ToList();
						break;
					case "13":
						// Comment user name ordering.   
						lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.CommentUserName).ToList() : data.OrderBy(p => p.CommentUserName).ToList();
						break;
					case "14":
						// Comment date time ordering.   
						lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.CommentDateTimeTicks).ToList() : data.OrderBy(p => p.CommentDateTimeTicks).ToList();
						break;
					default:
						// Default is date and time ordering.   
						lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.DateAndTimeTicks).ToList() : data.OrderBy(p => p.DateAndTimeTicks).ToList();
						break;
				}
			}
			catch (Exception)
			{
				// info.   
				//Console.Write(ex);
			}
			// info.   
			return lst;
		}

		[HttpPost]
		public ActionResult SaveViewState(string jsonViewState)
		{
			AlarmHistoryUserViewStateSettings alarmHistoryViewStateSettings = new AlarmHistoryUserViewStateSettings();
			alarmHistoryViewStateSettings.JsonViewState = jsonViewState;
			SaveViewStateSettings(this.Security, alarmHistoryViewStateSettings);
			return this.JsonWithErrorMessages(null);
		}
	}
}