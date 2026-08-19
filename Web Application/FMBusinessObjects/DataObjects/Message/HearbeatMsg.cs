
namespace FMBusinessObjects.DataObjects.Message
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public class HeartbeatMsg : EdgeData
	{
		public uint Counter { get; set; }

		public override void Load(MemoryStream memoryStream)
		{
			base.Load(memoryStream);

			byte[] counter = new byte[4];
			memoryStream.Read(counter, 0, 4);
			this.Counter = BitConverter.ToUInt32(counter.Reverse().ToArray(), 0);
		}
	}
}
