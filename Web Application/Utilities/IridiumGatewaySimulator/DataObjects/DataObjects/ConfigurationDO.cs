namespace DataObjects.DataObjects
{
	using System;
	using System.Xml.Serialization;

	[Serializable, XmlRoot("ConfigurationDO")]
	public class ConfigurationDO
	{
		#region Private data members
		private int? fmIpAddress1;
		private int? fmIpAddress2;
		private int? fmIpAddress3;
		private int? fmIpAddress4;

		private int? iridiumIpAddress1;
		private int? iridiumIpAddress2;
		private int? iridiumIpAddress3;
		private int? iridiumIpAddress4;

		private int? fmPortNumber;
		private int? iridiumPortNumber;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public ConfigurationDO()
		{
			this.Init();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will get and set the FuelsManager Listener IP Address 1.
		/// </summary>
		public string FmIpAddress1
		{
			get
			{
				return this.fmIpAddress1 == null ? string.Empty : this.fmIpAddress1.ToString();
			}
			set
			{
				this.fmIpAddress1 = this.ConvertToInteger(value);
			}
		}

		/// <summary>
		/// This property will get and set the FuelsManager Listener IP Address 2.
		/// </summary>
		public string FmIpAddress2
		{
			get
			{
				return this.fmIpAddress2 == null ? string.Empty : this.fmIpAddress2.ToString();
			}
			set
			{
				this.fmIpAddress2 = this.ConvertToInteger(value);
			}
		}

		/// <summary>
		/// This property will get and set the FuelsManager Listener IP Address 3.
		/// </summary>
		public string FmIpAddress3
		{
			get
			{
				return this.fmIpAddress3 == null ? string.Empty : this.fmIpAddress3.ToString();
			}
			set
			{
				this.fmIpAddress3 = this.ConvertToInteger(value);
			}
		}

		/// <summary>
		/// This property will get and set the FuelsManager Listener IP Address 4.
		/// </summary>
		public string FmIpAddress4
		{
			get
			{
				return this.fmIpAddress4 == null ? string.Empty : this.fmIpAddress4.ToString();
			}
			set
			{
				this.fmIpAddress4 = this.ConvertToInteger(value);
			}
		}

		/// <summary>
		/// Gets the entire FM IP address.
		/// </summary>
		public string FmIpAddress
		{
			get
			{
				return (this.fmIpAddress1 + "." + this.fmIpAddress2 + "." + this.fmIpAddress3 + "." + this.fmIpAddress4);
			}
		}

		/// <summary>
		/// This property will get and set the FuelsManager Listener port number.
		/// </summary>
		public string FmPortNumberStr
		{
			get
			{
				return this.fmPortNumber == null ? string.Empty : this.fmPortNumber.ToString();
			}
			set
			{
				this.fmPortNumber = this.ConvertToInteger(value);
			}
		}

		/// <summary>
		/// Gets the FM Port Number as an integer.
		/// </summary>
		public int? FmPortNumber
		{
			get { return this.fmPortNumber; }
		}

		/// <summary>
		/// This property will get and set the Iridium Listener IP Address 1.
		/// </summary>
		public string IridiumIpAddress1
		{
			get
			{
				return this.iridiumIpAddress1 == null ? string.Empty : this.iridiumIpAddress1.ToString();
			}
			set
			{
				this.iridiumIpAddress1 = this.ConvertToInteger(value);
			}
		}

		/// <summary>
		/// This property will get and set the Iridium Listener IP Address 2.
		/// </summary>
		public string IridiumIpAddress2
		{
			get
			{
				return this.iridiumIpAddress2 == null ? string.Empty : this.iridiumIpAddress2.ToString();
			}
			set
			{
				this.iridiumIpAddress2 = this.ConvertToInteger(value);
			}
		}

		/// <summary>
		/// This property will get and set the Iridium Listener IP Address 3.
		/// </summary>
		public string IridiumIpAddress3
		{
			get
			{
				return this.iridiumIpAddress3 == null ? string.Empty : this.iridiumIpAddress3.ToString();
			}
			set
			{
				this.iridiumIpAddress3 = this.ConvertToInteger(value);
			}
		}

		/// <summary>
		/// This property will get and set the Iridium Listener IP Address 4.
		/// </summary>
		public string IridiumIpAddress4
		{
			get
			{
				return this.iridiumIpAddress4 == null ? string.Empty : this.iridiumIpAddress4.ToString();
			}
			set
			{
				this.iridiumIpAddress4 = this.ConvertToInteger(value);
			}
		}

		/// <summary>
		/// Gets the entire Iridium IP address.
		/// </summary>
		public string IridiumIpAddress
		{
			get
			{
				return (this.iridiumIpAddress1 + "." + this.iridiumIpAddress2 + "." + this.iridiumIpAddress3 + "." + this.iridiumIpAddress4);
			}
		}

		/// <summary>
		/// This property will get and set the Iridium Listener port number.
		/// </summary>
		public string IridiumPortNumberStr
		{
			get
			{
				return this.iridiumPortNumber == null ? string.Empty : this.iridiumPortNumber.ToString();
			}
			set
			{
				this.iridiumPortNumber = this.ConvertToInteger(value);
			}
		}

		/// <summary>
		/// Gets the Iridium Port Number as an integer.
		/// </summary>
		public int? IridiumPortNumber
		{
			get { return this.iridiumPortNumber; }
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will convert a string into an integer.
		/// Returns null if cannot convert.
		/// </summary>
		/// <param name="inValue">Integer string to convert.</param>
		/// <returns>Return the integer value or null.</returns>
		private int? ConvertToInteger(string inValue)
		{
			if (string.IsNullOrEmpty(inValue))
			{
				return null;
			}

			int ipAddress;

			if (int.TryParse(inValue, out ipAddress))
			{
				return ipAddress;
			}

			return null;
		}

		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.fmIpAddress1 = null;
			this.fmIpAddress2 = null;
			this.fmIpAddress3 = null;
			this.fmIpAddress4 = null;
			this.fmPortNumber = null;

			this.iridiumIpAddress1 = null;
			this.iridiumIpAddress2 = null;
			this.iridiumIpAddress3 = null;
			this.iridiumIpAddress4 = null;
			this.iridiumPortNumber = null;
		}
		#endregion
	}
}
