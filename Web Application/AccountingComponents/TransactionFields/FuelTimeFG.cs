namespace TransactionFields
{
	using System;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using FMBusinessObjects.DataObjects;

	class FuelTimeFG : TextFieldGenerator, IHeaderField
	{
		#region Constructors
		public FuelTimeFG()
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
			get { return "FuelTime"; }
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
			var beginFG = fieldGenerator.GetFieldGenerator("FST") as DateTimeGenerator;
			var endFG = fieldGenerator.GetFieldGenerator("TimeEnd") as DateTimeGenerator;

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
			TextBox textBox = null;
			var updatePanel = this.cell.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;
			}

			if (textBox == null)
			{
				return;
			}

			textBox.Text = GetFormattedValue();
		}

		#region IHeaderField Members
		public object GetDataValue(TransactionDO transaction)
		{
			if (trans.TimeEnd == null || transaction.RouteSchedule == null || transaction.RouteSchedule.FST == null)
			{
				return null;
			}

			return (trans.TimeEnd.Value - transaction.RouteSchedule.FST.Value).TotalMinutes;
		}

		public string GetDataText(TransactionDO transaction)
		{
			string toRet = string.Empty;
			object val;

			if ((val = GetDataValue(transaction)) != null)
			{
				toRet = String.Format("{0:0} minutes", val);
			}

			return toRet;
		}

		public override string GetFormattedValue()
		{
			string toRet = string.Empty;
			object val;

			if ((val = GetDataValue()) != null)
			{
				toRet = String.Format("{0:0} minutes", val);
			}

			return toRet;
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			OnFieldChanged();
		}
		#endregion
	}
}

