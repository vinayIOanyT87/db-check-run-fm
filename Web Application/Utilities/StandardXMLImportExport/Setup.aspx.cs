using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Microsoft.Web.UI.WebControls;

using FMCommon;
using ConsolidatedDataObjects;
using ConsolidatedBLL;

using FM7Accounting;
using XMLImport;

namespace StandardXMLImportExport
{
	/// <summary>
	/// Summary description for Setup.
	/// </summary>
	public class Setup : System.Web.UI.Page
	{

		protected SecurityClass security;
		protected string site;
		protected string name;
		protected DocumentTop docTop;	

		protected ArrayList managerList;
		protected ArrayList ownerList;
		protected ArrayList productList;
		protected ArrayList supplierList;
		protected ArrayList carrierList;
		protected ArrayList consumerList;
		protected ArrayList aliasList;
		protected Hashtable listTable;

		protected System.Web.UI.WebControls.TextBox ConfigNameTextBox;
		protected System.Web.UI.WebControls.Label ConfigNameLabel;
		protected System.Web.UI.WebControls.Button OKButton;
		protected System.Web.UI.WebControls.CheckBox ManagerCheckBox;
		protected System.Web.UI.WebControls.CheckBox OwnerCheckBox;
		protected System.Web.UI.WebControls.CheckBox ProductCheckBox;
		protected System.Web.UI.WebControls.CheckBox ConsumerCheckBox;
		protected System.Web.UI.WebControls.CheckBox CarrierCheckBox;
		protected System.Web.UI.WebControls.CheckBox SupplierCheckBox;
		protected System.Web.UI.WebControls.CheckBox TransactionTypeCheckBox;
		protected Microsoft.Web.UI.WebControls.TreeView filterTreeView;
		protected System.Web.UI.WebControls.CheckBox IncludeDeletedTransactionsCheckBox;
		protected Microsoft.Web.UI.WebControls.TreeView AvailableTreeView;
		protected Microsoft.Web.UI.WebControls.TreeView AssignedTreeView;
		protected System.Web.UI.WebControls.Label AvailableLabel;
		protected System.Web.UI.WebControls.Label AssignedLabel;
		protected System.Web.UI.WebControls.Button AssignButton;
		protected System.Web.UI.WebControls.Image FadeImage;
		protected System.Web.UI.WebControls.Label FilterByLabel;
		protected System.Web.UI.WebControls.Button UnassignButton;
		
		private void Page_Load(object sender, System.EventArgs e)
		{
			site = Request.Params["Site"];
			name = Request.Params["Name"];

			ConfigNameTextBox.Text = name;

			if(docTop == null)
			{
				docTop = new DocumentTop();

				// Determine if the data dictionary shall be used.  True indicates that it
				// shall be used and false otherwise.
				bool useDataDictionary=false;
				if(Page.Session["UseDataDictionary"] == null
				|| (bool) Page.Session["UseDataDictionary"])
					useDataDictionary=true;

				docTop.AddDataDictionary(new DataDictionary(new AccountingSecurity().GetSecurity(Session["Token"] as string),useDataDictionary));
				Session.Add("DocumentTop", docTop);
			}

			security = (SecurityClass) Session["Security"];
			LoadLists();

			if(IsPostBack == false)
			{
				ApplyDataDictionary();

				SetupProcessor processor = new SetupProcessor();
				ImportFilter filter = processor.GetConfiguration(name);
				SetSelections(filter);
			}
		}

