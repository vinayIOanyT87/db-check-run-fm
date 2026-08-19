namespace FMBusinessObjects.DataObjects.Message
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

    public class VeederRootSystemAlarmsMsg : EdgeData
	{
		public ushort PntStatus { get; set; }

		public override void Load(MemoryStream memoryStream)
		{
			base.Load(memoryStream);

			byte[] pntStatus = new byte[2];
			memoryStream.Read(pntStatus, 0, 2);
			this.PntStatus = BitConverter.ToUInt16(pntStatus.Reverse().ToArray(), 0);
		}
	}
}
