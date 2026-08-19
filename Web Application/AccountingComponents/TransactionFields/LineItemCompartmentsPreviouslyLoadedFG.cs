using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemCompartmentsPreviouslyLoadedFG.
	/// </summary>
	public class LineItemCompartmentsPreviouslyLoadedFG : CheckBoxGenerator, ILineItemField
	{
		public LineItemCompartmentsPreviouslyLoadedFG()
		{
		}
		public override string FieldID
		{ get { return "LineItem CompartmentsPreviouslyLoaded"; } }

		public override bool Editable
		{ get { return false; }}

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			if (lineItem.CompartmentsPreviouslyLoaded == null)
				return false;

			return lineItem.CompartmentsPreviouslyLoaded.Value;
		}

		public string GetDataText(LineItemDO lineItem)
		{
			if(lineItem.CompartmentsPreviouslyLoaded != null)
				return lineItem.CompartmentsPreviouslyLoaded.Value.ToString();
			else
				return "";
		}

		public void SetDataValue(LineItemDO lineItem, object newValue)
		{
			lineItem.CompartmentsPreviouslyLoaded=(bool?) newValue;
			OnFieldChanged();
		}

		#endregion
	}
}
