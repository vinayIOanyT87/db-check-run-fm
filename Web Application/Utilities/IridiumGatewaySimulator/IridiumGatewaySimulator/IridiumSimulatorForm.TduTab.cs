namespace IridiumGatewaySimulator
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;

	using DataObjects.DataObjects;

	using FMBusinessObjects.Parsers;

	public partial class IridiumSimulatorForm
	{
		private List<TduDO> tduTankList;
		private int selectedRowIndex = -99;

		private void InitializeTduTab()
		{
			this.TduClearBtn.Enabled			= true;
			this.TduAddBtn.Enabled				= true;
			this.TduUpdateBtn.Enabled			= false;
			this.TduRemoveBtn.Enabled			= false;
			this.TduUpdatePayloadBtn.Enabled	= false;

			this.TduTankIdTb.Text		= string.Empty;
			this.TduVolumeTb.Text		= string.Empty;
			this.TduPressureTb.Text		= string.Empty;
			this.TduTemperatureTb.Text	= string.Empty;

			foreach (DataGridViewRow row in this.TduGrid.Rows)
			{
				if (row.IsNewRow == false)
				{
					this.TduGrid.Rows.Remove(row);
				}
			}

			this.tduTankList = new List<TduDO>();
			this.selectedRowIndex = -99;
		}

	/// <summary>
	/// This method will clear the TDCU text fields.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void TduClearBtnOnClick(object sender, EventArgs e)
		{
			this.TduClearBtn.Enabled			= true;
			this.TduAddBtn.Enabled				= true;
			this.TduUpdateBtn.Enabled			= false;
			this.TduRemoveBtn.Enabled			= false;
			this.TduUpdatePayloadBtn.Enabled	= false;

			this.TduTankIdTb.Text		= string.Empty;
			this.TduVolumeTb.Text		= string.Empty;
			this.TduPressureTb.Text		= string.Empty;
			this.TduTemperatureTb.Text	= string.Empty;

			this.selectedRowIndex = -99;

            if (this.tduTankList.Count > 0)
            {
                this.TduUpdatePayloadBtn.Enabled = true;
            }
        }

		/// <summary>
		/// This method will add a new TDU tank to the grid.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TduAddBtnOnClick(object sender, EventArgs e)
		{
			this.selectedRowIndex = -99;
			this.TduUpdatePayloadBtn.Enabled = false;

			if (string.IsNullOrEmpty(this.TduTankIdTb.Text)
				&& string.IsNullOrEmpty(this.TduVolumeTb.Text)
				&& string.IsNullOrEmpty(this.TduTemperatureTb.Text)
				&& string.IsNullOrEmpty(this.TduPressureTb.Text))
			{
				const string ErrMessage = "Must populate all fields.";

				MessageBox.Show(
								ErrMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);
				return;
			}

			bool isValid = this.ValidateFields(checkTankIdUniqueness: true);

			if (isValid)
			{
				string nextRowId = "0";
				if (this.tduTankList != null && this.tduTankList.Count > 0)
				{
					nextRowId = this.tduTankList.Count.ToString();
				}

				var newTduDo = new TduDO
				               {
					               TankId			= this.TduTankIdTb.Text,
					               VolumeStr		= this.TduVolumeTb.Text,
					               TemperatureStr	= this.TduTemperatureTb.Text,
					               PressureStr		= this.TduPressureTb.Text,
								   RowIdStr			= nextRowId
								};

				if (this.tduTankList == null)
				{
					this.tduTankList = new List<TduDO>();
				}

				this.tduTankList.Add(newTduDo);

				int rowIndex = this.tduTankList.Count - 1;
				this.TduGrid.Rows.Add();
				this.TduGrid.Rows[rowIndex].Cells[0] = new DataGridViewTextBoxCell { Value = newTduDo.RowIdStr };
				this.TduGrid.Rows[rowIndex].Cells[1] = new DataGridViewTextBoxCell { Value = newTduDo.TankId };
				this.TduGrid.Rows[rowIndex].Cells[2] = new DataGridViewTextBoxCell { Value = newTduDo.VolumeStr };
				this.TduGrid.Rows[rowIndex].Cells[3] = new DataGridViewTextBoxCell { Value = newTduDo.TemperatureStr };
				this.TduGrid.Rows[rowIndex].Cells[4] = new DataGridViewTextBoxCell { Value = newTduDo.PressureStr };

				this.TduTankIdTb.Text = string.Empty;
				this.TduVolumeTb.Text = string.Empty;
				this.TduPressureTb.Text = string.Empty;
				this.TduTemperatureTb.Text = string.Empty;

				if (this.tduTankList.Count > 0)
				{
					this.TduUpdatePayloadBtn.Enabled = true;
				}
			}
		}

		/// <summary>
		/// This method will validate the entry fields.
		/// </summary>
		/// <returns>Returns true if okay.</returns>
		private bool ValidateFields(bool checkTankIdUniqueness)
		{
			int tankIdOut;
			if (int.TryParse(this.TduTankIdTb.Text, out tankIdOut) == false)
			{
				const string ErrMessage = "Tank ID must be an integer.";

				MessageBox.Show(
								ErrMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);

				return false;
			}

			if (tankIdOut > 255 || tankIdOut < 0)
			{
				const string ErrMessage = "Tank ID must be an integer between 0 and 255.";

				MessageBox.Show(
								ErrMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);

				return false;

			}

			if (this.tduTankList != null && this.tduTankList.Count > 1 && checkTankIdUniqueness)
			{
				var foundTdu = this.tduTankList.Find(x => x.TankId == this.TduTankIdTb.Text.Trim());
				if (foundTdu != null)
				{
					const string ErrMessage = "Tank ID be unique.";

					MessageBox.Show(
									ErrMessage,
									"Error",
									MessageBoxButtons.OK,
									MessageBoxIcon.Error,
									MessageBoxDefaultButton.Button1);

					return false;
				}
			}

			double outValue;
			if (double.TryParse(this.TduVolumeTb.Text, out outValue) == false
				|| double.TryParse(this.TduTemperatureTb.Text, out outValue) == false
				|| double.TryParse(this.TduPressureTb.Text, out outValue) == false)
			{
				const string ErrMessage = "Values must be numeric.";

				MessageBox.Show(
								ErrMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);

				return false;
			}

			return true;
		}

		/// <summary>
		/// This method will handle the item selected event on the grid.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TduGridOnClick(object sender, EventArgs e)
		{
			int rowIndex = -99;

			foreach (DataGridViewRow row in this.TduGrid.Rows)
			{
				if (row.Selected)
				{
					try
					{
						rowIndex = int.Parse((string)row.Cells[0].Value);
						this.selectedRowIndex = rowIndex;
					}
					catch (Exception)
					{
						this.selectedRowIndex = -99;
						this.TduAddBtn.Enabled = true;
						this.TduUpdateBtn.Enabled = false;
						this.TduRemoveBtn.Enabled = false;

						this.TduTankIdTb.Text = string.Empty;
						this.TduVolumeTb.Text = string.Empty;
						this.TduPressureTb.Text = string.Empty;
						this.TduTemperatureTb.Text = string.Empty;

						return;
					}

					break;
				}
			}

			if (rowIndex >= 0)
			{
				this.TduUpdateBtn.Enabled = true;
				this.TduRemoveBtn.Enabled = true;
				this.TduAddBtn.Enabled = false;

				this.TduTankIdTb.Text = this.tduTankList[rowIndex].TankId;
				this.TduVolumeTb.Text = this.tduTankList[rowIndex].VolumeStr;
				this.TduTemperatureTb.Text = this.tduTankList[rowIndex].TemperatureStr;
				this.TduPressureTb.Text = this.tduTankList[rowIndex].PressureStr;
			}
		}

		/// <summary>
		/// This method will handle the update button event. It will update an item
		/// in the grid.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TduUpdateBtnOnClick(object sender, EventArgs e)
		{
            this.TduUpdatePayloadBtn.Enabled = false;

            if (string.IsNullOrEmpty(this.TduTankIdTb.Text)
				&& string.IsNullOrEmpty(this.TduVolumeTb.Text)
				&& string.IsNullOrEmpty(this.TduTemperatureTb.Text)
				&& string.IsNullOrEmpty(this.TduPressureTb.Text))
			{
				const string ErrMessage = "Must populate all fields.";

				MessageBox.Show(
								ErrMessage,
								"Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error,
								MessageBoxDefaultButton.Button1);
				return;
			}

			bool isValid = this.ValidateFields(checkTankIdUniqueness: false);

			if (isValid)
			{
				this.tduTankList[this.selectedRowIndex].TankId			= this.TduTankIdTb.Text;
				this.tduTankList[this.selectedRowIndex].VolumeStr		= this.TduVolumeTb.Text;
				this.tduTankList[this.selectedRowIndex].TemperatureStr	= this.TduTemperatureTb.Text;
				this.tduTankList[this.selectedRowIndex].PressureStr		= this.TduPressureTb.Text;

				this.TduGrid.Rows[this.selectedRowIndex].Cells[1].Value = this.TduTankIdTb.Text;
				this.TduGrid.Rows[this.selectedRowIndex].Cells[2].Value = this.TduVolumeTb.Text;
				this.TduGrid.Rows[this.selectedRowIndex].Cells[3].Value = this.TduTemperatureTb.Text;
				this.TduGrid.Rows[this.selectedRowIndex].Cells[4].Value = this.TduPressureTb.Text;

				this.selectedRowIndex		= -99;
				this.TduTankIdTb.Text		= string.Empty;
				this.TduVolumeTb.Text		= string.Empty;
				this.TduPressureTb.Text		= string.Empty;
				this.TduTemperatureTb.Text	= string.Empty;

				this.TduUpdateBtn.Enabled = false;
				this.TduRemoveBtn.Enabled = false;
				this.TduAddBtn.Enabled = true;

                if (this.tduTankList.Count > 0)
                {
                    this.TduUpdatePayloadBtn.Enabled = true;
                }
            }
		}

		/// <summary>
		/// This method will handle the remove button event. It will remove an item
		/// from the grid.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TduRemoveBtnOnClick(object sender, EventArgs e)
		{
			int rowCount = 0;
			this.tduTankList.RemoveAt(this.selectedRowIndex);

			foreach (TduDO tduDo in this.tduTankList)
			{
				tduDo.RowIdStr = rowCount.ToString();
				rowCount++;
			}

			foreach (DataGridViewRow row in this.TduGrid.Rows)
			{
				if (row.IsNewRow == false)
				{
					this.TduGrid.Rows.Remove(row);
				}
			}

			int rowIndex = 0;
			foreach (TduDO tduDo in this.tduTankList)
			{
				this.TduGrid.Rows.Add();
				this.TduGrid.Rows[rowIndex].Cells[0] = new DataGridViewTextBoxCell { Value = tduDo.RowIdStr };
				this.TduGrid.Rows[rowIndex].Cells[1] = new DataGridViewTextBoxCell { Value = tduDo.TankId };
				this.TduGrid.Rows[rowIndex].Cells[2] = new DataGridViewTextBoxCell { Value = tduDo.VolumeStr };
				this.TduGrid.Rows[rowIndex].Cells[3] = new DataGridViewTextBoxCell { Value = tduDo.TemperatureStr };
				this.TduGrid.Rows[rowIndex].Cells[4] = new DataGridViewTextBoxCell { Value = tduDo.PressureStr };

				rowIndex++;
			}

			this.selectedRowIndex		= -99;
			this.TduTankIdTb.Text		= string.Empty;
			this.TduVolumeTb.Text		= string.Empty;
			this.TduPressureTb.Text		= string.Empty;
			this.TduTemperatureTb.Text	= string.Empty;

			this.TduUpdateBtn.Enabled = false;
			this.TduRemoveBtn.Enabled = false;
			this.TduAddBtn.Enabled = true;

			if (this.tduTankList.Count > 0)
			{
				this.TduUpdateBtn.Enabled = true;
			}

            this.TduUpdatePayloadBtn.Enabled = false;
            if (this.tduTankList.Count > 0)
            {
                this.TduUpdatePayloadBtn.Enabled = true;
            }
        }

		/// <summary>
		/// This method will handle the Update Payload button event. It will populate the 
		/// payload section on the client tab.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TduUpdatePayloadBtnOnClick(object sender, EventArgs e)
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

			// TDU index byte.
			var payloadDo = new PayloadDO { ByteNumber = byteNumber++, ByteHexValue = "0x01" };
			this.moMessageDo.MoPayload.Add(payloadDo);

			foreach (TduDO tduDo in this.tduTankList)
			{
				List<string> pressureHexValues		= this.GetIeeeBytes(tduDo.PressureStr);
				List<string> temperatureHexValues	= this.GetIeeeBytes(tduDo.TemperatureStr);
				List<string> volumeHexValues		= this.GetIeeeBytes(tduDo.VolumeStr);

				int intValue = int.Parse(tduDo.TankId);
				var bytes = BitConverter.GetBytes(intValue);
				var integerValue = BitConverter.ToInt16(bytes, 0);

				string tankIdHexStr = "0x" + integerValue.ToString("X2");

				payloadDo = new PayloadDO { ByteNumber = byteNumber++, ByteHexValue = tankIdHexStr };
				this.moMessageDo.MoPayload.Add(payloadDo);

				// Must load the payload section in the order of pressure, temperature, and volume.
				foreach (string hexStr in pressureHexValues)
				{
					payloadDo = new PayloadDO { ByteNumber = byteNumber++, ByteHexValue = hexStr };
					this.moMessageDo.MoPayload.Add(payloadDo);
				}

				foreach (string hexStr in temperatureHexValues)
				{
					payloadDo = new PayloadDO { ByteNumber = byteNumber++, ByteHexValue = hexStr };
					this.moMessageDo.MoPayload.Add(payloadDo);
				}

				foreach (string hexStr in volumeHexValues)
				{
					payloadDo = new PayloadDO { ByteNumber = byteNumber++, ByteHexValue = hexStr };
					this.moMessageDo.MoPayload.Add(payloadDo);
				}
			}

			// Add checksum. NOTE: the checksum method will call the Update Client tab!
			this.ClientGenerateChecksumBtnOnClick(null, null);

			// Navigate to the client tab.
			this.ClientTab.Show();
			this.tabControl1.SelectedTab = this.ClientTab;
		}

		private void UpdateTduPayloadTab()
		{
			if (this.moMessageDo.MoPayload == null || this.moMessageDo.MoPayload.Count == 0)
			{
				return;
			}

			this.TduClearBtnOnClick(null, null);

			var tduParser = new IridiumTduPayloadParser();
			var payloadByteList = new List<byte>();

			foreach (PayloadDO payload in this.moMessageDo.MoPayload)
			{
				payloadByteList.Add(payload.RealValue);
			}

			tduParser.Parse(payloadByteList.ToArray());
			int rowIndex = 0;

			if (tduParser.HasTduData && tduParser.TduTankList != null && tduParser.TduTankList.Count > 0)
			{
				this.tduTankList.Clear();

				foreach (TduData tank in tduParser.TduTankList)
				{
					var newTduDo = new TduDO
							{
								TankId			= tank.TankConfigurationNumber.ToString(),
								VolumeStr		= tank.Volume.ToString(),
								TemperatureStr	= tank.Temperature.ToString(),
								PressureStr		= tank.Pressure.ToString(),
								RowIdStr		= rowIndex.ToString()
							};

					this.tduTankList.Add(newTduDo);

					this.TduGrid.Rows.Add();
					this.TduGrid.Rows[rowIndex].Cells[0] = new DataGridViewTextBoxCell { Value = newTduDo.RowIdStr };
					this.TduGrid.Rows[rowIndex].Cells[1] = new DataGridViewTextBoxCell { Value = newTduDo.TankId };
					this.TduGrid.Rows[rowIndex].Cells[2] = new DataGridViewTextBoxCell { Value = newTduDo.VolumeStr };
					this.TduGrid.Rows[rowIndex].Cells[3] = new DataGridViewTextBoxCell { Value = newTduDo.TemperatureStr };
					this.TduGrid.Rows[rowIndex].Cells[4] = new DataGridViewTextBoxCell { Value = newTduDo.PressureStr };

					this.TduTankIdTb.Text		= string.Empty;
					this.TduVolumeTb.Text		= string.Empty;
					this.TduPressureTb.Text		= string.Empty;
					this.TduTemperatureTb.Text	= string.Empty;

					rowIndex++;

					if (this.tduTankList.Count > 0)
					{
						this.TduUpdatePayloadBtn.Enabled = true;
					}
				}
			}
		}
	}
}
