namespace FMBusinessObjects.DataObjects.Message
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public class Enraf854TankGaugeDensityMsg : EdgeData
	{
		public double Density { get; set; }
		public double DensityTemp { get; set; }
		public DateTimeOffset DensityTime { get; set; }

		public override void Load(MemoryStream memoryStream)
		{
			base.Load(memoryStream);

			byte[] density = new byte[4];
			byte[] densityTemp = new byte[4];
			byte[] densityTime = new byte[4];

			memoryStream.Read(density, 0, 4);
			memoryStream.Read(densityTemp, 0, 4);
			memoryStream.Read(densityTime, 0, 4);

			this.Density = Convert.ToDouble(BitConverter.ToSingle(density.Reverse().ToArray(), 0));
			this.DensityTemp = Convert.ToDouble(BitConverter.ToSingle(densityTemp.Reverse().ToArray(), 0));
			this.DensityTime = new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(BitConverter.ToInt32(densityTime.Reverse().ToArray(), 0)));
		}
	}
}