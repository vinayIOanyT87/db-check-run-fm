// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMToolbar.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   FuelsManager toolbar control class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMControls
{
	using System.Web.UI;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	/// <summary>
	/// FuelsManager toolbar control class
	/// </summary>
	public class FMToolbar : WebControl
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FMToolbar"/> class.
		/// </summary>
		public FMToolbar ()
		{
			this.ListControl = new HtmlGenericControl("ul");
			this.ListControl.Attributes.Add("runat", "server");
			this.ListControl.Attributes.Add("ID", string.Empty);
			this.ListControl.Attributes.Add("class", "listToolBar");
			this.ListControl.Attributes.Add("LastButtonId", string.Empty);
			this.ListControl.Attributes.Add("FirstButtonTabIndex", string.Empty);
			this.ListControl.Attributes.Add("LastButtonTabIndex", string.Empty);

			this.Controls = new ControlCollection(this);
		}

		/// <summary>
		/// Gets or sets the last toolbar button id
		/// </summary>
		public string LastButtonId
		{
			get
			{
				return this.ListControl.Attributes["LastButtonId"];
			}
			set
			{
				this.ListControl.Attributes["LastButtonId"] = value;
			}
		}

		/// <summary>
		/// Gets or sets the first toolbar button tab index
		/// </summary>
		public string FirstButtonTabIndex
		{
			get
			{
				return this.ListControl.Attributes["FirstButtonTabIndex"];
			}
			set
			{
				this.ListControl.Attributes["FirstButtonTabIndex"] = value;
			}
		}

		/// <summary>
		/// Gets or sets the last toolbar button tab index
		/// </summary>
		public string LastButtonTabIndex
		{
			get
			{
				return this.ListControl.Attributes["LastButtonTabIndex"];
			}
			set
			{
				this.ListControl.Attributes["LastButtonTabIndex"] = value;
			}
		}

		/// <summary>
		/// Gets a <see cref="T:System.Web.UI.ControlCollection"/> object that represents the child controls for a specified server control in the UI hierarchy.
		/// </summary>
		/// <returns>The collection of child controls for the specified server control.</returns>
		public new ControlCollection Controls { get; private set; }

		/// <summary>
		/// Gets the list control.
		/// </summary>
		protected HtmlGenericControl ListControl { get; private set; }

		/// <summary>
		/// Renders the control to the specified HTML writer.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the control content.</param>
		protected override void Render( HtmlTextWriter writer )
		{
			if (string.IsNullOrEmpty( this.CssClass ) == false)
			{
				this.ListControl.Attributes["class"] = this.CssClass;
			}

			if (string.IsNullOrEmpty( this.ID ) == false)
			{
				this.ListControl.Attributes["ID"] = this.ID;
			}

			this.ListControl.Controls.Clear();

			foreach ( FMToolbarButton control in this.Controls )
			{
				this.ListControl.Controls.Add(control);
			}

			this.ListControl.RenderControl(writer);
		}
	}
}