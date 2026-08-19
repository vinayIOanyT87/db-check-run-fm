// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureStorage.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the AzureStorage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses.Azure
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Microsoft.WindowsAzure.ServiceRuntime;
    using Microsoft.WindowsAzure.Storage;

    /// <summary>
    /// Support class for getting information about AzureStorage running environment.
    /// </summary>
    public class AzureStorage
    {
        #region Private Attributes

        /// <summary>
        /// The storage account.
        /// </summary>
        private CloudStorageAccount storageAccount = null;

        #endregion Private Attributes

        #region Constructors / Initialization
        #endregion Constructors / Initialization

        #region Properties
        /// <summary>
        /// Gets the storage account.
        /// </summary>
        internal CloudStorageAccount StorageAccount
        {
            get
            {
                return this.storageAccount;
            }
        }
        #endregion Properties

        #region Private Methods and Operators

        /// <summary>
        /// The connect to azure storage account.
        /// </summary>
        private void ConnectToAzureStorageAccount()
        {
            if (null == this.storageAccount)
            {
                string dataConnectionStringValue = RoleEnvironment.GetConfigurationSettingValue("DataConnectionString");

                // Retrieve storage account from connection-string
                // There is a bug with the new Azure Storage 2.0 SDK which causes use development storage to not be parsed correctly
                // http://stackoverflow.com/questions/13110488/azure-october-2012-sdk-broke-usedevelopmentstorage-true
                if (string.Compare(dataConnectionStringValue, "UseDevelopmentStorage=true", System.StringComparison.OrdinalIgnoreCase) == 0)
                {
                    this.storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
                }
                else
                {
                    this.storageAccount = CloudStorageAccount.Parse(dataConnectionStringValue);
                }
            }
        }
        #endregion Private Methods and Operators
    }
}