
namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;
	using FMBusinessObjects.Exceptions;

	/// <summary>
	/// The configuration as found within the database for a single ExStars site for a single manager
	/// This class has data and validation, but lacks logic to populate itself
	/// </summary>
	[Serializable]
	[DataContract]
	public class ExStarsSiteConfigClass 
	{
		[DataMember]
		public Guid SiteGuid { get; set; }

		[DataMember]
		public Guid ManagerCompanyGuid { get; set; }

		/// <summary>
		/// ISA06
		/// </summary>
		[DataMember]
		public String InterchangeSenderId { get; set; }

		/// <summary>
		/// GS02
		/// </summary>
		[DataMember]
		public String ApplicationSendersCode { get; set; }

		/// <summary>
		/// ISA02 a user ID whose password is ISA04
		/// </summary>
		[DataMember]
		public String AuthorizationCode { get; set; }

		/// <summary>
		/// BTI12: Terminal operators only, blank for airports
		/// </summary>
		[DataMember]
		public String IRS_637Registration { get; set; }

		/// <summary>
		/// the identification number assigned by IRS to each approved terminal and published in the Federal Register; 
		/// used for reporting origin or destination on Forms 720-TO and 720-CS;
		/// </summary>
		[DataMember]
		public String TerminalControlNumber { get; set; }

		/// <summary>
		/// BTI08 = Beginning Tax Information
		/// Taxpayer’s EIN
		/// (Employer identification number): is a 9-digit number that IRS assigns in the following format: 
		/// 00-0000000. However, for employee plans, an alpha (for example, P) or the plan number (e.g., 003) may 
		/// follow the EIN. The IRS uses the number to identify taxpayers who are required to file various business 
		/// tax returns. EINs are used by employers, sole proprietors, corporations, partnerships, nonprofit associations, 
		/// trusts, estates of decedents, government agencies, certain individuals, and other business entities.
		/// </summary>
		[DataMember]
		public String FeinCode { get; set; }

		/// <summary>
		/// (ISA04): a 10-character code chosen by the Information Provider or Transmitter and submitted on its LOA
		///  or revised LOA. This code is used in each EDI transmission and becomes part of the Electronic Signature.
		/// </summary>
		[DataMember]
		public String SecurityCode { get; set; }

		[DataMember]
		public String InfoProviderName { get; set; }

		/// <summary>
		/// 4 characters is ideal. 2-4 is allowed
		/// </summary>
		[DataMember]
		public String AbbreviatedProviderName { get; set; }

		[DataMember]
		public String InterchangeControlNumber { get; set; }

		[DataMember]
		public String TransSetControlNumber { get; set; }

		/// <summary>
		/// Used for replacement, corrections, supplemental
		/// </summary>
		[DataMember]
		public String OriginalTransSetControlNumber { get; set; }


		public ExStarsSiteConfigClass()
		{
			this.OriginalTransSetControlNumber = "";
		}

		public void CopyFrom(ExStarsSiteConfigClass copyFrom)
		{
			this.SiteGuid = copyFrom.SiteGuid;
			this.ManagerCompanyGuid = copyFrom.ManagerCompanyGuid;
			this.InterchangeSenderId = copyFrom.InterchangeSenderId;
			this.ApplicationSendersCode = copyFrom.ApplicationSendersCode;
			this.AuthorizationCode = copyFrom.AuthorizationCode;
			this.IRS_637Registration = copyFrom.IRS_637Registration;
			this.TerminalControlNumber = copyFrom.TerminalControlNumber;
			this.FeinCode = copyFrom.FeinCode;
			this.SecurityCode = copyFrom.SecurityCode;
			this.InfoProviderName = copyFrom.InfoProviderName;
			this.AbbreviatedProviderName = copyFrom.AbbreviatedProviderName;
			this.InterchangeControlNumber = copyFrom.InterchangeControlNumber;
			this.TransSetControlNumber = copyFrom.TransSetControlNumber;
			this.OriginalTransSetControlNumber = copyFrom.OriginalTransSetControlNumber;
		}

	}
}
