using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Web;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.DataObjects;

using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class SequencesClass : ISequences
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public SequencesClass()
		{
		}

		[OperationBehavior ( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Save ( SecurityClass security, SequenceClass Sequence )
		{
			if (security == null)
				throw new ArgumentNullException( "Security" );

			if (Sequence == null)
				throw new ArgumentNullException( "Sequence" );

			using (SqlCommand cmd = new SqlCommand())
			{
				Sequence.SelectSQL(cmd, ContextUtil.IsInTransaction);
				DataSet Set = ConsolidatedDA.GetDataSet(cmd, security);

				using (SqlCommand cmd2 = new SqlCommand())
				{
					if (Set.Tables.Count != 0 && Set.Tables[0].Rows.Count != 0)
					{
						Sequence.UpdateSQL(cmd2);
						ConsolidatedDA.ExecuteQuery(security, cmd2);
					}
					else
					{
						Sequence.InsertSQL(cmd2);
						ConsolidatedDA.ExecuteQuery(security, cmd2);
					}
				}
			}
		}

		public Int64 Get( SecurityClass security, string Key )
		{
			if (security == null)
				throw new ArgumentNullException( "Security" );

			SequenceClass Sequence=new SequenceClass();
			Sequence.Key=Key;
			using (SqlCommand cmd = new SqlCommand())
			{
				Sequence.SelectSQL(cmd, ContextUtil.IsInTransaction);
				Sequence.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}
			return Sequence.Value;
		}

		[OperationBehavior ( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Purge ( SecurityClass security, string Key )
		{
			if (security == null)
				throw new ArgumentNullException( "Security" );

			SequenceClass Sequence=new SequenceClass();
			Sequence.Key=Key;

			using (SqlCommand cmd = new SqlCommand())
			{
				Sequence.PurgeSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}
	}
}
