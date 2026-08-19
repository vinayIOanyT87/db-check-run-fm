namespace FuelsManager.QualityControlWebApp
{
	using System;
	using System.Data;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.LogClient;
	using FMBusinessObjects.UtilityObjects;

	using FMWebApp;

	public partial class TestResults : FMFormBase
	{
		#region Constants and Fields

		public const string TestsetResultAssetType = "SelectedTestSetResultAssetType";
		public const string TestsetResultGuid = "SelectedTestSetResultGuid";
		public const string TestSetResultsObject = "TestingResultsGridView.Object";

		public DateTimeFormatInfo DateFormat = DateTimeFormatInfo.CurrentInfo;

		private const int ResultIndexGridviewColumnIndex = 1;
		private const string SortDirection = "TestingResultsGridView.SortDirection";
		private const string SortExpression = "TestingResultsGridView.SortExpression";
		private const int TestedAssetTypeGridviewColumnIndex = 5;
		private const string TestingResultsGridviewPageIndex = "TestingResultsGridView.PageIndex";
		private const string TestSetFilterAsset = "TestSetFilterAsset";
		private const string TestSetFilterAssetType = "TestSetFilterAssetType";
		private const string TestsetFilterResult = "TestSetFilterResult";
		private const string TestsetFilterStartTime = "TestSetFilterStartTime";
		private const string TestsetFilterStopTime = "TestSetFilterStopTime";
		private const string TestsetFilterTestset = "TestSetFilterTestSet";
		private const string TestingDateHiddenColName = "HiddenTestingDate";
		private string testingDateVisibleColName = "";

		#endregion

		#region Methods

		protected void AddCommand(object sender, CommandEventArgs e)
		{
			try
			{
				this.Session.Remove(TestsetResultGuid);
				this.Session.Remove(TestsetResultAssetType);

				this.Session.Add(TestSetResultsObject, new TestSetEquipmentResultClass());
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			this.Redirect(this.TestSetResultFormUrl);
		}

		protected void AssetTypeDropDownListSelectedIndexChanged(object source, EventArgs e)
		{
			try
			{
				// reload the associated asset drop down list
				this.LoadAssociatedAssetDropDownList();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.InitializeComponents();
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (this.IsPostBack == false)
				{
					this.Session[SortExpression] = "Sample Number";
					this.Session[SortDirection] = "ASC";
				}
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
				if (!this.Page.IsPostBack)
				{
					SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(sites => sites.Get(this.Security, this.Security.SiteGuid, false, false, false));

					DateTimeOffset siteTimeNow = TimeConverter.Now(site);

					if (this.Session[TestsetFilterStartTime] != null)
					{
						var stStartTime = this.Session[TestsetFilterStartTime] as string;
						DateTimeOffset startTime = DateTimeOffset.Parse(stStartTime);
						this.ToDate.CurrentValue = startTime;
					}
					else
					{
						this.ToDate.CurrentValue = siteTimeNow;
					}

					if (this.Session[TestsetFilterStopTime] != null)
					{
						var stStopTime = this.Session[TestsetFilterStopTime] as string;
						DateTimeOffset stopTime = DateTimeOffset.Parse(stStopTime);
						this.FromDate.CurrentValue = stopTime;
					}
					else
					{
						this.FromDate.CurrentValue = siteTimeNow.AddDays(-7);
					}

					this.LoadAssetTypeDropDownList();
					this.LoadAssociatedAssetDropDownList();
					this.LoadTestSetDropDownList();
					this.LoadResultDropDownList();

					// Disable the the Add button if the user doesn't have the correct rights. This fixes bug #6599.
					if (!this.Security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS) && !this.Security.HasRight(RIGHT.MODIFY_QUALITY_TESTS))
					{
						this.AddTopButton.Enabled = false;
					}

					if (null != this.Session[TestingResultsGridviewPageIndex])
					{
						this.TestingResultsGridView.PageIndex = (int)this.Session[TestingResultsGridviewPageIndex];
					}

					this.Session.Remove(TestsetFilterStartTime);
					this.Session.Remove(TestsetFilterStopTime);
					this.Session.Remove(TestSetFilterAssetType);
					this.Session.Remove(TestSetFilterAsset);
					this.Session.Remove(TestsetFilterTestset);
					this.Session.Remove(TestsetFilterResult);

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void RefreshCommand(object sender, CommandEventArgs e)
		{
			try
			{
				this.ValidateControls();

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void TestingResultsGridViewPageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			try
			{
				this.TestingResultsGridView.PageIndex = e.NewPageIndex;
				this.Session[TestingResultsGridviewPageIndex] = e.NewPageIndex;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void TestingResultsGridViewRowCommandReceived(object sender, GridViewCommandEventArgs e)
		{
			string redirectString = string.Empty;

			try
			{
				if (e.CommandName.Equals("Edit"))
				{
					var index = Convert.ToInt32(e.CommandArgument);
					GridViewRow row = this.TestingResultsGridView.Rows[index];

					// add the new selected asset type
					TableCell assetTypeCell = row.Cells[TestedAssetTypeGridviewColumnIndex + 1];
					this.Session[TestsetResultAssetType] = assetTypeCell.Text;

					// add the selected test result guid to session
					TableCell identityGuidCell = row.Cells[ResultIndexGridviewColumnIndex + 1];
					this.Session[TestsetResultGuid] = Guid.Parse(identityGuidCell.Text);

					this.Session.Remove(TestSetResultsObject);

					if (assetTypeCell.Text == this.GetTranslatedText("Tank"))
					{
						this.Session.Add(
							TestSetResultsObject, 
							FMChannelHelper.MakeCall<ITestSetTankResults, TestSetTankResultClass>(
								results => results.Get(this.Security, Guid.Parse(identityGuidCell.Text))));
					}
					else if (assetTypeCell.Text == this.GetTranslatedText("Equipment"))
					{
						this.Session.Add(
							TestSetResultsObject, 
							FMChannelHelper.MakeCall<ITestSetEquipmentResults, TestSetEquipmentResultClass>(
								results => results.Get(this.Security, Guid.Parse(identityGuidCell.Text))));
					}

					// store the filters so they can be restored when the user returns
					this.Session.Add(TestsetFilterStartTime, this.ToDate.CurrentValue.ToString());
					this.Session.Add(TestsetFilterStopTime, this.FromDate.CurrentValue.ToString());
					this.Session.Add(TestSetFilterAssetType, this.AssetTypeDropDownList.SelectedItem.Text);
					this.Session.Add(TestSetFilterAsset, this.AssetDropDownList.SelectedItem.Text);
					this.Session.Add(TestsetFilterTestset, this.TestSetDropDownList.SelectedItem.Text);
					this.Session.Add(TestsetFilterResult, this.ResultDropDownList.SelectedItem.Text);

					redirectString = this.TestSetResultFormUrl;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			// Do the redirect outside a try/catch or we get a "thread was being aborted" exception
			if (string.IsNullOrEmpty(redirectString) == false)
			{
				this.Redirect(redirectString);
			}
		}

		protected void TestingResultsGridViewRowDataBound(object sender, GridViewRowEventArgs e)
		{
			try
			{
				// we do this here because autocreatedcolumns do not exist as an object in the grid
				if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header
				    || e.Row.RowType == DataControlRowType.Footer)
				{
					// always hide the result index column. This fixes bug #7537.
					if (e.Row.Cells.Count > 1)
					{
						e.Row.Cells[ResultIndexGridviewColumnIndex + 1].Visible = false;
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private DataView EnumerateTestSetResults()
		{
			var testingResultsDataTable = new DataTable();
			DataRow testingResultsDataRow;

			this.testingDateVisibleColName = this.GetTranslatedText("Testing Date");
			const string CnTestingDate = TestingDateHiddenColName;

			string dictionaryPassed = this.GetTranslatedText("Passed");
			string dictionaryFailed = this.GetTranslatedText("Failed");
			string dictionaryPending = this.GetTranslatedText("Pending");
			string cnResultID = this.GetTranslatedText("Result ID");
			
			string cnTestSet = this.GetTranslatedText("Test Set");

			string cnTestedAsset = this.GetTranslatedText("Tested Asset");
			string cnTestedAssetType = this.GetTranslatedText("Tested Asset Type");
			string cnSampleNumber = this.GetTranslatedText("Sample Number");
			string cnStatus = this.GetTranslatedText("Status");
			string cnIsRetest = this.GetTranslatedText("Is Retest");
			string cnPreviousSample = this.GetTranslatedText("Previous Sample");
			string cnMemo = this.GetTranslatedText("Memo");

			testingResultsDataTable.Columns.Add(CnTestingDate, typeof(DateTime));
			testingResultsDataTable.Columns.Add(cnResultID, typeof(Guid));
			testingResultsDataTable.Columns.Add(this.testingDateVisibleColName, typeof(String), CnTestingDate);
			testingResultsDataTable.Columns.Add(cnTestSet, typeof(string));
			testingResultsDataTable.Columns.Add(cnTestedAsset, typeof(string));
			testingResultsDataTable.Columns.Add(cnTestedAssetType, typeof(string));
			testingResultsDataTable.Columns.Add(cnSampleNumber, typeof(Int32));
			testingResultsDataTable.Columns.Add(cnStatus, typeof(string));
			testingResultsDataTable.Columns.Add(cnIsRetest, typeof(bool));
			testingResultsDataTable.Columns.Add(cnPreviousSample, typeof(Int32));
			testingResultsDataTable.Columns.Add(cnMemo, typeof(string));
			

			DateTimeOffset fromDate = TimeConverter.ToStartOfDay(this.FromDate.CurrentValue);
			DateTimeOffset toDate = TimeConverter.ToEndOfDay(this.ToDate.CurrentValue);

			if (this.AssetTypeDropDownList.SelectedItem.Text == this.GetTranslatedText("Tank"))
			{
				TestSetTankResultCollectionClass testSetResultCollection;

				// Enumerate all tanks if the asset drop down list is empty
				Guid assetGuid =
					Guid.Parse(
						(string.Empty == this.AssetDropDownList.SelectedValue)
							? Guid.Empty.ToString()
							: this.AssetDropDownList.SelectedValue);
				if (Guid.Empty == assetGuid)
				{
					testSetResultCollection =
						FMChannelHelper.MakeCall<ITestSetTankResults, TestSetTankResultCollectionClass>(
							tResults => tResults.EnumerateByDates(this.Security, fromDate, toDate));
				}
				else
				{
					// otherwise enumerate results by the asset guid
					testSetResultCollection =
						FMChannelHelper.MakeCall<ITestSetTankResults, TestSetTankResultCollectionClass>(
							tResults => tResults.EnumerateByTankGuid(this.Security, assetGuid));
				}

				foreach (TestSetTankResultClass testSetResult in testSetResultCollection)
				{
					// first filter the results by date
					if (string.Empty != this.FromDate.Text && string.Empty == this.ToDate.Text)
					{
						if (testSetResult.ResultTimeStamp < fromDate)
						{
							continue;
						}
					}
					else if (string.Empty != this.FromDate.Text && string.Empty != this.ToDate.Text)
					{
						if (testSetResult.ResultTimeStamp < fromDate || testSetResult.ResultTimeStamp > toDate)
						{
							continue;
						}
					}

					// next filter the results by test set
					if (string.Empty != this.TestSetDropDownList.SelectedItem.Text)
					{
						if (this.TestSetDropDownList.SelectedItem.Text != testSetResult.TestSetName)
						{
							continue;
						}
					}

					// next filter the results by result
					if (dictionaryPassed == this.ResultDropDownList.SelectedValue)
					{
						if (dictionaryPassed != testSetResult.Status.ToString())
						{
							continue;
						}
					}
					else if (dictionaryFailed == this.ResultDropDownList.SelectedValue)
					{
						if (dictionaryFailed != testSetResult.Status.ToString())
						{
							continue;
						}
					}
					else if (dictionaryPending == this.ResultDropDownList.SelectedValue)
					{
						if (dictionaryPending != testSetResult.Status.ToString())
						{
							continue;
						}
					}

					testingResultsDataRow = testingResultsDataTable.NewRow();

					testingResultsDataRow[cnResultID] = testSetResult.TestSetTankResultGuid;
					testingResultsDataRow[cnSampleNumber] = testSetResult.SampleNumber;
					testingResultsDataRow[cnTestSet] = testSetResult.TestSetName;
					if (testSetResult.ResultTimeStamp.DateTime != null)
						testingResultsDataRow[CnTestingDate] = testSetResult.ResultTimeStamp.DateTime;
					testingResultsDataRow[cnStatus] = testSetResult.Status.ToString();
					testingResultsDataRow[cnTestedAssetType] = this.GetTranslatedText("Tank");
					testingResultsDataRow[cnTestedAsset] = testSetResult.TankID;
					testingResultsDataRow[cnMemo] = testSetResult.Memo;
					testingResultsDataRow[cnIsRetest] = testSetResult.IsRetest;
					testingResultsDataRow[cnPreviousSample] = testSetResult.PreviousSampleNumber;

					testingResultsDataTable.Rows.Add(testingResultsDataRow);
				}
			}
			else if (this.AssetTypeDropDownList.SelectedItem.Text == this.GetTranslatedText("Equipment"))
			{
				TestSetEquipmentResultCollectionClass testSetResultCollection;

				var timer = new StopWatch(StopWatch.Appnames.Accounting, "Quality- Enumerate Tests");

				// Enumerate all equipment if the asset drop down list is empty
				Guid assetGuid =
					Guid.Parse(
						(string.Empty == this.AssetDropDownList.SelectedValue)
							? Guid.Empty.ToString()
							: this.AssetDropDownList.SelectedValue);
				if (Guid.Empty == assetGuid)
				{
					testSetResultCollection =
						FMChannelHelper.MakeCall<ITestSetEquipmentResults, TestSetEquipmentResultCollectionClass>(
							eResults => eResults.Enumerate(this.Security, fromDate, toDate));
				}
				else
				{
					// otherwise enumerate results by the asset guid
					testSetResultCollection =
						FMChannelHelper.MakeCall<ITestSetEquipmentResults, TestSetEquipmentResultCollectionClass>(
							eResults => eResults.EnumerateByEquipmentGuid(this.Security, assetGuid));
				}

				timer.Stop();

				foreach (TestSetEquipmentResultClass testSetResult in testSetResultCollection)
				{
					// first filter the results by date
					if (string.Empty != this.FromDate.Text && string.Empty == this.ToDate.Text)
					{
						if (testSetResult.ResultTimeStamp < fromDate)
						{
							continue;
						}
					}
					else if (string.Empty != this.FromDate.Text && string.Empty != this.ToDate.Text)
					{
						if (testSetResult.ResultTimeStamp < fromDate || testSetResult.ResultTimeStamp > toDate)
						{
							continue;
						}
					}

					// next filter the results by test set
					if (string.Empty != this.TestSetDropDownList.SelectedItem.Text)
					{
						if (this.TestSetDropDownList.SelectedItem.Text != testSetResult.TestSetName)
						{
							continue;
						}
					}

					// next filter the results by result
					if (dictionaryPassed == this.ResultDropDownList.SelectedValue)
					{
						if (dictionaryPassed != testSetResult.Status.ToString())
						{
							continue;
						}
					}
					else if (dictionaryFailed == this.ResultDropDownList.SelectedValue)
					{
						if (dictionaryFailed != testSetResult.Status.ToString())
						{
							continue;
						}
					}
					else if (dictionaryPending == this.ResultDropDownList.SelectedValue)
					{
						if (dictionaryPending != testSetResult.Status.ToString())
						{
							continue;
						}
					}

					testingResultsDataRow = testingResultsDataTable.NewRow();

					testingResultsDataRow[cnResultID] = testSetResult.TestSetEquipmentResultGuid;
					testingResultsDataRow[cnSampleNumber] = testSetResult.SampleNumber;
					testingResultsDataRow[cnTestSet] = testSetResult.TestSetName;
					if (testSetResult.ResultTimeStamp.DateTime != null)
						testingResultsDataRow[CnTestingDate] = testSetResult.ResultTimeStamp.DateTime;
					testingResultsDataRow[cnStatus] = testSetResult.Status.ToString();
					testingResultsDataRow[cnTestedAssetType] = this.GetTranslatedText("Equipment");
					testingResultsDataRow[cnTestedAsset] = testSetResult.EquipmentID;
					testingResultsDataRow[cnMemo] = testSetResult.Memo;

					testingResultsDataRow[cnIsRetest] = testSetResult.IsRetest;
					testingResultsDataRow[cnPreviousSample] = testSetResult.PreviousSampleNumber;

					testingResultsDataTable.Rows.Add(testingResultsDataRow);
				}
			}
			else if (this.AssetTypeDropDownList.SelectedItem.Text == this.GetTranslatedText("{All}"))
			{
				// Get tanks first
				TestSetTankResultCollectionClass testSetTankResultCollection;

				// Enumerate all tanks if the asset drop down list is empty
				Guid assetGuid =
					Guid.Parse(
						(string.Empty == this.AssetDropDownList.SelectedValue)
							? Guid.Empty.ToString()
							: this.AssetDropDownList.SelectedValue);
				if (Guid.Empty == assetGuid)
				{
					testSetTankResultCollection =
						FMChannelHelper.MakeCall<ITestSetTankResults, TestSetTankResultCollectionClass>(
							results => results.EnumerateByDates(this.Security, fromDate, toDate));
				}
				else
				{
					// otherwise enumerate results by the asset guid
					testSetTankResultCollection =
						FMChannelHelper.MakeCall<ITestSetTankResults, TestSetTankResultCollectionClass>(
							results => results.EnumerateByTankGuid(this.Security, assetGuid));
				}

				foreach (TestSetTankResultClass testSetTankResult in testSetTankResultCollection)
				{
					// first filter the results by date
					if (string.Empty != this.FromDate.Text && string.Empty == this.ToDate.Text)
					{
						if (testSetTankResult.ResultTimeStamp < fromDate)
						{
							continue;
						}
					}
					else if (string.Empty != this.FromDate.Text && string.Empty != this.ToDate.Text)
					{
						if (testSetTankResult.ResultTimeStamp < fromDate || testSetTankResult.ResultTimeStamp > toDate)
						{
							continue;
						}
					}

					// next filter the results by test set
					if (string.Empty != this.TestSetDropDownList.SelectedItem.Text)
					{
						if (this.TestSetDropDownList.SelectedItem.Text != testSetTankResult.TestSetName)
						{
							continue;
						}
					}

					// next filter the results by result
					if (dictionaryPassed == this.ResultDropDownList.SelectedValue)
					{
						if (dictionaryPassed != testSetTankResult.Status.ToString())
						{
							continue;
						}
					}
					else if (dictionaryFailed == this.ResultDropDownList.SelectedValue)
					{
						if (dictionaryFailed != testSetTankResult.Status.ToString())
						{
							continue;
						}
					}
					else if (dictionaryPending == this.ResultDropDownList.SelectedValue)
					{
						if (dictionaryPending != testSetTankResult.Status.ToString())
						{
							continue;
						}
					}

					testingResultsDataRow = testingResultsDataTable.NewRow();

					testingResultsDataRow[cnResultID] = testSetTankResult.TestSetTankResultGuid;
					testingResultsDataRow[cnSampleNumber] = testSetTankResult.SampleNumber;
					testingResultsDataRow[cnTestSet] = testSetTankResult.TestSetName;
					if (testSetTankResult.ResultTimeStamp.DateTime != null)
						testingResultsDataRow[CnTestingDate] = testSetTankResult.ResultTimeStamp.DateTime;
					testingResultsDataRow[cnStatus] = testSetTankResult.Status.ToString();
					testingResultsDataRow[cnTestedAssetType] = this.GetTranslatedText("Tank");
					testingResultsDataRow[cnTestedAsset] = testSetTankResult.TankID;
					testingResultsDataRow[cnMemo] = testSetTankResult.Memo;

					testingResultsDataRow[cnIsRetest] = testSetTankResult.IsRetest;
					testingResultsDataRow[cnPreviousSample] = testSetTankResult.PreviousSampleNumber;

					testingResultsDataTable.Rows.Add(testingResultsDataRow);
				}

				// get equipment next
				TestSetEquipmentResultCollectionClass testSetEquipmentResultCollection;

				// Enumerate all equipment if the asset drop down list is empty
				if (Guid.Empty == assetGuid)
				{
					testSetEquipmentResultCollection =
						FMChannelHelper.MakeCall<ITestSetEquipmentResults, TestSetEquipmentResultCollectionClass>(
							eResults => eResults.Enumerate(this.Security, fromDate, toDate));
				}
				else
				{
					// otherwise enumerate results by the asset guid
					testSetEquipmentResultCollection =
						FMChannelHelper.MakeCall<ITestSetEquipmentResults, TestSetEquipmentResultCollectionClass>(
							eResults => eResults.EnumerateByEquipmentGuid(this.Security, assetGuid));
				}

				foreach (TestSetEquipmentResultClass testSetEquipmentResult in testSetEquipmentResultCollection)
				{
					// first filter the results by date
					if (string.Empty != this.FromDate.Text && string.Empty == this.ToDate.Text)
					{
						if (testSetEquipmentResult.ResultTimeStamp < fromDate)
						{
							continue;
						}
					}
					else if (string.Empty != this.FromDate.Text && string.Empty != this.ToDate.Text)
					{
						if (testSetEquipmentResult.ResultTimeStamp < fromDate || testSetEquipmentResult.ResultTimeStamp > toDate)
						{
							continue;
						}
					}

					// next filter the results by test set
					if (string.Empty != this.TestSetDropDownList.SelectedItem.Text)
					{
						if (this.TestSetDropDownList.SelectedItem.Text != testSetEquipmentResult.TestSetName)
						{
							continue;
						}
					}

					// next filter the results by result
					if (dictionaryPassed == this.ResultDropDownList.SelectedValue)
					{
						if (dictionaryPassed != testSetEquipmentResult.Status.ToString())
						{
							continue;
						}
					}
					else if (dictionaryFailed == this.ResultDropDownList.SelectedValue)
					{
						if (dictionaryFailed != testSetEquipmentResult.Status.ToString())
						{
							continue;
						}
					}
					else if (dictionaryPending == this.ResultDropDownList.SelectedValue)
					{
						if (dictionaryPending != testSetEquipmentResult.Status.ToString())
						{
							continue;
						}
					}

					testingResultsDataRow = testingResultsDataTable.NewRow();

					testingResultsDataRow[cnResultID] = testSetEquipmentResult.TestSetEquipmentResultGuid;
					testingResultsDataRow[cnSampleNumber] = testSetEquipmentResult.SampleNumber;
					testingResultsDataRow[cnTestSet] = testSetEquipmentResult.TestSetName;
					if (testSetEquipmentResult.ResultTimeStamp.DateTime != null)
						testingResultsDataRow[CnTestingDate] = testSetEquipmentResult.ResultTimeStamp.DateTime;
					testingResultsDataRow[cnStatus] = testSetEquipmentResult.Status.ToString();
					testingResultsDataRow[cnTestedAssetType] = this.GetTranslatedText("Equipment");
					testingResultsDataRow[cnTestedAsset] = testSetEquipmentResult.EquipmentID;
					testingResultsDataRow[cnMemo] = testSetEquipmentResult.Memo;

					testingResultsDataRow[cnIsRetest] = testSetEquipmentResult.IsRetest;
					testingResultsDataRow[cnPreviousSample] = testSetEquipmentResult.PreviousSampleNumber;

					testingResultsDataTable.Rows.Add(testingResultsDataRow);
				}
			}

			var testingResultsDataView = new DataView(testingResultsDataTable);

			if (string.IsNullOrEmpty(this.Session[SortExpression].ToString()) == false
			    && string.IsNullOrEmpty(this.Session[SortDirection].ToString()) == false)
			{
				testingResultsDataView.Sort = string.Format("{0} {1}", this.Session[SortExpression], this.Session[SortDirection]);
			}

			return testingResultsDataView;
		}

		private void InitializeComponents()
		{
			this.TestingResultsGridView.Sorting += this.TestingResultsGridViewSorting;
		}

		private void LoadAssetTypeDropDownList()
		{
			this.AssetTypeDropDownList.Items.Clear();
			this.AssetTypeDropDownList.Items.Add(new ListItem(this.GetTranslatedText("{All}")));
			this.AssetTypeDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Tank")));
			this.AssetTypeDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Equipment")));
			if (this.Session[TestSetFilterAssetType] != null)
			{
				var stAssetType = this.Session[TestSetFilterAssetType] as string;
				if (stAssetType != null)
				{
					this.AssetTypeDropDownList.SelectedIndex =
						this.AssetTypeDropDownList.Items.IndexOf(this.AssetTypeDropDownList.Items.FindByText(stAssetType));
				}
			}
			else
			{
				this.AssetTypeDropDownList.SelectedIndex =
					this.AssetTypeDropDownList.Items.IndexOf(
						this.AssetTypeDropDownList.Items.FindByText(this.GetTranslatedText("Equipment")));
			}
		}

		private void LoadAssociatedAssetDropDownList()
		{
			this.AssetDropDownList.Items.Clear();

			// Always add an empty item to the list
			var emptyitem = new ListItem(string.Empty, string.Empty);
			this.AssetDropDownList.Items.Add(emptyitem);
			this.AssetDropDownList.SelectedIndex = 0;

			// Load drop down list based on asset selection
			if (this.AssetTypeDropDownList.SelectedItem.Text == this.GetTranslatedText("Tank"))
			{
				TankCollectionClass tankCollection = FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(tanks => tanks.Enumerate(this.Security));

				foreach (TankClass tank in tankCollection)
				{
					var li = new ListItem(tank.ID, tank.IdentityGuid.ToString());
					this.AssetDropDownList.Items.Add(li);
				}
			}
			else if (this.AssetTypeDropDownList.SelectedItem.Text == this.GetTranslatedText("Equipment"))
			{
				EquipmentCollectionClass equipmentCollection = FMChannelHelper.MakeCall<IEquipments, EquipmentCollectionClass>(x => x.EnumerateManagedEquipment(this.Security));

				foreach (EquipmentClass equipment in equipmentCollection)
				{
					var li = new ListItem(equipment.ID, equipment.MasterRecordGuid.ToString());
					this.AssetDropDownList.Items.Add(li);
				}
			}
			else if (this.AssetTypeDropDownList.SelectedItem.Text == this.GetTranslatedText("{All}"))
			{
				// Add tanks first
				TankCollectionClass tankCollection =
					FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(tanks => tanks.Enumerate(this.Security));

				foreach (TankClass tank in tankCollection)
				{
					var li = new ListItem(tank.ID, tank.IdentityGuid.ToString());
					this.AssetDropDownList.Items.Add(li);
				}

				// Now add the equipment
				EquipmentCollectionClass equipmentCollection =
					FMChannelHelper.MakeCall<IEquipments, EquipmentCollectionClass>(x => x.EnumerateManagedEquipment(this.Security));

				foreach (EquipmentClass equipment in equipmentCollection)
				{
					var li = new ListItem(equipment.ID, equipment.MasterRecordGuid.ToString());
					this.AssetDropDownList.Items.Add(li);
				}
			}

			if (this.Session[TestSetFilterAsset] != null)
			{
				var stAssetType = this.Session[TestSetFilterAsset] as string;
				if (stAssetType != null)
				{
					this.AssetDropDownList.SelectedIndex =
						this.AssetDropDownList.Items.IndexOf(this.AssetDropDownList.Items.FindByText(stAssetType));
				}
			}
		}

		private void LoadResultDropDownList()
		{
			this.ResultDropDownList.Items.Clear();
			this.ResultDropDownList.Items.Add(new ListItem(this.GetTranslatedText("{All}")));
			this.ResultDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Passed")));
			this.ResultDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Failed")));
			this.ResultDropDownList.Items.Add(new ListItem(this.GetTranslatedText("Pending")));

			if (this.Session[TestsetFilterResult] != null)
			{
				var stAssetType = this.Session[TestsetFilterResult] as string;
				if (stAssetType != null)
				{
					this.ResultDropDownList.SelectedIndex =
						this.ResultDropDownList.Items.IndexOf(this.ResultDropDownList.Items.FindByText(stAssetType));
				}
			}
			else
			{
				this.ResultDropDownList.SelectedIndex = 0;
			}
		}

		private void LoadTestSetDropDownList()
		{
			this.TestSetDropDownList.Items.Clear();

			// Always add an empty item to the list
			var emptyitem = new ListItem(string.Empty, string.Empty);
			this.TestSetDropDownList.Items.Add(emptyitem);
			this.TestSetDropDownList.SelectedIndex = 0;

			TestSetCollectionClass testSetCollection =
				FMChannelHelper.MakeCall<ITestSets, TestSetCollectionClass>(
					sets => sets.Enumerate(this.Security, string.Empty, "TestSetName"));

			foreach (TestSetClass testSet in testSetCollection)
			{
				var li = new ListItem(testSet.ID, testSet.IdentityGuid.ToString());
				this.TestSetDropDownList.Items.Add(li);
			}

			if (this.Session[TestsetFilterTestset] != null)
			{
				var stAssetType = this.Session[TestsetFilterTestset] as string;
				if (stAssetType != null)
				{
					this.TestSetDropDownList.SelectedIndex =
						this.TestSetDropDownList.Items.IndexOf(this.TestSetDropDownList.Items.FindByText(stAssetType));
				}
			}
		}

		private void TestingResultsGridViewSorting(object sender, GridViewSortEventArgs e)
		{
			try
			{
				var sortExpression = this.Session[SortExpression] as string;
				var sortDirection = this.Session[SortDirection] as string;

				if (e.SortExpression != sortExpression)
				{
					this.Session[SortDirection] = "ASC";
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

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void UpdateView()
		{
			var timer = new StopWatch(StopWatch.Appnames.Accounting, "TestResults - EnumerateTestSetResults()");
			//DataTable TestingResultsDataTable;
			DataView testingResults = this.EnumerateTestSetResults();
			timer.Stop();

			// Create an empty table to bind to, next bind to the actual table.
			// This fixes the update problem when the grid begins with an empty dataset.
			var emptyDataTable = new DataTable();
			var emptyDataView = new DataView(emptyDataTable);
			this.TestingResultsGridView.DataSource = emptyDataView;
			this.TestingResultsGridView.DataBind();

			timer.Start("TestResults - DataBind()");

			this.TestingResultsGridView.AutoGenerateColumns = false;

			// If the columns have already been created for the results of EnumerateTestSetResults(), then there
			// is no need to do it again.
			if (this.TestingResultsGridView.Columns.Count <= 1)
			{
				foreach (DataColumn col in testingResults.Table.Columns)
				{
					var bf = new BoundField
					         {
						         DataField = col.ColumnName,
						         HeaderText = col.ColumnName,
						         SortExpression =
							         (col.ColumnName == this.testingDateVisibleColName)
								         ? TestingDateHiddenColName
								         : col.ColumnName,
						         Visible = (col.ColumnName != TestingDateHiddenColName)
					         };
					this.TestingResultsGridView.Columns.Add(bf);
				}
			}

			this.TestingResultsGridView.DataSource = testingResults;
			this.TestingResultsGridView.DataBind();
			timer.Stop();
		}

		private void ValidateControls()
		{
			// Check for invalid date range
			if (this.FromDate.CurrentValue > this.ToDate.CurrentValue)
			{
				// correct dates for the user
				this.ToDate.CurrentValue = this.FromDate.CurrentValue;
			}
		}
		protected string TestSetResultFormUrl
		{
			get
			{
				string testSetResultFormUrl = FMChannelHelper.MakeCall<IConfigurationSettings, string>(x => x.GetKeyValueByKey(Security, "TestSetResultFormURL"));
				if (string.IsNullOrEmpty(testSetResultFormUrl))
				{
					testSetResultFormUrl = "TestSetResultForm.aspx";
				}
				else
				{
					testSetResultFormUrl = "../" + testSetResultFormUrl;
				}
				return testSetResultFormUrl;
			}
		}
		#endregion
	}

}