using System;
using System.Messaging;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Microsoft.Win32;
using IBM.WMQ;

namespace MessageQueueInjector
{
	public partial class frmMain : Form {

#region [ Constants ]

		private const string REGISTRY_SOFTWARE = "Software";
		private const string REGISTRY_COMPANY = "Endress + Hauser";
		private const string REGISTRY_APPLICATION = "MessageQueueInjector";
		private const string REGISTRY_KEY = REGISTRY_SOFTWARE + "\\" + REGISTRY_COMPANY + "\\" + REGISTRY_APPLICATION;
		private const string REG_QUEUE_PATH = "MSMQ-QueuePath";
		private const string REG_TRANSACTIONAL = "MSMQ-Transactional";
		private const string REG_QUEUE_MANAGER = "WebSphere-QueueManager";
		private const string REG_HOST_NAME = "WebSphere-HostName";
		private const string PORT_NUMBER = "WebSphere-PortNumber";
		private const string REG_CHANNEL = "WebSphere-Channel";
		private const string REG_QUEUE_NAME = "WebSphere-QueueName";

#endregion

#region [ Auto-Generated Code ]

		public frmMain() {
			InitializeComponent();
		}

#endregion

#region [ Event Handlers ]

		private void frmMain_Load(object sender, EventArgs e) {
			try {
				LoadSavedValues();
			}
			catch {
				// Do nothing since this is only loading the UI values.
			}
		}

		private void frmMain_Close(object sender, EventArgs e) {
			try {
				SaveCurrentValues();
			}
			catch {
				// Do nothing since this is only saving the UI values.
			}
		}

