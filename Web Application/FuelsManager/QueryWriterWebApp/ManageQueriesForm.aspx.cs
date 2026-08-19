// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManageQueriesForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   This class provides code behind support for the ManageQueriesForm page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using FMCore;

namespace FuelsManager.QueryWriterWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Xml.Linq;
	using System.Security.Cryptography;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;

	using FuelsManager.FMWebApp;

	using Crypt;

	/// <summary>
	/// This class provides code behind support for the ManageQueriesForm page.
	/// </summary>
	public partial class ManageQueriesForm : FMFormBaseAjax
	{
		#region Constants and Fields

		/// <summary>
		/// Key for session storage of manage queries object.
		/// </summary>f
		public const string ManageQueriesObject = "QueryWriterManage_Edit";

		/// <summary>
		/// Key for session storage of data source.
		/// </summary>
		protected const string ManageQueriesDataSource = "MANAGE_QUERIES_DATA_SOURCE";

		/// <summary>
		/// Key for session storage of the data view.
		/// </summary>
		protected const string QueriesDataView = "QueryWriterManageQueriesDataView";

		protected string warningMessage = "";
		protected const string IMPORTED_QUERY_XDOCUMENT = "ImportedQueryDocument";

		#endregion

		#region Methods

		/// <summary>
		/// Initializes the event handlers for thisg page.
		/// </summary>
		protected void InitializeComponents()
		{
			this.AddButton1.Click += this.AddButtonClick;
			this.AddButton2.Click += this.AddButtonClick;
			this.ImportButton.Click += this.ImportButtonClick;
			this.QueryGrid.RowCommand += this.QueryGridRowCommand;
			this.QueryGrid.RowCreated += this.QueryGridRowCreated;
			this.QueryGrid.RowDataBound += this.QueryGrid_RowDataBound;
		}

		/// <summary>
		/// Raises the <see cref="OnInit"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.InitializeComponents();
		}

		/// <summary>
		/// Handles the Init event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Init(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				this.Session.Remove(ManageQueriesObject);

				if (this.IsPostBack == false)
				{
					this.Session.Remove(ManageQueriesDataSource);
					this.Session.Remove(IMPORTED_QUERY_XDOCUMENT);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

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

				if (this.Security.HasRight(RIGHT.MODIFY_QUERIES) == false)
				{
					this.ImportButton.Enabled = false;
					this.AddButton1.Enabled = false;
					this.AddButton2.Enabled = false;
				}

				if (this.IsPostBack)
				{
					if (this.Request.GetQueryOrFormValue("__EVENTTARGET") == "SaveManagedQueriesClick")
					{
						SaveManagedQueriesClick();
					}
					else if (this.Request.GetQueryOrFormValue("__EVENTTARGET") == "DropManagedQueriesClick")
					{
						DropManagedQueriesClick();
					}
				}


				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the RowCommand event of the QueryGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
		protected void QueryGridRowCommand(object sender, CommandEventArgs e)
		{
			string urlValue = string.Empty;

			try
			{
				var queryCollection = (QueryCollectionClass)this.QueryGrid.DataSource;

				if (e.CommandName == "Edit")
				{
					int index = Convert.ToInt32(e.CommandArgument);

					QueryClass query =
						FMChannelHelper.MakeCall<IQueries, QueryClass>(
							queries => queries.Get(this.Security, queryCollection[index].IdentityGuid));

					this.Session.Add(ManageQueriesObject, query);
					urlValue = "QueryDefinitionForm.aspx?Mode=Edit";
				}
				else if (e.CommandName == "Delete")
				{
					int index = Convert.ToInt32(e.CommandArgument);
					QueryClass query = queryCollection[index];
					
					if (query.SystemQuery == false)
					{
						FMChannelHelper.MakeCall<IQueries>(queries => queries.PurgeByIdentityGuid(this.Security, query.IdentityGuid));
					}

					this.UpdateView();
				}
				else if (e.CommandName == "View")
				{
					// using New command for View column
					int index = Convert.ToInt32(e.CommandArgument);

					QueryClass query =
						FMChannelHelper.MakeCall<IQueries, QueryClass>(
							queries => queries.Get(this.Security, queryCollection[index].IdentityGuid));

					this.Session.Add(QueryDefinitionForm.QuerywriterQueryObject, query);
					urlValue = "QueryResultsForm.aspx";
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			// Do the redirect outside the try/catch or a "Thread is being aborted" exception is logged in the event log.
			if (string.IsNullOrEmpty(urlValue) == false)
			{
				this.Redirect(urlValue);
			}
		}

		/// <summary>
		/// Responsible for updating the view on the query grid.
		/// </summary>
		protected void UpdateView()
		{
			QueryCollectionClass queryCollection;

			if (this.Session[ManageQueriesDataSource] != null)
			{
				queryCollection = (QueryCollectionClass)this.Session[ManageQueriesDataSource];
			}
			else
			{
				queryCollection =
					FMChannelHelper.MakeCall<IQueries, QueryCollectionClass>(x => x.Enumerate(this.Security, isQuickLoad: true));
			}

			this.QueryGrid.DataSource = queryCollection;
			this.QueryGrid.DataBind();
		}

		/// <summary>
		/// Handles the Click event of the AddButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void AddButtonClick(object sender, EventArgs e)
		{
			this.Redirect("QueryDefinitionForm.aspx?Mode=New");
		}

		/// <summary>
		/// Determines a unique name for the query.
		/// </summary>
		/// <param name="queries">Proxy to IQueries service class on FMBusinessServices.</param>
		/// <param name="query">The query object.</param>
		private void DetermineUniqueQueryName(IQueries queries, QueryClass query)
		{
			int index = 0;

			string queryName = query.QueryName;

			QueryClass existingQuery = queries.GetByQueryName(this.Security, queryName);
			while (existingQuery.IdentityGuid != Guid.Empty)
			{
				++index;
				queryName = query.QueryName + "-" + index.ToString(CultureInfo.InvariantCulture);
				existingQuery = queries.GetByQueryName(this.Security, queryName);
			}

			query.QueryName = queryName;
		}


		private void DetermineUniqueQueryMenuPath(IQueries queries, QueryClass query)
		{
			int index = 0;

			string queryMenuPath = query.NavNodePath.Trim();

			if (queryMenuPath.Length == 0)
			{
				query.NavNodePath = queryMenuPath;
				return;
			}

			QueryClass existingQuery = queries.GetByNodePath(Security, queryMenuPath);
			while (existingQuery.QueryStorageGuid != Guid.Empty)
			{
				++index;
				queryMenuPath = query.NavNodePath + "-" + index;
				existingQuery = queries.GetByNodePath(Security, queryMenuPath);
			}

			query.NavNodePath = queryMenuPath;
		}

		/// <summary>
		/// Handles the Click event of the ImportButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void ImportButtonClick(object sender, EventArgs e)
		{
			try
			{
				this.ImportQuery();

			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Imports the query.
		/// </summary>
		/// <param name="queries">The queries proxy to use.</param>
		private void ImportQuery()
		{
			Session.Remove(IMPORTED_QUERY_XDOCUMENT);
			string input = "";
			warningMessage = null;

			if (this.Request.Files.AllKeys.Length != 0)
			{
				HttpPostedFile file = this.Request.Files[0];

				var usePlainText = AppSettingsHelper.GetKeyValue<bool>("PlainTextImportExportManagedQueries", false);

				this.ValidateImportFile(file, usePlainText);

				var reader = new StreamReader(file.InputStream);
				
				if (usePlainText)
				{
					input = reader.ReadToEnd();
				}
				else
				{
					//Verify signature and decrypt data
					byte[] buffer = Convert.FromBase64String(reader.ReadToEnd());
					byte[] encryptedData = GetEncryptedData(buffer);

					int code = this.VerifySignature(buffer, encryptedData, out warningMessage);

					if (code == 0)
					{
						//Bad data
						ClientScriptManager csm = Page.ClientScript;
						if (!csm.IsStartupScriptRegistered(this.GetType(), "SignatureWarningMessage"))
						{
							csm.RegisterStartupScript(this.GetType(), "SignatureWarningMessage",
								string.Format("<script type=\"text/javascript\"> alert(\"{0} \") ;</script>",
									warningMessage), false);
						}
						return;
					}
					else{
						//Good data
						CryptoHelper cryptoHelper = new CryptoHelper(Guids.ManagedQueriesImportExportGuid);
						input = cryptoHelper.DecryptAesSymmetric(encryptedData);
						if (code == -1)
						{
							//Signature cannot be verified or data is not signed.
							Session[IMPORTED_QUERY_XDOCUMENT] = input;
							ClientScriptManager csm = Page.ClientScript;
							if (!csm.IsStartupScriptRegistered(this.GetType(), "SignatureWarningMessage"))
							{
								csm.RegisterStartupScript(this.GetType(), "SignatureWarningMessage",
									string.Format("<script type=\"text/javascript\"> if (confirm(\"{0} \\n\\rContinue import?\") == true) " +
													"__doPostBack(\"SaveManagedQueriesClick\",\"\"); " +
													"else " +
													"__doPostBack(\"DropManagedQueriesClick\",\"\");    </script>",
										warningMessage), false);
							}
							return;
						}
					}
				
				}


				PopulateQueries(input);
				this.UpdateView();
			}
		}
		//Get encrypted data. If first byte of the input buffer is set then encrypted data starts after byte 768. 
		//bytes between 2 and 768 contain certificate name and digital signature.
		//if first byte is not, then encrypted data starts at byte 1.
		private byte[] GetEncryptedData(byte[] buffer)
		{
			byte[] encryptedData = null;
			if (buffer.Length > 0)
			{
				if (buffer[0] == 1)
				{
					encryptedData = new byte[buffer.Length - (1 + 512 + 256)];
					for (int i = 0; i < buffer.Length - (1 + 512 + 256); i++)
					{
						encryptedData[i] = buffer[i + (1 + 512 + 256)];
					}
				}
				else
				{
					encryptedData = new byte[buffer.Length - 1];
					for (int i = 0; i < buffer.Length - 1; i++)
					{
						encryptedData[i] = buffer[i + 1];
					}

				}
			}
			return encryptedData;

		}

		//Extract signature hash from input buffer, where it is located between bytes 513 and 768
		private byte[] GetSignature(byte[] buffer)
		{
			byte[] signatureBytes = null;
			if (buffer.Length > 0)
			{
				if (buffer[0] == 1)
				{
					signatureBytes = new byte[256];
					for (int i = 0; i < 256; i++)
					{
						signatureBytes[i] = buffer[i + 1 + 512];
					}

				}
			}
			return signatureBytes;

		}

		//Extract certificate name from input buffer, where it is located between bytes 1 and 512
		private string GetCertificateName(byte[] buffer)
		{
			string certificateName = null;
			if (buffer.Length > 0)
			{

				if (buffer[0] == 1)
				{
					byte[] certificateBytes = new byte[512];
					for (int i = 0; i < 512; i++)
					{
						certificateBytes[i] = buffer[i + 1];
					}

					certificateName = System.Text.Encoding.UTF8.GetString(certificateBytes);
				}
			}
			return certificateName;

		}

		// Verifies digital signature. If first input buffer byte is set then data is signed.
		// buffer[1-512] has the certificate name to use for verification
 		// buffer[513-768] has the signature
		// remaining buffer area contains encrypted data.
		// return code :
		//				0 = No data found or exception thrown while processing data 
 		//				1 = data found. Signature verified.
		//				-1= data found. Signature failed verification. Allow user to decide whether to save or not.
		private short VerifySignature(byte []buffer, byte[] encryptedData, out string message)
		{
			short code = 0;//No data found or exception thrown while processing data 
			message = "Failed to import queries.";
			try
			{
				if (buffer.Length > 0)
				{

					if (buffer[0] == 1)
					{

						byte[] signatureBytes = this.GetSignature(buffer);
						if (signatureBytes == null)
						{
							return code;
						}

						string certificateName = this.GetCertificateName(buffer);
						if (string.IsNullOrEmpty(certificateName))
						{
							return code;
						}

						using (RSACertificate theCert = new RSACertificate(certificateName))
						{
							var p = new RSACryptoServiceProvider();
							RSAParameters rp = new RSAParameters();
							rp = ((RSACryptoServiceProvider)theCert.Certificate.PrivateKey).ExportParameters(true);
							p.ImportParameters(rp);
							p.PersistKeyInCsp = false;
							if (p.VerifyData(encryptedData, new SHA256CryptoServiceProvider(), signatureBytes))
							{
								code = 1;//Verified
							}
							else
							{
								message = "Signature verification failed. Failed to import queries.";
								return code;
							}
						}

					}
					else
					{
						code = -1; //Warning
						message = "Import file not signed. Source cannot be authenticated.";
					}

				}

				else
				{
					//Empty input
					message = "No data found to import.";
				}			
			}
			catch (Exception e)
			{
				LogErrorMessage(e.Message);
			}
			return code;
		}
		private void PopulateQueries(string input)
		{
			if (!string.IsNullOrEmpty(input))
			{
				try
				{
					XDocument	document = XDocument.Parse(input);
					IEnumerable<XElement> newQuery = from f in document.Descendants("FuelsManager.Query") select f;

					ConfigurationSettingDOClass setting = FMChannelHelper.MakeCall<IConfigurationSettings, ConfigurationSettingDOClass>(x => x.GetByKey(Security, ConfigurationSettingDOClass.Key_QueryWriterAssemblies));

					FMChannelHelper.MakeCall<IQueries>(queries =>
					{

						foreach (XElement xmlQuery in newQuery)
						{
							var queryElement = new XElement("FuelsManager.Queries");
							queryElement.Add(xmlQuery);

							var query = new QueryClass();
							query.ReadXML(this.Security, xmlQuery.ToString(), setting.SettingValue);

							this.DetermineUniqueQueryName(queries, query);
							this.DetermineUniqueQueryMenuPath(queries, query);

							queries.Add(this.Security, query);
						}
					}

					);

				}
				catch (Exception except)
				{
					this.ErrorHandler(except);
				}


			}
		}


		private void SaveManagedQueriesClick()
		{
			string input = Session[IMPORTED_QUERY_XDOCUMENT] as string;
			if (input != null)
			{
				try
				{
					PopulateQueries(input);

				}
				catch (Exception except)
				{
					this.ErrorHandler(except);
				} 
			}
			Session.Remove(IMPORTED_QUERY_XDOCUMENT);
		}

		private void DropManagedQueriesClick()
		{
			Session.Remove(IMPORTED_QUERY_XDOCUMENT);
		}

		private void ValidateImportFile(HttpPostedFile file, bool usePlainText)
		{
			if (string.IsNullOrEmpty(file.FileName) || file.ContentLength == 0)
			{
				throw new ApplicationException("Query import file name cannot be blank");
			}

			var fileExtension = Path.GetExtension(file.FileName);
			if (usePlainText && !fileExtension.Equals(".xml"))
			{
				throw new ApplicationException("Query import file extension must be '.xml'");
			}

			if (!usePlainText && !fileExtension.Equals(".vef"))
			{
				throw new ApplicationException("Query import file extension must be '.vef'");
			}
		}

		/// <summary>
		/// Handles the RowCreated event of the QueryGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewRowEventArgs"/> instance containing the event data.</param>
		private void QueryGridRowCreated(object sender, GridViewRowEventArgs e)
		{
			try
			{
				// If the user doesn't have modify rights, disable the edit and delete buttons
				if (this.Security.HasRight(RIGHT.MODIFY_QUERIES) == false)
				{
					if (e.Row.RowType == DataControlRowType.DataRow)
					{
						var editButton = e.Row.FindControl("EditButton") as FMEditLinkButton;
						if (editButton != null)
						{
							editButton.Enabled = false;
						}
						else
						{
							throw new Exception("Could not locate the edit button to disable it");
						}
					}
				}

				if (e.Row.RowType == DataControlRowType.DataRow)
				{
					var queryCollection = (QueryCollectionClass)QueryGrid.DataSource;

					if (queryCollection != null)
					{
						QueryClass query = queryCollection[e.Row.RowIndex];
						if (Security.HasRight(RIGHT.MODIFY_QUERIES) == false || query.SystemQuery)
						{
							var deleteButton = e.Row.FindControl("DeleteButton") as FMDeleteLinkButton;

							if (deleteButton != null)
							{
								deleteButton.Enabled = false;
							}
							else
							{
								throw new Exception("Could not locate the delete button to disable it");
							}
						}
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the RowDataBound event of the QueryGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewRowEventArgs"/> instance containing the event data.</param>
		private void QueryGrid_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				var edit = (FMEditLinkButton)e.Row.FindControl("EditButton");
				if (edit != null)
				{
					edit.CommandArgument = e.Row.RowIndex.ToString(CultureInfo.InvariantCulture);
				}

				var view = (FMViewLinkButton)e.Row.FindControl("ViewButton");
				if (view != null)
				{
					view.CommandArgument = e.Row.RowIndex.ToString(CultureInfo.InvariantCulture);
				}

				var delete = (FMDeleteLinkButton)e.Row.FindControl("DeleteButton");
				if (delete != null)
				{
					delete.CommandArgument = e.Row.RowIndex.ToString(CultureInfo.InvariantCulture);
				}
			}
		}

		#endregion
	}
}