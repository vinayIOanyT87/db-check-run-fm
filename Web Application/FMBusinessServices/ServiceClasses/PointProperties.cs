namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Diagnostics;
	using System.Linq;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMBusinessServices.DataAccessLayer;

	using FMBusinessServices.InternalClasses;
	using FMBusinessServices.InternalInterfaces;

	using FMPointCommon;



	using FMCore;

	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class PointProperties : IPointProperties
	{
		private ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();
		private EventLogging EventLogging = new EventLogging();


		private static readonly IPointServiceInfoGetter PointServiceInfoGetter = new PointServiceInfoGetter();

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Dictionary<Guid, Guid> AddPointProperties(SecurityClass security, List<PointProperty> properties)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Check security rights

			var oldGuidToNewGuidMapping = new Dictionary<Guid, Guid>();

			using (var cmd = new SqlCommand())
			{
				foreach (var property in properties)
				{
					var oldGuid = property.IdentityGuid;
					property.SetCreationStamp(security);
					property.AutoGenerateInsertProcSQL(cmd, "gsp_PointPropertyInsertByPK");
					cmd.Parameters["@PointPropertyGuid"].Direction = ParameterDirection.Output;

					ConsolidatedDa.ExecuteQuery(security, cmd);

					property.PointPropertyGuid = new Guid(cmd.Parameters["@PointPropertyGuid"].Value.ToString());

					oldGuidToNewGuidMapping.Add(oldGuid, property.PointPropertyGuid);
				}
			}
			return oldGuidToNewGuidMapping;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PurgeAll(SecurityClass security, Guid pointGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Check security rights.

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "DELETE FROM dbo.tblPointProperty WHERE PointGuid = @PointGuid";
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@PointGuid", pointGuid);
				ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		public Guid GetPointPropertyGuid(SecurityClass security, Guid pointGuid, string id)
		{
			security.ThrowIfNull("security");
			security.ThrowIfNull("pointGuid");
			security.ThrowIfNull("Id");

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT PointPropertyGuid FROM dbo.tblPointProperty WHERE PointGuid = @PointGuid AND ID = @ID";
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@PointGuid", pointGuid);
				cmd.Parameters.AddWithValue("@ID", id);
				set = ConsolidatedDa.GetDataSet(cmd, security);
			}

			Guid pointPropertyGuid = Guid.Empty;

			if(set != null
			&& set.Tables.Count == 1
			&& set.Tables[0].Rows.Count == 1
			&& !set.Tables[0].Rows[0].IsNull("PointPropertyGuid"))
			{
				pointPropertyGuid = (Guid)set.Tables[0].Rows[0]["PointPropertyGuid"];
			}

			return pointPropertyGuid;
		}

		public Dictionary<Guid, PointProperty> EnumerateByPoint(SecurityClass security, Guid pointGuid)
		{
			var pointList = new List<Guid>();
			pointList.Add(pointGuid);
			var pointPropertyList = EnumerateByPointList(security, pointList);
			Dictionary<Guid, PointProperty> ret;
			if (pointPropertyList.TryGetValue(pointGuid, out ret))
			{
				return ret;
			}
			return new Dictionary<Guid, PointProperty>();
		}



		public Dictionary<Guid, Dictionary<Guid, PointProperty>> EnumerateByPointList(SecurityClass security, List<Guid> pointGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Check security rights

			DataSet set = null;
			var modulePointProperty = new PointProperty();

			using (var cmd = new SqlCommand())
			{
				modulePointProperty.EnumerateByPointListSQL(cmd, pointGuidList);
				set = ConsolidatedDa.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			Guid? pointGuid = null;
			Dictionary<Guid, PointProperty> propertyList = null;
			Dictionary<Guid, Dictionary<Guid, PointProperty>> pointPropertyDictionary = new Dictionary<Guid, Dictionary<Guid, PointProperty>>();

			foreach (DataRow row in table.Rows)
			{
				modulePointProperty = new PointProperty();

				modulePointProperty.AutoLoad(row);

				if (propertyList == null
					|| pointGuid.Value != modulePointProperty.PointGuid)
				{
					pointGuid = modulePointProperty.PointGuid;
					propertyList = new Dictionary<Guid, PointProperty>();
					pointPropertyDictionary.Add(modulePointProperty.PointGuid, propertyList);
				}
				propertyList.Add(modulePointProperty.PointPropertyGuid, modulePointProperty);

			}

			return pointPropertyDictionary;
		}


		public Dictionary<Guid, PointProperty> EnumerateByPointPropertyList(SecurityClass security, List<Guid> pointPropertyGuidList)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			DataSet set = null;

			using (var cmd = new SqlCommand())
			{
                PointProperty.EnumerateByPointPropertyListSQL(cmd, pointPropertyGuidList);
				set = ConsolidatedDa.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			var propertyDictionary = new Dictionary<Guid, PointProperty>();

			foreach (DataRow row in table.Rows)
			{
				var modulePointProperty = new PointProperty();

				modulePointProperty.AutoLoad(row);

				propertyDictionary.Add(modulePointProperty.PointPropertyGuid, modulePointProperty);

			}

			return propertyDictionary;
		}

		public List<KeyValuePair<Guid, string>> EnumeratePointPropertyGuidsAndTypes(SecurityClass security)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			DataSet set = null;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT PointPropertyGuid, ValueType FROM tblPointProperty";
				set = ConsolidatedDa.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			var pointPropertyGuidAndTypeList = new List < KeyValuePair<Guid, string>>();

			foreach (DataRow row in table.Rows)
			{
				var propertyGuidAndType = new KeyValuePair<Guid, string> ((Guid) row[0], row[1] as string);

				pointPropertyGuidAndTypeList.Add(propertyGuidAndType);
			}

			return pointPropertyGuidAndTypeList;
		}

		public PointProperty Get(SecurityClass security, Guid modulePointPropertyGuid)
		{
			security.ThrowIfNull("security");
			// TODO: Check security rights.

			var property = new PointProperty();
			DataSet set = null;

			using (var cmd = new SqlCommand())
			{
				property.GetByPropertyGuid(cmd, modulePointPropertyGuid);
				set = ConsolidatedDa.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			property = new PointProperty();
			if (table.Rows.Count > 0)
			{
				DataRow row = table.Rows[0];
				property.AutoLoad(row);
			}

			return property;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ModifyPointPropertyValue(SecurityClass security, PointProperty pointProperty, Boolean bypassUpdatePointRowVersion, Boolean bypassIsPointInSystemUse)
		{
			security.ThrowIfNull("security");
			// TODO: Check security rights.

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "dbo.usp_PointPropertyDataUpdate";
				cmd.CommandType = CommandType.StoredProcedure;
				if (string.IsNullOrEmpty(pointProperty.ValueXml))
				{
					cmd.Parameters.AddWithValue("@Value", DBNull.Value);
				}
				else
				{
					cmd.Parameters.AddWithValue("@Value", pointProperty.ValueXml);
				}
				cmd.Parameters.AddWithValue("@PointPropertyGuid", pointProperty.PointPropertyGuid);
				cmd.Parameters.AddWithValue("@UpdatedBy", security.UserID);
				cmd.Parameters.AddWithValue("@UpdatedDate", DateTimeOffset.Now);
				cmd.Parameters.AddWithValue("@BypassUpdatePointRowVersion", bypassUpdatePointRowVersion);
				cmd.Parameters.AddWithValue("@BypassIsPointInSystemUse", bypassIsPointInSystemUse);

				ConsolidatedDa.ExecuteQuery(security, cmd);
			}

			// Signal Point Service
			try
			{
				if (!bypassUpdatePointRowVersion)
				{
					var pointToPointService = new PointsToPointServices();
					var hostDictionary = pointToPointService.EnumerateHostNameByPointGuid(security, new List<Guid> { pointProperty.PointGuid });
					if (hostDictionary.Count == 1
					&& !hostDictionary.ContainsKey("Deleted"))
					{
						var info = PointServiceInfoGetter.Info;
						string protocol = info.PointServiceBindingEndPointAddress.Substring(
						0,
						info.PointServiceBindingEndPointAddress.IndexOf("/", StringComparison.Ordinal));

						string endPoint = protocol + "//" + hostDictionary.Keys.ElementAt(0) + "/FMPointService";
						FMChannelHelper.MakeCall<IPointService>(
							info.PointServiceBindingType,
							info.PointServiceBindingConfiguration,
							endPoint,
							x => {
								((IClientChannel)x).OperationTimeout = new TimeSpan(0, 0, 5);
								x.SignalPointChanged(security);
							});

					}
				}
			}
			catch (Exception ex)
			{
				this.EventLogging.LogEvent("PointProperties.ModifyPointPropertyValue : " + ex.Message, EventLogEntryType.Error);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ModifyPointValues(SecurityClass security, List<PointValue> pointValues)
		{
			security.ThrowIfNull("security");
			pointValues.ThrowIfNull("pointValues");

			foreach (var pointValue in pointValues)
			{
				var pointProperty = Get(security, pointValue.PointValueIdentifier.IdentityGuid);
				var propertyType = pointProperty.Value.GetType();
				var propertyInfo = propertyType.GetProperty(pointValue.PointValueIdentifier.PropertyID);
				if (propertyInfo == null)
				{
					throw new Exception("No such property : " + pointValue.PointValueIdentifier.PropertyID);
				}

				var valueTypeString = propertyInfo.PropertyType.ToString();
				if (valueTypeString == typeof(PointPropertyUnitTypedDouble).ToString())
				{
					var value = propertyInfo.GetValue(pointProperty.Value);
					(value as PointPropertyUnitTypedDouble).Value = (double)pointValue.Value;
					propertyInfo.SetValue(pointProperty.Value, value);
				}
				else
				{
					propertyInfo.SetValue(pointProperty.Value, pointValue.Value);
				}

				ModifyPointPropertyValue(security, pointProperty, true, false);
			}
		}
	}
}