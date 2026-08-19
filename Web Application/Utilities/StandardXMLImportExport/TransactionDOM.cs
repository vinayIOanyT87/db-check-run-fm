using System;

using FM7Accounting; //For VDouble, VInteger, VDateTime

namespace StandardXMLImportExport
{
	/// <summary>
	/// Summary description for TransactionDOM.
	/// </summary>
	public class TransactionDOM
	{
		#region Attributes
		protected System.Xml.XmlDocument doc;
		#endregion Attributes
		

		public TransactionDOM()
		{
		
		}

		#region Public Methods
		protected string GetStringValue(string path) { return GetStringValue(path, true); }
		protected string GetStringValue(string path, bool required)
		{
			path = "/" + doc.DocumentElement.LocalName + "/" + path + "/child::text()";
			System.Xml.XmlNode node = doc.SelectSingleNode(path);
			if(node != null)
			{
				return node.Value;
			}
			if(required)
			{
				throw new Exception("Required value not found: " + path);
			}
			return null;
		}

		protected char GetCharValue(string path) { return GetCharValue(path, true); }
		protected char GetCharValue(string path, bool required)
		{
			string s = GetStringValue(path, required);
			if (s == null || s == "")
			{
				if(required)
				{
					throw new Exception("Required value not found: " + path);
				}
				return '\0';
			}
			return s[0];
		}

		protected bool GetBoolValue(string path) { return GetBoolValue(path, true); }
		protected bool GetBoolValue(string path, bool required)
		{
			string s = GetStringValue(path, required);
			if(s != null)
			{
				return bool.Parse(s);
			}
			return false;
		}

		protected System.DateTime GetDateValue(string path) { return GetDateValue(path, true); }
		protected System.DateTime GetDateValue(string path, bool required)
		{
			string s = GetStringValue(path, required);
	
			s = s.Substring(0, 10);
			return System.DateTime.Parse(s, System.Globalization.DateTimeFormatInfo.CurrentInfo);
		}

		protected System.DateTime GetDateTimeValue(string path) { return GetDateTimeValue(path, true); }
		protected System.DateTime GetDateTimeValue(string path, bool required)
		{
			string s = GetStringValue(path, required);

			return System.DateTime.Parse(s, System.Globalization.DateTimeFormatInfo.CurrentInfo);
		}
		protected VDateTime GetOptionalDateTime(string path)
		{
			string s = GetStringValue(path, false);
			if(s == null || s == "")
			{
				return null;
			}
			return new VDateTime(DateTime.Parse(s));
		}
		
		protected double GetDoubleValue(string path) { return GetDoubleValue(path, true); }
		protected double GetDoubleValue(string path, bool required)
		{
			string s = GetStringValue(path, required);
			if(s == null)
			{
				return double.NaN;
			}
			return System.Double.Parse(s);
		}

		protected long GetLongValue(string path) { return GetLongValue(path, true); }
		protected long GetLongValue(string path, bool required)
		{
			string s = GetStringValue(path, required);
			return long.Parse(s);
		}

		protected int GetIntValue(string path) { return GetIntValue(path, true); }
		protected int GetIntValue(string path, bool required)
		{
			string s = GetStringValue(path, required);

			return int.Parse(s);
		}
		protected VInteger GetOptionalInt(string path)
		{
			string s = GetStringValue(path, false);
			if(s == null || s == "")
			{
				return null;
			}
			return new VInteger(int.Parse(s));
		}

		protected System.Double GetDouble(string valuePath, string unitPath, Interop.ConvertEngUnitsU.CU_UNIT_TYPE unitType)
		{ return GetDouble(valuePath, unitPath, unitType, true); }
		protected System.Double GetDouble(string valuePath, string unitPath, Interop.ConvertEngUnitsU.CU_UNIT_TYPE unitType,
			bool required)
		{
			double doubleValue = GetDoubleValue(valuePath, required);
			if(double.IsNaN(doubleValue))
			{
				return doubleValue;
			}
			Interop.ConvertEngUnitsU.CU_UNIT units =
				GetUnitsValue(unitType, unitPath);
			ConsolidatedDataObjects.SIDouble dataValue = new ConsolidatedDataObjects.SIDouble(units, null);
			dataValue.Value = doubleValue;
			return dataValue.SIValue;
		}
		protected VDouble GetOptionalDoubleValue(string path)
		{
			string s = GetStringValue(path, false);
			return VDouble.Parse(s);
		}
		protected VDouble GetOptionalDoubleValue(string path, string unitPath, Interop.ConvertEngUnitsU.CU_UNIT_TYPE unitType)
		{
			VDouble v = GetOptionalDoubleValue(path);
			if((v == null) || (v.IsNull == true))
			{
				return v;
			}
			Interop.ConvertEngUnitsU.CU_UNIT units =
				GetUnitsValue(unitType, unitPath);
			ConsolidatedDataObjects.SIDouble dataValue = new ConsolidatedDataObjects.SIDouble(units, null);
			dataValue.Value = v.Value;
			return new VDouble(dataValue.SIValue);

		}

