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

	using FMBusinessServices.DataAccessLayer;
	using System.Linq;

	[SecuritySafeCritical]
	[ServiceBehavior( TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted )]
	public class ModuleToPointTemplateMaps : FMServiceBase, IModulePointTemplateMaps
	{
		public ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();

		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void AddModuletoPointTemplateMaps(SecurityClass security, List<ModuleToPointTemplateMap> moduleToPointTemplateMaps)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Check security rights

			using (var cmd = new SqlCommand())
			{
				foreach (var moduleToPointTemplate in moduleToPointTemplateMaps)
				{
					moduleToPointTemplate.SetCreationStamp(security);
					moduleToPointTemplate.AutoGenerateInsertProcSQL(cmd, "[map].[gsp_ModuleToPointTemplateInsertByPK]");
					cmd.Parameters["@ModuleToPointTemplateGuid"].Direction = ParameterDirection.InputOutput;

					ConsolidatedDa.ExecuteQuery(security, cmd);

					moduleToPointTemplate.ModuleToPointTemplateGuid = new Guid(cmd.Parameters["@ModuleToPointTemplateGuid"].Value.ToString());
				}
			}
		}


		public void Modify(SecurityClass security, ModuleToPointTemplateMap moduleToPointTemplateMap)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Check security rights

			using (var cmd = new SqlCommand())
			{
				moduleToPointTemplateMap.SetModifyStamp(security);
				moduleToPointTemplateMap.AutoGenerateModifyProcSQL(cmd, "[map].[gsp_ModuleToPointTemplateUpdateByPK]");
				ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}



		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Purge ( SecurityClass security, Guid moduleToPointTemplateMapGuid )
		{
			if ( security == null )
			{
				throw new ArgumentNullException( "security" );
			}


			using ( var cmd = new SqlCommand() )
			{
				cmd.CommandText = "map.gsp_ModuleToPointTemplateDeleteByRowGuid";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@ModuleToPointTemplateGuid", moduleToPointTemplateMapGuid);
				ConsolidatedDa.ExecuteQuery( security, cmd );
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public ModuleToPointTemplateMap Get(SecurityClass security, Guid pointGuid, Guid moduleToTemplateGuid)
		{
				if (security == null)
				{
					throw new ArgumentNullException("security");
				}

				DataSet set = null;
				var moduleToPointTemplateMap = new ModuleToPointTemplateMap();

				using (var cmd = new SqlCommand())
				{
					moduleToPointTemplateMap.EnumerateByModuleToPointTemplateGuidSQL(cmd, moduleToTemplateGuid); 
					set = ConsolidatedDa.GetDataSet(cmd, security);
				}

				DataTable table = set.Tables[0];

				moduleToPointTemplateMap.AutoLoad(table.Rows[0]);

				List<ModuleToPointTemplateMap> mtptList = new List<ModuleToPointTemplateMap>();
				mtptList.Add(moduleToPointTemplateMap);
				return moduleToPointTemplateMap;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Dictionary<Guid, ModuleToPointTemplateMap> EnumerateByTemplateGuid(SecurityClass security, Guid templateGuid)
		{
			if ( security == null )
			{
				throw new ArgumentNullException( "security" );
			}

			// TODO: Check security rights

			DataSet set = null;
			var moduleToPointTemplateMap = new ModuleToPointTemplateMap();

			using ( var cmd = new SqlCommand() )
			{
				moduleToPointTemplateMap.EnumerateByTemplateGuidSQL( cmd, templateGuid );
				set = ConsolidatedDa.GetDataSet( cmd, security );
			}

			return PopulateDictionary(security, set, false);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Dictionary<Guid, ModuleToPointTemplateMap> EnumerateByPointGuid(SecurityClass security, Guid pointGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Check security rights

			DataSet set = null;
			var moduleToPointTemplateMap = new ModuleToPointTemplateMap();

			using (var cmd = new SqlCommand())
			{
				moduleToPointTemplateMap.EnumerateByPointGuidSQL(cmd, pointGuid);
				set = ConsolidatedDa.GetDataSet(cmd, security);
			}

			return PopulateDictionary(security, set, false);
		}


		protected List<Guid> CreateMtptGuidList(List<ModuleToPointTemplateMap> mtptmList)
		{
				var mtptmGuidList = new List<Guid>();
				foreach (var mtptm in mtptmList)
				{
					mtptmGuidList.Add(mtptm.IdentityGuid);
				}
				mtptmGuidList = mtptmGuidList.Distinct().ToList();
				return mtptmGuidList;
		}

		protected Dictionary<Guid, ModuleToPointTemplateMap> PopulateDictionary(SecurityClass security, DataSet set, bool fillInProperties)
		{
				var moduleToPointTemplateMapList = new Dictionary<Guid, ModuleToPointTemplateMap>();
				List<ModuleToPointTemplateMap> mtptmList = new List<ModuleToPointTemplateMap>();


				DataTable table = set.Tables[0];


				foreach (DataRow row in table.Rows)
				{
					var moduleToPointTemplateMap = new ModuleToPointTemplateMap();

					moduleToPointTemplateMap.AutoLoad(row);
					moduleToPointTemplateMapList.Add(moduleToPointTemplateMap.ModuleToPointTemplateGuid, moduleToPointTemplateMap);
					mtptmList.Add(moduleToPointTemplateMap);
				}
				return moduleToPointTemplateMapList;
		}

	}
}
