namespace TransactionFields
{
	using System;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Constants;

	/// <summary>
	/// Summary description for LoadID_FG.
	/// </summary>
	public class LoadID_FG : TextFieldGenerator, IHeaderField
	{

		public override string FieldID
		{
			get
			{
				return "LoadID";
			}
		}

		public override void Generate(bool editable)
		{
			base.Generate(editable);

			var textBox = cell.FindControl(this.ID) as TextBox;

			if (textBox != null)
			{
				textBox.AutoPostBack = true;
				textBox.TextChanged += this.TextChanged;
			}
		}

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.LoadID;
		}


		public string GetDataText(TransactionDO transaction)
		{
			if (GetDataValue(transaction) != null)
			{
				return GetDataValue(transaction).ToString();
			}
			
			return null;
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			transaction.LoadID = newValue as string;
			OnFieldChanged();
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 50.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, 50);
			}
		}

		private void TextChanged(object sender, System.EventArgs e)
		{
			try
			{
				var loadIDTextBox = cell.FindControl(this.ID) as TextBox;

				if (loadIDTextBox == null || string.IsNullOrWhiteSpace(loadIDTextBox.Text))
				{
					return;
				}

				// Presently LoadID is applicable 
				if (trans.TransTypeID == TransactionTypes.T8_Receipt || 
					trans.TransTypeID == TransactionTypes.T18_SupplyOrder || 
					trans.TransTypeID == TransactionTypes.T9_Request)
				{
					var managerFG = fieldGenerator.GetFieldGenerator("ManagerID") as CompanyTextButtonGenerator;
					var ownerFG = fieldGenerator.GetFieldGenerator("OwnerID") as CompanyTextButtonGenerator;
					var supplierFG = fieldGenerator.GetFieldGenerator("SupplierID") as CompanyTextButtonGenerator;

					Guid loadIDShipToMapGuid = (loadIDTextBox.Text == string.Empty) ? Guid.Empty :
																	FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
																			x =>
																			x.GetOffLoadIdentityGuidByMapID(transContext.security, loadIDTextBox.Text)
																	);
					if (loadIDShipToMapGuid == Guid.Empty)
					{
						if (managerFG != null)
						{
							managerFG.SetValue(string.Empty);
						}

						if (ownerFG != null)
						{
							ownerFG.SetValue(string.Empty);
						}

						if (supplierFG != null)
						{
							supplierFG.SetValue(string.Empty);
						}
					}
					else
					{
						CompanyMapClass loadidMap = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapClass>(
														x =>
														x.Get(transContext.security, loadIDShipToMapGuid, COMPANY_MAP_TYPE.OFFLOADID_SUPPLIER_MAP)
												);

						if (loadidMap == null || loadidMap.IdentityGuid == Guid.Empty)
						{
							return;
						}

						CompanyMapClass supplierMap = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapClass>(
														x =>
														x.Get(transContext.security, loadidMap.AssignedToGuid, COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP)
												);

						if (supplierMap == null || supplierMap.IdentityGuid == Guid.Empty)
						{
							return;
						}

						if (supplierFG != null)
						{
							supplierFG.SetValue(supplierMap.AssignedID);
						}

						CompanyMapClass ownerMap = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapClass>(
												x =>
												x.Get(transContext.security, supplierMap.AssignedToGuid, COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP)
										);

						if (ownerMap == null || ownerMap.IdentityGuid == Guid.Empty)
						{
							return;
						}

						if (ownerFG != null)
						{
							ownerFG.SetValue(ownerMap.AssignedID);
						}

						if (managerFG != null)
						{
							managerFG.SetValue(ownerMap.AssignedToID);
						}
					}
				}
				else if (trans.TransTypeID == TransactionTypes.T5_PrimaryDisbursement
						|| trans.TransTypeID == TransactionTypes.T6_SecondaryDisbursement
						|| trans.TransTypeID == TransactionTypes.T25_Shipment
						|| trans.TransTypeID == TransactionTypes.T11_ConsumerTransfer
						|| trans.TransTypeID == TransactionTypes.T17_Order)
				{


					var managerFG = fieldGenerator.GetFieldGenerator("ManagerID") as CompanyTextButtonGenerator;
					var ownerFG = fieldGenerator.GetFieldGenerator("OwnerID") as CompanyTextButtonGenerator;
					var shipperFG = fieldGenerator.GetFieldGenerator("ShipperID") as CompanyTextButtonGenerator;
					var billtoFG = fieldGenerator.GetFieldGenerator("BillToID") as CompanyTextButtonGenerator;
					var shiptoFG = fieldGenerator.GetFieldGenerator("ShipToID") as CompanyTextButtonGenerator;

					Guid loadIDShipToMapGuid = (loadIDTextBox.Text == string.Empty) ? Guid.Empty :
						FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
																	 x =>
																	 x.GetIdentityGuidByMapID(transContext.security, loadIDTextBox.Text)
																);
					
					if (loadIDShipToMapGuid == Guid.Empty)
					{
						if (managerFG != null)
						{
							managerFG.SetValue(string.Empty);
						}

						if (ownerFG != null)
						{
							ownerFG.SetValue(string.Empty);
						}

						if (shipperFG != null)
						{
							shipperFG.SetValue(string.Empty);
						}

						if (billtoFG != null)
						{
							billtoFG.SetValue(string.Empty);
						}

						if (shiptoFG != null)
						{
							shiptoFG.SetValue(string.Empty);
						}
					}
					else
					{
						CompanyMapClass loadidMap = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapClass>(
															x =>
															x.Get(transContext.security, loadIDShipToMapGuid, COMPANY_MAP_TYPE.LOADID_SHIPTO_MAP)
													);

						if (loadidMap == null || loadidMap.IdentityGuid == Guid.Empty)
						{
							return;
						}

						CompanyMapClass shiptoMap =
							FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapClass>(
								x => x.Get(transContext.security, loadidMap.AssignedToGuid, COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP));

						if (shiptoMap == null || shiptoMap.IdentityGuid == Guid.Empty)
						{
							return;
						}

						if (shiptoFG != null)
						{
							shiptoFG.SetValue(shiptoMap.AssignedID);
						}

						CompanyMapClass billtoMap =
							FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapClass>(
								x => x.Get(transContext.security, shiptoMap.AssignedToGuid, COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP));

						if (billtoMap == null || billtoMap.IdentityGuid == Guid.Empty)
						{
							return;
						}

						if (billtoFG != null)
						{
							billtoFG.SetValue(billtoMap.AssignedID);
						}

						CompanyMapClass shipperMap =
							FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapClass>(
								x => x.Get(transContext.security, billtoMap.AssignedToGuid, COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP));

						if (shipperMap == null || shipperMap.IdentityGuid == Guid.Empty)
						{
							return;
						}

						if (shipperFG != null)
						{
							shipperFG.SetValue(shipperMap.AssignedID);
						}

						CompanyMapClass ownerMap =
							FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapClass>(
								x => x.Get(transContext.security, shipperMap.AssignedToGuid, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP));

						if (ownerMap == null || ownerMap.IdentityGuid == Guid.Empty)
						{
							return;
						}

						if (ownerFG != null)
						{
							ownerFG.SetValue(ownerMap.AssignedID);
						}

						if (managerFG != null)
						{
							managerFG.SetValue(ownerMap.AssignedToID);
						}
					}
				}
			}
			catch (Exception except)
			{
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(except.ToString(), FMEventLogEntryType.Error));
			}
		}
	}
}
