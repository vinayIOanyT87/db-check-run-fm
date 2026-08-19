// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMColor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMColor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMControls
{
	using System;
	using System.Configuration;
	using System.Drawing;

	/// <summary>
	/// Class contains popular color values used by FuelsManager.
	/// </summary>
	public class FMColor
	{
		#region Constants and Fields

		/// <summary>
		/// Gets the standard alternate row gray color used in grids.
		/// </summary>
		public static readonly Color AlternateRowGray = Color.FromArgb(220, 220, 220);

		/// <summary>
		/// Gets the standard dark blue color.
		/// </summary>
		public static readonly Color DarkBlue = Color.FromArgb(Convert.ToInt32(ConfigurationManager.AppSettings["ColorDarkBlue"]));

		/// <summary>
		/// The standard dark gray color object.
		/// </summary>
		public static readonly Color DarkGray = Color.FromArgb(90, 90, 90);

		/// <summary>
		/// The standard dark red color object.
		/// </summary>
		public static readonly Color DarkRed = Color.FromArgb(190, 0, 0);

		/// <summary>
		/// The standard gainsboro color object.
		/// </summary>
		public static readonly Color Gainsboro = Color.FromArgb(220, 220, 220);

		/// <summary>
		/// The standard header blue color object.
		/// </summary>
		public static readonly Color HeaderBlue = Color.FromArgb( Convert.ToInt32( ConfigurationManager.AppSettings["ColorHeaderBlue"]) );

		/// <summary>
		/// The standard row gray color object.
		/// </summary>
		public static readonly Color RowGray = Color.FromArgb(238, 238, 238);

		/// <summary>
		/// The standard selected row color object.
		/// </summary>
		public static readonly Color SelectedRowColor = Color.FromArgb(0, 138, 140);

		/// <summary>
		/// The standard sub total row color object.
		/// </summary>
		public static readonly Color SubTotalRowColor = Color.FromArgb(220, 220, 200);

		/// <summary>
		/// The standard total row color object.
		/// </summary>
		public static readonly Color TotalRowColor = Color.FromArgb(200, 200, 200);

		/// <summary>
		/// Not too dark red.
		/// </summary>
		public static Color RedBackground = Color.FromArgb(255, 150, 150);

		/// <summary>
		/// Not too dark green.
		/// </summary>
		public static Color GreenBackground = Color.Chartreuse;
		#endregion
	}
}