namespace FMBusinessObjects.PIDXTransactions
{
    using System;
    using System.Collections;

    using FMBusinessObjects.Constants;
    using FMBusinessObjects.Exceptions;

    // ReSharper disable once InconsistentNaming
    public class AuthorizationGrantedCA : AuthorizationGrantedBase
	{
		#region Private attributes
		private ArrayList productIdentifierList;
		#endregion

		#region Properties
		public ArrayList ProductIdentifierList => this.productIdentifierList;

        public bool HasProductIdentifiers
		{
			get
			{
				if (( this.productIdentifierList == null ) || ( this.productIdentifierList.Count <= 0 ))
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}
		#endregion

		#region Public override methods
		/// <summary>
		/// This method parses the CA response.
		/// </summary>
		/// <param name="response"></param>
		public override void Parse ( string response )
		{
			int headerLength = base.ParseHeader ( response );
			this.productIdentifierList = new ArrayList ( );

			int respEnd = response.IndexOf ( "R?", StringComparison.Ordinal ) - 1;
			int productLength = respEnd - headerLength;

			if (productLength > 0)
			{
				string authorizedProducts = response.Substring(31, productLength);
				this.ParseProductAuthorizations(authorizedProducts);
			}
			// raw response needed to check validity against check digit
		    this.ResponseNoCheckDigit = response.Substring ( 0, respEnd );
		}

        /// <summary>
        /// This method will parse out the product authorizations
        /// </summary>
        /// <param name="productStr"></param>
        protected override void ParseProductAuthorizations(string productStr)
        {
            string productIdent = productStr.Replace(" ", "");

            if (!string.IsNullOrEmpty(productIdent))
            {
                for (int nextChar = 0; nextChar < productIdent.Length; nextChar++)
                {
                    PIDXProductAuthorization productAuthorization = new PIDXProductAuthorization
                                                                    {
                                                                        ProductTypeIndicator = "F",
                                                                        PidxProductOrFamily = productIdent.Substring(nextChar, 1)
                                                                    };
                    this.PIDXProductAuthorizations.Add(productAuthorization);
                }
            }
        }

        /// <summary>
        /// This method parse the header portion of the response along with check
        /// digit value.
        /// </summary>
        /// <param name="response"></param>
        protected override int ParseHeader(string response)
        {
            int headerLength;

            if (!string.IsNullOrEmpty(response))
            {
                if (response.IndexOf("E!", StringComparison.Ordinal) >= 0)
                {
                    throw new PIDXException(PIDXConstants.ERR_MSG_025, PIDXException.ErrorTypes.WARNING);
                }

                int respLength = response.Length;
                int respEnd = response.IndexOf("R?", StringComparison.Ordinal) - 1;

                headerLength = 31;

                if (respLength < headerLength
                || respEnd < 0)
                {
                    throw new PIDXException(PIDXConstants.ERR_MSG_021);
                }

                this.ResponseType = response.Substring(0, 4);
                this.AuthorizationNumber = response.Substring(5, 8);
                this.ConsigneeNumber = response.Substring(13, 14);
                this.CarrierID = response.Substring(27, 4);
                this.CheckDigit37 = response.Substring(respEnd, 1);
                this.TerminatingString = response.Substring(respEnd + 1);
            }
            else
            {
                throw new PIDXException(PIDXConstants.ERR_MSG_022);
            }

            return headerLength;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method will parse out the product identifiers
        /// </summary>
        /// <param name="productStr"></param>
        private void FindProductIndentifiers ( string productStr )
		{
			string productIdent = productStr.Replace ( " ", "" );

			if (!string.IsNullOrEmpty(productIdent))
			{
				for (int nextChar = 0; nextChar < productIdent.Length; nextChar++)
				{
					this.productIdentifierList.Add ( productIdent.Substring ( nextChar, 1 ) );
				}
			}
		}
		#endregion
	}
}