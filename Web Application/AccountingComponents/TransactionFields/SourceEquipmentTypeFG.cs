namespace TransactionFields
{
	using System;
	using System.Collections.Specialized;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Source Equipment Type field generator class.
	/// </summary>
	public class SourceEquipmentTypeFG : EquipmentTypeFG, IHeaderField
	{
		#region Contructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		/// <param name="equipmentNumber">It can be 1, 2, or 3.</param>
		public SourceEquipmentTypeFG(byte equipmentNumber) : base(false, equipmentNumber)
		{
			this.autoPostBack = true;
		}
		#endregion

		#region Properties
		public override string FieldID
		{
			get { return "SourceEquipmentType" + this.equipmentNumber; }
		}

		protected override short MaxColumns
		{
			get { return 20; }
		}

		public override bool Editable
		{
			get
			{
				return this.transContext.aliasClass.PermitNonReferenceData;
			}
			set { }
		}
		#endregion

		/// <summary>
		/// This method will override the Get Entries for source equipment.
		/// </summary>
		/// <returns>A hydrid dictionary of the equipment type keys/values</returns>
		public override HybridDictionary GetEntries()
		{
			var listEntries = new HybridDictionary();
			bool useDataDictionary = this.transContext.useDataDictonary;

			EQUIPMENT_TYPE[] types = transContext.aliasClass.GetEquipmentTypes(false, equipmentNumber);

			foreach(EQUIPMENT_TYPE type in types)
			{
				if (type == EQUIPMENT_TYPE.COMPARTMENT_TYPE)
				{
					continue;
				}

				if (useDataDictionary)
				{
					listEntries.Add(this.GetDataDictionaryValueByKey(this.transContext.accountingSite.CurrentSiteGuid, 
									EquipmentTypeClass.TypeID(type)),
									EquipmentTypeClass.TypeID(type));
				}
				else
				{
					listEntries.Add(EquipmentTypeClass.TypeID(type),EquipmentTypeClass.TypeID(type));
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
                    equipmentDO = this.trans.SourceEQ1;
                    break;
                case 2:
                    equipmentDO = this.trans.SourceEQ2;
                    break;
                case 3:
                    equipmentDO = this.trans.SourceEQ3;
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
					return transaction.SourceEQ1.EquipmentType;
				case 2:
					return transaction.SourceEQ2.EquipmentType;
				case 3:
					return transaction.SourceEQ3.EquipmentType;
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
					return transaction.SourceEQ1.EquipmentType;
				case 2:
					return transaction.SourceEQ2.EquipmentType;
				case 3:
					return transaction.SourceEQ3.EquipmentType;
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
			DestinationEquipmentFG sourceFG = null;
			EquipmentDO equipmentDO = null;

			switch (equipmentNumber)
			{
                case 1:
                    sourceFG = fieldGenerator.GetFieldGenerator("SourceRegistrationID1") as DestinationEquipmentFG;
                    equipmentDO = transaction.SourceEQ1;
                    break;
                case 2:
                    sourceFG = fieldGenerator.GetFieldGenerator("SourceRegistrationID2") as DestinationEquipmentFG;
                    equipmentDO = transaction.SourceEQ2;
                    break;
                case 3:
                    sourceFG = fieldGenerator.GetFieldGenerator("SourceRegistrationID3") as DestinationEquipmentFG;
                    equipmentDO = transaction.SourceEQ3;
                    break;
			}

			if (equipmentDO != null)
			{
				equipmentDO.EquipmentType = newValue as String;

				if (sourceFG != null)
				{
					if (equipmentDO.EquipmentGuid != Guid.Empty)
					{
						sourceFG.SetValue(string.Empty);
					}
					else
					{
						sourceFG.SetValue(equipmentDO.RegistrationID);
					}
				}
			}

			OnFieldChanged();
		}
		#endregion
	}
}
