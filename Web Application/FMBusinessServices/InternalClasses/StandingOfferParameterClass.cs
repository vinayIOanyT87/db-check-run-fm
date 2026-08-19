// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StandingOfferParameterClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Implementation fo the ISaveTransmitTranListProcessor interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses
{
	using System;

	/// <summary>
	/// Implementation fo the ISaveTransmitTranListProcessor interface.
	/// </summary>
	internal class StandingOfferParameterClass
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StandingOfferParameterClass"/> class.
		/// </summary>
		public StandingOfferParameterClass()
		{
			this.ProductGuid = Guid.Empty;
			this.ProductID = string.Empty;
			this.SupplierGuid = Guid.Empty;
			this.SupplierID = string.Empty;
			this.TransID = string.Empty;
			this.InventoryDate = DateTime.MaxValue;
		}

		/// <summary>
		/// Gets or sets the inventory date
		/// </summary>
		public DateTime InventoryDate { get; set; }

		/// <summary>
		/// Gets or sets the product ID
		/// </summary>
		public string ProductID { get; set; }

		/// <summary>
		/// Gets or sets the product GUID
		/// </summary>
		public Guid ProductGuid { get; set; }

		/// <summary>
		/// Gets or sets the supplier ID
		/// </summary>
		public string SupplierID { get; set; }

		/// <summary>
		/// Gets or sets the supplier GUID
		/// </summary>
		public Guid SupplierGuid { get; set; }

		/// <summary>
		/// Gets or sets the trans ID
		/// </summary>
		public string TransID { get; set; }
	}
}
