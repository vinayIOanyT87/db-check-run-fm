namespace FMBusinessObjects.DataObjects.Message
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

    public class ModbusStorageMsg : EdgeData
	{
		public byte ModbusMap { get; set; }
		public double Level { get; set; }
		public double Temp { get; set; }
		public double WaterLevel { get; set; }
		public double Position { get; set; }
		public double Density { get; set; }


		public override void Load(MemoryStream memoryStream)
		{
			base.Load(memoryStream);

			byte[] modbusMap = new byte[1];
			byte[] device = new byte[1];
			byte[] level = new byte[4];
			byte[] temp = new byte[4];
			byte[] waterLevel = new byte[4];
			byte[] position = new byte[4];
			byte[] density = new byte[4];

			memoryStream.Read(modbusMap, 0, 1);
			memoryStream.Read(device, 0, 1);
			memoryStream.Read(level, 0, 4);
			memoryStream.Read(temp, 0, 4);
			memoryStream.Read(waterLevel, 0, 4);
			memoryStream.Read(position, 0, 4);
			memoryStream.Read(density, 0, 4);

			this.ModbusMap = modbusMap[0];
			this.Device = device[0];
			this.Level = Convert.ToDouble(BitConverter.ToSingle(level.Reverse().ToArray(), 0));
			this.Temp = Convert.ToDouble(BitConverter.ToSingle(temp.Reverse().ToArray(), 0));
			this.WaterLevel = Convert.ToDouble(BitConverter.ToSingle(waterLevel.Reverse().ToArray(), 0));
			this.Position = Convert.ToDouble(BitConverter.ToSingle(position.Reverse().ToArray(), 0));
			this.Density = Convert.ToDouble(BitConverter.ToSingle(density.Reverse().ToArray(), 0));
		}
	}
}
