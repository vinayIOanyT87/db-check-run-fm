using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using FMBusinessObjects.Constants;

namespace FMBusinessObjects.DataObjects
{
	[Serializable]
	[DataContract]
	public class SyncStatsFM
	{
		#region Attributes
		private int _TotalChangesDownloaded = 0;
		private int _TotalChangesUploaded = 0;

		private int _DownloadChangesApplied = 0;
		private int _DownloadChangesFailed = 0;
		private int _UploadChangesApplied = 0;
		private int _UploadChangesFailed = 0;

        private DateTime? _StartTime = null;
        private DateTime? _CompleteTime = null;
		#endregion Attributes

		#region Properties
		[DataMember]
		public int TotalChangesDownloaded
		{
			get { return _TotalChangesDownloaded; }
			set { _TotalChangesDownloaded = value; }
		}
		[DataMember]
		public int TotalChangesUploaded
		{
			get { return _TotalChangesUploaded; }
			set { _TotalChangesUploaded = value; }
		}
		[DataMember]
		public int DownloadChangesApplied
		{
			get { return _DownloadChangesApplied; }
			set { _DownloadChangesApplied = value; }
		}
		[DataMember]
		public int DownloadChangesFailed
		{
			get { return _DownloadChangesFailed; }
			set { _DownloadChangesFailed = value; }
		}
		[DataMember]
		public int UploadChangesApplied
		{
			get { return _UploadChangesApplied; }
			set { _UploadChangesApplied = value; }
		}
		[DataMember]
		public int UploadChangesFailed
		{
			get { return _UploadChangesFailed; }
			set { _UploadChangesFailed = value; }
		}
		[DataMember]
        public DateTime? StartTime
		{
			get { return _StartTime; }
			set { _StartTime = value; }
		}
		[DataMember]
        public DateTime? CompleteTime
		{
			get { return _CompleteTime; }
			set { _CompleteTime = value; }
		}
		#endregion Properties

		#region Constructor
		public SyncStatsFM()
		{
		}
		#endregion Constructor
	}
}
