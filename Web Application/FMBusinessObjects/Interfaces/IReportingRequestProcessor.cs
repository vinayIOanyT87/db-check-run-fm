// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReportingRequestProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IReportingRequestProcessor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.Interfaces
{
    using FMBusinessObjects.DataObjects;
    using System.Collections.Generic;
    using System.Data;

	/// <summary>
	/// The FMDataExchangeProcessor interface.
	/// </summary>
	public interface IReportingRequestProcessor
	{
		DataSet GetReportData(SecurityClass security, Dictionary<string, string> parameters);
        List<string> GetReportParameters(SecurityClass security);
	}
}
