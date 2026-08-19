using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransactionFields
{
   public class ADFLineItemReceiptVarianceFG : LineItemReceiptVarianceFG, ILineItemField
   {
      public override string FieldID
      {
         get
         {
            return "LineItem ADFReceiptVariance";
         }
      }
      /*
      #region NumericTextFieldGenerator Overrides
      public override NumericTextFieldGenerator.ENumericType NumericType
      {
         return ENumericType
      }
      #endregion // NumbericTextFieldGenerator Overrides
      */
      #region ILineItemField Members

      public new object GetDataValue(FM7Accounting.LineItemDO lineItem)
      {
         // if you change this, change all places marked with [ReceiptPriceQuantity]
         double recVariance = lineItem.Volume.NetInventoryChange -
            (lineItem.AlternativeNetVolume == null ? 0.0 : lineItem.AlternativeNetVolume.Value);
         lineItem.ReceiptVariance = new FM7Accounting.VDouble(recVariance);


         return Math.Round(recVariance, 0);
      }

      public new string GetDataText(FM7Accounting.LineItemDO lineItem)
      {
         return GetDataValue(lineItem).ToString();
      }

      public new void SetDataValue(FM7Accounting.LineItemDO lineItem, object newValue)
      {
         double recVariance = (lineItem.AlternativeNetVolume == null ? 0.0 : lineItem.AlternativeNetVolume.Value);
         lineItem.ReceiptVariance = new FM7Accounting.VDouble(recVariance);
         OnFieldChanged();
      }

      #endregion
   }
}
