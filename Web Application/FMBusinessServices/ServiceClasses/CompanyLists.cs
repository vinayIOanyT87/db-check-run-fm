// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanyLists.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   This class is responsible for collating a list of Companies into arrays
//   suitable for using as DataSources.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// This class is responsible for collating a list of Companies into arrays
	/// suitable for using as DataSources.  
	/// </summary>
	public class CompanyLists : ICompanyLists
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="CompanyLists"/> class. 
		/// </summary>
		public CompanyLists ( )
		{
			this.Reset();
		}
		#endregion

		/// <summary>
		/// Gets the master array list.
		/// </summary>
		/// <value>
		/// The master array list.
		/// </value>
		public Array MasterArrayList { get; private set; }

		#region Public Methods
		/// <summary>
		/// Resets this instance.
		/// </summary>
		public void Reset ( )
		{
			// Get the current size of the COMPANY_ROLE enum
			int companyRoleSize = Enum.GetValues(typeof(COMPANY_ROLE)).Length;

			// Create the master array of array lists
			this.MasterArrayList = new ArrayList[companyRoleSize];

			// Now initialize the data members
			for (int index = 0; index < companyRoleSize; ++index)
			{
				this.MasterArrayList.SetValue ( new ArrayList ( ), index );
			}
		}

		/// <summary>
		/// Fills the MasterArrayList with companies.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="enumerateByGroupCompanies">if set to <c>true</c> [by group companies].</param>
		public void Enumerate ( SecurityClass security, bool enumerateByGroupCompanies )
		{
			this.CheckSecurity ( security );

			CompaniesClass companies = new CompaniesClass();
			CompanyCollectionClass companyCollection = companies.EnumerateAllRoles ( security, enumerateByGroupCompanies );

			foreach (CompanyClass company in companyCollection)
			{
				// There should be only one role
				COMPANY_ROLE role = company.RoleCollection[0].Role;

				// Add the company to the proper arraylist
				var arrayList = this.MasterArrayList.GetValue((int)role) as ArrayList;
				if (arrayList != null)
				{
					arrayList.Add ( company.ID );
				}
			}
		}

		/// <summary>
		/// Gets the company list.
		/// </summary>
		/// <param name="role">The role.</param>
		/// <returns>An array list of companies.</returns>
		public ArrayList GetCompanyList ( COMPANY_ROLE role )
		{
			return this.MasterArrayList.GetValue ( (int) role ) as ArrayList;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// Checks the security object.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <exception cref="System.ArgumentNullException">security</exception>
		private void CheckSecurity ( SecurityClass security )
		{
			if (security == null)
			{
				throw new ArgumentNullException ( "security" );
			}
		}
		#endregion
	}
}