// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMDataGridFixedBase.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMDataGridFixedBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMControls
{
	using System.Drawing;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;

	/// <summary>
	/// Base class for a fixed data grid.
	/// </summary>
	public class FMDataGridFixedBase : FMDataGrid
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FMDataGridFixedBase"/> class.
		/// </summary>
		public FMDataGridFixedBase()
		{
			this.FMDataGridFixedBaseInit();
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets a value indicating whether [fixed headers].
		/// </summary>
		/// <value>
		///   <c>true</c> if [fixed headers]; otherwise, <c>false</c>.
		/// </value>
		public bool FixedHeaders { get; set; }

		/// <summary>
		/// Gets or sets the height of the fixed.
		/// </summary>
		/// <value>
		/// The height of the fixed.
		/// </value>
		public Unit FixedHeight { get; set; }

		/// <summary>
		/// Gets or sets the height of the Web server control.
		/// </summary>
		/// <returns>A <see cref="T:System.Web.UI.WebControls.Unit"/> that represents the height of the control. The default is <see cref="F:System.Web.UI.WebControls.Unit.Empty"/>.</returns>
		/// <exception cref="T:System.ArgumentException">The height was set to a negative value.</exception>
		public override Unit Height
		{
			get
			{
				return base.Height;
			}

			set
			{
				base.Height = value;
				this.FixedHeight = value;
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Renders the panel begin.
		/// </summary>
		/// <param name="writer">The writer to use for rendering.</param>
		public void RenderPanelBegin(HtmlTextWriter writer)
		{
			if (this.FixedHeaders)
			{
				if (this.Items.Count <= 10)
				{
					this.Height = new Unit(0, UnitType.Pixel);
				}

				// Add 20 pixels to allow for the vertical scroll bar of the <div>
				var newWidth = new Unit(this.Width.Value, this.Width.Type);
				if (newWidth.Type == UnitType.Pixel)
				{
					// invoicing pages in the core product are prone to cropping, increase its size
					if (this.DesignMode == false)
					{
						var isAdfKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(
																	 x =>
																	 x.IsADFKey()
																);

						if (isAdfKey && this.Page.ToString().ToUpper().Contains("INVOICEWEBAPP"))
						{
							newWidth = new Unit(newWidth.Value + 320);
						}
						else
						{
							newWidth = new Unit(this.Width.Value + 20);
						}
					}
					else
					{
						newWidth = new Unit(this.Width.Value + 20);
					}
				}

				string renderValue;
				if (this.Items.Count > 10)
				{
					renderValue = string.Format(
						"<div id=\"pnlContainer\" style=\"height:{0};width:{1};overflow-x:auto;overflow-y:auto\">", this.FixedHeight, newWidth);
				}
				else
				{
					renderValue = string.Format("<div id=\"pnlContainer\" style=\"width:{0};overflow-x:auto;\">", newWidth);
				}

				writer.Write(renderValue);
			}
		}

		/// <summary>
		/// Renders the panel end.
		/// </summary>
		/// <param name="writer">The writer to use for rendering.</param>
		public void RenderPanelEnd(HtmlTextWriter writer)
		{
			if (this.FixedHeaders)
			{
				writer.Write("</div>");
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Handles the Init event of the FMDataGridFixedBase control.
		/// </summary>
		private void FMDataGridFixedBaseInit()
		{
			this.FixedHeaders = true;
			this.ShowFooter = true;
			this.ShowHeader = true;
			//this.Height = new Unit(550, UnitType.Pixel);
			//this.FixedHeight = new Unit(550, UnitType.Pixel);

			this.HeaderStyle.Font.Bold = true;
			this.HeaderStyle.ForeColor = Color.White;
			this.HeaderStyle.BackColor = FMColor.HeaderBlue;
			this.HeaderStyle.CssClass = "GVFixedHeader";

			this.FooterStyle.ForeColor = Color.Black;
			this.FooterStyle.BackColor = FMColor.HeaderBlue;
			this.FooterStyle.CssClass = "GVFixedFooter";

			if (this.DesignMode == false)
			{
				if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey()))
				{
					this.ShowFooter = false;
				}
			}
		}

		/// <summary>
		/// Renders the control to the specified HTML writer.
		/// </summary>
		/// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that contains the output stream to render on the client.</param>
		protected override void Render(HtmlTextWriter writer)
		{
			this.RenderPanelBegin(writer);
			base.Render(writer);
			this.RenderPanelEnd(writer);
		}

		#endregion
	}
}