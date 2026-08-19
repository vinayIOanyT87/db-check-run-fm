namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemCOAIDFG.
	/// </summary>
	public class LineItemCOAIDFG : TextFieldGenerator, ILineItemField, ISublineItemField
	{
		public LineItemCOAIDFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem COAID";
			}
		}

		public override bool Editable
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 40.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, 40);
			}
		}

		#region ILineItemField Members

		public object GetDataValue(LineItemDO inLineItem)
		{
			return inLineItem.COAID;
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
			inLineItem.COAID = newValue as string;
			OnFieldChanged();
		}
		#endregion

		#region ISublineItemField Members
		public object GetDataValue(SubLineItemDO inSublineItem)
		{
			return inSublineItem.COAID;
		}

		public string GetDataText(SubLineItemDO inSublineItem)
		{
			if (GetDataValue(inSublineItem) != null)
			{
				return GetDataValue(inSublineItem).ToString();
			}

			return null;
		}

		public void SetDataValue(SubLineItemDO inSublineItem, object newValue)
		{
			inSublineItem.COAID = newValue as string;
			OnFieldChanged();
		}
		#endregion
	}
}
