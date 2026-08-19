// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMobileDeviceProfileToMobileDeviceMaps.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IMobileDeviceProfileToMobileDeviceMaps type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// The purpose of this interface class is to provide an interface between the client and the
	/// service level for Mobile Device Profile to Mobile Device Map object.
	/// </summary>
	[ServiceContract]
	public interface IMobileDeviceProfileToMobileDeviceMaps
	{
		/// <summary>
		/// Interface for the Add functionality
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="profileToMobileDeviceMap">
		/// The profile To Mobile device Map.
		/// </param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Add(SecurityClass security, MobileDeviceProfileToMobileDeviceMapClass profileToMobileDeviceMap);

		/// <summary>
		/// Interface for the Purge functionality
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="targetProfileGuid">
		/// The target Profile Guid.
		/// </param>
		/// <param name="targetMobileDeviceGuid">
		/// The target mobile device GUID
		/// </param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid targetProfileGuid, Guid targetMobileDeviceGuid);

		/// <summary>
		/// Interface for the Purge all by profile GUID functionality
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="targetProfileGuid">
		/// The target profile guid.
		/// </param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void PurgeAllByProfileGuid(SecurityClass security, Guid targetProfileGuid);

		/// <summary>
		/// The purge all by mobile device guid.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="targetMobileDeviceGuid">
		/// The target mobile device guid.
		/// </param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void PurgeAllByMobileDeviceGuid(SecurityClass security, Guid targetMobileDeviceGuid);

		/// <summary>
		/// Interface for the getting profiles functionality
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="profileGuid">
		/// The profile Guid.
		/// </param>
		/// <param name="inTransaction">
		/// The in Transaction.
		/// </param>
		/// <returns>
		/// Returns a dataset with all profiles.
		/// </returns>
		[OperationContract]
		MobileDeviceProfileToMobileDeviceMapCollection EnumerateMobileDeviceByProfileGuid(SecurityClass security, Guid profileGuid, bool inTransaction);

		/// <summary>
		/// The enumerate mobile device by mobile device guid.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="mobileDeviceGuid">
		/// The mobile device guid.
		/// </param>
		/// <param name="inTransaction">
		/// The in transaction.
		/// </param>
		/// <returns>
		/// The FMBusinessObjects.DataObjects.MobileDeviceProfileToMobileDeviceMapCollection.
		/// </returns>
		[OperationContract]
		MobileDeviceProfileToMobileDeviceMapCollection EnumerateMobileDeviceByMobileDeviceGuid(SecurityClass security, Guid mobileDeviceGuid, bool inTransaction);

		/// <summary>
		/// The enumerate unassigned profiles.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="mobileDeviceGuid">
		/// The mobile device guid.
		/// </param>
		/// <param name="inTransaction">
		/// The in transaction.
		/// </param>
		/// <returns>
		/// The FMBusinessObjects.DataObjects.MobileDeviceProfileToMobileDeviceMapCollection.
		/// </returns>
		[OperationContract]
		MobileDeviceProfileToMobileDeviceMapCollection EnumerateUnassignedProfiles(SecurityClass security, Guid mobileDeviceGuid, bool inTransaction);

		/// <summary>
		/// The enumerate unassigned mobile devices.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="profileGuid">
		/// The profile guid.
		/// </param>
		/// <param name="inTransaction">
		/// The in transaction.
		/// </param>
		/// <returns>
		/// The FMBusinessObjects.DataObjects.MobileDeviceProfileToMobileDeviceMapCollection.
		/// </returns>
		[OperationContract]
		MobileDeviceProfileToMobileDeviceMapCollection EnumerateUnassignedMobileDevices(SecurityClass security, Guid profileGuid, bool inTransaction);
	}
}
