// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PointManager.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMPointCommon
{
	using FMBusinessObjects.DataObjects;
	using Opc.Ua;
	using System;
	using System.Globalization;
	using Varec.CommonComponents.EngineeringUnitsLibrary;

	public class PointManager
	{
		#region NodeId Manipulations
		#region NodeId Prefixes
		public const string SiteNodeIdPrefix = "Site.";
		public const string PointNodeIdPrefix = "Point.";
		public const string TagNodeIdPrefix = "Tag.";
		public const string SettingNodeIdPrefix = "Setting.";
		public const string AlarmSourceTagNodeIdPrefix = "AlarmSourceTag.";
		public const string AlarmMonitorNodeIdPrefix = "AlarmMonitor.";
		public const string AcknowledgeNodeIdPrefix = "Acknowledge.";
		public const string AcknowledgedNodeIdPrefix = "Acknowledged.";
		public const string AckInputsNodeIdPrefix = "AckInputs.";
		public const string AckOutputsNodeIdPrefix = "AckOutputs.";
		public const string DefinitionNodeIdPrefix = "Definition.";
		public const string PointTagAlarmStatusNodeIdPrefix = "Ptas.";
		#endregion NodeId Prefixes
		#region Helper functions to decide NodeID Type
		public static bool IsSiteNodeID(string nodeId)
		{
			return nodeId.StartsWith(SiteNodeIdPrefix);
		}
		public static bool IsPointNodeID(string nodeId)
		{
			return nodeId.StartsWith(PointNodeIdPrefix);
		}
		public static bool IsTagNodeID(string nodeId)
		{
			return nodeId.StartsWith(TagNodeIdPrefix);
		}
		public static bool IsSettingNodeID(string nodeId)
		{
			return nodeId.StartsWith(SettingNodeIdPrefix);
		}

		public static bool IsAlarmSourceTagNodeID(string nodeId)
		{
			return nodeId.StartsWith(AlarmSourceTagNodeIdPrefix);
		}
		public static bool IsAlarmMonitorNodeID(string nodeId)
		{
			return nodeId.StartsWith(AlarmMonitorNodeIdPrefix);
		}
		public static bool IsAcknowledgeNodeID(string nodeId)
		{
			return nodeId.StartsWith(AcknowledgeNodeIdPrefix);
		}
		public static bool IsAcknowledgedNodeID(string nodeId)
		{
			return nodeId.StartsWith(AcknowledgedNodeIdPrefix);
		}
		public static bool IsAckInputsNodeID(string nodeId)
		{
			return nodeId.StartsWith(AckInputsNodeIdPrefix);
		}
		public static bool IsAckOutputsNodeID(string nodeId)
		{
			return nodeId.StartsWith(AckOutputsNodeIdPrefix);
		}
		public static bool IsDefinitionNodeID(string nodeId)
		{
			return nodeId.StartsWith(DefinitionNodeIdPrefix);
		}
		public static bool IsPointTagAlarmStatusNodeID(string nodeId)
		{
			return nodeId.StartsWith(PointTagAlarmStatusNodeIdPrefix);
		}

		#endregion Helper functions to decide NodeID Type
		#region Helper functions to create NodeID
		public static string CreateSiteNodeID(Guid siteGuid)
		{
			return SiteNodeIdPrefix + siteGuid;
		}
		public static string CreatePointNodeID(Guid pointGuid, string propertyID)
		{
			return PointNodeIdPrefix + pointGuid + (string.IsNullOrEmpty(propertyID) ? "" : "." + propertyID);
		}
		public static string CreateTagNodeID(Guid tagGuid)
		{
			return TagNodeIdPrefix + tagGuid;
		}

		public static string CreateSettingNodeID(Guid pointGuid, Guid propertyGuid, string propertyID)
		{
			return SettingNodeIdPrefix + pointGuid + "." + propertyGuid + "." + propertyID;
		}


		public static string CreateAlarmSourceTagNodeID(Guid pointGuid, Guid tagGuid)
		{
			return AlarmSourceTagNodeIdPrefix + pointGuid + "." + tagGuid;
		}

		public static string CreateAlarmMonitorNodeID(Guid pointGuid, Guid tagGuid, Guid alarmGuid)
		{
			return AlarmMonitorNodeIdPrefix + pointGuid + "." + tagGuid + "." + alarmGuid;
		}

		public static string CreateAcknowledgedNodeID(Guid pointGuid, Guid tagGuid, Guid alarmGuid)
		{
			return AcknowledgedNodeIdPrefix + pointGuid + "." + tagGuid + "." + alarmGuid;
		}

		public static string CreateAcknowledgeNodeID(Guid pointGuid, Guid tagGuid, Guid alarmGuid)
		{
			return AcknowledgeNodeIdPrefix + pointGuid + "." + tagGuid + "." + alarmGuid;
		}

		public static string CreateAckInputsNodeID(Guid pointGuid, Guid tagGuid, Guid alarmGuid)
		{
			return AckInputsNodeIdPrefix + pointGuid + "." + tagGuid + "." + alarmGuid;
		}

		public static string CreateAckOutputsNodeID(Guid pointGuid, Guid tagGuid, Guid alarmGuid)
		{
			return AckOutputsNodeIdPrefix + pointGuid + "." + tagGuid + "." + alarmGuid;
		}

		public static string CreateDefinitionNodeID(Guid tagGuid)
		{
			return DefinitionNodeIdPrefix + tagGuid;
		}

		public static string CreatePointTagAlarmStatusNodeID(Guid pointTagAlarmStatusGuid)
		{
			return PointTagAlarmStatusNodeIdPrefix + pointTagAlarmStatusGuid;
		}


		#endregion Helper functions to create NodeID
		#region Helper functions to parse NodeID
		public static void ParseSiteNodeID(string nodeId, out Guid siteGuid)
		{
			siteGuid = new Guid(nodeId.Replace(SiteNodeIdPrefix, ""));
		}

		public static void ParsePointNodeID(string nodeId, out Guid pointGuid, out string propertyID)
		{
			var pointGuidAndPropertyID = nodeId.Replace(PointNodeIdPrefix, "");
			var periodIndex = pointGuidAndPropertyID.IndexOf(".", StringComparison.Ordinal);
			if (periodIndex == -1)
			{
				pointGuid = new Guid(pointGuidAndPropertyID);
				propertyID = null;
			}
			else
			{
				pointGuid = new Guid(pointGuidAndPropertyID.Substring(0, periodIndex));
				propertyID = pointGuidAndPropertyID.Substring(periodIndex + 1);
			}
		}

		public static void ParseTagNodeID(string nodeId, out Guid tagGuid)
		{
			tagGuid = new Guid(nodeId.Replace(TagNodeIdPrefix, ""));
		}

		public static void ParseSettingNodeID(string nodeId, out Guid pointGuid, out Guid propertyGuid, out string propertyID)
		{
			var pointGuidPropertyGuidAndPropertyID = nodeId.Replace(SettingNodeIdPrefix, "");
			var periodIndex = pointGuidPropertyGuidAndPropertyID.IndexOf(".", StringComparison.Ordinal);
			pointGuid = new Guid(pointGuidPropertyGuidAndPropertyID.Substring(0, periodIndex));
			var propertyGuidAndpropertyID = pointGuidPropertyGuidAndPropertyID.Substring(periodIndex + 1);
			periodIndex = propertyGuidAndpropertyID.IndexOf(".", StringComparison.Ordinal);
			propertyGuid = new Guid(propertyGuidAndpropertyID.Substring(0, periodIndex));
			propertyID = propertyGuidAndpropertyID.Substring(periodIndex + 1);
		}



		public static void ParseAlarmSourceTagNodeID(string nodeId, out Guid pointGuid, out Guid tagGuid)
		{
			var pointandtagGuid = nodeId.Replace(AlarmSourceTagNodeIdPrefix, "");
			var periodIndex = pointandtagGuid.IndexOf(".", StringComparison.Ordinal);
			pointGuid = new Guid(pointandtagGuid.Substring(0, periodIndex));
			tagGuid = new Guid(pointandtagGuid.Substring(periodIndex + 1));
		}

		protected static void GetPointTagAlarmGuid(string pointTagAlarmGuid, out Guid pointGuid, out Guid tagGuid, out Guid alarmGuid)
		{
			var periodIndex = pointTagAlarmGuid.IndexOf(".", StringComparison.Ordinal);
			var pointGuidStr = pointTagAlarmGuid.Substring(0, periodIndex);
			pointGuid = new Guid(pointGuidStr);
			var tagAlarmGuid = pointTagAlarmGuid.Replace(pointGuidStr + ".", "");
			periodIndex = tagAlarmGuid.IndexOf(".", StringComparison.Ordinal);
			tagGuid = new Guid(tagAlarmGuid.Substring(0, periodIndex));
			alarmGuid = new Guid(tagAlarmGuid.Substring(periodIndex + 1));
		}

		public static void ParseAlarmMonitorNodeID(string nodeId, out Guid pointGuid, out Guid tagGuid, out Guid alarmGuid)
		{
			var pointTagAlarmGuid = nodeId.Replace(AlarmMonitorNodeIdPrefix, "");
			GetPointTagAlarmGuid(pointTagAlarmGuid, out pointGuid, out tagGuid, out alarmGuid);
		}

		public static void ParseAcknowledgedNodeID(string nodeId, out Guid pointGuid, out Guid tagGuid, out Guid alarmGuid)
		{
			var pointTagAlarmGuid = nodeId.Replace(AcknowledgedNodeIdPrefix, "");
			GetPointTagAlarmGuid(pointTagAlarmGuid, out pointGuid, out tagGuid, out alarmGuid);
		}

		public static void ParseAcknowledgeNodeID(string nodeId, out Guid pointGuid, out Guid tagGuid, out Guid alarmGuid)
		{
			var pointTagAlarmGuid = nodeId.Replace(AcknowledgeNodeIdPrefix, "");
			GetPointTagAlarmGuid(pointTagAlarmGuid, out pointGuid, out tagGuid, out alarmGuid);
		}

		public static void ParseAckInputsNodeID(string nodeId, out Guid pointGuid, out Guid tagGuid, out Guid alarmGuid)
		{
			var pointTagAlarmGuid = nodeId.Replace(AckInputsNodeIdPrefix, "");
			GetPointTagAlarmGuid(pointTagAlarmGuid, out pointGuid, out tagGuid, out alarmGuid);
		}

		public static void ParseAckOutputsNodeID(string nodeId, out Guid pointGuid, out Guid tagGuid, out Guid alarmGuid)
		{
			var pointTagAlarmGuid = nodeId.Replace(AckOutputsNodeIdPrefix, "");
			GetPointTagAlarmGuid(pointTagAlarmGuid, out pointGuid, out tagGuid, out alarmGuid);
		}

		public static void ParseDefinitionNodeID(string nodeId, out Guid tagGuid)
		{
			tagGuid = new Guid(nodeId.Replace(DefinitionNodeIdPrefix, ""));
		}
		#endregion Helper functions to parse NodeID
		#endregion NodeID Manipulations

		public static NodeId ConvertTypeToDataTypeId(Type dataType)
		{
			if (dataType == typeof(bool))
			{
				return DataTypeIds.Boolean;
			}
			else if (dataType == typeof(sbyte))
			{
				return DataTypeIds.Byte;
			}
			else if (dataType == typeof(short))
			{
				return DataTypeIds.Int16;
			}
			else if (dataType == typeof(int))
			{
				return DataTypeIds.Int32;
			}
			else if (dataType == typeof(long))
			{
				return DataTypeIds.Int64;
			}
			else if (dataType == typeof(float))
			{
				return DataTypeIds.Float;
			}
			else if (dataType == typeof(float))
			{
				return DataTypeIds.Float;
			}
			else if (dataType == typeof(double))
			{
				return DataTypeIds.Double;
			}
			else if (dataType == typeof(double))
			{
				return DataTypeIds.Double;
			}
			else if (dataType == typeof(byte))
			{
				return DataTypeIds.Byte;
			}
			else if (dataType == typeof(ushort))
			{
				return DataTypeIds.UInt16;
			}
			else if (dataType == typeof(uint))
			{
				return DataTypeIds.UInt32;
			}
			else if (dataType == typeof(ulong))
			{
				return DataTypeIds.UInt64;
			}
			else if (dataType == typeof(string))
			{
				return DataTypeIds.String;
			}
			else if (dataType.IsEnum)
			{
				return DataTypeIds.EnumValueType;
			}
			else if (dataType == typeof(DateTime))
			{
				return DataTypeIds.Date;
			}
			else if (dataType == typeof(DateTimeOffset))
			{
				return DataTypeIds.DateTime;
			}
			else if(dataType == typeof(TimeSpan))
			{
				return DataTypeIds.Int64;
			}
			else if (dataType == typeof(PointCommandStatusListReference))
			{
				return DataTypeIds.Int32;
			}
			else if (dataType == typeof(DeviceAlarmMapReference))
			{
				return DataTypeIds.UInt16;
			}
			else if (dataType == typeof(double?))
			{
				return DataTypeIds.Double;
			}

			throw new Exception("PointManager : ParseValue unsuported DataType " + dataType.ToString());

		}

		/// <summary>
		///	 Formats the value.
		/// </summary>
		/// <param name="dataType">Type of the data.</param>
		/// <param name="units">The units.</param>
		/// <param name="value">The value.</param>
		/// <param name="numberFormatInfo">The format.</param>
		/// <returns></returns>
		public static string FormatValue(
			Type dataType,
			EngineeringUnit units,
			NumberFormatInfo numberFormatInfo,
			object value)
		{
			if((value is double || value is float) && Double.IsNaN(Convert.ToDouble(value)))
			{
				return "NaN";
			}

			else if ((units == EngineeringUnit.FmlFtIn16Th || units == EngineeringUnit.FmlFtIn8Th) 
			&& (value is double || value is float))
			{
				var valueDouble = Convert.ToDouble(value);

				// Get Whole Feet to Integer
				var negative = (valueDouble < 0.00);
				if (negative)
				{
					valueDouble = -valueDouble;
				}

				var feet = (int)valueDouble;
				var fraction = valueDouble - feet;

				// Convert to Inches
				fraction *= 12.0000;
				var inch = (int)fraction;
				fraction -= inch;

				int factor = (units == EngineeringUnit.FmlFtIn16Th) ? 16 : 8;

				// Convert to Fraction
				fraction *= factor;
				var fract = (int)(fraction + 0.500);

				if (fract >= factor)
				{
					inch++;
					fract = 0;

					if (inch >= 12)
					{
						feet++;
						inch = 0;
					}
				}

				if (negative)
				{
					if (units == EngineeringUnit.FmlFtIn16Th)
					{
						return "-" + feet.ToString("D2") + "-" + inch.ToString("D2") + "-" + fract.ToString("D2");
					}

					return "-" + feet.ToString("D2") + "-" + inch.ToString("D2") + "-" + fract.ToString("D1");
				}

				if (units == EngineeringUnit.FmlFtIn16Th)
				{
					return feet.ToString("D2") + "-" + inch.ToString("D2") + "-" + fract.ToString("D2");
				}

				return feet.ToString("D2") + "-" + inch.ToString("D2") + "-" + fract.ToString("D1");
			}

			else if (value is bool)
			{
				return value.ToString();
			}
			else if (value is SByte)
			{
				return ((SByte)value).ToString("N", numberFormatInfo);
			}
			else if (value is Int16)
			{
				return ((Int16)value).ToString("N", numberFormatInfo);
			}
			else if (value is Int32)
			{
				return ((Int32)value).ToString("N", numberFormatInfo);
			}
			else if (value is Int64)
			{
				return ((Int64)value).ToString("N", numberFormatInfo);
			}
			else if (value is Single)
			{
				return ((float)value).ToString("N", numberFormatInfo);
			}
			else if (value is Double)
			{
				return ((double)value).ToString("N", numberFormatInfo);
			}
			else if (value is Byte)
			{
				return ((Byte)value).ToString("N", numberFormatInfo);
			}
			else if (value is UInt16)
			{
				return ((UInt16)value).ToString("N", numberFormatInfo);
			}
			else if (value is UInt32)
			{
				return ((UInt32)value).ToString("N", numberFormatInfo);
			}
			else if (value is UInt64)
			{
				return ((UInt64)value).ToString("N", numberFormatInfo);
			}
			else if (value is PointCommandStatusListReference)
			{
				return ((value as PointCommandStatusListReference).CurrentValue.HasValue) ? (value as PointCommandStatusListReference).CurrentValue.Value.ToString() : "";
			}
			else if (value is DeviceAlarmMapReference)
			{
				var damr = value as DeviceAlarmMapReference;
				if (damr.CurrentValue.HasValue)
				{
					return (((UInt32) damr.CurrentValue).ToString("N", numberFormatInfo));
				}
				else
				{
					return string.Empty;
				}
			}
			else if (value is TimeSpan)
			{
				return ((TimeSpan)value).ToString("c");
			}
			else if (value == null)
			{
				return string.Empty;
			}

			return value.ToString();
		}

		public static string FormatValueFullPrecision(
			Type dataType,
			EngineeringUnit units,
			NumberFormatInfo numberFormatInfo,
			object value)
		{
			if ((value is double || value is float) && Double.IsNaN(Convert.ToDouble(value)))
			{
				return "NaN";
			}

			else if ((units == EngineeringUnit.FmlFtIn16Th || units == EngineeringUnit.FmlFtIn8Th)
			&& (value is double || value is float))
			{
				var valueDouble = Convert.ToDouble(value);

				// Get Whole Feet to Integer
				var negative = (valueDouble < 0.00);
				if (negative)
				{
					valueDouble = -valueDouble;
				}

				var feet = (int)valueDouble;
				var fraction = valueDouble - feet;

				// Convert to Inches
				fraction *= 12.0000;
				var inch = (int)fraction;
				fraction -= inch;

				int factor = (units == EngineeringUnit.FmlFtIn16Th) ? 16 : 8;

				// Convert to Fraction
				fraction *= factor;
				var fract = (int)(fraction + 0.500);

				if (fract >= factor)
				{
					inch++;
					fract = 0;

					if (inch >= 12)
					{
						feet++;
						inch = 0;
					}
				}

				if (negative)
				{
					if (units == EngineeringUnit.FmlFtIn16Th)
					{
						return "-" + feet.ToString("D2") + "-" + inch.ToString("D2") + "-" + fract.ToString("D2");
					}

					return "-" + feet.ToString("D2") + "-" + inch.ToString("D2") + "-" + fract.ToString("D1");
				}

				if (units == EngineeringUnit.FmlFtIn16Th)
				{
					return feet.ToString("D2") + "-" + inch.ToString("D2") + "-" + fract.ToString("D2");
				}

				return feet.ToString("D2") + "-" + inch.ToString("D2") + "-" + fract.ToString("D1");
			}

			else if (value is bool)
			{
				return value.ToString();
			}
			else if (value is SByte)
			{
				return ((SByte)value).ToString("N", numberFormatInfo);
			}
			else if (value is Int16)
			{
				return ((Int16)value).ToString("N", numberFormatInfo);
			}
			else if (value is Int32)
			{
				return ((Int32)value).ToString("N", numberFormatInfo);
			}
			else if (value is Int64)
			{
				return ((Int64)value).ToString("N", numberFormatInfo);
			}
			else if (value is Single)
			{
				if ((float)value == 0.0)
				{
					return "0";
				}
				else
				{
					string dreturnValue = string.Empty;
					numberFormatInfo.NumberDecimalDigits = 9;
					if (((float)value).ToString().IndexOf(numberFormatInfo.NumberDecimalSeparator) >= 0)
						dreturnValue = ((float)value).ToString("N", numberFormatInfo).Trim('0');
					else
						dreturnValue = ((float)value).ToString("N", numberFormatInfo);

					// get rid of the trailing 0
					dreturnValue = dreturnValue.TrimEnd('0');
					if (dreturnValue.IndexOf(numberFormatInfo.NumberDecimalSeparator) == dreturnValue.Length - 1)
					{
						dreturnValue = dreturnValue.Remove(dreturnValue.Length - 1, 1);
					}

					return dreturnValue;
				}
			}
			else if (value is Double)
			{
				if ((double)value == 0.0)
				{
					return "0";
				}
				else
				{
					string dreturnValue = string.Empty;
					numberFormatInfo.NumberDecimalDigits = 9;
					if (((double)value).ToString().IndexOf(numberFormatInfo.NumberDecimalSeparator) >= 0)
						dreturnValue = ((double)value).ToString("N", numberFormatInfo).Trim('0');
					else
						dreturnValue = ((double)value).ToString("N", numberFormatInfo);

					// get rid of the trailing 0
					dreturnValue = dreturnValue.TrimEnd('0');
					if (dreturnValue.IndexOf(numberFormatInfo.NumberDecimalSeparator) == dreturnValue.Length - 1)
					{
						dreturnValue = dreturnValue.Remove(dreturnValue.Length - 1, 1);
					}

					return dreturnValue;
				}
				
			}
			else if (value is Byte)
			{
				return ((Byte)value).ToString("N", numberFormatInfo);
			}
			else if (value is UInt16)
			{
				return ((UInt16)value).ToString("N", numberFormatInfo);
			}
			else if (value is UInt32)
			{
				return ((UInt32)value).ToString("N", numberFormatInfo);
			}
			else if (value is UInt64)
			{
				return ((UInt64)value).ToString("N", numberFormatInfo);
			}
			else if (value is PointCommandStatusListReference)
			{
				return ((value as PointCommandStatusListReference).CurrentValue.HasValue) ? (value as PointCommandStatusListReference).CurrentValue.Value.ToString() : "";
			}
			else if (value is DeviceAlarmMapReference)
			{
				var damr = value as DeviceAlarmMapReference;
				if (damr.CurrentValue.HasValue)
				{
					return (((UInt32)damr.CurrentValue).ToString("N", numberFormatInfo));
				}
				else
				{
					return string.Empty;
				}
			}
			else if (value is TimeSpan)
			{
				return ((TimeSpan)value).ToString("c");
			}
			else if (value == null)
			{
				return string.Empty;
			}

			return value.ToString();
		}

		public static double RoundToFtIn16th (double value)
		{
			return (double)ParseValue(typeof(string), EngineeringUnit.FmlFtIn16Th, new NumberFormatInfo(), FormatValue(typeof(double), EngineeringUnit.FmlFtIn16Th, new NumberFormatInfo(), value));
		}

        public static double RoundToFtIn8th(double value)
        {
            return (double)ParseValue(typeof(string), EngineeringUnit.FmlFtIn8Th, new NumberFormatInfo(), FormatValue(typeof(double), EngineeringUnit.FmlFtIn8Th, new NumberFormatInfo(), value));
        }

        public static string GetCommandStatusKey(object value)
		{
			string result = "";
			if (value is PointCommandStatusListReference)
			{
				result = ((value as PointCommandStatusListReference).CurrentKey != null) ? (value as PointCommandStatusListReference).CurrentKey : "";
			}
			return result;
		}



		/// <summary>
		///	 Parses the value.
		/// </summary>
		/// <param name="dataType">Type of the data.</param>
		/// <param name="units">The units.</param>
		/// <param name="numberFormatInfo">The number format information.</param>
		/// <param name="valueString">The value string.</param>
		/// <returns></returns>
		/// <exception cref="System.Exception">
		///	 PointManager : ParseValue unsuported DataType  + dataType.ToString() +  value =  + valueString
		///	 or
		///	 PointManager : ParseValue conversion error  + dataType.ToString() +  value =  + valueString +	+ e.Message
		/// </exception>
		public static object ParseValue(
			Type dataType,
			EngineeringUnit units,
			NumberFormatInfo numberFormatInfo,
			string valueString)
		{
			try
			{
				if (string.IsNullOrEmpty(valueString))
				{
					return null;
				}

				if (units == EngineeringUnit.FmlFtIn16Th || units == EngineeringUnit.FmlFtIn8Th)
				{
					double value;
					bool negative = false;

					if (valueString.Length == 0) 
					{
						throw new Exception("Invalid");
					}

					// Trim Leading Spaces if Any
					valueString = valueString.Trim();

					if (valueString[0] == '-')
					{
						negative = true;
						valueString = valueString.Remove(0, 1);
					}

					int iDelimiter = valueString.IndexOf("-", StringComparison.Ordinal);

					if (iDelimiter == -1)
					{
						value = Convert.ToDouble(valueString);
					}
					else
					{
						value = Convert.ToDouble(valueString.Substring(0, iDelimiter));
						valueString = valueString.Substring(iDelimiter + 1);

						iDelimiter = valueString.IndexOf("-", StringComparison.Ordinal);

						if (iDelimiter == -1)
						{
							value += Convert.ToDouble(valueString) / 12;
						}
						else
						{
							value += Convert.ToDouble(valueString.Substring(0, iDelimiter)) / 12;
							valueString = valueString.Substring(iDelimiter + 1);
							int iFactor = (units == EngineeringUnit.FmlFtIn16Th) ? 192 : 96;
							value += Convert.ToDouble(valueString) / iFactor;
						}
					}

					if (negative)
					{
						value = -value;
					}

					if (dataType.ToString() == "System.Single")
					{
						return System.Convert.ToSingle(value);
					}
					else
					{
						return value;
					}
				}

				if (dataType.IsPrimitive)
				{
					switch (dataType.ToString())
					{
						case "System.Int16":
							return Int16.Parse(valueString, NumberStyles.Any, numberFormatInfo);
						case "System.Int32":
							return Int32.Parse(valueString, NumberStyles.Any, numberFormatInfo);
						case "System.Int64":
							return Int64.Parse(valueString, NumberStyles.Any, numberFormatInfo);
						case "System.UInt16":
							return UInt16.Parse(valueString, NumberStyles.Any, numberFormatInfo);
						case "System.UInt32":
							return UInt32.Parse(valueString, NumberStyles.Any, numberFormatInfo);
						case "System.UInt64":
							return UInt64.Parse(valueString, NumberStyles.Any, numberFormatInfo);
						default:
							return Convert.ChangeType(valueString, dataType, numberFormatInfo);
					}
				}
				else
				{
					if (dataType == typeof(string))
					{
						return valueString;
					}
					else
					{
						throw new Exception(
							"PointManager : ParseValue unsuported DataType " + dataType.ToString() + " value = " + valueString);
					}
				}
			}
			catch (Exception e)
			{
				throw new Exception(
					"PointManager : ParseValue conversion error " + dataType.ToString() + " value = " + valueString + " " + e.Message);
			}
		}

		/// <summary>
		///	Validates given point tag value against its type and verifies whether the value is
		///	with in the allowed range for CodedVariables
		/// </summary>
		/// <param name="dataType">point tag data type string</param>
		/// <param name="value">point tag value</param>
		/// <param name="valStatusCode">point tag value status code</param>
		/// <param name="isDeviceAlarmMapReference">is point tag value of type DeviceAlarmMapReference?</param>
		/// <param name="deviceAlarmMapGuid">if point tag value is of DeviceAlarmMapReference type, its Guid</param>
		/// <returns></returns>
		public static void ValidatePointTagValueByItsType(string dataType, ref object value, 
			ref StatusCode valStatusCode, bool isDeviceAlarmMapReference = false, Guid deviceAlarmMapGuid = default)
		{
			try
			{
				switch (dataType)
				{
					case "System.Boolean":
						if (value != null)
						{
							value = Convert.ToBoolean(value);
						}
						break;

					case "System.Int16":
						if (value != null)
						{
							value = Convert.ToInt16(value);
						}
						break;

					case "System.UInt16":
						if (value != null)
						{
							value = Convert.ToUInt16(value);
						}
						break;

					case "System.Int32":
						if (value != null)
						{
							value = Convert.ToInt32(value);
						}
						break;

					case "System.UInt32":
						if (value != null)
						{
							value = Convert.ToUInt32(value);
						}
						break;

					case "System.Single":
						if (value != null)
						{
							value = Convert.ToSingle(value);
						}
						break;

					case "System.Double":
						if (value != null)
						{
							value = Convert.ToDouble(value);
						}
						break;

					case "System.String":
						if (value != null)
						{
							value = Convert.ToString(value);
						}
						break;

					case "System.DateTimeOffset":
						if (value != null)
						{
							value = new DateTimeOffset(Convert.ToDateTime(value));
						}
						break;

					case "System.DateTime":
						if (value != null)
						{
							value = Convert.ToDateTime(value);
						}
						break;

					case "System.TimeSpan":
						if (value != null)
						{
							value = new TimeSpan(Convert.ToInt64(value));
						}
						break;

					case "FMBusinessObjects.DataObjects.DeviceAlarmMapReference":
							if (isDeviceAlarmMapReference)
							{
								try
								{
									value = Convert.ToUInt32(value);
								}
								catch (Exception)
								{
									value = null;
								}

								value = new DeviceAlarmMapReference()
								{
									DeviceAlarmMapGuid = deviceAlarmMapGuid,
									CurrentValue = (UInt32?)value
								};
							}
							break;

					case "FMBusinessObjects.DataObjects.CodedVariables.TankCommands":
                        if (value != null && Enum.IsDefined(typeof(FMBusinessObjects.DataObjects.CodedVariables.TankCommands), value)) 
						{ 
                            if (Enum.TryParse(value.ToString(), out FMBusinessObjects.DataObjects.CodedVariables.TankCommands outValue))
							{
						        value = outValue;
					        }
				            else
			                {
		                        value = (FMBusinessObjects.DataObjects.CodedVariables.TankCommands)Convert.ToInt32(value);
	                        }
						}
						else
						{
							value = null;
							valStatusCode = new StatusCode(StatusCodes.Bad);
						}

						break;

					case "FMBusinessObjects.DataObjects.CodedVariables.TankStatuses":
						if (value != null && Enum.IsDefined(typeof(FMBusinessObjects.DataObjects.CodedVariables.TankStatuses), value))
						{
                            if (Enum.TryParse(value.ToString(), out FMBusinessObjects.DataObjects.CodedVariables.TankStatuses outValue))
                            {
                                value = outValue;
                            }
                            else
                            {
                                value = (FMBusinessObjects.DataObjects.CodedVariables.TankStatuses)Convert.ToInt32(value);
                            }
                        }
                        else
						{
							value = null;
							valStatusCode = new StatusCode(StatusCodes.Bad);
						}
						break;

					case "FMBusinessObjects.DataObjects.CodedVariables.TransferModes":
						if (value != null && Enum.IsDefined(typeof(FMBusinessObjects.DataObjects.CodedVariables.TransferModes), value))
						{
                            if (Enum.TryParse(value.ToString(), out FMBusinessObjects.DataObjects.CodedVariables.TransferModes outValue))
                            {
                                value = outValue;
                            }
                            else
                            {
                                value = (FMBusinessObjects.DataObjects.CodedVariables.TransferModes)Convert.ToInt32(value);
                            }
                        }
                        else
						{
							value = null;
							valStatusCode = new StatusCode(StatusCodes.Bad);
						}
						break;

					case "FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode":
						if (value != null && Enum.IsDefined(typeof(FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode), value))
						{
                            if (Enum.TryParse(value.ToString(), out FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode outValue))
                            {
                                value = outValue;
                            }
                            else
                            {
                                value = (FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode)Convert.ToInt32(value);
                            }
                        }
                        else
						{
							value = null;
							valStatusCode = new StatusCode(StatusCodes.Bad);
						}
						break;

					case "FMBusinessObjects.DataObjects.CodedVariables.VolumeTransferMode":
						if (value != null && Enum.IsDefined(typeof(FMBusinessObjects.DataObjects.CodedVariables.VolumeTransferMode), value))
						{
                            if (Enum.TryParse(value.ToString(), out FMBusinessObjects.DataObjects.CodedVariables.VolumeTransferMode outValue))
                            {
                                value = outValue;
                            }
                            else
                            {
                                value = (FMBusinessObjects.DataObjects.CodedVariables.VolumeTransferMode)Convert.ToInt32(value);
                            }
                        }
                        else
						{
							value = null;
							valStatusCode = new StatusCode(StatusCodes.Bad);
						}
						break;

					case "FMBusinessObjects.DataObjects.CodedVariables.TransferStatuses":
						if (value != null && Enum.IsDefined(typeof(FMBusinessObjects.DataObjects.CodedVariables.TransferStatuses), value))
						{
                            if (Enum.TryParse(value.ToString(), out FMBusinessObjects.DataObjects.CodedVariables.TransferStatuses outValue))
                            {
                                value = outValue;
                            }
                            else
                            {
                                value = (FMBusinessObjects.DataObjects.CodedVariables.TransferStatuses)Convert.ToInt32(value);
                            }
                        }
                        else
						{
							value = null;
							valStatusCode = new StatusCode(StatusCodes.Bad);
						}
						break;

					case "FMBusinessObjects.DataObjects.CodedVariables.TankOperationalMode":
						if (value != null && Enum.IsDefined(typeof(FMBusinessObjects.DataObjects.CodedVariables.TankOperationalMode), value))
						{
                            if (Enum.TryParse(value.ToString(), out FMBusinessObjects.DataObjects.CodedVariables.TankOperationalMode outValue))
                            {
                                value = outValue;
                            }
                            else
                            {
                                value = (FMBusinessObjects.DataObjects.CodedVariables.TankOperationalMode)Convert.ToInt32(value);
                            }
                        }
                        else
						{
							value = null;
							valStatusCode = new StatusCode(StatusCodes.Bad);
						}
						break;

                    case "FMBusinessObjects.DataObjects.CodedVariables.MovementCommand":
                        if (value != null && Enum.IsDefined(typeof(FMBusinessObjects.DataObjects.CodedVariables.MovementCommand), value))
                        {
                            if (Enum.TryParse(value.ToString(), out FMBusinessObjects.DataObjects.CodedVariables.MovementCommand outValue))
                            {
                                value = outValue;
                            }
                            else
                            {
                                value = (FMBusinessObjects.DataObjects.CodedVariables.MovementCommand)Convert.ToInt32(value);
                            }
                        }
                        else
						{
							value = null;
							valStatusCode = new StatusCode(StatusCodes.Bad);
						}
						break;

					case "FMBusinessObjects.DataObjects.CodedVariables.MovementStatus":
						if (value != null && Enum.IsDefined(typeof(FMBusinessObjects.DataObjects.CodedVariables.MovementStatus), value))
						{
                            if (Enum.TryParse(value.ToString(), out FMBusinessObjects.DataObjects.CodedVariables.MovementStatus outValue))
                            {
                                value = outValue;
                            }
                            else
                            {
                                value = (FMBusinessObjects.DataObjects.CodedVariables.MovementStatus)Convert.ToInt32(value);
                            }
                        }
                        else
						{
							value = null;
							valStatusCode = new StatusCode(StatusCodes.Bad);
						}
						break;

					case "FMBusinessObjects.DataObjects.CodedVariables.StrapTableSelect":
						if (value != null && Enum.IsDefined(typeof(FMBusinessObjects.DataObjects.CodedVariables.StrapTableSelect), value))
						{
                            if (Enum.TryParse(value.ToString(), out FMBusinessObjects.DataObjects.CodedVariables.StrapTableSelect outValue))
                            {
                                value = outValue;
                            }
                            else
                            {
                                value = (FMBusinessObjects.DataObjects.CodedVariables.StrapTableSelect)Convert.ToInt32(value);
                            }
                        }
                        else
						{
							value = null;
							valStatusCode = new StatusCode(StatusCodes.Bad);
						}
						break;
					case "FMBusinessObjects.DataObjects.CodedVariables.Reset":
						if (value != null && Enum.IsDefined(typeof(FMBusinessObjects.DataObjects.CodedVariables.Reset), value))
						{
                            if (Enum.TryParse(value.ToString(), out FMBusinessObjects.DataObjects.CodedVariables.Reset outValue))
                            {
                                value = outValue;
                            }
                            else
                            {
                                value = (FMBusinessObjects.DataObjects.CodedVariables.Reset)Convert.ToInt32(value);
                            }
                        }
                        else
						{
							value = null;
							valStatusCode = new StatusCode(StatusCodes.Bad);
						}
						break;

					case "FMBusinessObjects.DataObjects.CodedVariables.RoofTypeEnum":
						if (value != null && Enum.IsDefined(typeof(FMBusinessObjects.DataObjects.CodedVariables.RoofTypeEnum), value))
						{
                            if (Enum.TryParse(value.ToString(), out FMBusinessObjects.DataObjects.CodedVariables.RoofTypeEnum outValue))
                            {
                                value = outValue;
                            }
                            else
                            {
                                value = (FMBusinessObjects.DataObjects.CodedVariables.RoofTypeEnum)Convert.ToInt32(value);
                            }
                        }
                        else
						{
							value = null;
							valStatusCode = new StatusCode(StatusCodes.Bad);
						}
						break;

					default:
						value = null;
						valStatusCode = new StatusCode(StatusCodes.BadDataTypeIdUnknown);
						break;
				}
			}
			catch (Exception)
			{
				value = null;
				valStatusCode = new StatusCode(StatusCodes.BadTypeMismatch);
			}

		}
	}
}