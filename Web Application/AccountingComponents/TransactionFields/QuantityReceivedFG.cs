namespace TransactionFields
{
	using System;

	using FMBusinessObjects.DataObjects;

	using FMControls;

	/// <summary>
	/// Summary description for QuantityReceivedFG.
	/// </summary>
	abstract class QuantityReceivedFG : LineItemVolumeFG
	{

		public override void Generate(bool editable)
		{
			base.Generate( false );

			//Create View button.
			var viewButton = new FMButton
			                 {
				                 ID = this.ID + " ViewButton",
				                 Text = "View",
				                 Visible = this.transContext.aliasClass.MultipleLineItems == false,
				                 Enabled = this.Editable
			                 };

			viewButton.Click += this.viewButton_Click;
			cell.Controls.Add(viewButton);

			//Create Add button.
			var addButton = new FMButton
			                {
				                ID = this.ID + " AddButton",
				                Text = "Add",
				                Visible = this.transContext.aliasClass.MultipleLineItems == false,
				                Enabled = this.DetermineAddButtonStatus()
			                };

			cell.Controls.Add(addButton);
			addButton.Click += this.addButton_Click;
		}

		private bool DetermineAddButtonStatus()
		{
			try
			{
				return ( trans.TransTypeID == TransactionTypes.T17_Order || 
					     trans.TransTypeID == TransactionTypes.T18_SupplyOrder )
					&& this.transContext.accountingSite.CurrentSite.SiteGroup == false;
			}
			catch
			{
				return false;
			}
		}

		public virtual void viewButton_Click(object sender, EventArgs e)
		{
			InvokeMethodOnCellPage( "QuantityReceivedFG_viewButton_Click", sender );
		}

		public virtual void addButton_Click(object sender, EventArgs e)
		{
			InvokeMethodOnCellPage( "QuantityReceivedFG_addButton_Click", sender );
		}
	}
}
