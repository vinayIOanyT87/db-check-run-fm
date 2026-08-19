//*****************************************************************************************************************
//  FILE NAME:		DocumentNumberFG.cs
//	PURPOSE:		This class inherits from the TextFieldGenerator class.
//
//	COMMENTS:
//		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
//		This file shall not be copied or reproduced in any form without
//		the express written consent of Endress+Hauser.
//
//	AUTHOR(S):	Thomas Beckum
//	VERSION:	1.0.0  Current version
//
//	MODIFICATION HISTORY:
//		Date:			By:					Reason:
//		----------	-----------------	-------------------------------------------
//		09/02/20008	W.Gray				7.4.6.0 - Revised to return Editable false when
//												Reversal or Update (CSI 6070)
//
//		09/02/2009	W.Gray				7.5.6.1 - Revised to return Editible true when Update
//												(CSI 6376)
//*****************************************************************************************************************

namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for DocumentNumberFG.
	/// </summary>
	public class DocumentNumberFG : TextFieldGenerator, IHeaderField
	{
		public DocumentNumberFG()
		{
		
		}

		public override string FieldID
		{
			get
			{
				return "DocumentNumber";
			}
		}

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.DocumentNumber;
		}

		public string GetDataText(TransactionDO transaction)
		{
			if (GetDataValue(transaction) != null)
			{
				return GetDataValue(transaction).ToString();
			}

			return null;
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			var stringTemp = newValue as string;

			// check to ensure that there is not a leading or trailing space
			if (stringTemp != null)
			{
				transaction.DocumentNumber = stringTemp.Trim();
			}

			OnFieldChanged();
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 30.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(this.FieldID, 30);
			}
		}

		public override bool Editable
		{
			get
			{
				return (	trans.ReversalType == TransactionDO.Reversal
							|| trans.ReversalType == TransactionDO.ReversalWithUpdate
                            || trans.ReversalType == TransactionDO.UpdateOriginal
                            || (trans.DocumentNumber != null && trans.TransTypeID == TransactionTypes.T5_PrimaryDisbursement)) ? false : true;
            }
		}
	}
}
