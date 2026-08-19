// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataRetriever.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IDataRetriever interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMExportService
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// The DataRetriever interface.
	/// </summary>
	public interface IDataRetriever
	{
		/// <summary>
		/// Gets the interface id.
		/// </summary>
		string InterfaceId {
			get;
		}

		string ConfigFileName {
			get;
		}

		/// <summary>
		/// Gets the data associated with the specified request id.
		/// </summary>
		/// <param name="requestId">The request id</param>
		/// <param name="securityObject">The FuelsManager security object</param>
		/// <returns> The specified DataResultClass object</returns>
		DataResultClass GetData(string requestId, SecurityClass securityObject);
	}
}
