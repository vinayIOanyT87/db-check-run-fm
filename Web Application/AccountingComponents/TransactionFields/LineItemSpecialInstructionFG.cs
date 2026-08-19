namespace TransactionFields
{
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;
	using System;

	internal class LineItemSpecialInstructionFG : LineItemNoteFG, ILineItemField, ISublineItemField
	{
		public LineItemSpecialInstructionFG()
		{
			virtualField = true;
		}

		public override string FieldID
		{
			get
			{
				return "LineItem SpecialInstructions";
			}
		}

		public override bool Required
		{
			get
			{
				return false;
			}
		}

		public override bool Editable
		{
			get
			{
				return false;
			}
		}

		public override object GetDataValue(LineItemDO inLineItem)
		{
			return inLineItem.SpecialInstructions;
		}

		public string GetDataText(LineItemDO inLineItem)
		{
			if (GetDataValue(inLineItem) != null)
			{
				return GetDataValue(inLineItem).ToString();
			}
			
			return null;
		}

		public override void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			// read-only field value - not setable
			OnFieldChanged();
		}

		public override object GetNewValue(WebControl control)
		{
			return string.Empty;
		}

		object ISublineItemField.GetDataValue(SubLineItemDO inSublineItem)
		{
			return inSublineItem.SpecialInstructions;
		}

		string ISublineItemField.GetDataText(SubLineItemDO inSublineItem)
		{
			return inSublineItem.SpecialInstructions;
		}

		void ISublineItemField.SetDataValue(SubLineItemDO inSublineItem, object newValue)
		{
			OnFieldChanged();
		}

		public override Guid GetDataIdentityGuid()
		{
			if (this.sublineItem != null)
			{
				return this.sublineItem.SpecialInstructionsNoteGuid;
			}

			return lineItem.SpecialInstructionsNoteGuid;
		}
	}
}
