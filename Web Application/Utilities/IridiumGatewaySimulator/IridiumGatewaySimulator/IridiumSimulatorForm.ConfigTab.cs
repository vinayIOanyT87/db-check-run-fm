namespace IridiumGatewaySimulator
{
	using System;
	using System.Windows.Forms;

	using DataObjects.DataObjects;
	using DataObjects.Handlers;

	public partial class IridiumSimulatorForm
	{
		#region Configuration tab code
		/// <summary>
		/// This method will handle the Clear button event.  
		/// It clear all the text boxes on the configuration tab.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ConfigurationClearBtn_Click(object sender, EventArgs e)
		{
			this.configDo = null;

			this.FmIpAddr1TextBox.Text = string.Empty;
			this.FmIpAddr2TextBox.Text = string.Empty;
			this.FmIpAddr3TextBox.Text = string.Empty;
			this.FmIpAddr4TextBox.Text = string.Empty;
			this.FmListenerPortTextBox.Text = string.Empty;

			this.IridiumIpAddr1TextBox.Text = string.Empty;
			this.IridiumIpAddr2TextBox.Text = string.Empty;
			this.IridiumIpAddr3TextBox.Text = string.Empty;
			this.IridiumIpAddr4TextBox.Text = string.Empty;
			this.IridiumPortTextBox.Text = string.Empty;
		}

		/// <summary>
		/// This method will handle the Restore button on click event.
		/// It will read the simulator configuration data and restore
		/// the data on the page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ConfigurationRestoreBtnOnClick(object sender, EventArgs e)
		{
			var fileHandler = new FileHandler();
			this.configDo = fileHandler.ReadIridiumSimulatorConfigurationDataFromFile();

			if (this.configDo != null)
			{
				this.FmIpAddr1TextBox.Text = this.configDo.FmIpAddress1;
				this.FmIpAddr2TextBox.Text = this.configDo.FmIpAddress2;
				this.FmIpAddr3TextBox.Text = this.configDo.FmIpAddress3;
				this.FmIpAddr4TextBox.Text = this.configDo.FmIpAddress4;

				this.IridiumIpAddr1TextBox.Text = this.configDo.IridiumIpAddress1;
				this.IridiumIpAddr2TextBox.Text = this.configDo.IridiumIpAddress2;
				this.IridiumIpAddr3TextBox.Text = this.configDo.IridiumIpAddress3;
				this.IridiumIpAddr4TextBox.Text = this.configDo.IridiumIpAddress4;

				this.FmListenerPortTextBox.Text = this.configDo.FmPortNumberStr;
				this.IridiumPortTextBox.Text = this.configDo.IridiumPortNumberStr;
			}
		}

		/// <summary>
		/// This method will save the simulation configuration data to the
		/// specificed file. Will display an error dialog if there are validation
		/// errors or an exception.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ConfigurationSaveBtn_Click(object sender, EventArgs e)
		{
			string errMessage;

			bool isValid = this.ValidateConfigurationData(out errMessage);

			if (isValid == false)
			{
				// Display error message
				MessageBox.Show(
								errMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);

				return;
			}

			this.configDo = new ConfigurationDO
			{
				FmIpAddress1 = this.FmIpAddr1TextBox.Text,
				FmIpAddress2 = this.FmIpAddr2TextBox.Text,
				FmIpAddress3 = this.FmIpAddr3TextBox.Text,
				FmIpAddress4 = this.FmIpAddr4TextBox.Text,
				IridiumIpAddress1 = this.IridiumIpAddr1TextBox.Text,
				IridiumIpAddress2 = this.IridiumIpAddr2TextBox.Text,
				IridiumIpAddress3 = this.IridiumIpAddr3TextBox.Text,
				IridiumIpAddress4 = this.IridiumIpAddr4TextBox.Text,
				FmPortNumberStr = this.FmListenerPortTextBox.Text,
				IridiumPortNumberStr = this.IridiumPortTextBox.Text
			};

			try
			{
				var fileHandler = new FileHandler();
				fileHandler.SaveIridiumSimulatorConfiguration(this.configDo);

				MessageBox.Show(
								"Simulator configuration data successfully saved.",
								"Information",
								MessageBoxButtons.OK,
								MessageBoxIcon.Information,
								MessageBoxDefaultButton.Button1);
			}
			catch (Exception ex)
			{
				MessageBox.Show(
								"Error saving simulator configuration data. " + ex.Message,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);
			}
		}

		/// <summary>
		/// This method will validate the simulation configuration data
		/// prior to saving the data.
		/// </summary>
		/// <param name="errMessage">If there is an error, it will contain the error message.</param>
		/// <returns>Returns "True" if valid, otherwise returns "False".</returns>
		private bool ValidateConfigurationData(out string errMessage)
		{
			int ipValue;

			if (string.IsNullOrEmpty(this.FmIpAddr1TextBox.Text) ||
				string.IsNullOrEmpty(this.FmIpAddr2TextBox.Text) ||
				string.IsNullOrEmpty(this.FmIpAddr3TextBox.Text) ||
				string.IsNullOrEmpty(this.FmIpAddr4TextBox.Text))
			{
				errMessage = "The FuelsManager IP addresses must be populated.";
				return false;
			}

			if (string.IsNullOrEmpty(this.FmListenerPortTextBox.Text))
			{
				errMessage = "The FuelsManager Listener Port number must be populated.";
				return false;
			}

			if (int.TryParse(this.FmIpAddr1TextBox.Text, out ipValue) == false)
			{
				errMessage = "The IP value must be an integer.";
				return false;
			}

			if (int.TryParse(this.FmIpAddr2TextBox.Text, out ipValue) == false)
			{
				errMessage = "The IP value must be an integer.";
				return false;
			}

			if (int.TryParse(this.FmIpAddr3TextBox.Text, out ipValue) == false)
			{
				errMessage = "The IP value must be an integer.";
				return false;
			}

			if (int.TryParse(this.FmIpAddr4TextBox.Text, out ipValue) == false)
			{
				errMessage = "The IP value must be an integer.";
				return false;
			}

			if (int.TryParse(this.FmListenerPortTextBox.Text, out ipValue) == false)
			{
				errMessage = "The Port Number value must be an integer.";
				return false;
			}

			if (ipValue < 1 || ipValue > 65535)
			{
				errMessage = "The Port Number value must be between 1 and 65,535.";
				return false;
			}

			// If all the Iridium IP addresses and port number are blank
			// then return true (ok).
			if (string.IsNullOrEmpty(this.IridiumIpAddr1TextBox.Text) &&
				string.IsNullOrEmpty(this.IridiumIpAddr2TextBox.Text) &&
				string.IsNullOrEmpty(this.IridiumIpAddr3TextBox.Text) &&
				string.IsNullOrEmpty(this.IridiumIpAddr4TextBox.Text) &&
				string.IsNullOrEmpty(this.IridiumPortTextBox.Text))
			{
				errMessage = null;
				return true;
			}

			if (string.IsNullOrEmpty(this.IridiumIpAddr1TextBox.Text) ||
				string.IsNullOrEmpty(this.IridiumIpAddr2TextBox.Text) ||
				string.IsNullOrEmpty(this.IridiumIpAddr3TextBox.Text) ||
				string.IsNullOrEmpty(this.IridiumIpAddr4TextBox.Text))
			{
				errMessage = "The Iridium GSS Listener IP addresses must be populated.";
				return false;
			}

			if (string.IsNullOrEmpty(this.IridiumPortTextBox.Text))
			{
				errMessage = "The Iridium GSS Listener Port number must be populated.";
				return false;
			}

			if (int.TryParse(this.IridiumIpAddr1TextBox.Text, out ipValue) == false)
			{
				errMessage = "The IP value must be an integer.";
				return false;
			}

			if (int.TryParse(this.IridiumIpAddr2TextBox.Text, out ipValue) == false)
			{
				errMessage = "The IP value must be an integer.";
				return false;
			}

			if (int.TryParse(this.IridiumIpAddr3TextBox.Text, out ipValue) == false)
			{
				errMessage = "The IP value must be an integer.";
				return false;
			}

			if (int.TryParse(this.IridiumIpAddr4TextBox.Text, out ipValue) == false)
			{
				errMessage = "The IP value must be an integer.";
				return false;
			}

			if (int.TryParse(this.IridiumPortTextBox.Text, out ipValue) == false)
			{
				errMessage = "The Port Number value must be an integer.";
				return false;
			}

			if (ipValue < 1 || ipValue > 65535)
			{
				errMessage = "The Port Number value must be between 1 and 65,535.";
				return false;
			}

			errMessage = null;
			return true;
		}
		#endregion

	}
}
