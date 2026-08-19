namespace TransactionFields
{
	using System;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	class DestinationEquipmentModelFG : TextFieldGenerator, IHeaderField
	{
		private readonly byte equipmentNumber;

		#region Contructors
		public DestinationEquipmentModelFG(byte equipmentNumber)
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
			get { return "DestinationEquipmentModel" + this.equipmentNumber; }
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 20.
		/// </summary>
		protected override short MaxColumns
		{
			get { return this.GetFieldLength(FieldID, 20); }
		}

		public override bool Editable
		{
			get { return true; }
			set { }
		}

		public void SetValue(string newValue)
		{
			TextBox textBox = null;
			var updatePanel = this.cell.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				updatePanel.Update();
				textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;
			}

			if (textBox != null)
			{
				textBox.Text = newValue;
				SpecializeControl(cell);
			}
		}

		protected override void SpecializeControl(WebControl control)
		{
			TextBox textBox = null;
			var updatePanel = control.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;

				if (textBox != null)
				{
					bool readOnly = false;
					if (!transContext.aliasClass.PermitNonReferenceData)
					{
						readOnly = true;
					}
					else
					{

						switch (equipmentNumber)
						{
							case 1:
								readOnly = (this.trans.DestinationEQ1.EquipmentGuid != Guid.Empty);
								break;
							case 2:
								readOnly = (this.trans.DestinationEQ2.EquipmentGuid != Guid.Empty);
								break;
							case 3:
								readOnly = (this.trans.DestinationEQ3.EquipmentGuid != Guid.Empty);
								break;
						}
					}

					textBox.ReadOnly = readOnly;
					if (readOnly)
					{
						textBox.BackColor = VarecBkgrndReadOnlyGray;
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
					return transaction.DestinationEQ1.EquipmentModel;
				case 2:
					return transaction.DestinationEQ2.EquipmentModel;
				case 3:
					return transaction.DestinationEQ3.EquipmentModel;
			}
			return null;
		}

		public string GetDataText(TransactionDO transaction)
		{
			switch (equipmentNumber)
			{
				case 1:
					return transaction.DestinationEQ1.EquipmentModel;
				case 2:
					return transaction.DestinationEQ2.EquipmentModel;
				case 3:
					return transaction.DestinationEQ3.EquipmentModel;
			}
			return null;
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			switch (equipmentNumber)
			{
				case 1:
					transaction.DestinationEQ1.EquipmentModel = newValue as string;
					break;
				case 2:
					transaction.DestinationEQ2.EquipmentModel = newValue as string;
					break;
				case 3:
					transaction.DestinationEQ3.EquipmentModel = newValue as string;
					break;
			}

			OnFieldChanged();
		}
		#endregion
	}
}
