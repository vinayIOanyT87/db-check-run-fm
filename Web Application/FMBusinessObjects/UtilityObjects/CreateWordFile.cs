namespace FMBusinessObjects.UtilityObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Diagnostics;
	using System.Web;

	using DocumentFormat.OpenXml;
	using DocumentFormat.OpenXml.Packaging;
	using DocumentFormat.OpenXml.Wordprocessing;

	public class CreateWordFile
	{
		public static void CreateWordDocumentAsStream(DataSet ds, string filename)
		{
			try
			{
				HttpResponse response = System.Web.HttpContext.Current.Response;
				var stream = new System.IO.MemoryStream();

				using(WordprocessingDocument wordDoc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
				{
					WriteWordFile(ds, wordDoc);
				}

				stream.Flush();
				stream.Position = 0;

				response.ClearContent();
				response.Clear();
				response.Buffer = true;
				response.Charset = "";

				//  NOTE: If you get an "HttpCacheability does not exist" error on the following line, make sure you have
				//  manually added System.Web to this project's References.

				response.Cache.SetCacheability(HttpCacheability.NoCache);
				response.AddHeader("content-disposition", "attachment; filename=" + filename);
				response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
				var data1 = new byte[stream.Length];
				stream.Read(data1, 0, data1.Length);
				stream.Close();
				response.BinaryWrite(data1);
				response.Flush();

				//  Needed to replace "Response.End();" with the following 3 lines, to make sure the file was fully written to the Response
				response.Flush();
				response.SuppressContent = true;
				HttpContext.Current.ApplicationInstance.CompleteRequest();
			}
			catch (Exception ex)
			{
				Trace.WriteLine("Failed, exception thrown: " + ex.Message);
			}
		}

		private static void WriteWordFile(DataSet ds, WordprocessingDocument wordDoc)
		{
			var mainPart = wordDoc.AddMainDocumentPart();
			var document = new Document();
			var body = new Body();

			mainPart.Document = document;
			mainPart.Document.Append(Params(body));
			body.Append(Params(GetSectionProperties()));

			foreach (DataTable dt in ds.Tables)
			{
				var table = new Table();
				table.Append(Params(GetTableProperties()));
				table.Append(Params(CreateHeaderRow(dt)));
				
				for (var i = 0; i < dt.Rows.Count; ++i)
				{
					var row = new TableRow();
					for (var j = 0; j < dt.Columns.Count; j++)
					{
						var cell = new TableCell();
						var para = new Paragraph();
						var run = new Run();
						var text = new Text(dt.Rows[i][j].ToString());
						run.PrependChild(GetCellFormat());
						run.Append(Params(text));
						para.Append(Params(run));
						cell.Append(Params(para));
						row.Append(Params(cell));
					}
					table.Append(Params(row));
				}
				body.Append(Params(table));
			}

			wordDoc.MainDocumentPart.Document.Save();
			wordDoc.Close();
		}

		private static TableProperties GetTableProperties()
		{
			var tblProp = new TableProperties(
				new TableBorders(new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 1 },
				new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 1 },
				new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 1 },
				new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 1 },
				new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 1 },
				new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 1 })
			);
			return tblProp;
		}

		private static SectionProperties GetSectionProperties()
		{
			var sectionProperties = new SectionProperties();
			var pageSize = new PageSize
			{
				Width = 15840U,
				Height = 12240U,
				Orient = PageOrientationValues.Landscape
			};
			var pageMargin = new PageMargin
			{
				Top = 1440,
				Right = 1440U,
				Bottom = 1440,
				Left = 1440U,
				Header = 720U,
				Footer = 720U,
				Gutter = 0U
			};
			var columns = new Columns { Space = "960" };
			var docGrid = new DocGrid { LinePitch = 360 };
			sectionProperties.Append(pageSize, pageMargin, columns, docGrid);
			return sectionProperties;
		}

		private static OpenXmlElement CreateHeaderRow(DataTable dt)
		{
			var tr = new TableRow();

			var numberOfColumns = dt.Columns.Count;
			for (var colInx = 0; colInx < numberOfColumns; colInx++)
			{
				var col = dt.Columns[colInx];
				var colName = col.ColumnName;

				var cell = new TableCell();
				var para = new Paragraph();
				var run = new Run();
				var text = new Text(colName);
				run.PrependChild(GetCellFormat());
				run.AppendChild(text);
				para.AppendChild(run);
				cell.AppendChild(para);
				tr.AppendChild(cell);
			}
			return tr;
		}

		private static RunProperties GetCellFormat()
		{
			var runProp = new RunProperties();
			var runFont = new RunFonts { Ascii = "Lucida Sans" };
			var fontSize = new FontSize { Val = new StringValue("16") }; // 16 half-point font size = size 8 font
			runProp.Append(Params((runFont)));
			runProp.Append(Params(fontSize));
			return runProp;
		}

		private static IEnumerable<OpenXmlElement> Params(object param)
		{
			return new List<OpenXmlElement> { param as OpenXmlElement };
		}

	}
}
