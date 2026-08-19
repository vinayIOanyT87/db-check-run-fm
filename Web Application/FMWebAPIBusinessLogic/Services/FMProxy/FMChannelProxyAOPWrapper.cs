using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.Services.FMProxy
{
    /// <summary>
    /// Will call wcf method using the current FMChannel implementation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FMChannelProxyAOPWrapper<T> : RealProxy where T : class
    {
        public FMChannelProxyAOPWrapper() 
            : base(typeof(T)) 
        {
        }

        public override IMessage Invoke(IMessage msg)
        {
            var methodCall = msg as IMethodCallMessage;
            var methodInfo = methodCall.MethodBase as MethodInfo;
            var channelInfo = FMChannelHelper.GetChannel<T>();
            for (int idx = 0; idx < channelInfo.NumberOfAttemptsConfigured; idx++)
            {
                try
                {
                    // Open the channel
                    ((IClientChannel)channelInfo.Channel).Open();

                    //temporary arguments required for out parameters, does not work when massing methodCall.Args
                    var tempArgs = new Object[methodCall.ArgCount];
                    for (int i = 0; i < methodCall.ArgCount; i++)
                    {
                        tempArgs[i] = methodCall.Args[i];
                    }

                    // call the method or segment of code
                    var result = methodInfo.Invoke(channelInfo.Channel, tempArgs);

                    // close the channel
                    FMChannelHelper.CloseChannel(channelInfo.Channel);

                    return new ReturnMessage(result, tempArgs, tempArgs.Length,
                        methodCall.LogicalCallContext, methodCall);

                }
                catch (Exception error)
                {
                    FMChannelHelper.AbortChannel(channelInfo.Channel);
                    channelInfo = FMChannelHelper.GetChannel<T>();
                    // If the error is a transient error, retry 
                    if ((idx + 1 < channelInfo.NumberOfAttemptsConfigured) &&
                        ((error is EndpointNotFoundException) ||
                        (error is ChannelTerminatedException) ||
                        (error is ServerTooBusyException) ||
                        (error is SyncCommunicationException)))
                    {
                        System.Threading.Thread.Sleep(channelInfo.RetryWaitTime);
                    }
                    else
                    {
                        System.Diagnostics.Trace.TraceError(error.ToString());
                        throw;
                    }
                }
            }
            throw new NotSupportedException("Method call was never attempted");
        }
    }
}


