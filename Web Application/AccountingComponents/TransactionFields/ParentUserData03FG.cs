namespace TransactionFields
{
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;

	public class ParentUserData03FG : ParentTextFieldGenerator, ILineItemField
	{
		public ParentUserData03FG()
		{
			virtualField = true;
		}

		public override string FieldID
		{
			get
			{
				return "LineItem ParentUserData03";
			}
		}

		public override bool Editable
		{
			get
			{
				return false;
			}
		}

		public object GetDataValue(LineItemDO inLineItem)
		{
			string returnVal = string.Empty;

			// get the parent association which should be an invoice
			List<TransactionDO> parentTransList = this.LoadParentTransactions(TransactionTypes.T21_AccountPayableInvoice, this.trans);

			TransactionDO matchingParent = null;

			if (parentTransList != null)
			{
				matchingParent = this.GetFirstMatchingParent(parentTransList, inLineItem);
			}

			if (matchingParent != null)
			{
				returnVal = matchingParent.UserData3 ?? string.Empty;
			}

			return returnVal;
		}

		public string GetDataText(LineItemDO inLineItem)
		{
			if (GetDataValue(inLineItem) != null)
			{
				return GetDataValue(inLineItem).ToString();
			}
			
			return null;
		}

		public void SetDataValue(LineItemDO transaction, object newValue)
		{
			// not needed
		}
	}
}
