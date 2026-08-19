// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeHelper.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Provides helper methods that perform additional type checking in order to safely validate an expression's type 
// or value.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.UtilityObjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using FMCore;

    public static class TypeHelper
    {
        public static bool IsGuid(object pExpression)
        {
            if (pExpression == null || pExpression is DateTime)
                return (false);

            if (IsNumeric(pExpression))
                return (false);

            if (pExpression is Guid)
                return (true);

            Guid result;
            string sValue = pExpression.ToString();

            if (Guid.TryParse(sValue, out result))
                return (true);

            return (false);
        }
        public static Guid? ToGuid(object pExpression)
        {
            if (IsGuid(pExpression))
            {
                if (pExpression is Guid)
                    return ((Guid)pExpression);
                else
                {
                    Guid result;
                    string sValue = pExpression.ToString();

                    if (Guid.TryParse(sValue, out result))
                        return (result);
                }
            }

            return (null);
        }
        public static bool IsNumeric(object pExpression)
        {
            if (pExpression == null || pExpression is DateTime)
                return (false);

            if (pExpression is int || pExpression is short || pExpression is long ||
                pExpression is uint || pExpression is ushort || pExpression is ulong ||
                pExpression is decimal ||
                pExpression is float ||
                pExpression is bool)
                return (true);

            double result = 0.0;
            string sValue = pExpression.ToString();

            if (Double.TryParse(sValue, out result))
                return (true);

            return (false);
        }

        public static bool IsTrue(object pExpression)
        {
            if (pExpression == null || pExpression is DateTime)
                return (false);

            if (pExpression is int || pExpression is short || pExpression is long ||
                pExpression is uint || pExpression is ushort || pExpression is ulong ||
                pExpression is string)
            {
                string sValue = pExpression.ToString();

                if (TypeHelper.IsNumeric(pExpression))
                {
                    if (Convert.ToInt64(sValue).Equals(1))
                        return (true);
                }
                else
                {
                    if (sValue.ToLower().Equals("true") || sValue.ToLower().Equals("yes") || sValue.Equals(bool.TrueString))
                        return (true);
                }
            }
            else if (pExpression is bool)
            {
                return ((bool)pExpression);
            }

            return (false);
        }


        /// <summary>
        /// Converts the incoming string to a DateTme type.
        /// </summary>
        /// <param name="val">The string value to convert into a DateTime data type.</param>
        /// <returns>DateTime.</returns>
        /// <remarks><see cref="DateTime.Now"/> will be returned if <see cref="val"/> is null, empty or contains only spaces.</remarks>
        public static DateTime ConvertDateTimeNoNull(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return DateTime.Now;
            }

            string tempStr = val.RemoveSpaces();

            if (string.IsNullOrEmpty(tempStr))
            {
                return DateTime.Now;
            }

            return DateTime.Parse(val);
        }

        /// <summary>
        /// Converts the incoming string to a DateTmeOffset instance.
        /// </summary>
        /// <param name="val">The string value to convert into a DateTimeOffset value.</param>
        /// <returns>DateTimeOffset.</returns>
        /// <remarks><see cref="DateTimeOffset.Now"/> will be returned if <see cref="val"/> is NULL, empty or contains only spaces.</remarks>
        public static DateTimeOffset ConvertDateTimeOffsetNoNull(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return DateTimeOffset.Now;
            }

            string tempStr = val.RemoveSpaces();

            if (string.IsNullOrEmpty(tempStr))
            {
                return DateTimeOffset.Now;
            }

            return DateTimeOffset.Parse(val);
        }

        /// <summary>
        /// Attempts to convert the incoming string to a DateTmeOffset instance.
        /// </summary>
        /// <param name="val">The string value to convert into a DateTimeOffset value.</param>
        /// <returns><see>
        ///         <cref>DateTimeOffset?</cref>
        ///     </see>
        /// </returns>
        /// <remarks><see cref="DateTimeOffset.Now"/> will be returned if <see cref="val"/> is NULL, empty or contains only spaces.</remarks>
        public static DateTimeOffset? ConvertDateTimeOffset(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return null;
            }
            string tempStr = val.RemoveSpaces();

            if (string.IsNullOrEmpty(tempStr))
            {
                return null;
            }
            return DateTimeOffset.Parse(val);
        }

        /// <summary>
        /// Converts a <see>
        ///         <cref>bool?</cref>
        ///     </see> reference type to a regular bool value type.
        /// </summary>
        /// <param name="val">The <see>
        ///         <cref>bool?</cref>
        ///     </see> value to convert.</param>
        /// <returns><see cref="bool"/> </returns>
        /// <remarks>returns false if the passed in value is null; otherwise it returns the corresponding boolean value.</remarks>
        public static bool ConvertNullable(bool? val)
        {
            if (val == null)
            {
                return false;
            }
            return (bool)val;
        }

        /// <summary>
        /// Converts a <see>
        ///         <cref>double?</cref>
        ///     </see> reference type to a regular double value type.
        /// </summary>
        /// <param name="val">The <see>
        ///         <cref>double?</cref>
        ///     </see> value to convert.</param>
        /// <returns><see cref="double"/> </returns>
        /// <remarks>returns 0.00 if the passed in value is null; otherwise it returns the corresponding double value.</remarks>
        public static double ConvertNullable(double? val)
        {
            if (val == null)
            {
                return 0.00;
            }
            return (double)val;
        }

        /// <summary>
        /// Converts a <see>
        ///         <cref>Guid?</cref>
        ///     </see> reference type to a regular Guid value type.
        /// </summary>
        /// <param name="val">The <see>
        ///         <cref>Guid?</cref>
        ///     </see> value to convert.</param>
        /// <returns><see cref="Guid"/> </returns>
        /// <remarks>returns <see cref="Guid.Empty"/> if the passed in value is null; otherwise it returns the corresponding Guid value.</remarks>
        public static Guid ConvertNullable(Guid? val)
        {
            if (val == null)
            {
                return Guid.Empty;
            }
            return (Guid)val;
        }

        /// <summary>
        /// Converts a <see>
        ///         <cref>int?</cref>
        ///     </see> reference type to a regular int value type.
        /// </summary>
        /// <param name="val">The <see>
        ///         <cref>int?</cref>
        ///     </see> value to convert.</param>
        /// <returns><see cref="int"/> </returns>
        /// <remarks>returns 0 if the passed in value is null; otherwise it returns the corresponding int value.</remarks>
        public static int ConvertNullable(int? val)
        {
            if (val == null)
            {
                return 0;
            }
            return (int)val;
        }

        /// <summary>
        /// Converts a <see>
        ///         <cref>long?</cref>
        ///     </see> reference type to a regular long value type.
        /// </summary>
        /// <param name="val">The <see>
        ///         <cref>long?</cref>
        ///     </see> value to convert.</param>
        /// <returns><see cref="long"/> </returns>
        /// <remarks>returns 0 if the passed in value is null; otherwise it returns the corresponding long value.</remarks>
        public static long ConvertNullable(long? val)
        {
            if (val == null)
            {
                return 0;
            }
            return (long)val;
        }

        /// <summary>
        /// Converts a <see>
        ///         <cref>short?</cref>
        ///     </see> reference type to a regular short value type.
        /// </summary>
        /// <param name="val">The <see>
        ///         <cref>short?</cref>
        ///     </see> value to convert.</param>
        /// <returns><see cref="short"/> </returns>
        /// <remarks>returns 0 if the passed in value is null; otherwise it returns the corresponding short value.</remarks>
        public static short ConvertNullable(short? val)
        {
            if (val == null)
            {
                return 0;
            }
            return (short)val;
        }
    }
}
