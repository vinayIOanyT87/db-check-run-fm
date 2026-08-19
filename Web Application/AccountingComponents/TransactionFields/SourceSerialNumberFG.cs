namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	class SourceSerialNumberFG : TextFieldGenerator, IHeaderField
	{
		readonly byte equipmentNumber;

		#region Contructors
		public SourceSerialNumberFG(byte equipmentNumber)
		{
			this.equipmentNumber = equipmentNumber;
		}
		#endregion

		protected override string ID
		{
			get
			{
				return base.ID + this.equipmentNumber;
			}
		}

		public override string FieldID
		{
			get
			{
				return "SourceSerialNumber" + this.equipmentNumber;
			}
		}

		protected override short MaxColumns
		{
			get
			{
				return 10;
			}
		}

		public override bool Editable
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		public void SetValue(string newValue)
		{
			var updatePanel = this.cell.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;

				if (textBox != null)
				{
					textBox.Text = newValue;
					this.SpecializeControl(this.cell);
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
					bool readOnly = !this.transContext.aliasClass.PermitNonReferenceData;
					textBox.ReadOnly = readOnly;

					if (readOnly)
					{
						textBox.BackColor = this.VarecBkgrndReadOnlyGray;
					}
					else
					{
						textBox.BackColor = System.Drawing.Color.White;
					}
				}
			}
		}

		#region IHeaderField Members
		public object GetDataValue(TransactionDO transaction)
		{
			switch (equipmentNumber)
			{
				case 1:
					return transaction.SourceEQ1.SerialNumber;
				case 2:
					return transaction.SourceEQ2.SerialNumber;
				case 3:
					return transaction.SourceEQ3.SerialNumber;
			}

			return null;
		}

		public string GetDataText(TransactionDO transaction)
		{
			switch (equipmentNumber)
			{
				case 1:
					return transaction.SourceEQ1.SerialNumber;
				case 2:
					return transaction.SourceEQ2.SerialNumber;
				case 3:
					return transaction.SourceEQ3.SerialNumber;
			}

			return null;
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			switch (equipmentNumber)
			{
				case 1:
					transaction.SourceEQ1.SerialNumber = newValue as string;
					break;
				case 2:
					transaction.SourceEQ2.SerialNumber = newValue as string;
					break;
				case 3:
					transaction.SourceEQ3.SerialNumber = newValue as string;
					break;
			}

			OnFieldChanged();
		}
		#endregion
	}
}
