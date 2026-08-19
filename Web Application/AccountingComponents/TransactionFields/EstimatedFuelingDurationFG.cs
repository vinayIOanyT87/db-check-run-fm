using System;
using FMBusinessObjects.DataObjects;


namespace TransactionFields
{
	/// <summary>
	/// Summary description for EstimatedFuelingDurationFG.
	/// </summary>
	public class EstimatedFuelingDurationFG : NumericTextFieldGenerator, IHeaderField
	{
		public EstimatedFuelingDurationFG()
		{
		
		}

		public override string FieldID { get { return "EstimatedFuelingDuration"; } }
		public override ENumericType NumericType { get { return ENumericType.Integer; } }
		public override SITE_VARIABLE_TYPE UnitType
		{ get { return SITE_VARIABLE_TYPE.DEFAULT; } }
		#region IHeaderField Members

		public object GetDataValue(TransactionDO transaction)
		{
			if(transaction.EstimatedFuelingDuration == null)
			{
				return null;
			}
			return transaction.EstimatedFuelingDuration.Value;
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
			if(newValue == null)
			{
				transaction.EstimatedFuelingDuration = null;
			}
			else
			{
				transaction.EstimatedFuelingDuration = (int?) newValue;
			}

			OnFieldChanged();
		}

		#endregion
	}
}
