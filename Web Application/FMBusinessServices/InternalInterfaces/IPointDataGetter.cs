// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPointTagDataGetter.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalInterfaces
{
	using System;
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;

	internal interface IPointDataGetter
	{
		List<PointTag> Get(SecurityClass security, List<Guid> pointTagGuids);

		List<PointValue> Get(SecurityClass security, List<PointValueIdentifier> pointValueIdentifiers, bool applyPointAccess);

		List<PointValue> GetChanges(SecurityClass security, List<PointValueIdentifier> pointValueIdentifiers, bool applyPointAccess);


		List<PointTag> GetWithoutPointAccess(SecurityClass security, List<Guid> pointTagGuids);
	}
}
