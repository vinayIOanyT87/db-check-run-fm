using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using System.Reflection;
using EngineeringUnitsLibrary;

namespace ASCReporter
{
	/// <summary>
	/// Form for reading or explicitly setting the maximum single day issuance of ground products
	/// of a specific type (currently Diesel fuels/heating oils, Leaded gasolines, or Unleaded gasolines)
	/// </summary>
	public partial class GroundActivity : Form
	{
		/// <summary>
		/// Enumeration identifying the type of ground fuels to be queried on
		/// </summary>
		public enum GroundActivityType
		{
			/// <summary>
			/// Indicates Diesel fuels and heating oils
			/// </summary>
			Diesel,
			/// <summary>
			/// Indicates Leaded gasolines
			/// </summary>
			Leaded,
			/// <summary>
			/// Indicates Unleaded gasolines
			/// </summary>
			Unleaded
		}

		/// <summary>
		/// Constructor for Ground Activity form
		/// </summary>
		/// <param name="refuelingType">one of the <see cref="GroundActivityType">GroundActivityType</see> enumeration
		/// identitying the type of fuels to include for calculation</param>
		/// <param name="eDate">End date of the year (365 consecutive days) to compile transactions for</param>
		/// <param name="conn">A configured SQL Server connection.  This connection needs to be open and have already created
		/// the #RefuelActivity and #DefuelActivity temporary tables</param>
		/// <param name="sGuid">Site Guid from current security context - expected to be 1</param>
		/// <param name="uName">user name/id from current security context - used for auditability</param>
		public GroundActivity(GroundActivityType refuelingType, DateTime eDate, SqlConnection conn, Guid sGuid, string uName)
		{
			InitializeComponent();
			this.connection = conn;
			this.fuelType = refuelingType;
			this.siteGuid = sGuid;
			this.userName = uName;
			this.endDate = eDate;

			this.maxQuantity = 0.0F;

			string fuelTypeString;

			switch (this.fuelType)
			{
				case GroundActivityType.Diesel:
					this.labelMaxOneDay.Text = Strings.DieselMaxCaption;
					fuelTypeString = "Diesel";
					break;
				case GroundActivityType.Leaded:
					this.labelMaxOneDay.Text = Strings.LeadedMaxCaption;
					fuelTypeString = "Leaded";
					break;
				case GroundActivityType.Unleaded:
					this.labelMaxOneDay.Text = Strings.UnleadedMaxCaption;
					fuelTypeString = "Unleaded";
					break;
				default:
					fuelTypeString = "Non-fuel";
					break;
			}

			this.isProcessing = true;

			SqlCommand	cmdShadowExcluded = this.connection.CreateCommand();
			cmdShadowExcluded.CommandText = Strings.CreateGroundExcludedDaysShadowTable;
			cmdShadowExcluded.ExecuteNonQuery();

			cmdShadowExcluded.CommandText = "INSERT INTO #ASC_GROUND_EXCLUDED_DAYS_SHADOW SELECT * FROM #ASC_GROUND_EXCLUDED_DAYS WHERE FuelType = @Class";
			cmdShadowExcluded.Parameters.AddWithValue("@Class",fuelTypeString);
			cmdShadowExcluded.ExecuteNonQuery();

			this.cmdGetGroundExcluded = this.connection.CreateCommand();
			this.cmdGetGroundExcluded.CommandText = "SELECT ndx, FuelType, ExcludedDay "
															+ "FROM #ASC_GROUND_EXCLUDED_DAYS "
															+ "WHERE FuelType = @Class";
			this.cmdGetGroundExcluded.CommandType = CommandType.Text;
			this.cmdGetGroundExcluded.Parameters.AddWithValue("@Class", fuelTypeString);

			this.adptGetGroundExcluded = new SqlDataAdapter(cmdGetGroundExcluded);

			this.adptGetGroundExcluded.InsertCommand = new SqlCommand("INSERT INTO #ASC_GROUND_EXCLUDED_DAYS "
																						+ "(FuelType, ExcludedDay) "
																						+ "VALUES (@Class, @ExcludedDay)", this.connection);
			this.adptGetGroundExcluded.InsertCommand.Parameters.AddWithValue("@Class",fuelTypeString);
			this.adptGetGroundExcluded.InsertCommand.Parameters.Add("@ExcludedDay", SqlDbType.DateTime, 8, "ExcludedDay");

			this.adptGetGroundExcluded.UpdateCommand = new SqlCommand("UPDATE #ASC_GROUND_EXCLUDED_DAYS "
																						+ "SET ExcludedDay = @ExcludedDay "
																						+ "WHERE ndx = @ndx",
																						this.connection);
			this.adptGetGroundExcluded.UpdateCommand.Parameters.Add("@ExcludedDay", SqlDbType.DateTime, 8, "ExcludedDay");
			this.adptGetGroundExcluded.UpdateCommand.Parameters.Add("@ndx", SqlDbType.Int, 4, "ndx");
			this.adptGetGroundExcluded.UpdateCommand.Parameters["@ndx"].SourceVersion = DataRowVersion.Original;

			this.adptGetGroundExcluded.DeleteCommand = new SqlCommand("DELETE FROM #ASC_GROUND_EXCLUDED_DAYS "
																						+ "WHERE ndx = @ndx",
																						this.connection);
			this.adptGetGroundExcluded.DeleteCommand.Parameters.Add("@ndx", SqlDbType.Int, 4, "ndx");

			this.dsGetGroundExcluded = new DataSet();
			this.adptGetGroundExcluded.Fill(this.dsGetGroundExcluded);
			DataColumn[] primaryKey = new DataColumn[1];
			primaryKey[0] = this.dsGetGroundExcluded.Tables[0].Columns["ndx"];
			this.dsGetGroundExcluded.Tables[0].PrimaryKey = primaryKey;
			this.dsGetGroundExcluded.Tables[0].Columns["ndx"].AutoIncrement = true;

			this.excludedDaysGrid.AutoGenerateColumns = false;
			this.excludedDaysGrid.DataSource = this.dsGetGroundExcluded.Tables[0];
			this.excludedDaysGridDate.DataPropertyName = "ExcludedDay";

			this.PopulateExcludedDaysGrid();
		}

