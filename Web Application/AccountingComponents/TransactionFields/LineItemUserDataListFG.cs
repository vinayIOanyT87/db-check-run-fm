using System;
using System.Collections.Specialized;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for LineItemUserDataListFG.
	/// Used to generate a drop down list for line item user data
	/// 
	/// 06-27-2008	V. Thompson			Created to allow for choosing line item user data from a list
	/// </summary>
	public class LineItemUserDataListFG : DropDownGenerator, ILineItemField
	{
		protected string key;

		public LineItemUserDataListFG(string key)
		{
			this.key = key;
		}

		public override string FieldID
		{
			get { return key; }
		}

		public override HybridDictionary GetEntries()
		{
			System.Collections.Specialized.HybridDictionary listEntries=
				new System.Collections.Specialized.HybridDictionary();

			foreach(FieldClass fieldClass 
						in transContext.aliasClass.DisplayOrder(
				TRANSACTION_SECTION_TYPE.LINE_ITEMS))
			{
				if(key == fieldClass.ID)
				{
					UserDataFieldClass userField =
						fieldClass as UserDataFieldClass;

					listEntries =
						new System.Collections.Specialized.HybridDictionary(
						userField.UserDataListValueCollection.Count);

					foreach(UserDataListValueClass listValue
								in userField.UserDataListValueCollection)
					{
						listEntries.Add(listValue.ID, listValue.ID);
					}
					return listEntries;
				}
			}
			return listEntries;
		}
		#region ILineItemField Members

		public object GetDataValue(LineItemDO lineItem)
		{
			if (lineItem.UserData.ContainsKey(key))
			{
				return lineItem.UserData[key];
			}

			return null;
		}

		public string GetDataText(LineItemDO lineItem)
		{
			if (GetDataValue(lineItem) != null)
			{
				return GetDataValue(lineItem).ToString();
			}
			else
			{
				return null;
			}
		}

		public void SetDataValue(LineItemDO lineItem, object newValue)
		{
			lineItem.UserData[key] = String.Format("{0}", newValue);
			OnFieldChanged();
		}

		#endregion

	}
}
