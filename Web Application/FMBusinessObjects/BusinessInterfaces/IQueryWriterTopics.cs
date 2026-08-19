using FMBusinessObjects.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]

		public interface IQueryWriterTopics
	{

		[OperationContract]
		QueryWriterTopicCollection Enumerate(SecurityClass security);

		[OperationContract]
		QueryWriterTopic Get(SecurityClass security, string objectType);
	}
}
