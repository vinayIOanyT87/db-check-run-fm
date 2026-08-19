namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;

	/// <summary>
	/// Summary description for CheckBoxGenerator.
	/// </summary>
	abstract public class CheckBoxGenerator : FieldGenerator
	{
		public CheckBoxGenerator()
		{

		}

		protected void Add508ComplianceAttributes(CheckBox ctl)
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
											ctl.InputAttributes.Add("aria-labelledby", id);


										}
									}
									p0 = txt.IndexOf(">", System.StringComparison.OrdinalIgnoreCase);
									if (p0 > 1)
									{
										int p1 = txt.IndexOf("</", p0 + 2, System.StringComparison.OrdinalIgnoreCase);
										if (p1 > p0 + 2)
										{
											string id = txt.Substring(p0 + 1, p1 - p0 - 1);
											ctl.InputAttributes.Add("alt", id);
											tc.Attributes.Add("aria-label", id);
											//tc.Attributes.Add("summary", id);

										}
									}
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
		public override void Generate ( bool editable )
		{
			var updatePanel = new UpdatePanel
			                  {
				                  UpdateMode = UpdatePanelUpdateMode.Conditional,
				                  ID = this.ID + "Panel"
			                  };

			var checkBox = new CheckBox { ID = this.ID };

			checkBox.ToolTip = this.DisplayName;

			updatePanel.ContentTemplateContainer.Controls.Add(checkBox);
			cell.Controls.Add(updatePanel);
			Add508ComplianceAttributes(checkBox);

			object dataValue = GetDataValue();
			if (dataValue == null)
			{
				checkBox.Checked = false;
			}
			else
			{
				checkBox.Checked = (bool)dataValue;
			}

			checkBox.Enabled = editable;

			SpecializeControl(cell);
		}

		public override object GetNewValue(WebControl control)
		{
			bool returnValue = false;
			var updatePanel = control.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var checkBox = updatePanel.ContentTemplateContainer.Controls[0] as CheckBox;
				if(checkBox != null)
				{
					returnValue = checkBox != null && (checkBox.Checked);
				}
			}

			return returnValue;
		}

		public void SetNewValue(bool? newValue)
		{
			if (cell != null)
			{
				var updatePanel = cell.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					updatePanel.Update();
					var checkBox = updatePanel.ContentTemplateContainer.Controls[0] as CheckBox;

					if (checkBox != null)
					{
						if (newValue == null)
						{
							checkBox.Checked = false;
						}
						else
						{
							checkBox.Checked = (bool) newValue;
						}
					}
				}
			}
		}
	}
}
