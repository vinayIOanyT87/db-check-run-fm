// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenerateFile.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the GenerateFile type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using FMBusinessObjects.UtilityObjects;

namespace FuelsManager.QueryWriterWebApp
{
	using System;
	using System.Data;
	using System.Globalization;
	using System.Linq;
	using System.Text;
	using System.Xml.Linq;
	using System.Security.Cryptography;
	using Crypt;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

    using FMCore;

	using FuelsManager.FMWebApp;


	/// <summary>
	/// Responsible for generating a file based on the query writer results.
	/// </summary>
	public partial class GenerateFile : FMFormBase
	{

		private DateTimeFormatInfo dateTimeInfoFormat = null;
		#region Methods

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				SiteClass currentSite =
					FMChannelHelper.MakeCall<ISites, SiteClass>(
						sites => sites.Get(this.Security, this.Security.SiteGuid, false, false, false));

				dateTimeInfoFormat = currentSite.GetDateTimeFormatInfo();

				this.Response.ClearContent();
				this.Response.ClearHeaders();

				if (this.Request.GetQueryOrFormValue("Mode").Equals("Single"))
				{
					this.GenerateSingleExport();
				}
				else if (this.Request.GetQueryOrFormValue("Mode").Equals("Multiple"))
				{
					this.GenerateMultipleExport();
				}
				else if (this.Request.GetQueryOrFormValue("Mode").Equals("CSV"))
				{
					this.GenerateCSVFile();
				}

