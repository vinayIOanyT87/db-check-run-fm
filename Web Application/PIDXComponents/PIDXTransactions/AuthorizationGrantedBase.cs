/// <summary>
/// File name:	AuthorizationGrantedBase.cs
/// Purpose:	
///	Comments:	Copyright (C) Varec, Inc. Norcross, GA, USA, 
///				2008.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Varec, Inc.
///	Author(s):	Ivan Orndorff
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:		By:				Reason:
///		----------	-------------	-------------------------------------------
///		09-Jul-08	I.Orndorff		1.0.5 - Modified "ValidateCheckBit()" to set the 
///											checkdigitvalue to zero when the calculation
///											returned 37. This fixes CSI #6020.
///		
/// </summary>
/// 

using System;
using System.Collections;
using System.Text;

namespace PIDXTransactions
{
    public abstract class AuthorizationGrantedBase : PIDXAuthorizationBase
    {
        #region Private attributes
        private string authorizationNumber;
        private string consigneeNumber;
        private string carrierID;
        private string checkDigit;
        private string terminatingString;
        private string responseNoCheckDigit;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor for the authorization granted base class.
        /// </summary>
        public AuthorizationGrantedBase()
        {
            this.Initialize();
        }
        #endregion

        #region Properties
        public string AuthorizationNumber
        {
            get { return this.authorizationNumber; }
            set { this.authorizationNumber = value; }
        }

        public string ConsigneeNumber
        {
            get { return this.consigneeNumber; }
            set { this.consigneeNumber = value; }
        }

        public string CarrierID
        {
            get { return this.carrierID; }
            set { this.carrierID = value; }
        }

        public string CheckDigit
        {
            get { return this.checkDigit; }
            set { this.checkDigit = value; }
        }

        public string TerminatingString
        {
            get { return this.terminatingString; }
            set { this.terminatingString = value; }
        }

