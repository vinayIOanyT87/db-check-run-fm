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

namespace ASCReporter
{
	/// <summary>
	/// Class for handling the Refueler Activity dialog.  This class handles all three
	/// tabs (partial classes in each of multiple files)
	/// </summary>
	public partial class RefuelerActivity : Form
	{
		/// <summary>
		/// Enumeration identifying the type of fuels to be queried on
		/// </summary>
		public enum RefuelerActivityType
		{
			/// <summary>
			/// Indicates fuels without special additives, such as stock JP-4 or JP-8
			/// </summary>
			StandardFuels,
			/// <summary>
			/// Indicates fuels with special additives such as +100 (i.e. JP-8 +100)
			/// </summary>
			SpecialFuels
		}

		#region Constructor
		/// <summary>
		/// Constructor for the RefuelerActivity dialog
		/// </summary>
		/// <param name="refuelingType">one of the <see cref="RefuelerActivityType">RefuelerActivityType</see> enumeration
		/// identitying the type of fuels to include for calculation</param>
		/// <param name="eDate">End date of the year (365 consecutive days) to compile transactions for</param>
		/// <param name="conn">A configured SQL Server connection.  This connection needs to be open and have already created
		/// the #RefuelActivity and #DefuelActivity temporary tables</param>
		/// <param name="sGuid">Site Guid from current security context - expected to be 1</param>
		/// <param name="uName">user name/id from current security context - used for auditability</param>
		public RefuelerActivity(RefuelerActivityType refuelingType, DateTime eDate, SqlConnection conn, Guid sGuid, string uName)
		{
			InitializeComponent();
			Cursor.Current = Cursors.WaitCursor;
			this.connection = conn;
			this.fuelType = refuelingType;
			this.siteGuid = sGuid;
			this.userName = uName;
			this.endDate = eDate;

			stringResources = new ResourceManager("ASCReporter.Strings", Assembly.GetCallingAssembly());

			Cursor.Current = Cursors.Default;
		}
		#endregion

		#region Event handlers
		/// <summary>
		/// Event handler for Load event.  Sets the isProcessing flag, which is used to prevent 
		/// closure of this dialog until all asynchronous query activity is complete
		/// </summary>
		/// <param name="sender">unused</param>
		/// <param name="e">unused</param>
		private void RefuelerActivity_Load(object sender, EventArgs e)
		{
			this.isProcessing = true;
			this.CompileTransactions();
		}

		/// <summary>
		/// Event handler to highlight all text in a masked edit box when it receives focus
		/// Must be attached to the Enter event of all masked edits which will use it.
		/// </summary>
		/// <param name="sender">Masked text box to select text in.</param>
		/// <param name="e">unused</param>
		private void MaskField_Enter(object sender, EventArgs e)
		{
			if (sender is MaskedTextBox)
			{
				MaskedTextBox sendingBox = (MaskedTextBox)sender;
				this.BeginInvoke((MethodInvoker)delegate() { sendingBox.Select(0, sendingBox.Text.Length); });
			}
		}

