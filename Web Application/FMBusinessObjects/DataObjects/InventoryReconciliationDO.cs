// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InventoryReconciliationDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the InventoryReconciliationDO type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections;
	using System.Runtime.Serialization;

	/// <summary>
	/// The inventory reconciliation data object.
	/// </summary>
	[DataContract]
	[Serializable]
	[KnownType(typeof(BaseCollections))]
	[KnownType(typeof(InventoryReconciliationLineItemDO))]
	public class InventoryReconciliationDO : DataObject
	{
		#region Private Attributes
		[DataMember] private ArrayList managerList;
		[DataMember] private ArrayList productList;
		[DataMember] private ArrayList monthList;
		[DataMember] private ArrayList yearList;
		[DataMember] private ArrayList tankList;
		[DataMember] private BaseCollections lineItems;
		[DataMember] private bool hasAdjustments;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="InventoryReconciliationDO"/> class.
		/// </summary>
		public InventoryReconciliationDO()
		{
			this.Init();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property sets and gets the line item array list attribute.
		/// </summary>
		public BaseCollections LineItems
		{
			get { return this.lineItems; }
			set { this.lineItems = value; }
		}

		/// <summary>
		/// This property sets and gets the manager array list attribute.
		/// </summary>
		public ArrayList ManagerList
		{
			get { return this.managerList; }
			set { this.managerList = value; }
		}

		/// <summary>
		/// This property sets and gets the product array list attribute.
		/// </summary>
		public ArrayList ProductList
		{
			get { return this.productList; }
			set { this.productList = value; }
		}

		/// <summary>
		/// This property sets and gets the month array list attribute.
		/// </summary>
		public ArrayList MonthList
		{
			get { return this.monthList; }
			set { this.monthList = value; }
		}

		/// <summary>
		/// This property sets and gets the year array list attribute.
		/// </summary>
		public ArrayList YearList
		{
			get { return this.yearList; }
			set { this.yearList = value; }
		}

		/// <summary>
		/// This property sets and gets the tank array list attribute.
		/// </summary>
		public ArrayList TankList
		{
			get { return this.tankList; }
			set { this.tankList = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether has adjustments.
		/// </summary>
		public bool HasAdjustments
		{
			get { return this.hasAdjustments; }
			set { this.hasAdjustments = value; }
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.managerList	= new ArrayList();
			this.productList	= new ArrayList();
			this.monthList		= new ArrayList();
			this.yearList		= new ArrayList();
			this.tankList		= new ArrayList();
			this.lineItems		= new BaseCollections();
			this.hasAdjustments = false;
		}
		#endregion

		#region Overrides
		public override string getDeleteCommand()
		{
			return null;
		}
		public override string getInsertCommand()
		{
			return null;
		}
		public override string getSelectCommand()
		{
			return null;
		}
		public override string getUpdateCommand()
		{
			return null;
		}
		#endregion Overrides
	}
}
