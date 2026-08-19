using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class TransactionValidationResult
	{
		#region Attributes
		[DataMember]
		protected string transID;
		[DataMember]
		protected string aliasName;
		[DataMember]
		protected System.Collections.Specialized.StringCollection errorList;
		[DataMember]
		protected System.Collections.Specialized.StringCollection warningList;
		#endregion Attributes

		#region Properties

		public string TransID
		{
			get { return transID; }
			set { transID = value; }
		}

		public string AliasName
		{
			get { return aliasName; }
			set { aliasName = value; }
		}

		public System.Collections.Specialized.StringCollection ErrorList
		{
			get { return errorList; }
		}

		public System.Collections.Specialized.StringCollection WarningList
		{
			get { return warningList; }
		}

		public bool IsValid
		{
			get { return (errorList.Count == 0); }
		}
		public bool HasWarnings
		{
			get { return (warningList.Count > 0); }
		}
		#endregion Properties

		public TransactionValidationResult()
		{
			aliasName = "";
			errorList = new System.Collections.Specialized.StringCollection();
			warningList = new System.Collections.Specialized.StringCollection();
		}
	}
}
