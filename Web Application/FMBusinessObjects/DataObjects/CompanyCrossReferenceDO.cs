// ------------------------------------------public --------------------------------------------------------------------------
// <copyright file="CompanyCrossReferenceDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CompanyCrossReferenceDO type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	/// <summary>
	/// The company cross reference data object.
	/// </summary>
	public class CompanyCrossReferenceDO
	{
		/// <summary>
		/// The map types.
		/// </summary>
		public enum CrossReferenceTypes
		{
			Navy = 1,
			BuyerDODAAC,
			None
		};
	}
}
