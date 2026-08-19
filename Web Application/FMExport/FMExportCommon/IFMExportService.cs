// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFMExportService.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines operations supported by the FMExportService
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMExportService
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.ServiceModel;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	/// <summary>
	/// Defines operations supported by the FMExportService
	/// </summary>
	[ServiceContract]
	public interface IFMExportService
	{
		/// <summary>
		/// Get a list of interfaces that are supported for exporting data
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <returns>A list of interfaces that are supported for exporting data</returns>
		[OperationContract]
		List<string> GetSupportedInterfaceIDs(SecurityClass security);

		/// <summary>
		/// Gets a list of ExportRequestClass objects from the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <returns>The specified list of ExportRequestClass objects</returns>
		[OperationContract]
		List<ExportRequestClass> GetRequests(SecurityClass security);

		/// <summary>
		/// Gets a table of in-memory data from the database.  Executes the
		/// specified SQL command and returns the resultant DataTable.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="cmd">The serializable SQL Command</param>
		/// <returns>The DataTable containing the results of the specified SQL command</returns>
		[OperationContract]
		DataTable GetDataTable(SecurityClass security, SerializableSqlCommand cmd);

		/// <summary>
		/// Adds an ExportRequestClass object to the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="exportRequest">The object to add to the database</param>
		[OperationContract]
		void Add(SecurityClass security, ExportRequestClass exportRequest);

		/// <summary>
		/// Modifies an existing ExportRequestClass object in the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="exportRequest">The object to modify in the database</param>
		[OperationContract]
		void Update(SecurityClass security, ExportRequestClass exportRequest);

		/// <summary>
		/// Deletes an existing ExportRequestClass object from the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="identityGuid">Identifies the object to delete in the database</param>
		[OperationContract]
		void Delete(SecurityClass security, Guid identityGuid);

		/// <summary>
		/// Gets an existing ExportRequestClass object from the database given the Identity Guid (ExportRequestGuid)
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="identityGuid">The identity guid identifying the ExportRequestClass record</param>
		/// <returns>The specified ExportRequestClass record</returns>
		[OperationContract]
		ExportRequestClass Get(SecurityClass security, Guid identityGuid);

		/// <summary>
		/// Gets an existing ExportRequestClass object from the database given the ID.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="id">The ID of the ExportRequestClass object</param>
		/// <returns>The specified ExportRequestClass object</returns>
		[OperationContract]
		ExportRequestClass GetRequestById(SecurityClass security, string id);

		/// <summary>
		/// Login to FuelsManager with the provided credentials
		/// </summary>
		/// <param name="changePasswordParam">Whether or not the user has to change their password</param>
		/// <param name="daysUntilExpirationParam">The number of days until the password expires</param>
		/// <param name="securityParam">The security object used to interact with FuelsManager</param>
		/// <param name="securityLoginRequest">The login request with information like the user name and password</param>
		/// <returns>A string with any information about why the login might not have been successful</returns>
		[OperationContract]
		string Login(out bool changePasswordParam, out int daysUntilExpirationParam, out SecurityClass securityParam, SecurityLoginRequest securityLoginRequest);

		/// <summary>
		/// Log the user out of FuelsManager
		/// </summary>
		/// <param name="security">Contains information identifying the user to logout</param>
		[OperationContract]
		void Logout(SecurityClass security);

        [OperationContract]
        List<FMAETranslation> EnumerateFMAETranslations(SecurityClass security, FMAETranslationType translationType);

        [OperationContract]
        List<string> GetSupportedWebServicePluginIDs(SecurityClass Security);

    }
}
