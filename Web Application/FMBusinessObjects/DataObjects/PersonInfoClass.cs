using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
    public class PersonInfoClass : BaseDataObject
    {
        [DataMember]
        public string CompanyID { get; set; }

        [DataMember]
        public string CardNumber { get; set; }

        public PersonInfoClass()
        {
        }

    }
}
