using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.InventoryManagement.ViewModels;

	using Newtonsoft.Json;

	public class AlarmTestEquationEditorController : FMBaseControllerEx
	{

		[NonAction]
		public static string SerializeModel(AlarmTestEquationEditorModel model)
		{
			return JsonConvert.SerializeObject(model);
		}

		[NonAction]
		public static AlarmTestEquationEditorModel DeserializeModel(string modelStr)
		{
			var jsonSerializerSettings = new JsonSerializerSettings
			{
				MissingMemberHandling = MissingMemberHandling.Ignore
			};

			var obj = JsonConvert.DeserializeObject<AlarmTestEquationEditorModel>(modelStr, jsonSerializerSettings);
			return obj;
		}

		private void GenerateBitmaskInfo(PointTag inputTag, AlarmTest test, AlarmTestEquationEditorModel model)
		{
			var bitmask = test.BitMask;
			model.Bitmask = bitmask.ToString("X2");
			model.BitMaskDigits = 2;
			model.UseBitmask = false;
			model.CanUseBitmask = false;

			Type valueToCompareType;
			switch (test.TagField)
			{
				case AlarmTestTemplate.TagFieldEnum.OpcStatusSubCode:
					valueToCompareType = inputTag.OpcStatusSubCode.GetType();
					break;
				case AlarmTestTemplate.TagFieldEnum.Status:
					valueToCompareType = inputTag.Status.GetType();
					break;
				default:
					valueToCompareType = inputTag.ValueType;
					break;
			}
			if (valueToCompareType == typeof(sbyte))
			{
				var bitmaskObj = (sbyte)bitmask;
				model.Bitmask = bitmaskObj.ToString("X1");
				model.BitMaskDigits = 1;
				model.UseBitmask = true;
				model.CanUseBitmask = true;
				return;
			}

			if (valueToCompareType == typeof(short))
			{
				var bitmaskObj = (short)bitmask;
				model.Bitmask = bitmaskObj.ToString("X4");
				model.BitMaskDigits = 4;
				model.UseBitmask = true;
				model.CanUseBitmask = true;
				return;
			}

			if (valueToCompareType == typeof(int))
			{
				var bitmaskObj = (int)bitmask;
				model.Bitmask = bitmaskObj.ToString("X8");
				model.BitMaskDigits = 8;
				model.UseBitmask = true;
				model.CanUseBitmask = true;
				return;
			}

			if (valueToCompareType == typeof(long))
			{
				var bitmaskObj = (long)bitmask;
				model.Bitmask = bitmaskObj.ToString("X16");
				model.BitMaskDigits = 16;
				model.UseBitmask = true;
				model.CanUseBitmask = true;
				return;
			}

			if (valueToCompareType == typeof(byte))
			{
				var bitmaskObj = (byte)bitmask;
				model.Bitmask = bitmaskObj.ToString("X1");
				model.BitMaskDigits = 1;
				model.UseBitmask = true;
				model.CanUseBitmask = true;
				return;
			}

			if (valueToCompareType == typeof(ushort))
			{
				var bitmaskObj = (ushort)bitmask;
				model.Bitmask = bitmaskObj.ToString("X4");
				model.BitMaskDigits = 4;
				model.UseBitmask = true;
				model.CanUseBitmask = true;
				return;
			}

			if (valueToCompareType == typeof(uint) || valueToCompareType == typeof(DeviceAlarmMapReference))
			{
				var bitmaskObj = (uint)bitmask;
				model.Bitmask = bitmaskObj.ToString("X8");
				model.BitMaskDigits = 8;
				model.UseBitmask = true;
				model.CanUseBitmask = true;
				return;
			}

			if (valueToCompareType == typeof(ulong))
			{
				var bitmaskObj = (ulong)bitmask;
				model.Bitmask = bitmaskObj.ToString("X16");
				model.BitMaskDigits = 16;
				model.UseBitmask = true;
				model.CanUseBitmask = true;
				return;
			}
		}

		private void GenerateBitmaskInfo(string inputTagDataType, AlarmTestTemplate.TagFieldEnum tagField, long bitMask, AlarmTestEquationEditorModel model)
		{

			model.Bitmask = bitMask.ToString("X");
			model.BitMaskDigits = 1;
			model.UseBitmask = false;
			model.CanUseBitmask = false;

			string valueToCompareType;
			switch (tagField)
			{
				case AlarmTestTemplate.TagFieldEnum.Status:
					valueToCompareType = "System.Int32"; // status is an integer
					break;
				default:
					valueToCompareType = inputTagDataType;
					break;
			}

			// if source tag is numeric we can use bitmaps
			if (inputTagDataType == "System.Int16" || inputTagDataType == "System.Int32" || inputTagDataType == "System.Int64"
					 || inputTagDataType == "System.UInt16" || inputTagDataType == "System.UInt32" || inputTagDataType == "System.UInt64" || inputTagDataType == "FMBusinessObjects.DataObjects.DeviceAlarmMapReference")
			{
				model.CanUseBitmask = true;
			}

			if (valueToCompareType == "System.Int16" || valueToCompareType == "System.UInt16")
			{
				model.Bitmask = bitMask.ToString("X4");
				model.BitMaskDigits = 4;
				model.UseBitmask = (bitMask != -1); // if bitmask is -1 we are not using the bitmask
				return;
			}
			if (valueToCompareType == "System.Int32" || valueToCompareType == "System.UInt32" || valueToCompareType == "FMBusinessObjects.DataObjects.DeviceAlarmMapReference")
			{
				model.Bitmask = bitMask.ToString("X8");
				model.BitMaskDigits = 8;
				model.UseBitmask = (bitMask != -1); // if bitmask is -1 we are not using the bitmask;
				return;
			}
			if (valueToCompareType == "System.Int64" || valueToCompareType == "System.UInt64")
			{
				model.Bitmask = bitMask.ToString("X16");
				model.BitMaskDigits = 16;
				model.UseBitmask = (bitMask != -1); // if bitmask is -1 we are not using the bitmask;
				return;
			}
		}


		[NonAction]
		private AlarmTestEquationEditorModel GetModel(Guid pointGuid, Guid alarmTestGuid)
		{
			var model = new AlarmTestEquationEditorModel();
			var point = FMChannelHelper.MakeCall<IPoints, Point>(x => x.Get(this.Security, pointGuid));
			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

			foreach (var tag in point.Tags.Values)
			{
				foreach (var alarm in tag.Alarms.Values)
				{
					AlarmTest alarmTest;
					if(alarm.AlarmTests.TryGetValue(alarmTestGuid,out alarmTest))
					{
						model.AlarmTestGuid = alarmTestGuid;
						model.TagName = tag.ID;
						model.TagType = tag.ValueTypeString;
						//SRM model.UseBitmask = false;
						model.TagGuid = tag.PointTagGuid;
						model.TagAttribute = (int)alarmTest.TagField;
						PointTag limitTag;
						if (point.Tags.TryGetValue(alarmTest.LimitTagGuid, out limitTag))
						{
							model.LimitName = limitTag.ID;
						}
						//SRM need to enhance AlarmTest and AlarmEngine to have multiple bitwise operators
						model.BitwiseOperator = 0;
						model.ComparisonOperator = (int)alarmTest.TestType;
						this.GenerateBitmaskInfo(tag, alarmTest, model);
					}
				}
			}
			return model;
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult GetAlarmTestEquationEditor(string pointGuid, string alarmTestGuid)
		{
			Guid alarmTest = new Guid(alarmTestGuid);
			Guid point = new Guid(pointGuid);
			var model = this.GetModel( point, alarmTest);
			return this.PartialViewWithErrorMessages("AlarmTestEquationEditorView", model, JsonRequestBehavior.AllowGet);
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult GetAlarmTestEquationEditorForUnsavedAlarmTest(string alarmTestGuid, string tagName, string tagValueTypeString, string tagGuid, string limitTagName, string inputTagDataType, AlarmTestTemplate.TagFieldEnum tagField, AlarmTestTemplate.TestTypeEnum testType, long bitMask, int bitwiseOperator)
		{
			Guid alarmTest = new Guid(alarmTestGuid);
			Guid alarmTestTagGuid = new Guid(tagGuid);

			var model = new AlarmTestEquationEditorModel();
			model.AlarmTestGuid = alarmTest;
			model.TagName = tagName;
			model.TagType = tagValueTypeString;
			//SRM model.UseBitmask = false;
			model.TagGuid = alarmTestTagGuid;
			model.TagAttribute = (int)tagField;
			model.LimitName = limitTagName;
			//SRM need to enhance AlarmTest and AlarmEngine to have multiple bitwise operators
			model.BitwiseOperator = bitwiseOperator;
			model.ComparisonOperator = (int)testType;

			this.GenerateBitmaskInfo(inputTagDataType, tagField, bitMask, model);

			return this.PartialViewWithErrorMessages("AlarmTestEquationEditorView", model, JsonRequestBehavior.AllowGet);
		}
	}
}