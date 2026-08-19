using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for ISublineItemField.
	/// </summary>
	public interface ISublineItemField
	{
		object GetDataValue(SubLineItemDO sublineItem);
		string GetDataText(SubLineItemDO sublineItem);
		void SetDataValue(SubLineItemDO sublineItem, object newValue);
	}
}
