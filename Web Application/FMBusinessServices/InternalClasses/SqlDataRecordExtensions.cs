// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlDataRecordExtensions.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Contains helpful extensions to the SqlDataRecord class to support things like nullable fields
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses
{
    using Microsoft.SqlServer.Server;
    using System;

    /// <summary>
    /// Contains helpful extensions to the SqlDataRecord class to support things like nullable fields
    /// </summary>
    public static class SqlDataRecordExtensions
    {
        /// <summary>
        /// Set a column in a SqlDataRecord to the integer value provided if it has a value, or set it to NULL otherwise.
        /// </summary>
        /// <param name="record">The SqlDataRecord we're operating on</param>
        /// <param name="index">Identifies the column we're setting the value for</param>
        /// <param name="value">The value to examine and set in the column specified.</param>
        public static void SetNullableInt(this SqlDataRecord record, int index, int? value)
        {
            if (value.HasValue)
            {
                record.SetInt32(index, value.GetValueOrDefault());
            }
            else
            {
                record.SetDBNull(index);
            }
        }

        /// <summary>
        /// Set a column in a SqlDataRecord to the double value provided if it has a value, or set it to NULL otherwise.
        /// </summary>
        /// <param name="record">The SqlDataRecord we're operating on</param>
        /// <param name="index">Identifies the column we're setting the value for</param>
        /// <param name="value">The value to examine and set in the column specified.</param>
        public static void SetNullableDouble(this SqlDataRecord record, int index, double? value)
        {
            if (value.HasValue)
            {
                record.SetDouble(index, value.GetValueOrDefault());
            }
            else
            {
                record.SetDBNull(index);
            }
        }

        /// <summary>
        /// Set a column in a SqlDataRecord to the DateTimeOffset value provided if it has a value, or set it to NULL otherwise.
        /// </summary>
        /// <param name="record">The SqlDataRecord we're operating on</param>
        /// <param name="index">Identifies the column we're setting the value for</param>
        /// <param name="value">The value to examine and set in the column specified.</param>
        public static void SetNullableDateTimeOffset(this SqlDataRecord record, int index, DateTimeOffset? value)
        {
            if (value.HasValue)
            {
                record.SetDateTimeOffset(index, value.GetValueOrDefault());
            }
            else
            {
                record.SetDBNull(index);
            }
        }

        /// <summary>
        /// Set a column in a SqlDataRecord to the Guid value provided if it has a non-empty value, or set it to NULL otherwise.
        /// </summary>
        /// <param name="record">The SqlDataRecord we're operating on</param>
        /// <param name="index">Identifies the column we're setting the value for</param>
        /// <param name="value">The value to examine and set in the column specified.</param>
        public static void SetNullableGuid(this SqlDataRecord record, int index, Guid value)
        {
            if (value != Guid.Empty)
            {
                record.SetGuid(index, value);
            }
            else
            {
                record.SetDBNull(index);
            }
        }

        /// <summary>
        /// Set a column in a SqlDataRecord to the boolean value provided if it has a value, or set it to NULL otherwise.
        /// </summary>
        /// <param name="record">The SqlDataRecord we're operating on</param>
        /// <param name="index">Identifies the column we're setting the value for</param>
        /// <param name="value">The value to examine and set in the column specified.</param>
        public static void SetNullableBoolean(this SqlDataRecord record, int index, bool? value)
        {
            if (value.HasValue)
            {
                record.SetBoolean(index, value.GetValueOrDefault());
            }
            else
            {
                record.SetDBNull(index);
            }
        }

        /// <summary>
        /// Set a column in a SqlDataRecord to the string value provided if it has a non-null and non-empty value, or set it to NULL otherwise.
        /// </summary>
        /// <param name="record">The SqlDataRecord we're operating on</param>
        /// <param name="index">Identifies the column we're setting the value for</param>
        /// <param name="value">The value to examine and set in the column specified.</param>
        public static void SetNullableString(this SqlDataRecord record, int index, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                record.SetString(index, value);
            }
            else
            {
                record.SetDBNull(index);
            }
        }
    }
}