 #pragma warning disable 1587
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
#pragma warning restore 1587
namespace FMBusinessObjects.PIDXTransactions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using FMBusinessObjects.Constants;
    using FMBusinessObjects.Exceptions;

    public abstract class AuthorizationGrantedBase : PIDXAuthorizationBase
	{
		#region Private attributes
		private string authorizationNumber;
		private string consigneeNumber;
		private string carrierID;
		private string totalPidxProducts;
		private string productAllocationMethod;
		private string checkDigit37;
		private string checkDigit16;
		private string terminatingString;
		private string responseNoCheckDigit;

        #endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the authorization granted base class.
		/// </summary>
		protected AuthorizationGrantedBase ( )
		{
			this.Initialize ( );
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

		public string TotalPidxProducts
		{
			get { return this.totalPidxProducts; }
			set { this.totalPidxProducts = value; }
		}

		public string ProductAllocationMethod
		{
			get { return this.productAllocationMethod; }
			set { this.productAllocationMethod = value; }
		}

		public string CheckDigit37
		{
			get { return this.checkDigit37; }
			set { this.checkDigit37 = value; }
		}

		public string CheckDigit16
		{
			get { return this.checkDigit16; }
			set { this.checkDigit16 = value; }
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

		public List<PIDXProductAuthorization> PIDXProductAuthorizations { get; } = new List<PIDXProductAuthorization>();

        #endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Initialize ( )
		{
			this.authorizationNumber = null;
			this.consigneeNumber = null;
			this.carrierID = null;
			this.totalPidxProducts = null;
			this.productAllocationMethod = null;
			this.checkDigit37 = null;
			this.checkDigit16 = null;
			this.terminatingString = null;
			this.responseNoCheckDigit = null;
		}
		#endregion

		#region Protected methods
		#endregion

		#region public methods
		public bool ValidateCheckBit ( )
		{
			if(this.checkDigit16 != null
			&& this.checkDigit16 != "    ")
			{
				if(this.checkDigit16 == PIDXRecordBase.CRC16(this.responseNoCheckDigit, this.responseNoCheckDigit.Length))
					return true;

				return false;
			}


			Hashtable mod37Hashtable = new Hashtable();
			Hashtable mod37Hashtablereverse = new Hashtable();
			bool result = false;

			// Added all entries into the hash table
			mod37Hashtable.Add ( '0', 0 );
			mod37Hashtable.Add ( '1', 1 );
			mod37Hashtable.Add ( '2', 2 );
			mod37Hashtable.Add ( '3', 3 );
			mod37Hashtable.Add ( '4', 4 );
			mod37Hashtable.Add ( '5', 5 );
			mod37Hashtable.Add ( '6', 6 );
			mod37Hashtable.Add ( '7', 7 );
			mod37Hashtable.Add ( '8', 8 );
			mod37Hashtable.Add ( '9', 9 );
			mod37Hashtable.Add ( 'A', 10 );
			mod37Hashtable.Add ( 'B', 11 );
			mod37Hashtable.Add ( 'C', 12 );
			mod37Hashtable.Add ( 'D', 13 );
			mod37Hashtable.Add ( 'E', 14 );
			mod37Hashtable.Add ( 'F', 15 );
			mod37Hashtable.Add ( 'G', 16 );
			mod37Hashtable.Add ( 'H', 17 );
			mod37Hashtable.Add ( 'I', 18 );
			mod37Hashtable.Add ( 'J', 19 );
			mod37Hashtable.Add ( 'K', 20 );
			mod37Hashtable.Add ( 'L', 21 );
			mod37Hashtable.Add ( 'M', 22 );
			mod37Hashtable.Add ( 'N', 23 );
			mod37Hashtable.Add ( 'O', 24 );
			mod37Hashtable.Add ( 'P', 25 );
			mod37Hashtable.Add ( 'Q', 26 );
			mod37Hashtable.Add ( 'R', 27 );
			mod37Hashtable.Add ( 'S', 28 );
			mod37Hashtable.Add ( 'T', 29 );
			mod37Hashtable.Add ( 'U', 30 );
			mod37Hashtable.Add ( 'V', 31 );
			mod37Hashtable.Add ( 'W', 32 );
			mod37Hashtable.Add ( 'X', 33 );
			mod37Hashtable.Add ( 'Y', 34 );
			mod37Hashtable.Add ( 'Z', 35 );
			mod37Hashtable.Add ( ' ', 36 );

			// Added all entries into the reverse hash table
			mod37Hashtablereverse.Add ( 0, "0" );
			mod37Hashtablereverse.Add ( 1, "1" );
			mod37Hashtablereverse.Add ( 2, "2" );
			mod37Hashtablereverse.Add ( 3, "3" );
			mod37Hashtablereverse.Add ( 4, "4" );
			mod37Hashtablereverse.Add ( 5, "5" );
			mod37Hashtablereverse.Add ( 6, "6" );
			mod37Hashtablereverse.Add ( 7, "7" );
			mod37Hashtablereverse.Add ( 8, "8" );
			mod37Hashtablereverse.Add ( 9, "9" );
			mod37Hashtablereverse.Add ( 10, "A" );
			mod37Hashtablereverse.Add ( 11, "B" );
			mod37Hashtablereverse.Add ( 12, "C" );
			mod37Hashtablereverse.Add ( 13, "D" );
			mod37Hashtablereverse.Add ( 14, "E" );
			mod37Hashtablereverse.Add ( 15, "F" );
			mod37Hashtablereverse.Add ( 16, "G" );
			mod37Hashtablereverse.Add ( 17, "H" );
			mod37Hashtablereverse.Add ( 18, "I" );
			mod37Hashtablereverse.Add ( 19, "J" );
			mod37Hashtablereverse.Add ( 20, "K" );
			mod37Hashtablereverse.Add ( 21, "L" );
			mod37Hashtablereverse.Add ( 22, "M" );
			mod37Hashtablereverse.Add ( 23, "N" );
			mod37Hashtablereverse.Add ( 24, "O" );
			mod37Hashtablereverse.Add ( 25, "P" );
			mod37Hashtablereverse.Add ( 26, "Q" );
			mod37Hashtablereverse.Add ( 27, "R" );
			mod37Hashtablereverse.Add ( 28, "S" );
			mod37Hashtablereverse.Add ( 29, "T" );
			mod37Hashtablereverse.Add ( 30, "U" );
			mod37Hashtablereverse.Add ( 31, "V" );
			mod37Hashtablereverse.Add ( 32, "W" );
			mod37Hashtablereverse.Add ( 33, "X" );
			mod37Hashtablereverse.Add ( 34, "Y" );
			mod37Hashtablereverse.Add ( 35, "Z" );
			mod37Hashtablereverse.Add ( 36, " " );

			int totalvalueresult = 0;
			int tabledecrement = 36;

			// find the value of each character
			for (int count = 0; count < this.responseNoCheckDigit.Length; count++, tabledecrement--)
			{
				// only decrease table position to one, never use zero
				if (0 == tabledecrement)
					tabledecrement = 36;

				char character= this.responseNoCheckDigit[count];

				if(!mod37Hashtable.ContainsKey(character))
					continue;

				int tablevalue = (int)mod37Hashtable[character];

				// multilply by a decreasing count of 36 to 1
				int valueresult = tablevalue * tabledecrement;
				// sum all values
				totalvalueresult += valueresult;
			}

			// divide total sum of all values by 37
			int modremainder = totalvalueresult % 37;

			// subtract remainder from 37 to determine Check Digit
			int checkdigitvalue = 37 - modremainder;
			if (checkdigitvalue == 37)
				checkdigitvalue = 0;

			// check the calculated check bit against the check bit gathered from "Parse()"
			if ((string)mod37Hashtablereverse[checkdigitvalue] == this.checkDigit37)
				result = true;

			return result;
		}
		#endregion

		#region abstract methods
		public abstract void Parse(string response);

		protected virtual int ParseHeader(string response)
        {
            int headerLength = 31;

            if (!string.IsNullOrEmpty(response))
            {
                if (response.IndexOf("E!", StringComparison.Ordinal) >= 0)
                {
                    throw new PIDXException(PIDXConstants.ERR_MSG_025, PIDXException.ErrorTypes.WARNING);
                }

                int respLength = response.Length;
                int respEnd = response.IndexOf("R?", StringComparison.Ordinal) - 1;

                if ((respLength < headerLength) || (respEnd < 0))
                {
                    throw new PIDXException(PIDXConstants.ERR_MSG_021);
                }

                this.ResponseType = response.Substring(0, 4);
                this.AuthorizationNumber = response.Substring(5, 8);
                this.ConsigneeNumber = response.Substring(13, 14);
                this.CarrierID = response.Substring(27, 4);
                this.CheckDigit37 = response.Substring(respEnd, 1);
                this.terminatingString = response.Substring(respEnd + 1);
            }
            else
            {
                throw new PIDXException(PIDXConstants.ERR_MSG_022);
            }

            return headerLength;
        }

        protected abstract void ParseProductAuthorizations(string productAuthorizations);
		#endregion
	}
}