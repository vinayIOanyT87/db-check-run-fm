namespace TransactionFields
{
	using System.Globalization;
	using System.Web.UI;

	using FMBusinessObjects.DataObjects;

	using System.Web.UI.WebControls;

	public class TransactionErrorFG : TextFieldGenerator, IHeaderField
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the Transaction Error Field Control.
		/// </summary>
		public TransactionErrorFG ( )
		{
		}
		#endregion

		#region Properties
		public override string FieldID
		{
			get { return "Error"; }
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 1000.
		/// </summary>
		protected override short MaxColumns
		{
			get { return this.GetFieldLength ( this.FieldID, 1000 ); }
		}

		/// <summary>
		/// This field is always non-editable.
		/// </summary>
		public override bool Editable
		{
			get { return false; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method returns the error text as an object type.
		/// </summary>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public object GetDataValue ( TransactionDO transaction )
		{
			return transaction.TransErrorText;
		}

		/// <summary>
		/// This method returns the error text as a string type. It will return
		/// null if the error flag is not set.
		/// </summary>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public string GetDataText(TransactionDO transaction)
		{
			if (transaction != null)
			{
				if (transaction.ErrorFlag)
				{
					var errorText = GetDataValue(transaction) as string;
					return errorText;
				}
				
				return null;
			}
			
			return null;
		}

		/// <summary>
		/// This method sets the value of the error text.
		/// </summary>
		/// <param name="transaction"></param>
		/// <param name="newValue"></param>
		public void SetDataValue ( TransactionDO transaction, object newValue )
		{
			var stringTemp = newValue as string;

			if ( stringTemp != null )
			{
				stringTemp = stringTemp.Trim ( );
			}

			if (stringTemp != null)
			{
				transaction.TransErrorText = stringTemp.Trim();
			}

			OnFieldChanged ( );
		}

		/// <summary>
		/// This method set the specialization of the control. The control will span 4 columns, 
		/// have 3 rows, and the column width of 120.
		/// </summary>
		/// <param name="control"></param>
		protected override void SpecializeControl ( WebControl control )
		{
			var localCell = control as TableCell;

			if (localCell != null)
			{
				var updatePanel = localCell.Controls[0] as UpdatePanel;

				localCell.ColumnSpan = 4;

				if (updatePanel != null)
				{
					var textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;

					if (textBox != null)
					{
						textBox.TextMode = TextBoxMode.MultiLine;
						textBox.Columns = 120;
						textBox.Rows = 3;
						textBox.Wrap = true;
						textBox.CssClass = "formfield";

						textBox.Attributes.Add("maxLength", this.MaxColumns.ToString(CultureInfo.InvariantCulture));
					}
				}
			}
		}
		#endregion
	}
}
