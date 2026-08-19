using System;
using System.Collections.Specialized;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemCustomsFG.
	/// </summary>
	public class LineItemCustomsFG : DropDownGenerator, ILineItemField, ISublineItemField
	{
		public LineItemCustomsFG()
		{
			
		}

		public override string FieldID { get { return "LineItem Customs"; } }
		public override HybridDictionary GetEntries()
		{
			HybridDictionary listEntries = new HybridDictionary(false);

			//TODO: Customs choices should be subject to the data dictionary.
			listEntries.Add("Domestic", "Domestic");
			listEntries.Add("FTZ", "FTZ");
			listEntries.Add("Bonded", "Bonded");

			return listEntries;
		}

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			return lineItem.Customs;
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
			lineItem.Customs = newValue as string;
			OnFieldChanged();
		}

		#endregion

		#region ISublineItemField Members

		object TransactionFields.ISublineItemField.GetDataValue(
			SubLineItemDO sublineItem)
		{
			return sublineItem.Customs;
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
			sublineItem.Customs = newValue as string;
			OnFieldChanged();
		}

		#endregion
	}
}
