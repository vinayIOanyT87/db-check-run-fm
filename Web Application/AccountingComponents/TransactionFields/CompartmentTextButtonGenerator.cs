/// <summary>
/// File name:	CompartmentTextButtonGenerator.cs
/// Purpose:	The purpose of this abstract class is to generate equipment text field button combination control.
///				It inherits from Field Generator.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	W. Gray
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	--------------------------------------------
///		10-15-2008      V. Thompson             Commented out the place where the textbox's readonly property was set
/// </summary>

namespace TransactionFields
{
	using System.Web.UI;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using System;
	using System.Globalization;

	abstract public class CompartmentTextButtonGenerator : FieldGenerator
	{
		#region Public Attributes
		public const short FIELD_LENGTH = 5;
		#endregion

		#region Protected Attributes
		protected string equipmentRole;
		protected bool autoPostBack;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the equipment text box button combo
		/// abstract class.
		/// </summary>
		public CompartmentTextButtonGenerator()
		{
			this.autoPostBack = false;
		}
		#endregion

		#region Abstract Properties
		/// <summary>
		/// This property is an abstract property that forces implementation of
		/// returning the maximum column size.
		/// </summary>
		abstract protected short MaxColumns
		{
			get;
		}
		#endregion


		#region Override methods
		/// <summary>
		/// This method will generate the actual web control. In this case, the
		/// FMCompartmentTextBox control is being generated.
		/// </summary>
		/// <param name="editable"></param>
		public override void Generate(bool editable)
		{
			var updatePanel = new UpdatePanel { UpdateMode = UpdatePanelUpdateMode.Conditional, ID = this.ID + "Panel" };

			var textBoxButtonCombo = new FMControls.FMCompartmentTextBox
			                         {
				                         ID				= this.ID,
				                         MaxLength		= this.MaxColumns,
				                         Columns		= this.MaxColumns,
				                         AutoPostBack	= this.autoPostBack,
				                         Width			= new System.Web.UI.WebControls.Unit(".5 in", CultureInfo.InvariantCulture),
				                         BackColor		= this.VarecBkgrndReadOnlyGray
			                         };

			textBoxButtonCombo.BackColor = this.VarecBkgrndReadOnlyGray;
			textBoxButtonCombo.ToolTip = this.DisplayName;
			updatePanel.ContentTemplateContainer.Controls.Add(textBoxButtonCombo);
			this.cell.Controls.Add(updatePanel);


			object fieldValue = GetDataValue();

			if (fieldValue != null)
			{
				textBoxButtonCombo.Text = fieldValue.ToString();
			}
		}

		/// <summary>
		/// This method is an override method that will return the contents of the FMCompartmentTextBox
		/// control.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public override object GetNewValue(System.Web.UI.WebControls.WebControl control)
		{
			var updatePanel = control.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var textBoxButtonCombo = updatePanel.ContentTemplateContainer.Controls[0] as FMControls.FMCompartmentTextBox;

				if (textBoxButtonCombo != null && 
				    ((textBoxButtonCombo.TextMode == System.Web.UI.WebControls.TextBoxMode.MultiLine) && 
				     (textBoxButtonCombo.Text.Length > this.MaxColumns)))
				{
					string message = this.GetLabel(control) + " length must be " + this.MaxColumns + " or less.";
					this.RenderErrorMessage(message);
					throw new RetrieveException(message);
				}

				if (textBoxButtonCombo != null)
				{
					return textBoxButtonCombo.Text;
				}
			}

			return string.Empty;
		}
		#endregion

		#region Protected Methods
		/// <summary>
		/// This method will return a equipment Guid that matches the equipment ID.  It will
		/// return guid.empty if there are no matches.
		/// </summary>
		/// <param name="compartmentID"></param>
		/// <returns></returns>
		protected Guid GetCompartmentIdentityGuid(string compartmentID)
		{
			Guid compartmentGuid = Guid.Empty;

			// Find the equipment guid that matches the Compartment ID.
			if (!string.IsNullOrEmpty(compartmentID))
			{
				Guid equipmentGuid = FMChannelHelper.MakeCall<IEquipments, Guid>(
																	 x =>
																	 x.GetIdentityGuid(this.transContext.security, compartmentID)
																);

				EquipmentClass equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
																	 x =>
																	 x.Get(this.transContext.security, equipmentGuid)
																);
				if (equipment != null)
				{
					compartmentGuid = equipment.IdentityGuid;
				}
			}

			return compartmentGuid;
		}

		#endregion
	}
}
