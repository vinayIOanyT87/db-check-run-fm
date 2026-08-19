/******************************************************************************
	FILE NAME:		ScheduledDateFG.cs
	PURPOSE:			Implementation of: ScheduledDateFG

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	W. Gray
	VERSION:	1.0.0  Current version

	MODIFICATION HISTORY:
		Date:		By:					Reason:
		----------	-----------------	-------------------------------------------
		2007-04-05	W.Gray				Reinstated setting of Transaction Status (CSI 4326)
*******************************************************************************/
namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;
	using System;

	/// <summary>
	/// Summary description for ScheduledDateFG.
	/// </summary>
	internal class ScheduledDateFG : DateTimeGenerator, IHeaderField
	{
		public ScheduledDateFG()
		{
		}

		public override string FieldID
		{
			get { return "ScheduledDate"; }
		}

		public override bool Required
		{
			get { return false; }
		}

		public override bool Editable
		{
			get
			{
				if (this.trans.TransTypeID == TransactionTypes.T17_Order)
				{
					// Only editable if you have MODIFY_ORDERS security priviledge
					return this.transContext.security.HasModifyTransactionRightByAliasName(trans.Alias);
				}

				return true;
			}
		}


		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.ScheduledDate;
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
			if (newValue == null)
			{
				transaction.ScheduledDate = null;
			}
			else
			{
				transaction.ScheduledDate = newValue as DateTimeOffset?;
				transaction.Status = TransactionStatus.Scheduled;

				foreach (LineItemDO localLineItem in this.trans.LineItems)
				{
					localLineItem.Status = TransactionStatus.Scheduled;
				}

				var status = (TransactionStatusFG)this.fieldGenerator.GetFieldGenerator("LookupTransactionStatusIndex");

				if (status != null)
				{
					status.SetNewValue(transaction, TransactionStatus.Scheduled);
				}
			}

			OnFieldChanged();
		}
	}
}
