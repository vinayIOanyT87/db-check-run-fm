// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestSetResultGeneralPage.ascx.cs" company="Varec, Inc.">
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
	using System.Globalization;
	using System.Linq;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;
    using FMCore;

	public partial class TestSetResultGeneralPage : TestSetResultPageBase
	{
		#region Constants and Fields

		public const int SampleNumMultiplier = 100000;

		public DateTimeFormatInfo DateFormat = DateTimeFormatInfo.CurrentInfo;

        protected int GridviewColumnIndex = 0;
        protected int TestindexGridviewColumnIndex = 1;
        protected int TestnameGridviewColumnIndex = 2;
        protected int ResultGridviewColumnIndex = 3;
        protected int StatusGridviewColumnIndex = 4;
        protected int RangeGridviewColumnIndex = 5;
        protected int TestPerformedDateColumnIndex = 6;
        protected int PerformedByColumnIndex = 7;
        protected int SupervisorNameColumnIndex = 8;

        protected const string TestResultDataTableGuidColumn = "IdentityGuid";
        protected const string TestResultDataTableCollindexColumn = "CollIndex";
        protected const string TestResultDataTableTestColumn = "Test";
        protected const string TestResultDataTableResultColumn = "Result";
        protected const string TestResultDataTableStatusColumn = "Status";
        protected const string TestResultDataTableRangeColumn = "Passing Range";
        protected const string TestResultDataTableTestdateColumn = "Test Date";
        protected const string TestResultDataTablePerformedbyColumn = "Performed By";
        protected const string TestResultDataTableSupervisorColumn = "Supervisor";

		private const string LocalTestsetFilterAsset = "TestSetFilterAssetLocal";
		private const string LocalTestsetFilterAssetType = "TestSetFilterAssetTypeLocal";
		private const string LocalTestsetFilterResult = "TestSetFilterResultLocal";
		private const string LocalTestsetFilterStartTime = "TestSetFilterStartTimeLocal";
		private const string LocalTestsetFilterStopTime = "TestSetFilterStopTimeLocal";
		private const string LocalTestsetFilterTestset = "TestSetFilterTestSetLocal";

		//private const int MAXIMUM_NUMBER_COLUMNS = 8;

		protected const int NumberGridControls = 1; // this grid only has an edit control so this is set at on1

        private const string TestsetFilterAsset = "TestSetFilterAsset";
        private const string TestsetFilterAssetType = "TestSetFilterAssetType";
        private const string TestsetFilterResult = "TestSetFilterResult";
        private const string TestsetFilterStartTime = "TestSetFilterStartTime";
        private const string TestsetFilterStopTime = "TestSetFilterStopTime";
        private const string TestsetFilterTestset = "TestSetFilterTestSet";

		//private const int TEST_PERFORMED_DATE = 5;

		private string assetType = string.Empty;
		private string testSetFilterAsset = string.Empty;
		private string testSetFilterAssetType = string.Empty;
		private string testSetFilterResult = string.Empty;
		private string testSetFilterStartTime = string.Empty;
		private string testSetFilterStopTime = string.Empty;
		private string testSetFilterTestSet = string.Empty;

		#endregion

		#region Public Methods and Operators

		public string[] EnumerateOperatorNames()
		{
			var personnelarray = new string[this.OperatorDropDownList.Items.Count];

			for (int iLoop = 0; iLoop < this.OperatorDropDownList.Items.Count; iLoop++)
			{
				personnelarray[iLoop] = this.OperatorDropDownList.Items[iLoop].Text;
			}

			return personnelarray;
		}

		public string[] EnumerateSupervisorNames()
		{
			var personnelarray = new string[this.SupervisorDropDownList.Items.Count];

			for (int iLoop = 0; iLoop < this.SupervisorDropDownList.Items.Count; iLoop++)
			{
				personnelarray[iLoop] = this.SupervisorDropDownList.Items[iLoop].Text;
			}

			return personnelarray;
		}

		public void ResetMainSummaryFilters()
		{
			// reset the main filter if we came from the summary display
			if (this.Session[LocalTestsetFilterStartTime] != null && this.Session[LocalTestsetFilterStopTime] != null
			    && this.Session[LocalTestsetFilterAssetType] != null && this.Session[LocalTestsetFilterAsset] != null
			    && this.Session[LocalTestsetFilterTestset] != null && this.Session[LocalTestsetFilterResult] != null)
			{
				this.testSetFilterStartTime = (string)this.Session[LocalTestsetFilterStartTime];
				this.testSetFilterStopTime = (string)this.Session[LocalTestsetFilterStopTime];
				this.testSetFilterAssetType = (string)this.Session[LocalTestsetFilterAssetType];
				this.testSetFilterAsset = (string)this.Session[LocalTestsetFilterAsset];
				this.testSetFilterTestSet = (string)this.Session[LocalTestsetFilterTestset];
				this.testSetFilterResult = (string)this.Session[LocalTestsetFilterResult];

				this.Session.Add(TestsetFilterStartTime, this.testSetFilterStartTime);
				this.Session.Add(TestsetFilterStopTime, this.testSetFilterStopTime);
				this.Session.Add(TestsetFilterAssetType, this.testSetFilterAssetType);
				this.Session.Add(TestsetFilterAsset, this.testSetFilterAsset);
				this.Session.Add(TestsetFilterTestset, this.testSetFilterTestSet);
				this.Session.Add(TestsetFilterResult, this.testSetFilterResult);
			}
		}

		virtual public bool UpdateData()
		{
			// Validate required fields. This is needed after bug #7490.
			if (string.Empty == this.AssetDropDownList.SelectedValue)
			{
				this.AssetDropDownList.Focus();
				string message = this.GetTranslatedText("The following field is required:") + " " + this.GetTranslatedText("Asset")
				                 + "!";
				var ex = new ApplicationException(message);
				this.HandleFieldError(ex);
				return false;
			}

			if (string.Empty == this.TestSetDropDownList.SelectedItem.Text)
			{
				this.TestSetDropDownList.Focus();
				string message = this.GetTranslatedText("The following field is required:") + " "
				                 + this.GetTranslatedText("Test Set") + "!";
				var ex = new ApplicationException(message);
				this.HandleFieldError(ex);
				return false;
			}

			if (string.Empty == this.OperatorDropDownList.SelectedItem.Text)
			{
				this.OperatorDropDownList.Focus();
				string message = this.GetTranslatedText("The following field is required:") + " "
				                 + this.GetTranslatedText("Operator") + "!";
				var ex = new ApplicationException(message);
				this.HandleFieldError(ex);
				return false;
			}

			if (string.Empty == QuantityRepTextbox.Text)
			{
				this.QuantityRepTextbox.Focus();
				string message = this.GetTranslatedText("The following field is required:") + " " 
								+ this.GetTranslatedText("Quantity Represented") + "!";
				var ex = new ApplicationException(message);
				this.HandleFieldError(ex);
				return false;
			}

			if (false == this.ValidSampleNumber())
			{
				return false;
			}

			// only require previous sample if the IsRetestCheckBox is checked. This fixes bug #7824. (IGO 2009-Oct-08)
			if (this.IsRetestCheckBox.Checked)
			{
				// previous sample number cannot be empty
				if (string.Empty == this.PreviousSampleTextbox.Text)
				{
					this.PreviousSampleTextbox.Focus();
					string message = this.GetTranslatedText("The following field is required:") + " "
					                 + this.GetTranslatedText("Previous Sample Number") + "!";
					var ex = new ApplicationException(message);
					this.HandleFieldError(ex);
					return false;
				}

				// previous sample number must be an integer
				int previoussamplenumber;
				if (false == int.TryParse(this.PreviousSampleTextbox.Text, out previoussamplenumber))
				{
					this.PreviousSampleTextbox.Focus();
					string message = this.GetTranslatedText("Previous Sample Number must be an integer value") + "!";
					var ex = new ApplicationException(message);
					this.HandleFieldError(ex);
					return false;
				}
			}

			// sample size must be a double
			double samplesize;
			if (false
			    == double.TryParse(
				    (string.Empty == this.SampleSizeTextbox.Text) ? "0" : this.SampleSizeTextbox.Text, out samplesize))
			{
				this.SampleSizeTextbox.Focus();
				string message = this.GetTranslatedText("Sample Size must be numeric") + "!";
				var ex = new ApplicationException(message);
				this.HandleFieldError(ex);
				return false;
			}

			// gallons represented must be a double
			double gallonsRepresented;
			if (false
			    == double.TryParse(
				    (string.Empty == this.QuantityRepTextbox.Text) ? "0" : this.QuantityRepTextbox.Text, out gallonsRepresented))
			{
				this.QuantityRepTextbox.Focus();
				string message = this.GetTranslatedText("Quantity Represented must be numeric") + "!";
				var ex = new ApplicationException(message);
				this.HandleFieldError(ex);
				return false;
			}

			// gallons represented must be positive
			if (gallonsRepresented <= 0)
			{
				QuantityRepTextbox.Focus();
				string message = GetTranslatedText("Quantity Represented must be greater than zero") + "!";
				var ex = new ApplicationException(message);
				HandleFieldError(ex);
				return false;
			}

			if (this.AssetTypeDropDownList.SelectedItem.Text == this.GetTranslatedText("Tank"))
			{
				this.Session[TestResults.TestsetResultAssetType] = this.AssetTypeDropDownList.SelectedItem.Text;
				this.TestSetTankResult.TestSetName = this.TestSetDropDownList.SelectedItem.Text;
				this.TestSetTankResult.TankGuid =
					Guid.Parse(
						(string.Empty == this.AssetDropDownList.SelectedValue)
							? Guid.Empty.ToString()
							: this.AssetDropDownList.SelectedValue);
				this.TestSetTankResult.TankID = this.AssetDropDownList.SelectedItem.Text;
				this.TestSetTankResult.ResultTimeStamp = this.TestDate.CurrentValue;
				this.TestSetTankResult.Inspector = this.OperatorDropDownList.SelectedItem.Text;
				this.TestSetTankResult.Supervisor = this.SupervisorDropDownList.SelectedItem.Text;
				this.TestSetTankResult.SampleSize =
					Convert.ToDouble((string.Empty == this.SampleSizeTextbox.Text) ? "0" : this.SampleSizeTextbox.Text);
				this.TestSetTankResult.SampleNumber =
					Convert.ToInt32((string.Empty == this.SampleNumberTextbox.Text) ? "0" : this.SampleNumberTextbox.Text);
				this.TestSetTankResult.GallonsRepresented = gallonsRepresented;
				this.TestSetTankResult.IsRetest = this.IsRetestCheckBox.Checked;
				this.TestSetTankResult.PreviousSampleNumber =
					Convert.ToInt32((string.Empty == this.PreviousSampleTextbox.Text) ? "0" : this.PreviousSampleTextbox.Text);
				this.TestSetTankResult.Memo = this.MemoTextBox.Text;

				switch (this.StatusLabelBox.Text)
				{
					case "Pending":
						this.TestSetTankResult.Status = TESTSET_STATUS.Pending;
						break;
					case "Passed":
						this.TestSetTankResult.Status = TESTSET_STATUS.Passed;
						break;
					default:
						this.TestSetTankResult.Status = TESTSET_STATUS.Failed;
						break;
				}

				// Now add all result from test result grid
				this.TestSetTankResult.TestTankResultCollection.Clear();
				var testResultDataTable = (DataTable)this.Session["TestResultDataTable"];
				foreach (DataRow row in testResultDataTable.Rows)
				{
					var testTankResult = new TestTankResultClass();

					string testname = row[TestResultDataTableTestColumn].ToString();
					string testresult = row[TestResultDataTableResultColumn].ToString();
					string rowstatus = row[TestResultDataTableStatusColumn].ToString();
					string cellDateTime = row[TestResultDataTableTestdateColumn].ToString();
					string preparedByString = row[TestResultDataTablePerformedbyColumn].ToString();
					string supervisorString = row[TestResultDataTableSupervisorColumn].ToString();

					testTankResult.TestName = testname;
					testTankResult.Measurement = testresult;
					testTankResult.TestDate = DateTimeOffset.Parse(cellDateTime);
					testTankResult.PerformedBy = preparedByString;
					testTankResult.Supervisor = supervisorString;

					switch (rowstatus)
					{
						case "Pending":
							testTankResult.Status = TESTSET_STATUS.Pending;
							break;
						case "Passed":
							testTankResult.Status = TESTSET_STATUS.Passed;
							break;
						default:
							testTankResult.Status = TESTSET_STATUS.Failed;
							break;
					}

					// add new test set tank result to the collection
					this.TestSetTankResult.TestTankResultCollection.Add(testTankResult);
				}
			}
			else if (this.AssetTypeDropDownList.SelectedItem.Text == this.GetTranslatedText("Equipment"))
			{
				this.Session[TestResults.TestsetResultAssetType] = this.AssetTypeDropDownList.SelectedItem.Text;
				this.TestSetEquipmentResult.TestSetName = this.TestSetDropDownList.SelectedItem.Text;
				this.TestSetEquipmentResult.EquipmentGuid =
					Guid.Parse(
						(string.Empty == this.AssetDropDownList.SelectedValue)
							? Guid.Empty.ToString()
							: this.AssetDropDownList.SelectedValue);
				this.TestSetEquipmentResult.EquipmentID = this.AssetDropDownList.SelectedItem.Text;
				this.TestSetEquipmentResult.ResultTimeStamp = this.TestDate.CurrentValue;
				this.TestSetEquipmentResult.Inspector = this.OperatorDropDownList.SelectedItem.Text;
				this.TestSetEquipmentResult.Supervisor = this.SupervisorDropDownList.SelectedItem.Text;
				this.TestSetEquipmentResult.SampleSize =
					Convert.ToDouble((string.Empty == this.SampleSizeTextbox.Text) ? "0" : this.SampleSizeTextbox.Text);
				this.TestSetEquipmentResult.SampleNumber =
					Convert.ToInt32((string.Empty == this.SampleNumberTextbox.Text) ? "0" : this.SampleNumberTextbox.Text);
				this.TestSetEquipmentResult.GallonsRepresented = gallonsRepresented;
				this.TestSetEquipmentResult.IsRetest = this.IsRetestCheckBox.Checked;
				this.TestSetEquipmentResult.PreviousSampleNumber =
					Convert.ToInt32((string.Empty == this.PreviousSampleTextbox.Text) ? "0" : this.PreviousSampleTextbox.Text);
				this.TestSetEquipmentResult.Memo = this.MemoTextBox.Text;

				switch (this.StatusLabelBox.Text)
				{
					case "Pending":
						this.TestSetEquipmentResult.Status = TESTSET_STATUS.Pending;
						break;
					case "Passed":
						this.TestSetEquipmentResult.Status = TESTSET_STATUS.Passed;
						break;
					default:
						this.TestSetEquipmentResult.Status = TESTSET_STATUS.Failed;
						break;
				}

				// Now add all result from test result grid
				this.TestSetEquipmentResult.TestEquipmentResultCollection.Clear();
				var testResultDataTable = (DataTable)this.Session["TestResultDataTable"];
                int i = 0;

                foreach (DataRow row in testResultDataTable.Rows)
				{
					var testEquipmentResult = new TestEquipmentResultClass();

					string testname = row[TestResultDataTableTestColumn].ToString();
					string testresult = row[TestResultDataTableResultColumn].ToString();
					string rowstatus = row[TestResultDataTableStatusColumn].ToString();
					string cellDateTime = row[TestResultDataTableTestdateColumn].ToString();
					string preparedByString = row[TestResultDataTablePerformedbyColumn].ToString();
					string supervisorString = row[TestResultDataTableSupervisorColumn].ToString();

					testEquipmentResult.TestName = testname;
					testEquipmentResult.Measurement = testresult;
					testEquipmentResult.TestDate = DateTimeOffset.Parse(cellDateTime);
					testEquipmentResult.PerformedBy = preparedByString;
					testEquipmentResult.Supervisor = supervisorString;

					switch (rowstatus)
					{
						case "Pending":
							testEquipmentResult.Status = TESTSET_STATUS.Pending;
							break;
						case "Passed":
							testEquipmentResult.Status = TESTSET_STATUS.Passed;
							break;
						default:
							testEquipmentResult.Status = TESTSET_STATUS.Failed;
							break;
					}
                    row[TestResultDataTableCollindexColumn] = i++;

					// add new test set equipment result to the collection
					this.TestSetEquipmentResult.TestEquipmentResultCollection.Add(testEquipmentResult);
				}
			}

			return true;
		}

		#endregion

		#region Methods

		protected void AssetTypeDropDownListSelectedIndexChanged(object source, EventArgs e)
		{
			try
			{
				if (this.AssetTypeDropDownList.SelectedItem.Text.Equals(this.GetTranslatedText("Equipment")))
				{
					this.TestSetEquipmentResult = new TestSetEquipmentResultClass();
				}
				else if (this.AssetTypeDropDownList.SelectedItem.Text.Equals(this.GetTranslatedText("Tank")))
				{
					this.TestSetTankResult = new TestSetTankResultClass();
				}

				this.LoadAssociatedAssetDropDownList();
			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}

		protected void HandleFieldError(Exception ex)
		{
			string message = ex.Message;
			string alertstring = "<script type=\"text/javascript\">\r\n<!--\r\nalert(\""
			                     + HttpUtility.JavaScriptStringEncode(message) + "\");\r\n-->\r\n</script>";
			ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "FieldError", alertstring, false);
		}

        virtual protected void OperatorDropDownListSelectedIndexChanged(object source, EventArgs e)
        {
            try
            {
                this.UpdateView();
            }
            catch (Exception except)
            {
                ErrorHandler(except);
            }
        }

        virtual protected void SupervisorDropDownListSelectedIndexChanged(object source, EventArgs e)
        {
            try
            {
                this.UpdateView();
            }
            catch (Exception except)
            {
                ErrorHandler(except);
            }
        }

		protected void IsRestestCheckBoxCheckChanged(object sender, EventArgs e)
		{
			// Only enable the previous sample number edit control if the IsRetestCheckBox 
			// 	 is checked. This fixed bug #7824. (IGO 2009-Oct-08)
			if (this.IsRetestCheckBox.Checked)
			{
				this.PreviousSampleLabel.Enabled = true;
				this.PreviousSampleTextbox.Enabled = true;
			}
			else
			{
				this.PreviousSampleLabel.Enabled = false;
				this.PreviousSampleTextbox.Enabled = false;
			}
		}

		protected void Page_Init(object sender, EventArgs e)
        {
            ((TestSetResultForm)this.Page).TestSetResultGeneralPage = this;
		}

		virtual protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (!this.Page.IsPostBack)
				{
					this.LoadAssetTypeDropDownList();
                    this.LoadTestSetDropDownList();
					// Set the initial focus to the asset type list. This fixes bug #7494.
					this.AssetTypeDropDownList.Focus();

					this.LoadOperatorDropDownList();
					this.LoadSupervisorDropDownList();

					SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(sites => sites.Get(this.Security, this.Security.SiteGuid, false, false, false));

					this.TestDate.Text = TimeConverter.Now(site).ToString(this.TestDate.FormatInfo);

					// Disable previous sample number. This fixes bug #7824. (IGO 2009-Oct-08)
					this.PreviousSampleLabel.Enabled = false;
					this.PreviousSampleTextbox.Enabled = false;

					this.Session.Remove(LocalTestsetFilterStartTime);
					this.Session.Remove(LocalTestsetFilterStopTime);
					this.Session.Remove(LocalTestsetFilterAssetType);
					this.Session.Remove(LocalTestsetFilterAsset);
					this.Session.Remove(LocalTestsetFilterTestset);
					this.Session.Remove(LocalTestsetFilterResult);

					// Check session for test set id to load
					if ((null != this.Session[TestResults.TestsetResultGuid])
						&& (null != this.Session[TestResults.TestsetResultAssetType]))
					{
						this.assetType = (string)this.Session[TestResults.TestsetResultAssetType];
						this.UpdateControlsForSelectedTestSetResult();

						if (string.IsNullOrEmpty(this.Request.GetQueryOrFormValue("TEST")))
						{
							this.DisableControlsOnEdit();
						}
						else
						{
							this.SetSampleNumberTextBox();
						}
					}
					else
					{
						this.LoadAssociatedAssetDropDownList();
						this.SetSampleNumberTextBox();
					}

					if (this.Session[TestsetFilterStartTime] != null)
					{
						this.testSetFilterStartTime = (string)this.Session[TestsetFilterStartTime];
						this.testSetFilterStopTime = (string)this.Session[TestsetFilterStopTime];
						this.testSetFilterAssetType = (string)this.Session[TestsetFilterAssetType];
						this.testSetFilterAsset = (string)this.Session[TestsetFilterAsset];
						this.testSetFilterTestSet = (string)this.Session[TestsetFilterTestset];
						this.testSetFilterResult = (string)this.Session[TestsetFilterResult];

						this.Session.Add(LocalTestsetFilterStartTime, this.testSetFilterStartTime);
						this.Session.Add(LocalTestsetFilterStopTime, this.testSetFilterStopTime);
						this.Session.Add(LocalTestsetFilterAssetType, this.testSetFilterAssetType);
						this.Session.Add(LocalTestsetFilterAsset, this.testSetFilterAsset);
						this.Session.Add(LocalTestsetFilterTestset, this.testSetFilterTestSet);
						this.Session.Add(LocalTestsetFilterResult, this.testSetFilterResult);

						this.Session.Remove(TestsetFilterStartTime);
						this.Session.Remove(TestsetFilterStopTime);
						this.Session.Remove(TestsetFilterAssetType);
						this.Session.Remove(TestsetFilterAsset);
						this.Session.Remove(TestsetFilterTestset);
						this.Session.Remove(TestsetFilterResult);
					}

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void TestResultsGridViewPageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			try
			{
				this.TestResultsGridView.PageIndex = e.NewPageIndex;

				// bind data to the GridView control.
				this.BindData();

				foreach (TableRow r in TestResultsGridView.Rows)
				{
					var editButton = (FMEditLinkButton)r.FindControl("EditButton");
					if (editButton != null)
					{
						editButton.Enabled = false;

						if (Security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS) || Security.HasRight(RIGHT.MODIFY_QUALITY_TESTS))
						{

							if (StatusLabelBox.Text.Equals("Pending"))
							{
								editButton.Enabled = true;
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

        protected void TestResultsGridViewRowCancelingEdit(object sender, CommandEventArgs e)
		{
			try
			{
				// reset the edit index
				this.TestResultsGridView.EditIndex = -1;

				// bind data to the GridView control.
				this.BindData();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

        //protected void TestResultsGridView_RowCommandReceived(object sender, GridViewCommandEventArgs e)
        //{
        //    try
        //    {
        //        if (e.CommandName == "Edit")
        //        {
        //        }
        //    }
        //    catch (Exception except)
        //    {
        //        this.ErrorHandler(except);
        //    }
        //}

		virtual protected void TestResultsGridViewRowDataBound(object sender, GridViewRowEventArgs e)
		{
			try
			{
				// we do this here because autocreatedcolumns do not exist as an object in the grid
				if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header
				    || e.Row.RowType == DataControlRowType.Footer)
				{
                    // Always hide "Index" column
                    e.Row.Cells[GridviewColumnIndex + NumberGridControls].Visible = false;
                    // only allow "Result" column to be editable
					if (this.TestResultsGridView.EditIndex == e.Row.RowIndex && e.Row.RowType == DataControlRowType.DataRow)
					{
						e.Row.Cells[TestnameGridviewColumnIndex + NumberGridControls].Enabled = false;
						e.Row.Cells[StatusGridviewColumnIndex + NumberGridControls].Enabled = false;
						e.Row.Cells[RangeGridviewColumnIndex + NumberGridControls].Enabled = false;
                        e.Row.Cells[ResultGridviewColumnIndex + NumberGridControls].Focus();

						var testResultDataTable = (DataTable)this.Session["TestResultDataTable"];
						if (testResultDataTable != null)
						{
							DataRow row = testResultDataTable.Rows[e.Row.RowIndex];
							if (row != null)
							{
                                string preparedByString = row[TestResultDataTablePerformedbyColumn].ToString();
                                string supervisorString = row[TestResultDataTableSupervisorColumn].ToString();

                                var cell = e.Row.Cells[PerformedByColumnIndex + NumberGridControls] as DataControlFieldCell;

								if (cell != null && !string.IsNullOrEmpty(preparedByString))
								{
									var personList = cell.Controls[1] as FMDropDownList;
									if (personList != null)
									{
										// select the entry in the drop down list
										personList.SelectedIndex = personList.Items.IndexOf(personList.Items.FindByText(preparedByString));
									}
								}

                                var supervisorcell = e.Row.Cells[SupervisorNameColumnIndex + NumberGridControls] as DataControlFieldCell;

								if (supervisorcell != null && !string.IsNullOrEmpty(supervisorString))
								{
									var supervisorList = supervisorcell.Controls[1] as FMDropDownList;
									if (supervisorList != null)
									{
										// select the entry in the drop down list
										supervisorList.SelectedIndex = supervisorList.Items.IndexOf(supervisorList.Items.FindByText(supervisorString));
									}
								}
							}
						}
					}
				}
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var editButton = (FMEditLinkButton)e.Row.FindControl("EditButton");
                    if (editButton != null)
                    {
                        editButton.CommandArgument = e.Row.RowIndex.ToString(CultureInfo.InvariantCulture);

						editButton.Enabled = false;

						if (Security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS) || Security.HasRight(RIGHT.MODIFY_QUALITY_TESTS))
						{
							if (StatusLabelBox.Text.Equals("Pending"))
							{
								editButton.Enabled = true;
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

        protected virtual void TestResultsGridViewRowEditing(object sender, CommandEventArgs e)
		{
			try
			{
				// set the edit index.
                this.TestResultsGridView.EditIndex = Convert.ToInt32(e.CommandArgument); 

				// bind data to the GridView control.
				this.BindData();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		virtual protected void TestResultsGridViewRowUpdating(object sender, GridViewUpdateEventArgs e)
		{
			try
			{
				// retrieve the table from the session object.
				var dt = (DataTable)this.Session["TestResultDataTable"];

				// update data table associated with the gridview
				GridViewRow row = this.TestResultsGridView.Rows[e.RowIndex];

				string testname = ((Label)row.Cells[TestnameGridviewColumnIndex + NumberGridControls].Controls[1]).Text;
				string measurement = ((TextBox)row.Cells[ResultGridviewColumnIndex + NumberGridControls].Controls[1]).Text;
                string cellDateTime = ((FMDateTime)row.Cells[TestPerformedDateColumnIndex + NumberGridControls].Controls[1]).Text;
				string preparedByString = ((FMDropDownList)row.Cells[PerformedByColumnIndex + NumberGridControls].Controls[1]).SelectedItem.Text;
				string supervisorString = ((FMDropDownList)row.Cells[SupervisorNameColumnIndex + NumberGridControls].Controls[1]).SelectedItem.Text;

				string status = this.UpdateTestStatus(testname, measurement).ToString();

				dt.Rows[row.DataItemIndex][TestResultDataTableResultColumn] = measurement;
				dt.Rows[row.DataItemIndex][TestResultDataTableStatusColumn] = status;
				dt.Rows[row.DataItemIndex][TestResultDataTableTestdateColumn] = DateTimeOffset.Parse(cellDateTime);
				dt.Rows[row.DataItemIndex][TestResultDataTablePerformedbyColumn] = preparedByString;
				dt.Rows[row.DataItemIndex][TestResultDataTableSupervisorColumn] = supervisorString;

				// update test set status
				this.UpdateTestSetStatus(dt);

				// reset the edit index
				this.TestResultsGridView.EditIndex = -1;

				// bind data to the GridView control.
				this.BindData();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		virtual protected void TestSetDropDownListSelectedIndexChanged(object source, EventArgs e)
		{
			try
			{
				if (this.IsEquipmentAssetType())
				{
					this.TestSetEquipmentResult.TestSetName = this.TestSetDropDownList.SelectedItem.Text;
				}
				else if (this.IsTankAssetType())
				{
					this.TestSetTankResult.TestSetName = this.TestSetDropDownList.SelectedItem.Text;
				}

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void BindData()
		{
			this.TestResultsGridView.DataSource = this.Session["TestResultDataTable"];
			this.TestResultsGridView.DataBind();
		}

	    protected virtual void DisableControlsOnEdit()
	    {
	        // Disable all controls except test set grid when editing a pending result
	        this.AssetTypeLabel.Enabled = false;
	        this.AssetTypeDropDownList.Enabled = false;
	        this.AssetLabel.Enabled = false;
	        this.AssetDropDownList.Enabled = false;
	        this.TestSetLabel.Enabled = false;
	        this.TestSetDropDownList.Enabled = false;
	        this.TestDateLabel.Enabled = false;
	        this.TestDate.Enabled = false;
	        this.OperatorLabel.Enabled = false;
	        this.OperatorDropDownList.Enabled = false;
	        this.SupervisorLabel.Enabled = false;
	        this.SupervisorDropDownList.Enabled = false;
	        this.SampleSizeLabel.Enabled = false;
	        this.SampleSizeTextbox.Enabled = false;
	        this.SampleNumberLabel.Enabled = false;
	        this.SampleNumberTextbox.Enabled = false;
	        this.QuantityRepLabel.Enabled = false;
	        this.QuantityRepTextbox.Enabled = false;
	        this.IsRetestCheckBox.Enabled = false;
	        this.PreviousSampleLabel.Enabled = false;
	        this.PreviousSampleTextbox.Enabled = false;
	        this.MemoLabel.Enabled = false;
	        this.MemoTextBox.Enabled = false;
	        this.TestResultsLabel.Enabled = true;
	        this.TestResultsGridView.Enabled = true;
	        this.OkButton.Enabled = false;

	        if (this.StatusLabelBox.Text.Equals("Pending"))
	        {
	            if (this.Security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS) || this.Security.HasRight(RIGHT.MODIFY_QUALITY_TESTS))
	            {
	                this.MemoLabel.Enabled = true;
	                this.MemoTextBox.Enabled = true;
	                this.OkButton.Enabled = true;
	            }
	        }
	    }

	    virtual protected ICollection EnumerateTestsFromTestSet()
		{
			var testResultDataTable = new DataTable { TableName = "testResultDataTable" };

			DataRow testResultDataRow;
			float testsetsamplesize = 0;
			const string CnIdentityGuid = "IdentityGuid";
            const string CnTest = TestResultDataTableTestColumn;
			const string CnResult = TestResultDataTableResultColumn;
            const string CnStatus = TestResultDataTableStatusColumn;
            const string CnPassingRange = TestResultDataTableRangeColumn;
            const string CnTestDate = TestResultDataTableTestdateColumn;
            const string CnPerformedBy = TestResultDataTablePerformedbyColumn;
            const string CnSupervisor = TestResultDataTableSupervisorColumn;

            testResultDataTable.Columns.Add(CnIdentityGuid, typeof(Guid));
            testResultDataTable.Columns.Add(TestResultDataTableCollindexColumn, typeof(Int32));
            testResultDataTable.Columns.Add(CnTest, typeof(string));
            testResultDataTable.Columns.Add(CnResult, typeof(string));
            testResultDataTable.Columns.Add(CnStatus, typeof(string));
            testResultDataTable.Columns.Add(CnPassingRange, typeof(string));
            testResultDataTable.Columns.Add(CnTestDate, typeof(DateTimeOffset));
            testResultDataTable.Columns.Add(CnPerformedBy, typeof(string));
            testResultDataTable.Columns.Add(CnSupervisor, typeof(string));

			int i = 0;
            if (this.AssetTypeDropDownList.SelectedItem.Text == this.GetTranslatedText("Tank"))
			{
				// load tests from test set result if the selected guid is non empty
				if (Guid.Empty != this.TestSetTankResult.TestSetTankResultGuid)
				{
					var testResultCollection =
						FMChannelHelper.MakeCall<ITestTankResults, TestTankResultCollectionClass>(
							x => x.EnumerateByTestSetTankResultGuid(this.Security, this.TestSetTankResult.TestSetTankResultGuid));

					FMChannelHelper.MakeCall<ITests>(
						tests =>
							{
								foreach (TestTankResultClass testResult in testResultCollection)
								{
									Guid testGuid = tests.GetIdentityGuid(this.Security, testResult.TestName);
									TestClass test = tests.Get(this.Security, testGuid);

									testResultDataRow = testResultDataTable.NewRow();

									testResultDataRow[CnIdentityGuid] = testResult.IdentityGuid;
                                    testResultDataRow[TestResultDataTableCollindexColumn] = i++;
                                    testResultDataRow[CnTest] = testResult.TestName;
									testResultDataRow[CnResult] = testResult.Measurement;
									testResultDataRow[CnStatus] = testResult.Status;
									testResultDataRow[CnPassingRange] = test.ValidationRule;
									testResultDataRow[CnTestDate] = testResult.TestDate;
									testResultDataRow[CnPerformedBy] = testResult.PerformedBy;
									testResultDataRow[CnSupervisor] = testResult.Supervisor;

									// Calculate the total sample size from suming all test in the testset. This fixes bug #7499.
									testsetsamplesize += test.SampleSize;

									testResultDataTable.Rows.Add(testResultDataRow);
								}
							});
				}
				else
				{
					// Load tests from the actual test set
					// Don't enumerate assets if test drop down list is empty. This addresses task #7490.
					if (string.Empty != this.TestSetDropDownList.SelectedValue)
					{
						Guid testSetGuid = Guid.Parse(this.TestSetDropDownList.SelectedValue);
						
						TestSetClass testSet = FMChannelHelper.MakeCall<ITestSets, TestSetClass>(
							sets => sets.GetByIncludeTests(this.Security, testSetGuid, true));


						foreach (TestClass test in testSet.testCollection)
						{
							testResultDataRow = testResultDataTable.NewRow();

							testResultDataRow[CnIdentityGuid] = Guid.Empty;
                            testResultDataRow[TestResultDataTableCollindexColumn] = i++;
                            testResultDataRow[CnTest] = test.ID;
							testResultDataRow[CnResult] = string.Empty;
							testResultDataRow[CnStatus] = TESTSET_STATUS.Pending.ToString();
							testResultDataRow[CnPassingRange] = test.ValidationRule;
							testResultDataRow[CnTestDate] = this.TestDate.CurrentValue;
							testResultDataRow[CnPerformedBy] = this.OperatorDropDownList.SelectedItem.Text;
							testResultDataRow[CnSupervisor] = this.SupervisorDropDownList.SelectedItem.Text;

							// Calculate the total sample size from suming all test in the testset. This fixes bug #7499.
							testsetsamplesize += test.SampleSize;

							testResultDataTable.Rows.Add(testResultDataRow);
						}
					}
				}
			}
			else if (this.AssetTypeDropDownList.SelectedItem.Text == this.GetTranslatedText("Equipment"))
			{
				// load tests from test set result if the selected guid is non empty
				if (Guid.Empty != this.TestSetEquipmentResult.TestSetEquipmentResultGuid)
				{
					var testResultCollection = FMChannelHelper.MakeCall<ITestEquipmentResults, TestEquipmentResultCollectionClass>(
						x =>
						x.EnumerateByTestSetEquipmentResultGuid(this.Security, this.TestSetEquipmentResult.TestSetEquipmentResultGuid));
                    var results = from testResult in testResultCollection
                                    orderby testResult.TestName
                                    select testResult;
                                 
					FMChannelHelper.MakeCall<ITests>(
						tests =>
							{
 								foreach (var testResult in results )
								{
									Guid testGuid = tests.GetIdentityGuid(this.Security, testResult.TestName);
									TestClass test = tests.Get(this.Security, testGuid);

									testResultDataRow = testResultDataTable.NewRow();

									testResultDataRow[CnIdentityGuid] = testResult.IdentityGuid;
                                    testResultDataRow[TestResultDataTableCollindexColumn] = i++;
                                    testResultDataRow[CnTest] = testResult.TestName;
									testResultDataRow[CnResult] = testResult.Measurement;
									testResultDataRow[CnStatus] = testResult.Status;
									testResultDataRow[CnPassingRange] = test.ValidationRule;
									testResultDataRow[CnTestDate] = testResult.TestDate;
									testResultDataRow[CnPerformedBy] = testResult.PerformedBy;
									testResultDataRow[CnSupervisor] = testResult.Supervisor;

									// Calculate the total sample size from suming all test in the testset. This fixes bug #7499.
									testsetsamplesize += test.SampleSize;

									testResultDataTable.Rows.Add(testResultDataRow);
								}
							});
				}
				else
				{
					// Load tests from the actual test set
					// Don't enumerate assets if test drop down list is empty. This addresses task #7490.
					if (string.Empty != this.TestSetDropDownList.SelectedValue)
					{
						Guid testSetGuid = Guid.Parse(this.TestSetDropDownList.SelectedValue);
						
						TestSetClass testSet = FMChannelHelper.MakeCall<ITestSets, TestSetClass>(
							sets => sets.GetByIncludeTests(this.Security, testSetGuid, true));

						foreach (TestClass test in testSet.testCollection)
						{
							testResultDataRow = testResultDataTable.NewRow();

							testResultDataRow[CnIdentityGuid] = Guid.Empty;
                            testResultDataRow[TestResultDataTableCollindexColumn] = i++;
                            testResultDataRow[CnTest] = test.ID;
							testResultDataRow[CnResult] = string.Empty;
							testResultDataRow[CnStatus] = TESTSET_STATUS.Pending.ToString();
							testResultDataRow[CnPassingRange] = test.ValidationRule;
							testResultDataRow[CnTestDate] = this.TestDate.CurrentValue;
							testResultDataRow[CnPerformedBy] = this.OperatorDropDownList.SelectedItem.Text;
							testResultDataRow[CnSupervisor] = this.SupervisorDropDownList.SelectedItem.Text;

							// Calculate the total sample size from suming all test in the testset. This fixes bug #7499.
							testsetsamplesize += test.SampleSize;

							testResultDataTable.Rows.Add(testResultDataRow);
						}
					}
				}
			}

			// Set the SampleSizeTextbox control with the newly calculated sample size. This fixes bug #7499.
			this.SampleSizeTextbox.Text = testsetsamplesize.ToString(CultureInfo.InvariantCulture);

			// update test set status
			this.UpdateTestSetStatus(testResultDataTable);

			this.Session["TestResultDataTable"] = testResultDataTable;

			return new DataView(testResultDataTable);
		}

		/// <summary>
		/// Given a previous sample number, this function will generate a new sample
		///     number based on a set of rules defined in the specification.
		/// </summary>
		/// <param name="previousSampleNumber">
		/// Previous testset result sample number.
		/// </param>
		/// <param name="previousDate">
		/// Previous testset result date.
		/// </param>
		/// <returns>
		/// An int containing the newly generated sample number.
		/// </returns>
		private int GetNextSampleNumber(int previousSampleNumber, DateTimeOffset previousDate)
		{
			int samplenumber = previousSampleNumber;

			DateTimeOffset currentdate = DateTimeOffset.Now;
			int year = currentdate.Year;

			year = year % 100;

			// If current year is greater than the year of the previous date, reset 
			// the sample number to the default value
			if ((currentdate.Year > previousDate.Year) && ((previousSampleNumber / SampleNumMultiplier) != year))
			{
				samplenumber = (year * SampleNumMultiplier) + 1;
			}
			else
			{
				samplenumber++;
			}

			// ensure that the numbers are not the same
			if (samplenumber <= previousSampleNumber)
			{
				samplenumber = previousSampleNumber + 1;
			}

			return samplenumber;
		}

		private bool IsEquipmentAssetType()
		{
			return this.AssetTypeDropDownList.SelectedItem.Text.Equals(this.GetTranslatedText("Equipment"));
		}

		private bool IsTankAssetType()
		{
			return this.AssetTypeDropDownList.SelectedItem.Text.Equals(this.GetTranslatedText("Tank"));
		}

		private void LoadAssetTypeDropDownList()
		{
			this.AssetTypeDropDownList.Items.Clear();
			this.AssetTypeDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Equipment")));
			this.AssetTypeDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Tank")));
		}

		private void LoadAssociatedAssetDropDownList()
		{
			this.AssetDropDownList.Items.Clear();

			// Always add an empty item to the list. This fixes task #7490.
			var emptyitem = new ListItem(string.Empty, string.Empty);
			this.AssetDropDownList.Items.Add(emptyitem);
			this.AssetDropDownList.SelectedIndex = 0;

			// Load drop down list based on asset selection
			if (this.AssetTypeDropDownList.SelectedItem.Text == this.GetTranslatedText("Tank"))
			{
				var tankCollection = FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(tanks => tanks.Enumerate(this.Security));

				foreach (TankClass tank in tankCollection)
				{
					var li = new ListItem(tank.ID, tank.IdentityGuid.ToString());
					this.AssetDropDownList.Items.Add(li);
				}
			}
			else if (this.AssetTypeDropDownList.SelectedItem.Text == this.GetTranslatedText("Equipment"))
			{
				var equipmentCollection =
					FMChannelHelper.MakeCall<IEquipments, EquipmentCollectionClass>(x => x.EnumerateManagedEquipment(this.Security));

				foreach (EquipmentClass equipment in equipmentCollection)
				{
					var li = new ListItem(equipment.ID, equipment.MasterRecordGuid.ToString());
					this.AssetDropDownList.Items.Add(li);
				}
			}
		}

		private void LoadOperatorDropDownList()
		{
			this.OperatorDropDownList.Items.Clear();

			// Always add an empty item to the list. This fixes task #7490.
			var emptyitem = new ListItem(string.Empty, string.Empty);
			this.OperatorDropDownList.Items.Add(emptyitem);
			this.OperatorDropDownList.SelectedIndex = 0;

			var personCollection = new PersonCollectionClass();
			var loaderCollection = 	FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(
					x => x.EnumerateByRoleSortByName(this.Security, PERSON_ROLE.LOADER_ROLE));
            var offloaderCollection = FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(
                    x => x.EnumerateByRoleSortByName(this.Security, PERSON_ROLE.OFFLOADER_ROLE));

            loaderCollection.Union(offloaderCollection).ToList().ForEach(x => personCollection.Add(x));

            foreach (PersonClass person in personCollection)
			{
				// Display the operators last name, first name. This fixes bug #7489.
				var li = new ListItem(person.LastName + ", " + person.FirstName, person.MasterRecordGuid.ToString());
				this.OperatorDropDownList.Items.Add(li);
			}
		}

		private void LoadSupervisorDropDownList()
		{
			this.SupervisorDropDownList.Items.Clear();

			// Always add an empty item to the list. This fixes task #7490.
			var emptyitem = new ListItem(string.Empty, string.Empty);
			this.SupervisorDropDownList.Items.Add(emptyitem);
			this.SupervisorDropDownList.SelectedIndex = 0;

			var personCollection =
				FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(
					x => x.EnumerateByRoleSortByName(this.Security, PERSON_ROLE.SUPERVISOR_ROLE));

			foreach (PersonClass person in personCollection)
			{
				// Display the supervisors last name, first name. This fixes bug #7489.
				var li = new ListItem(person.LastName + ", " + person.FirstName, person.MasterRecordGuid.ToString());
				this.SupervisorDropDownList.Items.Add(li);
			}
		}

		private void LoadTestSetDropDownList()
		{
			this.TestSetDropDownList.Items.Clear();

			// Always add an empty item to the list. This fixes task #7490.
			var emptyitem = new ListItem(string.Empty, string.Empty);
			this.TestSetDropDownList.Items.Add(emptyitem);
			this.TestSetDropDownList.SelectedIndex = 0;

			var testSetCollection =
				FMChannelHelper.MakeCall<ITestSets, TestSetCollectionClass>(
					sets => sets.Enumerate(this.Security, string.Empty, "TestSetName"));

			foreach (TestSetClass testSet in testSetCollection)
			{
				var li = new ListItem(testSet.ID, testSet.IdentityGuid.ToString());
				this.TestSetDropDownList.Items.Add(li);
			}
		}

		private void SetSampleNumberTextBox()
		{
			int samplenumber = 0;

			if (this.AssetTypeDropDownList.SelectedItem.Text == this.GetTranslatedText("Tank"))
			{
				// get the previously added record if there is one
				var testsettankresult =
					FMChannelHelper.MakeCall<ITestSetTankResults, TestSetTankResultClass>(
						x => x.GetPreviousSampleNumber(this.Security));

				// no previous record found
				if (Guid.Empty == testsettankresult.SiteGuid)
				{
					DateTimeOffset currentdate = DateTimeOffset.Now;
					int year = currentdate.Year;
					year = year % 100;
					samplenumber = (year * SampleNumMultiplier) + 1;
				}
				else
				{
					samplenumber = this.GetNextSampleNumber(testsettankresult.SampleNumber, testsettankresult.ResultTimeStamp);
				}

				this.TestSetTankResult.SampleNumber = samplenumber;
			}
			else if (this.AssetTypeDropDownList.SelectedItem.Text == this.GetTranslatedText("Equipment"))
			{
				// get the previously added record if there is one
				var testsetequipmentresult =
					FMChannelHelper.MakeCall<ITestSetEquipmentResults, TestSetEquipmentResultClass>(
						x => x.GetPreviousSampleNumber(this.Security));

				// no previous record found
				if (Guid.Empty == testsetequipmentresult.SiteGuid)
				{
					DateTimeOffset currentdate = DateTimeOffset.Now;
					int year = currentdate.Year;
					year = year % 100;
					samplenumber = (year * SampleNumMultiplier) + 1;
				}
				else
				{
					samplenumber = this.GetNextSampleNumber(
						testsetequipmentresult.SampleNumber, testsetequipmentresult.ResultTimeStamp);
				}

				this.TestSetEquipmentResult.SampleNumber = samplenumber;
			}

			this.SampleNumberTextbox.Text = samplenumber.ToString(CultureInfo.InvariantCulture);
		}

		private void UpdateControlsForSelectedTestSetResult()
		{
			if (this.assetType == this.GetTranslatedText("Tank"))
			{
				this.AssetTypeDropDownList.SelectedValue = this.GetTranslatedText("Tank");
				this.LoadAssociatedAssetDropDownList();
				this.AssetDropDownList.SelectedValue = this.TestSetTankResult.TankGuid.ToString();
				this.TestSetDropDownList.SelectedIndex =
					this.TestSetDropDownList.Items.IndexOf(
						this.TestSetDropDownList.Items.FindByText(this.TestSetTankResult.TestSetName));
				this.TestDate.Text = this.TestSetTankResult.ResultTimeStamp.ToString();
				this.OperatorDropDownList.SelectedIndex =
					this.OperatorDropDownList.Items.IndexOf(
						this.OperatorDropDownList.Items.FindByText(this.TestSetTankResult.Inspector));
				this.SupervisorDropDownList.SelectedIndex =
					this.SupervisorDropDownList.Items.IndexOf(
						this.SupervisorDropDownList.Items.FindByText(this.TestSetTankResult.Supervisor));
				this.SampleSizeTextbox.Text = this.TestSetTankResult.SampleSize.ToString(CultureInfo.InvariantCulture);
				this.SampleNumberTextbox.Text = this.TestSetTankResult.SampleNumber.ToString(CultureInfo.InvariantCulture);
				this.QuantityRepTextbox.Text = this.TestSetTankResult.GallonsRepresented.ToString(CultureInfo.InvariantCulture);
				this.IsRetestCheckBox.Checked = this.TestSetTankResult.IsRetest;
				this.PreviousSampleTextbox.Text = (0 == this.TestSetTankResult.PreviousSampleNumber)
					                                  ? string.Empty
					                                  : this.TestSetTankResult.PreviousSampleNumber.ToString(CultureInfo.InvariantCulture);
				this.MemoTextBox.Text = this.TestSetTankResult.Memo;
				this.SetStatusLabelBox(this.TestSetTankResult.Status);
			}
			else if (this.assetType == this.GetTranslatedText("Equipment"))
			{
				this.AssetTypeDropDownList.SelectedValue = this.GetTranslatedText("Equipment");
				this.LoadAssociatedAssetDropDownList();
				this.AssetDropDownList.SelectedValue = this.TestSetEquipmentResult.EquipmentGuid.ToString();
				if (this.AssetDropDownList.SelectedItem.Text != this.TestSetEquipmentResult.EquipmentID)
				{
					// Equipment must have been deleted. Add equipment ID to dropdown list, 
					// select it, and disable the OK button. User is allowed only to view.
					this.AssetDropDownList.Items.Add(new ListItem(this.TestSetEquipmentResult.EquipmentID, "-1"));
					this.AssetDropDownList.SelectByText(this.TestSetEquipmentResult.EquipmentID);
					((TestSetResultForm)this.Page).OK.Enabled = false;
				}

				this.TestSetDropDownList.SelectedIndex =
					this.TestSetDropDownList.Items.IndexOf(
						this.TestSetDropDownList.Items.FindByText(this.TestSetEquipmentResult.TestSetName));
				this.TestDate.Text = this.TestSetEquipmentResult.ResultTimeStamp.ToString(this.TestDate.FormatInfo);
				this.OperatorDropDownList.SelectedIndex =
					this.OperatorDropDownList.Items.IndexOf(
						this.OperatorDropDownList.Items.FindByText(this.TestSetEquipmentResult.Inspector));
				this.SupervisorDropDownList.SelectedIndex =
					this.SupervisorDropDownList.Items.IndexOf(
						this.SupervisorDropDownList.Items.FindByText(this.TestSetEquipmentResult.Supervisor));
				this.SampleSizeTextbox.Text = this.TestSetEquipmentResult.SampleSize.ToString(CultureInfo.InvariantCulture);
				this.SampleNumberTextbox.Text = this.TestSetEquipmentResult.SampleNumber.ToString(CultureInfo.InvariantCulture);
				this.QuantityRepTextbox.Text = this.TestSetEquipmentResult.GallonsRepresented.ToString(CultureInfo.InvariantCulture);
				this.IsRetestCheckBox.Checked = this.TestSetEquipmentResult.IsRetest;
				this.PreviousSampleTextbox.Text = (0 == this.TestSetEquipmentResult.PreviousSampleNumber)
					                                  ? string.Empty
					                                  : this.TestSetEquipmentResult.PreviousSampleNumber.ToString(CultureInfo.InvariantCulture);
				this.MemoTextBox.Text = this.TestSetEquipmentResult.Memo;
				this.SetStatusLabelBox(TestSetEquipmentResult.Status);
			}
		}

		virtual protected void UpdateTestSetStatus(DataTable testResultDataTable)
		{
			var status = TESTSET_STATUS.Passed;

			// set status to pending if no rows in testset table
			if (0 == testResultDataTable.Rows.Count)
			{
				status = TESTSET_STATUS.Pending;
			}
			else
			{
				foreach (DataRow row in testResultDataTable.Rows)
				{
					string rowstatus = row[TestResultDataTableStatusColumn].ToString();
					if ("Pending" == rowstatus)
					{
						status = TESTSET_STATUS.Pending;
					}
					else if ("Failed" == rowstatus)
					{
						if (TESTSET_STATUS.Passed == status)
						{
							status = TESTSET_STATUS.Failed;
						}
					}
				}
			}

            SetStatusLabelBox(status);
        }

        // TFS 35799 2013-Sep-24  Paul Carpenter: Passed should be in a green text/box Failed should be in a Red text/box
        private void SetStatusLabelBox(TESTSET_STATUS status)
        {
            this.StatusLabelBox.Text = status.ToString();
            switch (status)
            {
                case TESTSET_STATUS.Failed:
                    this.StatusLabelBox.BackColor = FMColor.RedBackground;
                    break;
                case TESTSET_STATUS.Passed:
                    this.StatusLabelBox.BackColor = FMColor.GreenBackground;
                    break;
                case TESTSET_STATUS.Pending:
                    this.StatusLabelBox.BackColor = System.Drawing.Color.Transparent;
                    break;
            }

        }

		private TESTSET_STATUS UpdateTestStatus(string testName, string measurement)
		{
			// set status to pending if no measurement entered 
			if (string.Empty == measurement)
			{
				return TESTSET_STATUS.Pending;
			}

			var status = TESTSET_STATUS.Failed;
			bool validresult = false;

			FMChannelHelper.MakeCall<ITests>(
				tests =>
					{
						Guid testGuid = tests.GetIdentityGuid(this.Security, testName);
						TestClass test = tests.Get(this.Security, testGuid);
						validresult = tests.ValidateTestResult(this.Security, test, measurement);
					});

			if (validresult)
			{
				status = TESTSET_STATUS.Passed;
			}

			return status;
		}

		virtual protected void UpdateView()
		{
			ICollection testResultsFromTestSet = this.EnumerateTestsFromTestSet();

			// Create an empty table to bind to, next bind to the actual table.
			// This fixes the update problem when the grid begins with an empty dataset.
			var emptyDataTable = new DataTable();
			var emptyDataView = new DataView(emptyDataTable);
			this.TestResultsGridView.DataSource = emptyDataView;
			this.TestResultsGridView.DataBind();

			this.TestResultsGridView.DataSource = testResultsFromTestSet;
			this.TestResultsGridView.DataBind();

            foreach (TableRow r in TestResultsGridView.Rows)
            {
                var editButton = (FMEditLinkButton)r.FindControl("EditButton");
                if (editButton != null)
                {
                    editButton.Enabled = false;

                    if (Security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS) || Security.HasRight(RIGHT.MODIFY_QUALITY_TESTS))
                    {
                        if (StatusLabelBox.Text.Equals("Pending"))
                        {
                            editButton.Enabled = true;
                        }
                    }
                }
            }
		}

		private bool ValidSampleNumber()
		{
			// sample number cannot be empty
			if (string.Empty == this.SampleNumberTextbox.Text)
			{
				this.SampleNumberTextbox.Focus();
				string message = this.GetTranslatedText("The following field is required:") + " "
				                 + this.GetTranslatedText("Sample Number") + "!";
				var ex = new ApplicationException(message);
				this.HandleFieldError(ex);
				return false;
			}

			// sample number must be an integer
			int samplenumber;
			if (false == int.TryParse(this.SampleNumberTextbox.Text, out samplenumber))
			{
				this.SampleNumberTextbox.Focus();
				string message = this.GetTranslatedText("Sample Number must be an integer value") + "!";
				var ex = new ApplicationException(message);
				this.HandleFieldError(ex);
				return false;
			}

			// sample number must be unique			
			if (this.AssetTypeDropDownList.SelectedItem.Text == this.GetTranslatedText("Tank"))
			{
				var result =
					FMChannelHelper.MakeCall<ITestSetTankResults, bool>(
						// ReSharper disable once AccessToModifiedClosure
						x => x.FindDuplicateSampleNumber(this.Security, samplenumber, this.TestSetTankResult.TestSetTankResultGuid));

				if (result)
				{

					TestSetTankResultClass testsettankresult = FMChannelHelper.MakeCall<ITestSetTankResults, TestSetTankResultClass>(x => x.GetPreviousSampleNumber(Security));
					samplenumber = GetNextSampleNumber(testsettankresult.SampleNumber,
													   testsettankresult.ResultTimeStamp);
					testsettankresult.SampleNumber = samplenumber;
					SampleNumberTextbox.Text = samplenumber.ToString(CultureInfo.InvariantCulture);

					this.SampleNumberTextbox.Focus();
					string message = this.GetTranslatedText("Sample Number must be unique") + "!";
					var ex = new ApplicationException(message);
					this.HandleFieldError(ex);
					return false;
				}
			}
			else if (this.AssetTypeDropDownList.SelectedItem.Text == this.GetTranslatedText("Equipment"))
			{
				var result =
					FMChannelHelper.MakeCall<ITestSetEquipmentResults, bool>(
						x =>
							// ReSharper disable once AccessToModifiedClosure
						x.FindDuplicateSampleNumber(this.Security, samplenumber, this.TestSetEquipmentResult.TestSetEquipmentResultGuid));

				if (result)
				{

					TestSetEquipmentResultClass testSetEquipmentResult = FMChannelHelper.MakeCall<ITestSetEquipmentResults, TestSetEquipmentResultClass>(x => x.GetPreviousSampleNumber(Security));
					samplenumber = GetNextSampleNumber(testSetEquipmentResult.SampleNumber,
													   testSetEquipmentResult.ResultTimeStamp);
					testSetEquipmentResult.SampleNumber = samplenumber;
					SampleNumberTextbox.Text = samplenumber.ToString(CultureInfo.InvariantCulture); //TODO:check this

					this.SampleNumberTextbox.Focus();
					string message = this.GetTranslatedText("Sample Number must be unique") + "!";
					var ex = new ApplicationException(message);
					this.HandleFieldError(ex);
					return false;
				}
			}

			return true;
		}

		#endregion
	}
}