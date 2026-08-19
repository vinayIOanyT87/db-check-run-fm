
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml;

namespace FMDataExchange
{
	/// <summary>
	/// This is the Exception class used by the MessageInspector 
	/// </summary>
	public class MessageInspectorError : ApplicationException
	{
		public MessageInspectorError(string message)
			:base(message)
		{
		}
	}

	/// <summary>
	/// See comment from MessageInspectionBehaviorExtension class
	/// </summary>
	public class MessageInspector : IDispatchMessageInspector
	{
		/// <summary>
		/// When customBinding is used and there is a fault/exception, WCF service passes back to the client with an "Action" header.
		/// When the client is handheld/using compact .Net framework, it does not recognize the namespace for the fault action.
		/// The way I found to trick the client is to define an action that the client recognizes.  
		/// Hence a NotUsed method is declared with the following response action.
		/// Also, when there is a fault/exception, the action namespace will be replaced with this namespace using MessageInspector.
		/// </summary>
		public const string FaultExceptionActionName = "http://tempuri.org/IExchangeService/FaultExceptionAction";

		/// <summary>
		/// In order to read from the message, we need to make a copy.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		private static Message PrepareMessage(ref Message request)
		{
			try
			{
				// Not sure why this is done this way, just copy from the web
				MessageBuffer buffer = request.CreateBufferedCopy(Int32.MaxValue);
				request = buffer.CreateMessage();
				return buffer.CreateMessage();
			}
			catch (Exception error)
			{
				throw new MessageInspectorError("Invalid message header - Error copying the message for checking in PrepareMessage : " + error.Message);
			}
		}

		///  <summary>
		///  This reads the Security header as XML from the message.  The header has the following format:
		///  
		///  <s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/" xmlns:a="http://www.w3.org/2005/08/addressing">
		/// 		<s:Header>
		/// 			<Action u:Id="_3" p1:mustUnderstand="1" xmlns="http://www.w3.org/2005/08/addressing" xmlns:u="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd" xmlns:p1="http://schemas.xmlsoap.org/soap/envelope/">http://tempuri.org/IExchangeService/Exchange</Action>
		/// 			<To u:Id="_4" p1:mustUnderstand="1" xmlns="http://www.w3.org/2005/08/addressing" xmlns:u="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd" xmlns:p1="http://schemas.xmlsoap.org/soap/envelope/">http://b1cswr1.us.saic.com:8401/FuelsManager/FMDataExchange/ExchangeService.svc</To>
		/// 			<o:Security s:mustUnderstand="1" xmlns:o="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd">
		/// 				<u:Timestamp u:Id="uuid-faf70a8d-da09-4b63-a6c1-7df5c4ac588c-14" xmlns:u="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd">
		/// 					<u:Created>2013-01-15T19:04:59.000Z</u:Created>
		/// 					<u:Expires>2013-01-15T19:09:59.000Z</u:Expires>
		/// 				</u:Timestamp>
		///				</o:Security>
		///			</s:Header>
		///	</s:Envelope>
		///  </summary>
		/// <param name="headerString"></param>
		private static XmlDocument  PrepareHeaderXml(string headerString)
		{
			try
			{
				XmlDocument securityXml = new XmlDocument();
				securityXml.LoadXml(headerString);
				return securityXml;
			}
			catch (Exception error)
			{
				throw new MessageInspectorError("Invalid message header - Error loading the Secruity Header XML(PrepareHeaderXml) : " + error.Message);
			}
		}

		/// <summary>
		/// Load an Xml node with the given name
		/// </summary>
		/// <returns></returns>
		private static XmlNode LoadXmlNode(XmlNode parentNode, string nodeName)
		{
			return LoadXmlNode(parentNode, nodeName, string.Empty);
		}

		/// <summary>
		/// Load an Xml node with the given name
		/// </summary>
		/// <returns></returns>
		private static XmlNode LoadXmlNode(XmlNode parentNode, string nodeName, string namespaceString)
		{
			
			try
			{
				XmlNode targetNode = parentNode.SelectSingleNode(string.Format("./*[local-name()='{0}']", nodeName));
				if (targetNode == null)
				{
					throw new MessageInspectorError(string.Format("Invalid message header - Error loading the Header XML(LoadXmlNode). (blank targetNode)"));
				}

				if ((!string.IsNullOrEmpty(namespaceString))
						&& (String.Compare(namespaceString, targetNode.NamespaceURI, StringComparison.OrdinalIgnoreCase) != 0))
				{
					throw new MessageInspectorError(string.Format("Invalid message header - {0} Node in the Header XML(LoadXmlNode) has the wrong namesapce.", nodeName));
				}

				return targetNode;
			}
			catch (MessageInspectorError)
			{
				throw;
			}
			catch (Exception error)
			{
				throw new MessageInspectorError(string.Format("Invalid message header - Error loading the {0} from the Header XML(LoadXmlNode) : {1}", nodeName, error.Message));
			}

		}

