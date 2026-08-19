namespace FMBusinessObjects.DataObjects.Message
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

    public class VeederRootInventoryReportMsg : EdgeData
	{
		public double Height { get; set; }
		public double Temperature { get; set; }
		public double Volume { get; set; }
		public double TCVolume { get; set; }
		public double Water { get; set; }
		public double Ullage { get; set; }
		public double WaterVolume { get; set; }

		public override void Load(MemoryStream memoryStream)
		{
			base.Load(memoryStream);
			byte[] height = new byte[4];
			byte[] temperature = new byte[4];
			byte[] volume = new byte[4];
			byte[] tcVolume = new byte[4];
			byte[] water = new byte[4];
			byte[] ullage = new byte[4];
			byte[] waterVolume = new byte[4];

			memoryStream.Read(height, 0, 4);
			memoryStream.Read(temperature, 0, 4);
			memoryStream.Read(volume, 0, 4);
			memoryStream.Read(tcVolume, 0, 4);
			memoryStream.Read(water, 0, 4);
			memoryStream.Read(ullage, 0, 4);
			memoryStream.Read(waterVolume, 0, 4);

			this.Height = Convert.ToDouble(BitConverter.ToSingle(height.Reverse().ToArray(), 0));
			this.Temperature = Convert.ToDouble(BitConverter.ToSingle(temperature.Reverse().ToArray(), 0));
			this.Volume = Convert.ToDouble(BitConverter.ToSingle(volume.Reverse().ToArray(), 0));
			this.TCVolume = Convert.ToDouble(BitConverter.ToSingle(tcVolume.Reverse().ToArray(), 0));
			this.Water = Convert.ToDouble(BitConverter.ToSingle(water.Reverse().ToArray(), 0));
			this.Ullage = Convert.ToDouble(BitConverter.ToSingle(ullage.Reverse().ToArray(), 0));
			this.WaterVolume = Convert.ToDouble(BitConverter.ToSingle(waterVolume.Reverse().ToArray(), 0));

		}
	}
}
