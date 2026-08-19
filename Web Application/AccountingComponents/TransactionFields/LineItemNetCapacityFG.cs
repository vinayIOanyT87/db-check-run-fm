namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemNetCapacityFG.
	/// </summary>
	public class LineItemNetCapacityFG : NumericTextFieldGenerator, ILineItemField, ISublineItemField
	{
		public LineItemNetCapacityFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem NetCapacity";
			}
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 15.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, 15);
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
				return SITE_VARIABLE_TYPE.VOLUME;
			}
		}

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			return inLineItem.NetCapacity;
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
			if ((newValue == null) || newValue.Equals(""))
			{
				inLineItem.NetCapacity = null;
			}
			else
			{
				inLineItem.NetCapacity = (double) newValue;
			}
		}
		#endregion

		#region ISublineItemField Members
		object ISublineItemField.GetDataValue(SubLineItemDO inSublineItem)
		{
			return inSublineItem.NetCapacity;
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
			if ((newValue == null) || newValue.Equals(""))
			{
				inSublineItem.NetCapacity = null;
			}
			else
			{
				inSublineItem.NetCapacity = (double) newValue;
			}
		}
		#endregion
	}
}
