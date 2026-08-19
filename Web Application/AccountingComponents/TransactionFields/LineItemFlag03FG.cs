namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemFlag03FG.
	/// Author: Van Thompson
	/// Created for ADF requirements for generic flag fields on line/sub-line items
	/// </summary>
	public class LineItemFlag03FG : CheckBoxGenerator, ILineItemField, ISublineItemField
	{
		#region Public constants
		public const string CLIENT_SIDE_SCRIPT_LINEITEM_FLAG03 = "CLIENT_SIDE_SCRIPT_LINEITEM_FLAG03";
		public const string CLIENT_SIDE_KEY_LINEITEM_FLAG03 = "CLIENT_SIDE_KEY_LINEITEM_FLAG03";
		#endregion

		public LineItemFlag03FG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem Flag03";
			}
		}

		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			return lineItem.Flag03;
		}

		public string GetDataText(LineItemDO lineItem)
		{
			return lineItem.Flag03.ToString();
		}

		public void SetDataValue(LineItemDO lineItem, object newValue)
		{
			lineItem.Flag03 = (bool)newValue;
			OnFieldChanged();
		}

		#endregion

		#region ISublineItemField Members

		object TransactionFields.ISublineItemField.GetDataValue(SubLineItemDO sublineItem)
		{
			return sublineItem.Flag03;
		}

		string TransactionFields.ISublineItemField.GetDataText(SubLineItemDO sublineItem)
		{
			return sublineItem.Flag03.ToString();
		}

		void TransactionFields.ISublineItemField.SetDataValue(SubLineItemDO sublineItem, object newValue)
		{
			sublineItem.Flag03 = (bool)newValue;
			OnFieldChanged();
		}

		#endregion
	}
}
