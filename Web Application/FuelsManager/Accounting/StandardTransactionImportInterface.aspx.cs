using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

using GenericParsing;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.Constants;
using FMBusinessObjects.DataObjects;
using Unity;
using XMLImport;
using FMDepedencyManager;
using System.Configuration;
using System.Collections.Generic;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;

namespace FuelsManager.Accounting
{
    public partial class Standard_Transaction_Import_Interface : AccountingWebFormView
    {
        protected AccountingSite accountingSite;
        private int recordPrintOutCount = 0;

		const string dataSetKey = "StandardImport.DataSet";
		const string fileDataKey = "StandardImport.FileData";
		const string fileNameKey = "StandardImport.FileName";

		const string uploadArchivePathConfigKey = "StandardImportUploadArchivePath";
		const string interimDataPathConfigKey = "StandardImportInterimDataArchivePath";

		private readonly XMLImportProcessor _xmlImportProcessor;
		private readonly ICurrentRequestContext _currentUserSecurity;
		private readonly ISiteProxy _siteProxy;

		public Standard_Transaction_Import_Interface()
		{
			//Using Service Locator like this is an antipattern, but ASMX is not designed for DI.  It must have a parameterless constructor.
			this._xmlImportProcessor = FMServiceLocator.Container.Resolve<XMLImportProcessor>();
			this._currentUserSecurity = FMServiceLocator.Container.Resolve<ICurrentRequestContext>();
			this._siteProxy = FMServiceLocator.Container.Resolve<ISiteProxy>();
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            this.InitializeComponent();
            this.CurrentSiteGuid = Guids.SiteAdminGuid;
            this.Initialize();
            base.OnInit(e);
        }

        /// <summary>
        ///     This is the main entry point for the closeout page.  It is called by IIS.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.GetSecurity();

				this.cancelFileButton.Enabled = false;
				this.importFileButton.Enabled = false;

                if (this.Page.IsPostBack == true && this.fileUpload.PostedFile.FileName != "")
                {
					this.CheckFile();
                    return;
                }

                return;
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }

