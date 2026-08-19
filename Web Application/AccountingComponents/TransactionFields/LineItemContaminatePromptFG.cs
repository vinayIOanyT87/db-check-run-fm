using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemContaminatePromptFG.
	/// </summary>
	public class LineItemContaminatePromptFG : CheckBoxGenerator, ILineItemField
	{
		public LineItemContaminatePromptFG()
		{
		}
		public override string FieldID
		{ get { return "LineItem ContaminatePrompt"; } }

		public override bool Editable
		{ get { return false; }}

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			if (lineItem.ContaminatePrompt == null)
				return false;

			return lineItem.ContaminatePrompt.Value;
		}

		public string GetDataText(LineItemDO lineItem)
		{
			if(lineItem.ContaminatePrompt != null)
				return lineItem.ContaminatePrompt.Value.ToString();
			else
				return "";
		}

		public void SetDataValue(LineItemDO lineItem, object newValue)
		{
			lineItem.ContaminatePrompt=(bool?) newValue;
			OnFieldChanged();
		}

		#endregion
	}
}
