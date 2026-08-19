// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMobileDevices.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IMobileDevices type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// The purpose of this interface class is to provide an interface between the client and the
	/// service level for Mobile Device object.
	/// </summary>
	[ServiceContract]
	public interface IMobileDevices
	{
		/// <summary>
		/// Interface for the Add functionality
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="mobileDevice">
		/// The mobile Device.
		/// </param>
		/// <returns>
		/// Returns a newly inserted GUID.
		/// </returns>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		MobileDeviceClass Add(SecurityClass security, MobileDeviceClass mobileDevice);

		/// <summary>
		/// Interface for the Modify functionality
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="mobileDevice">
		/// The mobile Device.
		/// </param>
		/// <returns>
		/// The FMBusinessObjects.DataObjects.MobileDeviceProfile.
		/// </returns>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		MobileDeviceClass Modify(SecurityClass security, MobileDeviceClass mobileDevice);

		/// <summary>
		/// Interface for the Purge functionality
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="mobileDeviceGuid">
		/// The mobile Device Guid.
		/// </param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid mobileDeviceGuid);

		/// <summary>
		/// Interface for the get profile by ID functionality
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="mobileDeviceId">
		/// The mobile Device Id.
		/// </param>
		/// <returns>
		/// Returns one Mobile device based on the Mobile Device ID.
		/// </returns>
		[OperationContract]
		MobileDeviceClass GetByMobileDeviceId(SecurityClass security, string mobileDeviceId);

		/// <summary>
		/// Interface for the get Mobile Device by GUID functionality
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="mobileDeviceGuid">
		/// The mobile Device Guid.
		/// </param>
		/// <returns>
		/// Returns one Mobile device based on the Mobile device GUID.
		/// </returns>
		[OperationContract]
		MobileDeviceClass GetByMobileDeviceGuid(SecurityClass security, Guid mobileDeviceGuid);

		/// <summary>
		/// Interface for the get GUID by Mobile Device ID functionality
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="mobileDeviceId">
		/// The mobile Device Id.
		/// </param>
		/// <returns>
		/// Returns the GUID based on a Mobile Device ID.
		/// </returns>
		[OperationContract]
		Guid GetGuid(SecurityClass security, string mobileDeviceId);

		/// <summary>
		/// Interface for the getting Mobile Devices functionality
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>Returns a dataset with all Mobile Devices.
		/// </returns>
		[OperationContract]
		MobileDeviceCollection EnumerateAll(SecurityClass security);

		/// <summary>
		/// Interface for the getting Mobile Devices functionality
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="findFilter">
		/// The find Filter.
		/// </param>
		/// <returns>
		/// Returns a dataset with all Mobile Devices.
		/// </returns>
		[OperationContract]
		MobileDeviceCollection EnumerateByFindFilter(SecurityClass security, string findFilter);

		/// <summary>
		/// The is mobile device unique.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="mobileDeviceId">
		/// The mobile device id.
		/// </param>
		/// <returns>
		/// The System.Boolean.
		/// </returns>
		[OperationContract]
		bool IsMobileDeviceUnique(SecurityClass security, string mobileDeviceId);
	}
}
