using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ADOFMSImport.DataObjects.Interfaces;

namespace ADOFMSImport.DataObjects
{
	public class SalesObject : CSVObject, IDataObject
	{
		public enum Columns : int
		{
			QUANTITY,
			ACTUAL_INVENTORY_DATE,
			CUSTOMER,
			BILLTO,
			PRODUCT,
			ASSETID,
			FUEL_CARD_ID
		}

		#region Construction
		public SalesObject ( Defaults a_defaults )
			: base ( a_defaults )
		{
			m_columnMap = new Hashtable ( )
		 {
			{(int)Columns.QUANTITY, "LITRES"},
			{(int)Columns.ACTUAL_INVENTORY_DATE, "LOCAL TIME"},
			{(int)Columns.CUSTOMER, "UNIT"},
			{(int)Columns.BILLTO, "UNIT"},
			{(int)Columns.PRODUCT, "TYPE"},
			{(int)Columns.ASSETID, "REGO"},
			{(int)Columns.FUEL_CARD_ID, "CARD"}
		 };
		}

		public SalesObject ( IssuesObject a_copy )
			: base ( a_copy )
		{
			this.CopyFrom ( a_copy );
		}
		#endregion // Construction

		#region IDataObject members
		public override void Reset ( )
		{
			base.Reset ( );
		}

		public override DataObject CopyFrom ( DataObject a_copy )
		{
			return base.CopyFrom ( a_copy );
		}
		#endregion // IDataObject

		public override bool IsAcceptableRow ( object[] row )
		{
			bool result = false;

			int transNameIndex = GetColumnOrder ( CSVObject.COLUMN_FMTRANSNAME );
			if (transNameIndex >= 0)
			{
				if (row[transNameIndex].ToString ( ).ToUpper ( ).Contains ( "SALE" ))
				{
					result = true;
				}
			}

			return result;
		}
	}
}
