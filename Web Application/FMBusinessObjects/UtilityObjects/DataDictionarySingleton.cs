using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.UtilityObjects
{
    [Serializable]
    public sealed class DataDictionarySingleton
    {
        private Dictionary<Guid, DataDictionaryCollectionClass> dictionary = new Dictionary<Guid, DataDictionaryCollectionClass>();

        public static DataDictionarySingleton Instance
        {
            get
            {
                DataDictionarySingleton dict = null;

                if (HttpContext.Current.Items["FMDataDictionary"] == null)
                {
                    dict = new DataDictionarySingleton();
                    HttpContext.Current.Items.Add("FMDataDictionary", dict);
                }
                else
                {
                    dict = (DataDictionarySingleton)HttpContext.Current.Items["FMDataDictionary"];
                }

                return dict;
            }
        }

        private DataDictionarySingleton()
        {
        }

        public static string Get(Guid siteGuid, string key)
        {
            return Instance.GetHelper(siteGuid, key);
        }

        private string GetHelper(Guid siteGuid, string key)
        {

            DataDictionaryCollectionClass localSiteDictionary;

            if (!dictionary.ContainsKey(siteGuid))
            {
                //load from db
                localSiteDictionary = FMChannelHelper.MakeCall<IDataDictionariesClass, DataDictionaryCollectionClass>(x => x.EnumerateCached(siteGuid));

                dictionary.Add(siteGuid, localSiteDictionary);
            }
            else
            {
                localSiteDictionary = dictionary[siteGuid];
                //it is being cached per request so it will get the latest version with every request
            }

            return localSiteDictionary[key];
        }
    }
}
