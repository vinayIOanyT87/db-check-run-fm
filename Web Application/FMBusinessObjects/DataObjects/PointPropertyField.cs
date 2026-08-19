
namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    using FMBusinessObjects.Attributes;
    using System.Xml.Serialization;
    using System.IO;
    using System.Web.Script.Serialization;

    using FMBusinessObjects.UtilityObjects;

	[KnownType(typeof(string))]
    [KnownType(typeof(double))]
    [KnownType(typeof(bool))]
    [DataContract]
    [Serializable]
    public class PointPropertyField 
    {
        [DataMember]
        [FMPersistedField]
        public EngineeringUnitType EngineeringUnitsType { get; set; }

        protected Type _ValueType;

        [ScriptIgnore]
        [XmlIgnore]
        public Type ValueType
        {
            get
            {
                return _ValueType;
            }
            set
            {
                _ValueType = value;
            }
        }

        public string ValueTypeString
        {
            get
            {
                if (_ValueType != null)
                {
                    return _ValueType.ToString();
                }
                return string.Empty;
            }
            set
            {
                _ValueType = Type.GetType(value);
            }
        }

        protected Object _Value;

        [ScriptIgnore]
        [XmlIgnore]
        [DataMember]
        public Object Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
                if (value != null)
                {
                    _ValueType = value.GetType();
                }
            }
        }

        public string ValueXml
        {
            get
            {
                if (_Value == null)
                {
                    return null;
                }
                XmlSerializer serializer = CachingXmlSerializerFactory.Create(_Value.GetType());
                StringWriter tempWriter = new StringWriter();
                serializer.Serialize(tempWriter, _Value);
                return tempWriter.ToString();
            }

            set
            {
                if (value == null)
                {
                    _Value = null;
                }
                else
                {
                    XmlSerializer serializer = CachingXmlSerializerFactory.Create(_ValueType);
                    StringReader tempReader = new StringReader(value);
                    _Value = serializer.Deserialize(tempReader);
                }
            }
        }

  
        public PointPropertyField Clone()
        {
            var t = (PointPropertyField)this.MemberwiseClone();
            t.ValueXml = ValueXml;
            t.ValueTypeString = ValueTypeString;
            return t;
        }

        public PointPropertyField()
        {
        }

        public PointPropertyField(object value, EngineeringUnitType engineeringUnitsType)
        {
            EngineeringUnitsType = engineeringUnitsType;
            Value = value;
        }


    }
 
}
