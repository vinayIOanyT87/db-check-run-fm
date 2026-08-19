using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.Specialized;

namespace FMBusinessObjects.DataObjects
{
	#region Import List Item class.
	[DataContract]
   [Serializable]
	public class ImportListItem
	{
	}
	#endregion

	#region Import List data object class.
	/// <summary>
	/// Summary description for ImportListDO.
	/// </summary>
	[DataContract]
   [Serializable]
	public class ImportListDO : DataObject
	{
		#region Protected data members
		protected StringCollection importTypeList;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the import list data
		/// object class.
		/// </summary>
		public ImportListDO ( )
		{
			importTypeList = new StringCollection ( );
		}
		#endregion

		#region Properties
		[DataMember]
		public StringCollection ImportTypeList
		{
			get { return this.importTypeList; }
			private set { this.importTypeList = value; }
		}
		#endregion

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
		#endregion SQL methods
	}
	#endregion
}
