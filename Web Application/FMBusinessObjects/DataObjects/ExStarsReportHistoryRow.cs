using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects
{
	using System.Runtime.Serialization;

	[Serializable]
	[CollectionDataContract]
	public class ExStarsReportHistoryList : List<ExStarsReportHistoryRow>
	{
		public ExStarsReportHistoryList()
			: base()
		{
			
		}
		public ExStarsReportHistoryList( ExStarsFilingStatusListClass statusList, CompanyClass managerObject)
			: this()
		{
			foreach (var status in statusList)
			{
				this.Add( new ExStarsReportHistoryRow( status, managerObject));
			}
		
		}
	}


	[Serializable]
	public class ExStarsReportHistoryRow : IComparable
	{
		public string Manager { get; protected set; }
		public string StartDate { get; protected set; }
		public string EndDate { get; protected set; }
		public string ReportType { get; protected set; }
		public string Modifier { get; protected set; }
		public string CtrlNumber { get; protected set; }
		public string OrigCtrlNumber { get; protected set; }
		public string CreatedDate { get; protected set; }
		public string ErrorCount { get; protected set; }
		public string WarningCount { get; protected set; }
		public string ResponseLoaded { get; protected set; }
		public string FilingsGuidAsStr { get; protected set; }


		public ExStarsReportHistoryRow(ExStarsFilingStatusClass statusRow, CompanyClass managerObject)
		{
			this.Manager = managerObject.Name;
			this.StartDate = statusRow.FilingStartDate.ToString("yyyy-MM-dd");
			this.EndDate = statusRow.FilingEndDate.ToString("yyyy-MM-dd");
			this.ReportType = ExStarsConstants.ToString(statusRow.ReportType);
			this.Modifier = statusRow.ModifierAsStr;
			this.CtrlNumber = statusRow.TransSetControlNumber;
			this.OrigCtrlNumber = statusRow.OriginalControlNumber;
			this.ErrorCount = string.Format("{0:####}", statusRow.UnresolvedErrors);
			this.WarningCount = string.Format("{0:####}", statusRow.UnresolvedWarnings);
			this.ResponseLoaded = statusRow.ResponseLoaded > ExStarsConstants.BeginningOfDateTime? "Y" : "N";
			this.CreatedDate = statusRow.FilingCreated.ToString("yyyy-MM-dd HH:mm");
			this.FilingsGuidAsStr = statusRow.ExStarsFilingsGuid.ToString();
		}

		public int CompareTo(object obj)
		{
			ExStarsReportHistoryRow row = obj as ExStarsReportHistoryRow;
			if (null == row)
			{
				return 0;
			}
			int c1 = this.Manager.CompareTo(row.Manager);
			int c2 = this.StartDate.CompareTo(row.StartDate);
			int c3 = this.EndDate.CompareTo(row.EndDate);
			int c4 = this.OrigCtrlNumber.CompareTo(row.OrigCtrlNumber);
			int c5 = this.CtrlNumber.CompareTo(row.CtrlNumber);
			int c6 = this.ReportType.CompareTo(row.ReportType);
			int c7 = this.CreatedDate.CompareTo(row.CreatedDate);

			return c1 != 0
				       ? c1
				       : c2 != 0
					         ? c2
					         : c3 != 0
						           ? c3
						           : c4 != 0
							             ? c4
							             : c5 != 0
								               ? c5
								               : c6 != 0
									                 ? c6
									                 : c7;
		}

		public override bool Equals(object obj)
		{
			ExStarsReportHistoryRow row = obj as ExStarsReportHistoryRow;
			if (null == row)
			{
				return false;
			}
			bool c1 = this.Manager.Equals(row.Manager);
			bool c2 = this.StartDate.Equals(row.StartDate);
			bool c3 = this.EndDate.Equals(row.EndDate);
			bool c4 = this.OrigCtrlNumber.Equals(row.OrigCtrlNumber);
			bool c5 = this.CtrlNumber.Equals(row.CtrlNumber);
			bool c6 = this.ReportType.Equals(row.ReportType);
			bool c7 = this.CreatedDate.Equals(row.CreatedDate);

			return c1 && c2 && c3 && c4 && c5 && c6 && c7;
		}

		public override int GetHashCode()
		{
			return this.Manager.GetHashCode() ^
			       this.StartDate.GetHashCode() ^
			       this.EndDate.GetHashCode() ^
			       this.CreatedDate.GetHashCode() ^
			       this.ReportType.GetHashCode() ^
			       this.CtrlNumber.GetHashCode() ^
			       this.OrigCtrlNumber.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{0};{1};{2};{3};{4};{5};{6};{7};",
				this.Manager,
				this.StartDate,
				this.EndDate,
				this.ReportType,
				this.Modifier,
				this.CtrlNumber,
				this.OrigCtrlNumber,
				this.CreatedDate
				);
		}
	}
}
