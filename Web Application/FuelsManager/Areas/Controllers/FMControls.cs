// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMControls.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Control helpers for FuelsManager MVC implementations
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Areas.Controllers
{
    using Microsoft.Ajax.Utilities;
    using System;
	using System.Web.Mvc;
	using System.Web.Mvc.Html;

	public static class FMControls
	{
		#region Public Methods and Operators

		/// <summary>
		/// Fms the button.
		/// </summary>
		/// <param name="html">The object for "this"</param>
		/// <param name="name">The name of the control.</param>
		/// <param name="text">The text to display on the control.</param>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="title">The title/helper text for the control.</param>
		/// <returns></returns>
		public static MvcHtmlString FMButton(this HtmlHelper html, string id, string name, string text, bool enabled, string title)
		{
			return MvcHtmlString.Create(GenerateButton("formfieldtitle", id, name, text, enabled, title, "submit"));
		}

		public static MvcHtmlString FMButtonSubdued(this HtmlHelper html, string id, string name, string text, bool enabled, string title)
		{
			return MvcHtmlString.Create(GenerateButton("formfield", id, name, text, enabled, title, "submit"));
		}

		public static MvcHtmlString FMButton(this HtmlHelper html, string id, string name, string text, bool enabled, string title, string backgroundcolor, string textcolor)
		{
			return MvcHtmlString.Create(GenerateButton("formfieldtitle", id, name, text, enabled, title, "submit", backgroundcolor, textcolor));
		}

		public static MvcHtmlString FMButton(this HtmlHelper html, string id, string name, string text, bool enabled, string title, string subclass)
		{
			return MvcHtmlString.Create(GenerateButton("formfield", id, name, text, enabled, title, "submit", subclass));
		}

		public static MvcHtmlString FMButtonIdAsCommand(this HtmlHelper html, string id, string name, string text, bool enabled, string title)
		{
			return MvcHtmlString.Create(GenerateButtonIdAsCommand("formfieldtitle", id, name, text, enabled, title, "submit"));
		}

		/// <summary>
		/// Fms the button.
		/// </summary>
		/// <param name="html">The object for "this"</param>
		/// <param name="name">The name of the control.</param>
		/// <param name="text">The text to display on the control.</param>
		/// <param name="enabled">if set to <c>true</c> [enabled].</param>
		/// <param name="title">The title/helper text for the control.</param>
		/// <param name="submit">do you want to automaticall submit the form?</param>
		/// <returns></returns>
		public static MvcHtmlString FMButton(this HtmlHelper html, string id, string name, string text, bool enabled, string title, bool submit)
        {
            string buttonType = "submit";
            if (!submit) { buttonType = "button"; }

            return MvcHtmlString.Create(GenerateButton("formfieldtitle", id, name, text, enabled, title, buttonType));
        }

        public static MvcHtmlString FMButtonSubdued(this HtmlHelper html, string id, string name, string text, bool enabled, string title, bool submit)
        {
            string buttonType = "submit";
            if (!submit) { buttonType = "button"; }
            return MvcHtmlString.Create(GenerateButton("formfield", id, name, text, enabled, title, buttonType));
        }


        public static MvcHtmlString FMConfirmationSubmitButton( this HtmlHelper html, string name, bool enabled )
		{
			if ( enabled )
			{
				var button = "<button class=\"formfieldtitle\" name=\"" + name
							 + "\" onclick=\" if (disabled) return; return confirm('Are you sure you wish to perform this operation?');\"></button>";

				return MvcHtmlString.Create( button );
			}

			return MvcHtmlString.Create( "Operation not supported." );
		}

		public static MvcHtmlString FMDeleteSubmitButton( this HtmlHelper html, string name, Guid entityGuid, bool enabled )
		{
			if (enabled)
			{
				var button = "<button class=\"deleteButton\" name=\"" + name + "\" value=\"" + entityGuid
							 + "\" onclick=\" return confirm('Are you sure you wish to delete this item?');\"></button>";

				return MvcHtmlString.Create( button );
			}

			return MvcHtmlString.Create(string.Format("<img  title=\"Delete not supported\" src=\"{0}/fmwebapp/images/delete_un.gif\" />",
                    html.ViewContext.HttpContext.Request.ApplicationPath));
		}

		public static MvcHtmlString FMDeleteButton( this HtmlHelper html, string deleteText, Guid entityGuid, bool enabled )
		{
			if (enabled)
			{
				return html.ActionLink(
					@deleteText,
					"Delete",
					new { id = entityGuid },
					new
					{
						onclick = "return confirm('Are you sure you wish to delete this item?');",
						@class = "deleteLinkClass"
					});
			}

			return MvcHtmlString.Create(string.Format("<img title=\"Delete not supported\" src=\"{0}/fmwebapp/images/delete_un.gif\" />",
                html.ViewContext.HttpContext.Request.ApplicationPath));
		}

		#endregion

		#region Methods

		private static string GenerateButton(string cssClass, string id, string name, string text, bool enabled, string title, string type)
		{

			string disabled = enabled ? string.Empty : "disabled=\"disabled\"";

			string value = FMBaseController.TranslateText(text);

			string accessKey = GetAccessKey(value);
			if (string.IsNullOrEmpty(accessKey) == false)
			{
				text = value.Replace("&" + accessKey, "<u>" + accessKey + "</u>");
				accessKey = "accessKey=\"" + accessKey + "\"";
			}

			const string BaseControl = "<button id=\"{7}\" name=\"command\" type=\"{6}\" value=\"{5}\" {1} {2} class=\"{3} pushButton\" title=\"{4}\" background-color: #4CAF50>{0}</button>";

			return string.Format( BaseControl, text, disabled, accessKey, cssClass, title, name, type, id);
		}

		private static string GenerateButtonIdAsCommand(string cssClass, string id, string name, string text, bool enabled, string title, string type)
		{

			string disabled = enabled ? string.Empty : "disabled=\"disabled\"";

			string value = FMBaseController.TranslateText(text);

			string accessKey = GetAccessKey(value);
			if (string.IsNullOrEmpty(accessKey) == false)
			{
				text = value.Replace("&" + accessKey, "<u>" + accessKey + "</u>");
				accessKey = "accessKey=\"" + accessKey + "\"";
			}

			const string BaseControl = "<button id=\"{7}\" name=\"{8}\" type=\"{6}\" value=\"{5}\" {1} {2} class=\"{3} pushButton\" title=\"{4}\" background-color: #4CAF50>{0}</button>";

			return string.Format(BaseControl, text, disabled, accessKey, cssClass, title, name, type, id, id);
		}

		private static string GenerateButton(string cssClass, string id, string name, string text, bool enabled, string title, string type,string subclass)
		{

			string disabled = enabled ? string.Empty : "disabled=\"disabled\"";

			string value = FMBaseController.TranslateText(text);

			string accessKey = GetAccessKey(value);
			if (string.IsNullOrEmpty(accessKey) == false)
			{
				text = value.Replace("&" + accessKey, "<u>" + accessKey + "</u>");
				accessKey = "accessKey=\"" + accessKey + "\"";
			}

			const string BaseControl = "<button id=\"{7}\" name=\"command\" type=\"{6}\" value=\"{5}\" {1} {2} class=\"{3} pushButtonWithTrashCan\" title=\"{4}\" background-color: #4CAF50>{0}</button>";

			return string.Format(BaseControl, text, disabled, accessKey, cssClass, title, name, type, id);
		}

		private static string GenerateButton(string cssClass, string id, string name, string text, bool enabled, string title, string type, string backgroundcolor, string textcolor)
		{

			string disabled = enabled ? string.Empty : "disabled=\"disabled\"";

			string value = FMBaseController.TranslateText(text);

			string accessKey = GetAccessKey(value);
			if (string.IsNullOrEmpty(accessKey) == false)
			{
				text = value.Replace("&" + accessKey, "<u>" + accessKey + "</u>");
				accessKey = "accessKey=\"" + accessKey + "\"";
			}

			const string BaseControl = "<button id=\"{7}\" name=\"command\" type=\"{6}\" style=\"color: {9}; background:{8}\" value=\"{5}\" {1} {2} class=\"{3} pushButton\" title=\"{4}\" background-color: #4CAF50>{0}</button>";

			return string.Format(BaseControl, text, disabled, accessKey, cssClass, title, name, type, id, backgroundcolor, textcolor);
		}

		public static string GetAccessKey(string text)
		{
			string result = string.Empty;
			int ampersandIndex = text.IndexOf('&');

			if (ampersandIndex >= 0 && ampersandIndex < text.Length - 1)
			{
				result = text.Substring(ampersandIndex + 1, 1);
			}

			return result;
		}

        public static string FormatButtonText(string message)
        {

            var formattedMessage = message;
            string accessKey = GetAccessKey(message);
            if (string.IsNullOrEmpty(accessKey) == false)
            {
                formattedMessage = message.Replace("&" + accessKey, "<u>" + accessKey + "</u>");
            }
            return formattedMessage;

        }

        #endregion
    }
}