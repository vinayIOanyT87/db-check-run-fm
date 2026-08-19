namespace FMBusinessObjects.PIDXTransactions
{
    using System;

    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;

    // ReSharper disable once InconsistentNaming
    public class BOLBBRecord : BOLBase
	{
		#region Private attributes
		private int authorizationNumber;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the BOL build record class.
		/// </summary>
		public BOLBBRecord ( )
		{
			this.Initialize ( );
			this.TransactionType = PIDXConstants.BOL_BB;
		}
		#endregion

		#region Properties
		public int AuthorizationNumberDigit
		{
			get { return this.authorizationNumber; }
			set { this.authorizationNumber = Math.Abs ( value ); }
		}

		public string AuthorizationNumber => this.authorizationNumber.ToString("D8");

        public override string BOLNumber => this.bolNumber.ToString("D9");

        // ReSharper disable once InconsistentNaming
        public bool AddBOLProduct(string productCode, int blendOrAlterationIndicatorDigit, double grossGallons, double netTemperature, int netTemperatureFlag)
		{
			bool result = false;

            // First, try to find this product already added to this BoL/BB record.
            foreach (object bolProductObject in this.ProductArrayList)
            {
                BOLBBProduct bolProduct = bolProductObject as BOLBBProduct;
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

                return true;
            }

			// only added products to BOL if we haven't reached the limit
			if (PIDXConstants.MAX_VERSION_1_BOL_PRODUCTS > this.ProductArrayList.Count)
			{
			    BOLBBProduct bbproduct = new BOLBBProduct
			                             {
			                                 ProductCode = productCode,
			                                 BlendOrAlterationIndicatorDigit = blendOrAlterationIndicatorDigit,
			                                 GrossDigit = grossGallons,
			                                 NetTemperatureDigit = netTemperature,
			                                 NetTemperatureFlagDigit = netTemperatureFlag
			                             };

			    this.ProductArrayList.Add ( bbproduct );
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
			this.ValidateRecord ( );

			// build header portion of BB
		    var bolRecord = this.TransactionType +
		                       this.SPLCCode +
		                       this.TerminalOperator +
		                       this.SellerID +
		                       this.AuthorizationNumber +
		                       this.BOLNumber +
		                       this.ShipDay;

			// Add each defined bolbbproduct to the BB
		    // ReSharper disable once ForCanBeConvertedToForeach
			for (int count = 0; count < this.ProductArrayList.Count; count++)
			{
				BOLBBProduct bbproduct = (BOLBBProduct)this.ProductArrayList[count];

				bolRecord +=
				bbproduct.ProductCode +
				bbproduct.BlendOrAlterationIndicator +
				bbproduct.Gross +
				bbproduct.NetTemperature +
				bbproduct.NetTemperatureFlag;
			}

		    this.GenerateCheckBit ( bolRecord );
			bolRecord += this.CheckDigit;

			return bolRecord;
		}

		/// <summary>
		/// This method implement that validation for the BOL Build record.  It throws
		/// an exception if the validation fails.
		/// </summary>
		public override void ValidateRecord ( )
		{
			this.ValidateSpecific();

			if(this.productArrayList.Count <= 0)
			{
				throw new PIDXException(PIDXConstants.ERR_MSG_026);
			}

			if (this.authorizationNumber == -99)
			{
				throw new PIDXException ( PIDXConstants.ERR_MSG_017 );
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Initialize ( )
		{
			this.authorizationNumber = -99;
		}
		#endregion
	}
}