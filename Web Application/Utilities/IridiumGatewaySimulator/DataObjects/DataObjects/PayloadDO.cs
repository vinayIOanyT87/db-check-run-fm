namespace DataObjects.DataObjects
{
	using System;
	using System.Text;
	using System.Xml.Serialization;

	[Serializable, XmlRoot("PayloadDO")]
	public class PayloadDO
	{
		#region Private data members
		private string byteHexValue;
		private byte realValue;
		#endregion

		#region Properties
		public int ByteNumber { get; set; }

		public string ByteHexValue
		{
			get { return this.byteHexValue; }
			set
			{
				this.realValue = 0;

				if (string.IsNullOrEmpty(value) == false)
				{
					if (TestHexString(value) == false)
					{
						throw new Exception("Must be in format 0x00 - 0xFF");
					}

					this.byteHexValue = value;
					this.realValue = ConvertHexStringToByte(value);
				}
			}
		}

		public byte RealValue
		{
			get { return this.realValue; }
			set
			{
				this.realValue = value;
				this.byteHexValue = ConvertByteToHexString(value);
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public PayloadDO()
		{
			this.Init();
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will test the string to see if it is a valid byte.
		/// </summary>
		/// <param name="inHexString">The string representing the byte.</param>
		/// <returns>Returns True if it is a byte or the in parameter is null. Otherwise, false is returned.</returns>
		public static bool TestHexString(string inHexString)
		{
			if (string.IsNullOrEmpty(inHexString))
			{
				return true;
			}

			try
			{
				ConvertHexStringToByte(inHexString);
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// This method will convert a byte into a hex string.
		/// </summary>
		/// <param name="inByte">The byte to convert to hex string.</param>
		/// <returns>Returns a hex string.</returns>
		public static string ConvertByteToHexString(byte inByte)
		{
			StringBuilder hex = new StringBuilder(2);
			hex.AppendFormat("{0:x2}", inByte);
			return "0x" + hex.ToString().ToUpper();
		}

		/// <summary>
		/// This method will convert a hex string into a byte.
		/// </summary>
		/// <param name="hex">Hex string to convert to byte</param>
		/// <returns>Returns a byte.</returns>
		public static byte ConvertHexStringToByte(string hex)
		{
			if (hex.Length < 4 || hex.Length > 4)
			{
				throw new Exception("Hex value must be '0x00' format.");
			}

			string prefix = hex.Substring(0, 2);

			if (prefix != "0x")
			{
				throw new Exception("Hex value must be '0x00' format.");
			}

			string hexPart = hex.Substring(2, 2);
			byte convertedByte = Convert.ToByte(hexPart, 16);

			return convertedByte;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.ByteNumber = 0;
			this.ByteHexValue = string.Empty;
			this.RealValue = 0;
		}
		#endregion
	}
}