		/// <summary>
		/// Load the Id off the Security/Timestamp node
		/// </summary>
		/// <param name="timestampNode"></param>
		/// <returns></returns>
		private static string LoadMessageID(XmlNode timestampNode)
		{
			try
			{
				// Get the Id attribute
				string msgIDTag = string.Format("{0}:Id", timestampNode.Prefix);
				XmlNode msgIDAttribute = timestampNode.Attributes.GetNamedItem(msgIDTag);
				if (msgIDAttribute == null)
				{
					throw new MessageInspectorError("Invalid message header - Can't find Id attribute in the Timestamp Node of the Security Header.");
				}

				// validate the ID
				string msgID = msgIDAttribute.InnerText;
				if (string.IsNullOrEmpty(msgID))
				{
					throw new MessageInspectorError("Invalid message header - Id attribute in the Timestamp Node of the Security Header can't be blank");
				}
				return msgID;
			}
			catch (MessageInspectorError)
			{
				throw;
			}
			catch (Exception error)
			{
				throw new MessageInspectorError("Invalid message header - Error retrieving the Security timestamp ID (LoadMessageID) : " + error.Message);
			}
		}

		/// <summary>
		/// Load the Created/Expires Timestamp
		/// </summary>
		/// <param name="timestampNode"></param>
		/// <param name="nodeName"></param>
		/// <returns></returns>
		private static DateTime LoadTimeStamp(XmlNode timestampNode, string nodeName)
		{
			try
			{
				XmlNode timestampChildNode = LoadXmlNode(timestampNode, nodeName);

				string timestampString = timestampChildNode.InnerText;
				DateTime theTimestamp;

				if (!DateTime.TryParse(timestampString, out theTimestamp))
				{
					string msg = string.Format("Invalid message header - {0} of XML Node from Security Header is not in valid DateTime format({1}).", nodeName, timestampString);
					throw new MessageInspectorError(msg);
				}
				return theTimestamp;
			}
			catch (MessageInspectorError)
			{
				throw;
			}
			catch (Exception error)
			{
				string msg = string.Format("Invalid message header - Error retrieving the {0} from the Security header (LoadTimeStamp) : {1}", nodeName, error.Message);
				throw new MessageInspectorError(msg);
			}
		}

		/// <summary>
		/// Retrieve ID, Created, Expires from Security Header's Timestamp
		/// </summary>
		/// <param name="headerString"></param>
		/// <param name="messageID"></param>
		/// <param name="createdTime"></param>
		/// <param name="expiresTime"></param>
		private static void RetrieveInfoFromHeader(string headerString, out string messageID, out DateTime createdTime, out DateTime expiresTime)
		{
			const string SecurityNameSpace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
			const string CreatedTimestampyXmlTag = "Created";
			const string ExpiresTimestampXmlTag = "Expires";
			try
			{				
				// Prepare XML
				XmlDocument xmlDoc = PrepareHeaderXml(headerString);
				XmlNode envelopeNode = LoadXmlNode(xmlDoc, "Envelope");
				XmlNode headerNode = LoadXmlNode(envelopeNode, "Header");
				XmlNode securityNode = LoadXmlNode(headerNode, "Security", SecurityNameSpace);
				XmlNode timestampNode = LoadXmlNode(securityNode, "Timestamp");

				// Load Info
				messageID = LoadMessageID(timestampNode);
				createdTime = LoadTimeStamp(timestampNode, CreatedTimestampyXmlTag);
				expiresTime = LoadTimeStamp(timestampNode, ExpiresTimestampXmlTag);
			}
			catch (MessageInspectorError)
			{
				throw;
			}
			catch (Exception error)
			{
				throw new MessageInspectorError("Invalid message header - Error retrieving parameters(RetrieveInfoFromHeader) : " + error.Message);
			}
		}

		public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{
			try
			{
				Message originalMessage = PrepareMessage(ref request);

				string msgID;
				DateTime msgCreatedTime;
				DateTime msgExpiresTime;
				RetrieveInfoFromHeader(originalMessage.ToString(), out msgID, out msgCreatedTime, out msgExpiresTime);

				var reliableMsg = new ReliableMessage(msgID, msgCreatedTime, msgExpiresTime);
				reliableMsg.Validate();
				return null;
			}
			catch (MessageInspectorError)
			{
				throw;
			}			
			catch (Exception error)
			{
				throw new MessageInspectorError("Invalid message header - Error in  AfterReceiveRequest : " + error.Message);
			}
		}

		public void BeforeSendReply(ref Message reply, object correlationState)
		{
			if (reply.IsFault)
			{
				// See comment for FaultExceptionActionName.
				reply.Headers.Action = FaultExceptionActionName;
			}
			MessageBuffer buffer = reply.CreateBufferedCopy(0x7fffffff);
			reply = buffer.CreateMessage();
		}
	}


}