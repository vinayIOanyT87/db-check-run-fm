// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPointGroupColumns.cs" company="Varec, Inc.">
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
	public interface IPointGroupColumns
	{
		[OperationContract]
		Guid Add(SecurityClass security, PointGroupColumn pointGroupColumn);

		[OperationContract]
		void Purge(SecurityClass security, Guid pointGroupColumnGuid);

		[OperationContract]
		void Modify(SecurityClass security, PointGroupColumn pointGroup);

		[OperationContract]
		PointGroupColumn GetByPointGroupGuid(SecurityClass security, Guid pointGroupGuid);

	}
}
