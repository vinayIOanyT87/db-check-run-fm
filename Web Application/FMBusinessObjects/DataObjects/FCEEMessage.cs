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
	[DataContract]
	[Serializable]
	public class FCEEMessage : BaseDataObject
	{
		public FCEEMessage()
		{
		}

		public FCEEMessage(string imeiNumber, DateTimeOffset timestamp, EDGEMESSAGETYPE msgType, int index, int? device, byte[] binaryData, string edgeData, string softwareVersion, bool validity)
		{
			ImeiNumber = imeiNumber;
			Timestamp = timestamp;
			MsgType = msgType;
			Index = index;
			Device = device;
			BinaryData = binaryData;
			EdgeData = edgeData;
			SoftwareVersion = softwareVersion;
			Validity = validity;
		}

		[DataMember]
		[FMPersistedField]
		public string ImeiNumber { get; set; }

		[DataMember]
		[FMPersistedField]
		public DateTimeOffset Timestamp { get; set; }

		[DataMember]
		[FMPersistedField]
		public EDGEMESSAGETYPE MsgType { get; set; }

		[DataMember]
		[FMPersistedField]
		public int Index { get; set; }

		[DataMember]
		[FMPersistedField]
		public int? Device { get; set; }

		[DataMember]
		[FMPersistedField]
		public Byte[] BinaryData { get; set; }

		[DataMember]
		[FMPersistedField]
		public string EdgeData { get; set; }


		[DataMember]
		[FMPersistedField]
		public string SoftwareVersion { get; set; }

		[DataMember]
		[FMPersistedField]
		public bool Validity { get; set; }

		[FMPersistedField]
		public Guid FCEEMessageGuid
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
        
		public void Enumerate(SqlCommand cmd)
		{             
			cmd.CommandType = System.Data.CommandType.Text;
			cmd.CommandText = "SELECT TOP (100) fm.FCEEMessageGuid, fm.Timestamp, fm.ImeiNumber, fm.MsgType, fm.[Index], fm.BinaryData, fm.EdgeData, fm.SoftwareVersion, fm.Validity FROM [dbo].[tblFCEEMessage] fm"
			+ " ORDER BY fm.Timestamp DESC";
		}
	}
}
