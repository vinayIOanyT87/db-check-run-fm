// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICompanyCrossReference.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ICompanyCrossReference type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// The Company Cross Reference Mapping interface.
	/// </summary>
	[ServiceContract]
	public interface ICompanyCrossReference
	{
		/// <summary>
		/// The get key name based on the reference name and cross reference type.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="referenceName">
		/// The reference name.
		/// </param>
		/// <param name="referenceType">
		/// The reference type index.
		/// </param>
		/// <returns>
		/// Key Name <see cref="string"/>.
		/// </returns>
		[OperationContract]
		string GetKeyName(SecurityClass security, string referenceName, CompanyCrossReferenceDO.CrossReferenceTypes referenceType);

		/// <summary>
		/// The get reference name based on the key name and cross reference type.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="keyName">
		/// The key name.
		/// </param>
		/// <param name="referenceType">
		/// The reference type.
		/// </param>
		/// <returns>
		/// Reference Name <see cref="string"/>.
		/// </returns>
		[OperationContract]
		string GetReferenceName(SecurityClass security, string keyName, CompanyCrossReferenceDO.CrossReferenceTypes referenceType);
	}
}
