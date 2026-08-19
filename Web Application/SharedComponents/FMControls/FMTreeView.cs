// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Varec, Inc." file="FMTreeView.cs">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// This class overcomes an issue with TreeView not rendering alt text for nodes with no children.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FMControls
{
	using System.IO;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	public class FMTreeView:TreeView
	{
		#region Methods

		//Override method to put "Node with no children" into the alt text in a node with null "" alt text. 
		protected override void Render(HtmlTextWriter writer)
		{
			StringBuilder sb = new StringBuilder();

			{
				using (StringWriter sw = new StringWriter(sb))
				using (HtmlTextWriter tw = new HtmlTextWriter(sw))
				{
					base.Render(tw);

					sw.Flush();

					sb.Replace("alt=\"\"", "alt=\"Node with no children\"");
					sb.Replace("<table ", "<table role=\"presentation\" aria-label=\"Tree View\" ");

					writer.Write(sb.ToString());
				}
			}
		}



		#endregion
	}
}
