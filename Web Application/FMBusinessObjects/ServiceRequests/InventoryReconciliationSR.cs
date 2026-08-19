// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InventoryReconciliationSR.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the InventoryReconciliationSR type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.ServiceRequests
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// The inventory reconciliation service request.
	/// </summary>
	[Serializable]
    [DataContract]
	public class InventoryReconciliationSR : AccountingServiceRequest
	{
		#region Public Attributes
		/// <summary>
		/// The request types.
		/// </summary>
		public enum RequestTypes { GET_HEADER_DATA, REFRESH, FindAdjustments, NONE };
		#endregion

		#region Private Attibutes
		[DataMember] private RequestTypes subrequest;
		[DataMember] private string month;
		[DataMember] private string productId;
		[DataMember] private string managerId;
		[DataMember] private string tankId;
		[DataMember] private bool useDataDictionary;
		[DataMember] private double? tolerance;
		[DataMember] private DateTime? inventoryDate;
		[DataMember] private string inventoryDateStr;
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="InventoryReconciliationSR"/> class.
		/// </summary>
		public InventoryReconciliationSR ( )
		{
			this.Init ( );
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the manager ID.
		/// </summary>
		public string ManagerID
		{
			get { return this.managerId; }
			set { this.managerId = value; }
		}

		/// <summary>
		/// Gets or sets the month.
		/// </summary>
		public string Month
		{
			get { return this.month; }
			set { this.month = value; }
		}

		/// <summary>
		/// Gets or sets the product ID.
		/// </summary>
		public string ProductID
		{
			get { return this.productId; }
			set { this.productId = value; }
		}

		/// <summary>
		/// Gets or sets the tank ID attribute.
		/// </summary>
		/// <remarks>
		/// The TankId property is used to limit the inventory reconciliation data
		/// to the specified tank.  If TankId is an empty string, then the inventory
		/// reconciliation will be collected across all tanks meeting the
		/// product and manager restrictions
		/// </remarks>
		public string TankId
		{
			get { return this.tankId; }
			set { this.tankId = value; }
		}

		/// <summary>
		/// Gets or sets the Tolerance attribute.
		/// </summary>
		public double? Tolerance
		{
			get { return this.tolerance; }
			set { this.tolerance = value; }
		}

		/// <summary>
		/// Gets or sets the use data dictionary attribute.
		/// </summary>
		public bool UseDataDictionary
		{
			get { return this.useDataDictionary; }
			set { this.useDataDictionary = value; }
		}

		/// <summary>
		/// Gets or sets the adjustment distribution sub-request attribute.
		/// </summary>
		public RequestTypes Subrequest
		{
			get { return this.subrequest; }
			set { this.subrequest = value; }
		}

		/// <summary>
		/// Gets or sets the inventory date string.
		/// </summary>
		public string InventoryDateStr
		{
			get { return this.inventoryDateStr; }
			set { this.inventoryDateStr = value; }
		}

		/// <summary>
		/// Gets or sets the inventory date.
		/// </summary>
		public DateTime? InventoryDate
		{
			get { return this.inventoryDate; }
			set { this.inventoryDate = value; }
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init ( )
		{
			this.subrequest			= RequestTypes.NONE;
			this.productId			= string.Empty;
			this.managerId			= string.Empty;
			this.tankId				= string.Empty;
			this.tolerance			= null;
			this.useDataDictionary	= false;
			this.inventoryDateStr	= string.Empty;
			this.inventoryDate		= null;
		}
		#endregion
	}
}
