using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using FMBusinessObjects.Constants;

namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects
{
	[Serializable]
	[DataContract]
	public class GasboyStationStats
	{
		#region Attributes
		private int totalChangesDownloaded = 0;
		private int totalChangesUploaded = 0;

		private int downloadChangesApplied = 0;
		private int downloadChangesFailed = 0;

		private int uploadChangesApplied = 0;
		private int uploadChangesFailed = 0;

        private DateTime? startTime = null;
        private DateTime? completeTime = null;
		#endregion Attributes

		#region Properties
		[DataMember]
		public int TotalChangesDownloaded
		{
			get { return totalChangesDownloaded; }
			set { totalChangesDownloaded = value; }
		}
		[DataMember]
		public int TotalChangesUploaded
		{
			get { return totalChangesUploaded; }
			set { totalChangesUploaded = value; }
		}
		[DataMember]
		public int DownloadChangesApplied
		{
			get { return this.downloadChangesApplied; }
			set { this.downloadChangesApplied = value; }
		}
		[DataMember]
		public int DownloadChangesFailed
		{
			get { return downloadChangesFailed; }
			set { downloadChangesFailed = value; }
		}
		[DataMember]
		public int UploadChangesApplied
		{
			get { return uploadChangesApplied; }
			set { uploadChangesApplied = value; }
		}
		[DataMember]
		public int UploadChangesFailed
		{
			get { return uploadChangesFailed; }
			set { uploadChangesFailed = value; }
		}
		[DataMember]
        public DateTime? StartTime
		{
			get { return startTime; }
			set { startTime = value; }
		}
		[DataMember]
        public DateTime? CompleteTime
		{
			get { return completeTime; }
			set { completeTime = value; }
		}
		#endregion Properties

		#region Constructor
		public GasboyStationStats()
		{
		}
		#endregion Constructor
	}
}
