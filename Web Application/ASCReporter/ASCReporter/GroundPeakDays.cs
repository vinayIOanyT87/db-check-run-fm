using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ASCReporter
{
	public partial class GroundPeakDays : Form
	{
		private struct PeakGroundDay
		{
			public DateTime peakDay;
			public int gallons;
		}

		float maxQuantity;

		public float MaxQuantity
		{
			get { return maxQuantity; }
			//set { avgQuantity = value; }
		}

		PeakGroundDay[] peakDays;

		LinkedList<DateTime> excludedDatesCollection;

		public GroundPeakDays()
		{
			InitializeComponent();

			excludedDatesCollection = new LinkedList<DateTime>();

			peakDays = new PeakGroundDay[12];

			maxQuantity = 0.0F;

			for (int i = 0; i < peakDays.GetLength(0); i++)
			{
				switch (i)
				{
					case 0:
						peakDays[i].gallons = 1325;
						peakDays[i].peakDay = new DateTime(2006, 7, 5);
						break;
					case 1:
						peakDays[i].gallons = 1321;
						peakDays[i].peakDay = new DateTime(2006, 8, 5);
						break;
					case 2:
						peakDays[i].gallons = 1265;
						peakDays[i].peakDay = new DateTime(2006, 12, 5);
						break;
					case 3:
						peakDays[i].gallons = 1221;
						peakDays[i].peakDay = new DateTime(2006, 10, 5);
						break;
					case 4:
						peakDays[i].gallons = 1165;
						peakDays[i].peakDay = new DateTime(2006, 2, 5);
						break;
					case 5:
						peakDays[i].gallons = 1152;
						peakDays[i].peakDay = new DateTime(2006, 4, 5);
						break;
					case 6:
						peakDays[i].gallons = 1096;
						peakDays[i].peakDay = new DateTime(2006, 3, 5);
						break;
					case 7:
						peakDays[i].gallons = 1025;
						peakDays[i].peakDay = new DateTime(2006, 1, 5);
						break;
					case 8:
						peakDays[i].gallons = 968;
						peakDays[i].peakDay = new DateTime(2006, 6, 5);
						break;
					case 9:
						peakDays[i].gallons = 941;
						peakDays[i].peakDay = new DateTime(2006, 5, 5);
						break;
					case 10:
						peakDays[i].gallons = 932;
						peakDays[i].peakDay = new DateTime(2006, 11, 5);
						break;
					case 11:
						peakDays[i].gallons = 923;
						peakDays[i].peakDay = new DateTime(2006, 9, 5);
						break;
					default:
						peakDays[i].gallons = 0;
						peakDays[i].peakDay = new DateTime(2006, 1, 31);
						break;
				}
			}

			peakDaysGrid.CellContentClick += new DataGridViewCellEventHandler(peakDaysGrid_CellContentClick);
			excludedDaysGrid.CellContentClick += new DataGridViewCellEventHandler(excludedDaysGrid_CellContentClick);
			PopulatePeakDaysGrid();
			PopulateExcludedDaysGrid();
		}

		private void PopulatePeakDaysGrid()
		{
			peakDaysGrid.Rows.Clear();
			maxQuantity = 0.0F;

			Object[] newRow = new Object[3];

			for (int i = 0; (i < peakDays.GetLength(0)) && (peakDaysGrid.Rows.Count < 6); i++)
			{
				if (null == excludedDatesCollection.Find(peakDays[i].peakDay))
				{
					newRow[0] = peakDays[i].peakDay;
					newRow[1] = peakDays[i].gallons;
					newRow[2] = "Exclude";

					peakDaysGrid.Rows.Add(newRow);

					if (peakDays[i].gallons > maxQuantity)
					{
						maxQuantity = peakDays[i].gallons;
					}
				}
			}

			quantityMax.Text = maxQuantity.ToString("####0.###");
		}

		private void PopulateExcludedDaysGrid()
		{
			excludedDaysGrid.Rows.Clear();

			Object[] newRow = new Object[2];

			foreach (DateTime excludedDay in excludedDatesCollection)
			{
				newRow[0] = excludedDay;
				newRow[1] = "Restore";

				excludedDaysGrid.Rows.Add(newRow);
			}
		}

		private void peakDaysGrid_CellContentClick(Object senders, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex != (peakDaysGrid.Columns.Count - 1))
			{
				return;
			}

			DateTime dateToExclude = (DateTime)peakDaysGrid.Rows[e.RowIndex].Cells[0].Value;

			if (null == excludedDatesCollection.Find(dateToExclude))
			{
				excludedDatesCollection.AddLast(dateToExclude);
			}

			PopulatePeakDaysGrid();
			PopulateExcludedDaysGrid();
		}

		private void excludedDaysGrid_CellContentClick(Object senders, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex != (excludedDaysGrid.Columns.Count - 1))
			{
				return;
			}

			DateTime dateToRestore = (DateTime)excludedDaysGrid.Rows[e.RowIndex].Cells[0].Value;

			excludedDatesCollection.Remove(dateToRestore);

			PopulatePeakDaysGrid();
			PopulateExcludedDaysGrid();
		}
	}
}