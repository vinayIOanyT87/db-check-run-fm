namespace Dispatch
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Data;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Forms;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

    using FMCore;
	using FMDispatchBusinessObjects.BusinessInterfaces;
	using FMDispatchBusinessObjects.ChannelFactories;

	public partial class DispatchForm : FMBaseForm
	{
		private const int WmKeydown = 0x100;

		private const int Grid1Numcols = 10;
		private const int Grid2Numcols = 4;
		private const int Grid3Numcols = 38;

		private readonly DispatchDataAccess dataAccess;
		private List<DispatchTransactionsSR.DispatchTranslationPair> translations;
		private readonly string fillStandTransactionAlias = string.Empty;
		private readonly string returnToBulkTransactionAlias = string.Empty;
		private readonly DateTime operationLockDate;

		private readonly int[] dispatchGrid1ColumnPositions = new int[Grid1Numcols];
		private readonly int[] dispatchGrid2ColumnPositions = new int[Grid2Numcols];
		private readonly int[] dispatchGrid3ColumnPositions = new int[Grid3Numcols];

		private readonly int[] dispatchGrid1ColumnWidths = new int[Grid1Numcols];
		private readonly int[] dispatchGrid2ColumnWidths = new int[Grid2Numcols];
		private readonly int[] dispatchGrid3ColumnWidths = new int[Grid3Numcols];

		private readonly DispatchTransactionsSR sr;

		private EquipmentCollectionClass equipmentCollection;
		private PersonCollectionClass personCollection;
		private ProductClass cachedProductForCheck; //used only for CheckGrade method to help with database calls

		private DataTable dtTransactions;
		int firstDisplayedScrollingRowIndex = 0;

		private enum FocusType
		{
			Equipment,
			Personnel,
			Transactions
		}

		private enum TransactionCheckResult
		{
			Ok,
			Cancel,
			Exempted
		}

		private FocusType lastFocus;

		public enum DisplayModeType
		{
			Normal,
			FlightLineStatus
		}

		public DisplayModeType DisplayMode
		{
			get;
			set;
		}

		private readonly string[] initialTransID;
		private readonly string selectedPersonID;
		private readonly string selectedEquipmentID;

		private readonly SiteTimeConverter timeConverter;

		public List<TransactionDO> Transactions;

		public DispatchForm(DispatchDataAccess access, 
							List<DispatchTransactionsSR.DispatchTranslationPair> translations, 
							string[] selectedTransID, 
							string selectedPerson, 
							string selectedEquipment, 
							DateTime lockDate)
		{
			try
			{
				this.GetSecurity();

				DispatchTransactionsSR refSR = DispatchContainerForm.DispatchSR;

				this.sr = new DispatchTransactionsSR
				     {
					     BeginDate			= refSR.BeginDate,
					     EndDate			= refSR.EndDate,
					     CurrentSiteGuid	= refSR.CurrentSiteGuid,
					     Security			= refSR.Security,
					     Site				= refSR.Site,
					     SiteList			= refSR.SiteList,
					     Translations		= refSR.Translations
				     };

				this.sr.Statuses.Add("Requested");
				this.sr.Statuses.Add("Dispatched");

				this.operationLockDate = lockDate;

				this.fillStandTransactionAlias = ConfigurationManager.AppSettings["FillStandTransactionAlias"];

				if (string.IsNullOrEmpty(this.fillStandTransactionAlias))
				{
					throw new Exception("FillStandTransactionAlias not in AppSettings");
				}

				this.returnToBulkTransactionAlias = ConfigurationManager.AppSettings["ReturnToBulkTransactionAlias"];

				if (string.IsNullOrEmpty(this.returnToBulkTransactionAlias))
				{
					throw new Exception("ReturnToBulkTransactionAlias not in AppSettings");
				}

				var site =
					FMChannelHelper.MakeCall<IClientDispatchService, SiteClass>(x => x.GetSite(this.Security, this.Security.SiteGuid));

				this.timeConverter = new SiteTimeConverter(site);

				this.translations = translations;
				this.initialTransID = selectedTransID;
				this.selectedPersonID = selectedPerson;
				this.selectedEquipmentID = selectedEquipment;

				this.GetColumnPositionsForGrid1();
				this.GetColumnPositionsForGrid2();
				this.GetColumnPositionsForGrid3();

				this.InitializeComponent();

				// format data grid dates based on site configuration
				this.GetSiteDateTimeFormatInfo();
				var dataGridViewCellStyleDateTime = new DataGridViewCellStyle
				                                    {
					                                    Format =
						                                    this.SiteDateTimeFormatInfo.ShortDatePattern + " "
						                                    + this.SiteDateTimeFormatInfo.ShortTimePattern
				                                    };

				this.Date.DefaultCellStyle = dataGridViewCellStyleDateTime;

				this.LocalComponentInitialization();

				this.GetColumnWidthsForGrid1();
				this.GetColumnWidthsForGrid2();
				this.GetColumnWidthsForGrid3();

				// set the grid indexs
				for (int iLoop = 0; iLoop < Grid1Numcols; iLoop++)
				{
					this.EquipmentDataGridView.Columns[iLoop].DisplayIndex = this.dispatchGrid1ColumnPositions[iLoop];
					this.EquipmentDataGridView.Columns[iLoop].Width = this.dispatchGrid1ColumnWidths[iLoop];
				}

				for (int iLoop = 0; iLoop < Grid2Numcols; iLoop++)
				{
					this.PersonDataGridView.Columns[iLoop].DisplayIndex = this.dispatchGrid2ColumnPositions[iLoop];
					this.PersonDataGridView.Columns[iLoop].Width = this.dispatchGrid2ColumnWidths[iLoop];
				}

				for (int iLoop = 0; iLoop < Grid3Numcols; iLoop++)
				{
					this.RequestDataGridView.Columns[iLoop].DisplayIndex = this.dispatchGrid3ColumnPositions[iLoop];
					this.RequestDataGridView.Columns[iLoop].Width = this.dispatchGrid3ColumnWidths[iLoop];
				}

				// we enable this here so we do not get a bunch of events for each index that we have set above
				this.EquipmentDataGridView.ColumnDisplayIndexChanged += this.EquipmentDataGridViewColumnDisplayIndexChanged;
				this.PersonDataGridView.ColumnDisplayIndexChanged += this.PersonDataGridViewColumnDisplayIndexChanged;
				this.RequestDataGridView.ColumnDisplayIndexChanged += this.RequestDataGridViewColumnDisplayIndexChanged;

				this.RequestDataGridView.VirtualMode = true;
				this.RequestDataGridView.CellValueNeeded += this.RequestDataGridViewCellValueNeeded;
				this.dataAccess = access;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		void RequestDataGridViewCellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
		{
			if (this.RequestDataGridView.ValidIndex(e.RowIndex) == false)
			{
				return;
			}

			string dataFieldName = this.RequestDataGridView.Columns[e.ColumnIndex].DataPropertyName;
			DataRowView row = this.RequestDataGridView.GetDataRow(e.RowIndex);

			if (string.IsNullOrEmpty(dataFieldName) == false)
			{
				e.Value = row[dataFieldName];
			}
		}

		private void LocalComponentInitialization()
		{
			this.SizeChanged += this.DispatchFormSizeChanged;

			this.RequestDataGridView.SelectionChanged += this.RequestDataGridViewSelectionChanged;

			this.RequestDataGridView.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
			this.RequestDataGridView.AllowUserToResizeColumns = true;
			this.RequestDataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
			this.RequestDataGridView.RowHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Regular);
			this.RequestDataGridView.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

			this.RequestDataGridView.RowHeaderMouseDoubleClick += this.RequestDataGridViewRowHeaderMouseDoubleClick;
			this.RequestDataGridView.CellFormatting += this.RequestDataGridViewCellFormatting;
			this.RequestDataGridView.CellDoubleClick += this.RequestDataGridViewCellDoubleClick;
			this.RequestDataGridView.LostFocus += this.RequestDataGridViewLostFocus;

			this.PersonDataGridView.SelectionChanged += this.PersonDataGridViewSelectionChanged;

			this.PersonDataGridView.RowHeadersVisible = false;
			this.PersonDataGridView.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;

			this.PersonDataGridView.CellFormatting += this.PersonDataGridViewCellFormatting;
			this.PersonDataGridView.LostFocus += this.PersonDataGridViewLostFocus;
			this.PersonDataGridView.CellDoubleClick += this.PersonDataGridViewCellDoubleClick;
			this.PersonDataGridView.Sorted += this.PersonDataGridViewSorted;

			this.EquipmentDataGridView.RowHeadersVisible = false;
			this.EquipmentDataGridView.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;

			this.EquipmentDataGridView.CellFormatting += this.EquipmentDataGridViewCellFormatting;
			this.EquipmentDataGridView.LostFocus += this.EquipmentDataGridViewLostFocus;
			this.EquipmentDataGridView.CellDoubleClick += this.EquipmentDataGridViewCellDoubleClick;
			this.EquipmentDataGridView.SelectionChanged += this.EquipmentDataGridViewSelectionChanged;

			this.EquipmentFindComboBox.TextChanged += this.EquipmentFindComboBoxTextChanged;
			this.PersonFindComboBox.TextChanged += this.PersonFindComboBoxTextChanged;
		}

		void EquipmentDataGridViewSelectionChanged(object sender, EventArgs e)
		{
			try
			{
				lock (this.EquipmentDataGridView)
				{
					// Change the selection in the find combo box
					if (this.EquipmentDataGridView.SelectedRows.Count > 0)
					{
						// Keep this event from firing this time
						this.EquipmentFindComboBox.TextChanged -= this.EquipmentFindComboBoxTextChanged;

						// Set the value
						var equipment = (EquipmentClass)this.EquipmentDataGridView.SelectedRows[0].DataBoundItem;
						this.EquipmentFindComboBox.SelectedItem = equipment;

						// Reinstate the event handler
						this.EquipmentFindComboBox.TextChanged += this.EquipmentFindComboBoxTextChanged;
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		void PersonDataGridViewSorted(object sender, EventArgs e)
		{
			try
			{
				this.PersonFindComboBoxTextChanged(null, null);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			// If the tab key is pressed to leave the EquipmentDataGridView or the EquipmentFindComboBox,
			// we want to automatically change selection in the PersonDataGrid to be the
			// operator assigned to this equipment (if any).
			if (msg.Msg == WmKeydown && keyData == Keys.Tab)
			{
				if (this.EquipmentDataGridView.Focused || this.EquipmentFindComboBox.Focused)
				{
					this.EquipmentDataGridViewSelectPersonnel();
				}
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}

		/// <summary>
		/// This method will search the PersonDataGrid and select the operator
		/// assigned to the currently selected equipment in the EquipmentDataGridView
		/// if any.
		/// </summary>
		void EquipmentDataGridViewSelectPersonnel()
		{
			try
			{
				if (this.EquipmentDataGridView.SelectedRows.Count > 0)
				{
					var equipment = (EquipmentClass)this.EquipmentDataGridView.SelectedRows[0].DataBoundItem;

					lock (this.PersonDataGridView)
					{
						foreach (DataGridViewRow row in this.PersonDataGridView.Rows)
						{
							var person = (PersonClass) row.DataBoundItem;

							if (person.AssignedEquipmentGuid == equipment.MasterRecordGuid)
							{
								this.SelectPersonnelRow(row);
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

		/// <summary>
		/// This method will select the indicated row in the PersonDataGrid and 
		/// set the PersonFindComboBox text as well.
		/// </summary>
		/// <param name="selectRow"></param>
		private void SelectPersonnelRow(DataGridViewRow selectRow)
		{
			foreach (DataGridViewRow row in this.PersonDataGridView.Rows)
			{
				row.Selected = false;
			}

			selectRow.Selected = true;

			if (this.PersonDataGridView.FirstDisplayedScrollingRowIndex > selectRow.Index
				|| this.PersonDataGridView.FirstDisplayedScrollingRowIndex + this.PersonDataGridView.DisplayedRowCount(false) <= selectRow.Index)
			{
				this.PersonDataGridView.FirstDisplayedScrollingRowIndex = selectRow.Index;
			}
		}

		/// <summary>
		/// This method will select the indicated row in the EquipmentDataGridView and 
		/// set the EquipmentFindComboBox text as well.
		/// </summary>
		/// <param name="selectRow"></param>
		private void SelectEquipmentRow(DataGridViewRow selectRow)
		{
			foreach (DataGridViewRow row in this.EquipmentDataGridView.Rows)
			{
				row.Selected = false;
			}

			selectRow.Selected = true;
			var equipment = (EquipmentClass) selectRow.DataBoundItem;
			this.EquipmentFindComboBox.Text = equipment.IssPtNum;

			if (this.EquipmentDataGridView.FirstDisplayedScrollingRowIndex > selectRow.Index
				|| this.EquipmentDataGridView.FirstDisplayedScrollingRowIndex + this.EquipmentDataGridView.DisplayedRowCount(false) <= selectRow.Index)
			{
				this.EquipmentDataGridView.FirstDisplayedScrollingRowIndex = selectRow.Index;
			}

		}

		void EquipmentDataGridViewCellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			try
			{
				if (this.EquipmentDataGridView.SelectedRows.Count > 0)
				{
					string baseAddress = ConfigurationManager.AppSettings["WebAppAddress"];

					if (String.IsNullOrEmpty(baseAddress))
					{
						throw new ApplicationException("WebAppAddress not in configuration file.");
					}

					var equipment = (EquipmentClass)this.EquipmentDataGridView.SelectedRows[0].DataBoundItem;
					string address = baseAddress + "/FMWebApp/EquipmentForm.aspx?DispatchEdit=" + equipment.IdentityGuid + "&ClientDispatch=true";
					var browser = new EmbeddedBrowser(address);

					this.firstDisplayedScrollingRowIndex = this.EquipmentDataGridView.FirstDisplayedScrollingRowIndex;
					browser.ShowDialog(this);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		void PersonFindComboBoxTextChanged(object sender, EventArgs e)
		{
			try
			{
				lock (this.PersonDataGridView)
				{
					foreach (DataGridViewRow row in this.PersonDataGridView.Rows)
					{
						var person = (PersonClass) row.DataBoundItem;

						if (person.LastName.StartsWith(this.PersonFindComboBox.Text, true, null))
						{
							this.PersonDataGridView.SelectionChanged -= this.PersonDataGridViewSelectionChanged;
							row.Selected = true;
							this.PersonDataGridView.SelectionChanged += this.PersonDataGridViewSelectionChanged;

							if (this.PersonDataGridView.FirstDisplayedScrollingRowIndex > row.Index
								 || this.PersonDataGridView.FirstDisplayedScrollingRowIndex + this.PersonDataGridView.DisplayedRowCount(false) <= row.Index)
							{
								this.PersonDataGridView.FirstDisplayedScrollingRowIndex = row.Index;
							}

							break;
						}
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		void EquipmentFindComboBoxTextChanged(object sender, EventArgs e)
		{
			try
			{
				lock (this.EquipmentDataGridView)
				{
					foreach (DataGridViewRow row in this.EquipmentDataGridView.Rows)
					{
						var equipment = (EquipmentClass) row.DataBoundItem;

						if (equipment.IssPtNum.StartsWith(this.EquipmentFindComboBox.Text, true, null))
						{
							this.EquipmentDataGridView.SelectionChanged -= this.EquipmentDataGridViewSelectionChanged;
							row.Selected = true;
							this.EquipmentDataGridView.SelectionChanged += this.EquipmentDataGridViewSelectionChanged;

							if (this.EquipmentDataGridView.FirstDisplayedScrollingRowIndex > row.Index
								 || this.EquipmentDataGridView.FirstDisplayedScrollingRowIndex + this.EquipmentDataGridView.DisplayedRowCount(false) <= row.Index)
							{
								this.EquipmentDataGridView.FirstDisplayedScrollingRowIndex = row.Index;
							}

							break;
						}
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		void PersonDataGridViewCellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			try
			{
				if (this.PersonDataGridView.SelectedRows.Count > 0)
				{
					string baseAddress = ConfigurationManager.AppSettings["WebAppAddress"];

					if (String.IsNullOrEmpty(baseAddress))
					{
						throw new ApplicationException("WebAppAddress not in configuration file.");
					}

					var person = (PersonClass)this.PersonDataGridView.SelectedRows[0].DataBoundItem;
					string address = baseAddress + "/FMWebApp/PersonForm.aspx?DispatchEdit=" + person.IdentityGuid + "&ClientDispatch=true";
					var browser = new EmbeddedBrowser(address);

					this.firstDisplayedScrollingRowIndex = this.PersonDataGridView.FirstDisplayedScrollingRowIndex;
					browser.ShowDialog(this);
				}

			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		void PersonDataGridViewLostFocus(object sender, EventArgs e)
		{
			try
			{
				this.lastFocus = FocusType.Personnel;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		void RequestDataGridViewLostFocus(object sender, EventArgs e)
		{
			try
			{
				this.lastFocus = FocusType.Transactions;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		void EquipmentDataGridViewLostFocus(object sender, EventArgs e)
		{
			try
			{
				this.lastFocus = FocusType.Equipment;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		void RequestDataGridViewCellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			try
			{
				this.RowDoubleClicked(e.RowIndex);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		void EquipmentDataGridViewCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			try
			{
				var equipment = (EquipmentClass)this.EquipmentDataGridView.Rows[e.RowIndex].DataBoundItem;

				e.CellStyle.SelectionBackColor = Color.Black;

				if (equipment.LockedOut
					|| equipment._QCDate.Value < this.timeConverter.Today())
				{
					e.CellStyle.ForeColor = Color.Gray;
					e.CellStyle.SelectionForeColor = Color.Gray;
				}
				else
				{
					if (equipment.InServiceFlag)
					{
						if (equipment.FuelingType == FUELING_TYPES.REFUELER)
						{
							e.CellStyle.ForeColor = Color.Blue;
							e.CellStyle.SelectionForeColor = Color.Yellow;
						}
						else if (equipment.FuelingType == FUELING_TYPES.DEFUELER)
						{
							e.CellStyle.ForeColor = Color.Red;
							e.CellStyle.SelectionForeColor = Color.Red;
						}
						else
						{
							e.CellStyle.ForeColor = Color.Black;
							e.CellStyle.SelectionForeColor = Color.White;
						}
					}
					else
					{
						e.CellStyle.ForeColor = Color.Gray;
						e.CellStyle.SelectionForeColor = Color.Gray;
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		void PersonDataGridViewCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			try
			{
				var person = (PersonClass)this.PersonDataGridView.Rows[e.RowIndex].DataBoundItem;

				e.CellStyle.SelectionBackColor = Color.Black;

				if (person.LockedOut)
				{
					e.CellStyle.ForeColor = Color.Gray;
					e.CellStyle.SelectionForeColor = Color.Gray;
				}
				else if (person.Status == PersonClass.STATUS.Out)
				{
					e.CellStyle.ForeColor = Color.Gray;
					e.CellStyle.SelectionForeColor = Color.Gray;
				}
				else
				{
					e.CellStyle.ForeColor = Color.Blue;
					e.CellStyle.SelectionForeColor = Color.Yellow;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		void RequestDataGridViewCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (this.RequestDataGridView.ValidIndex(e.RowIndex) == false)
			{
				return;
			}

			// Transactions need to be displayed in different colors by requirement
			// Red = open defuel
			// Gray = canceled or Complete requests
			// Blue = open refuel
			// Black = other dispatch transactions

			// Find the data record
			try
			{
				var row = this.RequestDataGridView.GetDataRow(e.RowIndex);

				// Set the appropriate color
				var aliasName = (string) row["AliasName"];
				var status = (TransactionStatus) Enum.Parse(typeof(TransactionStatus), (string) row["TransactionStatus"]);

				e.CellStyle.SelectionBackColor = Color.Black;

				if (status != TransactionStatus.Requested)
				{
					e.CellStyle.ForeColor = Color.Gray;
					e.CellStyle.SelectionForeColor = Color.Gray;
				}
				else if (aliasName.Equals("Defuel"))
				{
					e.CellStyle.ForeColor = Color.Red;
					e.CellStyle.SelectionForeColor = Color.Cyan;
				}
				else if (aliasName.Equals("Refuel"))
				{
					e.CellStyle.ForeColor = Color.Blue;
					e.CellStyle.SelectionForeColor = Color.Yellow;
				}
				else
				{
					e.CellStyle.ForeColor = Color.Black;
					e.CellStyle.SelectionForeColor = Color.White;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		void RequestDataGridViewRowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			try
			{
				this.RowDoubleClicked(e.RowIndex);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		void RowDoubleClicked(int rowIndex)
		{
			try
			{
				if (rowIndex >= 0)
				{
					var row = this.RequestDataGridView.GetDataRow(rowIndex);
					var fuelRequestForm = new FuelRequestForm(this.operationLockDate) { TransID = (string)row["TransID"] };
					fuelRequestForm.ShowDialog(this);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		void PersonDataGridViewSelectionChanged(object sender, EventArgs e)
		{
			try
			{
				if (this.PersonDataGridView.SelectedRows.Count > 0)
				{
					var person = (PersonClass)this.PersonDataGridView.SelectedRows[0].DataBoundItem;

					bool enable = this.Security.HasRight(RIGHT.MODIFY_DISPATCH);

					this.OutButton.Enabled = (person.Status != PersonClass.STATUS.Out) && enable;
					this.OutButton.TabStop = false;
					this.StandButton.Enabled = (person.Status != PersonClass.STATUS.STB) && enable;
					this.StandButton2.Enabled = this.StandButton.Enabled;
					this.StandButton2.TabStop = false;

					// find the person in the combo box
					foreach (PersonClass nextPersonInComboBox in this.PersonFindComboBox.Items)
					{
						if (nextPersonInComboBox.ID == person.ID)
						{
							this.PersonFindComboBox.TextChanged -= this.PersonFindComboBoxTextChanged;
							this.PersonFindComboBox.SelectedItem = nextPersonInComboBox;
							this.PersonFindComboBox.TextChanged += this.PersonFindComboBoxTextChanged;

							break;
						}
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		void RequestDataGridViewSelectionChanged(object sender, EventArgs e)
		{
			try
			{
				this.DispatchButton.Enabled = false;
				this.CancelDispatchButton.Enabled = false;

				lock (this.RequestDataGridView)
				{
					if (this.RequestDataGridView.SelectedRows.Count > 0)
					{
						bool bNewCancelState = this.Security.HasRight(RIGHT.MODIFY_DISPATCH);
						bool bNewDispatchState = this.Security.HasRight(RIGHT.MODIFY_DISPATCH);

						foreach (DataGridViewRow gridRow in this.RequestDataGridView.SelectedRows)
						{
							var dataRow = this.RequestDataGridView.GetDataRow(gridRow.Index);

							var statusText = (string) dataRow["TransactionStatus"];
							var status = (TransactionStatus) Enum.Parse(typeof(TransactionStatus), statusText);

							bNewCancelState &= (status == TransactionStatus.Dispatched)
							&& ((string) dataRow["AliasName"]).NotEquals(this.fillStandTransactionAlias);

							bNewDispatchState &= (status == TransactionStatus.Requested)
							&& ((string) dataRow["AliasName"]).NotEquals(this.fillStandTransactionAlias);

						}

						this.CancelDispatchButton.Enabled = bNewCancelState;
						this.DispatchButton.Enabled = bNewDispatchState;
					}

					this.RadioButton.Enabled = (this.RequestDataGridView.SelectedRows.Count == 1)
						&& this.Security.HasRight(RIGHT.MODIFY_DISPATCH);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		void DispatchFormSizeChanged(object sender, EventArgs e)
		{
			try
			{
				//RequestGroupBox.Top = Size.Height / 2;
				//RequestGroupBox.Height = (Size.Height / 2 ) - 50;
				//RequestGroupBox.Width = Size.Width - 30;
				//RequestDataGridView.Width = RequestGroupBox.Width - 20;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void CloseButtonClick(object sender, EventArgs e)
		{
			try
			{
				this.Close();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void DispatchFormLoad(object sender, EventArgs e)
		{
			try
			{
				this.dataAccess.OnEquipmentUpdated += this.UpdateEquipmentView;
				this.dataAccess.OnPersonnelUpdated += this.UpdatePersonnelView;
				this.dataAccess.OnDataUpdated += this.RefreshRequestView;
				this.dataAccess.OnError += this.ErrorHandler;

				this.UpdateView();

				// If flight line mode, set the display filters
				if (this.DisplayMode == DisplayModeType.FlightLineStatus)
				{
					int index = this.EquipmentFilterDropDown.FindString("Show Flight-Line Status");
					this.EquipmentFilterDropDown.SelectedIndex = index;
					this.EquipmentFilterDropDownSelectedIndexChanged(null, null);
				}

				bool bEnable = this.Security.HasRight(RIGHT.MODIFY_DISPATCH);
				this.HomeButton.Enabled = bEnable;
				this.HomeButton.TabStop = false;
				this.OutButton.Enabled = bEnable;
				this.OutButton.TabStop = false;
				this.StandButton.Enabled = bEnable;
				this.StandButton2.Enabled = bEnable;
				this.StandButton2.TabStop = false;
				this.FillButton.Enabled = bEnable;
				this.RTBButton.Enabled = bEnable;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method starts the data process for the form 
		/// </summary>
		private void UpdateView()
		{
			this.UpdateEquipmentView(this.dataAccess.GetEquipment());
			this.UpdatePersonnelView(this.dataAccess.GetPersonnel());
			this.UpdateRequestView();

			lock (this.RequestDataGridView)
			{
				// Attempt to select the initial transaction selection
				if (this.initialTransID != null)
				{
					int firstDisplayedScrollingIndex = this.RequestDataGridView.FirstDisplayedScrollingRowIndex;

					this.RequestDataGridView.ClearSelection();
					this.RequestDataGridView.CurrentCell = null;

					foreach (string passedintranid in this.initialTransID)
					{
						foreach (DataGridViewRow row in this.RequestDataGridView.Rows)
						{
							var dataRow = this.RequestDataGridView.GetDataRow(row.Index);

							if (dataRow["TransID"].ToString().Equals(passedintranid))
							{
								row.Selected = true;
								if (this.RequestDataGridView.CurrentCell == null)
								{
									this.RequestDataGridView.CurrentCell = row.Cells[0];
								}

								if (row.Index < firstDisplayedScrollingIndex
								    || row.Index > firstDisplayedScrollingIndex + this.RequestDataGridView.DisplayedRowCount(false))
								{
									firstDisplayedScrollingIndex = row.Index;
								}

								break;
							}
						}
					}

					if (firstDisplayedScrollingIndex >= 0 && firstDisplayedScrollingIndex < this.RequestDataGridView.Rows.Count)
					{
						this.RequestDataGridView.FirstDisplayedScrollingRowIndex = firstDisplayedScrollingIndex;
					}
				}
			}
		}

		private void RefreshRequestView(DataTable data)
		{
			this.RefreshRequestView(data, DateTime.Now);
		}

		/// <summary>
		/// This method is responsible for updating the grid's datasource
		/// </summary>
		private void RefreshRequestView(object data, DateTime resultTime)
		{
			try
			{
				ISynchronizeInvoke i = this;

				// Check if the event was generated from another
				// thread and needs invoke instead
				if (i.InvokeRequired)
				{
					DataTable dataTable = this.dataAccess.GetTransactions(this.sr).Transactions.Tables[0];
					resultTime = DateTime.Now; //resetting here since this is when we actually got the data

					var tempDelegate = new DispatchDataAccess.OnDataUpdatedHandler(this.RefreshRequestView);
					var results = new Object[] { dataTable, resultTime };
					i.Invoke(tempDelegate, results);

					return;
				}

				lock (this.RequestDataGridView)
				{
					this.RequestDataGridView.SelectionChanged -= this.RequestDataGridViewSelectionChanged;

					var dataTable = data as DataTable;
					this.RequestDataGridView.MergeTransactionsAndUpdateView(dataTable, resultTime);

					int rowIndex = 1;
					foreach (DataGridViewRow row in this.RequestDataGridView.Rows)
					{
						row.HeaderCell.Value = rowIndex.ToString(CultureInfo.InvariantCulture);
						rowIndex++;
					}

					this.RequestDataGridView.SelectionChanged += this.RequestDataGridViewSelectionChanged;

					this.RequestDataGridViewSelectionChanged(null, null);
					this.RequestDataGridView.Refresh();
				}
			}
			catch (ObjectDisposedException)
			{
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void UpdateRequestView()
		{
			try
			{
				if (this.IsHandleCreated == false)
				{
					return;
				}

				lock (this.RequestDataGridView)
				{
					var selectedTransID = new string[this.RequestDataGridView.SelectedRows.Count];
					int numberOfSelectedRows = this.RequestDataGridView.SelectedRows.Count;
					string currentRowTransID = string.Empty;

					for (int iLoop = 0; iLoop < numberOfSelectedRows; iLoop++)
					{
						var selectedRow = this.RequestDataGridView.SelectedRows[iLoop];
						var row = this.RequestDataGridView.GetDataRow(selectedRow.Index);
						selectedTransID[iLoop] = (string) row["TransID"];
					}

					// store the current row so it can be restored
					if (this.RequestDataGridView.CurrentRow != null)
					{
						var currentRow = this.RequestDataGridView.GetDataRow(this.RequestDataGridView.CurrentRow.Index);

						if (currentRow != null)
						{
							currentRowTransID = (string) currentRow["TransID"];
						}
					}

					DispatchTransactionsDO dispatchTransactionDO = this.dataAccess.GetTransactions(this.sr);

					int firstDisplayedScrollingIndex = this.RequestDataGridView.FirstDisplayedScrollingRowIndex;
					this.dtTransactions = dispatchTransactionDO.Transactions.Tables[0];

					// Sort before we bind to save time
					var view = new DataView(this.dtTransactions) { Sort = "RequestedDateTime" };

					this.RequestDataGridView.SelectionChanged -= this.RequestDataGridViewSelectionChanged;
					this.RequestDataGridView.DispatchDataView = view;

					// since the grid will autoselect the first row we need to reset the selection before we then set it
					// Set the row number display text values
					int index = 1;
					foreach (DataGridViewRow row in this.RequestDataGridView.Rows)
					{
						var dataRow = this.RequestDataGridView.GetDataRow(row.Index);
						row.HeaderCell.Value = index.ToString(CultureInfo.InvariantCulture);

						if (numberOfSelectedRows > 0)
						{
							row.Selected = false;
						}

						if (!string.IsNullOrEmpty(currentRowTransID) &&
							currentRowTransID.Equals((string) dataRow["TransID"]))
						{
							// restore the current selection
							// the only way to do this is by setting the currentcell variable since the currentrow is read only
							this.RequestDataGridView.CurrentCell = row.Cells[0];
							row.Selected = true;
						}

						++index;
					}

					// we only do this if there are selections already made
					if (numberOfSelectedRows > 0)
					{
						// restore the selected rows if they are still visible
						for (int iLoop = 0; iLoop < numberOfSelectedRows; iLoop++)
						{
							foreach (DataGridViewRow row in this.RequestDataGridView.Rows)
							{
								var dataRow = this.RequestDataGridView.GetDataRow(row.Index);
								if (dataRow != null)
								{
									if (selectedTransID[iLoop].Equals((string) dataRow["TransID"]))
									{
										row.Selected = true;

										if (row.Index < firstDisplayedScrollingIndex
										    || row.Index > firstDisplayedScrollingIndex + this.RequestDataGridView.DisplayedRowCount(false))
										{
											firstDisplayedScrollingIndex = row.Index;
										}

										break;
									}
								}
							}
						}
					}

					if (firstDisplayedScrollingIndex >= 0
						&& firstDisplayedScrollingIndex < this.RequestDataGridView.Rows.Count)
					{
						this.RequestDataGridView.FirstDisplayedScrollingRowIndex = firstDisplayedScrollingIndex;
					}

					this.RequestDataGridView.SelectionChanged += this.RequestDataGridViewSelectionChanged;
					this.RequestDataGridViewSelectionChanged(null, null);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void UpdatePersonnelView(object data)
		{
			this.UpdatePersonnelView(data, DateTime.Now);
		}

		private void UpdatePersonnelView(object data, DateTime resultTime)
		{
			try
			{
				ISynchronizeInvoke i = this;

				// Check if the event was generated from another
				// thread and needs invoke instead
				if (i.InvokeRequired)
				{
					PersonCollectionClass personCollectionClass = this.dataAccess.GetPersonnel();

					var tempDelegate = new DispatchDataAccess.OnDataUpdatedHandler(this.UpdatePersonnelView);
					var results = new Object[] { personCollectionClass, resultTime };

					i.Invoke(tempDelegate, results);
					return;
				}

				if (this.IsHandleCreated == false)
				{
					return;
				}

				lock (this.PersonDataGridView)
				{
					Guid selectedItemGuid = Guid.Empty;

					if (this.PersonDataGridView.SelectedRows.Count > 0)
					{
						var selectedPerson = (PersonClass)this.PersonDataGridView.SelectedRows[0].DataBoundItem;
						selectedItemGuid = selectedPerson.IdentityGuid;
					}

					this.personCollection = data as PersonCollectionClass;
					this.PersonFilterDropDownSelectedIndexChanged(null, null);

					if (this.personCollection != null 
						&& this.personCollection.Count > 0 
						&& this.PersonDataGridView.SelectedRows.Count == 0 
						&& this.PersonDataGridView.Rows.Count > 0)
					{
						this.PersonDataGridView.Rows[0].Selected = true;
					}

					// if the calling routine passed in a person try to locate and then select it as the default
					if (!string.IsNullOrEmpty(this.selectedPersonID))
					{
						int selectedIndex = this.PersonFindComboBox.FindString(this.selectedPersonID);

						if (selectedIndex >= 0)
						{
							// selection found so set it and update
							this.PersonFindComboBox.SelectedIndex = selectedIndex;
						}
					}
					else if (selectedItemGuid != Guid.Empty)
					{
						foreach (DataGridViewRow row in this.PersonDataGridView.Rows)
						{
							var checkPerson = (PersonClass) row.DataBoundItem;

							if (checkPerson.IdentityGuid == selectedItemGuid)
							{
								this.SelectPersonnelRow(row);
								break;
							}
						}
					}

					if (!string.IsNullOrEmpty(this.selectedEquipmentID))
					{
						foreach (EquipmentClass equipment in this.EquipmentFindComboBox.Items)
						{
							if (equipment.ID == this.selectedEquipmentID)
							{
								this.EquipmentFindComboBox.SelectedItem = equipment;
								break;
							}
						}
					}
				}
			}
			catch (ObjectDisposedException)
			{
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void UpdateEquipmentView(object data)
		{
			this.UpdateEquipmentView(data, DateTime.Now);
		}

		private void UpdateEquipmentView(object data, DateTime resultTime)
		{
			try
			{
				ISynchronizeInvoke i = this;

				// Check if the event was generated from another
				// thread and needs invoke instead
				if (i.InvokeRequired)
				{
					EquipmentCollectionClass equipmentCollectionClass = this.dataAccess.GetEquipment();

					var tempDelegate = new DispatchDataAccess.OnDataUpdatedHandler(this.UpdateEquipmentView);
					var results = new Object[] { equipmentCollectionClass, resultTime };

					i.Invoke(tempDelegate, results);
					return;
				}

				if (this.IsHandleCreated == false)
				{
					return;
				}

				lock (this.EquipmentDataGridView)
				{
					Guid selectedGuid = Guid.Empty;

					if (this.EquipmentDataGridView.SelectedRows.Count > 0)
					{
						var selectedEquipment = (EquipmentClass)this.EquipmentDataGridView.SelectedRows[0].DataBoundItem;
						selectedGuid = selectedEquipment.IdentityGuid;
					}

					this.equipmentCollection = data as EquipmentCollectionClass;
					this.EquipmentFilterDropDownSelectedIndexChanged(null, null);

					if (this.equipmentCollection != null
						&& this.equipmentCollection.Count > 0 
						&& this.EquipmentDataGridView.SelectedRows.Count == 0 
						&& this.EquipmentDataGridView.Rows.Count > 0)
					{
						this.EquipmentDataGridView.Rows[0].Selected = true;
					}

					if (selectedGuid != Guid.Empty)
					{
						foreach (DataGridViewRow row in this.EquipmentDataGridView.Rows)
						{
							var checkEquipment = (EquipmentClass) row.DataBoundItem;

							if (checkEquipment.IdentityGuid == selectedGuid)
							{
								this.SelectEquipmentRow(row);
								break;
							}
						}
					}
				}
			}
			catch (ObjectDisposedException)
			{
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void OutButtonClick(object sender, EventArgs e)
		{
			try
			{
				lock (this.PersonDataGridView)
				{
					if (this.PersonDataGridView.SelectedRows.Count > 0)
					{
						var person = (PersonClass)this.PersonDataGridView.SelectedRows[0].DataBoundItem;
						person.Status = PersonClass.STATUS.Out;

						FMChannelHelper.MakeCall<IClientDispatchService>(x => x.ModifyPerson(this.Security, person));
						this.UpdatePersonnelView(this.dataAccess.GetPersonnel());
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void StandButton2Click(object sender, EventArgs e)
		{
			try
			{
				this.StandButtonClick(sender, e);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void StandButtonClick(object sender, EventArgs e)
		{
			try
			{
				if (this.PersonDataGridView.SelectedRows.Count > 0 && this.EquipmentDataGridView.SelectedRows.Count > 0)
				{
					var person = (PersonClass)this.PersonDataGridView.SelectedRows[0].DataBoundItem;
					var equipment = (EquipmentClass)this.EquipmentDataGridView.SelectedRows[0].DataBoundItem;

					if (this.lastFocus == FocusType.Equipment)
					{
						lock (this.PersonDataGridView)
						{
							foreach (DataGridViewRow row in this.PersonDataGridView.Rows)
							{
								var rowPerson = (PersonClass) row.DataBoundItem;

								if (rowPerson.AssignedEquipmentGuid == equipment.MasterRecordGuid)
								{
									person = rowPerson;
									row.Selected = true;
									break;
								}
							}
						}
					}

					lock (this.EquipmentDataGridView)
					{

						var standByForm = new StandbyRegistrationSelectionForm
						                  {
							                  Person = person,
							                  RegistrationIDList = (EquipmentCollectionClass) this.EquipmentDataGridView.DataSource,
							                  InitialSelection = equipment
						                  };

						standByForm.ShowDialog(this);
						equipment = standByForm.SelectedItem;

						if (person != null && equipment != null)
						{
							person.Status = PersonClass.STATUS.STB;
							person.AssignedEquipmentGuid = equipment.MasterRecordGuid;

							FMChannelHelper.MakeCall<IClientDispatchService>(x => x.ModifyPerson(this.Security, person));
						}
					}

					this.UpdatePersonnelView(this.dataAccess.GetPersonnel());
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void RadioButtonClick(object sender, EventArgs e)
		{
			try
			{
				lock (this.RequestDataGridView)
				{
					if (this.RequestDataGridView.SelectedRows.Count > 0)
					{
						DataRowView row = this.RequestDataGridView.GetDataRow(this.RequestDataGridView.SelectedRows[0].Index);
						var radioForm = new RadioFieldForm(new Guid(row["TransactionGuid"].ToString()));
						radioForm.ShowDialog(this);

						this.RefreshRequestView(this.dataAccess.GetTransactions(this.sr).Transactions.Tables[0]);
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void HomeButtonClick(object sender, EventArgs e)
		{
			try
			{
				lock (this.PersonDataGridView)
				{
					if (this.PersonDataGridView.SelectedRows.Count > 0)
					{
						var person = (PersonClass)this.PersonDataGridView.SelectedRows[0].DataBoundItem;

						person.Status = PersonClass.STATUS.In;
						person.AssignedEquipmentGuid = Guid.Empty;
						person.AssignedEquipmentID = String.Empty;

						FMChannelHelper.MakeCall<IClientDispatchService>(x => x.ModifyPerson(this.Security, person));
						this.UpdatePersonnelView(this.dataAccess.GetPersonnel());
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void EquipmentFilterDropDownSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				// All Servicing Units
				EquipmentCollectionClass filteredCollection = this.equipmentCollection;

				if (this.EquipmentFilterDropDown.SelectedIndex == 1)
				{
					// Show Hydrant Service Units Only
					filteredCollection = new EquipmentCollectionClass();

					foreach (EquipmentClass equipment in this.equipmentCollection)
					{
						if (equipment.Type == EQUIPMENT_TYPE.HYDRANT_CART_TYPE)
						{
							filteredCollection.Add(equipment);
						}
					}
				}
				else if (this.EquipmentFilterDropDown.SelectedIndex == 2)
				{
					// Show In-Service Units Only
					filteredCollection = new EquipmentCollectionClass();

					foreach (EquipmentClass equipment in this.equipmentCollection)
					{
						if (equipment.InServiceFlag && equipment.LockedOut == false)
						{
							filteredCollection.Add(equipment);
						}
					}
				}
				else if (this.EquipmentFilterDropDown.SelectedIndex == 3)
				{
					// Show Vehicular Units Only
					filteredCollection = new EquipmentCollectionClass();

					foreach (EquipmentClass equipment in this.equipmentCollection)
					{
						if (equipment.Type == EQUIPMENT_TYPE.TANKER_TYPE || equipment.Type == EQUIPMENT_TYPE.TRAILER_TYPE)
						{
							filteredCollection.Add(equipment);
						}
					}
				}
				else if (this.EquipmentFilterDropDown.Text.Equals("Show Flight-Line Status"))
				{
					// Flight-Line Status Mode

					// Change the Personnel filter
					int index = this.PersonFilterDropDown.FindString("Show Flight-Line Status");
					this.PersonFilterDropDown.SelectedIndex = index;

					// Filter on only assigned equipment
					filteredCollection = new EquipmentCollectionClass();

					foreach (PersonClass person in this.personCollection)
					{
						if (person.AssignedEquipmentGuid != Guid.Empty)
						{
							if (filteredCollection.Find(x => x.MasterRecordGuid == person.AssignedEquipmentGuid) == null)
							{
								var equipment = this.FindEquipment(person.AssignedEquipmentGuid);

								if (equipment != null)
								{
									filteredCollection.Add(equipment);
								}
							}
						}
					}
				}

				filteredCollection.Sort((x, y) => string.Compare(x.Xref, y.Xref, StringComparison.Ordinal));

				this.EquipmentDataGridView.DataSource = filteredCollection;
				this.SetEquipmentFindDataSource(filteredCollection);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void SetEquipmentFindDataSource(EquipmentCollectionClass filteredCollection)
		{
			var sortedList = from equipment in filteredCollection
							 orderby equipment.IssPtNum
							 select equipment;

			var sortedCollection = new EquipmentCollectionClass();

			foreach (var equipment in sortedList)
			{
				sortedCollection.Add(equipment);
			}

			this.EquipmentFindComboBox.DisplayMember = "IssPtNum";
			this.EquipmentFindComboBox.DataSource = sortedCollection;
		}

		/// <summary>
		/// This method returns the EquipmentClass object if the Index
		/// exists in the Form's existing equipmentCollection.  If not,
		/// it returns null.
		/// </summary>
		private EquipmentClass FindEquipment(Guid personGuid)
		{
			foreach (EquipmentClass equipment in this.equipmentCollection)
			{
				if (equipment.MasterRecordGuid == personGuid)
				{
					return equipment;
				}
			}

			return null;
		}

		private void PersonFilterDropDownSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				//Show All Personnel
				PersonCollectionClass filteredCollection = this.personCollection;

				if (this.PersonFilterDropDown.SelectedIndex == 1)
				{
					//Show On-Duty and Standby Personnel
					filteredCollection = new PersonCollectionClass();

					foreach (PersonClass person in this.personCollection)
					{
						if (person.Status != PersonClass.STATUS.Out && person.LockedOut == false)
						{
							filteredCollection.Add(person);
						}
					}
				}
				else if (this.PersonFilterDropDown.SelectedIndex == 2)
				{
					//Show On-Duty Personnel Only
					filteredCollection = new PersonCollectionClass();

					foreach (PersonClass person in this.personCollection)
					{
						if (person.Status == PersonClass.STATUS.In && person.LockedOut == false)
						{
							filteredCollection.Add(person);
						}
					}
				}
				else if (this.PersonFilterDropDown.SelectedIndex == 3)
				{
					//Show Standby Personnel Only
					filteredCollection = new PersonCollectionClass();

					foreach (PersonClass person in this.personCollection)
					{
						if (person.Status == PersonClass.STATUS.STB && person.LockedOut == false)
						{
							filteredCollection.Add(person);
						}
					}
				}
				else if (this.PersonFilterDropDown.Text.Equals("Show Flight-Line Status"))
				{
					// Flight-line status mode

					// Change the equipment filter to flight-line status
					this.EquipmentFilterDropDown.SelectedIndex = this.EquipmentFilterDropDown.FindString("Show Flight-Line Status");

					// Filter to show only personnel with equipment assigned
					filteredCollection = new PersonCollectionClass();

					foreach (PersonClass person in this.personCollection)
					{
						if (person.AssignedEquipmentGuid != Guid.Empty)
						{
							filteredCollection.Add(person);
						}
					}
				}

				this.PersonDataGridView.DataSource = new SortableBindingList<PersonClass>(filteredCollection);
				this.PersonDataGridView.Sort(this.PersonDataGridView.Columns[0], ListSortDirection.Ascending);

				this.SetPersonFindDataSource(filteredCollection);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void SetPersonFindDataSource(PersonCollectionClass filteredCollection)
		{
			var sortedList = from person in filteredCollection
							 orderby person.LastName
							 select person;

			var sortedCollection = new PersonCollectionClass();

			foreach (var person in sortedList)
			{
				sortedCollection.Add(person);
			}

			this.PersonFindComboBox.DisplayMember = "FullName";
			this.PersonFindComboBox.ValueMember = "ID";
			this.PersonFindComboBox.DataSource = sortedCollection;
		}

		private void CancelDispatchButtonClick(object sender, EventArgs e)
		{
			try
			{
				Cursor currentCursor = this.Cursor;
				this.Cursor = Cursors.WaitCursor;

				lock (this.RequestDataGridView)
				{
					foreach (DataGridViewRow viewRow in this.RequestDataGridView.SelectedRows)
					{
						DataRowView row = this.RequestDataGridView.GetDataRow(viewRow.Index);
						var transID = (string) row["TransID"];
						TransactionDO transaction = this.GetTransaction(transID);
						this.UndispatchTransaction(transaction);
					}
				}

				this.RefreshRequestView(this.dataAccess.GetTransactions(this.sr).Transactions.Tables[0]);
				this.Cursor = currentCursor;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void DispatchButtonClick(object sender, EventArgs e)
		{
			try
			{
				// If no selections, stop
				if (this.EquipmentDataGridView.SelectedRows.Count == 0
					|| this.PersonDataGridView.SelectedRows.Count == 0
					|| this.RequestDataGridView.SelectedRows.Count == 0)
				{
					return;
				}

				var equipment = (EquipmentClass)this.EquipmentDataGridView.SelectedRows[0].DataBoundItem;
				var person = (PersonClass)this.PersonDataGridView.SelectedRows[0].DataBoundItem;

				equipment = LoadEquipment(equipment.IdentityGuid);

				if (this.CheckEquipmentAndOperator(equipment, person) == false)
				{
					return;
				}

				this.Transactions = new List<TransactionDO>();

				// Get all the transactions
				lock (this.RequestDataGridView)
				{
					foreach (DataGridViewRow row in this.RequestDataGridView.SelectedRows)
					{
						var dataRow = this.RequestDataGridView.GetDataRow(row.Index);

						var transID = (string) dataRow["TransID"];
						TransactionDO transaction = this.GetTransaction(transID);

						this.Transactions.Add(transaction);
					}
				}

				foreach (TransactionDO trans in this.Transactions)
				{
					TransactionCheckResult result = this.CheckAdditiveFlag(trans, equipment);

					if (result == TransactionCheckResult.Cancel)
					{
						return;
					}

					if (result == TransactionCheckResult.Exempted)
					{
						break;
					}

				}

				foreach (TransactionDO trans in this.Transactions)
				{
					TransactionCheckResult result = this.CheckDefuelStatus(trans, equipment);

					if (result == TransactionCheckResult.Cancel)
					{
						return;
					}

					if (result == TransactionCheckResult.Exempted)
					{
						break;
					}
				}

				foreach (TransactionDO trans in this.Transactions)
				{
					TransactionCheckResult result = this.CheckRefuelStatus(trans, equipment);

					if (result == TransactionCheckResult.Cancel)
					{
						return;
					}

					if (result == TransactionCheckResult.Exempted)
					{
						break;
					}
				}

				bool fixSubsequentGrade = false;

				foreach (TransactionDO trans in this.Transactions)
				{
					TransactionCheckResult result = this.CheckGrade(trans, equipment, fixSubsequentGrade);

					if (result == TransactionCheckResult.Cancel)
					{
						return;
					}

					if (result == TransactionCheckResult.Exempted)
					{
						fixSubsequentGrade = true;
					}
				}

				// Dispatch the transactions
				DateTimeOffset dispatchTime = this.timeConverter.Now();

				Cursor currentCursor = this.Cursor;
				this.Cursor = Cursors.WaitCursor;
				this.PerformDispatchTransactions(person, equipment, this.Transactions, dispatchTime);
				this.Cursor = currentCursor;

				this.RefreshRequestView(this.dataAccess.GetTransactions(this.sr).Transactions.Tables[0]);
			}

			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private bool CheckEquipmentAndOperator(EquipmentClass equipment, PersonClass person)
		{
			if (equipment.LockedOut)
			{
				MessageBox.Show(this, 
								"Servicing unit is locked-out.  Cannot dispatch this vehicle.", 
								"Dispatch", 
								MessageBoxButtons.OK, 
								MessageBoxIcon.Error);
				return false;
			}

			if (person.LockedOut)
			{
				string message = String.Format("[{0},{1}] is locked-out.  Cannot dispatch this agent.", person.LastName, person.FirstName);
				MessageBox.Show(this, message, "Dispatch", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}

			if (String.IsNullOrEmpty(equipment.IssPtNum))
			{
				MessageBox.Show(this, 
								"IssPtNum for Vehicle is blank.  Cannot dispatch this vehicle.", 
								"Dispatch", 
								MessageBoxButtons.OK, 
								MessageBoxIcon.Error);
				return false;
			}

			// Don't dispatch equipment if it is already assigned to another operator
			foreach (PersonClass anOperator in this.personCollection)
			{
				if (anOperator.IdentityGuid != person.IdentityGuid)
				{
					if (equipment.MasterRecordGuid == anOperator.AssignedEquipmentGuid)
					{
						const string Message = "Vehicle is already associated with another operator and may not be assigned to this operator.\n" +
						                       "Please choose a different vehicle or different operator and try again.";
						MessageBox.Show(this, Message, "Dispatch", MessageBoxButtons.OK, MessageBoxIcon.Error);

						return false;
					}
				}
			}

			// If selected equipment is not in service, give a warning
			if (equipment.InServiceFlag == false)
			{
				DialogResult result = MessageBox.Show(this,
													"This servicing unit is out of service.  Dispatch anyway?",
													"Dispatch",
													MessageBoxButtons.YesNo,
													MessageBoxIcon.Warning);

				if (result == DialogResult.No)
				{
					return false;
				}
			}

			// Give warning if operator is OUT
			if (person.Status == PersonClass.STATUS.Out)
			{
				DialogResult result = MessageBox.Show(this,
													"The operator is currently OUT.  Dispatch anyway and update status?",
													"Dispatch",
													MessageBoxButtons.YesNo,
													MessageBoxIcon.Warning);

				if (result == DialogResult.No)
				{
					return false;
				}
			}

			if (equipment._QCDate.Value.Date < this.timeConverter.Today())
			{
				DialogResult result = MessageBox.Show(this,
													"Equipment is overdue QC Checkup.  Dispatch Anyway?",
													"Dispatch",
													MessageBoxButtons.YesNo,
													MessageBoxIcon.Warning);

				if (result == DialogResult.No)
				{
					return false;
				}
			}

			if (person.AssignedEquipmentGuid != Guid.Empty
				&& person.AssignedEquipmentGuid != equipment.MasterRecordGuid)
			{
				string message = String.Format("{0},{1} is currently assigned to vehicle {2}.  Reassign {0},{1} to {3}?",
					person.LastName, person.FirstName, person.AssignedEquipmentID, equipment.ID);

				DialogResult result = MessageBox.Show(this, message, "Dispatch", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

				if (result == DialogResult.No)
				{
					this.SelectEquipment(person.AssignedEquipmentGuid);
					return false;
				}
			}

			// Have to a full get of the Equipment & Person object so we get all the collections we need to check
			person = FMChannelHelper.MakeCall<IClientDispatchService, PersonClass>(x => x.GetPerson(this.Security, person.IdentityGuid));

			equipment = LoadEquipment(equipment.IdentityGuid);

			var equipmentType =
				FMChannelHelper.MakeCall<IClientDispatchService, EquipmentTypeClass>(
					x => x.GetEquipmentTypeByGuid(this.Security, equipment.EquipmentTypeGuid));

			if (equipmentType.ReqQualificationsCollection.Count > 0)
			{
				foreach (QualificationMapClass qualification in equipmentType.ReqQualificationsCollection)
				{
					QualificationMapClass qualificationRecord = this.FindQualificationRecord(qualification, person.QualificationCollection);

					if (qualificationRecord == null)
					{
						string message = String.Format("{0},{1} does not have the '{2}' required qualification.  Dispatch anyway?",
							person.LastName, person.FirstName, qualification.ID);

						DialogResult result = MessageBox.Show(this, message, "Dispatch", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
						
						if (result == DialogResult.No)
						{
							return false;
						}
					}
					else if (qualificationRecord.ExpirationDate.Value < this.timeConverter.Today())
					{
						string message = String.Format("{0},{1} has the '{2}' required qualification but it has expired.  Dispatch anyway?",
							person.LastName, person.FirstName, qualification.ID);

						DialogResult result = MessageBox.Show(this, message, "Dispatch", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
						
						if (result == DialogResult.No)
						{
							return false;
						}
					}
				}
			}

			if (equipmentType.ReqTrainingCollection.Count > 0)
			{
				foreach (QualificationMapClass training in equipmentType.ReqTrainingCollection)
				{
					QualificationMapClass trainingRecord = this.FindQualificationRecord(training, person.TrainingCollection);

					if (trainingRecord == null)
					{
						string message = String.Format("{0},{1} does not have the '{2}' required training.  Dispatch anyway?",
							person.LastName, person.FirstName, training.ID);

						DialogResult result = MessageBox.Show(this, message, "Dispatch", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
						
						if (result == DialogResult.No)
						{
							return false;
						}
					}
					else if (trainingRecord.ExpirationDate.Value < this.timeConverter.Today())
					{
						string message = String.Format("{0},{1} has the '{2}' required training but it has expired.  Dispatch anyway?",
							person.LastName, person.FirstName, training.ID);

						DialogResult result = MessageBox.Show(this, message, "Dispatch", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
						
						if (result == DialogResult.No)
						{
							return false;
						}
					}
				}
			}

			if (equipment.TagAndLicenseCollection.Count > 0)
			{
				foreach (QualificationMapClass tagLicense in equipment.TagAndLicenseCollection)
				{
					if (tagLicense.ExpirationDate.Value < this.timeConverter.Today())
					{
						string message = String.Format("Tag/License [{0}] for servicing unit has expired.  Dispatch anyway?", tagLicense.ID);

						DialogResult result = MessageBox.Show(this, message, "Dispatch", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
						
						if (result == DialogResult.No)
						{
							return false;
						}
					}
				}
			}

			if (equipment.TestAndInspectionCollection.Count > 0)
			{
				foreach (QualificationMapClass testInspection in equipment.TestAndInspectionCollection)
				{
					if (testInspection.ExpirationDate.Value < this.timeConverter.Today())
					{
						string message = String.Format("Test/Inspection [{0}] for servicing unit has expired.  Dispatch anyway?", testInspection.ID);

						DialogResult result = MessageBox.Show(this, message, "Dispatch", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
						
						if (result == DialogResult.No)
						{
							return false;
						}
					}
				}
			}

			// If the equiment is in service, the last quality tag should not be taken into account here
			var tag =
				FMChannelHelper.MakeCall<IClientDispatchService, EquipmentQualityTagLogClass>(
					x => x.GetMostRecentQualityTagLogByEquipmentID(this.Security, equipment.ID));

			if (tag != null
				&& tag.IdentityGuid != Guid.Empty
				&& string.IsNullOrEmpty(tag.RemovedBy))
			{
				if (tag.QualityTagGuid != Guid.Empty)
				{
					var qualityTag = FMChannelHelper.MakeCall<IClientDispatchService, QualityTagClass>(x => x.GetQualityTagByGuid(this.Security, tag.QualityTagGuid));

					if (qualityTag.Severity == QUALITY_SEVERITY_LEVELS.CAUTION
						|| qualityTag.Severity == QUALITY_SEVERITY_LEVELS.WARNING)
					{
						string message = String.Format("The servicing unit has a {0} tag.  Do you still wish to send this servicing unit?",
							Enum.GetName(typeof(QUALITY_SEVERITY_LEVELS), equipment.QualityTag.Severity));

						DialogResult result = MessageBox.Show(this, message, "Dispatch", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
						
						if (result == DialogResult.No)
						{
							return false;
						}
					}

					if (qualityTag.Severity == QUALITY_SEVERITY_LEVELS.DANGER)
					{
						const string Message = "The servicing unit has a DANGER tag.  This service unit cannot be dispatched.";
						MessageBox.Show(this, Message, "Dispatch", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return false;
					}
				}
			}

			return true;
		}

		private TransactionCheckResult CheckDefuelStatus(TransactionDO transaction, EquipmentClass equipment)
		{
			if (transaction.TransTypeID == TransactionTypes.T4_SecondaryDefuel
				&& equipment.FuelingType != FUELING_TYPES.DEFUELER)
			{
				const string Message = "The fueling status of the unit dispatched does not match the type of request.  Dispatch anyway?";
				DialogResult result = MessageBox.Show(this, Message, "Dispatch", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				
				if (result == DialogResult.No)
				{
					return TransactionCheckResult.Cancel;
				}

				return TransactionCheckResult.Exempted;
			}

			return TransactionCheckResult.Ok;
		}

		private TransactionCheckResult CheckRefuelStatus(TransactionDO transaction, EquipmentClass equipment)
		{
			if (transaction.TransTypeID == TransactionTypes.T6_SecondaryDisbursement
				&& equipment.FuelingType != FUELING_TYPES.REFUELER)
			{
				const string Message = "The fueling status of the unit dispatched does not match the type of request.  Dispatch anyway?";
				DialogResult result = MessageBox.Show(this, Message, "Dispatch", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				
				if (result == DialogResult.No)
				{
					return TransactionCheckResult.Cancel;
				}

				return TransactionCheckResult.Exempted;
			}

			return TransactionCheckResult.Ok;
		}

		private TransactionCheckResult CheckGrade(TransactionDO transaction, EquipmentClass equipment, bool fixWithoutPrompt)
		{
			if (transaction.LineItems[0].ProductGuid == Guid.Empty
				|| equipment.ProductGuid != transaction.LineItems[0].ProductGuid)
			{
				DialogResult result;

				if (fixWithoutPrompt)
				{
					// need to fix all transactions that the grade does not match since they selected yes on the first one found
					result = DialogResult.Yes;
				}
				else
				{
					this.cachedProductForCheck = null;

					result = MessageBox.Show(this,
						"Grade in selected servicing unit and selected Request conflict.  Dispatch anyway?",
						"Dispatch",
						MessageBoxButtons.YesNo,
						MessageBoxIcon.Warning);
				}

				if (result == DialogResult.No)
				{
					return TransactionCheckResult.Cancel;
				}

				if (equipment.ProductGuid != Guid.Empty)
				{
					if (this.cachedProductForCheck == null)
					{
						this.cachedProductForCheck = FMChannelHelper.MakeCall<IClientDispatchService, ProductClass>(
															x => x.GetProduct(this.sr.Security, equipment.ProductGuid));
					}

					if (this.cachedProductForCheck != null)
					{
						//set the product in the transaction to what is on the truck. 
						transaction.LineItems[0].Product = this.cachedProductForCheck.ID;
						transaction.LineItems[0].ProductCode = this.cachedProductForCheck.Code;
						transaction.LineItems[0].ProductPrice = Convert.ToDouble(this.cachedProductForCheck.Price);
						transaction.LineItems[0].ProductType = this.cachedProductForCheck.ProductType.ToString();
						transaction.LineItems[0].ProductGuid = this.cachedProductForCheck.MasterRecordGuid;
					}
				}

				return TransactionCheckResult.Exempted;
			}

			return TransactionCheckResult.Ok;
		}

		private TransactionCheckResult CheckAdditiveFlag(TransactionDO transaction, EquipmentClass equipment)
		{
			if (equipment.FuelAdditiveFlag != transaction.Flag04)
			{
				DialogResult result = MessageBox.Show(this,
					"Attention, either the Servicing Unit or the Fuel Request is missing the Fuel Additive Flag.  Dispatch anyway?",
					"Dispatch",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Warning);

				if (result == DialogResult.No)
				{
					return TransactionCheckResult.Cancel;
				}

				return TransactionCheckResult.Exempted;
			}

			return TransactionCheckResult.Ok;
		}

		private bool CheckTransactionsForDispatch(EquipmentClass equipment, PersonClass person, TransactionDO transaction)
		{
			if (transaction == null)
			{
				throw new ArgumentException("Transaction not found");
			}

			// Give warning message if dispatching to cross type
			if (this.CheckDefuelStatus(transaction, equipment) == TransactionCheckResult.Cancel)
			{
				return false;
			}

			if (this.CheckRefuelStatus(transaction, equipment) == TransactionCheckResult.Cancel)
			{
				return false;
			}

			if (this.CheckGrade(transaction, equipment, false) == TransactionCheckResult.Cancel)
			{
				return false;
			}

			if (this.CheckAdditiveFlag(transaction, equipment) == TransactionCheckResult.Cancel)
			{
				return false;
			}

			return true;
		}

		private void DispatchTransaction(PersonClass person, EquipmentClass equipment, TransactionDO transaction)
		{
			if (this.CheckEquipmentAndOperator(equipment, person))
			{
				if (this.CheckTransactionsForDispatch(equipment, person, transaction))
				{
					this.Transactions = new List<TransactionDO>();
					this.Transactions.Add(transaction);
					this.PerformDispatchTransactions(person, equipment, this.Transactions, this.timeConverter.Now());
				}
			}
		}

		private void PerformDispatchTransactions(PersonClass person, 
												EquipmentClass equipment, 
												List<TransactionDO> transactions, 
												DateTimeOffset dispatchTime)
		{
			// Dispatch
			foreach (TransactionDO transaction in transactions)
			{
				transaction.OperatorPersonnelGuid = person.MasterRecordGuid;
				transaction.OperatorID = person.ID;
				transaction.OperatorName = person.FullName;

				LineItemDO lineItem = transaction.LineItems[0];

				switch (transaction.TransTypeID)
				{
					case TransactionTypes.T3_PrimaryDefuel:
					case TransactionTypes.T4_SecondaryDefuel:
					case TransactionTypes.T7_FillStand:
						transaction.DestinationEQ1 = new EquipmentDO(equipment) { RegistrationID = equipment.ID };
						lineItem.DestinationEQ = new EquipmentDO(equipment) { RegistrationID = equipment.ID };
						break;

					case TransactionTypes.T5_PrimaryDisbursement:
					case TransactionTypes.T6_SecondaryDisbursement:
					case TransactionTypes.T10_Unload:
					case TransactionTypes.T12_InventoryNotAffected:
						transaction.SourceEQ1 = new EquipmentDO(equipment) { RegistrationID = equipment.ID };
						lineItem.SourceEQ = new EquipmentDO(equipment) { RegistrationID = equipment.ID };
						break;

					default:
						throw new ApplicationException("Unhandled transaction type passed to dispatch.");
				}

				transaction.Status = TransactionStatus.Dispatched;

				foreach (LineItemDO lineitem in transaction.LineItems)
				{
					lineitem.Status = TransactionStatus.Dispatched;

					foreach (SubLineItemDO sublineitem in lineitem.SubLineItems)
					{
						sublineitem.Status = TransactionStatus.Dispatched;
					}
				}

				person.AssignedEquipmentID = equipment.ID;
				person.AssignedEquipmentGuid = equipment.MasterRecordGuid;
				person.Status = PersonClass.STATUS.In;
				person.UpdatedDate = this.timeConverter.Now();

				transaction.DispatchedDateTime = dispatchTime;

				// Set IssPt and IssPtNum
				transaction.IssuePoint = equipment.IssPt;
				transaction.IssuePointNumber = equipment.IssPtNum;
			}

			this.SaveTransaction(transactions, person);
		}

		private void SelectEquipment(Guid equipmentGuid)
		{
			lock (this.EquipmentDataGridView)
			{
				foreach (DataGridViewRow row in this.EquipmentDataGridView.Rows)
				{
					var equipment = (EquipmentClass) row.DataBoundItem;
					
					if (equipment.MasterRecordGuid == equipmentGuid)
					{
						this.SelectEquipmentRow(row);
					}
				}
			}
		}

		private QualificationMapClass FindQualificationRecord(QualificationMapClass qualification, QualificationMapCollectionClass collection)
		{
			foreach (QualificationMapClass personQualification in collection)
			{
				if (personQualification.AssignedGuid == qualification.AssignedGuid)
				{
					return personQualification;
				}
			}

			return null;
		}

		private void FillButtonClick(object sender, EventArgs e)
		{
			try
			{
				// Requires Equipment and Person selection
				if (this.EquipmentDataGridView.SelectedRows.Count == 0
					|| this.PersonDataGridView.SelectedRows.Count == 0)
				{
					return;
				}

				// Get Equipment and Person objects
				var equipment = (EquipmentClass)this.EquipmentDataGridView.SelectedRows[0].DataBoundItem;
				var person = (PersonClass)this.PersonDataGridView.SelectedRows[0].DataBoundItem;

				equipment = LoadEquipment(equipment.IdentityGuid);

				// Get the transaction alias
				var transactionAlias =
					FMChannelHelper.MakeCall<IClientDispatchService, TransactionAliasClass>(
						x => x.GetTransactionAliasFromAliasId(this.Security, this.fillStandTransactionAlias, true));

				// Equipment must be of a type authorized for the Fillstand alias
				if (this.CheckEquipmentType(transactionAlias, equipment) == false)
				{
					MessageBox.Show(this, 
									"Servicing unit is not authorized for Fillstand transaction.  Cannot create fillstand.", 
									"Dispatch", 
									MessageBoxButtons.OK);
					return;
				}

				// Create a fillstand transaction
				var site = FMChannelHelper.MakeCall<IClientDispatchService, SiteClass>(
					x => x.GetSite(this.Security, this.Security.SiteGuid));

				var localTimeConverter = new SiteTimeConverter(site);

				var transaction = new TransactionDO
				                  {
					                  TransactionDateTime = localTimeConverter.Now(),
					                  TransID = FuelsManagerId.NewId(),
					                  Site = this.Security.SiteID,
					                  SiteGuid = this.Security.SiteGuid,
					                  Alias = transactionAlias.ID,
					                  TransTypeID = transactionAlias.TransTypeID,
					                  TransactionAliasGuid = transactionAlias.MasterRecordGuid,
									  DocumentNumber = this.GenerateDocumentNumbers(transactionAlias.TransTypeID)
				                  };

				transaction.LineItems.Add(new LineItemDO());

				var inventoryDateSR = new InventoryDateSR { Security = this.Security, CurrentSiteGuid = this.Security.SiteGuid };

				var inventoryDateDO =
					FMChannelHelper.MakeCall<IClientDispatchService, InventoryDateDO>(x => x.ProcessInventoryDateServiceRequest(inventoryDateSR));

				transaction.InventoryDate = inventoryDateDO.InventoryDate;
				transaction.TransactionDateTime = localTimeConverter.Now();
				transaction.OriginApplication = TransactionOrigin.Dispatch;
				transaction.SubmittedToAccounting = false;
				transaction.Status = TransactionStatus.Requested;
				transaction.RequestedDateTime = localTimeConverter.Now();

				var unitsHelper = new UnitsHelperClass(this.Security, site, transactionAlias, null);
				unitsHelper.SetUnits(transaction, 0);

				var managerCollection =
					FMChannelHelper.MakeCall<IClientDispatchService, CompanyCollectionClass>(
						x => x.EnumerateCompanyByRole(this.Security, COMPANY_ROLE.MANAGER));

				if (managerCollection.Count == 0)
				{
					throw new Exception("No Manager");
				}

				if (managerCollection.Count > 1)
				{
					string strMgrs = string.Empty;

					foreach (CompanyClass manager in managerCollection)
					{
						if (strMgrs.Length > 0)
						{
							strMgrs += ", ";
						}
						strMgrs += string.Format("{0}", manager.Name);
					}
					string errorMsg = String.Format("Multiple manager are not allowed. {0} managers were found. They are {1}.", 
													managerCollection.Count, strMgrs);

					throw new Exception(errorMsg);
				}

				transaction.ManagerID = managerCollection[0].ID;
				transaction.ManagerCode = managerCollection[0].Code;
				transaction.ManagerCompanyGuid = managerCollection[0].MasterRecordGuid;

				var ownerCollection =
					FMChannelHelper.MakeCall<IClientDispatchService, CompanyCollectionClass>(
						x => x.EnumerateCompanyByRole(this.Security, COMPANY_ROLE.OWNER));

				if (ownerCollection.Count == 0)
				{
					throw new Exception("No Owner");
				}

				if (ownerCollection.Count > 1)
				{
					throw new Exception("Multiple Owners");
				}

				transaction.OwnerID = ownerCollection[0].ID;
				transaction.OwnerCode = ownerCollection[0].Code;
				transaction.OwnerCompanyGuid = ownerCollection[0].MasterRecordGuid;
				transaction.Number02 = Convert.ToDouble(FuelRequestForm.REQUEST_TYPE.FillStand);

				transaction.Notes = string.Empty;

				ProductClass product;

				if (equipment.ProductGuid != Guid.Empty)
				{
					product = FMChannelHelper.MakeCall<IClientDispatchService, ProductClass>(x => x.GetProduct(this.Security, equipment.ProductGuid));

					transaction.LineItems[0].Product = product.ID;
					transaction.LineItems[0].ProductCode = ProductClass.ProductTypeID(product.ProductType);
					transaction.LineItems[0].ProductGuid = product.MasterRecordGuid;
					unitsHelper.Product = product;
				}
				else
				{
					throw new Exception("Selected equipment has no product assignment");
				}

				unitsHelper.SetUnits(transaction.LineItems[0], 0, product);

				transaction.LineItems[0].Quantity = new QuantityDO(0, 0, 0, 0);

				// Registration ID
				var equipmentDO = new EquipmentDO(equipment);
				transaction.DestinationEQ1 = equipmentDO;
				transaction.DestinationEQ1.RegistrationID = equipment.ID;
				transaction.LineItems[0].DestinationEQ = new EquipmentDO(equipment);
				transaction.LineItems[0].PartialFill = false;

				// Dispatch the transaction
				this.DispatchTransaction(person, equipment, transaction);
				this.AddTransactionToRequestGrid(transaction);
				this.RefreshRequestView(this.dataAccess.GetTransactions(this.sr).Transactions.Tables[0]);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddTransactionToRequestGrid(TransactionDO transaction)
		{
			this.Transactions = new List<TransactionDO> { transaction };
			this.sr.TransactionList.Add(transaction.TransID);
			this.sr.AliasNames.Add(transaction.Alias);
			this.UpdateRequestView();
		}

		private bool CheckEquipmentType(TransactionAliasClass transactionAlias, EquipmentClass equipment)
		{
			EQUIPMENT_TYPE[] types = transactionAlias.GetEquipmentTypes(true, 1);

			foreach (EQUIPMENT_TYPE type in types)
			{
				if (type == equipment.Type)
				{
					return true;
				}
			}

			return false;
		}

		private EquipmentClass LoadEquipment(Guid equipmentGuid)
		{
			return FMChannelHelper.MakeCall<IClientDispatchService, EquipmentClass>(x => x.GetEquipment(this.Security, equipmentGuid));
		}

		private void RtbButtonClick(object sender, EventArgs e)
		{
			try
			{
				// Requires Equipment and Person selection
				if (this.EquipmentDataGridView.SelectedRows.Count == 0
					|| this.PersonDataGridView.SelectedRows.Count == 0)
				{
					return;
				}

				// Get Equipment and Person objects
				var equipment = (EquipmentClass)this.EquipmentDataGridView.SelectedRows[0].DataBoundItem;
				var person = (PersonClass)this.PersonDataGridView.SelectedRows[0].DataBoundItem;

				equipment = this.LoadEquipment(equipment.IdentityGuid); //fully load the equpment

				// Get the transaction alias
				var transactionAlias =
					FMChannelHelper.MakeCall<IClientDispatchService, TransactionAliasClass>(
						x => x.GetTransactionAliasFromAliasId(this.Security, this.returnToBulkTransactionAlias, true));

				// Equipment must be of a type authorized for the Fillstand alias
				if (this.CheckEquipmentType(transactionAlias, equipment) == false)
				{
					MessageBox.Show(this, 
									"Servicing unit is not authorized for Return to Bulk transaction.  Cannot create RTB.", 
									"Dispatch", 
									MessageBoxButtons.OK);
					return;
				}

				// Create a fillstand transaction
				var site = FMChannelHelper.MakeCall<IClientDispatchService, SiteClass>(x => x.GetSite(this.Security, this.Security.SiteGuid));
				var localTimeConverter = new SiteTimeConverter(site);

				var transaction = new TransactionDO
				                  {
					                  TransactionDateTime = localTimeConverter.Now(),
					                  TransID = FuelsManagerId.NewId(),
					                  Site = this.Security.SiteID,
					                  SiteGuid = this.Security.SiteGuid,
					                  Alias = transactionAlias.ID,
					                  TransTypeID = transactionAlias.TransTypeID,
					                  TransactionAliasGuid = transactionAlias.MasterRecordGuid,
					                  DocumentNumber = this.GenerateDocumentNumbers(transactionAlias.TransTypeID)
				                  };

				transaction.LineItems.Add(new LineItemDO());

				var inventoryDateSR = new InventoryDateSR { Security = this.Security, CurrentSiteGuid = this.Security.SiteGuid };

				var inventoryDateDO =
					FMChannelHelper.MakeCall<IClientDispatchService, InventoryDateDO>(x => x.ProcessInventoryDateServiceRequest(inventoryDateSR));

				transaction.InventoryDate = inventoryDateDO.InventoryDate;
				transaction.TransactionDateTime = localTimeConverter.Now();
				transaction.OriginApplication = TransactionOrigin.Dispatch;
				transaction.SubmittedToAccounting = false;
				transaction.Status = TransactionStatus.Requested;
				transaction.RequestedDateTime = localTimeConverter.Now();

				var unitsHelper = new UnitsHelperClass(this.Security, site, transactionAlias, null);
				unitsHelper.SetUnits(transaction, 0);

				var managerCollection =
					FMChannelHelper.MakeCall<IClientDispatchService, CompanyCollectionClass>(
						x => x.EnumerateCompanyByRole(this.Security, COMPANY_ROLE.MANAGER));

				if (managerCollection.Count == 0)
				{
					throw new Exception("No Manager");
				}

				if (managerCollection.Count > 1)
				{
					string strMgrs = string.Empty;

					foreach (CompanyClass manager in managerCollection)
					{
						if (strMgrs.Length > 0)
						{
							strMgrs += ", ";
						}
						strMgrs += string.Format("{0}", manager.Name);
					}

					string errorMsg = String.Format("Multiple managers are not allowed. {0} managers were found. They are {1}.", 
													managerCollection.Count, strMgrs);

					throw new Exception(errorMsg);
				}

				transaction.ManagerID = managerCollection[0].ID;
				transaction.ManagerCode = managerCollection[0].Code;
				transaction.ManagerCompanyGuid = managerCollection[0].MasterRecordGuid;

				var ownerCollection =
					FMChannelHelper.MakeCall<IClientDispatchService, CompanyCollectionClass>(
						x => x.EnumerateCompanyByRole(this.Security, COMPANY_ROLE.OWNER));

				if (ownerCollection.Count == 0)
				{
					throw new Exception("No Owner");
				}

				if (ownerCollection.Count > 1)
				{
					throw new Exception("Multiple Owners");
				}

				transaction.OwnerID = ownerCollection[0].ID;
				transaction.OwnerCode = ownerCollection[0].Code;
				transaction.OwnerCompanyGuid = ownerCollection[0].MasterRecordGuid;
				transaction.Number02 = Convert.ToDouble(FuelRequestForm.REQUEST_TYPE.FillStand);

				transaction.Notes = string.Empty;

				ProductClass product;

				if (equipment.ProductGuid != Guid.Empty)
				{
					product = FMChannelHelper.MakeCall<IClientDispatchService, ProductClass>(x => x.GetProduct(this.Security, equipment.ProductGuid));
					transaction.LineItems[0].Product = product.ID;
					transaction.LineItems[0].ProductCode = ProductClass.ProductTypeID(product.ProductType);
					transaction.LineItems[0].ProductGuid = product.MasterRecordGuid;
					unitsHelper.Product = product;
				}
				else
				{
					throw new Exception("Selected equipment has no product assignment");
				}

				unitsHelper.SetUnits(transaction.LineItems[0], 0, product);
				transaction.LineItems[0].Quantity = new QuantityDO(0, 0, 0, 0);

				// Registration ID
				var equipmentDO = new EquipmentDO(equipment);
				transaction.SourceEQ1 = equipmentDO;
				transaction.SourceEQ1.RegistrationID = equipment.ID;
				transaction.LineItems[0].SourceEQ = new EquipmentDO(equipment);

				// Dispatch the transaction
				this.DispatchTransaction(person, equipment, transaction);
				this.AddTransactionToRequestGrid(transaction);
				this.RefreshRequestView(this.dataAccess.GetTransactions(this.sr).Transactions.Tables[0]);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void PersonFindComboBoxKeyPress(object sender, KeyPressEventArgs e)
		{
			if (!(this.UpdatedItemInList((ComboBox) sender, e)))
			{
				Console.Beep();
				e.Handled = true;
			}
		}

		private void GetColumnPositionsForGrid1()
		{
			// get grid1 positions
			string appConfigItem = this.Security.UserID + "DispatchGrid1ColumnPositions";
			int startPosistion = 0;

			// check if there are configurations for this user
			string fileColumnPositions = ConfigurationManager.AppSettings[appConfigItem];

			if (fileColumnPositions != null)
			{
				// data exists so read into the array
				// there are 22 columns seperated by a semi-colon so parse the received string into the interger array
				for (int iLoop = 0; iLoop < Grid1Numcols; iLoop++)
				{
					int endPosistion = fileColumnPositions.IndexOf(';', startPosistion);
					this.dispatchGrid1ColumnPositions[iLoop] = 
								Convert.ToInt32(fileColumnPositions.Substring(startPosistion, endPosistion - startPosistion));
					startPosistion = endPosistion + 1;
				}
			}
			else
			{
				// no data so just set at the default
				for (int iLoop = 0; iLoop < Grid1Numcols; iLoop++)
				{
					this.dispatchGrid1ColumnPositions[iLoop] = iLoop;
				}
			}
		}

		private void StoreGrid1ColumnPositions()
		{
			string appConfigItem = this.Security.UserID + "DispatchGrid1ColumnPositions";
			string combinedColumnPositionString = string.Empty;

			// dataGridView1ColumnPositions
			// check if there are configurations for this user
			Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			string fileColumnPositions = ConfigurationManager.AppSettings[appConfigItem];

			// check if the data already exists and delete it before we add it
			if (fileColumnPositions != null)
			{
				// data exists so read into the array
				config.AppSettings.Settings.Remove(appConfigItem);
			}

			// build up a string with the values seperated by semi-colons
			for (int iLoop = 0; iLoop < Grid1Numcols; iLoop++)
			{
				combinedColumnPositionString += this.dispatchGrid1ColumnPositions[iLoop];
				combinedColumnPositionString += ";";
			}

			// write the data to the config file
			config.AppSettings.Settings.Add(appConfigItem, combinedColumnPositionString);

			// Save the configuration file.
			config.Save(ConfigurationSaveMode.Modified);

			// Force a reload of a changed section.
			ConfigurationManager.RefreshSection("appSettings");
		}

		private void GetColumnWidthsForGrid1()
		{
			// get grid1 positions
			string appConfigItem = this.Security.UserID + "DispatchGrid1ColumnWidths";
			int startPosistion = 0;

			// check if there are configurations for this user
			string fileColumnPositions = ConfigurationManager.AppSettings[appConfigItem];

			if (fileColumnPositions != null)
			{
				// data exists so read into the array
				// there are 22 columns seperated by a semi-colon so parse the received string into the interger array
				for (int iLoop = 0; iLoop < Grid1Numcols; iLoop++)
				{
					int endPosistion = fileColumnPositions.IndexOf(';', startPosistion);
					this.dispatchGrid1ColumnWidths[iLoop] = 
								Convert.ToInt32(fileColumnPositions.Substring(startPosistion, endPosistion - startPosistion));
					startPosistion = endPosistion + 1;
				}
			}
			else
			{
				// no data so just set at the default
				this.SetColumnWidthsForGrid1FromGrid();
			}
		}

		private void SetColumnWidthsForGrid1FromGrid()
		{
			for (int iLoop = 0; iLoop < Grid1Numcols; iLoop++)
			{
				this.dispatchGrid1ColumnWidths[iLoop] = this.EquipmentDataGridView.Columns[iLoop].Width;
			}
		}

		private void StoreGrid1ColumnWidths()
		{
			string appConfigItem = this.Security.UserID + "DispatchGrid1ColumnWidths";
			string combinedColumnPositionString = string.Empty;

			// dataGridView1ColumnPositions
			// check if there are configurations for this user
			Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			string fileColumnPositions = ConfigurationManager.AppSettings[appConfigItem];

			// check if the data already exists and delete it before we add it
			if (fileColumnPositions != null)
			{
				// data exists so read into the array
				config.AppSettings.Settings.Remove(appConfigItem);
			}

			// build up a string with the values seperated by semi-colons
			for (int iLoop = 0; iLoop < Grid1Numcols; iLoop++)
			{
				combinedColumnPositionString += this.dispatchGrid1ColumnWidths[iLoop];
				combinedColumnPositionString += ";";
			}

			// write the data to the config file
			config.AppSettings.Settings.Add(appConfigItem, combinedColumnPositionString);

			// Save the configuration file.
			config.Save(ConfigurationSaveMode.Modified);

			// Force a reload of a changed section.
			ConfigurationManager.RefreshSection("appSettings");
		}

		private void GetColumnPositionsForGrid2()
		{
			// get grid2 positions
			string appConfigItem = this.Security.UserID + "DispatchGrid2ColumnPositions";
			int startPosistion = 0;

			// check if there are configurations for this user
			string fileColumnPositions = ConfigurationManager.AppSettings[appConfigItem];

			if (fileColumnPositions != null)
			{
				// data exists so read into the array
				// there are 22 columns seperated by a semi-colon so parse the received string into the interger array
				for (int iLoop = 0; iLoop < Grid2Numcols; iLoop++)
				{
					int endPosistion = fileColumnPositions.IndexOf(';', startPosistion);
					this.dispatchGrid2ColumnPositions[iLoop] = 
						Convert.ToInt32(fileColumnPositions.Substring(startPosistion, endPosistion - startPosistion));
					startPosistion = endPosistion + 1;
				}
			}
			else
			{
				// no data so just set at the default
				for (int iLoop = 0; iLoop < Grid2Numcols; iLoop++)
				{
					this.dispatchGrid2ColumnPositions[iLoop] = iLoop;
				}
			}
		}

		private void StoreGrid2ColumnPositions()
		{
			string appConfigItem = this.Security.UserID + "DispatchGrid2ColumnPositions";
			string combinedColumnPositionString = string.Empty;

			// dataGridView1ColumnPositions
			// check if there are configurations for this user
			Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			string fileColumnPositions = ConfigurationManager.AppSettings[appConfigItem];

			// check if the data already exists and delete it before we add it
			if (fileColumnPositions != null)
			{
				// data exists so read into the array
				config.AppSettings.Settings.Remove(appConfigItem);
			}

			// build up a string with the values seperated by semi-colons
			for (int iLoop = 0; iLoop < Grid2Numcols; iLoop++)
			{
				combinedColumnPositionString += this.dispatchGrid2ColumnPositions[iLoop];
				combinedColumnPositionString += ";";
			}

			// write the data to the config file
			config.AppSettings.Settings.Add(appConfigItem, combinedColumnPositionString);

			// Save the configuration file.
			config.Save(ConfigurationSaveMode.Modified);

			// Force a reload of a changed section.
			ConfigurationManager.RefreshSection("appSettings");
		}

		private void GetColumnWidthsForGrid2()
		{
			// get grid1 positions
			string appConfigItem = this.Security.UserID + "DispatchGrid2ColumnWidths";
			int startPosistion = 0;

			// check if there are configurations for this user
			string fileColumnPositions = ConfigurationManager.AppSettings[appConfigItem];

			if (fileColumnPositions != null)
			{
				// data exists so read into the array
				// there are 22 columns seperated by a semi-colon so parse the received string into the interger array
				for (int iLoop = 0; iLoop < Grid2Numcols; iLoop++)
				{
					int endPosistion = fileColumnPositions.IndexOf(';', startPosistion);
					this.dispatchGrid2ColumnWidths[iLoop] = 
						Convert.ToInt32(fileColumnPositions.Substring(startPosistion, endPosistion - startPosistion));
					startPosistion = endPosistion + 1;
				}
			}
			else
			{
				// no data so just set at the default
				this.SetColumnWidthsForGrid2FromGrid();
			}
		}

		private void SetColumnWidthsForGrid2FromGrid()
		{
			for (int iLoop = 0; iLoop < Grid2Numcols; iLoop++)
			{
				this.dispatchGrid2ColumnWidths[iLoop] = this.PersonDataGridView.Columns[iLoop].Width;
			}
		}

		private void StoreGrid2ColumnWidths()
		{
			string appConfigItem = this.Security.UserID + "DispatchGrid2ColumnWidths";
			string combinedColumnPositionString = string.Empty;

			// dataGridView1ColumnPositions
			// check if there are configurations for this user
			Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			string fileColumnPositions = ConfigurationManager.AppSettings[appConfigItem];

			// check if the data already exists and delete it before we add it
			if (fileColumnPositions != null)
			{
				// data exists so read into the array
				config.AppSettings.Settings.Remove(appConfigItem);
			}

			// build up a string with the values seperated by semi-colons
			for (int iLoop = 0; iLoop < Grid2Numcols; iLoop++)
			{
				combinedColumnPositionString += this.dispatchGrid2ColumnWidths[iLoop];
				combinedColumnPositionString += ";";
			}

			// write the data to the config file
			config.AppSettings.Settings.Add(appConfigItem, combinedColumnPositionString);

			// Save the configuration file.
			config.Save(ConfigurationSaveMode.Modified);

			// Force a reload of a changed section.
			ConfigurationManager.RefreshSection("appSettings");
		}

		private void GetColumnPositionsForGrid3()
		{
			// get grid3 positions
			string appConfigItem = this.Security.UserID + "DispatchGrid3ColumnPositions";
			int startPosistion = 0;

			// check if there are configurations for this user
			string fileColumnPositions = ConfigurationManager.AppSettings[appConfigItem];

			if (fileColumnPositions != null)
			{
				// data exists so read into the array
				// there are 22 columns seperated by a semi-colon so parse the received string into the interger array
				for (int iLoop = 0; iLoop < Grid3Numcols; iLoop++)
				{
					int endPosistion = fileColumnPositions.IndexOf(';', startPosistion);
					this.dispatchGrid3ColumnPositions[iLoop] = 
						Convert.ToInt32(fileColumnPositions.Substring(startPosistion, endPosistion - startPosistion));
					startPosistion = endPosistion + 1;
				}
			}
			else
			{
				// no data so just set at the default
				for (int iLoop = 0; iLoop < Grid3Numcols; iLoop++)
				{
					this.dispatchGrid3ColumnPositions[iLoop] = iLoop;
				}
			}
		}

		private void StoreGrid3ColumnPositions()
		{
			string appConfigItem = this.Security.UserID + "DispatchGrid3ColumnPositions";
			string combinedColumnPositionString = string.Empty;

			// dataGridView1ColumnPositions
			// check if there are configurations for this user
			Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			string fileColumnPositions = ConfigurationManager.AppSettings[appConfigItem];

			// check if the data already exists and delete it before we add it
			if (fileColumnPositions != null)
			{
				// data exists so read into the array
				config.AppSettings.Settings.Remove(appConfigItem);
			}

			// build up a string with the values seperated by semi-colons
			for (int iLoop = 0; iLoop < Grid3Numcols; iLoop++)
			{
				combinedColumnPositionString += this.dispatchGrid3ColumnPositions[iLoop];
				combinedColumnPositionString += ";";
			}

			// write the data to the config file
			config.AppSettings.Settings.Add(appConfigItem, combinedColumnPositionString);

			// Save the configuration file.
			config.Save(ConfigurationSaveMode.Modified);

			// Force a reload of a changed section.
			ConfigurationManager.RefreshSection("appSettings");
		}

		private void GetColumnWidthsForGrid3()
		{
			// get grid1 positions
			string appConfigItem = this.Security.UserID + "DispatchGrid3ColumnWidths";
			int startPosistion = 0;

			// check if there are configurations for this user
			string fileColumnPositions = ConfigurationManager.AppSettings[appConfigItem];

			if (fileColumnPositions != null)
			{
				// data exists so read into the array
				// there are 22 columns seperated by a semi-colon so parse the received string into the interger array
				for (int iLoop = 0; iLoop < Grid3Numcols; iLoop++)
				{
					int endPosistion = fileColumnPositions.IndexOf(';', startPosistion);
					this.dispatchGrid3ColumnWidths[iLoop] = 
						Convert.ToInt32(fileColumnPositions.Substring(startPosistion, endPosistion - startPosistion));
					startPosistion = endPosistion + 1;
				}
			}
			else
			{
				// no data so just set at the default
				this.SetColumnWidthsForGrid3FromGrid();
			}
		}

		private void SetColumnWidthsForGrid3FromGrid()
		{
			for (int iLoop = 0; iLoop < Grid3Numcols; iLoop++)
			{
				this.dispatchGrid3ColumnWidths[iLoop] = this.RequestDataGridView.Columns[iLoop].Width;
			}
		}

		private void StoreGrid3ColumnWidths()
		{
			string appConfigItem = this.Security.UserID + "DispatchGrid3ColumnWidths";
			string combinedColumnPositionString = string.Empty;

			// dataGridView1ColumnPositions
			// check if there are configurations for this user
			Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			string fileColumnPositions = ConfigurationManager.AppSettings[appConfigItem];

			// check if the data already exists and delete it before we add it
			if (fileColumnPositions != null)
			{
				// data exists so read into the array
				config.AppSettings.Settings.Remove(appConfigItem);
			}

			// build up a string with the values seperated by semi-colons
			for (int iLoop = 0; iLoop < Grid3Numcols; iLoop++)
			{
				combinedColumnPositionString += this.dispatchGrid3ColumnWidths[iLoop];
				combinedColumnPositionString += ";";
			}

			// write the data to the config file
			config.AppSettings.Settings.Add(appConfigItem, combinedColumnPositionString);

			// Save the configuration file.
			config.Save(ConfigurationSaveMode.Modified);

			// Force a reload of a changed section.
			ConfigurationManager.RefreshSection("appSettings");
		}

		private void EquipmentDataGridViewColumnDisplayIndexChanged(object sender, DataGridViewColumnEventArgs e)
		{
			this.dispatchGrid1ColumnPositions[e.Column.Index] = e.Column.DisplayIndex;
		}

		private void PersonDataGridViewColumnDisplayIndexChanged(object sender, DataGridViewColumnEventArgs e)
		{
			this.dispatchGrid2ColumnPositions[e.Column.Index] = e.Column.DisplayIndex;
		}

		private void RequestDataGridViewColumnDisplayIndexChanged(object sender, DataGridViewColumnEventArgs e)
		{
			this.dispatchGrid3ColumnPositions[e.Column.Index] = e.Column.DisplayIndex;
		}

		private void DispatchFormFormClosing(object sender, FormClosingEventArgs e)
		{
			this.dataAccess.OnEquipmentUpdated	-= this.UpdateEquipmentView;
			this.dataAccess.OnPersonnelUpdated	-= this.UpdatePersonnelView;
			this.dataAccess.OnDataUpdated		-= this.RefreshRequestView;
			this.dataAccess.OnError				-= this.ErrorHandler;

			this.StoreGrid1ColumnPositions();
			this.SetColumnWidthsForGrid1FromGrid();
			this.StoreGrid1ColumnWidths();
			this.StoreGrid2ColumnPositions();
			this.SetColumnWidthsForGrid2FromGrid();
			this.StoreGrid2ColumnWidths();
			this.StoreGrid3ColumnPositions();
			this.SetColumnWidthsForGrid3FromGrid();
			this.StoreGrid3ColumnWidths();
		}
	}
}
