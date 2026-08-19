namespace TransactionFields
{
	using System;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	class SourceEquipmentModelFG : TextFieldGenerator, IHeaderField
	{
		private readonly byte equipmentNumber;

		#region Contructors
		public SourceEquipmentModelFG(byte equipmentNumber)
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
				return "SourceEquipmentModel" + this.equipmentNumber;
			}
		}

		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, 20);
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
				updatePanel.Update();
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
					bool readOnly = false;

					if (!this.transContext.aliasClass.PermitNonReferenceData)
					{
						readOnly = true;
					}
					else
					{

						switch (this.equipmentNumber)
						{
							case 1:
								readOnly = (this.trans.SourceEQ1.EquipmentGuid != Guid.Empty);
								break;
							case 2:
								readOnly = (this.trans.SourceEQ2.EquipmentGuid != Guid.Empty);
								break;
							case 3:
								readOnly = (this.trans.SourceEQ3.EquipmentGuid != Guid.Empty);
								break;
						}
					}

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
					return transaction.SourceEQ1.EquipmentModel;
				case 2:
					return transaction.SourceEQ2.EquipmentModel;
				case 3:
					return transaction.SourceEQ3.EquipmentModel;
			}

			return null;
		}

		public string GetDataText(TransactionDO transaction)
		{
			switch (equipmentNumber)
			{
				case 1:
					return transaction.SourceEQ1.EquipmentModel;
				case 2:
					return transaction.SourceEQ2.EquipmentModel;
				case 3:
					return transaction.SourceEQ3.EquipmentModel;
			}

			return null;
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			switch (equipmentNumber)
			{
				case 1:
					transaction.SourceEQ1.EquipmentModel = newValue as string;
					break;
				case 2:
					transaction.SourceEQ2.EquipmentModel = newValue as string;
					break;
				case 3:
					transaction.SourceEQ3.EquipmentModel = newValue as string;
					break;
			}

			OnFieldChanged();
		}
		#endregion
	}
}
