using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO;
using BsmeInterfaceLibrary.EBS.IDocs;
using System.Xml.Serialization;

namespace MockEBSService
{
	

	//[System.ServiceModel.ServiceContract]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
	[System.ServiceModel.ServiceContractAttribute(Namespace = "http://gex.ngc.com/pull/soap12")]
	public interface IPull
	{

		// CODEGEN: Generating message contract since the operation listing is neither RPC nor document wrapped.
		[System.ServiceModel.OperationContractAttribute(Action = "http://gex.ngc.com/pull/listing")]
		[System.ServiceModel.XmlSerializerFormatAttribute()]
		listingResponse listing(listingRequest request);

		// CODEGEN: Generating message contract since the operation item is neither RPC nor document wrapped.
		[System.ServiceModel.OperationContractAttribute(Action = "http://gex.ngc.com/pull/item")]
		[System.ServiceModel.XmlSerializerFormatAttribute()]
		itemResponse item(itemRequest request);

		// CODEGEN: Generating message contract since the operation delete is neither RPC nor document wrapped.
		[System.ServiceModel.OperationContractAttribute(Action = "http://gex.ngc.com/pull/delete")]
		[System.ServiceModel.XmlSerializerFormatAttribute()]
		deleteResponse delete(deleteRequest request);
	}

	/// <remarks/>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("svcutil", "3.0.4506.2152")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://gex.ngc.com/pull/soap12")]
	public partial class listingElementType
	{

		private string guidField;

		private string filenameField;

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute(Order = 0)]
		public string guid
		{
			get
			{
				return this.guidField;
			}
			set
			{
				this.guidField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute(Order = 1)]
		public string filename
		{
			get
			{
				return this.filenameField;
			}
			set
			{
				this.filenameField = value;
			}
		}
	}

	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
	[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
	public partial class listingRequest
	{

		[System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://gex.ngc.com/pull/soap12", Order = 0)]
		public string listing_request;

		public listingRequest()
		{
		}

		public listingRequest(string listing_request)
		{
			this.listing_request = listing_request;
		}
	}

	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
	[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
	public partial class listingResponse
	{

		[System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://gex.ngc.com/pull/soap12", Order = 0)]
		[System.Xml.Serialization.XmlArrayItemAttribute("file", IsNullable = false)]
		public listingElementType[] listing_response;

		public listingResponse()
		{
		}

		public listingResponse(listingElementType[] listing_response)
		{
			this.listing_response = listing_response;
		}
	}

	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
	[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
	public partial class itemRequest
	{

		[System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://gex.ngc.com/pull/soap12", Order = 0)]
		public string content_request;

		public itemRequest()
		{
		}

		public itemRequest(string content_request)
		{
			this.content_request = content_request;
		}
	}

	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
	[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
	public partial class itemResponse
	{

		[System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://gex.ngc.com/pull/soap12", Order = 0)]
		[System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")]
		public byte[] content_response;

		public itemResponse()
		{
		}

		public itemResponse(byte[] content_response)
		{
			this.content_response = content_response;
		}
	}

	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
	[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
	public partial class deleteRequest
	{

		[System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://gex.ngc.com/pull/soap12", Order = 0)]
		public string delete_request;

		public deleteRequest()
		{
		}

		public deleteRequest(string delete_request)
		{
			this.delete_request = delete_request;
		}
	}

	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
	[System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
	public partial class deleteResponse
	{

		[System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://gex.ngc.com/pull/soap12", Order = 0)]
		public bool delete_response;

		public deleteResponse()
		{
		}

		public deleteResponse(bool delete_response)
		{
			this.delete_response = delete_response;
		}
	}


	public class Pull : IPull
	{

		public listingResponse listing(listingRequest request)
		{
			listingResponse toRet = new listingResponse();
			Console.WriteLine("Received request to list.");
			System.Collections.Generic.List<listingElementType> listOfItems = new List<listingElementType>();


			string dirPath = ConfigurationManager.AppSettings["sendpath"];
			string dirArchive = Path.Combine(dirPath, "Sent\\");

			if (!Directory.Exists(dirPath))
				Directory.CreateDirectory(dirPath);

			if (!Directory.Exists(dirArchive))
				Directory.CreateDirectory(dirArchive);


			string[] filesToSend = Directory.GetFiles(dirPath, "*.xml", SearchOption.TopDirectoryOnly);

			foreach (string filePath in filesToSend)
			{
				string fileName = new System.IO.FileInfo(filePath).Name;
                listOfItems.Add(new listingElementType() { filename = fileName, guid = fileName });
			}

			toRet.listing_response = listOfItems.ToArray();
			return toRet;
		}


		public itemResponse item(itemRequest request)
		{
			string guid = request.content_request;
			
			Console.WriteLine("Received request to send item: " + guid);

			string dirPath = ConfigurationManager.AppSettings["sendpath"];
			string dirArchive = Path.Combine(dirPath, "Sent\\");

			if (!Directory.Exists(dirPath))
				Directory.CreateDirectory(dirPath);

			if (!Directory.Exists(dirArchive))
				Directory.CreateDirectory(dirArchive);


			string[] filesToSend = Directory.GetFiles(dirPath, guid, SearchOption.TopDirectoryOnly);

			itemResponse toRet = new itemResponse();

			Console.WriteLine("Sending response file: " + filesToSend[0] + ".");
			toRet.content_response = GetFileContents(filesToSend[0]);

			return toRet;
		}


		public deleteResponse delete(deleteRequest request)
		{
			string guid = request.delete_request;
			Console.WriteLine("Received request to delete item: " + guid);
			deleteResponse toRet = null;
			try
			{

				string dirPath = ConfigurationManager.AppSettings["sendpath"];
				string dirArchive = Path.Combine(dirPath, "Sent\\");

				if (!Directory.Exists(dirPath))
					Directory.CreateDirectory(dirPath);

				if (!Directory.Exists(dirArchive))
					Directory.CreateDirectory(dirArchive);

				string[] filesToSend = Directory.GetFiles(dirPath, guid, SearchOption.TopDirectoryOnly);
				dirArchive = Path.Combine(dirArchive, Path.GetFileName(filesToSend[0]));

				try
				{
					if (File.Exists(dirArchive))
						File.Delete(dirArchive);
				}
				catch
				{
					;
				}
				File.Move(filesToSend[0], dirArchive);

				toRet = new deleteResponse(true);
			}
			catch
			{
				toRet = new deleteResponse(false);
			}
			
			return toRet;
		}


		protected static byte[] GetFileContents(string filePath)
		{
			//XmlSerializer serializer = null;
			byte[] toRet = null;
			try
			{

				string fileContents = System.IO.File.ReadAllText(filePath);

				toRet = System.Text.Encoding.ASCII.GetBytes(fileContents);				
			}
			finally
			{

			}
			return toRet;
		}
	}
}
