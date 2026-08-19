// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EngUnitFG.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the EngUnitFG type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TransactionFields
{
	using System.Security;
	using System.Web.UI;
	using System.Web.UI.WebControls;

    using FMBusinessObjects.DataObjects;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    /// <summary>
	/// Base class for field generator for engineering units fields.
	/// </summary>
	public abstract class EngUnitFG : TextFieldGenerator, IHeaderField
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="EngUnitFG"/> class.
		/// </summary>
		public EngUnitFG()
		{
			this.virtualField = true;
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will return either a figured data length or the default length of 5.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return 20;
			}
		}
		#endregion 

		#region Public Methods and Operators
		/// <summary>
		/// Gets the data text.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <returns>The text data of the field represented by this field generator.</returns>
		public string GetDataText(TransactionDO transaction)
		{
			if (this.GetDataValue(transaction) != null)
			{
				return this.GetDataValue(transaction).ToString();
			}

			return null;
		}

		/// <summary>
		/// Gets the data value.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <returns>The data object represented by this field generator.</returns>
		public abstract object GetDataValue(TransactionDO transaction);

		/// <summary>
		/// Gets the unit as abbrev string.
		/// </summary>
		/// <param name="volumeUnit">The volume unit.</param>
		/// <returns>Abbreviated units name.</returns>
		[SecurityCritical]
		public string GetUnitAsAbbrevString(EngineeringUnit volumeUnit)
		{
			try
			{
				return EngineeringUnits.GetUnitAbbreviation(volumeUnit);
			}
			catch
			{
				return string.Empty;
			}
		}

		/// <summary>
		/// Sets the data value.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="newValue">The new value.</param>
		public void SetDataValue(TransactionDO transaction, object newValue)
		{
		}
		#endregion

		#region Methods

		/// <summary>
		/// Format the control as read-only without disabling the control
		/// </summary>
		/// <param name="control">
		/// The control to format 
		/// </param>
		protected override void SpecializeControl(WebControl control)
		{
			var updatePanel = control.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;

				if (textBox != null)
				{
					textBox.ReadOnly = true;
					textBox.Enabled = false;
					textBox.BackColor = this.VarecBkgrndReadOnlyGray;
				}
			}
		}
		#endregion
	}
}