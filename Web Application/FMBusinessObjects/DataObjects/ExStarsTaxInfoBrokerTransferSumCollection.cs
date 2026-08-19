namespace FMBusinessObjects.DataObjects
{
	using System.Collections.Generic;

	/// <summary>
	/// Data to populate TIA segments, sorted by date 
	/// </summary>
	public class ExStarsTaxInfoBrokerTransferSumCollection : SortedDictionary<string, ExStarsBrokerTransferClass>
	{
		/// <summary>
		/// If this key is new to the collection, insert the a copy of the transaction,  else add the gross and net values
		/// to the existing element.  
		/// </summary>
		/// <param name="key">The key format will vary depending on the transaction type</param>
		/// <param name="taxInfoSum">the object will be cloned so prevent leaking changes</param>
		public new void Add(string key, ExStarsBrokerTransferClass taxInfoSum)
		{
			if (!this.ContainsKey(key))
			{
				base.Add(key, taxInfoSum.Clone());
				return;
			}
			this[key].NetVolume += taxInfoSum.NetVolume;
			this[key].GrossVolume += taxInfoSum.GrossVolume;
		}
	}
}
