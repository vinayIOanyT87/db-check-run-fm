// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncClientServerProxyFMOffline.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SyncClientServerProxyFMOffline type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Data;
    using System.ServiceModel;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;

    using Microsoft.Synchronization.Data;

    /// <summary>
    /// The sync client server proxy fm.
    /// </summary>
    public class SyncClientServerProxyFMOffline : ISyncServerProviderFM, IDisposable
	{
		#region Private data members
        /// <summary>
        /// The _ disposed.
        /// </summary>
        private bool _Disposed = false;

        /// <summary>
        /// Offline synchronization output file path.
        /// </summary>
        private string _OfflineSynchronizationFilePath = string.Empty;

        /// <summary>
        /// Generated offline synchronization file(s).
        /// </summary>
        private List<string> _OfflineSynchronizationFiles = new List<string>();

        /// <summary>
        /// The _ context.
        /// </summary>
        private SyncContextFM _Context = null;

		/// <summary>
		/// The _ synchronize scope
		/// </summary>
		private SyncScopeDO _SyncScope;

        /// <summary>
        /// The _ sync client config.
        /// </summary>
        private SyncClientConfigurationDO _SyncClientConfig = null;

        #endregion Private data members

        #region Constructors / Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SyncClientServerProxyFMOffline" /> class.
		/// </summary>
		/// <param name="outputFolderPath">The output Folder Path.</param>
		/// <param name="syncClientConfig">Current synchronization client config.</param>
		/// <param name="contextFM">The context fm.</param>
		/// <param name="syncScope">The synchronize scope.</param>
		public SyncClientServerProxyFMOffline(string outputFolderPath, SyncClientConfigurationDO syncClientConfig, SyncContextFM contextFM, SyncScopeDO syncScope)
        {
            this._SyncClientConfig = syncClientConfig;
			this._Context = contextFM;
			this._SyncScope = syncScope;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SyncClientServerProxyFMOffline"/> class. 
        /// </summary>
        ~SyncClientServerProxyFMOffline()
        {
            this.Dispose(false);
        }
        #endregion Constructors / Destructors

        #region ISyncClientServerProxyServerProvider Interface Implementation

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        public SyncContextFM Context
        {
            get
            {
                return this._Context;
            }

            set
            {
// ReSharper disable RedundantCheckBeforeAssignment
                if (value == this._Context)
// ReSharper restore RedundantCheckBeforeAssignment
                {
                    return;
                }

                this._Context = value;
            }
        }

        /// <summary>
        /// The apply changes.
        /// </summary>
        /// <param name="groupMetadata">
        /// The p group metadata.
        /// </param>
        /// <param name="dataSet">
        /// The p data set.
        /// </param>
        /// <param name="syncSession">
        /// The p sync session.
        /// </param>
        /// <returns>
        /// The <see cref="SyncContext"/>.
        /// </returns>
        /// <exception cref="FaultException{T}">
        /// Thrown if an error is encountered while writing the current set of client changes to the offline synchronization output file.
        /// </exception>
        public SyncContext ApplyChanges(SyncGroupMetadata groupMetadata, DataSet dataSet, SyncSession syncSession)
        {
            try
            {
                SyncContext context = new SyncContext();

                context.DataSet = dataSet;
                context.GroupProgress = new SyncGroupProgress(groupMetadata, dataSet);

                return context;
            }
            catch (Exception eX)
            {
                throw new FaultException<Exception>(eX, new FaultReason("Offline synchronization exception encountered while writing local changes to file."), new FaultCode("APPLYERROR"));
            }
        }

        /// <summary>
        /// The get changes.
        /// </summary>
        /// <param name="groupMetadata">
        /// The p group metadata.
        /// </param>
        /// <param name="syncSession">
        /// The p sync session.
        /// </param>
        /// <returns>
        /// The <see cref="SyncContext"/>.
        /// </returns>
        /// <exception cref="FaultException{T}">
        /// Thrown if an error is encountered while writing data set placeholders in the current offline synchronization output file.
        /// </exception>
        public SyncContext GetChanges(SyncGroupMetadata groupMetadata, SyncSession syncSession)
        {
            try
            {
                SyncContext context = new SyncContext();

                context.DataSet = this.GeneratePlaceHolderDataSet(groupMetadata, syncSession);
                context.GroupProgress = new SyncGroupProgress(groupMetadata, context.DataSet);

                return context;
            }
            catch (Exception eX)
            {
                throw new FaultException<Exception>(eX, new FaultReason("Offline synchronization exception encountered while writing server changes placeholder to file."), new FaultCode("GETERROR"));
            }
        }

        /// <summary>
        /// The get schema.
        /// </summary>
        /// <param name="tableNames">
        /// The p table names.
        /// </param>
        /// <param name="syncSession">
        /// The p sync session.
        /// </param>
        /// <returns>
        /// The <see cref="SyncSchema"/>.
        /// </returns>
        /// <exception cref="FaultException{T}">
        /// Throws this exception if any exceptions were encountered during this mock offline file creation.
        /// </exception>
        public SyncSchema GetSchema(Collection<string> tableNames, SyncSession syncSession)
        {
            try
            {
                return new SyncSchema();
            }
            catch (Exception eX)
            {
                throw new FaultException<Exception>(eX, new FaultReason("Offline synchronization exception encountered while getting schema."), new FaultCode("SCHEMAERROR"));
            }
        }

        /// <summary>
        /// The get server info.
        /// </summary>
        /// <param name="syncSession">
        /// The p sync session.
        /// </param>
        /// <returns>
        /// The <see cref="SyncServerInfo"/>.
        /// </returns>
        /// <exception cref="FaultException{T}">
        /// Throws this exception if any exceptions were encountered during this mock offline file creation.
        /// </exception>
        public SyncServerInfo GetServerInfo(SyncSession syncSession)
        {
            try
            {
                return new SyncServerInfo();
            }
            catch (Exception eX)
            {
                throw new FaultException<Exception>(eX, new FaultReason("Offline synchronization exception encountered while getting server information."), new FaultCode("SERVERINFORROR"));
            }
        }
        #endregion ISyncClientServerProxyServerProvider Interface Implementation

        #region Supporting Methods

        /// <summary>
        /// The generate place holder data set.
        /// </summary>
        /// <param name="groupMetadata">
        /// The p group metadata.
        /// </param>
        /// <param name="syncSession">
        /// The p sync session.
        /// </param>
        /// <returns>
        /// The <see cref="DataSet"/>.
        /// </returns>
        private DataSet GeneratePlaceHolderDataSet(SyncGroupMetadata groupMetadata, SyncSession syncSession)
        {
            DataSet mockDataSet = new DataSet(groupMetadata.GroupName);

            foreach (var tmd in groupMetadata.TablesMetadata)
            {
                mockDataSet.Tables.Add(new DataTable(tmd.TableName));
            }

            return mockDataSet;
        }

        #endregion Supporting Methods

        #region IDisposable Interface Implementation

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (this._Disposed == false)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                }

                // Note disposing has been done.
                this._Disposed = true;
            }
        }
        #endregion IDisposable Interface Implementation
    }
}
