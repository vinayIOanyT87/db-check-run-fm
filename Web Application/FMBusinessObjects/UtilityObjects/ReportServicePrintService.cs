
/// <summary>
/// File name:	ReportServicePrinting.cs
/// Purpose:	The purpose of this class is to render a selected report from
///				reporting services and print the report programmatically to a
///				selected printer.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	----------------------------------
///		
/// </summary>

using System;
using System.Drawing.Printing;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Management;
using System.Linq;

using FMBusinessObjects.ReportExecutionSvr2005;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ChannelFactories;
using System.Collections.Generic;
using System.Printing;
using FMBusinessObjects.Constants;

namespace FMBusinessObjects.UtilityObjects
{
	public class ReportServicePrintService
	{
		#region public attributes
		#endregion

		#region Private Attributes
		private EventLog EventLog;
		private string reportingServiceUrl;
		private string printerName;
		private string reportName;
		private System.Drawing.Image reportImage;
		private int numberOfCopies;
		private const int MAX_COPIES = 99;
		private const int MIN_COPIES = 1;
		private int numberOfPages;
		private int pageCount;
		private string deviceInfo1;
		private string deviceInfo2;
		private SecurityClass security;
		private ReportExecutionService reportingExecutionService2005;

		private ReportSvr2005.ParameterValue[] parameterValues;
		private ReportExecutionSvr2005.ParameterValue[] reportParameters = null;
		private bool enableBOLPDFArchiving;
		private string bolPdfArchivingPath;
		private string bolPdfArchivingFileName;
		private static List<string> skipDrivers = new List<string>() { "Microsoft XPS Document Writer", "Microsoft Print to PDF", "Microsoft Shared Fax Driver" };

		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Report Service Print Service class.
		/// </summary>
		public ReportServicePrintService(EventLog eventLog)
		{
			this.EventLog = eventLog;
			this.printerName = null;
			this.reportName = null;
			this.numberOfCopies = MIN_COPIES;
			this.numberOfPages = 0;
			this.pageCount = 1;
			this.deviceInfo1 = "<DeviceInfo><OutputFormat>EMF</OutputFormat><StartPage>";
			this.deviceInfo2 = "</StartPage></DeviceInfo>";

			this.reportingExecutionService2005 = new ReportExecutionService();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will set and get the printer name.
		/// </summary>
		public string PrinterName
		{
			get { return this.printerName; }
			set { this.printerName = value; }
		}

		/// <summary>
		/// This property will set and get the report name.
		/// </summary>
		public string ReportName
		{
			get { return this.reportName; }
			set { this.reportName = value; }
		}

		/// <summary>
		/// This property will set and get the reportingservice URL.
		/// </summary>
		public string ReportingServiceUrl
		{
			get { return this.reportingServiceUrl; }
			set { this.reportingServiceUrl = value; }
		}

		public string BOLPDFArchivingPath
		{
			get { return this.bolPdfArchivingPath; }
			set { this.bolPdfArchivingPath = value; }
		}

		public string BOLPDFArchivingFileName
		{
			get { return this.bolPdfArchivingFileName; }
			set { this.bolPdfArchivingFileName = value; }
		}

		public bool EnableBOLPDFArchiving
		{
			get { return this.enableBOLPDFArchiving; }
			set { this.enableBOLPDFArchiving = value; }
		}
		/// <summary>
		/// This property will set and get the number of copies attribute.
		/// </summary>
		public int NumberOfCopies
		{
			get { return this.numberOfCopies; }
			set
			{
				if (((int)value < MIN_COPIES) || ((int)value > MAX_COPIES))
					this.numberOfCopies = MIN_COPIES;
				else
					this.numberOfCopies = value;
			}
		}


		/// <summary>
		/// This property will get the error message attribute.
		/// </summary>
		public string ErrorMessage
		{
			get { return this.ErrorMessage; }
		}

		/// <summary>
		/// This property will set the ParameterValues attribute.
		/// </summary>
		public ReportSvr2005.ParameterValue[] ParameterValues
		{
			set { this.parameterValues = value; }
		}

		/// <summary>
		/// This property will set the Security attribute.
		/// </summary>
		public SecurityClass Security
		{
			set { this.security = value; }
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// This method will call for the report to be render and then for it to be printed to
		/// the designated printer.
		/// </summary>
		/// <returns></returns>
		public void PrintReport()
		{
			this.RenderAndPrint();
			if (this.enableBOLPDFArchiving)
			{
				this.ArchiveReport();
			}
		}

		public void ArchiveReport()
		{
			byte[] pdfResult = this.RenderReportInPDF();
			this.SaveReport(pdfResult, this.BOLPDFArchivingFileName);
		}
		/// <summary>
		/// This method will render the report in PDF format. The method returns an array
		/// of bytes. It will return null if the report is not found or an error occurs.
		/// </summary>
		/// <returns></returns>
		public byte[] RenderReportInPDF()
		{
			try
			{
				SystemSettingClass systemSetting = FMChannelHelper.MakeCall<ISystemSettings, SystemSettingClass>(
																	 x =>
																	 x.Get(this.security)
																);


				// Create the proxy object and set credentials to Windows Authentication (default).
				this.reportingExecutionService2005.Url = systemSetting.ReportServerUrl + "/ReportExecution2005.asmx";
				this.reportingExecutionService2005.UseDefaultCredentials = false;
				this.reportingExecutionService2005.Credentials = this.reportingExecutionService2005.Credentials;
				if (!string.IsNullOrEmpty(systemSetting.ReportServerUserName))
				{
					string[] userName = systemSetting.ReportServerUserName.Split('\\');
					if (userName.Length > 1)
					{
						reportingExecutionService2005.Credentials = new NetworkCredential(userName[1], systemSetting.ReportServerPassword, userName[0]);
					}
					else
					{
						reportingExecutionService2005.Credentials = new NetworkCredential(userName[0], systemSetting.ReportServerPassword, ".");
					}
				}
				else
				{
					reportingExecutionService2005.Credentials = CredentialCache.DefaultCredentials;
				}


				byte[] result;
				string[] streamIDs;
				string optionalString = null;
				string extension = null;
				string historyID = null;
				string parameterLanguage = "en-us";

				// Create a device info request that will indicate the type of format (EMF) and the
				// number of pages the report contains.  The value of zero will render all the pages.
				string deviceInfo = this.deviceInfo1 + "0" + this.deviceInfo2;

				// Set the type of render to be an image. Other types can be HTML, PDF, XML, ...
				string format = "PDF";

				ReportExecutionSvr2005.Warning[] warnings = null;
				this.reportParameters = null;


				ReportExecutionSvr2005.ExecutionInfo executionInfo =
													this.reportingExecutionService2005.LoadReport(this.ReportName, historyID);

				if (executionInfo != null)
				{
					ReportExecutionSvr2005.ReportParameter[] validReportParameters = executionInfo.Parameters;

					if (validReportParameters != null
					&& validReportParameters.Length > 0
					&& this.parameterValues != null
					&& this.parameterValues.Length > 0)
					{
						int parameterCount = 0;
						this.reportParameters = new ReportExecutionSvr2005.ParameterValue[this.parameterValues.Length];

						foreach (ReportSvr2005.ParameterValue parameterValue in this.parameterValues)
						{
							foreach (ReportExecutionSvr2005.ReportParameter validReportParameter in validReportParameters)
							{
								if (validReportParameter.Name.ToUpper().Equals(parameterValue.Name.ToUpper()) == true)
								{
									this.reportParameters[parameterCount] = new ReportExecutionSvr2005.ParameterValue();
									this.reportParameters[parameterCount].Name = validReportParameter.Name;
									this.reportParameters[parameterCount].Value = parameterValue.Value;
									parameterCount++;
									break;
								}
							}
						}

						this.reportingExecutionService2005.SetExecutionParameters(this.reportParameters, parameterLanguage);
					}
				}

				result = this.reportingExecutionService2005.Render(format,             // Format of the report (Image, PDF, XML, CSV, ...)
																	deviceInfo,         // XML string containing device specific info.
																	out extension,      // Extension
																	out optionalString, // MimeType
																	out optionalString, // Encoding
																	out warnings,       // Warning objects
																	out streamIDs);     // The stream identifiers                                                          

				return result;
			}
			catch (Exception e)
			{
				EventLog.WriteEntry(e.Message, EventLogEntryType.Error);
				throw new Exception("Could not retrieve report parameters or render the following report: " + this.reportName);
			}
		}
		#endregion

		#region Private methods

		/// <summary>
		/// This method will render the entire report to discover the number of pages the report
		/// contains.  It will also set the printer name, the number of copies to be printed, the 
		/// event handler to handle the printing, and request the report to be printed.
		/// </summary>
		private void RenderAndPrint()
		{
			try
			{
				// Create the proxy object and set credentials to Windows Authentication (default).
				this.reportingExecutionService2005.Url = this.reportingServiceUrl + "/ReportExecution2005.asmx";
				this.reportingExecutionService2005.UseDefaultCredentials = true;
				this.reportingExecutionService2005.Credentials = this.reportingExecutionService2005.Credentials;


				byte[] result;
				string[] streamIDs;
				string optionalString = null;
				string extension = null;
				string historyID = null;
				string parameterLanguage = "en-us";

				// Create a device info request that will indicate the type of format (EMF) and the
				// number of pages the report contains.  The value of zero will render all the pages.
				string deviceInfo = this.deviceInfo1 + "0" + this.deviceInfo2;

				// Set the type of render to be an image. Other types can be HTML, PDF, XML, ...
				string format = "IMAGE";

				ReportExecutionSvr2005.Warning[] warnings = null;
				this.reportParameters = null;

				ReportExecutionSvr2005.ExecutionInfo executionInfo =
										  this.reportingExecutionService2005.LoadReport(this.ReportName, historyID);

				if (executionInfo != null)
				{
					ReportExecutionSvr2005.ReportParameter[] validReportParameters = executionInfo.Parameters;

					if (validReportParameters != null
					&& validReportParameters.Length > 0
					&& this.parameterValues != null
					&& this.parameterValues.Length > 0)
					{
						int parameterCount = 0;
						reportParameters = new ReportExecutionSvr2005.ParameterValue[this.parameterValues.Length];

						foreach (ReportSvr2005.ParameterValue parameterValue in this.parameterValues)
						{
							foreach (ReportExecutionSvr2005.ReportParameter validReportParameter in validReportParameters)
							{
								if (validReportParameter.Name.ToUpper().Equals(parameterValue.Name.ToUpper()) == true)
								{
									this.reportParameters[parameterCount] = new ReportExecutionSvr2005.ParameterValue();
									this.reportParameters[parameterCount].Name = validReportParameter.Name;
									this.reportParameters[parameterCount].Value = parameterValue.Value;
									parameterCount++;
									break;
								}
							}
						}

						this.reportingExecutionService2005.SetExecutionParameters(this.reportParameters, parameterLanguage);
					}
				}

				if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey()))
				{
					var credentialsList = new DataSourceCredentials[1];

					credentialsList[0] = new DataSourceCredentials
					{
						Password = FMChannelHelper.MakeCall<IDBAccess, string>(x => x.GetDBPassword(this.security.Password)),
						UserName = this.security.UserID,
						DataSourceName = "ConsolidatedDBDataSource"
					};

					this.reportingExecutionService2005.SetExecutionCredentials(credentialsList);
				}

				// Render the entire report to find out how many pages it contains.
				result = this.reportingExecutionService2005.Render(format,             // Format of the report (Image, PDF, XML, CSV, ...)
																deviceInfo,         // XML string containing device specific info.
																out extension,      // Extension
																out optionalString, // MimeType
																out optionalString, // Encoding
																out warnings,       // Warning objects
																out streamIDs);     // The stream identifiers                                                          

				// Find the number of pages the report contains and set the initial page count
				// to the first page.
				numberOfPages = streamIDs.Length;
				if (numberOfPages == 0)
				{
					numberOfPages++;
				}

				pageCount = 1;

				using (var printDoc = new PrintDocument())
				{
					printDoc.PrintPage += new PrintPageEventHandler(PrintPageEventHandler);

					// Use the local printer if a printer name is not returned.
					printDoc.PrinterSettings.PrinterName = printerName;
					if (printDoc.PrinterSettings.IsValid == false)
					{
						throw new Exception("ReportServicePrintService.RenderAndPrint : Printer - " + printerName + " is Invalid");
					}

					// Set the number of copies to print. Default is one.
					printDoc.PrinterSettings.Copies = (short)numberOfCopies;
					printDoc.Print();
				}
			}
			catch (Exception e)
			{
				this.EventLog.WriteEntry(e.Message, EventLogEntryType.Error);
			}
		}