				// Complete request and stop more than the file from rendering to the client
				this.Response.Flush();
				this.Response.SuppressContent = true;
			}
			catch (FMSessionInvalidException ex)
			{
				this.ErrorHandler(ex);
			}
			catch (Exception except)
			{
				this.Response.ClearContent();
				this.Response.ClearHeaders();
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Finds the data column.
		/// </summary>
		/// <param name="row">The row to search.</param>
		/// <param name="columnName">The name of the column to find.</param>
		/// <returns>The requested data column or null if not found.</returns>
		private static DataColumn FindDataColumn(DataRow row, string columnName)
		{
			foreach (DataColumn column in row.Table.Columns)
			{
				if (column.ColumnName == columnName)
				{
					return column;
				}
			}

			return null;
		}

		/// <summary>
		/// Converts query headers to CSV format.
		/// </summary>
		/// <param name="query">The query to convert.</param>
		/// <returns>A string containing CSV formatted headers.</returns>
		private static string QueryHeadersToCSV(QueryClass query)
		{
			var sb = new StringBuilder();
			foreach (QueryWriterField field in query.Fields)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "\"{0}\"", field.DisplayName));
				sb.Append(",");
			}

			// Remove the final comma
			sb.Remove(sb.Length - 1, 1);

			return sb.ToString();
		}

		/// <summary>
		/// Converts query row data to CSV format.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <param name="row">The row.</param>
		/// <returns>A string containing the CSV formatted row data.</returns>
		private static string QueryRowToCSV(QueryClass query, DataRow row, DateTimeFormatInfo dateFormat)
		{
			var sb = new StringBuilder();

			foreach (QueryWriterField field in query.Fields)
			{
				string fieldName;
				if (field.FieldType.IsEnum)
				{
					fieldName = field.EnumFieldName;
				}
				else
				{
					fieldName = field.DBFieldName;
				}

				DataColumn column = FindDataColumn(row, fieldName);

				if (column == null)
				{
					throw new ArgumentException("Data column not found.");
				}

				if (((field.FieldType == typeof(DateTimeOffset)) || (field.FieldType == typeof(DateAndTime))
					|| (field.FieldType == typeof(DateTimeOffset?)))
					&& (row[field.DBFieldName] != DBNull.Value && row[field.DBFieldName] is DateTimeOffset))
				{
					DateTimeOffset tmpDate = (DateTimeOffset)row[field.DBFieldName];
					sb.Append(string.Format("\"{0}\"", tmpDate.DateTime.ToString(dateFormat)));
				}
				else
				{
					sb.Append(string.Format(CultureInfo.InvariantCulture, "\"{0}\"", row[column.ColumnName]));
				}
				sb.Append(",");
			}

			// Remove the final comma
			sb.Remove(sb.Length - 1, 1);

			return sb.ToString();
		}

		/// <summary>
		/// Generates the CSV file.
		/// </summary>
		private void GenerateCSVFile()
		{
			this.GenerateCSVHeader();

			// Export the data rows from the result set
			var queryResults = this.Session[QueryResultsForm.QueryResultsDataTable] as DataTable;

			// Get the query object
			var query = (QueryClass)this.Session[QueryDefinitionForm.QuerywriterQueryObject];
			bool displayCUIDataMark = Global.IsFdsIM || AppSettingsHelper.GetKeyValue<bool>("DisplayCUIDataMark", false); ;

         if (displayCUIDataMark)
			{
                this.Response.Write("CUI\n");
            }

			// Write out the headers first
			this.Response.Write(QueryHeadersToCSV(query));
			this.Response.Write("\n");

			if (queryResults != null)
			{
				foreach (DataRow row in queryResults.Rows)
				{
					if (QueryClass.IsDataRow(row))
					{
						this.Response.Write(QueryRowToCSV(query, row, dateTimeInfoFormat));
						this.Response.Write("\n");
					}
				}
			}
            if (displayCUIDataMark)
            {
                this.Response.Write("CUI\n");
            }
        }

		/// <summary>
		/// Generates the CSV header.
		/// </summary>
		private void GenerateCSVHeader()
		{
			// Generate content header information
			this.Response.Buffer = false;
			this.Response.ContentType = "application/octet-stream";
			this.Response.AddHeader("Connecction", "Keep-Alive");
			this.Response.AddHeader("cache-control", "private, max-age=0");
			this.Response.AddHeader("Content-disposition", "attachment; filename=FMQueryExport.csv");
		}

		/// <summary>
		/// Generates the export file containing multiple queries.
		/// </summary>
		private void GenerateMultipleExport()
		{
			var usePlainText = this.UsePlainText();
			this.GenerateExportHeader(usePlainText);

			// Get the query collection to use
			var queryCollection = FMChannelHelper.MakeCall<IQueries, QueryCollectionClass>(x => x.Enumerate(this.Security, isQuickLoad: false));
			if (queryCollection == null)
			{
				throw new ApplicationException("Expected Query Collection");
			}

			// Loop through and generated the aggregate XElement object
			var mainElement = new XElement("FuelsManager.Queries");
			foreach (QueryClass query in queryCollection)
			{
				XElement queryElement = query.GetXML();
				mainElement.Add(queryElement.Elements().First());
			}

			var outputString = this.GenerateExportOutputString(mainElement.ToString(), usePlainText);
			this.Response.Write(outputString);
		}

		/// <summary>
		/// Generates a single query export.
		/// </summary>
		private void GenerateSingleExport()
		{
			var usePlainText = this.UsePlainText();
			this.GenerateExportHeader(usePlainText);

			var query = (QueryClass)this.Session[QueryDefinitionForm.QuerywriterQueryObject];
			XElement element = query.GetXML();

			var outputString = this.GenerateExportOutputString(element.ToString(), usePlainText);
			this.Response.Write(outputString);
		}

		private string Encrypt(string input)
		{
			byte[] buffer = Sign(CryptoHelper.EncryptAesSymmetric(input, Guids.ManagedQueriesImportExportGuid));
			return Convert.ToBase64String(buffer);
		}

		private byte[] Sign(byte []input)
		{

			string certificateName = FMChannelHelper.MakeCall<IConfigurationSettings, string>(x => x.GetKeyValueByKey(this.Security, "ManagedQueryCertificate"));
			if (!string.IsNullOrEmpty(certificateName))
			{
				Encoding encoding = Encoding.UTF8;
				
				byte[] certificateBytes = encoding.GetBytes(certificateName);
				//Sign password. Append signature to encrypted password
				using (RSACertificate theCert = new RSACertificate(certificateName))
				{
					if (theCert.Certificate != null)
					{
						var p = new RSACryptoServiceProvider();
						RSAParameters rp = new RSAParameters();
						rp = ((RSACryptoServiceProvider)theCert.Certificate.PrivateKey).ExportParameters(true);
						p.ImportParameters(rp);
						p.PersistKeyInCsp = false;

						SHA256CryptoServiceProvider hashAlg = new SHA256CryptoServiceProvider();
						
						byte[] signature = p.SignData(input, hashAlg);
						byte[] signedInput = new byte[1 + 512 + 256 + input.Length];
						signedInput[0] = 1; //Data signed
						for (int i = 0; i < 512; i++) signedInput[i+1] = 0;
						for (int i = 0; i < 512 && i < certificateBytes.Length; i++) signedInput[i+1] = certificateBytes[i];
						for (int i = 0; i < 256; i++) signedInput[i + 1 + 512] = signature[i];
						for (int i = 0; i < input.Length; i++) signedInput[i + 1 + 512 + 256] = input[i];

						return signedInput;
					}
				}
			}
			byte[] buf = new byte[input.Length+1];//first byte to indicate if signature is attached. 1=signed
			buf[0] = 0;//Data not signed
			for (int i = 0; i < input.Length; i++)
			{
				buf[1 + i] = input[i];
			}
			return buf;
		}

		private bool UsePlainText()
		{
			var usePlainText = AppSettingsHelper.GetKeyValue<bool>("PlainTextImportExportManagedQueries", false);
			return usePlainText;
		}

		private string GenerateExportOutputString(string input, bool usePlainText)
		{
			var outputString = usePlainText ? input : this.Encrypt(input);
			return outputString;
		}

		/// <summary>
		/// Generates the export header.
		/// </summary>
		private void GenerateExportHeader(bool usePlainText)
		{
			var fileName = usePlainText ? "FMQueryExport.xml" : "FMQueryExport.vef";  // .vef = Varec Encrypted File
			var attachmentClause = string.Format("attachment; filename={0}", fileName);

			// Generate content header information
			this.Response.AddHeader("Content-disposition", attachmentClause);
			this.Response.Buffer = false;
			this.Response.ContentType = "application/octet-stream";
			this.Response.AddHeader("cache-control", "private, max-age=0");
			this.Response.AddHeader("Connecction", "Keep-Alive");
		}

		#endregion
	}
}