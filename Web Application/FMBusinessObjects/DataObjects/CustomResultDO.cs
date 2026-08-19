using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

using FMBusinessObjects.Exceptions;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class CustomResultDO : DataObject
	{
		#region Attributes
		[DataMember]
		protected int m_savedCount;
		#endregion // Attributes

		#region Properties
		
		public int SavedCount
		{
			get { return m_savedCount; }
			set { m_savedCount = value; }
		}

		[DataMember]
		public List<AccountingServicesException> Errors 
		{ 
			get; 
			set; 
		}

		[DataMember]
		public List<AccountingServicesException> Warnings 
		{ 
			get; 
			set; 
		}
		#endregion // Properties

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Custom Result Data Object class.
		/// </summary>
		public CustomResultDO ( )
		{
			this.Errors	  = new List<AccountingServicesException> ( );
			this.Warnings = new List<AccountingServicesException> ( );
		}
		#endregion

		#region Static utilities
		static public void WriteLog ( string a_source, List<AccountingServicesException> a_list, EventLogEntryType a_type )
		{
			EventLog eventLog = new EventLog ( "Application", ".", "FuelsManager" );

			foreach (AccountingServicesException e in a_list)
			{
				eventLog.WriteEntry ( a_source + " - " + e.Message, a_type );
			}
		}
		#endregion // Static utilities

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