		private void ApplyDataDictionary()
		{
			// Determine if the data dictionary shall be used.  True indicates that it
			// shall be used and false otherwise.
			bool useDataDictionary=false;
			if(Page.Session["UseDataDictionary"] == null
			|| (bool) Page.Session["UseDataDictionary"])
				useDataDictionary=true;

			DataDictionary dd = new DataDictionary(new AccountingSecurity().GetSecurity(Session["Token"] as string),useDataDictionary);

			this.ConfigNameLabel.Text = dd.getNameFromGlobalDictionary(this.ConfigNameLabel.Text);
			this.IncludeDeletedTransactionsCheckBox.Text =
				dd.getNameFromGlobalDictionary(this.IncludeDeletedTransactionsCheckBox.Text);

			this.AssignedLabel.Text = dd.getNameFromGlobalDictionary(this.AssignedLabel.Text);
			this.AvailableLabel.Text = dd.getNameFromGlobalDictionary(this.AvailableLabel.Text);

			this.FilterByLabel.Text = dd.getNameFromGlobalDictionary(this.FilterByLabel.Text.Trim());
			this.ManagerCheckBox.Text = dd.getNameFromGlobalDictionary(this.ManagerCheckBox.Text);
			this.OwnerCheckBox.Text = dd.getNameFromGlobalDictionary(this.OwnerCheckBox.Text);
			this.ProductCheckBox.Text = dd.getNameFromGlobalDictionary(this.ProductCheckBox.Text);
			this.SupplierCheckBox.Text = dd.getNameFromGlobalDictionary(this.SupplierCheckBox.Text);
			this.CarrierCheckBox.Text = dd.getNameFromGlobalDictionary(this.CarrierCheckBox.Text);
			this.ConsumerCheckBox.Text = dd.getNameFromGlobalDictionary(this.ConsumerCheckBox.Text);
			this.TransactionTypeCheckBox.Text = dd.getNameFromGlobalDictionary(this.TransactionTypeCheckBox.Text);

			this.OKButton.Text = dd.getNameFromGlobalDictionary(this.OKButton.Text);
		}

		protected void SetSelections(ImportFilter filter)
		{
			this.IncludeDeletedTransactionsCheckBox.Checked = filter.IncludeDeletedTransactions;

			string name;

			ManagerCheckBox.Checked = false;
			if(filter.ManagerList != null)
			{
				ManagerCheckBox.Checked = true;

				name = "Managers";
				AddRoot(name);
				PopulateAvailableTree(name, managerList);

				TreeNode managerRoot = GetRootNode(AssignedTreeView, name);
				foreach(string managerName in filter.ManagerList)
				{
					TreeNode managerNode = new TreeNode();
					managerNode.Text = managerName;
					managerRoot.Nodes.Add(managerNode);
				}
			}
	
			OwnerCheckBox.Checked = false;
			if(filter.OwnerList != null)
			{
				OwnerCheckBox.Checked = true;

				name = "Owners";
				AddRoot(name);
				PopulateAvailableTree(name, ownerList);

				TreeNode ownerRoot = GetRootNode(AssignedTreeView, name);
				foreach(string ownerName in filter.OwnerList)
				{
					TreeNode ownerNode = new TreeNode();
					ownerNode.Text = ownerName;
					ownerRoot.Nodes.Add(ownerNode);
				}
			}
	
			ProductCheckBox.Checked = false;
			if(filter.ProductList != null)
			{
				ProductCheckBox.Checked = true;

				name = "Products";
				AddRoot(name);
				PopulateAvailableTree(name, productList);

				TreeNode productRoot = GetRootNode(AssignedTreeView, name);
				foreach(string productName in filter.ProductList)
				{
					TreeNode productNode = new TreeNode();
					productNode.Text = productName;
					productRoot.Nodes.Add(productNode);
				}
			}
	
			SupplierCheckBox.Checked = false;
			if(filter.SupplierList != null)
			{
				SupplierCheckBox.Checked = true;

				name = "Suppliers";
				AddRoot(name);
				PopulateAvailableTree(name, supplierList);

				TreeNode supplierRoot = GetRootNode(AssignedTreeView, name);
				foreach(string supplierName in filter.SupplierList)
				{
					TreeNode supplierNode = new TreeNode();
					supplierNode.Text = supplierName;
					supplierRoot.Nodes.Add(supplierNode);
				}
			}
	
			CarrierCheckBox.Checked = false;
			if(filter.CarrierList != null)
			{
				CarrierCheckBox.Checked = true;

				name = "Carriers";
				AddRoot(name);
				PopulateAvailableTree(name, carrierList);

				TreeNode carrierRoot = GetRootNode(AssignedTreeView, name);
				foreach(string carrierName in filter.CarrierList)
				{
					TreeNode carrierNode = new TreeNode();
					carrierNode.Text = carrierName;
					carrierRoot.Nodes.Add(carrierNode);
				}
			}
	
			ConsumerCheckBox.Checked = false;
			if(filter.ConsumerList != null)
			{
				ConsumerCheckBox.Checked = true;

				name = "Consumers";
				AddRoot(name);
				PopulateAvailableTree(name, consumerList);

				TreeNode consumerRoot = GetRootNode(AssignedTreeView, name);
				foreach(string consumerName in filter.ConsumerList)
				{
					TreeNode consumerNode = new TreeNode();
					consumerNode.Text = consumerName;
					consumerRoot.Nodes.Add(consumerNode);
				}
			}
	
			TransactionTypeCheckBox.Checked = false;
			if(filter.ManagerList != null)
			{
				TransactionTypeCheckBox.Checked = true;

				name = "Transaction Types";
				AddRoot(name);
				PopulateAvailableTree(name, aliasList);

				TreeNode transTypeRoot = GetRootNode(AssignedTreeView, name);
				foreach(string alias in filter.AliasList)
				{
					TreeNode transTypeNode = new TreeNode();
					transTypeRoot.Text = alias;
					transTypeRoot.Nodes.Add(transTypeNode);
				}
			}
		}

