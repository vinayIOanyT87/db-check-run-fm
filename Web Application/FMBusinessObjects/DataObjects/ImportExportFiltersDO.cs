using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class ImportExportFiltersDO : DataObject
	{
		#region Constructor
		/// <summary>
		/// This is the default constructor for the import export filters 
		/// data object class.
		/// </summary>
		public ImportExportFiltersDO ( )
		{
		}
		#endregion

		#region Properties
		[DataMember]
		public long FilterID
		{
			get;
			set;
		}

		[DataMember]
		public long ConfigurationID
		{
			get;
			set;
		}

		[DataMember]
		public string Site
		{
			get;
			set;
		}

		[DataMember]
		public string Role
		{
			get;
			set;
		}

		[DataMember]
		public string CompanyName
		{
			get;
			set;
		}

		[DataMember]
		public string CompanyID
		{
			get;
			set;
		}
		#endregion

		#region Public Override Methods
		override public string getDeleteCommand ( )
		{
			return null;
		}

		override public string getUpdateCommand ( )
		{
			return null;
		}

		override public string getSelectCommand ( )
		{
			return null;
		}

		override public string getInsertCommand ( )
		{
			return null;
		}
		#endregion
	}
}
