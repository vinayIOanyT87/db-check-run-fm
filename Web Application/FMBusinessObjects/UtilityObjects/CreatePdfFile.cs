namespace FMBusinessObjects.UtilityObjects
{
	using System;
	using System.Data;
	using System.Diagnostics;
	using System.IO;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Security.Cryptography;
	using System.Security.Cryptography.X509Certificates;

	using iTextSharp.text;
	using iTextSharp.text.html.simpleparser;
	using iTextSharp.text.pdf;

	using Crypt;

	public class CreatePdfFile
	{
		/// <summary>
		/// Adapated from http://www.aspsnippets.com/Articles/Export-DataSet-or-DataTable-to-Word-Excel-PDF-and-CSV-Formats.aspx
		/// </summary>
		public static void CreatePdfDocumentAsStream(DataSet ds, string filename)
		{
			try
			{
				HttpResponse response = System.Web.HttpContext.Current.Response;
				response.ClearContent();
				response.Clear();
				response.Buffer = true;
				response.Charset = "";
				response.Cache.SetCacheability(HttpCacheability.NoCache);
				response.AddHeader("content-disposition", "attachment; filename=" + filename);
				response.ContentType = "application/pdf";

				var gridView = new GridView { AllowPaging = false, DataSource = ds };
				gridView.DataBind();

				var sw = new StringWriter();
				var hw = new HtmlTextWriter(sw);
				gridView.RenderControl(hw);
				var sr = new StringReader(sw.ToString());
				var pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
				pdfDoc.SetPageSize(PageSize.A4.Rotate());

#pragma warning disable 612
				var htmlparser = new HTMLWorker(pdfDoc);
#pragma warning restore 612
				
				PdfWriter pw = PdfWriter.GetInstance(pdfDoc, response.OutputStream);
				Random rnd = new Random(DateTime.Now.Millisecond);
				string ownerPassword = rnd.Next(1000000000).ToString()+"asfGte573Kj*&^^%kwewqfaDssy";
				pw.SetEncryption(PdfWriter.ENCRYPTION_AES_256, "", ownerPassword, PdfWriter.ALLOW_PRINTING);
				
				pdfDoc.Open();
				htmlparser.Parse(sr);
				pdfDoc.Close();
				
				response.Write(pdfDoc);
				System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
			}
			catch (Exception ex)
			{
				Trace.WriteLine("Failed, exception thrown: " + ex.Message);
			}
		}
	}
}
