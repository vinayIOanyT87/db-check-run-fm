/// <summary>
///	FILE NAME:  CreatedByFG.cs
///	PURPOSE:		This class generates the Created By field.
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

	public class CreatedByFG : TextFieldGenerator, IHeaderField
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the Created By field generator.
		/// </summary>
		public CreatedByFG()
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will return the Field ID for the Created By field.
		/// </summary>
		public override string FieldID
		{
			get { return "CreatedBy"; }
		}

		/// <summary>
		/// This property will returned either a configured data length or the 
		/// default length of 30.
		/// </summary>
		protected override short MaxColumns
		{
			get { return this.GetFieldLength(FieldID, 30); }
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

		#region Public methods
		/// <summary>
		/// This method will return the value of the Created By field.
		/// </summary>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.CreatedBy;
		}

		/// <summary>
		/// This method will return the value in a string format of the Created By field.
		/// </summary>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public string GetDataText(TransactionDO transaction)
		{
			if (GetDataValue(transaction) != null)
			{
				return GetDataValue(transaction).ToString();
			}

			return null;
		}

		/// <summary>
		/// This method will set the value in the data object. Since the created by
		/// is read only, then this method will do nothing.
		/// </summary>
		/// <param name="transaction"></param>
		/// <param name="newValue"></param>
		public void SetDataValue(TransactionDO transaction, object newValue)
		{
		}
		#endregion
	}
}
