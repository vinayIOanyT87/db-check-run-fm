namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    [KnownType(typeof(InventoryReconciliationDO))]
    public class ProductIrdoClass : BaseDataObject
    {
        [DataMember]
        public string Key;
        [DataMember]
        public InventoryReconciliationDO Value;

        public ProductIrdoClass() { }

        public ProductIrdoClass(string key, InventoryReconciliationDO value)
        {
            Key = key;
            Value = value;
        }

    }

    [Serializable]
    [CollectionDataContract]
    public class ProductIrdoCollectionClass : List<ProductIrdoClass>
    {
        public void Add(string key, InventoryReconciliationDO value)
        {
            this.Add(new ProductIrdoClass(key, value));
        }

        public List<string> Keys
        {
            get
            {
                List<string> ret = new List<string>();
                foreach (ProductIrdoClass item in this)
                {
                    ret.Add(item.Key);
                }
                return ret;
            }
        }

        public InventoryReconciliationDO this[string key]
        {
            get
            {
                foreach (ProductIrdoClass item in this)
                {
                    if (item.Key == key)
                    {
                        return item.Value;
                    }
                }
                return null;
            }
        }
    }

}
