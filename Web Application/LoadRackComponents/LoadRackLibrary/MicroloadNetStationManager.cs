/******************************************************************************

	FILE NAME:		MicroloadNetStationManager.cs


	PURPOSE:			MicroloadNetStationManagerClass


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		11/24/2009	W.Gray		7.5.1.0 - Revised to implement ReadProductsUsingInjector (WI 9491) 
*******************************************************************************/

namespace LoadRackLibrary
{
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	using Opc;
	using Opc.Da;

	using System;
	using System.Collections;
    using System.Collections.Generic;
    using System.Data;
	using System.Diagnostics;

	using Factory = OpcCom.Factory;
	using Server = Opc.Da.Server;

	/// <summary>
	/// Summary description for MicroloadNetStationManager.
	/// </summary>
	public class MicroloadNetStationManager : AcculoadIIIStationManagerClass
	{
		public MicroloadNetStationManager(EventLog EventLog,
														LoadRackManagerClass LoadRackManager,
														StationClass Station,
														SiteManagerClass SiteManager,
														SecurityClass Security)
			: base(EventLog, LoadRackManager, Station, SiteManager, Security)
		{

		}

		protected override int MaxDisplayLineSize
		{
			get { return 21; }
		}

		protected override int MaxSelection
		{
			get { return 5; }
		}

		protected override int NumberOfDisplayLines
		{
			get { return 6; }
		}

		protected override string[] MenuWriteTags
		{
			get
			{
				string[] Val = {     ".Write First Line With Prompt",
											".Write Second Line",
											".Write Third Line",
											".Write Fourth Line",
											".Write Fifth Line",
											".Write Sixth Line" };

				return Val;

			}

		}

        /// <summary>
        /// Returns the number of actual load arms on a preset.  For a Microload, this is 1
        /// </summary>
        internal override int PhysicalArmsOnPreset
        {
            get
            {
                return 1;
            }
        }

        public void SetMeterFactor(int MeterFactorNumber, double MeterFactor, int FlowRate)
		{
			ProcessVariableClass StationPV = this.Station.ProcessVariableCollection[0];

			string OPCPath = this.GetStationOPCPath();

			Server Server = new Server(new Factory(), new URL(StationPV.URL));
			Server.Connect();

			ItemValue[] SubItems =  {  new ItemValue(new ItemIdentifier(OPCPath+".Arm 1.Program Code Change")),
												new ItemValue(new ItemIdentifier(OPCPath+".Arm 1.Program Code Change")),
												new ItemValue(new ItemIdentifier(OPCPath+".Arm 1.Log Out of Program Mode"))
											};

			int MeterAddress = 341 + ((MeterFactorNumber - 1) * 2);

			SubItems[0].Value = "SY " + MeterAddress.ToString("D3") + " " + MeterFactor.ToString("0.000000");
			SubItems[1].Value = "SY " + (MeterAddress + 1).ToString("D3") + " " + FlowRate.ToString("D5");

			try
			{
				Server.Write(SubItems);
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
			}

			Server.Disconnect();
			Server.Dispose();
		}

		/// <summary>
		/// This function is meant to allow remote authorization of the specified, existing transaction
		/// </summary>
		/// <param name="TransID"></param>
		/// <param name="StationManager"></param>
		public void AuthorizeTransaction(
		 StationManagerClass StationManager,
		 TransactionDO Transaction,
		 LineItemDO LineItem,
		 SubLineItemDO SubLineItem)
		{
			try
			{
				// Try to load the transaction
				this.Transaction = Transaction;

				if (this.Transaction == null)
				{
					throw new Exception("Could not find transaction");
				}

				if (LineItem == null)
				{
					throw new Exception("Must specify a line item.");
				}

				// Get the line item
				bool bFound = false;
				foreach (LineItemDO Line in this.Transaction.LineItems)
				{
					if (Line.TransactionLineItemGuid == LineItem.TransactionLineItemGuid)
					{
						bFound = true;
						break;
					}

				}

				if (bFound == false)
				{
					throw new Exception("Line item is not a member of specified transaction");
				}

				// Check the line item and make sure it is ok to load it
				if (LineItem.Quantity.GrossInventoryChange != 0.0
				|| LineItem.Quantity.NetInventoryChange != 0.0)
				{
					throw new Exception("Line item has existing volume applied.");
				}

				this.LoadAndCheckCompanies();

				LineItem.Status = TransactionStatus.LoadPending;

				// Check values
				if (SubLineItem != null)
				{
					if (SubLineItem.Density == null)
					{
						SubLineItem.Density = 0.0;
					}

					if (SubLineItem.Temperature == null)
					{
						SubLineItem.Temperature = 0.0;
					}
				}

				// Indicate we are in remote mode
				this.RemoteAuthorized = true;
				this.RemoteSubLineItem = SubLineItem;

				// Set the one preload item we can do
				LoadArmManagerClass LoadArmManager = this.LoadArmManagerCollection.Item(0);

				LoadArmManager.Bay(StationManager).PreLoads = new ArrayList
				{
					LineItem
				};

				// If there is a StorageLocation (tank) listed, change the station configuration to match the storage location
				// listed on the transaction
				if (SubLineItem != null)
				{
					if (SubLineItem.StorageLocationID != null && SubLineItem.StorageLocationID != "")
					{
						this.ChangeStationStorageLocation(SubLineItem.Product, SubLineItem.StorageLocationID);
					}
				}
				else
				{
					if (LineItem.StorageLocationID != null && LineItem.StorageLocationID != "")
					{
						this.ChangeStationStorageLocation(LineItem.Product, LineItem.StorageLocationID);
					}
				}

				this.LoadArmManagerCollection.ClearRecipeMap(this);

				LoadArmManager.PromptForNextBatch(StationManager, false);

			}
			catch
			{
				if (LineItem != null)
				{
					LineItem.Status = TransactionStatus.LoadPending;
				}

				this.ResetStationDevice();

				throw;
			}

		}

