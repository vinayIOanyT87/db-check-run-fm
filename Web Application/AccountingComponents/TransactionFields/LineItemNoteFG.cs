namespace TransactionFields
{
	using FMBusinessObjects.DataObjects;
	using System;
	using System.Web.UI.WebControls;

	using FMControls;

	/// <summary>
	/// Summary description for LineItemNoteFG.
	/// </summary>
	abstract public class LineItemNoteFG : FieldGenerator
	{
		public LineItemNoteFG()
		{
		}

		abstract public object GetDataValue(LineItemDO lineItem);
		abstract public void SetDataValue(LineItemDO lineItem, object newValue);
		abstract public Guid GetDataIdentityGuid();

		public override void Generate(bool editable)
		{
			//Create View button.
			var viewButton = new FMButton { ID = this.ID + " ViewButton", Text = "View" };
			viewButton.Attributes.Add("onclick", "InstructionsButton_Click('" + this.GetDataIdentityGuid() + "')");

			cell.HorizontalAlign = HorizontalAlign.Center;
			cell.Controls.Add(viewButton);

			var dataText = this.GetDataValue() as string;

			viewButton.Visible = !string.IsNullOrEmpty(dataText);
			viewButton.Enabled = true;
		}

		protected override bool GeneratedField
		{
			get
			{
				return true;
			}
		}
	}

}
