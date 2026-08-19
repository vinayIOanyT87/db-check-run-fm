// --------------------------------------------------------------------------------------------------------------------
// <copyright file="mobileDevices.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the mobileDevices type.
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
	/// The purpose of this call is to expose methods for the client to retrieve, add,
	/// delete, and modify mobile Device data.
	/// </summary>
	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class MobileDevices : IMobileDevices
	{
		#region Private data members
		/// <summary>
		/// The consolidated da.
		/// </summary>
		private ConsolidatedDAClass consolidatedDa;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="MobileDevices"/> class.
		/// </summary>
		public MobileDevices( )
		{
			this.consolidatedDa = new ConsolidatedDAClass();
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will add an Mobile Device to the database.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="mobileDevice">
		/// The Mobile Device.
		/// </param>
		/// <returns>
		/// Returns the an updated Mobile Device for the record.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Must have a none null sercurity and Mobile device object.
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public MobileDeviceClass Add(SecurityClass security, MobileDeviceClass mobileDevice)
		{
			Guid newGuid = Guid.NewGuid( );

			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			if ( mobileDevice == null )
			{
				throw new ArgumentNullException("mobileDevice");
			}

			mobileDevice.MobileDeviceGuid	= newGuid;
			mobileDevice.SiteGuid			= security.SiteGuid;
			mobileDevice.CreatedDate		= DateTimeOffset.Now;
			mobileDevice.CreatedBy			= security.UserID;
			mobileDevice.UpdatedDate		= mobileDevice.CreatedDate;
			mobileDevice.UpdatedBy			= security.UserID;

			using ( var sqlCommand = new SqlCommand( ) )
			{
				mobileDevice.InsertSql(sqlCommand);
				this.consolidatedDa.ExecuteQuery(security, sqlCommand);

				var profileToMobileDeviceMaps = new MobileDeviceProfileToMobileDeviceMaps( );

				foreach ( MobileDeviceProfileToMobileDeviceMapClass newProfileToMobileDeviceMap in mobileDevice.AssignedProfileCollection )
				{
					if ( newProfileToMobileDeviceMap.MobileDeviceProfileToMobileDeviceGuid == Guid.Empty )
					{
						newProfileToMobileDeviceMap.AssignedToMobileDeviceGuid = mobileDevice.MobileDeviceGuid;
						profileToMobileDeviceMaps.Add(security, newProfileToMobileDeviceMap);
					}
				}
			}

			mobileDevice = this.GetByMobileDeviceGuid(security, mobileDevice.MobileDeviceGuid);

			return mobileDevice;
		}

		/// <summary>
		/// This method will update a record in the database based on the GUID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="mobileDevice">
		/// The Mobile Device.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Invalid parameters.
		/// </exception>
		/// <returns>
		/// The FMBusinessObjects.DataObjects.MobileDeviceClass.
		/// </returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public MobileDeviceClass Modify(SecurityClass security, MobileDeviceClass mobileDevice)
		{
			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			if ( mobileDevice == null )
			{
				throw new ArgumentNullException("mobileDevice");
			}

			mobileDevice.CreatedDate	= DateTimeOffset.Now;
			mobileDevice.CreatedBy		= security.UserID;
			mobileDevice.UpdatedDate	= mobileDevice.CreatedDate;
			mobileDevice.UpdatedBy		= security.UserID;

			using ( var sqlCommand = new SqlCommand( ) )
			{
				mobileDevice.UpdateSql(sqlCommand);

				if ( string.IsNullOrEmpty(sqlCommand.CommandText) == false )
				{
					this.consolidatedDa.ExecuteQuery(security, sqlCommand);
				}

				var profileToMobileDeviceMaps = new MobileDeviceProfileToMobileDeviceMaps();

				// Delete any profile to mobile device mapping (unassigned mapping).
				foreach ( MobileDeviceProfileToMobileDeviceMapClass removeProfileToMobileDeviceMap in mobileDevice.RemovedAssignedCollection )
				{
					profileToMobileDeviceMaps.Purge(security, removeProfileToMobileDeviceMap.MobileDeviceProfileGuid, removeProfileToMobileDeviceMap.AssignedToMobileDeviceGuid);
				}

				// Add new profile to mobile device mapping (assigned).  New assignment is have an empty mapping 
				// GUID.
				foreach ( MobileDeviceProfileToMobileDeviceMapClass newProfileToMobileDeviceMap in mobileDevice.AssignedProfileCollection)
				{
					if ( newProfileToMobileDeviceMap.MobileDeviceProfileToMobileDeviceGuid == Guid.Empty )
					{
						newProfileToMobileDeviceMap.AssignedToMobileDeviceGuid = mobileDevice.MobileDeviceGuid;
						profileToMobileDeviceMaps.Add(security, newProfileToMobileDeviceMap);
					}
				}

				mobileDevice = this.GetByMobileDeviceGuid(security, mobileDevice.MobileDeviceGuid);
			}

			return mobileDevice;
		}

		/// <summary>
		/// This method will purge an Mobile Device record from the database based on the GUID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="mobileDeviceGuid">
		/// The Mobile Device Guid.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Parameter must be valid.
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid mobileDeviceGuid)
		{
			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			if ( (mobileDeviceGuid == null) || (mobileDeviceGuid == Guid.Empty) )
			{
				throw new ArgumentNullException("mobileDeviceGuid");
			}

			var mobileDevice = new MobileDeviceClass { MobileDeviceGuid = mobileDeviceGuid };

			using ( var sqlcommand = new SqlCommand( ) )
			{
				var profileToMobileDeviceMaps = new MobileDeviceProfileToMobileDeviceMaps();
				profileToMobileDeviceMaps.PurgeAllByMobileDeviceGuid(security, mobileDeviceGuid);
				
				mobileDevice.PurgeSql(sqlcommand);
				this.consolidatedDa.ExecuteQuery(security, sqlcommand);
			}
		}

		/// <summary>
		/// This method will return the Mobile Device record based on a given
		/// Mobile Device ID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="mobileDeviceId">
		/// The mobile Device Id.
		/// </param>
		/// <returns>
		/// Returns a populated MobileDeviceClass object.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Parameters must be valid.
		/// </exception>
		public MobileDeviceClass GetByMobileDeviceId(SecurityClass security, string mobileDeviceId)
		{
			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			if ( string.IsNullOrEmpty(mobileDeviceId) )
			{
				throw new ArgumentNullException("mobileDeviceId");
			}

			var mobileDevice = new MobileDeviceClass( ) { MobileDeviceId = mobileDeviceId };

			using ( var sqlcommand = new SqlCommand( ) )
			{
				mobileDevice.GetByMobileDeviceIdSql(sqlcommand, security);
				var dataSet = this.consolidatedDa.GetDataSet(sqlcommand, security);

				mobileDevice.Load(dataSet);

				// Get the assigned and unassigned profiles
				mobileDevice.AssignedProfileCollection   = this.GetAssignedProfiles(security, mobileDevice.MobileDeviceGuid, inTransaction: false);
				mobileDevice.UnassignedProfileCollection = this.GetUnassignedProfiles(security, mobileDevice.MobileDeviceGuid, inTransaction: false);
			}

			return mobileDevice;
		}

		/// <summary>
		/// This method will return the mobile Device record based on a given
		/// mobile device GUID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="mobileDeviceGuid">
		/// The mobile Device Guid.
		/// </param>
		/// <returns>
		/// The FMBusinessObjects.DataObjects.MobileDeviceClass.
		/// </returns>
		public MobileDeviceClass GetByMobileDeviceGuid(SecurityClass security, Guid mobileDeviceGuid)
		{
			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			if ( (mobileDeviceGuid == null) || (mobileDeviceGuid == Guid.Empty) )
			{
				throw new ArgumentNullException("mobileDeviceGuid");
			}

			var mobileDevice = new MobileDeviceClass { MobileDeviceGuid = mobileDeviceGuid };

			using ( var sqlcommand = new SqlCommand( ) )
			{
				mobileDevice.GetByMobileDeviceGuidSql(sqlcommand, security);
				var dataSet = this.consolidatedDa.GetDataSet(sqlcommand, security);

				mobileDevice.Load(dataSet);

				// Get the assigned and unassigned profiles
				mobileDevice.AssignedProfileCollection   = this.GetAssignedProfiles(security, mobileDevice.MobileDeviceGuid, inTransaction: false);
				mobileDevice.UnassignedProfileCollection = this.GetUnassignedProfiles(security, mobileDevice.MobileDeviceGuid, inTransaction: false);
			}

			return mobileDevice;
		}

		/// <summary>
		/// This method will return the mobile device GUID for a given
		/// mobile device ID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="mobileDeviceId">
		/// The mobile Device Id.
		/// </param>
		/// <returns>
		/// Returns a mobileDeviceGuid. If not found, then an empty GUID is returned.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Must have valid parameters.
		/// </exception>
		public Guid GetGuid(SecurityClass security, string mobileDeviceId)
		{
			var mobileDeviceGuid = Guid.Empty;

			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			if ( string.IsNullOrEmpty(mobileDeviceId) )
			{
				throw new ArgumentNullException("mobileDeviceId");
			}

			var mobileDevice = new MobileDeviceClass { MobileDeviceId = mobileDeviceId };

			using ( var sqlcommand = new SqlCommand( ) )
			{
				mobileDevice.GetGuidSql(sqlcommand, security);
				var dataSet = this.consolidatedDa.GetDataSet(sqlcommand, security);

				if ( (dataSet != null) && (dataSet.Tables.Count > 0) )
				{
					var table = dataSet.Tables[0];

					if ( (table != null) && (table.Rows.Count > 0) )
					{
						var row = table.Rows[0];

						if ( row != null )
						{
							mobileDeviceGuid = row.IsNull("MobileDeviceGuid") ? Guid.Empty : (Guid) row["MobileDeviceGuid"];
						}
					}
				}
			}

			return mobileDeviceGuid;
		}

		/// <summary>
		/// This method will return all the mobile devices. It is used for the summary page.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>Returns a collection of mobile Devices.
		/// </returns>
		/// <exception cref="ArgumentNullException">Invalid parameters.
		/// </exception>
		public MobileDeviceCollection EnumerateAll(SecurityClass security)
		{
			DataSet dataSet;
			var mobileDeviceCollection = new MobileDeviceCollection( );

			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			using ( var sqlcommand = new SqlCommand( ) )
			{
				var mobileDevice = new MobileDeviceClass( );
				mobileDevice.EnumerateAllSql(sqlcommand, security);
				dataSet = this.consolidatedDa.GetDataSet(sqlcommand, security);
			}

			if ( dataSet != null && dataSet.Tables.Count > 0 )
			{
				mobileDeviceCollection.Load(dataSet);
			}

			return mobileDeviceCollection;
		}

		/// <summary>
		/// This method will return all the mobile Devices based on the find filter. It is used for the summary page.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="findFilter">
		/// The find Filter.
		/// </param>
		/// <returns>
		/// Returns an mobile Device collection.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Invalid parameters.
		/// </exception>
		public MobileDeviceCollection EnumerateByFindFilter(SecurityClass security, string findFilter)
		{
			var mobileDeviceCollection = new MobileDeviceCollection();

			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			using ( var sqlcommand = new SqlCommand( ) )
			{
				var mobileDevice = new MobileDeviceClass( );
				mobileDevice.EnumerateByFindFilterSql(sqlcommand, security, findFilter);
				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlcommand, security);

				mobileDeviceCollection.Load(dataSet);
			}

			return mobileDeviceCollection;
		}

		/// <summary>
		/// This method will return true if the mobile device ID is unique. Othewise, it
		/// returns false.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="mobileDeviceId">
		/// The mobile device id.
		/// </param>
		/// <returns>
		/// The System.Boolean.
		/// </returns>
		/// <exception cref="ArgumentNullException">Invalid or null parameters.
		/// </exception>
		public bool IsMobileDeviceUnique(SecurityClass security, string mobileDeviceId)
		{
			bool isUnique;

			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			if ( string.IsNullOrEmpty(mobileDeviceId) )
			{
				throw new ArgumentNullException("mobileDeviceId");
			}

			using ( var sqlcommand = new SqlCommand( ) )
			{
				var mobileDevice = new MobileDeviceClass( );
				mobileDevice.IsMobileDeviceUniqueSql(sqlcommand, security);
				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlcommand, security);

				isUnique = mobileDevice.LoadIsMobileDeviceUnique(dataSet);
			}

			return isUnique;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will retrieve all the assigned profiles for a given mobile device
		/// GUID and return the collection.
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
		private MobileDeviceProfileToMobileDeviceMapCollection GetAssignedProfiles(SecurityClass security, Guid mobileDeviceGuid, bool inTransaction)
		{
			var profileToMobileDeviceMaps = new MobileDeviceProfileToMobileDeviceMaps();

			return profileToMobileDeviceMaps.EnumerateMobileDeviceByMobileDeviceGuid(security, mobileDeviceGuid, inTransaction);
		}

		/// <summary>
		/// This method will retrieve all the unassigned profiles for a given mobile device
		/// GUID and return the collection.
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
		private MobileDeviceProfileToMobileDeviceMapCollection GetUnassignedProfiles(SecurityClass security, Guid mobileDeviceGuid, bool inTransaction)
		{
			var profileToMobileDeviceMaps = new MobileDeviceProfileToMobileDeviceMaps( );

			return profileToMobileDeviceMaps.EnumerateUnassignedProfiles(security, mobileDeviceGuid, inTransaction);
		}
		#endregion
	}
}