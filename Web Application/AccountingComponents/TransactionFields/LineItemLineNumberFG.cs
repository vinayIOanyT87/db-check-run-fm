using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemLineNumberFG.
	/// </summary>
	public class LineItemLineNumberFG : NumericTextFieldGenerator, ILineItemField, ISublineItemField
	{
		public LineItemLineNumberFG()
		{
	
		}

		public override string FieldID { get { return "LineItem LineNumber"; } }
		public override ENumericType NumericType { get { return ENumericType.Integer; } }
		public override SITE_VARIABLE_TYPE UnitType
		{ get { return SITE_VARIABLE_TYPE.DEFAULT; } }


		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			if(lineItem.LineNumber == null)
			{
				return null;
			}
			return lineItem.LineNumber.Value;
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
			if(newValue == null)
			{
				lineItem.LineNumber = null;
			}
			else
			{
				lineItem.LineNumber = new int?((int) newValue);
			}
			OnFieldChanged();
		}

		#endregion

		#region ISublineItemField Members

		object TransactionFields.ISublineItemField.GetDataValue(SubLineItemDO sublineItem)
		{
			if(sublineItem.LineNumber == null)
			{
				return null;
			}
			return sublineItem.LineNumber.Value;
		}

		string TransactionFields.ISublineItemField.GetDataText(SubLineItemDO sublineItem)
		{
			if (((ISublineItemField)this).GetDataValue(sublineItem) != null)
			{
				return ((ISublineItemField)this).GetDataValue(sublineItem).ToString();
			}
			else
			{
				return null;
			}
		}

		void TransactionFields.ISublineItemField.SetDataValue(SubLineItemDO sublineItem, object newValue)
		{
			if(newValue == null)
			{
				sublineItem.LineNumber = null;
			}
			else
			{
				sublineItem.LineNumber = new int?((int) newValue);
			}
			OnFieldChanged();
		}

		#endregion
	}
}
