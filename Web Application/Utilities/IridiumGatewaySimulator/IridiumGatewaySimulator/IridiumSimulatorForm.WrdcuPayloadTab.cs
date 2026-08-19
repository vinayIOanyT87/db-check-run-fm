namespace IridiumGatewaySimulator
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;

	using DataObjects.DataObjects;

	using FMBusinessObjects.Parsers;

	public partial class IridiumSimulatorForm
	{
		#region WRDCU Payload tab code
		private void InitializeWrdcuPayloadTab()
		{
			this.WpLatitudeTb.Text = string.Empty;
			this.WpLongitudeTb.Text = string.Empty;

			this.WpVolume1Tb.Text = string.Empty;
			this.WpLevel1Tb.Text = string.Empty;
			this.WpDielectric1Tb.Text = string.Empty;

			this.WpVolume2Tb.Text = string.Empty;
			this.WpLevel2Tb.Text = string.Empty;
			this.WpDielectric2Tb.Text = string.Empty;

			this.WpVolume3Tb.Text = string.Empty;
			this.WpLevel3Tb.Text = string.Empty;
			this.WpDielectric3Tb.Text = string.Empty;

			this.WpVolume4Tb.Text = string.Empty;
			this.WpLevel4Tb.Text = string.Empty;
			this.WpDielectric4Tb.Text = string.Empty;

			this.WpUpdatePayloadBtn.Enabled = false;
		}

		/// <summary>
		/// This method will enable the update payload button if the 
		/// coordinates and tank 1 fields are populated.
		/// </summary>
		private void EnablePayloadUpdateButton()
		{
			this.WpUpdatePayloadBtn.Enabled = false;

			if (string.IsNullOrEmpty(this.WpLatitudeTb.Text) == false
				&& string.IsNullOrEmpty(this.WpLongitudeTb.Text) == false
				&& string.IsNullOrEmpty(this.WpVolume1Tb.Text) == false
				&& string.IsNullOrEmpty(this.WpLevel1Tb.Text) == false
				&& string.IsNullOrEmpty(this.WpDielectric1Tb.Text) == false)
			{
				this.WpUpdatePayloadBtn.Enabled = true;
			}
		}

		/// <summary>
		/// This method will handle the WRDCU text box on change event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void WpTextboxOnTextChange(object sender, EventArgs e)
		{
			this.EnablePayloadUpdateButton();
		}

		/// <summary>
		/// This method will handle the update payload button click.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void WpUpdatePayloadBtnOnClick(object sender, EventArgs e)
		{
			bool areTank2Populated = this.AreFieldsPopulated(this.WpLevel2Tb.Text, this.WpVolume2Tb.Text, this.WpDielectric2Tb.Text);
			bool areTank3Populated = this.AreFieldsPopulated(this.WpLevel3Tb.Text, this.WpVolume3Tb.Text, this.WpDielectric3Tb.Text);
			bool areTank4Populated = this.AreFieldsPopulated(this.WpLevel4Tb.Text, this.WpVolume4Tb.Text, this.WpDielectric4Tb.Text);

			if (areTank2Populated == false)
			{
				const string ErrMessage = "Not all the field for Tank 2 are populated.";

				MessageBox.Show(
								ErrMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);
				return;
			}

			if (areTank3Populated == false)
			{
				const string ErrMessage = "Not all the field for Tank 3 are populated.";

				MessageBox.Show(
								ErrMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);
				return;
			}

			if (areTank4Populated == false)
			{
				const string ErrMessage = "Not all the field for Tank 4 are populated.";

				MessageBox.Show(
								ErrMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);
				return;
			}

			bool validData = this.WpValidateFields();

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

			this.WpPopulateClientPayload();
		}

		/// <summary>
		/// This method will populate the Client tab payload data section.
		/// </summary>
		private void WpPopulateClientPayload()
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

			// WRDCU index byte.
			var payloadDo = new PayloadDO { ByteNumber = byteNumber++, ByteHexValue = "0x02" };
			this.moMessageDo.MoPayload.Add(payloadDo);

			// Coordinate bytes
			float latitude = float.Parse(this.WpLatitudeTb.Text);
			float longitude = float.Parse(this.WpLongitudeTb.Text);
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

			payloadDo = new PayloadDO { ByteNumber = byteNumber++, ByteHexValue = decimalLowerHex };
			this.moMessageDo.MoPayload.Add(payloadDo);

			// Tank 1
			var tankBoxList = new List<TextBox> { this.WpLevel1Tb, this.WpVolume1Tb, this.WpDielectric1Tb };
			this.LoadTankPayload("0x01", tankBoxList, ref byteNumber);

			// Tank 2
			if (string.IsNullOrEmpty(this.WpLevel2Tb.Text) == false
				&& string.IsNullOrEmpty(this.WpVolume2Tb.Text) == false
				&& string.IsNullOrEmpty(this.WpDielectric2Tb.Text) == false)
			{
				tankBoxList = new List<TextBox> { this.WpLevel2Tb, this.WpVolume2Tb, this.WpDielectric2Tb };
				this.LoadTankPayload("0x02", tankBoxList, ref byteNumber);
			}

			// Tank 3
			if (string.IsNullOrEmpty(this.WpLevel3Tb.Text) == false
				&& string.IsNullOrEmpty(this.WpVolume3Tb.Text) == false
				&& string.IsNullOrEmpty(this.WpDielectric3Tb.Text) == false)
			{
				tankBoxList = new List<TextBox> { this.WpLevel3Tb, this.WpVolume3Tb, this.WpDielectric3Tb };
				this.LoadTankPayload("0x03", tankBoxList, ref byteNumber);
			}

			// Tank 4
			if (string.IsNullOrEmpty(this.WpLevel4Tb.Text) == false
				&& string.IsNullOrEmpty(this.WpVolume4Tb.Text) == false
				&& string.IsNullOrEmpty(this.WpDielectric4Tb.Text) == false)
			{
				tankBoxList = new List<TextBox> { this.WpLevel4Tb, this.WpVolume4Tb, this.WpDielectric4Tb };
				this.LoadTankPayload("0x04", tankBoxList, ref byteNumber);
			}

			// Add checksum. NOTE: the checksum method will call the Update Client tab!
			this.ClientGenerateChecksumBtnOnClick(null, null);

			// Navigate to the client tab.
			this.ClientTab.Show();
			this.tabControl1.SelectedTab = this.ClientTab;
		}

		/// <summary>
		/// This method will load the tank payload data.  The text box list must be in the following
		/// order: Level, Volume, and Dielectric.
		/// </summary>
		/// <param name="tankId"></param>
		/// <param name="textBoxList"></param>
		/// <param name="byteNumber"></param>
		private void LoadTankPayload(string tankId, List<TextBox> textBoxList, ref int byteNumber)
		{
			var payloadDo = new PayloadDO { ByteNumber = byteNumber++, ByteHexValue = tankId };
			this.moMessageDo.MoPayload.Add(payloadDo);

			foreach (TextBox textBoxObj in textBoxList)
			{
				List<string> hexValues = this.GetIeeeBytes(textBoxObj.Text);

				payloadDo = new PayloadDO { ByteNumber = byteNumber++, ByteHexValue = hexValues[0] };
				this.moMessageDo.MoPayload.Add(payloadDo);

				payloadDo = new PayloadDO { ByteNumber = byteNumber++, ByteHexValue = hexValues[1] };
				this.moMessageDo.MoPayload.Add(payloadDo);

				payloadDo = new PayloadDO { ByteNumber = byteNumber++, ByteHexValue = hexValues[2] };
				this.moMessageDo.MoPayload.Add(payloadDo);

				payloadDo = new PayloadDO { ByteNumber = byteNumber++, ByteHexValue = hexValues[3] };
				this.moMessageDo.MoPayload.Add(payloadDo);
			}
		}

		/// <summary>
		/// This method will convert a float to IEEE formatted hex value.
		/// </summary>
		/// <param name="numberStr"></param>
		/// <returns></returns>
		private List<string> GetIeeeBytes(string numberStr)
		{
			float floatValue = float.Parse(numberStr);
			var bytes = BitConverter.GetBytes(floatValue);
			var integerValue = BitConverter.ToInt32(bytes, 0);

			string byteStr = integerValue.ToString("X8");
			var byteList = new List<string>
						   {
							   "0x" + byteStr.Substring(0, 2),
							   "0x" + byteStr.Substring(2, 2),
							   "0x" + byteStr.Substring(4, 2),
							   "0x" + byteStr.Substring(6, 2)
						   };

			return byteList;
		}
		/// <summary>
		/// This method will return true if a set of fields are all populated or
		/// all not populated. Otherwise false is returned.
		/// </summary>
		/// <param name="textValue1"></param>
		/// <param name="textValue2"></param>
		/// <param name="textValue3"></param>
		/// <returns></returns>
		private bool AreFieldsPopulated(string textValue1, string textValue2, string textValue3)
		{
			bool allEmpty = string.IsNullOrEmpty(textValue1)
							&& string.IsNullOrEmpty(textValue2)
							&& string.IsNullOrEmpty(textValue3);

			if (allEmpty)
			{
				return true;
			}

			bool oneEmpty = string.IsNullOrEmpty(textValue1) == false
							&& string.IsNullOrEmpty(textValue2) == false
							&& string.IsNullOrEmpty(textValue3) == false;

			if (oneEmpty)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// This method will ensure the fields are populated with numeric values.
		/// </summary>
		/// <returns></returns>
		private bool WpValidateFields()
		{
			double fieldValueOut;

			if (double.TryParse(this.WpLatitudeTb.Text, out fieldValueOut) == false)
			{
				return false;
			}

			if (double.TryParse(this.WpLongitudeTb.Text, out fieldValueOut) == false)
			{
				return false;
			}

			if (double.TryParse(this.WpLevel1Tb.Text, out fieldValueOut) == false)
			{
				return false;
			}

			if (double.TryParse(this.WpVolume1Tb.Text, out fieldValueOut) == false)
			{
				return false;
			}

			if (double.TryParse(this.WpDielectric1Tb.Text, out fieldValueOut) == false)
			{
				return false;
			}

			if (string.IsNullOrEmpty(this.WpLevel2Tb.Text) == false
				&& string.IsNullOrEmpty(this.WpVolume2Tb.Text) == false
				&& string.IsNullOrEmpty(this.WpDielectric2Tb.Text) == false)
			{
				if (double.TryParse(this.WpLevel2Tb.Text, out fieldValueOut) == false)
				{
					return false;
				}

				if (double.TryParse(this.WpVolume2Tb.Text, out fieldValueOut) == false)
				{
					return false;
				}

				if (double.TryParse(this.WpDielectric2Tb.Text, out fieldValueOut) == false)
				{
					return false;
				}
			}

			if (string.IsNullOrEmpty(this.WpLevel3Tb.Text) == false
				&& string.IsNullOrEmpty(this.WpVolume3Tb.Text) == false
				&& string.IsNullOrEmpty(this.WpDielectric3Tb.Text) == false)
			{
				if (double.TryParse(this.WpLevel3Tb.Text, out fieldValueOut) == false)
				{
					return false;
				}

				if (double.TryParse(this.WpVolume3Tb.Text, out fieldValueOut) == false)
				{
					return false;
				}

				if (double.TryParse(this.WpDielectric3Tb.Text, out fieldValueOut) == false)
				{
					return false;
				}
			}

			if (string.IsNullOrEmpty(this.WpLevel4Tb.Text) == false
				&& string.IsNullOrEmpty(this.WpVolume4Tb.Text) == false
				&& string.IsNullOrEmpty(this.WpDielectric4Tb.Text) == false)
			{
				if (double.TryParse(this.WpLevel4Tb.Text, out fieldValueOut) == false)
				{
					return false;
				}

				if (double.TryParse(this.WpVolume4Tb.Text, out fieldValueOut) == false)
				{
					return false;
				}

				if (double.TryParse(this.WpDielectric4Tb.Text, out fieldValueOut) == false)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// This method will clear all the WRDCU fields.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void WpClearBtnOnClick(object sender, EventArgs e)
		{
			this.InitializeWrdcuPayloadTab();
		}

		/// <summary>
		/// This method is called by the file open to populate the WRDCU tab.
		/// </summary>
		private void UpdateWrdcuPayloadTab()
		{
			if (this.moMessageDo.MoPayload == null || this.moMessageDo.MoPayload.Count == 0)
			{
				return;
			}

			this.WpClearBtnOnClick(null, null);

			var wrdcuParser = new IridiumWrdcuPayloadParser();
			var payloadByteList = new List<byte>();

			foreach (PayloadDO payload in this.moMessageDo.MoPayload)
			{
				payloadByteList.Add(payload.RealValue);
			}

			wrdcuParser.Parse(payloadByteList.ToArray());

			if (wrdcuParser.HasWrdcuData)
			{
				double latitude = wrdcuParser.Latitude;
				double longitude = wrdcuParser.Longitude;

				if (wrdcuParser.NorthSouthIndicator == 1)
				{
					latitude = latitude * -1;
				}

				if (wrdcuParser.EastWestIndicator == 0)
				{
					longitude = longitude * -1;
				}

				this.WpLatitudeTb.Text = latitude.ToString();
				this.WpLongitudeTb.Text = longitude.ToString();
			}

			if (wrdcuParser.HasWrdcuData)
			{
				if (wrdcuParser.WrdcuTankList == null || wrdcuParser.WrdcuTankList.Count == 0)
				{
					return;
				}

				int tankCount = 1;
				foreach (WrdcuData tank in wrdcuParser.WrdcuTankList)
				{
					if (tankCount == 1)
					{
						this.WpLevel1Tb.Text = tank.Level.ToString();
						this.WpVolume1Tb.Text = tank.Volume.ToString();
						this.WpDielectric1Tb.Text = tank.Dielectric.ToString();
					}

					if (tankCount == 2)
					{
						this.WpLevel2Tb.Text = tank.Level.ToString();
						this.WpVolume2Tb.Text = tank.Volume.ToString();
						this.WpDielectric2Tb.Text = tank.Dielectric.ToString();
					}

					if (tankCount == 3)
					{
						this.WpLevel3Tb.Text = tank.Level.ToString();
						this.WpVolume3Tb.Text = tank.Volume.ToString();
						this.WpDielectric3Tb.Text = tank.Dielectric.ToString();
					}

					if (tankCount == 4)
					{
						this.WpLevel4Tb.Text = tank.Level.ToString();
						this.WpVolume4Tb.Text = tank.Volume.ToString();
						this.WpDielectric4Tb.Text = tank.Dielectric.ToString();
					}

					tankCount++;
				}
			}
		}
		#endregion

	}
}
