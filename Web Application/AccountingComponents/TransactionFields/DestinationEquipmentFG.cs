namespace TransactionFields
{
	using System;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;

	/// <summary>
	/// Summary description for DestinationEquipmentFG.
	/// </summary>
	public class DestinationEquipmentFG : HeaderEquipmentFG, IHeaderField
	{
		#region Contructors
		public DestinationEquipmentFG(byte equipmentNumber)
			: base(true, equipmentNumber)
		{
			this.autoPostBack = true;
		}
		#endregion

		#region Override Properties
		public override string FieldID
		{
			get
			{
				return "DestinationRegistrationID" + eqNumber;
			}
		}

		/// <summary>
		/// This property returns the field's maximum column width.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(this.FieldID, FIELD_LENGTH);
			}
		}
		#endregion

		#region IHeaderField Members
		public object GetDataValue(TransactionDO transaction)
		{
			switch (eqNumber)
			{
				case 1:
					return transaction.DestinationEQ1.RegistrationID;
				case 2:
					return transaction.DestinationEQ2.RegistrationID;
				case 3:
					return transaction.DestinationEQ3.RegistrationID;
			}
			return null;
		}

		public string GetDataText(TransactionDO transaction)
		{
			switch (eqNumber)
			{
				case 1:
					return GetDataText(transaction.DestinationEQ1);
				case 2:
					return GetDataText(transaction.DestinationEQ2);
				case 3:
					return GetDataText(transaction.DestinationEQ3);
			}
			return null;
		}

		public void SetValue(object newValue)
		{
			var equipment = newValue as string;
			EquipmentDO equipmentDO = null;

			switch (eqNumber)
			{
				case 1:
					equipmentDO = trans.DestinationEQ1;
					break;
				case 2:
					equipmentDO = trans.DestinationEQ2;
					break;
				case 3:
					equipmentDO = trans.DestinationEQ3;
					break;
			}

			if (equipmentDO != null)
			{
				Guid priorEquipmentGuid = equipmentDO.EquipmentGuid;

				this.SetEquipment(equipment, equipmentDO);

				if ((priorEquipmentGuid != Guid.Empty && equipmentDO.EquipmentGuid == Guid.Empty) || equipmentDO.EquipmentGuid != Guid.Empty)
				{
					if (this.transContext.aliasClass.TransactionFieldCollection.Find("DestinationSerialNumber" + this.eqNumber) != null)
					{
						var destinationSerialNumberFG = this.fieldGenerator.GetFieldGenerator("DestinationSerialNumber" + this.eqNumber) as DestinationSerialNumberFG;

						if (destinationSerialNumberFG != null)
						{
							destinationSerialNumberFG.SetValue(equipmentDO.SerialNumber);
						}
					}

					if (this.transContext.aliasClass.TransactionFieldCollection.Find("DestinationEquipmentModel" + this.eqNumber) != null)
					{
						var destinationEquipmentModelFG = this.fieldGenerator.GetFieldGenerator("DestinationEquipmentModel" + this.eqNumber) as DestinationEquipmentModelFG;

						if (destinationEquipmentModelFG != null)
						{
							destinationEquipmentModelFG.SetValue(equipmentDO.EquipmentModel);
						}
					}

					if (this.transContext.aliasClass.TransactionFieldCollection.Find("DestinationEquipmentType" + this.eqNumber) != null)
					{
						var destinationEquipmentTypeFG = this.fieldGenerator.GetFieldGenerator("DestinationEquipmentType" + this.eqNumber) as DestinationEquipmentTypeFG;

						if (destinationEquipmentTypeFG != null)
						{
							destinationEquipmentTypeFG.SetValue(equipmentDO.EquipmentType);
						}
					}
				}
			}

			OnFieldChanged();
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
            this.trans = transaction;
            SetValue(newValue);

			EquipmentDO equipmentDO = null;
			switch (eqNumber)
			{
				case 1:
					equipmentDO = transaction.DestinationEQ1;
					break;
				case 2:
					equipmentDO = transaction.DestinationEQ2;
					break;
				case 3:
					equipmentDO = transaction.DestinationEQ3;
					break;
			}

			if (equipmentDO != null && equipmentDO.EquipmentGuid != Guid.Empty)
			{
				EquipmentClass equipment =
					FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(x => x.Get(transContext.security, equipmentDO.EquipmentGuid));

				// Fuel Card Functionality pertains to destination equipment on disbursement
				if ((this.transContext.aliasClass.TransTypeID == TransactionTypes.T5_PrimaryDisbursement
					 || this.transContext.aliasClass.TransTypeID == TransactionTypes.T6_SecondaryDisbursement)
					&& transContext.aliasClass.TransactionFieldCollection.Find("FuelCardID") != null)
				{
					if ((transaction.FuelCardGuid == Guid.Empty
						 || (transaction.FuelCardGuid != Guid.Empty && transaction.FuelCardGuid != equipment.FuelCardGuid))
						&& equipment.FuelCardGuid != Guid.Empty)
					{
						Guid identityGuid =
							FMChannelHelper.MakeCall<IFuelCards, Guid>(x => x.GetIdentityGuid(transContext.security, equipment.FuelCardID));

						if (identityGuid == equipment.FuelCardGuid)
						{
							var fuelCardFG = fieldGenerator.GetFieldGenerator("FuelCardID") as FuelCardFG;

							if (fuelCardFG != null)
							{
								fuelCardFG.SetDataValue(transaction, equipment.FuelCardID);
							}
						}
					}
				}

				if (equipment.ProductGuid != Guid.Empty &&
					!transContext.aliasClass.MultipleLineItems &&
					(transaction.LineItems[0].ProductGuid == Guid.Empty || transaction.LineItems[0].ProductGuid != equipment.ProductGuid))
				{
					if (this.ShouldSetProductForEquipment(equipment))
					{
						var productFG = fieldGenerator.GetFieldGenerator("LineItem Product") as LineItemProductFG;

						if (productFG != null)
						{
							productFG.SetDataValue(transaction.LineItems[0], equipment.ProductID);
						}
					}
				}
			}
		}

		protected virtual bool ShouldSetProductForEquipment(EquipmentClass equipment)
		{
			Guid identityGuid =
						FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetIdentityGuid(transContext.security, equipment.ProductID));

			return identityGuid == equipment.ProductGuid;
		}
		#endregion
	}
}
