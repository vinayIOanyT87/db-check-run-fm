namespace TransactionFields
{
    using System;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.DataObjects;

    /// <summary>
    /// Summary description for Number02FG.
    /// Author: Van Thompson
    /// Generic number transaction field needed for ADF
    /// </summary>
    public class Number02FG : NumericTextFieldGenerator, IHeaderField
	{
		public Number02FG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "Number02";
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
			if (transaction.Number02 == null)
				return null;
			else
				return transaction.Number02.Value;
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
				transaction.Number02 = null;
			else
				transaction.Number02 = new double?((double) newValue);
			OnFieldChanged();
		}

		#endregion
	}
}
