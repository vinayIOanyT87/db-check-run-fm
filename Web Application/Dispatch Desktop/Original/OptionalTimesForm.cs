using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using FMBusinessObjects.DataObjects;

namespace DispatchPrototype
{
	public partial class OptionalTimesForm : FMBaseForm
	{
		// MOD specific functionality (IGO 2010-Sep-22)
		protected System.Windows.Forms.CheckBox useDispatchedTimeCheckBox;

		public OptionalTimesForm()
		{
			InitializeComponent();

			// MOD specific functionality (IGO 2010-Sep-22)
			if (TargetCustomer.MOD == base.GetTargetCustomer())
			{
				// Create the new useDispatchedTimeCheckBox controls here instead of the 
				// OptionalTimesForm.designer.cs
				this.useDispatchedTimeCheckBox = new System.Windows.Forms.CheckBox();

				// useDispatchedTimeCheckBox
				this.useDispatchedTimeCheckBox.AutoSize = false;
				this.useDispatchedTimeCheckBox.Location = new System.Drawing.Point(140, 28);
				this.useDispatchedTimeCheckBox.Margin = new System.Windows.Forms.Padding(2);
				this.useDispatchedTimeCheckBox.Name = "useDispatchedTimeCheckBox";
				this.useDispatchedTimeCheckBox.Size = new System.Drawing.Size(103, 17);
				this.useDispatchedTimeCheckBox.TabIndex = 6;
				this.useDispatchedTimeCheckBox.Text = "Use Dispatched Time";

				// add the control to the group box
				this.selecctOptionalTimesGroupBox.Controls.Add(this.useDispatchedTimeCheckBox);
			}
		}

		private void OptionalTimesForm_Load(object sender,EventArgs e)
		{
         GetSecurity();
         okButton.Enabled = Security.HasRight( RIGHT.MODIFY_DISPATCH );

			string useArrivalTime = ConfigurationManager.AppSettings["Use Arrival Time"];
			string useStartTime = ConfigurationManager.AppSettings["Use Start Time"];
			string useStopTime = ConfigurationManager.AppSettings["Use Stop Time"];
			
			if(useArrivalTime != null)
			{
				useArrivalTimeCheckBox.Checked=System.Convert.ToBoolean(useArrivalTime);
			}

			if(useStartTime != null)
			{
				useStartTimeCheckBox.Checked=System.Convert.ToBoolean(useStartTime);
			}

			if(useStopTime != null)
			{
				useStopTimeCheckBox.Checked=System.Convert.ToBoolean(useStopTime);
			}

			// MOD specific functionality (IGO 2010-Sep-22)
			if (TargetCustomer.MOD == base.GetTargetCustomer())
			{
				string useDispatchedTime = ConfigurationManager.AppSettings["Use Dispatched Time"];
				if (useDispatchedTime != null)
				{
					useDispatchedTimeCheckBox.Checked = System.Convert.ToBoolean(useDispatchedTime);
				}
			}
		}

		private void cancelButton_Click(object sender,EventArgs e)
		{
			DialogResult=DialogResult.Cancel;
		}

		private void okButton_Click(object sender,EventArgs e)
		{
			Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

			configuration.AppSettings.Settings.Remove("Use Arrival Time");
			configuration.AppSettings.Settings.Remove("Use Start Time");
			configuration.AppSettings.Settings.Remove("Use Stop Time");
			
			configuration.AppSettings.Settings.Add("Use Arrival Time",System.Convert.ToString(useArrivalTimeCheckBox.Checked));
			configuration.AppSettings.Settings.Add("Use Start Time",System.Convert.ToString(useStartTimeCheckBox.Checked));
			configuration.AppSettings.Settings.Add("Use Stop Time",System.Convert.ToString(useStopTimeCheckBox.Checked));

			// MOD specific functionality (IGO 2010-Sep-22)
			if (TargetCustomer.MOD == base.GetTargetCustomer())
			{
				configuration.AppSettings.Settings.Remove("Use Dispatched Time");
				configuration.AppSettings.Settings.Add("Use Dispatched Time", System.Convert.ToString(useDispatchedTimeCheckBox.Checked));
			}

			// Save the configuration file.
			configuration.Save(ConfigurationSaveMode.Modified);

			// Force a reload of a changed section.
			ConfigurationManager.RefreshSection("appSettings");

			DialogResult=DialogResult.OK;
		}
	}
}
