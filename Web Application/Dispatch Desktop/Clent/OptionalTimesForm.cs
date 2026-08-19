namespace Dispatch
{
	using System;
	using System.Configuration;
	using System.Windows.Forms;

	using FMBusinessObjects.DataObjects;

	public partial class OptionalTimesForm : FMBaseForm
	{
		public OptionalTimesForm()
		{
			this.InitializeComponent();
		}

		private void OptionalTimesFormLoad(object sender, EventArgs e)
		{
			this.GetSecurity();
			this.okButton.Enabled = this.Security.HasRight(RIGHT.MODIFY_DISPATCH);

			string useArrivalTime = ConfigurationManager.AppSettings["Use Arrival Time"];
			string useStartTime = ConfigurationManager.AppSettings["Use Start Time"];
			string useStopTime = ConfigurationManager.AppSettings["Use Stop Time"];

			if (useArrivalTime != null)
			{
				this.useArrivalTimeCheckBox.Checked = Convert.ToBoolean(useArrivalTime);
			}

			if (useStartTime != null)
			{
				this.useStartTimeCheckBox.Checked = Convert.ToBoolean(useStartTime);
			}

			if (useStopTime != null)
			{
				this.useStopTimeCheckBox.Checked = Convert.ToBoolean(useStopTime);
			}
		}

		private void CancelButtonClick(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}

		private void OkButtonClick(object sender, EventArgs e)
		{
			Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

			configuration.AppSettings.Settings.Remove("Use Arrival Time");
			configuration.AppSettings.Settings.Remove("Use Start Time");
			configuration.AppSettings.Settings.Remove("Use Stop Time");

			configuration.AppSettings.Settings.Add("Use Arrival Time", Convert.ToString(this.useArrivalTimeCheckBox.Checked));
			configuration.AppSettings.Settings.Add("Use Start Time", Convert.ToString(this.useStartTimeCheckBox.Checked));
			configuration.AppSettings.Settings.Add("Use Stop Time", Convert.ToString(this.useStopTimeCheckBox.Checked));

			// Save the configuration file.
			configuration.Save(ConfigurationSaveMode.Modified);

			// Force a reload of a changed section.
			ConfigurationManager.RefreshSection("appSettings");

			this.DialogResult = DialogResult.OK;
		}
	}
}
