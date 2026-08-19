 #pragma warning disable 1587
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
#pragma warning restore 1587
namespace FMBusinessObjects.PIDXTransactions
{
    using System;
    using System.Collections;
    using System.Text;

    using FMBusinessObjects.Constants;
    using FMBusinessObjects.Exceptions;
    using FMBusinessObjects.DataObjects;

    public abstract class PIDXRecordBase
	{
		#region private attributes
		private string transactionType;
		private string checkDigit;
		private int terminalOperator;
		private int splcCode;
		private int sellerID;
		private int finalShipperID;
		private string rackDriverID;
		private string terminalControlNumber;
		private string releaseOrderNumber;


		// Table of CRC values for high–order byte
		private static readonly byte[] CrcHiValues = {
			0x00, 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70,	0x81, 0x91, 0xa1, 0xb1, 0xc1, 0xd1, 0xe1, 0xf1,
			0x12, 0x02, 0x32, 0x22, 0x52, 0x42, 0x72, 0x62, 0x93, 0x83, 0xb3, 0xa3, 0xd3, 0xc3, 0xf3, 0xe3,
			0x24, 0x34, 0x04, 0x14, 0x64, 0x74, 0x44, 0x54, 0xa5, 0xb5, 0x85, 0x95, 0xe5, 0xf5, 0xc5, 0xd5,
			0x36, 0x26, 0x16, 0x06, 0x76, 0x66, 0x56, 0x46, 0xb7, 0xa7, 0x97, 0x87, 0xf7, 0xe7, 0xd7, 0xc7,
			0x48, 0x58, 0x68, 0x78, 0x08, 0x18, 0x28, 0x38, 0xc9, 0xd9, 0xe9, 0xf9, 0x89, 0x99, 0xa9, 0xb9,
			0x5a, 0x4a, 0x7a, 0x6a, 0x1a, 0x0a, 0x3a, 0x2a, 0xdb, 0xcb, 0xfb, 0xeb, 0x9b, 0x8b, 0xbb, 0xab,
			0x6c, 0x7c, 0x4c, 0x5c, 0x2c, 0x3c, 0x0c, 0x1c, 0xed, 0xfd, 0xcd, 0xdd, 0xad, 0xbd, 0x8d, 0x9d,
			0x7e, 0x6e, 0x5e, 0x4e, 0x3e, 0x2e, 0x1e, 0x0e, 0xff, 0xef, 0xdf, 0xcf, 0xbf, 0xaf, 0x9f, 0x8f,
			0x91, 0x81, 0xb1, 0xa1, 0xd1, 0xc1, 0xf1, 0xe1, 0x10, 0x00, 0x30, 0x20, 0x50, 0x40, 0x70, 0x60,
			0x83, 0x93, 0xa3, 0xb3, 0xc3, 0xd3, 0xe3, 0xf3, 0x02, 0x12, 0x22, 0x32, 0x42, 0x52, 0x62, 0x72,
			0xb5, 0xa5, 0x95, 0x85, 0xf5, 0xe5, 0xd5, 0xc5, 0x34, 0x24, 0x14, 0x04, 0x74, 0x64, 0x54, 0x44,
			0xa7, 0xb7, 0x87, 0x97, 0xe7, 0xf7, 0xc7, 0xd7, 0x26, 0x36, 0x06, 0x16, 0x66, 0x76, 0x46, 0x56,
			0xd9, 0xc9, 0xf9, 0xe9, 0x99, 0x89, 0xb9, 0xa9, 0x58, 0x48, 0x78, 0x68, 0x18, 0x08, 0x38, 0x28,
			0xcb, 0xdb, 0xeb, 0xfb, 0x8b, 0x9b, 0xab, 0xbb, 0x4a, 0x5a, 0x6a, 0x7a, 0x0a, 0x1a, 0x2a, 0x3a,
			0xfd, 0xed, 0xdd, 0xcd, 0xbd, 0xad, 0x9d, 0x8d, 0x7c, 0x6c, 0x5c, 0x4c, 0x3c, 0x2c, 0x1c, 0x0c,
			0xef, 0xff, 0xcf, 0xdf, 0xaf, 0xbf, 0x8f, 0x9f, 0x6e, 0x7e, 0x4e, 0x5e, 0x2e, 0x3e, 0x0e, 0x1e
		};

