using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemDifferentialPressureFG.
	/// </summary>
	public class LineItemDifferentialPressureFG :
		NumericTextFieldGenerator, ILineItemField, ISublineItemField
	{
		public LineItemDifferentialPressureFG()
		{
			
		}

		public override string FieldID { get { return "LineItem DifferentialPressure"; } }
		public override ENumericType NumericType { get { return ENumericType.Double; } }
		public override SITE_VARIABLE_TYPE UnitType
		{ get { return SITE_VARIABLE_TYPE.PRESSURE; } }

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			if(lineItem.DifferentialPressure == null)
			{
				return null;
			}
			return lineItem.DifferentialPressure.Value;
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
				lineItem.DifferentialPressure = null;
			}
			else
			{
				lineItem.DifferentialPressure = new double?((double) newValue);
			}
			OnFieldChanged();
		}

		#endregion

		#region ISublineItemField Members

		object TransactionFields.ISublineItemField.GetDataValue(
			SubLineItemDO sublineItem)
		{
			if(sublineItem.DifferentialPressure == null)
			{
				return null;
			}
			return sublineItem.DifferentialPressure.Value;
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
				sublineItem.DifferentialPressure = null;
			}
			else
			{
				sublineItem.DifferentialPressure = new double?((double) newValue);
			}
			OnFieldChanged();
		}

		#endregion
	}
}
