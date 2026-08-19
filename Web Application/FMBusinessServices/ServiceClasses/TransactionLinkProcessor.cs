using System.ServiceModel;
using System.Data;
using System.Data.SqlClient;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;

using FMCore;

namespace FMBusinessServices.ServiceClasses
{
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class TransactionLinkProcessorClass : ITransactionLinkProcessor
	{
		#region Attributes
		private ConsolidatedDAClass consolidatedDA;
		private const string originalTransactionSQL = "SELECT OriginalTransID FROM tblTransactionLinks";
		private const string linkedTransactionSQL = "SELECT LinkedTransID FROM tblTransactionLinks";
		#endregion // Attributes

		#region Constructor
		public TransactionLinkProcessorClass ( )
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );
		}
		#endregion // Constructor

		#region Public methods
		[OperationBehavior ( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public TransactionLinkResultDO Process ( TransactionLinkSR sr )
		{
			TransactionLinkResultDO result = new TransactionLinkResultDO ( );

			switch (sr.PerformAction)
			{
				case TransactionLinkSR.Action.GET_LINKED_TRANSACTIONS:
					result = this.GetLinkedTransactions ( sr );
					break;
				case TransactionLinkSR.Action.DELETE_LINEITEM_LINKS:
					result = this.DeleteLineItemLinks ( sr );
					break;
				default:
					result = null;
					break;
			}

			return result;
		}
		#endregion // Overrides

		#region Methods
		private TransactionLinkResultDO DeleteLineItemLinks ( TransactionLinkSR sr )
		{
			TransactionLinkResultDO result = new TransactionLinkResultDO ( );

			if (0 == sr.OriginalLineItemGuids.Count)
			{
				return result;
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				string @inClauseParams = FuelsManagerExtensions.ConstructSqlParametersFromCollection(cmd.Parameters, sr.OriginalLineItemGuids, "TransactionLineItemGuid", SqlDbType.UniqueIdentifier);
				cmd.CommandText = "DELETE FROM tblTransactionLinks WHERE TransactionLineItemGuid IN (" + @inClauseParams + ")";

				this.consolidatedDA.ExecuteQuery(sr.Security, cmd);

				return result;
			}
		}

		protected TransactionLinkResultDO GetLinkedTransactions ( TransactionLinkSR sr )
		{
			TransactionLinkResultDO result = new TransactionLinkResultDO ( );

			// failsafe
			if (0 == sr.SourceTransIDs.Count)
			{
				return new TransactionLinkResultDO ( );
			}

			DataSet[] linkSets = new DataSet[2];

			using (SqlCommand cmd = new SqlCommand())
			{
				string @inClauseParams = FuelsManagerExtensions.ConstructSqlParametersFromCollection(cmd.Parameters, sr.SourceTransIDs, "TransID", SqlDbType.NVarChar);
				cmd.CommandText = originalTransactionSQL + " WHERE LinkedTransID in (" + @inClauseParams + ")";
				linkSets[0] = this.consolidatedDA.GetDataSet(cmd, sr.Security);
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				string @inClauseParams = FuelsManagerExtensions.ConstructSqlParametersFromCollection(cmd.Parameters, sr.SourceTransIDs, "TransID", SqlDbType.NVarChar);
				cmd.CommandText = linkedTransactionSQL + " WHERE OriginalTransID in (" + @inClauseParams + ")";
				linkSets[1] = this.consolidatedDA.GetDataSet(cmd, sr.Security);
			}

			foreach (DataSet set in linkSets)
			{
				foreach (DataRow row in set.Tables[0].Rows)
				{
					string transID = row["TransID"] as string;
					if (!result.ResultTransIDs.Contains ( transID ))
					{
						result.ResultTransIDs.Add ( transID );
					}
				}
			}

			return result;
		}
		#endregion // Methods
	}
}