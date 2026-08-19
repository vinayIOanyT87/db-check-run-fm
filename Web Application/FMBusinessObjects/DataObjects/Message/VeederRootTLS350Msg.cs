namespace FMBusinessObjects.DataObjects.Message
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

    public class VeederRootTLS350Msg : EdgeData
	{	
		public double Level { get; set; }
		public double Temp { get; set; }
		public double WaterLevel { get; set; }
		public double GrossVolume { get; set; }
		public double NetVolume { get; set; }
		public double WaterVolume { get; set; }
		public double Ullage { get; set; }

		public override void Load(MemoryStream memoryStream)
		{
			base.Load(memoryStream);
			byte[] level = new byte[4];
			byte[] temp = new byte[4];
			byte[] waterLevel = new byte[4];
			byte[] grossVolume = new byte[4];
			byte[] netVolume = new byte[4];
			byte[] waterVolume = new byte[4];
			byte[] ullage = new byte[4];

			memoryStream.Read(level, 0, 4);
			memoryStream.Read(temp, 0, 4);
			memoryStream.Read(waterLevel, 0, 4);
			memoryStream.Read(grossVolume, 0, 4);
			memoryStream.Read(netVolume, 0, 4);
			memoryStream.Read(waterVolume, 0, 4);
			memoryStream.Read(ullage, 0, 4);

			this.Level = Convert.ToDouble(BitConverter.ToSingle(level.Reverse().ToArray(), 0));
			this.Temp = Convert.ToDouble(BitConverter.ToSingle(temp.Reverse().ToArray(), 0));
			this.WaterLevel = Convert.ToDouble(BitConverter.ToSingle(waterLevel.Reverse().ToArray(), 0));
			this.GrossVolume = Convert.ToDouble(BitConverter.ToSingle(grossVolume.Reverse().ToArray(), 0));
			this.NetVolume = Convert.ToDouble(BitConverter.ToSingle(netVolume.Reverse().ToArray(), 0));
			this.WaterVolume = Convert.ToDouble(BitConverter.ToSingle(waterVolume.Reverse().ToArray(), 0));
			this.Ullage = Convert.ToDouble(BitConverter.ToSingle(ullage.Reverse().ToArray(), 0));

		}
	}
}