		private void SaveReport(byte[] reportStream, string fileName)
		{
			const int Offset = 0;
			int numberOfBytes = reportStream.Length;
			FileStream writeStream;

			if (String.IsNullOrEmpty(this.BOLPDFArchivingPath))
			{
				this.BOLPDFArchivingPath = @"C:\BOL Archive";
			}

			string pathString = Path.Combine(this.BOLPDFArchivingPath, DateTime.Now.ToString("yyyyMMdd"));

			if (!File.Exists(pathString))
			{
				Directory.CreateDirectory(pathString);
			}

			string fileNamePath = Path.Combine(pathString, fileName);

			try
			{
				writeStream = new FileStream(fileNamePath, FileMode.Create, FileAccess.Write);
			}
			catch (Exception ex)
			{
				this.EventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
				throw new Exception(ex.Message);
			}

			try
			{
				if (numberOfBytes > 0)
				{
					writeStream.Write(reportStream, Offset, numberOfBytes);

					writeStream.Close();
				}
				else
				{
					writeStream.Close();

					const string ErrMsg = "ReportServicePringService.SaveReport : Report file is empty.";
					this.EventLog.WriteEntry(ErrMsg, EventLogEntryType.Error);
					throw new Exception(ErrMsg);
				}
			}
			catch (Exception ex)
			{
				writeStream.Close();

				this.EventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
				throw new Exception(ex.Message);
			}
		}

