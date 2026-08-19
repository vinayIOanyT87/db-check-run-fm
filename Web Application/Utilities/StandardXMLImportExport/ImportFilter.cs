using System;

namespace StandardXMLImportExport
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
		protected System.Collections.Specialized.StringCollection siteList;
		protected System.Collections.Specialized.StringCollection aliasList;
		protected System.Collections.Specialized.StringCollection productList;
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
		}

		public System.Collections.Specialized.StringCollection ManagerList
		{
			get { return managerList; }
		}

		public System.Collections.Specialized.StringCollection CarrierList
		{
			get { return carrierList; }
		}

		public System.Collections.Specialized.StringCollection SupplierList
		{
			get { return supplierList; }
		}

		public System.Collections.Specialized.StringCollection ConsumerList
		{
			get { return consumerList; }
		}

		public System.Collections.Specialized.StringCollection SiteList
		{
			get { return siteList; }
		}

		public System.Collections.Specialized.StringCollection AliasList
		{
			get { return aliasList; }
		}

		public System.Collections.Specialized.StringCollection ProductList
		{
			get { return productList; }
		}

		#endregion Public Properties
		public ImportFilter()
		{
			ownerList = new System.Collections.Specialized.StringCollection();
			managerList = new System.Collections.Specialized.StringCollection();
			carrierList = new System.Collections.Specialized.StringCollection();
			supplierList = new System.Collections.Specialized.StringCollection();
			consumerList = new System.Collections.Specialized.StringCollection();
			siteList = new System.Collections.Specialized.StringCollection();
			aliasList = new System.Collections.Specialized.StringCollection();
			productList = new System.Collections.Specialized.StringCollection();
		}
	}
}
