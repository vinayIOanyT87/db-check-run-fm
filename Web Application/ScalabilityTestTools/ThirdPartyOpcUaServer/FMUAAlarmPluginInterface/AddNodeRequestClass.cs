
namespace FMUAAlarmPluginInterface
{
    using System;
    using System.Xml.Serialization;
    using System.IO;

    using FMBusinessObjects.DataObjects;

	[Serializable]
    public class AddNodeRequestClass
    {
        public string DynamicEntityType;

        public ParameterCollection InputParameters = new ParameterCollection();

        public string Sender;

        public string ToXML()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(AddNodeRequestClass));
            StringWriter tempWriter = new StringWriter();
            serializer.Serialize(tempWriter, this);
            return tempWriter.ToString();
        }

        public static AddNodeRequestClass FromXML(string aXmlString)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AddNodeRequestClass));
                StringReader tempReader = new StringReader(aXmlString);
                AddNodeRequestClass msgData = serializer.Deserialize(tempReader) as AddNodeRequestClass;
                return msgData;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
