// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlObjConverter.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the XmlObjConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.UtilityObjects
{
	using System;
	using System.IO;
	using System.Text;
	using System.Xml;
	using System.Xml.Schema;
	using System.Xml.Serialization;

	using FMBusinessObjects.Exceptions;

	/// <summary>
	/// The XML object converter.
	/// </summary>
	public static class XmlObjConverter
	{

        public static string PrettyPrint(String XML, out int offset)
        {
            String Result = XML;

            MemoryStream MS = new MemoryStream();
            XmlTextWriter W = new XmlTextWriter(MS, Encoding.Unicode);
            XmlDocument D = new XmlDocument();
            offset = 0;
            try
            {
                // Load the XmlDocument with the XML.
                D.LoadXml(XML);

                W.Formatting = Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                D.WriteContentTo(W);
                W.Flush();
                MS.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                MS.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                StreamReader SR = new StreamReader(MS);

                // Extract the text from the StreamReader.
                String FormattedXML = SR.ReadToEnd();

                Result = FormattedXML;

                offset = FormattedXML.Length - D.OuterXml.Length;


                MS.Close();
                W.Close();
            }
            catch (XmlException)
            {
            }
            return Result;
        }

		/// <summary>
		/// Creates an object from an XML string.
		/// </summary>
		/// <param name="xml">
		/// The XML.
		/// </param>
		/// <param name="objType">
		/// The object type.
		/// </param>
		/// <returns>
		/// The <see cref="object"/>.
		/// </returns>
		/// <exception cref="FMXmlException">XML reader exception.
		/// </exception>
		public static object FromXml ( string xml, Type objType )
		{
			object deserializedObject;
			XmlReader xmlReader = null;
			StringReader stringReader = null;

			try
			{
				var xmlSerializer = new XmlSerializer(objType);
				stringReader = new StringReader(xml);
				var xmlReaderSettings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null };

				xmlReader = XmlReader.Create(stringReader, xmlReaderSettings);
				deserializedObject = xmlSerializer.Deserialize(xmlReader);
			}
			catch ( Exception ex )
			{
				throw new FMXmlException(new XmlException(ex.Message, ex.InnerException));
			}
			finally
			{
				if (xmlReader != null)
				{
					xmlReader.Close( );
				}

				if (stringReader != null)
				{
					stringReader.Close( );
				}
			}

			return deserializedObject;
		}

		/// <summary>
		/// Serializes the object to an XML string.
		/// </summary>
		/// <param name="objectToSerialize">
		/// The object to serialize.
		/// </param>
		/// <param name="objType">
		/// The object type.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public static string ToXml ( object objectToSerialize, Type objType )
		{
			var xmlSerializer	= new XmlSerializer(objType);
			var memStream		= new MemoryStream( );
			var xmlWriter		= new XmlTextWriter(memStream, Encoding.UTF8) { Namespaces = true };

			xmlSerializer.Serialize(xmlWriter, objectToSerialize);
			xmlWriter.Close( );
			memStream.Close( );

			string xml = Encoding.UTF8.GetString(memStream.GetBuffer( ));
			xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
			xml = xml.Substring(0, xml.LastIndexOf(Convert.ToChar(62)) + 1);

			return xml;
		}

		/// <summary>
		/// Serializes the object to an XML string with serializer namespace.
		/// </summary>
		/// <param name="objectToSerialize">
		/// The object to serialize.
		/// </param>
		/// <param name="objectType">
		/// The object type.
		/// </param>
		/// <param name="settings">
		/// The settings.
		/// </param>
		/// <param name="serializerNamespaces">
		/// The serializer namespaces.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public static string ToXml(object objectToSerialize, Type objectType, XmlWriterSettings settings, XmlSerializerNamespaces serializerNamespaces)
		{
			var xmlSerializer	= new XmlSerializer(objectType);
			var memStream		= new MemoryStream( );
			var xmlWriter		= XmlWriter.Create(memStream, settings);

			xmlSerializer.Serialize(xmlWriter, objectToSerialize, serializerNamespaces);
			xmlWriter.Close( );
			memStream.Close( );

			string xml = Encoding.UTF8.GetString(memStream.GetBuffer( ));
			xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
			xml = xml.Substring(0, xml.LastIndexOf(Convert.ToChar(62)) + 1);

			return xml;
		}

        //Serializes the <i>Obj</i> to an XML string.
        public static string ToXml(object objectToSerialize, System.Type ObjType, string defaultNameSpacePrefix, string defaultNameSpaceReference)
        {
            XmlSerializerNamespaces SerNS = new XmlSerializerNamespaces();
            SerNS.Add(defaultNameSpacePrefix, defaultNameSpaceReference);
            XmlSerializer ser;
            ser = new XmlSerializer(ObjType,defaultNameSpacePrefix);
            MemoryStream memStream;
            memStream = new MemoryStream();
            XmlTextWriter xmlWriter;
            xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);
            xmlWriter.Namespaces = true;
            ser.Serialize(xmlWriter, objectToSerialize);
            xmlWriter.Close();
            memStream.Close();
            
            string xml;
           // xml = stringWriter.ToString();
            xml = Encoding.UTF8.GetString(memStream.GetBuffer());
            xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
            xml = xml.Substring(0, (xml.LastIndexOf(Convert.ToChar(62)) + 1));
            return xml;
            
        }

		/// <summary>
		/// The validate XML.
		/// </summary>
		/// <param name="xmlData">
		/// The XML data.
		/// </param>
		/// <param name="xsdNamePath">
		/// The XSD name path.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		/// <exception cref="XmlException">XML reader exception.
		/// </exception>
		public static bool ValidateXml(string xmlData, string xsdNamePath)
		{
			var xmlDoc			= new XmlDocument( );
			var settings		= new XmlReaderSettings { XmlResolver = null, DtdProcessing = DtdProcessing.Prohibit };
			var stringReader	= new StringReader(xmlData);
			XmlReader reader	= null;

			try
			{
				reader = XmlReader.Create(stringReader, settings);
				xmlDoc.Load(reader);
			}
			catch ( Exception ex )
			{
				throw new XmlException(ex.Message, ex.InnerException);
			}
			finally
			{
				if (reader != null)
				{
					reader.Close( );
				}

				stringReader.Close( );
			}

			return ValidateXml(xmlDoc, xsdNamePath);
		}

		/// <summary>
		/// The validate XML.
		/// </summary>
		/// <param name="xmlDoc">
		/// The XML document.
		/// </param>
		/// <param name="xsdNamePath">
		/// The XSD name path.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		/// <exception cref="XmlException">Validation exception.
		/// </exception>
		public static bool ValidateXml(XmlDocument xmlDoc, string xsdNamePath)
		{
			if ( string.IsNullOrEmpty(xsdNamePath) )
			{
				throw new XmlException("Invalid XSD.", new Exception("FMCommon.XmlObjConverter.ValidateXml: XSD name is not specified."));
			}

			try
			{
				var schemaSet = new XmlSchemaSet( );
				schemaSet.Add(null, xsdNamePath);
				schemaSet.Compile( );

				xmlDoc.Schemas = schemaSet;
				xmlDoc.Validate(MyValidationEventHandler);
			}
			catch ( Exception ex )
			{
				throw new XmlException(ex.Message, ex.InnerException);
			}

			return true;
		}

		/// <summary>
		/// The my validation event handler.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		/// <exception cref="XmlException">Validation exception.
		/// </exception>
		public static void MyValidationEventHandler(object sender, ValidationEventArgs e)
		{
			throw new XmlException("Xml validation failed.", new Exception(e.Message));
		}
	}
}
