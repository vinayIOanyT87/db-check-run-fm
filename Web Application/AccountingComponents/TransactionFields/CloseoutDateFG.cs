using System;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for CloseoutDateFG.
	/// </summary>
	internal class CloseoutDateFG : DateGenerator, IHeaderField
	{
		public CloseoutDateFG ( )
		{

		}

		public override string FieldID { get { return "CloseoutDate"; } }
		public override bool Editable { get { return false; } }

		#region IHeaderField Members

		public object GetDataValue ( TransactionDO transaction )
		{
			return transaction.CloseoutDate;
		}

		public string GetDataText ( TransactionDO transaction )
		{
			if (GetDataValue(transaction) != null)
			{
				return GetDataValue(transaction).ToString();
			}
			else
			{
				return null;
			}
		}

		public void SetDataValue ( TransactionDO transaction, object newValue )
		{
			//This should not happen because it is never editable.
			System.Diagnostics.Debug.Assert ( false, "CloseoutDateFG.SetDataValue() should never be called." );
		}

		#endregion
	}
}
