namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using FMBusinessObjects.DataObjects;
	[Serializable]
	public class ExStarsReportsBaseList : List<ExStarsReportsBase> { }
	[Serializable]
	public abstract class ExStarsReportsBase : ExStarReportAndSegmentElementBase
	{
		// For a session of creating an EDI report there should be only a single sequence of RefSequenceNumber
		public string ValidationErrors { get { return _ValidationErrors.ToString(); } }
		public bool IsValid { get { return ValidationErrors.Length == 0; } }
		public SegmentList SegmentList { get; protected set; }
		public new int EstimatedTextLength
		{
			get
			{
				return (from segment in this.SegmentList
						select segment.EstimatedTextLength).Sum();
			}
		}

		protected StringBuilder _ValidationErrors = new StringBuilder();
		protected ExStarsSiteConfigExpanded Config = null;

		protected string Description;

		/// <summary>
		/// Required by serialization, do not use this
		/// </summary>
		protected ExStarsReportsBase()
		{
			this.Config = null;
			this.SegmentList = new SegmentList();
			this.Description = "";			
		}

		protected ExStarsReportsBase(ExStarsSiteConfigExpanded config, string description, ref string validationErrors) :this()
		{
			this.Config = config;
			this.Description = description;
			// that space before the \n prevents the line from being discarded later
			this.SegmentList.Add(new ExStarsComment(" \nBEGIN " + description));
			// the inheriting class needs to assign validationErrors a value after processing
		}

		/// <summary>
		/// Create a TIA segment
		/// </summary>
		/// <param name="measurementBeingTaxed">TIA01 segment value</param>
		/// <param name="volumeBeingTaxed">quantity</param>
		/// <param name="totalNetReportedTiaItems"> this value is updatedonly for Net and NetPhysicalInventory</param>
		/// <returns></returns>
		protected ExStarsAmountSegment CreateAmountSegment(MeasurementBeingTaxed measurementBeingTaxed, double volumeBeingTaxed, ref double totalNetReportedTiaItems)
		{
			//
			// "The Tax Information and Amount (TIA) is a required segment for original replacement 
			// or supplemental filing but should not be provided in a correction."
			// FD-Publ 3536,  pg 179
			//
			if (this.Config.IsNotCorrectionOrHasReferencedError())
			{
				// ref ExSTARS_X12_Base_Segments.cpp ~3422  C_ExSTARS_X12_Base_Segments::get_lfTotalNetGallons()
				if (measurementBeingTaxed == MeasurementBeingTaxed.Net || measurementBeingTaxed == MeasurementBeingTaxed.NetPhysicalInventory)
				{
					totalNetReportedTiaItems += volumeBeingTaxed;
				}
				
				return new ExStarsAmountSegment(measurementBeingTaxed, volumeBeingTaxed);
			}
			return null;
		}

		protected ExStarsSegment CreateSequenceErrorToIdNumberRtti()
		{
			Config.IncSequenceNumber();
			// ref C_ExSTARS_X12_Base_Segments::Generate_Sequence_Error_ID_Number_Segment() ~ 679
			if (!this.Config.IsNotCorrectionOrHasReferencedError())
			{
				return null;
			}

			// ref page 72
			ExStarsSegment segment = new ExStarsSegment("REF", "Reference Identification");
			string formattedSeqNum = ExStarsSegment.PadLeft0(Config.SequenceNumber, 20);
			segment.AddElement(1, "Reference Identification Qualifier", "55 = Sequence Number, Use of this code is required.", EnumExStarsElementTypes.ID, 2, 2, ExStarsConstants.REF01_SequenceNumber);
			segment.AddElement(2, "Reference Identification", "unique filer assigned sequence number.", EnumExStarsElementTypes.AN, 1, 20, formattedSeqNum);
			// C_ExSTARS_X12_Base_Segments::Generate_Sequence_Error_ID_Number_Segment() ~ 771
			// ref pg 108
			if (this.Config.ReportModifier == ReportModifiersEnum.Correction)
			{
				segment.AddElement(3, "Description", "Error Response Code, Use only when responding to errors, 00001  = Record corrected", EnumExStarsElementTypes.AN, 1, 5, ExStarsConstants.REF03_CorrectRecord);				
			}
			return segment;
		}


		public void MarkEnd()
		{
			// that space before the \n prevents the line from being discarded later
			this.SegmentList.Add(new ExStarsComment(" \nEND " + this.Description));
		}


		public string ToStringEdi()
		{
			return ToStringEdi(false);
		}

		public string ToStringEasyRead()
		{
			return ToStringEdi(true);
		}

		public virtual string ToStringEdi(bool outputEasyRead)
		{
			var len = (from segment
					   in this.SegmentList
					   where segment is ExStarsComment
					   select (segment as ExStarsComment).EstimatedTextLength).Sum();

			StringBuilder edi = new StringBuilder(len);
 
			foreach (ExStarReportAndSegmentElementBase segment in this.SegmentList)
			{
				if (segment == null)
				{
					continue;
				}
				// the order of the IF's is important, do the base classes last
				if (segment is ExStarsSegment)
				{
					edi.Append((segment as ExStarsSegment).ToStringEdi(outputEasyRead));
				}
				else if (segment is ExStarsReportsBase)
				{
					edi.Append((segment as ExStarsReportsBase).ToStringEdi(outputEasyRead));
				}
				else if (segment is ExStarsComment)
				{
					edi.Append((segment as ExStarsComment).ToStringEdi(outputEasyRead));
					if (outputEasyRead)
					{
						edi.AppendLine();
					}
				}
			}

			return edi.ToString();
		}


		public void AppendMessage(string fmt, params object[] args)
		{
			_ValidationErrors.AppendLine(string.Format(fmt, args));
		}

	}
}
