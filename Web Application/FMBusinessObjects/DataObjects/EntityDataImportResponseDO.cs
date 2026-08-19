using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class EntityDataImportResponseDO : DataObject
	{
		#region Public enumeration
		public enum ResponseStatus { SUCCESS, FAIL };
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the entity data import response data object clas.
		/// </summary>
		public EntityDataImportResponseDO()
		{
			Status = ResponseStatus.FAIL;
			ProcessedChangeQueueRecords = new List<ChangeQueueRecordClass>();
		}
		#endregion

		#region Properties
		[DataMember]
		public ResponseStatus Status
		{
			get;
			set;
		}

		[DataMember]
		public string ErrorMessage
		{
			get;
			set;
		}

		[DataMember]
		public List<ChangeQueueRecordClass> ProcessedChangeQueueRecords
		{
			get;
			set;
		}

		[DataMember]
		public DateTimeOffset RequestReceiveTime
		{
			get;
			set;
		}

		[DataMember]
		public DateTimeOffset ResponseSendTime
		{
			get;
			set;
		}
		#endregion

		#region Public override methods
		override public string getUpdateCommand()
		{
			return null;
		}

		override public string getDeleteCommand()
		{
			return null;
		}

		override public string getInsertCommand()
		{
			return null;
		}

		override public string getSelectCommand()
		{
			return null;
		}
		#endregion
	}
}
