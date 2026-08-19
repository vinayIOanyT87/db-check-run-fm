namespace TransactionFields
{
	using System;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using FMBusinessObjects.DataObjects;

	class ResponseTimeFG : TextFieldGenerator, IHeaderField
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the Line Item Meter Total field generator.
		/// </summary>
		public ResponseTimeFG()
		{
			virtualField = true;
		}
		#endregion

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 10.
		/// </summary>
		protected override short MaxColumns
		{
			get { return this.GetFieldLength(FieldID, 20); }
		}

		public override string FieldID
		{
			get { return "ResponseTime"; }
		}


		/// <summary>
		/// This property will return true if the field is editable.
		/// Otherwise, it will return false.
		/// </summary>
		public override bool Editable
		{
			get
			{
				return false;
			}
		}

		public override void Generate(bool editable)
		{
			base.Generate(editable);

			var beginFG = fieldGenerator.GetFieldGenerator("RequestedDateTime") as DateTimeGenerator;
			var endFG = fieldGenerator.GetFieldGenerator("TimeIn") as DateTimeGenerator;

			OnDateChanged(null);

			if (beginFG != null)
			{
				beginFG.FieldChanged += this.OnDateChanged;
			}

			if (endFG != null)
			{
				endFG.FieldChanged += this.OnDateChanged;
			}
		}

		protected void OnDateChanged(FieldGenerator dateField)
		{
			var updatePanel = this.cell.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;

				if (textBox != null)
				{
					textBox.Text = this.GetFormattedValue();
				}
			}
		}

		#region IHeaderField Members
		public object GetDataValue(TransactionDO transaction)
		{
			if (transaction.RequestedDateTime == null || transaction.TimeIn == null)
			{
				return null;
			}

			return (transaction.TimeIn.Value - transaction.RequestedDateTime.Value).TotalMinutes;
		}

		public string GetDataText(TransactionDO transaction)
		{
			string responseTime = string.Empty;
			object val;

			if ((val = GetDataValue(transaction)) != null)
			{
				responseTime = String.Format("{0:0} minutes", val);
			}

			return responseTime;
		}

		public override string GetFormattedValue()
		{
			string responseTime = string.Empty;
			object val;

			if (( val = GetDataValue()) != null)
			{
				responseTime = String.Format("{0:0} minutes", val);
			}

			return responseTime;
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			OnFieldChanged();
		}
		#endregion
	}
}
