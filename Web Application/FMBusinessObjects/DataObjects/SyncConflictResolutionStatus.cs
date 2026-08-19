namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	[DataContract]
	public class SyncConflictResolutionStatus
	{
		#region Attributes
		private int _Pass = 0;
		private Int64? _LastRowVersion = 0;
		#endregion Attributes

		#region Properties

		[DataMember]
		public int Pass
		{
			get { return (_Pass); }
			set { _Pass = value; }
		}

		[DataMember]
		public Int64? LastRowVersion
		{
			get { return (_LastRowVersion); }
			set { _LastRowVersion = value; }
		}

		#endregion Properties
	}
}
