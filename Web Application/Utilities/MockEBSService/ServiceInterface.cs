using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;

using System.ServiceModel;
using System.ServiceModel.Description;
using BsmeInterfaceLibrary.EBS.IDocs;
using FMBusinessObjects.LogClient;
using FMBusinessObjects.UtilityObjects;

namespace MockEBSService
{

	[ServiceContract]
	public interface IResponseService
	{
		[OperationContract]
		ZSV_FMD_ACK GetResults();
	}

	[ServiceContract]
	public interface IRequestService
	{
		[OperationContract]
		string SendResults(ZSV_FMD iDoc);
	}

	public class RequestService : IRequestService
	{
		public string SendResults(ZSV_FMD iDoc)
		{
			string fileName = "";
			Console.WriteLine("FMD is sending an idoc.");
			fileName = LogIDoc(iDoc);
			Console.WriteLine("Received idoc and saved to" + fileName +".");
			return fileName;
		}

		private string LogIDoc(BsmeInterfaceLibrary.EBS.IDocs.ZSV_FMD iDoc)
		{

			string dirPath = ConfigurationManager.AppSettings["receivepath"];

			if (!Directory.Exists(dirPath))
				Directory.CreateDirectory(dirPath);

			string fileName = string.Format("Received_{0}_{1}.xml", DateTime.Now.ToString("yyyyddMM-hhmmss"), Guid.NewGuid());
			string filePath = System.IO.Path.Combine(dirPath, fileName);

			using (StreamWriter outfile = new StreamWriter(filePath, true))
			{
				outfile.Write(ToXML(iDoc));
			}

			Console.WriteLine("Logging recieved file to: " + filePath);

			return fileName;

		}

		protected static string ToXML(object objToSerialize)
		{
			XmlSerializer serializer = null;
			//FileStream stream = null;
			try
			{
				StringBuilder sb = new StringBuilder();
				StringWriter output = new StringWriter(sb);
				output.NewLine = String.Empty;
				serializer = new XmlSerializer(objToSerialize.GetType());
				serializer.Serialize(output, objToSerialize);
				return output.ToString();
			}
			catch
			{
				return "";
			}
			finally
			{

			}
		}
	}

	public class ResponseService : IResponseService
	{
		public ZSV_FMD_ACK GetResults()
		{
			Console.WriteLine("Received request to send response.");

			string dirPath = ConfigurationManager.AppSettings["sendpath"];
			string dirArchive = Path.Combine(dirPath, "Sent\\");

			if (!Directory.Exists(dirPath))
				Directory.CreateDirectory(dirPath);

			if (!Directory.Exists(dirArchive))
				Directory.CreateDirectory(dirArchive);


			string[] filesToSend = Directory.GetFiles(dirPath, "*.xml", SearchOption.TopDirectoryOnly);
			ZSV_FMD_ACK toRet = null;

			//only process one file

			if (filesToSend.Length > 0)
			{
				Console.WriteLine("Sending response file: " + filesToSend[0] + ".");
				toRet = FromXML(filesToSend[0]);
				dirArchive = Path.Combine(dirArchive, Path.GetFileName(filesToSend[0]));
				File.Move(filesToSend[0], dirArchive);
			}
			else
			{
				Console.WriteLine("No response files found to send.");
			}

			return toRet;
		}

		protected static ZSV_FMD_ACK FromXML(string filePath)
		{
			XmlSerializer serializer = null;
			ZSV_FMD_ACK toRet = null;
			try
			{
				// Create an instance of the XmlSerializer specifying type and namespace.
				serializer = new XmlSerializer(typeof(ZSV_FMD_ACK));

				// A FileStream is needed to read the XML document.
				FileStream fs = new FileStream(filePath, FileMode.Open);
				TextReader reader = new StreamReader(fs);
				//FileStream fs = new FileStream(filename, FileMode.Open);
				//XmlReader reader = new XmlTextReader(filePath);

				// Use the Deserialize method to restore the object's state.
				toRet = (ZSV_FMD_ACK)serializer.Deserialize(reader);
				fs.Close();
			}
			finally
			{

			}
			return toRet;
		}
	}
}
