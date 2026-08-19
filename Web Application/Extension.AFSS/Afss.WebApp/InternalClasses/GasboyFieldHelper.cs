// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyFieldHelper.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Contains methods which help display values on Gasboy screens
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Afss.Module.Gasboy.WebApp
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;

    /// <summary>
    /// Contains methods which help display values on Gasboy screens
    /// </summary>
    public static class GasboyFieldHelper
    {
        /// <summary>
        /// Display a user-friendly enumeration value based on the Display attribute for the enum value.
        /// </summary>
        /// <param name="value">The enumeration value</param>
        /// <returns>A user-friendly enumeration value based on the Display attribute for the enum value.</returns>
        public static string EnumDisplayName(this Enum value)
        {
            // Get the type of the value
            Type enumType = value.GetType();

            // Get a string representation of the value. We'll use this if there is no display attribute
            string enumValue = Enum.GetName(enumType, value);
            string outString = enumValue;

            // Find the display attribute 
            MemberInfo[] memberInfo = enumType.GetMember(enumValue);

            if (memberInfo.Length > 0)
            {
                MemberInfo member = enumType.GetMember(enumValue)[0];

                var displayAttributes = member.GetCustomAttributes(typeof(DisplayAttribute), false);

                if (displayAttributes.Length > 0)
                {
                    outString = ((DisplayAttribute)displayAttributes[0]).Name;
                }
            }

            return outString;
        }
    }
}