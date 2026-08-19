using System;
using System.Collections;
using System.Text;

namespace PIDXTransactions
{
    public class BOLBBRecord : BOLBase
    {
        #region Private attributes
        private int authorizationNumber;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor for the BOL build record class.
        /// </summary>
        public BOLBBRecord()
        {
            this.Initialize();
            base.TransactionType = PIDXConstants.BOL_BB;
        }
        #endregion

        #region Properties
        public int AuthorizationNumberDigit
        {
            get { return this.authorizationNumber;  }
            set { this.authorizationNumber = Math.Abs(value); }
        }

        public string AuthorizationNumber
        {
            get
            {
                string outStr = this.authorizationNumber.ToString();
                int length = 8 - outStr.Length;

                for (int count = length; count > 0; count--)
                {
                    outStr = "0" + outStr;
                }

                return outStr;
            }
        }

        public bool AddBOLProduct( string ProductCode, int BlendID, double GrossGallons, double NetTemperature, int NetTemperatureFlag )
        {
            bool result = false;

            // only added products to BOL if we haven't reached the limit
            if (PIDXConstants.MAX_BOL_PRODUCTS > this.ProductArrayList.Count)
            {
                BOLBBProduct bbproduct = new BOLBBProduct();
                bbproduct.ProductCode = ProductCode;
                bbproduct.BlendIDDigit = BlendID;
                bbproduct.GrossDigit = GrossGallons;
                bbproduct.NetTemperatureDigit = NetTemperature;
                bbproduct.NetTemperatureFlagDigit = NetTemperatureFlag;

                ProductArrayList.Add(bbproduct);     
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
        public override string GetDataRecord()
        {
            // Validate the fields to ensure that exist and are the appropriate length.
            this.ValidateRecord();

            // build header portion of BB
            string bolRecord = "";
            bolRecord = bolRecord +
                        base.TransactionType +
                        base.SPLCCode +
                        base.TerminalOperator +
                        base.SellerID +
                        this.AuthorizationNumber +
                        base.BOLNumber +
                        base.ShipDay;
            
            // Add each defined bolbbproduct to the BB
            for (int count = 0; count < ProductArrayList.Count; count++)
            {
                BOLBBProduct bbproduct = (BOLBBProduct) ProductArrayList[count];

                bolRecord += 
                bbproduct.ProductCode +
                bbproduct.BlendID +
                bbproduct.Gross +
                bbproduct.NetTemperature +
                bbproduct.NetTemperatureFlag;
            }

            GenerateCheckBit(bolRecord);
            bolRecord += base.CheckDigit;

            return bolRecord;
        }

        /// <summary>
        /// This method implement that validation for the BOL Build record.  It throws
        /// an exception if the validation fails.
        /// </summary>
        public override void ValidateRecord()
        {
            base.ValidateSpecific();

            if (this.authorizationNumber == -99)
            {
                throw new PIDXException(PIDXConstants.ERR_MSG_017);
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        private void Initialize()
        {
            this.authorizationNumber = -99;
        }
        #endregion
    }
}