		/// <summary>
		/// Event handler to cancel closing of the dialog while an asynchronous query is running.
		/// Depends on properly setting the isProcessing flag
		/// </summary>
		/// <param name="sender">unused</param>
		/// <param name="e">event arguments.  Used to cancel close.</param>
		private void RefuelerActivity_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.isProcessing)
			{
				e.Cancel = true;
				MessageBox.Show(Strings.WaitCloseDatabaseAccess);
			}
		}

		/// <summary>
		/// Performs final data commit or rollback based on exit reason of dialog
		/// </summary>
		/// <param name="sender">unused</param>
		/// <param name="e">unused</param>
		private void RefuelerActivity_FormClosed(object sender, FormClosedEventArgs e)
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
		#endregion

		#region Private member variables
		SqlConnection			connection;
		ResourceManager		stringResources;
		RefuelerActivityType	fuelType;
		Guid					siteGuid;
		string					userName;
		DateTime					endDate;
		bool						isProcessing;
		#endregion

		#region Private Properties
		private int PeacetimeVATA
		{
			get
			{
				int ret;
				int.TryParse(this.peacetimeVATA.Text, out ret);
				return ret;
			}
			set
			{
				this.peacetimeVATA.Text = value.ToString();
			}
		}

		private int PeacetimeVATB
		{
			get
			{
				int ret;
				int.TryParse(this.peacetimeVATB.Text, out ret);
				return ret;
			}
			set
			{
				this.peacetimeVATB.Text = value.ToString();
			}
		}

		private int PeacetimeVATL
		{
			get
			{
				int ret;
				int.TryParse(this.peacetimeVATL.Text, out ret);
				return ret;
			}
			set
			{
				this.peacetimeVATL.Text = value.ToString();
			}
		}

		private int PeacetimeVATM
		{
			get
			{
				int ret;
				int.TryParse(this.peacetimeVATM.Text, out ret);
				return ret;
			}
			set
			{
				this.peacetimeVATM.Text = value.ToString();
			}
		}

		private int PeacetimeVATN
		{
			get
			{
				int ret;
				int.TryParse(this.peacetimeVATN.Text, out ret);
				return ret;
			}
			set
			{
				this.peacetimeVATN.Text = value.ToString();
			}
		}

		private int PeacetimeVATP
		{
			get
			{
				int ret;
				int.TryParse(this.peacetimeVATP.Text, out ret);
				return ret;
			}
			set
			{
				this.peacetimeVATP.Text = value.ToString();
			}
		}

		private int PeacetimeVATQ
		{
			get
			{
				int ret;
				int.TryParse(this.peacetimeVATQ.Text, out ret);
				return ret;
			}
			set
			{
				this.peacetimeVATQ.Text = value.ToString();
			}
		}

		private int PeacetimeVATX
		{
			get
			{
				int ret;
				int.TryParse(this.peacetimeVATX.Text, out ret);
				return ret;
			}
			set
			{
				this.peacetimeVATX.Text = value.ToString();
			}
		}

		private int PeacetimeOther
		{
			get
			{
				int ret;
				int.TryParse(this.peacetimeOther.Text, out ret);
				return ret;
			}
			set
			{
				this.peacetimeOther.Text = value.ToString();
			}
		}

		private int WartimeVATA
		{
			get
			{
				int ret;
				int.TryParse(this.wartimeVATA.Text, out ret);
				return ret;
			}
			set
			{
				this.wartimeVATA.Text = value.ToString();
			}
		}

		private int WartimeVATB
		{
			get
			{
				int ret;
				int.TryParse(this.wartimeVATB.Text, out ret);
				return ret;
			}
			set
			{
				this.wartimeVATB.Text = value.ToString();
			}
		}

		private int WartimeVATL
		{
			get
			{
				int ret;
				int.TryParse(this.wartimeVATL.Text, out ret);
				return ret;
			}
			set
			{
				this.wartimeVATL.Text = value.ToString();
			}
		}

		private int WartimeVATM
		{
			get
			{
				int ret;
				int.TryParse(this.wartimeVATM.Text, out ret);
				return ret;
			}
			set
			{
				this.wartimeVATM.Text = value.ToString();
			}
		}

		private int WartimeVATN
		{
			get
			{
				int ret;
				int.TryParse(this.wartimeVATN.Text, out ret);
				return ret;
			}
			set
			{
				this.wartimeVATN.Text = value.ToString();
			}
		}

		private int WartimeVATP
		{
			get
			{
				int ret;
				int.TryParse(this.wartimeVATP.Text, out ret);
				return ret;
			}
			set
			{
				this.wartimeVATP.Text = value.ToString();
			}
		}

		private int WartimeVATQ
		{
			get
			{
				int ret;
				int.TryParse(this.wartimeVATQ.Text, out ret);
				return ret;
			}
			set
			{
				this.wartimeVATQ.Text = value.ToString();
			}
		}

		private int WartimeVATX
		{
			get
			{
				int ret;
				int.TryParse(this.wartimeVATX.Text, out ret);
				return ret;
			}
			set
			{
				this.wartimeVATX.Text = value.ToString();
			}
		}

		private int WartimeOther
		{
			get
			{
				int ret;
				int.TryParse(this.wartimeOther.Text, out ret);
				return ret;
			}
			set
			{
				this.wartimeOther.Text = value.ToString();
			}
		}
		#endregion

		#region Private Member Functions
		/// <summary>
		/// Actions to perform when dialog is closed by OK button.
		/// Commits all data changes.
		/// </summary>
		private void ClosedOK()
		{
			// Save off everything on the way out.

			// First, delete any current record for this type in the authorization table.
			SqlCommand saveRefuelerAuthorization = this.connection.CreateCommand();
			saveRefuelerAuthorization.CommandText = "DELETE FROM ##ASC_AVIATION_REFUELER_AUTHORIZATION WHERE "
																		+ "SpecialFuel = @SpecialFuel";
			saveRefuelerAuthorization.Parameters.AddWithValue("@SpecialFuel",
				(this.fuelType == RefuelerActivityType.SpecialFuels) ? "Special" : "Standard");
			saveRefuelerAuthorization.ExecuteNonQuery();

			saveRefuelerAuthorization.CommandText = "INSERT INTO ##ASC_AVIATION_REFUELER_AUTHORIZATION "
																+ "(SpecialFuel, PeacetimeVATC400, PeacetimeVATC1800, PeacetimeVATC2700, "
																+ "PeacetimeVATC3500, PeacetimeVATC3501, PeacetimeVATA, PeacetimeVATB, "
																+ "PeacetimeVATH, PeacetimeVATJ, PeacetimeVATK, PeacetimeVATL, PeacetimeVATM, "
																+ "PeacetimeVATN, PeacetimeVATP, PeacetimeVATQ, PeacetimeVATX, PeacetimeOther, "
																+ "WartimeVATC400, WartimeVATC1800, WartimeVATC2700, WartimeVATC3500, "
																+ "WartimeVATC3501, WartimeHydrantUsage, WartimeVATA, WartimeVATB, "
																+ "WartimeVATH, WartimeVATJ, WartimeVATK, WartimeVATL, WartimeVATM, "
																+ "WartimeVATN, WartimeVATP, WartimeVATQ, WartimeVATX, WartimeOther) "
																+ "VALUES "
																+ "(@SpecialFuel, @PeacetimeVATC400, @PeacetimeVATC1800, @PeacetimeVATC2700, "
																+ "@PeacetimeVATC3500, @PeacetimeVATC3501, @PeacetimeVATA, @PeacetimeVATB, "
																+ "@PeacetimeVATH, @PeacetimeVATJ, @PeacetimeVATK, @PeacetimeVATL, @PeacetimeVATM, "
																+ "@PeacetimeVATN, @PeacetimeVATP, @PeacetimeVATQ, @PeacetimeVATX, @PeacetimeOther, "
																+ "@WartimeVATC400, @WartimeVATC1800, @WartimeVATC2700, @WartimeVATC3500, "
																+ "@WartimeVATC3501, @WartimeHydrantUsage, @WartimeVATA, @WartimeVATB, "
																+ "@WartimeVATH, @WartimeVATJ, @WartimeVATK, @WartimeVATL, @WartimeVATM, "
																+ "@WartimeVATN, @WartimeVATP, @WartimeVATQ, @WartimeVATX, @WartimeOther) ";

			// Write peace time refueling-based authorizations
			saveRefuelerAuthorization.Parameters.AddWithValue("@PeacetimeVATC400", this.PeacetimeVATC400);
			saveRefuelerAuthorization.Parameters.AddWithValue("@PeacetimeVATC1800", this.PeacetimeVATC1800);
			saveRefuelerAuthorization.Parameters.AddWithValue("@PeacetimeVATC2700", this.PeacetimeVATC2700);
			saveRefuelerAuthorization.Parameters.AddWithValue("@PeacetimeVATC3500", this.PeacetimeVATC3500);
			saveRefuelerAuthorization.Parameters.AddWithValue("@PeacetimeVATC3501", this.PeacetimeVATC3501);

			// Write peace time defueling-based aauthorizations
			saveRefuelerAuthorization.Parameters.AddWithValue("@PeacetimeVATH", this.PeacetimeVATH);
			saveRefuelerAuthorization.Parameters.AddWithValue("@PeacetimeVATJ", this.PeacetimeVATJ);
			saveRefuelerAuthorization.Parameters.AddWithValue("@PeacetimeVATK", this.PeacetimeVATK);

			// Write peace time other authorizations
			saveRefuelerAuthorization.Parameters.AddWithValue("@PeacetimeVATA", this.PeacetimeVATA);
			saveRefuelerAuthorization.Parameters.AddWithValue("@PeacetimeVATB", this.PeacetimeVATB);
			saveRefuelerAuthorization.Parameters.AddWithValue("@PeacetimeVATL", this.PeacetimeVATL);
			saveRefuelerAuthorization.Parameters.AddWithValue("@PeacetimeVATM", this.PeacetimeVATM);
			saveRefuelerAuthorization.Parameters.AddWithValue("@PeacetimeVATN", this.PeacetimeVATN);
			saveRefuelerAuthorization.Parameters.AddWithValue("@PeacetimeVATP", this.PeacetimeVATP);
			saveRefuelerAuthorization.Parameters.AddWithValue("@PeacetimeVATQ", this.PeacetimeVATQ);
			saveRefuelerAuthorization.Parameters.AddWithValue("@PeacetimeVATX", this.PeacetimeVATX);
			saveRefuelerAuthorization.Parameters.AddWithValue("@PeacetimeOther", this.PeacetimeOther);

			// Write war time refueling-based authorizations
			saveRefuelerAuthorization.Parameters.AddWithValue("@WartimeVATC400", this.WartimeVATC400);
			saveRefuelerAuthorization.Parameters.AddWithValue("@WartimeVATC1800", this.WartimeVATC1800);
			saveRefuelerAuthorization.Parameters.AddWithValue("@WartimeVATC2700", this.WartimeVATC2700);
			saveRefuelerAuthorization.Parameters.AddWithValue("@WartimeVATC3500", this.WartimeVATC3500);
			saveRefuelerAuthorization.Parameters.AddWithValue("@WartimeVATC3501", this.WartimeVATC3501);
			saveRefuelerAuthorization.Parameters.AddWithValue("@WartimeHydrantUsage", this.WartimeHydrantUsage);

			// Write war time defueling-based aauthorizations
			saveRefuelerAuthorization.Parameters.AddWithValue("@WartimeVATH", this.WartimeVATH);
			saveRefuelerAuthorization.Parameters.AddWithValue("@WartimeVATJ", this.WartimeVATJ);
			saveRefuelerAuthorization.Parameters.AddWithValue("@WartimeVATK", this.WartimeVATK);

			// Write war time other authorizations
			saveRefuelerAuthorization.Parameters.AddWithValue("@WartimeVATA", this.WartimeVATA);
			saveRefuelerAuthorization.Parameters.AddWithValue("@WartimeVATB", this.WartimeVATB);
			saveRefuelerAuthorization.Parameters.AddWithValue("@WartimeVATL", this.WartimeVATL);
			saveRefuelerAuthorization.Parameters.AddWithValue("@WartimeVATM", this.WartimeVATM);
			saveRefuelerAuthorization.Parameters.AddWithValue("@WartimeVATN", this.WartimeVATN);
			saveRefuelerAuthorization.Parameters.AddWithValue("@WartimeVATP", this.WartimeVATP);
			saveRefuelerAuthorization.Parameters.AddWithValue("@WartimeVATQ", this.WartimeVATQ);
			saveRefuelerAuthorization.Parameters.AddWithValue("@WartimeVATX", this.WartimeVATX);
			saveRefuelerAuthorization.Parameters.AddWithValue("@WartimeOther", this.WartimeOther);

			saveRefuelerAuthorization.ExecuteNonQuery();

			// Save 6 peak days refuels
			SqlCommand cmdRefuelPeakDays = this.connection.CreateCommand();
			cmdRefuelPeakDays.CommandText = "DELETE FROM ##ASC_FUELING_PEAK_DAYS WHERE FuelingType = @FuelingType AND SpecialFuel = @SpecialFuel";
			cmdRefuelPeakDays.CommandType = CommandType.Text;
			cmdRefuelPeakDays.Parameters.AddWithValue("@FuelingType", "Refuel");
			cmdRefuelPeakDays.Parameters.AddWithValue("@SpecialFuel",
				(this.fuelType == RefuelerActivityType.SpecialFuels) ? "Special" : "Standard");
			cmdRefuelPeakDays.ExecuteNonQuery();

			cmdRefuelPeakDays.CommandText = "INSERT INTO ##ASC_FUELING_PEAK_DAYS "
													+ "(FuelingType, SpecialFuel, PeakDay, Total, Refuel400, Refuel1800, Refuel2700, Refuel3500, Refuel3501) "
													+ "VALUES "
													+ "(@FuelingType, @SpecialFuel, @PeakDay, @Total, @Refuel400, @Refuel1800, @Refuel2700, @Refuel3500, "
													+ "@Refuel3501)";
			cmdRefuelPeakDays.Parameters.AddWithValue("@Total", 0);
			cmdRefuelPeakDays.Parameters.Add("@PeakDay", SqlDbType.DateTime);
			cmdRefuelPeakDays.Parameters.Add("@Refuel400", SqlDbType.Int);
			cmdRefuelPeakDays.Parameters.Add("@Refuel1800", SqlDbType.Int);
			cmdRefuelPeakDays.Parameters.Add("@Refuel2700", SqlDbType.Int);
			cmdRefuelPeakDays.Parameters.Add("@Refuel3500", SqlDbType.Int);
			cmdRefuelPeakDays.Parameters.Add("@Refuel3501", SqlDbType.Int);
			foreach (DataGridViewRow currentRow in this.peakRefuelsGrid.Rows)
			{
				cmdRefuelPeakDays.Parameters["@PeakDay"].Value = currentRow.Cells[0].Value;
				cmdRefuelPeakDays.Parameters["@Refuel400"].Value = currentRow.Cells[1].Value;
				cmdRefuelPeakDays.Parameters["@Refuel1800"].Value = currentRow.Cells[2].Value;
				cmdRefuelPeakDays.Parameters["@Refuel2700"].Value = currentRow.Cells[3].Value;
				cmdRefuelPeakDays.Parameters["@Refuel3500"].Value = currentRow.Cells[4].Value;
				cmdRefuelPeakDays.Parameters["@Refuel3501"].Value = currentRow.Cells[5].Value;
				cmdRefuelPeakDays.ExecuteNonQuery();
			}

			// Save 6 peak days defuels
			SqlCommand cmdDefuelPeakDays = this.connection.CreateCommand();
			cmdDefuelPeakDays.CommandText = "DELETE FROM ##ASC_FUELING_PEAK_DAYS WHERE FuelingType = @FuelingType AND SpecialFuel = @SpecialFuel";
			cmdDefuelPeakDays.CommandType = CommandType.Text;
			cmdDefuelPeakDays.Parameters.AddWithValue("@FuelingType", "Defuel");
			cmdDefuelPeakDays.Parameters.AddWithValue("@SpecialFuel",
				(this.fuelType == RefuelerActivityType.SpecialFuels) ? "Special" : "Standard");
			cmdDefuelPeakDays.ExecuteNonQuery();

			cmdDefuelPeakDays.CommandText = "INSERT INTO ##ASC_FUELING_PEAK_DAYS "
													+ "(FuelingType, SpecialFuel, PeakDay, Total, "
													+ "Refuel400, Refuel1800, Refuel2700, Refuel3500, Refuel3501) "
													+ "VALUES "
													+ "(@FuelingType, @SpecialFuel, @PeakDay, @Defuels, "
													+ "@Refuel400, @Refuel1800, @Refuel2700, @Refuel3500, @Refuel3501)";
			cmdDefuelPeakDays.Parameters.Add("@PeakDay", SqlDbType.DateTime);
			cmdDefuelPeakDays.Parameters.Add("@Defuels", SqlDbType.Int);
			cmdDefuelPeakDays.Parameters.AddWithValue("@Refuel400", 0);
			cmdDefuelPeakDays.Parameters.AddWithValue("@Refuel1800", 0);
			cmdDefuelPeakDays.Parameters.AddWithValue("@Refuel2700", 0);
			cmdDefuelPeakDays.Parameters.AddWithValue("@Refuel3500", 0);
			cmdDefuelPeakDays.Parameters.AddWithValue("@Refuel3501", 0);
			foreach (DataGridViewRow currentRow in this.peakDefuelsGrid.Rows)
			{
				cmdDefuelPeakDays.Parameters["@PeakDay"].Value = currentRow.Cells[0].Value;
				cmdDefuelPeakDays.Parameters["@Defuels"].Value = currentRow.Cells[1].Value;
				cmdDefuelPeakDays.ExecuteNonQuery();
			}

			SqlCommand cmdDropShadows = this.connection.CreateCommand();
			cmdDropShadows.CommandText = Strings.DropDefuelExcludedDaysShadowTable;
			cmdDropShadows.ExecuteNonQuery();
			cmdDropShadows.CommandText = Strings.DropRefuelExcludedDaysShadowTable;
			cmdDropShadows.ExecuteNonQuery();
		}

		/// <summary>
		/// Actions to perform when dialog is closed by any means other than the OK button.
		/// Discards all changes, which includes restoring excluded day tables to initial state.
		/// </summary>
		private void ClosedCancel()
		{
			SqlCommand cmdRestoreDays = this.connection.CreateCommand();
			cmdRestoreDays.CommandText = "DELETE FROM #ASC_REFUEL_EXCLUDED_DAYS WHERE SpecialFuel = @SpecialFuel";
			cmdRestoreDays.Parameters.AddWithValue("@SpecialFuel",
				(this.fuelType == RefuelerActivityType.SpecialFuels) ? "Special" : "Standard");
			cmdRestoreDays.ExecuteNonQuery();

			cmdRestoreDays.CommandText = "INSERT INTO #ASC_REFUEL_EXCLUDED_DAYS (SpecialFuel, ExcludedDay) "
														+ "SELECT SpecialFuel, ExcludedDay FROM #ASC_REFUEL_EXCLUDED_DAYS_SHADOW";
			cmdRestoreDays.ExecuteNonQuery();

			cmdRestoreDays.CommandText = Strings.DropRefuelExcludedDaysShadowTable;
			cmdRestoreDays.ExecuteNonQuery();

			cmdRestoreDays.CommandText = "DELETE FROM #ASC_DEFUEL_EXCLUDED_DAYS WHERE SpecialFuel = @SpecialFuel";
			cmdRestoreDays.ExecuteNonQuery();

			cmdRestoreDays.CommandText = "INSERT INTO #ASC_DEFUEL_EXCLUDED_DAYS (SpecialFuel, ExcludedDay) "
														+ "SELECT SpecialFuel, ExcludedDay FROM #ASC_DEFUEL_EXCLUDED_DAYS_SHADOW";
			cmdRestoreDays.ExecuteNonQuery();

			cmdRestoreDays.CommandText = Strings.DropDefuelExcludedDaysShadowTable;
			cmdRestoreDays.ExecuteNonQuery();
		}

		
		/// <summary>
		/// Reads and groups Refuel and Defuel transactions from CONTROL_LOG table.
		/// Performed asynchronously, as this may take a long time (~30 seconds)
		/// </summary>
		void CompileTransactions()
		{
			SqlCommand compileRefuelActivity = this.connection.CreateCommand();
			compileRefuelActivity.CommandText = "EXEC ASC_CompileRefuelActivity @startdate, @enddate, @specialfuel; "
																+ "EXEC ASC_CompileDefuelActivity @startdate, @enddate, @specialfuel";
			compileRefuelActivity.CommandType = CommandType.Text;
			compileRefuelActivity.Parameters.AddWithValue("@startdate", this.endDate.AddDays(-365));
			compileRefuelActivity.Parameters.AddWithValue("@enddate", this.endDate);
			compileRefuelActivity.Parameters.AddWithValue("@specialfuel",
				(this.fuelType == RefuelerActivityType.SpecialFuels) ? "Special" : "Standard");
			compileRefuelActivity.CommandTimeout = 300;

			AsyncCallback callback = new AsyncCallback(this.CompileTransactionsCallback);
			compileRefuelActivity.BeginExecuteNonQuery(callback, compileRefuelActivity);

		}

		/// <summary>
		/// Enables all controls after the long running query has completed.  Also
		/// populates the grids.
		/// </summary>
		void CompiledTransactions()
		{
			this.RefuelSheet_Init();
			this.DefuelPeakDays_Init();

			this.buttonCancel.Enabled = true;
			this.buttonAccept.Enabled = true;
			this.refuelerActivityTabs.Visible = true;
			this.pleaseWait.Visible = false;

			SqlCommand loadRefuelerAuthorization = this.connection.CreateCommand();
			loadRefuelerAuthorization.CommandText = "SELECT * FROM ##ASC_AVIATION_REFUELER_AUTHORIZATION WHERE "
																	+ "SpecialFuel = @SpecialFuel";
			loadRefuelerAuthorization.Parameters.AddWithValue("@SpecialFuel",
				(this.fuelType == RefuelerActivityType.SpecialFuels) ? "Special" : "Standard");

			SqlDataAdapter loadAdapter = new SqlDataAdapter(loadRefuelerAuthorization);
			DataSet loadSet = new DataSet();

			loadAdapter.Fill(loadSet);

			// Should only be 0 or 1 rows.  If we have more than one row, there is a problem, and treat as zero
			if (loadSet.Tables[0].Rows.Count == 1)
			{
				DataRow loadRow = loadSet.Tables[0].Rows[0];

				this.PeacetimeOther = (int)loadRow["PeacetimeOther"];
				this.PeacetimeVATA = (int)loadRow["PeacetimeVATA"];
				this.PeacetimeVATB = (int)loadRow["PeacetimeVATB"];
				this.PeacetimeVATL = (int)loadRow["PeacetimeVATL"];
				this.PeacetimeVATM = (int)loadRow["PeacetimeVATM"];
				this.PeacetimeVATN = (int)loadRow["PeacetimeVATN"];
				this.PeacetimeVATP = (int)loadRow["PeacetimeVATP"];
				this.PeacetimeVATQ = (int)loadRow["PeacetimeVATQ"];
				this.PeacetimeVATX = (int)loadRow["PeacetimeVATX"];
				this.WartimeOther = (int)loadRow["WartimeOther"];
				this.WartimeVATA = (int)loadRow["WartimeVATA"];
				this.WartimeVATB = (int)loadRow["WartimeVATB"];
				this.WartimeVATC400 = (int)loadRow["WartimeVATC400"];
				this.WartimeVATC1800 = (int)loadRow["WartimeVATC1800"];
				this.WartimeVATC2700 = (int)loadRow["WartimeVATC2700"];
				this.WartimeVATC3500 = (int)loadRow["WartimeVATC3500"];
				this.WartimeVATC3501 = (int)loadRow["WartimeVATC3501"];
				this.WartimeHydrantUsage = (int)((double)loadRow["WartimeHydrantUsage"]);
				this.WartimeVATH = (int)loadRow["WartimeVATH"];
				this.WartimeVATJ = (int)loadRow["WartimeVATJ"];
				this.WartimeVATK = (int)loadRow["WartimeVATK"];
				this.WartimeVATL = (int)loadRow["WartimeVATL"];
				this.WartimeVATM = (int)loadRow["WartimeVATM"];
				this.WartimeVATN = (int)loadRow["WartimeVATN"];
				this.WartimeVATP = (int)loadRow["WartimeVATP"];
				this.WartimeVATQ = (int)loadRow["WartimeVATQ"];
				this.WartimeVATX = (int)loadRow["WartimeVATX"];
			}
			else
			{
				this.PeacetimeOther = 0;
				this.PeacetimeVATA = 0;
				this.PeacetimeVATB = 0;
				this.PeacetimeVATL = 0;
				this.PeacetimeVATM = 0;
				this.PeacetimeVATN = 0;
				this.PeacetimeVATP = 0;
				this.PeacetimeVATQ = 0;
				this.PeacetimeVATX = 0;
				this.WartimeOther = 0;
				this.WartimeVATA = 0;
				this.WartimeVATB = 0;
				this.WartimeVATC400 = 0;
				this.WartimeVATC1800 = 0;
				this.WartimeVATC2700 = 0;
				this.WartimeVATC3500 = 0;
				this.WartimeVATC3501 = 0;
				this.WartimeHydrantUsage = 0;
				this.WartimeVATH = 0;
				this.WartimeVATJ = 0;
				this.WartimeVATK = 0;
				this.WartimeVATL = 0;
				this.WartimeVATM = 0;
				this.WartimeVATN = 0;
				this.WartimeVATP = 0;
				this.WartimeVATQ = 0;
				this.WartimeVATX = 0;
			}

			this.isProcessing = false;
		}

		/// <summary>
		/// Called if the long-running main query fails.  Only enables cancel button.
		/// </summary>
		/// <param name="message">Error message to be displayed before enabling cancel button.</param>
		void CompiledTransactionsFailed(string message)
		{
			MessageBox.Show(message);
			this.buttonCancel.Enabled = true;
			this.isProcessing = false;
		}

		/// <summary>
		/// Callback for BeginExecuteNonQuery call in <see cref="CompileTransactions">CompileTransactions</see>.
		/// Calls success and failure handlers by invoking on UI thread.
		/// </summary>
		/// <param name="asyncResult">State objectcreated by BeginExecuteNonQuery</param>
		private void CompileTransactionsCallback(IAsyncResult asyncResult)
		{
			try
			{
				SqlCommand compileRefuelActivity = (SqlCommand)asyncResult.AsyncState;
				compileRefuelActivity.EndExecuteNonQuery(asyncResult);
				this.Invoke((MethodInvoker)delegate() {CompiledTransactions();});
			}
			catch (Exception ex)
			{
				this.Invoke((MethodInvoker)delegate() { CompiledTransactionsFailed(ex.ToString()); });
			}
		}
		#endregion
	}
}