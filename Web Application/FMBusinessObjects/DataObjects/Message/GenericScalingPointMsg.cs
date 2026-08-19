namespace FMBusinessObjects.DataObjects.Message
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

    public class GenericScalingPointMsg : EdgeData
	{
		public double Value { get; set; }

		public override void Load(MemoryStream memoryStream)
		{
			base.Load(memoryStream);

			byte[] value = new byte[4];
			byte[] densityTemp = new byte[4];
			byte[] densityTime = new byte[4];

			memoryStream.Read(value, 0, 4);
			this.Value = Convert.ToDouble(BitConverter.ToSingle(value.Reverse().ToArray(), 0));
		}
	}
}
