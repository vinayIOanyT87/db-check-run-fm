/// <summary>
///   FILE NAME:		LineItemLevelUnitFG.cs
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

	public class LineItemLevelUnitFG : LineItemEngUnitFG
	{
		public LineItemLevelUnitFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem LevelUnit";
			}
		}

		public override object GetDataValue(LineItemDO inLineItem)
		{
			return GetUnitAsAbbrevString(inLineItem.LevelUnits);
		}

		public override object GetDataValue(SubLineItemDO subLineItem)
		{
			return GetUnitAsAbbrevString(subLineItem.LevelUnits);
		}
	}
}
