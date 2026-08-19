/// <summary>
/// File name:	PIDXRecordBase.cs
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
///		19-Mar-08	I.Orndorff		1.0.3 - Made "ConsigneeNumber" property a virtual 
///											so that DTN derived classes can override 
///											and pad with spaces instead of zeros.
///		
///		16-Apr-08	I.Orndorff		1.0.4 - Modified "ConsigneeNumber" property to PadRight
///											with spaces instead of PadLeft with zeros.
///		
/// </summary>
/// 

using System;
using System.Collections;
using System.Text;

namespace PIDXTransactions
{
    public abstract class PIDXRecordBase
    {
        #region private attributes
        private string transactionType;
        private string checkDigit;
        private int    terminalOperator;
        private int    splcCode;
        private int    sellerID;
        private string consigneeNumber;
        private int    finalShipperID;
        private string carrierID;
        private int    truckNumber;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default construct for the PIDX record base class.
        /// </summary>
        public PIDXRecordBase()
        {
            this.Initialize();
        }
        #endregion

        #region Properties
        public int TerminalOperatorDigit
        {
            get { return this.terminalOperator;  }
            set { this.terminalOperator = value; }
        }

        public string TerminalOperator
        {
            get
            {
                string outStr = this.terminalOperator.ToString();
                int length = 3 - outStr.Length;

                for (int count = length; count > 0; count--)
                {
                    outStr = "0" + outStr;
                }

                return outStr;
            }
        }

        public int SPLCCodeDigit
        {
            get { return this.splcCode; }
            set { this.splcCode = Math.Abs(value); }
        }

        public string SPLCCode
        {
            get
            {
                string outStr = this.splcCode.ToString();
                int length = 6 - outStr.Length;

                for (int count = length; count > 0; count--)
                {
                    outStr = "0" + outStr;
                }

                return outStr;
            }
        }

        public int SellerIDDigit
        {
            get { return this.sellerID; }
            set { this.sellerID = Math.Abs(value); }
        }

        public string SellerID
        {
            get
            {
                string outStr = this.sellerID.ToString();
                int length = 3 - outStr.Length;

                for (int count = length; count > 0; count--)
                {
                    outStr = "0" + outStr;
                }

                return outStr;
            }
        }

        protected string CheckDigit
        {
            get { return this.checkDigit;  }
            set { this.checkDigit = value;  }
        }

        protected string TransactionType
        {
            get { return this.transactionType;  }
            set { this.transactionType = value; }

        }

		public virtual string ConsigneeNumber
		{
			get { return this.consigneeNumber; }
			set { this.consigneeNumber = value.PadRight(14,' '); }
        }

        public int FinalShipperIDDigit
        {
            get { return this.finalShipperID; }
            set { this.finalShipperID = Math.Abs(value); }
        }

        public string FinalShipperID
        {
            get
            {
                string outStr = this.finalShipperID.ToString();
                int length = 3 - outStr.Length;

                for (int count = length; count > 0; count--)
                {
                    outStr = "0" + outStr;
                }

                return outStr;
            }
        }

        public string CarrierID
        {
            get { return this.carrierID; }
            set { this.carrierID = value.PadLeft(8,'0'); }
        }

        public int TruckNumberDigit
        {
            get { return this.truckNumber; }
            set { this.truckNumber = Math.Abs(value); }
        }

        public string TruckNumber
        {
            get
            {
                string outStr = this.truckNumber.ToString();
                int length = 6 - outStr.Length;

                for (int count = length; count > 0; count--)
                {
                    outStr = "0" + outStr;
                }

                return outStr;
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        private void Initialize()
        {
            this.checkDigit       = null;
            this.transactionType  = null;
            this.terminalOperator = -99;
            this.splcCode         = -99;
            this.sellerID         = -99;
            this.consigneeNumber  = null;
            this.finalShipperID   = -99;
            this.carrierID        = null;
            this.truckNumber      = -99;
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// This method validates for the TDS base fields.  It throws
        /// an exception if the validation fails.
        /// </summary>
        protected void Validate()
        {
            if (this.terminalOperator == -99)
            {
                throw new PIDXException(PIDXConstants.ERR_MSG_002);
            }

            if (this.SPLCCodeDigit == -99)
            {
                throw new PIDXException(PIDXConstants.ERR_MSG_001);
            }

            if (this.SellerIDDigit == -99)
            {
                throw new PIDXException(PIDXConstants.ERR_MSG_003);
            }

            if (this.truckNumber == -99)
            {
                throw new PIDXException(PIDXConstants.ERR_MSG_004);
            }

            if (this.finalShipperID == -99)
            {
                throw new PIDXException(PIDXConstants.ERR_MSG_006);
            }

            if ((this.carrierID == null) ||
                (this.carrierID.Length != PIDXConstants.CARRIER_ID_LENGTH))
            {
                throw new PIDXException(PIDXConstants.ERR_MSG_007);
            }
        }

        protected void GenerateCheckBit(string Record)
        {
            System.Collections.Hashtable mod37hashtable = new System.Collections.Hashtable();
            System.Collections.Hashtable mod37hashtablereverse = new System.Collections.Hashtable();
            
            // Added all entries into the hash table
            mod37hashtable.Add('0',0);
            mod37hashtable.Add('1',1);
            mod37hashtable.Add('2',2);
            mod37hashtable.Add('3',3);
            mod37hashtable.Add('4',4);
            mod37hashtable.Add('5',5);
            mod37hashtable.Add('6',6);
            mod37hashtable.Add('7',7);
            mod37hashtable.Add('8',8);
            mod37hashtable.Add('9',9);
            mod37hashtable.Add('A',10);
            mod37hashtable.Add('B',11);
            mod37hashtable.Add('C',12);
            mod37hashtable.Add('D',13);
            mod37hashtable.Add('E',14);
            mod37hashtable.Add('F',15);
            mod37hashtable.Add('G',16);
            mod37hashtable.Add('H',17);
            mod37hashtable.Add('I',18);
            mod37hashtable.Add('J',19);
            mod37hashtable.Add('K',20);
            mod37hashtable.Add('L',21);
            mod37hashtable.Add('M',22);
            mod37hashtable.Add('N',23);
            mod37hashtable.Add('O',24);
            mod37hashtable.Add('P',25);
            mod37hashtable.Add('Q',26);
            mod37hashtable.Add('R',27);
            mod37hashtable.Add('S',28);
            mod37hashtable.Add('T',29);
            mod37hashtable.Add('U',30);
            mod37hashtable.Add('V',31);
            mod37hashtable.Add('W',32);
            mod37hashtable.Add('X',33);
            mod37hashtable.Add('Y',34);
            mod37hashtable.Add('Z',35);
            mod37hashtable.Add(' ',36);

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
            for (int count = 0; count < Record.Length; count++, tabledecrement--)
            {
                // only decrease table position to one, never use zero
                if (0 == tabledecrement)
                    tabledecrement = 36;

                int tablevalue = (int) mod37hashtable[Record[count]];
                
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

            checkDigit = (string)mod37hashtablereverse[checkdigitvalue];   
        }
        #endregion

        #region Abstract methods
        /// <summary>
        /// This is an abstract method that forces implementation of the get PIDX record at the
        /// derived class.
        /// </summary>
        /// <returns></returns>
        public abstract string GetDataRecord();


        /// <summary>
        /// This is an abstract method that forces implementation of the get PIDX record 
        /// validation at the derived class.
        /// </summary>
        /// <returns></returns>
        public abstract void ValidateRecord();
        #endregion
    }
}
