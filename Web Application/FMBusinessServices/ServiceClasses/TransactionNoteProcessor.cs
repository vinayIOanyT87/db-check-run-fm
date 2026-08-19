namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.ServiceModel;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessServices.DataAccessLayer;

	using FMBusinessObjects.Exceptions;

	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class TransactionNoteProcessorClass : ITransactionNoteProcessor
	{
		private readonly ConsolidatedDAClass consolidatedDA = new ConsolidatedDAClass();

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Process(TransactionNoteSR sr)
		{
			Guid notesGuid = Guid.Empty;

			if (sr == null)
			{
				throw new ArgumentNullException("Service Request");
			}

			if (sr.Security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (!sr.Security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
				&& !sr.Security.HasRight(RIGHT.MODIFY_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			this.CheckSR(sr);

			using (SqlCommand command = new SqlCommand())
			{
				sr.GetSQL(command);
				this.consolidatedDA.ExecuteQuery(sr.Security, command);
			}

			using (SqlCommand command = new SqlCommand())
			{
				sr.GetNoteGuidSql(command);
				DataSet dataSet =this.consolidatedDA.GetDataSet(command, sr.Security);

				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					DataRow row = dataSet.Tables[0].Rows[0];
					notesGuid = row.IsNull("TransactionNoteGuid") ? Guid.Empty : (Guid)row["TransactionNoteGuid"];
				}
			}

			return notesGuid;
		}

		private void CheckSR(TransactionNoteSR sr)
		{
			if (string.IsNullOrEmpty(sr.Note))
			{
				throw new ArgumentNullException("Note");
			}

			if (string.IsNullOrEmpty(sr.UpdatedBy))
			{
				throw new ArgumentNullException("UpdatedBy");
			}
		}
	}
}