using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects
{
    [DataContract]
    [Serializable]
    public class ErrorTransactionSubmissionClass : BaseDataObject
    {
        [DataMember]
        public string TransactionSubmissionInformation { get; set; }
        [DataMember]
        public Guid SubmittedUserGuid { get; set; }
        [DataMember]
        public Guid SubmittedSiteGuid { get; set; }


        public override void Reset()
        {
            base.Reset();
            this.TransactionSubmissionInformation = "";
        }

        public void AddSQL(SqlCommand cmd, bool bInTransaction = false)
        {
            cmd.CommandText = @"usp_ErrorTransactionSubmissionInsert";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@SubmittedUserGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@SubmittedSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@TransactionSubmissionInformation", SqlDbType.NVarChar);
            cmd.Parameters["@SubmittedUserGuid"].Value = this.SubmittedUserGuid;
            cmd.Parameters["@SubmittedSiteGuid"].Value = this.SubmittedSiteGuid;
            cmd.Parameters["@TransactionSubmissionInformation"].Value = this.TransactionSubmissionInformation;

            cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
            cmd.Parameters["@CreatedBy"].Value = this.CreatedBy;
            cmd.Parameters["@UpdatedBy"].Value = this.UpdatedBy;
        }

        public void SelectSQL(SqlCommand cmd, bool bInTransaction = false)
        {
            throw new NotImplementedException();
        }

        public void SelectSQLByUser(SqlCommand cmd, bool bInTransaction = false)
        {
            cmd.CommandText = "SELECT * FROM tblErrorTransactionSubmissions " + SQLUpdateLock(bInTransaction) + " WHERE SubmittedUserGuid = @SubmittedUserGuid";
            cmd.Parameters.Add("@SubmittedUserGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@SubmittedUserGuid"].Value = this.SubmittedUserGuid;
        }

        public void PurgeSQLByTransactionAliasGuid(SqlCommand cmd)
        {
            cmd.CommandText = "DELETE FROM tblTransactionAliasFieldPlacementInformation WHERE ErrorTransactionSubmissionGuid = @ErrorTransactionSubmissionGuid";
            cmd.Parameters.Add("@ErrorTransactionSubmissionGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@ErrorTransactionSubmissionGuid"].Value = this.IdentityGuid;
        }

        public void Load(DataRow row)
        {
            if (row == null) throw new ArgumentNullException(nameof(row));

            this.Reset();

            this.TransactionSubmissionInformation = DataObject.getValue<string>(row["TransactionAliasGuid"], "");
            this.SubmittedUserGuid = DataObject.getValue<Guid>(row["SubmittedUserGuid"], Guid.Empty);
            this.SubmittedSiteGuid = DataObject.getValue<Guid>(row["SubmittedSiteGuid"], Guid.Empty);

            //base object
            this._IdentityGuid = DataObject.getValue<Guid>(row["ErrorTransactionSubmissionGuid"], Guid.Empty);
            this._CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
            this._CreatedBy = DataObject.getValue<string>(row["CreatedBy"], ADMIN);
            this._UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], _CreatedDate);
            this._UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], ADMIN);
            this.RowVersion = DataObject.getValue<Byte[]>(row["_RowVersion"], null);
        }
    }
}
