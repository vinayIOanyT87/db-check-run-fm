
namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    using FMBusinessObjects.UtilityObjects;

	[Serializable]
    public sealed class ComplexObj
    {
        public string TypeName = String.Empty;

        public string Xml = String.Empty;

        public ComplexObj()
        {
        }

        public ComplexObj(object obj)
        {
            this.Initialize(obj);
        }

        public void Initialize(object obj)
        {
            TypeName = obj.GetType().AssemblyQualifiedName;
            XmlSerializer serializer = CachingXmlSerializerFactory.Create(obj.GetType());
            StringWriter tempWriter = new StringWriter();
            serializer.Serialize(tempWriter, obj);
            Xml = tempWriter.ToString();
        }

        public object GetObject()
        {
            try
            {
                Type complexType = Type.GetType(TypeName);
                if (complexType == null)
                {
                    return null;
                }
                else
                {
                    var serializer = CachingXmlSerializerFactory.Create(complexType);
                    StringReader tempReader = new StringReader(Xml);
                    object msgData = serializer.Deserialize(tempReader) as object;
                    return msgData;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string ToXML()
        {
            XmlSerializer serializer = CachingXmlSerializerFactory.Create(typeof(ComplexObj));
            StringWriter tempWriter = new StringWriter();
            serializer.Serialize(tempWriter, this);
            return tempWriter.ToString();
        }

        public static ComplexObj FromXML(string aXmlString)
        {
            try
            {
                XmlSerializer serializer = CachingXmlSerializerFactory.Create(typeof(ComplexObj));
                StringReader tempReader = new StringReader(aXmlString);
                ComplexObj msgData = serializer.Deserialize(tempReader) as ComplexObj;
                return msgData;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
