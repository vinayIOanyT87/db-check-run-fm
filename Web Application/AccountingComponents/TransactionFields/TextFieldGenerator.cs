/// <summary>
/// File name:	TextFieldGenerator.cs
/// Purpose:	The purpose of class is to generate all text fields. This class is an
///				abstract class that all text fields will inherit.
/// Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
/// Author(s):	Thomas Beckum
/// Version:	1.0.0  Current version
///	
/// Modification History:
/// Date:			By:						Reason:
/// ----------		--------------------	--------------------------------------------
/// 2006-11-21		Richard Panachida		Corrected the problem with the same client control ID
///												for the user data controls (CSI 3757).		
/// 2007-09-05    A.Sang					Correct the FieldID search value for UserData fields. CSI 5148
/// 2009-03-16    Richard Panachida    Defect 1841. Making user data fields required.
/// 2010-03-20		W.Gray					Revised to call cell.Controls.Clear to facilitate calling Generate
/// </summary>
namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.Exceptions;

	/// <summary>
	/// Summary description for TextFieldGenerator.
	/// </summary>
	abstract public class TextFieldGenerator : FieldGenerator
	{

		abstract protected short MaxColumns
		{
			get;
		}

		protected void Add508ComplianceAttributes(TextBox ctl)
		{
			System.Web.UI.Control c = ctl.Parent;

			if (c == null)
				return;


			while (c != null)
			{
				string str = c.ID;
				//Go up the chain of controls until finding control with ID containing "FieldValue".
				if (!string.IsNullOrEmpty(str) && str.Contains("FieldValue"))
				{
					//Look for control representing the label for this control. It should have an ID that contains FieldLabel. 
					string label = str.Replace("FieldValue", "FieldLabel");
					c = c.Parent.FindControl(label);
					if (c != null)
					{
						if (c.Controls.Count > 0)
						{
							c = c.Controls[0];
						}
						else
						{
							if (c is TableCell)
							{
								TableCell tc = c as TableCell;

								tc.Attributes.Add("role", "presentation");

								string txt = tc.Text;
								if (txt.Substring(0, 3) == "<a ")
								{
									int p0 = txt.IndexOf(" id=\"", System.StringComparison.OrdinalIgnoreCase);
									if (p0 > 1)
									{
										int p1 = txt.IndexOf("\"", p0 + 6, System.StringComparison.OrdinalIgnoreCase);
										if (p1 > p0 + 6)
										{
											string id = txt.Substring(p0 + 5, p1 - p0 - 5);
											ctl.Attributes.Add("aria-labelledby", id);

										}
									}
									p0 = txt.IndexOf(">", System.StringComparison.OrdinalIgnoreCase);
									if (p0 > 1)
									{
										int p1 = txt.IndexOf("</", p0 + 2, System.StringComparison.OrdinalIgnoreCase);
										if (p1 > p0 + 2)
										{
											string id = txt.Substring(p0 + 1, p1 - p0 - 1);
											ctl.Attributes.Add("alt", id);
											tc.Attributes.Add("aria-label", id);
											//tc.Attributes.Add("summary", id);

										}
									}
									break;
								}
								else
								{

								}
							}

						}
					}
					break;
				}
				c = c.Parent;
			}

		}
		public override void Generate(bool editable)
		{
			var updatePanel = new UpdatePanel { UpdateMode = UpdatePanelUpdateMode.Conditional };

			this.cell.Controls.Clear();
            var textBox = new TextField();

			textBox.ToolTip = this.DisplayName;

			// Since there can be many user data fields (up to 24) ensure that
			// the client HTML ID is unique.	
			string typeString = ID;

			if (typeString.ToUpper().EndsWith("USERDATATEXTFG"))
			{
				textBox.ID = typeString + FieldID;
				updatePanel.ID = typeString + FieldID + "Panel";
			}
			else
			{
				textBox.ID = typeString;
				updatePanel.ID = typeString + "Panel";
			}

			updatePanel.ContentTemplateContainer.Controls.Add(textBox);
			this.cell.Controls.Add(updatePanel);
			Add508ComplianceAttributes(textBox);

			textBox.ReadOnly = !(this.Editable && editable);

			if (textBox.ReadOnly)
			{
				textBox.BackColor = this.VarecBkgrndReadOnlyGray;
			}

			textBox.MaxLength = MaxColumns;
			textBox.Columns = MaxColumns + 4;

			object fieldValue = GetDataValue();

			if (fieldValue != null)
			{
				textBox.Text = fieldValue.ToString();
			}
		}

		/// <summary>
		/// This method will overwrite the base class method implementing a specific method of 
		/// retrieving the new value from the control.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public override object GetNewValue(WebControl control)
		{
			if (control.Controls.Count == 0)
			{
				return null;
         }

         var updatePanel = control.Controls[0] as UpdatePanel;

			if (updatePanel == null)
			{
				return null;
			}

			var textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;

			if (textBox == null)
			{
				return null;
			}

			if (textBox.TextMode == TextBoxMode.MultiLine && textBox.Text.Length > this.MaxColumns)
			{
				string message = this.GetLabel(control) + " length must be " + this.MaxColumns + " or less.";
				throw new RetrieveException(message);
			}

			string stringValue = textBox.Text;

			if (this.Required)
			{
				this.cell.BackColor = System.Drawing.Color.Red;

				if (string.IsNullOrEmpty(stringValue)) 
				{
					throw new FMFieldRequiredException();
				}

				this.cell.BackColor = System.Drawing.Color.Transparent;
			}

			return stringValue;
		}

		/// <summary>
		/// Update the TextBox control that is held in the TableCell control of this FieldGenerator.
		/// </summary>
		/// <param name="value"></param>
		public void SetDisplayValue(string value)
		{
			if (this.cell == null)
			{
				return;
			}

            var updatePanel = this.cell.Controls[0] as UpdatePanel;
            if (updatePanel == null)
            {
                return;
            }

            var textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;

            if (textBox != null)
            {
                textBox.Text = value;
            }

		}
	}
}
