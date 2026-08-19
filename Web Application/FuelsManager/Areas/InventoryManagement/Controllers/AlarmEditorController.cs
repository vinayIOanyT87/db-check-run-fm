
namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.Web.Mvc;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.InventoryManagement.ViewModels;

	using Newtonsoft.Json;
	using FMPointCommon;

	using Microsoft.Ajax.Utilities;
	using Opc.Ua;

	[SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
	public class AlarmEditorController : FMBaseControllerEx
	{
		#region Private data members
		private const string SuppressedString = "Suppressed";
		private const string ActiveAlarmString = "Active";
		private const string OneShelvedString = "One Shot Shelved";
		private const string TimeShelvedString = "Shelved Until ";
		#endregion

		[NonAction]
		public static string SerializeModel(AlarmEditorModel model)
		{
			return JsonConvert.SerializeObject(model);
		}
		
		[NonAction]
		public static AlarmEditorModel DeserializeModel(string modelStr)
		{
			var jsonSerializerSettings = new JsonSerializerSettings
			{
				MissingMemberHandling = MissingMemberHandling.Ignore
			};

			var model = JsonConvert.DeserializeObject<AlarmEditorModel>(modelStr, jsonSerializerSettings);

				//This is a hack to overcome limitations of serializing and deserilazing JSON objects using the
				//Newtonsoft JsonConvert.DeserializeObject method.  If &&s are in the string it causes an exception to be 
				//thrown.
			if (model != null)
			{
				foreach (var tag in model.Tags)
				{
					foreach (var alarm in tag.Alarms)
					{
						foreach (var test in alarm.AlarmTests)
						{
							if (test.AlarmTestEquation.Contains("#"))
							{
									test.AlarmTestEquation = test.AlarmTestEquation.Replace("#", "&");
							}
						}
					}
				}
			}
			return model;
		}

		[NonAction]
		private static void DoUpdates(SecurityClass security, SiteClass site, Point point, AlarmEditorModel model)
		{
			foreach (var modelTag in model.Tags)
			{
				var tag = point.Tags[modelTag.PointTagGuid];

				if (modelTag.AlarmsEnabled != tag.AlarmsEnabled)
				{
					tag.AlarmsEnabled = modelTag.AlarmsEnabled;
				}

				// delete the Alarms for the tag that are not part of the model anymore (deleted in the UI)
				var alarmsToDelete = tag.Alarms.Values.Where(o => !modelTag.Alarms.Any(x => x.AlarmGuid == o.AlarmGuid));

				foreach (var alarm in alarmsToDelete.ToArray())
				{
					tag.Alarms.Remove(alarm.AlarmGuid);
				}


				foreach (var modelAlarm in modelTag.Alarms)
				{
					Alarm alarm = new Alarm();
					if (tag.Alarms.ContainsKey(modelAlarm.AlarmGuid))
					{
						alarm = tag.Alarms[modelAlarm.AlarmGuid];
					}
					else
					{
						tag.Alarms.Add(modelAlarm.AlarmGuid, alarm);
					}

					alarm.PointGuid = model.PointGuid;
					alarm.InputTagGuid = tag.PointTagGuid;
					alarm.AlarmGuid = modelAlarm.AlarmGuid;
					alarm.ID = modelAlarm.Id;
					alarm.NotAlarmState = modelAlarm.NotAlarmState;
					alarm.Order = modelAlarm.Order;
					var alarmCategory = model.AlarmCategories.FirstOrDefault(x => x.Value == modelAlarm.Category).Key;
					alarm.AlarmCategoryApplicationStringGuid = alarmCategory;
					alarm.AlarmStateTagGuid = modelAlarm.AlarmStatusTagGuid;
					alarm.Enabled = modelAlarm.Enabled;
					alarm.Notify = modelAlarm.Notify;


					foreach (var modelAlarmTest in modelAlarm.AlarmTests)
					{
						AlarmTest alarmTest = new AlarmTest();
						if (alarm.AlarmTests.ContainsKey(modelAlarmTest.AlarmTestGuid))
						{
							alarmTest = alarm.AlarmTests[modelAlarmTest.AlarmTestGuid];
						}
						else
						{
							alarm.AlarmTests.Add(modelAlarmTest.AlarmTestGuid, alarmTest);

							// new alarm test which requires a new entry in the alarm status
							PointTagAlarmStatus alarmStatus = new PointTagAlarmStatus();
							alarmStatus.PointTagAlarmStatusGuid = Guid.NewGuid();
							alarmStatus.AlarmTestGuid = modelAlarmTest.AlarmTestGuid;
							alarm.AlarmStatus.Add(alarmStatus.PointTagAlarmStatusGuid, alarmStatus);
						}

						alarmTest.AlarmGuid = modelAlarm.AlarmGuid;
						alarmTest.AlarmTestGuid = modelAlarmTest.AlarmTestGuid;
						alarmTest.LimitTagGuid = modelAlarmTest.LimitTagGuid;
						alarmTest.ID = modelAlarmTest.Id;
						alarmTest.AlarmPriorityGuid = modelAlarmTest.AlarmPriorityGuid;
						alarmTest.NormalUnacknowledgedAlarmPriorityGuid = modelAlarmTest.NormalUnacknowledgedAlarmPriorityGuid;
						alarmTest.BitMask = modelAlarmTest.BitMask;
						alarmTest.BitwiseOperator = modelAlarmTest.BitwiseOperator;
						alarmTest.TagField = modelAlarmTest.TagField;
						alarmTest.TestType = modelAlarmTest.TestType;
						alarmTest.Order = modelAlarmTest.Order;



						var limitTag = point.Tags[alarmTest.LimitTagGuid];
						var inputTag = point.Tags[alarm.InputTagGuid];
						var hysteresis = GenerateHysteresisDoubleFromString(inputTag, point,site,modelTag.Hysteresis);
						var holdOffSeconds = GenerateHoldOffIntegerFromString(limitTag, point, site, modelAlarmTest.HoldOffSeconds);
						var holdOffMinutes = GenerateHoldOffIntegerFromString(limitTag, point, site, modelAlarmTest.HoldOffMinutes);
						var holdOff = (int)(holdOffMinutes * 60 + holdOffSeconds);

						if (limitTag.ValueTypeString == "FMBusinessObjects.DataObjects.PointCommandStatusListReference")
						{
							var pointCommandStatusListReference = limitTag.Value as PointCommandStatusListReference;
							if(pointCommandStatusListReference != null)
							{
								pointCommandStatusListReference.CurrentKey = modelAlarmTest.LimitValue;
								pointCommandStatusListReference.CurrentValue = null;
							}
						}
						else if (limitTag.ValueTypeString.IndexOf("FMBusinessObjects.DataObjects.CodedVariables") != -1)
						{
							if (!modelAlarmTest.LimitValue.IsNullOrWhiteSpace())
							{
								Type enumerationType = Type.GetType(limitTag.ValueTypeString + ", FMBusinessObjects");
								if (enumerationType != null)
								{
									foreach (var enumVal in GetEnumSelectList(limitTag.ValueTypeString))
									{
										if (enumVal.Text.Equals(modelAlarmTest.LimitValue, StringComparison.InvariantCultureIgnoreCase))
										{
											limitTag.Value = (Enum)Enum.ToObject(enumerationType, Int32.Parse(enumVal.Value));
										}
									}
								}
							}
						}
						else
						{
							limitTag.Value = PointManager.ParseValue(limitTag.ValueType, limitTag.Units, site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT), modelAlarmTest.LimitValue);
						}

						limitTag.Status = (limitTag.Value != null) ? StatusCodes.Good : StatusCodes.Bad;
						limitTag.ServerTimeStamp = DateTimeOffset.UtcNow;
						limitTag.SourceTimeStamp = DateTimeOffset.UtcNow;

						if (modelAlarmTest.AlarmState != alarmTest.AlarmState || modelAlarmTest.AlarmText != alarmTest.AlarmText || hysteresis != alarmTest.Holdoff || holdOff != alarmTest.TimedHoldOffInSeconds)
						{
							alarmTest.AlarmState = modelAlarmTest.AlarmState;
							alarmTest.AlarmText = modelAlarmTest.AlarmText;
							alarmTest.Holdoff = hysteresis;
							alarmTest.TimedHoldOffInSeconds = holdOff;
						}
					}
				}
			}

			// delete all the Alarms for tags that don't have alarms anymore (deleted in the UI)
			var tagsToRemoveAlarms = point.Tags.Values.Where(o => !model.Tags.Any(x => x.PointTagGuid == o.PointTagGuid));
			foreach (var tag in tagsToRemoveAlarms.ToArray())
			{
				tag.Alarms = new Dictionary<Guid, Alarm>();
			}
		}

		[NonAction]
		private AlarmEditorModel GetModel(SecurityClass security, Guid pointGuid)
		{
			var model = new AlarmEditorModel { Tags = new List<AlarmEditorTagModel>() };

			if (pointGuid != Guid.Empty)
			{
				var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(security, security.SiteGuid,false,false,false));
				var point = FMChannelHelper.MakeCall<IPoints, Point>(x => x.Get(security, pointGuid));
				var categories = DeviceAlarmMapsEditorController.GetAllAlarmCategories(this.Security);
				var drawings = GetDrawingIds(security, point);
				model.NumberDecimalSeparator = site.NumberDecimalSeparator;
				model.NumberGroupSeparator = site.NumberGroupSeparator;
				model.NumberGroupSizes = site.GetNumberGroupSizes();
				model.DecimalPlaces = 2;
				model.ShortDatePattern = site.ShortDatePattern;
				model.PointGuid = pointGuid;
				model.PointTemplateGuid = point.PointTemplateGuid;
				this.FillInModelTags(point, categories, drawings, model,site);
			}

			return model;
		}

		// GET: InventoryManagement/AlarmHistoryTab
		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GetAlarmEditor(string pointGuidStr)
		{
			Guid pointGuid = new Guid(pointGuidStr);
			var model = GetModel(this.Security, pointGuid);
			model.HasEnableAlarmOnPointRight = this.Security.HasRight(RIGHT.ENABLE_ALARMS_ON_POINTS);
            model.HasDisableAlarmOnPointRight = this.Security.HasRight(RIGHT.DISABLE_ALARMS_ON_POINTS);
            model.HasNotifyAlarmOnPointRight = this.Security.HasRight(RIGHT.CONFIGURE_NOTIFY_ALARMS_ON_POINTS);
            model.HasEnableAlarmOnPointTemplateRight = this.Security.HasRight(RIGHT.ENABLE_ALARMS_ON_POINT_TEMPLATES);
			model.HasDisableAlarmOnPointTemplateRight = this.Security.HasRight(RIGHT.DISABLE_ALARMS_ON_POINT_TEMPLATES);
			model.HasPointEditRight = this.Security.HasRight(RIGHT.MODIFY_POINTS);
			model.HasPTEditRight = this.Security.HasRight(RIGHT.MODIFY_POINT_TEMPLATES);
			return this.PartialViewWithErrorMessages("AlarmEditorView", model, JsonRequestBehavior.AllowGet);
		}

		// GET: InventoryManagement/AlarmHistoryTab
		[NonAction]
		public static List<string> UpdateAlarms(SecurityClass security, SiteClass site, Point point, string modelStr)
		{
			var errors = new List<string>();

			try
			{
				AlarmEditorModel model = DeserializeModel(modelStr);
				model.AlarmCategories = DeviceAlarmMapsEditorController.GetAllAlarmCategories(security);
				DoUpdates(security,site, point, model);
			}
			catch (Exception except)
			{
				errors.Add(except.Message);
			}
			return errors;
		}



		/// <summary>
		/// This method is called from the UI to update the Alarm Test object.
		/// </summary>
		/// <param name="modelStr">The Alarm Editor model in string format.</param>
		/// <param name="alarmTestGuidStr">The Alarm Test GUID to update.</param>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult AlarmEditorUpdateAlarmTest(string modelStr, string alarmTestGuidStr)
		{
			try
			{
				AlarmEditorModel model = DeserializeModel(modelStr);
				Guid alarmTestGuid = Guid.Parse(alarmTestGuidStr);

				UpdateAlarmTest(this.Security, model, alarmTestGuid);
			}
			catch (Exception)
			{
				return this.Json("ERROR");
			}

			return this.Json("SUCCESS");
		}

		/// <summary>
		/// This method will update the Alarm Test modifications.
		/// Note: currently it is only updating the drawing GUID.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="model">The Alarm Editor model.</param>
		/// <param name="alarmTestGuid">The Alarm Test GUID to update.</param>
		public static void UpdateAlarmTest(SecurityClass security, AlarmEditorModel model, Guid alarmTestGuid)
		{
			bool found = false;
			var alarmTestList = new List<AlarmTest>();

			foreach (AlarmEditorTagModel tagModel in model.Tags)
			{
				foreach (AlarmEditorAlarmModel alarmModel in tagModel.Alarms)
				{
					AlarmEditorAlarmTestModel alarmTestModel = alarmModel.AlarmTests.Find(x => x.AlarmTestGuid == alarmTestGuid);

					if (alarmTestModel != null)
					{
						var alarmTest = FMChannelHelper.MakeCall<IAlarmTests, AlarmTest>(x => x.Get(security, alarmTestGuid));

						if (alarmTest != null)
						{
							alarmTest.DrawingGuid = alarmTestModel.DrawingGuid;

							if (alarmTestModel.DrawingGuid == Guid.Empty)
							{
								alarmTest.DrawingGuid = null;
							}

							alarmTestList.Add(alarmTest);

							found = true;
							break;
						}
					}
				}

				if (found)
				{
					break;
				}
			}

			if (found)
			{
				const bool EnableAdd = false;
				const bool EnableModify = true;
				FMChannelHelper.MakeCall<IAlarmTests>(x => x.AddModifyAlarmTests(security, alarmTestList, EnableAdd, EnableModify));
			}
		}

		private static string GenerateBitmaskString(PointTag inputTag, AlarmTest test)
		{
			var bitmask = test.BitMask;
			if (bitmask == -1)
			{
				return "";
			}
			var bitmaskPrefix = " & ";

			switch (test.BitwiseOperator)
			{
				case AlarmTestTemplate.BitwiseOperatorEnum.And:
					bitmaskPrefix = " & ";
					break;
				case AlarmTestTemplate.BitwiseOperatorEnum.Or:
					bitmaskPrefix = " | ";
					break;
				case AlarmTestTemplate.BitwiseOperatorEnum.Nor:
					bitmaskPrefix = " NOR ";
					break;
				case AlarmTestTemplate.BitwiseOperatorEnum.Xor:
					bitmaskPrefix = " ^ ";
					break;
				default:
					bitmaskPrefix = "";
					break;
			}

			var bitmaskPostfix = "";

			Type valueToCompareType;
			switch (test.TagField)
			{
				case AlarmTestTemplate.TagFieldEnum.OpcStatusSubCode:
					valueToCompareType = typeof(uint); // PointTag.OpcStatusSubCode is of type uint;
					break;
				case AlarmTestTemplate.TagFieldEnum.Status:
					valueToCompareType = typeof(Int32); // PointTag.Status is Int32;
					break;
				default:
					valueToCompareType = inputTag.ValueType;
					break;
			}
			if (valueToCompareType == typeof(sbyte))
			{
				var bitmaskObj = (sbyte)bitmask;
				var bitMaskStr = bitmaskObj.ToString("X1");
				return bitmaskPrefix + bitMaskStr + bitmaskPostfix;
			}

			if (valueToCompareType == typeof(short))
			{
				var bitmaskObj = (short)bitmask;
				var bitMaskStr = bitmaskObj.ToString("X4");
				return bitmaskPrefix + bitMaskStr + bitmaskPostfix;
			}

			if (valueToCompareType == typeof(int))
			{
				var bitmaskObj = (int)bitmask;
				var bitMaskStr = bitmaskObj.ToString("X8");
				return bitmaskPrefix + bitMaskStr + bitmaskPostfix;
			}

			if (valueToCompareType == typeof(long))
			{
				var bitmaskObj = (long)bitmask;
				var bitMaskStr = bitmaskObj.ToString("X16");
				return bitmaskPrefix + bitMaskStr + bitmaskPostfix;
			}

			if (valueToCompareType == typeof(byte))
			{
				var bitmaskObj = (byte)bitmask;
				var bitMaskStr = bitmaskObj.ToString("X1");
				return bitmaskPrefix + bitMaskStr + bitmaskPostfix;
			}

			if (valueToCompareType == typeof(ushort))
			{
				var bitmaskObj = (ushort)bitmask;
				var bitMaskStr = bitmaskObj.ToString("X4");
				return bitmaskPrefix + bitMaskStr + bitmaskPostfix;
			}

			if (valueToCompareType == typeof(uint) || valueToCompareType == typeof(DeviceAlarmMapReference))
			{
				var bitmaskObj = (uint)bitmask;
				var bitMaskStr = bitmaskObj.ToString("X8");
				return bitmaskPrefix + bitMaskStr + bitmaskPostfix;
			}

			if (valueToCompareType == typeof(ulong))
			{
				var bitmaskObj = (ulong)bitmask;
				var bitMaskStr = bitmaskObj.ToString("X16");
				return bitmaskPrefix + bitMaskStr + bitmaskPostfix;
			}

			if (valueToCompareType == typeof(Int64))
			{
				var bitmaskObj = (Int64)bitmask;
				var bitMaskStr = bitmaskObj.ToString("X16");
				return bitmaskPrefix + bitMaskStr + bitmaskPostfix;
			}

			return "";
		}

		private string GenerateComparisonString(AlarmTestTemplate.TestTypeEnum comparison)
		{
			switch (comparison)
			{
				case AlarmTestTemplate.TestTypeEnum.GreaterThan:
					return " > ";
				case AlarmTestTemplate.TestTypeEnum.GreaterThanOrEqual:
					return " >= ";
				case AlarmTestTemplate.TestTypeEnum.LessThan:
					return " < ";
				case AlarmTestTemplate.TestTypeEnum.LessThanOrEqual:
					return " <= ";
				case AlarmTestTemplate.TestTypeEnum.Equals:
					return " = ";
				case AlarmTestTemplate.TestTypeEnum.NotEquals:
					return " != ";
				default:
					return " Undefined ";
			}
		}

		private string GenerateValueString(PointTag limitTag, Point p, SiteClass s)
		{
			if (limitTag != null && limitTag.InputOutputType == PointTemplateTag.PointTagInputOutputType.Manual)
			{
				var valueStr = string.Empty;
				if (limitTag.Value != null)
				{
					valueStr = limitTag.Value.ToString();
					if (limitTag.Units != EngineeringUnit.FmuNone)
					{
						valueStr = limitTag.FormatValueFullPrecision(p, s);
					}
				}
				return valueStr;
			}
			return "";
		}

		private static string GenerateHysteresisString(PointTag inputTag, Point p, SiteClass s, double hysteresis)
		{
			var valueStr = hysteresis.ToString();
			if (inputTag != null)
			{
				var decimalPlaces = inputTag.GetDecimalPlaces(p);
				var val = EngineeringUnitsHelperClass.FormatValue(hysteresis, inputTag.Units);
				if (val is double || val is float)
				{
					decimalPlaces = 9;
				}
				var numFormatProvider = s.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT);
				var formattedString = s.FormatValue(val, decimalPlaces);
				if (formattedString.IndexOf(numFormatProvider.NumberDecimalSeparator) > -1)
				{
					// remove the trailing '0'
					formattedString = formattedString.TrimEnd('0');
					if (formattedString.IndexOf(numFormatProvider.NumberDecimalSeparator) == formattedString.Length - 1)
					{
						formattedString = formattedString.Remove(formattedString.Length - 1, 1);
					}
				}
				return formattedString;
			}
			return valueStr;
		}

		private static string GenerateHoldOffString(PointTag limitTag, Point p, SiteClass s, int holdOff)
		{
			var valueStr = holdOff.ToString();
			if (limitTag != null)
			{
				var units = limitTag.GetEngineeringUnits(p);
				var decimalPlaces = limitTag.GetDecimalPlaces(p);
				var val = EngineeringUnitsHelperClass.FormatValue(holdOff, limitTag.Units);
				if (val is double || val is float)
				{
					decimalPlaces = 9;
				}
				var numFormatProvider = s.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT);
				var formattedString = s.FormatValue(val, decimalPlaces);
				if (formattedString.IndexOf(numFormatProvider.NumberDecimalSeparator) > -1)
				{
					// remove the trailing '0'
					formattedString = formattedString.TrimEnd('0');
					if (formattedString.IndexOf(numFormatProvider.NumberDecimalSeparator) == formattedString.Length - 1)
					{
						formattedString = formattedString.Remove(formattedString.Length - 1, 1);
					}
				}
				return formattedString;
			}
			return valueStr;
		}

		private static double GenerateHysteresisDoubleFromString(PointTag inputTag, Point p, SiteClass s, string hysteresis)
		{
			var numFormatInfo = s.GetNumberFormatInfo(SITE_VARIABLE_TYPE.ADDITIVE_VOLUME);
			numFormatInfo.NumberDecimalDigits = inputTag.GetDecimalPlaces(p);
			return (double)EngineeringUnitsHelperClass.ParseValue(typeof(double),hysteresis, inputTag.Units, numFormatInfo);
		}

		private static int GenerateHoldOffIntegerFromString(PointTag limitTag, Point p, SiteClass s, string holdOff)
		{
			var numFormatInfo = s.GetNumberFormatInfo(SITE_VARIABLE_TYPE.ADDITIVE_VOLUME);
			return (int)EngineeringUnitsHelperClass.ParseValue(typeof(int), holdOff, limitTag.Units, numFormatInfo);
		}

		private string GenerateTestEquationString(Point point, PointTag inputTag, PointTag limitTag, AlarmTest test, SiteClass s)
		{
			string limitTagId = limitTag != null ? limitTag.ID : "Unknown";
			var equation = inputTag.ID + "." + test.TagField + GenerateBitmaskString(inputTag, test) + this.GenerateComparisonString(test.TestType) + limitTagId + "." + AlarmTestTemplate.TagFieldEnum.Value;
			return equation;
		}

		private void FillInModelTests(
			Point point,
			PointTag inputTag,
			Alarm alarm,
			Dictionary<Guid, string> drawings,
			AlarmEditorAlarmModel modelAlarm,
			SiteClass s)
		{
			modelAlarm.AlarmTests = new List<AlarmEditorAlarmTestModel>();
			foreach (var test in alarm.AlarmTests.Values)
			{
				var modelTest = new AlarmEditorAlarmTestModel
									{
										AlarmTestGuid = test.AlarmTestGuid,
										Id = test.ID,
										LimitTagGuid = test.LimitTagGuid,
										AlarmState = test.AlarmState,
										AlarmText = test.AlarmText,
										Order = test.Order,
										HoldOff = test.Holdoff,
										HelpFile = test.HelpFile,
										DrawingGuid = test.DrawingGuid == null? Guid.Empty : (Guid)test.DrawingGuid,
										Enabled = test.Enabled,
										BitMask = test.BitMask,
										BitwiseOperator = test.BitwiseOperator,
										TagField = test.TagField,
										TestType = test.TestType,
										AlarmPriorityGuid = test.AlarmPriorityGuid,
										NormalUnacknowledgedAlarmPriorityGuid = test.NormalUnacknowledgedAlarmPriorityGuid
				};
				PointTag limitTag;
				if (point.Tags.TryGetValue(test.LimitTagGuid, out limitTag))
				{
					var holdOffSeconds = (int)(test.TimedHoldOffInSeconds % 60);
					var holdOffMinutes = (int)(test.TimedHoldOffInSeconds / 60);
					var holdOffSecondsStr = GenerateHoldOffString(limitTag, point, s, holdOffSeconds);
					var holdOffMinutesStr = GenerateHoldOffString(limitTag, point, s, holdOffMinutes);
					modelTest.HoldOffMinutes = holdOffMinutesStr;
					modelTest.HoldOffSeconds = holdOffSecondsStr;
					modelTest.LimitTagId = limitTag.ID;
					modelTest.AlarmTestEquation = this.GenerateTestEquationString(point, inputTag, limitTag, test, s);
					modelTest.LimitValue = this.GenerateValueString(limitTag, point, s);
					modelTest.LimitTagDecimalPlaces = limitTag.GetDecimalPlaces(point);
					modelTest.LimitTagUnitsType = limitTag.EngineeringUnitsType;
					modelTest.LimitTagUnits = limitTag.Units;
					modelTest.LimitTagMax = limitTag.GetMaximum(point);
					modelTest.LimitTagMin = limitTag.GetMinimum(point);
					modelTest.LimitTagValueType = limitTag.ValueTypeString;
					modelTest.LimitTagEditable = limitTag.InputOutputType == PointTemplateTag.PointTagInputOutputType.Manual || limitTag.IsForced();
				}
				if (test.DrawingGuid != null && test.DrawingGuid != Guid.Empty)
				{
					string drawing;
					if (drawings.TryGetValue((Guid)test.DrawingGuid, out drawing))
					{
						modelTest.DrawingId = drawing;
					}
				}
				modelAlarm.AlarmTests.Add(modelTest);
			}
		}

		private void FillInModelAlarms(Point point, PointTag tag, Dictionary<Guid, string> categories, Dictionary<Guid, string> drawings, AlarmEditorTagModel modelTag, SiteClass s)
		{
			modelTag.Alarms = new List<AlarmEditorAlarmModel>();
			foreach (var alarm in tag.Alarms.Values)
			{
				var modelAlarm = new AlarmEditorAlarmModel
				{
					Id = alarm.ID,
					AlarmGuid = alarm.AlarmGuid,
					Order = alarm.Order,
					AlarmStatusTagGuid = alarm.AlarmStateTagGuid,
                    Enabled = alarm.Enabled,
                    Notify = alarm.Notify,
                    Exclusive = true, //Need to update this when we implement Exclusive, Non-Exclusive functionality
					NotAlarmState = alarm.NotAlarmState
				};

				string category;
				if (categories.TryGetValue(alarm.AlarmCategoryApplicationStringGuid, out category))
				{
					modelAlarm.Category = category;
				}
				var suppressedAndShelvedStatus = ActiveAlarmString;
				if (alarm.Suppressed)
				{
					suppressedAndShelvedStatus = SuppressedString;
				}
				else
				{
					if (alarm.ShelvedOneShot)
					{
						suppressedAndShelvedStatus = OneShelvedString;
					}
					else
					{
						if (alarm.ShelvedEndTimeStamp > DateTimeOffset.Now)
						{
							//need to output time is site format
							suppressedAndShelvedStatus = TimeShelvedString + alarm.ShelvedEndTimeStamp.ToString();
						}
					}
				}
				modelAlarm.SuppressedAndShelvedStatus = suppressedAndShelvedStatus;

				PointTag alarmStatusTag;
				if (point.Tags.TryGetValue(alarm.AlarmStateTagGuid, out alarmStatusTag))
				{
					modelAlarm.AlarmStatusTagId = alarmStatusTag.ID;
				}
				FillInModelTests(point, tag, alarm, drawings, modelAlarm, s);
				modelTag.Alarms.Add(modelAlarm);
			}
			modelTag.Alarms = modelTag.Alarms.OrderBy(o => o.Id).ToList();
		}

		private void FillInModelTags(Point point, Dictionary<Guid, string> categories, Dictionary<Guid, string> drawings, AlarmEditorModel model, SiteClass s)
		{
			foreach (var tag in point.Tags.Values)
			{
				if (tag.Alarms != null && tag.Alarms.Count > 0)
				{
					var firstAlarmTeset = tag.Alarms.First().Value.AlarmTests.First().Value;
					var limitTag = point.Tags[firstAlarmTeset.LimitTagGuid];
					var hysteresisStr = GenerateHysteresisString(tag, point, s, firstAlarmTeset.Holdoff);
					var modelTag = new AlarmEditorTagModel { PointTagGuid = tag.PointTagGuid, Id = tag.ID, Hysteresis = hysteresisStr, AlarmsEnabled = tag.AlarmsEnabled, TagDecimalPlaces = tag.DecimalPlaces, TagUnits = tag.Units, DataType = tag.ValueTypeString};
					this.FillInModelAlarms(point, tag, categories, drawings, modelTag, s);
					model.Tags.Add(modelTag);
				}
			}
		}


		private Dictionary<Guid, string> GetDrawingIds(SecurityClass security, Point point)
		{
			var drawingDictionary = new Dictionary<Guid, string>();
			var drawingGuidList = new List<Guid>();
			foreach (var tag in point.Tags.Values)
			{
				foreach (var alarm in tag.Alarms.Values)
				{
					foreach (var test in alarm.AlarmTests.Values)
					{
						if (test.DrawingGuid != null && test.DrawingGuid != Guid.Empty)
						{
							if (!drawingGuidList.Contains((Guid)test.DrawingGuid))
							{
								drawingGuidList.Add((Guid)test.DrawingGuid);
							}
						}
					}
				}
			}
			if (drawingGuidList.Count > 0)
			{
				var drawingDict =
					FMChannelHelper.MakeCall<IDrawings, Dictionary<Guid, DrawingName>>(
						x => x.EnumerateByDrawingGuids(security, drawingGuidList));
				foreach (var drawing in drawingDict.Values)
				{
					drawingDictionary.Add(drawing.DrawingGuid, drawing.ID);
				}
			}

			return drawingDictionary;
		}
	}
}