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
	public class SyncConflictFM
	{
		#region Attributes
		private SYNCCONFLICTTYPE _ConflictType = SYNCCONFLICTTYPE.UNKNOWN;
		private string _ErrorMessage = null;
		private SYNCSTAGE _SyncStage = SYNCSTAGE.READY;
		#endregion Attributes

		#region Properties

		#region ConflictType property
		[DataMember]
		public SYNCCONFLICTTYPE ConflictType
		{
			get { return (_ConflictType); }
			set { _ConflictType = value; }
		}
		#endregion ConflictType property

		#region ErrorMessage property
		[DataMember]
		public string ErrorMessage
		{
			get { return (_ErrorMessage); }
			set { _ErrorMessage = value; }
		}
		#endregion ErrorMessage property

		#region SyncStage property
		[DataMember]
		public SYNCSTAGE SyncStage
		{
			get { return (_SyncStage); }
			set { _SyncStage = value; }
		}
		#endregion SyncStage property

		#endregion Properties

		#region Constructor
		public SyncConflictFM(SYNCCONFLICTTYPE ConflictType, SYNCSTAGE SyncStage)
		{
			_ConflictType = ConflictType;
			_SyncStage = SyncStage;
		}
		#endregion Constructor
	}
}
