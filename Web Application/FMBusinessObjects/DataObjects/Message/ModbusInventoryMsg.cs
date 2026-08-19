namespace FMBusinessObjects.DataObjects.Message
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

    public class ModbusInventoryMsg : EdgeData
	{
		public byte ModbusMap { get; set; }
		public ushort Level { get; set; }
		public ushort Temp { get; set; }
		public ushort WaterLevel { get; set; }
		public ushort Position { get; set; }
		public ushort GaugeStatus { get; set; }
		public ushort WaterSump { get; set; }
		public ushort FuelVolume { get; set; }
		public ushort WaterVolume { get; set; }


		public override void Load(MemoryStream memoryStream)
		{
			base.Load(memoryStream);

			byte[] modbusMap = new byte[1];
			byte[] device = new byte[1];
			byte[] level = new byte[2];
			byte[] temp = new byte[2];
			byte[] waterLevel = new byte[2];
			byte[] position = new byte[2];
			byte[] gaugeStatus = new byte[2];
			byte[] waterSump = new byte[2];
			byte[] fuelVolume = new byte[2];
			byte[] waterVolume = new byte[2];

			memoryStream.Read(modbusMap, 0, 1);
			memoryStream.Read(device, 0, 1);
			memoryStream.Read(level, 0, 2);
			memoryStream.Read(temp, 0, 2);
			memoryStream.Read(waterLevel, 0, 2);
			memoryStream.Read(position, 0, 2);
			memoryStream.Read(gaugeStatus, 0, 2);
			memoryStream.Read(waterSump, 0, 2);
			memoryStream.Read(fuelVolume, 0, 2);
			memoryStream.Read(waterVolume, 0, 2);

			this.ModbusMap = modbusMap[0];
			this.Device = device[0];
			this.Level = BitConverter.ToUInt16(level.Reverse().ToArray(), 0);
			this.Temp = BitConverter.ToUInt16(temp.Reverse().ToArray(), 0);
			this.WaterLevel = BitConverter.ToUInt16(waterLevel.Reverse().ToArray(), 0);
			this.Position = BitConverter.ToUInt16(position.Reverse().ToArray(), 0);
			this.GaugeStatus = BitConverter.ToUInt16(gaugeStatus.Reverse().ToArray(), 0);
			this.WaterSump = BitConverter.ToUInt16(waterSump.Reverse().ToArray(), 0);
			this.FuelVolume = BitConverter.ToUInt16(fuelVolume.Reverse().ToArray(), 0);
			this.WaterVolume = BitConverter.ToUInt16(waterVolume.Reverse().ToArray(), 0);
		}
	}
}
