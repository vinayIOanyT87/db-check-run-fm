namespace DataObjects.DataObjects
{
	public class RawDataDO
	{
		#region Private data members
		private string rawDataRecord;
		private string rawDataByteStr;
		private byte rawDataByte;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public RawDataDO()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public string RawDataRecord
		{
			get { return this.rawDataRecord; }
		}

		public string RawDataByteStr
		{
			get { return this.rawDataByteStr; }
		}

		public byte RawDataByte
		{
			get { return this.rawDataByte; }
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// This method will load the raw data record into the object.
		/// </summary>
		/// <param name="record">The record to load.</param>
		public void Load(string record)
		{
			if (record == null)
			{
				return;
			}

			this.rawDataRecord = record;
			int byteEndingIndex = record.IndexOf(" >>");

			this.rawDataByteStr = string.Empty;
			this.rawDataByte = 0;

			if (byteEndingIndex > 0)
			{
				this.rawDataByteStr = record.Substring(0, byteEndingIndex);
				int shift = 7;

				for (int nextBit = 0; nextBit < 8; nextBit++)
				{
					string bitStr = this.rawDataByteStr.Substring(nextBit, 1);
					byte bit = bitStr == "1" ? (byte)1 : (byte)0;
					this.rawDataByte = (byte)(this.rawDataByte | (byte)(bit << shift));
					shift--;
				}
			}
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.rawDataByteStr = string.Empty;
			this.rawDataRecord	= string.Empty;
			this.rawDataByte	= 0;
		}
		#endregion
	}
}
