using System;
using System.Collections;
using System.Text;

namespace PIDXTransactions
{
    public class BOLCBRecord : BOLBase
    {
        #region Private attributes
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor for the BOL complete record class.
        /// </summary>
        public BOLCBRecord()
        {
            this.Initialize();
            base.TransactionType = PIDXConstants.COMPLETED_BOL;
        }
        #endregion

        #region Properties
		  #endregion

		 #region public methods
        public bool AddBOLProduct(string ProductCode, int BlendID, double GrossGallons, double NetTemperature, int NetTemperatureFlag, int CreditIndicator)
        {
            bool result = false;

            // only added products to BOL if we haven't reached the limit
            if (PIDXConstants.MAX_BOL_PRODUCTS > this.ProductArrayList.Count)
            {
                BOLCBProduct cbproduct = new BOLCBProduct();
                cbproduct.ProductCode = ProductCode;
                cbproduct.BlendIDDigit = BlendID;
                cbproduct.GrossDigit = GrossGallons;
                cbproduct.NetTemperatureDigit = NetTemperature;
                cbproduct.NetTemperatureFlagDigit = NetTemperatureFlag;
                cbproduct.CreditIndicatorDigit = CreditIndicator;

                ProductArrayList.Add(cbproduct);
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

            string bolRecord = "";
            bolRecord = bolRecord +
                        base.TransactionType +
                        base.SPLCCode +
                        base.TerminalOperator +
                        base.SellerID +
                        base.ConsigneeNumber +
                        base.FinalShipperID +
                        base.CarrierID +
                        base.TruckNumber +
                        base.BOLNumber +
					         base.ShippedDate;

            // Add each defined bolbbproduct to the BB
            for (int count = 0; count < ProductArrayList.Count; count++)
            {
                BOLCBProduct cbproduct = (BOLCBProduct)ProductArrayList[count];

                bolRecord +=
                cbproduct.ProductCode +
                cbproduct.BlendID +
                cbproduct.Gross +
                cbproduct.NetTemperature +
                cbproduct.NetTemperatureFlag +
                cbproduct.CreditIndicator;
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

            if ((base.ConsigneeNumber == null) ||
                (base.ConsigneeNumber.Length > PIDXConstants.CONSIGNEE_ID_LENGTH) ||
                (base.ConsigneeNumber.Length < 1))
            {
                throw new PIDXException(PIDXConstants.ERR_MSG_008);
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        private void Initialize()
        {
        }
        #endregion
    }
}
