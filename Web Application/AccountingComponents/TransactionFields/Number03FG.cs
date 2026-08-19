using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for Number03FG.
	/// Author: Van Thompson
	/// Generic number transaction field needed for ADF
	/// </summary>
	public class Number03FG : NumericTextFieldGenerator, IHeaderField
	{
		public Number03FG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "Number03";
			}
		}

		public override ENumericType NumericType
		{
			get
			{
				return ENumericType.Double;
			}
		}

		/// <summary>
		/// This property will return the unit type which is set to default.
		/// </summary>
		public override SITE_VARIABLE_TYPE UnitType
		{
			get { return SITE_VARIABLE_TYPE.DEFAULT; }
		}

		#region IHeaderField Members

		public object GetDataValue(TransactionDO transaction)
		{
			if (transaction.Number03 == null)
				return null;
			else
				return transaction.Number03.Value;
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
			if (newValue == null)
				transaction.Number03 = null;
			else
				transaction.Number03 = new double?((double) newValue);
			OnFieldChanged();
		}

		#endregion
	}
}