		private void btnSendMSMQ_Click(object sender, EventArgs e) {
			try {
				bool bolQueueFound, bolIsValidInput = true;
				StringBuilder objBuilder = new StringBuilder();

				// Validate user input
				if (string.IsNullOrEmpty(txtMessage.Text)) {
					bolIsValidInput = false;
					objBuilder.AppendLine("You must enter a message to send.");
				}
				if (string.IsNullOrEmpty(txtQueuePath.Text.Trim())) {
					bolIsValidInput = false;
					objBuilder.AppendLine("You must enter a queue path to connect to.");
				}
				// If input is valid, send the message.
				if (bolIsValidInput) {
					this.Cursor = Cursors.WaitCursor;
					bolQueueFound = SendMessageToMSMQ(txtQueuePath.Text.Trim(), chkTransactional.Checked);
					this.Cursor = Cursors.Default;
					if (bolQueueFound)
						MessageBox.Show(this, "The message has been successfully sent to the queue.", "Message Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				else
					MessageBox.Show(this, "The message cannot be sent for the following reasons:\n\n" + objBuilder.ToString(), "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			catch (Exception objEx) {
				this.Cursor = Cursors.Default;
				MessageBox.Show(this, "The following error occcurred while sending the message:\n\n" + objEx.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnSendWebSphere_Click(object sender, EventArgs e) {
			try {
				bool bolIsValidInput = true;
				StringBuilder objBuilder = new StringBuilder();
				int? intPortNumber = null;

				// Validate user input
				if (string.IsNullOrEmpty(txtMessage.Text)) {
					bolIsValidInput = false;
					objBuilder.AppendLine("You must enter a message to send.");
				}
				if (string.IsNullOrEmpty(txtQueueManager.Text.Trim())) {
					bolIsValidInput = false;
					objBuilder.AppendLine("You must enter a queue manager to connect to.");
				}
				if (string.IsNullOrEmpty(txtHostName.Text.Trim())) {
					bolIsValidInput = false;
					objBuilder.AppendLine("You must enter a host to connect to.");
				}
				if (!string.IsNullOrEmpty(txtPort.Text.Trim())) {
					int intPort;
					bool bolIsInt = int.TryParse(txtPort.Text.Trim(), out intPort);
					if (bolIsInt) {
						if (intPort <= 0 || intPort > 65535) {
							bolIsValidInput = false;
							objBuilder.AppendLine("The port number must be a valid TCP/IP port number. (1 - 65535)");
						}
						else
							intPortNumber = intPort;
					}
					else {
						bolIsValidInput = false;
						objBuilder.AppendLine("The port number must be a valid number.");
					}
				}
				if (string.IsNullOrEmpty(txtChannel.Text.Trim())) {
					bolIsValidInput = false;
					objBuilder.AppendLine("You must enter a channel to send through.");
				}
				if (string.IsNullOrEmpty(txtQueueName.Text.Trim())) {
					bolIsValidInput = false;
					objBuilder.AppendLine("You must enter a destination queue name for the message.");
				}
				// If input is valid, send the message.
				if (bolIsValidInput) {
					this.Cursor = Cursors.WaitCursor;
					SendMessageToWebSphereMQ(txtQueueManager.Text.Trim(), txtHostName.Text.Trim(), intPortNumber, txtChannel.Text.Trim(), txtQueueName.Text.Trim());
					this.Cursor = Cursors.Default;
					MessageBox.Show(this, "The message has been successfully sent to the queue.", "Message Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				else
					MessageBox.Show(this, "The message cannot be sent for the following reasons:\n\n" + objBuilder.ToString(), "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			catch (Exception objEx) {
				this.Cursor = Cursors.Default;
				MessageBox.Show(this, "The following error occcurred while sending the message:\n\n" + objEx.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

#endregion

#region [ Private Methods ]

		/// <summary>
		/// If the registry key for this application exists, this function will load the values from the registry and populate the textbox controls used to configure the application.
		/// </summary>
		private void LoadSavedValues() {
			using (RegistryKey objHive = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64)) {
				using (RegistryKey objKey = objHive.OpenSubKey(REGISTRY_KEY)) {
					txtQueuePath.Text = objKey.GetValue(REG_QUEUE_PATH).ToString();
					chkTransactional.Checked = Convert.ToBoolean(objKey.GetValue(REG_TRANSACTIONAL));
					txtQueueManager.Text = objKey.GetValue(REG_QUEUE_MANAGER).ToString();
					txtHostName.Text = objKey.GetValue(REG_HOST_NAME).ToString();
					txtPort.Text = objKey.GetValue(PORT_NUMBER).ToString();
					txtChannel.Text = objKey.GetValue(REG_CHANNEL).ToString();
					txtQueueName.Text = objKey.GetValue(REG_QUEUE_NAME).ToString();
				}
			}
		}

		/// <summary>
		/// Creates the registry key for this application, if it doesn't exist. Then saves the values currently shown in the application UI.
		/// </summary>
		private void SaveCurrentValues() {
			using (RegistryKey objHive = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64)) {
				using (RegistryKey objSoftwareKey = objHive.OpenSubKey("Software", true)) {
					string[] strNames = objSoftwareKey.GetSubKeyNames();
					if (Array.Find(strNames, strName => strName == "Endress + Hauser") == null)
						objSoftwareKey.CreateSubKey("Endress + Hauser");
					using (RegistryKey objCompanyKey = objSoftwareKey.OpenSubKey("Endress + Hauser", true)) {
						using (RegistryKey objKey = objCompanyKey.CreateSubKey("MessageQueueInjector", RegistryKeyPermissionCheck.ReadWriteSubTree)) {
							objKey.SetValue(REG_QUEUE_PATH, txtQueuePath.Text.Trim());
							objKey.SetValue(REG_TRANSACTIONAL, chkTransactional.Checked.ToString());
							objKey.SetValue(REG_QUEUE_MANAGER, txtQueueManager.Text.Trim());
							objKey.SetValue(REG_HOST_NAME, txtHostName.Text.Trim());
							objKey.SetValue(PORT_NUMBER, txtPort.Text.Trim());
							objKey.SetValue(REG_CHANNEL, txtChannel.Text.Trim());
							objKey.SetValue(REG_QUEUE_NAME, txtQueueName.Text.Trim());
						}
					}
				}
			}
		}

		/// <summary>
		/// Sends a string message to a Microsoft message queue.
		/// </summary>
		/// <param name="QueuePath">The queue path in the form of .\Private$\QueueName</param>
		/// <param name="IsTransactional">Indicates if the queue is transactional or not.</param>
		/// <returns>Returns a boolean indicating whether or not the application was able to connect to the queue.</returns>
		private bool SendMessageToMSMQ(string QueuePath, bool IsTransactional) {
			MessageQueue objQueue = null;
			MessageQueueTransaction objTrans = null;
			bool bolQueueReady;

			// Bind to the message queue.
			bolQueueReady = false;
			if (MessageQueue.Exists(QueuePath)) {
				objQueue = new MessageQueue(QueuePath);
				bolQueueReady = true;
			}
			if (bolQueueReady) {

				// This queue is transactional; create and start a transaction.
				if (IsTransactional) {
					objTrans = new MessageQueueTransaction();
					objTrans.Begin();
				}

				// Write the message to the queue.
				if (IsTransactional) {
					objQueue.Send(txtMessage.Text, objTrans);
					objTrans.Commit();
				}
				else
					objQueue.Send(txtMessage.Text);

				// Close everything and dispose of unmanaged resources.
				//objTrans.Dispose();
				objQueue.Close();
				objQueue.Dispose();
			}
			else
				MessageBox.Show(this, "Could not find the message queue entered.", "Queue Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return bolQueueReady;
		}

		/// <summary>
		/// Sends a string message to an IBM WebSphere message queue.
		/// </summary>
		/// <param name="QueueManager">The name of the queue manager in IBM WebSphere.</param>
		/// <param name="HostName">The host name or IP address of the server hosting the queue.</param>
		/// <param name="ChannelName">The name of the channel to connect to.</param>
		/// <returns>Returns a boolean indicating whether or not the application was able to connect to the queue.</returns>
		private void SendMessageToWebSphereMQ(string QueueManager, string HostName, int? PortNumber, string ChannelName, string QueueName) {
			MQQueueManager objManager;
			MQQueue objQueue;
			Hashtable objConnectionProperties = new Hashtable();
			int intOpenOptions = MQC.MQOO_INPUT_AS_Q_DEF | MQC.MQOO_OUTPUT;

			// Setup the connection properties.
			objConnectionProperties.Add(MQC.TRANSPORT_PROPERTY, MQC.TRANSPORT_MQSERIES_BINDINGS);
			objConnectionProperties.Add(MQC.HOST_NAME_PROPERTY, HostName);
			objConnectionProperties.Add(MQC.CHANNEL_PROPERTY, ChannelName);
			if (PortNumber != null)
				objConnectionProperties.Add(MQC.PORT_PROPERTY, PortNumber.Value);

			// Create a connection to the queue manager using the connection
			objManager = new MQQueueManager(QueueManager, objConnectionProperties);
			objQueue = objManager.AccessQueue(QueueName, intOpenOptions);

			// Define a WebSphere MQ message, writing some text in UTF format and send the message.
			MQMessage objMessage = new MQMessage();
			objMessage.WriteUTF(txtMessage.Text);
			objQueue.Put(objMessage, new MQPutMessageOptions());

			// Close everything and dispose of unmanaged resources.
			objQueue.Close();
			objManager.Disconnect();
		}

#endregion

	}
}
