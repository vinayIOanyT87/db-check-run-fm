using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemQualityTestNumberFG.
	/// </summary>
	public class LineItemQualityTestNumberFG : TextFieldGenerator, ILineItemField
	{
		public LineItemQualityTestNumberFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem QualityTestNumber";
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


		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			return lineItem.QualityTestNumber;
		}

		public string GetDataText(LineItemDO lineItem)
		{
			if (GetDataValue(lineItem) != null)
			{
				return GetDataValue(lineItem).ToString();
			}
			else
			{
				return null;
			}
		}

		public void SetDataValue(LineItemDO lineItem, object newValue)
		{
			lineItem.QualityTestNumber = newValue as string;
			OnFieldChanged();
		}

		#endregion
	}
}
