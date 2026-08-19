using FMBusinessObjects.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects
{

	#region Point Group Collection Class
	[Serializable]
	[CollectionDataContract]
	public class PointGroupScheduleCollection : List<PointGroupSchedule>
	{
		public PointGroupScheduleCollection Clone()
		{
			var pointGroupScheduleCollection = new PointGroupScheduleCollection();
			foreach (var p in this)
			{
				pointGroupScheduleCollection.Add(p.Clone());
			}
			return pointGroupScheduleCollection;
		}
	}
	#endregion

	[DataContract]
	[Serializable]
	public class PointGroupSchedule : BaseDataObject, IAlarmAndEventDiscovery
	{

		private const string ScheduleReportStarting = "Running Scheduled Point Group Report";
		private const string ScheduleReportCompleted = "Completed Scheduled Point Group Report";

		public static readonly AlarmAndEventDescriptorClass ScheduleReportStartingEventDescriptor =
			 new AlarmAndEventDescriptorClass(false, SystemKey, ScheduleReportStarting);
		public static readonly AlarmAndEventDescriptorClass ScheduleReportCompletedEventDescriptor =
			 new AlarmAndEventDescriptorClass(false, SystemKey, ScheduleReportCompleted);

		public enum LayoutType
		{
			Portrait = 1,
			Landscape = 2
		}

		public enum ExportFileType
		{
			PDF = 1,
			CSV = 2
		}

		#region Constructors and Destructors

		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public PointGroupSchedule()
		{
			this.Init();
		}

		public PointGroupSchedule Clone()
		{
			PointGroupSchedule p = (PointGroupSchedule)this.MemberwiseClone();

			p.RowVersion = new byte[this.RowVersion.Length];
			for (int i = 0; i < this.RowVersion.Length; i++)
			{
				p.RowVersion[i] = this.RowVersion[i];
			}
			return p;
		}
		#endregion

		#region Properties
		[FMPersistedField]
		public Guid PointGroupScheduleGuid
		{
			get { return this.IdentityGuid; }
			set { this.IdentityGuid = value; }
		}

		[DataMember]
		[FMPersistedField]
		public Guid PointGroupGuid { get; set; }


		[DataMember]
		[FMPersistedField]
		public Guid UserGuid { get; set; }


		[DataMember]
		[FMPersistedField]
		public string CronSchedule { get; set; }

		[DataMember]
		[FMPersistedField]
		public DateTime StartSchedule { get; set; }

		[DataMember]
		[FMPersistedField]
		public string EndSchedule { get; set; }

		[DataMember]
		[FMPersistedField]
		public string Printer { get; set; }

		[DataMember]
		[FMPersistedField]
		public string EmailTo { get; set; }

		[DataMember]
		[FMPersistedField(DefaultValue = LayoutType.Portrait)]
		public LayoutType Layout { get; set; }

		[DataMember]
		[FMPersistedField(DefaultValue = ExportFileType.PDF)]
		public ExportFileType ExportFileFormat { get; set; }

		[DataMember]
		[FMPersistedField]
		public bool CreateNewExportFile { get; set; }


		[DataMember]
		[FMPersistedField]
		public bool FitToPage { get; set; }
#endregion

		  #region Private methods
		  /// <summary>
		  /// This method will initialize the object to its initial state.
		  /// </summary>
		  private void Init()
		{
			base.Reset();

			this.PointGroupScheduleGuid = Guid.Empty;
			this.PointGroupGuid = Guid.Empty;
			this.UserGuid = Guid.Empty;
			this.SiteGuid = Guid.Empty;
			this.CronSchedule = string.Empty;
			this.StartSchedule = new DateTime();
			this.EndSchedule = string.Empty;
			this.Printer = string.Empty;
			this.EmailTo = string.Empty;
			this.Layout = LayoutType.Portrait;
			this.ExportFileFormat = ExportFileType.PDF;
			this.CreateNewExportFile = false;
			this.FitToPage = false;
		}
		#endregion

		#region Explicit Interface Properties

		AlarmAndEventDescriptorClass[] IAlarmAndEventDiscovery.AlarmAndEvents
		{
			get
			{
				AlarmAndEventDescriptorClass[] descriptors =
				{
					ScheduleReportStartingEventDescriptor,
					ScheduleReportCompletedEventDescriptor
				};
				return descriptors;
			}
		}

		#endregion
	}
}
