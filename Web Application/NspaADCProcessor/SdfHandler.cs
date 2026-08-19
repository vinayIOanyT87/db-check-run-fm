// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlCeHelper.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the SqlCeHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nspa
{
	using System;
	using System.Collections.Specialized;
	using System.Diagnostics;
	using System.IO;
	using System.Web;

	using ADC.Nspa.General;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// This handles the sdf file download request
	/// Currently, it doesn't matter what the filename or path is.  It only cares about the user ID, password, siteId and file ID.
	/// </summary>
	public class SdfHandler : IHttpHandler
	{
		/// <summary>
		/// You will need to configure this handler in the Web.config file of your 
		/// web and register it with IIS before being able to use it. For more information
		/// see the following link: http://go.microsoft.com/?linkid=8101007
		/// </summary>
		#region IHttpHandler Members

		public bool IsReusable
		{
			// Return false in case your Managed Handler cannot be reused for another request.
			// Usually this would be false in case you have some state information preserved per request.
			get { return false; }
		}

		/// <summary>
		/// Matches the key and assign the value
		/// </summary>
		/// <param name="formVariables">The form variables.</param>
		/// <param name="index">The index.</param>
		/// <param name="isAMatch">if set to <c>true</c> [is a match].</param>
		/// <param name="valueString">The value string.</param>
		/// <returns></returns>
		public bool MatchAndAssign(NameValueCollection formVariables, int index, string key, ref string valueString)
		{
			var isAMatch = string.Equals(formVariables.GetKey(index), key, StringComparison.InvariantCultureIgnoreCase);
			if (isAMatch)
			{
				valueString = formVariables.Get(index);
			}
			return isAMatch;
		}

		/// <summary>
		/// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler" /> interface.
		/// </summary>
		/// <param name="context">An <see cref="T:System.Web.HttpContext" /> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
		public void ProcessRequest(HttpContext context)
		{
			var id = String.Empty;
			var password = String.Empty;
			var siteId = string.Empty;
			var fileId = string.Empty;
			var theRequest = context.Request;
			var theResponse = context.Response;
			var theForm = theRequest.Form;
			var parameterCount = theForm.Count;
			var errorString = string.Empty;

			
			try
			{
				for (var index = 0; index < parameterCount; index++)
				{
					// The password is passed as Note in case somewhere some requirement mentioned that you can use that.
					var isAMatch = MatchAndAssign(theForm, index, "ID", ref id) 
									|| MatchAndAssign(theForm, index, "Note", ref password)
									|| MatchAndAssign(theForm, index, "SiteID", ref siteId)
									|| MatchAndAssign(theForm, index, "FileID", ref fileId);
				}

				SecurityClass mySecurity;
				if (Helper.Login(id, password, siteId, out mySecurity, out errorString))
				{
					var filePath = SqlCeHelper.GetDbPathStatic(fileId);
					using (var imageFile = File.OpenRead(filePath))
					{
						imageFile.CopyTo(theResponse.OutputStream);
					}
					File.Delete(filePath);
				}
			}
			catch (Exception error)
			{
				errorString = string.Format("Error:{0}. ({1},{2},{3})",error.Message, id,password,fileId);
			}

			if (!string.IsNullOrWhiteSpace(errorString))
			{
                Helper.NspaADCEventLog.WriteEntry(errorString, EventLogEntryType.Error);
				theResponse.StatusCode = (int) DownloadFileInfo.CustomHttpErrorStatusCode;
				theResponse.ClearContent();
				theResponse.Write(errorString);
			}
		}

		#endregion
	}
}
