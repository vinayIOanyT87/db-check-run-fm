using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace MockEBSService
{

	public class ResponseThread : BaseThread
	{


		///// <summary>
		/// Creates an instance of the SendTransactionThread
		/// </summary>
		/// <param name="sleepTime">the amount of time (in seconds) to sleep between sending records</param>
		/// <param name="batchSize">the maximum number of records to send to EBS in a single interation</param>
		/// <param name="exportLogPath">the path to log the idocs sent</param>
		/// <param name="security">security object</param>
		public ResponseThread()
			: base()
		{

		}



		protected override void ThreadHandler()
		{
			Uri responseUri = new Uri(System.Configuration.ConfigurationManager.AppSettings["ResponseUri"]);
			
			ServiceHost host = null; ;
			try
			{

				//host = new ServiceHost(typeof(ResponseService), responseUri);

				host = new ServiceHost(typeof(Pull), responseUri);
		
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

				Console.WriteLine("The response service is ready at {0}", responseUri);
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

