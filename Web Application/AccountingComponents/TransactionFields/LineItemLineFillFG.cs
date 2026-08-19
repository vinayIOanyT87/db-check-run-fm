using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemLineFillFG.
	/// </summary>
	public class LineItemLineFillFG : LineItemVolumeFG, ILineItemField, ISublineItemField
	{
		public LineItemLineFillFG()
		{
			
		}

		public override string FieldID { get { return "LineItem LineFill"; } }

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			if(lineItem.LineFill == null)
			{
				return null;
			}
			return lineItem.LineFill.Value;
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
			if( (newValue == null) || (newValue.Equals("")) )
			{
				lineItem.LineFill = null;
			}
			else
			{
				lineItem.LineFill = new double?((double) newValue);
			}
			OnFieldChanged();
		}

		#endregion

		#region ISublineItemField Members

		object TransactionFields.ISublineItemField.GetDataValue(SubLineItemDO sublineItem)
		{
			if(sublineItem.LineFill == null)
			{
				return null;
			}
			return sublineItem.LineFill.Value;
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

		void TransactionFields.ISublineItemField.SetDataValue(SubLineItemDO sublineItem,
			object newValue)
		{
			if( (newValue == null) || (newValue.Equals("")) )
			{
				sublineItem.LineFill = null;
			}
			else
			{
				sublineItem.LineFill = new double?((double) newValue);
			}
			OnFieldChanged();
		}

		#endregion
	}
}
