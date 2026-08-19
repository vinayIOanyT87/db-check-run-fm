/*****************************************************************************
LineItemSourceEquipmentFG

Original Author: Van Thompson
Revisions: See source control comments

(C) Copyright 2008 by Varec, Inc.  All rights reserved.

Revision History
Date:		By:					Reason:

11/20/2008	V. Thompson			Made the Get/Set data values virtual so the To Source Equipment could override
//*****************************************************************************/
using System;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemSourceEquipmentModel.
	/// </summary>
	public class LineItemSourceEquipmentFG : LineItemEquipmentFG, ILineItemField
	{
		#region Contructors
		public LineItemSourceEquipmentFG() : base(false)
		{
				
		}
		#endregion

		#region Override Properties
		public override string FieldID { get { return "LineItem SourceRegistrationID"; } }

		/// <summary>
		/// This property returns the field's maximum column width.
		/// </summary>
		protected override short MaxColumns
		{
			get { return base.GetFieldLength(FieldID, EquipmentTextButtonGenerator.FIELD_LENGTH); } 
		}
		#endregion


		public virtual object GetDataValue(LineItemDO lineItem)
		{
			return lineItem.SourceEQ.RegistrationID;
		}


		public virtual string GetDataText(LineItemDO lineItem)
		{
			return GetDataText(lineItem.SourceEQ);
		}

		public virtual void SetDataValue(LineItemDO lineItem,	object newValue)
		{
			this.SetEquipment(newValue as string, lineItem.SourceEQ);
		}
	}
}
