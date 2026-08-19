namespace FMBusinessObjects.DataObjects.Message
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

    public class DeviceStatusMsg : EdgeData
	{
		public byte DeviceType { get; set; }
		public ushort DeviceStatus { get; set; }

		public override void Load(MemoryStream memoryStream)
		{
			base.Load(memoryStream);

			byte[] deviceType = new byte[1];
			byte[] deviceStatus = new byte[2];

			memoryStream.Read(deviceType, 0, 1);
			memoryStream.Read(deviceStatus, 0, 2);

			this.DeviceType = deviceType[0];
			this.DeviceStatus = BitConverter.ToUInt16(deviceStatus.Reverse().ToArray(), 0);

		}
	}
}
