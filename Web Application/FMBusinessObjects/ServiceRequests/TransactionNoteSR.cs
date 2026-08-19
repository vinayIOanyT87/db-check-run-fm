using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class TransactionNoteSR : AccountingServiceRequest
	{
		[DataMember]
		public string Note { get; set; }

		[DataMember]
		public Guid TransGuid { get; set; }

		[DataMember]
		public string UpdatedBy { get; set; }

		public void GetSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DECLARE @TransCount INT " +
				"SELECT @TransCount = COUNT(*) FROM tblTransactionNotes WHERE tblTransactionNotes.TransactionGuid = @TransactionGuid " +
				"IF (@TransCount > 0) " +
				"BEGIN " +
				"UPDATE tblTransactionNotes " +
				"SET Notes = @Notes, " +
				"UpdatedDate = SYSDATETIMEOFFSET(), " +
				"UpdatedBy = @UpdatedBy " +
				"WHERE tblTransactionNotes.TransactionGuid = @TransactionGuid " +
				"END " +
				"ELSE " +
				"BEGIN " +
				"INSERT INTO tblTransactionNotes ( " +
				"TransactionGuid, " + 
				"Notes, " + 
				"CreatedBy, " +
				"CreatedDate, " + 
				"UpdatedBy, " + 
				"UpdatedDate " +
				") VALUES ( " +
				"@TransactionGuid, " + 
				"@Notes, "  + 
				"@UpdatedBy, " +
				"SYSDATETIMEOFFSET(), " +
				"@UpdatedBy, " +
				"SYSDATETIMEOFFSET() " +
				") " +
				"END ";

			cmd.Parameters.AddWithValue("@Notes", this.Note);
			cmd.Parameters.AddWithValue("@UpdatedBy", this.UpdatedBy);
			cmd.Parameters.AddWithValue("@TransactionGuid", this.TransGuid);
		}

		public void GetNoteGuidSql(SqlCommand command)
		{
			command.CommandText = "SELECT TransactionNoteGuid FROM tblTransactionNotes WHERE TransactionGuid = @TransactionGuid ";

			SqlParameter parm = new SqlParameter("@TransactionGuid", SqlDbType.UniqueIdentifier) { Value = this.TransGuid };
			command.Parameters.Add(parm);
		}
	}
}
