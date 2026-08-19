namespace FuelsManager.Areas.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Helpers;
	using System.Web.Mvc;

   using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.UtilityObjects;

	using FuelsManager.FMWebApp;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	[HandleError(ExceptionType = typeof(HttpAntiForgeryException), View = "Unauthorised")]
	public class FMBaseController : Controller
	{
		 public const string IgnoreOnServiceAccountKey = "FMBaseController.SessionIgnoreOnServiceAccount";
		protected SecurityClass Security { get; set; }

		protected bool UseDataDictionary { get; set; }

		public static string CSRFToken
		{
			get
			{
				var csrfToken = System.Web.HttpContext.Current.Session["CSRFToken"] as string;
				if (csrfToken != null)
				{
					return csrfToken;
				}
				return string.Empty;
			}
		}
		protected int SessionStatus = 0;
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			try
			{
            SessionStatus = 0;
            if (this.Session == null)
				{
               SessionStatus = -1;

               return;
				}

            // The FMBaseController needs to ignore processing when the request contains the
            // Account/Login URL until the user is logged in and a security object is created.
            // This occurs when IIS is set to Window Authentication is enabled. 
            var ignoreOnSvrAccount = this.Session[IgnoreOnServiceAccountKey] as string;
				if (ignoreOnSvrAccount != null && ignoreOnSvrAccount.Equals("TRUE"))
				{
					this.Session.Remove(IgnoreOnServiceAccountKey);
					return;
				}

				base.OnActionExecuting(filterContext);

				this.GetSecurityObject();

				this.UseDataDictionary = FMFormBase.GetDataDictionaryFlag();
				var security = this.Session["Security"] as SecurityClass;

				if (security == null) 
				{
					throw new FMSessionInvalidException();
				}
				this.ViewBag.MenuUrl = "~/MenuBar/FMMenuBar.aspx?" + security.CSRFTokenWithParamName;

				if ( ShouldValidateAntiForgeryTokenManually(filterContext) )
				{
					var request = filterContext.HttpContext.Request;
					if (request.IsAjaxRequest())
					{
						var antiForgeryCookie = request.Cookies[AntiForgeryConfig.CookieName];
						var cookieValue = antiForgeryCookie != null ? antiForgeryCookie.Value : null;
						AntiForgery.Validate(cookieValue, request.Headers["__RequestVerificationToken"]);
					}
					else
					{
						var authorizationContext = new AuthorizationContext(
							filterContext.Controller.ControllerContext,
							filterContext.ActionDescriptor);

						// Use the authorization of the anti forgery token,
						// which can't be inhereted from because it is sealed
						new ValidateAntiForgeryTokenAttribute().OnAuthorization(authorizationContext);
					}
				}
			}
         catch (FMSessionInvalidException sessionInvalidException)
			{
            SessionStatus = -1;
            this.ErrorHandler(sessionInvalidException);
				filterContext.Result = this.RedirectToAction("SessionInvalidIndex", "SessionInvalid", new {area = "MainArea"} );
			}
			catch (HttpAntiForgeryException antiForgeryException)
			{
				this.ErrorHandler(antiForgeryException);
				filterContext.Result = this.Content(antiForgeryException.Message);
			}
			catch (Exception except)
			{
				if (except.Message == FMSessionInvalidException.SessionTimedOutExceptionMessage)
				{
               SessionStatus = -2;
            }
            else
				{
               SessionStatus = -1;
				}
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// We should validate the anti forgery token manually if the following criteria are met:
		/// 1. The http method must be POST
		/// 2. There is not an existing [ValidateAntiForgeryToken] attribute on the action
		/// 3. There is no [BypassAntiForgeryToken] attribute on the action
		/// </summary>
		private static bool ShouldValidateAntiForgeryTokenManually( ActionExecutingContext filterContext )
		{
			if (Global.ComplianceFlag)
			{
				return false;
			}
			var httpMethod = filterContext.HttpContext.Request.HttpMethod;
			//1. The http method must be POST
			if ( httpMethod != "POST" ) return false;

			// 2. There is not an existing anti forgery token attribute on the action
			var antiForgeryAttributes = filterContext.ActionDescriptor.GetCustomAttributes( typeof( ValidateAntiForgeryTokenAttribute ), false );

			if ( antiForgeryAttributes.Length > 0 ) return false;

			return true;
		}

		/// <summary>
		/// Gets the security object.
		/// </summary>
		private void GetSecurityObject()
		{
			this.Security = FindSecurityObject();

			// Log the session memory information if the Configuration setting "LogSessionMemoryState" is set to "1".
			FMFormBase.LogSessionInfo(this.Security);
		}

		private static SecurityClass FindSecurityObject()
		{
			var session = System.Web.HttpContext.Current.Session ?? throw new FMSessionInvalidException();
			var security = session["Security"] as SecurityClass;

			if (security == null)
			{
				if (session["Token"] != null)
				{
					var token = session["Token"].ToString();
					if (string.IsNullOrEmpty(token))
					{
						throw new FMSessionInvalidException();
					}

					security = FMChannelHelper.MakeCall<ISites, SecurityClass>(x => x.GetSecurity(token));
					session["Security"] = security ?? throw new FMSessionInvalidException();
					session["Token"] = security.Token;
				}
			}
			else
			{
				if (session["Token"] != null)
				{
					var token = session["Token"].ToString();
					if (token != security.Token.ToString())
					{
						return FindSecurityObject();
					}
				}

				FMChannelHelper.MakeCall<ISessions>(x => x.PingSession(security));
				if (security == null)
				{
					throw new FMSessionInvalidException();
				}			
			}

			if (security != null)
			{
				session["SiteGuid"] = security.SiteGuid;
				session["LoginSiteGuid"] = security.LoginSiteGuid;
			}
			else
			{
            session.Remove("SiteGuid");
            session.Remove("LoginSiteGuid");
         }

         return security;
		}

		/// <summary>
		/// Handles uncaught exceptions.
		/// </summary>
		/// <param name="filterContext"></param>
		protected override void OnException(ExceptionContext filterContext)
		{
			FMFormBase.LogErrorMessage(
				"Unhandled Exception: " + filterContext.Exception.Message + "\n\nStack Trace\n" + filterContext.Exception.StackTrace);

         var vr = new ViewResult
         {
            ViewName = "~/Areas/Views/Shared/Error.cshtml",
         }; 

			if (this.Session != null)
			{
				this.Session["Status"] = "Error";
			}
        
         // Display the error
         var errorMessage = filterContext.Exception.Message;
			try
			{
				errorMessage = this.GetErrorMessageText("FuelsManager", filterContext.Exception.Message);
			}
			catch
			{
            errorMessage = filterContext.Exception.Message;
         }				
			string htmLerrorMessage = errorMessage.Replace("\n", " ");
			htmLerrorMessage = htmLerrorMessage.Replace("--->", "");
			htmLerrorMessage = htmLerrorMessage.Replace("\r", " ");
			htmLerrorMessage = htmLerrorMessage.Replace(@"\\\\", @"\\");

			vr.ViewBag.FMErrorMessage = htmLerrorMessage;

         filterContext.Result = vr;
         filterContext.ExceptionHandled = true;

      }

      protected override void HandleUnknownAction(string actionName)
      {
         string msg = string.Format( "Unknown action : {0}   path : {1}    referrer : {2}.", actionName,this.Request.Url.AbsolutePath, this.Request.UrlReferrer.AbsolutePath);
         Global.WriteToEventLog(msg, System.Diagnostics.EventLogEntryType.Warning);
       //  this.Response.Redirect(Url.Content("~/FMWebApp/LogoutForm.aspx"));
      }

      protected ActionResult ErrorHandler(string key, string message)
		{
			return this.Content("Error messaging not fully implemented.  Got: " + message);
		}

		protected ActionResult ErrorHandler(string key, Exception except)
		{
			return this.Content("Error messaging not fully implemented.  Got: " + except.Message);
		}

		[NonAction]
		protected string ErrorHandler(Exception except)
		{
			// Process error message
			var errorMessage = this.GetErrorMessageText("FuelsManager", except.Message);
			string htmLerrorMessage = errorMessage.Replace("\n", " ");
			htmLerrorMessage = htmLerrorMessage.Replace("--->", "");
			htmLerrorMessage = htmLerrorMessage.Replace("\r", " ");
			htmLerrorMessage = htmLerrorMessage.Replace(@"\\\\", @"\\");

			// TLog the error
			FMFormBase.LogErrorMessage(errorMessage + "\n\nStack Trace\n" + except.StackTrace);
			if (this.Session != null)
			{
				this.Session["Status"] = "Error";
			}

			// Display the error
			this.ViewBag.FMErrorMessage = htmLerrorMessage;
			return htmLerrorMessage;
		}

		[NonAction]
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
			var errorMessage = this.GetTranslatedText(message);
			errorMessage = referenceName + " : " + errorMessage;
			return errorMessage;
		}

		public static string TranslatedText(string originalText, SecurityClass security, bool useDataDictionary)
		{
			string returnText = originalText;

			if (useDataDictionary)
			{
				if ((security != null))
				{
					Guid siteGuid = security.SiteGuid;
					returnText = DataDictionarySingleton.Get(siteGuid, originalText);
				}
			}
			else
			{
				returnText = new DataDictionaryCollectionClass()[originalText];
			}

			return returnText;
		}

		/// <summary>
		///		This function returns translated text if the "use data dictionary glossary" option is turned on; otherwise, it returns the OrignalText;
		/// </summary>
		/// <param name="originalText">
		/// </param>
		/// <returns>
		///		The text to use, translated as necessary.
		/// </returns>
		[NonAction]
		public string GetTranslatedText(string originalText)
		{
			return TranslatedText(originalText, this.Security, this.UseDataDictionary);
		}

		[NonAction]
		public static IEnumerable<SelectListItem> GetEnumSelectList(string typeString)
		{
			Type t = Type.GetType(typeString + ",FMBusinessObjects");
			if (t.IsEnum)
			{
				var list = new List<SelectListItem>();

				var values = Enum.GetValues(t).Cast<Enum>();

				foreach (var val in values)
				{
					list.Add(new SelectListItem()
								{
									Value = Convert.ToInt32(val).ToString(),
									//If Enum belongs to namespace FMBusinessObjects.DataObjects.CodedVariables then the strings are automatically in the Data Dictionary
									Text = TranslateText(FMBusinessObjects.DataObjects.CodedVariables.SelectList.CreateUIString(val))
								});

				}
				return list;

			}
			throw new ArgumentException("object type must be an enumerated type.");
		}

		[NonAction]
		public static IEnumerable<SelectListItem> GetEnumSelectList<T>() where T : struct, IConvertible
		{
			Type t = typeof(T);
			if (t.IsEnum)
			{
				var list = new List<SelectListItem>();

				var values = Enum.GetValues(t).Cast<Enum>();

				foreach (var val in values)
				{
					list.Add(new SelectListItem()
								{
									Value = Convert.ToInt32(val).ToString(),
									//If Enum belongs to namespace FMBusinessObjects.DataObjects.CodedVariables then the strings are automatically in the Data Dictionary
									Text = TranslateText(FMBusinessObjects.DataObjects.CodedVariables.SelectList.CreateUIString(val))
								});

				}
				return list;

			}
			throw new ArgumentException("<T> must be an enumerated type.");
		}

        [NonAction]
        public static IEnumerable<SelectListItem> GetEnumSelectListRaw<T>() where T : struct, IConvertible
        {
            Type t = typeof(T);
            if (t.IsEnum)
            {
                var list = new List<SelectListItem>();

                var values = Enum.GetValues(t).Cast<Enum>();

                foreach (var val in values)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = Convert.ToInt32(val).ToString(),
                        //If Enum belongs to namespace FMBusinessObjects.DataObjects.CodedVariables then the strings are automatically in the Data Dictionary
                        Text = FMBusinessObjects.DataObjects.CodedVariables.SelectList.CreateUIString(val)
                    });

                }
                return list;

            }
            throw new ArgumentException("<T> must be an enumerated type.");
        }

        [NonAction]
		public static string TranslateText(string originalText)
		{
			var translatedText = originalText;

			var useDataDictionary = FMFormBase.GetDataDictionaryFlag();

			var siteGuid = (Guid)System.Web.HttpContext.Current.Session["SiteGuid"];
			
			if (useDataDictionary && siteGuid != Guid.Empty)
			{
				translatedText = DataDictionarySingleton.Get(siteGuid, originalText);
			}
			else
			{
				translatedText = new DataDictionaryCollectionClass()[originalText];
			}

			return translatedText;
		}

		protected ActionResult RedirectToAspx(string url)
		{
			if (url.Contains("CSRFToken") == false && this.Session != null)
			{
				var security = this.Session["Security"] as SecurityClass;
				if (url.Contains("?"))
				{
					url += "&" + security.CSRFTokenWithParamName;
				}
				else
				{
					url += "?" + security.CSRFTokenWithParamName;

				}

			}

			return this.RedirectWithPleaseWait(url);
		}

		protected ActionResult RedirectWithPleaseWait( string url )
		{
			this.ViewBag.Url = Url.Content(url);
			return this.View("~/Areas/Views/Shared/RedirectWithPleaseWait.cshtml");
		}

		public ActionResult Unauthorised()
		{
			return View("Unauthorised");
		}

		public static string GetEngineeringUnitsString(EngineeringUnit unit)
		{
			return EngineeringUnits.GetUnitAbbreviation(unit);
		}
	}
}
