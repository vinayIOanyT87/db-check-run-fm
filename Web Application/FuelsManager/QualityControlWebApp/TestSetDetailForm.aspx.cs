// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestSetDetailForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.QualityControlWebApp
{
	using System;
	using System.Data;
	using System.Diagnostics;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

    using FMCore;

	using FMWebApp;

	/// <summary>
    /// The Test Set Detail Configuration form.
    /// </summary>
	public partial class TestSetDetailForm : FMAutoSubmitFormBase
	{
		#region Methods

		protected void AddButtonClick(object sender, EventArgs e)
		{
			try
			{
				var test = new TestClass { SiteGuid = this.Security.SiteGuid };
				var testSet = this.Session["TestSet"] as TestSetClass;

				if (testSet != null)
				{
					testSet.testCollection.Add(test);

					this.TestsDataGrid.CurrentPageIndex = (testSet.testCollection.Count - 1) / this.TestsDataGrid.PageSize;
					this.TestsDataGrid.EditItemIndex = (testSet.testCollection.Count - 1) % this.TestsDataGrid.PageSize;

					this.TestsDataGridPageSizeDropDown.Enabled = false;
					this.EnableControls(false);
					this.UpdateView();

					// Check to see if there are no tests left to assign to the test set. The test drop down is populated 
					// after a new test set is added to the grid so we have to cancel the edit if there are no tests left to assign
					if (this.TestsDataGrid != null && this.TestsDataGrid.Items.Count > 0
					    && this.TestsDataGrid.EditItemIndex < this.TestsDataGrid.Items.Count)
					{
						var dropdownlist =
							this.TestsDataGrid.Items[this.TestsDataGrid.EditItemIndex].FindControl("TestNameDropDownList") as DropDownList;

						// The dropdownlist.SelectedItem can be null if there are no tests left to assign to the test set
						if (dropdownlist == null || dropdownlist.SelectedItem == null)
						{
							this.ErrorHandler(new ApplicationException("All existing tests have already been assigned to this test set."));

							// Remove the test set we added since the are no tests left to assign
							testSet.testCollection.RemoveAt(testSet.testCollection.Count - 1);

							if (this.TestsDataGrid.CurrentPageIndex > 0 && this.TestsDataGrid.EditItemIndex == 0)
							{
								this.TestsDataGrid.CurrentPageIndex--;
							}

							// Cancel the edit and update the view
							this.TestsDataGrid.EditItemIndex = -1;
							this.TestsDataGridPageSizeDropDown.Enabled = true;
							this.EnableControls(true);
							this.UpdateView();
						}
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void CancelClick(object sender, EventArgs e)
		{
			try
			{
				this.Redirect("TestSetsForm.aspx");
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void OkClick(object sender, EventArgs e)
		{
			try
			{
				if (this.CommitData())
				{
					this.Redirect("TestSetsForm.aspx");
				}
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
					throw new ApplicationException( "Access denied." );
				}

				if (!this.Page.IsPostBack)
				{
					TestSetClass testSet;

					if (string.IsNullOrEmpty(this.Request.GetQueryOrFormValue("IdentityGuid")))
					{
						testSet = new TestSetClass();
					}
					else
					{
						Guid identityGuid = Guid.Parse(this.Request.GetQueryOrFormValue("IdentityGuid"));
						testSet = FMChannelHelper.MakeCall<ITestSets, TestSetClass>(
								sets => sets.GetByIncludeTests(this.Security, identityGuid, true));

						if (!this.Security.HasRight(RIGHT.MODIFY_TEST_ITEMS)
						    || (this.Security.SiteGuid != testSet.SiteGuid && testSet.SiteGuid != Guid.Empty))
						{
							this.EnableControls(false);
						}
					}

					this.Session["TestSet"] = testSet;

					// Set the title label with a key field from the bound object appended
					if (testSet != null)
					{
						this.TestSetTitleLabel.Text = this.GetTitleLabelText(this.TestSetTitleLabel.Text, testSet.ID);
					}

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void TestsDataGridCancelCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// get the grid row index
				var indexLabel = e.Item.FindControl("IndexLabel") as Label;

				// we need to determine if this is a new entry in the test assignment grid, 
				// and if it is, the correct behavior when cancel is pressed is to remove it from the grid
				if (indexLabel != null)
				{
					int testIndex = Convert.ToInt32(indexLabel.Text);

					var testSet = this.Session["TestSet"] as TestSetClass;

					if (testSet != null && testSet.testCollection.Count > testIndex)
					{
						TestClass test = testSet.testCollection[testIndex];

						// if the test has no identity guid, then it's new. The identity guid is set when the item is saved/updated
						if (test.IdentityGuid == Guid.Empty)
						{
							testSet.testCollection.RemoveAt(testIndex);
						}
					}
				}

				this.TestsDataGrid.EditItemIndex = -1;
                this.TestsDataGridPageSizeDropDown.Enabled = true;
				this.EnableControls(true);
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
				var indexLabel = e.Item.FindControl("IndexLabel") as Label;
				if (indexLabel != null)
				{
					int inx = Convert.ToInt32(indexLabel.Text); // grid row index
					var testSet = this.Session["TestSet"] as TestSetClass;
					if (testSet != null)
					{
						testSet.testCollection.RemoveAt(inx);
					}
				}

				this.TestsDataGrid.EditItemIndex = -1;
                this.TestsDataGridPageSizeDropDown.Enabled = true;
				this.EnableControls(true);

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void TestsDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Don't allow editing of one item while another is being edited
				if (this.TestsDataGrid.EditItemIndex < 0)
				{
					var label = this.TestsDataGrid.Items[e.Item.ItemIndex].FindControl("TestName") as Label;

					this.EnableControls(false);
                    this.TestsDataGridPageSizeDropDown.Enabled = false;
					this.TestsDataGrid.EditItemIndex = e.Item.ItemIndex;

					this.UpdateView();

					var dropdownlist = this.TestsDataGrid.Items[this.TestsDataGrid.EditItemIndex].FindControl("TestNameDropDownList") as DropDownList;
					if (dropdownlist != null && label != null)
					{
						foreach (ListItem listItem in dropdownlist.Items)
						{
							if (listItem.Text == label.Text)
							{
								listItem.Selected = true;
								break;
							}
						}
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void TestsDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			try
			{
				var dropDownList = e.Item.FindControl("TestNameDropDownList") as DropDownList;
				var indexLabel = e.Item.FindControl("IndexLabel") as Label;

				var testSet = this.Session["TestSet"] as TestSetClass;

				if (testSet == null)
				{
					throw new ApplicationException( "The Test Set associated with this page was not found in Session." );
				}

				if (dropDownList != null && indexLabel != null)
				{
					TestCollectionClass testCollection =
						FMChannelHelper.MakeCall<ITests, TestCollectionClass>(tests => tests.Enumerate(this.Security, null, null));

					foreach (TestClass test in testCollection)
					{
						if (DLAEnergyCheckBox.Checked && string.IsNullOrEmpty(test.TestCode))
						{
							continue;
						}

						int index = Convert.ToInt32(indexLabel.Text); // grid row index

						// is this test already assigned to the test set? if so, don't add it to the drop down list
						if (!this.FindDuplicate(testSet, test.IdentityGuid, index))
						{
							var listItem = new ListItem(test.ID, test.IdentityGuid.ToString());
							dropDownList.Items.Add(listItem);
						}
					}
				}

				// If the user does not have the right to modify test sets do not allow them to edit or delete tests from the list.
				// You can also not modify tests associated with the the test set if the test set has been assigned down from another site.
				bool hasModifyRights = this.Security.HasRight(RIGHT.MODIFY_TEST_ITEMS) && (testSet.SiteGuid == this.Security.SiteGuid || testSet.SiteGuid == Guid.Empty);

				var deleteButton = e.Item.FindControl("DeleteButton") as LinkButton;

				if (deleteButton != null)
				{				
				    if (this.TestsDataGrid.EditItemIndex != -1 && this.TestsDataGrid.EditItemIndex != e.Item.ItemIndex)
				    {
                        // If a row in the grid is being edited, disable the delete button for all other items in the grid
                        deleteButton.Enabled = false;
				    }
				    else
				    {
                        deleteButton.Enabled = hasModifyRights;
				    }
				}

				var editButton = e.Item.FindControl("EditButton") as LinkButton;

				if (editButton != null)
				{
					editButton.Enabled = hasModifyRights;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void TestsDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.TestsDataGrid.EditItemIndex > -1)
				{
					return;
				}

				this.TestsDataGrid.CurrentPageIndex = e.NewPageIndex;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void TestsDataGridUpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				this.UpdateTestsDataGridRow(e.Item);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private bool CommitData()
		{
			try
			{
				if (this.TestSetNameTextbox.Text == string.Empty)
				{
					var except = new ApplicationException( "Test Name is a required field." );
					this.ErrorHandler(except);
					return false;
				}

				var testSet = this.Session["TestSet"] as TestSetClass;
				if (testSet != null)
				{
					testSet.ID = this.TestSetNameTextbox.Text;

					if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescEnterpriseKey()))
					{
						testSet.Flag01 = this.DLAEnergyCheckBox.Checked;
					}

					if (testSet.IdentityGuid == Guid.Empty)
					{
						testSet.SiteGuid = this.Security.SiteGuid;
						FMChannelHelper.MakeCall<ITestSets>(sets => sets.Add(this.Security, testSet));
					}
					else
					{
						FMChannelHelper.MakeCall<ITestSets>(sets => sets.Modify(this.Security, testSet));
					}
				}

				return true;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return false;
			}
		}

		/// <summary>
		/// Enable or disable controls on the form
		/// </summary>
		/// <param name="enable">
		/// True to enable, false to disable
		/// </param>
		private void EnableControls(bool enable)
		{
			this.AddButton.Enabled = enable;
			this.AddButton2.Enabled = enable;
			this.OK.Enabled = enable;
		}

		private DataView EnumerateTests()
		{
			var mapDataTable = new DataTable();
			var testSet = this.Session["TestSet"] as TestSetClass;

			mapDataTable.Columns.Add("Index", typeof(Int32)); // index of grid row
			mapDataTable.Columns.Add("SiteGuid", typeof(string));
			mapDataTable.Columns.Add("TestName", typeof(string));

			if (testSet != null)
			{
				for (int nextItem = 0; nextItem < testSet.testCollection.Count; nextItem++)
				{
					DataRow mapDataRow = mapDataTable.NewRow();
					TestClass test = testSet.testCollection[nextItem];

					mapDataRow["Index"] = nextItem;
					mapDataRow["SiteGuid"] = test.SiteGuid.ToString();
					mapDataRow["TestName"] = test.ID;

					mapDataTable.Rows.Add(mapDataRow);
				}
			}

			var testsDataView = new DataView(mapDataTable);
			return testsDataView;
		}

		/// <summary>
		/// Search the existing tests assigned to a TestSet for one with a duplicate identityGuid.
		///     The data grid index of the test you are looking for duplicates of is required so
		///     that it does not count as a duplicate of itself
		/// </summary>
		/// <param name="testSet">
		/// a test set containing a test collection to search
		/// </param>
		/// <param name="identityGuid">
		/// The identity guid of the test to search on, to see if it is already assigned to another test
		/// </param>
		/// <param name="testDataGridIndex">
		/// the data grid index of the test we are searching for, so it doesn't get counted as a duplicate of itself
		/// </param>
		/// <returns>
		/// True if a duplicate is found. False otherwise.
		/// </returns>
		private bool FindDuplicate(TestSetClass testSet, Guid identityGuid, int testDataGridIndex)
		{
			if (testSet != null && testSet.testCollection != null)
			{
				for (int i = 0; i < testSet.testCollection.Count; ++i)
				{
					TestClass existingTest = testSet.testCollection[i];

					// if the identity guids are the same, and the index in the test collection is not the same, 
					// then we have a duplicate
					if (existingTest.IdentityGuid == identityGuid && testDataGridIndex != i)
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		private void UpdateTestsDataGridRow(DataGridItem dgItem)
		{
			var dropdownlist =
				this.TestsDataGrid.Items[this.TestsDataGrid.EditItemIndex].FindControl("TestNameDropDownList") as DropDownList;

			bool duplicateDetected = false;

			var testSet = this.Session["TestSet"] as TestSetClass;

			if (testSet != null && testSet.testCollection != null)
			{
				var label = dgItem.FindControl("IndexLabel") as Label;

				if (label != null)
				{
					int testInx = Convert.ToInt32(label.Text); // grid row index
					TestClass test = testSet.testCollection[testInx];

					// get the guid of the test that we're assigning to the test set
					// and see if it is already assigned to the test set
					Debug.Assert(dropdownlist != null, "dropdownlist != null");
					Guid assignedTestIdentityGuid = Guid.Parse(dropdownlist.SelectedItem.Value);
					duplicateDetected = this.FindDuplicate(testSet, assignedTestIdentityGuid, testInx);

					// if the test is not a duplicate, set the identity guid and ID
					if (!duplicateDetected)
					{
						test.IdentityGuid = assignedTestIdentityGuid;
						test.ID = dropdownlist.SelectedItem.Text;
					}
				}
			}

			// if we did not find a duplicate, enable the add / ok controls and update the view
			// otherwise, return an error to the user
			if (!duplicateDetected)
			{
                this.TestsDataGridPageSizeDropDown.Enabled = true;
				this.EnableControls(true);
				this.TestsDataGrid.EditItemIndex = -1;
				this.UpdateView();
			}
			else
			{
				this.ErrorHandler( new ApplicationException( "The test selected is already assigned to this test set" ) );
			}
		}

		private void UpdateView()
		{
			var testSet = this.Session["TestSet"] as TestSetClass;
			try
			{
				if (!this.IsPostBack && testSet != null)
				{
					this.TestSetNameTextbox.Text = testSet.ID;
					this.DLAEnergyCheckBox.Visible = false;

					if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescEnterpriseKey()))
					{
						this.DLAEnergyCheckBox.Visible = true;
						this.DLAEnergyCheckBox.Checked = testSet.IdentityGuid == Guid.Empty || testSet.Flag01;
					}
				}

				DataView dataCollection = this.EnumerateTests();

				if (this.TestsDataGridPageSizeDropDown != null)
				{
					this.TestsDataGridPageSizeDropDown.SetPageSize(this.TestsDataGrid, dataCollection.Count);
				}

				this.TestsDataGrid.DataSource = dataCollection;
				this.TestsDataGrid.DataBind();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		#endregion
	}
}