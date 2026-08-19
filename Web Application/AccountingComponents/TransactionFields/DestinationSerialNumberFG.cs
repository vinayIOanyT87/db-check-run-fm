namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	class DestinationSerialNumberFG : TextFieldGenerator, IHeaderField
	{
		private readonly byte equipmentNumber;

		#region Contructors
		public DestinationSerialNumberFG(byte equipmentNumber)
		{
			this.equipmentNumber = equipmentNumber;
		}
		#endregion

		protected override string ID
		{
			get { return base.ID + this.equipmentNumber; }
		}

		public override string FieldID
		{
			get { return "DestinationSerialNumber" + this.equipmentNumber; }
		}

		protected override short MaxColumns
		{
			get { return 10; }
		}

		public override bool Editable
		{
			get { return true; }
			set { }

		}

		public void SetValue(string newValue)
		{
			var updatePanel = this.cell.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				updatePanel.Update();
				var textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;

				if (textBox != null)
				{
					textBox.Text = newValue;
					SpecializeControl(cell);
				}
			}
		}

		protected override void SpecializeControl(WebControl control)
		{
			var updatePanel = control.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;

				if (textBox != null)
				{
					bool readOnly = !transContext.aliasClass.PermitNonReferenceData;

					textBox.ReadOnly = readOnly;
					if (readOnly)
						textBox.BackColor = VarecBkgrndReadOnlyGray;
					else
						textBox.BackColor = System.Drawing.Color.White;
				}
			}
		}


		#region IHeaderField Members
		public object GetDataValue(TransactionDO transaction)
		{
			switch (equipmentNumber)
			{
				case 1:
					return transaction.DestinationEQ1.SerialNumber;
				case 2:
					return transaction.DestinationEQ2.SerialNumber;
				case 3:
					return transaction.DestinationEQ3.SerialNumber;
			}
			return null;
		}

		public string GetDataText(TransactionDO transaction)
		{
			switch (equipmentNumber)
			{
				case 1:
					return transaction.DestinationEQ1.SerialNumber;
				case 2:
					return transaction.DestinationEQ2.SerialNumber;
				case 3:
					return transaction.DestinationEQ3.SerialNumber;
			}
			return null;
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			switch (equipmentNumber)
			{
				case 1:
					transaction.DestinationEQ1.SerialNumber = newValue as string;
					break;
				case 2:
					transaction.DestinationEQ2.SerialNumber = newValue as string;
					break;
				case 3:
					transaction.DestinationEQ3.SerialNumber = newValue as string;
					break;
			}

			OnFieldChanged();
		}
		#endregion
	}
}
