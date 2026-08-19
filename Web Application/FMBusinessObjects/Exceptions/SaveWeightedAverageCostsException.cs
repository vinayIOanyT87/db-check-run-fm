using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;
using System.Security;

namespace FMBusinessObjects.Exceptions
{
   [Serializable]
   [DataContract]
	public class SaveWeightedAverageCostsException : AccountingServicesException, ISerializable
	{
		#region Attributes
		protected ArrayList results;
		#endregion Attributes

		#region Properties
		public ArrayList Results
		{
			get { return this.results; }
			private set { this.results = value; }
		}
		#endregion // Properties

		#region Constructors
		public SaveWeightedAverageCostsException ( ArrayList results )
		{
			this.results = results;
		}
		#endregion

		#region ISerializable Members
		protected SaveWeightedAverageCostsException ( SerializationInfo info, StreamingContext context )
		{
			//			base.GetObjectData(info, context);
			this.results = (ArrayList) info.GetValue ( "results", typeof ( System.Collections.ArrayList ) );
		}

		[SecurityCritical]
		override public void GetObjectData ( SerializationInfo info, StreamingContext context )
		{
			//			base.GetObjectData(info, context);
			info.AddValue ( "results", this.results );
		}
		#endregion // ISerializable Members
	}
}
