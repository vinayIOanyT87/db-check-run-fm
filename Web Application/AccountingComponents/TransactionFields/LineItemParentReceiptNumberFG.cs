namespace TransactionFields
{
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;

	public class LineItemParentReceiptNumberFG : LineItemParentDocumentNumberFG
	{
		public LineItemParentReceiptNumberFG()
			: base(true)
		{
			this.m_parentType = TransactionTypes.T8_Receipt;
			virtualField = true;
		}

		public override string FieldID
		{
			get
			{
				return "LineItem ParentReceiptNumber";
			}
		}

		public override bool Editable
		{
			get
			{
				return false;
			}
		}

		protected override short MaxColumns
		{
			get
			{
				return 30;
			}
		}

		public override object GetDataValue(LineItemDO inLineItem)
		{
			var valueList = new List<string>();

			// get the parent association
			List<TransactionDO> parentTransList = this.LoadParentTransactions(this.m_parentType, this.trans);
			List<TransactionDO> matchingParentList = null;

			if (parentTransList != null)
			{
				matchingParentList = this.GetMatchingParents(parentTransList, inLineItem);
			}

			if (matchingParentList != null)
			{
				foreach (TransactionDO transaction in matchingParentList)
				{
					if (transaction.DocumentNumber != null)
					{
						valueList.Add(transaction.DocumentNumber);
					}
				}
			}

			string result = string.Empty;

			foreach (string value in valueList)
			{
				result += value + "\n";
			}

			return result;
		}

		public override string GetDataText(LineItemDO inLineItem)
		{
			if (GetDataValue(inLineItem) != null)
			{
				return GetDataValue(inLineItem).ToString();
			}
			
			return null;
		}
	}
}
