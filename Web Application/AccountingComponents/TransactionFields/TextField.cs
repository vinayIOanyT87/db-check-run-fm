using System.Web.UI;
using System.Web;
using System.Web.UI.WebControls;

namespace TransactionFields
{
    public class TextField : TextBox
    {
        // Overriding base TextBox render as MultiLine adds new line at start of textarea
        protected override void Render(HtmlTextWriter writer)
        {
            RenderBeginTag(writer);
            if (TextMode == TextBoxMode.MultiLine)
            {
                HttpUtility.HtmlEncode(Text, writer);
            }
            RenderEndTag(writer);
        }
    }

}

