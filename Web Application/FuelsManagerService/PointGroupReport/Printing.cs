using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Web;
using static FMBusinessObjects.DataObjects.PointGroupSchedule;

namespace FuelsManagerService.PointGroupReport
{
    static public class Printing
	{
		static public bool PrintReport(
			string printer,
			string paperName,
			short copies,
			bool landscape,
			Stream stream,
			ExportFileType fileType,
			out string errorMsg)
		{
			try
			{
				// Create the printer settings for our printer
				var printerSettings = new PrinterSettings
				{
					PrinterName = printer,
					Copies = copies,
				};

				// Create our page settings for the paper size selected
				var pageSettings = new PageSettings(printerSettings)
				{
					Margins = new Margins(0, 0, 0, 0),
					Landscape = landscape
				};

				foreach (PaperSize paperSize in printerSettings.PaperSizes)
				{
					if (paperSize.PaperName == paperName)
					{
						pageSettings.PaperSize = paperSize;
						break;
					}
				}

				stream.Seek(0, SeekOrigin.Begin);

				// Now print the PDF document
				if (fileType == ExportFileType.PDF || fileType == ExportFileType.CSV)
				{
					using (var document = PdfiumViewer.PdfDocument.Load(stream))
					{
						using (var printDocument = document.CreatePrintDocument())
						{
							printDocument.PrinterSettings = printerSettings;
							printDocument.DefaultPageSettings = pageSettings;
							printDocument.PrintController = new StandardPrintController();
							printDocument.Print();
						}
					}
				}

				errorMsg = "";
				return true;
			}
			catch (System.Exception e)
			{
				errorMsg = e.Message;
				return false;
			}
		}
	}
}