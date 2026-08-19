 #pragma warning disable 1587
/// <summary>
///	FILE NAME:		LineItemEngUnitFG.cs
///	PURPOSE:		
///
///	COMMENTS:
///		Copyright (C) Varec, Inc. Norcross, GA, USA, 2007
///		This file shall not be copied or reproduced in any form without
///		the express written consent of Varec.
///
///	AUTHOR(S):	
///	VERSION:	1.0.0  Current version
///
///	MODIFICATION HISTORY:
///	Date:			By:						Reason:
///	----------	-----------------		----------------------------------------------
/// </summary>
#pragma warning restore 1587
namespace TransactionFields
{
	using System.Security;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using FMBusinessObjects.DataObjects;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	abstract public class LineItemEngUnitFG : TextFieldGenerator, ILineItemField, ISublineItemField
	{
		public LineItemEngUnitFG()
		{
			this.virtualField = true;
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 5.
		/// </summary>
		protected override short MaxColumns => 20;

		 /// <summary>
		/// Format the control as read-only without disabling the control
		/// </summary>
		/// <param name="control">The control to format</param>
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

		abstract public object GetDataValue(LineItemDO lineItem);
		abstract public object GetDataValue(SubLineItemDO subLineItem);

		[SecurityCritical]
		protected object GetUnitAsAbbrevString(EngineeringUnit unit)
		{
			try
			{

				return EngineeringUnits.GetUnitAbbreviation(unit);
			}
			catch
			{
				return string.Empty;
			}
		}

		#region lineItemField Members
		public string GetDataText(LineItemDO inLineItem)
		{
			if (this.GetDataValue(inLineItem) != null)
			{
				return this.GetDataValue(inLineItem).ToString();
			}

			return null;
		}

		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
		}
		#endregion

		#region ISublineItemField Members
		public string GetDataText(SubLineItemDO inSublineItem)
		{
			if (this.GetDataValue(inSublineItem) != null)
			{
				return this.GetDataValue(inSublineItem).ToString();
			}

			return null;
		}

		public void SetDataValue(SubLineItemDO inSublineItem, object newValue)
		{
		}
		#endregion
	}
}
