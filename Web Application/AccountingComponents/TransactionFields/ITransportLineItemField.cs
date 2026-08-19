namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	public interface ITransportLineItemField
   {
      object GetDataValue(TransportLineItemDO tranportLineItem);
      string GetDataText(TransportLineItemDO tranportLineItem);
      void SetDataValue(TransportLineItemDO tranportLineItem, object newValue);
   }
}
