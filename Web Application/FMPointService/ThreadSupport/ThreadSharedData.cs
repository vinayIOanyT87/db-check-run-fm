namespace FMPointService.ThreadSupport
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using Logging;
	using System.Configuration;
	using InProcLogging;
	using Archiving;
	using PointExecution;
	using System.Reflection;

	using CSScriptLibrary;
	using OpcClient;
	using Opc.Ua;
	using Softing.Opc.Ua.Client;
   using global::FMPointService.AlarmAndEventArchive;

	internal class ThreadSharedData
	{
		#region interface imports

		public ArchiveProcessorSignaler ArchiveProcessorSignaler = new ArchiveProcessorSignaler();

		public ArchiveRecordQueuer ArchiveRecordQueuer = new ArchiveRecordQueuer();

		public PointExecutionQueuer PointExecutionQueuer = new PointExecutionQueuer();
		
		public PointTagValueChanger PointTagValueChanger = new PointTagValueChanger();

		public StatisticsLogger StatisticsLogger = new StatisticsLogger();

		public EventLogger eventLogger = new EventLogger();

		#endregion
		
		public const string UserId = "Administrator";

		protected Dictionary<Guid, PointTemplateLogic> PointLogicDictionary = new Dictionary<Guid, PointTemplateLogic>();

		protected Dictionary<Guid, Point> PointDictionary = new Dictionary<Guid, Point>();

		protected Dictionary<Guid, PointTag> TagDictionary = new Dictionary<Guid, PointTag>();

		protected Dictionary<Guid, PointProperty> PropertyDictionary = new Dictionary<Guid, PointProperty>();

		protected Dictionary<Guid, PointTemplateDataContainer> PointTemplateDataContainerDictionary = new Dictionary<Guid, PointTemplateDataContainer>();

		protected Dictionary<Guid, PointTag> TagHoldoffDeadbandDictionary = new Dictionary<Guid, PointTag>();


		public SecurityClass Login(string siteID)
		{
				var security = new SecurityClass { UserGuid = Guids.UserAdminGuid, SiteGuid = Guids.SiteAdminGuid };
				security.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
				security.UserID = UserId;
				if (siteID != "SiteAdmin")
				{
					var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.GetByID(security, siteID, false));
					security = new SecurityClass { UserGuid = Guids.UserAdminGuid, SiteGuid = site.SiteGuid };
					security.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
					security.UserID = UserId;
				}
				return security;
		}

		protected static ThreadSharedData inst = null;

		protected bool enableArchiveData = true;


      public bool EnableArchiveData
		{
			get
			{
				return enableArchiveData;
			}
		}


		protected int archiveQueueMaximum = 10000000;

		public int ArchiveQueueMaximum
		{
			get
			{
				return archiveQueueMaximum;
			}
		}

      protected int maxArchiveRecordsPerCall = 16;

      public int MaxArchiveRecordsPerCall
      {
         get
         {
                return maxArchiveRecordsPerCall;
         }
      }

      protected bool useOpcUaClientPolling = false;

		public bool UseOpcUaClientPolling
		{
			get
			{
				return useOpcUaClientPolling;
			}
		}

        protected int movementResolutionInSeconds = 1;
		  public int MovementResolutionInSeconds
		  {
				get { 
					 return movementResolutionInSeconds;
				}
        }
        protected int updateInactiveMovementsEveryXIterations = 10;
		  public int UpdateInactiveMovementsEveryXIterations
		  {
				get
				{
					 return updateInactiveMovementsEveryXIterations;
				}
		  }


        protected ThreadSharedData()
		{
			try
			{
				enableArchiveData = bool.Parse(ConfigurationManager.AppSettings["EnableArchivingData"]);
				archiveQueueMaximum = int.Parse(ConfigurationManager.AppSettings["ArchiveQueueMaximum"]);
            }
			catch (Exception eadEx)
			{
				Logger.LogError("ThreadSharedData exception : " + eadEx.Message);
			}

			try
			{
				useOpcUaClientPolling = bool.Parse(ConfigurationManager.AppSettings["UseOpcUaClientPolling"]);
			}
			catch (Exception eadEx)
			{
				Logger.LogError("ThreadSharedData exception : " + eadEx.Message);
			}

         try
         {
            maxArchiveRecordsPerCall = int.Parse(ConfigurationManager.AppSettings["MaxArchiveRecordsPerCall"]);
         }
         catch (Exception eadEx)
         {
             Logger.LogError("ThreadSharedData exception : " + eadEx.Message);
         }

			try
			{
             movementResolutionInSeconds = int.Parse(ConfigurationManager.AppSettings["MovementResolutionInSeconds"]);
             updateInactiveMovementsEveryXIterations = int.Parse(ConfigurationManager.AppSettings["UpdateInactiveMovementsEveryXIterations"]);
         }
            catch (Exception eadEx)
            {
                Logger.LogError("ThreadSharedData exception : " + eadEx.Message);
            }
        }

		public static ThreadSharedData Instance()
		{
			if (inst == null)
			{
				inst = new ThreadSharedData();
			}
			return inst;
		}

		protected object LockObject = new object();

		public int PointCount
		{
				get
				{
					lock (this.LockObject)
					{
						return this.PointDictionary.Count;
					}
				}
		}

		public int TagCount
		{
				get
				{
					lock (this.LockObject)
					{
						return this.TagDictionary.Count;
					}
				}
		}

		public int NumNonGoodOpcTags
		{
				get
				{
					lock (this.LockObject)
					{
						int numNonGoodOpcTags = 0;
						foreach(var tag in TagDictionary.Values)
						{
								if(tag.InputOutputType == PointTemplateTag.PointTagInputOutputType.OpcUa
								&& !string.IsNullOrEmpty(tag.OpcUaNodeId)
								&& tag.IsBad())
								{
									numNonGoodOpcTags++;
								}
						}
						return numNonGoodOpcTags;
					}
				}
		}

		public void Clear()
		{
			lock (this.LockObject)
			{
				this.PropertyDictionary.Clear();
				this.TagDictionary.Clear();
				this.PointDictionary.Clear();
				this.PointLogicDictionary.Clear();
				this.PropertyDictionary.Clear();
			}
		}


		protected void DeleteTag(PointTag tag)
		{
				this.TagDictionary.Remove(tag.PointTagGuid);
				this.PointDictionary[tag.PointGuid].Tags.Remove(tag.PointTagGuid);
		}

		protected void DeleteTags(PointTagCollection tags)
		{
				foreach (var t in tags)
				{
					this.DeleteTag(t);
				}
		}

		protected void DeleteProperty(PointProperty property)
		{
			this.PropertyDictionary.Remove(property.PointPropertyGuid);
			this.PointDictionary[property.PointGuid].Properties.Remove(property.PointPropertyGuid);
		}

		protected void DeleteProperties(PointPropertyCollection properties)
		{
			foreach (var p in properties)
			{
				this.DeleteProperty(p);
			}
		}


		protected void DeletePoints(PointCollection points)
		{
			foreach (var point in points)
			{
				foreach (var t in point.Tags)
				{
					this.DeleteTag(t.Value);
				}
				foreach (var p in point.Properties)
				{
					this.DeleteProperty(p.Value);
				}
				this.PointDictionary.Remove(point.PointGuid);
				this.PointLogicDictionary.Remove(point.PointGuid);
			}
		}

		protected void AddTag(PointTag tag)
		{
				this.TagDictionary.Add(tag.PointTagGuid, tag);
				this.PointDictionary[tag.PointGuid].Tags.Add(tag.PointTagGuid,tag);
		}

		protected void AddTags(PointTagCollection tags)
		{
				foreach (var t in tags)
				{
					this.AddTag(t);
				}
		}

		protected void AddProperty(PointProperty property)
		{
			this.PropertyDictionary.Add(property.PointPropertyGuid, property);
			this.PointDictionary[property.PointGuid].Properties.Add(property.PointPropertyGuid, property);
		}

		protected void AddProperties(PointPropertyCollection properties)
		{
			foreach (var p in properties)
			{
				this.AddProperty(p);
			}
		}


		protected void AddPoints(PointCollection points)
		{
			foreach (var point in points)
			{
				this.PointDictionary.Add(point.PointGuid, point);
				foreach (var t in point.Tags)
				{
					this.AddTag(t.Value);
				}
				foreach (var p in point.Properties)
				{
					this.AddProperty(p.Value);
				}
			}
		}

		protected void UpdateTag(PointTag tag)
		{
			this.TagDictionary.Remove(tag.PointTagGuid);
			this.TagDictionary.Add(tag.PointTagGuid, tag);
			this.PointDictionary[tag.PointGuid].Tags[tag.PointTagGuid] = tag;
		}

		protected void UpdateTags(PointTagCollection tags)
		{
			foreach (var t in tags)
			{
				this.UpdateTag(t);
			}
		}

		protected void UpdateTags(Dictionary<Guid,PointTag> tags)
		{
			foreach (var t in tags)
			{
				this.UpdateTag(t.Value);
			}
		}

		protected void UpdateProperty(PointProperty property)
		{
			this.PropertyDictionary.Remove(property.PointPropertyGuid);
			this.PropertyDictionary.Add(property.PointPropertyGuid, property);
			this.PointDictionary[property.PointGuid].Properties[property.PointPropertyGuid] = property;
		}

		protected void UpdateProperties(PointPropertyCollection properties)
		{
			foreach (var p in properties)
			{
				this.UpdateProperty(p);
			}
		}

		protected void UpdateProperties(Dictionary<Guid, PointProperty> properties)
		{
			foreach (var p in properties)
			{
				this.UpdateProperty(p.Value);
			}
		}


		protected void UpdatePoints(PointCollection points)
		{
			foreach (var point in points)
			{
				this.PointDictionary[point.PointGuid] = point;
				this.UpdateTags(point.Tags);
				this.UpdateProperties(point.Properties);

			}
		}

		internal static bool CompareRowVersion(byte[] rowVersion1, byte[] rowVersion2)
		{
				if (rowVersion1.Length != rowVersion2.Length)
				{
					return false;
				}
				if (rowVersion1.Length == 8)
				{
					if (rowVersion1[0] == rowVersion2[0] && rowVersion1[1] == rowVersion2[1] && rowVersion1[2] == rowVersion2[2]
						&& rowVersion1[3] == rowVersion2[3] && rowVersion1[4] == rowVersion2[4] && rowVersion1[5] == rowVersion2[5]
						&& rowVersion1[6] == rowVersion2[6] && rowVersion1[7] == rowVersion2[7])
					{
						return true;
					}

				}
				else if (rowVersion2.Length == 4)
				{
					if (rowVersion1[0] == rowVersion2[0] && rowVersion1[1] == rowVersion2[1] && rowVersion1[2] == rowVersion2[2]
						&& rowVersion1[3] == rowVersion2[3])
					{
						return true;
					}
				}
				return false;
		}

		internal static bool AreValuesSame(PointTag tag1, PointTag tag2)
		{
				if (tag1 == null || tag2 == null)
				{
					return false;
				}
				if (tag1.Value == null && tag2.Value == null)
				{
					return true;
				}
				if (tag1.Value == null || tag2.Value == null)
				{
					return false;
				}
				if(tag1.ValueType != tag2.ValueType)
				{
					return false;
				}
				if(tag1.Value != tag2.Value)
				{
					return false;
				}
				return true;
		}

		public List<PointTag> GetPointTags(List<Guid> pointTagGuids)
		{
			List<PointTag> pointTags = new List<PointTag>(pointTagGuids.Count);

			lock (this.LockObject)
			{
				foreach (var pointTagGuid in pointTagGuids)
				{
					PointTag pointTag = this.GetPointTag(pointTagGuid);

					if (pointTag == null)
					{
						pointTag = new PointTag()
						{
							PointTagGuid = pointTagGuid
						};
					}
					pointTags.Add(pointTag);
				}

				return pointTags;
			}
		}

		public PointTag GetPointTag(Guid pointTagGuid)
		{
			lock (this.LockObject)
			{
				PointTag pointTag;
				if (this.TagDictionary.TryGetValue(pointTagGuid, out pointTag))
				{
					return pointTag.Clone() as PointTag;
				}

				return null;
			}
		}

		public PointProperty GetPointProperty(Guid pointPropertyGuid)
		{
			lock (this.LockObject)
			{
				PointProperty pointProperty;
				if (this.PropertyDictionary.TryGetValue(pointPropertyGuid, out pointProperty))
				{
					return pointProperty;
				}

				return null;
			}
		}

		public Point GetPoint(Guid pointGuid)
		{
			lock (this.LockObject)
			{
				Point point;
				if (this.PointDictionary.TryGetValue(pointGuid, out point))
				{
					return point;
				}

				return null;
			}
		}

		public string GetPointCommandStatusKey(Guid pointGuid, Guid pointCommandStatusGuid, int value)
		{
			lock (this.LockObject)
			{
				Point point;
				if (this.PointDictionary.TryGetValue(pointGuid, out point))
				{
					PointTemplateDataContainer pointTemplateDataContainer;
					if(this.PointTemplateDataContainerDictionary.TryGetValue(point.PointTemplateGuid, out pointTemplateDataContainer))
					{
						foreach(var pointCommandStatusList in pointTemplateDataContainer.PointTemplatePointServiceData.PointCommandStatus.CommandStatusLists)
						{
							if(pointCommandStatusList.CommandStatusListGuid == pointCommandStatusGuid)
							{
								foreach(var commandStatusElement in pointCommandStatusList.CommandStatusList)
								{
									if(commandStatusElement.Value == value)
									{
										return commandStatusElement.Key;
									}
								}
							}
						}
					}
				}

				return string.Empty;
			}
		}

		public List<PointValue> GetPointValues(List<PointValueIdentifier> pointValueIdentifierList)
		{
			List<PointValue> pointValueList = new List<PointValue>(pointValueIdentifierList.Count);

			lock (this.LockObject)
			{
				foreach (PointValueIdentifier pointValueIdentifier in pointValueIdentifierList)
				{
					var pointValue = this.GetPointValue(pointValueIdentifier);
					if (pointValue == null)
					{
						pointValue = new PointValue()
						{
							PointGuid = pointValueIdentifier.IdentityGuid,
							PointValueIdentifier = pointValueIdentifier
						};

					}
					pointValueList.Add(pointValue);
				}
			}

			return pointValueList;
		}


		public List<PointValue> GetPointValueChanges(List<PointValueIdentifier> pointValueIdentifierList)
		{
			List<PointValue> pointValueList = new List<PointValue>(pointValueIdentifierList.Count);

			lock (this.LockObject)
			{
				foreach (PointValueIdentifier pointValueIdentifier in pointValueIdentifierList)
				{
					var pointValue = this.GetPointValueChanged(pointValueIdentifier);
					if (pointValue != null)
					{
						pointValueList.Add(pointValue);
					}
				}
			}

			return pointValueList;
		}


		public PointValue GetPointValue(PointValueIdentifier pointValueIdentifier)
		{

			lock (this.LockObject)
			{
				if (pointValueIdentifier.PointValueType == PointValueType.Tag)
				{
					PointTag pointTag;
					if (this.TagDictionary.TryGetValue(pointValueIdentifier.IdentityGuid, out pointTag))
					{
						var pointValueForPointTag = new PointValue(pointTag);
						if (pointValueIdentifier.IncludeAlarmLimits)
						{
							foreach (var alarm in pointTag.Alarms.Values)
							{
								foreach (var alarmTest in alarm.AlarmTests.Values)
								{
									PointTag limitTag;
									if (this.TagDictionary.TryGetValue(alarmTest.LimitTagGuid, out limitTag))
									{
										var alv = new AlarmLimitValue
										          {
											          IdentityGuid = limitTag.PointTagGuid,
											          Value = limitTag.Value,
											          AlarmPriorityGuid = alarmTest.AlarmPriorityGuid
										          };
										pointValueForPointTag.AlarmLimitList.Add(alv);
									}
								}
							}
						}
						return pointValueForPointTag;
					}
				}

				else if(pointValueIdentifier.PointValueType == PointValueType.Setting)
				{
					PointProperty pointProperty;
					if (this.PropertyDictionary.TryGetValue(pointValueIdentifier.IdentityGuid, out pointProperty))
					{
						Point point;
						if (this.PointDictionary.TryGetValue(pointProperty.PointGuid, out point))
						{
							return new PointValue(pointValueIdentifier, pointProperty, point);
						}
					}
				}

				else if(pointValueIdentifier.PointValueType == PointValueType.Point)
				{
					Point point;
					if (this.PointDictionary.TryGetValue(pointValueIdentifier.IdentityGuid, out point))
					{
						return new PointValue(pointValueIdentifier, point);
					}
				}

				return null;
			}
		}

		public PointValue GetPointValueChanged(PointValueIdentifier pointValueIdentifier)
		{
			PointValue pointValue = null;

			lock (this.LockObject)
			{
				if (pointValueIdentifier.PointValueType == PointValueType.Tag)
				{

					PointTag pointTag;
					if (this.TagDictionary.TryGetValue(pointValueIdentifier.IdentityGuid, out pointTag))
					{
						if (pointTag.ServerTimeStamp.UtcTicks != pointValueIdentifier.UtcTicks
						|| pointValueIdentifier.IncludeAlarmLimits)
						{
							pointValue = new PointValue(pointTag);
							if (pointValueIdentifier.IncludeAlarmLimits)
							{
								foreach (var alarm in pointTag.Alarms.Values)
								{
									foreach (var alarmTest in alarm.AlarmTests.Values)
									{
										PointTag limitTag;
										if (this.TagDictionary.TryGetValue(alarmTest.LimitTagGuid, out limitTag))
										{
											var alv = new AlarmLimitValue
											{
												IdentityGuid = limitTag.PointTagGuid,
												Value = limitTag.Value,
												AlarmPriorityGuid = alarmTest.AlarmPriorityGuid
											};

											pointValue.AlarmLimitList.Add(alv);
										}
									}
								}
							}
						}
					}
				}

				else if (pointValueIdentifier.PointValueType == PointValueType.Setting)
				{

					PointProperty pointProperty;
					if (this.PropertyDictionary.TryGetValue(pointValueIdentifier.IdentityGuid, out pointProperty))
					{
						Point point;
						if (this.PointDictionary.TryGetValue(pointProperty.PointGuid, out point))
						{
							if (pointProperty.UpdatedDate.UtcTicks != pointValueIdentifier.UtcTicks)
							{
								pointValue = new PointValue(pointValueIdentifier, pointProperty, point);
							}
						}
					}
				}

				else if (pointValueIdentifier.PointValueType == PointValueType.Point)
				{
					Point point;
					if (this.PointDictionary.TryGetValue(pointValueIdentifier.IdentityGuid, out point))
					{
						if (point.UpdatedDate.UtcTicks != pointValueIdentifier.UtcTicks)
						{
							pointValue = new PointValue(pointValueIdentifier, point);
						}
					}
				}
			}

			return pointValue;
		}


		public void SetPointTag(PointTag pointTag)
		{
			lock (this.LockObject)
			{
				PointTag currentPointTag;
				if (this.TagDictionary.TryGetValue(pointTag.IdentityGuid, out currentPointTag))
				{
					currentPointTag.EngineeringUnitsType = pointTag.EngineeringUnitsType;
					currentPointTag.Units = pointTag.Units;
					currentPointTag.ServerUnits = pointTag.ServerUnits;
					currentPointTag.DecimalPlaces = pointTag.DecimalPlaces;
					currentPointTag.Maximum = pointTag.Maximum;
					currentPointTag.Minimum = pointTag.Minimum;
					if (pointTag.Value is ValueType)
					{
						currentPointTag.Value = pointTag.Value;
					}
					else
					{
						currentPointTag.ValueXml = pointTag.ValueXml;
					}
					currentPointTag.Status = pointTag.Status;
					currentPointTag.ServerTimeStamp = pointTag.ServerTimeStamp;
					currentPointTag.SourceTimeStamp = pointTag.SourceTimeStamp;
					currentPointTag.WrittenToEnterprise = false;
				}
			}
		}

		public void SetPointProperty(PointProperty pointProperty)
		{
			lock (this.LockObject)
			{
				PointProperty currentPointProperty;
				if (this.PropertyDictionary.TryGetValue(pointProperty.IdentityGuid, out currentPointProperty))
				{
					currentPointProperty.Value = pointProperty.Value;
				}
				//else
				//{
				//	throw new Exception("SetPointProperty : Property - " + pointProperty.ID + " not found.");
				//}
			}
		}


		public bool SetPointTagAlarmAndAlarmStatusIfChanged(PointTag pointTag)
		{
			lock (this.LockObject)
			{
				PointTag currentPointTag;
				if (this.TagDictionary.TryGetValue(pointTag.IdentityGuid, out currentPointTag))
				{
					return this.PointTagValueChanger.SetAlarmAndAlarmStatusIfChanged(currentPointTag, pointTag);
				}

				//throw new Exception( "SetPointTag : Tag - " + pointTag.ID + " not found." );
			}
			return false;
		}



		public bool SetPointTagAlarmIfChanged(PointTag pointTag)
		{
			lock (this.LockObject)
			{
				PointTag currentPointTag;
				if (this.TagDictionary.TryGetValue(pointTag.IdentityGuid, out currentPointTag))
				{
					return this.PointTagValueChanger.SetAlarmIfChanged(currentPointTag, pointTag);
				}

				//throw new Exception( "SetPointTag : Tag - " + pointTag.ID + " not found." );
			}
			return false;
		}


		public void SetPointTagValueIfChanged(PointTag pointTag, bool preserveOverride, ref bool valueChanged, ref bool statusChanged, ref bool alarmChanged)
		{
			lock ( this.LockObject )
			{
				PointTag currentPointTag;
				if ( this.TagDictionary.TryGetValue( pointTag.IdentityGuid, out currentPointTag )
				&& pointTag.InputOutputType == currentPointTag.InputOutputType
				&& (pointTag.InputOutputType != PointTemplateTag.PointTagInputOutputType.Manual || pointTag.Input || pointTag.ServerTimeStamp >= currentPointTag.ServerTimeStamp)
				&& (currentPointTag.OpcStatusCodeBits != StatusCodes.GoodLocalOverride
				|| pointTag.Value == null
				|| !preserveOverride ))
				{
					if(this.PointTagValueChanger.SetAlarmAndAlarmStatusIfChanged(currentPointTag, pointTag))
					{
						alarmChanged = true;
					}

					this.PointTagValueChanger.SetValuesIfChanged(currentPointTag, pointTag, ref valueChanged, ref statusChanged);

					// Update the pointValue to reflect what was set for proper archive in PointService SetPointValueData & SetPointTagData
					if (!preserveOverride
					&& (valueChanged || statusChanged))
					{ 
						if (pointTag.Value is ValueType)
						{
							pointTag.Value = currentPointTag.Value;
						}
						else
						{
							pointTag.ValueXml = currentPointTag.ValueXml;
						}
						pointTag.Status = currentPointTag.Status;
						pointTag.ServerTimeStamp = currentPointTag.ServerTimeStamp;
						pointTag.SourceTimeStamp = currentPointTag.SourceTimeStamp;
					}
				}
			}
		}

		public bool SetShelvedIfChanged(PointTag pointTag)
		{
			lock (this.LockObject)
			{
				PointTag currentPointTag;
				if (this.TagDictionary.TryGetValue(pointTag.IdentityGuid, out currentPointTag))
				{
					return PointTagValueChanger.SetShelvedIfChanged(currentPointTag, pointTag);
				}

				//throw new Exception( "SetPointTag : Tag - " + pointTag.ID + " not found." );
			}
			return false;
		}

		public bool SetAcknowledgedAndSilencedIfChanged(PointTag pointTag, List<PointTagAlarmStatus> acknoledgedAlarms, List<AandEDataElement> aandEDataElements, string comment = "")
		{
			lock (this.LockObject)
			{
				Point point;
				PointTag currentPointTag;
				if (this.PointDictionary.TryGetValue(pointTag.PointGuid, out point)
				&&	this.TagDictionary.TryGetValue(pointTag.IdentityGuid, out currentPointTag))
				{
					return PointTagValueChanger.SetAcknowledgedAndSilencedIfChanged(point, currentPointTag, pointTag, acknoledgedAlarms, aandEDataElements, comment);
				}
			}

			return false;
		}

		public Dictionary<Guid, Point> GetPointDictionary(bool setWrittenToEnterprise)
		{
			var ret = new Dictionary<Guid, Point>();
			lock (this.LockObject)
			{
				foreach (var point in this.PointDictionary.Values)
				{
					var pointClone = point.Clone();
					if (setWrittenToEnterprise)
					{
						foreach (var tag in point.Tags.Values)
						{
							tag.WrittenToEnterprise = true;

							foreach (var alarm in tag.Alarms.Values)
							{
								foreach (var pointTagAlarmStatus in alarm.AlarmStatus.Values)
								{
									pointTagAlarmStatus.WrittenToEnterprise = true;
								}
							}
						}

						foreach (var property in point.Properties.Values)
						{
							property.WrittenToEnterprise = true;
						}
					}

					ret.Add(pointClone.PointGuid,pointClone);
				}
			}
			return ret;
		}

		public Dictionary<Guid, PointTag> GetTagDictionary()
		{
				var ret = new Dictionary<Guid, PointTag>();
				lock (this.LockObject)
				{
					foreach (var tag in this.TagDictionary.Values)
					{
						var tagClone = (PointTag)tag.Clone();
						ret.Add(tagClone.PointTagGuid, tagClone);
					}
				}
				return ret;
		}

		public Dictionary<Guid, PointTag> GetMonitoredTagDictionary()
		{
			var ret = new Dictionary<Guid, PointTag>();
			lock (this.LockObject)
			{
				foreach (var tag in this.TagDictionary.Values)
				{
					if (IsPointTagToBeMonitored(tag))
					{
						var tagClone = (PointTag)tag.Clone();
						ret.Add(tagClone.PointTagGuid, tagClone);
					}
				}
			}
			return ret;
		}

		public Dictionary<Guid, PointTag> GetOutputTagDictionary()
		{
			var ret = new Dictionary<Guid, PointTag>();
			lock (this.LockObject)
			{
				foreach (var tag in this.TagDictionary.Values)
				{
					if (IsPointTagToBeOutput(tag, true, PointTemplateTag.PointTagInputOutputType.Calculated))
					{
						var tagClone = (PointTag)tag.Clone();
						ret.Add(tagClone.PointTagGuid, tagClone);
					}
				}
			}
			return ret;
		}




		public void SetWrittenToEnterprise(bool tag, Guid pointTagGuid, bool state)
		{
			lock (this.LockObject)
			{
				if (tag)
				{
					PointTag dataStoreTag;
					if (this.TagDictionary.TryGetValue(pointTagGuid, out dataStoreTag))
					{
						dataStoreTag.WrittenToEnterprise = state;

						foreach (var alarm in dataStoreTag.Alarms.Values)
						{
							foreach (var pointTagAlarmStatus in alarm.AlarmStatus.Values)
							{
								pointTagAlarmStatus.WrittenToEnterprise = state;
							}
						}
					}
				}
				else
				{
					PointProperty dataStoreProperty;
					if (this.PropertyDictionary.TryGetValue(pointTagGuid, out dataStoreProperty))
					{
						dataStoreProperty.WrittenToEnterprise = state;
					}
				}
			}
		}

		public static void RangeCheck(PointTag tag)
		{
				var statusCode = new StatusCode((uint)tag.Status);
				if (StatusCode.IsGood(statusCode))
				{
					double? value = null;
					if (tag.Value != null)
					{
						if (tag.Value is Int16)
								value = (Int16)tag.Value;
						else if (tag.Value is Int32)
								value = (Int32)tag.Value;
						else if (tag.Value is Int64)
								value = (Int64)tag.Value;
						else if (tag.Value is char)
								value = (char)tag.Value;
						else if (tag.Value is byte)
								value = (byte)tag.Value;
						else if (tag.Value is UInt16)
								value = (UInt16)tag.Value;
						else if (tag.Value is UInt32)
								value = (UInt32)tag.Value;
						else if (tag.Value is UInt64)
								value = (UInt64)tag.Value;
						else if (tag.Value is float)
								value = (float)tag.Value;
						else if (tag.Value is double)
								value = (double)tag.Value;
					}

					if (value.HasValue)
					{
						if (value.Value > tag.Maximum)
						{
								statusCode.SetLimitBits(LimitBits.High);
						}
						else if (value.Value < tag.Minimum)
						{
								statusCode.SetLimitBits(LimitBits.Low);
						}
						else
						{
								statusCode.SetLimitBits(LimitBits.None);
						}
					}
				}

				tag.Status = statusCode.Code;
		}

		/// <summary>
		/// Determines if a PointTag needs to be monitored
		/// </summary>
		/// <param name="pointTag"></param>
		/// <returns></returns>
		public static bool IsPointTagToBeMonitored(PointTag pointTag)
		{
			if (pointTag == null)
			{
				return false;
			}


			return (((pointTag.Input
						&& pointTag.InputOutputType == PointTemplateTag.PointTagInputOutputType.OpcUa)
						|| (!pointTag.Input
						&& pointTag.OpcUaIsReadable))
						&& !string.IsNullOrEmpty(pointTag.OpcUaNodeId)
						&& pointTag.OpcUaServerGuid != null
						&& pointTag.OpcUaServerGuid != Guid.Empty) ? true : false;
		}

		/// <summary>
		/// Determines if a PointTag needs to be monitored
		/// </summary>
		/// <param name="pointTag"></param>
		/// <returns></returns>
		public static bool IsPointTagToBeOutput(PointTag pointTag, bool testDataSource, PointTemplateTag.PointTagInputOutputType dataSource = PointTemplateTag.PointTagInputOutputType.UnAssigned)
		{
			if (pointTag == null)
			{
				return false;
			}

			return (!pointTag.Input
						&& (!testDataSource
						|| pointTag.InputOutputType == dataSource)
						&& (!string.IsNullOrEmpty(pointTag.OpcUaNodeId)
						&& pointTag.OpcUaServerGuid != null
						&& pointTag.OpcUaServerGuid != Guid.Empty)) ? true : false;
		}

		public bool IsNumericDatatype(PointTag tag)
		{
			bool returnValue = false;
			if (tag.Value != null)
			{
				if (tag.Value is Int16)
					returnValue = true;
				else if (tag.Value is Int32)
					returnValue = true;
				else if (tag.Value is Int64)
					returnValue = true;
				else if (tag.Value is char)
					returnValue = true;
				else if (tag.Value is byte)
					returnValue = true;
				else if (tag.Value is UInt16)
					returnValue = true;
				else if (tag.Value is UInt32)
					returnValue = true;
				else if (tag.Value is UInt64)
					returnValue = true;
				else if (tag.Value is float)
					returnValue = true;
				else if (tag.Value is double)
					returnValue = true;
			}
			return returnValue;
		}
		public void ExternalUpdateTags(Dictionary<Guid, PointTag> tags, string userId = ThreadSharedData.UserId)
		{
			//This method is used by the OpcUaClientProcessor and only changes are reported
			var pointProcessingDictionary = new Dictionary<Guid, PointTag>();
			lock (this.LockObject)
			{
				foreach (var tag in tags.Values)
				{
					PointTag dataStoreTag;
					if (this.TagDictionary.TryGetValue(tag.PointTagGuid, out dataStoreTag)
					&& (dataStoreTag.OpcStatusCodeBits != StatusCodes.GoodLocalOverride
					|| !dataStoreTag.Input))
					{
						RangeCheck(tag);

						if (dataStoreTag.InputOutputType == tag.InputOutputType
						&& (dataStoreTag.InputOutputType != PointTemplateTag.PointTagInputOutputType.OpcUa
						|| (dataStoreTag.OpcUaServerGuid == tag.OpcUaServerGuid
						&& dataStoreTag.OpcUaNodeId == tag.OpcUaNodeId))
						&& ((dataStoreTag.Value == null && tag.Value != null)
						|| (dataStoreTag.Value != null && !dataStoreTag.Value.Equals(tag.Value))
						|| dataStoreTag.Status != tag.Status))
						{
							// check if it is within the dead band and the holdoff for this tag bds
							// if the status changes then force an update without regard to deadband
							if (tag.Value != null &&
								tag.OpcStatusCodeBits != StatusCodes.UncertainLastUsableValue &&
								tag.Value is ValueType &&
								IsNumericDatatype(tag) &&
								tag.Deadband > 0.0)
							{
								if (dataStoreTag.Value != null && tag.Value != null)// && !dataStoreTag.Value.Equals(tag.Value))
								{
									double dv1 = Math.Abs(Convert.ToDouble(dataStoreTag.Value.ToString()));
									double dv2 = Math.Abs(Convert.ToDouble(tag.Value.ToString()));

									if (Math.Abs(dv1 - dv2) < tag.Deadband &&
										dataStoreTag.Status == tag.Status)
									{
										// add to the dictionary to track changes for the holdoff timeout
										// check if the tag is already in the dictionary
										if (TagHoldoffDeadbandDictionary.ContainsKey(tag.PointTagGuid))
										{
											TagHoldoffDeadbandDictionary.Remove(tag.PointTagGuid);
										}
										//we do not want to change the original tag so we will create a copy first
										if (tag.Holdoff > 0)
										{
											PointTag localPointTag = (PointTag)tag.Clone();

											TagHoldoffDeadbandDictionary.Add(tag.PointTagGuid, localPointTag);
										}
										continue;
									}
									else
									{
										// we exceeded the deadband so remove from the holfoff dictionary if present
										if (TagHoldoffDeadbandDictionary.ContainsKey(tag.PointTagGuid))
										{
											TagHoldoffDeadbandDictionary.Remove(tag.PointTagGuid);
										}
									}
								}
							}

							if (tag.Value is ValueType)
							{
								dataStoreTag.Value = tag.Value;
							}
							else
							{
								dataStoreTag.ValueXml = tag.ValueXml;
							}

							var statusChanged = false;

							dataStoreTag.ServerTimeStamp = tag.ServerTimeStamp;
							dataStoreTag.SourceTimeStamp = tag.SourceTimeStamp;
							if (dataStoreTag.Status != tag.Status)
							{
								statusChanged = true;
							}

							dataStoreTag.Status = tag.Status;
							dataStoreTag.UpdatedBy = userId;
							dataStoreTag.UpdatedDate = DateTimeOffset.UtcNow;
							dataStoreTag.WrittenToEnterprise = false;

							if ((dataStoreTag.Input
							|| dataStoreTag.InputOutputType == PointTemplateTag.PointTagInputOutputType.Manual)
							&& !pointProcessingDictionary.ContainsKey(tag.PointGuid))
							{
								pointProcessingDictionary.Add(tag.PointGuid, tag);
							}

							if (dataStoreTag.Archived)
							{
								this.ArchiveRecordQueuer.CreateAndQueueArchiveRecord(new PointValue(dataStoreTag), false, false, statusChanged);
							}

							AlarmAndEventArchiveThread.Instance().LogEventsToArchive(new PointValue(dataStoreTag));
						}
						else
						{
							dataStoreTag.ServerTimeStamp = tag.ServerTimeStamp;
							dataStoreTag.SourceTimeStamp = tag.SourceTimeStamp;
						}
					}
				}
			}

			foreach (var pointGuid in pointProcessingDictionary.Keys)
			{
				this.PointExecutionQueuer.QueuePointForProcessing(pointGuid);
			}

			if(this.ArchiveRecordQueuer.Count > 0)
				this.ArchiveProcessorSignaler.SignalExpedite();
		}

		public void ProcessHoldoffDeadbandDictionary()	// bds
		{
			lock (this.LockObject)
			{
				if (TagHoldoffDeadbandDictionary.Count == 0)
					return;

				var pointProcessingDictionary = new Dictionary<Guid, PointTag>();
				var pointDeleteDictionary = new Dictionary<Guid,Guid>();
				foreach (var pointData in TagHoldoffDeadbandDictionary)
				{
					pointData.Value.Holdoff -= 1;
					if(pointData.Value.Holdoff <= 0)
					{
						PointTag tag = pointData.Value;
						PointTag dataStoreTag;
						string userId = ThreadSharedData.UserId;

						pointDeleteDictionary.Add(tag.PointTagGuid, tag.PointTagGuid);

						if (this.TagDictionary.TryGetValue(tag.PointTagGuid, out dataStoreTag)
						&& (dataStoreTag.OpcStatusCodeBits != StatusCodes.GoodLocalOverride
						|| !dataStoreTag.Input))
						{
							if (tag.Value is ValueType)
							{
								dataStoreTag.Value = tag.Value;
							}
							else
							{
								dataStoreTag.ValueXml = tag.ValueXml;
							}

							dataStoreTag.ServerTimeStamp = tag.ServerTimeStamp;
							dataStoreTag.SourceTimeStamp = tag.SourceTimeStamp;
							dataStoreTag.Status = tag.Status;
							dataStoreTag.UpdatedBy = userId;
							dataStoreTag.UpdatedDate = DateTimeOffset.UtcNow;

							if ((dataStoreTag.Input
							|| dataStoreTag.InputOutputType == PointTemplateTag.PointTagInputOutputType.Manual)
							&& !pointProcessingDictionary.ContainsKey(tag.PointGuid))
							{
								pointProcessingDictionary.Add(tag.PointGuid, tag);
							}

                            if (dataStoreTag.Archived)
                            {
                                this.ArchiveRecordQueuer.CreateAndQueueArchiveRecord(new PointValue(dataStoreTag), false, false, false);
                            }
                            AlarmAndEventArchiveThread.Instance().LogEventsToArchive(new PointValue(dataStoreTag));
                        }
					}
				}
				foreach (var pointGuid in pointProcessingDictionary.Keys)
				{
					this.PointExecutionQueuer.QueuePointForProcessing(pointGuid);
				}
				foreach (var tagtoDelete in pointDeleteDictionary)
				{
					TagHoldoffDeadbandDictionary.Remove(tagtoDelete.Value);
				}
				pointDeleteDictionary.Clear();

				if (this.ArchiveRecordQueuer.Count > 0)
					this.ArchiveProcessorSignaler.SignalExpedite();
			}
		}

		public void UpdateHoldoffDeadbandDictionarywithDeletedTags(Dictionary<Guid, PointTag> deletedMonitoredTags)   // bds
		{
			if (TagHoldoffDeadbandDictionary.Count == 0)
				return;

			foreach (var PTag in deletedMonitoredTags)
			{
				if(TagHoldoffDeadbandDictionary.ContainsKey(PTag.Value.PointTagGuid))
					TagHoldoffDeadbandDictionary.Remove(PTag.Value.PointTagGuid);
			}
		}

		public void UpdateValueStatusAndTimeStampForValues(Dictionary<Guid, WriteValue> valueListDictionary)
		{
			lock (this.LockObject)
			{
				foreach (var pointTagGuid in valueListDictionary.Keys)
				{
					var writeValue = valueListDictionary[pointTagGuid];

					PointTag dataStoreTag;

					if (this.TagDictionary.TryGetValue(pointTagGuid, out dataStoreTag)
					&& dataStoreTag.OpcStatusCodeBits != StatusCodes.GoodLocalOverride
					&& (!dataStoreTag.OpcUaIsReadable
					|| StatusCode.IsBad(writeValue.Value.StatusCode)))
					{
						object value = null;

						try
						{
							switch (dataStoreTag.ValueTypeString)
							{
								case "FMBusinessObjects.DataObjects.PointCommandStatusListReference":
									if (dataStoreTag.Value is PointCommandStatusListReference)
									{
										int? intValue;
										string keyValue;
										try
										{
											intValue = new int?(Convert.ToInt32(writeValue.Value.Value));
											keyValue = ThreadSharedData.Instance().GetPointCommandStatusKey(dataStoreTag.PointGuid, (dataStoreTag.Value as PointCommandStatusListReference).PointCommandStatusListGuid, intValue.Value);
										}
										catch (Exception)
										{
											intValue = null;
											keyValue = null;
										}
										value = new PointCommandStatusListReference()
										{
											PointCommandStatusListGuid = (dataStoreTag.Value as PointCommandStatusListReference).PointCommandStatusListGuid,
											CurrentValue = intValue,
											CurrentKey = keyValue
										};
									}
									break;

								default:
									StatusCode statusCode = writeValue.Value.StatusCode;
									value = writeValue.Value.Value;

									bool isDeviceAlarmMapReference = dataStoreTag.Value is DeviceAlarmMapReference;

									Guid deviceAlarmMapGuid = isDeviceAlarmMapReference ?
										(dataStoreTag.Value as DeviceAlarmMapReference).DeviceAlarmMapGuid :
										Guid.Empty;

									FMPointCommon.PointManager.ValidatePointTagValueByItsType(dataStoreTag.ValueTypeString,
										ref value, ref statusCode, isDeviceAlarmMapReference, deviceAlarmMapGuid);

									writeValue.Value.StatusCode = statusCode;
									break;
							}
						}
						catch (Exception)
						{
							value = null;
						}

						if (((dataStoreTag.Value == null && value != null)
						|| (dataStoreTag.Value != null && !dataStoreTag.Value.Equals(value))
						|| dataStoreTag.Status != writeValue.Value.StatusCode.Code))
						{
							dataStoreTag.Value = value;
							dataStoreTag.ServerTimeStamp = DateTimeOffset.UtcNow;
							dataStoreTag.SourceTimeStamp = DateTimeOffset.UtcNow;
							dataStoreTag.Status = writeValue.Value.StatusCode.Code;
							RangeCheck(dataStoreTag);
							if (dataStoreTag.Archived)
							{
								this.ArchiveRecordQueuer.CreateAndQueueArchiveRecord(new PointValue(dataStoreTag), false, false, false);
							}
							AlarmAndEventArchiveThread.Instance().LogEventsToArchive(new PointValue(dataStoreTag));
						}
					}

				}
			}

			if (this.ArchiveRecordQueuer.Count > 0)
				this.ArchiveProcessorSignaler.SignalExpedite();
		}


		public void UpdateShelvedAlarmInfo()
		{
			lock (this.LockObject)
			{
				foreach(var tag in this.TagDictionary.Values)
				{
					if(tag.UpdateShelvedAlarmInfo()
					&& tag.Archived)
					{ 
						this.ArchiveRecordQueuer.CreateAndQueueArchiveRecord(new PointValue(tag), false, false, true);
					}
				}
			}
		}

		public bool PointCommandStatusListReferenceChanged(PointTag tag, PointTag oldTag)
		{
			if (tag.Value == null
			|| tag.Value.GetType() != typeof(PointCommandStatusListReference)
			|| (oldTag.Value != null
			&& oldTag.Value.GetType() == typeof(PointCommandStatusListReference)
			&& (tag.Value as PointCommandStatusListReference).PointCommandStatusListGuid == (oldTag.Value as PointCommandStatusListReference).PointCommandStatusListGuid))
			{
				return false;
			}

			return true;		
		}

		public void MergePoints(PointCollection pointsToAddUpdate, List<Guid> pointsToDelete)
		{
			if((pointsToDelete == null
			&& pointsToAddUpdate == null)
			|| (pointsToDelete != null
			&& pointsToDelete.Count == 0
			&& pointsToAddUpdate != null
			&& pointsToAddUpdate.Count == 0))
			{
				return;
			}


			try
			{
				var tagDictionary = new Dictionary<Guid, PointTag>();
				var propertyDictionary = new Dictionary<Guid, PointProperty>();
				var addedPoints = new List<Guid>();

				lock (this.LockObject)
				{
					if (pointsToDelete != null)
					{
						foreach (var p in pointsToDelete)
						{
							this.PointDictionary.Remove(p);
							this.PointLogicDictionary.Remove(p);
						}
					}
					if (pointsToAddUpdate != null)
					{
						foreach (var point in pointsToAddUpdate)
						{
							Point oldPoint;
							if (this.PointDictionary.TryGetValue(point.PointGuid, out oldPoint))
							{

								// Remove Point Logic if the the PointTemplateRowVersion has changed. 
								PointTemplateDataContainer pointTemplateDataContainer = null;
								if (this.PointLogicDictionary.ContainsKey(point.PointGuid)
								&& (point.PointTemplateVersion != oldPoint.PointTemplateVersion
								|| !this.PointTemplateDataContainerDictionary.TryGetValue(point.PointTemplateGuid, out pointTemplateDataContainer)
								|| point.PointTemplateVersion != pointTemplateDataContainer.PointTemplatePointServiceData.Version))
								{
									this.PointLogicDictionary.Remove(point.PointGuid);
								}

								// This is the handling of a point change, merge the current values for all cases except manual inputs
								foreach (var tag in point.Tags.Values)
								{
									PointTag oldTag;
									if (oldPoint.Tags.TryGetValue(tag.PointTagGuid, out oldTag))
									{
										if (tag.InputOutputType == oldTag.InputOutputType
										&& tag.ValueType == oldTag.ValueType
										&& !PointCommandStatusListReferenceChanged(tag, oldTag)
										&& (tag.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated
										|| tag.InputOutputType == PointTemplateTag.PointTagInputOutputType.System
										|| (tag.InputOutputType == PointTemplateTag.PointTagInputOutputType.OpcUa
										&& tag.OpcUaServerGuid == oldTag.OpcUaServerGuid
										&& !string.IsNullOrEmpty(tag.OpcUaNodeId)
										&& tag.OpcUaNodeId == oldTag.OpcUaNodeId
										&& !string.IsNullOrEmpty(tag.OpcUaBrowsePath))
										|| (tag.InputOutputType == PointTemplateTag.PointTagInputOutputType.Manual && !tag.Input)))
										{

											if (oldTag.Value is ValueType)
											{
												tag.Value = oldTag.Value;
											}
											else
											{
												tag.ValueXml = oldTag.ValueXml;
											}

											tag.Status = oldTag.Status;
											tag.ServerTimeStamp = oldTag.ServerTimeStamp;
											tag.SourceTimeStamp = oldTag.SourceTimeStamp;
										}

										// Archive Tag Changes
										else if ((tag.Value == null && oldTag.Value != null)
										|| (tag.Value != null && oldTag.Value == null)
										|| (tag.Value != null && !tag.Value.Equals(oldTag.Value)))
										{
											RangeCheck(tag);

											// Do not archive calculated tags, they will be archived when calculated
											if (tag.Archived)
											{
												this.ArchiveRecordQueuer.CreateAndQueueArchiveRecord(new PointValue(tag), false, false, tag.Alarms.Any());
											}
											AlarmAndEventArchiveThread.Instance().LogEventsToArchive(new PointValue(tag));
										}
									}
								}

								List<PointValue> pointValues;
								List<PointValue> oldPointValues;
								int index;


								foreach (var property in point.Properties.Values)
								{
									PointProperty oldProperty;
									if (oldPoint.Properties.TryGetValue(property.PointPropertyGuid, out oldProperty))
									{
										pointValues = property.GetExposedSettings(point);
										oldPointValues = oldProperty.GetExposedSettings(oldPoint);
										index = 0;
										foreach (var pointValue in pointValues)
										{
											var oldPointValue = oldPointValues[index++];
											if ((pointValue.Value == null && oldPointValue.Value != null)
											|| (pointValue.Value != null && oldPointValue.Value == null)
											|| (pointValue.Value != null && !pointValue.Value.Equals(oldPointValue.Value)))
											{
												this.ArchiveRecordQueuer.CreateAndQueueArchiveRecord(pointValue, false, false, false);
											}
										}
									}
								}

								pointValues = point.GetExposedSettings();
								oldPointValues = oldPoint.GetExposedSettings();

								index = 0;
								foreach (var pointValue in pointValues)
								{
									var oldPointValue = oldPointValues[index++];
									if ((pointValue.Value == null && oldPointValue.Value != null)
									|| (pointValue.Value != null && oldPointValue.Value == null)
									|| (pointValue.Value != null && !pointValue.Value.Equals(oldPointValue.Value)))
									{
										this.ArchiveRecordQueuer.CreateAndQueueArchiveRecord(pointValue, false, false, false);
									}
								}


								this.PointDictionary[point.PointGuid] = point;
								addedPoints.Add(point.PointGuid);
							}
							else
							{
								this.PointDictionary.Add(point.PointGuid, point);
								addedPoints.Add(point.PointGuid);
								foreach (var tag in point.Tags.Values)
								{
									RangeCheck(tag);
								}
								this.ArchiveRecordQueuer.CreateAndQueuePointArchive(point, false);
							}
						}
					}

					foreach (var point in this.PointDictionary.Values)
					{
						foreach (var tag in point.Tags.Values)
						{
							tagDictionary.Add(tag.PointTagGuid, tag);
						}

						foreach (var property in point.Properties.Values)
						{
							propertyDictionary.Add(property.PointPropertyGuid, property);
						}
					}

					this.TagDictionary = tagDictionary;
					this.PropertyDictionary = propertyDictionary;

					// Have to be queued after they are in the PointDictionary since they are 
					// not official until then.
					addedPoints.ForEach(x => PointExecutionQueuer.QueuePointForProcessing(x));

					if (ThreadSharedData.Instance().UseOpcUaClientPolling)
					{
						OpcUaClientProcessor2.Instance().SignalTagChanges();
					}
					else
					{
						OpcUaClientProcessor.Instance().SignalTagChanges();
					}

					MovementProcessor.Instance().SignalPointChanges();
					LeakDetectionProcessor.Instance().SignalPointChanges();
				}
			}
			catch (Exception except)
			{
				Logger.LogError("ThreadSharedData exception Merge Points: " + except.Message);
			}
		}

		public void ClearPoints()
		{
				var pointDictionary = new Dictionary<Guid, Point>();
				var tagDictionary = new Dictionary<Guid, PointTag>();
				lock (this.LockObject)
				{
					this.PointDictionary = pointDictionary;
					this.TagDictionary = tagDictionary;
				}
		}

		/// <summary>
		/// In general this should not be called outside the actual point execution
		/// process.  Queue a point for execution using the PointExecutionQueuer class.  But
		/// calling this would not be too bad.  If a point was in the execution queue but
		/// this flag is cleared, it will just be skipped by the point execution process.
		/// </summary>
		public void ClearPointNeedsCalculation( Guid pointGuid )
		{
			this.SetNeedsCalculation( pointGuid, needsCalculation: false );
		}

		/// <summary>
		/// In general this should not be called outside the actual point execution
		/// process.  Queue a point for execution using the PointExecutionQueuer class.
		/// </summary>
		public bool SetPointNeedsCalculation( Guid pointGuid )
		{
			return this.SetNeedsCalculation(pointGuid, needsCalculation: true);
		}

		/// <summary>
		/// In general this should not be called outside the actual point execution
		/// process.  Queue a point for execution using the PointExecutionQueuer class.
		/// </summary>
		private bool SetNeedsCalculation( Guid pointGuid, bool needsCalculation )
		{
			lock ( this.LockObject )
			{
				Point point;
				if ( this.PointDictionary.TryGetValue( pointGuid, out point ) )
				{
					if (point.NeedsCalculation != needsCalculation)
					{
							point.NeedsCalculation = needsCalculation;
							return needsCalculation;
					}
				}

				return false;
			}
		}

		/// <summary>
		/// This method will return a clone of the points that match
		/// the provided list of point guids.
		/// </summary>
		public IEnumerable<Point> GetPoints( IEnumerable<Guid> pointGuids )
		{
			var points = new List<Point>(42);

			lock (this.LockObject)
			{
				foreach (var pointGuid in pointGuids)
				{
					Point point;
					if (this.PointDictionary.TryGetValue(pointGuid, out point))
					{
						points.Add(point.Clone());
					}

				}
			}

			return points;
		}

		public Dictionary<Guid, MovementContainer> GetMovementContainers()
		{
			Dictionary<Guid, MovementContainer> movementContainerDictionary = new Dictionary<Guid, MovementContainer>();
	
			lock (this.LockObject)
			{
				var movementPointList = this.PointDictionary.Values.Where(u => u.PointTemplateGuid == Guids.MovementTemplateGuid).ToList();
				foreach(var movementPoint in movementPointList)
				{
					movementContainerDictionary.Add(movementPoint.IdentityGuid, new MovementContainer(movementPoint.Clone()));
				}
			}

			return movementContainerDictionary;
		}

		public Dictionary<Guid, Point> GetRealTimeLeakDetectionPoints()
		{
			Dictionary<Guid, Point> realTimeLeakDetectionPointsDictionary = new Dictionary<Guid, Point>();

			lock (this.LockObject)
			{
					foreach (var point in this.PointDictionary.Values)
					{
						PointProperty leakDetectionSettingsProperty = point.Properties.Values.SingleOrDefault(u => u.ValueTypeString == LeakDetectionSettings.LeakDetectionSettingsIdentifier);
						if (leakDetectionSettingsProperty != null)
						{
							LeakDetectionSettings settings = (LeakDetectionSettings)leakDetectionSettingsProperty.Value;
							if (settings.AnalysisType.Equals(LeakAnalysisType.RealTime))
							{
								realTimeLeakDetectionPointsDictionary.Add(point.IdentityGuid, point.Clone());
							}
						}
					}
			}
			return realTimeLeakDetectionPointsDictionary;
		}

		public void ApplyTagChangesToCopy(Point point)
		{
			lock (this.LockObject)
			{
				var masterPoint = this.GetPoint(point.PointGuid);

				if (masterPoint == null)
				{
					return;
				}

				foreach (var tagToUpdate in point.Tags.Values)
				{
					PointTag tagDataSource = null;
					if (masterPoint.Tags.TryGetValue(tagToUpdate.PointTagGuid, out tagDataSource))
					{
						bool valueChanged = false;
						bool statusChanged = false;
						this.PointTagValueChanger.SetAlarmAndAlarmStatusIfChanged(tagToUpdate, tagDataSource);
						this.PointTagValueChanger.SetValuesIfChanged(tagToUpdate , tagDataSource, ref valueChanged, ref statusChanged);
					}
				}
			}
		}


		public void ApplyPropertyChangesToMaster(Point point)
		{
			lock (this.LockObject)
			{
				var masterPoint = this.GetPoint(point.PointGuid);

				if (masterPoint == null)
				{
					return;
				}

				foreach (var property in masterPoint.Properties.Values)
				{
					PointProperty propertyClone = null;
					if (point.Properties.TryGetValue(property.PointPropertyGuid, out propertyClone)
					&& propertyClone.UpdatedDate != property.UpdatedDate)
					{
						if (propertyClone.Value is ICloneable)
						{
							property.Value = (propertyClone.Value as ICloneable).Clone();
						}
						else
						{
							Logger.LogError("ApplyPropertyChanges error: Property is not clonabled : " + property.PointID + "." + property.ID);
						}
						property.UpdatedDate = propertyClone.UpdatedDate;
						property.WrittenToEnterprise = false;
					}
				}
			}
		}

		public IEnumerable<PointTemplateLogic> GetPointsClearNeedsCalculation(IEnumerable<Guid> pointGuids)
		{
			var pointLogicList= new List<PointTemplateLogic>(42);

			lock (this.LockObject)
			{
				foreach (var pointGuid in pointGuids)
				{
					Point point;
					if (this.PointDictionary.TryGetValue(pointGuid, out point))
					{
						try
						{
							point.NeedsCalculation = false;

							PointTemplateLogic pointLogic = null;

							if (!this.PointLogicDictionary.TryGetValue(pointGuid, out pointLogic))
							{
								PointTemplateDataContainer pointTemplateDataContainer = null;
								if (!this.PointTemplateDataContainerDictionary.TryGetValue(point.PointTemplateGuid, out pointTemplateDataContainer))
								{
									Assembly pointTemplateLogicAssembly = null;

									var security = new SecurityClass() { UserID = "FMPointService" };
									var pointTemplatePointServiceData = FMChannelHelper.MakeCall<IPointTemplates, PointTemplatePointServiceData>(x => x.GetPointTemplatePointServiceData(security, point.PointTemplateGuid));

									// Load Code for custom point templates
									if (point.PointTemplateGuid == new Guid("0ADB4947-1CC4-4A44-91F8-E76F281EA718"))
									{
										pointTemplateLogicAssembly = CSScript.LoadCode(StandardTankScript.StandardTank, null, false);
									}
									else if (point.PointTemplateGuid == new Guid("3C7895BF-8A90-40CB-AC3B-04FD089B438B"))
									{
										pointTemplateLogicAssembly = CSScript.LoadCode(StandardVolumeScript.StandardVolume, null, false);
									}
									else if (point.PointTemplateGuid == new Guid("0FE444B2-920F-4572-AC60-31171C1F4763"))
									{
										pointTemplateLogicAssembly = CSScript.LoadCode(MovementScript.StandardMovement, null, false);
									}
									else if (point.PointTemplateGuid == new Guid("03E2911F-3195-4BEF-98AB-E7292D4B5B7F"))
									{
										pointTemplateLogicAssembly = CSScript.LoadCode(MovementControlScript.StandardMovementControl, null, false);
									}
									else if (point.PointTemplateGuid == new Guid("186348C4-C81F-4BC0-8A9E-5ABB9579885A"))
									{
										pointTemplateLogicAssembly = CSScript.LoadCode(StandardTduScript.StandardTdu, null, false);
									}
									else if (point.PointTemplateGuid == new Guid("42EDBDBD-C8FC-4B66-BB36-7EC0C969E378"))
									{
										pointTemplateLogicAssembly = CSScript.LoadCode(StandardNodeScript.StandardNode, null, false);
									}

									else
									{
										pointTemplateLogicAssembly = CSScript.LoadCode(pointTemplatePointServiceData.PointLogicScript, null, false);
									}

									pointTemplateDataContainer = new PointTemplateDataContainer() { PointTemplatePointServiceData = pointTemplatePointServiceData, PointTemplateLogicAssembly = pointTemplateLogicAssembly };
									this.PointTemplateDataContainerDictionary.Add(point.PointTemplateGuid, pointTemplateDataContainer);

									MovementProcessor.Instance().SignalPointChanges();
									LeakDetectionProcessor.Instance().SignalPointChanges();
								}
								if (point.PointTemplateVersion < pointTemplateDataContainer.PointTemplatePointServiceData.Version)
								{
									string errorMessage = "PointTemplateVersion < PointTemplatePointServiceData. Point " + point.ID + " not Scanned.";
									Logger.LogInfo(errorMessage);
									continue;
								}

								else if (point.PointTemplateVersion > pointTemplateDataContainer.PointTemplatePointServiceData.Version)
								{
									var security = new SecurityClass() { UserID = "FMPointService" };
									var pointTemplatePointServiceData = FMChannelHelper.MakeCall<IPointTemplates, PointTemplatePointServiceData>(x => x.GetPointTemplatePointServiceData(security, point.PointTemplateGuid));
									Assembly pointTemplateLogicAssembly;

									// Standard Templates are loaded once
									// Load Code for custom point templates
									if (point.PointTemplateGuid == new Guid("0ADB4947-1CC4-4A44-91F8-E76F281EA718"))
									{
										pointTemplateLogicAssembly = CSScript.LoadCode(StandardTankScript.StandardTank, null, false);
									}
									else if (point.PointTemplateGuid == new Guid("3C7895BF-8A90-40CB-AC3B-04FD089B438B"))
									{
										pointTemplateLogicAssembly = CSScript.LoadCode(StandardVolumeScript.StandardVolume, null, false);
									}
									else if (point.PointTemplateGuid == new Guid("0FE444B2-920F-4572-AC60-31171C1F4763"))
									{
										pointTemplateLogicAssembly = CSScript.LoadCode(MovementScript.StandardMovement, null, false);
									}
									else if (point.PointTemplateGuid == new Guid("186348C4-C81F-4BC0-8A9E-5ABB9579885A"))
									{
										pointTemplateLogicAssembly = CSScript.LoadCode(StandardTduScript.StandardTdu, null, false);
									}
									else if (point.PointTemplateGuid == new Guid("42EDBDBD-C8FC-4B66-BB36-7EC0C969E378"))
									{
										pointTemplateLogicAssembly = CSScript.LoadCode(StandardNodeScript.StandardNode, null, false);
									}
									else
									{
										pointTemplateLogicAssembly = CSScript.LoadCode(pointTemplatePointServiceData.PointLogicScript, null, false);
									}

									pointTemplateDataContainer = new PointTemplateDataContainer() { PointTemplatePointServiceData = pointTemplatePointServiceData, PointTemplateLogicAssembly = pointTemplateLogicAssembly };
									this.PointTemplateDataContainerDictionary[point.PointTemplateGuid] = pointTemplateDataContainer;

									MovementProcessor.Instance().SignalPointChanges();
									LeakDetectionProcessor.Instance().SignalPointChanges();
								}

								if (point.PointTemplateVersion != pointTemplateDataContainer.PointTemplatePointServiceData.Version)
								{
									string errorMessage = "PointTemplateVersion != PointTemplatePointServiceData. Point " + point.ID + " not Scanned.";
									Logger.LogInfo(errorMessage);
									continue;
								}


								pointLogic = pointTemplateDataContainer.PointTemplateLogicAssembly.CreateObject("FMPointService.PointExecution." + pointTemplateDataContainer.PointTemplatePointServiceData.ID.Replace(" "
																																			, string.Empty)
																																			, new object[]
																																			{
																																					point.Clone()
																																				,  pointTemplateDataContainer.PointTemplatePointServiceData.ModuleInstances
																																				, pointTemplateDataContainer.PointTemplatePointServiceData.ModuleLogicScript
																																			}) as PointTemplateLogic;
								this.PointLogicDictionary.Add(pointGuid, pointLogic);
								pointLogicList.Add(pointLogic);
							}
							else
							{

								// Point has been reloaded, apply changes
								if (point.UpdatedDate != pointLogic.Point.UpdatedDate)
								{
									Point.Copy(point, pointLogic.Point);
								}

								// Apply Tag, Alarm, and Property Changes that may have occured outside the pointLogic
								else
								{
									foreach (var tag in point.Tags.Values)
									{
										PointTag tagClone = null;
										if (pointLogic.Point.Tags.TryGetValue(tag.PointTagGuid, out tagClone)
										&& (tag.Input
										|| tag.OpcStatusCodeBits == StatusCodes.GoodLocalOverride
										|| (tagClone.OpcStatusCodeBits == StatusCodes.GoodLocalOverride
										&& tag.OpcStatusCodeBits != StatusCodes.GoodLocalOverride)
										|| tag.InputOutputType == PointTemplateTag.PointTagInputOutputType.Manual))
										{
											bool valueChanged = false;
											bool statusChanged = false;
											this.PointTagValueChanger.SetAlarmAndAlarmStatusIfChanged(tagClone, tag);
											this.PointTagValueChanger.SetValuesIfChanged(tagClone, tag, ref valueChanged, ref statusChanged);
										}
									}

									foreach (var property in point.Properties.Values)
									{
										PointProperty propertyClone = null;
										if (pointLogic.Point.Properties.TryGetValue(property.PointPropertyGuid, out propertyClone)
										&& propertyClone.UpdatedDate != property.UpdatedDate)
										{
											// This was formally XmlValue assignment but it fails with MovementData when PointValue is a TimeStamp
											pointLogic.Point.Properties[property.PointPropertyGuid] = property.Clone() as PointProperty;
										}
									}
								}

								pointLogicList.Add(pointLogic);
							}
						}
						catch (Exception e)
						{
							Logger.LogError("GetPointsClearNeedsCalculation exception: Point ID " + point.ID + ' ' + e.Message);
							EventLogger eventLogger = new EventLogger();
							eventLogger.Error("GetPointsClearNeedsCalculation exception: Point ID " + point.ID + ' ' + e.Message);
						}
					}

				}
			}

			return pointLogicList;
		}

		public PointTemplateLogic GetPointsCalculatorNeedsCalculation(Guid pointGuid)
		{
			PointTemplateLogic pointLogic = null;

			lock (this.LockObject)
			{
				Point originalpoint;
				if (this.PointDictionary.TryGetValue(pointGuid, out originalpoint))
				{
					Point point = originalpoint.Clone();
					point.NeedsCalculation = false;

					if (!PointLogicDictionary.TryGetValue(pointGuid, out pointLogic))
					{
						return pointLogic;
					}//end
				}
			}

			return pointLogic;
		}

        public void ArchiveAllPoints()
		{
			if (enableArchiveData)
			{
				var pointDictionary = GetPointDictionary(false);

				foreach (var point in pointDictionary.Values)
				{
					this.ArchiveRecordQueuer.CreateAndQueuePointArchive(point, true);
				}
			}
		}
		public PointCalculatorData ExecutePointsCalculator(SecurityClass security, Guid pointGuid, PointCalculatorData pointCalculatorData)
		{
			if(security == null)
			{
				Logger.LogError("ExecutePointsCalculator exception: Security cannot be null");
				throw new Exception("ExecutePointsCalculator exception: Security cannot be null");
			}

			// create the pointlogic object so we can run through the standard calculator routines and use the selected script.
			var pointLogic = ThreadSharedData.Instance().GetPointsCalculatorNeedsCalculation(pointGuid);

			// make sure we have the pointlogic object
			if(pointLogic == null)
			{
				Logger.LogError("ExecutePointsCalculator exception : pointLogic creation failure");
				return pointCalculatorData;
			}

			try
			{
				// map the passed in values to the point values
				//pointLogic = MapCalculatorValues(pointLogic, pointTagList);

				var timer = StatisticsLogger.Start("Execute calculator Point");

				pointLogic.Execute(null, PointTemplateLogic.CalculationType.Calculator, pointCalculatorData);

                // map the calculated values back intot he passed in structure
                //pointTagList = MapCalculatorValuesBack(pointLogic, pointTagList);

                StatisticsLogger.Stop(timer);
			}
			catch (Exception except)
			{
				Logger.LogError("ExecutePointsCalculator exception : " + except.Message);
			}

			return pointCalculatorData;

        }

        public List<PointTag> ExecutePointsCalculator(SecurityClass security, Guid pointGuid, List<PointTag> pointTags)
        {
            if (security == null)
            {
                Logger.LogError("ExecutePointsCalculator exception: Security cannot be null");
                throw new Exception("ExecutePointsCalculator exception: Security cannot be null");
            }

            // create the pointlogic object so we can run through the standard calculator routines and use the selected script.
            var pointLogic = ThreadSharedData.Instance().GetPointsCalculatorNeedsCalculation(pointGuid);

            // make sure we have the pointlogic object
            if (pointLogic == null)
            {
                Logger.LogError("ExecutePointsCalculator exception : pointLogic creation failure");
                return pointTags;
            }

            try
            {
                // map the passed in values to the point values
                pointLogic = MapCalculatorValues(pointLogic, pointTags);

                var timer = StatisticsLogger.Start("Execute calculator Point");

                pointLogic.Execute(null, PointTemplateLogic.CalculationType.Calculator, null);

                // map the calculated values back intot he passed in structure
                pointTags = MapCalculatorValuesBack(pointLogic, pointTags);

                StatisticsLogger.Stop(timer);
            }
            catch (Exception except)
            {
                Logger.LogError("ExecutePointsCalculator exception : " + except.Message);
            }

            return pointTags;
        }

        public PointTemplateLogic MapCalculatorValues(PointTemplateLogic pointLogic, List<PointTag> pointTagList)
		{
			// because the order of the tags being sent by the web app is not deteministic we need to map the new values into the logic structure
			foreach (var pointTag in pointTagList)
			{
				foreach(var logicTag in pointLogic.Tags)
				{
					if (logicTag.Value.ID == pointTag.ID)
					{
						logicTag.Value.Value = pointTag.Value;
						logicTag.Value.ServerTimeStamp = pointTag.SourceTimeStamp;
						logicTag.Value.SourceTimeStamp = pointTag.SourceTimeStamp;
						break;
					}
				}
			}
			return pointLogic;
		}

        public List<PointTag> MapCalculatorValuesBack(PointTemplateLogic pointLogic, List<PointTag> pointTagList)
		{
			// because the order of the tags being sent by the web app is not deteministic we need to map the new values into the logic structure
			foreach (var pointTag in pointTagList)
			{
				foreach (var logicTag in pointLogic.Tags)
				{
					if (logicTag.Value.ID == pointTag.ID)
					{
						pointTag.Value = logicTag.Value.Value;
						break;
					}
				}
			}
			return pointTagList;
		}

    }
}
