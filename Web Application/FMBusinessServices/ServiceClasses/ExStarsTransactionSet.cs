#pragma warning disable 0168, 0169,0414,0649
namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Text;
	using System.Text.RegularExpressions;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	[Serializable]
	public class ExStarsTransactionSet : ExStarsReportsBase
	{
		protected ExStarsTerminalOperatorReport TerminalOperatorReport ;
		protected ExStarsScheduleDetails ScheduleDetails;
		protected ExStarsSegment TransactionSetTrailerSegment;

		[Serializable]
		public class TextChunkList : List<string>
		{
		}

		/// <summary>
		/// Required for serialization, do not use
		/// </summary>
		public ExStarsTransactionSet() : base() { }

		/// <summary>
		/// Standard constructor
		/// </summary>
		/// <param name="config"></param>
		/// <param name="validationErrors"></param>
		/// <param name="allMgrTotals"></param>
		public ExStarsTransactionSet(ExStarsSiteConfigExpanded config
			, ref string validationErrors
			, ref ExStarsManagerTotals allMgrTotals)
			: base(config, "TRANSACTION SET", ref validationErrors)
		{
			// Unique number for this transaction set
			Config.TransSetControlNumber = ExStarsSegment.UniqueControlNumber();

			int segmentCount = 0;
			string terminalOperatorReportValidationErrors = "";
			// create terminalOperatorReport before adding the segments to the output, since we need the total of all
			// TIA~5002 and TIA~5005 volume values.  This is a place holder until everything else is done.
			// Ref:  C_ExSTARS_X12_Base_Segments::get_lfTotalNetGallons() ~3422
			double totalNetReportedTiaItems = 0.0;
			bool isNotOrig = (config.ReportModifier == ReportModifiersEnum.Replacement
			                  || config.ReportModifier == ReportModifiersEnum.Correction
			                  || config.ReportModifier == ReportModifiersEnum.Supplemental);
			StringBuilder headerComment = new StringBuilder();
			headerComment.AppendFormat("Trans Set Control Number:    {0}\n", Config.TransSetControlNumber);
			headerComment.AppendFormat("Tax Payer:                   {0}\n", this.Config.AbbreviatedProviderName);
			if (isNotOrig)          
			{
				headerComment.AppendFormat(
					                   "For original Control Number: {0}\n", Config.OriginalTransSetControlNumber);
			}
			headerComment.AppendFormat("Submitted by:                {0}\n", this.Config.AuthorizationCode);

			SegmentList.Add( new ExStarsComment(headerComment.ToString()));
			// Number of segments included in the transaction set including SE and ST. The SE01 must reflect the accurate 
			// count of all segments in the transaction set beginning with the ST and ending with the SE. 
			// ref: IRS Publication 3536 Rev.11-2005 - p 88
			SegmentList.Add( this.TransactionSetHeader());
			SegmentList.Add( this.BeginTaxInformationSegment());
			//
			// Unlike other Tax Info Segments, and Date/Time segments these two always created for Correction reports
			// ref: C_ExSTARS_X12_Base_Segments::Generate_Date_Time_Segment() ~830
			//
			SegmentList.Add(new ExStarsDateTimeSegment(ExStarsConstants.DTM01_TaxPeriodEndDate, config.EndTransactionDateTime));
			// TIA 5001 = Net totals for Operator Reports, and schedule details
			ExStarsSegment totalReportedSegment = new ExStarsAmountSegment(MeasurementBeingTaxed.TotalNetReported, totalNetReportedTiaItems);
			const int NetTotalPosition = 4;
			ExStarsElement elementTia5001 = totalReportedSegment.ElementByIndex(NetTotalPosition);
			SegmentList.Add(totalReportedSegment);
			// ref C_ExSTARS_X12_Transaction_Set::Generate_Segment() case LINE_ITEM_CONTROL_NUMBER ~ 356
			if( isNotOrig) 
			{
				SegmentList.Add(SetRefOriginalControlNumberSegment());
			}
			SegmentList.AddRange(this.SetNameSegment());
			SegmentList.AddRange( this.SetAddress());
			SegmentList.Add(this.SetCityStZipSegment());
			SegmentList.Add(this.SetAdminCommContactSegment());
			// for EasyRead leave a blank line
			SegmentList.Add( new ExStarsSegment());
			// for EasyRead leave a blank line
			SegmentList.Add(new ExStarsSegment());
			SegmentList.Add(new ExStarsTerminalOperatorReport(config, ref segmentCount, ref totalNetReportedTiaItems, allMgrTotals, ref terminalOperatorReportValidationErrors));
			SegmentList.Add(new ExStarsSegment());
			string scheduleDetailstValidationErrors = "";
			SegmentList.Add(new ExStarsScheduleDetails(config, ref segmentCount, ref totalNetReportedTiaItems, allMgrTotals, ref scheduleDetailstValidationErrors));
			this.MarkEnd();
			// Generate_Schedule_Details: C_ExSTARS_X12_Document::Generate_Document() ~ 225
			SegmentList.Add( this.TransactionSetTrailer(SegmentList.CountInUse() + segmentCount));
			validationErrors += terminalOperatorReportValidationErrors + this.ValidationErrors;
			// Update the TotalNetVolume
			elementTia5001.Value = ExStarsConstants.RoundGallons(totalNetReportedTiaItems);
		}


		protected ExStarsSegment TransactionSetHeader()
		{
			ExStarsSegment headerSegment = new ExStarsSegment("ST", "Transaction Set Header");
			try
			{ 
				headerSegment.AddElement(1, "Transaction Set Code", "", EnumExStarsElementTypes.ID, 3, 3, ExStarsConstants.ST01_TransactionSetCode);
				headerSegment.AddElement(2, "Transaction Set Control Number", "", EnumExStarsElementTypes.AN, 4, 9, Config.TransSetControlNumber);
				headerSegment.AddElement(3, "Implementation Convention Reference", "", EnumExStarsRequired.OZ, EnumExStarsElementTypes.AN, 1, 5, ExStarsConstants.ST03_IrsExstarsImplimentationConvension);
			}
			catch (Exception e)
			{
				Config.AppendError(ExStarsErrorSource.Config, e.Message);
			}
			return headerSegment;
		}


		protected ExStarsSegment BeginTaxInformationSegment()
		{
			// ref  C_ExSTARS_X12_Transaction_Set::Generate_Begin_Tax_Information_Segment()
			// ref p 91,92
			// per report, this is only used once, ref p 36
			ExStarsSegment taxInformationSegment = new ExStarsSegment("BTI", "Beginning Tax Information");
			try
			{ 
				string currentDate = this.Config.ReportDateTime.ToString("yyyyMMdd");
				string nameControlId =
					this.Config.AbbreviatedProviderName.Substring(0, Math.Min(4, this.Config.AbbreviatedProviderName.Length)).PadRight(4, '*');
				taxInformationSegment.AddElement(1, "Reference Number Qualifier", "", EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.BTI01);
				taxInformationSegment.AddElement(2, "Reference Number", "", EnumExStarsElementTypes.AN, 3, 3, ExStarsConstants.BTI02);
				taxInformationSegment.AddElement(3, "ID Code Qualifier", "", EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.BTI03);
				taxInformationSegment.AddElement(4, "ID Code", "IRS Headquarters DUNS number", EnumExStarsElementTypes.AN, 9, 9, this.Config.DunsNumber);
				taxInformationSegment.AddElement(5, "Transaction Create Date", "", EnumExStarsRequired.O, EnumExStarsElementTypes.DT, 8, 8, currentDate);
				taxInformationSegment.AddElement(6, "Name Control ID", "", EnumExStarsRequired.O, EnumExStarsElementTypes.AN, 4, 4, nameControlId);
				taxInformationSegment.AddElement(7, "ID Code Qualifier", "24 = FEIN.  34 = SSN#", EnumExStarsRequired.X, EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.BTI07);
				taxInformationSegment.AddElement(8, "ID Code", "Informations FEIN or SSN#", EnumExStarsRequired.X, EnumExStarsElementTypes.AN, 9, 18, this.Config.FeinCode);
				taxInformationSegment.AddElement(9, "ID Code Qualifier", "49 = State (assigned) Identification Number", EnumExStarsRequired.M, EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.BTI09);
				taxInformationSegment.AddElement(10, "ID Code", "License or Permit Number", EnumExStarsRequired.X, EnumExStarsElementTypes.AN, 2, 20, this.Config.ApplicationSendersCode);
				// ref ExSTARS Reporting Utility/ExSTARS_Export.cpp ~ 803  CExSTARS_ExportApp::GetBTI12Value() 
				// only report if 637 Rg ID is not blank
				if (!string.IsNullOrEmpty(this.Config.IRS_637Registration))
				{
					taxInformationSegment.AddElement(11, "ID Code", "48 = IRS Electronic Filer ID Number", EnumExStarsRequired.X, EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.BTI11);
					taxInformationSegment.AddElement(12, "ID Code", "Form 637 #= Registration Number (for Terminal Operator Only)", EnumExStarsRequired.X, EnumExStarsElementTypes.AN, 9, 18, this.Config.IRS_637Registration);
				}
				// page 41, either BTI13 or BTI14 never both
				// Use this element without BTI14. Use this element when transmitting the initial filing for a reporting period. 
				taxInformationSegment.AddElement(
					this.Config.ReportModifier == ReportModifiersEnum.Original? 13: 14
					, "Transaction Set Purpose Code", "", EnumExStarsRequired.O, EnumExStarsElementTypes.ID, 2, 2, this.Config.ReportModifierCode());
			}
			catch (Exception e)
			{
				Config.AppendError(ExStarsErrorSource.Config, e.Message);
			}

			return taxInformationSegment;
		}


		protected static TextChunkList SplitText(string fullName)
		{
			return SplitText("[ .,-]", fullName);
		}


		protected static TextChunkList SplitText( string delimiters, params string[] allText)
		{
			// names have to be split into chunks on 35 chars
			const int Maxsize = 35;
			Regex splitHere = new Regex(delimiters);
			TextChunkList chunks = new TextChunkList();
			foreach (string text in allText)
			{
				string fullText = text;
				while (fullText.Length > 0)
				{
					string newChunk;
					if (fullText.Length > Maxsize)
					{
						for (int splitSize = Maxsize; splitSize > 0; splitSize--)
						{
							if (splitHere.IsMatch(fullText.Substring(splitSize, 1)))
							{
								newChunk = fullText.Substring(0, splitSize);
								fullText = fullText.Substring(splitSize);
								goto exitForLoop;
							}
						}
						newChunk = fullText.Substring(0, Maxsize);
						fullText = fullText.Substring(Maxsize);
					}
					else
					{
						newChunk = fullText;
						fullText = "";
					}
					exitForLoop:
					chunks.Add(newChunk);
				}
			}
			if (chunks.Count == 0)
			{
				chunks.Add("");
			}
			return chunks;
		}


		public static void Test_SplitInfoProviderName()
		{
			TextChunkList n1 = SplitText(
				//----+----1----+----2---+----3---+----4---+----5---+----6---+----7---+----8---+----9---+----0---+----1---+----2---+----3---+----4----+----5
				 "Test_SplitInfoProviderName 35 chars maximum length test1 test1 test1 test1 test1 test1 test1 test1 test1 test1 test1 test1"
				);
			TextChunkList n2 = SplitText(
				//----+----1----+----2---+----3---+----4---+----5---+----6---+----7---+----8---+----9---+----0---+----1---+----2---+----3---+----4----+----5
				 "Test_SplitInfoProviderName 35-chars-maximum length test2 test2 test2 test2 test2 test2 test2 test2 test2 test2 test2 test2"
				);
			TextChunkList n3 = SplitText(
				//----+----1----+----2---+----3---+----4---+----5---+----6---+----7---+----8---+----9---+----0---+----1---+----2---+----3---+----4----+----5
				 "Test_SplitInfoProviderName 35-chars-maximum-length-test2-test2-test2 test2 test2 test2 test2 test2 test2 test2 test2 test2"
				);
			TextChunkList n4 = SplitText(
				//----+----1----+----2---+----3---+----4---+----5---+----6---+----7---+----8---+----9---+----0---+----1---+----2---+----3---+----4----+----5
				 "Test_SplitInfoProviderName 35 chars maximum-length-test3-test3 test3-test3-test3 test3 test3 test3 test3 test3 test3 test3"
				);
			TextChunkList n5 = SplitText(
				//----+----1----+----2---+----3---+----4---+----5---+----6---+----7---+----8---+----9---+----0---+----1---+----2---+----3---+----4----+----5
				 "Test_SplitInfoProviderName 35"
				);
		}

		protected ExStarsSegment SetRefOriginalControlNumberSegment()
		{
			//
			// REF02 is the original Transaction Set Control Number being corrected or amended; 
			// (i.e. the ST02 element of the original or replacement file that is being corrected or replaced). 
			// This must not be the same number as the Transaction
			// ref: IRS Publication 3536 Rev.11-2005 - p 180
			//
			ExStarsSegment seg = new ExStarsSegment("REF", "Ref to OriginalControlNumber");
			seg.AddElement(1, "Reference Identification Qualifier", "Used to return original Transaction Set Control Number of the original or replacement file containing the errors to be resolved.", EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.REF01_LineItemControlNumber);
			seg.AddElement(2, "Reference Identification Qualifier", "Trans Set COntrol number from original submission", EnumExStarsElementTypes.AN, 4, 9, Config.OriginalTransSetControlNumber);
			return seg;
		}

		protected SegmentList SetNameSegment()
		{
			SegmentList list = new SegmentList();
			if (!this.Config.IsNotCorrectionOrHasReferencedError())
			{
				return list;
			}
			ExStarsSegment nameSegment1 = new ExStarsSegment("N1", "Name");
			try
			{
				TextChunkList textChunkList = SplitText(this.Config.InfoProviderName);
				nameSegment1.AddElement(1, "Entity Identification Code", "", EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.N101_InformationProvider);
				if (textChunkList.Count == 0)
				{
					throw new ExStarsSegmentException("SetNameSegment() textChunkList[0]");
				}
				nameSegment1.AddElement(2, "Name", "", EnumExStarsElementTypes.ID, 1, 35, textChunkList[0]);
				list.Add(nameSegment1);
			
				if (textChunkList.Count > 1)
				{
					ExStarsSegment nameSegment2 = new ExStarsSegment("N2", "Additional Name Information");
					// this segment is not always present
					nameSegment2.AddElement(1, "Name", "Taxpayer Name", EnumExStarsRequired.M, EnumExStarsElementTypes.ID, 1, 35, textChunkList[1]);
					if (textChunkList.Count > 2)
					{
						nameSegment2.AddElement(2, "Name", "Taxpayer Name", EnumExStarsRequired.O, EnumExStarsElementTypes.ID, 1, 35, textChunkList[2]);
					}
					list.Add(nameSegment2);
				}
			}
			catch (Exception e)
			{
				Config.AppendError(ExStarsErrorSource.Config, e.Message);
			}
			return list;
		}


		protected SegmentList SetAddress()
		{
			SegmentList list = new SegmentList();
			if (!this.Config.IsNotCorrectionOrHasReferencedError())
			{
				return list;
			}
			ExStarsSegment addressSegment1 = new ExStarsSegment("N3", "Address Information");
			try
			{

				TextChunkList addressLines = SplitText(" ,;", this.Config.Manager.Address1, this.Config.Manager.Address2);

				int recallIdx = 0;
				addressSegment1.AddElement(1, "Address Information", "", EnumExStarsRequired.M, EnumExStarsElementTypes.AN, 1, 35, addressLines[recallIdx++]);
				list.Add(addressSegment1);
				if (addressLines.Count > 1)
				{
					addressSegment1.AddElement(2, "Address Information", "", EnumExStarsRequired.O, EnumExStarsElementTypes.AN, 1, 35, addressLines[recallIdx++]);
					if (addressLines.Count > 2)
					{
						ExStarsSegment addressSegment2 = new ExStarsSegment("N3", "Address Information");
						addressSegment2.AddElement(1, "Address Information", "", EnumExStarsRequired.M, EnumExStarsElementTypes.AN, 1, 35, addressLines[recallIdx++]);
						if (addressLines.Count > 3)
						{
							addressSegment2.AddElement(2, "Address Information", "", EnumExStarsRequired.O, EnumExStarsElementTypes.AN, 1, 35, addressLines[recallIdx]);
						}
						list.Add(addressSegment2);
					}
				}
			}
			catch (Exception e)
			{
				Config.AppendError(ExStarsErrorSource.Config, e.Message);
			}
			return list;
		}


		protected ExStarsSegment SetCityStZipSegment() 
		{
			if (!this.Config.IsNotCorrectionOrHasReferencedError())
			{
				return null;
			}
			// ref pg 68
			ExStarsSegment cityStZipSegment = new ExStarsSegment("N4", "Geographic Location");
			try
			{
				string city = this.Config.Manager.City.Length > 60
								  ? this.Config.Manager.City.Substring(0, 60)
								  : this.Config.Manager.City;
				string state = this.Config.Manager.State.Length > 2
								  ? this.Config.Manager.State.Substring(0, 2)
								  : this.Config.Manager.State;
				string zipcode = this.Config.Manager.Zip.Length > 11
								  ? this.Config.Manager.Zip.Substring(0, 11)
								  : this.Config.Manager.Zip;
				string country = this.Config.Manager.Country.Length > 3
								  ? this.Config.Manager.Country.Substring(0, 3)
								  : this.Config.Manager.Country;

				cityStZipSegment.AddElement(1, "City Name", "", EnumExStarsRequired.O,  EnumExStarsElementTypes.ID, 2, 60, city);
				cityStZipSegment.AddElement(2, "State code or province", "", EnumExStarsRequired.O, EnumExStarsElementTypes.ID, 2, 2, state);
				cityStZipSegment.AddElement(3, "Postal (zip) code", "", EnumExStarsRequired.O, EnumExStarsElementTypes.ID, 3, 11, zipcode);
				cityStZipSegment.AddElement(4, "Country", "", EnumExStarsRequired.O, EnumExStarsElementTypes.ID, 3, 3, country);
			}
			catch (Exception e)
			{
				Config.AppendError(ExStarsErrorSource.Config, e.Message);
			}
			return cityStZipSegment;
		}


		/// <summary>
		/// Ref p 46, No formatting, no special characters, digits only
		/// If the input number looks like 1-(800) 555-1212 x 123
		/// the returned value will be 18005551212
		/// </summary>
		/// <param name="phone">phone number with separators</param>
		/// <param name="maxLen">maximum output length</param>
		/// <returns></returns>
		protected string TrimPhone(string phone, int maxLen)
		{
			phone = phone.Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "").Replace(".", "").Replace("+", "");
			Regex nonDigit = new Regex("[^0-9]");
			Match match = nonDigit.Match(phone);
			if (match.Success)
			{
				phone = phone.Substring(0, match.Index);
			}

			if (phone.Length > maxLen)
			{
				return "";
			}
			return phone;
		}


		protected ExStarsSegment SetAdminCommContactSegment()
		{
			if (!this.Config.IsNotCorrectionOrHasReferencedError())
			{
				return null;
			}
			// At least one occurrence of this PER segment is required for each N1 loop in the transaction header.
			// ref pg 69
			ExStarsSegment adminCommContactSegment = new ExStarsSegment("PER", "Administrative Communications Contact, person or office to whom administrative communications should be directed");
			try
			{
				adminCommContactSegment.AddElement(1, "Functional Identifier Code", "N = General Contact, EA = EDI Coordiantor", EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.PER01);
				adminCommContactSegment.AddElement(2, "Application Sender's Code", "Contact Name", EnumExStarsRequired.O, EnumExStarsElementTypes.AN, 1, 35, this.Config.Manager.Contact1Name);
				string voicePhone = this.TrimPhone( this.Config.Manager.Contact1PhoneOffice, 14);
				string faxPhone = this.TrimPhone( this.Config.Manager.Contact1Fax, 10);
				string email = this.Config.Manager.Contact1EmailAddress.Length > 80
					? ""
					: this.Config.Manager.Contact1EmailAddress;
				if (voicePhone.Length >= 10)
				{
					adminCommContactSegment.AddElement(3, "Communications Number Qualifier", "TE = Telephone Number", EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.PER03);
					adminCommContactSegment.AddElement(4, "Voice Telephone Number", "", EnumExStarsElementTypes.AN, 10, 14, voicePhone);				
				}
				if (faxPhone.Length >= 10)
				{
					adminCommContactSegment.AddElement(5, "Communications Number Qualifier", "FX = FAX Telephone Number", EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.PER05);
					adminCommContactSegment.AddElement(6, "FAX Telephone Number", "", EnumExStarsElementTypes.AN, 10, 10, faxPhone);
				}
				if (email.Length > 0)
				{
					adminCommContactSegment.AddElement(7, "Communications Number Qualifier", "EM = Electronic Mail", EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.PER07);
					adminCommContactSegment.AddElement(8, "E-mail Address", "", EnumExStarsElementTypes.AN, 1, 80, email);
				}
			}
			catch (Exception e)
			{
				Config.AppendError(ExStarsErrorSource.Config, e.Message);
			}

			return adminCommContactSegment;
		}


		protected ExStarsSegment TransactionSetTrailer(int segmentCount)
		{
			ExStarsSegment trailerSegment = new ExStarsSegment("SE", "Transaction Set Trailer");
			try
			{
				// Count this segment too
				segmentCount++; 
				trailerSegment.AddElement(1, "Number of Included Segments", "", EnumExStarsElementTypes.N0, 1, 10, segmentCount.ToString(CultureInfo.InvariantCulture));
				trailerSegment.AddElement(2, "Transaction Set Control Number", "", EnumExStarsElementTypes.AN, 4, 9, Config.TransSetControlNumber);
			}
			catch (Exception e)
			{
				Config.AppendError(ExStarsErrorSource.Config, e.Message);
			}
			return trailerSegment;
		}
	}
}