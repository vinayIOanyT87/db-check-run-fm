// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnterpriseSynchronization.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Defines the EnterpriseSynchronization Proxy Service type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.ServiceModel;
	using System.ServiceModel.Activation;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	using Microsoft.Synchronization.Data;

	/// <summary>
	/// The enterprise synchronization.
	/// </summary>
	[ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class EnterpriseSynchronization : IEnterpriseSynchronization
	{
		#region Service Interface Implementation - IEnterpriseSynchronization

		/// <summary>
		/// The create session.
		/// </summary>
		/// <param name="syncLoginRequest">
		/// The sync login request.
		/// </param>
		/// <param name="securityContext">
		/// The security context.
		/// </param>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <returns>
		/// The <see cref="SYNCSERVICESTATE"/>.
		/// </returns>
		/// <exception cref="Exception">
		/// Rethrows FaultException from application tier.
		/// </exception>
		public SYNCSERVICESTATE CreateSession(SecuritySyncLoginRequest syncLoginRequest, out SecurityClass securityContext, out string message)
		{
			try
			{
				SecurityClass remoteSecurityContext = null;
				string remoteMessage = string.Empty;

				SYNCSERVICESTATE serviceState =
					FMChannelHelper.MakeCall<IEnterpriseSynchronization, SYNCSERVICESTATE>((x) => x.CreateSession(syncLoginRequest, out remoteSecurityContext, out remoteMessage));

				securityContext = remoteSecurityContext;
				message = remoteMessage;

				return serviceState;
			}
			catch (FaultException)
			{
				throw;
			}
		}

		/// <summary>
		/// The end session.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="syncSessionID">Synchronization Session ID associated with the FuelsManager Session that should be terminated.</param>
		/// <param name="finalSessionStatus">
		/// The final session status.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		/// <exception cref="FaultException">
		/// Rethrows FaultException from application tier.
		/// </exception>
		public string EndSession(SecurityClass security, Guid syncSessionID, SYNCSESSIONSTATUS finalSessionStatus)
		{
			try
			{
				return
					FMChannelHelper.MakeCall<IEnterpriseSynchronization, string>(
							(x) => x.EndSession(security, syncSessionID, finalSessionStatus));
			}
			catch (FaultException)
			{
				throw;
			}
		}

		/// <summary>
		/// The get server id.
		/// </summary>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		/// <exception cref="FaultException">
		/// Rethrows FaultException from application tier.
		/// </exception>
		public Guid GetServerID()
		{
			try
			{
				return FMChannelHelper.MakeCall<IEnterpriseSynchronization, Guid>((x) => x.GetServerID());
			}
			catch (FaultException)
			{
				throw;
			}
		}

		/// <summary>
		/// The get node name.
		/// </summary>
		/// <returns>
		/// The name of the synchronization node.
		/// </returns>
		/// <exception cref="FaultException">
		/// Rethrows FaultException from application tier.
		/// </exception>
		public string GetNodeName()
		{
			try
			{
				return
					FMChannelHelper.MakeCall<IEnterpriseSynchronization, string>(
							(x) => x.GetNodeName());
			}
			catch (FaultException)
			{
				throw;
			}
		}

		/// <summary>
		/// The get synchronization site list.
		/// </summary>
		/// <param name="remoteSiteList">
		/// The remote site list.
		/// </param>
		/// <param name="syncContext">
		/// The sync context.
		/// </param>
		/// <returns>
		/// The <see cref="SiteSyncList"/>.
		/// </returns>
		/// <exception cref="FaultException">
		/// Rethrows FaultException from application tier.
		/// </exception>
		public SiteSyncList GetSynchronizationSiteList(SiteSyncList remoteSiteList, SyncContextFM syncContext)
		{
			try
			{
				return FMChannelHelper.MakeCall<IEnterpriseSynchronization, SiteSyncList>((x) => x.GetSynchronizationSiteList(remoteSiteList, syncContext));
			}
			catch (FaultException)
			{
				throw;
			}
		}

		/// <summary>
		/// Gets the current enterprise synchronization anchor.
		/// </summary>
		/// <returns>
		/// The current Enterprise Sync Anchor
		/// </returns>
		/// <exception cref="FaultException">
		/// Rethrows FaultException from application tier.
		/// </exception>
		public long GetEnterpriseSyncAnchor()
		{
			try
			{
				return
					FMChannelHelper.MakeCall<IEnterpriseSynchronization, long>(
							(x) => x.GetEnterpriseSyncAnchor());
			}
			catch (FaultException)
			{
				throw;
			}
		}

		/// <summary>
		/// The apply changes.
		/// </summary>
		/// <param name="groupMetadata">
		/// The group meta data.
		/// </param>
		/// <param name="dataSetSurrogateBytes">
		/// The data set.
		/// </param>
		/// <param name="syncSession">
		/// The sync session.
		/// </param>
		/// <param name="syncContextFMBytes">
		/// A serialized compressed byte array that contains an instance of the <see cref="SyncContextFM"/> object which provides FuelsManager synchronization attributes.
		/// </param>
		/// <returns>
		/// The <see cref="SyncContext"/>.
		/// </returns>
		/// <exception cref="FaultException">
		/// Rethrows FaultException from application tier.
		/// </exception>
		public SyncContext ApplyChanges(SyncGroupMetadata groupMetadata, byte[] dataSetSurrogateBytes, SyncSession syncSession, byte[] syncContextFMBytes)
		{
			try
			{
				return
					FMChannelHelper.MakeCall<IEnterpriseSynchronization, SyncContext>(
							(x) => x.ApplyChanges(groupMetadata, dataSetSurrogateBytes, syncSession, syncContextFMBytes));
			}
			catch (FaultException)
			{
				throw;
			}
		}

		/// <summary>
		/// The get changes.
		/// </summary>
		/// <param name="groupMetadata">
		/// The group meta data.
		/// </param>
		/// <param name="syncSession">
		/// The sync session.
		/// </param>
		/// <param name="syncContextFMBytes">
		/// A serialized compressed byte array that contains an instance of the <see cref="SyncContextFM"/> object which provides FuelsManager synchronization attributes.
		/// </param>
		/// <param name="dataSetSurrogateBytes">
		/// Dataset as a byte array.
		/// </param>
		/// <returns>
		/// The <see cref="SyncContext"/>.
		/// </returns>
		/// <exception cref="FaultException">
		/// Rethrows FaultException from application tier.
		/// </exception>
		public SyncContext GetChanges(SyncGroupMetadata groupMetadata, SyncSession syncSession, byte[] syncContextFMBytes, out byte[] dataSetSurrogateBytes)
		{
			try
			{
				byte[] remoteDataSetSurrogateBytes = null;

				SyncContext context =
					FMChannelHelper.MakeCall<IEnterpriseSynchronization, SyncContext>(
							(x) =>
							x.GetChanges(groupMetadata, syncSession, syncContextFMBytes, out remoteDataSetSurrogateBytes));

				dataSetSurrogateBytes = remoteDataSetSurrogateBytes;

				return context;
			}
			catch (FaultException)
			{
				throw;
			}
		}

		/// <summary>
		/// The get schema.
		/// </summary>
		/// <param name="tableNames">
		/// The table names.
		/// </param>
		/// <param name="syncSession">
		/// The sync session.
		/// </param>
		/// <param name="syncContextFMBytes">
		/// A serialized compressed byte array that contains an instance of the <see cref="SyncContextFM"/> object which provides FuelsManager synchronization attributes.
		/// </param>
		/// <returns>
		/// The <see cref="SyncSchema"/>.
		/// </returns>
		/// <exception cref="FaultException">
		/// Rethrows FaultException from application tier.
		/// </exception>
		public SyncSchema GetSchema(Collection<string> tableNames, SyncSession syncSession, byte[] syncContextFMBytes)
		{
			try
			{
				return
					FMChannelHelper.MakeCall<IEnterpriseSynchronization, SyncSchema>(
							(x) => x.GetSchema(tableNames, syncSession, syncContextFMBytes));
			}
			catch (FaultException)
			{
				throw;
			}
		}

		/// <summary>
		/// The get server info.
		/// </summary>
		/// <param name="syncSession">
		/// The sync session.
		/// </param>
		/// <param name="syncContextFMBytes">
		/// A serialized compressed byte array that contains an instance of the <see cref="SyncContextFM"/> object which provides FuelsManager synchronization attributes.
		/// </param>
		/// <returns>
		/// The <see cref="SyncServerInfo"/>.
		/// </returns>
		/// <exception cref="FaultException">
		/// Rethrows FaultException from application tier.
		/// </exception>
		public SyncServerInfo GetServerInfo(SyncSession syncSession, byte[] syncContextFMBytes)
		{
			try
			{
				return
					FMChannelHelper.MakeCall<IEnterpriseSynchronization, SyncServerInfo>(
							(x) => x.GetServerInfo(syncSession, syncContextFMBytes));
			}
			catch (FaultException)
			{
				throw;
			}
		}

		/// <summary>
		/// Reprocesses the conflicts.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="syncNodeGuid"></param>
		/// <param name="sessionToken"></param>
		public SyncConflictResolutionStatus ReprocessConflicts(SecurityClass security, Guid syncNodeGuid, Guid sessionToken, SyncConflictResolutionStatus syncConflictResolutionStatus)
		{
			try
			{
				return FMChannelHelper.MakeCall<IEnterpriseSynchronization, SyncConflictResolutionStatus>(
					(x) => x.ReprocessConflicts(security, syncNodeGuid, sessionToken, syncConflictResolutionStatus));
			}
			catch (FaultException)
			{
				throw;
			}
		}

		public void PurgeLogs(SecurityClass security, Guid syncNodeGuid, int maximumDaysToRetainLogs)
		{
			try
			{
				FMChannelHelper.MakeCall<IEnterpriseSynchronization>(
					(x) => x.PurgeLogs(security, syncNodeGuid, maximumDaysToRetainLogs));
			}
			catch (FaultException)
			{
				throw;
			}

		}

		public void SynchronizeValueArchive(SecurityClass security, List<ArchiveDataElement> archiveDataElementList)
		{
			try
			{
				FMChannelHelper.MakeCall<IEnterpriseSynchronization>(
					(x) =>
					{
						((IClientChannel)x).OperationTimeout = new TimeSpan(0, 10, 0);
						x.SynchronizeValueArchive(security, archiveDataElementList);
					});
			}
			catch (FaultException)
			{
				throw;
			}

		}

		public void SynchronizeAlarmAndEventArchive(SecurityClass security, List<AandEDataElement> archiveDataElementList)
		{
			try
			{
				FMChannelHelper.MakeCall<IEnterpriseSynchronization>(
					(x) =>
					{
						((IClientChannel)x).OperationTimeout = new TimeSpan(0, 10, 0);
						x.SynchronizeAlarmAndEventArchive(security, archiveDataElementList);
					});
			}
			catch (FaultException)
			{
				throw;
			}

		}



		#endregion Service Interface Implementation - IEnterpriseSynchronization

	}
}
