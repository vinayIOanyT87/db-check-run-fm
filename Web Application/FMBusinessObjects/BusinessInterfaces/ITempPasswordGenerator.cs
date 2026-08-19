// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITempPasswordGenerator.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   This interface defines the methods used in the Forgotten Password and Password Hint funtionality
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	/// <summary>
	///   This interface defines the methods used in the Forgotten Password and Password Hint funtionality
	/// </summary>
	[ServiceContract]
	public interface ITempPasswordGenerator
	{
		#region Public Methods and Operators

		/// <summary>
		/// This method defines a new temporary password for a user.
		/// </summary>
		/// <returns>
		/// A new temporary password.
		/// </returns>
		[OperationContract]
		string GenerateTemporaryPassword(SecurityClass security);

		/// <summary>
		/// This method retrieves the password hint for a user.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="loginRequest">The login request object.</param>
		/// <returns>
		/// The password hint configured for the user, if any.
		/// </returns>
		[OperationContract]
		string GetPasswordHint(SecurityClass security, SecurityLoginRequest loginRequest);

		#endregion
	}
}