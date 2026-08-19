namespace TransactionFields
{
	using System;
	using System.Collections.Specialized;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	public class DestinationEquipmentTypeFG : EquipmentTypeFG, IHeaderField
	{
		#region Contructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		/// <param name="equipmentNumber">It can be 1, 2, or 3.</param>
		public DestinationEquipmentTypeFG(byte equipmentNumber) : base(true, equipmentNumber)
		{
			this.autoPostBack = true;
		}
		#endregion

		#region Properties
		public override string FieldID
		{
			get { return "DestinationEquipmentType" + this.equipmentNumber; }
		}

		protected override short MaxColumns
		{
			get { return 20; }
		}

		public override bool Editable
		{
			get { return this.transContext.aliasClass.PermitNonReferenceData; }
			set { }
		}
		#endregion

		/// <summary>
		/// This method will override the Get Entries for destination equipment.
		/// </summary>
		/// <returns>A hydrid dictionary of the equipment type keys/values</returns>
		public override HybridDictionary GetEntries()
		{
			var listEntries = new HybridDictionary();
			bool useDataDictionary = this.transContext.useDataDictonary;

			EQUIPMENT_TYPE[] equipmentTypes = transContext.aliasClass.GetEquipmentTypes(true, equipmentNumber);

			foreach (EQUIPMENT_TYPE equipmentType in equipmentTypes)
			{
				if (equipmentType == EQUIPMENT_TYPE.COMPARTMENT_TYPE)
				{
					continue;
				}

				if (useDataDictionary)
				{
					listEntries.Add(this.GetDataDictionaryValueByKey(
																	this.transContext.accountingSite.CurrentSiteGuid,
																	EquipmentTypeClass.TypeID(equipmentType)),
																	EquipmentTypeClass.TypeID(equipmentType));
				}
				else
				{
					listEntries.Add(EquipmentTypeClass.TypeID(equipmentType), EquipmentTypeClass.TypeID(equipmentType));
				}
			}

			return listEntries;
		}

		/// <summary>
		/// This method will set the value of the selected equipment type.
		/// </summary>
		/// <param name="newValue">New equipment type value.</param>
		public void SetValue(object newValue)
		{
			EquipmentDO equipmentDO = null;
			switch (equipmentNumber)
			{
				case 1:
					equipmentDO = this.trans.DestinationEQ1;
					break;
				case 2:
					equipmentDO = this.trans.DestinationEQ2;
					break;
				case 3:
					equipmentDO = this.trans.DestinationEQ3;
					break;
			}

			if (equipmentDO != null)
			{
				equipmentDO.EquipmentType = newValue as string;

				this.SetEquipmentType(newValue);
			}
		}

		#region IHeaderField Members
		/// <summary>
		/// This method will return the equipment type from the transaction object.
		/// </summary>
		/// <param name="transaction">Transaction data object.</param>
		/// <returns>An object that contains the value of the equipment type.</returns>
		public object GetDataValue(TransactionDO transaction)
		{
			switch (equipmentNumber)
			{
				case 1:
					return transaction.DestinationEQ1.EquipmentType;
				case 2:
					return transaction.DestinationEQ2.EquipmentType;
				case 3:
					return transaction.DestinationEQ3.EquipmentType;
			}

			return null;
		}

		/// <summary>
		/// This method will return the equipment type from the transaction object.
		/// </summary>
		/// <param name="transaction">Transaction data object.</param>
		/// <returns>An string that contains the value of the equipment type.</returns>
		public string GetDataText(TransactionDO transaction)
		{
			switch (equipmentNumber)
			{
				case 1:
					return transaction.DestinationEQ1.EquipmentType;
				case 2:
					return transaction.DestinationEQ2.EquipmentType;
				case 3:
					return transaction.DestinationEQ3.EquipmentType;
			}

			return null;
		}

		/// <summary>
		/// This method will set the value of the selected equipment type.
		/// </summary>
		/// <param name="transaction">Transaction data object.</param>
		/// <param name="newValue">New equipment type value.</param>
		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			DestinationEquipmentFG destinationFG = null;
			EquipmentDO equipmentDO = null;
			switch (equipmentNumber)
			{
				case 1:
					destinationFG = fieldGenerator.GetFieldGenerator("DestinationRegistrationID1") as DestinationEquipmentFG;
					equipmentDO = transaction.DestinationEQ1;
					break;
				case 2:
					destinationFG = fieldGenerator.GetFieldGenerator("DestinationRegistrationID2") as DestinationEquipmentFG;
					equipmentDO = transaction.DestinationEQ2;
					break;
				case 3:
					destinationFG = fieldGenerator.GetFieldGenerator("DestinationRegistrationID3") as DestinationEquipmentFG;
					equipmentDO = transaction.DestinationEQ3;
					break;
			}

			if (equipmentDO != null)
			{
				equipmentDO.EquipmentType = newValue as String;

				if (destinationFG != null)
				{
					if (equipmentDO.EquipmentGuid != Guid.Empty)
					{
						destinationFG.SetValue(string.Empty);
					}
					else
					{
						destinationFG.SetDataValue(trans, equipmentDO.RegistrationID);
					}
				}
			}

			OnFieldChanged();
		}
		#endregion
	}
}
