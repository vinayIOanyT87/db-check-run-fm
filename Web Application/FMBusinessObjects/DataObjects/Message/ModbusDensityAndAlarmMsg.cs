namespace FMBusinessObjects.DataObjects.Message
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

    public class ModbusDensityAndAlarmMsg : EdgeData
	{
		public byte ModbusMap { get; set; }
		public ushort Density { get; set; }
		public ushort DensityTemp { get; set; }
		public DateTimeOffset DensityTime { get; set; }
		public ushort TroubleInfo { get; set; }
		public ushort LevelAlarm { get; set; }


		public override void Load(MemoryStream memoryStream)
		{
			base.Load(memoryStream);

			byte[] modbusMap = new byte[1];
			byte[] device = new byte[1];
			byte[] density = new byte[2];
			byte[] densityTemp = new byte[2];
			byte[] densityTime = new byte[4];
			byte[] troubleInfo = new byte[2];
			byte[] levelAlarm = new byte[2];

			memoryStream.Read(modbusMap, 0, 1);
			memoryStream.Read(device, 0, 1);
			memoryStream.Read(density, 0, 2);
			memoryStream.Read(densityTemp, 0, 2);
			memoryStream.Read(densityTime, 0, 4);
			memoryStream.Read(troubleInfo, 0, 2);
			memoryStream.Read(levelAlarm, 0, 2);

			this.ModbusMap = modbusMap[0];
			this.Device = device[0];
			this.Density = BitConverter.ToUInt16(density.Reverse().ToArray(), 0);
			this.DensityTemp = BitConverter.ToUInt16(densityTemp.Reverse().ToArray(), 0);
			this.DensityTime = new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(BitConverter.ToInt32(densityTime.Reverse().ToArray(), 0)));
			this.TroubleInfo = BitConverter.ToUInt16(troubleInfo.Reverse().ToArray(), 0);
			this.LevelAlarm = BitConverter.ToUInt16(levelAlarm.Reverse().ToArray(), 0);
		}
	}
}
