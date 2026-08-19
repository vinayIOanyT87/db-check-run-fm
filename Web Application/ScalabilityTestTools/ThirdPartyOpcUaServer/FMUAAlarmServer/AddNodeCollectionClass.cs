

namespace FMUAAlarmServer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    [Serializable]
    public class AddNodeCollectionClass : List<AddNodeClass>
    {
        public string ToXML()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(AddNodeCollectionClass));
            StringWriter tempWriter = new StringWriter();
            serializer.Serialize(tempWriter, this);
            return tempWriter.ToString();
        }

        public void ToFile(string aFilePath)
        {
            var serializer = new XmlSerializer(typeof(AddNodeCollectionClass));
            var fs = new FileStream(aFilePath, FileMode.CreateNew);
            TextWriter tw = new StreamWriter(fs);
            serializer.Serialize(tw, this);
        }

        public static AddNodeCollectionClass FromFile(string aFilePath)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AddNodeCollectionClass));
                FileStream fs = new FileStream(aFilePath,FileMode.Open);
                TextReader tr = new StreamReader(fs);
                AddNodeCollectionClass msgData = serializer.Deserialize(tr) as AddNodeCollectionClass;
                return msgData;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static AddNodeCollectionClass FromXML(string aXmlString)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AddNodeCollectionClass));
                StringReader tempReader = new StringReader(aXmlString);
                AddNodeCollectionClass msgData = serializer.Deserialize(tempReader) as AddNodeCollectionClass;
                return msgData;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
