using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class IntegerDO : DataObject
	{
		#region Construction
		/// <summary>
		/// This is the default constructor for the integer data object class.
		/// </summary>
		public IntegerDO ( ) : base ( )
		{
			Value = 0;
		}
		#endregion // Construction
	
		#region Properties
		[DataMember]
		public int Value 
		{ 
			get; 
			set; 
		}
		#endregion // Properties

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
