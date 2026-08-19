// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureBlobStorage.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the AzureBlobStorage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses.Azure
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.IO.Compression;

    using Microsoft.WindowsAzure.ServiceRuntime;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// Support class for getting information about AzureBlobStorage running environment.
    /// </summary>
    public class AzureBlobStorage : AzureStorage
    {
        #region Private Attributes

        /// <summary>
        /// The cloud table client.
        /// </summary>
        private CloudBlobClient blobClient = null;

        /// <summary>
        /// The cloud blob container
        /// </summary>
        private CloudBlobContainer blobContainer = null;

        #endregion Private Attributes

        #region Constructors / Initialization

        /// <summary>
        /// Prevents a default instance of the <see cref="AzureBlobStorage"/> class from being created. 
        /// </summary>
        private AzureBlobStorage()
        {
            this.GetBlobClient();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobStorage"/> class.
        /// </summary>
        /// <param name="containerName">
        /// The container Name.
        /// </param>
        public AzureBlobStorage(string containerName)
        {
            this.GetBlobClient();
            this.GetContainer(containerName);
        }
        #endregion Constructors / Initialization

        #region Static Methods and Operators
        #endregion Static Methods and Operators

        #region Methods and Operators
        public void SaveBlob(string blobName, MemoryStream blobStream)
        {
            CloudBlockBlob blockBlob = this.blobContainer.GetBlockBlobReference(blobName);
            
            blockBlob.UploadFromStream(blobStream);
        }
        #endregion Methods and Operators

        #region Private Methods and Operators

        /// <summary>
        /// Gets an instance of a cloud table client to use.
        /// </summary>
        private void GetBlobClient()
        {
            if (null == this.blobClient)
            {
                this.blobClient = this.StorageAccount.CreateCloudBlobClient();
            }
        }

        /// <summary>
        /// Returns a reference to a blob storage container
        /// </summary>
        /// <param name="containerName">
        /// The name of the blob container
        /// </param>
        private void GetContainer(string containerName)
        {
            this.blobContainer = this.blobClient.GetContainerReference(containerName);
            this.blobContainer.CreateIfNotExists();
        }

        #endregion Private Methods and Operators
    }
}