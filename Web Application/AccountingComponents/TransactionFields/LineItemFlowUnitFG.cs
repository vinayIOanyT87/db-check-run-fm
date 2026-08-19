/// <summary>
///   FILE NAME:		LineItemFlowUnitFG.cs
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

	public class LineItemFlowUnitFG : LineItemEngUnitFG
	{
		public LineItemFlowUnitFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem FlowUnit";
			}
		}

		public override object GetDataValue(LineItemDO inLineItem)
		{
			return GetUnitAsAbbrevString(inLineItem.FlowUnits);
		}

		public override object GetDataValue(SubLineItemDO inSubLineItem)
		{
			return GetUnitAsAbbrevString(inSubLineItem.FlowUnits);
		}
	}
}
