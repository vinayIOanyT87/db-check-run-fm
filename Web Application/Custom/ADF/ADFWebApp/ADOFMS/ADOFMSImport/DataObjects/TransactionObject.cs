using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ADOFMSImport.DataObjects.Interfaces;

namespace ADOFMSImport.DataObjects
{
	public class TransactionObject : CSVObject, IDataObject
	{
		#region Construction
		public TransactionObject ( Defaults a_defaults )
			: base ( a_defaults )
		{
		}

		public TransactionObject ( SalesObject a_copy )
			: base ( a_copy )
		{
			this.CopyFrom ( a_copy );
		}
		#endregion // Construction

		#region Internals
		internal Hashtable GetColumnMap ( )
		{
			return m_columnMap;
		}
		#endregion // Internals

		#region IDataObject members
		public override void Reset ( )
		{
			base.Reset ( );
		}

		public override DataObject CopyFrom ( DataObject a_copy )
		{
			if (a_copy.GetType ( ) != this.GetType ( ))
			{
				throw new Exception ( "CopyTo() expected type " + this.GetType ( ).ToString ( ) + " but got " + a_copy.GetType ( ).ToString ( ) );
			}

			DataObject copy = a_copy as DataObject;

			m_columnMap = ( copy as TransactionObject ).GetColumnMap ( );
			base.CopyFrom ( copy );

			return this;
		}
		#endregion // IDataObject
	}
}
