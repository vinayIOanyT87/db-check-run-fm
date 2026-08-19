using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemDensityFG.
	/// </summary>
	public class LineItemDensityFG : NumericTextFieldGenerator, ILineItemField, ISublineItemField
	{
		public LineItemDensityFG()
		{
	
		}

		protected override short MaxColumns { get { return base.GetFieldLength(FieldID, 5); } } 
		public override string FieldID { get { return "LineItem Density"; } }
		public override ENumericType NumericType { get { return ENumericType.Double; } }
		public override SITE_VARIABLE_TYPE UnitType
		{ get { return SITE_VARIABLE_TYPE.DENSITY; } }

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			if(lineItem.Density == null)
			{
				return null;
			}
			else
			{
				return lineItem.Density.Value;
			}
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
				lineItem.Density = null;
			}
			else
			{
				lineItem.Density = new double?((double) newValue);
			}

            if (base.cell != null)
            {
                System.Web.UI.WebControls.TextBox densityTextBox = base.cell.Controls[0].Controls[0].Controls[0] as System.Web.UI.WebControls.TextBox;
                if (densityTextBox != null)
                    densityTextBox.Text = GetFormattedValue();
            }

            OnFieldChanged();
		}

		#endregion

		#region ISublineItemField Members

		object TransactionFields.ISublineItemField.GetDataValue(SubLineItemDO sublineItem)
		{
			if(sublineItem.Density == null)
			{
				return null;
			}
			else
			{
				return sublineItem.Density.Value;
			}
		}

		public string GetDataText(SubLineItemDO lineItem)
		{
			if (((ISublineItemField)this).GetDataValue(lineItem) != null)
			{
				return ((ISublineItemField)this).GetDataValue(lineItem).ToString();
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
				sublineItem.Density = null;
			}
			else
			{
				sublineItem.Density = new double?((double) newValue);
			}
			OnFieldChanged();
		}

		#endregion		
	}
}
