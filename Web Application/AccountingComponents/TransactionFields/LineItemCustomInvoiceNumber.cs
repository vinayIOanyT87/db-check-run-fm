using System;

using ConsolidatedDataObjects;
using FM7Accounting;

namespace TransactionFields
{
    public class LineItemCustomInvoiceNumber : TextFieldGenerator, ILineItemField
    {
        public LineItemCustomInvoiceNumber()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public override string FieldID
        {
            get
            {
                return "LineItem CustomInvoiceNumber";
            }
        }

        /// <summary>
        /// This property will returned either a figured data length or the 
        /// default length of 50.
        /// </summary>
        protected override short MaxColumns
        {
            get
            {
                return base.GetFieldLength(FieldID, 50);
            }
        }

        public override bool  Editable
        {
	        get 
	        { 
		        return false;
	        }
        }

        #region ILineItemField Members

        public object GetDataValue(FM7Accounting.LineItemDO lineItem)
        {
            return lineItem.InvoiceNumber;
        }

        public string GetDataText(FM7Accounting.LineItemDO lineItem)
        {
            return GetDataValue(lineItem).ToString();
        }

        public void SetDataValue(FM7Accounting.LineItemDO lineItem, object newValue)
        {
            object returnVal = null;

            if (lineItem.AssociatedTransactions != null)
            {
                // only get the first associated transaction
                AssociatedTxDO tx = lineItem.AssociatedTransactions[0] as AssociatedTxDO;
                lineItem.InvoiceNumber = tx.DocumentNumber;
            }
        }

        #endregion
    }
}
