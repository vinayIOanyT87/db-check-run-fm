// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AboutBox1.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The about box for the application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DispatchPrototype
{
	using System;
	using System.IO;
	using System.Reflection;
	using System.Windows.Forms;

	/// <summary>
	/// The about box for the application.
	/// </summary>
	public partial class AboutBox1 : Form
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="AboutBox1"/> class.
		/// </summary>
		public AboutBox1()
		{
			this.InitializeComponent();
			this.labelProductName.Text = this.AssemblyProduct;
			this.labelVersion.Text = string.Format("Version {0}", this.AssemblyVersion);
			this.labelCopyright.Text = this.AssemblyCopyright;
			this.labelCompanyName.Text = this.AssemblyCompany;
			this.textBoxDescription.Text = this.AssemblyDescription;
		}

		#endregion Constructors and Destructors

		#region Public Properties

		/// <summary>
		/// Gets the assembly company.
		/// </summary>
		/// <value>
		/// The assembly company.
		/// </value>
		private string AssemblyCompany
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
				if (attributes.Length == 0)
				{
					return string.Empty;
				}

				return ((AssemblyCompanyAttribute)attributes[0]).Company;
			}
		}

		/// <summary>
		/// Gets the assembly copyright.
		/// </summary>
		/// <value>
		/// The assembly copyright.
		/// </value>
		private string AssemblyCopyright
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
				if (attributes.Length == 0)
				{
					return string.Empty;
				}

				return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
			}
		}

		/// <summary>
		/// Gets the assembly description.
		/// </summary>
		/// <value>
		/// The assembly description.
		/// </value>
		private string AssemblyDescription
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly()
				                              .GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
				if (attributes.Length == 0)
				{
					return string.Empty;
				}

				return ((AssemblyDescriptionAttribute)attributes[0]).Description;
			}
		}

		/// <summary>
		/// Gets the assembly product.
		/// </summary>
		/// <value>
		/// The assembly product.
		/// </value>
		private string AssemblyProduct
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
				if (attributes.Length == 0)
				{
					return string.Empty;
				}

				return ((AssemblyProductAttribute)attributes[0]).Product;
			}
		}

		/// <summary>
		/// Gets the assembly title.
		/// </summary>
		/// <value>
		/// The assembly title.
		/// </value>
		private string AssemblyTitle
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
				if (attributes.Length > 0)
				{
					var titleAttribute = (AssemblyTitleAttribute)attributes[0];
					if (titleAttribute.Title != string.Empty)
					{
						return titleAttribute.Title;
					}
				}

				return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
			}
		}

		/// <summary>
		/// Gets the assembly version.
		/// </summary>
		/// <value>
		/// The assembly version.
		/// </value>
		private string AssemblyVersion
		{
			get
			{
				return Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
		}

		#endregion

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Form.Load" /> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );
			this.Text = string.Format( "About {0}", this.AssemblyTitle );
		}
	}
}