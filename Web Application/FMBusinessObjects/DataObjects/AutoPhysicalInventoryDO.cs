using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	/// <summary>
	/// Summary description for AutoPhysicalInventoryDO.
	/// </summary>
	[DataContract]
   [Serializable]
   public class AutoPhysicalInventoryDO : DataObject
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the Auto Physical Inventory data object class.
		/// </summary>
		public AutoPhysicalInventoryDO ( )
		{

		}
		#endregion

		#region Public override methods
		public override string getSelectCommand ( )
		{
			return null;
		}
		public override string getDeleteCommand ( )
		{
			return null;
		}
		public override string getInsertCommand ( )
		{
			return null;
		}
		public override string getUpdateCommand ( )
		{
			return null;
		}
		#endregion
	}
}
