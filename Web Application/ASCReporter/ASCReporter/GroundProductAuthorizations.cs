//******************************************************************************
//	FILE NAME:		GroundProductAuthorizations.cs
//	PURPOSE:			The Main Window class for the ASC Reporting Interface
//						Covers functionality specific to the Ground Refueler
//						Allocation tab
//
//	COMMENTS:
//		Copyright (C) Varec, Inc. Norcross, GA, USA, 2007
//		This file shall not be copied or reproduced in any form without
//		the express written consent of Varec, Inc.
//
//	AUTHOR(S):	Chris Knight
//	VERSION:		1.0.0.0  Current version
//
//	MODIFICATION HISTORY:
//		Date:			By:				Reason:
//		---------	-------------- -------------------------------------------
//		10-May-2007	C. Knight		1.0.0.0	- Initial Creation
//
//*******************************************************************************       
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using DispatchPrototype;

namespace ASCReporter
{
   public partial class MainForm : FMBaseForm
	{
		#region Private member functions
		private void recalcTotalGroundUnits()
		{
			int intermediateResult;

			intermediateResult = int.Parse(authorizedDieselUnits.Text) +
										int.Parse(authorizedLeadedUnits.Text) +
										int.Parse(authorizedUnleadedUnits.Text) +
										int.Parse(authorizedWasteUnits.Text) +
										int.Parse(authorizedUTCJFDEFUnits.Text);

			authorizedTotalUnits.Text = intermediateResult.ToString();
		}

		private void gpaTab_Init()
		{
			this.maxDieselQuantity = 0.0F;
			this.maxLeadedQuantity = 0.0F;
			this.maxUnleadedQuantity = 0.0F;
			this.groundVBBD.SelectedItem = "No";
			this.groundVQWS.SelectedItem = "No";
			this.authorizedDieselUnits.Text = "0";
			this.authorizedLeadedUnits.Text = "0";
			this.authorizedUnleadedUnits.Text = "0";
			this.authorizedWasteUnits.Text = "0";
			this.authorizedUTCJFDEFUnits.Text = "0";
			this.manualDiesel = false;
			this.manualLeaded = false;
			this.manualUnleaded = false;
		}
		#endregion

		#region Event Handlers
		private void dieselUsage_Click(object sender, EventArgs e)
		{
			int trucks;
			DialogResult ret;
			GroundActivity dieselActivity = new GroundActivity(GroundActivity.GroundActivityType.Diesel, 
																				this.yearEndDate.Value.Date,
																				this.connection,
																				this.siteGuid,
																				this.userName);
			dieselActivity.MaxQuantity = (int)this.maxDieselQuantity;
			dieselActivity.MaxQuantityOverride = this.manualDiesel;
			ret = dieselActivity.ShowDialog();
			if (ret == DialogResult.OK)
			{
				trucks = (int)(dieselActivity.MaxQuantity / 2000);
				if ((trucks < 1) && (dieselActivity.MaxQuantity >= 1))
				{
					trucks = 1;
				}

				authorizedDieselUnits.Text = trucks.ToString();
				this.maxDieselQuantity = dieselActivity.MaxQuantity;
				this.manualDiesel = dieselActivity.MaxQuantityOverride;
				recalcTotalGroundUnits();
			}
		}

		private void leadedUsage_Click(object sender, EventArgs e)
		{
			int trucks;
			DialogResult ret;

			GroundActivity leadedActivity = new GroundActivity(GroundActivity.GroundActivityType.Leaded,
																				this.yearEndDate.Value.Date,
																				this.connection,
																				this.siteGuid,
																				this.userName);
			leadedActivity.MaxQuantity = (int)this.maxLeadedQuantity;
			leadedActivity.MaxQuantityOverride = this.manualLeaded;
			ret = leadedActivity.ShowDialog();
			if (ret == DialogResult.OK)
			{
				trucks = (int)(leadedActivity.MaxQuantity / 2000);
				if ((trucks < 1) && (leadedActivity.MaxQuantity >= 1))
				{
					trucks = 1;
				}

				authorizedLeadedUnits.Text = trucks.ToString();
				this.maxLeadedQuantity = leadedActivity.MaxQuantity;
				this.manualLeaded = leadedActivity.MaxQuantityOverride;
				recalcTotalGroundUnits();
			}
		}

		private void unleadedUsage_Click(object sender, EventArgs e)
		{
			int trucks;
			DialogResult ret;

			GroundActivity unleadedActivity = new GroundActivity(GroundActivity.GroundActivityType.Unleaded,
																					this.yearEndDate.Value.Date,
																					this.connection,
																					this.siteGuid,
																					this.userName);
			unleadedActivity.MaxQuantity = (int)this.maxUnleadedQuantity;
			unleadedActivity.MaxQuantityOverride = this.manualUnleaded;
			ret = unleadedActivity.ShowDialog();
			if (ret == DialogResult.OK)
			{
				trucks = (int)(unleadedActivity.MaxQuantity / 2000);
				if ((trucks < 1) && (unleadedActivity.MaxQuantity >= 1))
				{
					trucks = 1;
				}

				authorizedUnleadedUnits.Text = trucks.ToString();
				this.maxUnleadedQuantity = unleadedActivity.MaxQuantity;
				this.manualUnleaded = unleadedActivity.MaxQuantityOverride;
				recalcTotalGroundUnits();
			}
		}

		private void groundVBBD_SelectedIndexChanged(object sender, EventArgs e)
		{
			authorizedWasteUnits.Text = groundVBBD.SelectedIndex.ToString();
			recalcTotalGroundUnits();
		}

		private void groundVQWS_SelectedIndexChanged(object sender, EventArgs e)
		{
			authorizedUTCJFDEFUnits.Text = groundVQWS.SelectedIndex.ToString();
			recalcTotalGroundUnits();
		}
		#endregion

		#region Private Member Variables
		float	maxDieselQuantity;
		float maxLeadedQuantity;
		float maxUnleadedQuantity;
		bool manualDiesel;
		bool manualLeaded;
		bool manualUnleaded;
		#endregion

		#region Private Properties
		private bool GroundVBBD
		{
			get
			{
				bool ret;

				if (this.groundVBBD.SelectedItem.ToString() == "Yes")
				{
					ret = true;
				}
				else
				{
					ret = false;
				}
				return ret;
			}
		}

		private bool GroundVQWS
		{
			get
			{
				bool ret;

				if (this.groundVQWS.SelectedItem.ToString() == "Yes")
				{
					ret = true;
				}
				else
				{
					ret = false;
				}
				return ret;
			}
		}

		private int AuthorizedDieselUnits
		{
			get
			{
				int ret;
				int.TryParse(this.authorizedDieselUnits.Text, out ret);
				return ret;
			}
		}

		private int AuthorizedLeadedUnits
		{
			get
			{
				int ret;
				int.TryParse(this.authorizedLeadedUnits.Text, out ret);
				return ret;
			}
		}

		private int AuthorizedUnleadedUnits
		{
			get
			{
				int ret;
				int.TryParse(this.authorizedUnleadedUnits.Text, out ret);
				return ret;
			}
		}

		private int AuthorizedWasteUnits
		{
			get
			{
				int ret;
				int.TryParse(this.authorizedWasteUnits.Text, out ret);
				return ret;
			}
		}

		private int AuthorizedUTCJFDEFUnits
		{
			get
			{
				int ret;
				int.TryParse(this.authorizedUTCJFDEFUnits.Text, out ret);
				return ret;
			}
		}
		#endregion
	}
}