namespace TransactionFields
{
	using System;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemDestinationCompartmentID_FG.
	/// </summary>
	public class LineItemDestinationCompartmentID_FG : CompartmentTextButtonGenerator, ILineItemField
	{
		public LineItemDestinationCompartmentID_FG()
		{		
		}

		#region Override Properties
		public override string FieldID { get { return "LineItem DestinationCompartmentID"; } }

      /// <summary>
      /// This property will returned either a figured data length or the 
      /// default length of Compartment Text Button Generator.
      /// </summary>
      protected override short MaxColumns
		{
			get { return this.GetFieldLength(FieldID, FIELD_LENGTH); } 
		}
		#endregion

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			return inLineItem.DestinationCompartmentID;
		}

		public string GetDataText(LineItemDO inLineItem)
		{
			if (GetDataValue(inLineItem) != null)
			{
				return GetDataValue(inLineItem).ToString();
			}

			return null;
		}

		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			inLineItem.DestinationCompartmentID = newValue as string;
			inLineItem.DestinationCompartmentEquipmentGuid = Guid.Empty;

			EquipmentClass equipment=null;

			if (inLineItem.DestinationEQ.EquipmentGuid != Guid.Empty)
				equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
                                                    x =>
													x.Get(transContext.security, inLineItem.DestinationEQ.EquipmentGuid) 
                                                );

			else if (trans.DestinationEQ1.EquipmentGuid != Guid.Empty)
				equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
																	 x =>
																	 x.Get(transContext.security, trans.DestinationEQ1.EquipmentGuid)
																);
			if(equipment != null)
			{
				foreach(EquipmentClass compartment in equipment.CompartmentCollection)
				{
					if (compartment.EquipmentSequence == inLineItem.DestinationCompartmentID)
					{
						inLineItem.DestinationCompartmentEquipmentGuid = compartment.IdentityGuid;
						break;
					}
				}
			}

			OnFieldChanged();
		}
		#endregion
	}
}
