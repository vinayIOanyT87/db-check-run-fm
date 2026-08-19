namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Xml.Serialization;
	using System.Runtime.Serialization;


	[XmlType("SyncRecordConflictCount")]
	[DataContract]
	[Serializable]

	public class ForeignKeyDO
	{
		#region Properties
		[DataMember]
		public string ForeignKey { get; set; }

		[DataMember]
		public string Schema { get; set; }

		[DataMember]
		public string TableName { get; set; }

		[DataMember]
		public string ColumnName { get; set; }

		[DataMember]
		public string ReferenceSchema { get; set; }

		[DataMember]
		public string ReferenceTableName { get; set; }

		[DataMember]
		public string ReferenceColumnName { get; set; }


		#endregion Properties

		#region Methods

		public static void EnumerateSQL(SqlCommand cmd, string schema, string tableName)
		{
			string sql = "SELECT f.name AS ForeignKey,"
				+ " OBJECT_SCHEMA_NAME(f.parent_object_id) AS[Schema],"
				+ " OBJECT_NAME(f.parent_object_id) AS TableName,"
				+ " COL_NAME(fc.parent_object_id, fc.parent_column_id) AS ColumnName,"
				+ " OBJECT_SCHEMA_NAME(f.referenced_object_id) AS ReferenceSchema,"
				+ " OBJECT_NAME (f.referenced_object_id) AS ReferenceTableName,"
				+ " COL_NAME(fc.referenced_object_id, fc.referenced_column_id) AS ReferenceColumnName"
				+ " FROM sys.foreign_keys AS f"
				+ " INNER JOIN sys.foreign_key_columns AS fc ON f.OBJECT_ID = fc.constraint_object_id"
				+ " WHERE OBJECT_NAME(f.parent_object_id) = '{0}' AND OBJECT_SCHEMA_NAME(f.parent_object_id) = '{1}'";

			cmd.CommandText = string.Format(sql, tableName, schema);
		}

		public void Load(DataRow row)
		{
			ForeignKey = DataObject.getValue<string>(row["ForeignKey"],"");
			Schema = DataObject.getValue<string>(row["Schema"], "");
			TableName = DataObject.getValue<string>(row["TableName"], "");
			ColumnName = DataObject.getValue<string>(row["ColumnName"], "");
			ReferenceSchema = DataObject.getValue<string>(row["ReferenceSchema"], "");
			ReferenceTableName = DataObject.getValue<string>(row["ReferenceTableName"], "");
			ReferenceColumnName = DataObject.getValue<string>(row["ReferenceColumnName"], "");
		}

		#endregion Methods

	}
}
