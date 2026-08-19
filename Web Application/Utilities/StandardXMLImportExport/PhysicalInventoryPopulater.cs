using System;

namespace StandardXMLImportExport
{
	/// <summary>
	/// Summary description for PhysicalInventoryPopulater.
	/// </summary>
	public class PhysicalInventoryPopulater : TransactionPopulater
	{
		public PhysicalInventoryPopulater()
		{
			
		}

		protected override string TransactionTypeID
		{
			get
			{
				return "PhysicalInventory";
			}
		}

		protected override void Populate()
		{

		}

		protected override void PopulateLineItem()
		{
			SetLineItemLineFill();
			SetLineItemBottomVolume();
			SetLineItemNetCapacity();
			SetLineItemTankStatus();
		}


		protected override void SetOwner() { transaction.Owner = this.GetStringValue("Owner", false); }
	}
}
