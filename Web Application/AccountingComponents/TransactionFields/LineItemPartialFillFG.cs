using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemFlag01FG.
	/// Author: Van Thompson
	/// Created for ADF requirements for generic flag fields on line/sub-line items
	/// </summary>
	public class LineItemPartialFillFG : CheckBoxGenerator,ILineItemField
	{
		public LineItemPartialFillFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem PartialFill";
			}
		}

		public override bool Editable
		{
			get
			{
				return (trans.Status == TransactionStatus.Completed
				|| trans.Status == TransactionStatus.Posted) ? false : true;
			}
			set
			{
				base.Editable = value;
			}
		}

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			if(lineItem.PartialFill == null)
				return null;

			return lineItem.PartialFill.Value;
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

		public void SetDataValue(LineItemDO lineItem,object newValue)
		{
			if(newValue == null)
				lineItem.PartialFill=null;
			else	
				lineItem.PartialFill = new bool?((bool)newValue);
			OnFieldChanged();
		}

		#endregion

	}
}
