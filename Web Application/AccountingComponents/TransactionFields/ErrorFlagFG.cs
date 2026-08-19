/// <summary>
///	FILE NAME:  ErrorFlagFG.cs
///	PURPOSE:		This class generates the Error Flag field.
///
///	COMMENTS:
///		Copyright (C) Varec, Inc. Norcross, GA, USA, 2007
///		This file shall not be copied or reproduced in any form without
///		the express written consent of Varec.
///
///	AUTHOR(S):	Richard Panachida
///	VERSION:		1.0.0  Current version
///
///	MODIFICATION HISTORY:
///		Date:			By:					Reason:
///		----------	-----------------	-------------------------------------------
///      yyyy-mm-dd  Developer's name  Reason for the change
///      
/// </summary>
namespace TransactionFields
{
    using FMBusinessObjects.DataObjects;
    using System.Web.UI.WebControls;

    public class ErrorFlagFG : CheckBoxGenerator, IHeaderField
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the Error Flag field generator.
		/// </summary>
		public ErrorFlagFG()
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will return the Field ID of the error flag field.
		/// </summary>
		public override string FieldID
		{
			get { return "ErrorFlag"; }
		}

		/// <summary>
		/// This property will return false indicating that the field is always
		/// read only.
		/// </summary>
		public override bool Editable
		{
			get { return false; }
		}
		#endregion

		#region IHeaderField Members
		/// <summary>
		/// This method will return the value of the Error Flag field from
		/// the transaction data object.
		/// </summary>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.ErrorFlag;
		}

		/// <summary>
		/// This method will return the string value of the Error Flag field
		/// from the transaction data object.
		/// </summary>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public string GetDataText(TransactionDO transaction)
		{
			return transaction.ErrorFlag.ToString();
		}

		/// <summary>
		/// This method sets the transaction data object with the new value of the 
		/// error flag.  In addition, the checkbox is also set.
		/// </summary>
		/// <param name="transaction"></param>
		/// <param name="newValue"></param>
		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			if (newValue is bool)
			{
				transaction.ErrorFlag = (bool) newValue;
				this.SetNewValue((bool?)newValue);
				base.OnFieldChanged();
			}
		}
		#endregion
	}
}
