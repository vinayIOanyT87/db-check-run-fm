//*****************************************************************************************************************
//  FILE NAME:		LineItemCloseoutDateFG.cs
//	PURPOSE:		This class inherits from the DateGenerator, ILineItemField, and ISublineItemField 
//					classes. It is used to contain the line item CloseoutDate field information.
//
//	COMMENTS:
//		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
//		This file shall not be copied or reproduced in any form without
//		the express written consent of Endress+Hauser.
//
//	AUTHOR(S):	Thomas Beckum
//	VERSION:		1.0.0  Current version
//
//	MODIFICATION HISTORY:
//		Date:			By:					Reason:
//		----------	-----------------	-------------------------------------------
//		05/23/2008	W.Gray				7.4.4.0 - Corrected SubLineItem functionality (CSI 5924)
//*****************************************************************************************************************
using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemCloseoutDateFG.
	/// </summary>
	internal class LineItemCloseoutDateFG : DateGenerator, ILineItemField, ISublineItemField
	{
		public LineItemCloseoutDateFG()
		{
		}
		public override string FieldID { get { return "LineItem ItemCloseoutDate"; } }

		public override bool Editable { get { return false; } }

		#region ILineItemField Members

		object ILineItemField.GetDataValue(LineItemDO lineItem)
		{
			if (lineItem.CloseoutDate == null)
			{
				return null;
			}
			return lineItem.CloseoutDate.Value;
		}

		string ILineItemField.GetDataText(LineItemDO lineItem)
		{

			if ((this as ILineItemField).GetDataValue(lineItem) != null)
			{
				return (this as ILineItemField).GetDataValue(lineItem).ToString();
			}
			else
			{
				return null;
			}
		}

		void ILineItemField.SetDataValue(LineItemDO lineItem, object newValue)
		{
			lineItem.CloseoutDate = newValue as DateTime?;
			OnFieldChanged();
		}

		#endregion

		#region ISublineItemField Members

		object ISublineItemField.GetDataValue(SubLineItemDO sublineItem)
		{
			if (sublineItem.CloseoutDate == null)
				return null;

			return sublineItem.CloseoutDate.Value;
		}

		string ISublineItemField.GetDataText(SubLineItemDO sublineItem)
		{
			if (((ISublineItemField)this).GetDataValue(sublineItem) != null)
			{
				return ((ISublineItemField)this).GetDataValue(sublineItem).ToString();
			}
			else
			{
				return null;
			}
		}

		void ISublineItemField.SetDataValue(SubLineItemDO sublineItem, object newValue)
		{
			sublineItem.CloseoutDate = newValue as DateTime?;
			OnFieldChanged();
		}

		#endregion

	}
}
