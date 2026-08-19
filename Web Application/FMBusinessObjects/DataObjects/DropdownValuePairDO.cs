using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FMBusinessObjects.DataObjects
{
    [DataContract]
    [Serializable]
    public class DropdownValuePairDO
    {
        [DataMember]
        public string Text { get; set; }

        /// <summary>
        /// This property sets and gets the text value of a dropdown list control.
        /// </summary>
        [DataMember]
        public string TextValue { get; set; }
    }
}
