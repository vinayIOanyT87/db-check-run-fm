namespace FMBusinessObjects.DataObjects.Message
{
	using System;
	using System.Collections.Generic;
	using System.IO;

	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public class SoftwareVersionMsg : EdgeData
	{
		public string SWVersion { get; set; }
		public UInt32 MinTime { get; set; }
		public UInt32 MaxTime { get; set; }
		public double LevelDeadband { get; set; }
		public double TempDeadband { get; set; }
		public UInt32 Heartbeat { get; set; }
		public byte TLSTanks { get; set; }
		public byte ModbusMap { get; set; }
		public ushort MidnightOffset { get; set; }
		public double ShortDeadband { get; set; }
		public ushort ShortTime { get; set; }
		public double LongDeadband { get; set; }
		public ushort LongTime { get; set; }
		public byte[] ScalerType { get; set; }


		public override void Load(MemoryStream memoryStream)
		{
			base.Load(memoryStream);
			byte[] swVersion = new byte[32];
			byte[] minTime = new byte[4];
			byte[] maxTime = new byte[4];
			byte[] levelDeadband = new byte[4];
			byte[] tempDeadband = new byte[4];
			byte[] heartbeat = new byte[4];
			byte[] tlsTanks = new byte[1];
			byte[] modbusMap = new byte[1];
			byte[] midnightOffset = new byte[2];
			byte[] shortDeadband = new byte[4];
			byte[] shortTime = new byte[2];
			byte[] longDeadband = new byte[4];
			byte[] longTime = new byte[2];
			byte[] scalerType = new byte[12];

			memoryStream.Read(swVersion, 0, 32);
			memoryStream.Read(minTime, 0, 4);
			memoryStream.Read(maxTime, 0, 4);
			memoryStream.Read(levelDeadband, 0, 4);
			memoryStream.Read(tempDeadband, 0, 4);
			memoryStream.Read(heartbeat, 0, 4);
			memoryStream.Read(tlsTanks, 0, 1);
			memoryStream.Read(modbusMap, 0, 1);
			memoryStream.Read(midnightOffset, 0, 2);
			memoryStream.Read(shortDeadband, 0, 4);
			memoryStream.Read(shortTime, 0, 2);
			memoryStream.Read(longDeadband, 0, 4);
			memoryStream.Read(longTime, 0, 2);
			memoryStream.Read(scalerType, 0, 12);

			this.SWVersion = new string(swVersion.Select(c => (char)c).ToArray()).Trim();
			this.MinTime = BitConverter.ToUInt32(minTime.Reverse().ToArray(), 0);
			this.MaxTime = BitConverter.ToUInt32(maxTime.Reverse().ToArray(), 0);
			this.LevelDeadband = Convert.ToDouble(BitConverter.ToSingle(levelDeadband.Reverse().ToArray(), 0));
			this.TempDeadband = Convert.ToDouble(BitConverter.ToSingle(tempDeadband.Reverse().ToArray(), 0));
			this.Heartbeat = BitConverter.ToUInt32(heartbeat.Reverse().ToArray(), 0);
			this.TLSTanks = tlsTanks[0];
			this.ModbusMap = modbusMap[0];
			this.MidnightOffset = BitConverter.ToUInt16(midnightOffset.Reverse().ToArray(), 0);
			this.ShortDeadband = Convert.ToDouble(BitConverter.ToSingle(shortDeadband.Reverse().ToArray(), 0));
			this.ShortTime = BitConverter.ToUInt16(shortTime.Reverse().ToArray(), 0);
			this.LongDeadband = Convert.ToDouble(BitConverter.ToSingle(longDeadband.Reverse().ToArray(), 0));
			this.LongTime = BitConverter.ToUInt16(longTime.Reverse().ToArray(), 0);
			this.ScalerType = scalerType.ToArray();
		}
	}
}