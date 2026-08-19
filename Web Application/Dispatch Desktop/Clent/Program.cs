namespace Dispatch
{
	using System;
	using System.ComponentModel;
	using System.Configuration;
	using System.Reflection;
	using System.Runtime.InteropServices;
	using System.ServiceModel;
	using System.Threading;
	using System.Windows.Forms;

	using Dispatch;
	using FMBusinessObjects.ChannelFactories;
	using FMDispatchBusinessObjects.BusinessInterfaces;
	using FMDispatchBusinessObjects.ChannelFactories;


	//using FMBusinessObjects.BusinessInterfaces;
	//using FMBusinessObjects.ChannelFactories;

	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		public static void Main()
		{
			try
			{
				// get application GUID as defined in AssemblyInfo.cs
				string appGuid = ((GuidAttribute) 
									Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), false).GetValue(0)).Value;

				// unique id for global mutex - Global prefix means it is global to the machine
				string mutexId = string.Format("Global\\{{{0}}}", appGuid);

				using (var mutex = new Mutex(false, mutexId))
				{
					var hasHandle = false;

					try
					{
						try
						{
							hasHandle = mutex.WaitOne(0, false);

							if (hasHandle == false)
							{
								throw new TimeoutException("Timeout waiting for exclusive access");
							}
						}
						catch (AbandonedMutexException)
						{
							// Log the fact the mutex was abandoned in another process, it will still get aquired
							hasHandle = true;
						}

						// Remove "EXE" from default configuration file name per request of Varec Configuration Management.
						AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", "dispatch.config");

						Application.EnableVisualStyles();
						Application.SetCompatibleTextRenderingDefault(false);
						Application.Run(new WarningBannerForm());

						if (CheckFMBusinessServicesAddress())
						{
							Application.Run(new LoginForm());
						}
					}
					catch (TimeoutException)
					{
						// If there is more than one, it is already running. 
						MessageBox.Show("Application is already running. Only one instance of Dispatch is allowed at a time.");
					}
					finally
					{
						if (hasHandle)
						{
							mutex.ReleaseMutex();
						}
					}
				}
			}
			catch (Exception except)
			{
				MessageBox.Show(except.Message, "Dispatch");
			}
		}

		/// <summary>
		/// Checks the configured address for FMBusinessServices and prompts the user for entry if not valid.
		/// </summary>
		/// <returns>False if application should exit.</returns>
		private static bool CheckFMBusinessServicesAddress()
		{
			// Check the communication binding to see if we need to prompt.  We need to prompt if it is empty or
			// if we find no endpoint listening at the configured address.
			var promptForAddress = false;
			var endPointAddress = ConfigurationManager.AppSettings["dispatchEndPointAddress"];

			if (string.IsNullOrEmpty(endPointAddress))
			{
				promptForAddress = true;
			}
			else
			{
				var busy = new DelayedDisplayBusyDialog();

				// Try to contact the address and see if it is working
				try
				{
					using (var background = new BackgroundWorker())
					{
						background.WorkerReportsProgress = false;
						background.WorkerSupportsCancellation = true;
						background.DoWork += (sender, args) =>
							{
								Thread.Sleep(3000);
								Application.Run(busy);
							};

						background.RunWorkerAsync();

						FMChannelHelper.MakeCall<IClientDispatchService>(x=> x.IsDefenseKey());
					}
				}
				catch (EndpointNotFoundException)
				{
					// We need to prompt because the endpoint was not found when we tried to contact it.
					promptForAddress = true;
				}
				finally
				{
					busy.Close();
				}
			}

			if (promptForAddress)
			{
				// Open up an address prompting location.
				var dialog = new FMBAddressConfiguration();
				Application.Run(dialog);

				if (dialog.DialogResult == DialogResult.Cancel)
				{
					return false;
				}

				return CheckFMBusinessServicesAddress();
			}

			return true;
		}
	}
}
