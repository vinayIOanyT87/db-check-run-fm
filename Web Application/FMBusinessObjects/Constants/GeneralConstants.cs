// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneralConstants.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Contains error-related constants and strings (exposed from resources)
//   Instances of this class allow for aggregation of error messages for
//   a detailed summary of a failure
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.Constants
{
    /// <summary>
    /// Contains generic-constants and strings (exposed from resources)
    /// Instances of this class allow for aggregation of generic messages for 
    /// a detailed summary of a failure
    /// </summary>
    public static class GeneralConstants
    {
        #region Generic for UI
        /// <summary>
        /// Gets a password mask to use as a placeholder for password fields - **********
        /// </summary>
        public static string PasswordPlaceholder
        {
            get { return @"**********"; }
        }

        #endregion Generic for UI
    }
}
