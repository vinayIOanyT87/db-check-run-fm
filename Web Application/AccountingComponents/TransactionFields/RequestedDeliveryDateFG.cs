/******************************************************************************
	FILE NAME:		RequestedDeliveryDateFG.cs
	PURPOSE:			Implementation of: RequestedDeliveryDateFG

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	W. Gray
	VERSION:	1.0.0  Current version

	MODIFICATION HISTORY:
		Date:			By:					Reason:
		----------	-----------------	-------------------------------------------
		2007-09-13	W.Gray				7.1.1.1 - Changed from DateGenerator to DateTimeGenerator
												(CSI 5181)
*******************************************************************************/
using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for RequestedDeliveryDateFG.
	/// </summary>
	internal class RequestedDeliveryDateFG : DateTimeGenerator, IHeaderField
	{
		public RequestedDeliveryDateFG()
		{

		}

		public override string FieldID { get { return "RequestedDeliveryDate"; } }
		public object GetDataValue(TransactionDO transaction)
		{
			if (transaction.RequestedDeliveryDate != null)
			{
				return transaction.RequestedDeliveryDate;
			}
			return null;
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
			transaction.RequestedDeliveryDate = newValue as DateTimeOffset?;
			OnFieldChanged();
		}
	}
}
