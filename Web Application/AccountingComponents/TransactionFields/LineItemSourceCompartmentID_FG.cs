/*****************************************************************************
LineItemToStorageLocationFG

Original Author: Van Thompson
Revisions: See source control comments

(C) Copyright 2008 by Varec, Inc.  All rights reserved.

Revision History
Date:		By:					Reason:
11/20/2008	V. Thompson			Made the Get/Set value properties virtual
//*****************************************************************************/
using System;
using System.Collections.Specialized;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemSourceCompartmentID_FG.
	/// </summary>
	public class LineItemSourceCompartmentID_FG : CompartmentTextButtonGenerator, ILineItemField
	{
		public LineItemSourceCompartmentID_FG()
		{
			
		}

		#region Override properties
		public override string FieldID { get { return "LineItem SourceCompartmentID"; } }

		/// <summary>
		/// This property returns the field's maximum column width.
		/// </summary>
		protected override short MaxColumns
		{
			get { return base.GetFieldLength(FieldID, CompartmentTextButtonGenerator.FIELD_LENGTH); } 
		}
		#endregion



		#region ILineItemField Members

		public virtual object GetDataValue(LineItemDO lineItem)
		{
			return lineItem.SourceCompartmentID;
		}

		public virtual string GetDataText(LineItemDO lineItem)
		{
			if (GetDataValue(lineItem) != null)
			{
				return GetDataValue(lineItem).ToString();
			}
			else
			{
				return null;
			}
		}

		public virtual void SetDataValue(LineItemDO lineItem, object newValue)
		{
			lineItem.SourceCompartmentID = newValue as string;
			lineItem.SourceCompartmentEquipmentGuid = Guid.Empty;

			EquipmentClass equipment=null;

			if(lineItem.SourceEQ.EquipmentGuid != Guid.Empty)
				equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
																	 x =>
																	 x.Get(transContext.security, lineItem.SourceEQ.EquipmentGuid)
																);

			else if (trans.DestinationEQ1.EquipmentGuid != Guid.Empty)
				equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
																	 x =>
																	 x.Get(transContext.security, trans.SourceEQ1.EquipmentGuid)
																);
			if(equipment != null)
			{
				foreach(EquipmentClass compartment in equipment.CompartmentCollection)
				{
					if(compartment.EquipmentSequence.ToString() == lineItem.SourceCompartmentID)
					{
						lineItem.SourceCompartmentEquipmentGuid=compartment.IdentityGuid;
						break;
					}
				}
			}
			OnFieldChanged();
		}

		#endregion
	}
}
