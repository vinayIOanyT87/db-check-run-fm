// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpUserAgentBehaviorExtensionElement.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
//   Author: George Peters (reference sample by Paulo Morgado)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessObjects.ChannelFactories
{
    using System;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Configuration;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    public class HttpUserAgentBehaviorExtensionElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get
            {
                return typeof(HttpUserAgentEndpointBehavior);
            }
        }

        protected override object CreateBehavior()
        {
            return new HttpUserAgentEndpointBehavior(this.UserAgent);
        }

        [ConfigurationProperty("userAgent", IsRequired = true)]
        public string UserAgent
        {
            get { return (string)base["userAgent"]; }
            set { base["userAgent"] = value; }
        }
    }

    public class HttpUserAgentMessageInspector : IClientMessageInspector
    {
        // ReSharper disable once InconsistentNaming
        private const string USER_AGENT_HTTP_HEADER = "user-agent";
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private string userAgent;

        public HttpUserAgentMessageInspector(string userAgent)
        {
            this.userAgent = userAgent;
        }

        #region IClientMessageInspector Members
        void IClientMessageInspector.AfterReceiveReply(ref Message reply, object correlationState)
        {
        }

        object IClientMessageInspector.BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            HttpRequestMessageProperty httpRequestMessage;
            object httpRequestMessageObject;

            if (request.Properties.TryGetValue(HttpRequestMessageProperty.Name, out httpRequestMessageObject))
            {
                httpRequestMessage = httpRequestMessageObject as HttpRequestMessageProperty;

                if (httpRequestMessage != null)
                {
                    if (string.IsNullOrEmpty(httpRequestMessage.Headers[USER_AGENT_HTTP_HEADER]))
                    {
                        httpRequestMessage.Headers[USER_AGENT_HTTP_HEADER] = this.userAgent;
                    }
                }
            }
            else
            {
                httpRequestMessage = new HttpRequestMessageProperty();

                httpRequestMessage.Headers.Add(USER_AGENT_HTTP_HEADER, this.userAgent);

                request.Properties.Add(HttpRequestMessageProperty.Name, httpRequestMessage);
            }

            return null;
        }
        #endregion IClientMessageInspector Members
    }

    public class HttpUserAgentEndpointBehavior : IEndpointBehavior
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        public string UserAgent;

        public HttpUserAgentEndpointBehavior(string userAgent)
        {
            this.UserAgent = userAgent;
        }

        #region IEndpointBehavior Members
        void IEndpointBehavior.AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            // ReSharper disable once SuggestUseVarKeywordEvident
            HttpUserAgentMessageInspector inspector = new HttpUserAgentMessageInspector(this.UserAgent);

            clientRuntime.MessageInspectors.Add(inspector);
        }

        void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        void IEndpointBehavior.Validate(ServiceEndpoint endpoint)
        {
        }
        #endregion IEndpointBehavior Members
    }

}