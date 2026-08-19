namespace FMBusinessObjects.DataObjects.Message
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public class Enraf854TankGaugeMsg : EdgeData
	{
		public double Level { get; set; }
		public double Temp { get; set; }
		public double WaterLevel { get; set; }
		public double Position { get; set; }
		public ushort GaugeStatus { get; set; }
		public ushort PntStatus { get; set; }

		public override void Load(MemoryStream memoryStream)
		{
			base.Load(memoryStream);

			byte[] level = new byte[4];
			byte[] temp = new byte[4];
			byte[] waterLevel = new byte[4];
			byte[] position = new byte[4];
			byte[] gaugeStatus = new byte[2];
			byte[] pntStatus = new byte[2];

			memoryStream.Read(level, 0, 4);
			memoryStream.Read(temp, 0, 4);
			memoryStream.Read(waterLevel, 0, 4);
			memoryStream.Read(position, 0, 4);
			memoryStream.Read(gaugeStatus, 0, 2);
			memoryStream.Read(pntStatus, 0, 2);

			this.Level = Convert.ToDouble(BitConverter.ToSingle(level.Reverse().ToArray(), 0));
			this.Temp = Convert.ToDouble(BitConverter.ToSingle(temp.Reverse().ToArray(), 0));
			this.WaterLevel = Convert.ToDouble(BitConverter.ToSingle(waterLevel.Reverse().ToArray(), 0));
			this.Position = Convert.ToDouble(BitConverter.ToSingle(position.Reverse().ToArray(), 0));
			this.GaugeStatus = BitConverter.ToUInt16(gaugeStatus.Reverse().ToArray(), 0);
			this.PntStatus = BitConverter.ToUInt16(pntStatus.Reverse().ToArray(), 0);
		}
	}
}