		protected void LoadLists()
		{
			listTable = new Hashtable();

			LoadOwners();
			LoadManagers();
			LoadSuppliers();
			LoadCarriers();
			LoadConsumers();
			LoadAliases();
			LoadProducts();
		}
		
		protected void LoadOwners()
		{
			// Retrieve owner list from shared components.
			CompaniesClass companies = new CompaniesClass();
			CompanyCollectionClass companyCollection = (CompanyCollectionClass)
				companies.EnumerateByRole(security, COMPANY_ROLE.OWNER, false);

			ownerList = new ArrayList();
			foreach (CompanyClass company in companyCollection)
			{
				ownerList.Add(company.ID);
			}
			listTable.Add("Owners", ownerList);
		}

		protected void LoadManagers()
		{
			// Retrieve manager list from shared components.
			CompaniesClass companies = new CompaniesClass();
			CompanyCollectionClass companyCollection = (CompanyCollectionClass)
				companies.EnumerateByRole(security, COMPANY_ROLE.MANAGER, false);

			managerList = new ArrayList();
			foreach (CompanyClass company in companyCollection)
			{
				managerList.Add(company.ID);
			}
			listTable.Add("Managers", managerList);
		}

		protected void LoadSuppliers()
		{
			// Retrieve supplier list from shared components.
			CompaniesClass companies = new CompaniesClass();

			CompanyCollectionClass companyCollection = (CompanyCollectionClass)
				companies.EnumerateByRole(security, COMPANY_ROLE.SUPPLIER, false);

			supplierList = new ArrayList();
			foreach (CompanyClass company in companyCollection)
			{
				supplierList.Add(company.ID);
			}
			listTable.Add("Suppliers", supplierList);
		}

		protected void LoadCarriers()
		{
			// Retrieve carrier list from shared components.
			CompaniesClass companies = new CompaniesClass();
			CompanyCollectionClass companyCollection = (CompanyCollectionClass)
				companies.EnumerateByRole(security, COMPANY_ROLE.CARRIER, false);

			carrierList = new ArrayList();
			foreach (CompanyClass company in companyCollection)
			{
				carrierList.Add(company.ID);
			}
			listTable.Add("Carriers", carrierList);
		}
		
		protected void LoadConsumers()
		{
			// Retrieve consumer list from shared components.
			CompaniesClass companies = new CompaniesClass();
			CompanyCollectionClass companyCollection = (CompanyCollectionClass)
				companies.EnumerateByRole(security, COMPANY_ROLE.CUSTOMER_BILLTO, false);

			consumerList = new ArrayList();
			foreach (CompanyClass company in companyCollection)
			{
				consumerList.Add(company.ID);
			}
			listTable.Add("Consumers", consumerList);
		}

		protected void LoadProducts()
		{
			ProductsClass products = new ProductsClass();
			ProductCollectionClass productCollection = 
				(ProductCollectionClass) products.Enumerate(security);

			productList = new ArrayList();
			foreach (ProductClass product in productCollection)
			{
				productList.Add(product.ID);
			}
			listTable.Add("Products", productList);
		}

