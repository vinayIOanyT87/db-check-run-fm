

namespace Accounting
{
	using System.Collections.Generic;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;


	public class DictionaryStringString : Dictionary<string, string> { }

	public class ExStarsDataDictionary //: IDataDictionary	
	{
		
		DictionaryStringString LookUpDict = new DictionaryStringString();
		public static string[] Keys()
		{
			string[] keys =
				{
					 "ExSTARS StdMonthly"
					, "ExSTARS OutgoingManger"
					, "ExSTARS IncomingManager"
					,"Create Report"
					,"Upload To Server"
					,"View History"
					,"Manager"
					,"File Mode"
					,"Reporting Date"
					,"Create test File"
					,"Errors and Warnings"
					,"Download Report"
					,"Download Raw Report to PC"
					,"Download Easy-Read to PC"
					,"Upload File To Server"
					,"151 Acknowledgement"
					,"Report Previously Submitted to the IRS"
					,"Browse"
					,"Upload"
					,"From"
					,"Date Range"
					,"To"
					,"ExSTARS Std Monthly Report Description"
					,"ExSTARS Outgoing Mgr Report Description"
					,"ExSTARS Incoming Mgr Report Description"
					,"ExSTARS Incoming/Outgoing Mgr Warning"
					,"Confirm Selection"
					,"ExSTARS Recreation Warning"
					,"ExSTARS Recreation Description"
					,"Confirm Recreation"
					,"Original"
					,"Replacement"
					,"Supplemental"
					,"StdMonthly"
					,"OutgoingManger"
					,"IncomingManager"
				};

			return keys;
		}

		public ExStarsDataDictionary(SecurityClass security)
		{
			//foreach (string key in Keys())
			//{
			//	string value = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(security.SiteGuid, key));
			//	LookUpDict.Add(key, value);
			//}
		}

		public string GetText(string key)
		{
			if (LookUpDict.ContainsKey(key))
			{
				return LookUpDict[key];
			}
			return key;
		}

	}
}