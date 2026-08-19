namespace TransactionFields
{
	using System.Collections.Specialized;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for RouteStationGenerator.
	/// </summary>
	public class RouteStationGenerator : DropDownGenerator
	{
		public bool HasEntries
		{
			get;
			set;
		}

		public RouteStationGenerator()
		{
			this.HasEntries = false;
		}

        public override string FieldID { get { return "RouteStation"; } }

        public override HybridDictionary GetEntries()
		{
			IATACodeCollectionClass iataCodesCollection = FMChannelHelper.MakeCall<IIATACodes, IATACodeCollectionClass>(
																	 x =>
																	 x.Enumerate(transContext.security)
																);

			var listEntries = new HybridDictionary(iataCodesCollection.Count, false);

			foreach (IATACodeClass iataCode in iataCodesCollection)
			{
				listEntries.Add(iataCode.ID, iataCode.ID);
			}

			this.HasEntries = listEntries.Count > 0;
			return listEntries;
		}
	}
}