		protected void ChangeStationStorageLocation(string ProductName, string StorageLocation)
		{
			// Get the product
			ProductClass Product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
																	 x =>
																	 x.GetByID(this.Security, ProductName)
																);

			if (Product == null)
			{
				throw new Exception("Could not find product");
			}

			// Get the tank
			TankClass Tank = FMChannelHelper.MakeCall<ITanks, TankClass>(
																	 x =>
																	 x.Get(this.Security, x.GetIdentityGuid(this.Security, StorageLocation))
																);

			if (Tank == null)
			{
				throw new Exception("Could not find tank object");
			}

			foreach (LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
			{
				foreach (ProductMapClass ProductMap in LoadArmManager.LoadArm.ComponentCollection)
				{
					if (ProductMap.AssignedGuid == Product.IdentityGuid)
					{
						if (Tank.IdentityGuid != ProductMap.TankOrGroupGuid)
						{
							// Change the tank
							ProductMap.TankOrGroupGuid = Tank.IdentityGuid;
							ProductMap.TankOrGroupID = Tank.ID;
						}

					}

				}

			}

		}

		protected void LoadAndCheckCompanies()
		{
			// Get the Carrier company
			if (this.Transaction.CarrierCompanyGuid != Guid.Empty)
			{
				this.Carrier = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(this.Security, this.Transaction.CarrierCompanyGuid)
																);


				if (this.Carrier != null)
				{
					if (this.ValidateCompany(this.Carrier, COMPANY_ROLE.CARRIER) == false)
					{
						throw new Exception("Error validating Carrier company");
					}
				}
			}

			// Set the Ship-To company
			if (this.Transaction.ShipToCompanyGuid != Guid.Empty)
			{
				this.ShipTo = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(this.Security, this.Transaction.ShipToCompanyGuid)
																);


				if (this.ShipTo == null)
				{
					throw new Exception("Could not load Ship-To company.");
				}

				if (this.ValidateCompany(this.ShipTo, COMPANY_ROLE.CUSTOMER_SHIPTO) == false)
				{
					throw new Exception("Error validating Customer Ship-To company");
				}
			}

			// Check the bill-to company
			if (this.Transaction.BillToCompanyGuid != Guid.Empty)
			{
				this.BillTo = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(this.Security, this.Transaction.BillToCompanyGuid)
																);


