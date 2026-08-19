using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using DispatchPrototype;

namespace ASCReporter
{
   partial class MainForm : FMBaseForm
	{
		#region Private member functions
		#endregion

		#region Event Handlers
		#endregion

		#region Private Member Variables
		#endregion

		#region Private Properties
		private int HsvBoiA
		{
			get
			{
				int ret;
				int.TryParse(this.hsvBOIA.Text, out ret);
				return ret;
			}
		}

		private int HsvBoiB
		{
			get
			{
				int ret;
				int.TryParse(this.hsvBOIB.Text, out ret);
				return ret;
			}
		}

		private int HsvBoiC
		{
			get
			{
				int ret;
				int.TryParse(this.hsvBOIC.Text, out ret);
				return ret;
			}
		}
		#endregion
	}
}
