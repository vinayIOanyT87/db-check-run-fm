/// <summary>
///   File name:	DFPCommercialCurrencyValidation.cs
///   Purpose:	   To validate the foreign and domestic currency fields for the
///               Direct Fuel Purchase Import, Commercial and Direct Fuel
///               Purchase transactions.
///				
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				   2000.  This file shall not be copied or reproduced in any form 
///				   without the express written consent of Endress+Hauser.
///				
///	Author(s):	Richard R. Panachida
///	Version:	   1.0.0  Current version
///	
///	Modification History:
///		Date:			   By:						Reason:
///		----------		--------------------	----------------------------------
///	2010-Jun-2		W.Gray					WI 13431 - Revised call to GetUnitString to set LCID as 0
///			
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using EngineeringUnitsLibrary;
using System.Globalization;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.UtilityObjects;

namespace ADFWebApp
{
	public class DFPCommercialCurrencyValidation
	{
		#region Public data members
		public enum Sources { IMPORT, GUI };
		#endregion

		#region Private data members
		private string errorMsg;
		private SecurityClass security;
		private Hashtable currencyUnitHshTbl;
		private Guid currencyGuid;
		private CurrencyClass currencyRates;

		private Sources source;
		private CultureInfo cultureUS;
		private int? foundCUUnitIndex;
		private int? equipmentIndex;
		private ArrayList unitList;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Direct Fuel Purchase and Commercial
		/// Currency validations.
		/// </summary>
		public DFPCommercialCurrencyValidation(SecurityClass inSecurity)
		{
			this.errorMsg = "";
			this.security = inSecurity;
			this.currencyUnitHshTbl = new Hashtable();
			this.currencyGuid = Guid.Empty;
			this.currencyRates = new CurrencyClass(this.security);
			this.source = Sources.IMPORT;
			this.cultureUS = new CultureInfo("en-US");

			// Load all the currency units.
			this.GetCurrencyUnits();

			unitList = new ArrayList();
			unitList.Add(ENGINEERING_UNIT.FMV_BlLiq);
			unitList.Add(ENGINEERING_UNIT.FMV_BlOil);
			unitList.Add(ENGINEERING_UNIT.FMV_CM3);
			unitList.Add(ENGINEERING_UNIT.FMV_Feet3);
			unitList.Add(ENGINEERING_UNIT.FMV_ImpGal);
			unitList.Add(ENGINEERING_UNIT.FMV_Inch3);
			unitList.Add(ENGINEERING_UNIT.FMV_KL);
			unitList.Add(ENGINEERING_UNIT.FMV_Litre);
			unitList.Add(ENGINEERING_UNIT.FMV_Meter3);
			unitList.Add(ENGINEERING_UNIT.FMV_USGal);
			unitList.Add(ENGINEERING_UNIT.FMV_Yard3);
			unitList.Add(ENGINEERING_UNIT.FMV_MsFt3);
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will return the error message if there is one. It
		/// will be blank if no error.
		/// </summary>
		public string ErrorMsg
		{
			get { return this.errorMsg; }
		}

		/// <summary>
		/// This property will return the CU Unit Index.
		/// </summary>
		public int? FoundCUUnitIndex
		{
			get { return this.foundCUUnitIndex; }
		}

		/// <summary>
		/// This property will return the Equipment Index.
		/// </summary>
		public int? EquipmentIndex
		{
			get { return this.equipmentIndex; }
		}

		/// <summary>
		/// This property will return the Currency Guid
		/// </summary>
		public Guid CurrencyGuid
		{
			get { return this.currencyGuid; }
			set
			{
				if (this.source == Sources.GUI)
				{
					this.currencyGuid = value;
				}
				else
				{
					this.currencyGuid = Guid.Empty;
				}
			}
		}

		/// <summary>
		/// This property will set and get the source (Import or GUI).
		/// </summary>
		public Sources Source
		{
			get { return this.source; }
			set { this.source = value; }
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will return a hash table of all the currency units in the system.
		/// </summary>
		/// <returns></returns>
		private void GetCurrencyUnits()
		{
			this.currencyUnitHshTbl = new Hashtable();

			var currencyUnitList = FMChannelHelper.MakeCall<ICurrencies, CurrencyUnitDOCollectionClass>(
																	 x =>
																	 x.GetCurrencyUnits(this.security)
																);


			foreach (CurrencyUnitDO currencyUnitDO in currencyUnitList)
			{
				if (currencyUnitDO != null)
				{
					if (currencyUnitHshTbl.Contains(currencyUnitDO.CurrencyUnitName) == false)
					{
						currencyUnitHshTbl.Add(currencyUnitDO.CurrencyUnitName.ToUpper(), currencyUnitDO.Index);
					}
				}
			}
		}

		/// <summary>
		/// This method will return True if the foreign currency unit value is empty or it
		/// matches a currency unit.
		/// </summary>
		/// <param name="foreignCurrencyUnit"></param>
		/// <returns></returns>
		private bool ValidateCurrencyUnit(string foreignCurrencyUnit)
		{
			bool isValidate = true;

			if (string.IsNullOrEmpty(foreignCurrencyUnit) == false)
			{
				if (this.currencyUnitHshTbl.Contains(foreignCurrencyUnit.ToUpper()) == false)
				{
					isValidate = false;
				}
			}

			return isValidate;
		}

		/// <summary>
		/// This method will return the currency object from table tblCurrencies that matches
		/// the currency unit index. It will return null if not found.
		/// </summary>
		/// <param name="currencyUnitIndex"></param>
		/// <returns></returns>
		private CurrencyDO GetCurrency(int currencyUnitIndex)
		{
			CurrencyDO currencyDO = null;

			try
			{
				currencyDO = FMChannelHelper.MakeCall<ICurrencies, CurrencyDO>(
																	 x =>
																	 x.GetByUnitIndex(this.security, currencyUnitIndex)
																);

			}
			catch (Exception)
			{
				this.errorMsg = "Could not retrieve Currency Object, returning null.";
			}

			return currencyDO;
		}

		/// <summary>
		/// This method will return true if the UOM unit is found. It will return false if the
		/// UOM is not found or the value entered is null or blank.
		/// </summary>
		/// <param name="tfmsDO"></param>
		/// <returns></returns>
		private bool UOMValidation(TFMSDO tfmsDO)
		{
			bool found = false;
			string unitName = null;
			this.foundCUUnitIndex = null;

			if (string.IsNullOrEmpty(tfmsDO.UOM) == false)
			{
				// Loop through all the possible volume units to find a match.
				foreach (ENGINEERING_UNIT volumeUnit in this.unitList)
				{
					unitName = EngineeringUnits.GetUnitString(volumeUnit);

					if (unitName.ToUpper().Equals(tfmsDO.UOM.ToUpper()) == true)
					{
						found = true;
						this.foundCUUnitIndex = Convert.ToInt32(volumeUnit);
						break;
					}
					// Added this IF due to engineering units not supporting Australian English culture.
					else if ((tfmsDO.UOM.ToUpper().Equals("CUBIC CENTIMETRES") == true)
							|| (tfmsDO.UOM.ToUpper().Equals("KILOLITRES") == true)
							|| (tfmsDO.UOM.ToUpper().Equals("LITRES") == true)
							|| (tfmsDO.UOM.ToUpper().Equals("CUBIC METRES") == true))
					{
						found = true;
						this.foundCUUnitIndex = Convert.ToInt32(volumeUnit);
						break;
					}
				}
			}

			if (found == false)
			{
				this.errorMsg = "The UOM '" + tfmsDO.UOM + "' is not a standard value.\n";
			}

			return found;
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will clear the error message.
		/// </summary>
		public void ClearErrorMessage()
		{
			this.errorMsg = "";
		}

		/// <summary>
		/// This method will return True if the foreign currency rules are valid. Otherwise, it will
		/// return false.
		/// If Foreign Currency Identifier is entered, then the following rules apply:
		///   1. There must be an Invoice Foreign Currency price or a Total Foreign Price entered in the import (reject if not)
		///   2. If the Total Foreign price is entered and not the Invoice Foreign Currency Price, the Invoice Foreign 
		///      Currency Price is derived via the quantity
		///   3. If both the Invoice Foreign  Currency price and Total Foreign Price are entered on the import, the value for the 
		///      Invoice Foreign Price entered is to be ignored. Instead the Invoice Foreign Price is to be derived from the 
		///      Total Foreign Cuerrency Price and the quantity.
		///   4. If the Fuel Price (AUD) and/or Total Price (AUD) have been entered these fields will be ignored. They will 
		///      instead be derived via the current exchange rate plus the quantity
		///   5. The GST and Excise will only be populated if supplied
		/// </summary>
		/// <param name="tfmsDO"></param>
		/// <returns></returns>
		public bool ForeignCurrencyValidation(TFMSDO tfmsDO)
		{
			bool isValid = true;

			// Rule 1 - There must be an Invoice Foreign Currency price or a Total Foreign Price entered in the import (reject if not)
			if ((tfmsDO.TotalForeignCurrencyPrice == null) && (tfmsDO.ForeignCurrencyPrice == null))
			{
				isValid = false;
				this.errorMsg = "Must have either a Invoice Foreign Currency Price or a Total Foreign Currency Price.\n";
			}
			// Rule 3 - If both the Invoice Foreign Currency price and Total Foreign Price are entered on the import, 
			//          the value for the Invoice Foreign Currency price entered is to be ignored – Instead the 
			//          Invoice Foreign Currency price is to be derived from the Total Foreign Price and the Quantity 
			if ((tfmsDO.TotalForeignCurrencyPrice != null) && (tfmsDO.ForeignCurrencyPrice != null))
			{
				if (tfmsDO.Quantity == 0)
				{
					isValid = false;
					this.errorMsg = "Quantity must not be zero.\n";
				}
				else
				{
					tfmsDO.ForeignCurrencyPrice = tfmsDO.TotalForeignCurrencyPrice / tfmsDO.Quantity;
				}
			}
			// Rule 2 - If the Total Foreign Price is entered, and not the Invoice Foreign Currency Price, 
			//          the Invoice Foreign Currency Price is derived via the Quantity
			else if ((tfmsDO.TotalForeignCurrencyPrice != null) && (tfmsDO.ForeignCurrencyPrice == null))
			{
				if (tfmsDO.Quantity == 0)
				{
					isValid = false;
					this.errorMsg = "Quantity must not be zero.\n";
				}
				else
				{
					tfmsDO.ForeignCurrencyPrice = tfmsDO.TotalForeignCurrencyPrice / tfmsDO.Quantity;
				}
			}
			else if ((tfmsDO.TotalForeignCurrencyPrice == null) && (tfmsDO.ForeignCurrencyPrice != null))
			{
				tfmsDO.TotalForeignCurrencyPrice = tfmsDO.ForeignCurrencyPrice * tfmsDO.Quantity;
			}

			// Rule 4 - If the Fuel Price (AUD) and/or Total Price (AUD) have been entered these fields will be ignored. 
			//          They will instead be derived via the current exchange rate plus the quantity.
			if (isValid == true)
			{
				double rate = 1.0;

				if (source == Sources.IMPORT)
				{
					if (this.ValidateCurrencyUnit(tfmsDO.ForeignCurrencyUnit) == false)
					{
						isValid = false;
						this.errorMsg = "Invalid currency unit: " + tfmsDO.ForeignCurrencyUnit + ".\n";
					}
					else
					{
						this.currencyGuid = Guid.Empty;

						int unitIndex = (int)this.currencyUnitHshTbl[tfmsDO.ForeignCurrencyUnit.ToUpper()];
						CurrencyDO currencyDO = this.GetCurrency(unitIndex);

						if (currencyDO != null)
						{
							this.currencyGuid = currencyDO.IdentityGuid;

							if (tfmsDO.DateTime == null)
							{
								this.currencyRates.InventoryDate = DateTime.Now;
							}
							else
							{
								this.currencyRates.InventoryDate = tfmsDO.DateTime.Value;
							}

							this.currencyRates.BuildRates();
							rate = this.currencyRates.GetRate(this.currencyGuid);
						}
					}
				}
				else
				{
					if (this.currencyGuid != Guid.Empty)
					{
						isValid = false;
						this.errorMsg = "Invalid currency unit.\n";
					}
					else
					{
						if (tfmsDO.DateTime == null)
						{
							this.currencyRates.InventoryDate = DateTime.Now;
						}
						else
						{
							this.currencyRates.InventoryDate = tfmsDO.DateTime.Value;
						}

						this.currencyRates.BuildRates();
						rate = this.currencyRates.GetRate(this.currencyGuid);
					}
				}

				if (isValid == true)
				{
					tfmsDO.FuelPriceAUD = tfmsDO.ForeignCurrencyPrice / rate;
					tfmsDO.TotalPriceAUD = tfmsDO.FuelPriceAUD * tfmsDO.Quantity;
				}
			}

			return isValid;
		}

		/// <summary>
		/// This method will return True if the domestic currency rules are valid. Otherwise, it will
		/// return false.
		///   1. If Invoice Foreign Currency price and/or Total Foreign Price are entered on the import they will be ignored and 
		///      not uploaded
		///   2. If Fuel Price (AUD) only is entered, then the Total Price (AUD) is derived via Fuel Price (AUD) and the quantity
		///   3. if Total Price (AUD) only is entered, then the Fuel Price (AUD) is derived via Total Price (AUD) and the quantity
		///   4. If both the Fuel Price (AUD) and Total Price (AUD) are entered on the import, the value for the Fuel Price (AUD) 
		///      entered is to be ignored.  Instead the Fuel Price (AUD) is to be derived from the Total Price (AUD) and the quantity 
		///      (the thought is that the Total Price (AUD) is likely to be more accurate as you are more likely to have a calculation 
		///      error on Fuel Price (AUD))
		///   5. If GST and Excise are blank, the system calculates these values via the GST and Excise configured in the 
		///      Excise and GST configuration screens (supplier configured values)
		///   6. If GST and Excise are not blank, then these values will be uploaded (i.e. overwrite what might have been derived)
		/// </summary>
		/// <param name="tfmsDO"></param>
		/// <param name="supplierGuid"></param>
		/// <param name="productGuid"></param>
		/// <returns></returns>
		public bool DomesticCurrencyValidation(TFMSDO tfmsDO, Guid supplierGuid, Guid productGuid)
		{
			bool isValid = true;

			// Rule 1 - If Invoice Foreign Currency price and/or Total Foreign Price are entered on the 
			//          import they will be ignored and not uploaded.
			tfmsDO.ForeignCurrencyPrice = null;
			tfmsDO.TotalForeignCurrencyPrice = null;

			if ((tfmsDO.FuelPriceAUD == null) && (tfmsDO.TotalPriceAUD == null))
			{
				isValid = false;
				this.errorMsg = "Must have either a Fuel Price (AUD) or a Total Price (AUD).\n";
			}
			// Rule 4 - If both the Fuel Price (AUD) and Total Price (AUD) are entered 
			//          on the import, the value for the Fuel Price (AUD) entered is to be 
			//          ignored – Instead the Fuel Price (AUD)  is to be derived from the Total Price (AUD) 
			//          and the Quantity 
			else if ((tfmsDO.FuelPriceAUD != null) && (tfmsDO.TotalPriceAUD != null))
			{
				tfmsDO.FuelPriceAUD = tfmsDO.TotalPriceAUD / tfmsDO.Quantity;
			}
			// Rule 2 - If Fuel Price (AUD) only is entered, then the Total Price (AUD) is derived via Fuel Price (AUD) and the quantity
			else if ((tfmsDO.FuelPriceAUD != null) && (tfmsDO.TotalPriceAUD == null))
			{
				tfmsDO.TotalPriceAUD = tfmsDO.FuelPriceAUD * tfmsDO.Quantity;
			}
			// Rule 3 - if Total Price (AUD) only is entered, then the Fuel Price (AUD) is derived via Total Price (AUD) and the quantity
			else if ((tfmsDO.TotalPriceAUD != null) && (tfmsDO.FuelPriceAUD == null))
			{
				if (tfmsDO.Quantity == 0)
				{
					isValid = false;
					this.errorMsg = "Quantity must be greater that zero in order to calculate Fuel Price (AUD).\n";
				}
				else
				{
					tfmsDO.FuelPriceAUD = tfmsDO.TotalPriceAUD / tfmsDO.Quantity;
				}
			}

			// Rule 5 - If GST and Excise are blank, the system calculates these values via the GST and Excise configured in the 
			//          Excise and GST configuration screens (supplier configured values).
			if (tfmsDO.GST == null)
			{
				GoodsAndServicesTaxDO gstDO = null;

				if (supplierGuid != Guid.Empty)
				{
					gstDO = FMChannelHelper.MakeCall<IGoodsAndServices, GoodsAndServicesTaxDO>(
																	 x =>
																	 x.GetByDateAndCompany(this.security, tfmsDO.DateTime.Value, supplierGuid)
																);

				}

				if ((gstDO != null) && (gstDO.Index > 0))
				{
					tfmsDO.GST = gstDO.GstValue;
				}
				else
				{
					this.errorMsg = "GST is not configured";
					isValid = false;
				}

				// Calculate the GST amount based on the total price. Note that the GST
				// is a percentage and not a rate.
				if ((tfmsDO.GST != null) && (tfmsDO.TotalPriceAUD != null))
				{
					double gstRate = tfmsDO.GST.Value / 100.0;
					tfmsDO.GST = tfmsDO.TotalPriceAUD - (tfmsDO.TotalPriceAUD / (1.0 + gstRate));
				}
			}

			// Rule 5 - If GST and Excise are blank, the system calculates these values via the GST and Excise configured in the 
			//          Excise and GST configuration screens (supplier configured values).
			if (tfmsDO.Excise == null)
			{
				ExciseTaxDO exciseDO = null;

				if (supplierGuid != Guid.Empty)
				{
					exciseDO = FMChannelHelper.MakeCall<IExcises, ExciseTaxDO>(
																	 x =>
																	 x.GetForProductCompanyAndDate(productGuid,
																				  tfmsDO.DateTime.Value,
																				  supplierGuid,
																				  this.security)
																);

				}

				if ((exciseDO != null) && (exciseDO.Index > 0))
				{
					tfmsDO.Excise = exciseDO.ExciseRate;
				}
				else
				{
					this.errorMsg = "Excise is not configured";
					isValid = false;
				}

				// Calculate the Excise amount based on the quantity times the excise rate.
				// Note, the Excise value is a rate.
				if ((tfmsDO.Excise != null) && (tfmsDO.Quantity != null))
				{
					tfmsDO.Excise = tfmsDO.Excise.Value * tfmsDO.Quantity.Value;
				}
			}

			return isValid;
		}

		/// <summary>
		/// This method will return true if the UOM unit exists and is valid. 
		/// </summary>
		/// <param name="tfmsDO"></param>
		/// <returns></returns>
		public bool UOMQuantityValidation(TFMSDO tfmsDO)
		{
			bool isValid = true;

			if (tfmsDO.UOMQuantity != null)
			{
				if (string.IsNullOrEmpty(tfmsDO.UOM) == true)
				{
					this.errorMsg = "Must have a UOM value since the UOM Quantity is present.\n";
					isValid = false;
				}
				else
				{
					isValid = this.UOMValidation(tfmsDO);
				}
			}

			return isValid;
		}

		/// <summary>
		/// This method will return true if the defense asset ID is valid. Otherwise, it will
		/// return false.
		/// </summary>
		/// <param name="tfmsDO"></param>
		/// <returns></returns>
		public bool DefenseAssetValidation(TFMSDO tfmsDO)
		{
			bool isValid = true;

			if (string.IsNullOrEmpty(tfmsDO.DefenseAssetID) == false)
			{
				this.equipmentIndex = BaseDataObject.DUMMY_INDEX;// equipments.GetIdentityGuid(this.security, tfmsDO.DefenseAssetID);

				if (this.equipmentIndex <= 0)
				{
					this.errorMsg = "Invalid Defence Asset ID: " + tfmsDO.DefenseAssetID + ".\n";
					this.equipmentIndex = null;
					isValid = false;
				}
			}
			else
			{
				this.errorMsg = "Defence Asset ID is required.\n";
				this.equipmentIndex = null;
				isValid = false;
			}

			return isValid;
		}
		#endregion
	}
}
