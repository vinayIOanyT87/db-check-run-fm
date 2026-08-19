// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseBackupProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the DatabaseBackupProcessor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System.Data.SqlClient;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;

	using FMBusinessServices.DataAccessLayer;

	/// <summary>
	/// Service class for backing up FuelsManager database
	/// </summary>
	public class DatabaseBackupProcessor : IDatabaseBackupProcessor
	{
		/// <summary>
		/// Creates a backup of the FuelsManager consolidated database with the specified path name.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="backupFileName">The full path name of the backup file to create.</param>
		public void BackupConsolidatedDatabase(SecurityClass security, string backupFileName)
		{
			var consolidatedDa = new ConsolidatedDAClass();
			using (var command = new SqlCommand())
			{
				command.CommandText = @"BACKUP DATABASE ConsolidatedDB TO DISK ='" + backupFileName + "' WITH INIT, STATS=10";
				consolidatedDa.ExecuteQuery(security, command);
			}
		}
	}
}