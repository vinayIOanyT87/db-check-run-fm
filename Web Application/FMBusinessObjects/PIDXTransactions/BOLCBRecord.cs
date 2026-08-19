namespace FMBusinessObjects.PIDXTransactions
{
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;

    // ReSharper disable once InconsistentNaming
    public class BOLCBRecord : BOLBase
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the BOL complete record class.
		/// </summary>
		public BOLCBRecord ( )
		{
			this.Initialize ( );
			this.TransactionType = PIDXConstants.COMPLETED_BOL;
		}
		#endregion

		#region Properties
		public override string BOLNumber => this.bolNumber.ToString("D9");

		public override string TruckNumber => this.truckNumber.ToString("D6");

        #endregion

		#region public methods
		public virtual bool AddBolProduct(string productCode, int blendOrAlterationIndicatorDigit, double grossGallons, double netTemperature, int netTemperatureFlag, int creditIndicator)
		{
			bool result = false;

            // First, try to find this product already added to this BoL/BB record.
            foreach (object bolProductObject in this.ProductArrayList)
            {
                BOLCBProduct bolProduct = bolProductObject as BOLCBProduct;
                if (bolProduct == null)
                {
                    // Wasn't a Bol Product entry (shouldn't happen)
                    continue;
                }

                if (bolProduct.ProductCode != productCode)
                {
                    // Not for this product
                    continue;
                }

                bolProduct.GrossDigit += grossGallons;
                if (netTemperatureFlag == 1)
                {
                    // netTemperatureFlag == 1 means that netTemperature is a net quantity; 0 means temperature
                    // if it's temperature, we really can't merge it.  First one wins.
                    bolProduct.NetTemperatureDigit += netTemperature;
                }

                // Really can't merge creditIndicator.  First record wins.
                return true;
            }

            // only added products to BOL if we haven't reached the limit
			if (PIDXConstants.MAX_VERSION_1_BOL_PRODUCTS > this.ProductArrayList.Count)
			{
			    BOLCBProduct cbproduct = new BOLCBProduct
			                             {
			                                 ProductCode = productCode,
			                                 BlendOrAlterationIndicatorDigit = blendOrAlterationIndicatorDigit,
			                                 GrossDigit = grossGallons,
			                                 NetTemperatureDigit = netTemperature,
			                                 NetTemperatureFlagDigit = netTemperatureFlag,
			                                 CreditIndicatorDigit = creditIndicator
			                             };

			    this.ProductArrayList.Add(cbproduct);
				result = true;
			}

			return result;
		}
		#endregion

		#region Abstract method implementations
		/// <summary>
		/// This method implement the building of the BOL build record.
		/// </summary>
		/// <returns></returns>
		public override string GetDataRecord(PIDXVersion version)
		{
			// Validate the fields to ensure that exist and are the appropriate length.
			this.ValidateRecord();

			string bolRecord = "";
			bolRecord = bolRecord +
							this.TransactionType +
							this.SPLCCode +
							this.TerminalOperator +
							this.SellerID +
							this.ConsigneeNumber +
							this.FinalShipperID +
							this.CarrierID +
							this.TruckNumber +
							this.BOLNumber +
							this.ShippedDate;

			// Add each defined bolbbproduct to the BB
		    // ReSharper disable once ForCanBeConvertedToForeach
			for (int count = 0; count < this.ProductArrayList.Count; count++)
			{
				BOLCBProduct cbproduct = (BOLCBProduct)this.ProductArrayList[count];

				bolRecord +=
				cbproduct.ProductCode +
				cbproduct.BlendOrAlterationIndicator +
				cbproduct.Gross +
				cbproduct.NetTemperature +
				cbproduct.NetTemperatureFlag +
				cbproduct.CreditIndicator;
			}

		    this.GenerateCheckBit(bolRecord);
			bolRecord += this.CheckDigit;

			return bolRecord;
		}

		/// <summary>
		/// This method implement that validation for the BOL Build record.  It throws
		/// an exception if the validation fails.
		/// </summary>
		public override void ValidateRecord()
		{
			this.ValidateSpecific();

			if(this.productArrayList.Count <= 0)
			{
				throw new PIDXException(PIDXConstants.ERR_MSG_026);
			}

			if ((string.IsNullOrEmpty(this.ConsigneeNumber)) ||
				 (this.ConsigneeNumber.Length > PIDXConstants.CONSIGNEE_ID_LENGTH) ||
				 (this.ConsigneeNumber.Length < 1))
			{
				throw new PIDXException(PIDXConstants.ERR_MSG_008);
			}

			if (this.truckNumber == -99)
			{
				throw new PIDXException(PIDXConstants.ERR_MSG_004);
			}

		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Initialize()
		{
			this.truckNumber = -99;
		}
		#endregion
	}
}