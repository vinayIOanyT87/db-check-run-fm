namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	public abstract class NumericTextFieldGenerator : TextFieldGenerator
	{
		public NumericTextFieldGenerator()
		{
			this.ManualValueFlag = null;
		}

		public bool? ManualValueFlag { get; set; }

		public enum ENumericType
		{
			Integer, Double
		};

		public abstract ENumericType NumericType
		{
			get;
		}

		public abstract SITE_VARIABLE_TYPE UnitType
		{
			get;
		}

		public override void Generate(bool editable)
		{
			base.Generate(editable);
			var updatePanel = this.cell.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;

				if (textBox != null)
				{
					textBox.Text = this.GetFormattedValue();
					textBox.Columns = this.GetTextBoxSize();

					var hiddenField = new HiddenField();
					this.cell.Controls.Add(hiddenField);

					if (this.ManualValueFlag == false)
					{
						textBox.ForeColor = System.Drawing.Color.DarkGray;
					}

					hiddenField.ID = "Hidden" + textBox.ID;
					hiddenField.Value = "false";
				}
			}
		}

		public void SetNewValue()
		{
			var updatePanel = this.cell.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;

				if (textBox != null)
				{
					textBox.Text = this.GetFormattedValue();

					if (this.ManualValueFlag == false)
					{
						textBox.ForeColor = System.Drawing.Color.DarkGray;
					}
					else
					{
						textBox.ForeColor = System.Drawing.Color.Black;
					}
				}
			}
		}

		public override object GetNewValue(WebControl control)
		{
			var stringValue = base.GetNewValue(control) as string;

			if (string.IsNullOrEmpty(stringValue))
			{
				return null;
			}

			object newValue = null;

			try
			{
				switch (NumericType)
				{
					case ENumericType.Integer:
						newValue = GetInt(stringValue);
						break;
					case ENumericType.Double:
						newValue = GetDouble(stringValue);
						break;
				}
			}
			catch (System.FormatException)
			{
				string message = this.displayName + " is not numeric.";
				throw new RetrieveException(message);
			}

			return newValue;
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 13.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, 13);
			}
		}

		protected int GetInt(string stringValue)
		{
			return int.Parse(stringValue, this.fieldGenerator.accountingSite.CurrentSite.GetNumberFormatInfo(UnitType));
		}

		protected double GetDouble(string stringValue)
		{
			return double.Parse(stringValue, this.fieldGenerator.accountingSite.CurrentSite.GetNumberFormatInfo(UnitType));
		}

		/// <summary>
		/// This method will return the text box size. It will divide the maximum number of
		/// characters by 2 (some cultures place commas between every two digits) and add it
		/// to the total number of characters. This will make the text box large enough for
		/// numbers and commas.
		/// </summary>
		/// <returns></returns>
		protected int GetTextBoxSize()
		{
			int fieldSize = MaxColumns;

			if (MaxColumns >= 2)
			{
				int numberOfCommas = (MaxColumns / 2) - 1;
				fieldSize = MaxColumns + numberOfCommas;
			}

			return fieldSize;
		}

		override public string GetFormattedValue()
		{
			object dataValue = GetDataValue();

			if ((dataValue == null) || dataValue.Equals(string.Empty))
			{
				return string.Empty;
			}
			else if (dataValue is double)
			{
				return ((double) dataValue).ToString("N", this.fieldGenerator.accountingSite.CurrentSite.GetNumberFormatInfo(this.UnitType));
			}
			else if (dataValue is int)
			{
				return ((int) dataValue).ToString("G",
					this.fieldGenerator.accountingSite.CurrentSite.GetNumberFormatInfo(this.UnitType));
			}
			else if (dataValue is long)
			{
				return ((long) dataValue).ToString("G",
					this.fieldGenerator.accountingSite.CurrentSite.GetNumberFormatInfo(this.UnitType));
			}

			return null;
		}
	}
}
