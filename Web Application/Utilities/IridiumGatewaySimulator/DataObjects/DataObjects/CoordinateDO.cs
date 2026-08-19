namespace DataObjects.DataObjects
{
	using System;
	using System.Globalization;
	using System.Xml.Serialization;

	[Serializable, XmlRoot("CoordinateDO")]
	public class CoordinateDO
	{
		#region Private data members
		/// <summary>
		/// Reserved is bits 0 - 3.
		/// </summary>
		private int reserved;

		/// <summary>
		/// Format Code is bits 4 and 5.
		/// </summary>
		private int formatCode;

		/// <summary>
		/// The north/south indicator is bit 6.
		/// North = 0 and South = 1.
		/// </summary>
		private int northSouthIndicator;

		/// <summary>
		/// The east/west indicator is bit 7.
		/// East = 0 and West = 1.
		/// </summary>
		private int eastWestIndicator;

		/// <summary>
		/// Latitude is a decimal value of 0-90 and is byte 2
		/// </summary>
		private int latitude;

		/// <summary>
		/// Latitude minutes in thousands is a decimal value of 0-59,999 and is bytes 3 and 4.
		/// </summary>
		private uint latitudeThousandsMinute;

		/// <summary>
		/// This value is in degree/decimal format.
		/// </summary>
		private double latitudeDouble;

		/// <summary>
		/// Longitude is a decimal value of 0-180 and is byte 5.
		/// </summary>
		private int longitude;

		/// <summary>
		/// Longitude minutes in thousands is a decimal value of 0-59,999 and is bytes 6 and 7.
		/// </summary>
		private uint longitudeThousandsMinute;

		/// <summary>
		/// This value is in degree/decimal format.
		/// </summary>
		private double longitudeDouble;

		/// <summary>
		/// Coordinates in bytes contains the coordinates in a byte format.
		/// </summary>
		private byte[] coordinatesInBytes;
		private const double MinuteThousands = 60000;
		#endregion

		#region Constructors
		/// <summary>
		///  This is the default constructor.
		/// </summary>
		public CoordinateDO()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public int Reserved
		{
			get { return this.reserved; }
			set { this.reserved = value; }
		}

		public int FormatCode
		{
			get { return this.formatCode; }
			set { this.formatCode = value; }
		}

		public int NorthSouthIndicator
		{
			get { return this.northSouthIndicator; }
			set
			{
				// Default to North
				this.northSouthIndicator = 0;

				if (value >= 0 && value <= 1)
				{
					this.northSouthIndicator = value;
				}
			}
		}

		public int EastWestIndicator
		{
			get { return this.eastWestIndicator; }
			set
			{
				// Default to East
				this.eastWestIndicator = 0;

				if (value >= 0 && value <= 1)
				{
					this.eastWestIndicator = value;
				}
			}
		}

		public int Latitude
		{
			get { return this.latitude; }
			set
			{
				this.latitude = 0;

				// Set the north (0) / south (1) indicator base on the sign.
				this.northSouthIndicator = value < 0 ? 1 : 0;

				if (value >= -90 && value <= 90)
				{
					this.latitude = Math.Abs(value);
				}
			}
		}

		public uint LatitudeThousandsMinute
		{
			get { return this.latitudeThousandsMinute; }
			set
			{
				this.latitudeThousandsMinute = 0;

				if (value <= 59999)
				{
					this.latitudeThousandsMinute = value;
				}
			}
		}

		public int Longitude
		{
			get
			{
				return this.longitude;
			}
			set
			{
				this.longitude = 0;

				// Set the east (0) / West (1) indicator base on the sign.
				this.eastWestIndicator = value < 0 ? 1 : 0;

				if (value >= -180 && value <= 180)
				{
					this.longitude = Math.Abs(value);
				}
			}
		}

		public uint LongitudeThousandsMinute
		{
			get
			{
				return this.longitudeThousandsMinute;
			}
			set
			{
				this.longitudeThousandsMinute = 0;

				if (value <= 59999)
				{
					this.longitudeThousandsMinute = value;
				}
			}
		}

		public double LatitudeDouble
		{
			get
			{
				// North (0) = "+" and South(1) = "-".
				if (this.northSouthIndicator == 0)
				{
					return Math.Abs(this.latitudeDouble);
				}

				return (Math.Abs(this.latitudeDouble) * -1);
			}
			set
			{
				this.latitudeDouble = value;
				string strValue = value.ToString(CultureInfo.InvariantCulture);
				string[] parts = strValue.Split('.');

				this.Latitude = int.Parse(parts[0]);
				this.LatitudeThousandsMinute = 0;

				if (parts.Length > 1)
				{
					// Convert from a degree/decimal to degree minute format.
					var decimalPartFloat = double.Parse("." + parts[1]);
					this.LatitudeThousandsMinute = (uint) (decimalPartFloat * MinuteThousands);
				}
			}
		}

		public double LongitudeDouble
		{
			get
			{
				// East(0) = "+" and West(1) = "-".
				if (this.eastWestIndicator == 0)
				{
					return Math.Abs(this.longitudeDouble);
				}

				return (Math.Abs(this.longitudeDouble) * -1);
			}
			set
			{
				this.longitudeDouble = value;
				string strValue = value.ToString(CultureInfo.InvariantCulture);
				string[] parts = strValue.Split('.');

				this.Longitude = int.Parse(parts[0]);
				this.LongitudeThousandsMinute = 0;

				if (parts.Length > 1)
				{
					// Convert from a degree/decimal to degree minute format.
					var decimalPartFloat = double.Parse("." + parts[1]);
					this.LongitudeThousandsMinute = (uint) (decimalPartFloat * MinuteThousands);
				}
			}
		}

		public byte[] CoordinatesInBytes
		{
			get
			{
				this.coordinatesInBytes = new byte[7];

				// Byte 1 
				this.coordinatesInBytes[0] = (byte)(this.reserved << 4);
				this.coordinatesInBytes[0] = (byte)(this.coordinatesInBytes[0] | ((byte)(this.formatCode << 2)));
				this.coordinatesInBytes[0] = (byte)(this.coordinatesInBytes[0] | ((byte)(this.northSouthIndicator << 1)));
				this.coordinatesInBytes[0] = (byte) (this.coordinatesInBytes[0] | this.eastWestIndicator);

				// Byte 2
				this.coordinatesInBytes[1] = (byte)this.latitude;

				// Byte 3
				uint msd = (this.LatitudeThousandsMinute & 0xff00) >> 8;
				this.coordinatesInBytes[2] = (byte)msd;

				// Byte 4
				uint lsd = this.LatitudeThousandsMinute & 0x00ff;
				this.coordinatesInBytes[3] = (byte) lsd;

				// Byte 5
				this.coordinatesInBytes[4] = (byte) this.longitude;

				// Byte 6
				msd = (this.longitudeThousandsMinute & 0xff00) >> 8;
				this.coordinatesInBytes[5] = (byte) msd;

				// Byte 7
				int mlsd = this.longitude & 0x00ff;
				this.coordinatesInBytes[6] = (byte) mlsd;

				return this.coordinatesInBytes;
			}
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will load the coordinate object based on the 
		/// byte array.
		/// </summary>
		/// <param name="coordinateBytes">Coordinates in byte array format.</param>
		public void Load(byte[] coordinateBytes)
		{
			// Reset prior to loading the coordinates.
			this.Init();

			// Byte 1
			// MSB         Bit Position        LSB
			// 0  1  2  3 |    4   5    |  6  |  7
			//  Reserved  | Format Code | NSI | EWI
			//
			// Byte 2
			// 0 1 2 3 4 5 6 7
			// Latitude
			// 
			// Byte 3
			// 0 1 2 3 4 5 6 7
			// Latitude MS Byte
			// 
			// Byte 4
			// 0 1 2 3 4 5 6 7
			// Latitude LS Byte
			//
			// Byte 5
			// 0 1 2 3 4 5 6 7
			// Longitude
			// 
			// Byte 6
			// 0 1 2 3 4 5 6 7
			// Longitude MS Byte
			// 
			// Byte 7
			// 0 1 2 3 4 5 6 7
			// Longitude LS Byte
			if (coordinateBytes != null && coordinateBytes.Length == 7)
			{
				Byte locationByte			= coordinateBytes[0];
				this.eastWestIndicator		= locationByte & 0x01;
				this.northSouthIndicator	= (locationByte >> 1) & 0x01;
				this.formatCode				= (locationByte >> 2) & 0x03;
				this.reserved				= (locationByte >> 4) & 0x0f;

				this.latitude = coordinateBytes[1] & 0xff;
				this.longitude = coordinateBytes[4] & 0xff;

				this.latitudeThousandsMinute = (uint)((coordinateBytes[2] << 8) | coordinateBytes[3]);
				this.longitudeThousandsMinute = (uint)((coordinateBytes[5] << 8) | coordinateBytes[6]);
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.reserved					= 0;
			this.formatCode					= 0;
			this.northSouthIndicator		= 0;
			this.eastWestIndicator			= 0;
			this.latitude					= 0;
			this.latitudeThousandsMinute	= 0;
			this.latitudeDouble				= 0;
			this.longitude					= 0;
			this.longitudeThousandsMinute	= 0;
			this.longitudeDouble			= 0;
			this.coordinatesInBytes			= new byte[7];
			
			for (int nextByte = 0; nextByte < 7; nextByte++)
			{
				this.coordinatesInBytes[nextByte] = 0;
			}
		}
		#endregion
	}
}
