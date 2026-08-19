namespace FMBusinessObjects.Parsers
{
	using System;

	public class IridiumPositionPayloadParser : IridiumPayloadParserBase
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public IridiumPositionPayloadParser()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public bool HasCoordinateData { get; set; }
		#endregion

		#region Public Methods
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

			// 0x00 = Coordinate only payload message, therefore if not, then return.
			if (payloadArray[0] != 0x00)
			{
				return;
			}

			// Just the coordinates in the payload section along with the checksum.
			if (payloadArray.Length < 10)
			{
				throw new Exception("Byte array must be a minimum length of 10 bytes.");
			}

			// Parse the coordinate information
			const int Offset = 1;
			this.ParseCoordinates(Offset, payloadArray);
			
			this.ParseChecksum(payloadArray, payloadArray.Length);
			this.ChecksumValid = this.CompareChecksum(payloadArray, payloadArray.Length);

			this.HasCoordinateData = true;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.HasCoordinateData = false;
			base.BaseInit();
		}
		#endregion

	}
}
