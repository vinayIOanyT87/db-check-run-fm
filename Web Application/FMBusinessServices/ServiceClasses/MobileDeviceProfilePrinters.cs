// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MobileDeviceProfilePrinters.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the MobileDeviceProfilePrinters type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;

	using FMBusinessServices.DataAccessLayer;

	/// <summary>
	/// The mobile device profile analog inputs.
	/// </summary>
	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class MobileDeviceProfilePrinters : IMobileDeviceProfilePrinters
	{
		#region Private data members
		/// <summary>
		/// The consolidated da.
		/// </summary>
		private ConsolidatedDAClass consolidatedDa;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="MobileDeviceProfilePrinters"/> class.
		/// </summary>
		public MobileDeviceProfilePrinters ()
		{
			this.consolidatedDa = new ConsolidatedDAClass();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// The add.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="printer">
		/// The printer.
		/// </param>
		/// <returns>
		/// The System.Guid.
		/// </returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, MobileDeviceProfilePrinter printer)
		{
			bool duplicatePrinterId = false;
			var newGuid = new Guid( );

			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			if ( printer == null )
			{
				throw new ArgumentNullException("printer");
			}

			printer.MobileDeviceProfilePrinterGuid  = newGuid;
			printer.CreatedDate						= DateTimeOffset.Now;
			printer.CreatedBy						= security.UserID;
			printer.UpdatedDate						= printer.CreatedDate;
			printer.UpdatedBy						= security.UserID;

			using ( var sqlCommand = new SqlCommand( ) )
			{
				printer.CheckForDuplicatePrinterIDs(sqlCommand);

				if ( string.IsNullOrEmpty(sqlCommand.CommandText) == false )
				{
					DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);
					duplicatePrinterId = printer.DuplicatePrinterId(dataSet);
				}
			}

			// Cannot have duplicate Printer IDs for the same Profile.
			if ( duplicatePrinterId )
			{
				string errMsg = "Printer ID '" + printer.PrinterId + "' already exists for this profile.";
				throw new Exception(errMsg);
			}

			using (var sqlCommand = new SqlCommand())
			{
				printer.InsertSql(sqlCommand);
				this.consolidatedDa.ExecuteQuery(security, sqlCommand);
			}

			return newGuid;
		}

		/// <summary>
		/// The modify.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="printer">
		/// The printer.
		/// </param>
		[OperationBehavior ( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Modify ( SecurityClass security, MobileDeviceProfilePrinter printer )
		{
			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			if ( printer == null )
			{
				throw new ArgumentNullException("printer");
			}

			printer.CreatedDate = DateTimeOffset.Now;
			printer.CreatedBy	= security.UserID;
			printer.UpdatedDate = printer.CreatedDate;
			printer.UpdatedBy	= security.UserID;

			using ( var sqlCommand = new SqlCommand( ) )
			{
				printer.UpdateSql(sqlCommand);

				if ( string.IsNullOrEmpty(sqlCommand.CommandText) == false )
				{
					this.consolidatedDa.ExecuteQuery(security, sqlCommand);
				}
			}		
		}

		/// <summary>
		/// The purge.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="deleteList">
		/// The delete List.
		/// </param>
		[OperationBehavior ( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Purge ( SecurityClass security, List<MobileDeviceProfilePrinter> deleteList )
		{
			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			if ( deleteList == null )
			{
				throw new ArgumentNullException("deleteList");
			}

			var printer = new MobileDeviceProfilePrinter( );

			using ( var sqlcommand = new SqlCommand( ) )
			{
				printer.PurgeSql(sqlcommand, deleteList);

				if ( string.IsNullOrEmpty(sqlcommand.CommandText) == false )
				{
					this.consolidatedDa.ExecuteQuery(security, sqlcommand);
				}
			}		
		}

		/// <summary>
		/// This method will purge all printer configuration for a given profile
		/// GUID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="printerGuid">
		/// The printer guid.
		/// </param>
		/// <exception cref="ArgumentNullException">Invalid parameters
		/// </exception>
		public void PurgeAll(SecurityClass security, Guid printerGuid)
		{
			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			if ( printerGuid == null || printerGuid == Guid.Empty )
			{
				throw new ArgumentNullException("printerGuid");
			}

			var printer = new MobileDeviceProfilePrinter { MobileDeviceProfileGuid = printerGuid };

			using ( var sqlcommand = new SqlCommand( ) )
			{
				printer.PurgeByProfileGuidSql(sqlcommand);

				if ( string.IsNullOrEmpty(sqlcommand.CommandText) == false )
				{
					this.consolidatedDa.ExecuteQuery(security, sqlcommand);
				}
			}
		}

		/// <summary>
		/// The enumerate by profile guid.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="profileGuid">
		/// The profile guid.
		/// </param>
		/// <returns>
		/// The System.Data.DataSet.
		/// </returns>
		public DataSet EnumerateByProfileGuid ( SecurityClass security, Guid profileGuid )
		{
			DataSet dataSet;

			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			using ( var sqlcommand = new SqlCommand( ) )
			{
				var printer = new MobileDeviceProfilePrinter( );
				printer.EnumerateByMobileDeviceProfileGuidSql(sqlcommand, profileGuid);
				dataSet = this.consolidatedDa.GetDataSet(sqlcommand, security);
			}

			return dataSet;
		}
		#endregion
	}
}