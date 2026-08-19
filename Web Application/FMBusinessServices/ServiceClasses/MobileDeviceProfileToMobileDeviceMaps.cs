// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MobileDeviceProfileToMobileDeviceMaps.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the MobileDeviceProfileToMobileDeviceMaps type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;

	using FMBusinessServices.DataAccessLayer;

	/// <summary>
	/// The purpose of this call is to expose methods for the client to add,
	/// delete, and retrieve profile to Mobile Device mapping data.
	/// </summary>
	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class MobileDeviceProfileToMobileDeviceMaps : IMobileDeviceProfileToMobileDeviceMaps
	{
		#region Private data members
		/// <summary>
		/// The consolidated da.
		/// </summary>
		private ConsolidatedDAClass consolidatedDa;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="MobileDeviceProfileToMobileDeviceMaps"/> class.
		/// </summary>
		public MobileDeviceProfileToMobileDeviceMaps( )
		{
			this.consolidatedDa = new ConsolidatedDAClass();
		}
		#endregion

		#region Public methods

		/// <summary>
		/// This method will add a profile to Mobile device mapping to the database.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="profileToMobileDeviceMap">
		/// The profile to Mobile device map.
		/// </param>
		/// <exception cref="ArgumentNullException">Invalid security or profileToMobileDeviceMap parameters
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Add(SecurityClass security, MobileDeviceProfileToMobileDeviceMapClass profileToMobileDeviceMap)
		{
			Guid newGuid = Guid.NewGuid( );

			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			if ( profileToMobileDeviceMap == null )
			{
				throw new ArgumentNullException("profileToMobileDeviceMap");
			}

			profileToMobileDeviceMap.MobileDeviceProfileToMobileDeviceGuid = newGuid;
			profileToMobileDeviceMap.CreatedDate						 = DateTimeOffset.Now;
			profileToMobileDeviceMap.CreatedBy							 = security.UserID;
			profileToMobileDeviceMap.UpdatedDate						 = profileToMobileDeviceMap.CreatedDate;
			profileToMobileDeviceMap.UpdatedBy							 = security.UserID;

			using ( var sqlCommand = new SqlCommand( ) )
			{
				profileToMobileDeviceMap.InsertSql(sqlCommand);
				this.consolidatedDa.ExecuteQuery(security, sqlCommand);
			}
		}

		/// <summary>
		/// This method will remove a profile to equipment mapping from the database.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="targetProfileGuid">
		/// The target profile guid.
		/// </param>
		/// <param name="targetMobileDeviceGuid">
		/// The target Mobile Device guid.
		/// </param>
		/// <exception cref="ArgumentNullException">Invalid parameters.
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid targetProfileGuid, Guid targetMobileDeviceGuid)
		{
			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			if ( targetProfileGuid == null )
			{
				throw new ArgumentNullException("targetProfileGuid");
			}

			if ( targetMobileDeviceGuid == null )
			{
				throw new ArgumentNullException("targetMobileDeviceGuid");
			}

			var profileToMobileDeviceMap = new MobileDeviceProfileToMobileDeviceMapClass { MobileDeviceProfileGuid = targetProfileGuid, AssignedToMobileDeviceGuid = targetMobileDeviceGuid };

			using ( var sqlCommand = new SqlCommand( ) )
			{
				profileToMobileDeviceMap.PurgeSql(sqlCommand);
				this.consolidatedDa.ExecuteQuery(security, sqlCommand);
			}
		}

		/// <summary>
		/// This method will remove all profiles to Mobile Devices mapping from the database
		/// based on the profile GUID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="targetProfileGuid">
		/// The target profile guid.
		/// </param>
		/// <exception cref="ArgumentNullException">Invalid security or target profile GUID.
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PurgeAllByProfileGuid(SecurityClass security, Guid targetProfileGuid)
		{
			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			if ( targetProfileGuid == null )
			{
				throw new ArgumentNullException("targetProfileGuid");
			}

			var profileToMobileDeviceMap = new MobileDeviceProfileToMobileDeviceMapClass { MobileDeviceProfileGuid = targetProfileGuid };

			using ( var sqlCommand = new SqlCommand( ) )
			{
				profileToMobileDeviceMap.PurgeAllByProfileGuidSql(sqlCommand);
				this.consolidatedDa.ExecuteQuery(security, sqlCommand);
			}
		}

		/// <summary>
		/// This method will remove all profiles to Mobile Devices mapping from the database
		/// based on the mobile device GUID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="targetMobileDeviceGuid">
		/// The target mobile device guid.
		/// </param>
		/// <exception cref="ArgumentNullException">Invalid or null parameters.
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PurgeAllByMobileDeviceGuid(SecurityClass security, Guid targetMobileDeviceGuid)
		{
			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			if ( targetMobileDeviceGuid == null )
			{
				throw new ArgumentNullException("targetMobileDeviceGuid");
			}

			var profileToMobileDeviceMap = new MobileDeviceProfileToMobileDeviceMapClass { AssignedToMobileDeviceGuid = targetMobileDeviceGuid };

			using ( var sqlCommand = new SqlCommand( ) )
			{
				profileToMobileDeviceMap.PurgeAllByMobileDeviceGuidSql(sqlCommand);
				this.consolidatedDa.ExecuteQuery(security, sqlCommand);
			}
		}

		/// <summary>
		/// The enumerate.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="mobileDeviceGuid">
		/// The mobile Device Guid.
		/// </param>
		/// <param name="inTransaction">
		/// The in Transaction.
		/// </param>
		/// <returns>
		/// The System.Data.DataSet.
		/// </returns>
		public MobileDeviceProfileToMobileDeviceMapCollection EnumerateMobileDeviceByMobileDeviceGuid(SecurityClass security, Guid mobileDeviceGuid, bool inTransaction)
		{
			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			var profileToMobileDeviceCollection = new MobileDeviceProfileToMobileDeviceMapCollection();
			var profileToMobileDeviceMap = new MobileDeviceProfileToMobileDeviceMapClass { AssignedToMobileDeviceGuid = mobileDeviceGuid };

			using ( var sqlCommand = new SqlCommand( ) )
			{
				profileToMobileDeviceMap.EnumerateMobileDeviceByMobileDeviceGuidSql(sqlCommand, inTransaction);
				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);

				if (dataSet != null && dataSet.Tables.Count > 0)
				{
					profileToMobileDeviceCollection.Load(dataSet);
				}
			}

			return profileToMobileDeviceCollection;
		}

		/// <summary>
		/// The enumerate mobile device by profile guid.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="profileGuid">
		/// The profile guid.
		/// </param>
		/// <param name="inTransaction">
		/// The in transaction.
		/// </param>
		/// <returns>
		/// The FMBusinessObjects.DataObjects.MobileDeviceProfileToMobileDeviceMapCollection.
		/// </returns>
		/// <exception cref="ArgumentNullException">Invalid security parameter.
		/// </exception>
		public MobileDeviceProfileToMobileDeviceMapCollection EnumerateMobileDeviceByProfileGuid(SecurityClass security, Guid profileGuid, bool inTransaction)
		{
			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			var profileToMobileDeviceCollection = new MobileDeviceProfileToMobileDeviceMapCollection( );
			var profileToMobileDeviceMap = new MobileDeviceProfileToMobileDeviceMapClass { MobileDeviceProfileGuid = profileGuid };

			using ( var sqlCommand = new SqlCommand( ) )
			{
				profileToMobileDeviceMap.EnumerateMobileDeviceByProfileGuidSql(sqlCommand, inTransaction);
				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);

				if ( dataSet != null && dataSet.Tables.Count > 0 )
				{
					profileToMobileDeviceCollection.Load(dataSet);
				}
			}

			return profileToMobileDeviceCollection;
		}

		/// <summary>
		/// This method will enumerate all profiles that are unassigned to the given mobile
		/// device Guid.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="mobileDeviceGuid">
		/// The mobile device guid.
		/// </param>
		/// <param name="inTransaction">
		/// The in transaction.
		/// </param>
		/// <returns>
		/// The FMBusinessObjects.DataObjects.MobileDeviceProfileToMobileDeviceMapCollection.
		/// </returns>
		/// <exception cref="ArgumentNullException">Invalid or null parameters.
		/// </exception>
		public MobileDeviceProfileToMobileDeviceMapCollection EnumerateUnassignedProfiles(SecurityClass security, Guid mobileDeviceGuid, bool inTransaction)
		{
			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			if ( mobileDeviceGuid == null )
			{
				throw new ArgumentNullException("mobileDeviceGuid");
			}

			var profileToMobileDeviceCollection = new MobileDeviceProfileToMobileDeviceMapCollection( );
			var profileToMobileDeviceMap = new MobileDeviceProfileToMobileDeviceMapClass { AssignedToMobileDeviceGuid = mobileDeviceGuid };

			using ( var sqlCommand = new SqlCommand( ) )
			{
				profileToMobileDeviceMap.EnumerateUnassignedProfilesSql(sqlCommand, security, security.SiteGuid, inTransaction);
				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);

				if ( dataSet != null && dataSet.Tables.Count > 0 )
				{
					profileToMobileDeviceCollection.Load(dataSet);
				}
			}

			return profileToMobileDeviceCollection;
		}

		/// <summary>
		/// This method will enumerate all mobile devices that are unassigned to the given profile
		/// Guid.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="profileGuid">
		/// The profile guid.
		/// </param>
		/// <param name="inTransaction">
		/// The in transaction.
		/// </param>
		/// <returns>
		/// The FMBusinessObjects.DataObjects.MobileDeviceProfileToMobileDeviceMapCollection.
		/// </returns>
		/// <exception cref="ArgumentNullException">Invalid or null parameters.
		/// </exception>
		public MobileDeviceProfileToMobileDeviceMapCollection EnumerateUnassignedMobileDevices(SecurityClass security, Guid profileGuid, bool inTransaction)
		{
			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			if ( profileGuid == null )
			{
				throw new ArgumentNullException("profileGuid");
			}

			var profileToMobileDeviceCollection = new MobileDeviceProfileToMobileDeviceMapCollection( );
			var profileToMobileDeviceMap = new MobileDeviceProfileToMobileDeviceMapClass { MobileDeviceProfileGuid = profileGuid };

			using ( var sqlCommand = new SqlCommand( ) )
			{
				profileToMobileDeviceMap.EnumerateUnassignedMobileDevicesSql(sqlCommand, security.SiteGuid, inTransaction);
				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);

				if ( dataSet != null && dataSet.Tables.Count > 0 )
				{
					profileToMobileDeviceCollection.Load(dataSet);
				}
			}

			return profileToMobileDeviceCollection;

		}
		#endregion
	}
}