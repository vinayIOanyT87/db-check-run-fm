namespace FMBusinessObjects.PIDXTransactions
{
    using System;

    using FMBusinessObjects.Constants;
    using FMBusinessObjects.Exceptions;

    // ReSharper disable once InconsistentNaming
    public class BOLCBProduct : BOLProductBase
    {
        #region Private attributes
        private int creditIndicator;
		  private double netTemperature;
		  private int netTemperatureFlag;
		  #endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the BOL CB product class.
		/// </summary>
		public BOLCBProduct ( )
		{
			this.Initialize ( );
		}
		#endregion

        #region Properties
		  public string ProductCode
		  {
			  get { return this.productCode.PadLeft(6,'0'); }
			  set { this.productCode = value.ToUpper().Substring(0, (value.Length < 6) ? value.Length : 6); }
		  }

        public int CreditIndicatorDigit
        {
            get { return this.creditIndicator;  }
            set { this.creditIndicator = Math.Abs(value); }
        }

        public string CreditIndicator => this.creditIndicator.ToString();

        public string Gross => Convert.ToInt32(this.gross).ToString("D8");

        public double NetTemperatureDigit
		  {
			  get { return this.netTemperature; }
			  set { this.netTemperature = value; }
		  }

		  public string NetTemperature => Convert.ToInt32(this.netTemperature).ToString("D8");

        public int NetTemperatureFlagDigit
		  {
			  get { return this.netTemperatureFlag; }
			  set { this.netTemperatureFlag = Math.Abs(value); }
		  }

		  public string NetTemperatureFlag => this.netTemperatureFlag.ToString();

        public string BlendOrAlterationIndicator => this.blendOrAlterationIndicator.ToString("D1");

        #endregion

		#region Abstract method implementations
		/// <summary>
		/// This method implement that validation for the BOL CB product. It throws
		/// an exception if the validation fails.
		/// </summary>
		public override void ValidateProduct ( )
		{
			this.Validate ( );

            if (this.creditIndicator == -99)
            {
                throw new PIDXException(PIDXConstants.ERR_MSG_018);
            }
		    // ReSharper disable once CompareOfFloatsByEqualityOperator
				if (this.netTemperature == -9999.0)
				{
					throw new PIDXException(PIDXConstants.ERR_MSG_014);
				}

				if (this.netTemperatureFlag == -99)
				{
					throw new PIDXException(PIDXConstants.ERR_MSG_015);
				}
		  }
        #endregion

        #region Private methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        private void Initialize()
        {
            this.creditIndicator = -99;
				this.netTemperature = -9999.0;
				this.netTemperatureFlag = -99;
		  }
        #endregion
    }
}