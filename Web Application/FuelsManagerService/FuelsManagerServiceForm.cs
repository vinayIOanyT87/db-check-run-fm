// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelsManagerServiceForm.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Used to run the FuelsManager Service in debug mode
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManagerService
{
	using System;
	using System.Threading.Tasks;
	using System.Windows.Forms;

	/// <summary>
	/// Used to run the FuelsManager Service in debug mode
	/// </summary>
	public partial class FuelsManagerServiceForm : Form
	{
		private readonly FuelsManagerService fuelsManagerService = new FuelsManagerService();

		public FuelsManagerServiceForm()
		{
			this.InitializeComponent();

			FuelsManagerSettings.LoadConfigFile();

			this.textBoxMaxNumLogins.Text = FuelsManagerSettings.MaxLoginAttempts.ToString();

			this.checkBoxUserAccountCleanup.Checked = FuelsManagerSettings.UserAccountCleanupEnabled;
			this.textBoxUserAccountCleanup.Text = FuelsManagerSettings.UserAccountCleanupInterval.ToString();

			this.checkBoxAuditProcessing.Checked = FuelsManagerSettings.AuditProcessingEnabled;
			this.textBoxAuditProcessing.Text = FuelsManagerSettings.AuditProcessingInterval.ToString();

			this.checkBoxAlarmAndEventProcessing.Checked = FuelsManagerSettings.AlarmAndEventProcessingEnabled;
			this.textBoxAlarmAndEventProcessing.Text = FuelsManagerSettings.AlarmAndEventProcessingInterval.ToString();

			this.checkBoxAlarmAndEventLogCleanup.Checked = FuelsManagerSettings.AlarmAndEventLogCleanupEnabled;
			this.textBoxAlarmAndEventLogCleanup.Text = FuelsManagerSettings.AlarmAndEventLogCleanupInterval.ToString();

			this.checkBoxSessionCleanup.Checked = FuelsManagerSettings.SessionCleanupEnabled;
			this.textBoxSessionCleanup.Text = FuelsManagerSettings.SessionCleanupInterval.ToString();

			this.checkBoxFMaePing.Checked = FuelsManagerSettings.FMaePingEnabled;
			this.textBoxFMaePing.Text = FuelsManagerSettings.FMaePingInterval.ToString();

			this.checkBoxFCEEMessagesCleanup.Checked = FuelsManagerSettings.FCEEMessagesCleanupEnabled;
			this.textBoxFCEEMessagesCleanup.Text = FuelsManagerSettings.FCEEMessagesCleanupInterval.ToString();
		}

		private void EnableSettings(bool enable)
		{
			this.textBoxMaxNumLogins.Enabled = enable;

			this.checkBoxUserAccountCleanup.Enabled = enable;
			this.textBoxUserAccountCleanup.Enabled = enable;

			this.checkBoxAuditProcessing.Enabled = enable;
			this.textBoxAuditProcessing.Enabled = enable;

			this.checkBoxAlarmAndEventProcessing.Enabled = enable;
			this.textBoxAlarmAndEventProcessing.Enabled = enable;

			this.checkBoxAlarmAndEventLogCleanup.Enabled = enable;
			this.textBoxAlarmAndEventLogCleanup.Enabled = enable;

			this.checkBoxSessionCleanup.Enabled = enable;
			this.textBoxSessionCleanup.Enabled = enable;

			this.checkBoxFMaePing.Enabled = enable;
			this.textBoxFMaePing.Enabled = enable;

            this.checkBoxFCEEMessagesCleanup.Enabled = enable;
            this.textBoxFCEEMessagesCleanup.Enabled = enable;
        }

		/// <summary>
		/// Fires when the start button is clicked
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		private void btnStart_Click(object sender, EventArgs e)
		{
			this.EnableSettings(false);
			this.btnStart.Enabled = false;
			this.btnStop.Enabled = true;

			// Save configuration settings to preserve any user changes
			FuelsManagerSettings.SaveConfigFile(Application.ExecutablePath);

			// If you don't start the service on a new thread, the WCF calls will not act concurrently.
			Task.Factory.StartNew(() => this.fuelsManagerService.Start());
		}

		/// <summary>
		/// Fires when the stop button is clicked
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		private void btnStop_Click(object sender, EventArgs e)
		{
			this.fuelsManagerService.Stop();

			this.EnableSettings(true);
			this.btnStart.Enabled = true;
			this.btnStop.Enabled = false;
		}

		private void textBoxMaxNumLogins_TextChanged(object sender, EventArgs e)
		{
			int newValue;
			bool valid = int.TryParse(this.textBoxMaxNumLogins.Text, out newValue);
			if (valid)
			{
				FuelsManagerSettings.MaxLoginAttempts = newValue;
			}
		}

		private void checkBoxUserAccountCleanup_CheckedChanged(object sender, EventArgs e)
		{
			FuelsManagerSettings.UserAccountCleanupEnabled = this.checkBoxUserAccountCleanup.Checked;
		}

		private void textBoxUserAccountCleanup_TextChanged(object sender, EventArgs e)
		{
			int newValue;
			bool valid = int.TryParse(this.textBoxUserAccountCleanup.Text, out newValue);
			if (valid)
			{
				FuelsManagerSettings.UserAccountCleanupInterval = newValue;
			}
		}

		private void checkBoxAuditProcessing_CheckedChanged(object sender, EventArgs e)
		{
			FuelsManagerSettings.AuditProcessingEnabled = this.checkBoxAuditProcessing.Checked;
		}

		private void textBoxAuditProcessing_TextChanged(object sender, EventArgs e)
		{
			int newValue;
			bool valid = int.TryParse(this.textBoxAuditProcessing.Text, out newValue);
			if (valid)
			{
				FuelsManagerSettings.AuditProcessingInterval = newValue;
			}
		}

		private void checkBoxAlarmAndEventProcessing_CheckedChanged(object sender, EventArgs e)
		{
			FuelsManagerSettings.AlarmAndEventProcessingEnabled = this.checkBoxAlarmAndEventProcessing.Checked;
		}

		private void textBoxAlarmAndEventProcessing_TextChanged(object sender, EventArgs e)
		{
			int newValue;
			bool valid = int.TryParse(this.textBoxAlarmAndEventProcessing.Text, out newValue);
			if (valid)
			{
				FuelsManagerSettings.AlarmAndEventProcessingInterval = newValue;
			}
		}

		private void checkBoxAlarmAndEventLogCleanup_CheckedChanged(object sender, EventArgs e)
		{
			FuelsManagerSettings.AlarmAndEventLogCleanupEnabled = this.checkBoxAlarmAndEventLogCleanup.Checked;
		}

		private void textBoxAlarmAndEventLogCleanup_TextChanged(object sender, EventArgs e)
		{
			int newValue;
			bool valid = int.TryParse(this.textBoxAlarmAndEventLogCleanup.Text, out newValue);
			if (valid)
			{
				FuelsManagerSettings.AlarmAndEventLogCleanupInterval = newValue;
			}
		}

		private void checkBoxSessionCleanup_CheckedChanged(object sender, EventArgs e)
		{
			FuelsManagerSettings.SessionCleanupEnabled = this.checkBoxSessionCleanup.Checked;
		}

		private void textBoxSessionCleanup_TextChanged(object sender, EventArgs e)
		{
			int newValue;
			bool valid = int.TryParse(this.textBoxSessionCleanup.Text, out newValue);
			if (valid)
			{
				FuelsManagerSettings.SessionCleanupInterval = newValue;
			}
		}

		private void checkBoxFMaePing_CheckedChanged(object sender, EventArgs e)
		{
			FuelsManagerSettings.FMaePingEnabled = this.checkBoxFMaePing.Checked;
		}

		private void textBoxFMaePing_TextChanged(object sender, EventArgs e)
		{
			int newValue;
			bool valid = int.TryParse(this.textBoxFMaePing.Text, out newValue);
			if (valid)
			{
				FuelsManagerSettings.FMaePingInterval = newValue;
			}
		}

        private void checkBoxFCEEMessagesCleanup_CheckedChanged(object sender, EventArgs e)
        {
            FuelsManagerSettings.FCEEMessagesCleanupEnabled = this.checkBoxFCEEMessagesCleanup.Checked;
        }

        private void textBoxFCEEMessagesCleanup_TextChanged(object sender, EventArgs e)
        {
            int newValue;
            bool valid = int.TryParse(this.textBoxFCEEMessagesCleanup.Text, out newValue);
            if (valid)
            {
                FuelsManagerSettings.FCEEMessagesCleanupInterval = newValue;
            }
        }
    }
}
