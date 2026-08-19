namespace FMBusinessObjects.Parsers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using FMBusinessObjects.DataObjects;

	public class IridiumMessageParser
	{
		#region Private Data Members
		private List<AssetTrackingPayloadClass> assetTrackingPayloadCollection;
		private uint cdrReference;
		private string imei;
		private uint sessionStatus;
		private ushort momsn;
		private ushort mtmsn;
		private DateTime? sessionDateTime;
		private uint cepRadius;
		private double? latitude;
		private double? longitude;
		private int formatCode;
		private int reserved;
		private char? protocolVersion;
		private int totalMessageLength;
		private int northSouthIndicator;
		private int eastWestIndicator;
		private IridiumTduPayloadParser tduPayloadParser;
		private IridiumWrdcuPayloadParser wrdcuPayloadParser;
		private IridiumPositionPayloadParser positionPayloadParser;
		private bool checksumFlag;
		private AssetTrackingDetailClass.PayloadTypes payloadType;
		#endregion

		#region Constructors
		/// <summary>
		/// This method is the default constructor
		/// </summary>
		public IridiumMessageParser()
		{	
			this.Init();
		}
		#endregion

		#region Properties
		public List<AssetTrackingPayloadClass> AssetTrackingPayloadCollection => this.assetTrackingPayloadCollection;
		public IridiumTduPayloadParser TduPayloadParser => this.tduPayloadParser;
		public IridiumWrdcuPayloadParser WrdcuPayloadParser => this.wrdcuPayloadParser;
		public IridiumPositionPayloadParser PositionPayloadParser => this.positionPayloadParser;
		public uint CdrReference => this.cdrReference;
		public string Imei => this.imei;
		public uint SessionStatus => this.sessionStatus;
		public ushort Momsn => this.momsn;
		public ushort Mtmsn => this.mtmsn;
		public DateTime? SessionDateTime => this.sessionDateTime;
		public uint CepRadius => this.cepRadius;
		public double? Latitude => this.latitude;
		public double? Longitude => this.longitude;
		public int FormatCode => this.formatCode;
		public int Reserved => this.reserved;
		public char? ProtocolVersion => this.protocolVersion;
		public int TotalMessageLength => this.totalMessageLength;
		public int NorthSouthIndicator => this.northSouthIndicator;
		public int EastWestIndicator => this.eastWestIndicator;
		public bool ChecksumFlag => this.checksumFlag;
		public AssetTrackingDetailClass.PayloadTypes PayloadType => this.payloadType;
		#endregion

		#region Public methods
		/// <summary>
		/// This methods will parse the MO message byte array and
		/// load the object.
		/// </summary>
		/// <param name="messageArray">The byte array containing the MO message.</param>
		public void Parse(byte[] messageArray)
		{
			if (messageArray == null || messageArray.Count() < 32)
			{
				throw new Exception("Byte array must be at least 31 bytes in length.");
			}

			List<SectionClass> sections = this.FindSections(messageArray);

			foreach (SectionClass section in sections)
			{
				// Header section.
				if (section.SectionType == 0x01)
				{
					this.ParseHeaderInfo(messageArray, section.SectionStartingIndex, section.SectionLength);
				}

				// Payload section.
				if (section.SectionType == 0x02)
				{
					this.ParsePayloadSection(messageArray, section.SectionStartingIndex, section.SectionLength);
				}

				// Location Info section.
				if (section.SectionType == 0x03)
				{
					this.ParseLocationInfoSection(messageArray, section.SectionStartingIndex, section.SectionLength);
				}
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will find all the sections, section types, and section
		/// lengths for the parser.
		/// </summary>
		/// <param name="messageArray">The MO message array.</param>
		/// <returns>Returns a list of section information.</returns>
		private List<SectionClass> FindSections(byte[] messageArray)
		{
			// The first three bytes are the protocol version and the entire message length.
			this.protocolVersion = (char)messageArray[0];
			this.totalMessageLength = (messageArray[1] << 8) | messageArray[2];  

			var sections = new List<SectionClass>();
			int offsetIndex = 3;

			for (int nextSection = 0; nextSection < 3; nextSection++)
			{
				byte sectionType		= messageArray[offsetIndex];
				ushort sectionLength	= (ushort)(messageArray[offsetIndex + 1] << 8);
				sectionLength			= (ushort)(sectionLength | messageArray[offsetIndex + 2]);

				var sectionDo = new SectionClass
				{
					SectionLength			= sectionLength,
					SectionType				= sectionType,
					SectionStartingIndex	= offsetIndex
				};

				sections.Add(sectionDo);

				offsetIndex = offsetIndex + sectionLength + 3;

				// There may not be 3 sections in the message, so break out of the loop.
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
			if (headerByteLength != 28)
			{
				throw new Exception("Invalid header length: " + headerByteLength + ". Must be 28 bytes.");
			}

			int offsetIndex = startIndex + 3;

			// CDR Reference is bytes 3-6 (4 bytes in length).
			this.cdrReference = (uint)(headerArray[offsetIndex++] << 24);
			this.cdrReference = this.cdrReference | (uint)(headerArray[offsetIndex++] << 16);
			this.cdrReference = this.cdrReference | (uint)(headerArray[offsetIndex++] << 8);
			this.cdrReference = this.cdrReference | headerArray[offsetIndex++];

			// International Mobile Equipment Identity (IMEI) 15 bytes in length, bytes 7-21.
			var imeiChars = new char[15];

			for (int nextChar = 0; nextChar < 15; nextChar++)
			{
				imeiChars[nextChar] = (char)headerArray[offsetIndex];
				offsetIndex++;
			}

			this.imei = string.Join("", imeiChars).Trim();

			// Session Status 1 byte (byte 22).
			this.sessionStatus = headerArray[offsetIndex++];

			// Mobile Originated Message Sequence Number (MOMSN) 2 bytes in length (byte 23 & 24).
			this.momsn = (ushort)(headerArray[offsetIndex++] << 8);
			this.momsn = (ushort)(this.momsn | headerArray[offsetIndex++]);

			// Mobile Terminated Message Sequence Number (MTMSN) 2 bytes in length (byte 25 & 26).
			this.mtmsn = (ushort)(headerArray[offsetIndex++] << 8);
			this.mtmsn = (ushort)(this.mtmsn | headerArray[offsetIndex++]);

			// Time of session (Epoch time) 4 bytes in length (bytes 27 - 30).
			var timeOfSession = (uint)(headerArray[offsetIndex++] << 24);
			timeOfSession = timeOfSession | (uint)(headerArray[offsetIndex++] << 16);
			timeOfSession = timeOfSession | (uint)(headerArray[offsetIndex++] << 8);
			timeOfSession = timeOfSession | headerArray[offsetIndex];
			
			// Convert from Epoch time to DateTime.
			var epochDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			this.sessionDateTime = epochDateTime.AddSeconds(timeOfSession);
		}

		/// <summary>
		/// This method will parse the payload section.
		/// </summary>
		/// <param name="payloadArray">The message array with the payload information.</param>
		/// <param name="startIndex">The starting byte index.</param>
		/// <param name="payloadByteLength">The length of the payload section.</param>
		private void ParsePayloadSection(byte[] payloadArray, int startIndex, ushort payloadByteLength)
		{
			this.tduPayloadParser		= new IridiumTduPayloadParser();
			this.wrdcuPayloadParser		= new IridiumWrdcuPayloadParser();
			this.positionPayloadParser	= new IridiumPositionPayloadParser();

			int byteCount = 0;
			int payloadStartIndex = startIndex + 3;
			int payloadStopIndex = payloadStartIndex + payloadByteLength;

			this.assetTrackingPayloadCollection = new List<AssetTrackingPayloadClass>();
			var payloadOnly = new List<byte>();

			for (int nextByte = payloadStartIndex; nextByte < payloadStopIndex; nextByte++)
			{
				payloadOnly.Add(payloadArray[nextByte]);

				var assetTrackingPayload = new AssetTrackingPayloadClass
				                           {
					                           ByteNumber = byteCount,
					                           PayloadValue = payloadArray[nextByte]
				                           };

				this.assetTrackingPayloadCollection.Add(assetTrackingPayload);
				byteCount++;
			}

			// Parse the payload for TDU, WRDCU, or position only data.
			this.tduPayloadParser.Parse(payloadOnly.ToArray());
			this.wrdcuPayloadParser.Parse(payloadOnly.ToArray());
			this.positionPayloadParser.Parse(payloadOnly.ToArray());

			// Set the type of payload that was parsed and whether the 
			// data was valid based on the checksum comparison.
			this.payloadType = AssetTrackingDetailClass.PayloadTypes.None;
			this.checksumFlag = true;

			if (this.tduPayloadParser.HasTduData)
			{
				this.payloadType = AssetTrackingDetailClass.PayloadTypes.Tdu;
				this.checksumFlag = this.tduPayloadParser.ChecksumValid;
			}

			if (this.wrdcuPayloadParser.HasWrdcuData)
			{
				this.payloadType = AssetTrackingDetailClass.PayloadTypes.Wrdcu;
				this.checksumFlag = this.wrdcuPayloadParser.ChecksumValid;
			}

			if (this.positionPayloadParser.HasCoordinateData)
			{
				this.payloadType = AssetTrackingDetailClass.PayloadTypes.None;
				this.checksumFlag = this.wrdcuPayloadParser.ChecksumValid;
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
			const double MinuteThousands = 60000;

			if (locationInfoByteLength != 11)
			{
				throw new Exception("Invalid location length: " + locationInfoByteLength + ". Must be 11 bytes.");
			}

			int offsetIndex = startIndex + 3;

			byte locationByte = locationInfoArray[offsetIndex];
			this.reserved = (locationByte & 0xF0) >> 4;
			this.formatCode = locationByte & 0x0C >> 2;
			this.eastWestIndicator = locationByte & 0x01;
			this.northSouthIndicator = (locationByte >> 1) & 0x01;

			int latitudeInt = locationInfoArray[offsetIndex + 1] & 0xFF;
			uint latitudeThousandsMinute = (uint)((locationInfoArray[offsetIndex + 2] << 8) | locationInfoArray[offsetIndex + 3]);

			int longitudeInt = locationInfoArray[offsetIndex + 4] & 0xFF;
			uint longitudeThousandsMinute = (uint)((locationInfoArray[offsetIndex + 5] << 8) | locationInfoArray[offsetIndex + 6]);

			double decimalRepresentation = latitudeThousandsMinute / MinuteThousands;
			this.latitude = latitudeInt + decimalRepresentation;
			this.latitude = this.northSouthIndicator == 0 ? this.latitude : this.latitude * -1;

			decimalRepresentation = longitudeThousandsMinute / MinuteThousands;
			this.longitude = longitudeInt + decimalRepresentation;
			this.longitude = this.eastWestIndicator == 0 ? this.longitude : this.longitude * -1;

			this.cepRadius = (uint)(locationInfoArray[offsetIndex + 7] << 24);
			this.cepRadius = this.cepRadius | (uint)(locationInfoArray[offsetIndex + 8] << 16);
			this.cepRadius = this.cepRadius | (uint)(locationInfoArray[offsetIndex + 9] << 8);
			this.cepRadius = this.cepRadius | locationInfoArray[offsetIndex + 10];
		}

		/// <summary>
		/// This method will initial the object to its initial state.
		/// </summary>
		private void Init()
		{	
			this.assetTrackingPayloadCollection = null;
			this.cdrReference					= 0;
			this.imei							= string.Empty;
			this.sessionStatus					= 99;
			this.momsn							= 0;
			this.mtmsn							= 0;
			this.sessionDateTime				= null;
			this.cepRadius						= 0;
			this.latitude						= null;
			this.longitude						= null;
			this.totalMessageLength				= 0;
			this.protocolVersion				= null;
			this.formatCode						= 0;
			this.reserved						= 0;
			this.northSouthIndicator			= 0;
			this.eastWestIndicator				= 0;
			this.tduPayloadParser				= new IridiumTduPayloadParser();
			this.wrdcuPayloadParser				= new IridiumWrdcuPayloadParser();
			this.checksumFlag					= false;
			this.payloadType					= AssetTrackingDetailClass.PayloadTypes.None;
		}
		#endregion
	}

	#region Sections Class
	/// <summary>
	/// This class contains the section type, length and starting index of the
	/// gateway message.
	/// </summary>
	public class SectionClass
	{
		public byte SectionType { get; set; }
		public ushort SectionLength { get; set; }
		public int SectionStartingIndex { get; set; }
	}
	#endregion
}
