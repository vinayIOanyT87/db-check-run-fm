namespace TransactionFields
{
	using System;
	using System.Collections;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for HeaderEquipmentFG.
	/// </summary>
	public abstract class HeaderEquipmentFG:BaseEquipmentFG
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		/// <param name="destination">Whether it is source or destination equipment.</param>
		/// <param name="eqNumber">Equipment number.</param>
		public HeaderEquipmentFG(bool destination,byte eqNumber)
			: base(destination, eqNumber)
		{
		}
		#endregion

		protected EQUIPMENT_TYPE SelectedEquipmentType
		{
			get
			{
				if (destination)
				{
					switch (eqNumber)
					{
						case 1:
							return EquipmentTypeClass.Type(this.trans.DestinationEQ1.EquipmentType);
						case 2:
							return EquipmentTypeClass.Type(this.trans.DestinationEQ2.EquipmentType);
						case 3:
							return EquipmentTypeClass.Type(this.trans.DestinationEQ3.EquipmentType);
					}
				}
				else
				{
					switch (eqNumber)
					{
						case 1:
							return EquipmentTypeClass.Type(this.trans.SourceEQ1.EquipmentType);
						case 2:
							return EquipmentTypeClass.Type(this.trans.SourceEQ1.EquipmentType);
						case 3:
							return EquipmentTypeClass.Type(this.trans.SourceEQ1.EquipmentType);
					}
				}

				return EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE;
			}
		}


		#region Override methods
		protected override EquipmentInfo[] GetEntries()
		{
			EQUIPMENT_TYPE[] equipmentTypes;

			// The equipment is dependent on the equipment type. So if there is a selected
			// equipment type, set the equipment type list to match only the selected equipment
			// type.
			if (this.SelectedEquipmentType != EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE)
			{
				var equipmentTypeList = new ArrayList { this.SelectedEquipmentType };
				equipmentTypes = (EQUIPMENT_TYPE[])equipmentTypeList.ToArray(typeof(EQUIPMENT_TYPE));
			}
			else
			{
				equipmentTypes = transContext.aliasClass.GetEquipmentTypes(destination, eqNumber);
			}

			object companyGuid		= null;
			object productGuid		= null;
			object fuelCardGuid		= null;
			object secondaryStorage	= null;

			// JS20101014 WI-14935 for the following types of transactions, filter refuell tank id on site name 
			// as company
			if(FMChannelHelper.MakeCall<IHardwareKey, Boolean>(x =>x.IsADFKey() )
				// refueller tank ID for sales and issues are in source equipment
				&& (!destination
				&& (trans.Alias.ToUpper().Equals("SALE (AVIATION)")
				|| trans.Alias.ToUpper().EndsWith("ISSUE (AVIATION)")))
				// refueller tank ID for defuel, fill stand and return to bulk are in destination equipment
				|| (destination
				&& (trans.TransTypeID == TransactionTypes.T3_PrimaryDefuel
				||	trans.TransTypeID == TransactionTypes.T4_SecondaryDefuel
				||	trans.TransTypeID == TransactionTypes.T7_FillStand
				||	trans.TransTypeID == TransactionTypes.T10_Unload)))
			{
				companyGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(
                                                    x =>
                                                    x.GetIdentityGuid(transContext.security,trans.Site) 
                                                );
			}

			// Consumer Transfers enumerate based upon From or To ShipTo.  ShipTo must have carrier role and have equipment
			// assigned.
			else if(trans.TransTypeID == TransactionTypes.T11_ConsumerTransfer)
			{
				var consumerTransferDO = trans as ConsumerTransferDO;

				if(destination)
				{
					if(consumerTransferDO != null && (this.transContext.aliasClass.TransactionFieldCollection.Find("FuelCardID") != null
					                                  && consumerTransferDO.FuelCardGuid != Guid.Empty))
					{
						fuelCardGuid=consumerTransferDO.FuelCardGuid;
					}
					else if(consumerTransferDO != null && (this.transContext.aliasClass.TransactionFieldCollection.Find("ToShipToID") != null
					                                       && consumerTransferDO.ToShipToCompanyGuid != Guid.Empty))
					{
						companyGuid=consumerTransferDO.ToShipToCompanyGuid;
					}
				}
				else
				{
					if(consumerTransferDO != null && (this.transContext.aliasClass.TransactionFieldCollection.Find("FromShipToID") != null
					                                  && consumerTransferDO.ShipToCompanyGuid != Guid.Empty))
					{
						companyGuid=consumerTransferDO.ShipToCompanyGuid;
					}
				}
			}
			else if (trans.TransTypeID == TransactionTypes.T5_PrimaryDisbursement || trans.TransTypeID == TransactionTypes.T25_Shipment)
			{
				if (this.destination)
				{
					if (transContext.aliasClass.TransactionFieldCollection.Find("FuelCardID") != null
						&& trans.FuelCardGuid != Guid.Empty)
					{
						fuelCardGuid = trans.FuelCardGuid;
					}
					else if (transContext.aliasClass.TransactionFieldCollection.Find("CarrierID") != null
						&& trans.CarrierCompanyGuid != Guid.Empty)
					{
						companyGuid = trans.CarrierCompanyGuid;
					}
				}
			}
			else if (trans.TransTypeID == TransactionTypes.T6_SecondaryDisbursement)
			{
				if(this.destination)
				{
					if(transContext.aliasClass.TransactionFieldCollection.Find("FuelCardID") != null
						&& trans.FuelCardGuid != Guid.Empty)
					{
						fuelCardGuid = trans.FuelCardGuid;
					}
					else if(transContext.aliasClass.TransactionFieldCollection.Find("ShipToID") != null
							&& trans.ShipToCompanyGuid != Guid.Empty)
					{
						companyGuid = trans.ShipToCompanyGuid;
					}
				}
				else
				{
					if (transContext.aliasClass.TransactionFieldCollection.Find("CarrierID") != null
					    && trans.CarrierCompanyGuid != Guid.Empty)
					{
						companyGuid = trans.CarrierCompanyGuid;
					}
					else
					{
						if (transContext.aliasClass.TransactionFieldCollection.Find("ManagerID") != null
						    && trans.ManagerCompanyGuid != Guid.Empty)
						{
							companyGuid = trans.ManagerCompanyGuid;
						}

						secondaryStorage = true;
					}
				}
			}
			else if (trans.TransTypeID == TransactionTypes.T3_PrimaryDefuel)
			{
				if (this.destination == false)
				{
					if (transContext.aliasClass.TransactionFieldCollection.Find("FuelCardID") != null
						&& trans.FuelCardGuid != Guid.Empty)
					{
						fuelCardGuid = trans.FuelCardGuid;
					}
					else if (transContext.aliasClass.TransactionFieldCollection.Find("CarrierID") != null
						&& trans.CarrierCompanyGuid != Guid.Empty)
					{
						companyGuid = trans.CarrierCompanyGuid;
					}
				}
			}
			else if (trans.TransTypeID == TransactionTypes.T4_SecondaryDefuel)
			{
				if(this.destination == false)
				{
					if(transContext.aliasClass.TransactionFieldCollection.Find("FuelCardID") != null
						&& trans.FuelCardGuid != Guid.Empty)
					{
						fuelCardGuid = trans.FuelCardGuid;
					}

					else if(transContext.aliasClass.TransactionFieldCollection.Find("ShipToID") != null
							&& trans.ShipToCompanyGuid != Guid.Empty)
					{
						companyGuid = trans.ShipToCompanyGuid;
					}
				}

				else
				{
					if (transContext.aliasClass.TransactionFieldCollection.Find("CarrierID") != null
						&& trans.CarrierCompanyGuid != Guid.Empty)
					{
						companyGuid = trans.CarrierCompanyGuid;
					}
					else
					{
						if (transContext.aliasClass.TransactionFieldCollection.Find("ManagerID") != null
							&& trans.ManagerCompanyGuid != Guid.Empty)
						{
							companyGuid = trans.ManagerCompanyGuid;
						}

						secondaryStorage = true;
					}
				}
			}
			else if(trans.TransTypeID == TransactionTypes.T12_InventoryNotAffected)
			{
				if(this.destination)
				{
					if(transContext.aliasClass.TransactionFieldCollection.Find("CarrierID") != null
						&& trans.CarrierCompanyGuid != Guid.Empty)
					{
						companyGuid = trans.CarrierCompanyGuid;
					}
				}
			}
			else if(trans.TransTypeID == TransactionTypes.T8_Receipt)
			{
				if(destination == false)
				{
					if(transContext.aliasClass.TransactionFieldCollection.Find("CarrierID") != null
					&& trans.CarrierCompanyGuid != Guid.Empty)
					{
						companyGuid = trans.CarrierCompanyGuid;
					}

					else if(transContext.aliasClass.TransactionFieldCollection.Find("SupplierID") != null
					&& trans.ShipToCompanyGuid != Guid.Empty)
					{
						companyGuid = trans.ShipToCompanyGuid;
					}
				}
			}
			else if(transContext.aliasClass.TransactionFieldCollection.Find("CarrierID") != null
					&& trans.CarrierCompanyGuid != Guid.Empty)
			{
				companyGuid = trans.CarrierCompanyGuid;
			}
			else if(transContext.aliasClass.TransactionFieldCollection.Find("ShipToID") != null
					&& trans.ShipToCompanyGuid != Guid.Empty)
			{
				companyGuid = trans.ShipToCompanyGuid;
			}

			if(!transContext.aliasClass.MultipleLineItems
				&& trans.LineItems.Count == 1
				&& this.trans.LineItems[0].ProductGuid != Guid.Empty)
			{
				productGuid = this.trans.LineItems[0].ProductGuid;
			}

			return FMChannelHelper.MakeCall<IEquipments, EquipmentInfo[]>(
                    x =>
                    x.EnumerateInfoByTypesCompanyFuelCardProductAndSecondaryStorage(transContext.security, equipmentTypes, companyGuid, fuelCardGuid, productGuid, secondaryStorage, hideHiddenEquipmentRecords: true) 
                );
		}
		#endregion
	}
}
