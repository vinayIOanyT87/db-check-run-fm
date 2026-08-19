namespace FMBusinessObjects.DataObjects.Message
{
    using FMBusinessObjects.Constants;
    using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;


	public abstract class EdgeData
	{
		public string ImeiNumber { get; set; }

		public DateTimeOffset TimeStamp { get; set; }

		public byte MsgExt { get; set; }

		public EdgeMessageType MsgType { get; set; }

		public ushort Index { get; set; }

		public byte? Device { get; set; }

		public virtual void Load(MemoryStream memoryStream)
		{
			byte[] imeiNumber = new byte[15];
			byte[] timestamp = new byte[4];
			byte[] msgExt = new byte[1];
			byte[] msgType = new byte[1];
			byte[] index = new byte[1];

			memoryStream.Read(imeiNumber, 0, 15);
			memoryStream.Read(timestamp, 0, 4);
			memoryStream.Read(msgExt, 0, 1);
			memoryStream.Read(msgType, 0, 1);
			memoryStream.Read(index, 0, 1);

			this.MsgExt = msgExt[0];
			this.MsgType = (EdgeMessageType)Convert.ToInt16(msgType[0]);
			this.ImeiNumber = new string(imeiNumber.Select(c => (char)c).ToArray());
			this.TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(BitConverter.ToInt32(timestamp.Reverse().ToArray(), 0)));
			this.Index = (ushort) index[0];
		}
	}
}
