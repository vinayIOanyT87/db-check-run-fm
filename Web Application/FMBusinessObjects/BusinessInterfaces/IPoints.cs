// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPoints.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IPoints
	{
		[OperationContract]
		void CreatePoints(SecurityClass security, string prefix, int numberOfPoints, Guid pointTemplateGuid);

		[OperationContract]
		Guid CreatePoint(SecurityClass security, string id, PointTemplate pointTemplate);

		/// <summary>
		/// Adds the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="point">The point.</param>
		/// <param name="generateIdentityGuid">Create a new guid. Otherwise, uses the identity guid on the point.</param>
		/// <returns></returns>
		[OperationContract]
		Guid Add(SecurityClass security, Point point, bool generateIdentityGuid = true);

		[OperationContract]
		void Purge(SecurityClass security, Guid pointGuid);

		[OperationContract]
		void Modify(SecurityClass security, Point point);

		[OperationContract]
		void ModifyPointValues(SecurityClass security, List<PointValue> pointValues);


		/// <summary>
		/// Gets the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="pointGuid">The point unique identifier.</param>
		///  <param name="enforcePointAccess">Enforce point access.</param>
		/// <returns></returns>
		[OperationContract]
		Point Get(SecurityClass security, Guid pointGuid, bool enforcePointAccess = false);

		/// <summary>
		/// Gets the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="pointID">The point ID.</param>
		/// <returns></returns>
		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string pointID);


		/// <summary>
		/// Gets the specified points.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="pointGuidList">List of the point unique identifier.</param>
		/// <returns></returns>
		[OperationContract]
		PointCollection GetPoints(SecurityClass security, List<Guid> pointGuidList);

		/// <summary>
		/// Gets the specified point base data.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="pointGuid">The point unique identifier.</param>
		/// <returns></returns>
		[OperationContract]
		Point GetPointBaseData(SecurityClass security, Guid pointGuid);



		/// <summary>
		/// Enumerates the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="siteGuid">The site unique identifier.</param>
		/// <returns>
		/// A collection of Point.
		/// </returns>
		[OperationContract]
		PointCollection EnumerateBySite(SecurityClass security, Guid siteGuid);

		/// <summary>
		/// Enumerates the specified security after applying a filter.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="siteGuid">The site unique identifier.</param>
		/// <param name="pointFilter">Filter definition for the points</param>
		/// <param name="tagFilterList">List of tag IDs to return</param>
		/// <returns>
		/// A collection of Point.
		/// </returns>
		[OperationContract]
		PointCollection EnumerateBySiteFiltered(SecurityClass security, Guid siteGuid, PointGroupFilterRules pointFilter, List<string> tagFilterList);

		[OperationContract]
		PointCollection EnumerateActiveAlarmsBySite(SecurityClass security, Guid siteGuid);

		/// <summary>
		/// Enumerates enabled points from the specified site.
		/// </summary>
		/// <param name="security">A valid FuelsManager security object.</param>
		/// <param name="siteGuid">The identity guid of the site to load.</param>
		/// <returns>A collection of points.</returns>
		[OperationContract]
		PointCollection EnumerateEnabledBySite(SecurityClass security, Guid siteGuid);

		[OperationContract]
		int EnabledPointCountForSimulator(SecurityClass security, string opcUaEndPoint);


		[OperationContract]
		PointCollection EnumerateEnabledForSimulator(SecurityClass security, string opcUaEndPoint, int startIndex, int count);

		/// <summary>
		/// Enumerates points for the given site with information for the summary page.
		/// </summary>
		/// <param name="security">A valid FuelsManager security object.</param>
		/// <param name="siteGuid">The identity guid of the site to load.</param>
		/// <param name="includeDictionaries">Include dictionaries when set to true.</param>
		/// <param name="applyPointAccess">Enforce Point Access Rights.</param>
		/// <param name="propertyID">Only include points having a property.</param>
		/// <returns>A collection of partially populated points.</returns>
		[OperationContract]
		PointCollection EnumerateForSummary(SecurityClass security, Guid siteGuid, Boolean includeDictionaries, bool applyPointAccess = false, string propertyID = null);

		/// <summary>
		/// Enumerates points for the given site with information for the point list dropdown page.
		/// </summary>
		/// <param name="security">A valid FuelsManager security object.</param>
		/// <param name="siteGuid">The identity guid of the site to load.</param>
		/// <param name="includeDictionaries">Include dictionaries when set to true.</param>
		/// <param name="applyPointAccess">Enforce Point Access Rights.</param>
		/// <param name="propertyID">Only include points having a property.</param>
		/// <returns>A collection of partially populated points.</returns>
		[OperationContract]
		PointCollection EnumerateForSummaryWithCategories(SecurityClass security, Guid siteGuid, Boolean includeDictionaries = true, bool applyPointAccess = false, string propertyID = null);

		/// <summary>
		/// Gets the maximum point row version for a site.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="siteGuid">The site unique identifier.</param>
		/// <returns></returns>
		[OperationContract]
		Int64? GetMaxPointRowVersionForSite(SecurityClass security, Guid siteGuid);

		/// <summary>
		/// Gets the point identifier dictionary for site.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="siteGuid">The site unique identifier.</param>
		/// <param name="pointTemplateTypeGuid">The point template unique identifier.</param>
		/// <param name="pointTemplateGuid">The point template unique identifier.</param>
		/// <param name="pointCategoryGuid">The point category unique identifier.</param>
		/// <param name="applyPointAccess">The point category unique identifier.</param>
		/// <returns></returns>
		[OperationContract]
		List<KeyValuePair<Guid, string>> EnumeratePointIdListForSiteTemplateTypeTemplateCategory(SecurityClass security, Guid siteGuid, Guid? pointTemplateTypeGuid, Guid? pointTemplateGuid, Guid? pointCategoryGuid, bool applyPointAccess);

		[OperationContract]
		PointCollection EnumerateByPointTemplateGuid(SecurityClass security, Guid pointTemplateGuid);

		[OperationContract]
		PointCollection EnumerateBasicByPointTemplateGuids(SecurityClass security, Guid[] pointTemplateGuids);

		[OperationContract]
		PointCollection EnumerateByPointTemplateGuids(SecurityClass security, Guid[] pointTemplateGuids);

		[OperationContract]
		Dictionary<PointValueIdentifier, string> EnumeratePointValueIdentifiersForPoint(SecurityClass security, Guid pointGuid, PointValueType valueType, bool applyPointAccess);

		[OperationContract]
		List<Tuple<Guid, string, string, int>> EnumeratePointProductGraphicInfo(SecurityClass security);

		[OperationContract]
		Dictionary<PointValueIdentifier, string> EnumeratePointValueIdentifiersForPointFilterByType(
			SecurityClass security,
			Guid pointGuid,
			PointValueType valueType,
			bool filter,
			string dataTypeString,
			PointValueFieldType fieldFilter,
			bool applyPointAccess);

		[OperationContract]
		Dictionary<PointValueIdentifier, PointValueAccess> EnumerateRestrictedAccessByPointValueIdenfierList(SecurityClass security, List<PointValueIdentifier> pointValueIdentifierList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Import(SecurityClass security, Point point);
      [OperationContract]
      [TransactionFlow(TransactionFlowOption.Allowed)]
      void ModifyTagsOnly(SecurityClass security, Point point);

      [OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void UpdateRowVersion(SecurityClass security, Guid pointGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void UpdateRowVersionBySite(SecurityClass security, Guid siteGuid);

		[OperationContract]
		Dictionary<string, Dictionary<Guid, Guid>> EnumerateTerminalAutomationTankTags(SecurityClass security, Guid siteGuik);

		[OperationContract]
		Dictionary<PointValueIdentifier, PointValue> EnumerateByPointValueIdentifierList(SecurityClass security, List<PointValueIdentifier> pointValueIdentifierList);

		[OperationContract]
		NodeModuleType GetMovementNodeModuleType(SecurityClass security, Guid pointGuid);
	}
}