		protected void LoadAliases()
		{
			TransactionAliasListSR sr = new TransactionAliasListSR();
			sr.Site = security.SiteID;
			sr.Security = security;

			AccountingClient accountingClient = new AccountingClient();
			AccountingService accountingService = accountingClient.connect();
			TransactionAliasListDO aliasListDO = (TransactionAliasListDO) accountingService.request(sr);

			aliasList = new ArrayList();
			foreach(string alias in aliasListDO.aliasList.Keys)
			{
				aliasList.Add(alias);
			}
			listTable.Add("Transaction Types", aliasList);
		}


		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
			this.AssignedTreeView.SelectedIndexChange += new Microsoft.Web.UI.WebControls.SelectEventHandler(this.AssignedTreeView_SelectedIndexChange);
			this.TransactionTypeCheckBox.CheckedChanged += new System.EventHandler(this.TransactionTypeCheckBox_CheckedChanged);
			this.ManagerCheckBox.CheckedChanged += new System.EventHandler(this.ManagerCheckBox_CheckedChanged);
			this.ProductCheckBox.CheckedChanged += new System.EventHandler(this.ProductCheckBox_CheckedChanged);
			this.OwnerCheckBox.CheckedChanged += new System.EventHandler(this.OwnerCheckBox_CheckedChanged);
			this.SupplierCheckBox.CheckedChanged += new System.EventHandler(this.SupplierCheckBox_CheckedChanged);
			this.ConsumerCheckBox.CheckedChanged += new System.EventHandler(this.ConsumerCheckBox_CheckedChanged);
			this.CarrierCheckBox.CheckedChanged += new System.EventHandler(this.CarrierCheckBox_CheckedChanged);
			this.AvailableTreeView.SelectedIndexChange += new Microsoft.Web.UI.WebControls.SelectEventHandler(this.AvailableTreeView_SelectedIndexChange);
			this.AssignButton.Click += new System.EventHandler(this.AssignButton_Click);
			this.UnassignButton.Click += new System.EventHandler(this.UnassignButton_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private System.Collections.Specialized.StringCollection GetSelections(string name)
		{
			System.Collections.Specialized.StringCollection list = null;

			TreeNode root = this.GetRootNode(AssignedTreeView, name);
			if(root == null)
			{
				return null;
			}
			list = new System.Collections.Specialized.StringCollection();
			foreach(TreeNode node in root.Nodes)
			{
				list.Add(node.Text);
			}
			return list;
		}


		private void OKButton_Click(object sender, System.EventArgs e)
		{
			ImportFilter filter = new ImportFilter();

			filter.Name = name;

			filter.ManagerList = GetSelections("Managers");
			filter.OwnerList = GetSelections("Owners");
			filter.ProductList = GetSelections("Products");
			filter.SupplierList = GetSelections("Suppliers");
			filter.ConsumerList = GetSelections("Consumers");
			filter.CarrierList = GetSelections("Carriers");
			filter.AliasList = GetSelections("Transaction Types");
			filter.IncludeDeletedTransactions = this.IncludeDeletedTransactionsCheckBox.Checked;

			SetupProcessor processor = new SetupProcessor();
			processor.SaveConfiguration(filter);
			
			string url = "../Accounting/ImportExportConfiguration.aspx";
			Response.Redirect(url);
		}

		private void AssignButton_Click(object sender, System.EventArgs e)
		{
			string selectedIndex = AvailableTreeView.SelectedNodeIndex;
			TreeNode selectedNode = AvailableTreeView.GetNodeFromIndex(selectedIndex);
			
			TreeNode parentNode = (TreeNode) selectedNode.Parent;
			parentNode.Nodes.Remove(selectedNode);

			foreach(TreeNode assignedRoot in AssignedTreeView.Nodes)
			{
				if(assignedRoot.Text == parentNode.Text)
				{
					assignedRoot.Nodes.Add(selectedNode);
					break;
				}
			}

			AvailableTreeView.SelectedNodeIndex = selectedIndex.Substring(0, selectedIndex.IndexOf("."));
			AssignButton.Enabled = false;
		}

		private void UnassignButton_Click(object sender, System.EventArgs e)
		{
			string selectedIndex = AssignedTreeView.SelectedNodeIndex;
			TreeNode selectedNode = AssignedTreeView.GetNodeFromIndex(selectedIndex);
			TreeNode parentNode = (TreeNode)selectedNode.Parent;
			parentNode.Nodes.Remove(selectedNode);

			foreach(TreeNode assignedRoot in AvailableTreeView.Nodes)
			{
				if(assignedRoot.Text == parentNode.Text)
				{
					assignedRoot.Nodes.Add(selectedNode);
					break;
				}
			}

			AssignedTreeView.SelectedNodeIndex = selectedIndex.Substring(0, selectedIndex.IndexOf(".") - 1);
			AssignButton.Enabled = false;
		}

		private void ManagerCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			string name = "Managers";
			CheckBoxChange(name, sender);
		}

