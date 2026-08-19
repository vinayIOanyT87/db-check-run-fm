namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using FMBusinessObjects.Exceptions;
	using System.Runtime.Serialization;

	public class ExStarsFilingStatusListClass : List<ExStarsFilingStatusClass> { }

	public class ExStarsFilingStatusClass
	{
		#region Properties

		[DataMember]
		public DateTime FilingStartDate { get; set; }

		[DataMember]
		public DateTime FilingEndDate { get; set; }

		public DateTime ExpectedStartingDate { get { return FilingEndDate.Date.AddDays(1);}}

		[DataMember]
		public string ReportTypeAsStr
		{
			get
			{
				return this.ReportType.ToString();
			}
			set
			{
				//this.ReportType = (ReportTypeEnum)Enum.Parse(typeof(ReportTypeEnum), value);
				ReportTypeEnum rt;
				if (!ReportModifiersEnum.TryParse(value, false, out rt))
				{
					throw new ExStarsFilingException("Invalid value for ReportTypeAsString, valid values are StdMonthlyReport, OutgoingManger, IncomingManager");
				}
				this.ReportType = rt;

			}
		}

		public ReportTypeEnum ReportType { get; set; }

		[DataMember]
		public bool IncludeBeginningInventory { get; set; }

		[DataMember]
		public string ModifierAsStr
		{
			get
			{
				return this.Modifier.ToString();
			}
			set
			{
				ReportModifiersEnum rm;
				if (!ReportModifiersEnum.TryParse(value, false, out rm))
				{
					throw new ExStarsFilingException("\"{0}\" is an invalid value for ModifierAsString, valid values are Original, Replacement, Supplemental, Replaced", value);
				}
				this.Modifier = rm;
			}
		}

		public ReportModifiersEnum Modifier { get; set; }

		[DataMember]
		public string InterchangeControlNumber { get; set; }

		[DataMember]
		public string OriginalControlNumber { get; set; }

		[DataMember]
		public string TransSetControlNumber { get; set; }

		[DataMember]
		public string FilingStatusAsStr
		{
			get
			{
				return this.FilingStatus.ToString();
			}
			set
			{
				FileCreatingStatus status;
				if (!FileCreatingStatus.TryParse(value, false, out status))
				{
					throw new ExStarsFilingException("Invalid value for FilingStatusAsStr, valid values are Unknown, NotCreated, Working, FinishedNoErrors, FinishedWithErrors");
				}
				this.FilingStatus = status;
			}
		}

		public FileCreatingStatus FilingStatus { get; set; }

		[DataMember]
		public DateTimeOffset FilingCreated { get; set; }

		[DataMember]
		public DateTimeOffset FilingSent { get; set; }

		[DataMember]
		public DateTimeOffset ResponseLoaded { get; set; }

		public bool IsResponseLoaded { get {  return ResponseLoaded > ExStarsConstants.BeginningOfDateTimeOffset;} }
		 
		// a summary from ReportedErrors
		[DataMember]
		public int UnresolvedErrors { get; set; }

		// a summary from ReportedErrors
		[DataMember]
		public int UnresolvedWarnings { get; set; }

		[DataMember]
		public Guid ExStarsFilingsGuid { get; set; }

		[DataMember]
		public Guid ManagerCompanyGuid { get; set; }

		#endregion

		#region ExStarsFilingStatusClass Constructors


		public ExStarsFilingStatusClass()
		{
			this.UnresolvedErrors = 0;
			this.UnresolvedWarnings = 0;
		}

		public ExStarsFilingStatusClass(
			DateTime filingStartDate
			, DateTime filingEndDate
			, Guid managerCompanyGuid
			, string reportType
			, string modifier
			, string interchangeControlNumber
			, string transSetControlNumber
			, string originalControlNumber
			, string filingStatus
			, DateTimeOffset filingCreated
			, DateTimeOffset filingSent
			, DateTimeOffset responseLoaded
			, Guid exStarsFilingsGuid
			, int unresolvedErrors
			, int unresolvedWarnings )
			: this(
				filingStartDate
				, filingEndDate
				, managerCompanyGuid
				, (ReportTypeEnum)Enum.Parse(typeof(ReportTypeEnum), reportType)
				, (ReportModifiersEnum)Enum.Parse(typeof(ReportModifiersEnum), modifier)
				, interchangeControlNumber
				, transSetControlNumber
				, originalControlNumber
				, (FileCreatingStatus)Enum.Parse(typeof(FileCreatingStatus), filingStatus)
				, filingCreated
				, filingSent
				, responseLoaded
				, exStarsFilingsGuid
				, unresolvedErrors
				, unresolvedWarnings )
		{
		}

		public ExStarsFilingStatusClass(DateTime filingStartDate
				, DateTime filingEndDate
				, Guid managerCompanyGuid
				, ReportTypeEnum reportType
				, ReportModifiersEnum modifier
				, string interchangeControlNumber
				, string transSetControlNumber
				, string originalControlNumber
				, FileCreatingStatus filingStatus
				, DateTimeOffset filingCreated
				, DateTimeOffset filingSent
				, DateTimeOffset responseLoaded
				, Guid exStarsFilingsGuid
				, int unresolvedErrors
				, int unresolvedWarnings )
			: this()
		{
			this.FilingStartDate = filingStartDate;
			this.FilingEndDate = filingEndDate;
			this.ManagerCompanyGuid = managerCompanyGuid;
			this.ReportType = reportType;
			this.Modifier = modifier;
			this.InterchangeControlNumber = interchangeControlNumber;
			this.TransSetControlNumber = transSetControlNumber;
			this.OriginalControlNumber = originalControlNumber;
			this.FilingStatus = filingStatus;
			this.FilingCreated = filingCreated;
			this.FilingSent = filingSent;
			this.ResponseLoaded = responseLoaded;
			this.ExStarsFilingsGuid = exStarsFilingsGuid;
			this.UnresolvedErrors = unresolvedErrors;
			this.UnresolvedWarnings = unresolvedWarnings;

		}
		
		#endregion

		public override string ToString()
		{
			return string.Format("{0} {1} {2}", this.FilingEndDate.ToString("yyyyMMdd"), this.InterchangeControlNumber, this.ModifierAsStr);
		}
	}


	public class ExStarsFilingListClass : List<ExStarsFilingClass> { }

	public class ExStarsFilingClass : ExStarsFilingStatusClass
	{
		#region Properties

		[DataMember]
		public string RawDataFileName  { get; set; }

		[DataMember]
		public string EasyReadFileName  { get; set; }

		[DataMember]
		public Guid SiteGuid { get; set; }

		[DataMember]
		public string EdiReport { get; set; }

		[DataMember]
		public string EasyReadReport { get; set; }

		[DataMember]
		public string SerializedData { get; set; }

		[DataMember]
		public string Acknowledgement { get; set; }

		/// <summary>
		/// This is the acknowledgement from the IRS merged with the data from EDI report 
		/// and formatted nicely.
		/// </summary>
		[DataMember]
		public string AckEasyRead { get; set; }

		#endregion

		#region ExStarsFilingClass Constructors

		public ExStarsFilingClass()
		{
		}


		public ExStarsFilingClass(DateTime filingStartDate
					, DateTime filingEndDate
					, Guid managerCompanyGuid
					, Guid siteGuid
					, string reportType
					, bool includeBeginningInventory
					, string modifier
					, string interchangeControlNumber
					, string transSetControlNumber
					, string originalControlNumber
					, string filingStatus
					, string ediFilePath
					, string easyReadFilePath
					, DateTimeOffset filingCreated
					, DateTimeOffset filingSent
					, DateTimeOffset responseLoaded
					, string ediReport
					, string easyReadReport
					, string serializedData
					, Guid exStarsFilingsGuid)
			: this(
				filingStartDate
				, filingEndDate
				, managerCompanyGuid
				, siteGuid
				, (ReportTypeEnum)Enum.Parse(typeof(ReportTypeEnum), reportType)
				, includeBeginningInventory
				, (ReportModifiersEnum)Enum.Parse(typeof(ReportModifiersEnum), modifier)
				, interchangeControlNumber
				, transSetControlNumber
				, originalControlNumber
				, (FileCreatingStatus)Enum.Parse(typeof(FileCreatingStatus), filingStatus)
				, ediFilePath
				, easyReadFilePath
				, filingCreated
				, filingSent
				, responseLoaded
				, ediReport
				, easyReadReport
				, serializedData
				, exStarsFilingsGuid)
		{
		}


		public ExStarsFilingClass(DateTime filingStartDate
					, DateTime filingEndDate
					, Guid managerCompanyGuid
					, Guid siteGuid
					, ReportTypeEnum reportType
					, bool includeBeginningInventory
					, ReportModifiersEnum modifier
					, string interchangeControlNumber
					, string transSetControlNumber
					, string originalControlNumber
					, FileCreatingStatus filingStatus
					, string ediFilePath
					, string easyReadFilePath
					, DateTimeOffset filingCreated
					, DateTimeOffset filingSent
					, DateTimeOffset responseLoaded
					, string ediReport
					, string easyReadReport
					, string serializedData
					, Guid exStarsFilingsGuid)
			: base
				(filingStartDate
					, filingEndDate
					, managerCompanyGuid
					, reportType
					, modifier
					, interchangeControlNumber
					, transSetControlNumber
					, originalControlNumber
					, filingStatus
					, filingCreated
					, filingSent
					, responseLoaded
					, exStarsFilingsGuid
					, 0
					, 0)
		{
			this.SiteGuid = siteGuid;
			this.IncludeBeginningInventory = includeBeginningInventory;
			this.EdiReport = ediReport;
			this.RawDataFileName = ediFilePath;
			this.EasyReadReport = easyReadReport;
			this.EasyReadFileName = easyReadFilePath;
			this.SerializedData = serializedData;
			this.ExStarsFilingsGuid = exStarsFilingsGuid;
		}

	}

	#endregion	
}




	



