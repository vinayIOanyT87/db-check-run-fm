namespace TransactionFields
{
	using System.Web.UI;

	using FMControls;

	abstract public class CustomCheckBoxGenerator : FieldGenerator
	{
		#region Attributes
		private Pair[] attributes;
		private bool autoPostBack;
		#endregion // Attributes

		#region Constructors
		public CustomCheckBoxGenerator(Pair inAttribute)
		{
			this.attributes = new Pair[1];

			this.attributes[0] = inAttribute;
			this.autoPostBack = false;
		}

		public CustomCheckBoxGenerator(Pair[] inAttribute)
		{
			this.attributes = inAttribute;
			this.autoPostBack = false;
		}
		#endregion // Constructors

		#region Override methods
		/// <summary>
		/// This method will generate the actual web control. In this case, the
		/// FMCompanyTextBox control is being generated.
		/// </summary>
		/// <param name="editable"></param>
		public override void Generate(bool editable)
		{
			var updatePanel = new UpdatePanel { UpdateMode = UpdatePanelUpdateMode.Conditional, ID = this.ID + "Panel" };

			var checkBox = new FMCheckBox
			               {
				               ID = this.ID,
				               AutoPostBack = this.autoPostBack,
				               Enabled = editable
			               };

			checkBox.ToolTip = this.DisplayName;
			updatePanel.ContentTemplateContainer.Controls.Add(checkBox);
			this.cell.Controls.Add(updatePanel);

			foreach (Pair attributePair in this.attributes)
			{
				if (string.IsNullOrEmpty(attributePair.First as string) == false)
				{
					checkBox.Attributes[attributePair.First as string] = attributePair.Second as string;
				}
			}

			object fieldValue = GetDataValue();

			if (fieldValue != null)
			{
				checkBox.Checked = (bool)fieldValue;
			}
		}

		/// <summary>
		/// This method is an override method that will return the contents of the FMCompanyTextBox
		/// control.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public override object GetNewValue(System.Web.UI.WebControls.WebControl control)
		{
			var updatePanel = control.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var checkBox = updatePanel.ContentTemplateContainer.Controls[0] as FMCheckBox;

				return checkBox != null && checkBox.Checked;
			}

			return false;
		}
		#endregion
	}
}
