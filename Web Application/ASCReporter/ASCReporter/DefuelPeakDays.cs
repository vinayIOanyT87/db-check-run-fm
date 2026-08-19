using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ASCReporter
{
	public partial class RefuelerActivity : Form
	{
		#region Private Member Functions
		private void DefuelPeakDays_Init()
		{
			avgDefuels = 0.0F;

			this.cmdGetDefuelPeaks = this.connection.CreateCommand();
			this.cmdGetDefuelPeaks.CommandText = "ASC_DEFUEL_CALCPEAKS";
			this.cmdGetDefuelPeaks.CommandType = CommandType.StoredProcedure;
			this.cmdGetDefuelPeaks.Parameters.AddWithValue("@specialfuel",
				(this.fuelType == RefuelerActivityType.SpecialFuels) ? "Special" : "Standard");
			this.adptGetDefuelPeaks = new SqlDataAdapter(this.cmdGetDefuelPeaks);
			this.dsGetDefuelPeaks = new DataSet();
			this.adptGetDefuelPeaks.Fill(this.dsGetDefuelPeaks);

			this.peakDefuelsGrid.AutoGenerateColumns = false;
			this.peakDefuelsGrid.DataSource = this.dsGetDefuelPeaks.Tables[0];
			this.peakDefuelsGridDate.DataPropertyName = "REQUEST_DATE";
			this.peakDefuelsGridRuns.DataPropertyName = "totalruncount";

			SqlCommand cmdShadowExcluded = this.connection.CreateCommand();
			cmdShadowExcluded.CommandText = Strings.CreateDefuelExcludedDaysShadowTable;
			cmdShadowExcluded.ExecuteNonQuery();

			cmdShadowExcluded.CommandText = "INSERT INTO #ASC_DEFUEL_EXCLUDED_DAYS_SHADOW SELECT * FROM #ASC_DEFUEL_EXCLUDED_DAYS WHERE SpecialFuel = @SpecialFuel";
			cmdShadowExcluded.Parameters.AddWithValue("@SpecialFuel",
				(this.fuelType == RefuelerActivityType.SpecialFuels) ? "Special" : "Standard");
			cmdShadowExcluded.ExecuteNonQuery();


			this.cmdGetDefuelExcluded = this.connection.CreateCommand();
			this.cmdGetDefuelExcluded.CommandText = "SELECT ndx, SpecialFuel, ExcludedDay "
															+ "FROM #ASC_DEFUEL_EXCLUDED_DAYS "
															+ "WHERE SpecialFuel = @SpecialFuel";
			this.cmdGetDefuelExcluded.CommandType = CommandType.Text;
			this.cmdGetDefuelExcluded.Parameters.AddWithValue("@SpecialFuel",
				(this.fuelType == RefuelerActivityType.SpecialFuels) ? "Special" : "Standard");
			this.adptGetDefuelExcluded = new SqlDataAdapter(cmdGetDefuelExcluded);

			this.adptGetDefuelExcluded.InsertCommand = new SqlCommand("INSERT INTO #ASC_DEFUEL_EXCLUDED_DAYS " 
																						+ "(SpecialFuel, ExcludedDay) "
																						+ "VALUES (@SpecialFuel, @ExcludedDay)",
																						this.connection);
			this.adptGetDefuelExcluded.InsertCommand.Parameters.AddWithValue("@SpecialFuel",
				(this.fuelType == RefuelerActivityType.SpecialFuels) ? "Special" : "Standard");
			this.adptGetDefuelExcluded.InsertCommand.Parameters.Add("@ExcludedDay", SqlDbType.DateTime, 8, "ExcludedDay");

			this.adptGetDefuelExcluded.UpdateCommand = new SqlCommand("UPDATE #ASC_DEFUEL_EXCLUDED_DAYS "
																					+ "SET ExcludedDay = @ExcludedDay "
																					+ "WHERE ndx = @ndx",
																					this.connection);
			this.adptGetDefuelExcluded.UpdateCommand.Parameters.Add("@ExcludedDay", SqlDbType.DateTime, 8, "ExcludedDay");
			this.adptGetDefuelExcluded.UpdateCommand.Parameters.Add("@ndx", SqlDbType.Int, 4, "ndx");
			this.adptGetDefuelExcluded.UpdateCommand.Parameters["@ndx"].SourceVersion = DataRowVersion.Original;

			this.adptGetDefuelExcluded.DeleteCommand = new SqlCommand("DELETE FROM #ASC_DEFUEL_EXCLUDED_DAYS "
																						+ "WHERE ndx = @ndx",
																						this.connection);
			this.adptGetDefuelExcluded.DeleteCommand.Parameters.Add("@ndx", SqlDbType.Int, 4, "ndx");

			this.dsGetDefuelExcluded = new DataSet();
			this.adptGetDefuelExcluded.Fill(dsGetDefuelExcluded);
			DataColumn[] primaryKey = new DataColumn[1];
			primaryKey[0] = this.dsGetDefuelExcluded.Tables[0].Columns["ndx"];
			this.dsGetDefuelExcluded.Tables[0].PrimaryKey = primaryKey;
			this.dsGetDefuelExcluded.Tables[0].Columns["ndx"].AutoIncrement = true;

			this.excludedDefuelDaysGrid.AutoGenerateColumns = false;
			this.excludedDefuelDaysGrid.DataSource = this.dsGetDefuelExcluded.Tables[0];
			this.excludedDefuelDaysGridDate.DataPropertyName = "ExcludedDay";

			PopulatePeakDefuelDaysGrid();
			PopulateExcludedDefuelDaysGrid();
		}

		private void PopulatePeakDefuelDaysGrid()
		{
			this.avgDefuels = 0.0F;

			this.dsGetDefuelPeaks.Clear();
			this.adptGetDefuelPeaks.Fill(this.dsGetDefuelPeaks);
			this.peakDefuelsGrid.Refresh();

			if (this.peakDefuelsGrid.Rows.Count > 0)
			{
				for (int i = 0; i < this.peakDefuelsGrid.Rows.Count; i++)
				{
					this.avgDefuels += (int)this.peakDefuelsGrid.Rows[i].Cells[1].Value; // Column 1 is the defuels count column
				}
				this.avgDefuels /= this.peakDefuelsGrid.Rows.Count;
			}

			this.quantityAverage.Text = this.avgDefuels.ToString("#0.###");

			if (this.avgDefuels >= 8.5F)
			{
				this.peacetimeVATH.Text = "0";
				this.peacetimeVATJ.Text = "0";
				this.peacetimeVATK.Text = "3";
			}
			else if (this.avgDefuels >= 3.5F)
			{
				this.peacetimeVATH.Text = "0";
				this.peacetimeVATJ.Text = "2";
				this.peacetimeVATK.Text = "0";
			}
			else
			{
				this.peacetimeVATH.Text = "1";
				this.peacetimeVATJ.Text = "0";
				this.peacetimeVATK.Text = "0";
			}
		}

		private void PopulateExcludedDefuelDaysGrid()
		{
			this.dsGetDefuelExcluded.Clear();
			this.adptGetDefuelExcluded.Fill(this.dsGetDefuelExcluded);
			this.excludedDefuelDaysGrid.Refresh();
		}
		#endregion

		#region Event Handlers
		private void peakDefuelsGrid_CellContentClick(Object senders, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex != (peakDefuelsGrid.Columns.Count - 1))
			{
				this.peakDefuelsGrid.ClearSelection();
				return;
			}

			try
			{
				DateTime dateToExclude = (DateTime)peakDefuelsGrid.Rows[e.RowIndex].Cells[0].Value;
				DataRow newRow;

				newRow = this.dsGetDefuelExcluded.Tables[0].NewRow();
				newRow["ExcludedDay"] = dateToExclude;
				this.dsGetDefuelExcluded.Tables[0].Rows.Add(newRow);
				this.adptGetDefuelExcluded.Update(this.dsGetDefuelExcluded);

				PopulatePeakDefuelDaysGrid();
				PopulateExcludedDefuelDaysGrid();
			}
			catch (InvalidCastException err)
			{
				err.ToString(); // Simply silence the warning
			}
		}

		private void excludedDefuelDaysGrid_CellContentClick(Object senders, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex != (excludedDefuelDaysGrid.Columns.Count - 1))
			{
				this.excludedDefuelDaysGrid.ClearSelection();
				return;
			}

			this.excludedDefuelDaysGrid.Rows.RemoveAt(e.RowIndex);
			this.adptGetDefuelExcluded.Update(this.dsGetDefuelExcluded);

			PopulatePeakDefuelDaysGrid();
			PopulateExcludedDefuelDaysGrid();
		}
		#endregion

		#region Private Member Variables
		float avgDefuels;

		SqlCommand cmdGetDefuelPeaks;
		SqlDataAdapter adptGetDefuelPeaks;
		DataSet dsGetDefuelPeaks;

		SqlCommand cmdGetDefuelExcluded;
		SqlDataAdapter adptGetDefuelExcluded;
		DataSet dsGetDefuelExcluded;
		#endregion

		#region Private Properties
		private int PeacetimeVATH
		{
			get
			{
				int ret;
				int.TryParse(this.peacetimeVATH.Text, out ret);
				return ret;
			}
		}

		private int PeacetimeVATJ
		{
			get
			{
				int ret;
				int.TryParse(this.peacetimeVATJ.Text, out ret);
				return ret;
			}
		}

		private int PeacetimeVATK
		{
			get
			{
				int ret;
				int.TryParse(this.peacetimeVATK.Text, out ret);
				return ret;
			}
		}

		private int WartimeVATH
		{
			get
			{
				int ret;
				int.TryParse(this.wartimeVATH.Text, out ret);
				return ret;
			}
			set
			{
				this.wartimeVATH.Text = value.ToString();
			}
		}

		private int WartimeVATJ
		{
			get
			{
				int ret;
				int.TryParse(this.wartimeVATJ.Text, out ret);
				return ret;
			}
			set
			{
				this.wartimeVATJ.Text = value.ToString();
			}
		}

		private int WartimeVATK
		{
			get
			{
				int ret;
				int.TryParse(this.wartimeVATK.Text, out ret);
				return ret;
			}
			set
			{
				this.wartimeVATK.Text = value.ToString();
			}
		}
		#endregion
	}
}