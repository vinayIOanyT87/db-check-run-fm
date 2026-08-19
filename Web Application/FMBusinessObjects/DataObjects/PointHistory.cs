using FMBusinessObjects.Attributes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects
{
    [Serializable]
    [CollectionDataContract]
    public class PointHistoryCollectionClass : List<PointHistory>
    {
    }

    public class PointHistory : BaseDataObject
    {
        #region Constructors
        public PointHistory()
        {
            this.Init();
        }
        #endregion

        #region Properties
        [FMPersistedField]
        public Guid PointHistoryGuid
        {
            get { return this.IdentityGuid; }
            set { this.IdentityGuid = value; }
        }

        [DataMember]
        [FMPersistedField]
        public Guid UserGuid { get; set; }

        [DataMember]
        [FMPersistedField]
        public DateTimeOffset StartDate { get; set; }

        [DataMember]
        [FMPersistedField]
        public int IntervalQuantity { get; set; }

        [DataMember]
        [FMPersistedField]
        public int IntervalType { get; set; }

        [DataMember]
        [FMPersistedField]
        public int RangeQuantity { get; set; }

        [DataMember]
        [FMPersistedField]
        public int RangeType { get; set; }

        [DataMember]
        [FMPersistedField]
        public string ColumnsDefinition { get; set; }
        #endregion

        #region SQL
        public void EnumerateBySiteSQL(SqlCommand cmd, SecurityClass security, Guid siteGuid, bool bInTransaction)
        {
            cmd.CommandText = "SELECT [PointHistoryGuid], [SiteGuid], [UserGuid], [StartDate], [IntervalQuantity], [IntervalType], [RangeQuantity], [RangeType], [ColumnsDefinition] "
		                    + "FROM tblPointHistory WHERE SiteGuid = @SiteGuid";

            cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
        }

        public void EnumerateByUserSiteSQL(SqlCommand cmd, SecurityClass security, Guid userGuid, Guid siteGuid, bool bInTransaction)
        {
            cmd.CommandText = "SELECT [PointHistoryGuid], [SiteGuid], [UserGuid], [StartDate], [IntervalQuantity], [IntervalType], [RangeQuantity], [RangeType], [ColumnsDefinition] "
                            + "FROM tblPointHistory WHERE UserGuid = @UserGuid AND SiteGuid = @SiteGuid";

            cmd.Parameters.AddWithValue("@UserGuid", userGuid);
            cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
        }

        public void PurgeSQL (SqlCommand cmd) {
            cmd.CommandText = "DELETE FROM dbo.tblPointHistory WHERE SiteGuid = @SiteGuid AND UserGuid = @UserGuid ";
            cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
            cmd.Parameters.AddWithValue("@UserGuid", this.UserGuid);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method will initialize the object to its initial state.
        /// </summary>
        private void Init()
        {
            base.Reset();
            this.PointHistoryGuid = Guid.Empty;
            this.UserGuid = Guid.Empty;
            this.StartDate = DateTimeOffset.MinValue;
            this.IntervalQuantity = 1;
            this.IntervalType = 1;
            this.RangeQuantity = 1;
            this.RangeType = 1;
        }
        #endregion
    }
}