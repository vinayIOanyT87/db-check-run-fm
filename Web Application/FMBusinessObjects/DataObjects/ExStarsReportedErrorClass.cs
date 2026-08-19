namespace FMBusinessObjects.DataObjects
{
	using System;
	using FMBusinessObjects.Exceptions;
	using System.Runtime.Serialization;

	public class ExStarsReportedErrorClass
	{
		public const string BlankSequenceNumber = "00000000000000000000";

		#region Properties

		[DataMember]
		public Guid ManagerCompanyGuid { get; set; }

		[DataMember]
		public Guid SiteGuid { get; set; }

		[DataMember]
		public Guid ExStarsFilingsGuid { get; set; }
		
		[DataMember]
		public string SequenceNumber { get; set; }

		[DataMember]
		public bool MustCorrect { get; set; }

		// ReSharper disable InconsistentNaming

		[DataMember]
		public string PBI01_Primary { get; set; }

		[DataMember]
		public string PBI01_Secondary { get; set; }

		[DataMember]
		public string PBI03_Primary { get; set; }

		[DataMember]
		public string PBI03_Secondary { get; set; }

		[DataMember]
		public string PBI04 { get; set; }

		[DataMember]
		public string OriginalValue { get; set; }

		[DataMember]
		public string IrsErrorText { get; set; }

		[DataMember]
		public bool ErrorCorrected { get; set; }

		[DataMember]
		public string CreatedBy { get; set; }

		[DataMember]
		public string UpdatedBy { get; set; }

		[DataMember]
		public Guid ExStarsReportedErrorsGuid { get; set; }

		#endregion

		#region Constructors

		public ExStarsReportedErrorClass() { }

		public ExStarsReportedErrorClass(
			SecurityClass security
			, Guid managerCompanyGuid
			, Guid siteGuid
			, Guid exStarsFilingsGuid
			, string sequenceNumber
			, ExStarsSegment pbiSegment)
		{
			if (!pbiSegment.Id.Equals("PBI"))
			{
				throw new ExStarsSegmentException("Not PBI Segment");
			}
			/*
			 * 	this.ManagerCompanyGuid = managerCompanyGuid;
			 * 	this.SiteGuid = siteGuid;
			 */
			this.SetAccessControlFields(security, managerCompanyGuid, siteGuid);
			bool mustBeCorrected = pbiSegment.ElementByIndex(2).Value.Equals(ExStarsConstants.PBI02_CorrectionRequired);

			string codePBI04 = pbiSegment.ElementByIndex(4).Value;
			// CPBISegmentProcessor::ProcessPBISegment() ~ 58
			// translate PBI04 codes
			if ("E0030" == codePBI04 || "E0028" == codePBI04)
			{
				codePBI04 = "E0011";
			}

			string irsErrorText = pbiSegment.ElementByIndex(6).Value;


			this.ExStarsFilingsGuid = exStarsFilingsGuid;
			this.SequenceNumber = sequenceNumber;
			this.MustCorrect = mustBeCorrected;
			this.PBI01_Primary = pbiSegment.ElementByIndex(1).Value.Left(4);
			this.PBI01_Secondary = pbiSegment.ElementByIndex(1).Value.Substring(4, 2);
			this.PBI03_Primary = pbiSegment.ElementByIndex(3).Value.Left(3);
			this.PBI03_Secondary = pbiSegment.ElementByIndex(3).Value.Substring(3, 1);
			this.PBI04 = codePBI04;
			this.OriginalValue = irsErrorText;
			this.IrsErrorText = irsErrorText;
			this.ErrorCorrected = false;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// This must be done so that the record can be written to the database
		/// </summary>
		/// <param name="security"></param>
		/// <param name="managerCompanyGuid"></param>
		/// <param name="siteGuid"></param>
		public void SetAccessControlFields(SecurityClass security, Guid managerCompanyGuid, Guid siteGuid)
		{
			this.ManagerCompanyGuid = managerCompanyGuid;
			this.SiteGuid = siteGuid;
			this.CreatedBy = security.UserID;
			this.UpdatedBy = security.UserID;
		}


		#endregion

	}
}
