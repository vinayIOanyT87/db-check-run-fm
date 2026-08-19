using System.Web.UI.WebControls;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for OperatorNameFG.
	/// </summary>
	public class OperatorNameFG : TextFieldGenerator, IHeaderField
	{
		public OperatorNameFG()
		{

		}

		public override string FieldID
		{
			get { return "OperatorName"; }
		}

		public object GetDataValue(TransactionDO trans)
		{
			return trans.OperatorName;
		}

		public string GetDataText(TransactionDO trans)
		{
			return this.GetDataValue(trans).ToString();
		}

		public void SetDataValue(TransactionDO trans, object newValue)
		{
			trans.OperatorName = newValue as string;
			if(this.cell != null)
			{
				var operatorNameTextBox = cell.Controls[0] as TextBox;
				if (operatorNameTextBox != null)
				{
					operatorNameTextBox.Text = trans.OperatorName;
				}
			}

			this.OnFieldChanged();
		}

		public override bool Editable
		{
			get { return false; }
		}

		protected override short MaxColumns
		{
			get { return (short)base.GetFieldLength(FieldID, 60); }
		}
	}
}
