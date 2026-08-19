using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemLoadRackVarianceFG.
	/// </summary>
	public class LineItemLoadRackVarianceFG :
		LineItemVolumeFG, ILineItemField
	{
		public LineItemLoadRackVarianceFG()
		{
		
		}

		public override string FieldID { get { return "LineItem LoadRackVariance"; } }

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			if(lineItem.LoadRackVariance == null)
			{
				return null;
			}
			return lineItem.LoadRackVariance.Value;
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
				lineItem.LoadRackVariance = null;
			}
			else
			{
				lineItem.LoadRackVariance = new double?((double) newValue);
			}
			OnFieldChanged();
		}

		#endregion

	}
}
