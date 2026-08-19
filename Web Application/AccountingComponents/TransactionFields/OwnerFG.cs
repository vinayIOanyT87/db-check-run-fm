namespace TransactionFields
{
	using System;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	public class OwnerFG : CompanyTextButtonGenerator, IHeaderField
	{
		#region Contructors
		/// <summary>
		/// This is the default constructor for the OwnerFG class.
		/// </summary>
		public OwnerFG()
		{
			this.companyRole = OWNER_ROLE;
		}
		#endregion

		#region Override Properties
		/// <summary>
		/// This property returns the AutoPostBack
		/// </summary>
		protected override bool AutoPostBack
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// This property returns the field ID
		/// </summary>
		public override string FieldID
		{
			get
			{
				return "OwnerID";
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

		public override bool Required
		{
			get
			{
				switch (trans.TransTypeID)
				{
					case TransactionTypes.T1_PrimaryAdjustment:
					case TransactionTypes.T2_SecondaryAdjustment:
					case TransactionTypes.T3_PrimaryDefuel:
					case TransactionTypes.T4_SecondaryDefuel:
					case TransactionTypes.T5_PrimaryDisbursement:
					case TransactionTypes.T6_SecondaryDisbursement:
					case TransactionTypes.T7_FillStand:
					case TransactionTypes.T8_Receipt:
					case TransactionTypes.T9_Request:
					case TransactionTypes.T10_Unload:
					case TransactionTypes.T11_ConsumerTransfer:
					case TransactionTypes.T12_InventoryNotAffected:
					case TransactionTypes.T15_PrimaryRegrade:
					case TransactionTypes.T16_SecondaryRegrade:
					case TransactionTypes.T13_OwnerTransfer:
					case TransactionTypes.T17_Order:
					case TransactionTypes.T18_SupplyOrder:
					case TransactionTypes.T23_StorageTransfer:
					case TransactionTypes.T25_Shipment:
						return true;

					case TransactionTypes.T14_PhysicalInventory:
						return false;


					default:
						return false;
				}
			}
		}
		#endregion

		#region Override Methods
		protected override CompanyCollectionClass GetEntries()
		{
            Guid fuelCardGuid = Guid.Empty;
            if (transContext.aliasClass.TransactionFieldCollection.Find("FuelCardID") != null)
            {
                fuelCardGuid = trans.FuelCardGuid;
            }
            CompanyCollectionClass companyCollection = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
                                                                     x =>
                                                                     x.GetEntriesForFieldGeneratorByRole(transContext.security, COMPANY_ROLE.OWNER, trans.OwnerCompanyGuid, fuelCardGuid, hideHiddenCompanies: true));

			if (transContext.aliasClass.LimitSelectionsBasedOnHierarchy)
			{
				if (trans.ManagerCompanyGuid == Guid.Empty)
				{
					if (transContext.aliasClass.TransactionFieldCollection.Find("ManagerID") != null)
						companyCollection.Clear();
				}
				else
				{
					CompanyMapCollectionClass companyMapCollection;

					if (trans.TransTypeID == TransactionTypes.T5_PrimaryDisbursement
						|| trans.TransTypeID == TransactionTypes.T6_SecondaryDisbursement
						|| trans.TransTypeID == TransactionTypes.T10_Unload
						|| trans.TransTypeID == TransactionTypes.T17_Order)
					{
						companyMapCollection = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
																	 x =>
																	 x.EnumerateByAssignedToGuidAndType(transContext.security, trans.ManagerCompanyGuid, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP)
																);
					}
					else
					{
						companyMapCollection = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
																	 x =>
																	 x.EnumerateByAssignedToGuidAndType(transContext.security, trans.ManagerCompanyGuid, COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP)
																);
					}

					var limitedCompanyCollection = new CompanyCollectionClass();

					foreach (CompanyClass company in companyCollection)
					{
						if (companyMapCollection.Find(company.MasterRecordGuid) == null)
						{
							continue;
						}

						limitedCompanyCollection.Add(company);
					}

					companyCollection = limitedCompanyCollection;
				}
			}

			return companyCollection;
		}

		protected override void SetCompanyID(TransactionDO transaction, string newID)
		{
			transaction.OwnerID = newID;
		}

		protected override void SetCompanyCode(TransactionDO transaction, string newCode)
		{
			transaction.OwnerCode = newCode;
		}

		protected override void SetCompanyGuid(TransactionDO trans, Guid newGuid)
		{
			trans.OwnerCompanyGuid = newGuid;
		}

		public override object GetDataValue(TransactionDO transaction)
		{
			return transaction.OwnerID;
		}

		public override void SetDataValue(TransactionDO transaction, object newValue)
		{
			this.SetValue(newValue);

			if (transContext.aliasClass.LimitSelectionsBasedOnHierarchy)
			{
				if (transaction.TransTypeID == TransactionTypes.T8_Receipt
					|| transaction.TransTypeID == TransactionTypes.T18_SupplyOrder
					|| transaction.TransTypeID == TransactionTypes.T9_Request)
				{
					if (transContext.aliasClass.TransactionFieldCollection.Find("SupplierID") == null)
					{
						return;
					}

					var supplierFG = fieldGenerator.GetFieldGenerator("SupplierID") as CompanyTextButtonGenerator;

					if (transaction.OwnerCompanyGuid == Guid.Empty || transaction.ManagerCompanyGuid == Guid.Empty)
					{
						if (supplierFG != null)
						{
							supplierFG.SetDataValue(transaction, "");
						}

						return;
					}

					Guid companyMapGuid = FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
																	 x =>
																	 x.GetIdentityGuidByGuidsAndType(transContext.security, transaction.ManagerCompanyGuid,
																		transaction.OwnerCompanyGuid, COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP)
																);

					if (companyMapGuid == Guid.Empty)
					{
						if (supplierFG != null)
						{
							supplierFG.SetDataValue(transaction, "");
						}

						return;
					}

					CompanyMapCollectionClass companyMapCollection = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
						x =>
							x.EnumerateByAssignedToGuidAndType(this.transContext.security, companyMapGuid, COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP)
						);


					if (companyMapCollection.Count == 0)
					{
						if (supplierFG != null)
						{
							supplierFG.SetDataValue(transaction, string.Empty);
						}
					}
					else if (companyMapCollection.Count == 1)
					{
						if (transaction.SupplierCompanyGuid == Guid.Empty || transaction.SupplierCompanyGuid != companyMapCollection[0].AssignedGuid)
						{
							if (supplierFG != null)
							{
								supplierFG.SetDataValue(transaction, companyMapCollection[0].AssignedID);
							}
						}
					}
					else if (transaction.SupplierCompanyGuid == Guid.Empty || companyMapCollection.Find(transaction.SupplierCompanyGuid) == null)
					{
						if (supplierFG != null)
						{
							supplierFG.SetDataValue(transaction, "");
						}
					}

				}
				else if (transaction.TransTypeID == TransactionTypes.T5_PrimaryDisbursement
						|| transaction.TransTypeID == TransactionTypes.T6_SecondaryDisbursement
						|| transaction.TransTypeID == TransactionTypes.T11_ConsumerTransfer
						|| transaction.TransTypeID == TransactionTypes.T17_Order
						|| transaction.TransTypeID == TransactionTypes.T25_Shipment)
				{
					if (transContext.aliasClass.TransactionFieldCollection.Find("ShipperID") == null)
					{
						return;
					}

					var shipperFG = fieldGenerator.GetFieldGenerator("ShipperID") as CompanyTextButtonGenerator;

					if (transaction.OwnerCompanyGuid == Guid.Empty || transaction.ManagerCompanyGuid == Guid.Empty)
					{
						if (shipperFG != null)
						{
							shipperFG.SetDataValue(transaction, "");
						}
						
						return;
					}

					Guid companyMapGuid = FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
																	 x =>
																	 x.GetIdentityGuidByGuidsAndType(transContext.security, transaction.ManagerCompanyGuid, transaction.OwnerCompanyGuid,
																		COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP)
																);

					if (companyMapGuid == Guid.Empty)
					{
						if (shipperFG != null)
						{
							shipperFG.SetDataValue(transaction, string.Empty);
						}

						return;
					}

					CompanyMapCollectionClass companyMapCollection = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
																	 x =>
																	 x.EnumerateByAssignedToGuidAndType(transContext.security, companyMapGuid, COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP)
																);

					if (companyMapCollection.Count == 0)
					{
						if (shipperFG != null)
						{
							shipperFG.SetDataValue(transaction, string.Empty);
						}
					}
					else if (companyMapCollection.Count == 1)
					{
						if (transaction.ShipperCompanyGuid == Guid.Empty
						    || transaction.ShipperCompanyGuid != companyMapCollection[0].AssignedGuid)
						{
							if (shipperFG != null)
							{
								shipperFG.SetDataValue(transaction, companyMapCollection[0].AssignedID);
							}
						}
					}
					else if (transaction.ShipperCompanyGuid == Guid.Empty
					         || companyMapCollection.Find(transaction.ShipperCompanyGuid) == null)
					{
						if (shipperFG != null)
						{
							shipperFG.SetDataValue(transaction, string.Empty);
						}
					}
				}
			}
			else if (transaction.OwnerCompanyGuid != Guid.Empty)
			{
				CompanyMapCollectionClass ownerManagerMaps;

				if (transContext.aliasClass.TransTypeID == TransactionTypes.T8_Receipt || transContext.aliasClass.TransTypeID == TransactionTypes.T18_SupplyOrder)
				{
					ownerManagerMaps = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
																	 x =>
																	 x.EnumerateByAssignedGuidAndType(transContext.security, transaction.OwnerCompanyGuid, COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP)
																);
				}
				else
				{
					ownerManagerMaps = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
																	 x =>
																	 x.EnumerateByAssignedGuidAndType(transContext.security, transaction.OwnerCompanyGuid, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP)
																);
				}

				if (ownerManagerMaps.Count == 1)
				{
					if (transContext.aliasClass.TransactionFieldCollection.Find("ManagerID") != null)
					{
						var managerFG = fieldGenerator.GetFieldGenerator("ManagerID") as CompanyTextButtonGenerator;

						if (managerFG != null)
						{
							managerFG.SetDataValue(transaction, ownerManagerMaps[0].AssignedToID);
						}
					}
					else
					{
						CompanyClass manager = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(transContext.security, ownerManagerMaps[0].AssignedToGuid, false)
																);

						transaction.ManagerID = manager.ID;
						transaction.ManagerCode = manager.Code;
						transaction.ManagerCompanyGuid = manager.MasterRecordGuid;
					}
				}
			}
		}
		#endregion

		#region public methods
		public string GetDataText(TransactionDO transaction)
		{
			if (GetDataValue(transaction) != null)
			{
				return GetDataValue(transaction).ToString();
			}
			
			return null;
		}

		#endregion
	}
}
