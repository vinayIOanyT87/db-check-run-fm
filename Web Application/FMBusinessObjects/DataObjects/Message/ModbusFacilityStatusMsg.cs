namespace FMBusinessObjects.DataObjects.Message
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

    public class ModbusFacilityStatusMsg : EdgeData
	{
		public byte ModbusMap { get; set; }
		public UInt32 FacilityStatus { get; set; }


		public override void Load(MemoryStream memoryStream)
		{
			base.Load(memoryStream);

			byte[] modbusMap = new byte[1];
			byte[] facilityStatus = new byte[4];

			memoryStream.Read(modbusMap, 0, 1);
			memoryStream.Read(facilityStatus, 0, 4);

			this.ModbusMap = modbusMap[0];
			this.FacilityStatus = BitConverter.ToUInt32(facilityStatus.Reverse().ToArray(), 0);
		}
	}
}
