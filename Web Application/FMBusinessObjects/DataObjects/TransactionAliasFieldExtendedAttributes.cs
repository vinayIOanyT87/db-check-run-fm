using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects
{
    /// <summary>
    /// Extended database attributes for all the fields defined for transactions and sub nodes
    /// </summary>
    public class TransactionAliasFieldExtendedAttributes
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string ColumnType { get; set; }
        public string PropertyName { get; set; }
        public int? MaxLength { get; set; }
        public bool HasListAttached { get; set; }

        /// <summary>
        /// populates the sql command with sql to enumerate field types
        /// </summary>
        /// <param name="cmd"></param>
        public static void EnumerateSQL(SqlCommand cmd)
        {
            cmd.CommandText = @"
SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH FROM information_schema.columns
WHERE TABLE_SCHEMA = 'dbo' 
	AND (Table_Name = 'tblTransactions' 
		OR TABLE_NAME = 'tblTransactionLineItems'
		OR TABLE_NAME = 'tblTransactionUserData'
		OR TABLE_NAME = 'tblTransactionNotes')
";
        }

        public void Load(DataRow populateFrom)
        {
            this.TableName = populateFrom["TABLE_NAME"] as string;
            this.ColumnName = populateFrom["COLUMN_NAME"] as string;
            this.ColumnType = populateFrom["DATA_TYPE"] as string;
            this.MaxLength = populateFrom["CHARACTER_MAXIMUM_LENGTH"] as int?;
            var columnName = populateFrom["COLUMN_NAME"] as string;
            this.PropertyName = TransactionDO.GetPropertyName(columnName);
            this.HasListAttached = false;
        }

    }
}
