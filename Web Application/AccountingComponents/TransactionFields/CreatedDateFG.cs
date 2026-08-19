/// <summary>
///	FILE NAME:  CreatedDateFG.cs
///	PURPOSE:		This class generates the Created Date field.
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

	public class CreatedDateFG : DateTimeGenerator, IHeaderField
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the Created Date field generator.
		/// </summary>
		public CreatedDateFG ( )
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will return the Field ID for the Created Date field.
		/// </summary>
		public override string FieldID
		{
			get { return "CreatedDate"; }
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
		/// This method will return the value of the Created Date field.
		/// </summary>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public object GetDataValue ( TransactionDO transaction )
		{
			return transaction.CreatedDate;
		}

		/// <summary>
		/// This method will return the value in a string format of the Created Date field.
		/// </summary>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public string GetDataText ( TransactionDO transaction )
		{
			if (GetDataValue(transaction) != null)
			{
				return GetDataValue(transaction).ToString();
			}

			return null;
		}

		/// <summary>
		/// This method will set the value in the data object. Since the created date
		/// is read only, then this method will do nothing.
		/// </summary>
		/// <param name="transaction"></param>
		/// <param name="newValue"></param>
		public void SetDataValue ( TransactionDO transaction, object newValue )
		{
		}
		#endregion
	}
}