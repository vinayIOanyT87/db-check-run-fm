using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class IncomingTruckGetTxDO : DataObject
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the incoming truck get transaction
		/// data object class.
		/// </summary>
		public IncomingTruckGetTxDO ()
		{
		}
		#endregion

		#region Properties
		[DataMember]
		public DataSet Items 
		{ 
			get; 
			set; 
		}
		#endregion

		#region Public override methods
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
