// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplateLabelClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TemplateLabelClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMControls
{
	using System.Web.UI;
	using System.Web.UI.WebControls;

	/// <summary>
	/// Template label class
	/// </summary>
	public class TemplateLabelClass : ITemplate
	{
		#region Constants and Fields

		/// <summary>
		/// ID of the template label
		/// </summary>
		private readonly string id;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="TemplateLabelClass"/> class.
		/// </summary>
		/// <param name="id">The ID.</param>
		public TemplateLabelClass(string id)
		{
			this.id = id;
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// When implemented by a class, defines the <see cref="T:System.Web.UI.Control"/> object that child controls and templates belong to. These child controls are in turn defined within an inline template.
		/// </summary>
		/// <param name="container">The <see cref="T:System.Web.UI.Control"/> object to contain the instances of controls from the inline template.</param>
		public void InstantiateIn(Control container)
		{
			var label = new Label();
			label.ID = this.id;
			container.Controls.Add(label);
		}

		#endregion
	}
}