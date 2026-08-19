// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchContainerForm.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Dispatch
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Configuration;
	using System.Data;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Drawing;
	using System.Globalization;
	using System.Threading;
	using System.Windows.Forms;
	using DispatchPrototype;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

    using FMCore;
	using FMDispatchBusinessObjects.BusinessInterfaces;
	using FMDispatchBusinessObjects.ChannelFactories;

	internal struct transactionIdAndRequestTypeType
	{
		#region Fields
		public string requestType;
		public string transactionID;
		#endregion
	}

	public partial class DispatchContainerForm : FMBaseForm
	{
		#region Constants
		private const int Grid1Numcols = 40;
		#endregion

		#region Static Fields
		public static string UserID = string.Empty;
		public static DispatchTransactionsSR DispatchSR;
		#endregion

		#region Fields
		public TodaysAppointmentsCollectionClass TodaysAppointmentsCollection = new TodaysAppointmentsCollectionClass();

		protected ManualResetEvent KillEvent;
		protected object PriorFilter;
		protected Thread SchedulerMessageThread;
		protected bool HasLoadedTransactions = false;

		private readonly int appointmentWarnPeriod = 15; // Minutes
		private readonly ContextMenuStrip popupContextMenuForDataGrid = new ContextMenuStrip();
		private readonly int[] dataGridView1ColumnPositions = new int[Grid1Numcols];
		private readonly int[] dataGridView1ColumnWidths = new int[Grid1Numcols];
		private DateTime currentSelectedBeginDatePicker;
		private DateTime currentSelectedEndDatePicker;
		private DateTime operationLockDate = DateTime.UtcNow;
		private DataGridViewColumn columnBeingSorted;
		private string currentRowTransID = string.Empty;
		private SortOrder currentSortOrder = SortOrder.None;
		private DispatchDataAccess dataAccess;
		#endregion

		#region Constructors and Destructors
		public DispatchContainerForm()
		{
			this.GetSecurity();
			this.GetDisplayGridColumnPositions();

			string warnPeriod = ConfigurationManager.AppSettings["AppointmentWarningPeriod"];

			if (string.IsNullOrEmpty(warnPeriod))
			{
				throw new Exception("AppointmentWarningPeriod not in AppSettings");
			}

			if (warnPeriod.Length > 0)
			{
				this.appointmentWarnPeriod = Convert.ToInt32(warnPeriod);
			}

			this.InitializeComponent();

			// format data grid dates based on site configuration (IGO 2010-Aug-16)
			this.GetSiteDateTimeFormatInfo();
			var dataGridViewCellStyleShortDate = new DataGridViewCellStyle
				                                     {
					                                     Format = this.SiteDateTimeFormatInfo.ShortDatePattern,
					                                     NullValue = null
				                                     };

			this.Column2.DefaultCellStyle = dataGridViewCellStyleShortDate;

			// format date controls based on site configuration (IGO 2010-Aug-13)
			this.BeginDatePicker.CustomFormat = this.SiteDateTimeFormatInfo.ShortDatePattern;
			this.BeginDatePicker.Format = DateTimePickerFormat.Custom;
			this.EndDatePicker.CustomFormat = this.SiteDateTimeFormatInfo.ShortDatePattern;
			this.EndDatePicker.Format = DateTimePickerFormat.Custom;

			SiteClass site =
				FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

			var timeConverter = new SiteTimeConverter(site);
			DateTimeFormatInfo dateTimeFormatInfo = site.GetDateTimeFormatInfo();

			// Initial Julian Date Display
			this.julianDateLabel.Text = "Date: " + timeConverter.Now().ToMilitaryJulianDateString();

			// Also set the date ranges to the new time
			this.BeginDatePicker.Value = timeConverter.Today().DateTime;
			this.currentSelectedBeginDatePicker = this.BeginDatePicker.Value;

			this.EndDatePicker.Value = timeConverter.Today().DateTime;
			this.currentSelectedEndDatePicker = this.EndDatePicker.Value;

			this.operationLockDate = Convert.ToDateTime(site.OperationalLockDate, dateTimeFormatInfo);

			this.julianDateTimer.Interval = (((24 - timeConverter.Now().Hour - 1) * 60 * 60)
			                                 + ((60 - timeConverter.Now().Minute - 1) * 60) + (60 - DateTime.Now.Second)) * 1000;
			this.julianDateTimer.Enabled = true;

			this.dataGridView1.RowHeadersDefaultCellStyle.Padding = new Padding(0);
			this.dataGridView1.CellFormatting += this.DataGridView1CellFormatting;
			this.dataGridView1.RowHeaderMouseDoubleClick += this.DataGridView1RowHeaderMouseDoubleClick;
			this.dataGridView1.CellDoubleClick += this.DataGridView1CellDoubleClick;
			this.dataGridView1.SelectionChanged += this.DataGridView1SelectionChanged;
			this.dataGridView1.CellClick += this.DataGridView1CellClick;

			this.dataGridView1.CellValueNeeded += this.DataGridView1CellValueNeeded;
			this.dataGridView1.VirtualMode = true;

			this.GetColumnWidthsForGrid();

			// restore the column positions to what the user had last time
			for (int index = 0; index < Grid1Numcols; index++)
			{
				this.dataGridView1.Columns[index].DisplayIndex = this.dataGridView1ColumnPositions[index];
				this.dataGridView1.Columns[index].Width = this.dataGridView1ColumnWidths[index];
			}

			// we enable this here so we do not get 22 events for each index that we have set above
			this.dataGridView1.ColumnDisplayIndexChanged += this.DataGridView1ColumnDisplayIndexChanged;

			// launch the thread for handling the schedule messages
			this.SchedulerMessageThread = new Thread(this.SchedulerMessageProcessScan);
			this.SchedulerMessageThread.Start();
			this.AddAddInItemsToAddInMenu();

			this.exportToAccountingToolStripMenuItem.Enabled = Security.HasRight(RIGHT.MODIFY_DISPATCH);
		}
		#endregion

		#region Delegates
		private delegate void ShowReminderFormCallback();
		#endregion

		#region Public Methods and Operators
		public static bool TransactionStatusOpen(TransactionStatus status)
		{
			bool bClosed = status == TransactionStatus.Cancelled || status == TransactionStatus.Closed
			               || status == TransactionStatus.Completed || status == TransactionStatus.Posted
			               || status == TransactionStatus.Pending || status == TransactionStatus.OnHold;

			return !bClosed;
		}

		public bool CheckIfAppoinmentIsDue()
		{
			bool appointmentDue = false;
			DateTime todaysDateTime = DateTime.Now;

			foreach (TodaysAppointmentClass todaysAppointment in this.TodaysAppointmentsCollection)
			{
				if (todaysAppointment.InSleepMode == false && todaysAppointment.DoNotNotifyAgain == false
				    && todaysDateTime >= todaysAppointment.DueDate.AddMinutes(-this.appointmentWarnPeriod)
				    && todaysAppointment.DueDate >= todaysDateTime)
				{
					todaysAppointment.AppointmentIsDue = true;
					appointmentDue = true;
				}
				else if (todaysAppointment.InSleepMode && todaysAppointment.DoNotNotifyAgain == false
				         && todaysDateTime >= todaysAppointment.SleepTimeInterval)
				{
					todaysAppointment.AppointmentIsDue = true;
					appointmentDue = true;
				}

				// do not show over due appointments per the DOD testing department
			}

			return appointmentDue;
		}

		public void GetTodaysScheduledEvents()
		{
			try
			{
				DateTime todaysDate = DateTime.Now;

				todaysDate = todaysDate.AddHours(-todaysDate.Hour);
				todaysDate = todaysDate.AddMinutes(-todaysDate.Minute);
				todaysDate = todaysDate.AddSeconds(-todaysDate.Second);
				todaysDate = todaysDate.AddMilliseconds(-todaysDate.Millisecond);

				var endDate = todaysDate;

				todaysDate = todaysDate.AddSeconds(-1);

				// this will set it at 1 second prior to midnight to ensure we get the whole day
				AppointmentCollectionClass appointmentCollection = FMChannelHelper.MakeCall<IAppointments, AppointmentCollectionClass>(
					x => x.EnumerateByStartStopTime(this.Security, "Personnel", todaysDate, endDate));

				foreach (AppointmentClass appointmentData in appointmentCollection)
				{
					// we got an appointment so check it and map it
					this.CheckAndMapAppointment(appointmentData);
				}

				this.CheckForDateOverrun();
			}
			catch (Exception exception)
			{
				MessageBox.Show(this, exception.Message, this.Text);
			}
		}

		public void MapAppointment(AppointmentClass appointmentData, TodaysAppointmentClass newAppointment)
		{
			newAppointment.IdentityGuid = appointmentData.IdentityGuid;
			newAppointment.DueDate = appointmentData.DueDate;
			newAppointment.Description = appointmentData.Description;
			newAppointment.AppointmentCategory = appointmentData.AppointmentCategory;
			newAppointment.AssetText = appointmentData.AssetText;
			newAppointment.Duration = appointmentData.Duration;
		}
		#endregion

		#region Methods
		private void AddAddInItemsToAddInMenu()
		{
			int iLoop;

			ReCheckMenuItems:
			if (this.addInsToolStripMenuItem.DropDownItems.Count > 1)
			{
				for (iLoop = 0; iLoop < this.addInsToolStripMenuItem.DropDownItems.Count; iLoop++)
				{
					if (iLoop != 0)
					{
						this.addInsToolStripMenuItem.DropDownItems.RemoveAt(iLoop);
						goto ReCheckMenuItems;
					}
				}
			}

			iLoop = 0;
			while (true)
			{
				string appMenuItem = "MenuItem" + iLoop;
				string appPathItem = "AppPath" + iLoop;

				string lvText = ConfigurationManager.AppSettings[appMenuItem];
				string lvText1 = ConfigurationManager.AppSettings[appPathItem];

				if (lvText != null && lvText1 != null)
				{
					this.addInsToolStripMenuItem.DropDownItems.Add(lvText, null, this.AddInItemClicked);
				}
				else
				{
					break;
				}

				++iLoop;
			}
		}

		private void AddInItemClicked(object sender, EventArgs e)
		{
			int iLoop = 0;

			var menuItem = (ToolStripMenuItem)sender;
			while (true)
			{
				string appMenuItem = "MenuItem" + iLoop;
				string appPathItem = "AppPath" + iLoop;

				string lvText = ConfigurationManager.AppSettings[appMenuItem];
				string lvText1 = ConfigurationManager.AppSettings[appPathItem];

				if (lvText != null && lvText1 != null && lvText == menuItem.Text)
				{
					var newProcess = new Process();
					var processStartInfo = new ProcessStartInfo(lvText1) { UseShellExecute = false };

					var security = AppDomain.CurrentDomain.GetData("Security") as SecurityClass;

					processStartInfo.EnvironmentVariables["Security"] = string.Empty;

					if (security != null)
					{
						processStartInfo.EnvironmentVariables["UserID"]		= security.UserID;
						processStartInfo.EnvironmentVariables["UserGuid"]	= security.UserGuid.ToString();
						processStartInfo.EnvironmentVariables["SiteID"]		= security.SiteID;
						processStartInfo.EnvironmentVariables["SiteGuid"]	= security.SiteGuid.ToString();
						processStartInfo.EnvironmentVariables["PW"]			= security.Password;
						processStartInfo.EnvironmentVariables["CSRFToken"]	= security.CSRFToken;
					}

					processStartInfo.EnvironmentVariables["WebAddress"] = ConfigurationManager.AppSettings["WebAppAddress"];
					processStartInfo.EnvironmentVariables["Token"] = AppDomain.CurrentDomain.GetData("Token") as string;
					newProcess.StartInfo = processStartInfo;

					if (!newProcess.Start())
					{
						MessageBox.Show("Could not start " + lvText1, "Warning");
					}

					break;
				}

				if (string.IsNullOrEmpty(lvText) && string.IsNullOrEmpty(lvText1))
				{
					break;
				}

				++iLoop;
			}
		}

		private void AutoSelectFirstAvailableMenuItem()
		{
			for (int iLoop = 0; iLoop < this.popupContextMenuForDataGrid.Items.Count; iLoop++)
			{
				if (this.popupContextMenuForDataGrid.Items[iLoop].Enabled && iLoop != 5)
				{
					// this is the "-" menu line
					this.popupContextMenuForDataGrid.Items[iLoop].Select();
					break;
				}
			}
		}

		private void BeginDatePickerCloseUp(object sender, EventArgs e)
		{
		}

		private void BeginDatePickerLeave(object sender, EventArgs e)
		{
		}

		private void BeginDatePickerValueChanged(object sender, EventArgs e)
		{
			try
			{
				// if the user didi not change the date just exit
				if (this.currentSelectedBeginDatePicker == this.BeginDatePicker.Value)
				{
					return;
				}

				if (this.EndDatePicker.Value < this.BeginDatePicker.Value)
				{
					MessageBox.Show("Begin datetime must be before End datetime.");
				}
			}
			catch (Exception except)
			{
				this.Cursor = Cursors.Default;
				this.ErrorHandler(except);
			}
		}

		private void ButtonRefreshClick(object sender, EventArgs e)
		{
			try
			{
				if (this.EndDatePicker.Value < this.BeginDatePicker.Value)
				{
					MessageBox.Show("Begin datetime must be before End datetime.");
					return;
				}

				this.currentSelectedBeginDatePicker = this.BeginDatePicker.Value;
				this.Cursor = Cursors.WaitCursor;
				this.ButtonRefresh.Enabled = false;
				this.UpdateView(true);

				// reset the transversion incase we missed one, this ensures we get the latest one on the next polling
				this.ButtonRefresh.Enabled = true;
				this.Cursor = Cursors.Default;
			}
			catch (Exception except)
			{
				this.Cursor = Cursors.Default;
				this.ErrorHandler(except);
			}
		}

		private void CancelRequest(object sender, EventArgs e)
		{
			try
			{
				if (this.Security.HasRight(RIGHT.MODIFY_DISPATCH) == false)
				{
					return;
				}

				var transactionIdAndRequestTypeList = new List<transactionIdAndRequestTypeType>();

				// Lock on dataGridView1 to protect against changes
				lock (this.dataGridView1)
				{
					// Check for completed transactions
					foreach (DataGridViewRow selectedRow in this.dataGridView1.SelectedRows)
					{
						DataRowView row = this.dataGridView1.GetDataRow(selectedRow.Index);
						var status = (TransactionStatus)row["transactionStatusInt"];

						if (status == TransactionStatus.Completed)
						{
							throw new ApplicationException("Completed requests cannot be cancelled.");
						}
					}

					const string Message = "Once an operation is canceled it cannot be un-canceled.\nAre you sure you want to cancel this job(s)?";
					DialogResult result = MessageBox.Show(this, Message, "Dispatch", MessageBoxButtons.YesNo);
					
					if (result == DialogResult.No)
					{
						return;
					}

					foreach (DataGridViewRow selectedRow in this.dataGridView1.SelectedRows)
					{
						DataRowView row = this.dataGridView1.GetDataRow(selectedRow.Index);
						var transID = (string)row["TransID"];
						var requesttype = (string)row["AliasName"];

						transactionIdAndRequestTypeType transactionIdAndRequestType;
						transactionIdAndRequestType.transactionID = transID;
						transactionIdAndRequestType.requestType = requesttype;
						transactionIdAndRequestTypeList.Add(transactionIdAndRequestType);
					}
				}

				this.dataGridView1.ClearSelection();

				foreach (transactionIdAndRequestTypeType transactionIdAndRequestType in transactionIdAndRequestTypeList)
				{
					string transID = transactionIdAndRequestType.transactionID;
					string requesttype = transactionIdAndRequestType.requestType;

					TransactionDO transaction = this.GetTransaction(transID);

					string contactMemo = string.Empty;

					if (transaction.UserData.ContainsKey("TAUD23"))
					{
						contactMemo = transaction.UserData["TAUD23"];
					}

					// prompt the user to enter a comment for the cancellation
					var cancelComment = new CommentForm
					                    {
						                    CurrentComment = contactMemo,
						                    Forstring = "Request Type = " + requesttype
					                    };

					cancelComment.ShowDialog(this);

					// restore the comment
					transaction.Notes += " - " + cancelComment.CurrentComment;
					transaction.Status = TransactionStatus.Cancelled;

					foreach (LineItemDO lineItem in transaction.LineItems)
					{
						lineItem.Status = TransactionStatus.Cancelled;
						lineItem.Quantity = new QuantityDO(0, 0, 0, 0);

						foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
						{
							subLineItem.Status = TransactionStatus.Cancelled;
						}
					}

					this.SaveTransaction(transaction);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void ChangeStatusFilterIfNecessary(TransactionStatus transactionStatus)
		{
			if (this.vehicleComboBox.SelectedIndex != 0)
			{
				this.PriorFilter = null;
				this.StatusCombo.Text = Enum.GetName(typeof(TransactionStatus), transactionStatus);
				this.dataGridView1.SelectAll();
			}
			else if (this.PriorFilter != null)
			{
				this.StatusCombo.SelectedItem = this.PriorFilter;
				this.PriorFilter = null;
			}
		}

		private void CheckAndMapAppointment(AppointmentClass appointmentData)
		{
			bool appointmentFound = false;

			foreach (TodaysAppointmentClass todaysAppointment in this.TodaysAppointmentsCollection)
			{
				if (appointmentData.IdentityGuid == todaysAppointment.IdentityGuid)
				{
					appointmentFound = true;

					// check that the date is correct. We may have run over 24 hours so delete any old records
					// check if any of the appointment data has changed
					if (this.HasAnyAppointmentDataChanged(appointmentData, todaysAppointment))
					{
						// something has changed so remove and then readd the class
						this.TodaysAppointmentsCollection.Remove(todaysAppointment);
						var newAppointment = new TodaysAppointmentClass();
						this.MapAppointment(appointmentData, newAppointment);
						this.TodaysAppointmentsCollection.Add(newAppointment);
					}

					break;
				}
			}

			if (appointmentFound == false)
			{
				// add this apointment to the collection
				var newAppointment = new TodaysAppointmentClass();
				this.MapAppointment(appointmentData, newAppointment);
				this.TodaysAppointmentsCollection.Add(newAppointment);
			}
		}

		private void CheckForDateOverrun()
		{
			DateTime todaysDateTime = DateTime.Now;

			ReStartCheck:

			foreach (TodaysAppointmentClass todaysAppointment in this.TodaysAppointmentsCollection)
			{
				// check that the date is correct. We may have run over 24 hours so delete any old records
				// we only need to check the day and month
				if (todaysAppointment.DueDate.Day != todaysDateTime.Day || todaysAppointment.DueDate.Month != todaysDateTime.Month)
				{
					this.TodaysAppointmentsCollection.Remove(todaysAppointment);
					goto ReStartCheck;
				}
			}
		}

		private void CloseoutToolStripMenuItemClick(object sender, EventArgs e)
		{
			var releaseToAccountingForm = new ReleaseToAccountingForm( this.dataAccess, this.operationLockDate );
			
			if ( releaseToAccountingForm.ShowDialog( this ) == DialogResult.OK )
			{
				this.operationLockDate = releaseToAccountingForm.OperationLockDate;
			}
		}

		private void DispatchContainerFormFormClosing(object sender, FormClosingEventArgs e)
		{
			this.StoreDisplayGridColumnPositions();
			this.SetColumnWidthsForGridFromGrid();
			this.StoreGrid1ColumnWidths();
			this.dataAccess.Dispose();

			if (!this.ApplicationExitingFromError)
			{
				this.Logout();
			}
		}

		private void DispatchContainerFormLoad(object sender, EventArgs e)
		{
			try
			{
				this.StatusCombo.SelectedIndexChanged -= this.StatusComboSelectedIndexChanged;
				this.LoadStatusCombo();
				this.StatusCombo.SelectedIndexChanged += this.StatusComboSelectedIndexChanged;

				this.RequestTypeCombo.SelectedIndexChanged -= this.RequestTypeComboSelectedIndexChanged;
				this.LoadRequestCombo();
				this.RequestTypeCombo.SelectedIndexChanged += this.RequestTypeComboSelectedIndexChanged;

				string fuelRequestAlias = ConfigurationManager.AppSettings["FuelRequestTransactionAlias"];

				DispatchSR = new DispatchTransactionsSR
					             {
						             Security = this.Security
					             };

				DispatchSR.Translations.Add(
					new DispatchTransactionsSR.DispatchTranslationPair { AccountingName = fuelRequestAlias, DispatchName = "Refuel" });

				this.dataAccess = new DispatchDataAccess(this.Security);

				this.dataAccess.OnDataUpdated += this.RefreshView;
				this.dataAccess.OnError += this.ErrorHandler;
				this.UpdateView();
				this.dataAccess.StartPolling();

				// dataGridView1.Sort(dataGridView1.Columns["Requested"], ListSortDirection.Ascending);

				// Add context menu items
				this.SetContextPopupMenuItems();
				this.SetPopupMenuEnableDisable();

				// dataGridView1.Select();
				this.RenumberGrid(this.dataGridView1);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void EndDatePickerCloseUp(object sender, EventArgs e)
		{
		}

		private void EndDatePickerLeave(object sender, EventArgs e)
		{
		}

		private void EndDatePickerValueChanged(object sender, EventArgs e)
		{
			try
			{
				// if the user didi not change the date just exit
				if (this.currentSelectedEndDatePicker == this.EndDatePicker.Value)
				{
					return;
				}

				if (this.EndDatePicker.Value < this.BeginDatePicker.Value)
				{
					MessageBox.Show("'Begin' datetime must be before 'End' datetime.");
				}
			}
			catch (Exception except)
			{
				this.Cursor = Cursors.Default;
				this.ErrorHandler(except);
			}
		}

		private void ExitToolsStripMenuItemClick(object sender, EventArgs e)
		{
			this.Close();
		}

		private void FlightLineButtonClick(object sender, EventArgs e)
		{
			try
			{
				this.FlightLineStatusToolStripMenuItemClick(null, null);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void GetAliases(DispatchTransactionsSR sr)
		{
			sr.AliasNames.Clear();

			if (this.RequestTypeCombo.Text.Equals("{All}"))
			{
				foreach (string requestType in this.RequestTypeCombo.Items)
				{
					if (requestType.NotEquals("{All}"))
					{
						sr.AliasNames.Add(requestType);
					}
				}
			}
			else
			{
				sr.AliasNames.Add(this.RequestTypeCombo.Text);
			}
		}

		private void GetColumnWidthsForGrid()
		{
			// get grid1 positions
			string appConfigItem = this.Security.UserID + "Grid1ColumnWidths";
			int iStartPosistion = 0;

			// check if there are configurations for this user
			string fileColumnPositions = ConfigurationManager.AppSettings[appConfigItem];

			if (fileColumnPositions != null)
			{
				// data exists so read into the array
				// there are 22 columns seperated by a semi-colon so parse the received string into the interger array
				for (int iLoop = 0; iLoop < Grid1Numcols; iLoop++)
				{
					int iEndPosistion = fileColumnPositions.IndexOf(';', iStartPosistion);
					this.dataGridView1ColumnWidths[iLoop] =
						Convert.ToInt32(fileColumnPositions.Substring(iStartPosistion, iEndPosistion - iStartPosistion));
					iStartPosistion = iEndPosistion + 1;
				}
			}
			else
			{
				// no data so just set at the default
				this.SetColumnWidthsForGridFromGrid();
			}
		}

		private void GetDisplayGridColumnPositions()
		{
			string appConfigItem = this.Security.UserID + "Grid1ColumnPositions";
			int iStartPosistion = 0;

			// check if there are configurations for this user
			string fileColumnPositions = ConfigurationManager.AppSettings[appConfigItem];
			
			if (fileColumnPositions != null)
			{
				// data exists so read into the array
				// there are 22 columns seperated by a semi-colon so parse the received string into the interger array
				for (int iLoop = 0; iLoop < Grid1Numcols; iLoop++)
				{
					int iEndPosistion = fileColumnPositions.IndexOf(';', iStartPosistion);
					this.dataGridView1ColumnPositions[iLoop] =
						Convert.ToInt32(fileColumnPositions.Substring(iStartPosistion, iEndPosistion - iStartPosistion));
					iStartPosistion = iEndPosistion + 1;
				}
			}
			else
			{
				// no data so just set at the default
				for (int iLoop = 0; iLoop < Grid1Numcols; iLoop++)
				{
					this.dataGridView1ColumnPositions[iLoop] = iLoop;
				}
			}
		}

		private void GetStatusValues(DispatchTransactionsSR sr)
		{
			sr.Statuses.Clear();
			if (this.StatusCombo.Text.Equals("{All}"))
			{
				foreach (string status in this.StatusCombo.Items)
				{
					if (status.NotEquals("{All}"))
					{
						sr.Statuses.Add(status);
					}
				}
			}
			else
			{
				sr.Statuses.Add(this.StatusCombo.Text);
			}
		}

		private bool HasAnyAppointmentDataChanged(AppointmentClass appointmentData, TodaysAppointmentClass todaysAppointment)
		{
			if (todaysAppointment.IdentityGuid != appointmentData.IdentityGuid
			    || todaysAppointment.DueDate != appointmentData.DueDate
			    || todaysAppointment.Description != appointmentData.Description
			    || todaysAppointment.AppointmentCategory != appointmentData.AppointmentCategory
			    || todaysAppointment.AssetText != appointmentData.AssetText
			    || todaysAppointment.Duration != appointmentData.Duration)
			{
				return true;
			}

			return false;
		}

		private void HelpAboutClicked(object sender, EventArgs e)
		{
			var helpAbout = new AboutBox1();
			helpAbout.ShowDialog(this);
		}

		private void LoadRequestCombo()
		{
			this.RequestTypeCombo.Items.Clear();
			this.RequestTypeCombo.Sorted = false;

			this.RequestTypeCombo.Items.Add("{All}");
			this.RequestTypeCombo.Items.Add("Refuel");
			this.RequestTypeCombo.Items.Add("Defuel");
			this.RequestTypeCombo.Items.Add("Fillstand");
			this.RequestTypeCombo.Items.Add("Return to Bulk");
			this.RequestTypeCombo.Items.Add("Recirculation");

			this.RequestTypeCombo.SelectedIndex = 0;
		}

		private void LoadStatusCombo()
		{
			this.StatusCombo.Items.Clear();
			this.StatusCombo.Sorted = false;

			this.StatusCombo.Items.Add("{All}");
			this.StatusCombo.Items.Add("Requested");
			this.StatusCombo.Items.Add("Dispatched");
			this.StatusCombo.Items.Add("Arrived");
			this.StatusCombo.Items.Add("Started");
			this.StatusCombo.Items.Add("Stopped");
			this.StatusCombo.Items.Add("Completed");
			this.StatusCombo.Items.Add("Cancelled");
			this.StatusCombo.Items.Add("Pending");
			this.StatusCombo.Items.Add("Posted");

			this.StatusCombo.SelectedIndex = 0;
		}

		/// <summary>
		/// This method loads the Vehicle combobox with unique, non-empty vehicle XREF values
		///     from the data items in the main grid data source.  Selection is preserved
		///     if appropriate.
		/// </summary>
		/// <param name="view">
		/// The view.
		/// </param>
		private void LoadVehicleDropDown(DataView view)
		{
			// Set the vehicle combo box to the same - use a new view so they
			// are separate.
			DataTable dataTable = view.Table;

			// Save the current selection
			string selecteVehicleXref = this.vehicleComboBox.Text;

			// Clear the items from the combobox and add the ALL option
			this.vehicleComboBox.Items.Clear();
			this.vehicleComboBox.Items.Add("{All}");

			// Get a list of vehicle XREFs that are not empty
			EnumerableRowCollection<string> vehicleIDs = from V in dataTable.AsEnumerable()
			                                             where string.IsNullOrEmpty(V["IssuePointNumber"].ToString()) == false
			                                             select V["IssuePointNumber"] as string;

			// Load the combo box with unique values
			foreach (string V in vehicleIDs)
			{
				if (this.vehicleComboBox.FindStringExact(V) == -1)
				{
					this.vehicleComboBox.Items.Add(V);
				}
			}

			// Set the selection back to what it was if the item exists in the
			// combo any more.
			if (this.vehicleComboBox.FindStringExact(selecteVehicleXref) > 0)
			{
				this.vehicleComboBox.Text = selecteVehicleXref;
				view.RowFilter = string.Format("IssuePointNumber = '{0}'", selecteVehicleXref);
			}
			else
			{
				this.vehicleComboBox.SelectedIndexChanged -= this.VehicleComboBoxSelectedIndexChanged;
				view.RowFilter = string.Empty;
				this.vehicleComboBox.SelectedIndex = 0;
				this.vehicleComboBox.SelectedIndexChanged += this.VehicleComboBoxSelectedIndexChanged;
			}
		}

		private void Logout()
		{
			AppDomain appDomain = AppDomain.CurrentDomain;
			var security = appDomain.GetData("Security") as SecurityClass;
			FMChannelHelper.MakeCall<ISites>(x => x.Logout(security));

			//if (alertSetting == "1")
			//{
			//	MessageBox.Show("You have successfully logged out.");
			//}
		}

		private void OnAddInsConfigurationClicked(object sender, EventArgs e)
		{
			var addInCofiguration = new AddInsForm();

			if (addInCofiguration.ShowDialog(this) == DialogResult.OK)
			{
				// modify the add in menu with the selected values
				this.AddAddInItemsToAddInMenu();
			}
		}

		private void OnClickViewControlLog(object sender, EventArgs e)
		{
			var controlLog = new ControlLogForm { UserID = UserID };
			controlLog.Show(this);
		}

		private void RefreshView(object data)
		{
			this.RefreshView(data, DateTime.Now);
		}

		private void RefreshView(object data, DateTime queryTime)
		{
			ISynchronizeInvoke i = this;

			// Check if the event was generated from another
			// thread and needs invoke instead
			if (i.InvokeRequired)
			{
				DataTable dataTable = this.dataAccess.GetTransactions(DispatchSR).Transactions.Tables[0];
				DispatchDataAccess.OnDataUpdatedHandler tempDelegate = this.RefreshView;
				var results = new object[] { dataTable, queryTime };
				i.Invoke(tempDelegate, results);

				return;
			}

			lock (this.dataGridView1)
			{
				this.dataGridView1.SelectionChanged -= this.DataGridView1SelectionChanged;
				this.dataGridView1.SuspendLayout();

				// Store the currently selected rows so we can reset them after an update
				var selectedTransID = new string[this.dataGridView1.SelectedRows.Count];
				int numberOfSelectedRows = this.dataGridView1.SelectedRows.Count;

				for (int iLoop = 0; iLoop < numberOfSelectedRows; iLoop++)
				{
					DataGridViewRow row = this.dataGridView1.SelectedRows[iLoop];
					selectedTransID[iLoop] = this.dataGridView1.GetDataRow(row.Index)["TransID"].ToString();
				}

				// store the current row so it can be restored
				if (this.dataGridView1.CurrentRow != null)
				{
					DataGridViewRow currentRow = this.dataGridView1.Rows[this.dataGridView1.CurrentRow.Index];
					this.currentRowTransID = this.dataGridView1.GetDataRow(currentRow.Index)["TransID"].ToString();
				}

				var dataTable = data as DataTable;
				this.dataGridView1.MergeTransactionsAndUpdateView(dataTable, queryTime);

				int firstDisplayedScrollingRowIndex = this.dataGridView1.FirstDisplayedScrollingRowIndex;
				this.dataGridView1.SelectionChanged -= this.DataGridView1SelectionChanged;

				this.LoadVehicleDropDown(new DataView(dataTable));

				// Since the grid will autoselect the first row we need to reset the selection before we then set it
				// Set the row number display text values
				this.dataGridView1.ClearSelection();

				if (this.columnBeingSorted != null)
				{
					switch (this.currentSortOrder)
					{
						case SortOrder.Ascending:
							this.dataGridView1.Sort(this.columnBeingSorted, ListSortDirection.Ascending);
							break;
						case SortOrder.Descending:
							this.dataGridView1.Sort(this.columnBeingSorted, ListSortDirection.Descending);
							break;
						case SortOrder.None:
							this.dataGridView1.Sort(this.columnBeingSorted, ListSortDirection.Ascending);
							break;
					}
				}
				else
				{
					this.columnBeingSorted = this.dataGridView1.Columns["Requested"];
					this.currentSortOrder = SortOrder.Ascending;

					if (this.columnBeingSorted != null)
					{
						this.dataGridView1.Sort(this.columnBeingSorted, ListSortDirection.Ascending);
					}
				}

				this.ShowRowNumber();

				// we only do this if there are selections already made
				if (numberOfSelectedRows > 1)
				{
					// restore the selected rows if they are still visible
					for (int iLoop = 0; iLoop < numberOfSelectedRows; iLoop++)
					{
						foreach (DataGridViewRow row in this.dataGridView1.Rows)
						{
							DataRowView dataRow = this.dataGridView1.GetDataRow(row.Index);

							if (dataRow != null)
							{
								if (selectedTransID[iLoop].Equals((string)dataRow["TransID"]))
								{
									// Only set for the first one.
									if (iLoop == 0)
									{
										this.dataGridView1.CurrentCell = row.Cells[0];
									}

									row.Selected = true;

									if (row.Index < firstDisplayedScrollingRowIndex
									    || row.Index > firstDisplayedScrollingRowIndex + this.dataGridView1.DisplayedRowCount(false))
									{
										firstDisplayedScrollingRowIndex = row.Index;
									}

									break;
								}
							}
						}
					}
				}
				else
				{
					int rowIndex = 1;

					foreach (DataGridViewRow row in this.dataGridView1.Rows)
					{
						row.HeaderCell.Value = rowIndex.ToString(CultureInfo.InvariantCulture);

						DataRowView dataRow = this.dataGridView1.GetDataRow(row.Index);

						if (!string.IsNullOrEmpty(this.currentRowTransID) && this.currentRowTransID.Equals((string)dataRow["TransID"]))
						{
							// restore the current selection
							// the only way to do this is by setting the currentcell variable since the currentrow is read only
							this.dataGridView1.CurrentCell = row.Cells[0];
							row.Selected = true;

							// If no rows selected scroll to it.
							if (numberOfSelectedRows == 0)
							{
								firstDisplayedScrollingRowIndex = row.Index;
							}

							break;
						}

						rowIndex++;
					}
				}

				if (firstDisplayedScrollingRowIndex >= 0 && firstDisplayedScrollingRowIndex < this.dataGridView1.RowCount)
				{
					this.dataGridView1.FirstDisplayedScrollingRowIndex = firstDisplayedScrollingRowIndex;
				}

				this.dataGridView1.ResumeLayout();
			}

			this.dataGridView1.SelectionChanged += this.DataGridView1SelectionChanged;
			this.DataGridView1SelectionChanged(null, null);
			this.dataGridView1.Select();
		}

		private void Relog(object sender, EventArgs e)
		{
			Cursor currentCursor = this.Cursor;

			try
			{
				if (this.Security.HasRight(RIGHT.MODIFY_DISPATCH) == false)
				{
					return;
				}

				this.dataAccess.PausePolling();

				var sr = new CopyTransactionsSR { Security = this.Security };

				this.Cursor = Cursors.WaitCursor;

				lock (this.dataGridView1)
				{
					FMChannelHelper.MakeCall<IClientDispatchService>(
						copyTransactionsProcessor =>
							{
								foreach (DataGridViewRow selectedRow in this.dataGridView1.SelectedRows)
								{
									DataRowView row = this.dataGridView1.GetDataRow(selectedRow.Index);
									var type = (TransactionTypes)row["TransTypeID"];

									// Don't allow relog of FillStand
									if (type == TransactionTypes.T7_FillStand)
									{
										throw new ApplicationException("Cannot relog fillstand requests.");
									}

									var transID = (string)row["TransID"];
									sr.TransactionIds.Add(transID);

									// Each transaction type may need to request a different type of
									// document number.
									throw new NotImplementedException("Needs FMBusinessObjects to be merged");
									//switch (type)
									//{
									//	case TransactionTypes.T5_PrimaryDisbursement:
									//	case TransactionTypes.T25_Shipment:
									//		sr.DocumentTypes.Add(DOCUMENT_TYPE.MANUAL_BOL);
									//		break;
									//	case TransactionTypes.T17_Order:
									//	case TransactionTypes.T18_SupplyOrder:
									//		sr.DocumentTypes.Add(DOCUMENT_TYPE.ORDER);
									//		break;
									//	default:
									//		sr.DocumentTypes.Add(DOCUMENT_TYPE.TRANSACTION);
									//		break;
									//}

									//SaveTransactionsResultDO results = copyTransactionsProcessor.CopyTransaction(sr);
									//sr.TransactionIds.Clear();

									//// Display errors/warnings if there were any.
									//this.CheckForAndDisplayWarningMessages(results);

									//selectedRow.Selected = false;
								}
							});
				}

				// Now update the view so our changes are seen now and the user
				// does not have to wait for the refresh polling to fire.
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
			finally
			{
				this.Cursor = currentCursor;
				this.dataAccess.StartPolling();
			}
		}

		private void RequestTypeComboSelectedIndexChanged(object sender, EventArgs e)
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

		private void Reselect(List<TransactionDO> transactions)
		{
			lock (this.dataGridView1)
			{
				int firstDisplayedScrollingIndex = this.dataGridView1.FirstDisplayedScrollingRowIndex;

				this.dataGridView1.ClearSelection();
				this.dataGridView1.CurrentCell = null;

				foreach (TransactionDO trans in transactions)
				{
					foreach (DataGridViewRow row in this.dataGridView1.Rows)
					{
						DataRowView dataRow = this.dataGridView1.GetDataRow(row.Index);

						if (dataRow != null)
						{
							if (trans.TransID.Equals((string)dataRow["TransID"]))
							{
								row.Selected = true;

								if (this.dataGridView1.CurrentCell == null)
								{
									this.dataGridView1.CurrentCell = row.Cells[0];
									this.currentRowTransID = trans.TransID;
								}

								if (row.Index < firstDisplayedScrollingIndex
								    || row.Index > firstDisplayedScrollingIndex + this.dataGridView1.DisplayedRowCount(false))
								{
									firstDisplayedScrollingIndex = row.Index;
								}

								break;
							}
						}
					}
				}

				if (firstDisplayedScrollingIndex >= 0 && firstDisplayedScrollingIndex < this.dataGridView1.RowCount)
				{
					this.dataGridView1.FirstDisplayedScrollingRowIndex = firstDisplayedScrollingIndex;
				}
			}

			this.SetPopupMenuEnableDisable();
		}

		private void RowDoubleClicked(int rowIndex)
		{
			try
			{
				if (rowIndex >= 0)
				{
					DataRowView row = this.dataGridView1.GetDataRow(rowIndex);

					if ((string)row["AliasName"] == "Recirculation")
					{
						var recirculationForm = new RecirculationForm { TransID = (string)row["TransID"] };
						recirculationForm.ShowDialog(this);
					}
					else
					{
						// if the transaction type is a refuel or a defuel we need to check the operational lock date and if it is before
						// that date not allow any modification if it is complete
						var fuelRequestForm = new FuelRequestForm(this.operationLockDate) { TransID = (string)row["TransID"] };

						// if the transaction type is a refuel or a defuel we need to check the operational lock date and if it is before
						// that date not allow any modification if it is complete
						if (fuelRequestForm.transaction.Status == TransactionStatus.Completed
						    && ((string)row["AliasName"] == "Refuel" || (string)row["AliasName"] == "Defuel"))
						{
							DateTime completedDateTime = DateTime.UtcNow.AddYears(-100);

							if (row["RequestedDateTime"].ToString().Length > 0)
							{
								completedDateTime = ((DateTimeOffset)row["RequestedDateTime"]).DateTime;
							}

							// check the lock out date
							if (completedDateTime <= this.operationLockDate)
							{
								MessageBox.Show("Cannot edit completed transactions before the lock out date.\n\rEdit request ended.");
								return;
							}
						}

						fuelRequestForm.ShowDialog(this);
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void SchedulerMessageProcessScan()
		{
			this.KillEvent = new ManualResetEvent(false);
			WaitHandle[] waitHandles = { this.KillEvent };
			int waitInterval = 100;
			int reloadConfigCounter = 15;

			while (0 != WaitHandle.WaitAny(waitHandles, waitInterval, false))
			{
				waitInterval = 30000; // check once every 30 seconds

				// get the scheduled events for today
				if (reloadConfigCounter > 14)
				{
					this.GetTodaysScheduledEvents();
					reloadConfigCounter = 0;
				}

				++reloadConfigCounter;

				if (this.CheckIfAppoinmentIsDue())
				{
					// we do the following to ensure that the thread is safe
					this.ShowReminderForm();
				}
			}
		}

		private void SetColumnWidthsForGridFromGrid()
		{
			for (int iLoop = 0; iLoop < Grid1Numcols; iLoop++)
			{
				this.dataGridView1ColumnWidths[iLoop] = this.dataGridView1.Columns[iLoop].Width;
			}
		}

		private void SetContextPopupMenuItems()
		{
			this.popupContextMenuForDataGrid.Items.Add("Set Arrived", null, this.SetSelectedToArrived);
			this.popupContextMenuForDataGrid.Items.Add("Set Service Started", null, this.SetSelectedToStarted);
			this.popupContextMenuForDataGrid.Items.Add("Set Service Stopped", null, this.SetSelectedToStopped);
			this.popupContextMenuForDataGrid.Items.Add("Set Service Completed", null, this.SetSelectedToCompleted);
			this.popupContextMenuForDataGrid.Items.Add("Set Fillstand Completed", null, this.SetFillStandCompleted);
			this.popupContextMenuForDataGrid.Items.Add("-", null, null);
			this.popupContextMenuForDataGrid.Items.Add("Relog Transaction", null, this.Relog);
			this.popupContextMenuForDataGrid.Items.Add("Cancel Transaction", null, this.CancelRequest);
		}

		private void SetFillStandCompleted(object sender, EventArgs e)
		{
			var security = AppDomain.CurrentDomain.GetData("Security") as SecurityClass;
			if (security == null)
			{
				throw new Exception("Security not in AppDomain");
			}

			string[] transIDList;
			string[] selectedVehicleID;
			bool displayDispatchScreen = false;

			// since people can not grasp the concept of items being displayed in the order they are selected
			// we need to reorder the selected list based on the dispatch date and time	DispatchedDateTime
			int iLoop = this.dataGridView1.SelectedRows.Count - 1;

			lock (this.dataGridView1)
			{
				transIDList = new string[this.dataGridView1.SelectedRows.Count];
				selectedVehicleID = new string[this.dataGridView1.SelectedRows.Count];
				var dispatchDateTime = new DateTime[this.dataGridView1.SelectedRows.Count];

				foreach (DataGridViewRow selectedRow in this.dataGridView1.SelectedRows)
				{
					DataRowView row = this.dataGridView1.GetDataRow(selectedRow.Index);
					transIDList[iLoop] = (string)row["TransID"];
					selectedVehicleID[iLoop] = (string)row["VehicleID"];

					DateTimeOffset dispatchDateTimeOffset = (DateTimeOffset) row["DispatchedDateTime"];
					dispatchDateTime[iLoop] = dispatchDateTimeOffset.DateTime;
					iLoop--;
				}
			}

			int selectedGridItems = this.dataGridView1.SelectedRows.Count;

			if (selectedGridItems > 0)
			{
				string equipmentRefID = string.Empty;
				string selectedPerson = string.Empty;
				var selectedTransID = new string[1];
				selectedTransID[0] = string.Empty;

				var fuelRequestForm = new FuelRequestForm(this.operationLockDate)
				                      {
					                      TransToProcess = new FuelRequestForm.TransactionProcessingListClass(
																							  transIDList,
																							  selectedVehicleID,
																							  "Fillstand Completion for "),
					                      TransactionWillBeSetToCompleted = true,
					                      CompletionMode = true
				                      };

				// if there is only one selection store the data so we can select it when dispatch is opened
				fuelRequestForm.ShowDialog(this);

				if (fuelRequestForm.lastTransaction != null)
				{
					PersonClass person =
						FMChannelHelper.MakeCall<IClientDispatchService, PersonClass>(
							x => x.GetPerson(security, fuelRequestForm.lastTransaction.OperatorPersonnelGuid));

					equipmentRefID = fuelRequestForm.EquipmentRefID;
					selectedPerson = person.LastName;
					selectedTransID[0] = fuelRequestForm.lastTransaction.TransID;
					displayDispatchScreen = true;
				}

				if (displayDispatchScreen)
				{
					var dispatch = new DispatchForm(
						this.dataAccess, DispatchSR.Translations, selectedTransID, selectedPerson, equipmentRefID, this.operationLockDate);
					dispatch.ShowDialog(this);

					this.UpdateView();

					if (dispatch.Transactions != null)
					{
						this.Reselect(dispatch.Transactions);
					}
				}
			}

			this.SetPopupMenuEnableDisable();
		}

		/// <summary>
		///     This function enables and disables the appropriate state transitions on
		///     the Dispatch grid popup menu and the Operation menu on the menu bar
		///     The states of a transaction in Dispatch are:
		///     <list type="bullet">
		///         <item>
		///             <term>In-process states</term><descriptions>Active transactions; i.e., not completed or cancelled</descriptions>
		///         </item>
		///         <list type="bullet">
		///             <item>
		///                 <term>Requested</term>
		///             </item>
		///             <item>
		///                 <term>Dispatched</term>
		///             </item>
		///             <item>
		///                 <term>Arrived</term>
		///             </item>
		///             <item>
		///                 <term>Started</term>
		///             </item>
		///             <item>
		///                 <term>Stopped</term>
		///             </item>
		///         </list>
		///         <item>
		///             <term>Completed</term>
		///         </item>
		///         <item>
		///             <term>Cancelled</term>
		///         </item>
		///     </list>
		///     Possible state transitions are determined by the current state and the Optional Times configuration
		/// </summary>
		/// <remarks>
		///     Logic changed 3.7.2010 by CHK to allow a transition away from a state not currently required by the Optional Times configuration
		///     See bug 12158
		///     Logic change is that now the assumption starts that a transition is valid.  We then look for disqualifiers
		///     <list>
		///         <listheader>
		///             <term>Disqualifiers for in-process transitions</term>
		///         </listheader>
		///         <item>
		///             <term>Any selected transaction is already in an end state:  id est, Completed or Cancelled</term>
		///         </item>
		///         <item>
		///             <term>Any selected transaction is already at or past the state moved to by the tested transition</term>
		///         </item>
		///         <item>
		///             <term>Any selected transaction would skip a standard or configured state to make the tested transition</term>
		///         </item>
		///         <item>
		///             <term>Any selected transaction is a recirculation</term>
		///         </item>
		///     </list>
		///     <list>
		///         <listheader>
		///             <term>Disqualifiers for Completion transition</term>
		///         </listheader>
		///         <item>
		///             <term>Any selected transaction is already in an end state:  id est, Completed or Cancelled</term>
		///         </item>
		///         <item>
		///             <term>Any selected transaction would skip a standard or configured state to make the tested transition</term>
		///         </item>
		///         <item>
		///             <term>Any selected transaction is a recirculation</term>
		///         </item>
		///     </list>
		///     <list>
		///         <listheader>
		///             <term>Disqualifiers for Cancellation transitions</term>
		///         </listheader>
		///         <item>
		///             <term>Any selected transaction is already in an end state:  id est, Completed or Cancelled</term>
		///         </item>
		///         <item>
		///             <term>Any selected transaction is a recirculation</term>
		///         </item>
		///     </list>
		///     Note that the check for recirculations really is redundant; recirculations are created as completed.
		///     This check will be left in for now, as recirculations could change in the future.
		/// </remarks>
		private void SetPopupMenuEnableDisable()
		{
			bool anyDispatched = false;
			bool anyStarted = false;
			bool anyStopped = false;
			bool anyArrived = false;
			bool allCompleteCapable = true;
			bool allFillstandCapable = true;
			bool refuelDefuelSelected = true;
			bool allCancelCapable = true;
			bool anyRecirculation = false;
			bool anyRequested = false;
			bool anyCompletedCanceled = false;
			bool anyPostedSelected = false;
			bool anyPendingSelected = false;

			string useArrivalTime = ConfigurationManager.AppSettings["Use Arrival Time"];
			string useStartTime = ConfigurationManager.AppSettings["Use Start Time"];
			string useStopTime = ConfigurationManager.AppSettings["Use Stop Time"];

			// items selected so deal with those
			lock (this.dataGridView1)
			{
				if (this.dataGridView1.SelectedRows.Count > 0)
				{
					foreach (DataGridViewRow selectedRow in this.dataGridView1.SelectedRows)
					{
						DataRowView row = this.dataGridView1.GetDataRow(selectedRow.Index);
						var status = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), (string)row["TransactionStatus"]);
						var aliasName = (string)row["AliasName"];

						if (aliasName.Equals("Recirculation"))
						{
							allCompleteCapable = false;
							allFillstandCapable = false;

							if (status == TransactionStatus.Cancelled || status == TransactionStatus.Completed)
							{
								allCancelCapable = false;
								anyCompletedCanceled = true;
							}

							anyRecirculation = true;
						}
						else
						{
							if (status == TransactionStatus.Requested)
							{
								anyRequested = true;
							}

							if (status == TransactionStatus.Dispatched)
							{
								anyDispatched = true;
							}

							if (status == TransactionStatus.Arrived)
							{
								anyArrived = true;
							}

							if (status == TransactionStatus.Started)
							{
								anyStarted = true;
							}

							if (status == TransactionStatus.Stopped)
							{
								anyStopped = true;
							}

							if (status == TransactionStatus.Posted)
							{
								anyPostedSelected = true;
							}

							if (status == TransactionStatus.Pending)
							{
								anyPendingSelected = true;
							}

							if (aliasName.Equals("Fillstand") || aliasName.Equals("Return to Bulk"))
							{
								allCompleteCapable = false;
							}
							else
							{
								allFillstandCapable = false;
							}

							if (status == TransactionStatus.Cancelled || status == TransactionStatus.Completed)
							{
								allCancelCapable = false;
								allCompleteCapable = false;
								allFillstandCapable = false;
								anyCompletedCanceled = true;
							}
						}

						var transTypeID = (short)row["TransTypeID"];

						if (transTypeID != 4 && transTypeID != 6)
						{
							refuelDefuelSelected = false;
						}
					}
				}
				else
				{
					allCompleteCapable = false;
					allFillstandCapable = false;
					refuelDefuelSelected = false;
					allCancelCapable = false;
				}
			}

			if (this.popupContextMenuForDataGrid == null || this.popupContextMenuForDataGrid.Items.Count == 0)
			{
				return;
			}

			// if any transaction with status of posted is selected disable everything
			if (anyPostedSelected || anyPendingSelected || !Security.HasRight(RIGHT.MODIFY_DISPATCH))
			{
				this.popupContextMenuForDataGrid.Items[0].Enabled = false;
				this.popupContextMenuForDataGrid.Items[1].Enabled = false;
				this.popupContextMenuForDataGrid.Items[2].Enabled = false;
				this.popupContextMenuForDataGrid.Items[3].Enabled = false;
				this.popupContextMenuForDataGrid.Items[4].Enabled = false;
				this.popupContextMenuForDataGrid.Items[6].Enabled = false;
				this.popupContextMenuForDataGrid.Items[7].Enabled = false;
				this.arrivalToolStripMenuItem.Enabled = this.popupContextMenuForDataGrid.Items[0].Enabled;
				this.startOfServiceToolStripMenuItem.Enabled = this.popupContextMenuForDataGrid.Items[1].Enabled;
				this.stopOfServiceToolStripMenuItem.Enabled = this.popupContextMenuForDataGrid.Items[2].Enabled;
				this.serviceCompletionToolStripMenuItem.Enabled = this.popupContextMenuForDataGrid.Items[3].Enabled;
				this.fillstandCompletionToolStripMenuItem.Enabled = this.popupContextMenuForDataGrid.Items[4].Enabled;
				return;
			}

			if (Convert.ToBoolean(useArrivalTime) == false && Convert.ToBoolean(useStartTime) == false
			    && Convert.ToBoolean(useStopTime) == false)
			{
				this.popupContextMenuForDataGrid.Items[0].Enabled = false; // set arrived
				this.popupContextMenuForDataGrid.Items[1].Enabled = false; // Set Service Started
				this.popupContextMenuForDataGrid.Items[2].Enabled = false; // Set Service Stopped

				if (!anyRequested && allCompleteCapable)
				{
					// The only in-process status which invalidates the state transition to 
					// Completed is Requested (must still be Dispatched)
					this.popupContextMenuForDataGrid.Items[3].Enabled = true; // Set Service Completed
				}
				else
				{
					this.popupContextMenuForDataGrid.Items[3].Enabled = false; // Set Service Completed
				}

				if (!anyRequested && allFillstandCapable)
				{
					// The only in-process status which invalidates the state transition to 
					// Completed is Requested (must still be Dispatched)
					this.popupContextMenuForDataGrid.Items[4].Enabled = true; // Set Service Completed
				}
				else
				{
					this.popupContextMenuForDataGrid.Items[4].Enabled = false; // Set Fillstand Completed
				}
			}
			else if (Convert.ToBoolean(useArrivalTime) && Convert.ToBoolean(useStartTime) == false
			         && Convert.ToBoolean(useStopTime) == false)
			{
				this.popupContextMenuForDataGrid.Items[0].Enabled =
					!(anyRequested || anyArrived || anyStarted || anyStopped || anyCompletedCanceled || anyRecirculation);

				// set arrived
				this.popupContextMenuForDataGrid.Items[1].Enabled = false; // Set Service Started
				this.popupContextMenuForDataGrid.Items[2].Enabled = false; // Set Service Stopped

				if (!(anyRequested || anyDispatched) && allCompleteCapable)
				{
					this.popupContextMenuForDataGrid.Items[3].Enabled = true; // Set Service Completed
				}
				else
				{
					this.popupContextMenuForDataGrid.Items[3].Enabled = false; // Set Service Completed
				}

				if (!(anyRequested || anyDispatched) && allFillstandCapable)
				{
					this.popupContextMenuForDataGrid.Items[4].Enabled = true; // Set Fillstand Completed
				}
				else
				{
					this.popupContextMenuForDataGrid.Items[4].Enabled = false; // Set Fillstand Completed
				}
			}
			else if (Convert.ToBoolean(useArrivalTime) == false && Convert.ToBoolean(useStartTime)
			         && Convert.ToBoolean(useStopTime) == false)
			{
				this.popupContextMenuForDataGrid.Items[0].Enabled = false; // set arrived
				this.popupContextMenuForDataGrid.Items[1].Enabled =
					!(anyRequested || anyStarted || anyStopped || anyCompletedCanceled || anyRecirculation);

				// set arrived;	//Set Service Started
				this.popupContextMenuForDataGrid.Items[2].Enabled = false; // Set Service Stopped

				if (!(anyRequested || anyDispatched) && allCompleteCapable)
				{
					this.popupContextMenuForDataGrid.Items[3].Enabled = true; // Set Service Completed
				}
				else
				{
					this.popupContextMenuForDataGrid.Items[3].Enabled = false; // Set Service Completed
				}

				if (!(anyRequested || anyDispatched) && allFillstandCapable)
				{
					this.popupContextMenuForDataGrid.Items[4].Enabled = true; // Set Fillstand Completed
				}
				else
				{
					this.popupContextMenuForDataGrid.Items[4].Enabled = false; // Set Fillstand Completed
				}
			}
			else if (Convert.ToBoolean(useArrivalTime) == false && Convert.ToBoolean(useStartTime) == false
			         && Convert.ToBoolean(useStopTime))
			{
				this.popupContextMenuForDataGrid.Items[0].Enabled = false; // set arrived
				this.popupContextMenuForDataGrid.Items[1].Enabled = false; // Set Service Started
				this.popupContextMenuForDataGrid.Items[2].Enabled =
					!(anyRequested || anyStopped || anyCompletedCanceled || anyRecirculation); // Set Service Stopped

				if (!(anyRequested || anyDispatched) && allCompleteCapable)
				{
					this.popupContextMenuForDataGrid.Items[3].Enabled = true; // Set Service Completed
				}
				else
				{
					this.popupContextMenuForDataGrid.Items[3].Enabled = false; // Set Service Completed
				}

				if (!(anyRequested || anyDispatched) && allFillstandCapable)
				{
					this.popupContextMenuForDataGrid.Items[4].Enabled = true; // Set Fillstand Completed
				}
				else
				{
					this.popupContextMenuForDataGrid.Items[4].Enabled = false; // Set Fillstand Completed
				}
			}
			else if (Convert.ToBoolean(useArrivalTime) && Convert.ToBoolean(useStartTime)
			         && Convert.ToBoolean(useStopTime) == false)
			{
				this.popupContextMenuForDataGrid.Items[0].Enabled =
					!(anyRequested || anyArrived || anyStarted || anyStopped || anyCompletedCanceled || anyRecirculation);

				// set arrived
				this.popupContextMenuForDataGrid.Items[1].Enabled =
					!(anyRequested || anyDispatched || anyStarted || anyStopped || anyCompletedCanceled || anyRecirculation);

				// Set Service Started
				this.popupContextMenuForDataGrid.Items[2].Enabled = false; // Set Service Stopped

				if (!(anyRequested || anyDispatched || anyArrived) && allCompleteCapable)
				{
					this.popupContextMenuForDataGrid.Items[3].Enabled = true; // Set Service Completed
				}
				else
				{
					this.popupContextMenuForDataGrid.Items[3].Enabled = false; // Set Service Completed
				}

				if (!(anyRequested || anyDispatched || anyArrived) && allFillstandCapable)
				{
					this.popupContextMenuForDataGrid.Items[4].Enabled = true; // Set Fillstand Completed
				}
				else
				{
					this.popupContextMenuForDataGrid.Items[4].Enabled = false; // Set Fillstand Completed
				}
			}
			else if (Convert.ToBoolean(useArrivalTime) && Convert.ToBoolean(useStartTime) == false
			         && Convert.ToBoolean(useStopTime))
			{
				this.popupContextMenuForDataGrid.Items[0].Enabled =
					!(anyRequested || anyArrived || anyStarted || anyStopped || anyCompletedCanceled || anyRecirculation);

				// set arrived
				this.popupContextMenuForDataGrid.Items[1].Enabled = false; // Set Service Started
				this.popupContextMenuForDataGrid.Items[2].Enabled =
					!(anyRequested || anyDispatched || anyStopped || anyCompletedCanceled || anyRecirculation);

				// Set Service Stopped
				if (!(anyRequested || anyDispatched || anyArrived || anyStarted) && allCompleteCapable)
				{
					this.popupContextMenuForDataGrid.Items[3].Enabled = true; // Set Service Completed
				}
				else
				{
					this.popupContextMenuForDataGrid.Items[3].Enabled = false; // Set Service Completed
				}

				if (!(anyRequested || anyDispatched || anyArrived || anyStarted) && allFillstandCapable)
				{
					this.popupContextMenuForDataGrid.Items[4].Enabled = true; // Set Fillstand Completed
				}
				else
				{
					this.popupContextMenuForDataGrid.Items[4].Enabled = false; // Set Fillstand Completed
				}
			}
			else if (Convert.ToBoolean(useArrivalTime) == false && Convert.ToBoolean(useStartTime)
			         && Convert.ToBoolean(useStopTime))
			{
				this.popupContextMenuForDataGrid.Items[0].Enabled = false; // set arrived
				this.popupContextMenuForDataGrid.Items[1].Enabled =
					!(anyRequested || anyStarted || anyStopped || anyCompletedCanceled || anyRecirculation);

				// Set Service Started
				this.popupContextMenuForDataGrid.Items[2].Enabled =
					!(anyRequested || anyDispatched || anyStopped || anyCompletedCanceled || anyRecirculation);

				// Set Service Stopped
				if (!(anyRequested || anyDispatched || anyArrived || anyStarted) && allCompleteCapable)
				{
					this.popupContextMenuForDataGrid.Items[3].Enabled = true; // Set Service Completed
				}
				else
				{
					this.popupContextMenuForDataGrid.Items[3].Enabled = false; // Set Service Completed
				}

				if (!(anyRequested || anyDispatched || anyArrived || anyStarted) && allFillstandCapable)
				{
					this.popupContextMenuForDataGrid.Items[4].Enabled = true; // Set Fillstand Completed
				}
				else
				{
					this.popupContextMenuForDataGrid.Items[4].Enabled = false; // Set Fillstand Completed
				}
			}
			else if (Convert.ToBoolean(useArrivalTime) && Convert.ToBoolean(useStartTime) && Convert.ToBoolean(useStopTime))
			{
				this.popupContextMenuForDataGrid.Items[0].Enabled =
					!(anyRequested || anyArrived || anyStarted || anyStopped || anyCompletedCanceled || anyRecirculation);

				// set arrived
				this.popupContextMenuForDataGrid.Items[1].Enabled =
					!(anyRequested || anyDispatched || anyStarted || anyStopped || anyCompletedCanceled || anyRecirculation);

				// Set Service Started
				this.popupContextMenuForDataGrid.Items[2].Enabled =
					!(anyRequested || anyDispatched || anyArrived || anyStopped || anyCompletedCanceled || anyRecirculation);

				// Set Service Stopped
				if (!(anyRequested || anyDispatched || anyArrived || anyStarted) && allCompleteCapable)
				{
					this.popupContextMenuForDataGrid.Items[3].Enabled = true; // Set Service Completed
				}
				else
				{
					this.popupContextMenuForDataGrid.Items[3].Enabled = false; // Set Service Completed
				}

				if (!(anyRequested || anyDispatched || anyArrived || anyStarted) && allFillstandCapable)
				{
					this.popupContextMenuForDataGrid.Items[4].Enabled = true; // Set Fillstand Completed
				}
				else
				{
					this.popupContextMenuForDataGrid.Items[4].Enabled = false; // Set Fillstand Completed
				}
			}

			this.arrivalToolStripMenuItem.Enabled = this.popupContextMenuForDataGrid.Items[0].Enabled;
			this.startOfServiceToolStripMenuItem.Enabled = this.popupContextMenuForDataGrid.Items[1].Enabled;
			this.stopOfServiceToolStripMenuItem.Enabled = this.popupContextMenuForDataGrid.Items[2].Enabled;
			this.serviceCompletionToolStripMenuItem.Enabled = this.popupContextMenuForDataGrid.Items[3].Enabled;
			this.fillstandCompletionToolStripMenuItem.Enabled = this.popupContextMenuForDataGrid.Items[4].Enabled;

			// Only allow relog of Refuel or Defuel
			this.popupContextMenuForDataGrid.Items[6].Enabled = refuelDefuelSelected;
			this.popupContextMenuForDataGrid.Items[7].Enabled = allCancelCapable;

			// auto select the available menu option
			this.AutoSelectFirstAvailableMenuItem();
		}

		/// <summary>
		/// Sets the selected rows to arrived.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		/// <exception cref="System.Exception">Security not in AppDomain</exception>
		private void SetSelectedToArrived(object sender, EventArgs e)
		{
			var security = AppDomain.CurrentDomain.GetData("Security") as SecurityClass;

			if (security == null)
			{
				throw new Exception("Security not in AppDomain");
			}

			if (security.HasRight(RIGHT.MODIFY_DISPATCH) == false)
			{
				return;
			}

			SiteClass site = FMChannelHelper.MakeCall<IClientDispatchService, SiteClass>(x => x.GetSite(security, security.SiteGuid));

			var timeConverter = new SiteTimeConverter(site);

			Cursor currentCursor = this.Cursor;
			this.Cursor = Cursors.WaitCursor;

			lock (this.dataGridView1)
			{
				var sr = new SaveTransactionsSR
					         {
						         SubType = SaveTransactionsSR.SaveTransactionSubType.SaveTranactionFlagsAndStatus,
						         IndividualDbTransaction = false,
						         Security = security,
						         CurrentSiteGuid = security.SiteGuid,
						         ConvertUnits = true
					         };

				foreach (DataGridViewRow selectedRow in this.dataGridView1.SelectedRows)
				{
					DataRowView row = this.dataGridView1.GetDataRow(selectedRow.Index);

					var selectedTransID = (string)row["TransID"];

					var trans = new TransactionFlagsAndStatusDO(selectedTransID)
						            {
							            TransStatus = TransactionStatus.Arrived,
							            TimeIn = timeConverter.Now()
						            };

					sr.TransFlagsAndStatusCollection.Add(trans);
				}

				this.SaveTransactionWithServiceRequest(security, sr);
				this.SetPopupMenuEnableDisable();
				this.UpdateView();
			}

			this.Cursor = currentCursor;
		}

		/// <summary>
		/// Sets the selected rows to completed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		/// <exception cref="System.Exception">Security not in AppDomain</exception>
		private void SetSelectedToCompleted(object sender, EventArgs e)
		{
			var security = AppDomain.CurrentDomain.GetData("Security") as SecurityClass;

			if (security == null)
			{
				throw new Exception("Security not in AppDomain");
			}

			if (security.HasRight(RIGHT.MODIFY_DISPATCH) == false)
			{
				return;
			}

			string[] transIDList;
			string[] selectedAircraftRefID;

			// DateTime[] DispatchDateTime;
			int selectedGridItems;
			bool displayDispatchScreen = false;

			lock (this.dataGridView1)
			{
				transIDList = new string[this.dataGridView1.SelectedRows.Count];
				selectedAircraftRefID = new string[this.dataGridView1.SelectedRows.Count];

				// DispatchDateTime = new DateTime[dataGridView1.SelectedRows.Count];
				selectedGridItems = this.dataGridView1.SelectedRows.Count;

				// since people can not grasp the concept of items being displayed in the order they are selected
				// we need to reorder the selected list based on the dispatch date and time	DispatchedDateTime
				int iLoop = this.dataGridView1.SelectedRows.Count - 1;

				foreach (DataGridViewRow selectedRow in this.dataGridView1.SelectedRows)
				{
					DataRowView row = this.dataGridView1.GetDataRow(selectedRow.Index);
					transIDList[iLoop] = row["TransID"] as string;
					selectedAircraftRefID[iLoop] = row["XREF"] as string;

					// DispatchDateTime[iLoop] = (DateTime)Row["DispatchedDateTime"];
					iLoop--;
				}
			}

			if (selectedGridItems > 0)
			{
				string equipmentRefID = string.Empty;
				string selectedPerson = string.Empty;
				var selectedTransID = new string[1];

				selectedTransID[0] = string.Empty;

				var fuelRequestForm = new FuelRequestForm(this.operationLockDate);

				fuelRequestForm.TransToProcess = new FuelRequestForm.TransactionProcessingListClass(
					transIDList, selectedAircraftRefID, "Service Completion for ");
				fuelRequestForm.TransactionWillBeSetToCompleted = true;

				// fuelRequestForm.DialogHeaderText = "Service Completion for " + SelectedAircraftRefID[iLoop];
				fuelRequestForm.CompletionMode = true;

				// if there is only one selection store the data so we can select it when dispatch is opened
				fuelRequestForm.ShowDialog(this);

				if (fuelRequestForm.lastTransaction != null)
				{
					PersonClass person =
						FMChannelHelper.MakeCall<IClientDispatchService, PersonClass>(
							x => x.GetPerson(security, fuelRequestForm.lastTransaction.OperatorPersonnelGuid));

					equipmentRefID = fuelRequestForm.EquipmentRefID;
					selectedPerson = person.LastName;
					selectedTransID[0] = fuelRequestForm.lastTransaction.TransID;
					displayDispatchScreen = true;
				}

				if (displayDispatchScreen)
				{
					var dispatch = new DispatchForm(
						this.dataAccess, DispatchSR.Translations, selectedTransID, selectedPerson, equipmentRefID, this.operationLockDate);
					dispatch.ShowDialog(this);

					this.UpdateView();

					if (dispatch.Transactions != null)
					{
						this.Reselect(dispatch.Transactions);
					}
				}
			}

			this.SetPopupMenuEnableDisable();
			this.ChangeStatusFilterIfNecessary(TransactionStatus.Completed);
		}

		private void SetSelectedToStarted(object sender, EventArgs e)
		{
			var security = AppDomain.CurrentDomain.GetData("Security") as SecurityClass;

			if (security == null)
			{
				throw new Exception("Security not in AppDomain");
			}

			if (security.HasRight(RIGHT.MODIFY_DISPATCH) == false)
			{
				return;
			}

			SiteClass site = FMChannelHelper.MakeCall<IClientDispatchService, SiteClass>(x => x.GetSite(security, security.SiteGuid));

			var timeConverter = new SiteTimeConverter(site);

			Cursor currentCursor = this.Cursor;
			this.Cursor = Cursors.WaitCursor;

			lock (this.dataGridView1)
			{
				var sr = new SaveTransactionsSR
				         {
					         SubType = SaveTransactionsSR.SaveTransactionSubType.SaveTranactionFlagsAndStatus,
					         IndividualDbTransaction = false,
					         Security = security,
					         CurrentSiteGuid = security.SiteGuid,
					         ConvertUnits = true
				         };

				foreach (DataGridViewRow selectedRow in this.dataGridView1.SelectedRows)
				{
					DataRowView row = this.dataGridView1.GetDataRow(selectedRow.Index);

					var selectedTransID = (string)row["TransID"];

					var trans = new TransactionFlagsAndStatusDO(selectedTransID)
					            {
						            TransStatus = TransactionStatus.Started,
						            FST = timeConverter.Now()
					            };

					sr.TransFlagsAndStatusCollection.Add(trans);
				}

				this.SaveTransactionWithServiceRequest(security, sr);
				this.SetPopupMenuEnableDisable();
				this.UpdateView();
			}

			this.Cursor = currentCursor;
		}

		private void SetSelectedToStopped(object sender, EventArgs e)
		{
			var security = AppDomain.CurrentDomain.GetData("Security") as SecurityClass;

			if (security == null)
			{
				throw new Exception("Security not in AppDomain");
			}

			if (security.HasRight(RIGHT.MODIFY_DISPATCH) == false)
			{
				return;
			}

			SiteClass site = FMChannelHelper.MakeCall<IClientDispatchService, SiteClass>(x => x.GetSite(security, security.SiteGuid));

			var timeConverter = new SiteTimeConverter(site);

			Cursor currentCursor = this.Cursor;
			this.Cursor = Cursors.WaitCursor;

			lock (this.dataGridView1)
			{
				var sr = new SaveTransactionsSR
				         {
					         SubType = SaveTransactionsSR.SaveTransactionSubType.SaveTranactionFlagsAndStatus,
					         IndividualDbTransaction = false,
					         Security = security,
					         CurrentSiteGuid = security.SiteGuid,
					         ConvertUnits = true
				         };

				foreach (DataGridViewRow selectedRow in this.dataGridView1.SelectedRows)
				{
					DataRowView row = this.dataGridView1.GetDataRow(selectedRow.Index);

					var selectedTransID = (string)row["TransID"];

					var trans = new TransactionFlagsAndStatusDO(selectedTransID)
					            {
						            TransStatus = TransactionStatus.Stopped,
						            TimeEnd = timeConverter.Now()
					            };

					sr.TransFlagsAndStatusCollection.Add(trans);
				}

				this.SaveTransactionWithServiceRequest(security, sr);
				this.SetPopupMenuEnableDisable();
				this.UpdateView();
			}

			this.Cursor = currentCursor;
		}

		private void ShowReminderForm()
		{
			if (this.InvokeRequired)
			{
				ShowReminderFormCallback spfc = this.ShowReminderForm;

				this.Invoke(spfc);
			}
			else
			{
				var reminderForm = new AppointmentReminderForm(this);

				reminderForm.ShowDialog(this);
			}
		}

		private void ShowRowNumber()
		{
			lock (this.dataGridView1)
			{
				int rowIndex = 1;

				foreach (DataGridViewRow row in this.dataGridView1.Rows)
				{
					row.HeaderCell.Value = rowIndex.ToString(CultureInfo.InvariantCulture);
					rowIndex++;
				}
			}
		}

		private void StatusBarToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.statusStrip.Visible = this.statusBarToolStripMenuItem.Checked;
		}

		private void StatusComboSelectedIndexChanged(object sender, EventArgs e)
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

		private void StoreDisplayGridColumnPositions()
		{
			string appConfigItem = this.Security.UserID + "Grid1ColumnPositions";
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
				combinedColumnPositionString += this.dataGridView1ColumnPositions[iLoop];
				combinedColumnPositionString += ";";
			}

			// write the data to the config file
			config.AppSettings.Settings.Add(appConfigItem, combinedColumnPositionString);

			// Save the configuration file.
			config.Save(ConfigurationSaveMode.Modified);

			// Force a reload of a changed section.
			ConfigurationManager.RefreshSection("appSettings");
		}

		private void StoreGrid1ColumnWidths()
		{
			string appConfigItem = this.Security.UserID + "Grid1ColumnWidths";
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
				combinedColumnPositionString += this.dataGridView1ColumnWidths[iLoop];
				combinedColumnPositionString += ";";
			}

			// write the data to the config file
			config.AppSettings.Settings.Add(appConfigItem, combinedColumnPositionString);

			// Save the configuration file.
			config.Save(ConfigurationSaveMode.Modified);

			// Force a reload of a changed section.
			ConfigurationManager.RefreshSection("appSettings");
		}

		private void ToolBarToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.toolStrip.Visible = this.toolBarToolStripMenuItem.Checked;
		}

		private void UpdateView(bool resetPolling = false)
		{
			lock (this.dataGridView1)
			{
				DispatchSR.BeginDate = this.BeginDatePicker.Value;
				DispatchSR.EndDate = this.EndDatePicker.Value;

				this.GetStatusValues(DispatchSR);
				this.GetAliases(DispatchSR);

				DispatchTransactionsDO data = this.dataAccess.GetTransactions(DispatchSR, resetPolling);

				this.RefreshView(data.Transactions.Tables[0]);

				this.dataGridView1.Select();
			}
		}

		private void ViewAllClick(object sender, EventArgs e)
		{
			// filter for all
			int selectedIndex = this.StatusCombo.FindString("{All}");

			if (selectedIndex >= 0)
			{
				this.StatusCombo.SelectedIndex = selectedIndex;
			}

			selectedIndex = this.RequestTypeCombo.FindString("{All}");

			if (selectedIndex >= 0)
			{
				this.RequestTypeCombo.SelectedIndex = selectedIndex;
			}

			selectedIndex = this.vehicleComboBox.FindString("{All}");

			if (selectedIndex >= 0)
			{
				this.vehicleComboBox.SelectedIndex = selectedIndex;
			}

			this.dataGridView1.Select();
		}

		private void ViewArrivedClick(object sender, EventArgs e)
		{
			// filter for arrived
			int selectedIndex = this.StatusCombo.FindString("Arrived");

			if (selectedIndex >= 0)
			{
				this.StatusCombo.SelectedIndex = selectedIndex;
			}

			this.FocusOnVehicleComboBox();
		}

		private void ViewCanceledClick(object sender, EventArgs e)
		{
			// filter for cancelled
			int selectedIndex = this.StatusCombo.FindString("Cancelled");

			if (selectedIndex >= 0)
			{
				this.StatusCombo.SelectedIndex = selectedIndex;
			}

			this.FocusOnVehicleComboBox();
		}

		private void ViewCompletedClick(object sender, EventArgs e)
		{
			// filter for Complete
			int selectedIndex = this.StatusCombo.FindString("Completed");

			if (selectedIndex >= 0)
			{
				this.StatusCombo.SelectedIndex = selectedIndex;
			}

			this.FocusOnVehicleComboBox();
		}

		private void ViewDispatchedClick(object sender, EventArgs e)
		{
			// filter for dispatch
			int selectedIndex = this.StatusCombo.FindString("Dispatched");

			if (selectedIndex >= 0)
			{
				this.StatusCombo.SelectedIndex = selectedIndex;
			}

			this.FocusOnVehicleComboBox();
		}

		private void ViewRequestedClick(object sender, EventArgs e)
		{
			// filter for requested
			int selectedIndex = this.StatusCombo.FindString("Requested");

			if (selectedIndex >= 0)
			{
				this.StatusCombo.SelectedIndex = selectedIndex;
			}

			this.FocusOnVehicleComboBox();
		}

		private void ViewStartedClick(object sender, EventArgs e)
		{
			// filter for started
			int selectedIndex = this.StatusCombo.FindString("Started");

			if (selectedIndex >= 0)
			{
				this.StatusCombo.SelectedIndex = selectedIndex;
			}

			this.FocusOnVehicleComboBox();
		}

		private void ViewStoppedClick(object sender, EventArgs e)
		{
			// filter for started
			int selectedIndex = this.StatusCombo.FindString("Stopped");

			if (selectedIndex >= 0)
			{
				this.StatusCombo.SelectedIndex = selectedIndex;
			}

			this.FocusOnVehicleComboBox();
		}

		private void ArrivalToolStripMenuItemClick(object sender, EventArgs e)
		{
			try
			{
				this.SetSelectedToArrived(sender, e);
				this.ChangeStatusFilterIfNecessary(TransactionStatus.Arrived);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void CancelToolStripMenuItemClick(object sender, EventArgs e)
		{
			try
			{
				this.CancelRequest(sender, e);
				this.ChangeStatusFilterIfNecessary(TransactionStatus.Cancelled);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void ChangeOfOperatorToolStripMenuItemClick(object sender, EventArgs e)
		{
			try
			{
				var changeForm = new ChangeOperatorStatusForm();
				changeForm.ShowDialog(this);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void ContentsToolStripMenuItemClick(object sender, EventArgs e)
		{
			try
			{
				Help.ShowHelp(this, @"FMDispatchHelp.chm", HelpNavigator.TableOfContents);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void DataGridView1CellClick(object sender, DataGridViewCellEventArgs e)
		{
			lock (this.dataGridView1)
			{
				if (e.RowIndex == -1 && e.ColumnIndex == -1)
				{
					this.columnBeingSorted = this.dataGridView1.Columns["Requested"];
					this.currentSortOrder = SortOrder.Ascending;
					this.dataGridView1.ClearSelection();

					if (this.columnBeingSorted != null)
					{
						this.dataGridView1.Sort(this.columnBeingSorted, ListSortDirection.Ascending);
					}
				}

				if ((e.RowIndex == -1) && (e.ColumnIndex >= 0))
				{
					this.columnBeingSorted = this.dataGridView1.Columns[e.ColumnIndex];

					switch (this.currentSortOrder)
					{
						case SortOrder.Ascending:
							this.currentSortOrder = SortOrder.Descending;
							this.dataGridView1.Sort(this.columnBeingSorted, ListSortDirection.Descending);
							break;
						case SortOrder.Descending:
							this.currentSortOrder = SortOrder.Ascending;
							this.dataGridView1.Sort(this.columnBeingSorted, ListSortDirection.Ascending);
							break;
						case SortOrder.None:
							this.currentSortOrder = SortOrder.Ascending;
							this.dataGridView1.Sort(this.columnBeingSorted, ListSortDirection.Ascending);
							break;
					}
				}
			}
		}

		private void DataGridView1CellDoubleClick(object sender, DataGridViewCellEventArgs e)
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

		private void DataGridView1CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (this.dataGridView1.ValidIndex(e.RowIndex) == false)
			{
				return;
			}

			// Transactions need to be displayed in different colors by requirement
			// Red = open defuel
			// Gray = canceled or Complete requests
			// Blue = open refuel
			// Black = other dispatch transactions

			// Find the data record
			DataRowView row = this.dataGridView1.GetDataRow(e.RowIndex);

			// Set the appropriate color
			var aliasName = (string)row["AliasName"];
			var status = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), (string)row["TransactionStatus"]);
			bool transactionStatusOpen = TransactionStatusOpen(status);

			e.CellStyle.SelectionBackColor = Color.Black;

			if (transactionStatusOpen == false)
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

		private void DataGridView1CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
		{
			if (this.dataGridView1.ValidIndex(e.RowIndex) == false)
			{
				return;
			}

			string dataFieldName = this.dataGridView1.Columns[e.ColumnIndex].DataPropertyName;
			e.Value = this.dataGridView1.GetDataRow(e.RowIndex)[dataFieldName];
		}

		private void DataGridView1ColumnDisplayIndexChanged(object sender, DataGridViewColumnEventArgs e)
		{
			this.dataGridView1ColumnPositions[e.Column.Index] = e.Column.DisplayIndex;
		}

		private void DataGridView1ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			const int RequestedColumnIndex = 2;
			const int CancelColumnIndex = 23;
			const int FuelAdditiveFlagIndex = 37;

			if (e.ColumnIndex == RequestedColumnIndex)
			{
				// sort by request date;				
				foreach (DataGridViewRow dgvRow in this.dataGridView1.SelectedRows)
				{
					dgvRow.Selected = false;
				}

				// selected the record that is the latest requested so the latest will always show in the window.
				DateTime dtLastestRequested = DateTime.MinValue;

				foreach (DataGridViewRow dgvRow in this.dataGridView1.Rows)
				{
					DataRowView drvRow = this.dataGridView1.GetDataRow(dgvRow.Index);
					var dtRequested = ((DateTimeOffset) drvRow["RequestedDateTime"]).DateTime;

					if (dtLastestRequested < dtRequested)
					{
						dtLastestRequested = dtRequested;
					}
				}

				// find the latest and set it to selected. 
				foreach (DataGridViewRow dgvRow in this.dataGridView1.Rows)
				{
					DataRowView drvRow = this.dataGridView1.GetDataRow(dgvRow.Index);
					var dtCurrentRequested = ((DateTimeOffset)drvRow["RequestedDateTime"]).DateTime;

					if (dtCurrentRequested == dtLastestRequested)
					{
						dgvRow.Selected = true;

						// Scroll to the this row.
						this.dataGridView1.FirstDisplayedScrollingRowIndex = dgvRow.Index;
					}
				}
			}
			else if (e.ColumnIndex == CancelColumnIndex || e.ColumnIndex == FuelAdditiveFlagIndex)
			{
				// Ignore sorting on the Cancelled and Fuel Additive Flag columns since they are boolean.
				// The column property when set to not sortable throws an error.
				// Therefore, we are ignoring it here.
			}
		}

		private void DataGridView1MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				// get the selected row and column
				DataGridView.HitTestInfo hti = this.dataGridView1.HitTest(e.X, e.Y);

				if (hti.RowIndex < 0)
				{
					// user selected a blank area in the grid
					return;
				}

				if (this.dataGridView1.Rows[hti.RowIndex].Selected == false)
				{
					this.dataGridView1.Rows[hti.RowIndex].Selected = true;
				}

				if (this.dataGridView1.SelectedRows.Count <= 0)
				{
					return;
				}

				this.SetPopupMenuEnableDisable();

				// popup the context menu
				this.popupContextMenuForDataGrid.Show(this.dataGridView1, new System.Drawing.Point(e.X, e.Y));
			}
		}

		private void DataGridView1OnKeyDown(object sender, KeyEventArgs e)
		{
			// if the user presses the space bar and there are selections open the popup menu
			if (e.KeyCode == Keys.Space && this.dataGridView1.SelectedRows.Count > 0)
			{
				// get the menu position based on the first selected item
				if (this.dataGridView1.SelectedRows.Count > 0)
				{
					Rectangle rt = Screen.PrimaryScreen.Bounds;
					this.SetPopupMenuEnableDisable();

					// popup the context menu
					this.popupContextMenuForDataGrid.Show(this.dataGridView1, new System.Drawing.Point(rt.Width / 4, rt.Height / 4));
				}
			}
			else if (e.KeyCode == Keys.F7 || e.KeyCode == Keys.F8)
			{
				if (e.KeyCode == Keys.F7 && e.Modifiers == Keys.None && !this.arrivalToolStripMenuItem.Enabled)
				{
					this.PriorFilter = this.StatusCombo.SelectedItem;
					this.ViewDispatchedClick(sender, e);
				}
				else if (e.KeyCode == Keys.F7 && e.Modifiers == Keys.Control && !this.startOfServiceToolStripMenuItem.Enabled)
				{
					this.PriorFilter = this.StatusCombo.SelectedItem;
					this.ViewArrivedClick(sender, e);
				}
				else if (e.KeyCode == Keys.F8 && e.Modifiers == Keys.Control && !this.stopOfServiceToolStripMenuItem.Enabled)
				{
					this.PriorFilter = this.StatusCombo.SelectedItem;
					this.ViewStartedClick(sender, e);
				}
				else if (e.KeyCode == Keys.F8 && e.Modifiers == Keys.None && !this.serviceCompletionToolStripMenuItem.Enabled)
				{
					bool useArrivalTime = Convert.ToBoolean(ConfigurationManager.AppSettings["Use Arrival Time"]);
					bool useStartTime = Convert.ToBoolean(ConfigurationManager.AppSettings["Use Start Time"]);
					bool useStopTime = Convert.ToBoolean(ConfigurationManager.AppSettings["Use Stop Time"]);

					this.PriorFilter = this.StatusCombo.SelectedItem;

					if (useStopTime)
					{
						this.ViewStoppedClick(sender, e);
					}
					else if (useStartTime)
					{
						this.ViewStartedClick(sender, e);
					}
					else if (useArrivalTime)
					{
						this.ViewArrivedClick(sender, e);
					}
					else
					{
						this.ViewDispatchedClick(sender, e);
					}
				}
			}
			else if (e.KeyCode == Keys.Escape)
			{
				if (this.PriorFilter != null)
				{
					this.StatusCombo.SelectedItem = this.PriorFilter;
					this.PriorFilter = null;
					this.dataGridView1.Select();
				}
			}
		}

		private void DataGridView1RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
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

		private void DataGridView1SelectionChanged(object sender, EventArgs e)
		{
			try
			{
				if (this.dataGridView1.SelectedRows.Count > 0 && Security.HasRight(RIGHT.MODIFY_DISPATCH))
				{
					this.SetPopupMenuEnableDisable();

					// Set the cancel and relog menu availability
					this.cancelToolStripMenuItem.Enabled = true;
					this.toolStripCancelButton.Enabled = true;
					this.relogToolStripMenuItem.Enabled = true;
					this.toolStripRelogButton.Enabled = true;

					lock (this.dataGridView1)
					{
						foreach (DataGridViewRow row in this.dataGridView1.SelectedRows)
						{
							DataRowView dataRow = this.dataGridView1.GetDataRow(row.Index);

							var status = (TransactionStatus)dataRow["TransactionStatusInt"];

							// if any selected transaction has a status of posted or Pending disable everything
							if (status == TransactionStatus.Posted || status == TransactionStatus.Pending)
							{
								this.cancelToolStripMenuItem.Enabled = false;
								this.toolStripCancelButton.Enabled = false;
								this.relogToolStripMenuItem.Enabled = false;
								this.toolStripRelogButton.Enabled = false;
								break;
							}

							if (status == TransactionStatus.Completed || status == TransactionStatus.Cancelled)
							{
								this.cancelToolStripMenuItem.Enabled = false;
								this.toolStripCancelButton.Enabled = false;
								break;
							}

							var transTypeID = (short)dataRow["TransTypeID"];

							if (transTypeID != 4 && transTypeID != 6)
							{
								this.relogToolStripMenuItem.Enabled = false;
								this.toolStripRelogButton.Enabled = false;
							}
						}
					}
				}
				else
				{
					this.cancelToolStripMenuItem.Enabled = false;
					this.toolStripCancelButton.Enabled = false;
					this.relogToolStripMenuItem.Enabled = false;
					this.toolStripRelogButton.Enabled = false;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void DispatchToolStripMenuItemClick(object sender, EventArgs e)
		{
			try
			{
				string[] selectedTransID;
				lock (this.dataGridView1)
				{
					selectedTransID = new string[this.dataGridView1.SelectedRows.Count];

					for (int iLoop = 0; iLoop < this.dataGridView1.SelectedRows.Count; iLoop++)
					{
						DataRowView row = this.dataGridView1.GetDataRow(this.dataGridView1.SelectedRows[iLoop].Index);
						selectedTransID[iLoop] = (string)row["TransID"];
					}
				}

				var dispatch = new DispatchForm(
					this.dataAccess, DispatchSR.Translations, selectedTransID, null, null, this.operationLockDate);
				dispatch.ShowDialog(this);

				if (dispatch.Transactions != null)
				{
					this.UpdateView();
					this.Reselect(dispatch.Transactions);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void EvacuateToolStripMenuItemOnClick(object sender, EventArgs e)
		{
			var openEvacuateForm = new EvacuateForm(this.BeginDatePicker.Value, DispatchSR.Translations);
			openEvacuateForm.ShowDialog(this);
		}

		private void FastLogFillstandToolStripMenuItemClick(object sender, EventArgs e)
		{
			var fuelRequestForm = new FuelRequestForm(this.operationLockDate)
				                      {
					                      RequestType =
						                      FuelRequestForm.REQUEST_TYPE.FastLogFillStand
				                      };

			fuelRequestForm.ShowDialog(this);

			// Setup to select the current TransID
			if (fuelRequestForm.lastTransaction != null)
			{
				this.dataGridView1.ClearSelection();
				this.dataGridView1.CurrentCell = null;
				this.currentRowTransID = fuelRequestForm.lastTransaction.TransID;
				this.UpdateView();
			}
		}

		private void FastLogToolStripMenuItemClick(object sender, EventArgs e)
		{
			var fuelRequestForm = new FuelRequestForm(this.operationLockDate)
				                      {
					                      RequestType = FuelRequestForm.REQUEST_TYPE.FastLog
				                      };

			fuelRequestForm.ShowDialog(this);

			// Setup to select the current TransID
			if (fuelRequestForm.lastTransaction != null)
			{
				this.dataGridView1.ClearSelection();
				this.dataGridView1.CurrentCell = null;
				this.currentRowTransID = fuelRequestForm.lastTransaction.TransID;
				this.UpdateView();
			}
		}

		private void FillstandCompletionToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.SetFillStandCompleted(sender, e);
		}

		private void FlightLineStatusToolStripMenuItemClick(object sender, EventArgs e)
		{
			try
			{
				var dispatch = new DispatchForm(
					this.dataAccess, DispatchSR.Translations, null, null, null, this.operationLockDate)
					               {
						               DisplayMode = DispatchForm.DisplayModeType.FlightLineStatus
					               };

				dispatch.ShowDialog(this);

				if (dispatch.Transactions != null)
				{
					this.Reselect(dispatch.Transactions);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void FocusOnVehicleComboBox()
		{
			this.vehicleComboBox.Focus();
			this.vehicleComboBox.SelectAll();
			this.vehicleComboBox.Capture = true;
		}

		private void IndexToolStripMenuItemClick(object sender, EventArgs e)
		{
			try
			{
				Help.ShowHelpIndex(this, @"FMDispatchHelp.chm");
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void JulianDateTimerTick(object sender, EventArgs e)
		{
			try
			{
				SiteClass site =
					FMChannelHelper.MakeCall<IClientDispatchService, SiteClass>(x => x.GetSite(this.Security, this.Security.SiteGuid));
				var timeConverter = new SiteTimeConverter(site);

				// Initial Julian Date Display
				this.julianDateLabel.Text = "Date: " + timeConverter.Now().ToMilitaryJulianDateString();

				// Also set the date ranges to the new time
				this.BeginDatePicker.ValueChanged -= this.BeginDatePickerValueChanged;
				this.EndDatePicker.ValueChanged -= this.EndDatePickerValueChanged;

				this.BeginDatePicker.Value = timeConverter.Today().DateTime;
				this.currentSelectedBeginDatePicker = this.BeginDatePicker.Value;
				this.EndDatePicker.Value = timeConverter.Today().DateTime;
				this.currentSelectedEndDatePicker = this.EndDatePicker.Value;

				this.BeginDatePicker.ValueChanged += this.BeginDatePickerValueChanged;
				this.EndDatePicker.ValueChanged += this.EndDatePickerValueChanged;

				this.julianDateTimer.Interval = (((24 - timeConverter.Now().Hour - 1) * 60 * 60)
				                                 + ((60 - timeConverter.Now().Minute - 1) * 60) + (60 - DateTime.Now.Second)) * 1000;

				this.RefreshToolStripMenuItemClick(this, null);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void OperationToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.SetPopupMenuEnableDisable();
		}

		private void OperationToolStripMenuItemDropDownOpening(object sender, EventArgs e)
		{
			//string useArrivalTime = ConfigurationManager.AppSettings["Use Arrival Time"];
			//string useStartTime = ConfigurationManager.AppSettings["Use Start Time"];
			//string useStopTime = ConfigurationManager.AppSettings["Use Stop Time"];
		}

		private void OptionalTimesToolStripMenuItem1Click(object sender, EventArgs e)
		{
			var optionalTimesForm = new OptionalTimesForm();
			optionalTimesForm.ShowDialog(this);
		}

		private void QueryWriterToolStripMenuItemClick(object sender, EventArgs e)
		{
			try
			{
				string baseAddress = ConfigurationManager.AppSettings["WebAppAddress"];

				if (string.IsNullOrEmpty(baseAddress))
				{
					throw new ApplicationException("WebAppAddress not in configuration file.");
				}

				var embeddedBrowser = new EmbeddedBrowser(baseAddress + "/QueryWriterWebApp/ManageQueriesForm.aspx");
				embeddedBrowser.ShowDialog(this);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void RecirculationToolStripMenuItemClick(object sender, EventArgs e)
		{
			var recirculationForm = new RecirculationForm();
			recirculationForm.ShowDialog(this);
		}

		private void RefreshToolStripMenuItemClick(object sender, EventArgs e)
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

		private void RelogToolStripMenuItemClick(object sender, EventArgs e)
		{
			try
			{
				this.Relog(sender, e);
				this.ChangeStatusFilterIfNecessary(TransactionStatus.Requested);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void ReportsToolStripMenuItemClick(object sender, EventArgs e)
		{
			try
			{
				string baseAddress = ConfigurationManager.AppSettings["WebAppAddress"];

				if (string.IsNullOrEmpty(baseAddress))
				{
					throw new ApplicationException("WebAppAddress not in configuration file.");
				}

				var embeddedBrowser = new EmbeddedBrowser(baseAddress + "/FMReportWebMain/FMReportDynamicSelectionPage.aspx");
				embeddedBrowser.ShowDialog(this);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void RequestRefuelToolStripMenuItemClick(object sender, EventArgs e)
		{
			var fuelRequestForm = new FuelRequestForm(this.operationLockDate)
				                      {
					                      RequestType =
						                      FuelRequestForm.REQUEST_TYPE.RequestFuel
				                      };

			fuelRequestForm.ShowDialog(this);

			// Setup to select the current TransID
			if (fuelRequestForm.lastTransaction != null)
			{
				this.dataGridView1.ClearSelection();
				this.dataGridView1.CurrentCell = null;
				this.currentRowTransID = fuelRequestForm.lastTransaction.TransID;
				this.UpdateView();
			}
		}

		private void ServiceCompletionToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.SetSelectedToCompleted(sender, e);
			this.ChangeStatusFilterIfNecessary(TransactionStatus.Completed);
		}

		private void StandByToolStripMenuItemClick(object sender, EventArgs e)
		{
			var standbyStatusForm = new StandbyStatusForm();

			if (standbyStatusForm.ShowDialog(this) == DialogResult.OK)
			{
				// get the person and the equipment selected
				string selectedPerson = standbyStatusForm.SelectedPerson;
				string selectedEquipment = standbyStatusForm.SelectedEquipment;

				// set the values for the dispatch dialog and then open
				try
				{
					string[] selectedTransID;

					lock (this.dataGridView1)
					{
						selectedTransID = new string[this.dataGridView1.SelectedRows.Count];

						for (int iLoop = 0; iLoop < this.dataGridView1.SelectedRows.Count; iLoop++)
						{
							DataGridViewRow selectedRow = this.dataGridView1.SelectedRows[iLoop];
							DataRowView row = this.dataGridView1.GetDataRow(selectedRow.Index);
							selectedTransID[iLoop] = (string)row["TransID"];
						}
					}

					var dispatch = new DispatchForm(
													this.dataAccess, 
													DispatchSR.Translations, 
													selectedTransID, 
													selectedPerson, 
													selectedEquipment, 
													this.operationLockDate);
					dispatch.ShowDialog(this);

					if (dispatch.Transactions != null)
					{
						this.Reselect(dispatch.Transactions);
					}
				}
				catch (Exception except)
				{
					this.ErrorHandler(except);
				}
			}
		}

		private void StartOfServiceToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.SetSelectedToStarted(sender, e);
			this.ChangeStatusFilterIfNecessary(TransactionStatus.Started);
		}

		private void StopToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.SetSelectedToStopped(sender, e);
			this.ChangeStatusFilterIfNecessary(TransactionStatus.Stopped);
		}

		private void ToolStripItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
		}

		private void TotalAndAverageToolStripMenuItemClick(object sender, EventArgs e)
		{
			var totalandAverageForm = new TotalAndAverageForm(
				this.dataAccess, DispatchSR.Translations, this.BeginDatePicker.Value, this.EndDatePicker.Value);
			totalandAverageForm.ShowDialog(this);
		}

		private void TransientToolStripMenuItemClick(object sender, EventArgs e)
		{
			var fuelRequestForm = new FuelRequestForm(this.operationLockDate)
				                      {
					                      RequestType = FuelRequestForm.REQUEST_TYPE.Transient
				                      };

			fuelRequestForm.ShowDialog(this);

			// Setup to select the current TransID
			if (fuelRequestForm.lastTransaction != null)
			{
				this.dataGridView1.ClearSelection();
				this.dataGridView1.CurrentCell = null;
				this.currentRowTransID = fuelRequestForm.lastTransaction.TransID;
				this.UpdateView();
			}
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
		private void VehicleComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				lock (this.dataGridView1)
				{
					// Limit grid view to only this vehicle
					if (this.vehicleComboBox.SelectedIndex > 0)
					{
						string vehicleXREF = this.vehicleComboBox.Text;
						this.dataGridView1.RowFilter = string.Format("ISSPTNUM = '{0}'", vehicleXREF);

						// now select all the vehicle rows
						this.dataGridView1.SelectAll();
					}
					else
					{
						//// when the filter is cleared the grid is deselecting all of the records
						//// we need to store the current selected records so we can then restore them
						//// Clear the filter
						//var selectedTransID = new string[this.dataGridView1.SelectedRows.Count];
						//string rowTransID = string.Empty;
						//int numberOfSelectedRows = this.dataGridView1.SelectedRows.Count;

						//// store the current row so it can be restored
						//if (this.dataGridView1.CurrentRow != null)
						//{
						//	DataRowView currentRow = this.dataGridView1.GetDataRow(this.dataGridView1.CurrentRow.Index);

						//	if (currentRow != null)
						//	{
						//		rowTransID = (string)currentRow["TransID"];
						//	}
						//}

						//for (int iLoop = 0; iLoop < numberOfSelectedRows; iLoop++)
						//{
						//	DataGridViewRow selectedRow = this.dataGridView1.SelectedRows[iLoop];
						//	DataRowView row = this.dataGridView1.GetDataRow(selectedRow.Index);
						//	selectedTransID[iLoop] = (string)row["TransID"];
						//}

						this.dataGridView1.RowFilter = string.Empty;

						//// we need to set the current cell first since the grid will change the number of selections to 1 when it is set
						//foreach (DataGridViewRow row in this.dataGridView1.Rows)
						//{
						//	DataRowView dataRow = this.dataGridView1.GetDataRow(row.Index);
						//	if (numberOfSelectedRows > 0)
						//	{
						//		row.Selected = false;
						//	}

						//	if (!string.IsNullOrEmpty(rowTransID) && rowTransID.Equals((string)dataRow["TransID"]))
						//	{
						//		// restore the current selection
						//		// the only way to do this is by setting the currentcell variable since the currentrow is read only
						//		this.dataGridView1.CurrentCell = row.Cells[0];
						//	}
						//}
					}

					this.RenumberGrid(this.dataGridView1);
					this.dataGridView1.Select();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}
		#endregion
	}

	public sealed class CustomDataGridView : DataGridView
	{
		#region Fields
		private DataView dataView;
		private DateTime dateOfOriginalData;
		#endregion

		#region Constructors and Destructors
		public CustomDataGridView()
		{
			this.DoubleBuffered = true;
			this.AutoGenerateColumns = false;
			this.AllowUserToAddRows = false;
			this.AllowUserToDeleteRows = false;
		}
		#endregion

		#region Public Properties
		public DataView DispatchDataView
		{
			get
			{
				return this.dataView;
			}

			set
			{
				this.dateOfOriginalData = DateTime.Now;

				// Must set rowcount to 0 before saving data view (datagridview bug)
				if (value != null && value.Count < this.RowCount)
				{
					this.RowCount = 0;
				}

				this.dataView = value;
				this.RowCount = (value == null) ? 0 : value.Count;
			}
		}

		public string RowFilter
		{
			get
			{
				if (this.dataView != null)
				{
					return this.dataView.RowFilter;
				}
				
				return null;
			}

			set
			{
				if (value != null)
				{
					if (this.VirtualMode == false)
					{
						this.dataView.RowFilter = value;
					}
					else
					{
						// this.RowCount = 0; // Must set rowcount to 0 before saving data view (datagridview bug)
						this.dataView.RowFilter = value;

						// Must set rowcount to 0 before saving data view (datagridview bug)
						if (this.dataView.Count < this.RowCount)
						{
							this.RowCount = 0;
						}

						this.RowCount = this.dataView.Count;
					}
				}
			}
		}
		#endregion

		#region Public Methods and Operators
		public DataRowView GetDataRow(int rowIndex)
		{
			return this.DispatchDataView[rowIndex];
		}

		/// <summary>
		/// This will merge transaction updates into a single up todate set.  We use this one incase a pending timer has older data
		///     then what is already in the grid.  This can happen when a user performs something that locks the grid and then the timer
		///     fires waiting for the grid to be unlocked.  While the first request still has the grid locked, it querys for fresh data.
		///     After it finishes, the pending lock is aquired by the timer that has old data then trys to update the grid with the old data.
		/// </summary>
		/// <param name="currentTransactionSet">
		/// The current transaction set, this will get udpated in place
		/// </param>
		/// <param name="updatedTransactionSet">
		/// the transactions needed to be merged in
		/// </param>
		/// <param name="timeCreated">
		/// The time Created.
		/// </param>
		public void MergeTransactionsAndUpdateView(DataTable currentTransactionSet, DataTable updatedTransactionSet, DateTime timeCreated)
		{
			// only process the newest data
			if (timeCreated < this.dateOfOriginalData)
			{
				// System.Diagnostics.Debug.WriteLine(String.Format("old data detected {0}, {1}", timeCreated, dateOfOriginalData));
				return;
			}

			DataView oldView = this.DispatchDataView;
			var newView = new DataView(updatedTransactionSet);

			if (oldView != null)
			{
				newView.RowFilter = oldView.RowFilter;
				newView.Sort = oldView.Sort;
			}

			this.DispatchDataView = newView;
			this.dateOfOriginalData = timeCreated;
		}

		/// <summary>
		/// This will merge transaction updates into a single up todate set.  We use this one incase a pending timer has older data
		///     then what is already in the grid.  This can happen when a user performs something that locks the grid and then the timer
		///     fires waiting for the grid to be unlocked.  While the first request still has the grid locked, it querys for fresh data.
		///     After it finishes, the pending lock is aquired by the timer that has old data then trys to update the grid with the old data.
		/// </summary>
		/// <param name="updatedTransactionSet">
		/// the transactions needed to be merged in
		/// </param>
		/// <param name="timeCreated">
		/// The time Created.
		/// </param>
		public void MergeTransactionsAndUpdateView(DataTable updatedTransactionSet, DateTime timeCreated)
		{
			// only process the newest data
			if (timeCreated < this.dateOfOriginalData)
			{
				// System.Diagnostics.Debug.WriteLine(String.Format("old data detected {0}, {1}", timeCreated, dateOfOriginalData));
				return;
			}

			this.dateOfOriginalData = timeCreated;

			DataView oldView = this.DispatchDataView;
			var newView = new DataView(updatedTransactionSet);

			if (oldView != null)
			{
				newView.RowFilter = oldView.RowFilter;
				newView.Sort = oldView.Sort;
			}

			this.DispatchDataView = newView;
		}

		public override void Sort(DataGridViewColumn dataGridViewColumn, ListSortDirection direction)
		{
			if (this.VirtualMode == false)
			{
				base.Sort(dataGridViewColumn, direction);
			}
			else
			{		
				string dataPropertyName = dataGridViewColumn.DataPropertyName;

				if (direction == ListSortDirection.Descending)
				{
					this.DispatchDataView.Sort = dataPropertyName + " DESC";
				}
				else
				{
					this.DispatchDataView.Sort = dataPropertyName;
				}

				SortOrder glyph = (direction == ListSortDirection.Ascending) ? SortOrder.Ascending : SortOrder.Descending;

				foreach (DataGridViewColumn column in this.Columns)
				{
					if (column.DataPropertyName == dataPropertyName)
					{
						column.HeaderCell.SortGlyphDirection = glyph;
					}
					else
					{
						column.HeaderCell.SortGlyphDirection = SortOrder.None;
					}
				}

				this.Refresh();
			}
		}

		/// <summary>
		/// Only use when grid is transactions and in virtual mode
		/// </summary>
		/// <param name="index">
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ValidIndex(int index)
		{
			var dispatchDataView = this.DispatchDataView;
			bool toRet = dispatchDataView != null && (!((dispatchDataView.Count == 0) && index >= 0)
			                                               || (index >= dispatchDataView.Count));

			return toRet;
		}
		#endregion
	}
}