		#region Public Properties
		/// <summary>
		/// exposes to the caller the maximum quantity computed for or entered into the
		/// Maximum quantity box
		/// </summary>
		public int MaxQuantity
		{
			get 
			{
				int ret;
				int.TryParse(quantityMax.Text, out ret);
				return ret;
			}
			set 
			{
				quantityMax.Text = value.ToString();
			}
		}

		/// <summary>
		/// exposes to the caller whether or not the manual entry feature is being used.
		/// </summary>
		public bool MaxQuantityOverride
		{
			get
			{
				return manualEntry.Checked;
			}
			set
			{
				manualEntry.Checked = value;
			}
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Handles the user clicking an "Exclude" button.  Clicks anywhere else in the grid will be ignored
		/// </summary>
		/// <param name="senders">unused</param>
		/// <param name="e">event info.  We use the RowIndex and ColumnIndex from here</param>
		private void peakDaysGrid_CellContentClick(Object senders, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex != (peakDaysGrid.Columns.Count - 1))
			{
				this.peakDaysGrid.ClearSelection();
				return;
			}

			try
			{
				DateTime dateToExclude = (DateTime)peakDaysGrid.Rows[e.RowIndex].Cells[0].Value;
				DataRow newRow;

				newRow = this.dsGetGroundExcluded.Tables[0].NewRow();
				newRow["ExcludedDay"] = dateToExclude;
				this.dsGetGroundExcluded.Tables[0].Rows.Add(newRow);
				this.adptGetGroundExcluded.Update(this.dsGetGroundExcluded);

				this.PopulatePeakDaysGrid();
				this.PopulateExcludedDaysGrid();
			}
			catch (InvalidCastException err)
			{
				err.ToString(); // Simply silence the warning
			}
		}

		/// <summary>
		/// Handles the user clicking a "Restore" button.  Clicks anywhere else in the grid will be ignored
		/// </summary>
		/// <param name="senders">unused</param>
		/// <param name="e">event info.  We use the RowIndex and ColumnIndex from here</param>
		private void excludedDaysGrid_CellContentClick(Object senders, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex != (excludedDaysGrid.Columns.Count - 1))
			{
				this.excludedDaysGrid.ClearSelection();
				return;
			}

			this.excludedDaysGrid.Rows.RemoveAt(e.RowIndex);
			this.adptGetGroundExcluded.Update(this.dsGetGroundExcluded);

			this.PopulatePeakDaysGrid();
			this.PopulateExcludedDaysGrid();
		}

