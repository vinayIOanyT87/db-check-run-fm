using FMBusinessObjects.BusinessInterfaces;
using FMBusinessServices.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Web;
using FMBusinessObjects.DataObjects;
using System.Data.SqlClient;
using System.Data;

namespace FMBusinessServices.ServiceClasses
{
	/// <summary>
	/// Service providing access to point group Schedule configuration data.
	/// </summary>
	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class PointGroupSchedules : FMServiceBase, IPointGroupSchedules
	{
		#region Private data members
		private readonly ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public PointGroupSchedules()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, PointGroupSchedule pointGroupSchedule)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (pointGroupSchedule == null)
			{
				throw new ArgumentNullException("pointGroupSchedule");
			}

			if (pointGroupSchedule.PointGroupScheduleGuid == Guid.Empty)
			{
				pointGroupSchedule.PointGroupGuid = Guid.NewGuid();
			}

			using (var cmd = new SqlCommand())
			{
				pointGroupSchedule.SetCreationStamp(security);
				pointGroupSchedule.AutoGenerateInsertProcSQL(cmd, "usp_PointGroupScheduleInsert");

				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
			return pointGroupSchedule.PointGroupScheduleGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public PointGroupSchedule Get(SecurityClass security, Guid pointGroupGuid, Guid userGuid, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (userGuid == null)
			{
				throw new ArgumentNullException("userGuid");
			}

			if (siteGuid == null)
			{
				throw new ArgumentNullException("siteGuid");
			}

			var pointGroupSchedule = new PointGroupSchedule();
			DataSet set;
			// get the main PointGroup data
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "dbo.usp_PointGroupScheduleGet";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@pointGroupGuid", pointGroupGuid);
				cmd.Parameters.AddWithValue("@userGuid", userGuid);
				cmd.Parameters.AddWithValue("@siteGuid", siteGuid);

				this.consolidatedDA.ExecuteQuery(security, cmd);

				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			if (table.Rows.Count > 0)
			{
				pointGroupSchedule.AutoLoad(table.Rows[0]);
			}

			return pointGroupSchedule;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public PointGroupSchedule GetByPK(SecurityClass security, Guid scheduleGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (scheduleGuid == null)
			{
				throw new ArgumentNullException("scheduleGuid");
			}

			var pointGroupSchedule = new PointGroupSchedule();
			DataSet set;
			// get the main PointGroup data
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "dbo.usp_PointGroupScheduleGetByPK";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@PointGroupScheduleGuid", scheduleGuid);

				this.consolidatedDA.ExecuteQuery(security, cmd);

				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			if (table.Rows.Count > 0)
			{
				pointGroupSchedule.AutoLoad(table.Rows[0]);
			}

			return pointGroupSchedule;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, PointGroupSchedule pointGroupSchedule)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (pointGroupSchedule == null)
			{
				throw new ArgumentNullException("pointGroupSchedule");
			}

			using (var cmd = new SqlCommand())
			{
				pointGroupSchedule.SetCreationStamp(security);
				pointGroupSchedule.AutoGenerateModifyProcSQL(cmd, "usp_PointGroupScheduleUpdateByPK");

				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid pointGroupGuid, Guid userGuid, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// Delete point
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "usp_PointGroupScheduleDelete";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@PointGroupGuid", pointGroupGuid);
				cmd.Parameters.AddWithValue("@userGuid", userGuid);
				cmd.Parameters.AddWithValue("@siteGuid", siteGuid);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public PointGroupScheduleCollection EnumerateAll(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}


			var pointGroupScheduleList = new PointGroupScheduleCollection();
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "dbo.usp_PointGroupScheduleGetAll";
				cmd.CommandType = CommandType.StoredProcedure;

				this.consolidatedDA.ExecuteQuery(security, cmd);

				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var pointGroupSchedule = new PointGroupSchedule();

				pointGroupSchedule.AutoLoad(row);
				pointGroupScheduleList.Add(pointGroupSchedule);

			}

			return pointGroupScheduleList;
		}

	}
}