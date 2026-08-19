// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestsAndInspectionsForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.QualityControlWebApp
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using FMWebApp;

	/// <summary>
	///     Summary description for TestsAndInspectionsForm.
	/// </summary>
	public partial class TestsAndInspectionsForm : FMFormBase, IEntityDiscovery
	{
		#region Constants and Fields

		private const string SortDirection = "TestSortDirection";
		private const string SortExpression = "TestSortExpression";
		private const string TestFindString = "TestFindString";

		#endregion

		private bool IsDLAEnterprise
		{
			get
			{
				return FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescEnterpriseKey());
			}
		}

		#region Explicit Interface Properties

		bool IEntityDiscovery.EntityAssignable
		{
			get
			{
				return true;
			}
		}

		Type IEntityDiscovery.EntityEngineType
		{
			get
			{
				return typeof(ITests);
			}
		}

		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.TEST;
			}
		}

		#endregion

		#region Explicit Interface Methods

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(
			SecurityClass security, ENTITY_ASSIGNMENT_TYPE type)
		{
			TestCollectionClass testCollection =
				FMChannelHelper.MakeCall<ITests, TestCollectionClass>(tests => tests.Enumerate(security, null, null));

			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (TestClass test in testCollection)
			{
				if (type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (security.SiteGuid == test.SiteGuid)
					{
						continue;
					}

					if (security.LoginSiteGuid != test.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (security.SiteGuid != test.SiteGuid)
					{
						continue;
					}
				}

				var entityToSiteMap = new EntityToSiteMapClass(test);
				entityToSiteMapCollection.Add(entityToSiteMap);
			}

			return entityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string id)
		{
			return FMChannelHelper.MakeCall<ITests, Guid>(tests => tests.GetIdentityGuid(security, id));
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid siteGuid)
		{
			FMChannelHelper.MakeCall<ITests>(
				tests =>
					{
						TestClass test = tests.Get(security, guid);
						test.SiteGuid = siteGuid;
						tests.Modify(security, test);
					});
		}

		#endregion

		#region Methods

		protected void AddButtonClick(object sender, EventArgs e)
		{
			try
			{
				this.Redirect("TestDetailPage.aspx");
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void FindBtnClick(object sender, EventArgs e)
		{
			try
			{
				if ((this.FindTextBox == null) || (this.FindTextBox.Text.Length < 1))
				{
					this.Session.Remove(TestFindString);
				}
				else
				{
					this.FindTextBox.Text = this.FindTextBox.Text.ToUpper();
					this.Session[TestFindString] = this.FindTextBox.Text.ToUpper();
				}

				// Update the page with the new contents.
				this.TestDataGrid.CurrentPageIndex = 0;
				
				TestCollectionClass testCollection =
					FMChannelHelper.MakeCall<ITests, TestCollectionClass>(
						tests =>
						tests.Enumerate(
							this.Security, 
							this.Session[TestFindString] as string, 
							this.Session[SortExpression] as string + " " + this.Session[SortDirection]));

				this.Session["TestCollection"] = testCollection;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void PageSizeDropDownSelectedIndexChanged(object source, EventArgs e)
		{
			try
			{
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Security.HasRight(RIGHT.MODIFY_TEST_ITEMS) && !this.Security.HasRight(RIGHT.VIEW_TEST_ITEMS))
				{
					throw new Exception("Access denied.");
				}

				if (!this.Security.HasRight(RIGHT.MODIFY_TEST_ITEMS))
				{
					this.EnableControls(false);
				}

				if (!this.Page.IsPostBack)
				{
					if (this.Session["TestDataGrid.CurrentPageIndex"] == null)
					{
						this.TestDataGrid.CurrentPageIndex = 0;
					}
					else
					{
						this.TestDataGrid.CurrentPageIndex = (int)this.Session["TestDataGrid.CurrentPageIndex"];
					}

					if (this.Session[SortExpression] == null)
					{
						this.Session[SortExpression] = "TestName";
					}

					if (this.Session[SortDirection] == null)
					{
						this.Session[SortDirection] = "DESC";
					}

					this.ShowDlaEnergyColumns();
					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void ShowAllButtonClick(object sender, EventArgs e)
		{
			try
			{
				this.Session.Remove(TestFindString);
				this.FindTextBox.Text = string.Empty;
				this.TestDataGrid.CurrentPageIndex = 0;
				TestCollectionClass testCollection =
					FMChannelHelper.MakeCall<ITests, TestCollectionClass>(
						tests =>
						tests.Enumerate(
							this.Security, 
							this.Session[TestFindString] as string, 
							this.Session[SortExpression] as string + " " + this.Session[SortDirection]));

				this.Session["TestCollection"] = testCollection;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void TestDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (this.TestDataGrid == null)
			{
				return;
			}

			try
			{
				var siteGuidLabel = e.Item.FindControl("SiteGuid") as Label;
				if (siteGuidLabel != null)
				{
					bool disableEdit = false;

					if (!this.Security.HasRight(RIGHT.MODIFY_TEST_ITEMS) || this.Security.SiteGuid != Guid.Parse(siteGuidLabel.Text))
					{
						disableEdit = true;
					}
					else if (IsDLAEnterprise && !Security.HasRight(RIGHT.CONFIGURE_DLA_TEST))
					{
						var rowView = e.Item.DataItem as DataRowView;
						
						if (rowView != null && rowView.Row != null)
						{
							var testCodeValue = rowView.Row["TestCode"] as string;
							disableEdit = !string.IsNullOrEmpty(testCodeValue);
						}
					}

					if (disableEdit)
					{
						var editLinkButton = e.Item.FindControl("EditLinkButton") as LinkButton;
						if (editLinkButton != null)
						{
							editLinkButton.Enabled = false;
						}

						var deleteLinkButton = e.Item.FindControl("DeleteLinkButton") as LinkButton;
						if (deleteLinkButton != null)
						{
							deleteLinkButton.Enabled = false;
						}
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void TestDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.TestDataGrid.EditItemIndex > -1)
				{
					return;
				}

				this.TestDataGrid.CurrentPageIndex = e.NewPageIndex;
				this.Session["TestDataGrid.CurrentPageIndex"] = e.NewPageIndex;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void TestDataGridSortCommand(object source, DataGridSortCommandEventArgs e)
		{
			try
			{
				var sortExpression = this.Session[SortExpression] as string;
				var sortDirection = this.Session[SortDirection] as string;

				if (e.SortExpression != sortExpression)
				{
					this.Session[SortDirection] = "DESC";
				}
				else
				{
					if (sortDirection != null && sortDirection.Equals("DESC"))
					{
						this.Session[SortDirection] = "ASC";
					}
					else
					{
						this.Session[SortDirection] = "DESC";
					}
				}

				this.Session[SortExpression] = e.SortExpression;

				TestCollectionClass testCollection =
					FMChannelHelper.MakeCall<ITests, TestCollectionClass>(
						tests =>
						tests.Enumerate(
							this.Security, 
							this.Session[TestFindString] as string, 
							this.Session[SortExpression] as string + " " + this.Session[SortDirection]));

				this.Session["TestCollection"] = testCollection;

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void TestsDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var indexLabel = (Label)e.Item.FindControl("Index");
				if (indexLabel != null)
				{
					var testCollection = (TestCollectionClass)this.Session["TestCollection"];

					TestClass test = testCollection[Convert.ToInt32(indexLabel.Text)];

					bool isAssociated =
						FMChannelHelper.MakeCall<ITests, bool>(
							tests => tests.IsAssociatedWithTestResult(this.Security, test.IdentityGuid));

					if (isAssociated)
					{
						throw new Exception("Cannot Delete Test because it is associated with one or more Test Set Results.");
					}

					// Non Zero Index indicates Qualification has been committed to database
					if (test.IdentityGuid != Guid.Empty)
					{
						this.GetSecurity();

						FMChannelHelper.MakeCall<ITests>(tests => tests.Purge(this.Security, test.IdentityGuid));
					}

					if (this.TestDataGrid.EditItemIndex == e.Item.ItemIndex)
					{
						this.TestDataGrid.EditItemIndex = -1;
						this.EnableControls(true);
					}
					else if (this.TestDataGrid.EditItemIndex > e.Item.ItemIndex)
					{
						this.TestDataGrid.EditItemIndex--;
					}

					testCollection.RemoveAt(Convert.ToInt32(indexLabel.Text));
					if (this.TestDataGrid.CurrentPageIndex > 0
					    && this.TestDataGrid.CurrentPageIndex * this.TestDataGrid.PageSize >= testCollection.Count)
					{
						this.TestDataGrid.CurrentPageIndex--;
					}

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void TestsDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			var label = (Label)e.Item.FindControl("Index");
			if (label != null)
			{
				try
				{
					int inx = Convert.ToInt32(label.Text);
					var testCollection = this.Session["TestCollection"] as TestCollectionClass;
					if (testCollection != null)
					{
						TestClass test = testCollection[inx];

						this.Redirect("TestDetailPage.aspx?IdentityGuid=" + test.IdentityGuid);
					}
				}
				catch (Exception except)
				{
					this.ErrorHandler(except);
				}
			}
		}

		private void ShowDlaEnergyColumns()
		{
			const int ProductColumnIndex = 4;//bds
			const int TestCodeColumnIndex = 5;//bds
			const int TestMethodColumnIndex = 6;//bds
			bool showColumn = IsDLAEnterprise && Security.HasRight(RIGHT.CONFIGURE_DLA_TEST);
			this.TestDataGrid.Columns[ProductColumnIndex].Visible = showColumn;
			this.TestDataGrid.Columns[TestCodeColumnIndex].Visible = showColumn;
			this.TestDataGrid.Columns[TestMethodColumnIndex].Visible = showColumn;
		}

		

		private void EnableControls(bool bEnable)
		{
			this.AddButton.Enabled = bEnable;
			this.AddButton2.Enabled = bEnable;
			this.TestsFormPageSizeDropDown.Enabled = bEnable;
		}

		private ICollection Enumerate()
		{
            SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(sites => sites.Get(this.Security, this.Security.SiteGuid, false, false, false));

			// Enumerate 
			TestCollectionClass testCollection =
				FMChannelHelper.MakeCall<ITests, TestCollectionClass>(
					tests =>
					tests.Enumerate(
						this.Security, 
						this.Session[TestFindString] as string, 
						this.Session[SortExpression] as string + " " + this.Session[SortDirection]));

			this.Session["TestCollection"] = testCollection;

			var mapDataTable = new DataTable();

			mapDataTable.Columns.Add("SiteGuid", typeof(string));
			mapDataTable.Columns.Add("Index", typeof(Int32)); // index into TestCollection, not TestClass Index
			mapDataTable.Columns.Add("TestName", typeof(string));
			mapDataTable.Columns.Add("MeasurementUnit", typeof(string));
			mapDataTable.Columns.Add("ValidationRule", typeof(string));
			mapDataTable.Columns.Add("SampleSize", typeof(string));
			mapDataTable.Columns.Add("Product", typeof(string));
			mapDataTable.Columns.Add("TestCode", typeof(string));
			mapDataTable.Columns.Add("TestMethod", typeof(string));

			for (int iItem = 0; iItem < testCollection.Count; iItem++)
			{
				DataRow mapDataRow = mapDataTable.NewRow();

				TestClass test = testCollection[iItem];
				mapDataRow["SiteGuid"] = test.SiteGuid.ToString();
				mapDataRow["Index"] = iItem;
				mapDataRow["TestName"] = test.ID;
				mapDataRow["MeasurementUnit"] = test.MeasurementUnit;
				mapDataRow["ValidationRule"] = test.ValidationRule;
                mapDataRow["SampleSize"] = test.SampleSize.ToString(site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
				mapDataRow["Product"] = test.ProductID;
				mapDataRow["TestCode"] = test.TestCode;
				mapDataRow["TestMethod"] = test.TestMethod;

				mapDataTable.Rows.Add(mapDataRow);
			}

			var testDataView = new DataView(mapDataTable);
			return testDataView;
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		private void UpdateView()
		{
			ICollection data = this.Enumerate();

			if (this.TestsFormPageSizeDropDown != null)
			{
				this.TestsFormPageSizeDropDown.SetPageSize(this.TestDataGrid, data.Count);
			}

			this.TestDataGrid.DataSource = data;
			this.TestDataGrid.DataBind();
		}

		#endregion
	}
}