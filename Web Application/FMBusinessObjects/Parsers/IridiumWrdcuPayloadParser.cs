namespace FMBusinessObjects.Parsers
{
	using System;
	using System.Collections.Generic;

	public class IridiumWrdcuPayloadParser : IridiumPayloadParserBase
	{
		#region Private data members
		private List<WrdcuData> wrdcuTankList;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public IridiumWrdcuPayloadParser()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public bool HasWrdcuData { get; set; }
		public List<WrdcuData> WrdcuTankList => this.wrdcuTankList;
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

			// 0x02 = WRDCU, therefore if not WRDCU, then return.
			if (payloadArray[0] != 0x02)
			{
				return;
			}

			if (payloadArray.Length < 23)
			{
				throw new Exception("Byte array must be a minimum length of 23 bytes for 1 tank.");
			}

			int numberOfTanks = this.HowManyTanks(payloadArray);

			if (numberOfTanks == 0)
			{
				throw new Exception("Invalid number of Tanks.");
			}

			// Parse the coordinate information
			int offset = 1;
			this.ParseCoordinates(offset, payloadArray);

			// Parse the tank data.
			this.wrdcuTankList = new List<WrdcuData>();
			offset = offset + 7;
			for (int nextTank = 0; nextTank < numberOfTanks; nextTank++)
			{
				this.ParseForTanks(offset, payloadArray);
				offset = offset + 13;
			}

			this.ParseChecksum(payloadArray, payloadArray.Length);
			this.ChecksumValid = this.CompareChecksum(payloadArray, payloadArray.Length);

			this.HasWrdcuData = true;
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
			var wrdcuTankData = new WrdcuData
								{
									TankConfigurationNumber = payloadArray[offset],
									Level					= this.IeeeBitToFloatConverter(payloadArray, offset + 1),
									Volume					= this.IeeeBitToFloatConverter(payloadArray, offset + 5),
									Dielectric				= this.IeeeBitToFloatConverter(payloadArray, offset + 9)
								};

			this.wrdcuTankList.Add(wrdcuTankData);
		}

		/// <summary>
		/// This method will determine how many tanks are in the
		/// payload.  It is assume a max of 4 tanks.
		/// </summary>
		/// <param name="payloadArray">The payload array.</param>
		/// <returns>Returns the number of tanks in the payload message.</returns>
		private int HowManyTanks(byte[] payloadArray)
		{
			// Byte		Contents
			// -----	------------
			// 1		Prefix
			// 2:8		GPS Data
			// 9		Tank 1 ID
			// 10:21	Tank 1 Data
			// 22		Tank 2 ID
			// 23:34	Tank 2 Data
			// 35		Tank 3 ID
			// 36:47	Tank 3 Data
			// 48		Tank 4 ID
			// 49:60	Tank 4 Data
			// 61:62	Checksum
			int[] lengthMatch = { 23, 36, 49, 62 };

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
			this.HasWrdcuData	= false;
			this.wrdcuTankList	= null;

			base.BaseInit();
		}
		#endregion
	}

	#region WRDCU data class
	public class WrdcuData
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor
		/// </summary>
		public WrdcuData()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public int TankConfigurationNumber { get; set; }
		public double Level { get; set; }
		public double Volume { get; set; }
		public double Dielectric { get; set; }
		#endregion

		#region Private methods
		/// <summary>
		/// This method initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.TankConfigurationNumber	= 0;
			this.Level						= 0.0;
			this.Volume						= 0.0;
			this.Dielectric					= 0.0;
		}
		#endregion
	}
	#endregion
}
