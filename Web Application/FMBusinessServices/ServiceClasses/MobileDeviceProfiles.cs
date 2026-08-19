// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MobileDeviceProfiles.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the MobileDeviceProfiles type.
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
	/// delete, and modify profile data.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class MobileDeviceProfiles : IMobileDeviceProfiles
	{
		#region Private data members
		/// <summary>
		/// The consolidated da.
		/// </summary>
		private ConsolidatedDAClass consolidatedDa;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="MobileDeviceProfiles"/> class.
		/// </summary>
		public MobileDeviceProfiles ( )
		{
			this.consolidatedDa = new ConsolidatedDAClass();
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will add a profile to the database.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="mobileDeviceProfile">
		/// The mobile device profile.
		/// </param>
		/// <returns>Returns the an updated MobileDeviceProfile for the record.
		/// </returns>
		/// <exception cref="ArgumentNullException">Must have a none null sercurity and mobile device object.
		/// </exception>
		[OperationBehavior ( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public MobileDeviceProfile Add ( SecurityClass security, MobileDeviceProfile mobileDeviceProfile )
		{
			Guid newGuid = Guid.NewGuid();

			if ( security == null )
			{
				throw new ArgumentNullException ( "security" );
			}

			if ( mobileDeviceProfile == null )
			{
				throw new ArgumentNullException ( "mobileDeviceProfile" );
			}

			if (string.IsNullOrEmpty(mobileDeviceProfile.ProfileId))
			{
				throw new Exception("Profile ID Required");
			}


			// Can only have one default profile for a site or from an assigned site.
			if ( this.CheckDefaultProfileConstraint(security, mobileDeviceProfile) )
			{
				throw new Exception("A default profile already exists for site or assigned site.");
			}

			// The profile ID must be unique for a given site and that includes an ID
			// that has been assigned down.
			if ( this.DoesProfileIdExists(security, mobileDeviceProfile) )
			{
				throw new Exception("Profile ID '" + mobileDeviceProfile.ProfileId + "' already exists.");
			}

			mobileDeviceProfile.MobileDeviceProfileGuid = newGuid;
			mobileDeviceProfile.SiteGuid				= security.SiteGuid;
			mobileDeviceProfile.CreatedDate				= DateTimeOffset.Now;
			mobileDeviceProfile.CreatedBy				= security.UserID;
			mobileDeviceProfile.UpdatedDate				= mobileDeviceProfile.CreatedDate;
			mobileDeviceProfile.UpdatedBy				= security.UserID;

			using ( var sqlCommand = new SqlCommand ( ) )
			{
				mobileDeviceProfile.InsertSql(sqlCommand);
				this.consolidatedDa.ExecuteQuery(security, sqlCommand);

				// Get the associated analog input information.
				var analogInputs = new MobileDeviceProfileAnalogInputs ( );

				foreach ( MobileDeviceProfileAnalogInput analogInput in mobileDeviceProfile.AnalogInputCollection )
				{
					analogInput.MobileDeviceProfileGuid = newGuid;
					var analogInputGuid = analogInputs.Add(security, analogInput);
					analogInput.MobileDeviceProfileAnalogInputGuid = analogInputGuid;
				}

				// Get the associated printer configuration information.
				var printers = new MobileDeviceProfilePrinters();

				foreach ( MobileDeviceProfilePrinter printer in mobileDeviceProfile.PrinterCollection )
				{
					printer.MobileDeviceProfileGuid = newGuid;
					var printerGuid = printers.Add(security, printer);
					printer.MobileDeviceProfilePrinterGuid = printerGuid;
				}

				// Save Profile to Mobile Device mapping
				this.SaveProfileToMobileDeviceMapping(security, mobileDeviceProfile);
			}

			mobileDeviceProfile = this.GetByProfileGuid(security, mobileDeviceProfile.MobileDeviceProfileGuid, withEntityClause : false);

			// Create Entity to Site Map
			var entityToSiteMaps = new EntityToSiteMaps( );
			var entityToSiteMap = new EntityToSiteMapClass(	
															mobileDeviceProfile.ProfileId,
															ENTITY_TYPE.MOBILE_DEVICE_PROFILE,
															mobileDeviceProfile.SiteGuid,
															mobileDeviceProfile.MobileDeviceProfileGuid );

			entityToSiteMaps.Add(security, entityToSiteMap, GetType( ).GUID);

			return mobileDeviceProfile;
		}

		/// <summary>
		/// This method will update a record in the database based on the GUID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="mobileDeviceProfile">
		/// The mobile device profile object that contains the data.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Invalid parameters.
		/// </exception>
		/// <returns>
		/// The FMBusinessObjects.DataObjects.MobileDeviceProfile.
		/// </returns>
		[OperationBehavior ( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public MobileDeviceProfile Modify ( SecurityClass security, MobileDeviceProfile mobileDeviceProfile )
		{
			if ( security == null )
			{
				throw new ArgumentNullException ( "security" );
			}

			if ( mobileDeviceProfile == null )
			{
				throw new ArgumentNullException ( "mobileDeviceProfile" );
			}

			if (string.IsNullOrEmpty(mobileDeviceProfile.ProfileId))
			{
				throw new Exception("Profile ID Required");
			}

			// Can only have one default profile for a site or from an assigned site.
			if ( this.CheckDefaultProfileConstraint(security, mobileDeviceProfile) )
			{
				throw new Exception("A default profile already exists for site or assigned site.");
			}

			mobileDeviceProfile.CreatedDate = DateTimeOffset.Now;
			mobileDeviceProfile.CreatedBy	= security.UserID;
			mobileDeviceProfile.UpdatedDate = mobileDeviceProfile.CreatedDate;
			mobileDeviceProfile.UpdatedBy	= security.UserID;

			using ( var sqlCommand = new SqlCommand ( ) )
			{
				mobileDeviceProfile.UpdateSql ( sqlCommand );

				if ( string.IsNullOrEmpty ( sqlCommand.CommandText ) == false )
				{
					this.consolidatedDa.ExecuteQuery ( security, sqlCommand );
				}

				// Delete analog input items that have been marked in the delete collection.
				var analogInputs = new MobileDeviceProfileAnalogInputs ( );
				analogInputs.Purge(security, mobileDeviceProfile.DeletedAnalogInputCollection);

				// Update existing items and/or insert new items.
				foreach ( MobileDeviceProfileAnalogInput analogInput in mobileDeviceProfile.AnalogInputCollection )
				{
					if ( analogInput.MobileDeviceProfileAnalogInputGuid.Equals ( Guid.Empty ) )
					{
						analogInput.MobileDeviceProfileGuid = mobileDeviceProfile.MobileDeviceProfileGuid;
						var analogInputGuid = analogInputs.Add ( security, analogInput );
						analogInput.MobileDeviceProfileAnalogInputGuid = analogInputGuid;
					}
					else
					{
						analogInputs.Modify(security, analogInput);
					}
				}

				// Delete printer items that have been marked in the delete collection.
				var printers = new MobileDeviceProfilePrinters( );
				printers.Purge(security, mobileDeviceProfile.DeletedPrinterCollection);

				// Update existing items and/or insert new items.
				foreach ( MobileDeviceProfilePrinter printer in mobileDeviceProfile.PrinterCollection )
				{
					if ( printer.MobileDeviceProfilePrinterGuid.Equals(Guid.Empty) )
					{
						printer.MobileDeviceProfileGuid = mobileDeviceProfile.MobileDeviceProfileGuid;
						var printerGuid = printers.Add(security, printer);
						printer.MobileDeviceProfilePrinterGuid = printerGuid;
					}
					else
					{
						printers.Modify(security, printer);
					}
				}

				// Save Profile to Mobile Device mapping
				this.SaveProfileToMobileDeviceMapping(security, mobileDeviceProfile);

				mobileDeviceProfile = this.GetByProfileGuid(security, mobileDeviceProfile.MobileDeviceProfileGuid, withEntityClause : false);
			}

			return mobileDeviceProfile;
		}

		/// <summary>
		/// This method will purge a profile record from the database based on the GUID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="profileGuid">
		/// The profile guid.
		/// </param>
		/// <exception cref="ArgumentNullException">Parameter must be valid.
		/// </exception>
		[OperationBehavior ( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Purge ( SecurityClass security, Guid profileGuid )
		{
			if ( security == null )
			{
				throw new ArgumentNullException ( "security" );
			}

			if ( ( profileGuid == null ) || ( profileGuid == Guid.Empty ) )
			{
				throw new ArgumentNullException ( "profileGuid" );
			}

			var mobileDeviceProfile = new MobileDeviceProfile { MobileDeviceProfileGuid = profileGuid };

			// Purge associated analog input records.
			var analogInputs = new MobileDeviceProfileAnalogInputs();
			analogInputs.PurgeAll(security, profileGuid);

			// Purge associated printer records.
			var printers = new MobileDeviceProfilePrinters();
			printers.PurgeAll(security, profileGuid);

			// Purge associated Mobile Devices
			var profileToMobileDeviceMaps = new MobileDeviceProfileToMobileDeviceMaps();
			profileToMobileDeviceMaps.PurgeAllByProfileGuid(security, profileGuid);

			using ( var sqlcommand = new SqlCommand ( ) )
			{
				mobileDeviceProfile.PurgeSql ( sqlcommand );
				this.consolidatedDa.ExecuteQuery ( security, sqlcommand );
			}

			// Purge from EntityToSiteMap
			var entityToSiteMaps = new EntityToSiteMaps( );
			EntityToSiteMapCollectionClass entityToSiteMapCollection = 
							entityToSiteMaps.EnumerateByTypeIDAndGuid(security, mobileDeviceProfile.EntityType, profileGuid);

			foreach ( EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection )
			{
				entityToSiteMap.ID = mobileDeviceProfile.ProfileId;
				entityToSiteMaps.Purge(security, entityToSiteMap);
			}
		}

		/// <summary>
		/// This method will return the Mobile Device Profile record based on a given
		/// profile ID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="profileId">
		/// The profile id.
		/// </param>
		/// <returns>Returns a populated MobileDeviceProfile object.
		/// </returns>
		/// <exception cref="ArgumentNullException">Parameters must be valid.
		/// </exception>
		public MobileDeviceProfile GetByProfileId ( SecurityClass security, string profileId )
		{
			if ( security == null )
			{
				throw new ArgumentNullException ( "security" );
			}

			if ( string.IsNullOrEmpty ( profileId ) )
			{
				throw new ArgumentNullException ( "profileId" );
			}

			var mobileDeviceProfile = new MobileDeviceProfile { ProfileId = profileId };

			using ( SqlCommand sqlcommand = new SqlCommand ( ) )
			{
				mobileDeviceProfile.GetByProfileIdSql ( sqlcommand, security );
				var dataSet = this.consolidatedDa.GetDataSet ( sqlcommand, security );

				mobileDeviceProfile.Load(dataSet);

				// Get the associated analog input information.
				var analogInputs			= new MobileDeviceProfileAnalogInputs ( );
				var analogInputDataSet		= analogInputs.EnumerateByProfileGuid ( security, mobileDeviceProfile.MobileDeviceProfileGuid );
				var analogInputCollection	= new MobileDeviceProfileAnalogInputCollection();

				analogInputCollection.Load ( analogInputDataSet );
				mobileDeviceProfile.AnalogInputCollection = analogInputCollection;

				// Get the associated printer information.
				var printers			= new MobileDeviceProfilePrinters( );
				var printerDataSet		= printers.EnumerateByProfileGuid(security, mobileDeviceProfile.MobileDeviceProfileGuid);
				var printerCollection	= new MobileDeviceProfilePrinterCollection( );

				printerCollection.Load(printerDataSet);
				mobileDeviceProfile.PrinterCollection = printerCollection;

				// Get assigned Mobile Devices
				var profileToMobileDeviceMaps = new MobileDeviceProfileToMobileDeviceMaps( );
				mobileDeviceProfile.AssignedMobileDeviceCollection =
										profileToMobileDeviceMaps.EnumerateMobileDeviceByProfileGuid(security, mobileDeviceProfile.MobileDeviceProfileGuid, inTransaction: false);
				
				// Get unassigned Mobile Devices
				mobileDeviceProfile.UnassignMobileDeviceCollection = 
										profileToMobileDeviceMaps.EnumerateUnassignedMobileDevices(security, mobileDeviceProfile.MobileDeviceProfileGuid, inTransaction: false);
			}

			return mobileDeviceProfile;
		}

		/// <summary>
		/// This method will return the Mobile Device Profile record based on a given
		/// profile GUID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="profileGuid">
		/// The profile guid.
		/// </param>
		/// <returns>
		/// The FMBusinessObjects.DataObjects.MobileDeviceProfile.
		/// </returns>
		public MobileDeviceProfile GetByProfileGuid (SecurityClass security, Guid profileGuid)
		{
			// True = using the entity clause.
			return this.GetByProfileGuid(security, profileGuid, true);
		}

		/// <summary>
		/// This method will return the mobile device profile GUID for a given
		/// profile ID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="profileId">
		/// The profile id.
		/// </param>
		/// <returns>Returns a MobileDeviceProfileGuid. If not found, then an empty GUID is returned.
		/// </returns>
		/// <exception cref="ArgumentNullException">Must have valid parameters.
		/// </exception>
		public Guid GetGuid ( SecurityClass security, string profileId )
		{
			var profileGuid = Guid.Empty;

			if ( security == null )
			{
				throw new ArgumentNullException ( "security" );
			}

			if ( string.IsNullOrEmpty (profileId ) )
			{
				throw new ArgumentNullException ( "profileId" );
			}

			var mobileDeviceProfile = new MobileDeviceProfile { ProfileId = profileId };

			using ( var sqlcommand = new SqlCommand ( ) )
			{
				mobileDeviceProfile.GetGuidSql ( sqlcommand, security );
				var dataSet = this.consolidatedDa.GetDataSet ( sqlcommand, security );

				if ( ( dataSet != null ) && ( dataSet.Tables.Count > 0 ) )
				{
					var table = dataSet.Tables[0];

					if ( ( table != null ) && ( table.Rows.Count > 0 ) )
					{
						var row = table.Rows[0];

						if ( row != null )
						{
							profileGuid = row.IsNull("MobileDeviceProfileGuid") ? Guid.Empty : (Guid) row["MobileDeviceProfileGuid"];
						}
					}
				}
			}

			return profileGuid;
		}

		/// <summary>
		/// This method will return all the profiles. It is used for the summary page.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>Returns a dataset of profiles.
		/// </returns>
		/// <exception cref="ArgumentNullException">Invalid parameters.
		/// </exception>
		public DataSet EnumerateAll ( SecurityClass security )
		{
			DataSet dataSet;

			if ( security == null )
			{
				throw new ArgumentNullException ( "security" );
			}

			using ( var sqlcommand = new SqlCommand ( ) )
			{
				var mobileDeviceProfile = new MobileDeviceProfile ( );
				mobileDeviceProfile.EnumerateAllSql ( sqlcommand, security );
				dataSet = this.consolidatedDa.GetDataSet ( sqlcommand, security );
			}

			return dataSet;
		}

		/// <summary>
		/// This method will return all the profiles based on the find filter. It is used for the summary page.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="findFilter">
		/// The find Filter.
		/// </param>
		/// <returns>
		/// Returns a dataset of profiles.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Invalid parameters.
		/// </exception>
		public DataSet EnumerateByFindFilter(SecurityClass security, string findFilter)
		{
			DataSet dataSet;

			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			using ( var sqlcommand = new SqlCommand( ) )
			{
				var mobileDeviceProfile = new MobileDeviceProfile( );
				mobileDeviceProfile.EnumerateByFindFilterSql(sqlcommand, security, findFilter);
				dataSet = this.consolidatedDa.GetDataSet(sqlcommand, security);
			}

			return dataSet;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will return the Mobile Device Profile record based on a given
		/// profile GUID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="profileGuid">
		/// The profile guid.
		/// </param>
		/// <param name="withEntityClause">
		/// The with Entity Clause.
		/// </param>
		/// <returns>
		/// Returns a populated MobileDeviceProfile object.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Parameters must be valid.
		/// </exception>
		private MobileDeviceProfile GetByProfileGuid(SecurityClass security, Guid profileGuid, bool withEntityClause)
		{
			if ( security == null )
			{
				throw new ArgumentNullException("security");
			}

			if ( (profileGuid == null) || (profileGuid == Guid.Empty) )
			{
				throw new ArgumentNullException("profileGuid");
			}

			var mobileDeviceProfile = new MobileDeviceProfile { MobileDeviceProfileGuid = profileGuid };

			using ( var sqlcommand = new SqlCommand( ) )
			{
				mobileDeviceProfile.GetByProfileGuidSql(sqlcommand, security, withEntityClause);
				var dataSet = this.consolidatedDa.GetDataSet(sqlcommand, security);

				mobileDeviceProfile.Load(dataSet);

				// Get the associated analog input information.
				var analogInputs = new MobileDeviceProfileAnalogInputs( );
				var analogInputDataSet = analogInputs.EnumerateByProfileGuid(security, profileGuid);
				var analogInputCollection = new MobileDeviceProfileAnalogInputCollection( );

				analogInputCollection.Load(analogInputDataSet);
				mobileDeviceProfile.AnalogInputCollection = analogInputCollection;

				// Get the associated printer information.
				var printers = new MobileDeviceProfilePrinters( );
				var printerDataSet = printers.EnumerateByProfileGuid(security, profileGuid);
				var printerCollection = new MobileDeviceProfilePrinterCollection( );

				printerCollection.Load(printerDataSet);
				mobileDeviceProfile.PrinterCollection = printerCollection;

				// Get assigned Mobile Devices
				var profileToMobileDeviceMaps = new MobileDeviceProfileToMobileDeviceMaps();
				mobileDeviceProfile.AssignedMobileDeviceCollection =
										profileToMobileDeviceMaps.EnumerateMobileDeviceByProfileGuid(security, profileGuid, inTransaction: false);

				// Get unassigned Mobile Devices
				mobileDeviceProfile.UnassignMobileDeviceCollection = 
										profileToMobileDeviceMaps.EnumerateUnassignedMobileDevices(security, profileGuid, inTransaction: false);
			}

			return mobileDeviceProfile;
		}

		/// <summary>
		/// This method will check to ensure that an existing Default Profile does not exist
		/// for the given site or assigned site.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="mobileDeviceProfile">
		/// The mobile device profile.
		/// </param>
		/// <returns>
		/// The System.Boolean.
		/// </returns>
		private bool CheckDefaultProfileConstraint(SecurityClass security, MobileDeviceProfile mobileDeviceProfile)
		{
			bool defaultProfileExist = false;

			// Only perform the check if the Make Default Profile is set to true.
			if ( mobileDeviceProfile.MakeDefaultProfile )
			{
				using (var sqlCommand = new SqlCommand())
				{
					mobileDeviceProfile.DefaultProfileConstraintSql(sqlCommand, security);

					if (string.IsNullOrEmpty(sqlCommand.CommandText) == false)
					{
						DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);
						defaultProfileExist = mobileDeviceProfile.LoadCheckDefaultProfileConstraint(dataSet);
					}
				}
			}

			return defaultProfileExist;
		}

		/// <summary>
		/// This method will return true if the Profile ID exists in the current site
		/// or has been assigned down to the current site.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="mobileDeviceProfile">
		/// The mobile device profile.
		/// </param>
		/// <returns>
		/// The System.Boolean.
		/// </returns>
		private bool DoesProfileIdExists(SecurityClass security, MobileDeviceProfile mobileDeviceProfile)
		{
			bool profileIdExists = false;

			using (var sqlCommand = new SqlCommand())
			{
				mobileDeviceProfile.CheckForProfileIdUniquenessSql(sqlCommand, security);

				if ( string.IsNullOrEmpty(sqlCommand.CommandText) == false )
				{
					DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);
					profileIdExists = mobileDeviceProfile.LoadProfileIdCheckUniqueness(dataSet);
				}
			}

			return profileIdExists;
		}

		/// <summary>
		/// This method will add or remove a Mobile Device Profile to Mobile Device mapping.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="mobileDeviceProfile">
		/// The mobile device profile.
		/// </param>
		/// <exception cref="Exception">Invalid GUID for Mobile Device
		/// </exception>
		private void SaveProfileToMobileDeviceMapping(SecurityClass security, MobileDeviceProfile mobileDeviceProfile)
		{
			var mobileDevices = new MobileDevices();
			var profileToMobileDeviceMaps = new MobileDeviceProfileToMobileDeviceMaps();

			// Remove unassigned Mobile Devices.
			if ( mobileDeviceProfile.RemoveMobileDeviceMapCollection.Count > 0 )
			{
				foreach ( MobileDeviceProfileToMobileDeviceMapClass profileToMobileDeviceMap in mobileDeviceProfile.RemoveMobileDeviceMapCollection )
				{
					// The collection may contain new mapping which will not have a GUID for the Mobile Device Profile item and
					// Mobile Device item.  Therefore, it does not exist in the database and does not need to be removed.
					if ( profileToMobileDeviceMap.AssignedToMobileDeviceGuid != Guid.Empty && profileToMobileDeviceMap.MobileDeviceProfileGuid != Guid.Empty )
					{
						profileToMobileDeviceMaps.Purge(security, profileToMobileDeviceMap.MobileDeviceProfileGuid, profileToMobileDeviceMap.AssignedToMobileDeviceGuid);
					}
				}
			}

			// Add assigned Movile Device Profile to Mobile Device mapping.
			if ( mobileDeviceProfile.AssignedMobileDeviceCollection.Count > 0 )
			{
				var dataDictionaries = new DataDictionariesClass( );

				foreach ( MobileDeviceProfileToMobileDeviceMapClass profileToMobileDeviceMap in mobileDeviceProfile.AssignedMobileDeviceCollection )
				{
					// If there is a mapping GUID present, that means it exist in the database. 
					// Therefore, we can ignore it.
					if ( profileToMobileDeviceMap.MobileDeviceProfileToMobileDeviceGuid != Guid.Empty )
					{
						continue;
					}

					profileToMobileDeviceMap.MobileDeviceProfileGuid = mobileDeviceProfile.MobileDeviceProfileGuid;
					profileToMobileDeviceMap.AssignedToMobileDeviceGuid = mobileDevices.GetGuid(security, profileToMobileDeviceMap.MobileDeviceId);

					if ( profileToMobileDeviceMap.AssignedToMobileDeviceGuid == Guid.Empty )
					{
						throw new Exception(dataDictionaries.Get(security.SiteGuid, "Invalid GUID for Mobile Device") + ": " + profileToMobileDeviceMap.MobileDeviceId);
					}

					profileToMobileDeviceMaps.Add(security, profileToMobileDeviceMap);
				}
			}
		}
		#endregion
	}
}