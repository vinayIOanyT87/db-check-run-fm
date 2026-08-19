using System;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemDestinationEquipmentModel.
	/// </summary>
	public class LineItemDestinationEquipmentFG : LineItemEquipmentFG, ILineItemField
	{
		#region Contructors
		public LineItemDestinationEquipmentFG() : base(true)
		{
				
		}
		#endregion

		#region Override Properties
		public override string FieldID { get { return "LineItem DestinationRegistrationID"; } }

		/// <summary>
		/// This property returns the field's maximum column width.
		/// </summary>
		protected override short MaxColumns
		{
			get { return (short) base.GetFieldLength(FieldID, EquipmentTextButtonGenerator.FIELD_LENGTH); }
		}
		#endregion


		public object GetDataValue(LineItemDO lineItem)
		{
			return lineItem.DestinationEQ.RegistrationID;
		}

		public string GetDataText(LineItemDO lineItem)
		{
			return GetDataText(lineItem.DestinationEQ);
		}

		public void SetDataValue(LineItemDO lineItem,	object newValue)
		{
			this.SetEquipment(newValue as string, lineItem.DestinationEQ);
		}

	}
}
