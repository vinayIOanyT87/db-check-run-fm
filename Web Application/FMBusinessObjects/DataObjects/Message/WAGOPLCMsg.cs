namespace FMBusinessObjects.DataObjects.Message
{
	 using System;
	 using System.Collections.Generic;
	 using System.IO;
	 using System.Linq;
	 using System.Text;
	 using System.Threading.Tasks;

	 public class WAGOPLCMsg : EdgeData
	 {
		  public byte DeviceType { get; set; }
		  public double Level { get; set; }
		  public double Temp { get; set; }
		  public uint Density { get; set; }
		  public double DensityTemp { get; set; }
		  public double Position { get; set; }
		  public double WaterLevel { get; set; }
		  public double WaterSump { get; set; }
		  public ushort GaugeStatus { get; set; }
		  public ushort TroubleInfo { get; set; }
		  public ushort LevelAlarm { get; set; }
		  public uint Volume { get; set; }
		  public uint WaterVolume { get; set; }
		  public uint NetVolume { get; set; }
		  public uint Ullage { get; set; }
		  public uint WaterSumpVol { get; set; }

		  public override void Load(MemoryStream memoryStream)
		  {
				base.Load(memoryStream);
				byte[] deviceType = new byte[1];
				byte[] level = new byte[4];
				byte[] temp = new byte[4];
				byte[] density = new byte[4];
				byte[] densityTemp = new byte[4];
				byte[] position = new byte[4];
				byte[] waterLevel = new byte[4];
				byte[] waterSump = new byte[4];
				byte[] gaugeStatus = new byte[2];
				byte[] troubleInfo = new byte[2];
				byte[] levelAlarm = new byte[2];
				byte[] volume = new byte[4];
				byte[] waterVolume = new byte[4];
				byte[] netVolume = new byte[4];
				byte[] ullage = new byte[4];
				byte[] waterSumpVol = new byte[4];

				memoryStream.Read(deviceType, 0, 1);
				memoryStream.Read(level, 0, 4);
				memoryStream.Read(temp, 0, 4);
				memoryStream.Read(density, 0, 4);
				memoryStream.Read(densityTemp, 0, 4);
				memoryStream.Read(position, 0, 4);
				memoryStream.Read(waterLevel, 0, 4);
				memoryStream.Read(waterSump, 0, 4);
				memoryStream.Read(gaugeStatus, 0, 2);
				memoryStream.Read(troubleInfo, 0, 2);
				memoryStream.Read(levelAlarm, 0, 2);
				memoryStream.Read(volume, 0, 4);
				memoryStream.Read(waterVolume, 0, 4);
				memoryStream.Read(netVolume, 0, 4);
				memoryStream.Read(ullage, 0, 4);
				memoryStream.Read(waterSumpVol, 0, 4);

				this.DeviceType = deviceType[0];
				this.Level = Convert.ToDouble(BitConverter.ToSingle(level.Reverse().ToArray(), 0));
				this.Temp = Convert.ToDouble(BitConverter.ToSingle(temp.Reverse().ToArray(), 0));
				this.Density = BitConverter.ToUInt32(density.Reverse().ToArray(), 0);
				this.DensityTemp = Convert.ToDouble(BitConverter.ToSingle(densityTemp.Reverse().ToArray(), 0));
				this.Position = Convert.ToDouble(BitConverter.ToSingle(position.Reverse().ToArray(), 0));
				this.WaterLevel = Convert.ToDouble(BitConverter.ToSingle(waterLevel.Reverse().ToArray(), 0));
				this.WaterSump = Convert.ToDouble(BitConverter.ToSingle(waterSump.Reverse().ToArray(), 0));
				this.GaugeStatus = BitConverter.ToUInt16(gaugeStatus.Reverse().ToArray(), 0);
				this.TroubleInfo = BitConverter.ToUInt16(troubleInfo.Reverse().ToArray(), 0);
				this.LevelAlarm = BitConverter.ToUInt16(levelAlarm.Reverse().ToArray(), 0);
				this.Volume = BitConverter.ToUInt32(volume.Reverse().ToArray(), 0);
				this.WaterVolume = BitConverter.ToUInt32(waterVolume.Reverse().ToArray(), 0);
				this.NetVolume = BitConverter.ToUInt32(netVolume.Reverse().ToArray(), 0);
				this.Ullage = BitConverter.ToUInt32(ullage.Reverse().ToArray(), 0);
				this.WaterSumpVol = BitConverter.ToUInt32(waterSumpVol.Reverse().ToArray(), 0);
		  }
	 }
}
