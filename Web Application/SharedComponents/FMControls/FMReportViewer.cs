// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMReportViewer.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMReportViewer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMControls
{
	using System.Web.UI;

	using Microsoft.Reporting.WebForms;

	/// <summary>
	/// FuelsManager report viewer wrapper class
	/// </summary>
	public class FMReportViewer : ReportViewer
	{
		#region Delegates

		/// <summary>
		/// Delegate for report rendering events.
		/// </summary>
		public delegate void ReportRenderingHandler();

		#endregion

		#region Public Events

		/// <summary>
		/// Report rendering events
		/// </summary>
		public event ReportRenderingHandler ReportRender;

		#endregion

		#region Methods

		/// <summary>
		/// Override method for rendering the control.
		/// </summary>
		/// <param name="writer">
		/// The writer.
		/// </param>
		protected override void Render(HtmlTextWriter writer)
		{
			base.Render(writer);

			if (this.ReportRender != null)
			{
				this.ReportRender.Invoke();
			}
		}

		#endregion
	}
}