using System;

using FM7Accounting;

namespace StandardXMLImportExport
{
	/// <summary>
	/// Summary description for SecondaryAdjustmentPopulater.
	/// </summary>
	public class SecondaryAdjustmentPopulater : TransactionPopulater
	{
		public SecondaryAdjustmentPopulater()
		{

		}

		protected override string TransactionTypeID
		{
			get
			{
				return "SecondaryAdjustment";
			}
		}

		protected override void Populate()
		{
//			PopulateGroundEquipment(transaction, doc);
		}

		protected override void PopulateLineItem()
		{
			
		}

//		protected void PopulateGroundEquipment(SuperTransactionDO transaction, System.Xml.XmlDocument doc)
//		{
//			string standardID = GetStringValue("GroundEquipment/StandardID");
//			string id = GetStringValue("GroundEquipment/ID");
//			string type = GetStringValue("GroundEquipment/Type");
//
//			transaction.RegistrationID = id;
//			transaction.SerialNumber = standardID;
//			transaction.EquipmentType = type;
//		}
	}
}
