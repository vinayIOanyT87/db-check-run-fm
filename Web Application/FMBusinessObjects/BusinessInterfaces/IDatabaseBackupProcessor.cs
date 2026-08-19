// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDatabaseBackupProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IDatabaseBackupProcessor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Service interface for backing up FuelsManager database
	/// </summary>
	[ServiceContract]
	public interface IDatabaseBackupProcessor
	{
		/// <summary>
		/// Creates a backup of the FuelsManager consolidated database with the specified path name.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="backupFileName">The full path name of the backup file to create.</param>
		[OperationContract]
		void BackupConsolidatedDatabase(SecurityClass security, string backupFileName);
	}
}