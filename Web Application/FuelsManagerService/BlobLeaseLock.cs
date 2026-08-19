// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlobLeaseLock.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//  Uses Azure Storage blob leases to prevent multiple instances of a worker role from
// executing the same method simultaneously.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManagerService
{
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.ServiceRuntime;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using System;
    using System.Configuration;
    using Timer = System.Timers.Timer;

    /// <summary>
    /// Uses Azure Storage blob leases to prevent multiple instances of a worker role from
    /// executing the same method simultaneously.
    /// This idea and implementation are based on Steve Marx's article "Managing Concurrency in Windows Azure with Leases."
    /// http://blog.smarx.com/posts/managing-concurrency-in-windows-azure-with-leases
    /// </summary>
    public class BlobLeaseLock : IDisposable
    {
        /// <summary>
        /// The number of seconds that should elapse between attempts to renew the lease.
        /// Intentionally shorter than the lease duration to be sure the lease gets renewed before it expires.
        /// </summary>
        private const int RenewLeaseTimerTickSeconds = 40;

        /// <summary>
        /// The amount of time to acquire the lease for.
        /// Can be 15 - 60 seconds, or infinite.
        /// Leases are released automatically after this period elapses if not renewed.
        /// Don't use infinite! Think of what would happen if for some reason the lease was not released - the method would never execute again.
        /// </summary>
        private const int LeaseDurationSeconds = 60;

        /// <summary>
        /// Must be lowercase and 3 - 63 characters long.
        /// </summary>
        private const string BlobContainerName = "system";

        /// <summary>
        /// The name of the Configuration Setting in the Service Configuration file which contains the connection string to our storage account
        /// </summary>
        private const string ConnectionStringConfigurationSettingName = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";

        /// <summary>
        /// The lease ID we get we when acquire the lease on the blob.
        /// </summary>
        private readonly string leaseID;

        /// <summary>
        /// Represents the blob that we're trying to acquire the lease for.
        /// </summary>
        private CloudBlockBlob blob;

        /// <summary>
        /// A timer which renews the lease when it ticks.
        /// </summary>
        private Timer renewLeaseTimer = null;

        /// <summary>
        /// Used to prevent the possibility of disposing twice.
        /// </summary>
        private bool isDisposed = false;

        /// <summary>
        /// True if a lease was successfully acquired.
        /// </summary>
        public readonly bool HasLease = false;

        /// <summary>
        /// Construct a BlobLeaseLock. 
        /// Attempt to acquire a lease on the blob representing the provided methodIdentifier and start a timer to renew the lease.
        /// </summary>
        /// <param name="methodIdentifier">Identifies what we're trying to "lock" - for example, this might be a name of a method or a piece of functionality.</param>
        public BlobLeaseLock(string methodIdentifier)
        {
            try
            {
                // Append the DeploymentId to the methodIdentifier provided in case two deployments are sharing the same Storage account (e.g. Production and Staging)
                string blobName = methodIdentifier + RoleEnvironment.DeploymentId;

                // Connect to the storage account
                string storageAccountConnectionString = CloudConfigurationManager.GetSetting(ConnectionStringConfigurationSettingName);
                if (string.IsNullOrEmpty(storageAccountConnectionString))
                {
                    throw new ConfigurationErrorsException("Cannot locate the Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString setting");
                }

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                // Get the container we expect the blobs to "lock" on to be in. Create the container if it doesn't exist.
                CloudBlobContainer blobContainer = blobClient.GetContainerReference(BlobContainerName);
                blobContainer.CreateIfNotExists();

                // Get a reference to the blob matching the name which identifies the method we're executing.
                // Create the blob if it doesn't exist.
                this.blob = blobContainer.GetBlockBlobReference(blobName);
                this.CreateBlobIfNotExists();

                // Try to get a lease on the blob. This will throw if it's not successful
                this.leaseID = this.blob.AcquireLease(TimeSpan.FromSeconds(LeaseDurationSeconds), null);
                this.HasLease = true;

                this.renewLeaseTimer = new Timer(TimeSpan.FromSeconds(RenewLeaseTimerTickSeconds).TotalMilliseconds);
                this.renewLeaseTimer.Elapsed += this.RenewLeaseTimerElapsed;
                this.renewLeaseTimer.Start();
            }
            catch (StorageException storageException)
            {
                // A 409 error in this situation indicates that the blob is already leased
                if (storageException.RequestInformation == null || storageException.RequestInformation.HttpStatusCode != 409)
                {
                    FuelsManagerServiceLogger.Instance.LogError(storageException);
                    throw;
                }
            }
        }

        /// <summary>
        /// Call the provided method using a blob lease to ensure that other processes or instances can't call it as well.
        /// </summary>
        /// <param name="methodToExecute">The method to execute exclusively of other instances or processes.</param>
        /// <param name="methodIdentifier">Identifies what we're trying to "lock" - for example, this might be a name of a method or a piece of functionality.</param>
        public static void Execute(Action methodToExecute, string methodIdentifier)
        {
            using (BlobLeaseLock blobLeaseLock = new BlobLeaseLock(methodIdentifier))
            {
                if (blobLeaseLock.HasLease)
                {
                    methodToExecute();
                }
            }
        }

        /// <summary>
        /// When the timer that renews leases ticks, renew the lease.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void RenewLeaseTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                // Prevent any possibility of this event firing after we've disposed.
                // Stopping a Timer or disposing of it doesn't necessarily mean that the elapsed event won't fire again.
                if (!this.isDisposed && this.HasLease)
                {
                    blob.RenewLease(AccessCondition.GenerateLeaseCondition(this.leaseID));
                }
            }
            catch (StorageException storageException)
            {
                // A 409 error in this situation indicates that the blob is not leased, or that the lease is breaking, broken, or expired
                if (storageException.RequestInformation == null || storageException.RequestInformation.HttpStatusCode != 409)
                {
                    FuelsManagerServiceLogger.Instance.LogError(storageException);
                }
            }
            catch (Exception exception)
            {
                FuelsManagerServiceLogger.Instance.LogError(exception);
            }
        }

        /// <summary>
        /// Create the blob if it doesn't exist. 
        /// </summary>
        private void CreateBlobIfNotExists()
        {
            try
            {
                if (!this.blob.Exists())
                {
                    // If the blob doesn't exist, create an empty file. 
                    // The AccessCondition prevents any possibility of the object being created if it already exists (* is a wildcard).
                    this.blob.UploadFromByteArray(new byte[0], 0, 0, AccessCondition.GenerateIfNoneMatchCondition("*"));
                }
            }
            catch (StorageException storageException)
            {
                // We want to throw unless we're getting an exception just because the blob already exists. 
                if (storageException.RequestInformation == null || storageException.RequestInformation.HttpStatusCode != 409) 
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// This Dispose is from IDisposable. When it's called, dispose of managed and unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            // Tell the garbage collector that it doesn't need to finalize the object.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of the BlobLeaseLock object. Most importantly, stops the timer which renews the lease and releases the lease.
        /// </summary>
        /// <param name="disposing">Indicates whether managed resourcse should be disposed of. True if called from Dispose(). False if called from the Finalizer.</param>
        protected void Dispose(bool disposing)
        {
            // Make sure we don't dispose twice
            if (this.isDisposed)
            {
                return;
            }

            // If this method is called from Dispose(), release managed resources
            if (disposing)
            {
                // Stop and dispose of the timer
                if (this.renewLeaseTimer != null)
                {
                    this.renewLeaseTimer.Stop();
                    this.renewLeaseTimer.Dispose();
                    this.renewLeaseTimer = null;
                }

                // Release the lease on the blob
                if (this.blob != null)
                {
                    try
                    {
                        if (this.HasLease)
                        {
                            this.blob.ReleaseLease(AccessCondition.GenerateLeaseCondition(this.leaseID));
                        }
                    }
                    catch (Exception exception)
                    {
                        FuelsManagerServiceLogger.Instance.LogError(exception);
                    }

                    this.blob = null;
                }
            }

            this.isDisposed = true;
        }

        /// <summary>
        /// The Finalizer for the BlobLeaseLock object. Calls Dispose() telling it to only dispose of unmanaged resources. 
        /// Managed resources have already been cleaned up by the garbage collector.
        /// </summary>
        ~BlobLeaseLock()
        {
            this.Dispose(false);
        }
    }
}
