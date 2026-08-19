namespace FMBusinessServices.ServiceClasses
{

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessServices.DataAccessLayer;
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	/// <summary>
	/// Service providing access to movement summary configuration data.
	/// </summary>
	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class MovementSummaries : FMServiceBase, IMovementSummaries
	{
		#region Private data members
		private readonly ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public MovementSummaries()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, MovementSummary movementSummary)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (movementSummary == null)
			{
				throw new ArgumentNullException("movementSummary");
			}

			if (movementSummary.MovementSummaryGuid == Guid.Empty)
			{
				movementSummary.MovementSummaryGuid = Guid.NewGuid();
			}

			using (var cmd = new SqlCommand())
			{

				movementSummary.SetCreationStamp(security);
				movementSummary.AutoGenerateInsertProcSQL(cmd, "usp_MovementSummaryInsert");

				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
			return movementSummary.MovementSummaryGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid movementSummaryGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Check security rights

			var movementSummary = this.Get(security, movementSummaryGuid, security.UserGuid, security.SiteGuid);
			if (movementSummary.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Movement Summary not found.");
			}

			// Delete movement summary
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "gsp_MovementSummaryDeleteByRowGuid";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@MovementSummaryGuid", movementSummaryGuid);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, MovementSummary movementSummary, out byte[] rowVersion)
		{
			rowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };

			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (movementSummary == null)
			{
				throw new ArgumentNullException("movementSummary");
			}

			DataSet set;

			MovementSummary existingMS = Get(security, movementSummary.MovementSummaryGuid, security.UserGuid, security.SiteGuid);

			if (existingMS != null)
			{
				// Exclude "id" property of the JSON objects from the columns
				dynamic existingColumns = Newtonsoft.Json.JsonConvert.DeserializeObject(existingMS.ColumnsDefinition);
				dynamic receivedColumns = Newtonsoft.Json.JsonConvert.DeserializeObject(movementSummary.ColumnsDefinition);

				foreach (JObject item in existingColumns.Children<JObject>())
				{
					item.Property("id")?.Remove();
				}

				foreach (JObject item in receivedColumns.Children<JObject>())
				{
					item.Property("id")?.Remove();
				}

				// Exclude "id" property of the JSON objects from the rows
				dynamic existingRows = Newtonsoft.Json.JsonConvert.DeserializeObject(existingMS.RowsDefinition);
				dynamic receivedRows = Newtonsoft.Json.JsonConvert.DeserializeObject(movementSummary.RowsDefinition);

				foreach (JObject item in existingRows.Children<JObject>())
				{
					item.Property("id")?.Remove();
					item.Property("parentRowId")?.Remove();
					item.Property("'masterRowId")?.Remove();
				}

				foreach (JObject item in receivedRows.Children<JObject>())
				{
					item.Property("id")?.Remove();
					item.Property("parentRowId")?.Remove();
					item.Property("'masterRowId")?.Remove();
				}

				if (existingMS.ID == movementSummary.ID
				&&	existingMS.Description == movementSummary.Description
				&&	existingMS.MovementSummaryType == movementSummary.MovementSummaryType
				&&	existingColumns.ToString() == receivedColumns.ToString()
				&&	existingRows.ToString() == receivedRows.ToString())
				{
					rowVersion = existingMS.RowVersion;
					return; 
				}
			}

			using (var cmd = new SqlCommand())
			{

				movementSummary.SetCreationStamp(security);
				movementSummary.AutoGenerateModifyProcSQL(cmd, "usp_MovementSummaryUpdateByPK");
				cmd.Parameters.Add("@RowVersion", SqlDbType.Timestamp).Value = movementSummary.RowVersion;

				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			if (set.Tables.Count > 0)
			{
				DataTable table = set.Tables[0];

				if (table.Rows.Count > 0)
				{
					rowVersion = table.Rows[0]["Row_Version"] as byte[];
				}
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public MovementSummary Get(SecurityClass security, Guid movementSummaryGuid, Guid userGuid, Guid siteGuid)
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

			var movementSummary = new MovementSummary();
			DataSet set;
			// get the main MovementSummary data
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "dbo.usp_MovementSummaryGetByPK";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@movementSummaryGuid", movementSummaryGuid);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			if (table.Rows.Count > 0)
			{
				movementSummary.AutoLoad(table.Rows[0]);
			}

			if (movementSummary.ColumnsDefinition.IndexOf("\"field\":\"Type\"") == -1)
			{
				dynamic existingColumns = JsonConvert.DeserializeObject<dynamic>(movementSummary.ColumnsDefinition) as JArray;
				List<JObject> columnList = existingColumns?.ToObject<List<JObject>>();

				var typeColumn = new MovementSummaryColumnDefinition("Type", "Type", true, true, 80, false, "text-center grid-font-14", true, true, 80, "Type", "ui-state-default text-center grid-font-14", null, 80);

				columnList.Insert(1, JObject.FromObject(typeColumn));

				JArray updatedColumns = new JArray(columnList);

				movementSummary.ColumnsDefinition = JsonConvert.SerializeObject(updatedColumns);

				using (var cmd = new SqlCommand())
				{

					movementSummary.SetCreationStamp(security);
					movementSummary.AutoGenerateModifyProcSQL(cmd, "usp_MovementSummaryUpdateByPK");
					cmd.Parameters.Add("@RowVersion", SqlDbType.Timestamp).Value = movementSummary.RowVersion;
					set = this.consolidatedDA.GetDataSet(cmd, security);
				}

				if (set.Tables.Count > 0)
				{
					table = set.Tables[0];

					var rowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
					if (table.Rows.Count > 0)
					{
						rowVersion = table.Rows[0]["Row_Version"] as byte[];
					}

					movementSummary.RowVersion = rowVersion;
				}
			}

			return movementSummary;

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid? GetDuplicate(SecurityClass security, string id, int movementSummaryType, Guid ownerUserGuid, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}


			if (id == null)
			{
				throw new ArgumentNullException("id");
			}

			if (ownerUserGuid == null)
			{
				throw new ArgumentNullException("ownerUserGuid");
			}

			if (siteGuid == null)
			{
				throw new ArgumentNullException("siteGuid");
			}

			var movementSummary = new MovementSummary();
			DataSet set;
			// get the main MovementSummary data
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "dbo.usp_MovementSummaryGetDuplicate";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@ID", id);
				cmd.Parameters.AddWithValue("@MovementSummaryType", movementSummaryType);
				cmd.Parameters.AddWithValue("@OwnerUserGuid", ownerUserGuid);
				cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);

				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			if (table.Rows.Count > 0)
			{
				movementSummary.AutoLoad(table.Rows[0]);
			}
			return movementSummary.MovementSummaryGuid;
		}

		public MovementSummaryCollection EnumerateByUserSite(SecurityClass security, Guid userGuid, Guid siteGuid)
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

			var movementSummaryList = new MovementSummaryCollection();
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "dbo.usp_MovementSummaryEnumerateByUserSite";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@userGuid", userGuid);
				cmd.Parameters.AddWithValue("@siteGuid", siteGuid);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var movementSummary = new MovementSummary();

				movementSummary.AutoLoad(row);
				movementSummaryList.Add(movementSummary);

			}

			return movementSummaryList;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]

		public void GetMovementSummaryIfNewer(SecurityClass security, Guid movementSummaryGuid, byte[] prevRowVersion, out MovementSummary movementSummary)
		{
			DataSet set;

			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandText = "dbo.usp_GetMovementSummaryIfNewer";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@movementSummaryGuid", movementSummaryGuid);
				cmd.Parameters.AddWithValue("@prevRowVersion", prevRowVersion);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}
			
			movementSummary = null;

			if (set.Tables.Count > 0)
			{
				DataTable table = set.Tables[0];

				if (table.Rows.Count > 0)
				{
					movementSummary = new MovementSummary();

					movementSummary.AutoLoad(table.Rows[0]);
				}
			}
		}
	}
}