// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestSetsForm.aspx.cs" company="Varec, Inc.">
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
	///     Summary description for TestSetsForm.
	/// </summary>
	public partial class TestSetsForm : FMFormBase, IEntityDiscovery
	{
		#region Constants and Fields

		private const string SortDirection = "TestSetSortDirection";
		private const string SortExpression = "TestSetSortExpression";
		private const string TestsetFindString = "TestSetFindString";

		#endregion

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
				return typeof(ITestSets);
			}
		}

		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.TEST_SET;
			}
		}

		#endregion

		#region Explicit Interface Methods

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(
			SecurityClass security, ENTITY_ASSIGNMENT_TYPE type)
		{
			TestSetCollectionClass testSetCollection =
				FMChannelHelper.MakeCall<ITestSets, TestSetCollectionClass>(sets => sets.Enumerate(security, null, null));

			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (TestSetClass testSet in testSetCollection)
			{
				if (type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (security.SiteGuid == testSet.SiteGuid)
					{
						continue;
					}

					if (security.LoginSiteGuid != testSet.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (security.SiteGuid != testSet.SiteGuid)
					{
						continue;
					}
				}

				var entityToSiteMap = new EntityToSiteMapClass(testSet);
				entityToSiteMapCollection.Add(entityToSiteMap);
			}

			return entityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string id)
		{
			return FMChannelHelper.MakeCall<ITestSets, Guid>(sets => sets.GetIdentityGuid(security, id));
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid siteGuid)
		{
			FMChannelHelper.MakeCall<ITestSets>(
				sets =>
					{
						TestSetClass testSet = sets.Get(security, guid);
						testSet.SiteGuid = siteGuid;
						sets.Modify(security, testSet);
					});
		}

		#endregion

		#region Methods

		protected void AddButtonClick(object sender, EventArgs e)
		{
			try
			{
				this.Redirect("TestSetDetailForm.aspx");
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void EnableControls(bool bEnable)
		{
			this.AddButton.Enabled = bEnable;
			this.AddButton2.Enabled = bEnable;
		}

		protected void FindBtnClick(object sender, EventArgs e)
		{
			try
			{
				if ((this.FindTextBox == null) || (this.FindTextBox.Text.Length < 1))
				{
					this.Session.Remove(TestsetFindString);
				}
				else
				{
					this.FindTextBox.Text = this.FindTextBox.Text.ToUpper();
					this.Session[TestsetFindString] = this.FindTextBox.Text.ToUpper();
				}

				// Update the page with the new contents.
				this.TestSetDataGrid.CurrentPageIndex = 0;

				TestSetCollectionClass testSetCollection =
					FMChannelHelper.MakeCall<ITestSets, TestSetCollectionClass>(
						sets =>
						sets.Enumerate(
							this.Security,
							this.Session[TestsetFindString] as string,
							this.Session[SortExpression] as string + " " + this.Session[SortDirection]));

				this.Session["TestSetCollection"] = testSetCollection;
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

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Security.HasRight(RIGHT.MODIFY_TEST_ITEMS) && !this.Security.HasRight(RIGHT.VIEW_TEST_ITEMS))
				{
					throw new Exception("Access denied.");
				}

				if (!this.Page.IsPostBack)
				{
					if (!this.Security.HasRight(RIGHT.MODIFY_TEST_ITEMS))
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					if (this.Session["TestSetDataGrid.CurrentPageIndex"] == null)
					{
						this.TestSetDataGrid.CurrentPageIndex = 0;
					}
					else
					{
						this.TestSetDataGrid.CurrentPageIndex = (int)this.Session["TestSetDataGrid.CurrentPageIndex"];
					}

					if (this.Session[SortExpression] == null)
					{
						this.Session[SortExpression] = "TestSetName";
					}

					if (this.Session[SortDirection] == null)
					{
						this.Session[SortDirection] = "DESC";
					}

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
				this.Session.Remove(TestsetFindString);
				this.FindTextBox.Text = string.Empty;
				this.TestSetDataGrid.CurrentPageIndex = 0;

				TestSetCollectionClass testSetCollection =
					FMChannelHelper.MakeCall<ITestSets, TestSetCollectionClass>(
						sets =>
						sets.Enumerate(
							this.Security,
							this.Session[TestsetFindString] as string,
							this.Session[SortExpression] as string + " " + this.Session[SortDirection]));

				this.Session["TestSetCollection"] = testSetCollection;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void TestSetDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var indexLabel = e.Item.FindControl("Index") as Label;
				var testSetCollection = this.Session["TestSetCollection"] as TestSetCollectionClass;

				if (indexLabel != null && testSetCollection != null)
				{
					int inx = Convert.ToInt32(indexLabel.Text);

					TestSetClass testSet = testSetCollection[inx];
					if (testSet.IdentityGuid != Guid.Empty)
					{
						this.GetSecurity();

						FMChannelHelper.MakeCall<ITestSets>(
							sets =>
							{
								testSet = sets.GetByIncludeTests(this.Security, testSet.IdentityGuid, true);
								if (testSet.testCollection.Count > 0)
								{
									throw new Exception(
										"There are still tests associated with the test set definition. Please delete the associated tests and try again.");
								}

								sets.Purge(this.Security, testSet.IdentityGuid);
							});
					}

					testSetCollection.RemoveAt(inx);
				}
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void TestSetDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var label = (Label)e.Item.FindControl("Index");
				var testSetCollection = this.Session["TestSetCollection"] as TestSetCollectionClass;

				if (label != null && testSetCollection != null)
				{
					try
					{
						int inx = Convert.ToInt32(label.Text);
						
						TestSetClass testSet = testSetCollection[inx];
						this.Redirect("TestSetDetailForm.aspx?IdentityGuid=" + testSet.IdentityGuid);
					}
					catch (Exception except)
					{
						this.ErrorHandler(except);
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void TestSetDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			try
			{
				var deleteButton = (LinkButton)e.Item.FindControl("DeleteLinkButton");
				var siteGuidLabel = (Label)e.Item.FindControl("SiteGuid");

				// Disable the edit and delete buttons if the user does not have modify rights or
				// not login to a site group.
				if (deleteButton != null)
				{
					if ((this.Security.HasRight(RIGHT.MODIFY_TEST_ITEMS) == false)
					    || this.Security.SiteGuid != Guid.Parse(siteGuidLabel.Text))
					{
						deleteButton.Enabled = false;
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void TestSetDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.TestSetDataGrid.EditItemIndex > -1)
				{
					return;
				}

				this.TestSetDataGrid.CurrentPageIndex = e.NewPageIndex;
				this.Session["TestSetDataGrid.CurrentPageIndex"] = e.NewPageIndex;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void TestSetDataGridSortCommand(object source, DataGridSortCommandEventArgs e)
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
					if (sortDirection == "DESC")
					{
						this.Session[SortDirection] = "ASC";
					}
					else
					{
						this.Session[SortDirection] = "DESC";
					}
				}

				this.Session[SortExpression] = e.SortExpression;
				
				TestSetCollectionClass testSetCollection =
					FMChannelHelper.MakeCall<ITestSets, TestSetCollectionClass>(
						sets =>
						sets.Enumerate(
							this.Security,
							this.Session[TestsetFindString] as string,
							this.Session[SortExpression] as string + " " + this.Session[SortDirection]));

				this.Session["TestSetCollection"] = testSetCollection;

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private ICollection Enumerate()
		{
			// Enumerate 
			TestSetCollectionClass testSetCollection =
				FMChannelHelper.MakeCall<ITestSets, TestSetCollectionClass>(
					sets =>
					sets.Enumerate(
						this.Security,
						this.Session[TestsetFindString] as string,
						this.Session[SortExpression] as string + " " + this.Session[SortDirection]));

			this.Session["TestSetCollection"] = testSetCollection;

			var mapDataTable = new DataTable();

			mapDataTable.Columns.Add("SiteGuid", typeof(string));
			mapDataTable.Columns.Add("Index", typeof(Int32)); // index into TestCollection, not TestClass Index
			mapDataTable.Columns.Add("TestSetName", typeof(string));

			for (int iItem = 0; iItem < testSetCollection.Count; iItem++)
			{
				DataRow mapDataRow = mapDataTable.NewRow();

				var testSet = testSetCollection[iItem];
				mapDataRow["SiteGuid"] = testSet.SiteGuid.ToString();
				mapDataRow["Index"] = iItem;
				mapDataRow["TestSetName"] = testSet.ID;

				mapDataTable.Rows.Add(mapDataRow);
			}

			var testSetDataView = new DataView(mapDataTable);
			return testSetDataView;
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
			this.TestSetDataGrid.DataSource = data;
			this.TestSetDataGrid.DataBind();
		}

		#endregion
	}
}