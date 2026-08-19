using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace MockEBSService
{
	public class ReceiveThread : BaseThread
	{

		public ReceiveThread()
			: base()
		{

		}



		protected override void ThreadHandler()
		{
			Uri receiveingUri = new Uri(System.Configuration.ConfigurationManager.AppSettings["ReceiveUri"]);

			//using (ServiceHost host = new ServiceHost(typeof(RequestService), receiveingUri))
			//{
			//   // Enable metadata publishing.
			//   ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
			//   smb.HttpGetEnabled = true;
			//   smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
			//   host.Description.Behaviors.Add(smb);

			//   // Open the ServiceHost to start listening for messages. Since
			//   // no endpoints are explicitly configured, the runtime will create
			//   // one endpoint per base address for each service contract implemented
			//   // by the service.
			//   host.Open();

			//   Console.WriteLine("The receive service is ready at {0}", receiveingUri);
			//   _stopEvent.WaitOne();

			//   // Close the ServiceHost.
			//   host.Close();

			//}


			ServiceHost host = null;
			try
			{
				host = new ServiceHost(typeof(RequestService), receiveingUri);

				//// Enable metadata publishing.
				//ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
				//smb.HttpGetEnabled = true;
				//smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
				//host.Description.Behaviors.Add(smb);

				// Open the ServiceHost to start listening for messages. Since
				// no endpoints are explicitly configured, the runtime will create
				// one endpoint per base address for each service contract implemented
				// by the service.
				host.Open();

				Console.WriteLine("The receive service is ready at {0}", receiveingUri);
				_stopEvent.WaitOne();

				// Close the ServiceHost.
				host.Close();
			}
			catch (System.Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				if (host != null)
					((IDisposable)host).Dispose();
			}
		}
	}
}