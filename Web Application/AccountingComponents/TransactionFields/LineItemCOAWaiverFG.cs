using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemCOAWaiverFG.
	/// </summary>
	public class LineItemCOAWaiverFG : CheckBoxGenerator, ILineItemField
	{
		public LineItemCOAWaiverFG()
		{
		}

		public override string FieldID
		{ get { return "LineItem COAWaiver"; } }

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			return lineItem.COAWaiver;
		}

		public string GetDataText(LineItemDO lineItem)
		{
			return lineItem.COAWaiver.ToString();
		}

		public void SetDataValue(LineItemDO lineItem, object newValue)
		{
			lineItem.COAWaiver=(bool) newValue;
			OnFieldChanged();
		}

		#endregion


	}
}
