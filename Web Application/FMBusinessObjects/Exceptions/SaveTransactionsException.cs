using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Web;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.Exceptions
{
	[System.Serializable]
	[KnownType(typeof(List<TransactionValidationResult>))]
	public class SaveTransactionsException : Exception, ISerializable
	{
		#region Attributes

		protected List<TransactionValidationResult> results;

		/// <summary>
		/// Defines the fault reason used when throwing a SaveTransactionsException
		/// </summary>
		public const string FaultExceptionReason = "An error occurred while saving a transaction";

		#endregion Attributes

		#region Properties
		public List<TransactionValidationResult> Results
		{
			get { return results; }
		}

		public override string Message
		{
		   get
		   {
				string formattedErrorList = "";
				foreach (TransactionValidationResult validationResult in results)
				{
					foreach (string errorMsg in validationResult.ErrorList)
					{
						formattedErrorList += "\r\n" + errorMsg;
					}
				}

		      if (formattedErrorList.Length == 0)
					return "An unknown exception occurred while saving the transaction";
		      else
					return "The following errors occurred while saving the transaction:" + formattedErrorList;
		   }
		}
		#endregion Properties

		public SaveTransactionsException()
		{
		}

		public SaveTransactionsException(List<TransactionValidationResult> results)
		{
			this.results = results;
		}
		
		#region ISerializable Members
		protected SaveTransactionsException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			this.results = (List<TransactionValidationResult>)info.GetValue("results", typeof(List<TransactionValidationResult>));
		}

		[SecurityCritical]
		override public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("results", this.results);
		}
		#endregion
	}
}
