using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class CloseoutSiteDO : DataObject
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the Closeout Site Data Object.
		/// </summary>
		public CloseoutSiteDO ( )
		{

		}
		#endregion

		#region Public override methods
		public override string getInsertCommand ( )
		{
			return null;
		}
		public override string getSelectCommand ( )
		{
			return null;
		}
		public override string getUpdateCommand ( )
		{
			return null;
		}
		public override string getDeleteCommand ( )
		{
			return null;
		}
		#endregion
	}
}
