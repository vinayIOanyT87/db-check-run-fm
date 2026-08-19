/// <summary>
///   FILE NAME:		DensityUnitFG.cs
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
namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;

	public class DensityUnitFG : EngUnitFG
	{
		public DensityUnitFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "DensityUnit";
			}
		}

		public override object GetDataValue(TransactionDO transaction)
		{
			return GetUnitAsAbbrevString(transaction.DensityUnits);
		}
	}
}