		protected double GetVolume(string volumePath, string unitPath) { return GetVolume(volumePath, unitPath, true); }
		protected double GetVolume(string volumePath, string unitPath, bool required)
		{
			return GetDouble(volumePath, unitPath, Interop.ConvertEngUnitsU.CU_UNIT_TYPE.FMU_VOLUME, required);
		}
		protected VDouble GetOptionalVolume(string volumePath, string unitPath)
		{
			return GetOptionalDoubleValue(volumePath, unitPath, Interop.ConvertEngUnitsU.CU_UNIT_TYPE.FMU_VOLUME);
		}

		protected double GetTemperature(string temperaturePath, string unitPath)
		{ return GetTemperature(temperaturePath, unitPath, true); }
		protected double GetTemperature(string temperaturePath, string unitPath, bool required)
		{
			return GetDouble(temperaturePath, unitPath, Interop.ConvertEngUnitsU.CU_UNIT_TYPE.FMU_TEMP, required);
		}
		protected VDouble GetOptionalTemperature(string temperaturePath, string unitPath)
		{
			return GetOptionalDoubleValue(temperaturePath, unitPath, Interop.ConvertEngUnitsU.CU_UNIT_TYPE.FMU_TEMP);
		}

		protected double GetDensity(string densityPath, string unitPath) { return GetDensity(densityPath, unitPath, true); }
		protected double GetDensity(string densityPath, string unitPath, bool required)
		{
			return GetDouble(densityPath, unitPath, Interop.ConvertEngUnitsU.CU_UNIT_TYPE.FMU_DENSITY, required);
		}
		protected VDouble GetOptionalDensity(string densityPath, string unitPath)
		{ return GetOptionalDoubleValue(densityPath, unitPath, Interop.ConvertEngUnitsU.CU_UNIT_TYPE.FMU_DENSITY); }
		//		protected double GetGravity(string gravityPath, string unitPath) { return GetGravity(gravityPath, unitPath, true); }
		//		protected double GetGravity(string gravityPath, string unitPath, bool required)
		//		{
		//			return GetDouble(gravityPath, unitPath, Interop.ConvertEngUnitsU.CU_UNIT_TYPE.FMU_DENSITY, required);
		//		}

		protected double GetDuration(string durationPath, string unitPath)
		{ return GetDuration(durationPath, unitPath, true); }
		protected double GetDuration(string durationPath, string unitPath, bool required)
		{
			return GetDouble(durationPath, unitPath, Interop.ConvertEngUnitsU.CU_UNIT_TYPE.FMU_TIME, required);
		}
		protected VDouble GetOptionalDuration(string durationPath, string unitPath)
		{
			return GetOptionalDoubleValue(durationPath, unitPath, Interop.ConvertEngUnitsU.CU_UNIT_TYPE.FMU_TIME);
		}

		protected Interop.ConvertEngUnitsU.CU_UNIT GetUnitsValue(Interop.ConvertEngUnitsU.CU_UNIT_TYPE unitType, string path)
		{
			Interop.ConvertEngUnitsU.CU_UNIT unit;
			string s = GetStringValue(path);
			unit = (Interop.ConvertEngUnitsU.CU_UNIT) System.Enum.Parse(typeof(Interop.ConvertEngUnitsU.CU_UNIT), s);
			return unit;
		}

		protected double ConvertDouble(double temp, Interop.ConvertEngUnitsU.CU_UNIT units)
		{
			ConsolidatedDataObjects.SIDouble tempSI =
				new ConsolidatedDataObjects.SIDouble(units, null);
			tempSI.Value = temp;
			return tempSI.SIValue;
		}
		#endregion Public Methods
	}
}