		// Table of CRC values for low–order byte
		private static readonly byte[] CrcLoValues = {
			0x00, 0x21, 0x42, 0x63, 0x84, 0xa5, 0xc6, 0xe7, 0x08, 0x29, 0x4a, 0x6b, 0x8c, 0xad, 0xce, 0xef,
			0x31, 0x10, 0x73, 0x52, 0xb5, 0x94, 0xf7, 0xd6, 0x39, 0x18, 0x7b, 0x5a, 0xbd, 0x9c, 0xff, 0xde,
			0x62, 0x43, 0x20, 0x01, 0xe6, 0xc7, 0xa4, 0x85, 0x6a, 0x4b, 0x28, 0x09, 0xee, 0xcf, 0xac, 0x8d,
			0x53, 0x72, 0x11, 0x30, 0xd7, 0xf6, 0x95, 0xb4, 0x5b, 0x7a, 0x19, 0x38, 0xdf, 0xfe, 0x9d, 0xbc,
			0xc4, 0xe5, 0x86, 0xa7, 0x40, 0x61, 0x02, 0x23, 0xcc, 0xed, 0x8e, 0xaf, 0x48, 0x69, 0x0a, 0x2b,
			0xf5, 0xd4, 0xb7, 0x96, 0x71, 0x50, 0x33, 0x12, 0xfd, 0xdc, 0xbf, 0x9e, 0x79, 0x58, 0x3b, 0x1a,
			0xa6, 0x87, 0xe4, 0xc5, 0x22, 0x03, 0x60, 0x41, 0xae, 0x8f, 0xec, 0xcd, 0x2a, 0x0b, 0x68, 0x49,
			0x97, 0xb6, 0xd5, 0xf4, 0x13, 0x32, 0x51, 0x70, 0x9f, 0xbe, 0xdd, 0xfc, 0x1b, 0x3a, 0x59, 0x78,
			0x88, 0xa9, 0xca, 0xeb, 0x0c, 0x2d, 0x4e, 0x6f, 0x80, 0xa1, 0xc2, 0xe3, 0x04, 0x25, 0x46, 0x67,
			0xb9, 0x98, 0xfb, 0xda, 0x3d, 0x1c, 0x7f, 0x5e, 0xb1, 0x90, 0xf3, 0xd2, 0x35, 0x14, 0x77, 0x56,
			0xea, 0xcb, 0xa8, 0x89, 0x6e, 0x4f, 0x2c, 0x0d, 0xe2, 0xc3, 0xa0, 0x81, 0x66, 0x47, 0x24, 0x05,
			0xdb, 0xfa, 0x99, 0xb8, 0x5f, 0x7e, 0x1d, 0x3c, 0xd3, 0xf2, 0x91, 0xb0, 0x57, 0x76, 0x15, 0x34,
			0x4c, 0x6d, 0x0e, 0x2f, 0xc8, 0xe9, 0x8a, 0xab, 0x44, 0x65, 0x06, 0x27, 0xc0, 0xe1, 0x82, 0xa3,
			0x7d, 0x5c, 0x3f, 0x1e, 0xf9, 0xd8, 0xbb, 0x9a, 0x75, 0x54, 0x37, 0x16, 0xf1, 0xd0, 0xb3, 0x92,
			0x2e, 0x0f, 0x6c, 0x4d, 0xaa, 0x8b, 0xe8, 0xc9, 0x26, 0x07, 0x64, 0x45, 0xa2, 0x83, 0xe0, 0xc1,
			0x1f, 0x3e, 0x5d, 0x7c, 0x9b, 0xba, 0xd9, 0xf8, 0x17, 0x36, 0x55, 0x74, 0x93, 0xb2, 0xd1, 0xf0 
		};


		#endregion

		#region protected attributes
		protected string carrierID;
		protected int truckNumber;
		protected string consigneeNumber;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default construct for the PIDX record base class.
		/// </summary>
		protected PIDXRecordBase ( )
		{
			this.Initialize ( );
		}
		#endregion

		#region Properties
		public int TerminalOperatorDigit
		{
			get { return this.terminalOperator; }
			set { this.terminalOperator = value; }
		}

		public string TerminalOperator => this.terminalOperator.ToString("D3");

        public int SPLCCodeDigit
		{
			get { return this.splcCode; }
			set { this.splcCode = Math.Abs ( value ); }
		}

		public string SPLCCode => this.splcCode.ToString("D6");

        public int SellerIDDigit
		{
			get { return this.sellerID; }
			set { this.sellerID = Math.Abs ( value ); }
		}

		public string SellerID => this.sellerID.ToString("D3");

        protected string CheckDigit
		{
			get { return this.checkDigit; }
			set { this.checkDigit = value; }
		}

		protected string TransactionType
		{
			get { return this.transactionType; }
			set { this.transactionType = value; }

		}

		public virtual string ConsigneeNumber
		{
			get { return this.consigneeNumber.PadRight(14, ' '); }
			set { this.consigneeNumber = value.Substring(0, (value.Length < 14) ? value.Length : 14); }
		}

