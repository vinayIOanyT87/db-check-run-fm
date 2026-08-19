using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace FMBusinessObjects.DataObjects
{
	public class SequenceClass : BaseDataObject
	{
		public string Key;
		public Int64 Value;

		public SequenceClass()
		{
			Reset();
		}

		public override void Reset()
		{
			base.Reset();
			Key="";
			Value=0;
		}

		public void Load( DataSet Set )
		{
			if (Set == null)
				throw new ArgumentNullException( "Set" );

			Reset();

			DataTable Table=Set.Tables[0];
			if (Table.Rows.Count == 0)
				return;

			DataRow Row=Table.Rows[0];

			Key = DataObject.getValue<string>(Row["SequenceKey"], "");
			Value = DataObject.getValue<long>(Row["SequenceValue"], 0);
		}

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblSequences " +
					"(SequenceKey," +
					"SequenceValue" +
					") VALUES (" +
					"@SequenceKey," +
					"@SequenceValue)";

			cmd.Parameters.AddWithValue("@SequenceKey", Key);
			cmd.Parameters.AddWithValue("@SequenceValue", Value);
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText =	"UPDATE tblSequences "+
					"SET SequenceValue = @SequenceValue " +
					"WHERE SequenceKey = @SequenceKey";

			cmd.Parameters.AddWithValue("@SequenceKey", Key);
			cmd.Parameters.AddWithValue("@SequenceValue", Value);
		}

		public void SelectSQL( SqlCommand cmd, bool bInTransaction )
		{
			cmd.CommandText = "SELECT * FROM tblSequences " + SQLUpdateLock(bInTransaction) + " WHERE SequenceKey = @SequenceKey";
			cmd.Parameters.AddWithValue("@SequenceKey", Key);
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText =	"DELETE FROM tblSequences WHERE SequenceKey = @SequenceKey";
			cmd.Parameters.AddWithValue("@SequenceKey", Key);
		}
	}
}
