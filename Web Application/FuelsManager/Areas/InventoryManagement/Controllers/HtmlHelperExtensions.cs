using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FuelsManager.Areas.InventoryManagement.Controllers
{
    using System.Web.Mvc;

    public static class HtmlHelperExtensions
    {

        // It will set the control being created to disable
        public static MvcHtmlString Disable(this MvcHtmlString helper, bool disabled)
        {
            if (helper == null)
                throw new ArgumentNullException();

            if (disabled)
            {
                string html = helper.ToString();
                int startIndex = html.IndexOf('>');

                html = html.Insert(startIndex, " disabled=\"disabled\"");
                return MvcHtmlString.Create(html);
            }

            return helper;
        }
    }

 
}