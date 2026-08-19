namespace FMBusinessObjects.DataObjects.Message
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

    public class VeederRootInTankStatusReportMsg : EdgeData
	{
		public UInt32 TankStatus { get; set; }

		public override void Load(MemoryStream memoryStream)
		{
			base.Load(memoryStream);

			byte[] tankStatus = new byte[4];
			memoryStream.Read(tankStatus, 0, 4);
			this.TankStatus = BitConverter.ToUInt32(tankStatus.Reverse().ToArray(), 0);
		}
	}
}
