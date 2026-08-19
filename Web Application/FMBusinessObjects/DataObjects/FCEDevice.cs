using FMBusinessObjects.Attributes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects
{
	public class FCEDevice : BaseDataObject
	{

		[FMPersistedField]
		public Guid FCEDeviceGuid
		{
			get
			{
				return this.IdentityGuid;
			}
			set
			{
				this.IdentityGuid = value;
			}
		}

		[DataMember]
		[FMPersistedField]
		public string ImeiNumber { get; set; }

		[DataMember]
		[FMPersistedField]
		public string FriendlyName { get; set; }

		[DataMember]
		[FMPersistedField]
		public bool HeartbeatTimeoutProcessed { get; set; }

		[DataMember]
		[FMPersistedField]
		public bool ConfigReady { get; set; }

		[DataMember]
		[FMPersistedField]
		public int MinTime { get; set; }

		[DataMember]
		[FMPersistedField]
		public int MaxTime { get; set; }

		[DataMember]
		[FMPersistedField]
		public double LevelDeadband { get; set; }

		[DataMember]
		[FMPersistedField]
		public double TempDeadband { get; set; }

		[DataMember]
		[FMPersistedField]
		public int Heartbeat { get; set; }

		[DataMember]
		[FMPersistedField]
		public Int16 TLStanks { get; set; }

		[DataMember]
		[FMPersistedField]
		public Int16 ModbusMap { get; set; }

		[DataMember]
		[FMPersistedField]
		public int MidnightOffset { get; set; }

		[DataMember]
		[FMPersistedField]
		public double ShortDeadband { get; set; }

		[DataMember]
		[FMPersistedField]
		public int ShortTime { get; set; }

		[DataMember]
		[FMPersistedField]
		public double LongDeadband { get; set; }

		[DataMember]
		[FMPersistedField]
		public int LongTime { get; set; }

		[DataMember]
		[FMPersistedField]
		public string SoftwareVersion { get; set; }


		[DataMember]
		public byte[] ScalerType{ get; set; }

		[DataMember]
		public byte[] ScalerConfiguration { get; set; }




		public FCEDevice()
		{
			this.HeartbeatTimeoutProcessed = false;
			this.ConfigReady = false;
			this.MinTime = 1;
			this.MaxTime = 360;
			this.LevelDeadband = 0.00521;
			this.TempDeadband = 0.5;
			this.Heartbeat = 15;
			this.TLStanks = 12;
			this.ModbusMap = 0;
			this.MidnightOffset = 360;
			this.ShortDeadband = 0.01;
			this.ShortTime = 15;
			this.LongDeadband = 0.25;
			this.LongTime = 360;
			this.SoftwareVersion = "FCE-20221212.1";
			this.ScalerType = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
			this.ScalerConfiguration = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
		}
	}
}
