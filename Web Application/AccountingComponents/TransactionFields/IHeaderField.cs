using System;
using System.Collections.Specialized;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for IHeaderField.
	/// </summary>
	public interface IHeaderField
	{
		object GetDataValue(TransactionDO transaction);
		string GetDataText(TransactionDO transaction);
		void SetDataValue(TransactionDO transaction, object newValue);
	}
}
