using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects
{
	using System.ComponentModel;
	using System.Globalization;
	using System.Reflection;
    using Varec.CommonComponents.EngineeringUnitsLibrary;

    /// <summary>
    /// Commonly used extension methods to existing classes
    /// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Return right portion of string
		/// </summary>
		/// <param name="value"></param>
		/// <param name="maxLen">the maximum number of chars to return (will be less is string is shorter)</param>
		/// <returns></returns>
		public static string Right(this string value, int maxLen)
		{
			return value.Substring(value.Length - maxLen);
		}


		/// <summary>
		/// Return left portion of string
		/// </summary>
		/// <param name="value"></param>
		/// <param name="maxLen">the maximum number of chars to return (will be less is string is shorter)</param>
		/// <returns></returns>
		public static string Left(this string value, int maxLen)
		{
			return value.Substring(0, Math.Min(maxLen, value.Length));
		}

		/// <summary>
		/// Zero pad an integer
		/// </summary>
		/// <param name="val">input value</param>
		/// <param name="length">length to return</param>
		/// <returns>val as a string padded to the left</returns>
		public static string PadLeft0(this int val, int length)
		{
			return val.ToString(CultureInfo.InvariantCulture).PadLeft(length, '0');
		}

		/// <summary>
		/// Create a string that is composed of "value" repeatedly.
		/// </summary>
		/// <param name="originalValue">left most part of returned value, usually empty string</param>
		/// <param name="pattern">pattern to repeat</param>
		/// <param name="maxLen">The returned value will be this length, truncating from the right if needed</param>
		/// <returns></returns>
		public static string Repeat(this string originalValue, string pattern, int maxLen)
		{
			StringBuilder build = new StringBuilder(maxLen + originalValue.Length + pattern.Length);
			build.Append(originalValue);
			while (build.Length < maxLen)
			{
				build.Append(pattern);
			}
			return build.ToString().Left(maxLen);
		}

		public static string Repeat(this string originalValue, int maxLen)
		{
			StringBuilder build = new StringBuilder((maxLen + 1) * originalValue.Length);
			while (build.Length < maxLen)
			{
				build.Append(originalValue);
			}
			return build.ToString().Left(maxLen);
		}

		public static string GetDescription(this Enum value)
		{
			Type type = value.GetType();
			string name = Enum.GetName(type, value);
			if (name != null)
			{
				FieldInfo field = type.GetField(name);
				if (field != null)
				{
					DescriptionAttribute attr =
							 Attribute.GetCustomAttribute(field,
								typeof(DescriptionAttribute)) as DescriptionAttribute;
					if (attr != null)
					{
						return attr.Description;
					}
				}
			}
			return null;
		}

        public static string GetAbbreviation(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    EngineeringUnitAbbreviationAttribute attr =
                           Attribute.GetCustomAttribute(field,
                             typeof(EngineeringUnitAbbreviationAttribute)) as EngineeringUnitAbbreviationAttribute;
                    if (attr != null)
                    {
                        return attr.Abbreviation;
                    }
                }
            }
            return null;
        }

    }
}
