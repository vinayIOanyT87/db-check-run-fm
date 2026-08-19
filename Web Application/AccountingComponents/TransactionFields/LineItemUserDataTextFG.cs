namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemUserDataTextFG.
	/// 
	/// 06-27-2008	V. Thompson			Created for line item user data
	/// </summary>
	public class LineItemUserDataTextFG : TextFieldGenerator, ILineItemField
	{
		protected string key;

		public LineItemUserDataTextFG(string key)
		{
			this.key = key;
		}

		public override string FieldID
		{
			get
			{
				return key;
			}
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 60.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(this.FieldID, 60);
			}
		}

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			if (inLineItem.UserData.ContainsKey(key))
			{
				return inLineItem.UserData[key];
			}

			return null;
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
			inLineItem.UserData[key] = string.Format("{0}", newValue);
			OnFieldChanged();
		}
		#endregion
	}
}
