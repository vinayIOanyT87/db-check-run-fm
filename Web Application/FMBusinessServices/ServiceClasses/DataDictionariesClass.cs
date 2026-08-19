// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataDictionariesClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for DataDictionariesClass.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;
	using FMBusinessObjects.UtilityObjects;

	/// <summary>
	/// Implements the DataDictionariesClass
	/// </summary>
	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class DataDictionariesClass : IDependency, IDataDictionariesClass
	{
		#region Constants and Fields

		/// <summary>
		/// Provides access to the database.
		/// </summary>
		internal readonly ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Adds the specified security.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="dataDictionary">The data dictionary.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Add(SecurityClass security, DataDictionaryClass dataDictionary)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (dataDictionary == null)
			{
				throw new ArgumentNullException("dataDictionary");
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			// If EntityAssignmentMap exists do not allow addition
			var entityToSiteMaps = new EntityToSiteMaps();
			var entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndSiteGuid(security, dataDictionary.EntityType, security.SiteGuid);
			if (entityToSiteMapCollection.Count > 0)
			{
				throw new Exception("Dictionary Assigned");
			}

			dataDictionary.SiteGuid = security.SiteGuid;
			dataDictionary.CreatedDate = DateTimeOffset.Now;
			dataDictionary.CreatedBy = security.UserID;
			dataDictionary.UpdatedDate = dataDictionary.CreatedDate;
			dataDictionary.UpdatedBy = security.UserID;
			dataDictionary.DataDictionaryGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				dataDictionary.InsertSQL(cmd);
				object newRowVerisonObj = this.ConsolidatedDA.ExecuteScalar(cmd, security);

				// Set the new row version if not null.
				if (newRowVerisonObj != null)
				{
					dataDictionary.RowVersion = (byte[])newRowVerisonObj;
				}
			}

			// Update all instances of DataDictionary in AppDomain
			var domain = AppDomain.CurrentDomain;

			var keyDictionary = (DataDictionaryCollectionClass)domain.GetData("KeyDictionary" + dataDictionary.SiteGuid.ToString());
			var guidDictionary = (Dictionary<Guid, string>)domain.GetData("GuidDictionary" + dataDictionary.SiteGuid.ToString());


			if (keyDictionary != null)
			{
				keyDictionary.Add(dataDictionary.Key, dataDictionary.Value, dataDictionary.UpdatedDate, DataDictionaryClass.DataDictionaryRowVersion(dataDictionary.RowVersion));
				if (guidDictionary.ContainsValue(dataDictionary.Key))
				{
					guidDictionary.Remove(dataDictionary.DataDictionaryGuid);
				}
				guidDictionary.Add(dataDictionary.DataDictionaryGuid, dataDictionary.Key);
			}

			entityToSiteMaps = new EntityToSiteMaps();
			entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(security, dataDictionary.EntityType, dataDictionary.SiteGuid);

			foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
			{
				keyDictionary = (DataDictionaryCollectionClass)domain.GetData("KeyDictionary" + entityToSiteMap.SiteGuid.ToString());
				guidDictionary = (Dictionary<Guid, string>)domain.GetData("GuidDictionary" + entityToSiteMap.SiteGuid.ToString());

				if (keyDictionary != null)
				{
					keyDictionary.Add(dataDictionary.Key, dataDictionary.Value, dataDictionary.UpdatedDate, DataDictionaryClass.DataDictionaryRowVersion(dataDictionary.RowVersion));
					if (guidDictionary.ContainsValue(dataDictionary.Key))
					{
						guidDictionary.Remove(dataDictionary.DataDictionaryGuid);
					}
					guidDictionary.Add(dataDictionary.DataDictionaryGuid, dataDictionary.Key);
				}
			}
		}

		/// <summary>
		/// Enumerates the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns>A collection of data dictionary keys.</returns>
		public DataDictionaryCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var dataDictionary = new DataDictionaryClass { SiteGuid = security.SiteGuid};
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				dataDictionary.EnumerateSQL(cmd);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			var dataDictionaryCollection = new DataDictionaryCollectionClass();
			DataTable table = set.Tables[0];

			while (table.Rows.Count != 0)
			{
				dataDictionary.Load(set);
				dataDictionaryCollection.Add(dataDictionary.Key, dataDictionary.Value, dataDictionary.UpdatedDate, DataDictionaryClass.DataDictionaryRowVersion(dataDictionary.RowVersion));
				table.Rows.RemoveAt(0);
			}

			return dataDictionaryCollection;
		}

		/// <summary>
		/// Enumerates the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns>A collection of data dictionary keys.</returns>
		public DataDictionaryCollectionClass EnumerateCached(Guid siteGuid)
		{
			AppDomain domain = AppDomain.CurrentDomain;
			var keyDictionary = (DataDictionaryCollectionClass)domain.GetData("KeyDictionary" + siteGuid.ToString());
			var guidDictionary = (Dictionary<Guid,string>)domain.GetData("GuidDictionary" + siteGuid.ToString());

			var security = new SecurityClass { SiteGuid = siteGuid, UserID = DBAccess.ServiceLoginAccess };

			if (keyDictionary == null)
			{
				var dictionaryClassList = this.EnumerateBySite2(security);
			
				keyDictionary = new DataDictionaryCollectionClass();
				guidDictionary = new Dictionary<Guid, String>(dictionaryClassList.Count);
				foreach(var dictionaryClass in dictionaryClassList)
				{
					keyDictionary.Add(dictionaryClass.Key, dictionaryClass.Value, dictionaryClass.UpdatedDate, DataDictionaryClass.DataDictionaryRowVersion(dictionaryClass.RowVersion));
					if (guidDictionary.ContainsValue(dictionaryClass.Key))
					{
						guidDictionary.Remove(dictionaryClass.DataDictionaryGuid);
					}
					guidDictionary.Add(dictionaryClass.DataDictionaryGuid, dictionaryClass.Key);
				}
			}
			else
			{
				// If the data dictionary is more than (setting) minutes old, then check for changes
				DateTimeOffset checkDataDictTime = DateTimeOffset.Now;

				int dataDictionaryRefreshIntervalMinutes = AppSettingsHelper.GetKeyValue("DataDictionaryFailsafeCheckForChangesIntervalMinutes", 0);

				var timeDifference = checkDataDictTime - keyDictionary.LatestUpdatedDateTime;
				var refreshTimeInterval = new TimeSpan(0, dataDictionaryRefreshIntervalMinutes, 0);

				if (timeDifference > refreshTimeInterval)
				{
					var deletedDataDictionaryList = this.EnumerateDeletedFromRowVersion(security, keyDictionary.DeletedRowVersion);

					foreach (var dataDictionaryKeyValuePair in deletedDataDictionaryList)
					{
						if (guidDictionary.ContainsKey(dataDictionaryKeyValuePair.Key))
						{
							if (keyDictionary.Contains(guidDictionary[dataDictionaryKeyValuePair.Key]))
							{
								keyDictionary.Remove(guidDictionary[dataDictionaryKeyValuePair.Key]);
							}
							guidDictionary.Remove(dataDictionaryKeyValuePair.Key);
						}

						keyDictionary.DeletedRowVersion = DataDictionaryClass.DataDictionaryRowVersion(dataDictionaryKeyValuePair.Value);
					}


					var differenceDictionary = this.EnumerateFromRowVersion(security, keyDictionary.RowVersion);

					foreach (string translationKey in differenceDictionary.Keys)
					{
						if (keyDictionary.Contains(translationKey))
						{
							keyDictionary.Remove(translationKey);
						}

						keyDictionary.Add(translationKey, differenceDictionary[translationKey].Value, differenceDictionary[translationKey].UpdatedDate, DataDictionaryClass.DataDictionaryRowVersion(differenceDictionary[translationKey].RowVersion));
						if (guidDictionary.ContainsValue(differenceDictionary[translationKey].Key))
						{
							guidDictionary.Remove(differenceDictionary[translationKey].DataDictionaryGuid);
						}
						guidDictionary.Add(differenceDictionary[translationKey].DataDictionaryGuid, differenceDictionary[translationKey].Key);
					}

					// Dictionary is now up to date, so set LatestUpdatedDateTime
					keyDictionary.LatestUpdatedDateTime = checkDataDictTime;
				}
			}

			domain.SetData("KeyDictionary" + security.SiteGuid.ToString(), keyDictionary);
			domain.SetData("GuidDictionary" + security.SiteGuid.ToString(), guidDictionary);

			return keyDictionary;
		}

		/// <summary>
		/// Enumerates the by site.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns>A collection of data dictionary keys</returns>
		public DataDictionaryCollectionClass EnumerateBySite(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var dataDictionary = new DataDictionaryClass { SiteGuid = security.SiteGuid };

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				dataDictionary.EnumerateBySiteSQL(cmd);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			var dataDictionaryCollection = new DataDictionaryCollectionClass();
			DataTable table = set.Tables[0];

			while (table.Rows.Count != 0)
			{
				dataDictionary.Load(set);
				dataDictionaryCollection.Add(dataDictionary.Key, dataDictionary.Value, dataDictionary.UpdatedDate, DataDictionaryClass.DataDictionaryRowVersion(dataDictionary.RowVersion));
				table.Rows.RemoveAt(0);
			}

			return dataDictionaryCollection;
		}

        /// <summary>
        /// Enumerates the by site.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <returns>A collection of data dictionary keys</returns>
        public List<DataDictionaryClass> EnumerateBySite2(SecurityClass security)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            var dataDictionary = new DataDictionaryClass { SiteGuid = security.SiteGuid };

            DataSet set;

            using (var cmd = new SqlCommand())
            {
                dataDictionary.EnumerateBySiteSQL(cmd);
                set = this.ConsolidatedDA.GetDataSet(cmd, security);
            }

            var dataDictionaryList= new List<DataDictionaryClass>();

            if (set == null || set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
            {
                return dataDictionaryList;
            }

            foreach (DataRow row in set.Tables[0].Rows)
            {
                dataDictionary = new DataDictionaryClass();
                dataDictionary.Load(row);
				dataDictionaryList.Add(dataDictionary);
            }

            return dataDictionaryList;
        }

        /// <summary>
        /// Enumerates from updated date time.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="updatedDateTime">The updated date time.</param>
        /// <returns>A collection of data dictionary keys.</returns>
        public DataDictionaryCollectionClass EnumerateFromUpdatedDateTime(SecurityClass security, DateTimeOffset updatedDateTime)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var dataDictionary = new DataDictionaryClass { SiteGuid = security.SiteGuid, UpdatedDate = updatedDateTime };
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				dataDictionary.EnumerateSQLFromUpdatedDateTime(cmd);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			var dataDictionaryCollection = new DataDictionaryCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				dataDictionary.Load(set);
				dataDictionaryCollection.Add(dataDictionary.Key, dataDictionary.Value, dataDictionary.UpdatedDate, DataDictionaryClass.DataDictionaryRowVersion(dataDictionary.RowVersion));
				table.Rows.RemoveAt(0);
			}

			return dataDictionaryCollection;
		}

		/// <summary>
		/// Resets the specified data dictionary cache item.
		/// </summary>
		/// <param name="siteGuid">Guid identifying the site for which the data dictionary cache should be reset.</param>
		public void ResetDataDictionaryCache(Guid siteGuid)
		{
			var key = "KeyDictionary" + siteGuid.ToString();
			var guid = "GuidDictionary" + siteGuid.ToString();

			AppDomain domain = AppDomain.CurrentDomain;
			var keyDictionary = (DataDictionaryCollectionClass) domain.GetData(key);
			var guidDictionary = (DataDictionaryCollectionClass)domain.GetData(guid);

			if (keyDictionary != null)
			{
				domain.SetData(key, null);
			}

			if (guidDictionary != null)
			{
				domain.SetData(guid, null);
			}

		}

		/// <summary>
		/// Returns the properly translated text for the specified key at the specified site or the default translation for the key, if no translation exists.
		/// </summary>
		/// <param name="siteGuid">The site to use for translation context.</param>
		/// <param name="key">The key to translate.</param>
		/// <returns>
		/// Properly translated text.
		/// </returns>
		public string Get(Guid siteGuid, string key)
		{
			var dictionary = this.EnumerateCached( siteGuid );			

			return dictionary[key];
		}

		/// <summary>
		/// Populates the values for keys in the provided keyTable.
		/// </summary>
		/// <param name="siteGuid">The site for which to draw translations.</param>
		/// <param name="keyTable">A table containing Keys and Values fields for translation.</param>
		/// <returns>The data table with translated values.</returns>
		public Dictionary<string,string> TranslateKeyPairTable(Guid siteGuid, Dictionary<string,string> keyTable)
		{
			// Do a get and let the standard routine determine if a dictionary update is necessary.
			this.Get(siteGuid, "Varec");

			// Now get the dictionary out of the domain.  
			// It should be there unless the Get call above failed in some way.
			AppDomain domain = AppDomain.CurrentDomain;
			var keyDictionary = (DataDictionaryCollectionClass) domain.GetData( "KeyDictionary" + siteGuid.ToString() );

			// But just in case some other thread got active and deleted it.
			if (keyDictionary == null )
			{
				var security = new SecurityClass { SiteGuid = siteGuid, UserID = DBAccess.ServiceLoginAccess };
				keyDictionary = this.Enumerate( security );
			}

			char[] seperator = { '|' };

			var list = keyTable.ToList();

			for (var index = 0; index < list.Count; ++index)
			{
				var keyPair = list[index];

				var key = keyPair.Key;
				var value = keyDictionary[key];

				string[] keyStrings = key.Split( seperator );

				// We are iterating through a list based on the keyTable but
				// we want to set the translated values in the keyTable itself.
				if ( keyStrings.Length > 1 && value == keyStrings[1] || key == value )
				{
					keyTable[key] = string.Empty;
				}
				else
				{
					value = value.Replace( "&", "&amp;" );
					keyTable[key] = value.Replace( "\"", "&quot;" );
				}
			}

			return keyTable;
		}

		/// <summary>
		/// Modifies the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="dataDictionary">The data dictionary.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, DataDictionaryClass dataDictionary)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (dataDictionary == null)
			{
				throw new ArgumentNullException("dataDictionary");
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			// If EntityAssignmentMap exists it do not allow modification
			var entityToSiteMaps = new EntityToSiteMaps();
			var entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndSiteGuid(security, dataDictionary.EntityType, dataDictionary.SiteGuid);

			if (entityToSiteMapCollection.Count > 0)
			{
				throw new Exception("Dictionary Assigned");
			}

			dataDictionary.UpdatedDate = DateTimeOffset.Now;
			dataDictionary.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				dataDictionary.UpdateSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			// Update all instances of DataDictionary in AppDomain
			AppDomain domain = AppDomain.CurrentDomain;

			var keyDictionary = (DataDictionaryCollectionClass)domain.GetData("KeyDictionary" + dataDictionary.SiteGuid.ToString());

			if (keyDictionary != null)
			{
				keyDictionary[dataDictionary.Key] = dataDictionary.Value;
			}

			entityToSiteMaps = new EntityToSiteMaps();
			entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(security, dataDictionary.EntityType, dataDictionary.SiteGuid);

			foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
			{
				keyDictionary = (DataDictionaryCollectionClass)domain.GetData("KeyDictionary" + entityToSiteMap.SiteGuid.ToString());

				if (keyDictionary != null)
				{
					keyDictionary[dataDictionary.Key] = dataDictionary.Value;
				}
			}
		}

		/// <summary>
		/// Purges the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="key">The key.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, string key)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			var dataDictionary = new DataDictionaryClass();

			// If EntityAssignmentMap exists it do not allow modification
			var entityToSiteMaps = new EntityToSiteMaps();

			var entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndSiteGuid(
				security, dataDictionary.EntityType, security.SiteGuid);

			if (entityToSiteMapCollection.Count > 0)
			{
				throw new Exception("Dictionary Assigned");
			}

			dataDictionary.SiteGuid = security.SiteGuid;
			dataDictionary.Key = key;

			using (var cmd = new SqlCommand())
			{
				dataDictionary.PurgeSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			// Update all instances of DataDictionary in AppDomain
			AppDomain domain = AppDomain.CurrentDomain;

			var keyDictionary = (DataDictionaryCollectionClass)domain.GetData("KeyDictionary" + security.SiteGuid.ToString());
			var guidDictionary = (Dictionary<Guid, string>)domain.GetData("GuidDictionary" + security.SiteGuid.ToString());
			if (keyDictionary != null)
			{
				if (keyDictionary.Contains(key))
				{
					keyDictionary.Remove(key);
				}
			}

			if(guidDictionary != null)
			{
				if (guidDictionary.ContainsValue(key))
				{
					var guid = guidDictionary.FirstOrDefault(x => x.Value == key).Key;
					if (guid != null)
					{
						guidDictionary.Remove(guid);
					}
				}
			}

			entityToSiteMaps = new EntityToSiteMaps();
			entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
			security, dataDictionary.EntityType, security.SiteGuid);

			foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
			{
				keyDictionary = (DataDictionaryCollectionClass)domain.GetData("KeyDictionary" + entityToSiteMap.SiteGuid.ToString());
				guidDictionary = (Dictionary<Guid, string>)domain.GetData("GuidDictionary" + entityToSiteMap.SiteGuid.ToString());

				if (keyDictionary != null)
				{
					if (keyDictionary.Contains(key))
					{
						keyDictionary.Remove(key);
					}
				}

				if (guidDictionary != null)
				{
					if (guidDictionary.ContainsValue(key))
					{
						var guid = guidDictionary.FirstOrDefault(x => x.Value == key).Key;
						if (guid != null)
						{
							guidDictionary.Remove(guid);
						}
					}
				}

			}
		}

        /// <summary>
        /// This method will start the process of importing the data dictionary items. Some items will be
        /// added, modified, or deleted.
        /// </summary>
        /// <param name="security">The security object.</param>
        /// <param name="addList">The add list.</param>
        /// <param name="modList">The modify list.</param>
        /// <param name="delList">The delete list.</param>
        public void ImportData(SecurityClass security, List<DataDictionaryClass> addList, List<DataDictionaryClass> modList, List<DataDictionaryClass> delList)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
            {
                throw new FMInsufficientRightsException();
            }

            // If EntityAssignmentMap exists it do not allow modification
            var entityToSiteMaps = new EntityToSiteMaps();
            var dataDictionary = new DataDictionaryClass();

            var entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndSiteGuid(security, dataDictionary.EntityType, security.SiteGuid);

            if (entityToSiteMapCollection.Count > 0)
            {
                throw new Exception("Dictionary Assigned");
            }

            using (var cmd = new SqlCommand())
            {
                dataDictionary.ImportDataSql(security, cmd, addList, modList, delList);
                this.ConsolidatedDA.ExecuteQuery(security, cmd);
            }

            var domain = AppDomain.CurrentDomain;
            string dictionaryKeyName = "Dictionary" + security.SiteGuid.ToString();

            // Update instance in application domain.
            var dictionaryCollection = this.Enumerate(security);
            domain.SetData(dictionaryKeyName, dictionaryCollection);

            entityToSiteMaps = new EntityToSiteMaps();
            entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(security, dataDictionary.EntityType, security.SiteGuid);

            foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
            {
                dictionaryKeyName = "Dictionary" + entityToSiteMap.SiteGuid.ToString();
                domain.SetData(dictionaryKeyName, dictionaryCollection);
            }
        }
		#endregion

		#region Private methods
		/// <summary>
		/// Enumerates from updated date time.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="rowVersion">The most recent row version.</param>
		/// <returns>A collection of data dictionary keys.</returns>
		private Dictionary<string, DataDictionaryClass> EnumerateFromRowVersion(SecurityClass security, long rowVersion)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			byte[] convertedRowVersion = this.SwapBytes(rowVersion);

			var dataDictionary = new DataDictionaryClass { SiteGuid = security.SiteGuid, RowVersion = convertedRowVersion };
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				dataDictionary.EnumerateSQLFromRowVersion(cmd);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];
			var dataDictionaryDictionary = new Dictionary<string, DataDictionaryClass>(table.Rows.Count);

			while (table.Rows.Count != 0)
			{
				var dataDictionaryClass = new DataDictionaryClass();
				dataDictionaryClass.Load(set);
				dataDictionaryDictionary.Add(dataDictionaryClass.Key, dataDictionaryClass);
				table.Rows.RemoveAt(0);
			}

			return dataDictionaryDictionary;
		}

		/// <summary>
		/// Enumerates from updated date time.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="rowVersion">The most recent row version.</param>
		/// <returns>A list of data dictionary guids.</returns>
		private List<KeyValuePair<Guid, byte[]>> EnumerateDeletedFromRowVersion(SecurityClass security, long rowVersion)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			byte[] convertedRowVersion = this.SwapBytes(rowVersion);

			var dataDictionary = new DataDictionaryClass { SiteGuid = security.SiteGuid, RowVersion = convertedRowVersion };
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				dataDictionary.EnumerateSQLDeletedFromRowVersion(cmd);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];
			var dataDictionaryList = new List<KeyValuePair<Guid, byte[]>>(table.Rows.Count);

			foreach (DataRow row in table.Rows)
			{
				dataDictionaryList.Add(new KeyValuePair<Guid, byte[]> (DataObject.getValue<Guid>(row["PK_DataDictionaryGuid"], Guid.Empty), DataObject.getValue<byte[]>(row["RowVersion"], null)));
			}

			return dataDictionaryList;
		}


		/// <summary>
		/// This method will reverse the byte order for SQL.
		/// </summary>
		/// <param name="rowVersion">The row version as a long.</param>
		/// <returns>Returns a byte array.</returns>
		private byte[] SwapBytes(long rowVersion)
        {
			byte[] convertedRowVersion = BitConverter.GetBytes(rowVersion);
			byte[] swappedBytes = new byte[8];

			if (convertedRowVersion.Length == 8)
			{
				swappedBytes[7] = convertedRowVersion[0];
				swappedBytes[6] = convertedRowVersion[1];
				swappedBytes[5] = convertedRowVersion[2];
				swappedBytes[4] = convertedRowVersion[3];
				swappedBytes[3] = convertedRowVersion[4];
				swappedBytes[2] = convertedRowVersion[5];
				swappedBytes[1] = convertedRowVersion[6];
				swappedBytes[0] = convertedRowVersion[7];
			}
			else if (convertedRowVersion.Length == 4)
			{
				swappedBytes[7] = 0;
				swappedBytes[6] = 0;
				swappedBytes[5] = 0;
				swappedBytes[4] = 0;
				swappedBytes[3] = convertedRowVersion[0];
				swappedBytes[2] = convertedRowVersion[1];
				swappedBytes[1] = convertedRowVersion[2];
				swappedBytes[0] = convertedRowVersion[3];
			}

			return swappedBytes;
		}
		#endregion

		#region Explicit Interface Methods

		/// <summary>
		/// Inserts the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="baseObject">The object.</param>
		/// <param name="preOperation"></param>
		void IDependency.Insert(SecurityClass security, BaseDataObject baseObject, bool preOperation)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (baseObject == null)
			{
				throw new ArgumentNullException("baseObject");
			}

			if (preOperation && baseObject is EntityToSiteMapClass)
			{
				var entityToSiteMap = (EntityToSiteMapClass)baseObject;

				// Verify there is no DataDictionary owned by this Site
				var dataDictionary = new DataDictionaryClass();
				if (entityToSiteMap.TypeID == dataDictionary.EntityType)
				{
					var dataDictionaryCollection = this.Enumerate(security);
					if (dataDictionaryCollection.Count != 0)
					{
						throw new Exception("DataDictionary Exist - " + entityToSiteMap.ID);
					}
				}
			}
		}

		/// <summary>
		/// Purges the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="baseObject">The object.</param>
		void IDependency.Purge(SecurityClass security, BaseDataObject baseObject)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (baseObject == null)
			{
				throw new ArgumentNullException("baseObject");
			}

			if (baseObject is SiteClass)
			{
				var site = (SiteClass)baseObject;
				var entityToSiteMaps = new EntityToSiteMaps();
				var dataDictionary = new DataDictionaryClass();

				var entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
					security, dataDictionary.EntityType, site.IdentityGuid);

				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMaps.Purge(security, entityToSiteMap);
				}

				var dataDictionaryCollection = this.EnumerateBySite(security);
				ICollection keys = dataDictionaryCollection.Keys;

				foreach (string key in keys)
				{
					this.Purge(security, key);
				}
			}
		}

		/// <summary>
		/// Updates the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="baseObject">The object.</param>
		void IDependency.Update(SecurityClass security, BaseDataObject baseObject)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (baseObject == null)
			{
				throw new ArgumentNullException("baseObject");
			}

			// See Sites.Modify, this call only occurs with SiteGroup is changed.
			if (baseObject is SiteClass)
			{
				var site = (SiteClass)baseObject;
				var dataDictionary = new DataDictionaryClass();
				var entityToSiteMaps = new EntityToSiteMaps();

				var entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
					security, dataDictionary.EntityType, site.IdentityGuid);

				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMaps.Purge(security, entityToSiteMap);
				}
			}
		}

		#endregion
	}
}
