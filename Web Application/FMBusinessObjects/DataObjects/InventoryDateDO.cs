using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class InventoryDateDO : DataObject
	{
		[DataMember]
		public DateTime InventoryDate { get; set; }

		public InventoryDateDO() : base()
		{
		}

		public InventoryDateDO(DateTime calculatedDate) : base()
		{
			InventoryDate = calculatedDate;
		}

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

	}

}