		/// <summary>
		/// Enables/disables controls as appropriate when the Manual Entry checkbox is clicked
		/// </summary>
		/// <param name="sender">unused</param>
		/// <param name="e">unused</param>
		private void manualEntry_CheckedChanged(object sender, EventArgs e)
		{
			if (manualEntry.Checked)
			{
				this.quantityMax.ReadOnly = false;
				this.quantityMax.TabStop = true;
			}
			else
			{
				this.quantityMax.ReadOnly = true;
				this.quantityMax.TabStop = false;
				this.PopulatePeakDaysGrid();
			}
		}

		private void quantityMax_TextChanged(object sender, EventArgs e)
		{
			float.TryParse(quantityMax.Text, out this.maxQuantity);
		}

		/// <summary>
		/// Prevents closing of the dialog while the asynchronous query is running
		/// Relies on proper setting of the isProcessing variable
		/// </summary>
		/// <param name="sender">unused</param>
		/// <param name="e">Event object.  Used to cancel close</param>
		private void GroundActivity_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.isProcessing)
			{
				MessageBox.Show(Strings.WaitCloseDatabaseAccess);
				e.Cancel = true;
			}
		}

		/// <summary>
		/// Handles proper data commit or rollback after the dialog is closed
		/// </summary>
		/// <param name="sender">unused</param>
		/// <param name="e">unused</param>
		private void GroundActivity_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (this.DialogResult == DialogResult.OK)
			{
				ClosedOK();
			}
			else
			{
				ClosedCancel();
			}
		}

		// Starts asynchonous actions after the form is loaded.  Window handle exists at this point.
		private void GroundActivity_Load(object sender, EventArgs e)
		{
			this.CompileTransactions();
		}
		#endregion

		#region Private Member Functions
		/// <summary>
		/// Refreshes the Peak Days grid after dialog start or change in excluded days.
		/// </summary>
		private void PopulatePeakDaysGrid()
		{
			this.dsGetGroundPeaks.Clear();
			this.adptGetGroundPeaks.Fill(this.dsGetGroundPeaks);
			this.peakDaysGrid.Refresh();

			if (!this.manualEntry.Checked)
			{
				this.maxQuantity = 0.0F;

				foreach (DataGridViewRow currentRow in this.peakDaysGrid.Rows)
				{
					if ((float)(double)currentRow.Cells[1].Value > this.maxQuantity)
					{
						this.maxQuantity = (float)(double)currentRow.Cells[1].Value;
					}
				}
				this.quantityMax.Text = this.maxQuantity.ToString("####0.#");
			}
		}


		/// <summary>
		/// Refreshes the Excluded Days grid after dialog start or change in excluded days.
		/// </summary>
		private void PopulateExcludedDaysGrid()
		{
			this.dsGetGroundExcluded.Clear();
			this.adptGetGroundExcluded.Fill(this.dsGetGroundExcluded);
			this.excludedDaysGrid.Refresh();
		}

		/// <summary>
		/// Starts the long-running query for transactions from the Accounting table (t_acct_tx5) in a separate thread
		/// using BeginExecuteNonQuery.
		/// </summary>
		private void CompileTransactions()
		{
			double conversion = 0.0;
			try
			{
				// Accounting database stores quantities in liters, but USAF wants quantities in US Gallons (liquid).
				//converter.ConvertUnits(1.0, CU_UNIT.FMV_Litre, ref conversion, CU_UNIT.FMV_USGal, 0.0);
				conversion = EngineeringUnits.Convert ( 1.0, ENGINEERING_UNIT.FMV_Litre, ENGINEERING_UNIT.FMV_USGal, 0 );
			}
			catch (Exception e)
			{
				e.ToString();
				MessageBox.Show(ErrorMessages.UnableToLoadConvertEngUnits);
			}

			string fuelTypeString;

			switch (this.fuelType)
			{
				case GroundActivityType.Diesel:
					fuelTypeString = "Diesel";
					break;
				case GroundActivityType.Leaded:
					fuelTypeString = "Leaded";
					break;
				case GroundActivityType.Unleaded:
					fuelTypeString = "Unleaded";
					break;
				default:
					fuelTypeString = "Non-fuel";
					break;
			}

			SqlCommand compileGroundActivity;
			compileGroundActivity = this.connection.CreateCommand();

			compileGroundActivity.CommandText = "ASC_CompileGroundActivity";
			compileGroundActivity.CommandType = CommandType.StoredProcedure;
			compileGroundActivity.Parameters.AddWithValue("@startdate", this.endDate.AddDays(-365));
			compileGroundActivity.Parameters.AddWithValue("@enddate", this.endDate);
			compileGroundActivity.Parameters.AddWithValue("@conv", conversion);
			compileGroundActivity.Parameters.AddWithValue("@class", fuelTypeString);
			compileGroundActivity.CommandTimeout = 300;
			AsyncCallback callback = new AsyncCallback(this.CompileTransactionsCallback);
			compileGroundActivity.BeginExecuteNonQuery(callback, compileGroundActivity);
			//compileGroundActivity.ExecuteNonQuery();
		}

		/// <summary>
		/// Callback point for BeginExecuteNonQuery call in CompileTransactions.
		/// Uses invoke to perform further actions as this function is running on a thread other
		/// than the main UI.
		/// </summary>
		/// <param name="asyncResult"></param>
		private void CompileTransactionsCallback(IAsyncResult asyncResult)
		{
			try
			{
				SqlCommand compileGroundActivity = (SqlCommand)asyncResult.AsyncState;
				compileGroundActivity.EndExecuteNonQuery(asyncResult);

				this.Invoke((MethodInvoker)delegate() { this.CompiledTransactions(); });
			}
			catch (Exception ex)
			{
				this.Invoke((MethodInvoker)delegate() { this.CompiledTransactionsFailed(ex.ToString()); });
			}
		}

		/// <summary>
		/// Called after transactions have been grouped.  Sets up grids and enables all controls.
		/// </summary>
		private void CompiledTransactions()
		{
			string fuelTypeString;

			switch (this.fuelType)
			{
				case GroundActivityType.Diesel:
					fuelTypeString = "Diesel";
					break;
				case GroundActivityType.Leaded:
					fuelTypeString = "Leaded";
					break;
				case GroundActivityType.Unleaded:
					fuelTypeString = "Unleaded";
					break;
				default:
					fuelTypeString = "Non-fuel";
					break;
			}

			this.cmdGetGroundPeaks = this.connection.CreateCommand();
			this.cmdGetGroundPeaks.CommandText = "ASC_GROUND_CALCPEAKS";
			this.cmdGetGroundPeaks.CommandType = CommandType.StoredProcedure;
			this.cmdGetGroundPeaks.Parameters.AddWithValue("@class", fuelTypeString);
			this.adptGetGroundPeaks = new SqlDataAdapter(this.cmdGetGroundPeaks);
			this.dsGetGroundPeaks = new DataSet();
			this.adptGetGroundPeaks.Fill(this.dsGetGroundPeaks);

			this.peakDaysGrid.AutoGenerateColumns = false;
			this.peakDaysGrid.DataSource = this.dsGetGroundPeaks.Tables[0];
			this.peakDaysGridDate.DataPropertyName = "REQUEST_DATE";
			this.peakDaysGridQuantity.DataPropertyName = "totalquantity";

			this.PopulatePeakDaysGrid();

			this.buttonCancel.Enabled = true;
			this.buttonAccept.Enabled = true;

			this.pleaseWait.Visible = false;

			this.labelExcludedDays.Visible = true;
			this.manualEntry.Visible = true;
			this.quantityMax.Visible = true;
			this.labelMaxOneDay.Visible = true;
			this.excludedDaysGrid.Visible = true;
			this.labelPeakDays.Visible = true;
			this.peakDaysGrid.Visible = true;

			this.isProcessing = false;
		}

		/// <summary>
		///  Called after error on asynchronous query thread.  Displays error message in message box, then
		/// enables only cancel button and close dialog button.
		/// </summary>
		/// <param name="message">Error message to display</param>
		private void CompiledTransactionsFailed(string message)
		{
			MessageBox.Show(message);
			this.buttonCancel.Enabled = true;
			this.isProcessing = false;
		}

		/// <summary>
		/// Commits all changes.  Called when dialog was closed by OK button
		/// </summary>
		private void ClosedOK()
		{
			string fuelTypeString;

			switch (this.fuelType)
			{
				case GroundActivityType.Diesel:
					fuelTypeString = "Diesel";
					break;
				case GroundActivityType.Leaded:
					fuelTypeString = "Leaded";
					break;
				case GroundActivityType.Unleaded:
					fuelTypeString = "Unleaded";
					break;
				default:
					fuelTypeString = "Non-fuel";
					break;
			}

			// Save 6 peak days defuels
			SqlCommand cmdGroundPeakDays = this.connection.CreateCommand();
			cmdGroundPeakDays.CommandText = "DELETE FROM ##ASC_FUELING_PEAK_DAYS WHERE FuelingType = @FuelingType AND SpecialFuel = @Class";
			cmdGroundPeakDays.CommandType = CommandType.Text;
			cmdGroundPeakDays.Parameters.AddWithValue("@FuelingType", "Ground");
			cmdGroundPeakDays.Parameters.AddWithValue("@Class", fuelTypeString);
			cmdGroundPeakDays.ExecuteNonQuery();

			cmdGroundPeakDays.CommandText = "INSERT INTO ##ASC_FUELING_PEAK_DAYS "
													+ "(FuelingType, SpecialFuel, PeakDay, Total, "
													+ "Refuel400, Refuel1800, Refuel2700, Refuel3500, Refuel3501) "
													+ "VALUES "
													+ "(@FuelingType, @Class, @PeakDay, @Quantity, "
													+ "@Refuel400, @Refuel1800, @Refuel2700, @Refuel3500, @Refuel3501)";
			cmdGroundPeakDays.Parameters.AddWithValue("@DeleteFlag", false);
			cmdGroundPeakDays.Parameters.Add("@PeakDay", SqlDbType.DateTime);
			cmdGroundPeakDays.Parameters.Add("@Quantity", SqlDbType.Int);
			cmdGroundPeakDays.Parameters.AddWithValue("@Refuel400", 0);
			cmdGroundPeakDays.Parameters.AddWithValue("@Refuel1800", 0);
			cmdGroundPeakDays.Parameters.AddWithValue("@Refuel2700", 0);
			cmdGroundPeakDays.Parameters.AddWithValue("@Refuel3500", 0);
			cmdGroundPeakDays.Parameters.AddWithValue("@Refuel3501", 0);
			foreach (DataGridViewRow currentRow in this.peakDaysGrid.Rows)
			{
				cmdGroundPeakDays.Parameters["@PeakDay"].Value = currentRow.Cells[0].Value;
				cmdGroundPeakDays.Parameters["@Quantity"].Value = currentRow.Cells[1].Value;
				cmdGroundPeakDays.ExecuteNonQuery();
			}

			SqlCommand cmdDropShadow = this.connection.CreateCommand();
			cmdDropShadow.CommandText = Strings.DropGroundExcludedDaysShadowTable;
			cmdDropShadow.ExecuteNonQuery();
		}

		/// <summary>
		/// Backs out all changes.  Called when the dialog is closed by any means other than the OK button
		/// </summary>
		private void ClosedCancel()
		{
			string fuelTypeString;

			switch (this.fuelType)
			{
				case GroundActivityType.Diesel:
					fuelTypeString = "Diesel";
					break;
				case GroundActivityType.Leaded:
					fuelTypeString = "Leaded";
					break;
				case GroundActivityType.Unleaded:
					fuelTypeString = "Unleaded";
					break;
				default:
					fuelTypeString = "Non-fuel";
					break;
			}

			// Save 6 peak days defuels
			SqlCommand cmdGroundRestoreDays = this.connection.CreateCommand();
			cmdGroundRestoreDays.CommandText = "DELETE FROM #ASC_GROUND_EXCLUDED_DAYS WHERE FuelType = @Class";
			cmdGroundRestoreDays.Parameters.AddWithValue("@Class", fuelTypeString);
			cmdGroundRestoreDays.ExecuteNonQuery();

			cmdGroundRestoreDays.CommandText = "INSERT INTO #ASC_GROUND_EXCLUDED_DAYS (FuelType, ExcludedDay) "
														+ "SELECT FuelType, ExcludedDay FROM #ASC_GROUND_EXCLUDED_DAYS_SHADOW";
			cmdGroundRestoreDays.ExecuteNonQuery();

			cmdGroundRestoreDays.CommandText = Strings.DropGroundExcludedDaysShadowTable;
			cmdGroundRestoreDays.ExecuteNonQuery();
		}
		#endregion

		#region Private Member Variables
		float maxQuantity;
		SqlConnection connection;
		SqlCommand cmdGetGroundPeaks;
		SqlCommand cmdGetGroundExcluded;
		SqlDataAdapter adptGetGroundPeaks;
		SqlDataAdapter adptGetGroundExcluded;
		DataSet dsGetGroundPeaks;
		DataSet dsGetGroundExcluded;
		GroundActivityType fuelType;
		Guid siteGuid;
		string userName;
		DateTime endDate;
		bool isProcessing;
		#endregion
	}
}