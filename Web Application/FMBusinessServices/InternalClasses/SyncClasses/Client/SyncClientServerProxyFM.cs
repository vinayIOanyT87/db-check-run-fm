// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncClientServerProxyFM.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SyncClientServerProxyFM type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
    using System;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Data;
    using System.IO;
    using System.IO.Compression;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.ServiceModel;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;
    using FMBusinessObjects.UtilityObjects;

    using FMBusinessServices.InternalClasses;
    using FMBusinessServices.InternalClasses.SyncClasses;

    using Microsoft.Synchronization.Data;

    /// <summary>
    /// The sync client server proxy fm.
    /// </summary>
    public class SyncClientServerProxyFM : ISyncServerProviderFM, IDisposable
	{
		#region Private data members

        /// <summary>
        /// The _ disposed.
        /// </summary>
        private bool _Disposed = false;

        /// <summary>
        /// The _ context.
        /// </summary>
        private SyncContextFM _Context;

		/// <summary>
		/// The _ synchronize scope
		/// </summary>
	    private SyncScopeDO _SyncScope;

        /// <summary>
        /// The _ service binding type.
        /// </summary>
        private string _ServiceBindingType = string.Empty;

        /// <summary>
        /// The _ service binding configuration.
        /// </summary>
        private string _ServiceBindingConfiguration = string.Empty;

        /// <summary>
        /// The _ service proxy url.
        /// </summary>
        private string _ServiceProxyURL = string.Empty;

        /// <summary>
        /// The _ sync provider factory.
        /// </summary>
        private FMChannelFactory<IEnterpriseSynchronization> _SyncProviderFactory = null;
        #endregion Private data members

        #region Constructors / Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SyncClientServerProxyFM"/> class.
		/// </summary>
		/// <param name="syncClientConfig">The synchronize client configuration.</param>
		/// <param name="contextFM">The context fm.</param>
		/// <param name="syncScope">The synchronize scope.</param>
        public SyncClientServerProxyFM(SyncClientConfigurationDO syncClientConfig, SyncContextFM contextFM, SyncScopeDO syncScope)
            : this(syncClientConfig.EnterpriseURL, syncClientConfig, contextFM, syncScope)
        {
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="SyncClientServerProxyFM"/> class.
		/// </summary>
		/// <param name="serviceProxyUrl">The service proxy URL.</param>
		/// <param name="syncClientConfig">The synchronize client configuration.</param>
		/// <param name="contextFM">The context fm.</param>
		/// <param name="syncScope">The synchronize scope.</param>
		/// <exception cref="System.Exception"></exception>
       public SyncClientServerProxyFM(string serviceProxyUrl, SyncClientConfigurationDO syncClientConfig, SyncContextFM contextFM, SyncScopeDO syncScope)
        {
            this._ServiceBindingType = ConfigurationManager.AppSettings[FMSyncChannelHelper.BindingTypeConfigKey];

            if (this._ServiceBindingType == null)
            {
                throw new Exception(ErrorConstants.SYNC_ERR_MSG_08001); // Binding error message
            }

            this._ServiceBindingConfiguration = ConfigurationManager.AppSettings[FMSyncChannelHelper.BindingConfigurationConfigKey];

            this._SyncProviderFactory = FMSyncChannelHelper.SyncChannelFactory<IEnterpriseSynchronization>(syncClientConfig, serviceProxyUrl);
            this._ServiceProxyURL = serviceProxyUrl;
	        this._Context = contextFM;
	        this._SyncScope = syncScope;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SyncClientServerProxyFM"/> class. 
        /// </summary>
        ~SyncClientServerProxyFM()
        {
            this.Dispose(false);
        }
        #endregion Constructors / Destructors

        #region ISyncClientServerProxyServerProvider Interface Implementation

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
        /// Throws this exception if any exceptions were encountered while processing at the remote server.
        /// </exception>
        public SyncContext ApplyChanges(SyncGroupMetadata groupMetadata, DataSet dataSet, SyncSession syncSession)
        {
            if (SyncTracer.IsVerboseEnabled())
            {
                SyncTracer.Verbose("Calling Remote Synchronization WebService method: ApplyChanges()");
                SyncTracer.Verbose(1, "** APPLYING CLIENT CHANGES TO SERVER **");
            }

            Func<IEnterpriseSynchronization, SyncGroupMetadata, byte[], SyncSession, byte[], SyncContext>
                callback =
                    delegate(IEnterpriseSynchronization channelProxy,
                        SyncGroupMetadata tempGroupMetaData,
                        byte[] tempCompressedDataSetBytes,
                        SyncSession tempSyncSession,
                        byte[] tempCompressedContextFmBytes)
                    {
                        return channelProxy.ApplyChanges(
                            tempGroupMetaData,
                            tempCompressedDataSetBytes,
                            tempSyncSession,
                            tempCompressedContextFmBytes);
                    };

            long originalDataSetLength = 0;
            long originalLength;

            if (SyncTracer.IsInfoEnabled())
            {
                originalDataSetLength = CompressionProcessor.GetApproximateDataSetSize(dataSet);
            }

            byte[] compressedDataSetBytes = CompressionProcessor.CompressDataSet(dataSet);
            byte[] compressedContextFmBytes = CompressionProcessor.CompressObject(this._Context, out originalLength);

            if (SyncTracer.IsErrorEnabled())
            {
                SyncTracer.Info(1, "ApplyChanges(): ");
                SyncTracer.Info(2, "  Sent contextFM UnCompressed: {0,12:D10}   Compressed: {1,12:D10}", originalLength, compressedContextFmBytes.Length);
                SyncTracer.Info(2, "    Sent DataSet UnCompressed: {0,12:D10}   Compressed: {1,12:D10}", originalDataSetLength, compressedDataSetBytes.Length);
            }


	        SyncContext context = null;

	        try
	        {
		        // Our implementation will strip off the context.DataSet property before returning back to us in order to minimize the return context size.
		        context = FMSyncChannelHelper.MakeCall<IEnterpriseSynchronization, SyncContext>(
			        this._SyncProviderFactory,
			        (pChannelProxy) =>
				        callback(pChannelProxy, groupMetadata, compressedDataSetBytes, syncSession, compressedContextFmBytes));
	        }
	        catch (Exception e)
	        {
		        if (SyncTracer.IsErrorEnabled())
		        {
			        SyncTracer.Error(3, e.ToString());
		        }
				throw new SyncCommunicationException("There was an error executing ApplyChanges(), it should be retried automatically.", e);
	        }


            // This may happen if the remote server does not support or recognize the specified SyncGroup.
            if (null == context)
            {
                context = new SyncContext();
                // context.DataSet = dataSet;
                // context.GroupProgress = new SyncGroupProgress(groupMetadata, dataSet);
            }
            else
            {
                // We need to repopulate this property before we give the context back to the SyncFramework or it will blow chunks.
                // context.DataSet = context.GroupProgress.Changes;
            }

            if (SyncTracer.IsVerboseEnabled())
            {
                SyncTracer.Verbose(1, "** FINISHED APPLYING CLIENT CHANGES TO SERVER **");
                SyncTracer.Verbose("Called Remote Synchronization WebService method: ApplyChanges()");
            }

            return context;
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
        /// Throws this exception if any exceptions were encountered while processing at the remote server.
        /// </exception>
        public SyncContext GetChanges(SyncGroupMetadata groupMetadata, SyncSession syncSession)
        {
            if (SyncTracer.IsVerboseEnabled())
            {
                SyncTracer.Verbose("Calling Remote Synchronization WebService method: GetChanges()");
                SyncTracer.Verbose(1, "** GETTING SERVER CHANGES FOR CLIENT **");
            }

            Func<IEnterpriseSynchronization, SyncGroupMetadata, SyncSession, SyncContextFM, SyncContext> callback =
                delegate(IEnterpriseSynchronization channelProxy,
                            SyncGroupMetadata tempGroupMetaData,
                            SyncSession tempSyncSession,
                            SyncContextFM contextFm)
                    {
                        byte[] dataSetSurrogateBytes;

                        long originalLength = 0;

                        if (SyncTracer.IsInfoEnabled())
                        {
                            originalLength = CompressionProcessor.GetApproximateObjectSize(contextFm);
                        }

                        byte[] compressedContextFmBytes = CompressionProcessor.CompressObject(contextFm);

							  SyncContext syncContext = channelProxy.GetChanges(
                            tempGroupMetaData,
                            tempSyncSession,
                            compressedContextFmBytes,
                            out dataSetSurrogateBytes);

                        syncContext.DataSet = CompressionProcessor.DecompressDataSet(dataSetSurrogateBytes);
						syncContext.GroupProgress = new SyncGroupProgress(groupMetadata, syncContext.DataSet);

						SyncProviderHelperFM.GetMaxRowVersions(groupMetadata, this._Context, this._SyncScope, syncContext, true);

                        if (SyncTracer.IsInfoEnabled())
                        {
                            long dataSetLength = CompressionProcessor.GetApproximateDataSetSize(syncContext.DataSet);

                            SyncTracer.Info(1, "GetChanges():");
                            SyncTracer.Info(2, "  Sent contextFM UnCompressed: {0,12:D10}   Compressed: {1,12:D10}", originalLength, compressedContextFmBytes.Length);
                            SyncTracer.Info(2, "Returned DataSet UnCompressed: {0,12:D10}   Compressed: {1,12:D10}", dataSetLength, dataSetSurrogateBytes.Length);
                        }

                        return syncContext;
                    };



	        SyncContext context = null;
	        try
	        {
				context = FMSyncChannelHelper.MakeCall<IEnterpriseSynchronization, SyncContext>(this._SyncProviderFactory, (pChannelProxy) => callback(pChannelProxy, groupMetadata, syncSession, this._Context));
	        }
	        catch (Exception e)
	        {
				if (SyncTracer.IsErrorEnabled())
		        {
			        SyncTracer.Error(3, e.ToString());
		        }
				throw new SyncCommunicationException("There was an error executing GetChanges(), it should be retried automatically.", e);
	        }


			// This may happen if the remote server does not support or recognize the specified SyncGroup.
			if (null == context)
			{
				context = new SyncContext();
				context.DataSet = this.GeneratePlaceHolderDataSet(groupMetadata, syncSession);
				context.GroupProgress = new SyncGroupProgress(groupMetadata, context.DataSet);
			}
			
            if (SyncTracer.IsVerboseEnabled())
            {
                SyncTracer.Verbose(1, "** FINISHED GETTING SERVER CHANGES FOR CLIENT**");
                SyncTracer.Verbose("Called Remote Synchronization WebService method: GetChanges()");
            }

            return context;
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
        /// Throws this exception if any exceptions were encountered while processing at the remote server.
        /// </exception>
        public SyncSchema GetSchema(Collection<string> tableNames, SyncSession syncSession)
        {
            Func<IEnterpriseSynchronization, Collection<string>, SyncSession, byte[], SyncSchema> callback = delegate(IEnterpriseSynchronization channelProxy, Collection<string> tempTableNames, SyncSession tempSyncSession, byte[] tempCompressedContextFmBytes)
            {
                return channelProxy.GetSchema(tempTableNames, tempSyncSession, tempCompressedContextFmBytes);
            };

            byte[] compressedContextFmBytes = CompressionProcessor.CompressObject(this._Context);

            return FMSyncChannelHelper.MakeCall<IEnterpriseSynchronization, SyncSchema>(this._SyncProviderFactory, (pChannelProxy) => callback(pChannelProxy, tableNames, syncSession, compressedContextFmBytes));
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
        /// Throws this exception if any exceptions were encountered while processing at the remote server.
        /// </exception>
        public SyncServerInfo GetServerInfo(SyncSession syncSession)
        {
            Func<IEnterpriseSynchronization, SyncSession, byte[], SyncServerInfo> callback = delegate(IEnterpriseSynchronization channelProxy, SyncSession tempSyncSession, byte[] tempCompressedContextFmBytes)
            {
                return channelProxy.GetServerInfo(tempSyncSession, tempCompressedContextFmBytes);
            };

            byte[] compressedContextFmBytes = CompressionProcessor.CompressObject(this._Context);

            return FMSyncChannelHelper.MakeCall<IEnterpriseSynchronization, SyncServerInfo>(this._SyncProviderFactory, (pChannelProxy) => callback(pChannelProxy, syncSession, compressedContextFmBytes));
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
                    this._SyncProviderFactory = null;
                }

                // Note disposing has been done.
                this._Disposed = true;
            }
        }
        #endregion IDisposable Interface Implementation
    }
}
