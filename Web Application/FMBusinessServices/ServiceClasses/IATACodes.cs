namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	using IsolationLevel = System.Transactions.IsolationLevel;

	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = IsolationLevel.ReadCommitted)]
	public class IATACodesClass : FMServiceBase, IDependency, IIATACodes
	{
		#region Private data members
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		public IATACodesClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, IATACodeClass IATACode)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (IATACode == null)
			{
				throw new ArgumentNullException("IATACode");
			}

			this.Validate(security, IATACode);

			if (!security.HasRight(RIGHT.MODIFY_TICKETING_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			if (GetIdentityGuid(security, IATACode.ID) != Guid.Empty)
			{
				throw (new Exception("Delivery Location Exists"));
			}

			IATACode.SiteGuid = security.SiteGuid;
			IATACode.CreatedDate = DateTimeOffset.Now;
			IATACode.CreatedBy = security.UserID;
			IATACode.UpdatedDate = IATACode.CreatedDate;
			IATACode.UpdatedBy = security.UserID;
			IATACode.IdentityGuid = Guid.NewGuid();

			using (SqlCommand cmd = new SqlCommand())
			{
				IATACode.InsertSQL(cmd);
				consolidatedDA.ExecuteQuery(security, cmd);
			}

			// Create Entity to Site Map
			EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapClass EntityToSiteMap = new EntityToSiteMapClass(IATACode);
			EntityToSiteMaps.Add(security, EntityToSiteMap, GetType().GUID);

			return IATACode.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, IATACodeClass IATACode)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (IATACode == null)
			{
				throw new ArgumentNullException("IATACode");
			}

            this.Validate(security, IATACode);

			if (!security.HasRight(RIGHT.MODIFY_TICKETING_DATA) &&
				!security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT))
			{
				throw new FMInsufficientRightsException();
			}

			Guid identityGuid = GetIdentityGuid(security, IATACode.ID);

			if ((identityGuid != Guid.Empty) && (identityGuid != IATACode.IdentityGuid))
			{
				throw (new Exception("Delivery Location Exists"));
			}

			IATACodeClass OldIATACode = Get(security, IATACode.IdentityGuid);

			if (OldIATACode.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("Delivery Location Not Found"));
			}

			IATACode.UpdatedDate = DateTimeOffset.Now;
			IATACode.UpdatedBy = security.UserID;
			using (SqlCommand cmd = IATACode.UpdateSQL)
			{
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

			EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass EntityToSiteMapCollection = EntityToSiteMaps.EnumerateByTypeIDAndGuid(security, IATACode.EntityType, IATACode.IdentityGuid);

			if (IATACode.SiteGuid != OldIATACode.SiteGuid)
			{
				// Purge from EntityToSiteMap
				foreach (EntityToSiteMapClass EntityToSiteMap in EntityToSiteMapCollection)
				{
					EntityToSiteMap.ID = IATACode.ID;
					EntityToSiteMaps.Purge(security, EntityToSiteMap);
				}

				// Create Entity to Site Map
				EntityToSiteMapClass NewEntityToSiteMap = new EntityToSiteMapClass(IATACode);
				EntityToSiteMaps.Add(security, NewEntityToSiteMap, GetType().GUID);
			}
		}

		public IATACodeClass Get(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (!security.HasRight(RIGHT.VIEW_TICKETING_DATA) &&
				!security.HasRight(RIGHT.MODIFY_TICKETING_DATA) &&
				!security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) &&
				!security.HasRight(RIGHT.MODIFY_DISPATCH) &&
				!security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			IATACodeClass IATACode = new IATACodeClass();
			IATACode.IdentityGuid = identityGuid;
			using (SqlCommand cmd = IATACode.SelectSQL(ContextUtil.IsInTransaction))
			{
				IATACode.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			return IATACode;
		}


		public Guid GetIdentityGuid(SecurityClass security, string IATACodeID)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (!security.HasRight(RIGHT.VIEW_TICKETING_DATA) &&
				!security.HasRight(RIGHT.MODIFY_TICKETING_DATA) &&
				!security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) &&
				!security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) &&
				!security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) &&
				!security.HasRight(RIGHT.VIEW_BILLS_OF_LADING) &&
				!security.HasRight(RIGHT.MODIFY_DISPATCH) &&
				!security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			IATACodeClass IATACode = new IATACodeClass();
			IATACode.SiteGuid = security.SiteGuid;
			IATACode.ID = IATACodeID;
			using (SqlCommand cmd = IATACode.SelectByIDSQL(security, ContextUtil.IsInTransaction))
			{
				IATACode.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			return IATACode.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (!security.HasRight(RIGHT.MODIFY_TICKETING_DATA) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			IATACodeClass IATACode = Get(security, identityGuid);

			if (IATACode.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("Delivery Location Not Found"));
			}

			// Purge from EntityToSiteMap
			EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();

			EntityToSiteMapCollectionClass EntityToSiteMapCollection = EntityToSiteMaps.EnumerateByTypeIDAndGuid(security, IATACode.EntityType, identityGuid);
			foreach (EntityToSiteMapClass EntityToSiteMap in EntityToSiteMapCollection)
			{
				EntityToSiteMap.ID = IATACode.ID;
				EntityToSiteMaps.Purge(security, EntityToSiteMap);
			}

			using (SqlCommand cmd = IATACode.PurgeSQL)
			{
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

        public DataSet EnumerateWithFilter(SecurityClass security, string filterString)
	    {
            IATACodeClass IATACode = new IATACodeClass();

            DataSet dataSet = null;
            using (SqlCommand cmd = IATACode.EnumerateSQL(security, filterString))
            {
                dataSet = this.consolidatedDA.GetDataSet(cmd, security);
            }
            return dataSet;
	    }

		public IATACodeCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (!security.HasRight(RIGHT.VIEW_TICKETING_DATA)
				&& !security.HasRight(RIGHT.MODIFY_TICKETING_DATA)
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.MODIFY_FINANCIAL_DATA)
				&& !security.HasRight(RIGHT.VIEW_FINANCIAL_DATA)
				&& !security.HasRight(RIGHT.MODIFY_STANDING_OFFERS)
				&& !security.HasRight(RIGHT.VIEW_STANDING_OFFERS)
				&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.VIEW_DISPATCH)
				&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.EXPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

		    DataSet dataSet = EnumerateWithFilter(security, string.Empty);

            IATACodeClass IATACode = new IATACodeClass();
            IATACodeCollectionClass IATACodeCollection = new IATACodeCollectionClass();

			if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
			{
				DataTable table = dataSet.Tables[0];

				if (table.Rows != null)
				{
					while (table.Rows.Count != 0)
					{
						IATACode = new IATACodeClass();
						IATACode.Load(dataSet);
						IATACodeCollection.Add(IATACode);
						table.Rows.RemoveAt(0);
					}
				}
			}

			return IATACodeCollection;
		}

		/// <summary>
		/// This method will get all the IATA that have coordinates.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>Returns a collection of IATA code that have coordinates.</returns>
		public IATACodeCollectionClass EnumerateWhereCoordinatesExist(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_TICKETING_DATA)
				&& !security.HasRight(RIGHT.MODIFY_TICKETING_DATA)
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.MODIFY_FINANCIAL_DATA)
				&& !security.HasRight(RIGHT.VIEW_FINANCIAL_DATA)
				&& !security.HasRight(RIGHT.MODIFY_STANDING_OFFERS)
				&& !security.HasRight(RIGHT.VIEW_STANDING_OFFERS)
				&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.VIEW_DISPATCH)
				&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.EXPORT_ENTERPRISE_DATA)
				&& !security.HasRight(RIGHT.VIEW_MAPS)
				&& !security.HasRight(RIGHT.VIEW_MAP_CONFIGURATION)
				&& !security.HasRight(RIGHT.MODIFY_MAP_CONFIGURATION))
			{
				throw new FMInsufficientRightsException();
			}

			var iataCode = new IATACodeClass();
			DataSet dataSet;

			using (SqlCommand cmd = iataCode.EnumerateWhereCoordinateSQL(security))
			{
				dataSet = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var iataCodeCollection = new IATACodeCollectionClass();

			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				DataTable table = dataSet.Tables[0];

				while (table.Rows.Count != 0)
				{
					iataCode = new IATACodeClass();
					iataCode.Load(dataSet);
					iataCodeCollection.Add(iataCode);
					table.Rows.RemoveAt(0);
				}
			}

			return iataCodeCollection;
		}

		public IATACodeCollectionClass EnumerateByPrefix(SecurityClass security, string Prefix)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (!security.HasRight(RIGHT.VIEW_TICKETING_DATA) && !security.HasRight(RIGHT.MODIFY_TICKETING_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			IATACodeClass iataCode = new IATACodeClass();
			DataSet dataSet = null;
			using (SqlCommand cmd = iataCode.EnumerateByPrefixSQL(security, Prefix))
			{
				dataSet = this.consolidatedDA.GetDataSet(cmd, security);
			}
			IATACodeCollectionClass iataCodeCollection = new IATACodeCollectionClass();

			if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
			{
				DataTable table = dataSet.Tables[0];

				if (table.Rows != null)
				{
					while (table.Rows.Count != 0)
					{
						iataCode = new IATACodeClass();
						iataCode.Load(dataSet);
						iataCodeCollection.Add(iataCode);
						table.Rows.RemoveAt(0);
					}
				}
			}

			return iataCodeCollection;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Import(SecurityClass security, IATACodeClass IATACode)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (IATACode == null)
			{
				throw new ArgumentNullException("IATACode");
			}

			try
			{
				IATACode.IdentityGuid = GetIdentityGuid(security, IATACode.ID);

				// If the entity exists and is not owned by this site, do not update it.
				if ((IATACode.IdentityGuid != Guid.Empty) && (Get(security, IATACode.IdentityGuid).SiteGuid != security.SiteGuid))
				{
					return;
				}

				if (IATACode.IdentityGuid == Guid.Empty)
				{
					this.Add(security, IATACode);
				}
				else
				{
					this.Modify(security, IATACode);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("[Delivery Location Import Error ID] : " + IATACode.ID + ", " + ex.Message);
			}
		}

		#region Private methods
		private void Validate(SecurityClass security, IATACodeClass IATACode)
		{
			if (IATACode.ID == "")
			{
				throw (new Exception("ID Required"));
			}

			if (IATACode.ID == "{None}" || IATACode.ID == "{Unassigned}" || IATACode.ID == "{All}")
			{
				throw new Exception("ID is reserved key word " + IATACode.ID);
			}

			if (IATACode.ID[0] < 'A' || IATACode.ID[0] > 'Z')
			{
				throw new Exception("ID must begin with Capital Alphabetic character");
			}

            this.ValidateUserData(security, IATACode);
        }
		#endregion

		#region Dependency methods
		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (Object == null)
				throw new ArgumentNullException("Object");
		}

		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (Object == null)
				throw new ArgumentNullException("Object");

			if (typeof(SiteClass).IsInstanceOfType(Object))
			{
				SiteClass Site = (SiteClass)Object;
				IATACodeCollectionClass IATACodeCollection = Enumerate(security);
				EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();
				foreach (IATACodeClass IATACode in IATACodeCollection)
				{
					if (Site.SiteGuid == IATACode.SiteGuid)
					{
						EntityToSiteMapCollectionClass EntityToSiteMapCollection = EntityToSiteMaps.EnumerateByTypeIDAndGuid(security, IATACode.EntityType, IATACode.IdentityGuid);
						foreach (EntityToSiteMapClass EntityToSiteMap in EntityToSiteMapCollection)
						{
							if (EntityToSiteMap.SiteGuid != Site.SiteGuid)
							{
								EntityToSiteMap.ID = IATACode.ID;
								EntityToSiteMaps.Purge(security, EntityToSiteMap);
							}
						}
					}
				}
			}

		}

		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (Object == null)
				throw new ArgumentNullException("Object");

			// Purge IATACodes
			if (typeof(SiteClass).IsInstanceOfType(Object))
			{
				SiteClass Site = (SiteClass)Object;
				IATACodeCollectionClass IATACodeCollection = Enumerate(security);
				EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();
				foreach (IATACodeClass IATACode in IATACodeCollection)
				{
					if (Site.SiteGuid == IATACode.SiteGuid)
						Purge(security, IATACode.IdentityGuid);
					else
					{
						EntityToSiteMapClass EntityToSiteMap = new EntityToSiteMapClass(IATACode);
						EntityToSiteMap.SiteGuid = Site.SiteGuid;
						EntityToSiteMaps.Purge(security, EntityToSiteMap);
					}
				}
			}
		}
		#endregion
	}
}