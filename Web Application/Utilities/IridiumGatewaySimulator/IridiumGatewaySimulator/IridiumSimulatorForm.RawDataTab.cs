namespace IridiumGatewaySimulator
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;

	using DataObjects.DataObjects;
	using DataObjects.Handlers;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Parsers;

	public partial class IridiumSimulatorForm
	{

		#region Raw Data tab code
		/// <summary>
		/// This method will initialize the raw data tab to its initial state.
		/// </summary>
		private void InitializeRawDataTab()
		{
			this.RawDataClearBtnOnClick(null, null);
			this.RawDataLoadBtn.Enabled = false;
		}

		/// <summary>
		/// This method will clear the raw data collection and the 
		/// raw data view text box.
		/// </summary>
		/// <param name="sender">The calling module.</param>
		/// <param name="e">Event arguments.</param>
		private void RawDataClearBtnOnClick(object sender, EventArgs e)
		{
			this.rawDataCollection = null;
			this.RawDataViewTextBox.Text = string.Empty;
			this.RawDataLoadBtn.Enabled = false;
		}

		/// <summary>
		/// This method will handle the open raw data file event.
		/// It will open the file and read the data.
		/// </summary>
		/// <param name="sender">The calling module.</param>
		/// <param name="e">Event arguments.</param>
		private void RawDataOpenBtnOnClick(object sender, EventArgs e)
		{
			// Create an instance of the open file dialog box.
			if (this.openRawDataFileDialog == null)
			{
				this.openRawDataFileDialog = new OpenFileDialog();
			}

			// Set filter options and filter index.
			this.openRawDataFileDialog.Filter = "Txt Files (.txt)|*.txt";
			this.openRawDataFileDialog.FilterIndex = 1;

			this.openRawDataFileDialog.Multiselect = false;

			// Call the ShowDialog method to show the dialog box.
			this.openRawDataFileDialog.ShowDialog();

			// If the user did not selected a file, then just return.
			if (string.IsNullOrEmpty(this.openRawDataFileDialog.FileName))
			{
				return;
			}

			try
			{
				this.RawDataClearBtnOnClick(null, null);

				var fileHandler = new FileHandler();
				this.rawDataCollection = fileHandler.ReadRawData(this.openRawDataFileDialog);
				this.UpdateRawDataView();
			}
			catch (Exception ex)
			{
				string errMessage = "Error reading Raw Data file. " + ex.Message;

				MessageBox.Show(
								errMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);
			}
		}

		/// <summary>
		/// This method will handle the raw data load button event.
		/// It will parse the data and load the client tab with the
		/// data.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RawDataLoadBtnOnClick(object sender, EventArgs e)
		{
			try
			{
				var byteArray = new byte[this.rawDataCollection.Count];
				int byteCount = 0;

				foreach (RawDataDO rawDataDo in this.rawDataCollection)
				{
					byteArray[byteCount] = rawDataDo.RawDataByte;
					byteCount++;
				}

				var parser = new IridiumMessageParser();
				parser.Parse(byteArray);

				// Enable the Generater Checksum button if there is payload data.
				if (parser.TduPayloadParser.HasTduData || parser.WrdcuPayloadParser.HasWrdcuData)
				{
					this.ClientGenerateChecksumBtn.Enabled = true;
				}

				var coordinate = new CoordinateDO
				{
					Reserved = parser.Reserved,
					FormatCode = parser.FormatCode,
					LatitudeDouble = parser.Latitude ?? 0.0,
					LongitudeDouble = parser.Longitude ?? 0.0,
					NorthSouthIndicator = parser.NorthSouthIndicator,
					EastWestIndicator = parser.EastWestIndicator
				};

				var epochConverter = new EpochDate();
				uint? epochDateTime = null;

				if (parser.SessionDateTime != null)
				{
					epochDateTime = epochConverter.ConvertToEpochDate(parser.SessionDateTime.Value);
				}

				this.moMessageDo = new MobileOriginatedMessageDO
				{
					CdrReference = parser.CdrReference,
					Imei = parser.Imei,
					SessionStatus = (ushort)parser.SessionStatus,
					Momsn = parser.Momsn,
					Mtmsn = parser.Mtmsn,
					CepRadius = parser.CepRadius,
					LatitudeLongitude = coordinate
				};

				if (epochDateTime != null)
				{
					this.moMessageDo.TimeOfSession = epochDateTime.Value;
				}

				if (parser.AssetTrackingPayloadCollection != null && parser.AssetTrackingPayloadCollection.Count > 0)
				{
					this.moMessageDo.MoPayload = new List<PayloadDO>();

					foreach (AssetTrackingPayloadClass payload in parser.AssetTrackingPayloadCollection)
					{
						var payloadDo = new PayloadDO { RealValue = (byte)payload.PayloadValue, ByteNumber = payload.ByteNumber };
						this.moMessageDo.MoPayload.Add(payloadDo);
					}
				}

				// Populate the client tab with the new data.
				this.UpdateClientTab();
				this.UpdateWrdcuPayloadTab();
				this.UpdatePositionPayloadTab();
				this.UpdateTduPayloadTab();
				this.ClientTab.Show();
				this.tabControl1.SelectedTab = this.ClientTab;
			}
			catch (Exception ex)
			{
				string errMessage = "Error loading raw data. " + ex.Message;

				MessageBox.Show(
								errMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);

			}

			this.RawDataLoadBtn.Enabled = false;
		}

		/// <summary>
		/// This method will load the raw data view text box with the 
		/// raw data.
		/// </summary>
		private void UpdateRawDataView()
		{
			foreach (RawDataDO rawDataDo in this.rawDataCollection)
			{
				this.RawDataViewTextBox.AppendText(rawDataDo.RawDataRecord + "\n");
			}

			this.RawDataLoadBtn.Enabled = true;
		}
		#endregion

	}
}
