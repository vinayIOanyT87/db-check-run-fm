//******************************************************************************
//	FILE NAME:		worksheetPATs.cs
//	PURPOSE:			tab for user input of Per Accomplished Task Times
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
//		07-May-2007	C. Knight		1.0.0.0	- Initial Creation
//
//*******************************************************************************       

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using DispatchPrototype;

namespace ASCReporter
{
	/// <summary>
	/// Dialog class for the Per Accomplished Task Times worksheet
	/// </summary>
   public partial class MainForm : FMBaseForm
	{
		#region Private Member Functions
		#endregion

		#region Event handlers
		#endregion

		#region Private member variables
		#endregion

		#region Private Properties
		private int PeacetimeDayLength
		{
			get
			{
				int ret;

				int.TryParse(this.peacetimeDayLength.SelectedItem.ToString(), out ret);
				return ret;
			}
		}

		private float LeavesRCC
		{
			get
			{
				float ret;

				float.TryParse(this.leavesRCC.Text, out ret);
				return ret;
			}
		}

		private float PositionsUnit
		{
			get
			{
				float ret;

				float.TryParse(this.positionsUnit.Text, out ret);
				return ret;
			}
		}

		private float PreparesUnit
		{
			get
			{
				float ret;

				float.TryParse(this.preparesUnit.Text, out ret);
				return ret;
			}
		}

		private float CompletesServicing
		{
			get
			{
				float ret;

				float.TryParse(this.completesServicing.Text, out ret);
				return ret;
			}
		}

		private float TravelsNextLocation
		{
			get
			{
				float ret;

				float.TryParse(this.travelsNextLocation.Text, out ret);
				return ret;
			}
		}

		private float TravelsStorage
		{
			get
			{
				float ret;

				float.TryParse(this.travelsStorage.Text, out ret);
				return ret;
			}
		}

		private float WaitsRefill
		{
			get
			{
				float ret;

				float.TryParse(this.waitsRefill.Text, out ret);
				return ret;
			}
		}

		#endregion
	}
}