// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPointGroups.cs" company="Varec, Inc.">
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
	using System.Linq;
	using System.ServiceModel;
	using System.Text;
	using System.Threading.Tasks;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IPointGroups
	{
		[OperationContract]
		Guid Add(SecurityClass security, PointGroup pointGroup);

		[OperationContract]
		void Purge(SecurityClass security, Guid pointGroupGuid);

		[OperationContract]
		void Modify(SecurityClass security, PointGroup pointGroup);

		[OperationContract]
		PointGroup Get(SecurityClass security, Guid pointGroupGuid, Guid userGuid, Guid siteGuid);

		[OperationContract]
		Guid? GetDuplicate(SecurityClass security, string id, int pointGroupType, Guid ownerUserGuid, Guid siteGuid);

		[OperationContract]
		PointGroupCollection EnumerateByUserSite(SecurityClass security, Guid userGuid, Guid siteGuid);

	}
}
