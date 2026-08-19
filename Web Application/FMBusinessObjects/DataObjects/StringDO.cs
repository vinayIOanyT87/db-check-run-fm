using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class StringDO : DataObject
	{
		#region Properties
		[DataMember]
		public string Value 
		{ 
			get; 
			set; 
		}
		#endregion // Properties

		#region Construction
		/// <summary>
		/// This is the default constructor for the string data object class.
		/// </summary>
		public StringDO ( ) : base ( )
		{
			Value = null;
		}
		#endregion // Construction

		#region Overrides
		override public string getUpdateCommand ( )
		{
			return null;
		}

		override public string getDeleteCommand ( )
		{
			return null;
		}

		override public string getInsertCommand ( )
		{
			return null;
		}

		override public string getSelectCommand ( )
		{
			return null;
		}
		#endregion
	}
}
