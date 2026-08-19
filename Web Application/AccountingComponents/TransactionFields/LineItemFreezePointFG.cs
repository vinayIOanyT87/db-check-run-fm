using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemFreezePointFG.
	/// </summary>
	public class LineItemFreezePointFG : NumericTextFieldGenerator, ILineItemField, ISublineItemField
	{
		public LineItemFreezePointFG()
		{
	
		}

		public override string FieldID { get { return "LineItem FreezePoint"; } }
		public override ENumericType NumericType { get { return ENumericType.Double; } }
		public override SITE_VARIABLE_TYPE UnitType
		{ get { return SITE_VARIABLE_TYPE.TEMPERATURE; } }

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			if(lineItem.FreezePoint == null)
			{
				return null;
			}
			return lineItem.FreezePoint.Value;
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
				lineItem.FreezePoint = null;
			}
			else
			{
				lineItem.FreezePoint = new double?((double) newValue);
			}
			OnFieldChanged();
		}

		#endregion

		#region ISublineItemField Members

		object TransactionFields.ISublineItemField.GetDataValue(
			SubLineItemDO sublineItem)
		{
			if(sublineItem.FreezePoint == null)
			{
				return null;
			}
			return sublineItem.FreezePoint.Value;
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

		void TransactionFields.ISublineItemField.SetDataValue(
			SubLineItemDO sublineItem, object newValue)
		{
			if(newValue == null)
			{
				sublineItem.FreezePoint = null;
			}
			else
			{
				sublineItem.FreezePoint = new double?((double) newValue);
			}
			OnFieldChanged();
		}

		#endregion
	}
}
