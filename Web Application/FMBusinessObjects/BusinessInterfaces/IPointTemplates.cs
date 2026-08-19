// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPointTemplates.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Defines the IPointTemplates type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.ServiceModel;
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IPointTemplates
	{
		/// <summary>
		/// Enumerates the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="templateTypeGuid">The template type unique identifier.</param>
		/// <returns>
		/// A collection of PointTemplate.
		/// </returns>
		[OperationContract]
		PointTemplateCollection EnumerateByType(SecurityClass security, Guid? templateTypeGuid);

		/// <summary>
		/// Enumerates all point templates irrespective of license key.
		/// </summary>
		/// <param name="security">The security context.</param>
		/// <returns>
		/// A collection of PointTemplate.
		/// </returns>
		[OperationContract]
		PointTemplateCollection EnumerateForSiteCreation(SecurityClass security);

		[OperationContract]
		PointTemplateCollection EnumerateByModule(SecurityClass security, Guid moduleGuid);

		[OperationContract]
		[TransactionFlow( TransactionFlowOption.Allowed )]
		void Add( SecurityClass security, PointTemplate template );

		[OperationContract]
		[TransactionFlow( TransactionFlowOption.Allowed )]
		void Modify( SecurityClass security, PointTemplate template );

		[OperationContract]
		[TransactionFlow( TransactionFlowOption.Allowed )]
		void Purge(SecurityClass security, Guid pointTemplateGuid);

		[OperationContract]
		PointTemplate Get( SecurityClass security, Guid pointTemplateGuid );

		[OperationContract]
		PointTemplate GetPointTemplateBaseData(SecurityClass security, Guid pointTemplateGuid);


		[OperationContract]
		Guid? GetDuplicate(SecurityClass security, string id, Guid siteGuid);

		[OperationContract]
		Dictionary<Guid, string> GetPointCommandStatusDictionary(SecurityClass security, Guid pointTemplateGuid);

		[OperationContract]
		Dictionary<Guid, string> GetDeviceAlarmMapDictionary(SecurityClass security, Guid pointTemplateGuid);


		[OperationContract]
		PointCommandStatusList GetPointCommandStatusList(SecurityClass security, Guid pointTemplateGuid, Guid commandStatusListGuid);

		[OperationContract]
		DeviceAlarmMap GetDeviceAlarmMap(SecurityClass security, Guid pointTemplateGuid, Guid deviceAlarmMapGuid);

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string ID);

		[OperationContract]
		void PointCommandStatusListDeleted(SecurityClass security, Guid pointTemplateGuid, List<string> pointCommandStatusListsDeleted);

		[OperationContract]
		Dictionary<PointValueIdentifier, string> EnumeratePointValueIdentifiersForPointTemplate(
			SecurityClass security,
			Guid pointTemplateGuid,
			PointValueType valueType);

		[OperationContract]
		Dictionary<PointValueIdentifier, string> EnumeratePointValueIdentifiersForPointTemplateFilterByType(
			SecurityClass security,
			Guid pointTemplateGuid,
			PointValueType valueType,
			bool filter,
			string dataTypeString,
			PointValueFieldType fieldFilter);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		PointTemplate AddModule(SecurityClass security, Guid pointTemplateGuid, List<PointTemplateTag> tags, List<PointTemplateProperty> properties, List<ModuleToPointTemplateMap> moduleInstances);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		PointTemplate DeleteModule(SecurityClass security, Guid pointTemplateGuid, List<Guid> tagGuidList, List<Guid> tagsWithAlarmsGuidList, List<Guid> propertyGuidList, Guid moduleInstanceGuid);


		[OperationContract]
		PointTemplatePointServiceData GetPointTemplatePointServiceData(SecurityClass security, Guid pointTemplateGuid);


		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Import(SecurityClass security, PointTemplate pointTemplate);

	}
}
