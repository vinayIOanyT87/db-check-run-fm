// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMFormBase.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMFormBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Channels.Tcp;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.UI;
    using System.Web.UI.WebControls;
	 using System.Configuration;

	using AjaxControlToolkit;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;
    using FMBusinessObjects.Interfaces;
    using FMBusinessObjects.UtilityObjects;

    using FMControls;

    using FMCore;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    /// <summary>
	///    Primary base class for all FuelsManager forms.
	/// </summary>
	public partial class FMFormBase : Page
	{
		#region Constants and Fields
		public SecurityClass Security;

		protected int AviationBackgroundColor = 0x598dca;

		protected int DefenseBackgroundColor = 0x00269D;

		protected int OilAndGasBackgroundColor = 0x002854;

		protected bool useDataDictionary;

		protected bool versionCheckFailed;

		private static bool? usingLoadRack;

		private bool hasDisposed;

		private bool ignoreInputDisable;

		protected static int TextBoxDefaultMaxLength = 4096;

		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public FMFormBase ()
		{
			this.ignoreInputDisable = false;
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Get and Set the ignore input disable flag.
		/// </summary>
		public bool IgnoreInputDisable
		{
			get { return this.ignoreInputDisable; }
			set { this.ignoreInputDisable = value; }
		}

		/// <summary>
		///    Gets a value indicating whether we're using LoadRack or not.
		/// </summary>
		public static bool UsingLoadRack
		{
			get
			{
				if (!usingLoadRack.HasValue)
				{
				    usingLoadRack = true;
				}

				return usingLoadRack.Value;
			}
		}

		/// <summary>
		///    Gets the dispatch entity guid.
		/// </summary>
		public Guid DispatchEntityGuid => Guid.Parse(this.Request.GetQueryOrFormValue("DispatchEdit").DefaultIfNull(Guid.Empty.ToString()));

	    /// <summary>
		///    Gets a value indicating whether is from dispatch.
		/// </summary>
		public bool IsFromDispatch => this.Request.GetQueryOrFormValue("DispatchEdit").DefaultIfNull(string.Empty).Equals(string.Empty) == false;

		/// <summary>
		/// Gets a value indicating whether this instance is from client dispatch.
		/// </summary>
		public bool IsFromClientDispatch => this.Request.GetQueryOrFormValue("ClientDispatch").DefaultIfNull(string.Empty).Equals(string.Empty) == false;

        /// <summary>
		///    Gets a value indicating whether is from query writer.
		/// </summary>
		public bool IsFromQueryWriter => this.Request.GetQueryOrFormValue("QueryEdit").DefaultIfNull(string.Empty).Equals(string.Empty) == false;

		public bool IsEnterprise
		{
			get
			{
				return FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsEnterpriseKey());
			}
		}

		/// <summary>
		///    Gets the query entity guid.
		/// </summary>
		public Guid QueryEntityGuid => Guid.Parse(this.Request.GetQueryOrFormValue("QueryEdit").DefaultIfNull(Guid.Empty.ToString()));

	    /// <summary>
		///    Gets a value indicating whether session variable has an error or not.
		/// </summary>
		public bool SessionHasErrors => (this.Session["Status"] != null) && ((string)this.Session["Status"] == "Error");

	    /// <summary>
		/// Gets a value indicating whether ignore apostrophe during load.
		/// </summary>
		protected virtual bool IgnoreApostropheDuringLoad => false;

	    #endregion

		#region Public Methods and Operators

		/// <summary>
		///    Recursively iterates the child controls of a control to find
		///    the first instance of a System.Web.UI.UserControl
		/// </summary>
		/// <param name="ctl">Control to search in</param>
		/// <returns>Found user control, or null if none found</returns>
		public static UserControl FindFirstUserControl(Control ctl)
		{
		    var control = ctl as UserControl;
		    if (control != null)
			{
				return control;
			}

			foreach (Control childCtl in ctl.Controls)
			{
				UserControl foundCtl = FindFirstUserControl(childCtl);
				if (foundCtl != null)
				{
					return foundCtl;
				}
			}

			return null;
		}

		/// <summary>
		/// The log error message.
		/// </summary>
		/// <param name="errorMessage">
		/// The error message.
		/// </param>
		public static void LogErrorMessage(string errorMessage)
		{
			// Log the error in the application event log
			try
			{
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(errorMessage, FMEventLogEntryType.Error));
			}
			catch
			{
			    // ignored
			}
		}

		public static int GetSessionTimeout()
		{
			int returnValue = 20;

			try
			{
				var config = WebConfigurationManager.OpenWebConfiguration( HttpContext.Current.Request.ApplicationPath);
                var sss = config.GetSection("system.web/sessionState") as SessionStateSection;
				if (sss != null && sss.Timeout.TotalMinutes > 0)
				{
					returnValue = (int)sss.Timeout.TotalMinutes;
				}
			}
			// ReSharper disable once EmptyGeneralCatchClause
			catch
			{
			}

			return returnValue;
		}

		/// <summary>
		///    Clear Session Error
		/// </summary>
		public void ClearSessionErrors()
		{
			this.Session.Remove("Status");
		}

		public override void Dispose()
		{
			this.Dispose(true);

			// Use SupressFinalize in case a subclass 
			// of this type implements a finalizer.
			GC.SuppressFinalize(this);
		}

		/// <summary>
		///    Default error handler routine.
		/// </summary>
		/// <param name="except">
		///    The exception to log.
		/// </param>
		public void ErrorHandler(Exception except)
		{
			this.ErrorHandler("FuelsManager", except);
		}

		/// <summary>
		/// The error handler.
		/// </summary>
		/// <param name="referenceName">
		/// The reference name.
		/// </param>
		/// <param name="except">
		/// The except.
		/// </param>
		public virtual void ErrorHandler(string referenceName, Exception except)
		{
			// Channel factory timeout.  When this occurs there is no recovery by continuing to attempt to log errors
			if (except.Message.Contains("The open operation did not complete within the allotted timeout"))
			{
				return;
			}
			string logAndDisplayMessage = except.Message;

            // Log all the inner exceptions
			while (except.InnerException != null)
			{                
				except = except.InnerException;
                string logMessage = this.GetErrorMessageText(referenceName, except.Message);
                LogErrorMessage(logMessage);
			}


			// Process unhandled FMFatalErrorException type and if FuelsManager has been
			// shut down as a result then notify the user and stop all processing.
			var fatalErrorEx = except as FMFatalErrorException;
			if (fatalErrorEx != null)
			{
				if (this.Security == null)
				{
					this.GetSecurity();
				}

				bool shutdownFuelsManager = FMChannelHelper.MakeCall<IFMFatalErrorHandler, bool>(x => x.ProcessFatalError(this.Security, fatalErrorEx));

				if (shutdownFuelsManager)
				{
				    this.Response.Clear();
				    this.Response.Write(FMFatalErrorHandlerClass.Header);
					string notificationMessage = string.Format(FMFatalErrorHandlerClass.NotificationFormatter, fatalErrorEx.Message);
				    this.Response.Write(notificationMessage);
				    this.Response.Write(FMFatalErrorHandlerClass.Footer);
				    this.Response.End();
					return;
				}
			}

            // Log and display the outermost exception
            this.ErrorHandler(referenceName, logAndDisplayMessage, except.StackTrace);
		}

		/// <summary>
		/// The error handler.
		/// </summary>
		/// <param name="referenceName">
		/// The reference name.
		/// </param>
		/// <param name="message">
		/// The message.
		/// </param>
		public void ErrorHandler(string referenceName, string message)
		{
			this.ErrorHandler(referenceName, message, string.Empty);
		}

		/// <summary>
		///    Constructs a key based on the current page, or, the user control on the
		///    active tab (if applicable) in order to uniquely identify the context
		///    for bringing up context-sensitive help. Can be overridden for pages
		///    that need to further distinguish context
		/// </summary>
		/// <returns>Key for lookup into tblHelpMapping</returns>
		public virtual string GetHelpContextKey()
		{
			Type formType = this.GetType();

			FieldInfo[] fields = formType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

			// See if there is a tab control on the page
			foreach (FieldInfo fi in fields)
			{
				if (fi.FieldType == typeof(FMTabContainer))
				{
					var tabContainer = (FMTabContainer)fi.GetValue(this);

					UserControl foundCtl = FindFirstUserControl(tabContainer.ActiveTab);
					if (foundCtl != null)
					{
						// Found user control on active tab. Return path without the
						// leading "~/"
						return foundCtl.AppRelativeVirtualPath.Substring(2);
					}
				}
				else if (fi.FieldType == typeof(FMMenuTab))
				{
					// This logic was added to support pressing the help button from the 
					// Query Writer Create New Query page.
					// The Query Writer page uses a different type of tab control.
					// Ideally, it would use the FMTabContainer like everything else.
					var menuTab = (FMMenuTab)fi.GetValue(this);

					// The FMMenuTab control has a property that identifies the MultiView
					// The MultiView is what actually holds the controls we want to find.
					var multiView = (MultiView)this.FindControl(menuTab.MultiViewID);

					if (multiView != null)
					{
						// To identity the active tab we use the ActiveViewIndex of the MultiView control
						UserControl foundCtl = FindFirstUserControl(multiView.Views[multiView.ActiveViewIndex]);

						if (foundCtl != null)
						{
							// Found user control on active tab. Return path without the
							// leading "~/"
							return foundCtl.AppRelativeVirtualPath.Substring(2);
						}
					}
				}
			}

			// Return path of page without the leading "~/"
			return this.AppRelativeVirtualPath.Substring(2);
		}

		/// <summary>
		///    Constructs a key based on the current page, or, the user control on the
		///    active tab (if applicable) in order to uniquely identify the context
		///    for bringing up context-sensitive help. Can be overridden for pages
		///    that need to further distinguish context
		/// </summary>
		/// <returns>Key for lookup into tblHelpMapping</returns>
		public virtual List<string> GetHelpContextKeys()
		{
			var HelpKeys = new List<string>();
			Type formType = this.GetType();

			FieldInfo[] fields = formType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

			// See if there is a tab control on the page
			foreach (FieldInfo fi in fields)
			{
				if (fi.FieldType == typeof(FMTabContainer))
				{
					var tabContainer = (FMTabContainer)fi.GetValue(this);
					foreach (TabPanel tab in tabContainer.Tabs)
					{
						UserControl foundCtl = FindFirstUserControl(tab);
						if (foundCtl != null)
						{
							// Found user control on active tab. Return path without the
							// leading "~/"
							HelpKeys.Add(foundCtl.AppRelativeVirtualPath.Substring(2));
						}
					}
				}
				else if (fi.FieldType == typeof(FMMenuTab))
				{
					// This logic was added to support pressing the help button from the 
					// Query Writer Create New Query page.
					// The Query Writer page uses a different type of tab control.
					// Ideally, it would use the FMTabContainer like everything else.
					FMMenuTab menuTab = (FMMenuTab)fi.GetValue(this);

					// The FMMenuTab control has a property that identifies the MultiView
					// The MultiView is what actually holds the controls we want to find.
					MultiView multiView = (MultiView)this.FindControl(menuTab.MultiViewID);

					if (multiView != null)
					{
						// To identity the active tab we use the ActiveViewIndex of the MultiView control
						UserControl foundCtl = FindFirstUserControl(multiView.Views[multiView.ActiveViewIndex]);

						if (foundCtl != null)
						{
							// Found user control on active tab. Return path without the
							// leading "~/"
							HelpKeys.Add(foundCtl.AppRelativeVirtualPath.Substring(2));
							//Query writer needs to be hardcoded in
							if (foundCtl.AppRelativeVirtualPath.Substring(2) == "QueryWriterWebApp/QueryDefinitionBasic.ascx")
								HelpKeys.Add("QueryWriterWebApp/QueryDefinitionAdvanced.ascx");
						}
					}
				}
			}
			if (HelpKeys.Count == 0)
				HelpKeys.Add(this.AppRelativeVirtualPath.Substring(2));
			// Return path of page without the leading "~/"
			return HelpKeys;
		}

		/// <summary>
		/// The get load rack manager.
		/// </summary>
		/// <returns>
		/// The <see cref="ILoadRackManager"/>.
		/// </returns>
		/// <exception cref="SocketException">
		/// </exception>
		public ILoadRackManager GetLoadRackManager()
		{
			object loadRackInstalled;

			string strLoadRackInstalled =
				FMChannelHelper.MakeCall<IConfigurationSettings, string>(
					x => x.GetKeyValueByKey(this.Security, ConfigurationSettingDOClass.Key_LoadRackInstalled));

			if (string.IsNullOrEmpty(strLoadRackInstalled))
			{
				// Default to LoadRackInstalled in case the value is not present.
				loadRackInstalled = 1;
			}
			else
			{
				try
				{
					int isLoadRackInstalled = Convert.ToInt32(strLoadRackInstalled);

					if ((isLoadRackInstalled < 0) || (isLoadRackInstalled > 1))
					{
						loadRackInstalled = 0;
					}
					else
					{
						loadRackInstalled = isLoadRackInstalled;
					}
				}
				catch (Exception)
				{
					// Default to LoadRackInstalled in case the value is not present.
					loadRackInstalled = 1;
				}
			}

			if (loadRackInstalled != null && (int)loadRackInstalled == 0)
			{
				throw new SocketException(10061);
			}

			if (ChannelServices.GetChannel("tas") == null)
			{
				var channel = new TcpClientChannel("tas", new BinaryClientFormatterSinkProvider());
				ChannelServices.RegisterChannel(channel, true);
			}

			const string Hostname = "127.0.0.1";
			object port;

			string strPort =
				FMChannelHelper.MakeCall<IConfigurationSettings, string>(
					x => x.GetKeyValueByKey(this.Security, ConfigurationSettingDOClass.Key_LoadRackPort));

			if (string.IsNullOrEmpty(strPort))
			{
				port = 8087;
			}
			else
			{
				try
				{
					int portNumber = Convert.ToInt32(strPort);
					port = portNumber;
				}
				catch (Exception)
				{
					port = 8087;
				}
			}

			string url = "tcp://" + Hostname + ":" + ((int)port).ToString(CultureInfo.InvariantCulture) + "/LoadRackManager";
			return (ILoadRackManager)Activator.GetObject(typeof(ILoadRackManager), url);
		}

		/// <summary>
		///    Retrieves the security object from session storage and checks for valid session.
		/// </summary>
		/// <exception cref="FMSessionInvalidException">
		///    Thrown if session is found to be invalid.
		/// </exception>
		public void GetSecurity()
		{
			if (this.Session["Security"] != null)
			{
				this.Security = this.Session["Security"] as SecurityClass;
			}

			if (this.Security == null)
			{
				throw new FMSessionInvalidException();
			}

			this.Session["SiteGuid"] = this.Security.SiteGuid;

			// Log the session memory information if the Configuration setting "LogSessionMemoryState" is set to "1".
			LogSessionInfo(this.Security);
		}

		/// <summary>
		///    This function returns translated text if the "use data dictionary glossary" option is turned on; otherwise, it returns the OrignalText;
		/// </summary>
		/// <param name="originalText">
		/// </param>
		/// <returns>
		///    The System.String.
		/// </returns>
		public string GetTranslatedText(string originalText)
		{
			string returnText = originalText;

			if (this.useDataDictionary)
			{
				if ((this.Security != null))
				{
					Guid siteGuid = this.Security.SiteGuid;
					returnText = this.GetDataDictionaryValueByKey(siteGuid, originalText);
				}
			}
			else
			{
				returnText = new DataDictionaryCollectionClass()[originalText];
			}
			return returnText;
		}

		/// <summary>
		/// This method will log session memory information.
		/// </summary>
		static public void LogSessionInfo(SecurityClass security)
		{
			try
			{
				if ( AppSettingsHelper.GetKeyValue( "LogSessionMemoryState", defaultValue: false ) == false )
				{
					return;
				}

				string sessionOutput = string.Empty;
				long totalSessionBytes = 0;
				var binaryFormatter = new BinaryFormatter( );

				var session = HttpContext.Current.Session;

				foreach ( string key in session )
				{
					var obj = session[key];

					if (obj == null)
					{
						continue;
					}

					var memoryStream = new MemoryStream( );
					binaryFormatter.Serialize(memoryStream, obj);
					totalSessionBytes += memoryStream.Length;

					long memoryLength = memoryStream.Length / 1024;
					sessionOutput = sessionOutput + key + " - " + memoryLength + " kb\n";
				}

				totalSessionBytes = totalSessionBytes / 1024;
				sessionOutput = sessionOutput + "Total Size of Session Data: " + totalSessionBytes + " kb\n";

				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(sessionOutput, FMEventLogEntryType.Warning));
			}
			catch
			{
			    // ignored
			}
		}

		/// <summary>
		/// The get data dictionary value by key.
		/// </summary>
		/// <param name="siteGuid">
		/// The site GUID.
		/// </param>
		/// <param name="key">
		/// The key.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public string GetDataDictionaryValueByKey(Guid siteGuid, string key)
		{
			return DataDictionarySingleton.Get(siteGuid, key);
		}

		#endregion

		#region Methods

		protected virtual void Dispose(bool disposing)
		{
			if (this.hasDisposed == false)
			{
				if (disposing) { }
				this.hasDisposed = true;
			}
			base.Dispose();
		}

		/// <summary>
		///    Returns an url with the given query parameters
		/// </summary>
		/// <param name="url">
		/// </param>
		/// <param name="queryParameters">
		///    The query Parameters.
		/// </param>
		/// <returns>
		///    The System.String.
		/// </returns>
		protected virtual string FMFormatUrl(string url, NameValueCollection queryParameters)
		{
			if (string.IsNullOrWhiteSpace(url) || queryParameters == null)
			{
				throw new ApplicationException("Invalid url speciefied.");
			}

			// HttpValueCollection is .net internal class, you can really use it directly
			// we can declare var and ToString will format the parameters for you.
			NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);

			// all all parameters
			foreach (string parameterKey in queryParameters.Keys)
			{
				queryString.Add(parameterKey, queryParameters[parameterKey]);
			}

			// compose url
			return $"{url}?{queryString}";
		}

		/// <summary>
		///    Using the title text provided, apply the data dictionary to it if necessary, and if additional text is provided, append it to the title text. This can be used to get titles for forms with the ID of the bound object appended, for example, "Site Configuration - Site Admin"
		/// </summary>
		/// <param name="titleText">
		///    the default text of the title label of the page
		/// </param>
		/// <param name="textToAppend">
		///    text to append to the title, for example the ID of the bound object
		/// </param>
		/// <returns>
		///    The title text, with the data dictionary applied and text appended if it was provided
		/// </returns>
		protected string GetTitleLabelText(string titleText, string textToAppend)
		{
			string newTitleLabelText = this.GetTranslatedText(titleText);

			// if the provided text is not empty, append it to the title text
			if (!string.IsNullOrEmpty(textToAppend))
			{
				newTitleLabelText += " - " + textToAppend;
			}

            return HttpUtility.HtmlEncode(newTitleLabelText);
		}

		[SecurityCritical]
		protected void InitializeUnitsDropDownList(
			DropDownList unitsDropDownList,
			EngineeringUnit beginningUnits,
			EngineeringUnit endingUnits,
			EngineeringUnit selectedUnits)
		{
		    for (EngineeringUnit index = beginningUnits; index < endingUnits; index++)
			{
				if (Enum.IsDefined(typeof(EngineeringUnit), index) == false)
				{
					continue;
				}
				
				string abbrevString = EngineeringUnits.GetUnitAbbreviation(index);

				var newUnitsListItem = new ListItem(abbrevString, ((int)index).ToString());

				foreach (ListItem existingUnitsItem in unitsDropDownList.Items)
				{
					if (string.Compare(existingUnitsItem.Text, newUnitsListItem.Text, StringComparison.Ordinal) > 0)
					{
						int insert = unitsDropDownList.Items.IndexOf(existingUnitsItem);
						unitsDropDownList.Items.Insert(insert, newUnitsListItem);
						if (selectedUnits == index)
						{
							unitsDropDownList.SelectedIndex = insert;
						}

						newUnitsListItem = null;
						break;
					}
				}

				if (newUnitsListItem != null)
				{
					unitsDropDownList.Items.Add(newUnitsListItem);
					if (selectedUnits == index)
					{
						unitsDropDownList.SelectedIndex = unitsDropDownList.Items.Count - 1;
					}
				}
			}
		}

		protected override void OnInit(EventArgs e)
		{
			try
			{
				this.RegisterFormBaseScript();
				this.useDataDictionary = GetDataDictionaryFlag();
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
			}

			base.OnInit(e);
		}

		static public bool GetDataDictionaryFlag()
		{
			var dataDictionaryFlag = false;

			var session = HttpContext.Current.Session;

			if ( session["UseDataDictionary"] == null || (bool) session["UseDataDictionary"] )
			{
				dataDictionaryFlag = true;
			}

			return dataDictionaryFlag;
		}

		protected override void OnPreInit(EventArgs e)
		{
		}

		/// <summary>
		/// This method will register the disable input script for the page.
		/// </summary>
		protected void RegisterFormBaseScript()
		{

			if (this.ignoreInputDisable == false)
			{
				// This script disables input controls on post back so that the user cannot
				// invoke another event until the post back is completed.
				const string OnLoadScript = @"<script language=""javascript"" type=""text/javascript"">
										function DisableButtons(event)
										{
											// DisableButtons cannot be used with forms that perform export
											if(document.getElementById('DataDictionaryForm') != null){
												return;
											}
											var inputs = document.getElementsByTagName(""INPUT"");

											for (var i in inputs)
											{
												if (inputs[i].type == ""button"" || inputs[i].type == ""submit"")
												{
													if (inputs[i].id != ""FMM_cmdLogout""
													&& inputs[i].id != ""FMM_cmdHelp""
													&& inputs[i].id != ""OK""
													&& inputs[i].id != ""Cancel""
													&& inputs[i].id != ""New"")
													{
														inputs[i].disabled = true;
													}
												}
											}
										}

										window.onbeforeunload = DisableButtons;
										</script>";

				this.Page.ClientScript.RegisterStartupScript(this.GetType(), "ONLOAD", OnLoadScript);
			}

			var titleScript = @"<script language=""javascript"" type=""text/javascript"">
									document.title = 'FuelsManager';
								</script>";

			string tabTitle = ConfigurationManager.AppSettings["LoginPageWelcomeTitle"];

			if (string.IsNullOrEmpty(tabTitle) == false)
			{
				char doubleQuote = '"';
				titleScript = "<script language=" + doubleQuote + "javascript" + doubleQuote 
									+ " type=" + doubleQuote + "text/javascript" + doubleQuote + ">"
									+ " document.title = '" + tabTitle + "';"
								   + "</script>";
			}

			this.Page.ClientScript.RegisterStartupScript( this.GetType(), "ONLOAD2", titleScript);
		}

		/// <summary>
		///    Try to log page error
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Error(object sender, EventArgs e)
		{
			Exception error = this.Server.GetLastError();
			if (error != null)
			{
				Exception except = this.Server.GetLastError();
				do
				{
					System.Diagnostics.Trace.TraceError(except.ToString());
					except = except.InnerException;
				}
				while (except != null);
			}
			// should go to the Global application_error handler
		}

		/// <summary>
		///    Redirects navigation back to the login page after session timeout.
		/// </summary>
		protected virtual void RedirectAfterSessionTimeout()
		{
			this.Response.Write("<script language=\"JavaScript\">\r\n<!--\r\nwindow.top.location=\"../FMWebApp/LogoutForm.aspx\";\r\n-->\r\n</script>");
		}

		/// <summary>
		/// This method will render the error message on the current page.
		/// </summary>
		/// <param name="errorMessage">Contains the error message.</param>
		protected virtual void RenderErrorMessage(string errorMessage)
		{
			// suppress the display of this message, as the status for the service disruption is noted on the login form
			//The message could not be dispatched because the service at the endpoint address 'net.tcp://localhost/FMBusinessServices/IWebLinks.svc' is unavailable for the protocol of the address.
			if (errorMessage.Contains("The message could not be dispatched because the service at the endpoint address") &&
				errorMessage.EndsWith("/FMBusinessServices/IWebLinks.svc' is unavailable for the protocol of the address."))
			{
				return;
			}
			// Use setTimeout() so that all other content is rendered before message box appears
			// Doubly-encode inner string because it's a javascript call nested inside another one
			// Add a Guid to the key for RegisterStartupScript so that all messages get displayed
			string alertString = "setTimeout("
										+ HttpUtility.JavaScriptStringEncode(
											"alert(" + HttpUtility.JavaScriptStringEncode(errorMessage, true) + ")", true) + ", 0);\r\n";

			if (ScriptManager.GetCurrent(this) != null)
			{
				ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ErrorMessageScript" + Guid.NewGuid(), alertString, true);
			}
			else
			{
				this.ClientScript.RegisterStartupScript(this.GetType(), "ErrorMessageScript" + Guid.NewGuid(), alertString, true);
			}
		}

	    // ReSharper disable once InconsistentNaming
		protected void ScriptManager_AsyncPostBackError(object sender, AsyncPostBackErrorEventArgs e)
		{
			string message = "FuelsManager : " + e.Exception.Message;

			this.GetSecurity();

			if ((this.Security != null)
				 && (this.Page.Session["UseDataDictionary"] == null || (bool)this.Page.Session["UseDataDictionary"]))
			{
				message = this.GetTranslatedText(message);
			}

		    var scriptManager = ScriptManager.GetCurrent(this);
		    if (scriptManager != null)
		    {
		        scriptManager.AsyncPostBackErrorMessage = this.Server.HtmlEncode(message);
		    }
		}

        /// <summary>
        ///    Properly handles logging and reporting of error messages.
        /// </summary>
        /// <param name="referenceName">
        ///    The reference name to use.
        /// </param>
        /// <param name="message">
        ///    The message to report.
        /// </param>
        /// <param name="stackTrace">
        ///    The stack trace information to log.
        /// </param>
        private void ErrorHandler(string referenceName, string message, string stackTrace)
        {        
            string errorMessage = this.GetErrorMessageText(referenceName, message);

            string htmLerrorMessage = errorMessage.Replace("\n", " ");
            htmLerrorMessage = htmLerrorMessage.Replace("--->", "");
            htmLerrorMessage = htmLerrorMessage.Replace("\r", " ");
            htmLerrorMessage = htmLerrorMessage.Replace(@"\\\\", @"\\");
            this.RenderErrorMessage(htmLerrorMessage);

            this.Session["Status"] = "Error";

            if (message == FMSessionInvalidException.SessionNotFoundExceptionMessage
				|| message == FMSessionInvalidException.SessionTimedOutExceptionMessage)
            {
                this.RedirectAfterSessionTimeout();
            }

            LogErrorMessage(errorMessage + "\n\nStack Trace\n" + stackTrace);
        }

        private string GetErrorMessageText(string referenceName, string message)
        {
            if (string.IsNullOrEmpty(referenceName))
            {
                referenceName = "FuelsManager";
            }

            // Set to default message rather than throwing a new exception since we are 
            // presumably handling an existing exception.
            if (string.IsNullOrEmpty(message))
            {
                message = "== Message passed to error handler null! ==";
            }

			// Set initial message
			var errorMessage = this.GetTranslatedText( message );
            errorMessage = referenceName + " : " + errorMessage;
            return errorMessage;
        }
		#endregion
	}
}
