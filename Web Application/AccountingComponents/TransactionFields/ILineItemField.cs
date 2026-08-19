using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for ILineItemField.
	/// </summary>
	public interface ILineItemField
	{
		object GetDataValue(LineItemDO lineItem);
		string GetDataText(LineItemDO lineItem);
		void SetDataValue(LineItemDO lineItem, object newValue);
	}
}
