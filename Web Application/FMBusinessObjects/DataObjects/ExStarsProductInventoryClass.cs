
namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	public class ExStarsProductInventoryListClass : Dictionary<string, ExStarsProductInventoryClass>
	{
		public void Add(ExStarsProductInventoryClass element)
		{
			Add( element.TaxCode, element);
		}
	}


	/// <summary>
	/// The configuration as found within the database for a single ExStars site for a single manager
	/// This class has data and validation, but lacks logic to populate itself
	/// </summary>
	[Serializable]
	[DataContract]
	public class ExStarsProductInventoryClass : ExStarsBaseTransactionClass
	{

		[DataMember]
		public Guid ManagerCompanyGuid { get; set; }

		[DataMember]
		public bool AviationFuelFlag { get; set; }

		[DataMember]
		public bool PriorInventoryExists { get; set; }

		[DataMember]
		public DateTime EndingInventoryDate { get; set; }

		[DataMember]
		public int Count { get; set; }


	}
}
