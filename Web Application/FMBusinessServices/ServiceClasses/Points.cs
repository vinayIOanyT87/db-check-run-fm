// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Points.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Service providing access to point configuration data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
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
	using Opc.Ua;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.UtilityObjects;
	using FMBusinessServices.InternalClasses;
	using FMBusinessServices.InternalInterfaces;

	using FMPointCommon;

	using DataAccessLayer;
	using System.Globalization;


	using FMCore;
    using FMBusinessObjects.ChannelFactories;


    /// <summary>
    /// Service providing access to point configuration data.
    /// </summary>
	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class Points : FMServiceBase, IPoints, IDependency
	{
		#region Private data members
		private readonly ConsolidatedDAClass consolidatedDA;
		private static readonly IPointServiceInfoGetter PointServiceInfoGetter = new PointServiceInfoGetter();
		private EventLogging EventLogging;

		#endregion

		public static string BulkPointDescription = "Bulk generated point.";
		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public Points()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
			this.EventLogging = new EventLogging();
		}
		#endregion

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void CreatePoints(SecurityClass security, string prefix, int numberOfPoints, Guid pointTemplateGuid)
		{

            updateSiteCloseoutXML(security);

            var templates = new PointTemplates();
				var template = templates.Get(security, pointTemplateGuid);
				var digits = numberOfPoints.ToString(CultureInfo.InvariantCulture).Length;
				var format = "D" + digits.ToString(CultureInfo.InvariantCulture);
				for (var index = 1; index <= numberOfPoints; ++index)
				{
					this.CreatePoint( security, prefix + index.ToString(format), template);
				}
		}

		protected void AddAlarms(List<Tuple<PointTemplateTag, PointTag>> templateTagToTagList, Dictionary<Guid, Guid> templateTagGuidToTagGuidMap)
		{
			foreach (var templateTagToTag in templateTagToTagList)
			{
				var templateTag = templateTagToTag.Item1;

				if (templateTag.AlarmTemplates.Count > 0)
				{
					var tag = templateTagToTag.Item2;
					foreach (var alarmTemplate in templateTag.AlarmTemplates.Values)
					{
						var alarm = new Alarm(templateTag.ValueType, alarmTemplate, templateTagGuidToTagGuidMap);
						tag.Alarms.Add(alarm.IdentityGuid, alarm);
					}
				}
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid CreatePoint(SecurityClass security, string id, PointTemplate template)
		{
			var point = new Point(template)
			{
				ID = id,
				Enabled = true,
				Description = BulkPointDescription,
				SiteGuid = security.SiteGuid
			};

			var templateTagToTagList = new List<Tuple<PointTemplateTag, PointTag>>();
			var pointTemplateTagGuidToPointTagGuidDictionary = new Dictionary<Guid, Guid>();
			point.Tags = new Dictionary<Guid, PointTag>();

			// get dictionary of DAM tags
			var damTagDictionary = new Dictionary<Guid, PointTemplateTag>();
			foreach(var templateTag in template.Tags)
			{
				if(templateTag.Value.ValueTypeString == "FMBusinessObjects.DataObjects.DeviceAlarmMapReference")
				{
					foreach (var alarm in templateTag.Value.AlarmTemplates.Values)
					{
						damTagDictionary.Add(alarm.AlarmStateTemplateTagGuid, template.Tags[alarm.AlarmStateTemplateTagGuid]);
						foreach(var alarmTest in alarm.AlarmTestTemplates.Values)
						{
							damTagDictionary.Add(alarmTest.LimitTemplateTagGuid, template.Tags[alarmTest.LimitTemplateTagGuid]);
						}
					}
				}
			}


			// Add tags
			foreach (var templateTag in template.Tags)
			{
				var tag = new PointTag(templateTag.Value, !damTagDictionary.ContainsKey(templateTag.Value.PointTemplateTagGuid));
				point.Tags.Add(tag.IdentityGuid, tag);
				pointTemplateTagGuidToPointTagGuidDictionary.Add(templateTag.Key, tag.IdentityGuid);
				templateTagToTagList.Add(new Tuple<PointTemplateTag, PointTag>(templateTag.Value, tag));
			}

			//Add Alarms
			this.AddAlarms(templateTagToTagList, pointTemplateTagGuidToPointTagGuidDictionary);

			point.ModuleInstances = new Dictionary<Guid, ModuleToPointTemplateMap>();
			foreach (var modInst in template.ModuleInstances)
			{
				point.ModuleInstances.Add(modInst.Key, modInst.Value.Clone());
			}

			var templatePropertyGuidToPointPropertyGuid = new Dictionary<Guid, Guid>();
			point.Properties = new Dictionary<Guid, PointProperty>();
			foreach (var prop in template.Properties.Values)
			{
				var pointProp = new PointProperty(prop);
				point.Properties.Add(pointProp.PointPropertyGuid, pointProp);
				templatePropertyGuidToPointPropertyGuid.Add(prop.PointTemplatePropertyGuid, pointProp.PointPropertyGuid);
			}

			foreach (var tag in point.Tags.Values)
			{
				if (tag.Alarms.Any())
				{
					foreach (var alarm in tag.Alarms.Values)
					{
						var stateTag = point.Tags[alarm.AlarmStateTagGuid];
						if (!tag.AlarmsEnabled
						|| !alarm.Enabled)
						{
							if (stateTag.Status != StatusCodes.BadOutOfService)
							{
								stateTag.Status = StatusCodes.BadOutOfService;
								stateTag.SourceTimeStamp = DateTimeOffset.UtcNow;
								stateTag.ServerTimeStamp = stateTag.SourceTimeStamp;
							}
						}
						else
						{
							if (stateTag.Status != StatusCodes.Bad)
							{
								stateTag.Status = StatusCodes.Bad;
								stateTag.SourceTimeStamp = DateTimeOffset.UtcNow;
								stateTag.ServerTimeStamp = stateTag.SourceTimeStamp;
							}
						}
					}
				}
			}

			this.Add(security, point);

			return point.PointGuid;
		}

		/// <summary>
		/// Adds the specified point.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="point">The point to add.</param>
		/// <param name="generateIdentityGuid">Create a new guid. Otherwise, uses the identity guid on the point.</param>
		/// <returns>The identity guid of the newly added point.</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, Point point, bool generateIdentityGuid = true)
		{
			security.ThrowIfNull("security");
			point.ThrowIfNull("point");

			if(string.IsNullOrEmpty(point.ProductID))
			{
				point.ProductGuid = null;
			}
			else
			{
				var products = new ProductsClass();
				point.ProductGuid = products.GetIdentityGuid(security, point.ProductID);
			}

			using (var cmd = new SqlCommand())
			{
				point.SetCreationStamp(security);
				point.AutoGenerateInsertProcSQL(cmd, "usp_PointInsertByPK");
				if (generateIdentityGuid)
				cmd.Parameters["@PointGuid"].Direction = ParameterDirection.Output;
				else
				{
					cmd.Parameters["@PointGuid"].Direction = ParameterDirection.Input;
					cmd.Parameters["@PointGuid"].Value = point.IdentityGuid;
				}

				this.consolidatedDA.ExecuteQuery(security, cmd);
				if (generateIdentityGuid)
					point.PointGuid = new Guid(cmd.Parameters["@PointGuid"].Value.ToString());
			}

			var tags = new PointTags();
			point.Tags.ToList().ForEach(x => x.Value.PointGuid = point.IdentityGuid);
			tags.AddTags(security, point.Tags);

			var props = new PointProperties();


			point.Properties.Values.ToList().ForEach(x => x.PointGuid = point.IdentityGuid);

			// Set PointId for MovementData
			var movementDataPointProperty = point.Properties.Values.SingleOrDefault(u => u.ValueTypeString == "FMBusinessObjects.DataObjects.MovementData");
			if(movementDataPointProperty != null)
			{ 
				var pointValueIdentifier = new PointValueIdentifier() { IdentityGuid = point.PointGuid, PointValueType = PointValueType.Point, PropertyID = "PointId" };
				var pointValue = new PointValue(pointValueIdentifier, point);
				(movementDataPointProperty.Value as MovementData).PointId.Add(pointValue);
			}

			var oldPropertyGuidToNewPropertyGuidMapping = props.AddPointProperties(security, point.Properties.Values.ToList());

			ApplicationStringMapsClass ApplicationStringMaps = new ApplicationStringMapsClass();
			ApplicationStringMaps.ModifyCollection(security, point.IdentityGuid, point.PointCategoryCollection, null);


			return point.IdentityGuid;
		}

		/// <summary>
		/// Purges the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="pointGuid">The point unique identifier.</param>
		/// <exception cref="System.ArgumentNullException">security</exception>
		/// <exception cref="System.Exception">Point not found.</exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid pointGuid)
		{
			updateSiteCloseoutXML(security);

         security.ThrowIfNull("security");

			// TODO: Check security rights

			var point = this.Get(security, pointGuid);
			if (point.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Point not found.");
			}

			var dependencies = new DependenciesClass(security);
			dependencies.Purge(security, point);

			ApplicationStringMapsClass ApplicationStringMaps = new ApplicationStringMapsClass();
			ApplicationStringMaps.ModifyCollection(security, point.IdentityGuid, null, point.PointCategoryCollection);

			var pointsToPointServices = new PointsToPointServices();
			pointsToPointServices.PurgeByPointGuid(security, pointGuid);

			// Delete tags
			var tags = new PointTags();
			tags.DeleteTags(security, point.IdentityGuid);

			var modulePointProperties = new PointProperties();
			modulePointProperties.PurgeAll(security, pointGuid);

			// Delete point
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "usp_PointDeleteByRowGuid";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@PointGuid", pointGuid);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

			// Delete OpcUaServers
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "DELETE FROM dbo.tblOpcUaServer WHERE OpcUaServerGuid NOT IN (SELECT DISTINCT OpcUaServerGuid FROM tblPointTag WHERE OpcUaServerGuid IS NOT NULL)";
				cmd.CommandType = CommandType.Text;
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public Guid GetIdentityGuid(SecurityClass security, string ID)
		{
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT PointGuid FROM tblPoint WHERE ID = @ID AND SiteGuid = @SiteGuid";
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@ID", ID);
				cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];
			if (table.Rows.Count == 0)
			{
				return Guid.Empty;
			}

			return (Guid) table.Rows[0][0];
		}

		/// <summary>
		/// Gets the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="pointGuid">The point unique identifier.</param>
		/// <param name="enforcePointAccess">Enforce Point Access Security.</param>
		/// <returns></returns>
		public Point Get(SecurityClass security, Guid pointGuid, bool enforcePointAccess = false)
		{
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				Point.GetSQL(cmd, pointGuid);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];
			if (table.Rows.Count == 0)
			{
				return null;
			}

			var point = new Point();
			point.AutoLoad(table.Rows[0]);

			var tags = new PointTags();
			point.Tags = tags.EnumerateByPointGuid(security, point.IdentityGuid, enforcePointAccess);

			var props = new PointProperties();
			point.Properties = props.EnumerateByPoint(security, point.PointGuid);

			var moduleInstances = new ModuleToPointTemplateMaps();
			point.ModuleInstances = moduleInstances.EnumerateByPointGuid(security, point.PointGuid);

			var applicationStringMaps = new ApplicationStringMapsClass();
			point.PointCategoryCollection = applicationStringMaps.EnumerateByAssignedToGuidAndType(security, point.IdentityGuid, STRING_MAP_TYPE.POINT_CATEGORY);

			return point;
		}

		/// <summary>
		/// Gets the specified points.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="pointGuidList">List of the point unique identifier.</param>
		/// <returns></returns>
		public PointCollection GetPoints(SecurityClass security, List<Guid> pointGuidList)
		{
			return this.Get(security, pointGuidList);
		}

		public Point GetPointBaseData(SecurityClass security, Guid pointGuid)
		{
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				Point.GetSQL(cmd, pointGuid);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];
			if (table.Rows.Count == 0)
			{
				return null;
			}

			var point = new Point();
			point.AutoLoad(table.Rows[0]);

			return point;
		}


		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ModifyPointValues(SecurityClass security, List<PointValue> pointValues)
		{
			security.ThrowIfNull("security");
			pointValues.ThrowIfNull("pointValues");

			var pointDictionary = new Dictionary<Guid, Point>();

			foreach (var pointValue in pointValues)
			{
				Point point;
				if (!pointDictionary.TryGetValue(pointValue.PointValueIdentifier.IdentityGuid, out point))
				{
					point = Get(security, pointValue.PointValueIdentifier.IdentityGuid);
					pointDictionary.Add(pointValue.PointValueIdentifier.IdentityGuid, point);
				}

				var propertyType = point.GetType();
				var propertyInfo = propertyType.GetProperty(pointValue.PointValueIdentifier.PropertyID);
				if (propertyInfo == null)
				{
					throw new Exception("No such property : " + pointValue.PointValueIdentifier.PropertyID);
				}

				var valueTypeString = propertyInfo.PropertyType.ToString();
				if (valueTypeString == typeof(PointPropertyUnitTypedDouble).ToString())
				{
					var value = propertyInfo.GetValue(point);
					(value as PointPropertyUnitTypedDouble).Value = (double)pointValue.Value;
					propertyInfo.SetValue(point, value);
				}
				else
				{
					propertyInfo.SetValue(point, pointValue.Value);
				}
			}

			var currentSiteGuid = security.SiteGuid;
			try
			{
				foreach (var point in pointDictionary.Values)
				{
					security.SiteGuid = point.SiteGuid;
					Modify(security, point);
				}
			}
			finally
			{
				security.SiteGuid = currentSiteGuid;
			}
		}

		[OperationBehavior(TransactionScopeRequired = false, TransactionAutoComplete = true)]
		public void UpdateRowVersion(SecurityClass security, Guid pointGuid)
		{
			security.ThrowIfNull("security");

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "UPDATE dbo.tblPoint SET UpdatedBy = @UpdatedBy, UpdatedDate = GETDATE() WHERE PointGuid = @PointGuid";
				cmd.Parameters.AddWithValue("@UpdatedBy", security.UserID);
				cmd.Parameters.AddWithValue("@PointGuid", pointGuid);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = false, TransactionAutoComplete = true)]
		public void UpdateRowVersionBySite(SecurityClass security, Guid siteGuid)
		{
			security.ThrowIfNull("security");

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "UPDATE dbo.tblPoint SET UpdatedBy = @UpdatedBy, UpdatedDate = GETDATE() WHERE SiteGuid = @SiteGuid";
				cmd.Parameters.AddWithValue("@UpdatedBy", security.UserID);
				cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}



		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, Point point)
		{
			security.ThrowIfNull("security");
			point.ThrowIfNull("point");

			var product = new ProductClass();
			var useProductStandardDensity = false;
			var useProductDensityLimits = false;
			var useProductTemperatureLimits = false;
			List<Guid> standardDensityGuids = new List<Guid>();
			List<Guid> densityProductHighGuids = new List<Guid>();
			List<Guid> densityProductLowGuids = new List<Guid>();
			List<Guid> temperatureProductHiHiGuids = new List<Guid>();
			List<Guid> temperatureProductHighGuids = new List<Guid>();
			List<Guid> temperatureProductLoLoGuids = new List<Guid>();
			List<Guid> temperatureProductLowGuids = new List<Guid>();

			if (string.IsNullOrEmpty(point.ProductID))
			{
				point.ProductGuid = null;
			}
			else
			{
				var products = new ProductsClass();
				product = products.GetByID(security, point.ProductID);
				if (point.ProductGuid != product.MasterRecordGuid)
				{
					point.ProductGuid = product.MasterRecordGuid;

					/* If we have a product we need to make sure that we use the product properties when required*/
					useProductStandardDensity = product.ApplyStandardDensity;
					useProductDensityLimits = product.ApplyDensityLimits;
					useProductTemperatureLimits = product.ApplyTemperatureLimits;

					// if we are using product info for tags we need to get the template tags
					if (useProductStandardDensity || useProductDensityLimits || useProductTemperatureLimits)
					{
						var pointTemplateTags = new PointTemplateTags().EnumerateByPointTemplateGuid(security, point.PointTemplateGuid);
						if (useProductStandardDensity)
						{
							standardDensityGuids =
								pointTemplateTags.Values.ToList()
									.Where(x => x.WellKnownIdentityGuid == Guids.DensityProductStandardGuid)
									.Select(y => y.PointTemplateTagGuid).ToList();
						}
						if (useProductDensityLimits)
						{
							densityProductHighGuids =
								pointTemplateTags.Values.ToList()
									.Where(x => x.WellKnownIdentityGuid == Guids.DensityProductHighGuid)
									.Select(y => y.PointTemplateTagGuid).ToList();
							densityProductLowGuids =
								pointTemplateTags.Values.ToList()
									.Where(x => x.WellKnownIdentityGuid == Guids.DensityProductLowGuid)
									.Select(y => y.PointTemplateTagGuid).ToList();
						}
						if (useProductTemperatureLimits)
						{
							temperatureProductHiHiGuids =
								pointTemplateTags.Values.ToList()
									.Where(x => x.WellKnownIdentityGuid == Guids.TemperatureProductHiHiGuid)
									.Select(y => y.PointTemplateTagGuid).ToList();
							temperatureProductHighGuids =
								pointTemplateTags.Values.ToList()
									.Where(x => x.WellKnownIdentityGuid == Guids.TemperatureProductHighGuid)
									.Select(y => y.PointTemplateTagGuid).ToList();
							temperatureProductLoLoGuids =
								pointTemplateTags.Values.ToList()
									.Where(x => x.WellKnownIdentityGuid == Guids.TemperatureProductLoLoGuid)
									.Select(y => y.PointTemplateTagGuid).ToList();
							temperatureProductLowGuids =
								pointTemplateTags.Values.ToList()
									.Where(x => x.WellKnownIdentityGuid == Guids.TemperatureProductLowGuid)
									.Select(y => y.PointTemplateTagGuid).ToList();
						}
					}
				}
			}


			var pointTags = new PointTags();
			var alarmTests = new AlarmTests();
			var alarms = new Alarms();
			var tagsWithAlarms = new List<Guid>();

			var processedtagList = new List<Guid>();

			foreach (var tag in point.Tags.Values)
			{
				Dictionary<Guid, Dictionary<Guid, AlarmTest>> alarmTestList;

				// apply the product values if configured to do so
				// standard density
				if (useProductStandardDensity && standardDensityGuids.Contains(tag.PointTemplateTagGuid))
				{
					product._StandardDensity.Units = tag.Units;
					tag.Value = product._StandardDensity.Value;
					tag.Status = StatusCodes.Good;
					tag.ServerTimeStamp = DateTimeOffset.UtcNow;
					tag.SourceTimeStamp = DateTimeOffset.UtcNow;
				}


				// density limits
				if (useProductDensityLimits && densityProductHighGuids.Contains(tag.PointTemplateTagGuid))
				{
					product._DensityHighLimit.Units = tag.Units;
					tag.Value = product._DensityHighLimit.Value;
					tag.Status = StatusCodes.Good;
					tag.ServerTimeStamp = DateTimeOffset.UtcNow;
					tag.SourceTimeStamp = DateTimeOffset.UtcNow;
					alarmTestList = alarmTests.EnumerateByPointLimitTagGuids(security, new List<Guid>(new Guid[] { tag.PointTagGuid }));
					var listAlarmTest = alarmTestList.Values.Select(x => x.Values).ToList();
					product._DensityDeadband.Units = tag.Units;
					foreach (var alarmTestCollection in listAlarmTest)
					{
						foreach (var tagAlarmTest in alarmTestCollection)
						{
							tagAlarmTest.Holdoff = product._DensityDeadband.Value;
							alarmTests.Modify(security, tagAlarmTest);
						}
					}
				}

				if (useProductDensityLimits && densityProductLowGuids.Contains(tag.PointTemplateTagGuid))
				{
					product._DensityLowLimit.Units = tag.Units;
					tag.Value = product._DensityLowLimit.Value;
					tag.Status = StatusCodes.Good;
					tag.ServerTimeStamp = DateTimeOffset.UtcNow;
					tag.SourceTimeStamp = DateTimeOffset.UtcNow;
					alarmTestList = alarmTests.EnumerateByPointLimitTagGuids(security, new List<Guid>(new Guid[] { tag.PointTagGuid }));
					var listAlarmTest = alarmTestList.Values.Select(x => x.Values).ToList();
					product._DensityDeadband.Units = tag.Units;
					foreach (var alarmTestCollection in listAlarmTest)
					{
						foreach (var tagAlarmTest in alarmTestCollection)
						{
							tagAlarmTest.Holdoff = product._DensityDeadband.Value;
							alarmTests.Modify(security, tagAlarmTest);
						}
					}
				}

				// temperature limits
				if (useProductTemperatureLimits && temperatureProductHiHiGuids.Contains(tag.PointTemplateTagGuid))
				{
					product._TemperatureHiHiLimit.Units = tag.Units;
					tag.Value = product._TemperatureHiHiLimit.Value;
					tag.Status = StatusCodes.Good;
					tag.ServerTimeStamp = DateTimeOffset.UtcNow;
					tag.SourceTimeStamp = DateTimeOffset.UtcNow;
					alarmTestList = alarmTests.EnumerateByPointLimitTagGuids(security, new List<Guid>(new Guid[] { tag.PointTagGuid }));
					var listAlarmTest = alarmTestList.Values.Select(x => x.Values).ToList();
					product._TemperatureDeadband.Units = tag.Units;
					foreach (var alarmTestCollection in listAlarmTest)
					{
						foreach (var tagAlarmTest in alarmTestCollection)
						{
							tagAlarmTest.Holdoff = product._TemperatureDeadband.Value;
							alarmTests.Modify(security, tagAlarmTest);
						}
					}
				}

				if (useProductTemperatureLimits && temperatureProductHighGuids.Contains(tag.PointTemplateTagGuid))
				{
					product._TemperatureHighLimit.Units = tag.Units;
					tag.Value = product._TemperatureHighLimit.Value;
					tag.Status = StatusCodes.Good;
					tag.ServerTimeStamp = DateTimeOffset.UtcNow;
					tag.SourceTimeStamp = DateTimeOffset.UtcNow;
					alarmTestList = alarmTests.EnumerateByPointLimitTagGuids(security, new List<Guid>(new Guid[] { tag.PointTagGuid }));
					var listAlarmTest = alarmTestList.Values.Select(x => x.Values).ToList();
					product._TemperatureDeadband.Units = tag.Units;
					foreach (var alarmTestCollection in listAlarmTest)
					{
						foreach (var tagAlarmTest in alarmTestCollection)
						{
							tagAlarmTest.Holdoff = product._TemperatureDeadband.Value;
							alarmTests.Modify(security, tagAlarmTest);
						}
					}
				}

				if (useProductTemperatureLimits && temperatureProductLoLoGuids.Contains(tag.PointTemplateTagGuid))
				{
					product._TemperatureLoLoLimit.Units = tag.Units;
					tag.Value = product._TemperatureLoLoLimit.Value;
					tag.Status = StatusCodes.Good;
					tag.ServerTimeStamp = DateTimeOffset.UtcNow;
					tag.SourceTimeStamp = DateTimeOffset.UtcNow;
					alarmTestList = alarmTests.EnumerateByPointLimitTagGuids(security, new List<Guid>(new Guid[] { tag.PointTagGuid }));
					var listAlarmTest = alarmTestList.Values.Select(x => x.Values).ToList();
					product._TemperatureDeadband.Units = tag.Units;
					foreach (var alarmTestCollection in listAlarmTest)
					{
						foreach (var tagAlarmTest in alarmTestCollection)
						{
							tagAlarmTest.Holdoff = product._TemperatureDeadband.Value;
							alarmTests.Modify(security, tagAlarmTest);
						}
					}
				}

				if (useProductTemperatureLimits && temperatureProductLowGuids.Contains(tag.PointTemplateTagGuid))
				{
					product._TemperatureLowLimit.Units = tag.Units;
					tag.Value = product._TemperatureLowLimit.Value;
					tag.Status = StatusCodes.Good;
					tag.ServerTimeStamp = DateTimeOffset.UtcNow;
					tag.SourceTimeStamp = DateTimeOffset.UtcNow;
					alarmTestList = alarmTests.EnumerateByPointLimitTagGuids(security, new List<Guid>(new Guid[] { tag.PointTagGuid }));
					var listAlarmTest = alarmTestList.Values.Select(x => x.Values).ToList();
					product._TemperatureDeadband.Units = tag.Units;
					foreach (var alarmTestCollection in listAlarmTest)
					{
						foreach (var tagAlarmTest in alarmTestCollection)
						{
							tagAlarmTest.Holdoff = product._TemperatureDeadband.Value;
							alarmTests.Modify(security, tagAlarmTest);
						}
					}
				}

				if (!point.Enabled)
				{
					if ((tag.InputOutputType != PointTemplateTag.PointTagInputOutputType.Manual
					&& tag.OpcStatusCodeBits != StatusCodes.GoodLocalOverride)
					|| tag.Value == null)
					{
						tag.Status = StatusCodes.BadOutOfService;
					}

					if (tag.Alarms.Any())
					{
						foreach (var alarm in tag.Alarms.Values)
						{
							foreach (var alarmStatus in alarm.AlarmStatus.Values)
							{
								alarmStatus.AlarmTestFailed = false;
								alarmStatus.Acknowledged = true;
							}
						}
					}
				}

				else
				{
					if (tag.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
					&& tag.Value != null
					&& tag.OpcStatusCodeBits != StatusCodes.GoodLocalOverride
					&& tag.OpcStatusCodeBits != StatusCodes.Good)
					{
						tag.Status = StatusCodes.Good;
						tag.SourceTimeStamp = DateTimeOffset.UtcNow;
						tag.ServerTimeStamp = tag.SourceTimeStamp;
					}
					else if ((tag.InputOutputType != PointTemplateTag.PointTagInputOutputType.Manual
					&& tag.OpcStatusCodeBits != StatusCodes.GoodLocalOverride
					&& tag.OpcStatusCodeBits != StatusCodes.Bad
					&& tag.OpcStatusCodeBits != StatusCodes.BadOutOfService)
					&& tag.Value == null)
					{
						tag.Status = StatusCodes.Bad;
						tag.SourceTimeStamp = DateTimeOffset.UtcNow;
						tag.ServerTimeStamp = tag.SourceTimeStamp;
					}
				}

				if (tag.Alarms.Any())
				{
					foreach (var alarm in tag.Alarms.Values)
					{
						var stateTag = point.Tags[alarm.AlarmStateTagGuid];
						if (!tag.AlarmsEnabled
						|| !alarm.Enabled)
						{
							if(stateTag.OpcStatusCodeBits != StatusCodes.BadOutOfService)
							{
								stateTag.Value = null;
								stateTag.Status = StatusCodes.BadOutOfService;
								stateTag.SourceTimeStamp = DateTimeOffset.UtcNow;
								stateTag.ServerTimeStamp = stateTag.SourceTimeStamp;
							}
						}
						else
						{
							if (stateTag.Value == null
							&& stateTag.OpcStatusCodeBits != StatusCodes.Bad)
							{
								stateTag.Status = StatusCodes.Bad;
								stateTag.SourceTimeStamp = DateTimeOffset.UtcNow;
								stateTag.ServerTimeStamp = stateTag.SourceTimeStamp;
							}
						}

						foreach (var alarmStatus in alarm.AlarmStatus.Values)
						{
							var alarmTest = alarm.AlarmTests[alarmStatus.AlarmTestGuid];
							if (!tag.AlarmsEnabled
							|| !alarm.Enabled
							|| !alarmTest.Enabled)
							{
								alarmStatus.AlarmTestFailed = false;
								alarmStatus.Acknowledged = true;
							}
						}
					}
				}


				processedtagList.Add(tag.PointTagGuid);

				if (tag.Alarms.Any())
				{
					tagsWithAlarms.Add(tag.PointTagGuid);
				}
			}

			// Now that all tags are up todate save them
			foreach (var tag in point.Tags.Values)
			{
				pointTags.Modify(security, tag);

				// remove the alarm definitions for the tag that may have been deleted in the UI
				alarms.DeleteAlarmForTagNotInList(
					security,
					tag.PointTagGuid,
					tag.Alarms.Values.Select(x => x.AlarmGuid).ToList());

			}

			foreach (var tagWithAlarmGuid in tagsWithAlarms)
			{
				var tag = point.Tags[tagWithAlarmGuid];

				alarms.AddModifyAlarms(security, tag.Alarms.Values.ToList(), true, true);
			}


			// All Alarms may have been deleted for a tag in the UI. Delete any alarm definition for any tag that has not been updated
			alarms.DeleteAlarmsFromTagsNotInList(security, point.PointGuid, tagsWithAlarms);


			pointTags.PurgeByPointGuidAndNotInList(security, point.PointGuid, processedtagList);


			var pointProperties = new PointProperties();
			foreach(var property in point.Properties.Values)
			{
				if(property.ValueType == typeof(VcfModuleSettings))
				{
					property.Value = product._VcfModuleSettings;
				}

				// Set PointId for MovementData
				if(property.ValueType == typeof(MovementData))
				{ 
					var pointValueIdentifier = new PointValueIdentifier() { IdentityGuid = point.PointGuid, PointValueType = PointValueType.Point, PropertyID = "PointId" };
					var pointValue = new PointValue(pointValueIdentifier, point);
					(property.Value as MovementData).PointId[0] = pointValue;
				}



				pointProperties.ModifyPointPropertyValue(security, property, true, true);
			}



			ApplicationStringMapsClass applicationStringMaps = new ApplicationStringMapsClass();
			var oldPointCategoryCollection = applicationStringMaps.EnumerateByAssignedToGuidAndType(security, point.IdentityGuid, STRING_MAP_TYPE.POINT_CATEGORY);
			applicationStringMaps.ModifyCollection(security, point.IdentityGuid, point.PointCategoryCollection, oldPointCategoryCollection);

			using (var cmd = new SqlCommand())
			{
				point.SetModifyStamp(security);
				point.AutoGenerateModifyProcSQL(cmd, "gsp_PointUpdateByPK");
				cmd.Parameters.AddWithValue("@NullOverrideProductGuid", true);
				cmd.Parameters.AddWithValue("@NullOverrideOverrideDefaultDrawingGuid", true);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

			// Signal Point Service
			try
			{
				var pointToPointService = new PointsToPointServices();
				var hostDictionary = pointToPointService.EnumerateHostNameByPointGuid(security, new List<Guid> { point.PointGuid });
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
			catch(Exception ex)
			{
				this.EventLogging.LogEvent("Points.Modify : " + ex.Message, EventLogEntryType.Error);
			}
		}

		protected void FillInSubLists(SecurityClass security, PointCollection points)
		{
			var pointGuidList = new List<Guid>();
			foreach(var point in points)
			{
				pointGuidList.Add(point.IdentityGuid);
			}

			this.FillInSubLists(security, points, pointGuidList);

		}

		protected void FillInSubLists(SecurityClass security, PointCollection points, List<Guid> pointGuidList)
		{
			var tags = new PointTags();
			var pointTagDictionary = tags.EnumerateByPointList(security, pointGuidList);


			var props = new PointProperties();
			var pointPointPropertiesDictionary = props.EnumerateByPointList(security, pointGuidList);
			foreach(var point in points)
			{
				Dictionary<Guid, PointTag> tagDictionary;
				if (pointTagDictionary.TryGetValue(point.IdentityGuid, out tagDictionary))
				{
					point.Tags = tagDictionary;
					foreach (PointTag tag in point.Tags.Values)
					{
							tag.PointID = point.ID;
					}
				}

				Dictionary<Guid, PointProperty> propertiesDictionary;
				if (pointPointPropertiesDictionary.TryGetValue(point.IdentityGuid, out propertiesDictionary))
				{
					point.Properties = propertiesDictionary;
				}
			}
		}

		/// <summary>
		/// Enumerates points for the specified site.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="siteGuid">The site unique identifier.</param>
		/// <returns>
		/// A collection of Points.
		/// </returns>
		public PointCollection EnumerateBySite(SecurityClass security, Guid siteGuid)
		{
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				Point.EnumerateBySiteSQL(cmd, siteGuid);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var pointCollection = new PointCollection();

			DataTable table = set.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var point = new Point();

				point.AutoLoad(row);
				pointCollection.Add(point);

			}

			this.FillInSubLists(security, pointCollection);

			return pointCollection;
		}

		/// <summary>
		/// Enumerates points for the specified site.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="siteGuid">The site unique identifier.</param>
		/// <param name="pointFilter">Filter for Points</param>
		/// <param name="tagFilterList">List of tag IDs we need to return the results for</param>
		/// <returns>
		/// A collection of Points.
		/// </returns>
		public PointCollection EnumerateBySiteFiltered(SecurityClass security, Guid siteGuid, PointGroupFilterRules pointFilter, List<string> tagFilterList)
		{
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				Point.EnumerateBySiteSQLFiltered(cmd, siteGuid, security.UserGuid, pointFilter);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var pointCollection = new PointCollection();

			DataTable table = set.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var point = new Point();

				point.AutoLoad(row);
				pointCollection.Add(point);

			}

			// populate the lists of tags for the points
			var tags = new PointTags();
			var pointGuidList = pointCollection.Select(x => x.PointGuid).ToList();
			var pointTagDictionary = tags.EnumerateByPointList(security, pointGuidList, tagFilterList);

			foreach (var point in pointCollection)
			{
				Dictionary<Guid, PointTag> tagDictionary;
				if (pointTagDictionary.TryGetValue(point.IdentityGuid, out tagDictionary))
				{
					point.Tags = tagDictionary;
				}
			}

			return pointCollection;
		}

		public Dictionary<Guid, Point> EnumerateByPointPropertyList(SecurityClass security, List<Guid> pointPropertyGuidList)
		{
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				Point.EnumerateByPointPropertyListSQL(cmd, pointPropertyGuidList);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var pointDictionary = new Dictionary<Guid, Point>();

			DataTable table = set.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var point = new Point();

				point.AutoLoad(row);
				pointDictionary.Add(point.PointGuid, point);

			}

			return pointDictionary;
		}


		public PointCollection EnumerateActiveAlarmsBySite(SecurityClass security, Guid siteGuid)
		{
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				Point.EnumerateActiveAlarmsBySiteSQL(cmd, siteGuid);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var pointCollection = new PointCollection();

			DataTable table = set.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var point = new Point();

				point.AutoLoad(row);
				pointCollection.Add(point);

			}

			this.FillInSubLists(security, pointCollection);

			return pointCollection;
		}

		public PointCollection EnumerateByPointTemplateGuid(SecurityClass security, Guid pointTemplateGuid)
		{
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				Point.EnumerateByPointTemplateGuidSQL(cmd, pointTemplateGuid);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var pointCollection = new PointCollection();

			DataTable table = set.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var point = new Point();

				point.AutoLoad(row);
				pointCollection.Add(point);

			}

			this.FillInSubLists(security, pointCollection);

			return pointCollection;
		}

		public PointCollection EnumerateByPointTemplateGuids(SecurityClass security, Guid[] pointTemplateGuids)
		{
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				Point.EnumerateByPointTemplateGuidsSQL(cmd, security, pointTemplateGuids);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var pointCollection = new PointCollection();
            var applicationStringMaps = new ApplicationStringMapsClass();

            DataTable table = set.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var point = new Point();

				point.AutoLoad(row);
                point.PointCategoryCollection = applicationStringMaps.EnumerateByAssignedToGuidAndType(security, point.IdentityGuid, STRING_MAP_TYPE.POINT_CATEGORY);
                
				pointCollection.Add(point);

			}

			this.FillInSubLists(security, pointCollection);

			return pointCollection;
		}

		/// <summary>
		/// This method will enumerate the Points by point template Guids. It only gets the
		/// Points without tags and properties. Used by the Movement Setting dialog.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="pointTemplateGuids">The point template Guids used to filter.</param>
		/// <returns>Returns a collection of points based on the filter.</returns>
		public PointCollection EnumerateBasicByPointTemplateGuids(SecurityClass security, Guid[] pointTemplateGuids)
		{
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				Point.EnumerateByPointTemplateGuidsSQL(cmd, security, pointTemplateGuids);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var pointCollection = new PointCollection();

            DataTable table = set.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var point = new Point();

				point.AutoLoad(row);
                pointCollection.Add(point);

			}

			return pointCollection;
		}


		public PointCollection EnumerateBySiteAndPointTemplate(SecurityClass security, Guid siteGuid, Guid pointTemplateGuid)
		{
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				Point.EnumerateBySiteAndPointTemplateSQL(cmd, siteGuid, pointTemplateGuid);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var pointCollection = new PointCollection();

			DataTable table = set.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var point = new Point();

				point.AutoLoad(row);
				pointCollection.Add(point);

			}

			this.FillInSubLists(security, pointCollection);

			return pointCollection;
		}


		/// <summary>
		/// Enumerates points for the given site with information for the summary page.
		/// </summary>
		/// <param name="security">A valid FuelsManager security object.</param>
		/// <param name="siteGuid">The identity guid of the site to load.</param>
		/// <param name="includeDictionaries">boolean to indicate whether to include dictionaries</param>
		/// <param name="applyPointAccess">Enforce Point Access Rights.</param>
		/// <param name="propertyID">Only include points having a property.</param>
		/// <returns>A collection of partially populated points.</returns>
		public PointCollection EnumerateForSummary( SecurityClass security, Guid siteGuid, Boolean includeDictionaries = true, bool applyPointAccess = false, string propertyID = null)
		{
			DataSet set;

			using ( var cmd = new SqlCommand() )
			{
				Point.EnumerateForSummarySQL( cmd, siteGuid, security.UserGuid, applyPointAccess, propertyID);
				set = this.consolidatedDA.GetDataSet( cmd, security );
			}

			var pointCollection = new PointCollection();

			DataTable table = set.Tables[0];

			foreach ( DataRow row in table.Rows )
			{
				var point = new Point();

				point.AutoLoad( row );
				pointCollection.Add( point );
			}

			if (includeDictionaries)
			{
				this.FillInSubLists(security, pointCollection);
			}		

			return pointCollection;
		}

		/// <summary>
		/// Enumerates points for the given site with information for the point list page.
		/// It includes the associated categories.
		/// </summary>
		/// <param name="security">A valid FuelsManager security object.</param>
		/// <param name="siteGuid">The identity guid of the site to load.</param>
		/// <param name="includeDictionaries">boolean to indicate whether to include dictionaries</param>
		/// <param name="applyPointAccess">Enforce Point Access Rights.</param>
		/// <param name="propertyID">Only include points having a property.</param>
		/// <returns>A collection of partially populated points.</returns>
		public PointCollection EnumerateForSummaryWithCategories(SecurityClass security, Guid siteGuid, Boolean includeDictionaries = true, bool applyPointAccess = false, string propertyID = null)
		{
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				Point.EnumerateForSummarySQL(cmd, siteGuid, security.UserGuid, applyPointAccess, propertyID);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var pointCollection = new PointCollection();
			var applicationStringMaps = new ApplicationStringMapsClass();

			DataTable table = set.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var point = new Point();

				point.AutoLoad(row);
				point.PointCategoryCollection = applicationStringMaps.EnumerateByAssignedToGuidAndType(security, point.IdentityGuid, STRING_MAP_TYPE.POINT_CATEGORY);

				pointCollection.Add(point);
			}

			if (includeDictionaries)
			{
				this.FillInSubLists(security, pointCollection);
			}

			return pointCollection;
		}

		public List<PointValueIdentifier> EnumerateArchivedPointValueIdentifiersBySite(SecurityClass security, Guid siteGuid)
		{
			var pointValueIdentifierList = new List<PointValueIdentifier>();
			var settingDictionary = new Dictionary<string, object>();


			var pointTags = new PointTags();
			var pointTagGuidList = pointTags.EnumerateArchivedPointTagGuidsBySite(security, siteGuid);

			foreach(var pointTagGuid in pointTagGuidList)
			{
				pointValueIdentifierList.Add(new PointValueIdentifier(pointTagGuid, PointValueType.Tag, string.Empty));
			}

			var pointProperties = new PointProperties();
			var pointPropertyGuidAndTypeList = pointProperties.EnumeratePointPropertyGuidsAndTypes(security);

			foreach(var pointPropertyGuidAndType in pointPropertyGuidAndTypeList)
			{
				if(!pointPropertyGuidAndType.Value.Contains("FMBusinessObjects."))
				{
					continue;
				}

				object setting;

				if (!settingDictionary.TryGetValue(pointPropertyGuidAndType.Value, out setting))
				{
					setting = Activator.CreateInstance(Type.GetType(pointPropertyGuidAndType.Value + ",FMBusinessObjects"));
					settingDictionary.Add(pointPropertyGuidAndType.Value, setting);
				}

				var property = new PointProperty();
				property.PointPropertyGuid = pointPropertyGuidAndType.Key;
				property.Value = setting;

				var exposedtSettingPointValueIdentifierList = property.GetExposedSettingPointValueIdentifiers();
				foreach(var exsposedSettingPointValueIdentifier in exposedtSettingPointValueIdentifierList)
				{
					pointValueIdentifierList.Add(exsposedSettingPointValueIdentifier);
				}
			}

			var pointGuidList = this.EnumeratePointGuids(security);
			var point = new Point();
			foreach(var guid in pointGuidList)
			{
				point.PointGuid = guid;
				var exposedtSettingPointValueIdentifierList = point.GetExposedSettingPointValueIdentifiers();
				foreach (var exsposedSettingPointValueIdentifier in exposedtSettingPointValueIdentifierList)
				{
					pointValueIdentifierList.Add(exsposedSettingPointValueIdentifier);
				}

			}

			return pointValueIdentifierList;
		}

		public List<Guid> EnumeratePointGuids(SecurityClass security)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			DataSet set = null;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT PointGuid FROM tblPoint";
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			var pointGuidList = new List<Guid>();

			foreach (DataRow row in table.Rows)
			{
				pointGuidList.Add((Guid)row[0]);
			}

			return pointGuidList;
		}

		/// <summary>
		/// This method will retrieve the product graphic info based on the point association.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>Returns a list of product graphic info records.</returns>
		public List<Tuple<Guid, string, string, int>> EnumeratePointProductGraphicInfo(SecurityClass security )
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			DataSet dataSet;

			using (var cmd = new SqlCommand())
			{
				Point.EnumeratePointProductInfoSql(cmd);
				dataSet = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var pointProductInfoList = new List<Tuple<Guid, string, string, int>>();

			if (dataSet == null || dataSet.Tables[0] == null || dataSet.Tables[0].Rows.Count == 0)
			{
				return pointProductInfoList;
			}

			DataTable table = dataSet.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				Guid pointGuid		= row.IsNull("PointGuid") ? Guid.Empty : (Guid)row["PointGuid"];
				string productColor = row.IsNull("ProductColor") ? string.Empty : (string) row["ProductColor"];
				string patternColor = row.IsNull("PatternColor") ? string.Empty : (string) row["PatternColor"];
				int patternNumber	= row.IsNull("PatternNumber") ? 1 : (int) row["PatternNumber"];

				var pointProductInfoRecord = new Tuple<Guid, string, string, int>(pointGuid, productColor, patternColor, patternNumber);
				pointProductInfoList.Add(pointProductInfoRecord);
			}

			return pointProductInfoList;
		}


		public int EnabledPointCountForSimulator(SecurityClass security, string opcUaEndPoint)
		{
			DataSet set;

			using (SqlCommand cmd = new SqlCommand())
			{
				Point.CountEnabledForSimulatorSQL(cmd, opcUaEndPoint);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			if (set == null || set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
			{
				return -1;
			}

			DataTable table = set.Tables[0];
			DataRow row = table.Rows[0];

			if (row.IsNull("PointCount"))
			{
				return -1;
			}

			return (int)row["PointCount"];
		}

		public PointCollection EnumerateEnabledForSimulator(SecurityClass security, string opcUaEndPoint, int startIndex, int count)
		{
			//Need to modify to limit to number of points below so we don't bring back all the tags.
			var pointCollection = new PointCollection();
 
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				Point.EnumerateEnabledForSimulatorSQL(cmd, opcUaEndPoint, startIndex, count);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var point = new Point();

				point.AutoLoad(row);
				pointCollection.Add(point);
			}

			this.FillInSubLists(security, pointCollection);

			return pointCollection;
		}

		public PointCollection Get(SecurityClass security, List<Guid> pointGuidList)
		{
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				Point.GetListSQL(cmd, pointGuidList);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var pointCollection = new PointCollection();

			DataTable table = set.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var point = new Point();

				point.AutoLoad(row);

				pointCollection.Add(point);
			}

			this.FillInSubLists(security, pointCollection, pointGuidList);

			return pointCollection;
		}


		/// <summary>
		/// Enumerates enabled points for the specified site.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="siteGuid">The site unique identifier.</param>
		/// <returns>
		/// A collection of Points.
		/// </returns>
		public PointCollection EnumerateEnabledBySite( SecurityClass security, Guid siteGuid )
		{
			DataSet set;

			using ( var cmd = new SqlCommand() )
			{
				Point.EnumerateEnabledBySiteSQL( cmd, siteGuid );
				set = this.consolidatedDA.GetDataSet( cmd, security );
			}

			var pointCollection = new PointCollection();

			DataTable table = set.Tables[0];

				foreach ( DataRow row in table.Rows )
			{
				var point = new Point();

				point.AutoLoad( row );
				pointCollection.Add( point );
				}

			this.FillInSubLists(security, pointCollection);

			return pointCollection;
		}

		public Int64? GetMaxPointRowVersionForSite(SecurityClass security, Guid siteGuid)
		{
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT MAX(RowVersion) AS RowVersion FROM"
										+ " (SELECT MAX(UpdatedRowVersion) AS RowVersion FROM track.tblPoint WHERE CurrentSiteGuid = @SiteGuid AND UpdatedRowVersion <  MIN_ACTIVE_ROWVERSION()"
										+ " UNION SELECT MAX(InsertedRowVersion) AS RowVersion FROM track.tblPoint WHERE CurrentSiteGuid = @SiteGuid AND InsertedRowVersion <  MIN_ACTIVE_ROWVERSION()"
										+ " UNION SELECT MAX(DeletedRowVersion) AS RowVersion FROM track.tblPoint WHERE CurrentSiteGuid = @SiteGuid AND DeletedRowVersion <  MIN_ACTIVE_ROWVERSION()) RowVersions";

				cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);

				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			if (set == null || set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
			{
				return null;
			}

			DataTable table = set.Tables[0];
			DataRow row = table.Rows[0];

			if (row.IsNull("RowVersion"))
			{
				return null;
			}

			return BaseDataObject.RowVersionToInt64(row["RowVersion"] as byte[]);
		}

		public Dictionary<PointValueIdentifier, PointValueAccess> EnumerateRestrictedAccessByPointValueIdenfierList(SecurityClass security, List<PointValueIdentifier> pointValueIdentifierList)
		{
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "[dbo].[usp_EnumerateRestrictedAccessByPointValueIdentifiers]";
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
				cmd.Parameters.AddWithValue("@UserGuid", security.UserGuid);

				var pointValueIdentifiers = new DataTable();
				pointValueIdentifiers.Columns.Add("Guid", typeof(Guid));
				pointValueIdentifiers.Columns.Add("PropertyId", typeof(string));
				pointValueIdentifiers.Columns.Add("ValueType", typeof(byte));

				foreach (var pointValueIdentifier in pointValueIdentifierList)
				{
					var row = pointValueIdentifiers.NewRow();
					row[0] = pointValueIdentifier.IdentityGuid;
					row[1] = pointValueIdentifier.PropertyID;
					row[2] = pointValueIdentifier.PointValueType;

					pointValueIdentifiers.Rows.Add(row);
				}

				SqlParameter tableValuedParameter = cmd.Parameters.Add("@PointValueIdentifiers", SqlDbType.Structured);
				tableValuedParameter.Value = pointValueIdentifiers;
				tableValuedParameter.TypeName = "dbo.utt_PointValueIdentifier";

				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var pointValueAccessDictionary = new Dictionary<PointValueIdentifier, PointValueAccess>();
			if (set == null || set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
			{
				return pointValueAccessDictionary;
			}

			DataTable table = set.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var pointValueIdentifier = new PointValueIdentifier() { IdentityGuid = (Guid)row["PointValueGuid"], PropertyID = (string)row["PointValuePropertyId"], PointValueType = (PointValueType) Convert.ToInt32(row["PointValueType"]) };
				var pointValueAccess = new PointValueAccess() { View = (bool)row["View"], Modify = (bool)row["Modify"], ExceedRange = (bool)row["ExceedRange"], Override = (bool)row["Override"] };
				pointValueAccessDictionary.Add(pointValueIdentifier, pointValueAccess);
			}

			return pointValueAccessDictionary;

		}

		public List<KeyValuePair<Guid, string>> EnumeratePointIdListForSiteTemplateTypeTemplateCategory(SecurityClass security, Guid siteGuid, Guid? pointTemplateTypeGuid, Guid? pointTemplateGuid, Guid? pointCategoryGuid, bool applyPointAccess)
		{
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText		+= "SET NOCOUNT ON";

				if (applyPointAccess)
				{
					cmd.CommandText	+= " DECLARE @PointAccessGroupGuidTable TABLE(PointAccessGroupGuid UniqueIdentifier)";

					cmd.CommandText	+= " INSERT INTO @PointAccessGroupGuidTable"
											+ " SELECT pagtug.PointAccessGroupGuid FROM map.tblUserToGroup utg"
											+ " INNER JOIN map.tblPointAccessGroupToUserGroup pagtug ON pagtug.UserGroupGuid = utg.GroupGuid"
											+ " INNER JOIN dbo.tblPointAccessGroup pag ON pag.PointAccessGroupGuid = pagtug.PointAccessGroupGuid AND pag.SiteGuid = utg.SiteGuid"
											+ " WHERE utg.SiteGuid = @SiteGuid AND utg.UserGuid = @UserGuid";

					cmd.CommandText += " IF OBJECT_ID('tempdb.#PointTable') IS NOT NULL"
											+ " DROP TABLE tempdb.#PointTable"
											+ " CREATE TABLE tempdb.#PointTable"
											+ " ("
											+ "		PointGuid UniqueIdentifier,"
											+ "		PointAccessGroupGuid UniqueIdentifier"
											+ " )";

					cmd.CommandText += " INSERT INTO #PointTable SELECT DISTINCT PointGuid, PointAccessGroupGuid FROM"
											+ " (SELECT p.PointGuid, pagtpt.PointAccessGroupGuid FROM dbo.tblPoint p"
											+ " INNER JOIN map.tblPointAccessGroupToPointTemplate pagtpt ON pagtpt.PointTemplateGuid = p.PointTemplateGuid"
											+ " INNER JOIN @PointAccessGroupGuidTable paggt ON paggt.PointAccessGroupGuid = pagtpt.PointAccessGroupGuid"
											+ " WHERE p.SiteGuid = @SiteGuid"
											+ " UNION"
											+ " SELECT p.PointGuid, pagtp.PointAccessGroupGuid FROM dbo.tblPoint p"
											+ " INNER JOIN map.tblPointAccessGroupToPoint pagtp ON pagtp.PointGuid = p.PointGuid"
											+ " INNER JOIN @PointAccessGroupGuidTable paggt ON paggt.PointAccessGroupGuid = pagtp.PointAccessGroupGuid"
											+ " WHERE p.SiteGuid = @SiteGuid) s";
				}

				cmd.CommandText  += " SELECT DISTINCT p.PointGuid, p.ID FROM tblPoint p";

				if (pointTemplateTypeGuid.HasValue && pointTemplateTypeGuid != Guid.Empty)
				{
					cmd.CommandText += " INNER JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = p.PointTemplateGuid AND pt.PointTemplateTypeApplicationStringGuid = @PointTemplateTypeGuid";
					cmd.Parameters.AddWithValue("@PointTemplateTypeGuid", pointTemplateTypeGuid.Value);
				}

				if (pointCategoryGuid.HasValue && pointCategoryGuid != Guid.Empty)
				{
					cmd.CommandText += " INNER JOIN map.tblApplicationStringToPointCategory astpc ON astpc.PointGuid = p.PointGuid AND astpc.ApplicationStringGuid = @PointCategoryGuid";
					cmd.Parameters.AddWithValue("@PointCategoryGuid", pointCategoryGuid.Value);
				}

				if(applyPointAccess)
				{
					cmd.CommandText += " INNER JOIN #PointTable pgt ON pgt.PointGuid = p.PointGuid";
					cmd.Parameters.AddWithValue("@UserGuid", security.UserGuid);
				}

				cmd.CommandText += " WHERE p.SiteGuid = @SiteGuid";
				cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);

				if (pointTemplateGuid.HasValue && pointTemplateGuid != Guid.Empty)
				{
					cmd.CommandText += " AND p.PointTemplateGuid = @PointTemplateGuid";
					cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid.Value);
				}


				cmd.CommandText += " ORDER BY ID";


				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var pointList = new List<KeyValuePair<Guid, string>>();
			if (set == null || set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
			{
				return pointList;
			}

			DataTable table = set.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				pointList.Add(new KeyValuePair<Guid, string>((Guid)row["PointGuid"], row["ID"] as string));
			}

			return pointList;
		}

		public Dictionary<Guid, Point> EnumerateByPointList(SecurityClass security, List<Guid> pointGuidList)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			DataSet set = null;
			var modulePointProperty = new PointProperty();

			using (var cmd = new SqlCommand())
			{
				Point.EnumerateByPointListSQL(cmd, pointGuidList);
				set =	this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			var pointDictionary = new Dictionary<Guid, Point>();

			foreach (DataRow row in table.Rows)
			{
				var point = new Point();

				point.AutoLoad(row);

				pointDictionary.Add(point.PointGuid, point);

			}

			return pointDictionary;
		}


		public Dictionary<PointValueIdentifier, PointValue> EnumerateByPointValueIdentifierList(SecurityClass security, List<PointValueIdentifier> pointValueIdentifierList)
		{
			var tagGuidDictionary = new Dictionary<Guid, Guid>();
			var propertyGuidDictionary = new Dictionary<Guid, Guid>();
			var pointGuidDictionary = new Dictionary<Guid, Guid>();

			foreach (var pointValueIdentifier in pointValueIdentifierList)
			{
				if (pointValueIdentifier.PointValueType == PointValueType.Tag)
				{
					if (!tagGuidDictionary.ContainsKey(pointValueIdentifier.IdentityGuid))
					{
						tagGuidDictionary.Add(pointValueIdentifier.IdentityGuid, pointValueIdentifier.IdentityGuid);
					}
				}
				else if (pointValueIdentifier.PointValueType == PointValueType.Setting)
				{
					if (!propertyGuidDictionary.ContainsKey(pointValueIdentifier.IdentityGuid))
					{
						propertyGuidDictionary.Add(pointValueIdentifier.IdentityGuid, pointValueIdentifier.IdentityGuid);
					}
				}
				else if (pointValueIdentifier.PointValueType == PointValueType.Point)
				{
					if (!pointGuidDictionary.ContainsKey(pointValueIdentifier.IdentityGuid))
					{
						pointGuidDictionary.Add(pointValueIdentifier.IdentityGuid, pointValueIdentifier.IdentityGuid);
					}
				}
			}

			var pointValueDictionary = new Dictionary<PointValueIdentifier, PointValue>();
			Dictionary<Guid, PointTag> pointTagDictionary= null;
			Dictionary<Guid, PointProperty> pointPropertyDictionary = null;
			Dictionary<Guid, Point> pointDictionary = null;
			var pointTags = new PointTags();

			if (tagGuidDictionary.Count > 0)
			{
				pointTagDictionary = pointTags.EnumerateByTagList(security, tagGuidDictionary.Values.ToList());
			}

			if (propertyGuidDictionary.Count > 0)
			{
				var pointProperties = new PointProperties();
				var pointPropertyList = propertyGuidDictionary.Values.ToList();
				pointPropertyDictionary = pointProperties.EnumerateByPointPropertyList(security, pointPropertyList);
				pointDictionary = this.EnumerateByPointPropertyList(security, pointPropertyList);
			}

				if (pointGuidDictionary.Count > 0)
				{
					var pointList = pointGuidDictionary.Values.ToList();

					if (pointDictionary == null)
					{
						pointDictionary = this.EnumerateByPointList(security, pointList);
					}
					else
					{
						foreach (var point in this.EnumerateByPointList(security, pointList).Values)
						{
							if (!pointDictionary.ContainsKey(point.PointGuid))
							{
								pointDictionary.Add(point.PointGuid, point);
							}
						}
					}
				}

			var limitTagGuidList = new List<Guid>();
			foreach (var pointValueIdentifier in pointValueIdentifierList)
			{
				if (pointValueIdentifier.PointValueType == PointValueType.Tag)
				{
					if (!pointValueDictionary.ContainsKey(pointValueIdentifier))
					{
						PointTag pointTag;
						if (pointTagDictionary.TryGetValue(pointValueIdentifier.IdentityGuid, out pointTag))
						{
							var pointValueForPointTag = new PointValue(pointTag);
							if (pointValueIdentifier.IncludeAlarmLimits)
							{
								foreach (var alarm in pointTag.Alarms.Values)
								{
									foreach (var alarmTest in alarm.AlarmTests.Values)
									{
										limitTagGuidList.Add(alarmTest.LimitTagGuid);
									}
								}
							}
							pointValueDictionary.Add(pointValueIdentifier, pointValueForPointTag);
						}
					}
				}
				else if (pointValueIdentifier.PointValueType == PointValueType.Setting)
				{
					if (!pointValueDictionary.ContainsKey(pointValueIdentifier))
					{
						PointProperty pointProperty;
						if (pointPropertyDictionary.TryGetValue(pointValueIdentifier.IdentityGuid, out pointProperty))
						{
							Point point;
							if (pointDictionary.TryGetValue(pointProperty.PointGuid, out point))
							{
								pointValueDictionary.Add(pointValueIdentifier, new PointValue(pointValueIdentifier, pointProperty, point));
							}
						}
					}
				}
				else if (pointValueIdentifier.PointValueType == PointValueType.Point)
				{
					if (!pointValueDictionary.ContainsKey(pointValueIdentifier))
					{
						Point point;
						if (pointDictionary.TryGetValue(pointValueIdentifier.IdentityGuid, out point))
						{
							pointValueDictionary.Add(pointValueIdentifier, new PointValue(pointValueIdentifier, point));

						}
					}
				}
			}

			//populate limitValues
			if (limitTagGuidList.Count > 0 && pointTagDictionary != null)
			{
				var limitTagDictionary = pointTags.EnumerateByTagList(security, limitTagGuidList);
				if (limitTagDictionary != null && limitTagDictionary.Count > 0)
				{
					foreach (var pointValueDictionaryEntry in pointValueDictionary)
					{
						if (pointValueDictionaryEntry.Key.PointValueType == PointValueType.Tag
						    && pointValueDictionaryEntry.Key.IncludeAlarmLimits)
						{
							PointTag alarmPointTag;
							if (pointTagDictionary.TryGetValue(pointValueDictionaryEntry.Key.IdentityGuid, out alarmPointTag))
							{
								foreach (var alarm in alarmPointTag.Alarms.Values)
								{
									foreach (var alarmTest in alarm.AlarmTests.Values)
									{
										PointTag limitPointTag;
										if (limitTagDictionary.TryGetValue(alarmTest.LimitTagGuid, out limitPointTag))
										{
											var alv = new AlarmLimitValue
											{
												IdentityGuid = limitPointTag.PointTagGuid,
												Value = limitPointTag.Value,
												AlarmPriorityGuid = alarmTest.AlarmPriorityGuid
											};
											pointValueDictionaryEntry.Value.AlarmLimitList.Add(alv);
										}
									}
								}
							}
						}
					}	
				}
			}
			return pointValueDictionary;
		}

		public Dictionary<PointValueIdentifier, string> EnumeratePointValueIdentifiersForPointFilterByType(SecurityClass security, Guid pointGuid, PointValueType valueType, bool filter, string dataTypeString, PointValueFieldType fieldFilter, bool applyPointAccess)
		{
			var point = this.Get(security, pointGuid, applyPointAccess);
			if (point !=null)
				return point.EnumeratePointValueIdentifiersForPointFilterByType(valueType, filter, dataTypeString, fieldFilter);
			else
				return new Dictionary<PointValueIdentifier, string>(); //if there is no point, just return an empty dictionary so the model can still render
		}

		public Dictionary<PointValueIdentifier, string> EnumeratePointValueIdentifiersForPoint(SecurityClass security, Guid pointGuid, PointValueType valueType, bool applyPointAccess)
		{
			return this.EnumeratePointValueIdentifiersForPointFilterByType(security, pointGuid, valueType, false, string.Empty, PointValueFieldType.VALUE, applyPointAccess);
		}


		private void ClearProductAssignmentByProduct(SecurityClass security, Guid productGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Check security rights


			// Delete point
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "UPDATE dbo.tblPoint SET ProductGuid = null WHERE ProductGuid = @ProductGuid";
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@ProductGuid", productGuid);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		private void ClearProductAssignmentBySiteAndProduct(SecurityClass security, Guid siteGuid, Guid productGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Check security rights


			// Delete point
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "UPDATE dbo.tblPoint SET ProductGuid = null WHERE SiteGuid = @SiteGuid AND ProductGuid = @ProductGuid";
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
				cmd.Parameters.AddWithValue("@ProductGuid", productGuid);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = false, TransactionAutoComplete = true)]
		public void Import(SecurityClass security, Point point)
		{
			security.ThrowIfNull("security");
			point.ThrowIfNull("point");

			try
			{

				var systemPoint = this.Get(security, point.IdentityGuid);
				//point.IdentityGuid = this.GetIdentityGuid(security, point.ID);
				if (systemPoint == null && point.IdentityGuid == Guid.Parse("00000000-0000-0000-0000-000000000000"))
				{
					point.IdentityGuid = Guid.NewGuid();

						List<Guid[]> guidTable = new List<Guid[]>();
					//give new guids to all the tags
					List<Guid> tagGuidsToDelete = new List<Guid>();
					List<PointTag> tagsToAdd = new List<PointTag>();
					foreach (var tag in point.Tags)
					{
						tagGuidsToDelete.Add(tag.Value.IdentityGuid);
						guidTable.Add(new Guid[] { tag.Value.IdentityGuid, tag.Value.IdentityGuid = Guid.NewGuid() });
						tag.Value.PointGuid = point.IdentityGuid;

						//give new guids to all the alarms, and point them to the new tag&point
						List<Guid> alarmGuidsToDelete = new List<Guid>();
						List<Alarm> alarmstoAdd = new List<Alarm>();
						foreach (var alarm in tag.Value.Alarms)
						{
							alarmGuidsToDelete.Add(alarm.Value.IdentityGuid);
							guidTable.Add(new Guid[] { alarm.Value.IdentityGuid, alarm.Value.IdentityGuid = Guid.NewGuid() });
							alarm.Value.PointGuid = point.IdentityGuid;
							alarm.Value.InputTagGuid = tag.Value.IdentityGuid;

							List<Guid> alarmStatusGuidstoDelete = new List<Guid>();
							List<PointTagAlarmStatus> alarmStatusestoAdd = new List<PointTagAlarmStatus>();
							foreach (var alarmStatus in alarm.Value.AlarmStatus)
							{
								alarmStatusGuidstoDelete.Add(alarmStatus.Value.IdentityGuid);
								alarmStatus.Value.IdentityGuid = Guid.NewGuid();
								alarmStatusestoAdd.Add(alarmStatus.Value);
							}

							foreach (var alarmStatusGuidtoDelete in alarmStatusGuidstoDelete)
							{
								alarm.Value.AlarmStatus.Remove(alarmStatusGuidtoDelete);
							}
							foreach (var alarmStatustoAdd in alarmStatusestoAdd) 
							{
							alarm.Value.AlarmStatus.Add(alarmStatustoAdd.IdentityGuid, alarmStatustoAdd);
							}

						//give new guids to all the alarm tests and point them to the new alarm/tag/point
							List<Guid> alarmTestGuidstoDelete = new List<Guid>();
							List<AlarmTest> alarmTeststoAdd = new List<AlarmTest>();
							foreach (var alarmTest in alarm.Value.AlarmTests)
							{
								alarmTestGuidstoDelete.Add(alarmTest.Value.IdentityGuid);
								guidTable.Add(new Guid[] { alarmTest.Value.IdentityGuid, alarmTest.Value.IdentityGuid = Guid.NewGuid()});
								alarmTest.Value.AlarmGuid = alarm.Value.IdentityGuid;

								alarmTeststoAdd.Add(alarmTest.Value);
							}
							foreach (var alarmTestGuidtoDelete in alarmTestGuidstoDelete)
							{
								alarm.Value.AlarmTests.Remove(alarmTestGuidtoDelete);
							}
							foreach (var alarmTestToAdd in alarmTeststoAdd)
							{
								alarm.Value.AlarmTests.Add(alarmTestToAdd.IdentityGuid, alarmTestToAdd);
							}
							alarmstoAdd.Add(alarm.Value);
						}
						foreach (var alarmGuidToDelete in alarmGuidsToDelete)
						{
							tag.Value.Alarms.Remove(alarmGuidToDelete);
						}
						foreach (var alarmToAdd in alarmstoAdd)
						{
							tag.Value.Alarms.Add(alarmToAdd.IdentityGuid,alarmToAdd);
						}

						tagsToAdd.Add(tag.Value);
					}
					foreach (Guid guidtoDelete in tagGuidsToDelete)
					{
						point.Tags.Remove(guidtoDelete);
					}
					foreach (PointTag tagToAdd in tagsToAdd)
					{
						point.Tags.Add(tagToAdd.IdentityGuid, tagToAdd);
					}

					//repair the relationships
					foreach (var tag in point.Tags)
					{
						foreach (var alarm in tag.Value.Alarms)
						{
							foreach (Guid[] guidSet in guidTable)
							{
								if (alarm.Value.InputTagGuid == guidSet[0])
								{
									alarm.Value.InputTagGuid = guidSet[1];
								}
								if (alarm.Value.AlarmStateTagGuid == guidSet[0])
								{
									alarm.Value.AlarmStateTagGuid = guidSet[1];
								}
							}
							foreach (var alarmTest in alarm.Value.AlarmTests)
							{
								foreach (Guid[] guidSet in guidTable)
								{
									if (alarmTest.Value.AlarmGuid == guidSet[0])
									{
										alarmTest.Value.AlarmGuid = guidSet[1];
									}
									if (alarmTest.Value.LimitTagGuid == guidSet[0])
									{
										alarmTest.Value.LimitTagGuid = guidSet[1];
									}
								}
							}
							foreach (var alarmStatus in alarm.Value.AlarmStatus)
							{
								foreach (Guid[] guidSet in guidTable)
								{
									if (alarmStatus.Value.AlarmTestGuid == guidSet[0])
									{
										alarmStatus.Value.AlarmTestGuid = guidSet[1];
									}
								}
							}
						}
					}

					List<Guid> propertyGuidstoDelete = new List<Guid>();
					List<PointProperty> propertiesToAdd = new List<PointProperty>();
					foreach (var property in point.Properties)
					{
						propertyGuidstoDelete.Add(property.Value.IdentityGuid);
						property.Value.IdentityGuid = Guid.NewGuid();
						propertiesToAdd.Add(property.Value);
					}

					AddStrapTablePropertyIfMissing(security, ref point);
               updateSiteCloseoutXML(security);
               this.Add(security, point, false);

				}
				else if (systemPoint == null)
				{
					AddStrapTablePropertyIfMissing(security, ref point);
					updateSiteCloseoutXML(security);
					this.Add(security, point, false);
				}
				else
				{ 
					this.Modify(security, point);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("[Point Import Error ID] : " + point.ID + ", " + ex.Message);
			}
		}

		private void AddStrapTablePropertyIfMissing(SecurityClass security, ref Point point)
		{
			PointTemplates ptsObj = new PointTemplates();
			PointTemplate pt = ptsObj.Get(security, point.PointTemplateGuid);

			foreach (var kvp in pt.Properties)
			{
				// if template has StrapTable property but the point does not
				if (kvp.Value.ID.ToUpper() == "STRAP TABLE")
				{
					bool strapTableExists = false;
					foreach (PointProperty pp in point.Properties.Values)
					{
						if (pp.ID.ToUpper() == "STRAP TABLE")
						{
							strapTableExists = true; 
							break;
						}
					}
					if (!strapTableExists)
					{
						PointProperty pointProperty = new PointProperty(kvp.Value);
						point.Properties.Add(pointProperty.IdentityGuid, pointProperty);
						break;
					}
				}
			}
		}

		[OperationBehavior(TransactionScopeRequired = false, TransactionAutoComplete = true)]
        public void ModifyTagsOnly(SecurityClass security, Point point)
        {
            security.ThrowIfNull("security");
            point.ThrowIfNull("point");

            try
            {

                var systemPoint = this.Get(security, point.IdentityGuid);
                //point.IdentityGuid = this.GetIdentityGuid(security, point.ID);
                if (systemPoint == null || point.IdentityGuid == Guid.Parse("00000000-0000-0000-0000-000000000000"))
                {
						  throw new Exception("Guid for point " + point.ID + " not found. Creation of points are not supported for Import - Point Tag Modify");
                }
                else
                {
						  point.Properties = systemPoint.Properties;
                    foreach (PointTag tag in point.Tags.Values)
                    {
								try
								{
									 PointTag systemTag = systemPoint.Tags[tag.IdentityGuid] as PointTag;
									 tag.Alarms = systemTag.Alarms;
								}
								catch
								(Exception ex)
								{
									 throw new Exception("Creation of tags are not supported for Import - Point Tag Modify. " + ex.Message);
								}
                    }
                    this.Modify(security, point);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("[Point Import Error ID] : " + point.ID + ", " + ex.Message);
            }
        }


      public Dictionary<string, Dictionary<Guid, Guid>> EnumerateTerminalAutomationTankTags(SecurityClass security, Guid siteGuid)
		{
			var pointDictionary = new Dictionary<string, Dictionary<Guid, Guid>>();

			var wellKnownIdentityGudList = new List<Guid>() { Guids.LevelProductGuid,
																		Guids.TemperatureProductGuid,
																		Guids.VolumeGrossObservedGuid,
																		Guids.VolumeNetStandardGuid,
																		Guids.DensityProductObservedGuid,
																		Guids.DensityProductStandardGuid,
																		Guids.MassLiquidGuid,
																		Guids.VolumeCorrectionFactorGuid,
																		Guids.VolumeGrossObservedAvailableGuid,
																		Guids.VolumeGrossObservedRemainingGuid,
																		Guids.OperationalModeGuid,
																		Guids.PressureVaporGuid,
																		Guids.VolumeNetStandardAvailableGuid,
																		Guids.VolumeNetStandardRemainingGuid,
																		Guids.TankStatusGuid
																	};
			DataSet set = null;
			using (var cmd = new SqlCommand())
			{
				Point.EnumerateWellKnownIdentitySQL(cmd, siteGuid, Guids.PointTypeTankGuid, wellKnownIdentityGudList);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var pointID = row["ID"] as string;
				var wellKnownIdentityGuid = (Guid) row["WellKnownIdentityGuid"];
				var tagGuid = (Guid)row["PointTagGuid"];


				if(!pointDictionary.ContainsKey(pointID))
				{
					pointDictionary.Add(pointID, new Dictionary<Guid,Guid> ());
				}

				var tagDictionary = pointDictionary[pointID];

				tagDictionary.Add(wellKnownIdentityGuid, tagGuid);
			}


			return pointDictionary;
		}

		public NodeModuleType GetMovementNodeModuleType(SecurityClass security, Guid pointGuid)
		{
			NodeModuleType nodeModuleType = NodeModuleType.None;
			DataSet set = null;
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "[dbo].[usp_GetMovementNodeModuleType]";
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue("@PointGuid", pointGuid);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			if (set != null
			&& set.Tables.Count == 1
			&& set.Tables[0].Rows.Count == 1)
			{
				nodeModuleType = (NodeModuleType)set.Tables[0].Rows[0][0];
			}

			return nodeModuleType;

		}

		#region Explicit Interface Methods
		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
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
				throw new ArgumentNullException("security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}

			// Purge Points
			var siteClassObj = Object as SiteClass;

			if (siteClassObj != null)
			{
				var site = siteClassObj;
				PointCollection pointCollection = this.EnumerateBySite(security, site.SiteGuid);

				foreach (var point in pointCollection)
				{
					this.Purge(security, point.IdentityGuid);
				}
			}

			else if (typeof(ApplicationStringClass).IsInstanceOfType(Object))
			{
				var applicationString = (ApplicationStringClass)Object;


				if (applicationString.Type == STRING_TYPE.POINT_CATEGORY)
				{
					var applicationStringMaps = new ApplicationStringMapsClass();
					var collection = applicationStringMaps.EnumerateByApplicationStringGuidAndType(security, applicationString.IdentityGuid, STRING_MAP_TYPE.POINT_CATEGORY);

					foreach (var category in collection)
					{
						applicationStringMaps.Purge(security, category.IdentityGuid, category.Type);
					}
				}
			}

			else if (typeof(ProductClass).IsInstanceOfType(Object))
			{
				var product = Object as ProductClass;
				this.ClearProductAssignmentByProduct(security, product.IdentityGuid);
			}

			else if (typeof(EntityToSiteMapClass).IsInstanceOfType(Object))
			{
				var entityToSiteMap = Object as EntityToSiteMapClass;
				if(entityToSiteMap.TypeID == ENTITY_TYPE.PRODUCT)
				{
					this.ClearProductAssignmentBySiteAndProduct(security, entityToSiteMap.SiteGuid, entityToSiteMap.IdentityGuid);
				}
				else if(entityToSiteMap.TypeID == ENTITY_TYPE.POINT_TEMPLATE)
				{
					var points = this.EnumerateBySiteAndPointTemplate(security, entityToSiteMap.SiteGuid, entityToSiteMap.IdentityGuid);
					foreach(var point in points)
					{
							this.Purge(security, point.IdentityGuid);
					}

				}
			}
		}

		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}
		}

		void updateSiteCloseoutXML(SecurityClass security)
		{
            //Capture PointTagRefDataAsXML for day before the points are changed.

            SiteCloseoutTimeClass closeoutTime = new SiteCloseoutTimeClass();
            SiteCloseoutTimes closeoutTimes = new SiteCloseoutTimes();

            closeoutTime.SiteGuid = security.SiteGuid;
            closeoutTime.ExpirationDate = DateTimeOffset.Now;
				closeoutTime.PointsChanged = true;

            closeoutTimes.SetCloseoutTime(security, closeoutTime);
        }
        #endregion
    }


}
