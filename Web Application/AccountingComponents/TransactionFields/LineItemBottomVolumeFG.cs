using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemBottomVolumeFG.
	/// </summary>
	public class LineItemBottomVolumeFG : LineItemVolumeFG, ILineItemField, ISublineItemField
	{
		public LineItemBottomVolumeFG()
		{
		
		}

		public override string FieldID { get { return "LineItem BottomVolume"; } }

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			if(lineItem.BottomVolume == null)
			{
				return null;
			}
			return lineItem.BottomVolume.Value;
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
				lineItem.BottomVolume = null;
			}
			else
			{
				lineItem.BottomVolume = new double?((double) newValue);
			}
			OnFieldChanged();
		}

		#endregion

		#region ISublineItemField Members

		object TransactionFields.ISublineItemField.GetDataValue(SubLineItemDO sublineItem)
		{
			if(sublineItem.BottomVolume == null)
			{
				return null;
			}
			return sublineItem.BottomVolume.Value;
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
				sublineItem.BottomVolume = null;
			}
			else
			{
				sublineItem.BottomVolume = new double?((double) newValue);
			}
			OnFieldChanged();
		}

		#endregion
	}
}
