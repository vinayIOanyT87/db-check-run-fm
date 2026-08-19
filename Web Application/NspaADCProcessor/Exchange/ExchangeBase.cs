// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExchangeBase.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ExchangeBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ADC.Nspa.General
{
    using Nspa;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    using FMBusinessObjects.DataObjects;

    public class ExchangeBase
    {
        [XmlIgnore]
        public ExchangeType ExchangeType { get; set; }

	    private const string Version = "9.0.0.1";

	    public string ExchangeVersion { get; set; }

        public string ClientHostName { get; set; }
        
        public ExchangeBase()
		{
			ExchangeVersion = Version;
        }

        public override string ToString()
        {
            string thisXml;
            var xmlSerializer = new XmlSerializer(this.GetType(), string.Empty);
            using (var writer = new StringWriter())
            {
                xmlSerializer.Serialize(writer, this);
                thisXml = writer.ToString();
            }

            return thisXml;
        }

        public static TExchange CreateExchangeFromXml<TExchange>(string xmlData) where TExchange : ExchangeBase
        {
            try
            {
                using (var stringReader = new StringReader(xmlData))
                {
                    using (var xtr = new XmlTextReader(stringReader))
                    {
                        var xmlSerializer = new XmlSerializer(typeof(TExchange), string.Empty);
                        var exchange = (TExchange)xmlSerializer.Deserialize(xtr);
                        return exchange;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg =
                    string.Format(
                        "Error encountered during CreateExchangeFromXML - type is {0}{1}Error Message: {2}{1}Data is:{1}{3}",
                        typeof(TExchange).ToString(),
                        Environment.NewLine,
                        ex.Message,
                        xmlData.Substring(0, 5000));

                throw new Exception(msg, ex);
            }
        }
    }
}
