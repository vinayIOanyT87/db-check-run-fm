namespace TransactionFields
{
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;

	public class LineItemParentDocumentNumberFG : ParentTextFieldGenerator, ILineItemField
	{
		#region Attributes
		protected TransactionTypes m_parentType = TransactionTypes.T_Maximum;
		#endregion // Attributes

		public LineItemParentDocumentNumberFG()
		{
			virtualField = true;
		}

		public LineItemParentDocumentNumberFG(bool multipleFlag)
			: base(multipleFlag)
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem ParentDocumentNumber";
			}
		}

		public override bool Editable
		{
			get
			{
				return false;
			}
		}

		public virtual object GetDataValue(LineItemDO inLineItem)
		{
			string returnVal = string.Empty;

			// get the parent association
			List<TransactionDO> parentTransList = this.LoadParentTransactions(m_parentType, this.trans);

			TransactionDO matchingParent = null;

			if (parentTransList != null)
			{
				matchingParent = this.GetFirstMatchingParent(parentTransList, inLineItem);
			}

			if (matchingParent != null)
			{
				returnVal = matchingParent.DocumentNumber ?? string.Empty;
			}

			return returnVal;
		}

		public virtual string GetDataText(LineItemDO inLineItem)
		{
			if (GetDataValue(inLineItem) != null)
			{
				return GetDataValue(inLineItem).ToString();
			}

			return null;
		}

		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			// not needed
		}
	}
}
