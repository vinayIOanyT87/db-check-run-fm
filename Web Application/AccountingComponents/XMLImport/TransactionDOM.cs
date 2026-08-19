namespace XMLImport
{
    using System;
    using System.Xml.XPath;
    //using EngineeringUnitsLibrary;
    using FMBusinessObjects.DataObjects;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    /// <summary>
	/// Reads values from XML
	/// </summary>
	public class TransactionDOM
	{
		/// <summary>
		/// Contains any errors we encounter when validating a transaction. For example, the manager may be invalid
		/// </summary>
		protected TransactionValidationResult transactionValidationResult;

		protected string GetStringValue(string path, bool required, XPathNavigator xpathNavigator)
		{
			XPathNavigator navigator = xpathNavigator.SelectSingleNode(path + "/child::text()");

			if (navigator != null)
			{
                //Underscores removed for compatibility with 7.x data.  This is necessary for Aviation imports.  If this 
                //needs to be changed, please work with someone in the aviation department
				return navigator.Value.Trim().Replace("_",string.Empty);
			}

			if (required)
			{
				this.transactionValidationResult.ErrorList.Add("Required value not found: " + path);
			}

			return null;
		}

		protected string GetTextStringValue(string path, bool required, XPathNavigator xpathNavigator)
		{
			XPathNavigator navigator = xpathNavigator.SelectSingleNode(path + "/text()");

			if (navigator != null)
			{
				return navigator.Value;
			}

			if (required)
			{
				this.transactionValidationResult.ErrorList.Add("Required value not found: " + path);
			}

			return null;
		}

		protected Guid? GetNullableGuid(string path, bool required, XPathNavigator xpathNavigator)
		{
			string guidAsStr = GetStringValue( path, required, xpathNavigator);
			if( null == guidAsStr)
			{
				return null;
			}

			return new Guid(guidAsStr);
		}

		protected bool GetBoolValue(string path, bool required, XPathNavigator xpathNavigator)
		{
			return this.GetNullableBool(path, required, xpathNavigator).GetValueOrDefault();
		}

		protected bool? GetNullableBool(string path, bool required, XPathNavigator xpathNavigator)
		{
			string s = this.GetStringValue(path, required, xpathNavigator);

			if (string.IsNullOrEmpty(s))
			{
				return null;
			}

			bool boolValue;

			if (bool.TryParse(s, out boolValue))
			{
				return boolValue;
			}

			// There was a value, but it wasn't an bool
			string msg = this.GetFieldNameFromPath(path) + " = " + s + " : Invalid bool";
			this.transactionValidationResult.ErrorList.Add(msg);
			return null;
		}


		protected DateTimeOffset GetDateValue(string path, bool required, XPathNavigator xpathNavigator)
		{
			return this.GetDateTimeValue(path, required, xpathNavigator).Date;
		}

		protected DateTimeOffset GetDateTimeValue(string path, bool required, XPathNavigator xpathNavigator)
		{
			return this.GetNullableDateTime(path, required, xpathNavigator).GetValueOrDefault();
		}

		protected DateTimeOffset? GetNullableDateTime(string path, bool required, XPathNavigator xpathNavigator)
		{
			TimeSpan timeZoneHoursOffset = new TimeSpan(0);
			XPathNavigator nodePath = xpathNavigator.SelectSingleNode(path);
			if (nodePath != null)
			{
				string timeZoneOffsetAsStr = nodePath.GetAttribute("TimeZoneOffset", "");
				if (!String.IsNullOrEmpty(timeZoneOffsetAsStr))
				{
					TimeSpan.TryParse(timeZoneOffsetAsStr, out timeZoneHoursOffset);
				}
			}

			return GetNullableDateTime(path, required, xpathNavigator, timeZoneHoursOffset);
		}  

		protected DateTimeOffset? GetNullableDateTime(string path, bool required, XPathNavigator xpathNavigator, TimeSpan timeZoneHoursOffset)
		{
			string s = this.GetStringValue(path, required, xpathNavigator);

			// If there was not a value, return null.
			// If the value was required an error will be logged by GetStringValue
			if (string.IsNullOrEmpty(s))
			{
				return null;
			}

            bool isUTC = false;
			if (s.EndsWith("Z"))
			{
				s = string.Format("{0} " + (timeZoneHoursOffset.Hours >= 0 ? "+" : string.Empty) + "{1:D2}:{2:D2}", s.Left(s.Length - 1), timeZoneHoursOffset.Hours, timeZoneHoursOffset.Minutes);
                isUTC = true;
			}

			DateTimeOffset dateTimeOffset;

			if (DateTimeOffset.TryParse(s, out dateTimeOffset))
			{
				return isUTC ? dateTimeOffset + timeZoneHoursOffset : dateTimeOffset;
			}

			// There was a value, but the date is invalid
			string msg = this.GetFieldNameFromPath(path) + " = " + s + " : " + " Invalid DateTimeOffset";
			this.transactionValidationResult.ErrorList.Add(msg);

			return null;
		}

		protected double GetDouble(string path, bool required, XPathNavigator xpathNavigator)
		{
			return this.GetNullableDouble(path, required, xpathNavigator).GetValueOrDefault();
		}

		protected double? GetNullableDouble(string path, bool required, XPathNavigator xpathNavigator, double? defaultValue = null)
		{
			string s = this.GetStringValue(path, required, xpathNavigator);

			// If there was not a value, return null.
			// If the value was required an error will be logged by GetStringValue
			if (string.IsNullOrEmpty(s))
			{
				return defaultValue;
			}

			double doubleValue;

			if (double.TryParse(s, out doubleValue))
			{
				return doubleValue;
			}

			// There was a value, but it wasn't a double
			string msg = this.GetFieldNameFromPath(path) + " = " + s + " : Invalid double";
			this.transactionValidationResult.ErrorList.Add(msg);
			return null;
		}

		protected double GetDoubleSIValue(string valuePath, string unitPath, bool required, XPathNavigator xpathNavigator)
		{
			return this.GetNullableDoubleSIValue(valuePath, unitPath, required, xpathNavigator).GetValueOrDefault();
		}

		protected double? GetNullableDoubleSIValue(string path, string unitPath, bool required, XPathNavigator xpathNavigator, double? defaultValue = null)
		{
			EngineeringUnit units = this.GetEnumValue(unitPath, default(EngineeringUnit), required, xpathNavigator);
			return this.GetNullableDoubleSIValue(path, units, required, xpathNavigator, defaultValue);
		}

		protected double? GetNullableDoubleSIValue(string path, EngineeringUnit units, bool required, XPathNavigator xpathNavigator, double? defaultValue = null)
		{
			double? v = this.GetNullableDouble(path, required, xpathNavigator, defaultValue);

			if (!v.HasValue)
			{
				return defaultValue;
			}

			SIDouble dataValue = new SIDouble(units, null, 0) { Value = v.Value };
			return dataValue.SIValue;
		}

		protected int GetIntValue(string path, bool required, XPathNavigator xpathNavigator)
		{
			return this.GetNullableInt(path, required, xpathNavigator).GetValueOrDefault();
		}

		protected int? GetNullableInt(string path, bool required, XPathNavigator xpathNavigator)
		{
			string s = this.GetStringValue(path, required, xpathNavigator);

			if (string.IsNullOrEmpty(s))
			{
				return null;
			}

			int intValue;

			if (int.TryParse(s, out intValue))
			{
				return intValue;
			}

			// There was a value, but it wasn't an int
			string msg = this.GetFieldNameFromPath(path) + " = " + s + " : Invalid integer";
			this.transactionValidationResult.ErrorList.Add(msg);
			return null;			
		}

		protected TEnum GetEnumValue<TEnum>(string path, TEnum defaultValue, bool isRequired, XPathNavigator xpathNavigator) where TEnum : struct 
		{
			string s = this.GetStringValue(path, isRequired, xpathNavigator);

			TEnum result;

            //Ignore case on comaprison
            return Enum.TryParse(s, true, out result) ? result : defaultValue;
        }

		private string GetFieldNameFromPath(string path)
		{
			int index = path.LastIndexOf('/');
			return index >= 0 ? path.Remove(0, index + 1) : path;
		}
	}
}
