namespace FMBusinessObjects.DataObjects.Message
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public class ITTBarton3500ATGMsg : EdgeData
	{
		public double Level { get; set; }
		public double Temp { get; set; }
		public double WaterLevel { get; set; }
		public double Density { get; set; }
		public double Value1 { get; set; }
		public ushort AlarmFlag { get; set; }
		public ushort PntStatus { get; set; }

		public override void Load(MemoryStream memoryStream)
		{
			base.Load(memoryStream);

			byte[] level = new byte[4];
			byte[] temp = new byte[4];
			byte[] waterLevel = new byte[4];
			byte[] density = new byte[4];
			byte[] value1 = new byte[4];
			byte[] alarmFlag = new byte[2];
			byte[] pntStatus = new byte[2];

			memoryStream.Read(level, 0, 4);
			memoryStream.Read(temp, 0, 4);
			memoryStream.Read(waterLevel, 0, 4);
			memoryStream.Read(density, 0, 4);
			memoryStream.Read(value1, 0, 4);
			memoryStream.Read(alarmFlag, 0, 2);
			memoryStream.Read(pntStatus, 0, 2);

			this.Level = Convert.ToDouble(BitConverter.ToSingle(level.Reverse().ToArray(), 0));
			this.Temp = Convert.ToDouble(BitConverter.ToSingle(temp.Reverse().ToArray(), 0));
			this.WaterLevel = Convert.ToDouble(BitConverter.ToSingle(waterLevel.Reverse().ToArray(), 0));
			this.Density = Convert.ToDouble(BitConverter.ToSingle(density.Reverse().ToArray(), 0));
			this.Value1 = Convert.ToDouble(BitConverter.ToSingle(value1.Reverse().ToArray(), 0));
			this.AlarmFlag = BitConverter.ToUInt16(alarmFlag.Reverse().ToArray(), 0);
			this.PntStatus = BitConverter.ToUInt16(pntStatus.Reverse().ToArray(), 0);
		}
	}



}
