using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ASCReporter
{
	public partial class AviationRefuelerWorksheet : Form
	{
		public AviationRefuelerWorksheet()
		{
			InitializeComponent();

			peacetimeVATK.Text = "0";
			peacetimeVATJ.Text = "0";
			peacetimeVATH.Text = "0";
		}

		private void AviationRefuelerWorksheet_Load(object sender, EventArgs e)
		{

		}

		private void calculateAverages_Click(object sender, EventArgs e)
		{
			RefuelPeakDays refuelPeakDays = new RefuelPeakDays();
			DialogResult	ret;

			ret = refuelPeakDays.ShowDialog();
			if (ret == DialogResult.OK)
			{
				peacetimeVATC400.Text = refuelPeakDays.AvgRuns400.ToString("###0.###");
				peacetimeVATC1800.Text = refuelPeakDays.AvgRuns1800.ToString("###0.###");
				peacetimeVATC2700.Text = refuelPeakDays.AvgRuns2700.ToString("###0.###");
				peacetimeVATC3500.Text = refuelPeakDays.AvgRuns3500.ToString("###0.###");
				peacetimeVATC3501.Text = refuelPeakDays.AvgRuns3501.ToString("###0.###");
			}
		}

		private void calculateDefuelAverage_Click(object sender, EventArgs e)
		{
			DefuelPeakDays defuelPeakDays = new DefuelPeakDays();
			DialogResult	ret;

			ret = defuelPeakDays.ShowDialog();
			if (ret == DialogResult.OK)
			{
				if (defuelPeakDays.AvgDefuels >= 8.5)
				{
					peacetimeVATK.Text = "3";
					peacetimeVATJ.Text = "0";
					peacetimeVATH.Text = "0";
				}
				else if (defuelPeakDays.AvgDefuels >= 3.5)
				{
					peacetimeVATK.Text = "0";
					peacetimeVATJ.Text = "2";
					peacetimeVATH.Text = "0";
				}
				else
				{
					peacetimeVATK.Text = "0";
					peacetimeVATJ.Text = "0";
					peacetimeVATH.Text = "1";
				}
			}
		}
	}
}