		/// <summary>
		/// This method will handle the print event request. It will render each page of the report
		/// individually and print that page. With an Image type the printer functionality cannot
		/// page through the image printing each page.  Therefore, each page of the report is rendered
		/// individually and printed.  This handler keeps being called until the event argument method
		/// HasMorePages is set to false.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="ev"></param>
		private void PrintPageEventHandler(object sender, PrintPageEventArgs eventArgs)
		{
			byte[] result;
			string[] streamIDs;
			string optionalString = null;
			string extension = null;
			string historyID = null;
			string parameterLanguage = "en-us";

			// Set the type of render to be an image.
			string format = "IMAGE";

			//ParameterValue[] optionalParams = null;
			//Warning[] warnings = null;
			System.IO.MemoryStream stream = null;
			ReportExecutionSvr2005.Warning[] warnings = null;

			try
			{
				// Loop through the report rendering each page and printing it.
				if (this.pageCount <= this.numberOfPages)
				{
					ReportExecutionSvr2005.ExecutionInfo executionInfo =
															 this.reportingExecutionService2005.LoadReport(this.ReportName, historyID);
					this.reportingExecutionService2005.SetExecutionParameters(this.reportParameters, parameterLanguage);

					// Create the device info for each page of the report. Then render that page of the
					// report and print it.
					string deviceInfo = deviceInfo1 + this.pageCount + deviceInfo2;

					result = this.reportingExecutionService2005.Render(format,             // Format of the report (Image, PDF, XML, CSV, ...)
																		deviceInfo,         // XML string containing device specific info.
																		out extension,      // Extension
																		out optionalString, // MimeType
																		out optionalString, // Encoding
																		out warnings,       // Warning objects
																		out streamIDs);     // The stream identifiers                                                          

					// Create a memory stream of the rendered report
					stream = new MemoryStream(result);
					this.reportImage = System.Drawing.Image.FromStream(stream);

					// Draw a picture.
					eventArgs.Graphics.DrawImage(this.reportImage, eventArgs.Graphics.VisibleClipBounds);

					// This will keep a blank page from being printed.
					if (this.pageCount == this.numberOfPages)
					{
						eventArgs.HasMorePages = false;
					}
					else
					{
						eventArgs.HasMorePages = true;
					}

					// Increment to next page.
					pageCount++;
				}
				else
				{
					// Indicate that this is the last page to print.
					eventArgs.HasMorePages = false;
				}
			}
			catch (Exception e)
			{
				EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
				}
			}
		}

		public static string[] EnumeratePrinters(string callerDescription = null)
		{
				try
				{
					 var printers = new SortedSet<string>();
					 foreach (String printer in PrinterSettings.InstalledPrinters)
					 {
						  var server = new PrintServer(printer);
						  PrintQueueCollection queues = server.GetPrintQueues();
						  // Get a unique list of all printers
						  // skip those known to require user interactions to print e.g. select where to save a file or phone number for a fax.
						  printers.UnionWith(queues.Where(q => skipDrivers.All(d => !q.QueueDriver.Name.StartsWith(d, StringComparison.InvariantCultureIgnoreCase))).Select(q => q.Name));

					 }
					 return printers.ToArray();
				}
				catch (Exception e)
				{
					 string msg = String.Empty;

                if (callerDescription != null)
					 {
						  msg = callerDescription;
                }

                msg += " Printer Configuration: Print Spooler service must be enabled to allow printing. Inner Exception: ";
                FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msg + " " + e.Message, FMEventLogEntryType.Error));
					 return new string[] { };
            }
		}

		#endregion
	}

}
