namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Text;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// See ExSTARS Document: IRS Publication 3536 Rev.11-2005 - Page 34 
	/// </summary>
	[Serializable]
	public class ExStarsFunctionalGroup : ExStarsReportsBase
	{		
		protected string ControlGroupNumber;

		/// <summary>
		/// Required by serialization, do not use this
		/// </summary>
		public ExStarsFunctionalGroup() : base() { }

		/// <summary>
		/// Standard constructor
		/// </summary>
		/// <param name="config">access to the globally shared config</param>
		/// <param name="validationErrors">object is passed between levels for all to update</param>
		/// <param name="allMgrTotals">object is passed between levels for all to update</param>
		public ExStarsFunctionalGroup(ExStarsSiteConfigExpanded config, ref string validationErrors, ref ExStarsManagerTotals allMgrTotals)
			: base(config, "FUNCTIONAL GROUP SECTION", ref validationErrors)
		{
			this.ControlGroupNumber = ExStarsSegment.UniqueControlNumber();
			FunctionalGroupHeader();
			// Each submission can contain ... only one transaction set (ST/SE loop) within the functional group.
			// ref p 25
			string unused = "";
			this.SegmentList.Add(new ExStarsTransactionSet(config, ref unused, ref allMgrTotals));
			this.MarkEnd();
			FunctionalGroupTrailer();
			validationErrors += this.ValidationErrors;
		}


		protected void FunctionalGroupHeader()
		{
			try
			{
				string currentDate = this.Config.ReportDateTime.ToString("yyyyMMdd");
				string currentTime = this.Config.ReportDateTime.ToString("HHmmss");
				ExStarsSegment headerSegment = new ExStarsSegment("GS", "Functional Group Header Section");
				headerSegment.AddElement(1, "Functional Identifier Code", "TF = Electronic Filing of Tax Return Data", EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.GS01_TransactionSet813);
				headerSegment.AddElement(2, "Application Sender’s Code", "", EnumExStarsElementTypes.AN, 2, 15, this.Config.ApplicationSendersCode);
				headerSegment.AddElement(3, "Application Receiver’s Code", "040539587050 = IRS Exise - Motor Fuels Section", EnumExStarsElementTypes.AN, 2, 15, Config.GS03_ApplicationReceiversCode);
				headerSegment.AddElement(4, "Date", "", EnumExStarsRequired.MZ, EnumExStarsElementTypes.DT, 8, 8, currentDate);
				headerSegment.AddElement(5, "Time", "", EnumExStarsRequired.MZ, EnumExStarsElementTypes.TM, 4, 8, currentTime);
				headerSegment.AddElement(6, "Group Control Number", "Unique Random #", EnumExStarsRequired.MZ, EnumExStarsElementTypes.N0, 1, 9, this.ControlGroupNumber);
				headerSegment.AddElement(7, "Responsible Agency Code", "X = ASC X12", EnumExStarsRequired.M, EnumExStarsElementTypes.ID, 1, 2, ExStarsConstants.GS07_ResponsibleAgencyCode);
				headerSegment.AddElement(8, "Version/Release/Industry Identifier Code", "004030 = Draft Standards Approved for Publication 3536 Rev.11-2005", EnumExStarsRequired.M, EnumExStarsElementTypes.AN, 1, 12, Config.GS08_FuncGrpHdrVerReleaseIndustryIdCode);
				SegmentList.Add(headerSegment);
				SegmentList.Add(new ExStarsSegment());
			}
			catch (Exception e)
			{				
				Config.AppendError(ExStarsErrorSource.Config,  e.Message);
			}
		}


		protected void FunctionalGroupTrailer()
		{
			try
			{
				ExStarsSegment trailerSegment = new ExStarsSegment("GE", "Functional Group Trailer Section");
				// ref: IRS Publication 3536 Rev.11-2005 - p 35
				trailerSegment.AddElement(1, "Number Of Transaction Sets Included", "For ExSTARS, this should always be 1.", EnumExStarsElementTypes.N0, 1, 6, "1");
				trailerSegment.AddElement(2, "Group Control Number", "Same as GS06", EnumExStarsRequired.MZ, EnumExStarsElementTypes.N0, 1, 9, this.ControlGroupNumber);
				SegmentList.Add(trailerSegment);
			}
			catch (Exception e)
			{
				Config.AppendError(ExStarsErrorSource.Config, e.Message);
			}
		}


	}
}