namespace DataObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Xml.Serialization;

	[Serializable, XmlRoot("MobileOriginatedMessageDO")]
	public class MobileOriginatedMessageDO : MessageBaseDO
	{
		#region Private data members
		/// <summary>
		/// MO Header IEI (Information Element Identifier).
		/// Type: Char
		/// Length: 1 byte
		/// </summary>
		private byte headerIei;

		/// <summary>
		/// MO Header Length.
		/// Type: unsigned short
		/// Length: 2 bytes
		/// </summary>
		private ushort headerLength;

		/// <summary>
		/// CDR (Called Detail Record) Reference (Auto ID).
		/// Type: unsigned integer
		/// Length: 4 bytes
		/// </summary>
		private uint cdrReference;

		/// <summary>
		/// IMEI (International Mobile Equipment Identity).  ASCII Numeric characters
		/// Type: char
		/// Length: 15 bytes
		/// </summary>
		private string imei;

		/// <summary>
		/// Session Status: 
		///  0 - The SBD session completed successfully
		///  1 - The MO message transfer, if any, was successful. The MT message queued at the
		///      Iridium gateway is too large to be transferred within a single SBD session.
		///  2 - The MO message transfer, if any, was successful. The reported location was 
		///      determined to be of unacceptable quality. This value is only applicable to 
		///      IMEIs using SBD protocol revision 1.
		/// 10 - The SBD session timed out before session completion.
		/// 12 - The MO message being transferred by the IMEI is too large to be transferred 
		///      within a single SBD session.
		/// 13 - An RF link loss occurred during the SBD session.
		/// 14 - An IMEI protocol anomaly occurred during SBD session.
		/// 15 - The IMEI is prohibited from accessing the Iridium Gateway.
		/// Type: unsigned char
		/// Length: 1 byte
		/// </summary>
		private ushort sessionStatus;

		/// <summary>
		/// MOMSN - Mobile Originated Message Sequence Number.
		/// Type: unsigned short
		/// Length: 2 bytes
		/// </summary>
		private ushort momsn;

		/// <summary>
		/// MTMSN - Mobile Terminated Message Sequence Number.
		/// Type: unsigned short
		/// Length: 2 bytes
		/// </summary>
		private ushort mtmsn;

		/// <summary>
		/// Time Of Session (Epoch Time)
		/// Type: unsigned integer
		/// Length: 4 bytes
		/// </summary>
		private uint timeOfSession;

		/// <summary>
		/// Message Originated Payload Information Element Identifier.
		/// Type: char
		/// Length: 1 byte
		/// </summary>
		private byte moPayloadIei;

		/// <summary>
		/// Message Originated Payload Length.
		/// Type: unsigned short
		/// Length: 2 bytes
		/// </summary>
		private ushort moPayloadLength;

		/// <summary>
		/// Message Originated Payload content.
		/// Type: char
		/// Length: 1 - 1960 bytes
		/// </summary>
		private List<PayloadDO> moPayload;

		/// <summary>
		/// Message Originated Location Information Element Identifier.
		/// Type: char 
		/// Length: 1 byte
		/// </summary>
		private byte moLocationIei;

		/// <summary>
		/// Message Originated Location Information Length.
		/// Type: unsinged short
		/// Length: 2 bytes
		/// </summary>
		private ushort moLocationInfoLength;

		/// <summary>
		/// The Latitude/Longitude value
		/// </summary>
		private CoordinateDO latitudeLongitude;

		/// <summary>
		/// The CEP Radius. This is the radius around the center of the
		/// location.
		/// Type: unsigned integer
		/// Length: 4 bytes
		/// </summary>
		private uint? cepRadius;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor
		/// </summary>
		public MobileOriginatedMessageDO()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public byte HeaderIei
		{
			get { return this.headerIei; }
		}

		public ushort HeaderLength
		{
			get { return this.headerLength; }
		}

		public uint CdrReference
		{
			get { return this.cdrReference; }
			set { this.cdrReference = value; }
		}

		public string Imei
		{
			get { return this.imei; }
			set { this.imei = value; }
		}

		public ushort SessionStatus
		{
			get { return this.sessionStatus; }
			set { this.sessionStatus = value; }
		}

		public ushort Momsn
		{
			get { return this.momsn; }
			set { this.momsn = value; }
		}

		public ushort Mtmsn
		{
			get { return this.mtmsn; }
			set { this.mtmsn = value; }
		}

		public uint TimeOfSession
		{
			get { return this.timeOfSession; }
			set
			{
				this.timeOfSession = value;
			}
		}

		public byte MoPayloadIei
		{
			get { return this.moPayloadIei; }
		}

		public ushort MoPayloadLength
		{
			get { return this.moPayloadLength; }
		}

		public List<PayloadDO> MoPayload
		{
			get { return this.moPayload; }
			set { this.moPayload = value; }
		}

		public byte MoLocationIei
		{
			get { return this.moLocationIei; }
		}

		public ushort MoLocationInfoLength
		{
			get { return this.moLocationInfoLength; }
			set { this.moLocationInfoLength = value; }
		}

		public CoordinateDO LatitudeLongitude
		{
			get { return this.latitudeLongitude; }
			set { this.latitudeLongitude = value; }
		}

		public uint? CepRadius
		{
			get { return this.cepRadius; }
			set { this.cepRadius = value; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will create the MO message.
		/// </summary>
		/// <returns>Returns the MO message as a byte array.</returns>
		public byte[] CreateMessage()
		{
			byte[] headerArray			= this.CreateHeaderMessage();
			byte[] locationArray		= this.CreateLocationMessage();
			byte[] payloadArray			= this.CreatePayloadMessage();

			int messageLength = headerArray.Length;

			if (locationArray != null)
			{
				messageLength = messageLength + locationArray.Length;
			}

			if (payloadArray != null)
			{
				messageLength = messageLength + payloadArray.Length;
			}

			const int OverallMessageFormatLength = 3;
			var messageArray = new byte[messageLength + OverallMessageFormatLength];
			int messageOffsetIndex = 0;

			// Protocol revision number is 1 (char value).
			messageArray[messageOffsetIndex] = (byte)'1';
			messageOffsetIndex++;

			// Set the entire message length which is an unsigned short and is
			// stored in two bytes.
			messageArray[messageOffsetIndex] = (byte)((messageLength & 0xFF00) >> 8);
			messageOffsetIndex++;
			messageArray[messageOffsetIndex] = (byte)(messageLength & 0x00FF);
			messageOffsetIndex++;

			foreach(byte headerByte in headerArray)
			{
				messageArray[messageOffsetIndex] = headerByte;
				messageOffsetIndex++;
			}

			if (locationArray != null)
			{
				foreach (byte locationByte in locationArray)
				{
					messageArray[messageOffsetIndex] = locationByte;
					messageOffsetIndex++;
				}
			}

			if (payloadArray != null)
			{
				foreach(byte payloadByte in payloadArray)
				{
					messageArray[messageOffsetIndex] = payloadByte;
					messageOffsetIndex++;
				}
			}

			return messageArray;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will create the head message.
		/// </summary>
		/// <returns>Returns the header message in a byte array.</returns>
		private byte[] CreateHeaderMessage()
		{
			this.ValidateHearder();
			var headerArray = new byte[31];

			// Pad the IMEI value if it is least than 15 characters.
			char[] imeiCharArray = this.imei.ToCharArray();
			if (this.imei.Length < 15)
			{
				string padStr = string.Empty;
				int padCount = 15 - this.imei.Length;

				for (int padIndex = 0; padIndex < padCount; padIndex++)
				{
					padStr = padStr + " ";
				}

				string imeiPaddedValue = this.imei + padStr;
				imeiCharArray = imeiPaddedValue.ToCharArray();
			}

			headerArray[0] = this.HeaderIei;
			headerArray[1] = (byte) ((this.headerLength & 0xFF00) >> 8);
			headerArray[2] = (byte) (this.headerLength & 0x00FF);
			headerArray[3] = (byte)((this.cdrReference & 0xFF000000) >> 24);
			headerArray[4] = (byte)((this.cdrReference & 0x00FF0000) >> 16);
			headerArray[5] = (byte)((this.cdrReference & 0x0000FF00) >> 8);
			headerArray[6] = (byte)(this.cdrReference & 0x000000FF);
			headerArray[7] = (byte)imeiCharArray[0];
			headerArray[8] = (byte)imeiCharArray[1];
			headerArray[9] = (byte)imeiCharArray[2];
			headerArray[10] = (byte)imeiCharArray[3];
			headerArray[11] = (byte)imeiCharArray[4];
			headerArray[12] = (byte)imeiCharArray[5];
			headerArray[13] = (byte)imeiCharArray[6];
			headerArray[14] = (byte)imeiCharArray[7];
			headerArray[15] = (byte)imeiCharArray[8];
			headerArray[16] = (byte)imeiCharArray[9];
			headerArray[17] = (byte)imeiCharArray[10];
			headerArray[18] = (byte)imeiCharArray[11];
			headerArray[19] = (byte)imeiCharArray[12];
			headerArray[20] = (byte)imeiCharArray[13];
			headerArray[21] = (byte)imeiCharArray[14];
			headerArray[22] = (byte)this.sessionStatus;
			headerArray[23] = (byte)((this.momsn & 0xFF00) >> 8);
			headerArray[24] = (byte)(this.momsn & 0x00FF);
			headerArray[25] = (byte)((this.mtmsn & 0xFF00) >> 8);
			headerArray[26] = (byte)(this.mtmsn & 0x00FF);
			headerArray[27] = (byte)((this.timeOfSession & 0xFF000000) >> 24);
			headerArray[28] = (byte)((this.timeOfSession & 0x00FF0000) >> 16);
			headerArray[29] = (byte)((this.timeOfSession & 0x0000FF00) >> 8);
			headerArray[30] = (byte)(this.timeOfSession & 0x000000FF);

			return headerArray;
		}

		/// <summary>
		/// This method will validate the header information. It will throw an exception if 
		/// the validation fails.
		/// </summary>
		private void ValidateHearder()
		{
			if (this.momsn < 1 || this.momsn > 65535)
			{
				throw new Exception("MOMSN must have a value between 1 and 65535.");
			}

			if (this.momsn > 65535)
			{
				throw new Exception("MTMSN must have a value between 0 and 65535.");
			}

			if (this.cdrReference > 4294967294)
			{
				throw new Exception("CDR Reference must have a value between 0 and 4294967294.");
			}

			if (string.IsNullOrEmpty(this.imei) || this.imei.Length > 15)
			{
				throw new Exception("IMEI must have an alphanumeric value up up to 15 characters.");
			}

			if (this.timeOfSession == 0)
			{
				throw new Exception("Must have a valid Session Time.");
			}

			this.headerLength = 28;
		}

		/// <summary>
		/// This method will create the location message.
		/// </summary>
		/// <returns>Returns the location message in a byte array.</returns>
		private byte[] CreateLocationMessage()
		{
			if (this.latitudeLongitude == null)
			{
				return null;
			}

			this.ValidateLocation();
			var locationArray = new byte[14];
			this.MoLocationInfoLength = 11;

			byte formatCodeByte = (byte)((this.latitudeLongitude.FormatCode & 0x3) << 2);
			byte nsiByte		= (byte)((this.latitudeLongitude.NorthSouthIndicator & 0x1) << 1);
			byte ewiByte		= (byte)(this.latitudeLongitude.EastWestIndicator & 0x1);

			locationArray[0] = this.MoLocationIei;
			locationArray[1] = (byte)((this.MoLocationInfoLength & 0xFF00) >> 8);
			locationArray[2] = (byte)(this.MoLocationInfoLength & 0x00FF);
			locationArray[3] = (byte)((this.latitudeLongitude.Reserved & 0xF) << 4);
			locationArray[3] = (byte)(locationArray[3] | formatCodeByte);
			locationArray[3] = (byte)(locationArray[3] | nsiByte);
			locationArray[3] = (byte)(locationArray[3] | ewiByte);
			locationArray[4] = (byte)this.latitudeLongitude.Latitude;
			locationArray[5] = (byte)((this.latitudeLongitude.LatitudeThousandsMinute & 0xFF00) >> 8);
			locationArray[6] = (byte)(this.latitudeLongitude.LatitudeThousandsMinute & 0x00FF);
			locationArray[7] = (byte)this.latitudeLongitude.Longitude;
			locationArray[8] = (byte)((this.latitudeLongitude.LongitudeThousandsMinute & 0xFF00) >> 8);
			locationArray[9] = (byte)(this.latitudeLongitude.LongitudeThousandsMinute & 0x00FF);
			locationArray[10] = (byte)((this.cepRadius & 0xff000000) >> 24);
			locationArray[11] = (byte)((this.cepRadius & 0x00ff0000) >> 16);
			locationArray[12] = (byte)((this.cepRadius & 0x0000ff00) >> 8);
			locationArray[13] = (byte)(this.cepRadius & 0x000000ff);

			return locationArray;
		}

		/// <summary>
		/// Validate the location information.
		/// </summary>
		private void ValidateLocation()
		{
			if (this.cepRadius == null || this.cepRadius < 1 || this.cepRadius > 2000)
			{
				throw new Exception("CEP Radius value must be between 1 and 2000.");
			}
		}

		/// <summary>
		/// This method will create the payLoad message.
		/// </summary>
		/// <returns>Returns the payload message in a byte array.</returns>
		private byte[] CreatePayloadMessage()
		{
			if (this.moPayload == null || this.moPayload.Count == 0)
			{
				return null;
			}

			var payloadArray = new byte[this.moPayload.Count + 3];
			int payloadIndex = 0;

			payloadArray[payloadIndex++]	= this.MoPayloadIei;
			payloadArray[payloadIndex++]	= (byte)((this.moPayload.Count & 0xFF00) >> 8);
			payloadArray[payloadIndex++]	= (byte)(this.moPayload.Count & 0x00FF);

			foreach (PayloadDO payload in this.moPayload)
			{
				payloadArray[payloadIndex] = payload.RealValue;
				payloadIndex++;
			}

			return payloadArray;
		}

		/// <summary>
		/// This method will find all the sections, section types, and section
		/// lengths for the parser.
		/// </summary>
		/// <param name="messageArray">The MO message array.</param>
		/// <returns>Returns a list of section information.</returns>
		private List<SectionClass> FindSections(byte[] messageArray)
		{
			var sections = new List<SectionClass>();
			int offsetIndex = 0;

			for(int nextSection = 0; nextSection < 4; nextSection++)
			{
				char sectionType = (char) messageArray[offsetIndex];
				ushort sectionLength = (ushort) (messageArray[offsetIndex + 1] << 8);
				sectionLength = (ushort) (sectionLength | messageArray[offsetIndex + 2]);

				var sectionDo = new SectionClass
				                {
					                SectionLength = sectionLength,
					                SectionType = sectionType,
					                SectionStartingIndex = offsetIndex
				                };

				sections.Add(sectionDo);

				offsetIndex = offsetIndex + sectionLength + 3;

				// There may not be 4 sections in the message, so break out of the loop.
				if (offsetIndex > messageArray.Count())
				{
					break;
				}
			}

			return sections;
		}

		/// <summary>
		/// This method will parse the header section.
		/// </summary>
		/// <param name="headerArray">The message array with the header information.</param>
		/// <param name="startIndex">The starting byte index.</param>
		/// <param name="headerByteLength">The length of the header section.</param>
		private void ParseHeaderInfo(byte[] headerArray, int startIndex, ushort headerByteLength)
		{
			this.headerIei = headerArray[startIndex];

			// Header Length is bytes 1 & 2 (2 bytes in length).
			this.headerLength = headerByteLength;

			if (this.headerLength != 28)
			{
				throw new Exception("Invalid header length: " + this.headerLength + ". Must be 28 bytes.");
			}

			// CDR Reference is bytes 3-6 (4 bytes in length).
			this.cdrReference = (uint) (headerArray[startIndex + 3] << 24);
			this.cdrReference = this.cdrReference | (uint) (headerArray[startIndex + 4] << 16);
			this.cdrReference = this.cdrReference | (uint) (headerArray[startIndex + 5] << 8);
			this.cdrReference = this.cdrReference | headerArray[startIndex + 6];

			// International Mobile Equipment Identity (IMEI) 15 bytes in length, bytes 7-21.
			this.imei = headerArray[startIndex + 7].ToString()
						+ headerArray[startIndex + 8]
						+ headerArray[startIndex + 9]
						+ headerArray[startIndex + 10]
						+ headerArray[startIndex + 11]
						+ headerArray[startIndex + 12]
						+ headerArray[startIndex + 13]
						+ headerArray[startIndex + 14]
						+ headerArray[startIndex + 15]
						+ headerArray[startIndex + 16]
						+ headerArray[startIndex + 17]
						+ headerArray[startIndex + 18]
						+ headerArray[startIndex + 19]
						+ headerArray[startIndex + 20]
						+ headerArray[startIndex + 21];

			// Session Status 1 byte (byte 22).
			this.sessionStatus = headerArray[startIndex + 22];

			// Mobile Originated Message Sequence Number (MOMSN) 2 bytes in length (byte 23 & 24).
			this.momsn = (ushort) (headerArray[startIndex + 23] << 8);
			this.momsn = (ushort) (this.momsn | headerArray[startIndex + 24]);

			// Mobile Terminated Message Sequence Number (MTMSN) 2 bytes in length (byte 25 & 26).
			this.mtmsn = (ushort) (headerArray[startIndex + 25] << 8);
			this.mtmsn = (ushort) (this.mtmsn | headerArray[startIndex + 26]);

			// Time of session (Epoch time) 4 bytes in length (bytes 27 - 30).
			this.timeOfSession = (uint) (headerArray[startIndex + 27] << 24);
			this.timeOfSession = this.timeOfSession | (uint) (headerArray[startIndex + 28] << 16);
			this.timeOfSession = this.timeOfSession | (uint) (headerArray[startIndex + 29] << 8);
			this.timeOfSession = this.timeOfSession | headerArray[startIndex + 30];
		}

		/// <summary>
		/// This method will parse the payload section.
		/// </summary>
		/// <param name="payloadArray">The message array with the payload information.</param>
		/// <param name="startIndex">The starting byte index.</param>
		/// <param name="payloadByteLength">The length of the payload section.</param>
		private void ParsePayloadSection(byte[] payloadArray, int startIndex, ushort payloadByteLength)
		{
			this.moPayloadIei = payloadArray[startIndex];
			this.moPayloadLength = payloadByteLength;

			int payloadStartIndex = startIndex + 3;
			int paloadStopIndex = payloadStartIndex + payloadByteLength;

			for (int nextByte = payloadStartIndex; nextByte < paloadStopIndex; nextByte++)
			{
				var payloadDo = new PayloadDO
				                {
					                ByteNumber = nextByte, 
									RealValue = payloadArray[nextByte]
				                };

				this.moPayload.Add(payloadDo);
			}
		}

		/// <summary>
		/// This method will parse the location info section.
		/// </summary>
		/// <param name="locationInfoArray">The message array with the location information.</param>
		/// <param name="startIndex">The starting byte index.</param>
		/// <param name="locationInfoByteLength">The length of the location info section.</param>
		private void ParseLocationInfoSection(byte[] locationInfoArray, int startIndex, ushort locationInfoByteLength)
		{
			this.moLocationIei = locationInfoArray[startIndex];
			this.moLocationInfoLength = locationInfoByteLength;

			this.cepRadius = (uint)(locationInfoArray[startIndex + 10] << 24);
			this.cepRadius = this.cepRadius | (uint) (locationInfoArray[startIndex + 11] << 16);
			this.cepRadius = this.cepRadius | (uint) (locationInfoArray[startIndex + 12] << 8);
			this.cepRadius = this.cepRadius | locationInfoArray[startIndex + 13];

			int offsetIndex = startIndex + 3;			
			var coordinateArray = new byte[7];

			for(int nextByte = 0; nextByte < 7; nextByte++)
			{
				coordinateArray[nextByte] = locationInfoArray[offsetIndex++];
			}

			this.latitudeLongitude = new CoordinateDO();
			this.latitudeLongitude.Load(coordinateArray);
		}

		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.headerIei				= 0x01;
			this.headerLength			= 0;
			this.cdrReference			= 0;
			this.imei					= string.Empty;
			this.sessionStatus			= 0;
			this.momsn					= 0;
			this.mtmsn					= 0;
			this.timeOfSession			= 0;
			this.moPayloadIei			= 0x02;
			this.moPayloadLength		= 0;
			this.moPayload				= new List<PayloadDO>();
			this.moLocationIei			= 0x03;
			this.moLocationInfoLength	= 0;
			this.latitudeLongitude		= null;
			this.cepRadius				= 0;
		}
		#endregion
	}

	#region Sections Class
	public class SectionClass
	{
		public char SectionType { get; set; }
		public ushort SectionLength { get; set; }
		public int SectionStartingIndex { get; set; }
	}
	#endregion
}
