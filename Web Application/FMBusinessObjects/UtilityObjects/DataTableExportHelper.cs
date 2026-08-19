namespace FMBusinessObjects.UtilityObjects
{
	using System;
	using System.Data;
	using System.Web;

	public class DataTableExportHelper
	{
		private DataSet DataSet { get; set; }

		public DataTableExportHelper(DataSet dataSet)
		{
			this.DataSet = dataSet;
		}

		public void ExportData(string dataFormat, string reportName)
		{
			if (GetDataTable(this.DataSet) == null)
			{
				throw new ApplicationException("Export data table is null.");
			}

			switch (dataFormat)
			{
				case "CSV":
					CreateCsvFile.CreateCsvDocument(this.DataSet, reportName + ".csv");
					break;
				case "Excel":
					CreateExcelFile.CreateExcelDocumentAsStream(this.DataSet, reportName + ".xlsx");
					break;
				case "PDF":
					CreatePdfFile.CreatePdfDocumentAsStream(this.DataSet, reportName + ".pdf");
					break;
				case "Word":
					CreateWordFile.CreateWordDocumentAsStream(this.DataSet, reportName + ".doc");
					break;
			}
		}

		private static DataTable GetDataTable(DataSet dataSet)
		{
			if (dataSet != null && dataSet.Tables != null && dataSet.Tables[0] != null)
			{
				return dataSet.Tables[0];
			}
			return null;
		}
	}
}