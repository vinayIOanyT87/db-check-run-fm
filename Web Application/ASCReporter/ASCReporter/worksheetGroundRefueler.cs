using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ASCReporter
{
	public partial class GroundRefuelerWorksheet : Form
	{
		public GroundRefuelerWorksheet()
		{
			InitializeComponent();

			groundVBBD.SelectedIndex = 1;
			groundVQWS.SelectedIndex = 1;
		}

		private void calcDiesel_Click(object sender, EventArgs e)
		{
			GroundPeakDays groundPeakDays = new GroundPeakDays();
			DialogResult	ret;

			ret = groundPeakDays.ShowDialog();
			if (ret == DialogResult.OK)
			{
				maxDiesel.Text = groundPeakDays.MaxQuantity.ToString("####0.###");
				if (groundPeakDays.MaxQuantity >= 6000.0F)
				{
					authorizedDieselUnits.Text = "3";
				}
				else if (groundPeakDays.MaxQuantity >= 4000.0F)
				{
					authorizedDieselUnits.Text = "2";
				}
				else
				{
					authorizedDieselUnits.Text = "1";
				}
			}
			recalcTotalAuthorized();
		}

		private void calcLeaded_Click(object sender, EventArgs e)
		{
			GroundPeakDays groundPeakDays = new GroundPeakDays();
			DialogResult ret;

			ret = groundPeakDays.ShowDialog();
			if (ret == DialogResult.OK)
			{
				maxLeaded.Text = groundPeakDays.MaxQuantity.ToString("####0.###");
				if (groundPeakDays.MaxQuantity >= 6000.0F)
				{
					authorizedLeadedUnits.Text = "3";
				}
				else if (groundPeakDays.MaxQuantity >= 4000.0F)
				{
					authorizedLeadedUnits.Text = "2";
				}
				else
				{
					authorizedLeadedUnits.Text = "1";
				}
			}
			recalcTotalAuthorized();
		}

		private void calcUnleaded_Click(object sender, EventArgs e)
		{
			GroundPeakDays groundPeakDays = new GroundPeakDays();
			DialogResult ret;

			ret = groundPeakDays.ShowDialog();
			if (ret == DialogResult.OK)
			{
				maxUnleaded.Text = groundPeakDays.MaxQuantity.ToString("####0.###");
				if (groundPeakDays.MaxQuantity >= 6000.0F)
				{
					authorizedUnleadedUnits.Text = "3";
				}
				else if (groundPeakDays.MaxQuantity >= 4000.0F)
				{
					authorizedUnleadedUnits.Text = "2";
				}
				else
				{
					authorizedUnleadedUnits.Text = "1";
				}
			}
			recalcTotalAuthorized();
		}

		private void groundVBBD_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (groundVBBD.SelectedIndex == 0)
			{
				authorizedWasteUnits.Text = "1";
			}
			else
			{
				authorizedWasteUnits.Text = "0";
			}
			recalcTotalAuthorized();
		}

		private void groundVQWS_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (groundVQWS.SelectedIndex == 0)
			{
				authorizedUTCJFDFUnits.Text = "1";
			}
			else
			{
				authorizedUTCJFDFUnits.Text = "0";
			}
			recalcTotalAuthorized();
		}

		private void recalcTotalAuthorized()
		{
			int trucksDiesel;
			int trucksLeaded;
			int trucksUnleaded;
			int trucksWaste;
			int trucksUTC;
			int trucksTotal;

			try
			{
				trucksDiesel = int.Parse(authorizedDieselUnits.Text);
			}
			catch (Exception e)
			{
				e.ToString();
				trucksDiesel = 0;
			}

			try
			{
				trucksLeaded = int.Parse(authorizedLeadedUnits.Text);
			}
			catch (Exception e)
			{
				e.ToString();
				trucksLeaded = 0;
			}

			try
			{
				trucksUnleaded = int.Parse(authorizedUnleadedUnits.Text);
			}
			catch (Exception e)
			{
				e.ToString();
				trucksUnleaded = 0;
			}

			try
			{
				trucksWaste = int.Parse(authorizedWasteUnits.Text);
			}
			catch (Exception e)
			{
				e.ToString();
				trucksWaste = 0;
			}

			try
			{
				trucksUTC = int.Parse(authorizedUTCJFDFUnits.Text);
			}
			catch (Exception e)
			{
				e.ToString();
				trucksUTC = 0;
			}

			trucksTotal = trucksDiesel + trucksLeaded + trucksUnleaded + trucksWaste + trucksUTC;

			authorizedTotalUnits.Text = trucksTotal.ToString();
		}

		private void maxDiesel_TextChanged(object sender, EventArgs e)
		{
			int maxQuantity;
			int calculatedTrucks;

			maxQuantity = int.Parse(maxDiesel.Text);
			calculatedTrucks = maxQuantity / 2000; // integer division - meets specification in AS 019

			if ((calculatedTrucks < 1) && (maxQuantity > 0))
			{
				calculatedTrucks = 1;
			}

			authorizedDieselUnits.Text = calculatedTrucks.ToString();

			recalcTotalAuthorized();
		}

		private void maxLeaded_TextChanged(object sender, EventArgs e)
		{
			int maxQuantity;
			int calculatedTrucks;

			maxQuantity = int.Parse(maxLeaded.Text);
			calculatedTrucks = maxQuantity / 2000; // integer division - meets specification in AS 019

			if ((calculatedTrucks < 1) && (maxQuantity > 0))
			{
				calculatedTrucks = 1;
			}

			authorizedLeadedUnits.Text = calculatedTrucks.ToString();

			recalcTotalAuthorized();
		}

		private void maxUnleaded_TextChanged(object sender, EventArgs e)
		{
			int maxQuantity;
			int calculatedTrucks;

			maxQuantity = int.Parse(maxUnleaded.Text);
			calculatedTrucks = maxQuantity / 2000; // integer division - meets specification in AS 019

			if ((calculatedTrucks < 1) && (maxQuantity > 0))
			{
				calculatedTrucks = 1;
			}

			authorizedUnleadedUnits.Text = calculatedTrucks.ToString();

			recalcTotalAuthorized();
		}
	}
}