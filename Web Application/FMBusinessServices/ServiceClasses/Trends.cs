namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;

	using DataAccessLayer;
	using InternalClasses;

	using FMCore;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class Trends : ITrends, IDependency
	{
		private readonly ConsolidatedDAClass consolidatedDA = new ConsolidatedDAClass();

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, Trend trend)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			if (trend.PointTemplateGuid == Guid.Empty)
			{

				Guid identityGuid = GetIdentityGuid(security, trend.ID);
				if (identityGuid != Guid.Empty)
				{
					throw (new Exception("Trend Exists"));
				}
			}
			else
			{

				Guid identityGuid = GetIdentityGuidByPointTemplateGuid(security, trend.PointTemplateGuid);
				if (identityGuid != Guid.Empty)
				{
					throw (new Exception("Trend Exists"));
				}

				trend.ID = "Point Trend";
				trend.Description = "Point Description";
			}


			using (var cmd = new SqlCommand())
			{
				trend.SetCreationStamp(security);
				trend.AutoGenerateInsertProcSQL(cmd, "gsp_TrendInsertByPK");
				if((Guid) cmd.Parameters["@PointTemplateGuid"].Value == Guid.Empty)
				{
					cmd.Parameters["@PointTemplateGuid"].Value = null;
				}
				cmd.Parameters["@TrendGuid"].Direction = ParameterDirection.InputOutput;

				this.consolidatedDA.ExecuteQuery(security, cmd);

				trend.TrendGuid = new Guid(cmd.Parameters["@TrendGuid"].Value.ToString());
			}

			ModifyTrendPenCollection(security, trend.TrendGuid, trend.PointTemplateGuid, trend.Pens, null);

			return trend.TrendGuid;

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, Trend trend)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			// Check to see if we need to do a saveas

			var existing = this.Get(security, trend.TrendGuid);

			if (existing == null)
			{
				throw new Exception("Trend not found.");
			}

			if (trend.PointTemplateGuid == Guid.Empty)
			{
				Guid identityGuid = GetIdentityGuid(security, trend.ID);
				if (identityGuid != Guid.Empty
				&& identityGuid != trend.IdentityGuid)
				{
					throw (new Exception("Trend Exists"));
				}
			}
			else
			{

				Guid identityGuid = GetIdentityGuidByPointTemplateGuid(security, trend.PointTemplateGuid);
				if (identityGuid != Guid.Empty
				&& identityGuid != trend.IdentityGuid)
				{
					throw (new Exception("Trend Exists"));
				}

				trend.ID = "Point Trend";
				trend.Description = "Point Description";
			}


			using (var cmd = new SqlCommand())
			{
				trend.SetModifyStamp(security);
				trend.AutoGenerateModifyProcSQL(cmd, "gsp_TrendUpdateByPK");
				if ((Guid)cmd.Parameters["@PointTemplateGuid"].Value == Guid.Empty)
				{
					cmd.Parameters["@PointTemplateGuid"].Value = null;
					cmd.Parameters.Add("@NullOverridePointTemplateGuid", SqlDbType.Bit).Value = true;
				}


				cmd.Parameters.Add("@NullOverrideDescription", SqlDbType.Bit).Value = true;

				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

			ModifyTrendPenCollection(security,trend.TrendGuid, trend.PointTemplateGuid, trend.Pens, existing.Pens);
		}


		private Guid AddTrendPen(SecurityClass security, Guid pointTemplateGuid, TrendPen trendPen)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			using (var cmd = new SqlCommand())
			{
				trendPen.SetCreationStamp(security);
				if (pointTemplateGuid == Guid.Empty)
				{
					trendPen.AutoGenerateInsertProcSQL(cmd, "map.gsp_TrendPenToPointTrendInsertByPK");
					cmd.Parameters.RemoveAt("@PointTemplateTagGuid");
					cmd.Parameters["@TrendPenToTrendGuid"].ParameterName = "@TrendPenToPointTrendGuid";
					cmd.Parameters["@TrendPenToPointTrendGuid"].Direction = ParameterDirection.InputOutput;
				}
				else
				{
					trendPen.AutoGenerateInsertProcSQL(cmd, "map.gsp_TrendPenToDetailTrendInsertByPK");
					cmd.Parameters.RemoveAt("@PointTagGuid");
					cmd.Parameters["@TrendPenToTrendGuid"].ParameterName = "@TrendPenToDetailTrendGuid";
					cmd.Parameters["@TrendPenToDetailTrendGuid"].Direction = ParameterDirection.InputOutput;
				}

				this.consolidatedDA.ExecuteQuery(security, cmd);

				if (pointTemplateGuid == Guid.Empty)
				{
					trendPen.TrendPenToTrendGuid = new Guid(cmd.Parameters["@TrendPenToPointTrendGuid"].Value.ToString());
				}
				else
				{
					trendPen.TrendPenToTrendGuid = new Guid(cmd.Parameters["@TrendPenToDetailTrendGuid"].Value.ToString());
				}
			}

			return trendPen.TrendPenToTrendGuid;
		}


		private void ModifyTrendPen(SecurityClass security, Guid pointTemplateGuid, TrendPen trendPen)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			using (var cmd = new SqlCommand())
			{
				trendPen.SetModifyStamp(security);
				if (pointTemplateGuid == Guid.Empty)
				{
					trendPen.AutoGenerateModifyProcSQL(cmd, "map.gsp_TrendPenToPointTrendUpdateByPK");
					cmd.Parameters.RemoveAt("@PointTemplateTagGuid");
					cmd.Parameters["@TrendPenToTrendGuid"].ParameterName = "@TrendPenToPointTrendGuid";
				}
				else
				{
					trendPen.AutoGenerateModifyProcSQL(cmd, "map.gsp_TrendPenToDetailTrendUpdateByPK");
					cmd.Parameters.RemoveAt("@PointTagGuid");
					cmd.Parameters["@TrendPenToTrendGuid"].ParameterName = "@TrendPenToDetailTrendGuid";
				}
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}



		public void ModifyTrendPenCollection(	SecurityClass security,
															Guid trendGuid,
															Guid pointTemplateGuid,
															List<TrendPen> newTrendPenList,
															List<TrendPen> existingTrendPenList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}


			if (newTrendPenList != null)
			{
				for (int newItem = 0; newItem < newTrendPenList.Count; newItem++)
				{
					int existingItem;
					TrendPen newTrendPen = newTrendPenList[newItem];

					newTrendPen.TrendGuid = trendGuid;

					if (existingTrendPenList != null)
					{
						for (existingItem = 0; existingItem < existingTrendPenList.Count; existingItem++)
						{
							TrendPen existingTrendPen = existingTrendPenList[existingItem];
							if (existingTrendPen.TrendPenToTrendGuid == newTrendPen.TrendPenToTrendGuid)
							{
								if (existingTrendPen.PointTagGuid != newTrendPen.PointTagGuid
								|| existingTrendPen.PenColor != newTrendPen.PenColor)
								{
									ModifyTrendPen(security, pointTemplateGuid, newTrendPen);
								}
								break;
							}
						}

						if (existingItem == existingTrendPenList.Count)
						{
							newTrendPen.TrendPenToTrendGuid = AddTrendPen(security, pointTemplateGuid, newTrendPen);
						}
						else
						{
							existingTrendPenList.RemoveAt(existingItem);
						}
					}
					else
					{
						newTrendPen.TrendPenToTrendGuid = AddTrendPen(security, pointTemplateGuid, newTrendPen);
					}
				}
			}

			if (existingTrendPenList != null)
			{
				foreach (var trendPen in existingTrendPenList)
				{
					PurgeTrendPen(security, pointTemplateGuid, trendPen.TrendPenToTrendGuid);
				}
			}
		}



		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid trendGuid)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			var trend = this.Get(security, trendGuid);
			if (trend.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Trend not found.");
			}

			// Delete trend
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SET NOCOUNT ON"
										+ " DELETE FROM map.tblTrendPenToPointTrend WHERE TrendGuid = @TrendGuid"
										+ " DELETE FROM map.tblTrendPenToDetailTrend WHERE TrendGuid = @TrendGuid"
										+ " DELETE FROM dbo.tblTrend WHERE TrendGuid = @TrendGuid";
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@TrendGuid", trendGuid);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}


		private void PurgeTrendPen(SecurityClass security, Guid pointTemplateGuid, Guid trendPenToTrendGuid)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			// Delete trend
			using (var cmd = new SqlCommand())
			{
				if (pointTemplateGuid == Guid.Empty)
				{
					cmd.CommandText = "DELETE FROM map.tblTrendPenToPointTrend WHERE TrendPenToPointTrendGuid = @TrendPenToTrendGuid";
				}
				else
				{
					cmd.CommandText = "DELETE FROM map.tblTrendPenToDetailTrend WHERE TrendPenToDetailTrendGuid = @TrendPenToTrendGuid";
				}
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@TrendPenToTrendGuid", trendPenToTrendGuid);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}


		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PurgeBySite(SecurityClass security, Guid siteGuid)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SET NOCOUNT ON"
										+ " DELETE FROM map.tblTrendPenToPointTrend WHERE TrendGuid IN (SELECT TrendGuid FROM dbo.tblTrend WHERE SiteGuid = @SiteGuid)"
										+ " DELETE FROM map.tblTrendPenToDetailTrend WHERE TrendGuid IN (SELECT TrendGuid FROM dbo.tblTrend WHERE SiteGuid = @SiteGuid)"
										+ " DELETE FROM dbo.tblTrend WHERE SiteGuid = @SiteGuid";
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PurgeByPoint(SecurityClass security, Guid pointGuid)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SET NOCOUNT ON"
										+ " DELETE FROM map.tblTrendPenToPointTrend WHERE PointTagGuid IN (SELECT PointTagGuid FROM dbo.tblPointTag WHERE PointGuid = @PointGuid)";
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@PointGuid", pointGuid);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PurgeByPointTemplate(SecurityClass security, Guid pointTemplateGuid)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SET NOCOUNT ON"
										+ " DELETE FROM map.tblTrendPenToDetailTrend WHERE PointTemplateTagGuid IN (SELECT PointTemplateTagGuid FROM dbo.tblPointTemplateTag WHERE PointTemplateGuid = @PointTemplateGuid)"
										+ " DELETE FROM dbo.tblTrend WHERE PointTemplateGuid = @PointTemplateGuid";
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Boolean CanAddPointTrend(SecurityClass security)
		{
			security.ThrowIfNull("security");

			DataSet set;
			var valid = 0;

			using (var cmd = new SqlCommand())
			{
				Trend.CanCreatePointTrendSQL(cmd, security.SiteGuid);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			if (set.Tables.Count == 1
			&& set.Tables[0].Rows.Count == 1)
			{
				DataTable table = set.Tables[0];
				valid = (int)table.Rows[0]["valid"];
			}

			return valid != 0;

		}

		public Trend Get(SecurityClass security, Guid trendGuid)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				Trend.SelectSQL(cmd, trendGuid);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable trendTable = set.Tables[0];
			DataTable trendPenTable = set.Tables[1];
			var trend = new Trend();
			trend.Pens = new List<TrendPen>();
			if (trendTable.Rows.Count > 0)
			{
				trend.AutoLoad(trendTable.Rows[0]);
			}

			foreach(DataRow row in trendPenTable.Rows)
			{
				var trendPen = new TrendPen();
				trendPen.AutoLoad(row);
				trendPen.UnitString = EngineeringUnits.GetUnitAbbreviation(trendPen.Units);
				trend.Pens.Add(trendPen);			
			}

			return trend;
		}

		public Trend GetPointTrend(SecurityClass security, Guid pointGuid)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				Trend.SelectByPointSQL(cmd, security.SiteGuid, pointGuid);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable trendTable = set.Tables[0];
			DataTable trendPenTable = set.Tables[1];
			var trend = new Trend();
			trend.Pens = new List<TrendPen>();
			if (trendTable.Rows.Count > 0)
			{
				trend.AutoLoad(trendTable.Rows[0]);
			}

			if (trend.TrendGuid != Guid.Empty)
			{
				foreach (DataRow row in trendPenTable.Rows)
				{
					var trendPen = new TrendPen();
					trendPen.AutoLoad(row);
					trendPen.UnitString = EngineeringUnits.GetUnitAbbreviation(trendPen.Units);
					trend.Pens.Add(trendPen);
				}
			}
			else
			{
				if (this.CanAddPointTrend(security))
				{

					var points = new Points();
					var point = points.Get(security, pointGuid);
					trend.SiteGuid = security.SiteGuid;
					trend.PointTemplateGuid = point.PointTemplateGuid;
					trend.Mode = FMBusinessObjects.DataObjects.CodedVariables.TrendModeEnum.Realtime;
					trend.PeriodType = FMBusinessObjects.DataObjects.CodedVariables.TrendPeriodType.Minutes;
					trend.Period = 10;

					trend.IdentityGuid = Add(security, trend);

					trend.ID = point.ID;
					trend.Description = point.Description;
				}
				else
				{
					trend = null;
				}

			}

			return trend;
		}



		public Guid GetIdentityGuid(SecurityClass security, string id)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			DataSet set;
			var trendGuid = Guid.Empty;

			using (var cmd = new SqlCommand())
			{
				Trend.SelectTrendGuidByIdSQL(cmd, security.SiteGuid, id);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			if (set.Tables.Count == 1
			&& set.Tables[0].Rows.Count == 1)
			{
				DataTable table = set.Tables[0];
				trendGuid = (Guid)table.Rows[0]["TrendGuid"];
			}

			return trendGuid;
		}

		public Guid GetIdentityGuidByPointTemplateGuid(SecurityClass security, Guid pointTemplateGuid)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			DataSet set;
			var trendGuid = Guid.Empty;

			using (var cmd = new SqlCommand())
			{
				Trend.SelectTrendGuidByPointTemplateGuidSQL(cmd, security.SiteGuid, pointTemplateGuid);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			if (set.Tables.Count == 1
			&& set.Tables[0].Rows.Count == 1)
			{
				DataTable table = set.Tables[0];
				trendGuid = (Guid)table.Rows[0]["TrendGuid"];
			}

			return trendGuid;
		}


		public List<TrendName> EnumerateAvailableTrendNames(SecurityClass security)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				TrendName.EnumerateAvailableTrendNames(cmd, security);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];
			var names = new List<TrendName>();

			foreach (DataRow row in table.Rows)
			{
				var trendName = new TrendName();
				BaseDataObject.AutoLoad(trendName, row);

				names.Add(trendName);
			}

			return names;
		}

		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}
		}

		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}

			if (Object is SiteClass)
			{
				var site = (SiteClass)Object;
				this.PurgeBySite(security, site.IdentityGuid);
			}

			else if(Object is Point)
			{
				var point = (Point)Object;
				this.PurgeByPoint(security, point.PointGuid);
			}

			else if (Object is PointTemplate)
			{
				var pointTemplate = (PointTemplate)Object;
				this.PurgeByPointTemplate(security, pointTemplate.PointTemplateGuid);
			}

		}

		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}
		}
	}
}