		private void OwnerCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			string name = "Owners";
			CheckBoxChange(name, sender);
		}

		private void ProductCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			string name = "Products";
			CheckBoxChange(name, sender);
		}

		private void ConsumerCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			string name = "Consumers";
			CheckBoxChange(name, sender);
		}

		private void CarrierCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			string name = "Carriers";
			CheckBoxChange(name, sender);
		}

		private void SupplierCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			string name = "Suppliers";
			CheckBoxChange(name, sender);
		}

		private void TransactionTypeCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			string name = "Transaction Types";
			CheckBoxChange(name, sender);
		}

		private void CheckBoxChange(string name, object sender)
		{
			System.Web.UI.WebControls.CheckBox checkbox =
				(System.Web.UI.WebControls.CheckBox)sender;
			if(checkbox.Checked == true)
			{
				AddRoot(name);
				PopulateAvailableTree(name, (ArrayList) listTable[name]);
			}
			else
			{
				RemoveRoot(name);
			}		
		}

		private void PopulateAvailableTree(string rootName, ArrayList list)
		{
			TreeNode root = GetRootNode(AvailableTreeView, rootName);
			foreach(string item in list)
			{
				AddNode(root, item);
			}
		}

		private TreeNode AddNode(TreeNode parent, string name)
		{
			TreeNode newNode = new TreeNode();
			newNode.Text = name;
			parent.Nodes.Add(newNode);
			return newNode;
		}

		private void AddRoot(string name)
		{
			TreeNode newNode = new TreeNode();
			newNode.Text = name;
			AssignedTreeView.Nodes.Add(newNode);

			newNode = new TreeNode();
			newNode.Text = name;
			AvailableTreeView.Nodes.Add(newNode);
		}
		
		private void RemoveRoot(string name)
		{
			foreach(TreeNode node in AssignedTreeView.Nodes)
			{
				if(node.Text == name)
				{
					AssignedTreeView.Nodes.Remove(node);
					break;
				}
			}
			foreach(TreeNode node in AvailableTreeView.Nodes)
			{
				if(node.Text == name)
				{
					AvailableTreeView.Nodes.Remove(node);
					break;
				}
			}
		}

		private TreeNode GetRootNode(TreeView tree, string name)
		{
			foreach(TreeNode node in tree.Nodes)
			{
				if(node.Text == name)
				{
					return node;
				}
			}
			return null;
		}

		private void AvailableTreeView_SelectedIndexChange(object sender, Microsoft.Web.UI.WebControls.TreeViewSelectEventArgs e)
		{
			string selectedIndex = AvailableTreeView.SelectedNodeIndex;
			if(selectedIndex.IndexOf(".") < 0)
			{
				this.AssignButton.Enabled = false;
				this.AssignButton.Enabled = false;
				return;
			}
			this.AssignButton.Enabled = true;
			this.AssignButton.Enabled = true;
		}

		private void AssignedTreeView_SelectedIndexChange(object sender, Microsoft.Web.UI.WebControls.TreeViewSelectEventArgs e)
		{
			string selectedIndex = AssignedTreeView.SelectedNodeIndex;
			if(selectedIndex.IndexOf(".") < 0)
			{
				this.AssignButton.Enabled = true;
				this.AssignButton.Enabled = true;
				return;
			}
			this.AssignButton.Enabled = false;
			this.AssignButton.Enabled = false;
		}
	}
}
