using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using FMBusinessObjects.Constants;

namespace FMBusinessObjects.DataObjects
{
	[Serializable]
	[DataContract]
    [KnownType(typeof(SyncTableMetadataFM))]
    public class SyncGroupMetadataFM
	{
		#region Attributes
		private string _GroupName = "";
		private int _BatchCount = 0;
		private byte[] _MaxAnchor = null;
		private byte[] _NewAnchor = null;

		private List<SyncTableMetadataFM> _GroupTablesMetadata = null;
		#endregion Attributes

		#region Properties

		#region GroupName property
		[DataMember]
		public string GroupName
		{ 
			get { return (_GroupName); }
			set { _GroupName = value; }
		}
		#endregion GroupName property

		#region BatchCount property
		[DataMember]
		public int BatchCount
		{ 
			get { return (_BatchCount); }
			set { _BatchCount = value; }
		}
		#endregion BatchCount property

		#region MaxAnchor property
		[DataMember]
		public byte[] MaxAnchor
		{ 
			get { return (_MaxAnchor); }
			set { _MaxAnchor = value; }
		}
		#endregion MaxAnchor property

		#region NewAnchor property
		[DataMember]
		public byte[] NewAnchor
		{ 
			get { return (_NewAnchor); }
			set { _NewAnchor = value; }
		}
		#endregion NewAnchor property

		#endregion Properties

		#region Constructor
		public SyncGroupMetadataFM(string GroupName)
		{
			_GroupName = GroupName;

			_GroupTablesMetadata = new List<SyncTableMetadataFM>();
		}
		#endregion Constructor
	}
}
