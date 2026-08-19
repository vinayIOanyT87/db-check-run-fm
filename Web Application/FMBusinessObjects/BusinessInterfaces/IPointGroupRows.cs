// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPointGroupRows.cs" company="Varec, Inc.">
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
	public interface IPointGroupRows
	{
		[OperationContract]
		Guid Add(SecurityClass security, PointGroupRow pointGroupRow);

		[OperationContract]
		void Purge(SecurityClass security, Guid pointGroupRowGuid);

		[OperationContract]
		void Modify(SecurityClass security, PointGroupRow pointGroupRow);

		[OperationContract]
		PointGroupRow GetByPointGroupGuid(SecurityClass security, Guid pointGroupGuid);

	}
}
