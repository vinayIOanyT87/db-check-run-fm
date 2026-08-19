namespace IridiumGatewaySimulator
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Windows.Forms;

	using DataObjects.DataObjects;

	using FMBusinessObjects.Parsers;

	public partial class IridiumSimulatorForm
	{
		#region Position Payload tab code
		private void InitializePositionPayloadTab()
		{
			this.PosLatitudeTb.Text = string.Empty;
			this.PosLongitudeTb.Text = string.Empty;

			this.PosUpdatePayloadBtn.Enabled = false;
		}

		/// <summary>
		/// This method will enable the update payload button if the 
		/// coordinates and tank 1 fields are populated.
		/// </summary>
		private void EnablePositionPayloadUpdateButton()
		{
			this.PosUpdatePayloadBtn.Enabled = false;

			if (string.IsNullOrEmpty(this.PosLatitudeTb.Text) == false
				&& string.IsNullOrEmpty(this.PosLongitudeTb.Text) == false)
			{
				this.PosUpdatePayloadBtn.Enabled = true;
			}
		}

		/// <summary>
		/// This method will handle the position text box on change event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PositionTextboxOnTextChange(object sender, EventArgs e)
		{
			this.EnablePositionPayloadUpdateButton();
		}

		/// <summary>
		/// This method will handle the update payload button click.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PositionUpdatePayloadBtnOnClick(object sender, EventArgs e)
		{
			bool validData = this.PositionValidateFields();

			if (validData == false)
			{
				const string ErrMessage = "All fields must be numeric.";

				MessageBox.Show(
								ErrMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);
				return;
			}

			this.PositionPopulateClientPayload();
		}

		/// <summary>
		/// This method will populate the Client tab payload data section.
		/// </summary>
		private void PositionPopulateClientPayload()
		{
			if (this.moMessageDo == null)
			{
				return;
			}

			// Populate payload section
			int byteNumber = 0;

			if (this.moMessageDo.MoPayload == null)
			{
				this.moMessageDo.MoPayload = new List<PayloadDO>();
			}

			this.moMessageDo.MoPayload.Clear();

			// Position Only index byte.
			var payloadDo = new PayloadDO { ByteNumber = byteNumber++, ByteHexValue = "0x00" };
			this.moMessageDo.MoPayload.Add(payloadDo);

			// Coordinate bytes
			float latitude = float.Parse(this.PosLatitudeTb.Text);
			float longitude = float.Parse(this.PosLongitudeTb.Text);
			string direction = "0x00";

			if (latitude >= 0.0 && longitude < 0.0)
			{
				direction = "0x01";
			}

			if (latitude < 0.0 && longitude < 0.0)
			{
				direction = "0x11";
			}

			if (latitude < 0.0 && longitude >= 0.0)
			{
				direction = "0x10";
			}

			payloadDo = new PayloadDO { ByteNumber = byteNumber++, ByteHexValue = direction };
			this.moMessageDo.MoPayload.Add(payloadDo);

			string mantissaHex;
			string decimalUpperHex;
			string decimalLowerHex;

			// Latitude
			this.ConvertCoordinateToHex(latitude, out mantissaHex, out decimalUpperHex, out decimalLowerHex);
			payloadDo = new PayloadDO { ByteNumber = byteNumber++, ByteHexValue = mantissaHex };
			this.moMessageDo.MoPayload.Add(payloadDo);

			payloadDo = new PayloadDO { ByteNumber = byteNumber++, ByteHexValue = decimalUpperHex };
			this.moMessageDo.MoPayload.Add(payloadDo);

			payloadDo = new PayloadDO { ByteNumber = byteNumber++, ByteHexValue = decimalLowerHex };
			this.moMessageDo.MoPayload.Add(payloadDo);

			// Longitude
			this.ConvertCoordinateToHex(longitude, out mantissaHex, out decimalUpperHex, out decimalLowerHex);
			payloadDo = new PayloadDO { ByteNumber = byteNumber++, ByteHexValue = mantissaHex };
			this.moMessageDo.MoPayload.Add(payloadDo);

			payloadDo = new PayloadDO { ByteNumber = byteNumber++, ByteHexValue = decimalUpperHex };
			this.moMessageDo.MoPayload.Add(payloadDo);

			payloadDo = new PayloadDO { ByteNumber = byteNumber, ByteHexValue = decimalLowerHex };
			this.moMessageDo.MoPayload.Add(payloadDo);

			// Add checksum. NOTE: the checksum method will call the Update Client tab!
			this.ClientGenerateChecksumBtnOnClick(null, null);

			// Navigate to the client tab.
			this.ClientTab.Show();
			this.tabControl1.SelectedTab = this.ClientTab;
		}

		/// <summary>
		/// This method will ensure the fields are populated with numeric values.
		/// </summary>
		/// <returns></returns>
		private bool PositionValidateFields()
		{
			double fieldValueOut;

			if (double.TryParse(this.PosLatitudeTb.Text, out fieldValueOut) == false)
			{
				return false;
			}

			if (double.TryParse(this.PosLongitudeTb.Text, out fieldValueOut) == false)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// This method will clear all the position fields.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PositionClearBtnOnClick(object sender, EventArgs e)
		{
			this.InitializePositionPayloadTab();
		}

		/// <summary>
		/// This method is called by the file open to populate the Position tab.
		/// </summary>
		private void UpdatePositionPayloadTab()
		{
			if (this.moMessageDo.MoPayload == null || this.moMessageDo.MoPayload.Count == 0)
			{
				return;
			}

			this.PositionClearBtnOnClick(null, null);

			var positionParser = new IridiumPositionPayloadParser();
			var payloadByteList = new List<byte>();

			foreach (PayloadDO payload in this.moMessageDo.MoPayload)
			{
				payloadByteList.Add(payload.RealValue);
			}

			positionParser.Parse(payloadByteList.ToArray());

			if (positionParser.HasCoordinateData)
			{
				double latitude = positionParser.Latitude;
				double longitude = positionParser.Longitude;

				if (positionParser.NorthSouthIndicator == 1)
				{
					latitude = latitude * -1;
				}

				if (positionParser.EastWestIndicator == 0)
				{
					longitude = longitude * -1;
				}

				this.PosLatitudeTb.Text = latitude.ToString(CultureInfo.InvariantCulture);
				this.PosLongitudeTb.Text = longitude.ToString(CultureInfo.InvariantCulture);
			}
		}
		#endregion

	}
}

