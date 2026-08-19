/*****************************************************************************
SourceEquipmentFG

Original Author: ?
Revisions: See source control comments

(C) Copyright 2008 by Varec, Inc.  All rights reserved.

Revision History
Date:		By:					Reason:
-----		---					-------
11/21/2008	V. Thompson			Made Get/Set value methods virtual
//*****************************************************************************/
namespace TransactionFields
{
	using System;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for SourceEquipmentFG.
	/// </summary>
	public class SourceEquipmentFG : HeaderEquipmentFG, IHeaderField
	{
		#region Contructors
		public SourceEquipmentFG(byte equipmentNumber)
			: base(false, equipmentNumber)
		{
			this.autoPostBack = true;
		}
		#endregion

		#region Override Properties
		public override string FieldID
		{
			get
			{
				return "SourceRegistrationID" + eqNumber;
			}
		}

		/// <summary>
		/// This property returns the field's maximum column width.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, FIELD_LENGTH);
			}
		}
		#endregion

		#region Public Properties
		public void SetValue(object newValue)
		{
			var equipment = newValue as string;
			EquipmentDO equipmentDO = null;

			switch (eqNumber)
			{
				case 1:
					equipmentDO = trans.SourceEQ1;
					break;
				case 2:
					equipmentDO = trans.SourceEQ2;
					break;
				case 3:
					equipmentDO = trans.SourceEQ3;
					break;
			}

			if (equipmentDO != null)
			{
				Guid priorEquipmentGuid = equipmentDO.EquipmentGuid;

				this.SetEquipment(equipment, equipmentDO);

				if ((priorEquipmentGuid != Guid.Empty && equipmentDO.EquipmentGuid == Guid.Empty)
					|| equipmentDO.EquipmentGuid != Guid.Empty)
				{
					if (this.transContext.aliasClass.TransactionFieldCollection.Find("SourceSerialNumber" + this.eqNumber) != null)
					{
						var sourceSerialNumberFG = this.fieldGenerator.GetFieldGenerator("SourceSerialNumber" + this.eqNumber) as SourceSerialNumberFG;
						
						if (sourceSerialNumberFG != null)
						{
							sourceSerialNumberFG.SetValue(equipmentDO.SerialNumber);
						}
					}

					if (this.transContext.aliasClass.TransactionFieldCollection.Find("SourceEquipmentModel" + this.eqNumber) != null)
					{
						var sourceEquipmentModelFG = this.fieldGenerator.GetFieldGenerator("SourceEquipmentModel" + this.eqNumber) as SourceEquipmentModelFG;
						
						if (sourceEquipmentModelFG != null)
						{
							sourceEquipmentModelFG.SetValue(equipmentDO.EquipmentModel);
						}
					}

					if (this.transContext.aliasClass.TransactionFieldCollection.Find("SourceEquipmentType" + this.eqNumber) != null)
					{
						var sourceEquipmentTypeFG = this.fieldGenerator.GetFieldGenerator("SourceEquipmentType" + this.eqNumber) as SourceEquipmentTypeFG;
						
						if (sourceEquipmentTypeFG != null)
						{
							sourceEquipmentTypeFG.SetValue(equipmentDO.EquipmentType);
						}
					}
				}
			}

			OnFieldChanged();
		}
		#endregion

		#region IHeaderField Members
		public virtual object GetDataValue(TransactionDO transaction)
		{
			switch (eqNumber)
			{
				case 1:
					return transaction.SourceEQ1.RegistrationID;
				case 2:
					return transaction.SourceEQ2.RegistrationID;
				case 3:
					return transaction.SourceEQ3.RegistrationID;
			}

			return null;
		}

		public virtual string GetDataText(TransactionDO transaction)
		{
			switch (eqNumber)
			{
				case 1:
					return GetDataText(transaction.SourceEQ1);
				case 2:
					return GetDataText(transaction.SourceEQ2);
				case 3:
					return GetDataText(transaction.SourceEQ3);
			}

			return null;
		}

		public virtual void SetDataValue(TransactionDO transaction, object newValue)
		{
			SetValue(newValue);

			EquipmentDO equipmentDO = null;
			switch (eqNumber)
			{
				case 1:
					equipmentDO = transaction.SourceEQ1;
					break;
				case 2:
					equipmentDO = transaction.SourceEQ2;
					break;
				case 3:
					equipmentDO = transaction.SourceEQ3;
					break;
			}

			if (equipmentDO != null && equipmentDO.EquipmentGuid != Guid.Empty)
			{
				EquipmentClass equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
																 x =>
																 x.Get(transContext.security, equipmentDO.EquipmentGuid)
															);

				// Fuel Card Functionality pertains to source equipment on defuels
				if ((this.transContext.aliasClass.TransTypeID == TransactionTypes.T3_PrimaryDefuel
					 || this.transContext.aliasClass.TransTypeID == TransactionTypes.T4_SecondaryDefuel)
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

				if (equipment.ProductGuid != Guid.Empty
					&& !transContext.aliasClass.MultipleLineItems
					&& (transaction.LineItems[0].ProductGuid == Guid.Empty
					|| transaction.LineItems[0].ProductGuid != equipment.ProductGuid))
				{
					Guid identityGuid = FMChannelHelper.MakeCall<IProducts, Guid>(
																	x =>
																	x.GetIdentityGuid(transContext.security, equipment.ProductID)
															);

					if (identityGuid == equipment.ProductGuid)
					{
						var productFG = fieldGenerator.GetFieldGenerator("LineItem Product") as LineItemProductFG;

						if (productFG != null)
						{
							productFG.SetDataValue(transaction.LineItems[0] as LineItemDO, equipment.ProductID);
						}
					}
				}
			}
		}
		#endregion
	}
}