        protected void UploadFileButton_Click(object sender, EventArgs e)
        {
            try
            {
                //This will kick off the parsing process
                this.CheckFile();
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }

        protected void ClearFileButton_Click(object sender, EventArgs e)
        {
            try
            {
				this.ClearForm();
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }

        protected void ImportFileButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.SubmitFile();
				//-|------------------------
				//-|Manage Control States
				//-|------------------------
				this.Session[fileDataKey] = string.Empty;
				this.Session[fileNameKey] = string.Empty;
				this.Session[dataSetKey] = null;
                this.cancelFileButton.Enabled = true;
                this.importFileButton.Enabled = false;
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }

        private void ClearForm()
        {
            try
            {
				this.Session[fileDataKey] = string.Empty;
				this.Session[fileNameKey] = string.Empty;
				this.Session[dataSetKey] = null;
				this.filePathLabel.Text = "No file selected";
                this.results.InnerHtml = string.Empty;
				this.preview.InnerHtml = string.Empty;
				this.cancelFileButton.Enabled = false;
				this.importFileButton.Enabled = false;
				this.uploadIcon.ImageUrl = "~/Content/icons/gray-upload-icon.png";
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }

        private void CheckFile()
        {
            try
            {
                string results = this.ParseFile();
                if (results == "NO_FILE_SELECTED")
                {
                    //-|-----------------------------
                    //-|Don't change control states
                    //-|-----------------------------
                    results = "No file selected.";
                    this.results.InnerHtml = results;
                    throw new ApplicationException(results);
                }
                else
                {
					//-|------------------------
					//-|Manage Control States
					//-|------------------------
					this.filePathLabel.Text = this.fileUpload.FileName;
					this.cancelFileButton.Enabled = true;
					this.importFileButton.Enabled = true;
					this.uploadIcon.ImageUrl = "~/Content/icons/cyan-upload-icon.png";
                }
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }

        private string ParseFile()
        {
            if (!this.fileUpload.HasFile)
                return "NO_FILE_SELECTED";

            string data = System.Text.ASCIIEncoding.ASCII.GetString(this.fileUpload.FileBytes);

			//-|--------------------------------------------------------
			//-|save file data on server side to avoid uploading twice
			//-|--------------------------------------------------------
			this.Session[fileNameKey] = this.fileUpload.FileName;
			this.Session[fileDataKey] = data;

            using (GenericParserAdapter parser = new GenericParserAdapter(new StreamReader(this.fileUpload.PostedFile.InputStream)))
            {
                parser.FirstRowHasHeader = true;
				DataSet uploadedDataRecords = parser.GetDataSet();
                DataTable dt = uploadedDataRecords.Tables[0];

                StringBuilder strHTMLBuilder = new StringBuilder();
                strHTMLBuilder.Append("<table id='preview'>");
                
                strHTMLBuilder.Append("<tr>");
                foreach (DataColumn myColumn in dt.Columns)
                {
                    strHTMLBuilder.Append("<th>");
                    strHTMLBuilder.Append(myColumn.ColumnName);
                    strHTMLBuilder.Append("</th>");
                }
                strHTMLBuilder.Append("</tr>");

                foreach (DataRow myRow in dt.Rows)
                {
                    strHTMLBuilder.Append("<tr>");
                    foreach (DataColumn myColumn in dt.Columns)
                    {
                        strHTMLBuilder.Append("<td>");
                        strHTMLBuilder.Append(myRow[myColumn.ColumnName].ToString());
                        strHTMLBuilder.Append("</td>");
                    }
                    strHTMLBuilder.Append("</tr>");
                }

                strHTMLBuilder.Append("</table>");
                
				this.preview.InnerHtml = strHTMLBuilder.ToString();
                this.results.InnerHtml = "<p>" + dt.Rows.Count.ToString() + " transactions found in data file.</p>";
				this.Session[dataSetKey] = uploadedDataRecords;
            }

            return string.Empty;
        }

        private void SubmitFile()
        {
            try
            {
                int numberOfRecords = 0;
                int successCount = 0;
                int duplicateCount = 0;
                int failureCount = 0;

                string resultMessage = "";
                string successMessage = "";
                string failureMessage = "";
                string duplicateMessage = "";

                string data = this.Session[fileDataKey].ToString();
				string fileName = this.Session[fileNameKey].ToString();
				DataSet uploadedDataRecords = this.Session[dataSetKey] as DataSet;

                var records = data.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n').ToList<string>();

                foreach (string item in records)
                {
                    if (records[this.recordPrintOutCount] != "")
                    {
                        numberOfRecords++;
                    }
					this.recordPrintOutCount++;
                }

				if (numberOfRecords > 0)
				{
					numberOfRecords--; // reduce count by 1 for the header
				}

                if (this.fileUpload.HasFile == true && data.Length < 1)
                {
                    this.ClearForm();
                    throw new ApplicationException("There is no data in this file. Please select a new file.");
                }

                MemoryStream xmlStream = new MemoryStream();
				WriteRecordToXml xml = new WriteRecordToXml(this.Security);
                Dictionary<string, string> allTransactions = new Dictionary<string, string>();
				List<TransactionValidationResult> parseValidationResults = new List<TransactionValidationResult>();
                List<string> duplicateTransactions = new List<string>();

				xml.WriteRecord(xmlStream, fileName, uploadedDataRecords, this.EnumerateProducts(), allTransactions, parseValidationResults, duplicateTransactions);

				string uploadArchivePath = ConfigurationManager.AppSettings[uploadArchivePathConfigKey];
				if (!string.IsNullOrEmpty(uploadArchivePath))
				{
					var uploadArchiveFile = Path.Combine(uploadArchivePath, fileName);
					File.WriteAllText(uploadArchiveFile, data);
				}

				string interimDataPath = ConfigurationManager.AppSettings[interimDataPathConfigKey];

				xmlStream.Seek(0, SeekOrigin.Begin);
				string filePath = Path.ChangeExtension(Path.Combine(interimDataPath, fileName), "xml");
				using (FileStream xmlFileStream = File.Create(filePath))
				{
					xmlStream.CopyTo(xmlFileStream);
				}

				xmlStream.Seek(0, SeekOrigin.Current);
				var result = this._xmlImportProcessor.Import(this.Security, this.Security.SiteID, xmlStream, null);

                // check if all records failed or contained duplicates and remove invalid 'node' 
                // resulting in 'ReadSubtree() can be called only if the reader is on an element node' error
                if (result.Count == 1 && result[0].TransID == "")
                {
                    result.Remove(result[0]);
                }

                result.InsertRange(0, parseValidationResults);
				duplicateCount = duplicateTransactions.Count;

                resultMessage = "<div>";
				resultMessage += "<p class='records-processed'>" + numberOfRecords + " transactions" + (numberOfRecords == 1 ? "" : "s") + " processed.</p>";

                // create duplicate grouping
				if (duplicateCount > 0)
				{
                    duplicateMessage = "<input id='duplicate-transactions' class='toggle' type='checkbox'>";
                    duplicateMessage += "<label for='duplicate-transactions' class='lbl-toggle duplicate-transactions'>" + duplicateCount + " duplicate transaction" + (duplicateCount == 1 ? "" : "s") + " skipped during import.</label>";
                    duplicateMessage += "<div class='collapsible-content'><div class='content-inner'>";

					foreach(string duplicateTransaction in duplicateTransactions)
					{
                        duplicateMessage += "<p>Transaction " + duplicateTransaction + " - skipped</p>";

                        // remove from list of all transactions for final success count
                        var key = allTransactions.FirstOrDefault(x => x.Value == duplicateTransaction).Key;
                        allTransactions.Remove(key);
					}
                    duplicateMessage += "</div></div>";
                }

                // create failure grouping
                failureCount = result.Count;
                if (failureCount > 0)
                {
                    failureMessage = "<input id='failed-transactions' class='toggle' type='checkbox'>";
                    failureMessage += "<label for='failed-transactions' class='lbl-toggle failed-transactions'>" + failureCount + " transaction" + (failureCount == 1 ? "" : "s") + " failed during import.</label>";
                    failureMessage += "<div class='collapsible-content'><div class='content-inner'>";

                    foreach (TransactionValidationResult validationResult in result)
                    {
                        string val;
                        if (allTransactions.TryGetValue(validationResult.TransID, out val))
                        {
                            if (string.IsNullOrEmpty(val))
                            {
                                val = validationResult.TransID;
                            }
                            failureMessage += "<p>Transaction " + val + " - ";
                            // remove failed transactions by TransID
                            allTransactions.Remove(validationResult.TransID);
                        }
                        else
                        {
                            failureMessage += "<p>Transaction " + validationResult.TransID + " - ";
                            // remove failed transactions by DocumentNumber
                            var key = allTransactions.FirstOrDefault(x => x.Value == validationResult.TransID).Key;
                            allTransactions.Remove(key);
                        }

                        foreach (string error in validationResult.ErrorList)
                        {
                            failureMessage += error + " ";
                        }
                        failureMessage += "</p>";
                    }
                    failureMessage += "</div></div>";
                }

                // create success grouping
                successCount = allTransactions.Count;
                if (successCount > 0)
                {
                    successMessage = "<input id='success-transactions' class='toggle' type='checkbox'>";
                    successMessage += "<label for='success-transactions' class='lbl-toggle success-transactions'>" + successCount + " transaction" + (successCount == 1 ? "" : "s") + " imported successfully.</label>";
                    successMessage += "<div class='collapsible-content'><div class='content-inner'>";

                    foreach (KeyValuePair<string, string> transaction in allTransactions)
                    {
                        successMessage += "<p>Transaction " + transaction.Value + " - processed successfully!</p>";
                    }
                    successMessage += "</div></div>";
                }

                resultMessage += successMessage + duplicateMessage + failureMessage;
                resultMessage += "</div>";
                this.results.InnerHtml = resultMessage;
            }
			catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }

        private SortedList EnumerateProducts()
        {
            SortedList listItems = new SortedList();

            ProductCollectionClass productCollection =
                    FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(x => x.Enumerate(this.security));

            foreach (ProductClass product in productCollection)
            {
                listItems.Add(product.ID, product.Code);
            }

            return listItems;
        }
    }
}