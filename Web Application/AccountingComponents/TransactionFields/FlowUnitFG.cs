 #pragma warning disable 1587
/// <summary>
///   FILE NAME:		FlowUnitFG.cs
///	PURPOSE:		
///
///	COMMENTS:
///		Copyright (C) Varec, Inc. Norcross, GA, USA, 2007
///		This file shall not be copied or reproduced in any form without
///		the express written consent of Varec.
///
///	AUTHOR(S):	
///	VERSION:	1.0.0  Current version
///
///	MODIFICATION HISTORY:
///   Date:		   By:			         Reason:
///   ----------	-----------------	   ----------------------------------------------
/// </summary>
#pragma warning restore 1587

namespace TransactionFields
{
    using FMBusinessObjects.DataObjects;

    public class FlowUnitFG : EngUnitFG
	{
        public override string FieldID => "FlowUnit";

        public override object GetDataValue(TransactionDO transaction)
		{
			return this.GetUnitAsAbbrevString(transaction.FlowUnits);
		}
	}
}
