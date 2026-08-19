namespace FMBusinessObjects.Parsers
{
	using System;
	using System.Collections.Generic;

	public class IridiumTduPayloadParser : IridiumPayloadParserBase
	{
		#region Private data members
		private List<TduData> tduTankList;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public IridiumTduPayloadParser()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public bool HasTduData { get; set; }
		public List<TduData> TduTankList => this.tduTankList;
		#endregion

		#region Public methods

		/// <summary>
		/// This method will parse the payload section.
		/// </summary>
		/// <param name="payloadArray">The payload array.</param>
		public void Parse(byte[] payloadArray)
		{
			if (payloadArray == null || payloadArray.Length == 0)
			{
				return;
			}

			// 0x01 = TDU, therefore if not TDU, then return.
			if (payloadArray[0] != 0x01)
			{
				return;
			}

			if (payloadArray.Length < 16)
			{
				throw new Exception("Byte array must be a minimum length of 16 bytes.");
			}

			int numberOfTanks = this.HowManyTanks(payloadArray);

			if (numberOfTanks == 0)
			{
				throw new Exception("Invalid number of Tanks.");
			}

			this.tduTankList = new List<TduData>();
			int offset = 1;

			for (int nextTank = 0; nextTank < numberOfTanks; nextTank++)
			{
				this.ParseForTanks(offset, payloadArray);
				offset = offset + 13;
			}

			this.ParseChecksum(payloadArray, payloadArray.Length);
			this.ChecksumValid = this.CompareChecksum(payloadArray, payloadArray.Length);
			this.HasTduData = true;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will parse the TDU data and add it to the list.
		/// </summary>
		/// <param name="offset">The offset for the next tank.</param>
		/// <param name="payloadArray">The payload array.</param>
		private void ParseForTanks(int offset, byte[] payloadArray)
		{
			var tduTankData = new TduData
			                  {
				                  TankConfigurationNumber	= payloadArray[offset],
				                  Pressure					= this.IeeeBitToFloatConverter(payloadArray, offset + 1),
				                  Temperature				= this.IeeeBitToFloatConverter(payloadArray, offset + 5),
				                  Volume					= this.IeeeBitToFloatConverter(payloadArray, offset + 9)
			                  };

			this.tduTankList.Add(tduTankData);
		}

		/// <summary>
		/// This method will determine how many tanks are in the
		/// payload.  It is assume a max of 4 tanks.
		/// </summary>
		/// <param name="payloadArray">The payload array.</param>
		/// <returns>Returns the number of tanks in the payload message.</returns>
		private int HowManyTanks(byte[] payloadArray)
		{
			int[] lengthMatch = { 16, 29, 42, 55 };

			for (int nextMatch = 0; nextMatch < 4; nextMatch++)
			{
				if (payloadArray.Length == lengthMatch[nextMatch])
				{
					return nextMatch + 1;
				}
			}

			return 0;
		}

		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.HasTduData		= false;
			this.tduTankList	= null;

			base.BaseInit();
		}
		#endregion
	}

	#region TDU data class
	public class TduData
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor
		/// </summary>
		public TduData()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public int TankConfigurationNumber { get; set; }
		public double Pressure { get; set; }
		public double Temperature { get; set; }
		public double Volume { get; set; }
		#endregion

		#region Private methods
		/// <summary>
		/// This method initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.TankConfigurationNumber = 0;
			this.Pressure				 = 0.0;
			this.Temperature			 = 0.0;
			this.Volume					 = 0.0;
		}
		#endregion
	}
	#endregion
}
