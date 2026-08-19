using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using FMBusinessObjects.Constants;

namespace FMBusinessObjects.DataObjects
{
    public delegate void SyncProgressEventHandler(object Sender, SyncProgressEventArgsFM Args);
    
    [Serializable]
	[DataContract]
    [KnownType(typeof(SyncTableMetadataFM))]
    [KnownType(typeof(SyncTableProgressFM))]
    [KnownType(typeof(SyncGroupMetadataFM))]
    [KnownType(typeof(SyncGroupProgressFM))]
    [KnownType(typeof(SYNCSTAGE))]
    public class SyncProgressEventArgsFM : EventArgs
	{
		#region Attributes
		private SyncGroupMetadataFM _GroupMetadata = null;
		private SyncGroupProgressFM _GroupProgress = null;
		private SYNCSTAGE _SyncStage = SYNCSTAGE.READY;
		private SyncTableMetadataFM _TableMetadata = null;
		private SyncTableProgressFM _TableProgress = null;

		private int _MaxProgress = 0;
		private int _CurrentProgress = 0;
		#endregion Attributes

		#region Properties

		#region GroupMetadata property
		[DataMember]
		public SyncGroupMetadataFM GroupMetadata
		{
			get { return _GroupMetadata; }
			internal set { _GroupMetadata = value; }
		}
		#endregion GroupMetadata property

		#region GroupProgress property
		[DataMember]
		public SyncGroupProgressFM GroupProgress
		{
			get { return _GroupProgress; }
			private set { _GroupProgress = value; }
		}
		#endregion GroupProgress property

		#region SyncStage property
		[DataMember]
		public SYNCSTAGE SyncStage
		{
			get { return _SyncStage; }
			private set { _SyncStage = value; }
		}
		#endregion SyncStage property

		#region TableMetadata property
		[DataMember]
		public SyncTableMetadataFM TableMetadata
		{
			get { return _TableMetadata; }
			private set { _TableMetadata = value; }
		}
		#endregion TableMetadata property

		#region TableProgress property
		[DataMember]
		public SyncTableProgressFM TableProgress
		{
			get { return _TableProgress; }
			private set { _TableProgress = value; }
		}
		#endregion TableProgress property

		#region MaxProgress property
		[DataMember]
		public int MaxProgress
		{ 
			get { return (_MaxProgress); }
			private set { _MaxProgress = value; }
		}
		#endregion MaxProgress property

		#region CurrentProgress property
		[DataMember]
		public int CurrentProgress
		{ 
			get { return (_CurrentProgress); }
			set { _CurrentProgress = value; }
		}
		#endregion CurrentProgress property

		#endregion Properties

		#region Constructors / Initialization
		public SyncProgressEventArgsFM(SyncTableMetadataFM SyncTableMetadata,
												SyncTableProgressFM SyncTableProgress,
												SyncGroupMetadataFM SyncGroupMetadata,
												SyncGroupProgressFM SyncGroupProgress, 
												SYNCSTAGE SyncStage,
												int MaxProgress, 
												int CurrentProgress)
		{
			_TableMetadata = SyncTableMetadata;
			_TableProgress = SyncTableProgress;

			_GroupMetadata = SyncGroupMetadata;
			_GroupProgress= SyncGroupProgress;

			_SyncStage = SyncStage;

			_MaxProgress = MaxProgress;
			_CurrentProgress = CurrentProgress;
		}
		#endregion Constructors / Initialization
	}
}
