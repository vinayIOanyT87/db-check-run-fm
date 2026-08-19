// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteFlagFG.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for DeleteFlagFG.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;
	using System.Web.UI.WebControls;

	/// <summary>
	/// DeleteFlag field generator
	/// </summary>
	public class DeleteFlagFG : CheckBoxGenerator, IHeaderField
	{
		#region Public Properties
		public override bool Editable
		{
			get { return false; }
			set { base.Editable = value; }
		}

		public override string FieldID
		{
			get { return "DeleteFlag"; }
		}
		#endregion

		#region Public Methods and Operators
		public string GetDataText(TransactionDO transaction)
		{
			return transaction.DeleteFlag.ToString();
		}

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.DeleteFlag;
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			if (newValue is bool)
			{
				transaction.DeleteFlag = (bool)newValue;
				this.SetNewValue((bool?)newValue);
				OnFieldChanged();
			}
		}
		#endregion
	}
}