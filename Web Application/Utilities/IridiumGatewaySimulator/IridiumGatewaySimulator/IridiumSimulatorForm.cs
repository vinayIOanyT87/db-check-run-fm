namespace IridiumGatewaySimulator
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Globalization;
	using System.Linq;
	using System.Net.Sockets;
	using System.Windows.Forms;

	using DataObjects.DataObjects;
	using DataObjects.Handlers;

	public partial class IridiumSimulatorForm : Form
	{
		private MobileOriginatedMessageDO moMessageDo;
		private ConfigurationDO configDo;
		private SaveFileDialog saveMoMessageFileDialog;
		private OpenFileDialog openMoMessageFileDialog;
		private OpenFileDialog openRawDataFileDialog;
		private List<RawDataDO> rawDataCollection;
		private readonly EventLog eventLog;

		public IridiumSimulatorForm()
		{
			this.InitializeComponent();

			// Load the simulator configuration data.
			this.ConfigurationRestoreBtnOnClick(null, null);

			// Initialize client tab.
			this.InitializeClientTab();

			// Initialize raw data tab.
			this.InitializeRawDataTab();

			// Initialize WRDCU payload data tab.
			this.InitializeWrdcuPayloadTab();

			// Initialize TDU payload data tab.
			this.InitializeTduTab();

			// Initialize Position payload data tab.
			this.InitializePositionPayloadTab();

			this.eventLog = new EventLog("Application", ".", "FuelsManager");
		}

		#region Client Tab Code
		/// <summary>
		/// This method will initialize the client tab to its initial state.
		/// </summary>
		private void InitializeClientTab()
		{
			this.ClientClearBtnOnClick(null, null);
		}

		/// <summary>
		/// This method will handle the change event for the North radio button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NorthIndicatorRadioBtnOnClick(object sender, EventArgs e)
		{
			if (this.NorthIndicatorRadioBtn.Checked)
			{
				this.SouthIndicatorRadioBtn.Checked = false;
				
				if (string.IsNullOrEmpty(this.LatitudeTextBox.Text) == false)
				{
					double latitude;

					if (double.TryParse(this.LatitudeTextBox.Text, out latitude))
					{
						latitude = Math.Abs(latitude);
						this.LatitudeTextBox.Text = latitude.ToString(CultureInfo.InvariantCulture);

						if (this.moMessageDo.LatitudeLongitude == null)
						{
							this.moMessageDo.LatitudeLongitude = new CoordinateDO();
						}

						this.moMessageDo.LatitudeLongitude.LatitudeDouble = latitude;
						this.moMessageDo.LatitudeLongitude.EastWestIndicator = this.NorthIndicatorRadioBtn.Checked ? 0 : 1;
					}
				}
			}
		}

		/// <summary>
		/// This method will handle the change event for the South radio button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SouthIndicatorRadioBtnOnClick(object sender, EventArgs e)
		{
			if (this.SouthIndicatorRadioBtn.Checked)
			{
				this.NorthIndicatorRadioBtn.Checked = false;

				if (string.IsNullOrEmpty(this.LatitudeTextBox.Text) == false)
				{
					double latitude;

					if (double.TryParse(this.LatitudeTextBox.Text, out latitude))
					{
						latitude = Math.Abs(latitude) * -1;
						this.LatitudeTextBox.Text = latitude.ToString(CultureInfo.InvariantCulture);

						if (this.moMessageDo.LatitudeLongitude == null)
						{
							this.moMessageDo.LatitudeLongitude = new CoordinateDO();
						}

						this.moMessageDo.LatitudeLongitude.LatitudeDouble = latitude;
						this.moMessageDo.LatitudeLongitude.EastWestIndicator = this.NorthIndicatorRadioBtn.Checked ? 0 : 1;
					}
				}
			}
		}

		/// <summary>
		/// This method will handle the change event for the East radio button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void EastIndicatorRadioBtnOnClick(object sender, EventArgs e)
		{
			if (this.EastIndicatorRadioBtn.Checked)
			{
				this.WestRadioIndicatorBtn.Checked = false;

				if (string.IsNullOrEmpty(this.LongitudeTextBox.Text) == false)
				{
					double longitude;

					if (double.TryParse(this.LongitudeTextBox.Text, out longitude))
					{
						longitude = Math.Abs(longitude);
						this.LongitudeTextBox.Text = longitude.ToString(CultureInfo.InvariantCulture);

						if (this.moMessageDo.LatitudeLongitude == null)
						{
							this.moMessageDo.LatitudeLongitude = new CoordinateDO();
						}

						this.moMessageDo.LatitudeLongitude.LongitudeDouble = longitude;
						this.moMessageDo.LatitudeLongitude.EastWestIndicator = this.EastIndicatorRadioBtn.Checked ? 0 : 1;
					}
				}
			}
		}

		/// <summary>
		/// This method will handle the change event for the West radio button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void WestIndicatorRadioBtnOnClick(object sender, EventArgs e)
		{
			if (this.WestRadioIndicatorBtn.Checked)
			{
				this.EastIndicatorRadioBtn.Checked = false;

				if (string.IsNullOrEmpty(this.LongitudeTextBox.Text) == false)
				{
					double longitude;

					if (double.TryParse(this.LongitudeTextBox.Text, out longitude))
					{
						longitude = Math.Abs(longitude) * -1;
						this.LongitudeTextBox.Text = longitude.ToString(CultureInfo.InvariantCulture);

						if (this.moMessageDo.LatitudeLongitude == null)
						{
							this.moMessageDo.LatitudeLongitude = new CoordinateDO();
						}

						this.moMessageDo.LatitudeLongitude.LongitudeDouble = longitude;
						this.moMessageDo.LatitudeLongitude.EastWestIndicator = this.EastIndicatorRadioBtn.Checked ? 0 : 1;
					}
				}
			}
		}

		/// <summary>
		/// This method will handle the Leave event on the latitude text box.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LatitudeTextBoxOnLeave(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(this.LatitudeTextBox.Text))
			{
				return;
			}

			double latitude;

			if (double.TryParse(this.LatitudeTextBox.Text, out latitude) == false)
			{
				const string ErrMessage = "Must be numeric.";

				MessageBox.Show(
								ErrMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);

				return;
			}

			this.NorthIndicatorRadioBtn.Checked = true;
			this.SouthIndicatorRadioBtn.Checked = false;

			if (latitude < 0.0)
			{
				this.NorthIndicatorRadioBtn.Checked = false;
				this.SouthIndicatorRadioBtn.Checked = true;
			}

			if (latitude < -90.0 || latitude > 90.0)
			{
				this.LatitudeTextBox.Text = string.Empty;
				const string ErrMessage = "Latitude must be between -90 and 90 degrees.";

				MessageBox.Show(
								ErrMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);

				this.NorthIndicatorRadioBtn.Checked = true;
				this.SouthIndicatorRadioBtn.Checked = false;
				return;
			}

			if (this.moMessageDo.LatitudeLongitude == null)
			{
				this.moMessageDo.LatitudeLongitude = new CoordinateDO();
			}

			this.moMessageDo.LatitudeLongitude.LatitudeDouble = latitude;
			this.moMessageDo.LatitudeLongitude.NorthSouthIndicator = this.NorthIndicatorRadioBtn.Checked ? 0 : 1;
		}

		/// <summary>
		/// This method will handle the Leave event on the longitude text box.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LongitudeTextBoxOnLeave(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(this.LongitudeTextBox.Text))
			{
				return;
			}

			double longitude;

			if (double.TryParse(this.LongitudeTextBox.Text, out longitude) == false)
			{
				const string ErrMessage = "Must be numeric.";

				MessageBox.Show(
								ErrMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);

				return;
			}

			this.EastIndicatorRadioBtn.Checked = true;
			this.WestRadioIndicatorBtn.Checked = false;

			if (longitude < 0.0)
			{
				this.EastIndicatorRadioBtn.Checked = false;
				this.WestRadioIndicatorBtn.Checked = true;
			}

			if (longitude < -180.0 || longitude > 180.0)
			{
				this.LongitudeTextBox.Text = string.Empty;
				const string ErrMessage = "Longitude must be between -180 and 180 degrees.";

				MessageBox.Show(
								ErrMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);

				this.EastIndicatorRadioBtn.Checked = true;
				this.WestRadioIndicatorBtn.Checked = false;
			}

			if (this.moMessageDo.LatitudeLongitude == null)
			{
				this.moMessageDo.LatitudeLongitude = new CoordinateDO();
			}

			this.moMessageDo.LatitudeLongitude.LongitudeDouble = longitude;
			this.moMessageDo.LatitudeLongitude.EastWestIndicator = this.EastIndicatorRadioBtn.Checked ? 0 : 1;
		}

		/// <summary>
		/// This method handles the Leave event for the CEP Radius text box.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CepRadiusTextBoxOnLeave(object sender, EventArgs e)
		{
			int cepRadius;

			if (string.IsNullOrEmpty(this.CepRadiusTextBox.Text))
			{
				return;
			}

			if (int.TryParse(this.CepRadiusTextBox.Text, out cepRadius) == false)
			{
				this.CepRadiusTextBox.Text = string.Empty;
				const string ErrMessage = "Must be an integer.";

				MessageBox.Show(
								ErrMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);

				return;
			}

			if (cepRadius < 1 || cepRadius > 2000)
			{
				this.CepRadiusTextBox.Text = string.Empty;
				const string ErrMessage = "Must be an integer value between 1 - 2000.";

				MessageBox.Show(
								ErrMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);
				return;
			}

			this.moMessageDo.CepRadius = (uint)cepRadius;
		}

		/// <summary>
		/// This method will handle the Leave event on the Session Status text box.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SessionStatusTextBoxOnLeave(object sender, EventArgs e)
		{
			int sessionStatus;

			if (string.IsNullOrEmpty(this.SessionStatusTextBox.Text))
			{
				return;
			}

			if (int.TryParse(this.SessionStatusTextBox.Text, out sessionStatus) == false)
			{
				this.SessionStatusTextBox.Text = string.Empty;
				const string ErrMessage = "Must be a numeric value.";

				MessageBox.Show(
								ErrMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);
				return;
			}

			if (sessionStatus != 0 && 
				sessionStatus != 1 &&
				sessionStatus != 2 &&
				sessionStatus != 10 &&
				sessionStatus != 13 &&
				sessionStatus != 14 &&
				sessionStatus != 15)
			{
				this.SessionStatusTextBox.Text = string.Empty;

				const string ErrMessage = "Valid entries are:\n"
										+ " 0 - The SBD session completed successfully.\n"
										+ " 1 - The MO message transfer, if any, was successful.\n"
										+ "     The MT message queued at the Iridium Gateway is\n"
										+ "     too large to be transferred within a single SBD session.\n"
										+ " 2 - The MO message transfer, if any, was successful.\n"
										+ "     The reported location was determined to be of\n"
										+ "     unacceptable quality. This value is only applicable\n"
										+ "     to IMEIs using SBD protocol revision 1.\n"
										+ "10 - The SBD session timed out before session completion.\n"
										+ "12 - The MO message being transferred by the IMEI is too\n"
										+ "     large to be transferred within a single SBD session.\n"
										+ "13 - An RF link loss occurred during the SBD session.\n"
										+ "14 - An IMEI protocol anomaly occurred during SBD session.\n"
										+ "15 - The IMEI is prohibited from accessing the Iridium Gateway.";

				MessageBox.Show(
								ErrMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);

				return;
			}

			this.moMessageDo.SessionStatus = (ushort)sessionStatus;
		}

		/// <summary>
		/// This method will handle the Leave event on the CDR Reference text box.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CdrReferenceTextBoxOnLeave(object sender, EventArgs e)
		{
			uint cdrReference;

			if (string.IsNullOrEmpty(this.CdrReferenceTextBox.Text))
			{
				return;
			}

			if (uint.TryParse(this.CdrReferenceTextBox.Text, out cdrReference) == false)
			{
				this.CdrReferenceTextBox.Text = string.Empty;
				const string ErrMessage = "Must be an unsigned integer value between 0 - 4294967295.";

				MessageBox.Show(
								ErrMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);
				return;
			}

			this.moMessageDo.CdrReference = cdrReference;
		}

		/// <summary>
		/// This method will handle the Leave event on the MOMSN text box.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MomsnTextBoxOnLeave(object sender, EventArgs e)
		{
			int momsn;

			if (string.IsNullOrEmpty(this.MomsnTextBox.Text))
			{
				return;
			}

			if (int.TryParse(this.MomsnTextBox.Text, out momsn) == false)
			{
				this.MomsnTextBox.Text = string.Empty;
				const string ErrMessage = "Must be a numeric value.";

				MessageBox.Show(
								ErrMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);

				return;
			}

			if (momsn < 1 || momsn > 65535)
			{
				const string ErrMessage = "Must be a value between 1 - 65535.";

				this.MomsnTextBox.Text = string.Empty;

				MessageBox.Show(
								ErrMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);

				return;
			}

			this.moMessageDo.Momsn = (ushort)momsn;
		}

		/// <summary>
		/// This method will handle the Leave event on the MOMSN text box.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MtmsnTextBoxOnLeave(object sender, EventArgs e)
		{
			int mtmsn;

			if (string.IsNullOrEmpty(this.MtmsnTextBox.Text))
			{
				return;
			}

			if (int.TryParse(this.MtmsnTextBox.Text, out mtmsn) == false)
			{
				this.MtmsnTextBox.Text = string.Empty;
				const string ErrMessage = "Must be a numeric value.";

				MessageBox.Show(
								ErrMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);

				return;
			}

			if (mtmsn < 0 || mtmsn > 65535)
			{
				const string ErrMessage = "Must be a value between 0 - 65535.";

				this.MtmsnTextBox.Text = string.Empty;

				MessageBox.Show(
								ErrMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);

				return;
			}

			this.moMessageDo.Mtmsn = (ushort)mtmsn;
		}

		/// <summary>
		/// This method will handle the leave event on the Session Time text box.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HeaderSessionTimeOnLeave(object sender, EventArgs e)
		{
			DateTime sessionTime;

			if (string.IsNullOrEmpty(this.SessionTimeTextBox.Text))
			{
				return;
			}

			bool dateValid = this.ValidateDateFormat();

			if (dateValid == false || DateTime.TryParse(this.SessionTimeTextBox.Text, out sessionTime) == false)
			{
				const string ErrMessage = "Session Time must be in the following format: yyyy/mm/dd hh:mm:ss";

				this.SessionStatusTextBox.Text = string.Empty;

				MessageBox.Show(
								ErrMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);

				return;
			}

			var epoch = new EpochDate();
			uint? epochDateTime = epoch.ConvertToEpochDate(this.SessionTimeTextBox.Text);

			if (epochDateTime != null)
			{
				this.moMessageDo.TimeOfSession = epochDateTime.Value;
			}
		}

		/// <summary>
		/// This method will valide the session date to ensure it is in the correct
		/// format yyyy/mm/dd hh:mm:ss.
		/// </summary>
		/// <returns>Returns true if valid, otherwise, it return false.</returns>
		private bool ValidateDateFormat()
		{
			if (this.SessionTimeTextBox.Text.Length < 19)
			{
				return false;
			}

			var parts = this.SessionTimeTextBox.Text.Split(' ');
			if (parts.Length != 2)
			{
				return false;
			}

			var dateParts = parts[0].Split('/');
			if (dateParts.Length != 3)
			{
				return false;
			}

			var timeParts = parts[1].Split(':');
			if (timeParts.Length != 3)
			{
				return false;
			}

			if (dateParts[0].Length < 4 || dateParts[1].Length < 2 || dateParts[2].Length < 2)
			{
				return false;
			}

			if (timeParts[0].Length < 2 || timeParts[1].Length < 2 || timeParts[2].Length < 2)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// This method handles the Payload List Grid cell select event. It will remove a process
		/// from the monitor list.
		/// </summary>
		/// <param name="sender">The sending object.</param>
		/// <param name="e">Event arguments.</param>
		private void PayloadDataGridViewCellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			const int RemoveButtonIndex = 2;
			const int ByteNumberCellIndex = 0;

			// Only process if the remove button was selected.
			if (e.ColumnIndex != RemoveButtonIndex)
			{
				return;
			}

			PayloadDO payloadDo = this.moMessageDo.MoPayload[e.RowIndex];

			DialogResult dialogResult = MessageBox.Show(
								"Remove Byte Number'" + payloadDo.ByteNumber + "' from the list?",
								"Question",
								MessageBoxButtons.YesNo,
								MessageBoxIcon.Question,
								MessageBoxDefaultButton.Button1);

			if (dialogResult == DialogResult.Yes)
			{
				if (e.RowIndex < this.moMessageDo.MoPayload.Count)
				{
					this.moMessageDo.MoPayload.RemoveAt(e.RowIndex);
					this.PayloadDataGridView.Rows.RemoveAt(e.RowIndex);
					int byteCount = 0;

					// Re-number the byte number since one was removed.
					foreach(PayloadDO payload in this.moMessageDo.MoPayload)
					{
						payload.ByteNumber = byteCount;
						var byteCountCell = this.PayloadDataGridView.Rows[byteCount].Cells[ByteNumberCellIndex];
						byteCountCell.Value = byteCount;

						byteCount++;
					}

					this.moMessageDo.MoPayload = this.moMessageDo.MoPayload.OrderBy(x => x.ByteNumber).ToList();
					this.PayloadLengthTextBox.Text = (this.moMessageDo.MoPayload.Count + 3).ToString();

					if (this.moMessageDo.MoPayload.Count == 0)
					{
						this.ClientGenerateChecksumBtn.Enabled = false;
					}
				}
			}
		}

		/// <summary>
		/// This method will handle the cell end edit event. This will be either a 
		/// new row being added or an existing row being edited.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PayloadDataGridViewCellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex >= 1960)
			{
				const string ErrMessage = "Cannot add more than 1960 payload bytes.";

				MessageBox.Show(
								ErrMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);
				return;
			}

			// If the row index is great than or equal to the payload count list,
			// then this will be a new row added.  Else it is an existing row being
			// edited.
			if (e.RowIndex >= this.moMessageDo.MoPayload.Count)
			{
				var cell = this.PayloadDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];

				if (PayloadDO.TestHexString((string)cell.Value) == false)
				{
					const string ErrMessage = "Hex value must be '0x00' - '0xFF' format.";

					MessageBox.Show(
									ErrMessage,
									"Error",
									MessageBoxButtons.OK,
									MessageBoxIcon.Error,
									MessageBoxDefaultButton.Button1);

					cell.Value = string.Empty;
				}

				
				var payloadDo = new PayloadDO
				                {
					                ByteNumber = this.moMessageDo.MoPayload.Count,
									RealValue = PayloadDO.ConvertHexStringToByte((string)cell.Value)
				};

				this.moMessageDo.MoPayload.Add(payloadDo);

				var byteCountCell = this.PayloadDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex -1];
				byteCountCell.Value = payloadDo.ByteNumber;

				this.PayloadLengthTextBox.Text = (this.moMessageDo.MoPayload.Count + 3).ToString();
				this.ClientGenerateChecksumBtn.Enabled = true;
			}
			else
			{
				PayloadDO payloadDo;
				var cell = this.PayloadDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];

				if (PayloadDO.TestHexString((string) cell.Value) == false)
				{
					const string ErrMessage = "Hex value must be '0x00' - '0xFF' format.";

					MessageBox.Show(
									ErrMessage,
									"Error",
									MessageBoxButtons.OK,
									MessageBoxIcon.Error,
									MessageBoxDefaultButton.Button1);

					payloadDo = this.moMessageDo.MoPayload[e.RowIndex];
					cell.Value = payloadDo.ByteHexValue;
					return;
				}

				payloadDo = this.moMessageDo.MoPayload[e.RowIndex];
				payloadDo.RealValue = PayloadDO.ConvertHexStringToByte((string)cell.Value);
			}
		}

		/// <summary>
		/// This method will handle the Client Clear button on click.
		/// It will clear all fields.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ClientClearBtnOnClick(object sender, EventArgs e)
		{
			this.moMessageDo = new MobileOriginatedMessageDO();

			// Clear header section
			this.HeaderLengthTextBox.Text	= string.Empty;
			this.CdrReferenceTextBox.Text	= string.Empty;
			this.ImeiTextBox.Text			= string.Empty;
			this.MomsnTextBox.Text			= string.Empty;
			this.MtmsnTextBox.Text			= string.Empty;
			this.SessionStatusTextBox.Text	= string.Empty;
			this.SessionTimeTextBox.Text	= string.Empty;

			// Clear location section
			this.LocationLengthTextBox.Text		= string.Empty;
			this.FormatCodeTextBox.Text			= "0";
			this.ReservedTextBox.Text			= "0";
			this.LatitudeTextBox.Text			= string.Empty;
			this.LongitudeTextBox.Text			= string.Empty;
			this.NorthIndicatorRadioBtn.Checked = true;
			this.SouthIndicatorRadioBtn.Checked = false;
			this.EastIndicatorRadioBtn.Checked	= true;
			this.WestRadioIndicatorBtn.Checked	= false;
			this.CepRadiusTextBox.Text			= string.Empty;

			// Clear payload section
			this.PayloadLengthTextBox.Text = string.Empty;
			this.moMessageDo.MoPayload.Clear();
			this.ClientGenerateChecksumBtn.Enabled = false;

			int startingIndex = this.PayloadDataGridView.Rows.Count - 1;
			for (int rowIndex = startingIndex; rowIndex >= 0; rowIndex--)
			{
				if (this.PayloadDataGridView.Rows[rowIndex].IsNewRow == false)
				{
					this.PayloadDataGridView.Rows.RemoveAt(rowIndex);
				}
			}

			// Clear confirmation section
			this.ConfirmationMessageTextBox.Text = string.Empty;
			this.ConfirmationMessageBytesRecTextBox.Text = string.Empty;
		}

		/// <summary>
		/// This method will open a File Open Dialog for the user
		/// to select the MO message file load.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ClientOpenFileBtnOnClick(object sender, EventArgs e)
		{
			// Create an instance of the open file dialog box.
			if (this.openMoMessageFileDialog == null)
			{
				this.openMoMessageFileDialog = new OpenFileDialog();
			}

            // Set filter options and filter index.
            this.openMoMessageFileDialog.Filter = "XML Files (.xml)|*.xml";
            this.openMoMessageFileDialog.FilterIndex = 1;

            this.openMoMessageFileDialog.Multiselect = false;

            // Call the ShowDialog method to show the dialog box.
			this.openMoMessageFileDialog.ShowDialog();

			try
			{
				this.ClientClearBtnOnClick(null, null);

				var fileHandler = new FileHandler();
				this.moMessageDo = fileHandler.ReadMoMessageFile(this.openMoMessageFileDialog);
				this.UpdateWrdcuPayloadTab();
				this.UpdatePositionPayloadTab();
				this.UpdateTduPayloadTab();
				this.UpdateClientTab();
				this.ClientGenerateChecksumBtn.Enabled = true;
			}
			catch (Exception ex)
			{
				string errMessage = "Error reading MO Message file. " + ex.Message;

				MessageBox.Show(
								errMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);
			}
		}

		/// <summary>
		/// This method will update the client tab based on the file read.
		/// </summary>
		private void UpdateClientTab()
		{
			var epoch = new EpochDate();

			// Populate header section
			this.HeaderLengthTextBox.Text	= this.moMessageDo.HeaderLength.ToString();
			this.CdrReferenceTextBox.Text	= this.moMessageDo.CdrReference.ToString();
			this.ImeiTextBox.Text			= this.moMessageDo.Imei;
			this.MomsnTextBox.Text			= this.moMessageDo.Momsn.ToString();
			this.MtmsnTextBox.Text			= this.moMessageDo.Mtmsn.ToString();
			this.SessionStatusTextBox.Text	= this.moMessageDo.SessionStatus.ToString();
			this.SessionTimeTextBox.Text	= epoch.ConvertFromEpochDate(this.moMessageDo.TimeOfSession);

			// Populate location section
			if (this.moMessageDo.LatitudeLongitude != null)
			{
				this.LocationLengthTextBox.Text		= "7";
				this.FormatCodeTextBox.Text			= this.moMessageDo.LatitudeLongitude.FormatCode.ToString();
				this.ReservedTextBox.Text			= this.moMessageDo.LatitudeLongitude.Reserved.ToString();
				this.LatitudeTextBox.Text			= this.moMessageDo.LatitudeLongitude.LatitudeDouble.ToString(CultureInfo.InvariantCulture);
				this.LongitudeTextBox.Text			= this.moMessageDo.LatitudeLongitude.LongitudeDouble.ToString(CultureInfo.InvariantCulture);
				this.NorthIndicatorRadioBtn.Checked = this.moMessageDo.LatitudeLongitude.NorthSouthIndicator == 0 ? true : false;
				this.SouthIndicatorRadioBtn.Checked = this.moMessageDo.LatitudeLongitude.NorthSouthIndicator == 1 ? true : false;
				this.EastIndicatorRadioBtn.Checked	= this.moMessageDo.LatitudeLongitude.EastWestIndicator == 0 ? true : false;
				this.WestRadioIndicatorBtn.Checked	= this.moMessageDo.LatitudeLongitude.EastWestIndicator == 1 ? true : false;
				this.CepRadiusTextBox.Text			= this.moMessageDo.CepRadius.ToString();
			}
			else
			{
				this.LocationLengthTextBox.Text		= "0";
				this.FormatCodeTextBox.Text			= "0";
				this.ReservedTextBox.Text			= "0";
				this.LatitudeTextBox.Text			= string.Empty;
				this.LongitudeTextBox.Text			= string.Empty;
				this.NorthIndicatorRadioBtn.Checked = true;
				this.EastIndicatorRadioBtn.Checked	= true;
				this.CepRadiusTextBox.Text			= string.Empty;		
			}

			// Populate payload section
			int rowIndex = 0;
			foreach(PayloadDO payload in this.moMessageDo.MoPayload)
			{
				var byteNumberCell = new DataGridViewTextBoxCell { Value = payload.ByteNumber };
				var byteValueCell = new DataGridViewTextBoxCell { Value = payload.ByteHexValue };

				this.PayloadDataGridView.Rows.Add();
				this.PayloadDataGridView.Rows[rowIndex].Cells[0] = byteNumberCell;
				this.PayloadDataGridView.Rows[rowIndex].Cells[1] = byteValueCell;

				rowIndex++;
			}

			// Populate confirmation section
			this.ConfirmationMessageTextBox.Text = string.Empty;
			this.ConfirmationMessageBytesRecTextBox.Text = string.Empty;
		}

		/// <summary>
		/// This method will save the MO message to a file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ClientSaveBtnOnClick(object sender, EventArgs e)
		{
			// Update the data object with the info from the UI.
			this.UpdateMoMessageData();

			// Will open a save dialog and handle the OK button 
			// to save the data.
			this.OpenMoMessageSaveDialog();
		}

		/// <summary>
		/// This method will update the MO Message data object with the contents from
		/// the UI.
		/// </summary>
		private void UpdateMoMessageData()
		{
			if (moMessageDo.LatitudeLongitude == null)
			{
				this.moMessageDo.LatitudeLongitude = new CoordinateDO();
			}

			this.moMessageDo.LatitudeLongitude.NorthSouthIndicator = this.NorthIndicatorRadioBtn.Checked ? 0 : 1;
			double latitude;

			if (double.TryParse(this.LatitudeTextBox.Text, out latitude))
			{
				this.moMessageDo.LatitudeLongitude.LatitudeDouble = latitude;
			}

			this.moMessageDo.LatitudeLongitude.EastWestIndicator = this.EastIndicatorRadioBtn.Checked ? 0 : 1;
			double longitude;

			if (double.TryParse(this.LongitudeTextBox.Text, out longitude))
			{
				this.moMessageDo.LatitudeLongitude.LongitudeDouble = longitude;
			}

			uint cepRadius;
			if (uint.TryParse(this.CepRadiusTextBox.Text, out cepRadius))
			{
				this.moMessageDo.CepRadius = cepRadius;
			}

			this.moMessageDo.MoLocationInfoLength = 7;

			if (string.IsNullOrEmpty(this.LatitudeTextBox.Text) &&
				string.IsNullOrEmpty(this.LongitudeTextBox.Text) &&
				string.IsNullOrEmpty(this.CepRadiusTextBox.Text))
			{
				this.moMessageDo.MoLocationInfoLength = 0;
				this.moMessageDo.LatitudeLongitude = null;
				this.moMessageDo.CepRadius = null;
			}

			uint cdrReference;
			if (uint.TryParse(this.CdrReferenceTextBox.Text, out cdrReference))
			{
				this.moMessageDo.CdrReference = cdrReference;
			}

			this.moMessageDo.Imei = this.ImeiTextBox.Text;

			ushort momsn;
			if (ushort.TryParse(this.MomsnTextBox.Text, out momsn))
			{
				this.moMessageDo.Momsn = momsn;
			}

			ushort mtmsn;
			if (ushort.TryParse(this.MtmsnTextBox.Text, out mtmsn))
			{
				this.moMessageDo.Mtmsn = mtmsn;
			}

			ushort sessionStatus;
			if (ushort.TryParse(this.SessionStatusTextBox.Text, out sessionStatus))
			{
				this.moMessageDo.SessionStatus = sessionStatus;
			}

			var epoch = new EpochDate();
			uint? epochDateTime = epoch.ConvertToEpochDate(this.SessionTimeTextBox.Text);

			if (epochDateTime != null)
			{
				this.moMessageDo.TimeOfSession = epochDateTime.Value;
			}
		}

		/// <summary>
		/// This method will send the MO message to the server via TCP/IP.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ClientSendBtnOnClick(object sender, EventArgs e)
		{
			this.ConfirmationMessageBytesRecTextBox.Text = string.Empty;
			this.ConfirmationMessageTextBox.Text = string.Empty;

			if (this.configDo == null || this.configDo.FmPortNumber == null)
			{
				const string ErrMessage = "The simulator's configuration has not be setup or saved.";

				MessageBox.Show(
								ErrMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);

				return;
			}

			TcpClient tcpClient = null;
			NetworkStream stream = null;

			try
			{
				byte[] messageArray;

				try
				{
					// Update the data object with the info from the UI.
					this.UpdateMoMessageData();

					messageArray = this.moMessageDo.CreateMessage();
					this.HeaderLengthTextBox.Text = this.moMessageDo.HeaderLength.ToString();
				}
				catch (Exception ex)
				{
					string errMessage = ex.Message;

					MessageBox.Show(
									errMessage,
									"Error",
									MessageBoxButtons.OK,
									MessageBoxIcon.Error,
									MessageBoxDefaultButton.Button1);
					return;
				}
				
				this.eventLog.WriteEntry("Starting communication...", EventLogEntryType.Information);
				tcpClient = new TcpClient(this.configDo.FmIpAddress, this.configDo.FmPortNumber.Value);
				this.eventLog.WriteEntry("Connected to the FM Iridium GSS Listener Service.", EventLogEntryType.Information);

				// Get a client stream for reading and writing. 
				// Stream stream = client.GetStream();
				stream = tcpClient.GetStream();

				// Send the message to the connected TcpServer. 
				stream.Write(messageArray, 0, messageArray.Length);
				this.eventLog.WriteEntry("Message was written to the FM Iridium GSS Listener Service.", EventLogEntryType.Information);

				// Read response from the server
				this.eventLog.WriteEntry("Waiting on confirmation message from FM Iridium GSS Listener Service.", EventLogEntryType.Information);
				var confirmationMessage = new byte[4];
				int byteCount = stream.Read(confirmationMessage, 0, confirmationMessage.Length);
				int messageStatus = confirmationMessage[3];

				this.ConfirmationMessageTextBox.Text = messageStatus == 1 ? "Message was successfully received." : "Message failed by receiver.";
				this.ConfirmationMessageBytesRecTextBox.Text = byteCount.ToString();

				// Close everything.
				stream.Flush();
				stream.Close();
				tcpClient.Close();
			}
			catch (Exception ex)
			{
				// Close everything.
				if (stream != null)
				{
					stream.Flush();
					stream.Close();
				}

				if (tcpClient != null)
				{
					tcpClient.Close();
				}

				string errMessage = "Error in TCP Client: " + ex.Message;

				MessageBox.Show(
								errMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);
			}
		}

		/// <summary>
		/// This method will handle Convert Button on click event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FpConvertBtnOnClick(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(this.FloatingPointTextbox.Text))
			{
				return;
			}

			float fpNumberOut;

			if (float.TryParse(this.FloatingPointTextbox.Text, out fpNumberOut))
			{
				string mantissaHex;
				string decimalUpperHex;
				string decimalLowerHex;
				int error = this.ConvertCoordinateToHex(fpNumberOut, out mantissaHex, out decimalUpperHex, out decimalLowerHex);

				if (error == 0)
				{
					this.FPByte1Textbox.Text = mantissaHex;
					this.FPByte2Textbox.Text = decimalUpperHex;
					this.FPByte3Textbox.Text = decimalLowerHex;
				}
				else if (error == 1)
				{
					const string ErrMessage = "Decimal portion must be less than 59,999";

					MessageBox.Show(
									ErrMessage,
									"Error",
									MessageBoxButtons.OK,
									MessageBoxIcon.Error,
									MessageBoxDefaultButton.Button1);
				}
				else if (error == 2)
				{
					const string ErrMessage = "Mantissa must be less than 90 for latitude and 180 for longitude.";

					MessageBox.Show(
									ErrMessage,
									"Error",
									MessageBoxButtons.OK,
									MessageBoxIcon.Error,
									MessageBoxDefaultButton.Button1);
				}
			}
			else
			{
				this.FPByte1Textbox.Text = string.Empty;
				this.FPByte2Textbox.Text = string.Empty;
				this.FPByte3Textbox.Text = string.Empty;

				const string ErrMessage = "Must be a floating point number.";

				MessageBox.Show(
								ErrMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);
			}
		}

		/// <summary>
		/// This method will convert the floating point value to an Iridium coordinate hex values.
		/// </summary>
		/// <param name="fpNumber">The floating point number to convert.</param>
		/// <param name="mantissaHex">The mantissa hex value</param>
		/// <param name="decimalUpperHex">The decimal upper hex value</param>
		/// <param name="decimalLowerHex">The decimal lower hex value</param>
		private int ConvertCoordinateToHex(float fpNumber, out string mantissaHex, out string decimalUpperHex, out string decimalLowerHex)
		{
			const double MinuteThousands = 60000;
			mantissaHex		= "0x00";
			decimalUpperHex = "0x00";
			decimalLowerHex = "0x00";

			string fpNumberStr = fpNumber.ToString(CultureInfo.InvariantCulture);
			string[] parts = fpNumberStr.Split('.');

			if (parts.Length > 0 && parts.Length >= 1)
			{
				var mantissa = Math.Abs(int.Parse(parts[0]));

				if (mantissa > 180)
				{
					return 2;
				}

				mantissaHex = PayloadDO.ConvertByteToHexString((byte)mantissa);
			}

			if (parts.Length > 0 && parts.Length == 2)
			{

				var decimalPartFloat = double.Parse("." + parts[1]);
				uint thousandMinutes = (uint)(decimalPartFloat * MinuteThousands);

				if (thousandMinutes > 59999)
				{
					return 1;
				}

				uint upperMask = (uint)(thousandMinutes & 0xff00);
				byte upper = (byte)(upperMask >> 8);
				byte lower = (byte)(thousandMinutes & 0x00ff);

				decimalUpperHex = PayloadDO.ConvertByteToHexString(upper);
				decimalLowerHex = PayloadDO.ConvertByteToHexString(lower);
			}

			return 0;
		}

		/// <summary>
		/// This method will handle the coordinate indicator North radio button
		/// on change event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CoorNorthRbOnChange(object sender, EventArgs e)
		{
			string indicatorHex = "0x";

			if (this.CoorNorthRB.Checked)
			{
				this.CoorSouthRB.Checked = false;
			}

			indicatorHex = indicatorHex + (this.CoorNorthRB.Checked ? "0" : "1");
			indicatorHex = indicatorHex + (this.CoorEastRB.Checked ? "0" : "1");

			this.IndicatorTB.Text = indicatorHex;
		}

		/// <summary>
		/// This method will handle the coordinate indicator South radio button
		/// on change event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CoorSouthRbOnChange(object sender, EventArgs e)
		{
			string indicatorHex = "0x";

			if (this.CoorSouthRB.Checked)
			{
				this.CoorNorthRB.Checked = false;
			}

			indicatorHex = indicatorHex + (this.CoorNorthRB.Checked ? "0" : "1");
			indicatorHex = indicatorHex + (this.CoorEastRB.Checked ? "0" : "1");

			this.IndicatorTB.Text = indicatorHex;
		}

		/// <summary>
		/// This method will handle the coordinate indicator East radio button
		/// on change event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CoorEastRbOnChange(object sender, EventArgs e)
		{
			string indicatorHex = "0x";

			if (this.CoorEastRB.Checked)
			{
				this.CoorWestRB.Checked = false;
			}

			indicatorHex = indicatorHex + (this.CoorNorthRB.Checked ? "0" : "1");
			indicatorHex = indicatorHex + (this.CoorEastRB.Checked ? "0" : "1");

			this.IndicatorTB.Text = indicatorHex;
		}

		/// <summary>
		/// This method will handle the coordinate indicator West radio button
		/// on change event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CoorWestRbOnChange(object sender, EventArgs e)
		{
			string indicatorHex = "0x";

			if (this.CoorWestRB.Checked)
			{
				this.CoorEastRB.Checked = false;
			}

			indicatorHex = indicatorHex + (this.CoorNorthRB.Checked ? "0" : "1");
			indicatorHex = indicatorHex + (this.CoorEastRB.Checked ? "0" : "1");

			this.IndicatorTB.Text = indicatorHex;
		}

		/// <summary>
		/// This method handles the Generate Checksum button event.  It will generate a checksum
		/// based on the bytes in the payload array.  There must at least one byte. Normally,
		/// the payload consists of the following:
		/// Prefix > one byte
		/// GPS data > 7 bytes
		/// Tank ID > 1 byte
		/// Tank data > 12 bytes.
		/// There can be up to 4 tanks.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ClientGenerateChecksumBtnOnClick(object sender, EventArgs e)
		{
			if (this.moMessageDo.MoPayload.Count < 1)
			{
				return;
			}

			ushort checksum = 0;
			int byteNumber = this.moMessageDo.MoPayload.Count;

			foreach (PayloadDO payloadDo in this.moMessageDo.MoPayload)
			{
				checksum += (ushort)payloadDo.RealValue;
			}

			byte upper = (byte)((checksum & 0xff00) >> 8);
			byte lower = (byte)(checksum & 0x00ff);

			var upperDo = new PayloadDO { RealValue = upper, ByteNumber = byteNumber };
			var lowerDo = new PayloadDO { RealValue = lower, ByteNumber = ++byteNumber };

			this.moMessageDo.MoPayload.Add(upperDo);
			this.moMessageDo.MoPayload.Add(lowerDo);

			this.UpdateClientTab();
		}

		/// <summary>
		/// This method will convert the byte to a byte string representation
		/// of the data. It will return the collection.
		/// </summary>
		/// <param name="rawData">The raw data to covert.</param>
		/// <returns>Return the raw data string collection representation of the byte.</returns>
		private List<string> ConvertBytesToStringRepresentation(byte[] rawData)
		{
			var convertedByteList = new List<string>();

			foreach (byte rawDataByte in rawData)
			{
				string s1 = (rawDataByte & 0x01).ToString();
				string s2 = ((rawDataByte & 0x02) >> 1).ToString();
				string s3 = ((rawDataByte & 0x04) >> 2).ToString();
				string s4 = ((rawDataByte & 0x08) >> 3).ToString();
				string s5 = ((rawDataByte & 0x10) >> 4).ToString();
				string s6 = ((rawDataByte & 0x20) >> 5).ToString();
				string s7 = ((rawDataByte & 0x40) >> 6).ToString();
				string s8 = ((rawDataByte & 0x80) >> 7).ToString();
				string rawDataByteStr = s8 + s7 + s6 + s5 + s4 + s3 + s2 + s1;

				convertedByteList.Add(rawDataByteStr);
			}

			return convertedByteList;
		}
		#endregion

		#region File Dialog Events
		/// <summary>
		/// This method will open a save dialog for the MO message.
		/// </summary>
		private void OpenMoMessageSaveDialog()
		{
			if (this.saveMoMessageFileDialog == null)
			{
				this.saveMoMessageFileDialog = new SaveFileDialog();
				this.saveMoMessageFileDialog.FileOk += this.SaveMoMessageFileDialogOkEvent;
			}

			DateTimeOffset currentTime = DateTimeOffset.UtcNow;

			this.saveMoMessageFileDialog.FileName = "MoMessage_"
													+ currentTime.Year
													+ currentTime.Month
													+ currentTime.Day
													+ "_"
													+ currentTime.Hour
													+ currentTime.Minute
													+ currentTime.Second
													+ ".xml";
			this.saveMoMessageFileDialog.ShowDialog();
		}

		/// <summary>
		/// This method will handle the OK event on the save dialog.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SaveMoMessageFileDialogOkEvent(object sender, EventArgs e)
		{
			FileHandler fileHandler = new FileHandler();
			fileHandler.SaveMoMessageToFile(this.saveMoMessageFileDialog, this.moMessageDo);
		}
		#endregion
	}
}
