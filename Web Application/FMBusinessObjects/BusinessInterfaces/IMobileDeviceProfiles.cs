// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMobileDeviceProfiles.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IMobileDeviceProfiles type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Data;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// The purpose of this interface class is to provide an interface between the client and the
	/// service level for Mobile Device Profile object.
	/// </summary>
	[ServiceContract]
	public interface IMobileDeviceProfiles
	{
		/// <summary>
		/// Interface for the Add functionality
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="mobileDeviceProfile">
		/// The mobile device profile.
		/// </param>
		/// <returns>Returns a newly inserted GUID.
		/// </returns>
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		MobileDeviceProfile Add ( SecurityClass security, MobileDeviceProfile mobileDeviceProfile );

		/// <summary>
		/// Interface for the Modify functionality
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="mobileDeviceProfile">
		/// The mobile device profile.
		/// </param>
		/// <returns>
		/// The FMBusinessObjects.DataObjects.MobileDeviceProfile.
		/// </returns>
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		MobileDeviceProfile Modify ( SecurityClass security, MobileDeviceProfile mobileDeviceProfile );

		/// <summary>
		/// Interface for the Purge functionality
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="profileGuid">
		/// The profile guid.
		/// </param>
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge ( SecurityClass security, Guid profileGuid );

		/// <summary>
		/// Interface for the get profile by ID functionality
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="profileId">
		/// The profile id.
		/// </param>
		/// <returns>Returns one mobile device profile based on the profile ID.
		/// </returns>
		[OperationContract]
		MobileDeviceProfile GetByProfileId ( SecurityClass security, string profileId );

		/// <summary>
		/// Interface for the get profile by GUID functionality
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="profileGuid">
		/// The profile guid.
		/// </param>
		/// <returns>Returns one mobile device profile based on the profile GUID.
		/// </returns>
		[OperationContract]
		MobileDeviceProfile GetByProfileGuid ( SecurityClass security, Guid profileGuid );

		/// <summary>
		/// Interface for the get GUID by profile ID functionality
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="profileId">
		/// The profile id.
		/// </param>
		/// <returns>Returns the GUID based on a profile ID.
		/// </returns>
		[OperationContract]
		Guid GetGuid ( SecurityClass security, string profileId );

		/// <summary>
		/// Interface for the getting profiles functionality
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>Returns a dataset with all profiles.
		/// </returns>
		[OperationContract]
		DataSet EnumerateAll ( SecurityClass security );

		/// <summary>
		/// Interface for the getting profiles functionality
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="findFilter">
		/// The find Filter.
		/// </param>
		/// <returns>
		/// Returns a dataset with all profiles.
		/// </returns>
		[OperationContract]
		DataSet EnumerateByFindFilter(SecurityClass security, string findFilter);
	}
}
