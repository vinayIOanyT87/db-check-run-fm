
using System;
using System.Collections.Specialized;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemTransactionQualityFG.
	/// </summary>
	public class LineItemTransactionQualityFG :
		DropDownGenerator, ILineItemField, ISublineItemField
	{
		public LineItemTransactionQualityFG()
		{

		}

		public override string FieldID { get { return "LineItem Quality"; } }
		public override bool Editable { get { return true; } }

        public override HybridDictionary GetEntries()
		{
			// Create a new dictionary
			HybridDictionary newDictionary = new HybridDictionary();

			int NumberOfStatuses = Enum.GetValues(typeof(TransactionQuality)).Length;

			// Check to see if the datadictionary is used
			bool UseDataDictionary = this.transContext.useDataDictonary;

			for (int nLoop = 0; nLoop < NumberOfStatuses; ++nLoop)
			{
				//newDictionary.Add( Enum.GetName( typeof(TransactionQuality), nLoop ).ToString(), nLoop.ToString() );
				string Value = Enum.GetName(typeof(TransactionQuality), nLoop).ToString();

				if (UseDataDictionary)
				{
					string Value2 = this.GetDataDictionaryValueByKey(this.transContext.accountingSite.CurrentSiteGuid, Value);
					newDictionary.Add(Value2, Value);
				}
				else
				{
					newDictionary.Add(Value, Value);
				}
			}

			return newDictionary;

		}


		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			return lineItem.Quality.ToString();
		}

		public string GetDataText(LineItemDO lineItem)
		{
			if (this.transContext.useDataDictonary)
			{
                string datatext = GetDataDictionaryValueByKey(this.transContext.accountingSite.CurrentSiteGuid, GetDataValue(lineItem).ToString());
				return datatext;

			}

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
			lineItem.Quality = (TransactionQuality)Enum.Parse(
				typeof(TransactionQuality), newValue as string);
			OnFieldChanged();
		}

		#endregion

		#region ISublineItemField Members

		object TransactionFields.ISublineItemField.GetDataValue(
			SubLineItemDO sublineItem)
		{
			return sublineItem.Quality.ToString();
		}

		string TransactionFields.ISublineItemField.GetDataText(SubLineItemDO sublineItem)
		{

			if (this.transContext.useDataDictonary)
			{
                string datatext = GetDataDictionaryValueByKey(this.transContext.accountingSite.CurrentSiteGuid, ((ISublineItemField)this).GetDataValue(sublineItem).ToString());
				return datatext;
			}

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
			sublineItem.Quality = (TransactionQuality)Enum.Parse(
				typeof(TransactionQuality), newValue as string);
			OnFieldChanged();
		}

		#endregion
	}
}
