using System;

namespace FMBusinessObjects.DataObjects
{
	/// <summary>
	/// Summary description for ImportRequest.
	/// </summary>
	public class ImportFilter
	{

		#region Attributes
		protected string name;
		protected System.Collections.Specialized.StringCollection ownerList;
		protected System.Collections.Specialized.StringCollection managerList;
		protected System.Collections.Specialized.StringCollection carrierList;
		protected System.Collections.Specialized.StringCollection supplierList;
		protected System.Collections.Specialized.StringCollection consumerList;
		protected System.Collections.Specialized.StringCollection aliasList;
		protected System.Collections.Specialized.StringCollection productList;
		protected Date fromDate;
		protected Date toDate;

		protected bool includeDeletedTransactions;
		#endregion Attributes

		#region Public Properties
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public System.Collections.Specialized.StringCollection OwnerList
		{
			get { return ownerList; }
			set { ownerList = value; }
		}

		public System.Collections.Specialized.StringCollection ManagerList
		{
			get { return managerList; }
			set { managerList = value; }
		}

		public System.Collections.Specialized.StringCollection CarrierList
		{
			get { return carrierList; }
			set { carrierList = value; }
		}

		public System.Collections.Specialized.StringCollection SupplierList
		{
			get { return supplierList; }
			set { supplierList = value; }
		}

		public System.Collections.Specialized.StringCollection ConsumerList
		{
			get { return consumerList; }
			set { consumerList = value; }
		}

		public System.Collections.Specialized.StringCollection AliasList
		{
			get { return aliasList; }
			set { aliasList = value; }
		}

		public System.Collections.Specialized.StringCollection ProductList
		{
			get { return productList; }
			set { productList = value; }
		}
		public bool IncludeDeletedTransactions
		{
			get { return includeDeletedTransactions; }
			set { includeDeletedTransactions = value; }
		}
		public Date FromDate
		{
			get { return fromDate; }
			set { fromDate = value; }
		}
		public Date ToDate
		{
			get { return toDate; }
			set { toDate = value; }
		}
		#endregion Public Properties
		public ImportFilter()
		{
		}
	}
}
