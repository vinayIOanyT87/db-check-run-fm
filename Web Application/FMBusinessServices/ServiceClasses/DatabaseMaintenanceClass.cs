
namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessServices.DataAccessLayer;

	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class DatabaseMaintenanceClass : IDatabaseMaintenance
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ReindexDatabase(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			using (var cmd = new SqlCommand())
			{
				DatabaseMaintenanceDAO.ReindexDatabaseSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}
	}
}