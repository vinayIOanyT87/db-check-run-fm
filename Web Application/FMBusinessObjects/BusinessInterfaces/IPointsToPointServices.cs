// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPointsToPointServices.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IPointsToPointServices
	{
		[OperationContract]
		void PurgeByPointGuid(SecurityClass security, Guid pointGuid);

		[OperationContract]
		Dictionary<string, List<Guid>> EnumerateHostNameByPointTagGuid(SecurityClass security, List<Guid> pointTagGuids);

		[OperationContract]
		Dictionary<string, List<Guid>> EnumerateHostNameByPointGuid(SecurityClass security, List<Guid> pointGuids);

	}
}