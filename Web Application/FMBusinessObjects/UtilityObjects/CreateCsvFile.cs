using FMCore;

namespace FMBusinessObjects.UtilityObjects
{
	using System;
	using System.Data;
	using System.Diagnostics;
	using System.Web;

	public class CreateCsvFile
	{
		public static void CreateCsvDocument(DataSet ds, string filename)
		{
			try
			{
				HttpResponse response = System.Web.HttpContext.Current.Response;
				DataTable dataTable = ds.Tables[0];
				var csvData = dataTable.ToCsv();
				var bytes = csvData.GetBytes();

				response.Clear();
				response.ContentType = "text/csv";
				response.AddHeader("content-disposition", "attachment; filename=" + filename);
				response.BinaryWrite(bytes);
				response.Flush();
				response.Close();
				System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
			}
			catch (Exception ex)
			{
				Trace.WriteLine("Failed, exception thrown: " + ex.Message);
			}
		}
	}
}