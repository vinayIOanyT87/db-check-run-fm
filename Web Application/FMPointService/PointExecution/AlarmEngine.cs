

namespace FMPointService.PointExecution
{
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using System.Reflection;
   using System.ServiceModel.Configuration;

   using CSScriptLibrary;

   using FMBusinessObjects.BusinessInterfaces;
   using FMBusinessObjects.ChannelFactories;
   using FMBusinessObjects.DataObjects;
   using FMBusinessObjects.Interfaces;
   using Opc.Ua;
   using Logging;
   using FMBusinessObjects.UtilityObjects;

   using FMPointCommon;

   using global::FMPointService.ThreadSupport;

   using InProcLogging;

   internal class AlarmEngine
	{
		protected object GetValueToCompare(PointTag inputTag, AlarmTestTemplate.TagFieldEnum field, long bitmask, AlarmTestTemplate.BitwiseOperatorEnum bitwiseOperator)
		{
			switch (field)
			{
				case AlarmTestTemplate.TagFieldEnum.OpcStatusSubCode:
					return this.BitMask(bitmask, inputTag.OpcStatusSubCode, bitwiseOperator);
				case AlarmTestTemplate.TagFieldEnum.Status:
					return this.BitMask(bitmask, (UInt32) (inputTag.Status & 0xFFFFFFFF), bitwiseOperator);
				case AlarmTestTemplate.TagFieldEnum.Value:
					return this.BitMask(bitmask, inputTag.Value, bitwiseOperator);
			}
			return null;
		}

		protected object GetLimitToCompare(PointTag limitTag)
		{
			if (limitTag.Value == null)
			{
				return null;
			}

			else if (limitTag.Value.GetType() == typeof(PointCommandStatusListReference))
			{
				var pointCommandStatusListReference = limitTag.Value as PointCommandStatusListReference;
				if (pointCommandStatusListReference == null || !pointCommandStatusListReference.CurrentValue.HasValue)
				{
					return null;
				}
				else
				{
					return pointCommandStatusListReference.CurrentValue.Value;
				}
			}
			else if (limitTag.Value.GetType() == typeof(DeviceAlarmMapReference))
			{
				var deviceAlarmMapReference = limitTag.Value as DeviceAlarmMapReference;
				if (deviceAlarmMapReference == null || !deviceAlarmMapReference.CurrentValue.HasValue)
				{
					return null;
				}
				else
				{
					return deviceAlarmMapReference.CurrentValue.Value;
				}
			}
			else
			{
				return limitTag.Value;
			}
		}

		protected object BitMask(long bitmask, object value, AlarmTestTemplate.BitwiseOperatorEnum bitwiseOperator)
		{
			if (value == null)
			{
				return null;
			}

			if (bitmask == -1)
			{
				if (value.GetType() == typeof(PointCommandStatusListReference))
				{
					var pointCommandStatusListReference = value as PointCommandStatusListReference;
					if (pointCommandStatusListReference == null || !pointCommandStatusListReference.CurrentValue.HasValue)
					{
						return null;
					}
					return pointCommandStatusListReference.CurrentValue.Value;
				}
				else if (value.GetType() == typeof(DeviceAlarmMapReference))
				{
					var deviceAlarmMapReference = value as DeviceAlarmMapReference;
					if (deviceAlarmMapReference == null || !deviceAlarmMapReference.CurrentValue.HasValue)
					{
						return null;
					}
					return deviceAlarmMapReference.CurrentValue.Value;
				}
				else
				{
					return value;
				}
			}

