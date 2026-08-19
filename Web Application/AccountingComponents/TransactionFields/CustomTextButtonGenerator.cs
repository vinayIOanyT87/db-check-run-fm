namespace TransactionFields
{
	using System;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;

	using FMControls;

	abstract public class CustomTextButtonGenerator : FieldGenerator
	{
		#region Public Attributes
		public const short FIELD_LENGTH = 30;
		#endregion

		#region Protected Attributes
		protected bool autoPostBack;
		protected CustomTextBoxType type;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the product text box button combo
		/// abstract class.
		/// </summary>
		public CustomTextButtonGenerator(CustomTextBoxType inType)
		{
			this.autoPostBack = false;
			type = inType;
		}
		#endregion

		#region Abstract Properties
		/// <summary>
		/// This property is an abstract property that forces implementation of
		/// returning the maximum column size.
		/// </summary>
		abstract protected short MaxColumns { get; }
		#endregion


		#region Override methods
		/// <summary>
		/// This method will generate the actual web control. In this case, the
		/// FMCompanyTextBox control is being generated.
		/// </summary>
		/// <param name="editable"></param>
		public override void Generate(bool editable)
		{
			var updatePanel = new UpdatePanel
			                          {
				                          ID = this.ID + "Panel",
				                          UpdateMode = UpdatePanelUpdateMode.Conditional
			                          };

			var textBoxButtonCombo = new FMCustomTextBox ( type )
			                         {
				                         ID				= this.ID,
				                         MaxLength		= this.MaxColumns,
				                         Columns		= this.MaxColumns,
				                         AutoPostBack	= this.autoPostBack,
				                         BackColor		= this.VarecBkgrndReadOnlyGray,
				                         Enabled		= editable
			                         };

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
		/// This method is an override method that will return the contents of the FMCompanyTextBox
		/// control.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public override object GetNewValue(WebControl control)
		{
			var updatePanel = control.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var textBoxButtonCombo = updatePanel.ContentTemplateContainer.Controls[0] as FMCustomTextBox;

				if (textBoxButtonCombo != null && 
					(( textBoxButtonCombo.TextMode == TextBoxMode.MultiLine ) &&
				     ( textBoxButtonCombo.Text.Length > this.MaxColumns )))
				{
					string message = this.GetLabel ( control ) + " length must be " + this.MaxColumns + " or less.";
					this.RenderErrorMessage(message);
					throw new RetrieveException ( message );
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
		/// This method will return a product that matches the product ID.
		/// </summary>
		/// <returns></returns>
		protected Object GetCustomObject(Object obj)
		{
			Object result = null;

			if (type == CustomTextBoxType.INVOICE_QUERY)
			{
				var queryGuid = (Guid)obj;

				result = new InvoiceQueryClass();

				if (queryGuid != Guid.Empty)
				{
					result = FMChannelHelper.MakeCall<IInvoiceQueries, object>(
																	 x =>
																	 x.GetByIdentityGuid(this.transContext.security, queryGuid) as Object
																);
				}
			}

			return result;
		}
		#endregion
	}
}
