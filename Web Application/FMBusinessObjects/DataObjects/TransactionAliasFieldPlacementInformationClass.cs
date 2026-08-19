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
    public class TransactionAliasFieldPlacementInformationClass : BaseDataObject
    {
        [DataMember]
        public Guid TransactionAliasGuid { get; set; }

        [DataMember]
        public string PlacementInformation { get; set; }

        public override void Reset()
        {
            base.Reset();
            this.PlacementInformation = "";
        }

        /// <summary>
        /// Will update or insert based on any matching TransactionAliases
        /// </summary>
        /// <param name="cmd"></param>
        public void UpsertSQLByTransactionAliasGuid(SqlCommand cmd)
        {
            cmd.CommandText = @"usp_UpdateOrInsertTransactionAliasFieldPlacementInformation";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@TransactionAliasGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@PlacementInformation", SqlDbType.NVarChar);
            cmd.Parameters["@TransactionAliasGuid"].Value = this.TransactionAliasGuid;
            cmd.Parameters["@PlacementInformation"].Value = this.PlacementInformation;
            
            cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
            cmd.Parameters["@CreatedBy"].Value = this.CreatedBy;
            cmd.Parameters["@UpdatedBy"].Value = this.UpdatedBy;
        }

        public void SelectSQLByTransactionAliasGuid(SqlCommand cmd, bool bInTransaction = false)
        {
            cmd.CommandText = "SELECT * FROM tblTransactionAliasFieldPlacementInformation " + SQLUpdateLock(bInTransaction) + " WHERE TransactionAliasGuid = @TransactionAliasGuid";
            cmd.Parameters.Add("@TransactionAliasGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TransactionAliasGuid"].Value = this.TransactionAliasGuid;
        }

        public void PurgeSQLByTransactionAliasGuid(SqlCommand cmd)
        {
            cmd.CommandText = "DELETE FROM tblTransactionAliasFieldPlacementInformation WHERE TransactionAliasGuid = @TransactionAliasGuid";
            cmd.Parameters.Add("@TransactionAliasGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TransactionAliasGuid"].Value = this.TransactionAliasGuid;
        }

        public void Load(DataSet set)
        {
            if (set == null) throw new ArgumentNullException(nameof(set));

            this.Reset();
            
            var table = set.Tables.Cast<DataTable>().FirstOrDefault();
            if (table == null)
            {
                //nothing to load
                return;
            }
            var row = table.Rows.Cast<DataRow>().FirstOrDefault();
            if (row == null)
            {
                //nothing to load
                return;
            }

            this.TransactionAliasGuid = DataObject.getValue<Guid>(row["TransactionAliasGuid"], Guid.Empty);
            this.PlacementInformation = DataObject.getValue<string>(row["PlacementInformation"], "");
            
            //base object
            this._IdentityGuid = DataObject.getValue<Guid>(row["TransactionAliasFieldPlacementInformationGuid"], Guid.Empty);
            this._CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
            this._CreatedBy = DataObject.getValue<string>(row["CreatedBy"], ADMIN);
            this._UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], _CreatedDate);
            this._UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], ADMIN);
            this.RowVersion = DataObject.getValue<Byte[]>(row["_RowVersion"], null);
        }
    }
}
