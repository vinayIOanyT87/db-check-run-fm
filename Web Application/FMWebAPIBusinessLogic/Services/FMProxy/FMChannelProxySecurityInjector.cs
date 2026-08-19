using FMBusinessObjects.DataObjects;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.Services.FMProxy
{
    public class FMSecurityClassInjector<T, U>: RealProxy
    {
        private readonly U _decorated;
        private readonly IProxySecurityFactory _securityFactory;
        public FMSecurityClassInjector(U decorated, IProxySecurityFactory securityFactory)
          : base(typeof(T))
        {
            _decorated = decorated;
            _securityFactory = securityFactory;
        }

        public override IMessage Invoke(IMessage msg)
        {
            var methodCall = msg as IMethodCallMessage;
            var methodInfo = methodCall.MethodBase as MethodInfo;
            bool needToOverrideSecurityParameter = false;

            //lets find a matching method via name and parameter count
            var matchedMethods = typeof(U).GetMethods()
                .Where(x => x.Name == methodInfo.Name);
            MethodInfo matchedMethod = null;
            foreach (var method in matchedMethods)
            {
                var properties = method.GetParameters();
                //the first parameter HAS to be security class and calling method has to match paramater count
                if (properties.FirstOrDefault()?.ParameterType == typeof(SecurityClass) &&
                    properties.Count() == (methodCall.ArgCount + 1))
                {
                    needToOverrideSecurityParameter = true;
                    matchedMethod = method;
                }
            }

            if (!needToOverrideSecurityParameter)
            {
                foreach (var method in matchedMethods)
                {
                    var properties = method.GetParameters();
                    //match on property count
                    if (properties.Count() == methodCall.ArgCount)
                    {
                        matchedMethod = method;
                    }
                }
            }

            if (matchedMethod == null)
            {
                throw new EntryPointNotFoundException("No matched method found");
            }

            object[] tempArgs;
            if (needToOverrideSecurityParameter)
            {
                //the first parameter is security, lets add it to the arguments
                tempArgs = new object[methodCall.ArgCount + 1];
                tempArgs[0] = _securityFactory.GetSecurity();
                for (int i = 1; i < methodCall.ArgCount; i++)
                {
                    tempArgs[i] = methodCall.Args[i];
                }
            }
            else
            {
                //pass everything thru
                tempArgs = new object[methodCall.ArgCount];
                for (int i = 0; i < methodCall.ArgCount; i++)
                {
                    tempArgs[i] = methodCall.Args[i];
                }
            }

            var result = matchedMethod.Invoke(_decorated, tempArgs);
            return new ReturnMessage(result, tempArgs, tempArgs.Length,
              methodCall.LogicalCallContext, methodCall);
        }
    }
}
