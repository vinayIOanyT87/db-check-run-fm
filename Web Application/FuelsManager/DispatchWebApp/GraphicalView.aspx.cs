// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphicalView.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the GraphicalView type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.DispatchWebApp
{
	using System;
	using System.Collections.Generic;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FMControls;

	using FuelsManager.FMWebApp;

	using global::FMWebApp;

	/// <summary>
	///    Code behind for GraphicalView page.
	/// </summary>
	public partial class GraphicalView : FMFormBase
	{
		#region Public Methods and Operators

		/// <summary>
		///    Identifies the data dictionary keys needed for this page.
		/// </summary>
		/// <param name="security">
		///    The current security object.
		/// </param>
		/// <returns>
		///    An array of data dictionary keys.
		/// </returns>
		public string[] Keys(SecurityClass security)
		{
			string[] keys =
				{
					"++Scale", "+1 Hr", "+3 Hrs", "+6 Hrs", "+Locations", "--Scale", "-1 Hr", "-3 Hrs", "-6 Hrs",
					"-Locations"
				};

			return keys;
		}

		#endregion

		#region Methods

		/// <summary>
		///    Page_Load event handler for page.  Creates the custom toolbar command buttons
		///    associated with the graphical view toolbar and adds them to that toolbar.
		/// </summary>
		/// <param name="sender">The sender parameter</param>
		/// <param name="e">The event args parameter</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				// A postback always clears the toolbar controls so create toolbar each time the page is loaded
				this.CreateToolbar();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Creates the graphical view custom toolbar.
		/// </summary>
		private void CreateToolbar()
		{
			var toolbarInfoMap = new Dictionary<string, ToolbarInfo>
				{
					// Add the default toolbar command buttons in the desired display order
					{ "Tabular View", new ToolbarInfo(true, "toolStripDefaultButton.Image.png", null) },
					{ "Subtract Six Hours", new ToolbarInfo(true, "toolStripClockButton.Image.png", "-6 Hrs") },
					{ "Add Six Hours", new ToolbarInfo(true, "toolStripClockButton.Image.png", "+6 Hrs") },
					{ "Subtract Three Hours", new ToolbarInfo(true, "toolStripClockButton.Image.png", "-3 Hrs") },
					{ "Add Three Hours", new ToolbarInfo(true, "toolStripClockButton.Image.png", "+3 Hrs") },
					{ "Subtract One Hour", new ToolbarInfo(true, "toolStripClockButton.Image.png", "-1 Hr") },
					{ "Add One Hour", new ToolbarInfo(true, "toolStripClockButton.Image.png", "+1 Hr") },
					{ "Increase Scale", new ToolbarInfo(true, "toolStripPlusButton.Image.png", "++Scale") },
					{ "Decrease Scale", new ToolbarInfo(true, "toolStripMinusButton.Image.png", "--Scale") },
					{ "Add Locations", new ToolbarInfo(true, "toolStripPlusButton.Image.png", "+Locations") },
					{ "Remove Locations", new ToolbarInfo(true, "toolStripMinusButton.Image.png", "-Locations") },
					{ "Refresh", new ToolbarInfo(true, null, null) },
					{ "Operator Log", new ToolbarInfo(true, null, null) },
					{ "Flight Changes", new ToolbarInfo(true, null, null) },
					{ "Dispatchers List", new ToolbarInfo(true, null, null) }
				};

			// A toolbar command button is either a standard command defined in the toolbar info map
			// or a transaction alias command contained in the custom toolbar command list.
			var buttons = new List<ButtonInfo>();

			bool entityAssigned;
			Guid dispatchConfigGuid = 
				FMChannelHelper.MakeCall<IDispatchConfigurations, Guid>(
					x =>
					x.GetIdentityGuidBySiteIdAndAssigned(this.Security, this.Security.SiteGuid,
									DispatchConfigurationClass.DefaultId, true, out entityAssigned)
				);

			// Get the Graphical View Custom Toolbar
			var customToolbar = new CustomToolbarClass { ID = "Dispatch Graphical View" };

			FMChannelHelper.MakeCall<ICustomToolbars>(
				customToolbars =>
				{
					Guid customToolbarGuid = customToolbars.GetIdentityGuidById(
						this.Security, customToolbar.ID, dispatchConfigGuid);
					if (customToolbarGuid != Guid.Empty)
					{
						customToolbar = customToolbars.Get(this.Security, customToolbarGuid);
					}
				});

			if (customToolbar.IdentityGuid != Guid.Empty)
			{
				// Populate the command button list with the custom set of toolbar commands
				foreach (CustomToolbarCommandClass toolbarCommand in customToolbar.ToolbarCommandList)
				{
					bool isTransactionAlias = toolbarCommand.TransactionAliasGuid != Guid.Empty;
					buttons.Add(new ButtonInfo(toolbarCommand.ID, isTransactionAlias));
				}
			}
			else
			{
				// Populate the command button list with the default set of toolbar commands
				foreach (KeyValuePair<string, ToolbarInfo> toolbarItem in toolbarInfoMap)
				{
					if (toolbarItem.Value.DefaultCommand)
					{
						buttons.Add(new ButtonInfo(toolbarItem.Key, false));
					}
				}
			}

			// Create the toolbar command buttons
			foreach (ButtonInfo button in buttons)
			{
				if (button.IsTransactionAlias)
				{
					string aliasName = button.CommandName;
					string text = this.GetTranslatedText(aliasName);
					string aliasNameNoSpaces = aliasName.Replace(" ", string.Empty);

					string id = aliasNameNoSpaces + "TransactionAliasButton";
					const string CssClass = "buttonStyle";
					string onClick = "GraphicalViewLib.TransactionAliasButtonOnClick('" + aliasName + "')";
					this.toolBarGraphical.Controls.Add(new FMToolbarButton(null, text, id, CssClass, onClick, 0));
				}
				else
				{
					string commandName = button.CommandName;
					string commandNameNoSpaces = commandName.Replace(" ", string.Empty);
					string img = toolbarInfoMap[commandName].SourceImage;
					if (img != null)
					{
						img = "images/" + img;
					}

					string text = toolbarInfoMap[commandName].CustomText;
					if (string.IsNullOrEmpty(text))
					{
						text = this.GetTranslatedText(commandName);
					}
					else
					{
						text = this.GetTranslatedText(text);
					}

					string id = commandNameNoSpaces + "Button";
					const string CssClass = "buttonStyle";
					string onClick = "GraphicalViewLib." + id + "OnClick()";
					this.toolBarGraphical.Controls.Add(new FMToolbarButton(img, text, id, CssClass, onClick, 0));
				}
			}
		}

		#endregion

		/// <summary>
		///    Structure containing the toolbar button command name and a flag indicating
		///    whether or not the toolbar button command is a transaction alias command
		/// </summary>
		public struct ButtonInfo
		{
			/// <summary>
			///    The toolbar button command name
			/// </summary>
			public string CommandName;

			/// <summary>
			///    True if command is a transaction alias command
			/// </summary>
			public bool IsTransactionAlias;

			/// <summary>
			///    Initializes a new instance of the ButtonInfo struct.
			/// </summary>
			/// <param name="name">The toolbar button command name</param>
			/// <param name="isAlias">True if command is a transaction alias command</param>
			public ButtonInfo(string name, bool isAlias)
			{
				this.CommandName = name;
				this.IsTransactionAlias = isAlias;
			}
		}

		/// <summary>
		///    Dictionary value item used in the generation of the custom toolbar
		/// </summary>
		public struct ToolbarInfo
		{
			#region Constants and Fields

			/// <summary>
			///    The custom text to display for the toolbar command name
			/// </summary>
			public string CustomText;

			/// <summary>
			///    True if toolbar command is a default command
			/// </summary>
			public bool DefaultCommand;

			/// <summary>
			///    The filename of the toolbar buttom image
			/// </summary>
			public string SourceImage;

			#endregion

			#region Constructors and Destructors

			/// <summary>
			///    Initializes a new instance of the <see cref="ToolbarInfo" /> struct.
			/// </summary>
			/// <param name="defaultCommand">True indicates the toolbar command is a default command</param>
			/// <param name="sourceImage">The filename of the toolbar buttom image</param>
			/// <param name="customText">The custom text to display for the toolbar command name</param>
			public ToolbarInfo(bool defaultCommand, string sourceImage, string customText)
			{
				this.DefaultCommand = defaultCommand;
				this.SourceImage = sourceImage;
				this.CustomText = customText;
			}

			#endregion
		}
	}
}