		public int FinalShipperIDDigit
		{
			get { return this.finalShipperID; }
			set { this.finalShipperID = Math.Abs ( value ); }
		}

		public string FinalShipperID => this.finalShipperID.ToString("D3");

        public virtual string CarrierID
		{
			get { return this.carrierID.PadLeft(8, '0'); }
			set { this.carrierID = value; }
		}

		public string RackDriverID
		{
			get { return this.rackDriverID.ToUpper().PadRight(20, ' '); }
			set { this.rackDriverID = value.Substring(0,(value.Length < 20) ? value.Length : 20); }
		}

		public virtual int TruckNumberDigit
		{
			get { return this.truckNumber; }
			set { this.truckNumber = Math.Abs ( value ); }
		}

		public virtual string TruckNumber
		{
			get
			{
				string outStr = this.truckNumber.ToString ( );
				int length = 6 - outStr.Length;

				for (int count = length; count > 0; count--)
				{
					outStr = "0" + outStr;
				}

				return outStr;
			}
		}

		public string TerminalControlNumber
		{
			get { return this.terminalControlNumber.ToUpper().PadRight(9, ' '); }
			set { this.terminalControlNumber = value.Substring(0,(value.Length < 9) ? value.Length : 9); }
		}

		public string ReleaseOrderNumber
		{
			get { return this.releaseOrderNumber.PadRight(16, ' '); }
			set { this.releaseOrderNumber = value.Substring(0, (value.Length < 16) ? value.Length : 16); }
		}


		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Initialize ( )
		{
			this.checkDigit = null;
			this.transactionType = null;
			this.terminalOperator = -99;
			this.splcCode = -99;
			this.sellerID = -99;
			this.consigneeNumber = null;
			this.finalShipperID = -99;
			this.carrierID = null;
			this.releaseOrderNumber="";
			this.truckNumber = -99;
			this.terminalControlNumber=null;
			this.rackDriverID="";
		}
		#endregion

		#region Protected methods
		/// <summary>
		/// This method validates for the TDS base fields.  It throws
		/// an exception if the validation fails.
		/// </summary>
		protected void Validate ( )
		{
			if (this.terminalOperator == -99)
			{
				throw new PIDXException ( PIDXConstants.ERR_MSG_002 );
			}

			if (this.SPLCCodeDigit == -99)
			{
				throw new PIDXException ( PIDXConstants.ERR_MSG_001 );
			}

			if (this.SellerIDDigit == -99)
			{
				throw new PIDXException ( PIDXConstants.ERR_MSG_003 );
			}

			if (this.finalShipperID == -99)
			{
				throw new PIDXException ( PIDXConstants.ERR_MSG_006 );
			}

			if (string.IsNullOrEmpty(this.carrierID))
			{
				throw new PIDXException ( PIDXConstants.ERR_MSG_007 );
			}
		}

        // ReSharper disable once InconsistentNaming
		public static string CRC16(string dataString,int length)
		{
			// Translate the passed message into ASCII and store it as a Byte array.
			Byte[] data = Encoding.ASCII.GetBytes(dataString);

			string hex="0123456789ABCDEF";
			byte crcHi = 0x00;
			byte crcLo = 0x00;
		    int dataIndex=0;

			while(length > 0)
			{
				var crcIndex = (byte)(crcLo ^ data[dataIndex]);
				crcLo = (byte)(crcHi ^ CrcHiValues[crcIndex]);
				crcHi = CrcLoValues[crcIndex];

				dataIndex++;
				length--;
			}

			char [] crc={hex[(crcLo & 0xF0) >> 4],hex[(crcLo & 0x0F)],hex[(crcHi & 0xF0) >> 4],hex[(crcHi & 0x0F)]};
			return new String(crc);
		}


		protected void GenerateCheckBit ( string record )
		{
			Hashtable mod37Hashtable = new Hashtable ( );
			Hashtable mod37Hashtablereverse = new Hashtable ( );

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
			for (int count = 0; count < record.Length; count++, tabledecrement--)
			{
				// only decrease table position to one, never use zero
				if (0 == tabledecrement)
					tabledecrement = 36;

				char character=record[count];
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

		    this.checkDigit = (string) mod37Hashtablereverse[checkdigitvalue];
		}
		#endregion

		#region Abstract methods
		/// <summary>
		/// This is an abstract method that forces implementation of the get PIDX record at the
		/// derived class.
		/// </summary>
		/// <returns></returns>
		public abstract string GetDataRecord(PIDXVersion version);


		/// <summary>
		/// This is an abstract method that forces implementation of the get PIDX record 
		/// validation at the derived class.
		/// </summary>
		/// <returns></returns>
		public abstract void ValidateRecord ( );
		#endregion
	}
}