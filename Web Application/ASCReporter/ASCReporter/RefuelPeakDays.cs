using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ASCReporter
{
	public partial class RefuelerActivity : Form
	{
		#region Private member functions
		private void RefuelSheet_Init()
		{
			avgRuns400 = 0.0F;
			avgRuns1800 = 0.0F;
			avgRuns2700 = 0.0F;
			avgRuns3500 = 0.0F;
			avgRuns3501 = 0.0F;


			this.cmdGetRefuelPeaks = this.connection.CreateCommand();
			this.cmdGetRefuelPeaks.CommandText = "ASC_REFUEL_CALCPEAKS";
			this.cmdGetRefuelPeaks.CommandType = CommandType.StoredProcedure;
			this.cmdGetRefuelPeaks.Parameters.AddWithValue("@specialfuel",
				(this.fuelType == RefuelerActivityType.SpecialFuels) ? "Special" : "Standard");
			this.adptGetRefuelPeaks = new SqlDataAdapter(this.cmdGetRefuelPeaks);
			this.dsGetRefuelPeaks = new DataSet();
			this.adptGetRefuelPeaks.Fill(this.dsGetRefuelPeaks);

			this.peakRefuelsGrid.AutoGenerateColumns = false;
			this.peakRefuelsGrid.DataSource = this.dsGetRefuelPeaks.Tables[0];
			this.peakRefuelsGridDate.DataPropertyName = "REQUEST_DATE";
			this.peakRefuelsGrid400.DataPropertyName = "run400count";
			this.peakRefuelsGrid1800.DataPropertyName = "run1800count";
			this.peakRefuelsGrid2700.DataPropertyName = "run2700count";
			this.peakRefuelsGrid3500.DataPropertyName = "run3500count";
			this.peakRefuelsGrid3501.DataPropertyName = "run3501count";

			peakRefuelsGrid.CellContentClick += new DataGridViewCellEventHandler(peakRefuelsGrid_CellContentClick);
			excludedRefuelDaysGrid.CellContentClick += new DataGridViewCellEventHandler(excludedRefuelDaysGrid_CellContentClick);

			SqlCommand cmdShadowExcluded = this.connection.CreateCommand();
			cmdShadowExcluded.CommandText = Strings.CreateRefuelExcludedDaysShadowTable;
			cmdShadowExcluded.ExecuteNonQuery();

			cmdShadowExcluded.CommandText = "INSERT INTO #ASC_REFUEL_EXCLUDED_DAYS_SHADOW SELECT * FROM #ASC_REFUEL_EXCLUDED_DAYS WHERE SpecialFuel = @SpecialFuel";
			cmdShadowExcluded.Parameters.AddWithValue("@SpecialFuel",
				(this.fuelType == RefuelerActivityType.SpecialFuels) ? "Special" : "Standard");
			cmdShadowExcluded.ExecuteNonQuery();

			this.cmdGetRefuelExcluded = this.connection.CreateCommand();
			this.cmdGetRefuelExcluded.CommandText = "SELECT ndx, SpecialFuel, ExcludedDay "
															+ "FROM #ASC_REFUEL_EXCLUDED_DAYS WHERE SpecialFuel = @SpecialFuel";
			this.cmdGetRefuelExcluded.CommandType = CommandType.Text;
			this.cmdGetRefuelExcluded.Parameters.AddWithValue("@SpecialFuel",
				(this.fuelType == RefuelerActivityType.SpecialFuels) ? "Special" : "Standard");
			this.adptGetRefuelExcluded = new SqlDataAdapter(cmdGetRefuelExcluded);

			this.adptGetRefuelExcluded.InsertCommand = new SqlCommand("INSERT INTO #ASC_REFUEL_EXCLUDED_DAYS "
																							+ "(SpecialFuel, ExcludedDay) "
																							+ "VALUES (@SpecialFuel, @ExcludedDay)",
																							this.connection);
			this.adptGetRefuelExcluded.InsertCommand.Parameters.AddWithValue("@SpecialFuel",
				(this.fuelType == RefuelerActivityType.SpecialFuels) ? "Special" : "Standard");
			this.adptGetRefuelExcluded.InsertCommand.Parameters.Add("@ExcludedDay", SqlDbType.DateTime, 8, "ExcludedDay");

			this.adptGetRefuelExcluded.UpdateCommand = new SqlCommand( "UPDATE #ASC_REFUEL_EXCLUDED_DAYS "
																							+ "SET ExcludedDay = @ExcludedDay "
																							+ "WHERE ndx = @ndx",
																							this.connection);
			this.adptGetRefuelExcluded.UpdateCommand.Parameters.Add("@ExcludedDay", SqlDbType.DateTime, 8, "ExcludedDay");
			this.adptGetRefuelExcluded.UpdateCommand.Parameters.Add("@ndx", SqlDbType.Int, 4, "ndx");
			this.adptGetRefuelExcluded.UpdateCommand.Parameters["@ndx"].SourceVersion = DataRowVersion.Original;

			this.adptGetRefuelExcluded.DeleteCommand = new SqlCommand( "DELETE FROM #ASC_REFUEL_EXCLUDED_DAYS "
																						+ "WHERE ndx = @ndx",
																						this.connection);
			this.adptGetRefuelExcluded.DeleteCommand.Parameters.Add("@ndx", SqlDbType.Int, 4, "ndx");

			this.dsGetRefuelExcluded = new DataSet();
			this.adptGetRefuelExcluded.Fill(dsGetRefuelExcluded);
			DataColumn[] primaryKey = new DataColumn[1];
			primaryKey[0] = this.dsGetRefuelExcluded.Tables[0].Columns["ndx"];
			this.dsGetRefuelExcluded.Tables[0].PrimaryKey = primaryKey;
			this.dsGetRefuelExcluded.Tables[0].Columns["ndx"].AutoIncrement = true;

			this.excludedRefuelDaysGrid.AutoGenerateColumns = false;
			this.excludedRefuelDaysGrid.DataSource = this.dsGetRefuelExcluded.Tables[0];
			this.excludedRefuelDaysGridDate.DataPropertyName = "ExcludedDay";

			PopulatePeakRefuelDaysGrid();
			PopulateExcludedRefuelDaysGrid();
		}

		private void PopulatePeakRefuelDaysGrid()
		{
			avgRuns400 = 0.0F;
			avgRuns1800 = 0.0F;
			avgRuns2700 = 0.0F;
			avgRuns3500 = 0.0F;
			avgRuns3501 = 0.0F;

			this.dsGetRefuelPeaks.Clear();
			this.adptGetRefuelPeaks.Fill(this.dsGetRefuelPeaks);
			this.peakRefuelsGrid.Refresh();

			if (this.peakRefuelsGrid.Rows.Count > 0)
			{
				for (int i = 0; i < this.peakRefuelsGrid.Rows.Count; i++)
				{
					avgRuns400 += (int)this.peakRefuelsGrid.Rows[i].Cells[1].Value; // Column 1 is the 1-400 column
					avgRuns1800 += (int)this.peakRefuelsGrid.Rows[i].Cells[2].Value; // Column 2 is the 401-1800 column
					avgRuns2700 += (int)this.peakRefuelsGrid.Rows[i].Cells[3].Value; // Column 2 is the 1801-2700 column
					avgRuns3500 += (int)this.peakRefuelsGrid.Rows[i].Cells[4].Value; // Column 2 is the 401-1800 column
					avgRuns3501 += (int)this.peakRefuelsGrid.Rows[i].Cells[5].Value; // Column 2 is the 401-1800 column
				}
				avgRuns400 /= peakRefuelsGrid.Rows.Count;
				avgRuns1800 /= peakRefuelsGrid.Rows.Count;
				avgRuns2700 /= peakRefuelsGrid.Rows.Count;
				avgRuns3500 /= peakRefuelsGrid.Rows.Count;
				avgRuns3501 /= peakRefuelsGrid.Rows.Count;
			}

			peacetimeVATC400.Text = avgRuns400.ToString("##0.###");
			peacetimeVATC1800.Text = avgRuns1800.ToString("##0.###");
			peacetimeVATC2700.Text = avgRuns2700.ToString("##0.###");
			peacetimeVATC3500.Text = avgRuns3500.ToString("##0.###");
			peacetimeVATC3501.Text = avgRuns3501.ToString("##0.###");
		}

		private void PopulateExcludedRefuelDaysGrid()
		{
			this.dsGetRefuelExcluded.Clear();
			this.adptGetRefuelExcluded.Fill(this.dsGetRefuelExcluded);
			this.excludedRefuelDaysGrid.Refresh();
		}
		#endregion

		#region Event Handlers
		private void peakRefuelsGrid_CellContentClick(Object senders, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex != (peakRefuelsGrid.Columns.Count - 1))
			{
				this.peakRefuelsGrid.ClearSelection();
				return;
			}

			try
			{
				DateTime dateToExclude = (DateTime)peakRefuelsGrid.Rows[e.RowIndex].Cells[0].Value;
				DataRow	newRow;

				newRow = this.dsGetRefuelExcluded.Tables[0].NewRow();
				newRow["ExcludedDay"] = dateToExclude;
				this.dsGetRefuelExcluded.Tables[0].Rows.Add(newRow);
				this.adptGetRefuelExcluded.Update(this.dsGetRefuelExcluded);

				PopulatePeakRefuelDaysGrid();
				PopulateExcludedRefuelDaysGrid();
			}
			catch (InvalidCastException err)
			{
				err.ToString(); // Simply silence the warning
			}
		}

		private void excludedRefuelDaysGrid_CellContentClick(Object senders, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex != (excludedRefuelDaysGrid.Columns.Count - 1))
			{
				return;
			}

			this.excludedRefuelDaysGrid.Rows.RemoveAt(e.RowIndex);
			this.adptGetRefuelExcluded.Update(this.dsGetRefuelExcluded);

			PopulatePeakRefuelDaysGrid();
			PopulateExcludedRefuelDaysGrid();
		}
		#endregion

		#region Private members
		float avgRuns400;
		float avgRuns1800;
		float avgRuns2700;
		float avgRuns3500;
		float avgRuns3501;

		SqlCommand		cmdGetRefuelPeaks;
		SqlDataAdapter	adptGetRefuelPeaks;
		DataSet			dsGetRefuelPeaks;

		SqlCommand		cmdGetRefuelExcluded;
		SqlDataAdapter	adptGetRefuelExcluded;
		DataSet			dsGetRefuelExcluded;
		#endregion

		#region Private Properties
		private float PeacetimeVATC400
		{
			get
			{
				float ret;
				float.TryParse(this.peacetimeVATC400.Text, out ret);
				return ret;
			}
		}

		private float PeacetimeVATC1800
		{
			get
			{
				float ret;
				float.TryParse(this.peacetimeVATC1800.Text, out ret);
				return ret;
			}
		}

		private float PeacetimeVATC2700
		{
			get
			{
				float ret;
				float.TryParse(this.peacetimeVATC2700.Text, out ret);
				return ret;
			}
		}

		private float PeacetimeVATC3500
		{
			get
			{
				float ret;
				float.TryParse(this.peacetimeVATC3500.Text, out ret);
				return ret;
			}
		}

		private float PeacetimeVATC3501
		{
			get
			{
				float ret;
				float.TryParse(this.peacetimeVATC3501.Text, out ret);
				return ret;
			}
		}

		private int WartimeVATC400
		{
			get
			{
				int ret;
				int.TryParse(this.wartimeVATC400.Text, out ret);
				return ret;
			}
			set
			{
				this.wartimeVATC400.Text = value.ToString();
			}
		}

		private int WartimeVATC1800
		{
			get
			{
				int ret;
				int.TryParse(this.wartimeVATC1800.Text, out ret);
				return ret;
			}
			set
			{
				this.wartimeVATC1800.Text = value.ToString();
			}
		}

		private int WartimeVATC2700
		{
			get
			{
				int ret;
				int.TryParse(this.wartimeVATC2700.Text, out ret);
				return ret;
			}
			set
			{
				this.wartimeVATC2700.Text = value.ToString();
			}
		}

		private int WartimeVATC3500
		{
			get
			{
				int ret;
				int.TryParse(this.wartimeVATC3500.Text, out ret);
				return ret;
			}
			set
			{
				this.wartimeVATC3500.Text = value.ToString();
			}
		}

		private int WartimeVATC3501
		{
			get
			{
				int ret;
				int.TryParse(this.wartimeVATC3501.Text, out ret);
				return ret;
			}
			set
			{
				this.wartimeVATC3501.Text = value.ToString();
			}
		}

		private int WartimeHydrantUsage
		{
			get
			{
				int ret;
				int.TryParse(this.wartimeHydrantUsage.Text, out ret);
				return ret;
			}
			set
			{
				this.wartimeHydrantUsage.Text = value.ToString();
			}
		}
		#endregion
	}
}