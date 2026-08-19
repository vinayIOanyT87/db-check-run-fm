using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class TransactionLinkResultDO : DataObject
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the transaction link result
		/// data object class.
		/// </summary>
		public TransactionLinkResultDO ( )
		{
			this.ResultTransIDs = new List<string> ( );
		}
		#endregion

		#region Properties
		[DataMember]
		public List<string> ResultTransIDs 
		{ 
			get; 
			set; 
		}
		#endregion

		#region Overrides
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
		#endregion // Overrides
	}
}
