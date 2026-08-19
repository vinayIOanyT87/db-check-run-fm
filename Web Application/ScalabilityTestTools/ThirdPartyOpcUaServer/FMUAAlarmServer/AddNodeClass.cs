
namespace FMUAAlarmServer
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    [Serializable]
    public class AddNodeClass
    {
        public string ParentNodeID;

        public string NodeName;

        public string NodeXML;

        public string ToXML()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(AddNodeClass));
            StringWriter tempWriter = new StringWriter();
            serializer.Serialize(tempWriter, this);
            return tempWriter.ToString();
        }

        public static AddNodeClass FromXML(string aXmlString)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AddNodeClass));
                StringReader tempReader = new StringReader(aXmlString);
                AddNodeClass msgData = serializer.Deserialize(tempReader) as AddNodeClass;
                return msgData;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
