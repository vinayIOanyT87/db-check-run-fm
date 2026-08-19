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

    using IsolationLevel = System.Transactions.IsolationLevel;

    [SecuritySafeCritical]
    [ServiceBehavior(TransactionIsolationLevel = IsolationLevel.ReadCommitted)]
    public class VruTrackings : IVruTrackings
    {
        private readonly ConsolidatedDAClass consolidatedDA = new ConsolidatedDAClass();

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public Guid Add(SecurityClass security, VruTrackingClass vcu)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (vcu == null)
            {
                throw new ArgumentNullException(nameof(vcu));
            }

            if (!security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
            {
                throw new FMInsufficientRightsException("Access Denied");
            }

            vcu.SiteGuid = security.SiteGuid;
            vcu.CreatedDate = DateTime.UtcNow;
            vcu.CreatedBy = security.UserID;
            vcu.UpdatedDate = vcu.CreatedDate;
            vcu.UpdatedBy = security.UserID;

            SqlCommand cmd = new SqlCommand();
            vcu.InsertSql(cmd);
            vcu.IdentityGuid = (Guid)this.consolidatedDA.ExecuteQuery(security, cmd, ConsolidatedDAClass.Uniquifier).Tables[0].Rows[0][0];

            return vcu.IdentityGuid;
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Modify(SecurityClass security, VruTrackingClass vcu)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (vcu == null)
            {
                throw new ArgumentNullException(nameof(vcu));
            }

            if (!security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
            {
                throw new FMInsufficientRightsException("Access Denied");
            }

            VruTrackingClass oldVruTrackingTrackingConfiguration = this.Get(security, vcu.IdentityGuid);
            if (oldVruTrackingTrackingConfiguration.IdentityGuid == Guid.Empty)
            {
                throw new Exception("VRU Tracking Entry Not Found");
            }

            vcu.UpdatedDate = DateTime.UtcNow;
            vcu.UpdatedBy = security.UserID;

            SqlCommand cmd = new SqlCommand();
            vcu.UpdateSqlCmd(cmd);
            this.consolidatedDA.ExecuteQuery(security, cmd);
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void UpdateResetDate(SecurityClass security, Guid vruTrackingGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (vruTrackingGuid == Guid.Empty)
            {
                throw new Exception("VRU Threshold Not Found");
            }

            if (!security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
            {
                throw new FMInsufficientRightsException("Access Denied");
            }

            VruTrackingClass oldVru = this.Get(security, vruTrackingGuid);
            if (oldVru.IdentityGuid == Guid.Empty)
            {
                throw new Exception("VRU Tracking Not Found");
            }

            oldVru.UpdatedDate = DateTime.UtcNow;
            oldVru.UpdatedBy = security.UserID;

            SqlCommand cmd = new SqlCommand();
            oldVru.ResetDateSql(cmd);
            this.consolidatedDA.ExecuteQuery(security, cmd);
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Purge(SecurityClass security, Guid vruTrackingGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (!security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
            {
                throw new FMInsufficientRightsException("Access Denied");
            }

            VruTrackingClass oldVruTrackingTrackingConfiguration = this.Get(security, vruTrackingGuid);
            if (oldVruTrackingTrackingConfiguration.IdentityGuid == Guid.Empty)
            {
                throw new Exception("Message Not Found");
            }

            SqlCommand cmd = new SqlCommand();
            oldVruTrackingTrackingConfiguration.PurgeSql(cmd);
            this.consolidatedDA.ExecuteQuery(security, cmd);
        }

        public VruTrackingClass Get(SecurityClass security, Guid vruTrackingGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (!security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA) && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
            {
                throw new FMInsufficientRightsException("Access Denied");
            }

            var sites = new SitesClass();
            var site = sites.Get(security, security.SiteGuid, false, false, false);

            var vruTracking = new VruTrackingClass { IdentityGuid = vruTrackingGuid, SiteGuid = security.SiteGuid };

            SqlCommand cmd = new SqlCommand();
            vruTracking.SelectSql(cmd);
            vruTracking.Load(
                this.consolidatedDA.GetDataSet(cmd, security),
                security, site);

            return vruTracking;
        }

        public bool IsThresholdExceeded(SecurityClass security)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (!security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA) && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
            {
                throw new FMInsufficientRightsException("Access Denied");
            }

            var vruTracking = new VruTrackingClass { SiteGuid = security.SiteGuid };

            SqlCommand cmd = new SqlCommand();
            vruTracking.IsThresholdExceededSql(cmd);
            var dataSet = this.consolidatedDA.GetDataSet(cmd, security);

            return dataSet.Tables[0].Rows.Count > 0;
        }

        public VRUTrackingCollectionClass Enumerate(SecurityClass security)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (!security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA) && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
            {
                throw new FMInsufficientRightsException("Access Denied");
            }

            var vruTracking = new VruTrackingClass();
            SqlCommand cmd = new SqlCommand();
            vruTracking.EnumerateSql(security, cmd);
            DataSet set = this.consolidatedDA.GetDataSet(cmd, security);
            var vruCollection = new VRUTrackingCollectionClass();

            var sites = new SitesClass();
            var site = sites.Get(security, security.SiteGuid, false, false, false);

            DataTable table = set.Tables[0];
            while (table.Rows.Count != 0)
            {
                vruTracking = new VruTrackingClass();
                vruTracking.Load(set, security, site);
                vruCollection.Add(vruTracking);
                table.Rows.RemoveAt(0);
            }

            return vruCollection;
        }

        public VRUTrackingCollectionClass EnumerateThresholdsExceeded(SecurityClass security)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (!security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA) && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
            {
                throw new FMInsufficientRightsException("Access Denied");
            }

            var vruTracking = new VruTrackingClass();
            SqlCommand cmd = new SqlCommand();
            vruTracking.EnumerateThresholdExceededSql(security, cmd);
            DataSet set = this.consolidatedDA.GetDataSet(cmd, security);
            var vruCollection = new VRUTrackingCollectionClass();

            var sites = new SitesClass();
            var site = sites.Get(security, security.SiteGuid, false, false, false);

            DataTable table = set.Tables[0];
            while (table.Rows.Count != 0)
            {
                vruTracking = new VruTrackingClass();
                vruTracking.Load(set, security, site);
                vruCollection.Add(vruTracking);
                table.Rows.RemoveAt(0);
            }

            return vruCollection;
        }

        public void CalculateRunningTotals(SecurityClass security)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            var vruTracking = new VruTrackingClass();
            SqlCommand cmd = new SqlCommand();
            vruTracking.CalculateCurrentValueSql(cmd);
            this.consolidatedDA.ExecuteQuery(security, cmd);
        }

        public void AggregateDailyBolTotals(SecurityClass security)
        {
            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            var vruTracking = new VruTrackingClass();
            SqlCommand cmd = new SqlCommand();
            vruTracking.AggregateDailyBolSql(cmd);
            this.consolidatedDA.ExecuteQuery(security, cmd);
        }

        public int? GetAutoCalculationInterval(SecurityClass security)
        {
            int? interval = null;

            if (security == null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            var vruTracking = new VruTrackingClass();
            SqlCommand cmd = new SqlCommand();
            vruTracking.GetAutoCalculateScheduleSql(cmd);
            DataSet set = this.consolidatedDA.GetDataSet(cmd, security);

            DataTable table = set.Tables[0];
            if (table.Rows.Count != 0)
            {
                // the query returns only one column, the number of minutes
                interval = int.Parse(table.Rows[0][0].ToString());
            }

            return interval;
        }
    }
}