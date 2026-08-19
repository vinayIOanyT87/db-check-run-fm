using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for InventoryDateFG.
	/// </summary>
	internal class InventoryDateFG : DateGenerator, IHeaderField
	{
		public InventoryDateFG()
		{

		}

		public override string FieldID { get { return "InventoryDate"; } }
		public override bool Required { get { return true; } }


		#region IHeaderField Members

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.InventoryDate;
		}

		public string GetDataText(TransactionDO transaction)
		{
			if (GetDataValue(transaction) != null)
			{
				return GetDataValue(transaction).ToString();
			}
			else
			{
				return null;
			}
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			if (newValue is DateTimeOffset)
			{
				transaction.InventoryDate = ((DateTimeOffset)newValue).Date;
				OnFieldChanged();
			}
		}

		#endregion
	}
}
