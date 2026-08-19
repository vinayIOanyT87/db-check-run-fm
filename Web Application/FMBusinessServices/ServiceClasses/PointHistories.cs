// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PointGroups.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Service providing access to point group data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace FMBusinessServices.ServiceClasses
{

	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Security;
	using System.ServiceModel;
	using System.Web;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	/// <summary>
	/// Service providing access to point history configuration data.
	/// </summary>
	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class PointHistories : FMServiceBase, IPointHistories, IDependency
	{
		#region Private data members
		private readonly ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public PointHistories()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}
        #endregion

        public PointHistoryCollectionClass EnumerateBySite(SecurityClass security, Guid siteGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            using (var cmd = new SqlCommand())
            {
                var pointHistory = new PointHistory();
                pointHistory.EnumerateBySiteSQL(cmd, security, siteGuid, ContextUtil.IsInTransaction);
                DataSet set = this.consolidatedDA.GetDataSet(cmd, security);

                var toRet = new PointHistoryCollectionClass();

                var table = set.Tables[0];
                while (table.Rows.Count != 0)
                {
                    var pntHistory = new PointHistory();
                    pntHistory.AutoLoad(table.Rows[0]);
                    toRet.Add(pntHistory);
                    table.Rows.RemoveAt(0);
                }

                return toRet;
            }
        }
        public PointHistoryCollectionClass EnumerateByUserSite(SecurityClass security, Guid userGuid, Guid siteGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            using (var cmd = new SqlCommand())
            {
                var pointHistory = new PointHistory();
                pointHistory.EnumerateByUserSiteSQL(cmd, security, userGuid, siteGuid, ContextUtil.IsInTransaction);
                DataSet set = this.consolidatedDA.GetDataSet(cmd, security);

                var toRet = new PointHistoryCollectionClass();

                var table = set.Tables[0];
                while (table.Rows.Count != 0)
                {
                    var pntHistory = new PointHistory();
                    pntHistory.AutoLoad(table.Rows[0]);
                    toRet.Add(pntHistory);
                    table.Rows.RemoveAt(0);
                }

                return toRet;
            }
        }


        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Add(SecurityClass security, PointHistory pointHistory)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (pointHistory == null)
            {
                throw new ArgumentNullException("pointHistory");
            }

            using (var cmd = new SqlCommand())
            {
                pointHistory.SetCreationStamp(security);
                pointHistory.AutoGenerateInsertProcSQL(cmd, "usp_PointHistoryInsert");

                this.consolidatedDA.ExecuteQuery(security, cmd);
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Modify(SecurityClass security, PointHistory pointHistory)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (pointHistory == null)
            {
                throw new ArgumentNullException("pointHistory");
            }

            using (var cmd = new SqlCommand())
            {
                pointHistory.SetCreationStamp(security);
                pointHistory.AutoGenerateModifyProcSQL(cmd, "usp_PointHistoryUpdateByUserSite");

                this.consolidatedDA.ExecuteQuery(security, cmd);
            }
        }

        public PointHistory Get(SecurityClass security, Guid userGuid, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (userGuid == null)
			{
				throw new ArgumentNullException("userGuid");
			}

			if (siteGuid == null)
			{
				throw new ArgumentNullException("siteGuid");
			}

			var pointHistory = new PointHistory();
			DataSet set;
			// get the main PointHistory data
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "dbo.usp_PointHistoryGetByUserSite";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@userGuid", userGuid);
				cmd.Parameters.AddWithValue("@siteGuid", siteGuid);

				this.consolidatedDA.ExecuteQuery(security, cmd);

				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			if (table.Rows.Count > 0)
			{
				pointHistory.AutoLoad(table.Rows[0]);
			}

			return pointHistory;

		}

        #region Explicit Interface Methods

        void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (Object == null)
            {
                throw new ArgumentNullException("Object");
            }
        }

        void IDependency.Purge(SecurityClass security, BaseDataObject Object)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (Object == null)
            {
                throw new ArgumentNullException("Object");
            }

            // Purge Site
            if (typeof(SiteClass).IsInstanceOfType(Object))
            {
                var site = Object as SiteClass;
                var pointHistoryCollection = this.EnumerateBySite(security, site.IdentityGuid);

                foreach (PointHistory pointHistory in pointHistoryCollection)
                {
                    using (var cmd = new SqlCommand())
                    {
                        pointHistory.PurgeSQL(cmd);
                        this.consolidatedDA.ExecuteQuery(security, cmd);
                    }
                }
            }
            else if (typeof(EntityToSiteMapClass).IsInstanceOfType(Object))
            {
                var entityToSiteMap = Object as EntityToSiteMapClass;
                PointHistoryCollectionClass pointHistoryCollection = null;
                if (entityToSiteMap.TypeID == ENTITY_TYPE.USER)
                {
                    pointHistoryCollection = this.EnumerateByUserSite(security, entityToSiteMap.IdentityGuid, entityToSiteMap.SiteGuid);
                }

                if (pointHistoryCollection != null)
                {
                    foreach (PointHistory pointHistory in pointHistoryCollection)
                    {
                        using (var cmd = new SqlCommand())
                        {
                            pointHistory.PurgeSQL(cmd);
                            this.consolidatedDA.ExecuteQuery(security, cmd);
                        }
                    }
                }
            }
        }

        void IDependency.Update(SecurityClass security, BaseDataObject Object)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (Object == null)
            {
                throw new ArgumentNullException("Object");
            }

        }

        #endregion
    }
}