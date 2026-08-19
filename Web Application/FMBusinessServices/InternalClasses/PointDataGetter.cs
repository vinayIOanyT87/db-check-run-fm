// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PointTagDataGetter.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Gets point tag data based on a list of point tag guids
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMBusinessServices.InternalInterfaces;
	using FMBusinessServices.ServiceClasses;

	using FMCore;

	using FMPointCommon;

	internal class PointDataGetter : IPointDataGetter
	{
		private static readonly IPointServiceInfoGetter PointServiceInfoGetter = new PointServiceInfoGetter();

		protected void GetFromDB(SecurityClass security, List<Guid> pointTagGuidList, ref Dictionary<Guid,PointTag> outputTagDictionary )
		{
			var pointTags = new PointTags();
			var pointTagDict = pointTags.EnumerateByTagList(security, pointTagGuidList);
			foreach (var pointTag in pointTagDict.Values)
			{
				if (!outputTagDictionary.ContainsKey(pointTag.PointTagGuid))
				{
					outputTagDictionary.Add(pointTag.PointTagGuid, pointTag);
				}
			}
		}

		protected void GetFromDB(SecurityClass security, List<PointValueIdentifier> pointValueIdentifierList, ref Dictionary<PointValueIdentifier, PointValue> outputPointValueDictionary)
		{
			var points = new Points();
			var pointValueDictonary = points.EnumerateByPointValueIdentifierList(security, pointValueIdentifierList);
			foreach (var pointValue in pointValueDictonary.Values)
			{
				var pointValueIdentifier = new PointValueIdentifier(pointValue);

				if (!outputPointValueDictionary.ContainsKey(pointValueIdentifier))
				{
					outputPointValueDictionary.Add(pointValueIdentifier, pointValue);
				}
			}
		}

		protected void GetChangesFromDB(SecurityClass security, List<PointValueIdentifier> pointValueIdentifierList, ref Dictionary<PointValueIdentifier, PointValue> outputPointValueIdentifierDictionary)
		{
			var points = new Points();
			var pointValueIdentifierDictonary = points.EnumerateByPointValueIdentifierList(security, pointValueIdentifierList);
			foreach(var pointValueIdentifier in pointValueIdentifierList)
			{
				PointValue pointValue = null;
				if(pointValueIdentifierDictonary.TryGetValue(pointValueIdentifier, out pointValue)
				&& pointValue.ServerTimeStamp.UtcTicks != pointValueIdentifier.UtcTicks)
				{
					outputPointValueIdentifierDictionary.Add(pointValueIdentifier, pointValue);
				}
			}
		}


		protected void GetFromPointService(SecurityClass security, List<Guid> pointTagGuidList, string hostName, ref Dictionary<Guid, PointTag> outputTagDictionary)
		{
			var info = PointServiceInfoGetter.Info;
			string protocol = info.PointServiceBindingEndPointAddress.Substring(
			0,
			info.PointServiceBindingEndPointAddress.IndexOf("/", StringComparison.Ordinal));

			string endPoint = protocol + "//" + hostName + "/FMPointService";
			var pointTagList = FMChannelHelper.MakeCall<IPointService, List<PointTag>>(
				info.PointServiceBindingType,
				info.PointServiceBindingConfiguration,
				endPoint,
				x => x.GetPointTagData(security, pointTagGuidList));
			foreach (var pointTag in pointTagList)
			{
				if (outputTagDictionary.ContainsKey(pointTag.PointTagGuid) == false)
				{
					outputTagDictionary.Add(pointTag.PointTagGuid, pointTag);
				}
			}
		}


		protected void GetFromPointService(SecurityClass security, List<PointValueIdentifier> pointValueIdentifierList, string hostName, ref Dictionary<PointValueIdentifier, PointValue> outputPointValueDictionary)
		{
			var info = PointServiceInfoGetter.Info;
			string protocol = info.PointServiceBindingEndPointAddress.Substring(
			0,
			info.PointServiceBindingEndPointAddress.IndexOf("/", StringComparison.Ordinal));

			string endPoint = protocol + "//" + hostName + "/FMPointService";
			var hostPointValueList = FMChannelHelper.MakeCall<IPointService, List<PointValue>>(
				info.PointServiceBindingType,
				info.PointServiceBindingConfiguration,
				endPoint,
				x => x.GetPointValueData(security, pointValueIdentifierList));

			foreach (var pointValue in hostPointValueList)
			{
				var pointValueIdentifier = new PointValueIdentifier(pointValue);
				if (!outputPointValueDictionary.ContainsKey(pointValueIdentifier))
				{
					outputPointValueDictionary.Add(pointValueIdentifier, pointValue);
				}
			}
		}

		protected void GetChangesFromPointService(SecurityClass security, List<PointValueIdentifier> pointValueIdentifierList, string hostName, ref Dictionary<PointValueIdentifier, PointValue> outputPointValueIdentifierDictionary)
		{
			var info = PointServiceInfoGetter.Info;
			string protocol = info.PointServiceBindingEndPointAddress.Substring(
			0,
			info.PointServiceBindingEndPointAddress.IndexOf("/", StringComparison.Ordinal));

			string endPoint = protocol + "//" + hostName + "/FMPointService";
			var hostPointValueList = FMChannelHelper.MakeCall<IPointService, List<PointValue>>(
				info.PointServiceBindingType,
				info.PointServiceBindingConfiguration,
				endPoint,
				x => x.GetPointValueDataChanges(security, pointValueIdentifierList));

			foreach (var pointValue in hostPointValueList)
			{
				var pointValueIdentifier = new PointValueIdentifier(pointValue);
				if (!outputPointValueIdentifierDictionary.ContainsKey(pointValueIdentifier))
				{
					outputPointValueIdentifierDictionary.Add(pointValueIdentifier, pointValue);
				}
			}
		}



		/// <summary>
		/// This method will get the point tag without the point access permission filtering.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="pointTagGuids"></param>
		/// <returns></returns>
		public List<PointTag> GetWithoutPointAccess(SecurityClass security, List<Guid> pointTagGuids)
		{
			security.ThrowIfNull("security");
			pointTagGuids.ThrowIfNull("pointTagGuids");

			try
			{

				var pointsToPointServices = new PointsToPointServices();

				var hostNameToPointTagGuidListDictionary = pointsToPointServices.EnumerateHostNameByPointTagGuid(
					security,
					pointTagGuids);

				var pointTagDictionary = new Dictionary<Guid, PointTag>();

				foreach (var hostName in hostNameToPointTagGuidListDictionary.Keys)
				{
					if (string.IsNullOrEmpty(hostName))
					{
						this.GetFromDB(security, hostNameToPointTagGuidListDictionary[hostName], ref pointTagDictionary);
					}
					else
					{
						// Errors will occur when FMPointService(s) are down, but return all data.
						try
						{
							this.GetFromPointService(security, hostNameToPointTagGuidListDictionary[hostName], hostName, ref pointTagDictionary);
						}
						catch (Exception)
						{
							this.GetFromDB(security, hostNameToPointTagGuidListDictionary[hostName], ref pointTagDictionary);
						}
					}
				}

				return this.ConsolidatePointTagsAndFillInGaps(security, pointTagDictionary, pointTagGuids);
			}
			catch (Exception e)
			{
				string errorMessage = string.Format("PointDataGetter.Get Exception : " + e.Message);
				var eventLogging = new EventLogging();
				eventLogging.LogEvent(errorMessage, EventLogEntryType.Error);
			}

			return null;
		}

		public List<PointTag> Get(SecurityClass security, List<Guid> pointTagGuids)
		{
			security.ThrowIfNull("security");
			pointTagGuids.ThrowIfNull("pointTagGuids");

			try
			{

				var pointsToPointServices = new PointsToPointServices();
				var pointTags = new PointTags();
				var pointTagGuidsFilteredbyPointAccess = pointTags.EnumerateTagListByPointAccess(security, pointTagGuids);
				var hostNameToPointTagGuidListDictionary = pointsToPointServices.EnumerateHostNameByPointTagGuid(
					security,
					pointTagGuidsFilteredbyPointAccess);

				var pointTagDictionary = new Dictionary<Guid, PointTag>();

				foreach (var hostName in hostNameToPointTagGuidListDictionary.Keys)
				{
					if (string.IsNullOrEmpty(hostName))
					{
						this.GetFromDB(security, hostNameToPointTagGuidListDictionary[hostName],ref pointTagDictionary);
					}
					else
					{
						// Errors will occur when FMPointService(s) are down, but return all data.
						try
						{
							this.GetFromPointService(security, hostNameToPointTagGuidListDictionary[hostName], hostName, ref pointTagDictionary);
						}
						catch (Exception)
						{
							this.GetFromDB(security, hostNameToPointTagGuidListDictionary[hostName], ref pointTagDictionary);
						}
					}
				}

				return this.ConsolidatePointTagsAndFillInGaps(security, pointTagDictionary, pointTagGuids);
			}
			catch (Exception e)
			{
				string errorMessage = string.Format("PointDataGetter.Get Exception : " + e.Message);
				var eventLogging = new EventLogging();
				eventLogging.LogEvent(errorMessage, EventLogEntryType.Error);
			}

			return null;
		}

		public List<PointValue> Get(SecurityClass security, List<PointValueIdentifier> pointValueItentifierList, bool applyPointAccess)
		{
			security.ThrowIfNull("security");
			pointValueItentifierList.ThrowIfNull("pointValueItentifiers");

			try
			{

				var pointsToPointServices = new PointsToPointServices();
				var hostNameToPointValueIdentifierListDictionary = pointsToPointServices.EnumerateHostNameByPointValueIdentifier(
					security,
					pointValueItentifierList);

				var pointValueDictionary = new Dictionary<PointValueIdentifier, PointValue>(pointValueItentifierList.Count);

				foreach (var hostName in hostNameToPointValueIdentifierListDictionary.Keys)
				{
					if (hostName == "Deleted")
					{
						continue;
					}
					else if (string.IsNullOrEmpty(hostName))
					{
						this.GetFromDB(security, hostNameToPointValueIdentifierListDictionary[hostName], ref pointValueDictionary);
					}
					else
					{
						// Errors will occur when FMPointService(s) are down, but return all data.
						try
						{
							this.GetFromPointService(security, hostNameToPointValueIdentifierListDictionary[hostName], hostName, ref pointValueDictionary);
						}
						catch (Exception)
						{
							this.GetFromDB(security, hostNameToPointValueIdentifierListDictionary[hostName], ref pointValueDictionary);
						}
					}
				}


				if (applyPointAccess)
				{
					var points = new Points();

					var pointAccessDictionary = points.EnumerateRestrictedAccessByPointValueIdenfierList(security, pointValueItentifierList);
					foreach (var pointValueIdentifier in pointAccessDictionary.Keys)
					{
						PointValue pointValue;
						if (pointValueDictionary.TryGetValue(pointValueIdentifier, out pointValue))
						{
							pointValue.Access = pointAccessDictionary[pointValueIdentifier];
							if (!pointValue.Access.View
							&& !pointValue.Access.Modify)
							{
								pointValue.Value = null;
								pointValue.AlarmState = null;
							}
						}
					}
				}

				return this.ConsolidatePointValuesAndFillInGaps(security, pointValueDictionary, pointValueItentifierList);
			}
			catch (Exception e)
			{
				string errorMessage = string.Format("PointDataGetter.Get Exception : " + e.Message);
				var eventLogging = new EventLogging();
				eventLogging.LogEvent(errorMessage, EventLogEntryType.Error);
			}

			return null;
		}


		public List<PointValue> GetChanges(SecurityClass security, List<PointValueIdentifier> pointValueItentifierList, bool applyPointAccess)
		{
			security.ThrowIfNull("security");
			pointValueItentifierList.ThrowIfNull("pointValueItentifiers");

			try
			{

				var pointsToPointServices = new PointsToPointServices();
				var hostNameToPointValueIdentifierListDictionary = pointsToPointServices.EnumerateHostNameByPointValueIdentifier(
					security,
					pointValueItentifierList);

				var pointValueDictionary = new Dictionary<PointValueIdentifier, PointValue>(pointValueItentifierList.Count);

				foreach (var hostName in hostNameToPointValueIdentifierListDictionary.Keys)
				{

					if (hostName == "Deleted")
					{
						foreach (var pointValueIdentifier in hostNameToPointValueIdentifierListDictionary[hostName])
						{
							var pointValue = new PointValue()
							{
								PointValueIdentifier = pointValueIdentifier,
								Value = null,
								ServerTimeStamp = DateTimeOffset.UtcNow
							};

							pointValueDictionary.Add(pointValueIdentifier, pointValue);
						}
					}
					else if (string.IsNullOrEmpty(hostName))
					{
						this.GetChangesFromDB(security, hostNameToPointValueIdentifierListDictionary[hostName], ref pointValueDictionary);
					}
					else
					{
						// Errors will occur when FMPointService(s) are down, but return all data.
						try
						{
							this.GetChangesFromPointService(security, hostNameToPointValueIdentifierListDictionary[hostName], hostName, ref pointValueDictionary);
						}
						catch (Exception)
						{
							this.GetChangesFromDB(security, hostNameToPointValueIdentifierListDictionary[hostName], ref pointValueDictionary);
						}
					}
				}

				// Potentially big performance impprovedment to only acquire access when point values changes are detected
				if (applyPointAccess && pointValueDictionary.Count > 0)
				{
					var points = new Points();

					var pointValueIdentifierSubset = pointValueDictionary.Values.Select(x => x.PointValueIdentifier);

					var pointAccessDictionary = points.EnumerateRestrictedAccessByPointValueIdenfierList(security, pointValueIdentifierSubset.ToList());
					foreach (var pointValueIdentifier in pointAccessDictionary.Keys)
					{
						PointValue pointValue;
						if (pointValueDictionary.TryGetValue(pointValueIdentifier, out pointValue))
						{
							pointValue.Access = pointAccessDictionary[pointValueIdentifier];
							if (!pointValue.Access.View
							&& !pointValue.Access.Modify)
							{
								pointValue.Value = null;
								pointValue.AlarmState = null;
							}
						}
					}
				}

				return pointValueDictionary.Values.ToList();
			}
			catch (Exception e)
			{
				string errorMessage = string.Format("PointDataGetter.Get Exception : " + e.Message);
				var eventLogging = new EventLogging();
				eventLogging.LogEvent(errorMessage, EventLogEntryType.Error);
			}

			return null;
		}



		public List<PointTag> ConsolidatePointTagsAndFillInGaps(
				SecurityClass security,
				Dictionary<Guid, PointTag> pointTagDictionary,
				List<Guid> pointTagGuids)
		{
			security.ThrowIfNull("security");
			pointTagDictionary.ThrowIfNull("pointTagDictionary");
			pointTagGuids.ThrowIfNull("pointTagGuids");

			var pointTagList = new List<PointTag>(pointTagGuids.Count);

			var pointTags = new PointTags();

			var restricted = "Restricted";
			if (security.UseDataDictionary)
			{
				var dataDictionaries = new DataDictionariesClass();
				restricted = dataDictionaries.Get(security.SiteGuid, restricted);
			}

			pointTagGuids.ForEach(
				pointTagGuid =>
				{
					PointTag pointTag;

					if (!pointTagDictionary.ContainsKey(pointTagGuid))
					{                    // return a tag with null values and bad status
						pointTag = new PointTag() { PointTagGuid = pointTagGuid, Value = restricted, ValueType = Type.GetType("System.String"), AlarmState = restricted};
					}
					else if (pointTagDictionary.TryGetValue(pointTagGuid, out pointTag) == false)
					{
						// return a tag with null values and bad status
						pointTag = new PointTag() { PointTagGuid = pointTagGuid };
					}

					pointTagList.Add(pointTag);
				});

			return pointTagList;
		}


		public List<PointValue> ConsolidatePointValuesAndFillInGaps(
				SecurityClass security,
				Dictionary<PointValueIdentifier, PointValue> pointValueDictionary,
				List<PointValueIdentifier> pointValueIdentifiers)
		{
			security.ThrowIfNull("security");
			pointValueDictionary.ThrowIfNull("pointTagDictionary");
			pointValueIdentifiers.ThrowIfNull("pointValueIdentifiers");

			var pointValueList = new List<PointValue>(pointValueIdentifiers.Count);

			pointValueIdentifiers.ForEach(
				pointValueIdentifier =>
				{
					PointValue pointValue;

					if (!pointValueDictionary.TryGetValue(pointValueIdentifier, out pointValue))
					{
						// return a value with null values and bad status
						pointValue = new PointValue() { PointValueIdentifier = pointValueIdentifier };
					}

					if(pointValue == null)
                    {
						pointValue = new PointValue() { PointValueIdentifier = pointValueIdentifier };
					}

					pointValue.WellKnownIdentityGuid = pointValueIdentifier.WellKnownIdentityGuid;

					pointValueList.Add(pointValue);
				});

			return pointValueList;
		}
	}
}
