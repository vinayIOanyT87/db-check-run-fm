namespace TransactionFields
{
	using FMControls;
	using FMBusinessObjects.DataObjects;

	public class AssocTxControl : FieldGenerator, IHeaderField
	{
		#region Construction
		public AssocTxControl() { }
		#endregion // Construction

		#region Overrides
		public override bool Editable
		{
			get
			{
				return true;
			}
		}

		public override string FieldID
		{
			get { return "AssocTxControl"; }
		}

		public override void Generate(bool editable)
		{
			var viewBtn = new FMViewAssociatedTxLinkButton();
			this.cell.Controls.Add(viewBtn);

			var addBtn = new FMAddAssociatedTxLinkButton { OnClientClick = "AssociateSingleTx(0)" };
			this.cell.Controls.Add(addBtn);
		}

		public override object GetNewValue(System.Web.UI.WebControls.WebControl control)
		{
			return string.Empty;
		}
		#endregion

		#region IHeaderField Impl

		public object GetDataValue(TransactionDO transaction)
		{
			return string.Empty;
		}

		public string GetDataText(TransactionDO transaction)
		{
			return string.Empty;
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			// NA
		}
		#endregion
	}
}
