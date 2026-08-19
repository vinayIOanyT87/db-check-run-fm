using System;

namespace StandardXMLImportExport
{
	/// <summary>
	/// Summary description for Type12Populater.
	/// </summary>
	public class Type12Populater : TransactionPopulater
	{
		public Type12Populater()
		{
			
		}

		protected override string TransactionTypeID
		{
			get
			{
				return "Type12";
			}
		}

		protected override void Populate()
		{
			SetShipTo();

		}

		protected override void PopulateLineItem()
		{

		}
		

		protected override void SetLineItemProductCode()
		{
			lineItem.ProductCode = GetStringValue(lineItemPath + "ProductInfo/ToProductCode", false);
		}
		protected override void SetLineItemProduct()
		{
			lineItem.Product = GetStringValue(lineItemPath + "ProductInfo/Product", false);
		}
		protected override void SetLineItemProductType()
		{
			lineItem.ProductType = GetStringValue(lineItemPath + "ProductInfo/ProductType", false);
		}
		protected override void SetLineItemGrossQuantity()
		{
			lineItem.Volume.GrossInventoryChange = GetVolume(lineItemPath + "AccountingData/Quantity/Gross",
																  lineItemPath + "AccountingData/Quantity/Units", false);
		}
		protected override void SetLineItemNetQuantity()
		{
			lineItem.Volume.NetInventoryChange = GetVolume(lineItemPath + "AccountingData/Quantity/Net",
				lineItemPath + "AccountingData/Quantity/Units", false);
		}
	}
}
