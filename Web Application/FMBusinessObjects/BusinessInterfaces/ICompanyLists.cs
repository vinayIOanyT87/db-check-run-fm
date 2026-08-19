// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICompanyLists.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Interface for the CompanyLists service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessObjects.BusinessInterfaces
{
	using System.Collections;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Interface for the CompanyLists service class.
	/// </summary>
	[ServiceContract]
	public interface ICompanyLists
	{
		#region Public Methods and Operators

		/// <summary>
		/// Fills the MasterArrayList with companies.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="enumerateByGroupCompanies">if set to <c>true</c> [by group companies].</param>
		[OperationContract]
		void Enumerate(SecurityClass security, bool enumerateByGroupCompanies);

		/// <summary>
		/// Gets the company list.
		/// </summary>
		/// <param name="role">The role.</param>
		/// <returns>An array list of companies.</returns>
		[OperationContract]
		ArrayList GetCompanyList(COMPANY_ROLE role);

		#endregion
	}
}
