namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemTemperatureFG.
	/// </summary>
	public class LineItemTemperatureFG : NumericTextFieldGenerator, ILineItemField, ISublineItemField
	{
		public LineItemTemperatureFG()
		{
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 5.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, 5);
			}
		}

		public override string FieldID
		{
			get
			{
				return "LineItem Temperature";
			}
		}

		public override ENumericType NumericType
		{
			get
			{
				return ENumericType.Double;
			}
		}

		public override SITE_VARIABLE_TYPE UnitType
		{
			get
			{
				return SITE_VARIABLE_TYPE.TEMPERATURE;
			}
		}

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			return inLineItem.Temperature;
		}

		public string GetDataText(LineItemDO inLineItem)
		{
			if (GetDataValue(inLineItem) != null)
			{
				return GetDataValue(inLineItem).ToString();
			}
			
			return null;
		}

		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			if (newValue == null || newValue.Equals(string.Empty))
			{
				inLineItem.Temperature = null;
			}
			else
			{
				inLineItem.Temperature = (double) newValue;
			}

            if (base.cell != null)
            {
                System.Web.UI.WebControls.TextBox temperatureTextBox = base.cell.Controls[0].Controls[0].Controls[0] as System.Web.UI.WebControls.TextBox;
                if (temperatureTextBox != null)
                    temperatureTextBox.Text = GetFormattedValue();
            }

            OnFieldChanged();
		}
		#endregion

		#region ISublineItemField Members
		object ISublineItemField.GetDataValue(SubLineItemDO inSublineItem)
		{
			return inSublineItem.Temperature;
		}

		string ISublineItemField.GetDataText(SubLineItemDO inSublineItem)
		{
			if (((ISublineItemField) this).GetDataValue(inSublineItem) != null)
			{
				return ((ISublineItemField) this).GetDataValue(inSublineItem).ToString();
			}
			
			return null;
		}

		void ISublineItemField.SetDataValue(SubLineItemDO inSublineItem, object newValue)
		{
			if ((newValue == null) || (newValue.Equals(string.Empty)))
			{
				inSublineItem.Temperature = null;
			}
			else
			{
				inSublineItem.Temperature = (double) newValue;
			}
			
			OnFieldChanged();
		}
		#endregion
	}
}
