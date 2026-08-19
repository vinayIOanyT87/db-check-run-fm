using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class LedgerPageDO : DataObject
	{
		#region Attributes
		[DataMember]
		private bool singleOwnerSystem;
		[DataMember]
		private ArrayList monthList;
		[DataMember]
		private ArrayList yearList;
		[DataMember]
		private ArrayList productList;
		[DataMember]
		private ArrayList managerList;
		[DataMember]
		private ArrayList ownerList;
		#endregion Attributes

		#region Properties
		/// <summary>
		/// This property will set and get the flag that indicates
		/// whether the system is a single owner (true) or multiple
		/// owner (false).
		/// </summary>
		public bool SingleOwnerSystem
		{
			get { return this.singleOwnerSystem; }
			set { this.singleOwnerSystem = value; }
		}

		/// <summary>
		/// This property will set and get the product list.
		/// </summary>
		public ArrayList ProductList
		{
			get { return this.productList; }
			set { this.productList = value; }
		}

		/// <summary>
		/// This property will set and get the manager list.
		/// </summary>
		public ArrayList ManagerList
		{
			get { return this.managerList; }
			set { this.managerList = value; }
		}

		/// <summary>
		/// This property will set and get the owner list.
		/// </summary>
		public ArrayList OwnerList
		{
			get { return this.ownerList; }
			set { this.ownerList = value; }
		}

		/// <summary>
		/// This property will set and get the list of months in name
		/// format (i.e. June).
		/// </summary>
		public ArrayList MonthList
		{
			get { return this.monthList; }
			set { this.monthList = value; }
		}

		/// <summary>
		/// This property will set and get the list of years in the
		/// following format: "yyyy".
		/// </summary>
		public ArrayList YearList
		{
			get { return this.yearList; }
			set { this.yearList = value; }
		}
		#endregion Properties

		#region Constructors
		/// <summary>
		/// This is the default constructor for the ledger page data object class.
		/// </summary>
		public LedgerPageDO()
		{
			init();
		}
		#endregion

		#region Protected methods
		protected void init()
		{
			this.singleOwnerSystem = true;
		}
		#endregion

		#region Overrides
		public override string getSelectCommand()
		{
			return null;
		}
		public override string getInsertCommand()
		{
			return null;
		}
		public override string getDeleteCommand()
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
