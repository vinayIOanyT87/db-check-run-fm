using System;

namespace StandardXMLImportExport
{
	/// <summary>
	/// Summary description for BaseDBI.
	/// </summary>
	public abstract class BaseDBI
	{
		#region Attributes
		protected string user;
		protected string createdBy;
		protected System.DateTime createdDateTime;
		protected string updatedBy;
		protected System.DateTime updatedDateTime;

		protected System.DateTime now;

		protected System.Data.SqlClient.SqlConnection conn;
		protected System.Data.SqlClient.SqlCommand selectCmd;
		protected System.Data.SqlClient.SqlCommand insertCmd;
		protected System.Data.SqlClient.SqlCommand deleteCmd;

		#endregion Attributes

		public BaseDBI(System.Data.SqlClient.SqlConnection conn, string user)
		{
			this.conn = conn;
			PrepareSQLStatements();
			this.user = user;
			now = System.DateTime.Now;
			
		}
	
		public void finalize()
		{
			CloseSQLStatements();
		}

		#region Abstract members
		abstract protected void PrepareSelectStatement();
		abstract protected void PrepareInsertStatement();
		abstract protected void PrepareDeleteStatement();
		#endregion Abstract members

		protected void PrepareSQLStatements()
		{			
			PrepareSelectStatement();
			PrepareInsertStatement();
			PrepareDeleteStatement();
		}

		protected void CloseSQLStatements()
		{
			selectCmd = null;
			insertCmd = null;
			deleteCmd = null;
			conn = null;
		}

		public void SetTransaction(System.Data.SqlClient.SqlTransaction tx)
		{
			selectCmd.Transaction = tx;
			insertCmd.Transaction = tx;
			deleteCmd.Transaction = tx;
		}

		protected long GetSequenceValue(string name)
		{
			System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
			cmd.CommandType = System.Data.CommandType.StoredProcedure;
			cmd.CommandText = "spGetSequenceValue";
			cmd.Parameters.Add("@SequenceName", System.Data.SqlDbType.NVarChar, 50);
			cmd.Parameters[0].Value = name;
			cmd.Connection = conn;
			cmd.Transaction = selectCmd.Transaction;
//			System.Data.SqlClient.SqlDataReader rdr  = cmd.ExecuteReader();
//			rdr.Read();
//			long result = (long) rdr.GetValue(0);
			long result = (long) cmd.ExecuteScalar();
			return result;
		}
	}
}
