namespace TransactionFields
{
	using System;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemOperatorFG.
	/// </summary>
	public class LineItemOperatorFG : OperatorTextButtonGenerator, ILineItemField
	{
		#region Override properties

		public override string FieldID { get { return "LineItem OperatorID"; } }

		/// <summary>
		/// This property returns the field's maximum column width.
		/// </summary>
		protected override short MaxColumns
		{
			get { return OperatorTextButtonGenerator.FIELD_LENGTH; } 
		}

		protected override void SetOperatorID(string newID)
		{
			if (this.lineItem != null)
			{
				this.lineItem.OperatorID = newID;
			}
		}

		protected override void SetOperatorGuid(Guid newGuid)
		{
			if (this.lineItem != null)
			{
				this.lineItem.OperatorPersonnelGuid = newGuid;
			}
		}

		protected override void SetSignature(byte[] Signature){}

		protected override void SetOperatorName(string operatorName){}

		protected override bool AutoPostBack
		{
			get
			{
				return false;
			}
		}
		
		#endregion



		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			return lineItem.OperatorID;
		}

		public string GetDataText(LineItemDO lineItem)
		{
			if (GetDataValue(lineItem) != null)
			{
				return GetDataValue(lineItem).ToString();
			}
			else
			{
				return null;
			}
		}

		public void SetDataValue(LineItemDO lineItem, object newValue)
		{
			this.lineItem = lineItem;
			this.SetValue(newValue);
		}

		#endregion
	}
}
