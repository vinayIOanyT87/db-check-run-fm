
namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
	 using FMBusinessObjects.UtilityObjects;

	 [Serializable]
    public sealed class ParameterCollection
    {
        public ObjCollection Collection = new ObjCollection();

        public object this[string parameterName]
        {
            get
            {
                return Collection.GetParameterValue(parameterName);
            }
            set
            {
                Collection.Add(parameterName, value);
            }
        }

        public bool HasParameter(string parameterName)
        {
            return Collection.HasParameter(parameterName);
        }

        public List<string> GetParameterNames()
        {
            return Collection.GetParameterNames();
        }

        public string ToXML()
        {
            var serializer = CachingXmlSerializerFactory.Create(typeof(ParameterCollection));
            var tempWriter = new StringWriter();
            serializer.Serialize(tempWriter, this);
            return tempWriter.ToString();
        }

        public static ParameterCollection FromXML(string aXmlString)
        {
            try
            {
                var serializer = CachingXmlSerializerFactory.Create(typeof(ParameterCollection));
                var tempReader = new StringReader(aXmlString);
                var msgData = serializer.Deserialize(tempReader) as ParameterCollection;
                return msgData;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
