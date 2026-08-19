using System;
using System.Collections;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemEquipmentFG.
	/// </summary>
	abstract public class LineItemEquipmentFG : BaseEquipmentFG
	{
		public LineItemEquipmentFG(bool Destination) : base(Destination, 1)
		{
			this.autoPostBack=false;
		}

		#region Override methods
		protected override EquipmentInfo [] GetEntries()
		{
			ArrayList equipmentDOs = new ArrayList();
			ArrayList equipmentInfoArray = new ArrayList();

			if(destination)
			{
				if(transContext.aliasClass.TransactionFieldCollection.Find("DestinationRegistrationID1") != null)
					equipmentDOs.Add(trans.DestinationEQ1);
				if(transContext.aliasClass.TransactionFieldCollection.Find("DestinationRegistrationID2") != null)
					equipmentDOs.Add(trans.DestinationEQ2);
				if(transContext.aliasClass.TransactionFieldCollection.Find("DestinationRegistrationID3") != null)
					equipmentDOs.Add(trans.DestinationEQ3);
			}

			else
			{
				if(transContext.aliasClass.TransactionFieldCollection.Find("SourceRegistrationID1") != null)
					equipmentDOs.Add(trans.SourceEQ1);
				if(transContext.aliasClass.TransactionFieldCollection.Find("SourceRegistrationID2") != null)
					equipmentDOs.Add(trans.SourceEQ2);
				if(transContext.aliasClass.TransactionFieldCollection.Find("SourceRegistrationID3") != null)
					equipmentDOs.Add(trans.SourceEQ3);
			}

			if(equipmentDOs.Count > 0)
			{
				foreach(EquipmentDO equipmentDO in equipmentDOs)
				{
					if(equipmentDO.EquipmentGuid != Guid.Empty
					&& EquipmentTypeClass.HasCompartments(EquipmentTypeClass.Type(equipmentDO.EquipmentType)))
					{
						EquipmentInfo equipmentInfo=new EquipmentInfo();
						equipmentInfo.ID=equipmentDO.RegistrationID;
						equipmentInfo.Xref=equipmentDO.EquipmentRefID;
						equipmentInfoArray.Add(equipmentInfo);
					}
				}
				return equipmentInfoArray.ToArray(typeof(EquipmentInfo)) as EquipmentInfo [];
			}

			EQUIPMENT_TYPE[] types = transContext.aliasClass.GetEquipmentTypes(destination,eqNumber);

			return FMChannelHelper.MakeCall<IEquipments, EquipmentInfo[]>(
																	 x =>
																	 x.EnumerateInfoByTypesCompanyFuelCardProductAndSecondaryStorage(transContext.security, types, null, null, null, null, hideHiddenEquipmentRecords: true)
																);
		}

		#endregion
	}
}
