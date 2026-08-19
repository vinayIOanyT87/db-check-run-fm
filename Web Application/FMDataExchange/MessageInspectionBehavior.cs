using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace FMDataExchange
{
	/// <summary>
	/// MessageInspectionBehaviorExtension and MessageInspectionBehavior are created
	/// for the case the endpoint is setup without ReliableMessaging(ReliableSession).
	/// The behavior classes intercept the message.
	/// The MessageInspector pulls the information from the message.
	/// ReliableMessage class will validate the message.
	/// Handheld's .NET compact framework doesn't support ReliableMessaging.
	/// MessageInspector is created to fulfill Defense requirements:
	///   (APP3820: CAT I) The Designer will ensure web services provide a mechanism for detecting resubmitted SOAP messages. 
	///   (APP3880: CAT I) The Designer will ensure validity periods are verified on all messages using
	///   WS-Security or SAML assertions. 	
	/// Note: ReliableMessage class doesn't implement the full blown ReliableMessage functionality.  
	///       It only covers Defense's requirements.
	/// </summary>
	public class MessageInspectionBehaviorExtension : BehaviorExtensionElement
	{
		protected override object CreateBehavior()
		{
			return new MessageInspectionBehavior();
		}

		public override Type BehaviorType
		{
			get
			{
				return typeof(MessageInspectionBehavior);
			}
		}
	}

	/// <summary>
	/// See comment from MessageInspectionBehaviorExtension class
	/// </summary>
	public class MessageInspectionBehavior : IEndpointBehavior
	{
		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			throw new Exception("Behavior not supported on the consumer side!");
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			try
			{
				endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new MessageInspector());
			}
			catch (Exception error)
			{
				throw new Exception("ApplyDispatchBehavior error: " + error.Message);
			}
		}

		public void Validate(ServiceEndpoint endpoint)
		{
		}
	}
}