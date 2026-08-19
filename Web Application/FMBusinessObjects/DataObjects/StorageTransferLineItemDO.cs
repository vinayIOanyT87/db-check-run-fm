using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class StorageTransferLineItemDO : LineItemDO
	{
		[DataMember]
		protected string toStorageLocation;
		[DataMember]
		private Guid toStorageLocationTankGuid;

		public StorageTransferLineItemDO()
		{
		}


		public Guid ToStorageLocationTankGuid
		{
			get { return toStorageLocationTankGuid; }
			set { toStorageLocationTankGuid = value; }
		}

		public string ToStorageLocation
		{
			get { return toStorageLocation; }
			set { toStorageLocation = value; }
		}
	}
}