				if (this.BillTo != null)
				{
					if (this.ValidateCompany(this.BillTo, COMPANY_ROLE.CUSTOMER_BILLTO) == false)
					{
						throw new Exception("Error validating Bill-To company");
					}
				}
			}
		}
		protected override void ProcessOffLoadProductSelect(string response)
		{
			bool bProductFound = false;
			if (response == EscapeString)
			{
				if (this.Station.OffLoadByOffLoadID == false && this.UseOffLoadSupplyOrders == true)
				{
					this.PromptForSupplyOrderNumber();
				}
				else
				{
					this.StationState = StationState.OFFLOADID_PROMPT;
					this.DisplayMessage("[LoadRack|Enter] [LoadRack|Off Load ID]", null, PromptLength, this.PROMPT_TIMEOUT);
				}
				return;
			}

			int MenuNumber = 1;
			if (this.Station.OffLoadByOffLoadID == true ||
				this.UseOffLoadSupplyOrders == false)
			{
				foreach (LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
				{
					foreach (ProductMapClass ProductMap in LoadArmManager.LoadArm.ComponentCollection)
					{
						if (MenuNumber == System.Convert.ToInt32(response))
						{
							this.SelectedProductID = ProductMap.AssignedID;
							LoadArmManager.CurrentLineItemProduct = FMChannelHelper.MakeCall<IProducts, ProductClass>(
																	 x =>
																	 x.GetByProductAuthorizedCompanies(this.Security, ProductMap.AssignedGuid, false)
																);

							if (LoadArmManager.CurrentLineItemProduct != null)
							{
								bProductFound = true;
							}

							break;
						}
						++MenuNumber;
					}
				}
			}
			else
			{
				if (this.SupplyOrder.LineItems.Count > 0)
				{
					foreach (LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
					{
						// check for different products in the line items and present the user with a selection
						for (int Index = 0; Index < this.SupplyOrder.LineItems.Count; Index++)
						{
							LineItemDO LineItem = this.SupplyOrder.LineItems[Index];
							if (MenuNumber == System.Convert.ToInt32(response))
							{

								this.SelectedProductID = LineItem.Product;
								LoadArmManager.CurrentLineItemProduct = FMChannelHelper.MakeCall<IProducts, ProductClass>(
																	 x =>
																	 x.GetByProductAuthorizedCompanies(this.Security, LineItem.ProductGuid, false)
																);

								if (LoadArmManager.CurrentLineItemProduct != null)
								{
									bProductFound = true;
								}

								break;
							}
							++MenuNumber;
						}
					}
				}
			}

			if (bProductFound == false)
			{
				this.DisplayOffLoadProductSelect();
				return;
			}

			this.PromptForBOLNumber();
		}

		public override bool SetDensityInUnit(string Density)
		{
			if (this.AvailableLoadArmManagers == 0)
			{
				return false;
			}

			ProcessVariableClass StationPV = this.Station.ProcessVariableCollection[0];

			ProcessVariableClass SetArmDensity = new ProcessVariableClass
			{
				URL = StationPV.URL,

				OPCItemID = StationPV.OPCItemID + ".Arm 1.Dynamic Values.System.Current Density",

				ServerValue = Density
			};
			this.OPCServerManager.Write(SetArmDensity);

			return true;
		}

		public override void SetUnloadPresetAmount(string Response)
		{

			if (Response == EscapeString)
			{
				this.PromptForBOLNumber();

				return;
			}

			// set the output permissives
			this.UpdatePermissives(true);
			if (this.StationState == StationState.RESET_ON_TIMEOUT)
			{
				// turn anything off that we may of turned on
				this.UpdatePermissives(false);
				return;
			}

			this.StartDateTime = this.LastActivityDateTime = DateTimeOffset.Now;
			this.OffLoadPresetAmount = System.Convert.ToDouble(Response);

			AcculoadIIILoadArmManagerClass LoadArmManager = (AcculoadIIILoadArmManagerClass)this.LoadArmManagerCollection.Item(0);

			this.StationState = StationState.AUTHORIZED;
			if (!LoadArmManager.Authorize(this, System.Convert.ToDouble(Response)))
			{
				// turn anything off that we may of turned on
				this.UpdatePermissives(false);
			}

			return;
		}

		public override void UpdatePermissives(bool authorized)
		{
			foreach (ProcessVariableClass PV in this.Station.StationPermissives.Outputs)
			{
				switch (PV.ProcessVariableType)
				{
					case PROCESS_VARIABLE_TYPE.OUTPUT_PERMISSIVE_PV:
						{
							PermissivesClass Permissives = PV.Parent;
							if (Permissives == null)
							{
								break;
							}

							PV.ServerValue = authorized;

							this.OPCServerManager.Update(true);

							if (!PV.IsQualityGood
							|| ((bool)PV.ServerValue) != authorized)
							{
								this.StationState = StationState.RESET_ON_TIMEOUT;
								this.DisplayMessage("LoadRack|Error Setting Permissive" + " " + PV.OPCItemID, null, 0, this.MESSAGE_TIMEOUT);
								return;
							}

							break;
						}

					default:
						this.eventLog.WriteEntry("StationManager OnInvoke : Unknown PV : " + PV.OPCItemID);
						break;
				}
			}
		}

		// Now simply allow the parent class implementation to be used
		// TODO:  Delete entirely once we've confirmed that this is no longer needed
		//public override void DisplayOffLoadProductSelect()
		//{
		//	// for the preset we need to populate the menu with the configured arm products
		//	if (this.AvailableLoadArmManagers == 0)
		//	{
		//		this.StationState = StationState.RESET_ON_TIMEOUT;
		//		this.DisplayMessage("LoadRack|No Products Available", null, 0, this.MESSAGE_TIMEOUT);
		//		return;
		//	}

		//	// check that the supplier has authorized products configured
		//	if (this.Supplier.SupplierAuthorizedProductCollection.Count == 0)
		//	{
		//		this.AddAlarmAndEventLogs(this.Security, this.Station.NoProductsAvailableEvent(this.Station.ID));
		//		this.LoadRackManager.EventOrAlarmEvent.Set();

		//		this.StationState = StationState.RESET_ON_TIMEOUT;
		//		this.DisplayMessage("LoadRack|No Products Available", null, 0, this.MESSAGE_TIMEOUT);
		//		return;
		//	}

		//	// Build menu parameter set
		//	DisplayMenuParameters parameters = new DisplayMenuParameters
		//	{
		//		ApplyDataDictionary = true,
		//		DefaultItem = 0,
		//		MenuTimeout = this.PROMPT_TIMEOUT,
		//		SaveForCancelProcessing = false,
		//		Caption = "LoadRack|Select Off Load Product"
		//	};

		//	var menu = new List<string>();

		//	// Save last station state
		//	this.PriorStationState = this.StationState;

		//	foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
		//	{
		//		foreach (ProductMapClass productMap in loadArmManager.LoadArm.ComponentCollection)
		//		{
		//			foreach (ProductMapClass supplierProduct in this.Supplier.SupplierAuthorizedProductCollection)
		//			{
		//				if (supplierProduct.AssignedID == productMap.AssignedID)
		//				{
		//					menu.Add(productMap.AssignedID);
		//				}
		//			}
		//		}
		//	}

		//	if (menu.Count == 0)
		//	{
		//		this.AddAlarmAndEventLogs(this.Security, this.Station.NoProductsAvailableEvent(this.Station.ID));
		//		this.LoadRackManager.EventOrAlarmEvent.Set();

		//		this.StationState = StationState.RESET_ON_TIMEOUT;
		//		this.DisplayMessage("LoadRack|No Products Available", null, 0, this.MESSAGE_TIMEOUT);
		//		return;
		//	}

		//	parameters.Menu = menu.ToArray();

		//	this.StationState = StationState.SELECT_OFFLOAD_PRODUCT;

		//	this.DisplayMenu(parameters);
		//}

		public override void DisplayVerifySupplyOrderProduct()
		{
			//SelectedSupplyOrder
			string DocumentNumber;
			bool bProductFound = false;
			//			CardID=Response;
			// Check for preloads for the current driver
			GetTransactionSR getTransactionSR = new GetTransactionSR
			{
				Security = this.Security,
				Request = GetTransactionRequest.SITE_TYPEID_ALIAS_DOCUMENTNUMBER,
				Site = this.SiteManager.Site.ID,
				TransTypeID = TransactionTypes.T18_SupplyOrder,
				Status = ((int)TransactionStatus.Scheduled).ToString(),
				DocumentNumber = this.SelectedSupplyOrder
			};

			GetTransactionDO getTransactionDO = FMChannelHelper.MakeCall<IGetTransactionProcessor, GetTransactionDO>(
																	 x =>
																	 x.Process(getTransactionSR)
																);


			// Build menu parameter set
			DisplayMenuParameters Parameters = new DisplayMenuParameters
			{
				ApplyDataDictionary = true,
				DefaultItem = 0,
				MenuTimeout = 0,
				SaveForCancelProcessing = false,
				Caption = "LoadRack|Select Off Load Product"
			};

			ArrayList Menu = new ArrayList();

			// Save last station state
			this.PriorStationState = this.StationState;


			if (getTransactionDO != null
				&& getTransactionDO.TransactionDataSet != null
				&& getTransactionDO.TransactionDataSet.Tables.Count != 0
				&& getTransactionDO.TransactionDataSet.Tables[0].Rows.Count != 0)
			{
				foreach (DataRow Row in getTransactionDO.TransactionDataSet.Tables[0].Rows)
				{
					DocumentNumber = Row["TransID"] as string;
					if (DocumentNumber != "")
					{
						this.SupplyOrder = this.GetTransaction(DocumentNumber);

						// check for multiple line items to very product
						if (this.SupplyOrder.LineItems.Count > 0)
						{
							// check for different products in the line items and present the user with a selection
							for (int Index = 0; Index < this.SupplyOrder.LineItems.Count; Index++)
							{
								LineItemDO LineItem = this.SupplyOrder.LineItems[Index];
								if (LineItem.Product != null && LineItem.Status == TransactionStatus.Scheduled)
								{
									// make sure this product is in the arm or the transaction will not be saved
									foreach (LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
									{
										foreach (ProductMapClass ProductMap in LoadArmManager.LoadArm.ComponentCollection)
										{
											if (LineItem.Product == ProductMap.AssignedID)
											{
												Menu.Add(ProductMap.AssignedID);
												bProductFound = true;
												Parameters.Menu = (string[])Menu.ToArray(typeof(string));
											}
										}
									}
								}
							}
						}

						if (bProductFound == false)
						{
							this.StationState = StationState.RESET_ON_TIMEOUT;
							this.DisplayMessage("LoadRack|No Products Available", null, 0, this.MESSAGE_TIMEOUT);
							return;
						}

						if (this.SupplyOrder.ShipToCompanyGuid != Guid.Empty)
						{
							this.ShipTo = this.GetCompanyInfo(this.Security, this.SupplyOrder.ShipToCompanyGuid);
						}

						if (this.SupplyOrder.BillToCompanyGuid != Guid.Empty)
						{
							this.BillTo = this.GetCompanyInfo(this.Security, this.SupplyOrder.BillToCompanyGuid);
						}

						if (this.SupplyOrder.ShipperCompanyGuid != Guid.Empty)
						{
							this.Shipper = this.GetCompanyInfo(this.Security, this.SupplyOrder.ShipperCompanyGuid);
						}

						if (this.SupplyOrder.OwnerCompanyGuid != Guid.Empty)
						{
							this.Owner = this.GetCompanyInfo(this.Security, this.SupplyOrder.OwnerCompanyGuid);
						}

						if (this.SupplyOrder.ManagerCompanyGuid != Guid.Empty)
						{
							this.Manager = this.GetCompanyInfo(this.Security, this.SupplyOrder.ManagerCompanyGuid);
						}

						if (this.SupplyOrder.SupplierCompanyGuid != Guid.Empty)
						{
							this.Supplier = this.GetCompanyInfo(this.Security, this.SupplyOrder.SupplierCompanyGuid);
						}

						this.StationState = StationState.SELECT_OFFLOAD_PRODUCT;

						this.DisplayMenu(Parameters);
						return;
					}
				}
				// default error message if any of the above does not complete
				this.DisplayMessage("[LoadRack|Invalid Selection]", null, 0, this.MESSAGE_TIMEOUT);
				this.StationState = StationState.INVALID_SUPPLIER_PROMPT_RESPONSE_MESSAGE;
			}
		}

		private CompanyClass GetCompanyInfo(SecurityClass security, Guid guid)
		{
			return FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(security, guid)
																);
		}

		protected override byte ReadAdditiveProductsUsingInjector(
			ProductMapClass AdditiveInjector,
			Server Server,
			LoadArmManagerClass LoadArmManager)
		{
			return 0xFF;
		}

        protected override void SetProductsInStation()
        {
            // Do nothing; we aren't doing recipe rewrite for Microload yet
        }

        protected override void ClearRecipes(bool clearAll)
        {
            // DynamicRecipes don't apply to the generic case
            this.RecipeInternalNumberMap = new Dictionary<int, ProductMapClass>();
            this.LastDownloadedRecipe = 0;
            return;
        }

        internal override void ClearSingleRecipe(int recipeNumber)
        {
            // Do nothing; we aren't doing recipe rewrite for Microload yet
        }

        internal override int WriteSingleRecipe(LoadArmManagerClass loadArmManager, ProductMapClass recipeToArmMap)
        {
            // Do nothing beyond map the configured recipe number to itself; we aren't doing recipe rewrite for Microload yet
            return loadArmManager.GetRecipeNumber(recipeToArmMap);
        }

        protected override int GetNextAvailableRecipeNumber()
        {
            // Do nothing; we aren't doing recipe rewrite for Microload yet
            return 0;
        }

        /// <summary>
        /// Returns whether the recipce in question belongs to this FuelsManager station
        /// Comes in to play with swing arms and split bays, where two stations in FuelsManager may
        /// address the same physical preset
        /// </summary>
        /// <param name="recipeNumber">Recipe number to check</param>
        /// <returns>true</returns>
        protected override bool RecipeBelongsToThisStation(int recipeNumber)
        {
            return true;
        }
    }
}
