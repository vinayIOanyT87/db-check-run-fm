namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;

	using FMControls;

	abstract public class EquipmentTypeFG : DropDownGenerator
	{
		#region Public Attributes
		public const short FieldLength = 30;
		#endregion

		#region Protected Attributes
		protected string equipmentRole;
		protected bool destination;
		protected byte equipmentNumber;

        protected override string ID
        {
            get
            {
                return base.ID + equipmentNumber.ToString();
            }
        }
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the equipment type text box button combo
		/// abstract class.
		/// </summary>
		public EquipmentTypeFG(bool destination, byte eqNumber)
		{
			this.destination = destination;
			this.equipmentNumber = eqNumber;
		}
		#endregion

		#region Protected Methods
		/// <summary>
		/// This method sets the equipment type value.
		/// </summary>
		/// <param name="newValue">New equipment type value.</param>
		protected void SetEquipmentType(object newValue)
		{
			if (cell != null)
			{
				HtmlSelect selectList	= null;
				TextBox textBox			= null;
				FMComboBox comboBox		= null;
				var updatePanel			= cell.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					selectList	= updatePanel.ContentTemplateContainer.Controls[0] as HtmlSelect;
					textBox		= updatePanel.ContentTemplateContainer.Controls[0] as TextBox;
					comboBox	= updatePanel.ContentTemplateContainer.Controls[0] as FMComboBox;
					updatePanel.Update();
				}

				var newValueStr = newValue as string;

				if (this.transContext.useDataDictonary)
				{
					newValueStr = GetDataDictionaryValueByKey(transContext.security.SiteGuid, newValueStr);
				}

				if (selectList != null && string.IsNullOrEmpty(newValueStr) == false)
				{
					selectList.SelectedIndex = selectList.Items.IndexOf(selectList.Items.FindByText(newValueStr));
				}
				else if (textBox != null)
				{
					textBox.Text = newValueStr;
				}
				else if (comboBox != null)
				{
					if (string.IsNullOrEmpty(newValueStr) == false && comboBox.Items.FindByText(newValueStr) == null)
					{
						var newItem = new ListItem ( newValue as string, newValue as string );

						foreach (ListItem item in comboBox.Items)
						{
							if (item.Text.CompareTo(newValueStr) > 0)
							{
								comboBox.Items.Insert ( comboBox.Items.IndexOf ( item ) + 1, newItem );
								newItem = null;
								break;
							}
						}

						if (newItem != null)
						{
							comboBox.Items.Add ( newItem );
						}
					}

					comboBox.Text = newValue as string;
				}
			}

			OnFieldChanged ( );
		}
		#endregion
	}
}
