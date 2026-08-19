using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class ImportResultsDO : DataObject
	{
		public ImportResultsDO ( )
		{

		}

		#region SQL methods
		override public string getSelectCommand ( )
		{
			return null;
		}
		override public string getInsertCommand ( )
		{
			return null;
		}
		override public string getUpdateCommand ( )
		{
			return null;
		}
		override public string getDeleteCommand ( )
		{
			return null;
		}
		#endregion
	}
}
