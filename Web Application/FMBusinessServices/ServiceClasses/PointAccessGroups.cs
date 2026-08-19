
namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using System.Security;
	using System.ServiceModel;
	using System.Web;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using System.Data.SqlClient;
	using FMBusinessObjects.UtilityObjects;
	using System.Runtime.Serialization;

	using FMCore;

	using InternalClasses;

	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class PointAccessGroups : FMServiceBase, IPointAccessGroups, IDependency
	{
		public ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, PointAccessGroup pointAccessGroup)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			using (var cmd = new SqlCommand())
			{
				pointAccessGroup.SetCreationStamp(security);
				pointAccessGroup.AutoGenerateInsertProcSQL(cmd, "[dbo].[gsp_PointAccessGroupInsertByPK]");
				cmd.Parameters["@PointAccessGroupGuid"].Direction = ParameterDirection.InputOutput;

				ConsolidatedDa.ExecuteQuery(security, cmd);

				pointAccessGroup.PointAccessGroupGuid = new Guid(cmd.Parameters["@PointAccessGroupGuid"].Value.ToString());
			}
			return pointAccessGroup.PointAccessGroupGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, PointAccessGroup pointAccessGroup)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			using (var cmd = new SqlCommand())
			{
				pointAccessGroup.SetModifyStamp(security);
				pointAccessGroup.AutoGenerateModifyProcSQL(cmd, "[dbo].[gsp_PointAccessGroupUpdateByPK]");

				ConsolidatedDa.ExecuteQuery(security, cmd);
			}

			var pointAccessGroupToPointTemplateMaps = new PointAccessGroupToPointTemplateMaps();
			pointAccessGroupToPointTemplateMaps.Modify(security, pointAccessGroup.PointAccessGroupGuid, pointAccessGroup.PointAccessGroupToPointTemplateMapList);

			var pointAccessGroupToPointMaps = new PointAccessGroupToPointMaps();
			pointAccessGroupToPointMaps.Modify(security, pointAccessGroup.PointAccessGroupGuid, pointAccessGroup.PointAccessGroupToPointMapList.ToList());

			var pointAccessGroupToSettingMaps = new PointAccessGroupToExposedSettingMaps();
			pointAccessGroupToSettingMaps.Modify(security, pointAccessGroup.PointAccessGroupGuid, pointAccessGroup.PointAccessGroupToExposedSettingMapList);

			var pointAccessGroupToTagMaps = new PointAccessGroupToTagMaps();
			pointAccessGroupToTagMaps.Modify(security, pointAccessGroup.PointAccessGroupGuid, pointAccessGroup.PointAccessGroupToTagMapList);

			var pointAccessGroupToPointTagMaps = new PointAccessGroupToPointTagMaps();
			pointAccessGroupToPointTagMaps.Modify(security, pointAccessGroup.PointAccessGroupGuid, pointAccessGroup.PointAccessGroupToPointTagMapList);

			var pointAccessGroupToAlarmTestMaps = new PointAccessGroupToAlarmTestMaps();
			pointAccessGroupToAlarmTestMaps.Modify(security, pointAccessGroup.PointAccessGroupGuid, pointAccessGroup.PointAccessGroupToAlarmTestMapList);

         var pointAccessGroupToPointAlarmTestMaps = new PointAccessGroupToPointAlarmTestMaps();
         pointAccessGroupToPointAlarmTestMaps.Modify(security, pointAccessGroup.PointAccessGroupGuid, pointAccessGroup.PointAccessGroupToPointAlarmTestMapList);

			var pointAccessGroupToUserGroupMaps = new PointAccessGroupToUserGroupMaps();
			pointAccessGroupToUserGroupMaps.Modify(security, pointAccessGroup.PointAccessGroupGuid, pointAccessGroup.PointAccessGroupToUserGroupMapList);

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ModifyByList(SecurityClass security, List<PointAccessGroup> pointAccessGroupList)
		{
			security.ThrowIfNull("security");
			pointAccessGroupList.ThrowIfNull( "Point Access Group List");

			// TODO: Check security rights
			foreach (var pointAccessGroup in pointAccessGroupList)
			{
				using (var cmd = new SqlCommand())
				{
					pointAccessGroup.SetModifyStamp(security);
					pointAccessGroup.AutoGenerateModifyProcSQL(cmd, "[dbo].[gsp_PointAccessGroupUpdateByPK]");

					ConsolidatedDa.ExecuteQuery(security, cmd);
				}

				var pointAccessGroupToPointTemplateMaps = new PointAccessGroupToPointTemplateMaps();
				pointAccessGroupToPointTemplateMaps.Modify(security, pointAccessGroup.PointAccessGroupGuid, pointAccessGroup.PointAccessGroupToPointTemplateMapList);

				var pointAccessGroupToPointMaps = new PointAccessGroupToPointMaps();
				pointAccessGroupToPointMaps.Modify(security, pointAccessGroup.PointAccessGroupGuid, pointAccessGroup.PointAccessGroupToPointMapList.ToList());

				var pointAccessGroupToSettingMaps = new PointAccessGroupToExposedSettingMaps();
				pointAccessGroupToSettingMaps.Modify(security, pointAccessGroup.PointAccessGroupGuid, pointAccessGroup.PointAccessGroupToExposedSettingMapList);

				var pointAccessGroupToTagMaps = new PointAccessGroupToTagMaps();
				pointAccessGroupToTagMaps.Modify(security, pointAccessGroup.PointAccessGroupGuid, pointAccessGroup.PointAccessGroupToTagMapList);

				var pointAccessGroupToPointTagMaps = new PointAccessGroupToPointTagMaps();
				pointAccessGroupToPointTagMaps.Modify(security, pointAccessGroup.PointAccessGroupGuid, pointAccessGroup.PointAccessGroupToPointTagMapList);

				var pointAccessGroupToAlarmTestMaps = new PointAccessGroupToAlarmTestMaps();
				pointAccessGroupToAlarmTestMaps.Modify(security, pointAccessGroup.PointAccessGroupGuid, pointAccessGroup.PointAccessGroupToAlarmTestMapList);

				var pointAccessGroupToPointAlarmTestMaps = new PointAccessGroupToPointAlarmTestMaps();
				pointAccessGroupToPointAlarmTestMaps.Modify(security, pointAccessGroup.PointAccessGroupGuid, pointAccessGroup.PointAccessGroupToPointAlarmTestMapList);

				var pointAccessGroupToUserGroupMaps = new PointAccessGroupToUserGroupMaps();
				pointAccessGroupToUserGroupMaps.Modify(security, pointAccessGroup.PointAccessGroupGuid, pointAccessGroup.PointAccessGroupToUserGroupMapList);

			}

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid pointAccessGroupGuid)
		{
			security.ThrowIfNull("security");

			var pointAccessGroupToPointTemplateMaps = new PointAccessGroupToPointTemplateMaps();
			pointAccessGroupToPointTemplateMaps.PurgeByPointAccessGroupGuid(security, pointAccessGroupGuid);

			var pointAccessGroupToPointMaps = new PointAccessGroupToPointMaps();
			pointAccessGroupToPointMaps.PurgeByPointAccessGroupGuid(security, pointAccessGroupGuid);

			var pointAccessGroupToTagMaps = new PointAccessGroupToTagMaps();
			pointAccessGroupToTagMaps.PurgeByPointAccessGroupGuid(security, pointAccessGroupGuid);

			var pointAccessGroupToPointTagMaps = new PointAccessGroupToPointTagMaps();
			pointAccessGroupToPointTagMaps.PurgeByPointAccessGroupGuid(security, pointAccessGroupGuid);

			var pointAccessGroupToAlarmTestMaps = new PointAccessGroupToAlarmTestMaps();
			pointAccessGroupToAlarmTestMaps.PurgeByPointAccessGroupGuid(security, pointAccessGroupGuid);

			var pointAccessGroupToPointAlarmTestMaps = new PointAccessGroupToPointAlarmTestMaps();
			pointAccessGroupToPointAlarmTestMaps.PurgeByPointAccessGroupGuid(security, pointAccessGroupGuid);

			var pointAccessGroupToExposedSettingMaps = new PointAccessGroupToExposedSettingMaps();
			pointAccessGroupToExposedSettingMaps.PurgeByPointAccessGroupGuid(security, pointAccessGroupGuid);

			var pointAccessGroupToUserGroupMaps = new PointAccessGroupToUserGroupMaps();
			pointAccessGroupToUserGroupMaps.PurgeByPointAccessGroupGuid(security, pointAccessGroupGuid);

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "dbo.gsp_PointAccessGroupDeleteByRowGuid";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@PointAccessGroupGuid", pointAccessGroupGuid);
				ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PurgeBySiteGuid(SecurityClass security, Guid siteGuid)
		{
			security.ThrowIfNull("security");


			using (var cmd = new SqlCommand())
			{
				PointAccessGroup.PurgeBySiteGuidSQL(cmd, security.SiteGuid);
				ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Dictionary<Guid, PointAccessGroup> Enumerate(SecurityClass security)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			DataSet set = null;

			using (var cmd = new SqlCommand())
			{
				PointAccessGroup.EnumerateBySiteGuidSQL(cmd, security.SiteGuid);
				set = ConsolidatedDa.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			var pointAccessGroupDictionary = new Dictionary<Guid, PointAccessGroup>();

			foreach (DataRow row in table.Rows)
			{
				var pointAccessGroup = new PointAccessGroup();

				pointAccessGroup.AutoLoad(row);

				pointAccessGroupDictionary.Add(pointAccessGroup.PointAccessGroupGuid, pointAccessGroup);

			}

			return pointAccessGroupDictionary;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public List<PointAccessGroup> EnumerateByUserGroup(SecurityClass security, Guid userGroupGuid)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			DataSet set = null;

			using (var cmd = new SqlCommand())
			{
				PointAccessGroup.EnumerateByUserGroupGuidSQL(cmd, security.SiteGuid, userGroupGuid);
				set = ConsolidatedDa.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			var pointAccessGroupDictionary = new List<PointAccessGroup>();

			foreach (DataRow row in table.Rows)
			{
				var pointAccessGroup = new PointAccessGroup();

				pointAccessGroup.AutoLoad(row);

				var pointAccessGroupToPointTemplateMaps = new PointAccessGroupToPointTemplateMaps();
				pointAccessGroup.PointAccessGroupToPointTemplateMapList = pointAccessGroupToPointTemplateMaps.EnumerateByPointAccessGroupGuid(security, pointAccessGroup.PointAccessGroupGuid);

				var pointAccessGroupToPointMaps = new PointAccessGroupToPointMaps();
				pointAccessGroup.PointAccessGroupToPointMapList = pointAccessGroupToPointMaps.EnumerateByPointAccessGroupGuid(security, pointAccessGroup.PointAccessGroupGuid);

				var pointAccessGroupToTagMaps = new PointAccessGroupToTagMaps();
				pointAccessGroup.PointAccessGroupToTagMapList = pointAccessGroupToTagMaps.EnumerateByPointAccessGroupGuid(security, pointAccessGroup.PointAccessGroupGuid);

				var pointAccessGroupToPointTagMaps = new PointAccessGroupToPointTagMaps();
				pointAccessGroup.PointAccessGroupToPointTagMapList = pointAccessGroupToPointTagMaps.EnumerateByPointAccessGroupGuid(security, pointAccessGroup.PointAccessGroupGuid);

				var pointAccessGroupToAlarmTestMaps = new PointAccessGroupToAlarmTestMaps();
				pointAccessGroup.PointAccessGroupToAlarmTestMapList = pointAccessGroupToAlarmTestMaps.EnumerateByPointAccessGroupGuid(security, pointAccessGroup.PointAccessGroupGuid);

				var pointAccessGroupToPointAlarmTestMaps = new PointAccessGroupToPointAlarmTestMaps();
				pointAccessGroup.PointAccessGroupToPointAlarmTestMapList = pointAccessGroupToPointAlarmTestMaps.EnumerateByPointAccessGroupGuid(security, pointAccessGroup.PointAccessGroupGuid);

				var pointAccessGroupToExposedSettingMaps = new PointAccessGroupToExposedSettingMaps();
				pointAccessGroup.PointAccessGroupToExposedSettingMapList = pointAccessGroupToExposedSettingMaps.EnumerateByPointAccessGroupGuid(security, pointAccessGroup.PointAccessGroupGuid);

				var pointAccessGroupToUserGroupMaps = new PointAccessGroupToUserGroupMaps();
				pointAccessGroup.PointAccessGroupToUserGroupMapList = pointAccessGroupToUserGroupMaps.EnumerateByPointAccessGroupGuid(security, pointAccessGroup.PointAccessGroupGuid);

                
				pointAccessGroupDictionary.Add(pointAccessGroup);

			}

			return pointAccessGroupDictionary;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid? GetDuplicate(SecurityClass security, string id, Guid siteGuid)
		{
			security.ThrowIfNull("security");
			id.ThrowIfNull("id");

			var consolidatedDA = new ConsolidatedDAClass();
			var pointAccessGroup = new PointAccessGroup();
			DataSet set;
			// get the main PointGroup data
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "dbo.usp_PointAccessGroupGetDuplicate";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@ID", id);
				cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);

				set = consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			if (table.Rows.Count > 0)
			{
				pointAccessGroup.PointAccessGroupGuid = (Guid)table.Rows[0].ItemArray[0];  // returns only 1 value
			}
			return pointAccessGroup.PointAccessGroupGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public PointAccessGroup Get(SecurityClass security, Guid pointAccessGroupGuid)
		{
			security.ThrowIfNull("security");

			DataSet set = null;

			using (var cmd = new SqlCommand())
			{
				PointAccessGroup.GetByPointAccessGroupGuidSQL(cmd, pointAccessGroupGuid);
				set = ConsolidatedDa.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			PointAccessGroup pointAccessGroup = null;

			if (table.Rows.Count == 1)
			{
				pointAccessGroup = new PointAccessGroup();

				pointAccessGroup.AutoLoad(table.Rows[0]);

				var pointAccessGroupToPointTemplateMaps = new PointAccessGroupToPointTemplateMaps();
				pointAccessGroup.PointAccessGroupToPointTemplateMapList = pointAccessGroupToPointTemplateMaps.EnumerateByPointAccessGroupGuid(security, pointAccessGroupGuid);

				var pointAccessGroupToPointMaps = new PointAccessGroupToPointMaps();
				pointAccessGroup.PointAccessGroupToPointMapList = pointAccessGroupToPointMaps.EnumerateByPointAccessGroupGuid(security, pointAccessGroupGuid);

				var pointAccessGroupToTagMaps = new PointAccessGroupToTagMaps();
				pointAccessGroup.PointAccessGroupToTagMapList = pointAccessGroupToTagMaps.EnumerateByPointAccessGroupGuid(security, pointAccessGroupGuid);

				var pointAccessGroupToPointTagMaps = new PointAccessGroupToPointTagMaps();
				pointAccessGroup.PointAccessGroupToPointTagMapList = pointAccessGroupToPointTagMaps.EnumerateByPointAccessGroupGuid(security, pointAccessGroupGuid);

				var pointAccessGroupToAlarmTestMaps = new PointAccessGroupToAlarmTestMaps();
				pointAccessGroup.PointAccessGroupToAlarmTestMapList = pointAccessGroupToAlarmTestMaps.EnumerateByPointAccessGroupGuid(security, pointAccessGroupGuid);

				var pointAccessGroupToPointAlarmTestMaps = new PointAccessGroupToPointAlarmTestMaps();
				pointAccessGroup.PointAccessGroupToPointAlarmTestMapList = pointAccessGroupToPointAlarmTestMaps.EnumerateByPointAccessGroupGuid(security, pointAccessGroup.PointAccessGroupGuid);

				var pointAccessGroupToExposedSettingMaps = new PointAccessGroupToExposedSettingMaps();
				pointAccessGroup.PointAccessGroupToExposedSettingMapList = pointAccessGroupToExposedSettingMaps.EnumerateByPointAccessGroupGuid(security, pointAccessGroupGuid);

				var pointAccessGroupToUserGroupMaps = new PointAccessGroupToUserGroupMaps();
				pointAccessGroup.PointAccessGroupToUserGroupMapList = pointAccessGroupToUserGroupMaps.EnumerateByPointAccessGroupGuid(security, pointAccessGroupGuid);
			}

			return pointAccessGroup;
		}

		#region Explicit Interface Methods

		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			security.ThrowIfNull("security");

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}
		}

		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
			security.ThrowIfNull("security");

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}

			// Purge Users
			var o = Object as SiteClass;
			if (o != null)
			{
				var site = o;

				PurgeBySiteGuid(security, site.SiteGuid);
			}
		}

		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
			security.ThrowIfNull("security");

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}
		}

		#endregion
	}
}