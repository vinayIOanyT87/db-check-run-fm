namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using FMBusinessObjects.UtilityObjects;

	/*
	 * Order of status: Unknown->Created->Submitted->FinishedWithErrors->Replaced or Corrected
	 *                                             ->FinishedNoErrors
	 */

	public enum FileCreatingStatus : byte { Unknown, NotCreated, Created, Submitted, FinishedNoErrors, FinishedWithErrors, Replaced, Corrected }
	public enum ReportTypeEnum : byte { Undefined, StdMonthly, OutgoingManger, IncomingManager, Acknowledgement }
	public enum ReportModifiersEnum : byte { Undefined, Original, Replacement, Supplemental, Correction, Replaced, AllTypes }
	public enum ExStarsErrorSource: byte { Unknown, Config, Site, Company, Equipment, EquipmentType, Transaction, UserSelection }
	public enum ExStarsFileFormat: byte { unknown, edi, easyread, errorReport } 

	// ReSharper disable InconsistentNaming

	// C_ExSTARS_X12_Terminal_Operator_Report::Generate_Segment() ~163
	public enum SegmentStateEnum
	{
		Begin,
		RelationshipToTheInformation,
		SequenceErrorToIdNumberRtti,
		NoBusinessActivity,
		DateOfTransfer,
		EndingInventoryDate,
		EndingInventoryLoopByProductCode,
		BeginTaxInformation,
		InformationFilingPeriod,
		VersionControlForIrsEdiMap,
		TotalGallonsReported,
		LineItemControlNumber,
		InformationProviderNameDetail,
		AdditionalInformationProviderNameDetail,
		InformationProviderAddressDetail,
		CityStateZipCodeDetail,
		InformationProviderContactPersonInformationDetail,
		Finish,
		Abort,
		PositionHolderFein,
		Option1OriginTerminal,
		CarrierInformation,
		TwoPartyExchange,
		Option1DestinationTerminal,
		Option2DestinationState,
		ShippingDocumentNumber,
		SequenceErrorIdNumberSdn,
		ShippingDocumentDate,
		ShippingVesselName,
		ShippingDocumentNetGallons,
		ShippingDocumentGrossGallons
	}


	/// <summary>
	/// Defined pg 5, 34
	/// </summary>
	public enum TransactionSetEnum { TS813, TS151, TS997 };

	/*
	     ref: FD-Publ 3536-Motor Fuel Excise Tax EDI Guide (ExSTARS) Table 2 
	     – List of Attribute Conventions 2 p 30
	Nn – Numeric Numeric type data element is symbolized by the two-position
		representation (Nn). N indicates a numeric, and n indicates the
		decimal places to the right of a fixed, implied decimal point. N0 (N
		Zero) is a numeric with no decimal places.
		Data Element Type
	R – Decimal (Real) The decimal point is optional for integer values, but is required for
		fractional values. For negative values, the leading minus (-) sign is
		used. Absence of this sign indicates a positive value. The plus (+)
		sign should not be transmitted.
	ID – Identifier An identifier data element must always contain a value from a
		predefined list of values that is maintained by ASC X12 or by other
		bodies that are recognized by ASC X12.
	AN – String A string (Alphanumeric) is a sequence of any characters from the basic
		or extended character sets. It must contain at least one nonspace
		character. The significant characters must be left justified. Leading
		spaces, if any, are assumed to be significant. Trailing spaces should be
		suppressed.
	DT – Date Format for the date type is CCYYMMDD. CC is the century digits of
		the year (ex. 19, 20). YY is the last 2 digits of the year (00-99), MM is
		the numeric value of the month (01-12), and DD for the day (01-31).
	TM – Time Format for the time type is HHMMSS, expressed in 24-hour clock
		format. HH is the numeric value for hour (00-23), MM for minute (00-
		59), and SS for second (00-59).
	 */
 
	/// <summary>
	/// The enumeration constants belows match the constants defined in:
	/// FD-Publ 3536-Motor Fuel Excise Tax EDI Guide (ExSTARS)  page 30, Table 2
	/// and used hundreds of times within the document. 
	/// Although these constant may seem cryptic to someone who is not viewing the  FD-353, renaming them 
	/// would make verification of the code vs the document very difficult. 
	/// </summary>
	public enum EnumExStarsElementTypes { none, N, N0, R, ID, AN, DT, TM, MultiPart, dontValidate, undefined };
	/*
	 *  M Mandatory data element - This element is required to appear in the segment.
		O Optional data element - The appearance of this data element is at the option of the
		   sending party or is based on a mutual agreement of the interchange parties.
		X Relational data element - Relational conditions may exist between two or more data
		  elements. If one is present the other/s is required. The relational condition is displayed
		  under the Syntax Noted of the X12 Standards.
	   Z  Designator A data element within a segment may have a designator (Z) that indicates the
		  existence of a semantic note. Semantic notes are considered part of the
		  standard. If a condition designator and a semantic note both affect a single
		 data element, the condition will appear first, separated from the semantic note
         designator by a vertical bar (|). Semantic notes that are general in nature are
         identified by the number 00 to the left of the comment.
	 */
	public enum EnumExStarsRequired { M, O, X, MZ, OZ, unknown };

	public enum ExStarsInventoryStatus { undefined, noActivity, hasInventory, noInventory };

	public enum VolumeMeasurement { Gross, Net };

	public enum MeasurementBeingTaxed { TotalNetReported = 5001, NetPhysicalInventory = 5002,   TotalNetTransported = 5004, Net = 5005, Gross = 5006 }

	// these assigned numbers are meaningless
	public enum EnumExStarsTrxType { Undefined=-1, BeginningInventory = 8, EndingInventory = 999, Receipt = 9, Issue = 5, Defuel = 4, BulkIssue = 3, BrokerReceipt = 29, BrokerDisbursement = 25, Adjustment = 2 };

	public class ExStarsErrorsAndWarningsList : SortedList<string, string> { };

	public class ExStarsConstants
	{
		// SqlDateTime must be between 1/1/1753 12:00:00 AM and 12/31/9999 11:59:59 PM.
		public static readonly DateTimeOffset BeginningOfDateTimeOffset =  TimeConverter.MinFMDate; //new DateTimeOffset(BeginningOfDateTime);
		public static readonly DateTime BeginningOfDateTime = new DateTime(BeginningOfDateTimeOffset.Year, BeginningOfDateTimeOffset.Month, BeginningOfDateTimeOffset.Day);

		public const string SubElementSeparator = "^";

		public const string BTA01_Accepted = "AT";
		public const string BTA01_AcceptedWithWarnings = "AD";
		public const string BTA01_Rejected = "RD";


		public const string BTI01 = "T6";
		public const string BTI02 = "050";
		public const string BTI03 = "47";
		public const string BTI07 = "24";
		public const string BTI09 = "49";
		public const string BTI11 = "48";
		public const string BTI13_Original = "00";
		// not used: public const string BTI13_Resubmission = "15";
		public const string BTI14_Replacement = "6R";
		public const string BTI14_Supplemental = "6S";
		public const string BTI14_Corrected = "CO";

		// pg 71
		/// <summary> 194 </summary>
		public const string DTM01_TaxPeriodEndDate = "194";
		/// <summary> 095 </summary>
		public const string DTM01_BillOfLadingDate = "095";
		public const string DTM01_DatePropertySold = "572";
		public const string DTM01_DatePropertyAquired = "631";
		public const string DTM01_InventoryDate = "184";

		public const string FGS01_BeginningInventory = "BI";
		public const string FGS01_EndingInventory = "EI";
		public const string FGS01_GainsLosses = "GL";
		/// <summary> Bill of Lading = Ticket number </summary>
		public const string FGS02_BillOfLading = "BM";
		public const string FGS01_ScheduleDetail = "D";
		public const string FGS02_ProductGroup = "PG";

		public const string ISA01_AuthorizationInfoQualifier = "03";
		public const string ISA03_Password = "01";
		public const string ISA05_IdQualifier = "ZZ";
		public const string ISA05_EIN = "32";
		public const string ISA07_DunsNumber = "01";
		public const string ISA14_NoAckRequired = "0";
		public const string ISA15_ProductionData = "P";
		public const string ISA15_TestData = "T";


		public const string GS01_TransactionSet813 = "TF";
		public const string GS07_ResponsibleAgencyCode = "X";

		public const string N101_InformationProvider = "L9";
		public const string N101_Carrier = "CA";
		public const string N101_OriginTerminal = "OT";
		public const string N101_PositionHolder = "ON";
		public const string N101_Exchanger = "EC";

		public const string N101_ShipTo = "ST";
		public const string N101_DestinationTerminal = "DT";
		public const string N101_VesselName = "FV";
		public const string N103_EIN = "24";
		public const string N103_IrsFacilityCode = "TC";
		public const string N103_TransportShipperCode = "TS";

		public const string PBI02_NoActionRequired = "NA";
		public const string PBI02_CorrectionRequired = "CO";

		public const string PER01 = "CN";
		public const string PER03 = "TE";
		public const string PER05 = "FX";
		public const string PER07 = "EM";

		public const string REF01_SpecialProcessing = "SU";
		public const string REF01_SequenceNumber = "55";
		public const string REF01_LineItemControlNumber = "FJ";

		public const string REF03_CorrectRecord = "00001";
		//public const string REF03_DeleteRecord = "00003";
		public const string REF0401_SpecialApproval = "S0";

		public const string ST01_TransactionSetCode = "813";
		public const string ST03_IrsExstarsImplimentationConvension = "0200";

		public const string TIA01_Gallons = "GA";

		/// <summary> T3 </summary>
		public const string TFS01_TaxScheduleCode = "T3";
		/// <summary> 15A </summary>
		public const string TFS02_TerminalReceipts = "15A";
		/// <summary> 15B </summary>
		public const string TFS02_TerminalDisbursements = "15B";
		/// <summary> 94 </summary>
		public const string TFS05_IdentCodeQualifier = "94";
		/// <summary> RS </summary>
		public const string TFS06_DeliveryVehicle_GSE = "RS";
		public const string TFS06_SummaryReporting = "CE";
		public const string TFS06_DeliveryVehicle_Truck = "J ";



		public static string ToString(EnumExStarsTrxType type)
		{
			// some values will be non-standard
			switch (type)
			{
				case EnumExStarsTrxType.BulkIssue:
					return "Bulk Issue";
				default:
					return type.ToString();
			}
		}


		public static string ToString(ReportTypeEnum reportType)
		{
			switch (reportType)
			{
				case ReportTypeEnum.StdMonthly:
					return "Standard Monthly";
				case ReportTypeEnum.OutgoingManger:
					return "Outgoing Manager";
				case ReportTypeEnum.IncomingManager:
					return "Incoming Manager";
				default:
					return reportType.ToString();
			}			
		}


		public static string RoundGallons(double gallonsFuel)
		{
			const string Fmt = "F0";
			// fractional gallon amounts are not accepted
			// FD-Publ 3536-Motor Fuel Excise Tax EDI Guide-09
			// Rev 11-2005, page 7
			return Math.Round(gallonsFuel, 0, MidpointRounding.AwayFromZero).ToString(Fmt);
		}

		public static string PrependDefaultPathToFileName(string fileNameNoPath)
		{
			return  Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
				, "Varec"
				, "ExSTARS"
				, fileNameNoPath);
		}
	}


}