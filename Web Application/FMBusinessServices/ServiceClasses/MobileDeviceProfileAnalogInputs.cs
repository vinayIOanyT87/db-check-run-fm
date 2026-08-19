// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MobileDeviceProfileAnalogInputs.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the MobileDeviceProfileAnalogInputs type.
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
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class MobileDeviceProfileAnalogInputs : IMobileDeviceProfileAnalogInputs
	{
		#region Private data members
		/// <summary>
		/// The consolidated da.
		/// </summary>
		private ConsolidatedDAClass consolidatedDa;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="MobileDeviceProfileAnalogInputs"/> class.
		/// </summary>
		public MobileDeviceProfileAnalogInputs ()
		{
			this.consolidatedDa = new ConsolidatedDAClass();
		}
		#endregion

		#region Public methods

		/// <summary>
		/// The add.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="analogInput">
		/// The analog input.
		/// </param>
		/// <returns>
		/// The System.Guid.
		/// </returns>
		/// <exception cref="ArgumentNullException">Must have valid parameters.
		/// </exception>
		[OperationBehavior ( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public Guid Add ( SecurityClass security, MobileDeviceProfileAnalogInput analogInput )
		{
			var newGuid = new Guid ( );

			if ( security == null )
			{
				throw new ArgumentNullException ( "security" );
			}

			if ( analogInput == null )
			{
				throw new ArgumentNullException ( "analogInput" );
			}

			analogInput.MobileDeviceProfileAnalogInputGuid	= newGuid;
			analogInput.CreatedDate							= DateTimeOffset.Now;
			analogInput.CreatedBy							= security.UserID;
			analogInput.UpdatedDate							= analogInput.CreatedDate;
			analogInput.UpdatedBy							= security.UserID;

			using ( SqlCommand sqlCommand = new SqlCommand ( ) )
			{
				analogInput.InsertSql ( sqlCommand );
				this.consolidatedDa.ExecuteQuery ( security, sqlCommand );
			}

			return newGuid;
		}

		/// <summary>
		/// The modify.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="analogInput">
		/// The analog input.
		/// </param>
		/// <exception cref="ArgumentNullException">Must have valid parameters.
		/// </exception>
		[OperationBehavior ( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Modify ( SecurityClass security, MobileDeviceProfileAnalogInput analogInput )
		{
			if ( security == null )
			{
				throw new ArgumentNullException ( "security" );
			}

			if ( analogInput == null )
			{
				throw new ArgumentNullException ( "analogInput" );
			}

			analogInput.CreatedDate = DateTimeOffset.Now;
			analogInput.CreatedBy	= security.UserID;
			analogInput.UpdatedDate = analogInput.CreatedDate;
			analogInput.UpdatedBy	= security.UserID;

			using ( SqlCommand sqlCommand = new SqlCommand ( ) )
			{
				analogInput.UpdateSql ( sqlCommand );

				if ( string.IsNullOrEmpty ( sqlCommand.CommandText ) == false )
				{
					this.consolidatedDa.ExecuteQuery ( security, sqlCommand );
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
		/// <exception cref="ArgumentNullException">
		/// Must have valid parameters.
		/// </exception>
		[OperationBehavior ( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Purge ( SecurityClass security, List<MobileDeviceProfileAnalogInput> deleteList )
		{
			if ( security == null )
			{
				throw new ArgumentNullException ( "security" );
			}

			if ( deleteList == null )
			{
				throw new ArgumentNullException("deleteList");
			}

			var analogInput = new MobileDeviceProfileAnalogInput();

			using ( var sqlcommand = new SqlCommand ( ) )
			{
				analogInput.PurgeSql ( sqlcommand, deleteList );

				if ( string.IsNullOrEmpty(sqlcommand.CommandText) == false )
				{
					this.consolidatedDa.ExecuteQuery(security, sqlcommand);
				}
			}
		}

		/// <summary>
		/// This method will purge all the analog inputs associated to a profile
		/// GUID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="profileGuid">
		/// The profile guid.
		/// </param>
		/// <exception cref="ArgumentNullException">Invalid parameters
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PurgeAll(SecurityClass security, Guid profileGuid)
		{
			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			if ( profileGuid == null || profileGuid == Guid.Empty )
			{
				throw new ArgumentNullException("profileGuid");
			}

			var analogInput = new MobileDeviceProfileAnalogInput { MobileDeviceProfileGuid = profileGuid };

			using ( var sqlcommand = new SqlCommand( ) )
			{
				analogInput.PurgeByProfileGuidSql(sqlcommand);

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
		/// <exception cref="ArgumentNullException">Must have valid parameters
		/// </exception>
		public DataSet EnumerateByProfileGuid ( SecurityClass security, Guid profileGuid )
		{
			DataSet dataSet;

			if ( security == null )
			{
				throw new ArgumentNullException ( "security" );
			}

			using ( SqlCommand sqlcommand = new SqlCommand ( ) )
			{
				var analogInput = new MobileDeviceProfileAnalogInput ( );
				analogInput.EnumerateByMobileDeviceProfileGuidSql ( sqlcommand, profileGuid );
				dataSet = this.consolidatedDa.GetDataSet ( sqlcommand, security );
			}

			return dataSet;
		}
		#endregion
	}
}