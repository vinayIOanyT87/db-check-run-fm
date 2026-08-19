// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPointHistories.cs" company="Varec, Inc.">
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
	public interface IPointHistories
	{
        [OperationContract]
        void Add(SecurityClass security, PointHistory pointHistory);

        [OperationContract]
		void Modify(SecurityClass security, PointHistory pointHistory);

		[OperationContract]
		PointHistory Get(SecurityClass security, Guid userGuid, Guid siteGuid);
	}
}
