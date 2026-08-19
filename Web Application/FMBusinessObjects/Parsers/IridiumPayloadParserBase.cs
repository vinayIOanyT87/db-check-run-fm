namespace FMBusinessObjects.Parsers
{
	using System;

	public abstract class IridiumPayloadParserBase
	{
		#region Public properties
		public byte Checksum1 { get; private set; }
		public byte Checksum2 { get; private set; }
		public ushort ChecksumTotal { get; private set; }
		public bool ChecksumValid { get; set; }

		public int NorthSouthIndicator { get; set; }
		public int EastWestIndicator { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public double Crc { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		protected IridiumPayloadParserBase()
		{
			this.BaseInit();
		}
		#endregion

		#region Protected methods
		/// <summary>
		/// This method will parse out the checksum value.
		/// </summary>
		/// <param name="payloadArray">The payload array.</param>
		/// <param name="length">The length of the payload message</param>
		protected virtual void ParseChecksum(byte[] payloadArray, int length)
		{
			this.Checksum1 = payloadArray[length - 2];
			this.Checksum2 = payloadArray[length - 1];

			ushort upper = (ushort)(this.Checksum1 << 8);
			this.ChecksumTotal = (ushort)(upper | this.Checksum2);
		}

		/// <summary>
		/// This method will calculate the checksum based on the payload array and 
		/// compare it to the checksum in the message.
		/// </summary>
		/// <param name="payloadArray">The byte array that contains the payload data.</param>
		/// <param name="length">The length of the paylaod data.</param>
		/// <returns>Returns true if the calculated checksum equals the sent checksum. Otherwise, it returns false.</returns>
		protected virtual bool CompareChecksum(byte[] payloadArray, int length)
		{
			// The ending index is to not include the checksum bytes which are the last
			// two bytes in the array.
			int endIndex = length - 2;
			ushort calculatedCheckSum = 0;

			for (int index = 0; index < endIndex; index++)
			{
				calculatedCheckSum += (ushort)payloadArray[index];
			}

			return this.ChecksumTotal == calculatedCheckSum;
		}

		/// <summary>
		/// This method will convert an IEEE 4 byte representation of a floating point number
		/// to a decimal number (double).
		/// </summary>
		/// <param name="payloadData">The payload data.</param>
		/// <param name="startingIndex">The starting index of the 4 bytes to be converted.</param>
		/// <returns></returns>
		protected double IeeeBitToFloatConverter(byte[] payloadData, int startingIndex)
		{
			// [        byte 1       ] [    byte 2           ] [    byte 3           ] [   byte 4            ]
			// 32|31 30 29 28 27 26 25 24|23 22 21 20 19 18 17 16 15 14 13 12 11 10 09 08 07 06 05 04 03 02 01
			// --|-----------------------|--------------------------------------------------------------------
			//  s|     Exponent          |                     Mantissa


			//         Bits
			//========================
			// 08 07 06 05 04 03 02 01

			// The sign bit is in byte 1 and bit 16.
			int signBit = (payloadData[startingIndex] & 0x80) >> 7;
			int sign = signBit == 0 ? 1 : -1;

			// The exponent is 8 bits in length. It is in byte 1 (bits 7 - 1) and byte 2 (bit 8).
			byte exponentMask1		= (byte)(payloadData[startingIndex] & 0x7f);
			byte exponentMask2		= (byte)(payloadData[startingIndex + 1] & 0x80);
			byte exponentByte		= (byte)((exponentMask1 << 1) | (exponentMask2 >> 7));
			int originalExponentInt = this.ConvertExponentBitsToInteger(exponentByte);
			int exponentInt;

			byte mantissaMask1 = (byte)(payloadData[startingIndex + 1] & 0x7f);
			byte mantissaMask2 = (byte)(payloadData[startingIndex + 2] & 0x80);
			byte mantissaMask3 = (byte)(payloadData[startingIndex + 2] & 0x7f);
			byte mantissaMask4 = (byte)(payloadData[startingIndex + 3] & 0x80);
			byte mantissaMask5 = (byte)(payloadData[startingIndex + 3] & 0x7f);

			byte[] mantissaBytes = new byte[3];
			mantissaBytes[0]	 = (byte)((mantissaMask1 << 1)| (mantissaMask2 >> 7));
			mantissaBytes[1]	 = (byte)((mantissaMask3 << 1) | (mantissaMask4 >> 7));
			mantissaBytes[2]	 = (byte)(mantissaMask5 << 1);

			// When exponent is zero then the it is denormalized and the value is set to -126.
			if (originalExponentInt == 0)
			{
				exponentInt = -126;
			}
			else
			{
				// If not denormalized, then subtract 127 from the original value.
				exponentInt = originalExponentInt - 127;
			}

			double mantissaDouble = this.ConvertMantissaBitsToInteger(mantissaBytes);

			// When exponent is not zero then it is not denormalized and 
			// the mantissa value is added to 1.
			if (originalExponentInt != 0)
			{
				mantissaDouble = mantissaDouble + 1.0;
			}


			double decimalValue = sign * Math.Pow(2, exponentInt) * mantissaDouble;

			return decimalValue;

		}

		/// <summary>
		/// This method will parse the coordinate information from the
		/// payload.
		/// </summary>
		/// <param name="offset">The start byte of the coordinate info.</param>
		/// <param name="payloadArray">The payload array containing the coordinate info.</param>
		protected void ParseCoordinates(int offset, byte[] payloadArray)
		{
			const double MinuteThousands = 60000;

			this.NorthSouthIndicator = (payloadArray[offset] & 0x02) >> 1;
			this.EastWestIndicator = payloadArray[offset] & 0x01;

			int latitudeInt = payloadArray[offset + 1];
			uint latitudeMsInt = payloadArray[offset + 2];
			uint latitudeLsInt = payloadArray[offset + 3];
			uint latitudeThousandsMinute = ((latitudeMsInt << 8) | latitudeLsInt);
			double decimalRepresentation = latitudeThousandsMinute / MinuteThousands;

			double sign = this.NorthSouthIndicator == 0 ? 1 : -1;
			this.Latitude = (latitudeInt + decimalRepresentation) * sign;

			int longitudeInt = payloadArray[offset + 4];
			uint longitudeMsInt = payloadArray[offset + 5];
			uint longitudeLsInt = payloadArray[offset + 6];
			uint longitudeThousandsMinute = ((longitudeMsInt << 8) | longitudeLsInt);
			decimalRepresentation = longitudeThousandsMinute / MinuteThousands;

			sign = this.EastWestIndicator == 0 ? 1 : -1;
			this.Longitude = (longitudeInt + decimalRepresentation) * sign;

			int crcMs = payloadArray[offset + 7];
			int crcLs = payloadArray[offset + 8];

			string crcStr = crcMs.ToString() + crcLs;
			this.Crc = int.Parse(crcStr);
		}

		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		protected void BaseInit()
		{
			this.Checksum1		= 0x00;
			this.Checksum2		= 0x00;
			this.ChecksumTotal	= 0;
			this.ChecksumValid	= false;

			this.NorthSouthIndicator	= 0;
			this.EastWestIndicator		= 0;
			this.Latitude				= 0.0;
			this.Longitude				= 0.0;
			this.Crc					= 0.0;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will convert bits to a integer value.
		/// </summary>
		/// <param name="inExponentByte"></param>
		/// <returns></returns>
		private int ConvertExponentBitsToInteger(byte inExponentByte)
		{
			int total = 0;
			int weight = 128;
			byte exponentByte = inExponentByte;

			for (int bitIndex = 1; bitIndex < 9; bitIndex++)
			{
				byte bit = (byte)(exponentByte & 0x80);

				if (bit == 0x80)
				{
					total = total + weight;
				}

				exponentByte = (byte)(exponentByte << 1);
				weight = weight / 2;
			}

			return total;
		}

		/// <summary>
		/// This method will convert the mantiss bits to a double number.
		/// </summary>
		/// <param name="mantissaBytes"></param>
		/// <returns></returns>
		private double ConvertMantissaBitsToInteger(byte[] mantissaBytes)
		{
			double total = 0;
			double weight = 0.5;
			int byteCount = 1;

			foreach(byte nextMantissaByte in mantissaBytes)
			{
				byte mantissaByte = nextMantissaByte;

				for (int bitIndex = 1; bitIndex < 9; bitIndex++)
				{
					// Ignore the last bit (bit 1) in byte 3.  The mantissa is 23 bits in length
					// and shifted left one bit.
					if (byteCount == 3 && bitIndex == 8)
					{
						break;
					}

					byte bit = (byte)(mantissaByte & 0x80);

					if (bit == 0x80)
					{
						total = total + weight;
					}

					mantissaByte = (byte)(mantissaByte << 1);
					weight = weight / 2;
				}

				byteCount++;
			}

			return total;
		}
		#endregion
	}
}
