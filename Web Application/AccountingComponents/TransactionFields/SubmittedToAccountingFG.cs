// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubmittedToAccountingFG.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SubmittedToAccountingFG type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TransactionFields
{
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for SubmittedToAccountingFG.
	/// </summary>
	public class SubmittedToAccountingFG : CheckBoxGenerator, IHeaderField
	{
		public override string FieldID
		{
			get { return "SubmittedToAccounting"; }
		}

		#region IHeaderField Members
		public object GetDataValue(TransactionDO transaction)
		{
			if (transaction.SubmittedToAccounting == null)
			{
				return null;
			}

			return transaction.SubmittedToAccounting.Value;
		}

		public string GetDataText(TransactionDO transaction)
		{
			if (transaction.SubmittedToAccounting == null)
			{
				return bool.FalseString;
			}

			return transaction.SubmittedToAccounting.Value.ToString();
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			if (newValue is bool)
			{
				transaction.SubmittedToAccounting = (bool) newValue;
				this.SetNewValue((bool?)newValue);
				OnFieldChanged();
			}
			else
			{
				transaction.SubmittedToAccounting = null;
			}

		}

		#endregion
	}
}