			if (value.GetType() == typeof(sbyte))
			{
				var valObj = (sbyte)value;
				var bitmaskObj = (sbyte)bitmask;
				switch (bitwiseOperator)
				{
					case AlarmTestTemplate.BitwiseOperatorEnum.And:
						valObj = (sbyte)(bitmaskObj & valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nor:
						valObj = (sbyte)(~(bitmaskObj | valObj));
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Or:
						valObj = (sbyte)(bitmaskObj | valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Xor:
						valObj = (sbyte)(bitmaskObj ^ valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nand:
						valObj = (sbyte)(~(bitmaskObj & valObj));
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nxor:
						valObj = (sbyte)(~(bitmaskObj ^ valObj));
						break;
					default:
						valObj = (sbyte)(bitmaskObj & valObj);
						break;
				}
				return valObj;
			}

			if (value.GetType() == typeof(short))
			{
				var valObj = (short)value;
				var bitmaskObj = (short)bitmask;
				switch (bitwiseOperator)
				{
					case AlarmTestTemplate.BitwiseOperatorEnum.And:
						valObj = (short)(bitmaskObj & valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nor:
						valObj = (short)(~(bitmaskObj | valObj));
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Or:
						valObj = (short)(bitmaskObj | valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Xor:
						valObj = (short)(bitmaskObj ^ valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nand:
						valObj = (short)(~(bitmaskObj & valObj));
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nxor:
						valObj = (short)(~(bitmaskObj ^ valObj));
						break;
					default:
						valObj = (short)(bitmaskObj & valObj);
						break;
				}
				return valObj;
			}

			if (value.GetType() == typeof(int))
			{
				var valObj = (int)value;
				var bitmaskObj = (int)bitmask;
				switch (bitwiseOperator)
				{
					case AlarmTestTemplate.BitwiseOperatorEnum.And:
						valObj = (int)(bitmaskObj & valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nor:
						valObj = (int)(~(bitmaskObj | valObj));
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Or:
						valObj = (int)(bitmaskObj | valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Xor:
						valObj = (int)(bitmaskObj ^ valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nand:
						valObj = (int)(~(bitmaskObj & valObj));
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nxor:
						valObj = (int)(~(bitmaskObj ^ valObj));
						break;
					default:
						valObj = (int)(bitmaskObj & valObj);
						break;
				}
				return valObj;
			}

			if (value.GetType() == typeof(long))
			{
				var valObj = (long)value;
				var bitmaskObj = (long)bitmask;
				switch (bitwiseOperator)
				{
					case AlarmTestTemplate.BitwiseOperatorEnum.And:
						valObj = (long)(bitmaskObj & valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nor:
						valObj = (long)(~(bitmaskObj | valObj));
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Or:
						valObj = (long)(bitmaskObj | valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Xor:
						valObj = (long)(bitmaskObj ^ valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nand:
						valObj = (long)(~(bitmaskObj & valObj));
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nxor:
						valObj = (long)(~(bitmaskObj ^ valObj));
						break;
					default:
						valObj = (long)(bitmaskObj & valObj);
						break;
				}
				return valObj;
			}

			if (value.GetType() == typeof(byte))
			{
				var valObj = (byte)value;
				var bitmaskObj = (byte)bitmask;
				switch (bitwiseOperator)
				{
					case AlarmTestTemplate.BitwiseOperatorEnum.And:
						valObj = (byte)(bitmaskObj & valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nor:
						valObj = (byte)(~(bitmaskObj | valObj));
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Or:
						valObj = (byte)(bitmaskObj | valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Xor:
						valObj = (byte)(bitmaskObj ^ valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nand:
						valObj = (byte)(~(bitmaskObj & valObj));
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nxor:
						valObj = (byte)(~(bitmaskObj ^ valObj));
						break;
					default:
						valObj = (byte)(bitmaskObj & valObj);
						break;
				}
				return valObj;
			}

			if (value.GetType() == typeof(ushort))
			{
				var valObj = (ushort)value;
				var bitmaskObj = (ushort)bitmask;
				switch (bitwiseOperator)
				{
					case AlarmTestTemplate.BitwiseOperatorEnum.And:
						valObj = (ushort)(bitmaskObj & valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nor:
						valObj = (ushort)(~(bitmaskObj | valObj));
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Or:
						valObj = (ushort)(bitmaskObj | valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Xor:
						valObj = (ushort)(bitmaskObj ^ valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nand:
						valObj = (ushort)(~(bitmaskObj & valObj));
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nxor:
						valObj = (ushort)(~(bitmaskObj ^ valObj));
						break;
					default:
						valObj = (ushort)(bitmaskObj & valObj);
						break;
				}
				return valObj;
			}

			if (value.GetType() == typeof(uint))
			{
				var valObj = (uint)value;
				var bitmaskObj = (uint)bitmask;
				switch (bitwiseOperator)
				{
					case AlarmTestTemplate.BitwiseOperatorEnum.And:
						valObj = (uint)(bitmaskObj & valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nor:
						valObj = (uint)(~(bitmaskObj | valObj));
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Or:
						valObj = (uint)(bitmaskObj | valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Xor:
						valObj = (uint)(bitmaskObj ^ valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nand:
						valObj = (uint)(~(bitmaskObj & valObj));
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nxor:
						valObj = (uint)(~(bitmaskObj ^ valObj));
						break;
					default:
						valObj = (uint)(bitmaskObj & valObj);
						break;
				}
				return valObj;
			}

			if (value.GetType() == typeof(ulong))
			{
				var valObj = (ulong)value;
				var bitmaskObj = (ulong)bitmask;
				switch (bitwiseOperator)
				{
					case AlarmTestTemplate.BitwiseOperatorEnum.And:
						valObj = (ulong)(bitmaskObj & valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nor:
						valObj = (ulong)(~(bitmaskObj | valObj));
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Or:
						valObj = (ulong)(bitmaskObj | valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Xor:
						valObj = (ulong)(bitmaskObj ^ valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nand:
						valObj = (ulong)(~(bitmaskObj & valObj));
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nxor:
						valObj = (ulong)(~(bitmaskObj ^ valObj));
						break;
					default:
						valObj = (ulong)(bitmaskObj & valObj);
						break;
				}
				return valObj;
			}

			if (value.GetType() == typeof(DeviceAlarmMapReference))
			{
				var deviceAlarmMapReference = value as DeviceAlarmMapReference;
				if(deviceAlarmMapReference == null || !deviceAlarmMapReference.CurrentValue.HasValue)
				{
					return null;
				}

				var valObj = (UInt32)deviceAlarmMapReference.CurrentValue.Value;
				var bitmaskObj = (UInt32)bitmask;
				switch (bitwiseOperator)
				{
					case AlarmTestTemplate.BitwiseOperatorEnum.And:
						valObj = (UInt32)(bitmaskObj & valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nor:
						valObj = (UInt32)(~(bitmaskObj | valObj));
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Or:
						valObj = (UInt32)(bitmaskObj | valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Xor:
						valObj = (UInt32)(bitmaskObj ^ valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nand:
						valObj = (UInt32)(~(bitmaskObj & valObj));
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nxor:
						valObj = (UInt32)(~(bitmaskObj ^ valObj));
						break;
					default:
						valObj = (UInt32)(bitmaskObj & valObj);
						break;
				}
				return valObj;
			}

			if (value.GetType() == typeof(PointCommandStatusListReference))
			{
				var pointCommandStatusListReference = value as PointCommandStatusListReference;
				if (pointCommandStatusListReference == null || !pointCommandStatusListReference.CurrentValue.HasValue)
				{
					return null;
				}

				var valObj = (Int32)pointCommandStatusListReference.CurrentValue.Value;
				var bitmaskObj = (ushort)bitmask;
				switch (bitwiseOperator)
				{
					case AlarmTestTemplate.BitwiseOperatorEnum.And:
						valObj = (Int32)(bitmaskObj & valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nor:
						valObj = (Int32)(~(bitmaskObj | valObj));
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Or:
						valObj = (Int32)(bitmaskObj | valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Xor:
						valObj = (Int32)(bitmaskObj ^ valObj);
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nand:
						valObj = (Int32)(~(bitmaskObj & valObj));
						break;
					case AlarmTestTemplate.BitwiseOperatorEnum.Nxor:
						valObj = (Int32)(~(bitmaskObj ^ valObj));
						break;
					default:
						valObj = (Int32)(bitmaskObj & valObj);
						break;
				}
				return valObj;
			}


			return value;
		}


		protected bool IsCompareFailed(ulong value, ulong limit, AlarmTestTemplate.TestTypeEnum testType, double holdOff, bool prevFailed)
		{
			ulong lHoldOff = (ulong)Math.Abs(holdOff);
			if (prevFailed)
			{
				switch (testType)
				{
					case AlarmTestTemplate.TestTypeEnum.Equals:
						return (value >= limit - lHoldOff && value <= limit + lHoldOff);
					case AlarmTestTemplate.TestTypeEnum.GreaterThan:
						return (value > limit - lHoldOff);
					case AlarmTestTemplate.TestTypeEnum.GreaterThanOrEqual:
						return (value >= limit - lHoldOff);
					case AlarmTestTemplate.TestTypeEnum.LessThan:
						return (value < limit + lHoldOff);
					case AlarmTestTemplate.TestTypeEnum.LessThanOrEqual:
						return (value <= limit + lHoldOff);
					case AlarmTestTemplate.TestTypeEnum.NotEquals:
						// deadband does not apply to not equals
						return !(value >= limit && value <= limit);
				}
			}
			else
			{
				switch (testType)
				{
					case AlarmTestTemplate.TestTypeEnum.Equals:
						return (value == limit);
					case AlarmTestTemplate.TestTypeEnum.GreaterThan:
						return (value > limit );
					case AlarmTestTemplate.TestTypeEnum.GreaterThanOrEqual:
						return (value >= limit );
					case AlarmTestTemplate.TestTypeEnum.LessThan:
						return (value < limit );
					case AlarmTestTemplate.TestTypeEnum.LessThanOrEqual:
						return (value <= limit );
					case AlarmTestTemplate.TestTypeEnum.NotEquals:
						return (value != limit);
				}
			}
			return true;
		}

		protected bool IsCompareFailed(long value, long limit, AlarmTestTemplate.TestTypeEnum testType, double holdOff, bool prevFailed)
		{
			long lHoldOff = (long)Math.Abs(holdOff);
			if (prevFailed)
			{
				switch (testType)
				{
					case AlarmTestTemplate.TestTypeEnum.Equals:
						return (value >= limit - lHoldOff && value <= limit + lHoldOff);
					case AlarmTestTemplate.TestTypeEnum.GreaterThan:
						return (value > limit - lHoldOff);
					case AlarmTestTemplate.TestTypeEnum.GreaterThanOrEqual:
						return (value >= limit - lHoldOff);
					case AlarmTestTemplate.TestTypeEnum.LessThan:
						return (value < limit + lHoldOff);
					case AlarmTestTemplate.TestTypeEnum.LessThanOrEqual:
						return (value <= limit + lHoldOff);
					case AlarmTestTemplate.TestTypeEnum.NotEquals:
						// deadband does not apply to not equals
						return !(value >= limit && value <= limit);
				}
			}
			else
			{
				switch (testType)
				{
					case AlarmTestTemplate.TestTypeEnum.Equals:
						return (value == limit );
					case AlarmTestTemplate.TestTypeEnum.GreaterThan:
						return (value > limit );
					case AlarmTestTemplate.TestTypeEnum.GreaterThanOrEqual:
						return (value >= limit );
					case AlarmTestTemplate.TestTypeEnum.LessThan:
						return (value < limit );
					case AlarmTestTemplate.TestTypeEnum.LessThanOrEqual:
						return (value <= limit );
					case AlarmTestTemplate.TestTypeEnum.NotEquals:
						return (value != limit );
				}
			}
			return true;
		}

		protected bool IsCompareFailed(bool value, bool limit, AlarmTestTemplate.TestTypeEnum testType, double holdOff, bool prevFailed)
		{
			switch (testType)
			{
				case AlarmTestTemplate.TestTypeEnum.Equals:
					return (value == limit);
				case AlarmTestTemplate.TestTypeEnum.NotEquals:
					return (value != limit);
			}
			return true;
		}

		protected bool IsCompareFailed(string value, string limit, AlarmTestTemplate.TestTypeEnum testType, double holdOff, bool prevFailed)
		{
			switch (testType)
			{
				case AlarmTestTemplate.TestTypeEnum.Equals:
					return (value == limit);
				case AlarmTestTemplate.TestTypeEnum.GreaterThan:
					return (value.CompareTo(limit) > 0);
				case AlarmTestTemplate.TestTypeEnum.GreaterThanOrEqual:
					return (value.CompareTo(limit) >= 0);
				case AlarmTestTemplate.TestTypeEnum.LessThan:
					return (value.CompareTo(limit) < 0);
				case AlarmTestTemplate.TestTypeEnum.LessThanOrEqual:
					return (value.CompareTo(limit) <= 0);
				case AlarmTestTemplate.TestTypeEnum.NotEquals:
					return (value != limit);
			}
			return true;
		}

		protected bool IsCompareFailed(double value, double limit, AlarmTestTemplate.TestTypeEnum testType, double holdOff, bool prevFailed)
		{
			double lHoldOff = (double)Math.Abs(holdOff);
			if (prevFailed)
			{
				switch (testType)
				{
					case AlarmTestTemplate.TestTypeEnum.Equals:
						return (value >= limit - lHoldOff && value <= limit + lHoldOff);
					case AlarmTestTemplate.TestTypeEnum.GreaterThan:
						return (value > limit - lHoldOff);
					case AlarmTestTemplate.TestTypeEnum.GreaterThanOrEqual:
						return (value >= limit - lHoldOff);
					case AlarmTestTemplate.TestTypeEnum.LessThan:
						return (value < limit + lHoldOff);
					case AlarmTestTemplate.TestTypeEnum.LessThanOrEqual:
						return (value <= limit + lHoldOff);
					case AlarmTestTemplate.TestTypeEnum.NotEquals:
						// deadband does not apply to not equals
						return !(value >= limit && value <= limit);
				}
			}
			else
			{
				switch (testType)
				{
					case AlarmTestTemplate.TestTypeEnum.Equals:
						return (value == limit );
					case AlarmTestTemplate.TestTypeEnum.GreaterThan:
						return (value > limit );
					case AlarmTestTemplate.TestTypeEnum.GreaterThanOrEqual:
						return (value >= limit );
					case AlarmTestTemplate.TestTypeEnum.LessThan:
						return (value < limit );
					case AlarmTestTemplate.TestTypeEnum.LessThanOrEqual:
						return (value <= limit );
					case AlarmTestTemplate.TestTypeEnum.NotEquals:
						return (value != limit );
				}
			}
			return true;
		}

		protected bool IsCompareFailed(float value, float limit, AlarmTestTemplate.TestTypeEnum testType, double holdOff, bool prevFailed)
		{
			float lHoldOff = (float)Math.Abs(holdOff);
			if (prevFailed)
			{
				switch (testType)
				{
					case AlarmTestTemplate.TestTypeEnum.Equals:
						return (value >= limit - lHoldOff && value <= limit + lHoldOff);
					case AlarmTestTemplate.TestTypeEnum.GreaterThan:
						return (value > limit - lHoldOff);
					case AlarmTestTemplate.TestTypeEnum.GreaterThanOrEqual:
						return (value >= limit - lHoldOff);
					case AlarmTestTemplate.TestTypeEnum.LessThan:
						return (value < limit + lHoldOff);
					case AlarmTestTemplate.TestTypeEnum.LessThanOrEqual:
						return (value <= limit + lHoldOff);
					case AlarmTestTemplate.TestTypeEnum.NotEquals:
						// deadband does not apply to not equals
						return !(value >= limit && value <= limit);
				}
			}
			else
			{
				switch (testType)
				{
					case AlarmTestTemplate.TestTypeEnum.Equals:
						return (value == limit );
					case AlarmTestTemplate.TestTypeEnum.GreaterThan:
						return (value > limit );
					case AlarmTestTemplate.TestTypeEnum.GreaterThanOrEqual:
						return (value >= limit );
					case AlarmTestTemplate.TestTypeEnum.LessThan:
						return (value < limit );
					case AlarmTestTemplate.TestTypeEnum.LessThanOrEqual:
						return (value <= limit );
					case AlarmTestTemplate.TestTypeEnum.NotEquals:
						return !(value != limit );
				}
			}
			return true;
		}

		protected bool IsCompareFailed(object value, object limit, AlarmTestTemplate.TestTypeEnum testType, double holdOff, bool prevFailed)
		{
			var valueType = value.GetType();

			if (valueType.IsEnum)
			{
				var valObj = (long)(int)value;
				var limitObj = (long)(int)limit;
				return this.IsCompareFailed(valObj, limitObj, testType, holdOff, prevFailed);
			}
			else if (valueType == typeof(double))
			{
				var valObj = (double)value;
				var limitObj = (double)limit;
				return this.IsCompareFailed(valObj, limitObj, testType, holdOff, prevFailed);
			}
			else if (valueType == typeof(float))
			{
				var valObj = (float)value;
				var limitObj = (float)limit;
				return this.IsCompareFailed(valObj, limitObj, testType, holdOff, prevFailed);
			}
			else if (valueType == typeof(sbyte))
			{
				var valObj = (long)(sbyte)value;
				var limitObj = (long)(sbyte)limit;
				return this.IsCompareFailed(valObj, limitObj, testType, holdOff, prevFailed);
			}
			else if (valueType == typeof(short))
			{
				var valObj = (long)(short)value;
				var limitObj = (long)(short)limit;
				return this.IsCompareFailed(valObj, limitObj, testType, holdOff, prevFailed);
			}
			else if (valueType == typeof(int))
			{
				var valObj = (long)(int)value;
				var limitObj = (long)(int)limit;
				return this.IsCompareFailed(valObj, limitObj, testType, holdOff, prevFailed);
			}
			else if (valueType == typeof(long))
			{
				var valObj = (long)value;
				var limitObj = (long)limit;
				return this.IsCompareFailed(valObj, limitObj, testType, holdOff, prevFailed);
			}
			else if (valueType == typeof(byte))
			{
				var valObj = (ulong)(byte)value;
				var limitObj = (ulong)(byte)limit;
				return this.IsCompareFailed(valObj, limitObj, testType, holdOff, prevFailed);
			}
			else if (valueType == typeof(ushort))
			{
				var valObj = (ulong)(ushort)value;
				var limitObj = (ulong)(ushort)limit;
				return this.IsCompareFailed(valObj, limitObj, testType, holdOff, prevFailed);
			}
			else if (valueType == typeof(uint))
			{
				var valObj = (ulong)(uint)value;
				var limitObj = (ulong)(uint)limit;
				return this.IsCompareFailed(valObj, limitObj, testType, holdOff, prevFailed);
			}
			else if (valueType == typeof(ulong))
			{
				var valObj = (ulong)value;
				var limitObj = (ulong)limit;
				return this.IsCompareFailed(valObj, limitObj, testType, holdOff, prevFailed);
			}
			else if (valueType == typeof(string))
			{
				var valObj = (string)value;
				var limitObj = (string)limit;
				return this.IsCompareFailed(valObj, limitObj, testType, holdOff, prevFailed);
			}
			else if (valueType == typeof(bool))
			{
				var valObj = (bool)value;
				var limitObj = (bool)limit;
				return this.IsCompareFailed(valObj, limitObj, testType, holdOff, prevFailed);
			}
			else if (valueType == typeof(PointCommandStatusListReference))
			{
				var valObj = ((value as PointCommandStatusListReference).CurrentValue.HasValue) ? (value as PointCommandStatusListReference).CurrentValue.Value : 0;
				var limitObj = ((limit as PointCommandStatusListReference).CurrentValue.HasValue) ? (limit as PointCommandStatusListReference).CurrentValue.Value : 0;
				return this.IsCompareFailed(valObj, limitObj, testType, holdOff, prevFailed);
			}
			else if (valueType == typeof(DeviceAlarmMapReference))
			{
				var valObj = (ulong)(((value as DeviceAlarmMapReference).CurrentValue.HasValue) ? (value as DeviceAlarmMapReference).CurrentValue.Value : 0);
				var limitObj = (ulong)(UInt32)limit;
				return this.IsCompareFailed(valObj, limitObj, testType, holdOff, prevFailed);
			}



			return true;
		}

		protected bool IsAlarmTestFailed(PointTag inputTag, AlarmTest alarmTest, PointTag limitTag, PointTagAlarmStatus ptas)
		{
			var timerName = "AlarmTest_" + alarmTest.AlarmTestGuid;
			var tagValue = this.GetValueToCompare(inputTag, alarmTest.TagField, alarmTest.BitMask, alarmTest.BitwiseOperator);
			var limitValue = this.GetLimitToCompare(limitTag);

			if(ptas.AlarmTestFailed
			&& ptas.ReAlarm
			&& !ptas.ReAlarmDone)
			{
				ptas.ReAlarmInProgress = true;
			}

			if (!ptas.ReAlarm
			&& ptas.ReAlarmInProgress)
			{
				ptas.ReAlarmInProgress = false;
				ptas.ReAlarmDone = false;
			}


			var prevFailed = ptas.AlarmTestFailed;
			var prevInTimedHoldOff = ptas.AlarmTestInTimedHoldOff;
			var currentFailed = this.IsCompareFailed(tagValue, limitValue, alarmTest.TestType, alarmTest.Holdoff, prevFailed);
			var timeHoldOffInSeconds = alarmTest.TimedHoldOffInSeconds;


			if ((timeHoldOffInSeconds > 0
			&& currentFailed
			&& !prevFailed)
			|| ptas.ReAlarmInProgress)
			{
            var timerExpiration = ptas.AlarmTestInTimedHoldOffTimestamp ?? DateTimeOffset.UtcNow.AddSeconds(timeHoldOffInSeconds);

				if (!prevInTimedHoldOff)
				{
					ptas.AlarmTestInTimedHoldOff = currentFailed;
               ptas.AlarmTestInTimedHoldOffTimestamp = inputTag.ServerTimeStamp.AddSeconds(timeHoldOffInSeconds);
               SRMTimerFunctions.RemoveTimer(timerName);
					SRMTimerFunctions.AddTimer(timerName, inputTag.PointGuid, timerExpiration);
               return prevFailed;
				}

				else if (DateTimeOffset.UtcNow >= timerExpiration)
            {
               if (ptas.ReAlarmInProgress)
					{
						ptas.ReAlarmInProgress = false;
						ptas.ReAlarmDone = true;
					}
					SRMTimerFunctions.RemoveTimer(timerName);
					return currentFailed;
				}

				return prevFailed;
			}
			else
			{
				if (ptas.ReAlarmInProgress)
				{
					ptas.ReAlarmInProgress = false;
					ptas.ReAlarmDone = true;
				}

				if (timeHoldOffInSeconds > 0)
				{
					SRMTimerFunctions.RemoveTimer(timerName);
				}

				ptas.AlarmTestInTimedHoldOff = false;
				ptas.AlarmTestInTimedHoldOffTimestamp = null;
				return currentFailed;
			}
		}

		protected void UpdateAlarmStatus(Point point, PointTag inputTag,Alarm alarm, PointTagAlarmStatus ptas, AlarmTest alarmTest, DateTimeOffset alarmTestFailedTimestamp, bool isFailed, bool setAcknowledged = false)
		{
			if (isFailed != ptas.AlarmTestFailed
			|| ptas.ReAlarmDone)
			{
            ptas.AlarmTestFailed = isFailed;
				if (isFailed
				|| ptas.ReAlarmDone)
				{
					if (ptas.AlarmTestInTimedHoldOffTimestamp != null && ptas.AlarmTestInTimedHoldOffTimestamp.HasValue)
					{
                  ptas.AlarmTestFailedTimestamp = ptas.AlarmTestInTimedHoldOffTimestamp.Value;
               }
               else
					{
						ptas.AlarmTestFailedTimestamp = alarmTestFailedTimestamp.AddSeconds(alarmTest.TimedHoldOffInSeconds);
					}
					ptas.Acknowledged = false;
					ptas.AcknowledgedBy = null;
					ptas.AcknowledgedTimestamp = null;
					ptas.AcknowledgedComment = null;
					ptas.Silenced = false;
					ptas.SilencedBy = null;
					ptas.SilencedTimestamp = null;
					ptas.UpdatedDate = inputTag.ServerTimeStamp;
					inputTag.ServerTimeStamp = DateTimeOffset.UtcNow;
					inputTag.SourceTimeStamp = DateTimeOffset.UtcNow;
				}
				else
				{
					if (setAcknowledged)
					{
						ptas.Acknowledged = true;
					}
				}

				this.AlarmStatusChangedList.Add(ptas);
				AlarmAndEventArchiveFunctions.Archive(new AandEDataElement(ptas,point,inputTag,alarm, alarmTest));
			}
		}

		protected void EvaluateAlarmTests(Point point,
			PointTag inputTag,
			Alarm alarm,
			Dictionary<Guid,
			PointTag> limitTagDictionary,
			Dictionary<Guid, PointTagAlarmStatus> alarmStatusDictionary,
			DateTimeOffset alarmTestFailedTimestamp)
		{
			bool highestPriorityAlarmReported = false;
			var orderedAlarmTest = alarm.AlarmTests.Values.ToList().OrderBy(at => at.Order);
			foreach (var alarmTest in orderedAlarmTest)
			{
				PointTag limitTag;
				if (limitTagDictionary.TryGetValue(alarmTest.LimitTagGuid, out limitTag))
				{
					var ptas = alarmStatusDictionary[alarmTest.AlarmTestGuid];

					if (inputTag.AlarmsEnabled
					&& alarm.Enabled
					&& alarmTest.Enabled
					&& (alarmTest.TagField != AlarmTestTemplate.TagFieldEnum.Value || this.IsGoodOrOverrideAndNotNull(inputTag))
					&& this.IsGoodOrOverrideAndNotNull(limitTag))
					{

						if (highestPriorityAlarmReported)
						{
							this.UpdateAlarmStatus(point, inputTag, alarm, ptas, alarmTest, alarmTestFailedTimestamp, false, true);
						}
						else
						{

							var testFailed = this.IsAlarmTestFailed(inputTag, alarmTest, limitTag, ptas);
							if (testFailed)
							{
								highestPriorityAlarmReported = true;

								// Identify the Highest Prioirty AlarmTestFailed
								if ((inputTag.HighestPriorityAlarm == null
								|| inputTag.HighestOrderAlarmTest.AlarmPriority > alarmTest.AlarmPriority)
								|| (inputTag.HighestOrderPointTagAlarmStatus != null
								&& !inputTag.HighestOrderPointTagAlarmStatus.AlarmTestFailed))
								{
									inputTag.HighestPriorityAlarm = alarm;
									inputTag.HighestOrderAlarmTest = alarmTest;
									inputTag.HighestOrderPointTagAlarmStatus = alarmStatusDictionary[inputTag.HighestOrderAlarmTest.AlarmTestGuid];
								}
							}
							else 
							{
								// Identify any Normal Unacknowledged
								if (inputTag.HighestOrderPointTagAlarmStatus == null
								|| (!inputTag.HighestOrderPointTagAlarmStatus.AlarmTestFailed
								&& inputTag.HighestOrderPointTagAlarmStatus.Acknowledged))
								{
									inputTag.HighestPriorityAlarm = alarm;
									inputTag.HighestOrderAlarmTest = alarmTest;
									inputTag.HighestOrderPointTagAlarmStatus = alarmStatusDictionary[inputTag.HighestOrderAlarmTest.AlarmTestGuid];
								}
							}

							this.UpdateAlarmStatus(point, inputTag, alarm, ptas, alarmTest, alarmTestFailedTimestamp, testFailed);
						}
					}
					else
					{
						this.UpdateAlarmStatus(point, inputTag,alarm, ptas, alarmTest, alarmTestFailedTimestamp, false);
					}
				}
			}
		}

		protected AlarmTest FindLowestOrderActiveAlarmTest(Alarm alarm)
		{
			PointTagAlarmStatus lowestAlarmStatus = null;
			AlarmTest lowestAlarmStatusAlarmTest = null;

			foreach (var alarmStatus in alarm.AlarmStatus.Values)
			{

				if (alarmStatus.AlarmTestFailed)
				{
					if (lowestAlarmStatus == null)
					{
						lowestAlarmStatus = alarmStatus;
						lowestAlarmStatusAlarmTest = alarm.AlarmTests[lowestAlarmStatus.AlarmTestGuid];
					}
					else
					{
						var alarmTest = alarm.AlarmTests[alarmStatus.AlarmTestGuid];

						if (alarmTest.Order < lowestAlarmStatusAlarmTest.Order)
						{
							lowestAlarmStatus = alarmStatus;
							lowestAlarmStatusAlarmTest = alarm.AlarmTests[lowestAlarmStatus.AlarmTestGuid];
						}
					}
				}
			}
			return lowestAlarmStatusAlarmTest;
		}

		#region Status Abstraction

		protected bool IsGoodOrOverrideAndNotNull(PointTag tag)
		{
			return ((this.IsGood(tag) || this.IsOverride(tag)) && tag.Value != null);
		}

		protected bool IsGoodOrOverride(PointTag tag)
		{
			return (this.IsGood(tag) || this.IsOverride(tag));
		}

		protected bool IsGood(PointTag tag)
		{
			var statusCode = new StatusCode((uint)tag.Status);
			return (StatusCode.IsNotBad(statusCode) && StatusCode.IsNotBad(tag.OpcStatusSubCode));
		}

		protected bool IsOverride(PointTag tag)
		{
			var statusCode = new StatusCode((uint)tag.Status);
			return (StatusCode.IsGood(statusCode) && tag.OpcStatusCodeBits == StatusCodes.GoodLocalOverride);
		}

		protected bool IsBad(PointTag tag)
		{
			var statusCode = new StatusCode((uint)tag.Status);
			return StatusCode.IsBad(statusCode);
		}

		protected bool IsStatusNotEquals(PointTag tag, uint status)
		{
			return (tag.Status != status);
		}

		protected bool IsStatusEquals(PointTag tag, uint status)
		{
			return (tag.Status == status);
		}

		protected void SetGood(PointTag tag)
		{
			tag.Status = StatusCodes.Good;
		}

		protected void SetOverride(PointTag tag)
		{
			tag.Status = StatusCodes.GoodLocalOverride;
		}

		protected void SetBad(PointTag tag)
		{
			tag.Status = StatusCodes.Bad;
		}

		protected void SetStatusCode(PointTag tag, uint status)
		{
			tag.Status = status;
		}

		#endregion Status Abstraction

		protected uint GetAlarmStateStatus(Alarm alarm, Dictionary<Guid, PointTag> limitTags, PointTag inputTag, PointTag alarmStateTag)
		{
			uint alarmStateStatus = StatusCodes.Good;

			if (!this.IsOverride(alarmStateTag))
			{
				int counterGoodLimits = 0;

				foreach (var alarmTest in alarm.AlarmTests.Values)
				{
					PointTag limitTag;
					if (limitTags.TryGetValue(alarmTest.LimitTagGuid, out limitTag))
					{
						if ((this.IsGoodOrOverride(inputTag)
						|| alarmTest.TagField == AlarmTestTemplate.TagFieldEnum.Status
						|| alarmTest.TagField == AlarmTestTemplate.TagFieldEnum.OpcStatusSubCode)
						&& this.IsGoodOrOverride(limitTag))
						{
							counterGoodLimits++;
						}
					}
				}

				if (counterGoodLimits == limitTags.Count)
				{
					alarmStateStatus = StatusCodes.Good;
				}
				else if (counterGoodLimits < limitTags.Count && counterGoodLimits > 0)
				{
					alarmStateStatus = StatusCodes.GoodResultsMayBeIncomplete;
				}
				else if (counterGoodLimits == 0)
				{
					alarmStateStatus = StatusCodes.Bad;
				}
			}
			else
			{
				alarmStateStatus = StatusCodes.GoodLocalOverride;
			}

			return alarmStateStatus;
		}

		protected void SetAlarmStatusTag(Alarm alarm, Dictionary<Guid, PointTag> limitTagDictionary, PointTag inputTag, PointTag alarmStateTag)
		{

			if (!inputTag.AlarmsEnabled
			|| !alarm.Enabled)
			{
				alarmStateTag.Status = StatusCodes.BadOutOfService;
				alarmStateTag.Value = null;
			}
			else
			{

				if (!(this.IsOverride(alarmStateTag)) && alarm.AlarmStatus.Any())
				{
					var alarmStateStatus = this.GetAlarmStateStatus(alarm, limitTagDictionary, inputTag, alarmStateTag);
					string alarmState = alarm.GetActiveAlarmState(false);
					//Find lowest order alarmTest
					if (alarmState == alarm.NotAlarmState && (string)alarmStateTag.Value != alarm.NotAlarmState)
					{
						if (alarm.ShelvedOneShot)
						{
							alarm.ShelvedOneShot = false;
							var alarmClone = (Alarm)alarm.Clone();
							alarmClone.AlarmStatus = new Dictionary<Guid, PointTagAlarmStatus>();
							alarmClone.AlarmTests = new Dictionary<Guid, AlarmTest>();
							this.ShelvedOneShotChangedList.Add(alarmClone);
						}
					}

					//Set alarm state 
					if (alarmState != (string)alarmStateTag.Value || this.IsStatusNotEquals(alarmStateTag, alarmStateStatus))
					{
						alarmStateTag.Value = alarmState;
						this.SetStatusCode(alarmStateTag, alarmStateStatus);
						alarmStateTag.ServerTimeStamp = inputTag.ServerTimeStamp;
						alarmStateTag.SourceTimeStamp = inputTag.SourceTimeStamp;
						alarmStateTag.UpdatedDate = alarmStateTag.ServerTimeStamp;
					}
				}
			}
		}

		protected void SetAlarmStatusTagToMismatchType(PointTag alarmStateTag)
		{
			this.SetStatusCode(alarmStateTag, StatusCodes.BadTypeMismatch);
			alarmStateTag.ServerTimeStamp = DateTimeOffset.UtcNow;
			alarmStateTag.SourceTimeStamp = DateTimeOffset.UtcNow;
			alarmStateTag.UpdatedDate = alarmStateTag.ServerTimeStamp;
		}

		protected void SetAlarmStatusTagToBad(PointTag alarmStateTag)
		{
			alarmStateTag.Value = null;
			this.SetStatusCode(alarmStateTag, StatusCodes.Bad);
			alarmStateTag.ServerTimeStamp = DateTimeOffset.UtcNow;
			alarmStateTag.SourceTimeStamp = DateTimeOffset.UtcNow;
			alarmStateTag.UpdatedDate = alarmStateTag.ServerTimeStamp;
		}

		protected ETagMatchEnum IsTypeMismatchBetweenTags(PointTag inputTag, AlarmTest alarmTest, PointTag limitTag)
		{
			var tagValue = this.GetValueToCompare(inputTag, alarmTest.TagField, alarmTest.BitMask, alarmTest.BitwiseOperator);
			object limitValue = this.GetLimitToCompare(limitTag);
			if (tagValue == null || limitValue == null)
			{
				return ETagMatchEnum.Null;
			}
			return (tagValue.GetType() != limitValue.GetType()) ? ETagMatchEnum.Mismatch : ETagMatchEnum.Good;
		}

		protected enum ETagMatchEnum
		{
			Good,
			Null,
			Mismatch
		}

		protected ETagMatchEnum CreateLimitTagDictionary(Point point, PointTag inputTag, Alarm alarm, out Dictionary<Guid, PointTag> limitTagDictionary)
		{
			ETagMatchEnum ret = ETagMatchEnum.Null;
			limitTagDictionary = new Dictionary<Guid, PointTag>();
			foreach (var alarmTest in alarm.AlarmTests.Values)
			{
				var limitTag = point.Tags[alarmTest.LimitTagGuid];
				var misMatchResult = this.IsTypeMismatchBetweenTags(inputTag, alarmTest, limitTag);
				if (misMatchResult == ETagMatchEnum.Mismatch)
				{
					return misMatchResult;
				}
				if (misMatchResult == ETagMatchEnum.Good)
				{
					ret = misMatchResult;
					limitTagDictionary.Add(alarmTest.LimitTagGuid, limitTag);
				}
			}
			return ret;
		}

		protected Dictionary<Guid, PointTagAlarmStatus> CreateAlarmStatusDictionaryIndexOfAlarmTest(Alarm alarm)
		{
			Dictionary<Guid, PointTagAlarmStatus> alarmStatusDictionary = new Dictionary<Guid, PointTagAlarmStatus>();
			foreach (var ptas in alarm.AlarmStatus.Values)
			{
				alarmStatusDictionary.Add(ptas.AlarmTestGuid, ptas);
			}
			return alarmStatusDictionary;
		}

		protected void EvaluateTagAlarms(Point point, PointTag inputTag)
		{
			inputTag.HighestPriorityAlarm = null;
			inputTag.HighestOrderAlarmTest = null;
			inputTag.HighestOrderPointTagAlarmStatus = null;

			var alarmTestFailedTimestamp = inputTag.ServerTimeStamp;

         foreach (var alarm in inputTag.Alarms.Values)
			{

				if (alarm.AlarmTests.Any())
				{
					Dictionary<Guid, PointTag> limitTagDictionary;
					var misMatchResult = this.CreateLimitTagDictionary(point, inputTag, alarm, out limitTagDictionary);

					var alarmStateTag = point.Tags[alarm.AlarmStateTagGuid];

					switch (misMatchResult)
					{
						case ETagMatchEnum.Good:
							Dictionary<Guid, PointTagAlarmStatus> alarmStatusDictionary = this.CreateAlarmStatusDictionaryIndexOfAlarmTest(alarm);
							this.EvaluateAlarmTests(point, inputTag, alarm, limitTagDictionary, alarmStatusDictionary, alarmTestFailedTimestamp);
							this.SetAlarmStatusTag(alarm, limitTagDictionary, inputTag, alarmStateTag);
							break;
						case ETagMatchEnum.Mismatch:
							this.SetAlarmStatusTagToMismatchType(alarmStateTag);
							break;
						case ETagMatchEnum.Null:
							this.SetAlarmStatusTagToBad(alarmStateTag);
							break;
					}
				}
			}

			if(inputTag.HighestOrderAlarmTest != null)
			{
				if (inputTag.HighestOrderPointTagAlarmStatus.AlarmTestFailed)
				{
					if (!inputTag.HighestPriorityAlarm.ShelvedOneShot
					&& (inputTag.HighestPriorityAlarm.ShelvedEndTimeStamp == null
					|| inputTag.HighestPriorityAlarm.ShelvedEndTimeStamp < DateTimeOffset.UtcNow))
					{
						inputTag.AlarmPriorityGuid = inputTag.HighestOrderAlarmTest.AlarmPriorityGuid;
					}
					else
					{
						inputTag.AlarmPriorityGuid = Guid.Empty;
					}

					inputTag.AlarmState = inputTag.HighestOrderAlarmTest.AlarmState;
				}
				else
				{
					if (inputTag.HighestOrderPointTagAlarmStatus.Acknowledged)
					{
						inputTag.AlarmPriorityGuid = Guid.Empty;
					}
					else
					{
						if (!inputTag.HighestPriorityAlarm.ShelvedOneShot
						&& (inputTag.HighestPriorityAlarm.ShelvedEndTimeStamp == null
						|| inputTag.HighestPriorityAlarm.ShelvedEndTimeStamp < DateTimeOffset.UtcNow))
						{
							inputTag.AlarmPriorityGuid = inputTag.HighestOrderAlarmTest.NormalUnacknowledgedAlarmPriorityGuid;
						}
						else
						{
							inputTag.AlarmPriorityGuid = Guid.Empty;
						}
					}

					inputTag.AlarmState = inputTag.HighestPriorityAlarm.NotAlarmState;
				}

				inputTag.Acknowledged = inputTag.HighestOrderPointTagAlarmStatus.Acknowledged;
			}
			else
			{
				inputTag.AlarmPriorityGuid = Guid.Empty;
				inputTag.Acknowledged = true;
				inputTag.AlarmState = "Normal";
			}
		}

		public bool ReportAlarmStatusAndShelvedOneShotChangesEnable = true;

		protected void ReportPointAlarmStatusAndShelvedOneShotChanges(SecurityClass security, Point point)
		{
			if (this.ReportAlarmStatusAndShelvedOneShotChangesEnable
			&& ((this.AlarmStatusChangedList != null && this.AlarmStatusChangedList.Any())
			|| (this.ShelvedOneShotChangedList != null && this.ShelvedOneShotChangedList.Any())))
			{
				if (this.AlarmStatusChangedList != null && this.AlarmStatusChangedList.Any())
				{
					foreach (var alarmStatus in this.AlarmStatusChangedList)
					{
						//This entry is filling up the log file and not necessary. When Cassandra is running, the entries are sent to Cassandra. When it is not, they are recorded in the Event Log.
						//Logger.LogCritical(
						//	"AlarmEngine:ReportPointAlarmStatusChanges " + alarmStatus.AlarmID + ":" + alarmStatus.AlarmTestID
						//	+ " Failed Status " + (alarmStatus.AlarmTestFailed == false ? "false" : "true"));
					}
				}
				if (this.ShelvedOneShotChangedList != null && this.ShelvedOneShotChangedList.Any())
				{
					foreach (var alarm in this.ShelvedOneShotChangedList)
					{
							Logger.LogCritical(
								"AlarmEngine:ReportPointAlarmStatusChanges " + alarm.ID + " Shelved One Shot Status "
								+ (alarm.ShelvedOneShot == false ? "false" : "true"));
					}
				}
				FMChannelHelper.MakeCall<IPointServiceManager>(x => x.UpdateTestFailedAndOneShot(security,
																									this.AlarmStatusChangedList, 
																									this.ShelvedOneShotChangedList));
			}
		}

		protected List<PointTagAlarmStatus> AlarmStatusChangedList;

		protected List<Alarm> ShelvedOneShotChangedList;


		public void EvaluateAlarms(Point point, SecurityClass security)
		{
			if (point.Tags.Any())
			{
				this.AlarmStatusChangedList = new List<PointTagAlarmStatus>();
				this.ShelvedOneShotChangedList = new List<Alarm>();

				foreach (var tag in point.Tags.Values)
				{
					if (tag.Alarms.Any())
					{
						this.EvaluateTagAlarms(point, tag);
					}
				}
				this.ReportPointAlarmStatusAndShelvedOneShotChanges(security, point);
			}
		}
	}
}