        public string ResponseNoCheckDigit
        {
            get { return this.responseNoCheckDigit; }
            set { this.responseNoCheckDigit = value; }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        private void Initialize()
        {
            this.authorizationNumber = null;
            this.consigneeNumber     = null;
            this.carrierID           = null;
            this.checkDigit          = null;
            this.terminatingString   = null;
            this.responseNoCheckDigit = null;
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// This method parse the header portion of the response along with check
        /// digit value.
        /// </summary>
        /// <param name="response"></param>
        protected int ParseHeader(string response)
        {
            int headerLength = 31;

            if ((response != null) && (response.Length > 0))
            {
                if (response.IndexOf("E!") >= 0)
                {
                    throw new PIDXException(PIDXConstants.ERR_MSG_025, PIDXException.ErrorTypes.WARNING);
                }

                int respLength = response.Length;
                int respEnd = response.IndexOf("R?") - 1;

                if ((respLength < headerLength) || (respEnd < 0))
                {
                    throw new PIDXException(PIDXConstants.ERR_MSG_021);
                }

                base.ResponseType        = response.Substring(0, 4);
                this.AuthorizationNumber = response.Substring(5, 8);
                this.ConsigneeNumber     = response.Substring(13, 14);
                this.CarrierID           = response.Substring(27, 4);
                this.CheckDigit          = response.Substring(respEnd, 1);
                this.terminatingString   = response.Substring(respEnd + 1);
            }
            else
            {
                throw new PIDXException(PIDXConstants.ERR_MSG_022);
            }

            return headerLength;
        }
        #endregion

        #region public methods
        public bool ValidateCheckBit()
        {
            System.Collections.Hashtable mod37hashtable = new System.Collections.Hashtable();
            System.Collections.Hashtable mod37hashtablereverse = new System.Collections.Hashtable();
            bool result = false;

            // Added all entries into the hash table
            mod37hashtable.Add('0', 0);
            mod37hashtable.Add('1', 1);
            mod37hashtable.Add('2', 2);
            mod37hashtable.Add('3', 3);
            mod37hashtable.Add('4', 4);
            mod37hashtable.Add('5', 5);
            mod37hashtable.Add('6', 6);
            mod37hashtable.Add('7', 7);
            mod37hashtable.Add('8', 8);
            mod37hashtable.Add('9', 9);
            mod37hashtable.Add('A', 10);
            mod37hashtable.Add('B', 11);
            mod37hashtable.Add('C', 12);
            mod37hashtable.Add('D', 13);
            mod37hashtable.Add('E', 14);
            mod37hashtable.Add('F', 15);
            mod37hashtable.Add('G', 16);
            mod37hashtable.Add('H', 17);
            mod37hashtable.Add('I', 18);
            mod37hashtable.Add('J', 19);
            mod37hashtable.Add('K', 20);
            mod37hashtable.Add('L', 21);
            mod37hashtable.Add('M', 22);
            mod37hashtable.Add('N', 23);
            mod37hashtable.Add('O', 24);
            mod37hashtable.Add('P', 25);
            mod37hashtable.Add('Q', 26);
            mod37hashtable.Add('R', 27);
            mod37hashtable.Add('S', 28);
            mod37hashtable.Add('T', 29);
            mod37hashtable.Add('U', 30);
            mod37hashtable.Add('V', 31);
            mod37hashtable.Add('W', 32);
            mod37hashtable.Add('X', 33);
            mod37hashtable.Add('Y', 34);
            mod37hashtable.Add('Z', 35);
            mod37hashtable.Add(' ', 36);

            // Added all entries into the reverse hash table
            mod37hashtablereverse.Add(0, "0");
            mod37hashtablereverse.Add(1, "1");
            mod37hashtablereverse.Add(2, "2");
            mod37hashtablereverse.Add(3, "3");
            mod37hashtablereverse.Add(4, "4");
            mod37hashtablereverse.Add(5, "5");
            mod37hashtablereverse.Add(6, "6");
            mod37hashtablereverse.Add(7, "7");
            mod37hashtablereverse.Add(8, "8");
            mod37hashtablereverse.Add(9, "9");
            mod37hashtablereverse.Add(10, "A");
            mod37hashtablereverse.Add(11, "B");
            mod37hashtablereverse.Add(12, "C");
            mod37hashtablereverse.Add(13, "D");
            mod37hashtablereverse.Add(14, "E");
            mod37hashtablereverse.Add(15, "F");
            mod37hashtablereverse.Add(16, "G");
            mod37hashtablereverse.Add(17, "H");
            mod37hashtablereverse.Add(18, "I");
            mod37hashtablereverse.Add(19, "J");
            mod37hashtablereverse.Add(20, "K");
            mod37hashtablereverse.Add(21, "L");
            mod37hashtablereverse.Add(22, "M");
            mod37hashtablereverse.Add(23, "N");
            mod37hashtablereverse.Add(24, "O");
            mod37hashtablereverse.Add(25, "P");
            mod37hashtablereverse.Add(26, "Q");
            mod37hashtablereverse.Add(27, "R");
            mod37hashtablereverse.Add(28, "S");
            mod37hashtablereverse.Add(29, "T");
            mod37hashtablereverse.Add(30, "U");
            mod37hashtablereverse.Add(31, "V");
            mod37hashtablereverse.Add(32, "W");
            mod37hashtablereverse.Add(33, "X");
            mod37hashtablereverse.Add(34, "Y");
            mod37hashtablereverse.Add(35, "Z");
            mod37hashtablereverse.Add(36, " ");

            int totalvalueresult = 0;
            int tabledecrement = 36;

            // find the value of each character
            for (int count = 0; count < responseNoCheckDigit.Length; count++, tabledecrement--)
            {
                // only decrease table position to one, never use zero
                if (0 == tabledecrement)
                    tabledecrement = 36;

                int tablevalue = (int)mod37hashtable[responseNoCheckDigit[count]];

                // multilply by a decreasing count of 36 to 1
                int valueresult = tablevalue * tabledecrement;
                // sum all values
                totalvalueresult += valueresult;
            }

            // divide total sum of all values by 37
            int modremainder = totalvalueresult % 37;

            // subtract remainder from 37 to determine Check Digit
            int checkdigitvalue = 37 - modremainder;
			if(checkdigitvalue == 37)
				checkdigitvalue=0;

            // check the calculated check bit against the check bit gathered from "Parse()"
            if ((string)mod37hashtablereverse[checkdigitvalue] == checkDigit)
                result = true;

            return result;
        }
        #endregion

        #region abstract methods
        public abstract void Parse(string response);
        #endregion
    }
}
