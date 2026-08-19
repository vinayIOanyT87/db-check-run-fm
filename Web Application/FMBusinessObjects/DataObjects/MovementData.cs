namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;


	[DataContract(Namespace = "")]
	[KnownType(typeof(CodedVariables.MovementType))]
	[Serializable()]
	public class MovementData : ICloneable
	{
		[DataMember(Order = 0)]
		public List<PointValue> PointId { get; set; }
		[DataMember(Order = 1)]
		public List<PointValue> CreatedBy { get; set; }
		[DataMember(Order = 2)]
		public List<PointValue> Status { get; set; }
		[DataMember(Order = 3)]
		public List<PointValue> TransferStatus { get; set; }
		[DataMember(Order = 4)]
		public List<PointValue> Product { get; set; }
		[DataMember(Order = 5)]
		public List<PointValue> TransferStartTime { get; set; }
		[DataMember(Order = 6)]
		public List<PointValue> TransferStopTime { get; set; }
		[DataMember(Order = 7)]
		public List<PointValue> InitiationCount { get; set; }
		[DataMember(Order = 8)]
		public List<PointValue> LevelProduct { get; set; }
		[DataMember(Order = 9)]
		public List<PointValue> LevelWater { get; set; }
		[DataMember(Order = 10)]
		public List<PointValue> MassLiquid { get; set; }
		[DataMember(Order = 11)]
		public List<PointValue> TemperatureAmbient { get; set; }
		[DataMember(Order = 12)]
		public List<PointValue> TemperatureDensity { get; set; }
		[DataMember(Order = 13)]
		public List<PointValue> TemperatureProduct { get; set; }
		[DataMember(Order = 14)]
		public List<PointValue> DensityProductObserved { get; set; }
		[DataMember(Order = 15)]
		public List<PointValue> DensityProductinAir { get; set; }
		[DataMember(Order = 16)]
		public List<PointValue> DensityProductStandard { get; set; }
		[DataMember(Order = 17)]
		public List<PointValue> DensityProductStandardinAir { get; set; }
		[DataMember(Order = 18)]
		public List<PointValue> VolumeCorrectionFactor { get; set; }
		[DataMember(Order = 19)]
		public List<PointValue> VolumeGrossObserved { get; set; }
		[DataMember(Order = 20)]
		public List<PointValue> VolumeGrossStandard { get; set; }
		[DataMember(Order = 21)]
		public List<PointValue> VolumeNetStandard { get; set; }
		[DataMember(Order = 22)]
		public List<PointValue> VolumeTotalObserved { get; set; }
		[DataMember(Order = 23)]
		public List<PointValue> VolumeWater { get; set; }
		[DataMember(Order = 24)]
		public List<PointValue> VolumeRoofCorrection { get; set; }
		[DataMember(Order = 25)]
		public List<PointValue> TankShellCorrection { get; set; }
		[DataMember(Order = 26)]
		public List<PointValue> VolumeGrossObservedRate { get; set; }
		[DataMember(Order = 27)]
		public List<PointValue> VolumeNetStandardRate { get; set; }
		[DataMember(Order = 28)]
		public List<PointValue> VolumeTotalObservedRate { get; set; }
		[DataMember(Order = 29)]
		public List<PointValue> UserData01 { get; set; }
		[DataMember(Order = 30)]
		public List<PointValue> UserData02 { get; set; }
		[DataMember(Order = 31)]
		public List<PointValue> UserData03 { get; set; }
		[DataMember(Order = 32)]
		public List<PointValue> UserData04 { get; set; }
		[DataMember(Order = 33)]
		public List<PointValue> UserData05 { get; set; }
		[DataMember(Order = 34)]
		public List<PointValue> UserData06 { get; set; }
		[DataMember(Order = 35)]
		public List<PointValue> UserData07 { get; set; }
		[DataMember(Order = 36)]
		public List<PointValue> UserData08 { get; set; }
		[DataMember(Order = 37)]
		public List<PointValue> UserData09 { get; set; }
		[DataMember(Order = 38)]
		public List<PointValue> UserData10 { get; set; }
		[DataMember(Order = 39)]
		public List<PointValue> TransferredGOV { get; set; }
		[DataMember(Order = 40)]
		public List<PointValue> TransferredNSV { get; set; }
		[DataMember(Order = 41)]
		public List<PointValue> TransferredVolumeWater { get; set; }
		[DataMember(Order = 42)]
		public List<PointValue> TransferredVolume { get; set; }
		[DataMember(Order = 43)]
		public List<PointValue> TransferStartLevel { get; set; }
		[DataMember(Order = 44)]
		public List<PointValue> TransferStartGOV { get; set; }
		[DataMember(Order = 45)]
		public List<PointValue> TransferStartNSV { get; set; }
		[DataMember(Order = 46)]
		public List<PointValue> TransferStartWaterVolume { get; set; }
		[DataMember(Order = 47)]
		public List<PointValue> TransferStartVolume { get; set; }
		[DataMember(Order = 48)]
		public List<PointValue> TransferMode { get; set; }
		[DataMember(Order = 49)]
		public List<PointValue> TransferTarget { get; set; }
		[DataMember(Order = 50)]
		public List<PointValue> TransferLevelTarget { get; set; }
		[DataMember(Order = 51)]
		public List<PointValue> TransferVolumeTarget { get; set; }
		[DataMember(Order = 52)]
		public List<PointValue> TransferTimeRemaining { get; set; }
		[DataMember(Order = 53)]
		public List<PointValue> TransferTimeCompleton { get; set; }
		[DataMember(Order = 54)]
		public List<PointValue> Deviation { get; set; }
		[DataMember(Order = 55)]
		public List<PointValue> PercentDeviation { get; set; }
        [DataMember(Order = 56)]
        public List<PointValue> PercentBsw { get; set; }
        [DataMember(Order = 57)]
        public List<PointValue> VolumeBsw { get; set; }
        [DataMember(Order = 58)]
		public List<PointValue> StartTemperatureAmbient { get; set; }
		[DataMember(Order = 59)]
		public List<PointValue> StartDensityProductObserved { get; set; }
		[DataMember(Order = 60)]
		public List<PointValue> StartDensityProductinAir { get; set; }
		[DataMember(Order = 61)]
		public List<PointValue> StartDensityProductStandard { get; set; }
		[DataMember(Order = 62)]
		public List<PointValue> StartDensityProductStandardinAir { get; set; }
		[DataMember(Order = 63)]
		public List<PointValue> StartLevelWater { get; set; }
		[DataMember(Order = 64)]
		public List<PointValue> StartMassLiquid { get; set; }
        [DataMember(Order = 65)]
        public List<PointValue> StartPercentBsw { get; set; }
        [DataMember(Order = 66)]
		public List<PointValue> StartTankShellCorrection { get; set; }
		[DataMember(Order = 67)]
		public List<PointValue> StartTemperatureDensity { get; set; }
		[DataMember(Order = 68)]
		public List<PointValue> StartTemperatureProduct { get; set; }
        [DataMember(Order = 69)]
        public List<PointValue> StartVolumeBsw { get; set; }
        [DataMember(Order = 70)]
		public List<PointValue> StartVolumeCorrectionFactor { get; set; }
		[DataMember(Order = 71)]
		public List<PointValue> StartVolumeRoofCorrection { get; set; }
		[DataMember(Order = 72)]
		public List<PointValue> StartVolumeTotalObserved { get; set; }
		[DataMember(Order = 73)]
		public List<PointValue> StartVolumeGrossStandard { get; set; }
		[DataMember(Order = 74)]
		public List<PointValue> OpeningTemperatureAmbient { get; set; }
		[DataMember(Order = 75)]
		public List<PointValue> OpeningDensityProductObserved { get; set; }
		[DataMember(Order = 76)]
		public List<PointValue> OpeningDensityProductinAir { get; set; }
		[DataMember(Order = 77)]
		public List<PointValue> OpeningDensityProductStandard { get; set; }
		[DataMember(Order = 78)]
		public List<PointValue> OpeningDensityProductStandardinAir { get; set; }
		[DataMember(Order = 79)]
		public List<PointValue> OpeningLevelProduct { get; set; }
		[DataMember(Order = 80)]
		public List<PointValue> OpeningLevelWater { get; set; }
		[DataMember(Order = 81)]
		public List<PointValue> OpeningMassLiquid { get; set; }
        [DataMember(Order = 82)]
        public List<PointValue> OpeningPercentBsw { get; set; }
        [DataMember(Order = 83)]
		public List<PointValue> OpeningTankShellCorrection { get; set; }
		[DataMember(Order = 84)]
		public List<PointValue> OpeningTemperatureDensity { get; set; }
		[DataMember(Order = 85)]
		public List<PointValue> OpeningTemperatureProduct { get; set; }
        [DataMember(Order = 86)]
        public List<PointValue> OpeningVolumeBsw { get; set; }
        [DataMember(Order = 87)]
		public List<PointValue> OpeningVolumeCorrectionFactor { get; set; }
		[DataMember(Order = 88)]
		public List<PointValue> OpeningVolumeGrossObserved { get; set; }
		[DataMember(Order = 89)]
		public List<PointValue> OpeningVolumeGrossStandard { get; set; }
		[DataMember(Order = 90)]
		public List<PointValue> OpeningVolumeNetStandard { get; set; }
		[DataMember(Order = 91)]
		public List<PointValue> OpeningVolumeRoofCorrection { get; set; }
		[DataMember(Order = 92)]
		public List<PointValue> OpeningVolumeTotalObserved { get; set; }
		[DataMember(Order = 93)]
		public List<PointValue> OpeningVolumeWater { get; set; }
		[DataMember(Order = 94)]
		public List<PointValue> TransferDirection { get; set; }
		[DataMember(Order = 95)]
		public List<PointValue> IndividualNodeControl { get; set; }
		[DataMember(Order = 96)]
		public List<PointValue> Comment { get; set; }
		[DataMember(Order = 97)]
		public List<PointValue> OrderNumber { get; set; }
		[DataMember(Order = 98)]
		public List<PointValue> PlannedStartTime { get; set; }
		[DataMember(Order = 99)]
		public List<PointValue> Type { get; set; }

        public MovementData()
		{
			
			this.PointId = new List<PointValue>();
			this.CreatedBy = new List<PointValue>();
			this.Status = new List<PointValue>();
			this.TransferStatus = new List<PointValue>();
			this.Product = new List<PointValue>();
			this.TransferStartTime = new List<PointValue>();
			this.TransferStopTime = new List<PointValue>();
			this.InitiationCount = new List<PointValue>();
			this.LevelProduct = new List<PointValue>();
			this.LevelWater = new List<PointValue>();
			this.MassLiquid = new List<PointValue>();
			this.TemperatureAmbient = new List<PointValue>();
			this.TemperatureDensity = new List<PointValue>();
			this.TemperatureProduct = new List<PointValue>();
			this.DensityProductObserved = new List<PointValue>();
			this.DensityProductinAir = new List<PointValue>();
			this.DensityProductStandard = new List<PointValue>();
			this.DensityProductStandardinAir = new List<PointValue>();
			this.VolumeCorrectionFactor = new List<PointValue>();
			this.VolumeGrossObserved = new List<PointValue>();
			this.VolumeGrossStandard = new List<PointValue>();
			this.VolumeNetStandard = new List<PointValue>();
			this.VolumeTotalObserved = new List<PointValue>();
			this.VolumeWater = new List<PointValue>();
			this.VolumeRoofCorrection = new List<PointValue>();
			this.TankShellCorrection = new List<PointValue>();
			this.VolumeGrossObservedRate = new List<PointValue>();
			this.VolumeNetStandardRate = new List<PointValue>();
			this.VolumeTotalObservedRate = new List<PointValue>();
			this.UserData01 = new List<PointValue>();
			this.UserData02 = new List<PointValue>();
			this.UserData03 = new List<PointValue>();
			this.UserData04 = new List<PointValue>();
			this.UserData05 = new List<PointValue>();
			this.UserData06 = new List<PointValue>();
			this.UserData07 = new List<PointValue>();
			this.UserData08 = new List<PointValue>();
			this.UserData09 = new List<PointValue>();
			this.UserData10 = new List<PointValue>();
			this.TransferredGOV = new List<PointValue>();
			this.TransferredNSV = new List<PointValue>();
			this.TransferredVolumeWater = new List<PointValue>();
			this.TransferredVolume = new List<PointValue>();
			this.TransferStartLevel = new List<PointValue>();
			this.TransferStartGOV = new List<PointValue>();
			this.TransferStartNSV = new List<PointValue>();
			this.TransferStartWaterVolume = new List<PointValue>();
			this.TransferStartVolume = new List<PointValue>();
			this.TransferMode = new List<PointValue>();
			this.TransferTarget = new List<PointValue>();
			this.TransferLevelTarget = new List<PointValue>();
			this.TransferVolumeTarget = new List<PointValue>();
			this.TransferTimeRemaining = new List<PointValue>();
			this.TransferTimeCompleton = new List<PointValue>();
			this.Deviation = new List<PointValue>();
			this.PercentDeviation = new List<PointValue>();
            this.PercentBsw = new List<PointValue>();
            this.VolumeBsw = new List<PointValue>();
            this.StartTemperatureAmbient = new List<PointValue>();
			this.StartDensityProductObserved = new List<PointValue>();
			this.StartDensityProductinAir = new List<PointValue>();
			this.StartDensityProductStandard = new List<PointValue>();
			this.StartDensityProductStandardinAir = new List<PointValue>();
			this.StartLevelWater = new List<PointValue>();
			this.StartMassLiquid = new List<PointValue>();
            this.StartPercentBsw = new List<PointValue>();
            this.StartTankShellCorrection = new List<PointValue>();
			this.StartTemperatureDensity = new List<PointValue>();
			this.StartTemperatureProduct = new List<PointValue>();
            this.StartVolumeBsw = new List<PointValue>();
            this.StartVolumeCorrectionFactor = new List<PointValue>();
			this.StartVolumeRoofCorrection = new List<PointValue>();
			this.StartVolumeTotalObserved = new List<PointValue>();
			this.StartVolumeGrossStandard = new List<PointValue>();
			this.OpeningTemperatureAmbient = new List<PointValue>();
			this.OpeningDensityProductObserved = new List<PointValue>();
			this.OpeningDensityProductinAir = new List<PointValue>();
			this.OpeningDensityProductStandard = new List<PointValue>();
			this.OpeningDensityProductStandardinAir = new List<PointValue>();
			this.OpeningLevelProduct = new List<PointValue>();
			this.OpeningLevelWater = new List<PointValue>();
			this.OpeningMassLiquid = new List<PointValue>();
            this.OpeningPercentBsw = new List<PointValue>();
            this.OpeningTankShellCorrection = new List<PointValue>();
			this.OpeningTemperatureDensity = new List<PointValue>();
			this.OpeningTemperatureProduct = new List<PointValue>();
            this.OpeningVolumeBsw = new List<PointValue>();
            this.OpeningVolumeCorrectionFactor = new List<PointValue>();
			this.OpeningVolumeGrossObserved = new List<PointValue>();
			this.OpeningVolumeGrossStandard = new List<PointValue>();
			this.OpeningVolumeNetStandard = new List<PointValue>();
			this.OpeningVolumeRoofCorrection = new List<PointValue>();
			this.OpeningVolumeTotalObserved = new List<PointValue>();
			this.OpeningVolumeWater = new List<PointValue>();
			this.TransferDirection = new List<PointValue>();
			this.IndividualNodeControl = new List<PointValue>();
			this.Comment = new List<PointValue>();
			this.OrderNumber = new List<PointValue>();
			this.PlannedStartTime = new List<PointValue>();
			this.Type = new List<PointValue>();
        }

        public object Clone()
		{
			var p = (MovementData)this.MemberwiseClone();
			return p;
		}
	}
}
