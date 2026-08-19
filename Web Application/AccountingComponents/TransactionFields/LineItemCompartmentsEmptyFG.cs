using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemCompartmentsEmptyFG.
	/// </summary>
	public class LineItemCompartmentsEmptyFG : CheckBoxGenerator, ILineItemField
	{
		public LineItemCompartmentsEmptyFG()
		{
		}
		public override string FieldID
		{ get { return "LineItem CompartmentsEmpty"; } }

		public override bool Editable
		{ get { return false; }}

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			if (lineItem.CompartmentsEmpty == null)
				return false;

			return lineItem.CompartmentsEmpty.Value;
		}

		public string GetDataText(LineItemDO lineItem)
		{
			if(lineItem.CompartmentsEmpty != null)
				return lineItem.CompartmentsEmpty.Value.ToString();
			else
				return "";
		}

		public void SetDataValue(LineItemDO lineItem, object newValue)
		{
			lineItem.CompartmentsEmpty=(bool?) newValue;
			OnFieldChanged();
		}

		#endregion
	}
}
