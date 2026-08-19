


namespace FMUAAlarmServer
{
    using System;
    using System.Threading;
    using Softing.Opc.Ua.Sdk;
    using Softing.Opc.Ua.Sdk.Client;
    using Softing.Opc.Ua.Sdk.Configuration;
    using InProcLogging;

    public class AddDelAlarmsCli : IDisposable
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the DAClient class.
        /// </summary>
        public AddDelAlarmsCli(string serverUrl)
        {
            //this.setToSingleApartment();
            //mailBox = new Mailbox(100, 1000);
           // mailBox.registerMailbox(MailBoxName);
            ServerEndPoint = serverUrl;
            ApplicationInstance application = new ApplicationInstance();
            application.ApplicationType = ApplicationType.Client;
            application.ConfigSectionName = "NodeManagementClient";

            Softing.Opc.Ua.Toolkit.Application.ActivateLicense(Softing.Opc.Ua.Toolkit.LicenseFeature.Client, "0fa0-00d8-b0b4-a329-439d");

            while (true)
            {
                try
                {
                    // load the application configuration.
                    application.LoadApplicationConfiguration(Environment.CurrentDirectory + @"\NodeManagementClient.Config.xml", true);
                    break;
                }
                catch(Exception e)
                {
                    System.Console.WriteLine("AddDelAlarmsCli.AddDelAlarmsCli Exception: " + e.Message);
                    Logger.LogError("AddDelAlarmsCli.AddDelAlarmsCli Exception: " + e.Message);
                    Thread.Sleep(1000);
                }
            }

            // check the application certificate.
            application.CheckApplicationInstanceCertificate(false, 0);

            m_configuration = application.ApplicationConfiguration;
            m_configuration.CertificateValidator.CertificateValidation += new CertificateValidationEventHandler(CertificateValidator_CertificateValidation);

            this.Connect();
          
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the session.
        /// </summary>
        public Session Session
        {
            get
            {
                return m_session;
            }
        }

        /// <summary>
        /// Gets the server URL.
        /// </summary>
        public string ServerUrl
        {
            get
            {
                return ServerEndPoint;
            }
        }

        #endregion

        #region Public Methods

        //public void Process()
        //{
        //    PointModCalcs alarmsToAdd;
        //    PointModCalcs alarmsToDel;
        //    SharedObject.Instance().GetAlarms(out alarmsToAdd, out alarmsToDel);
        //    foreach (var addAlarm in alarmsToAdd)
        //    {
        //        this.AddNodes(addAlarm);
        //    }
        //    foreach (var delAlarm in alarmsToDel)
        //    {
        //        this.DeleteNodes(delAlarm);
        //    }
        //}

        /// <summary>
        /// Creates a communication session with the server
        /// </summary>
        public bool Connect()
        {
            try
            {
                if (m_session != null && m_session.Connected == true)
                {
                    Console.WriteLine("Session already connected!");
                    return true;
                }
                else
                {
                    Console.WriteLine("Connecting...");

                    // Get the endpoint by connecting to server's discovery endpoint
                    EndpointDescription endpointDescription = SelectEndpoint(ServerEndPoint);

                    EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(m_configuration);
                    ConfiguredEndpoint endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);
                    
                    //// Create the session
                    Session session = Session.Create(
                        m_configuration,
                        endpoint,
                        false,
                        false,
                        m_configuration.ApplicationName,
                        30 * 60 * 1000,
                        new UserIdentity("user", "password"),
                        null);

                    // Assign the created session
                    if (session != null && session.Connected)
                    {
                        m_session = session;
                    }

                    // Log Session Created event
                    string logMessage = String.Format("New Session Created with SessionName = {0}.", m_session.SessionName);
                    Softing.Opc.Ua.Sdk.Trace.Instance.Log(TraceLevels.Information, TraceMasks.User1, "DAClient", logMessage);

                    Console.WriteLine(logMessage);
                    return true;
                }                
            }
            catch (Exception exception)
            {
                // Log Error
                string logMessage = String.Format("Create Session Error : {0}.", exception.Message);
                Softing.Opc.Ua.Sdk.Trace.Instance.Log(TraceLevels.Error, TraceMasks.User1, "DAClient", logMessage);
                Console.WriteLine(logMessage);
            }
            return false;
        }

        // Flag: Has Dispose already been called?
        bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
            }

            try
            {
                if (m_session != null)
                {
                    Console.WriteLine("Disconnecting...");

                    m_session.Close();
                    m_session = null;
                }
            }
            catch (Exception)
            {
            }
            disposed = true;
        }

        ~AddDelAlarmsCli()
        {
            Dispose(false);
        }


        /// <summary>
        /// Closes the session
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (m_session != null)
                {
                    Console.WriteLine("Disconnecting...");

                    m_session.Close();
                    m_session = null;

                    // Log Session Disconnected event
                    string logMessage = String.Format("Session Disconnected.");
                    Softing.Opc.Ua.Sdk.Trace.Instance.Log(TraceLevels.Information, TraceMasks.User1, "DAClient", logMessage);
                    Console.WriteLine(logMessage);
                }
                else
                {
                    Console.WriteLine("Session already disconnected!");
                }
            }
            catch (Exception exception)
            {
                // Log Error
                string logMessage = String.Format("Disconnect Error : {0}.", exception.Message);
                Softing.Opc.Ua.Sdk.Trace.Instance.Log(TraceLevels.Error, TraceMasks.User1, "DAClient", logMessage);
                Console.WriteLine(logMessage);
            }
        }

        public void AddNodes(AddNodeClass node)
        {
            AddNodes(node.ParentNodeID,node.NodeName,node.NodeXML);
        }

        /// <summary>
        /// Invokes the AddNodes service.
        /// </summary>
        public void AddNodes(string parentNodeID, string nodeName, string nodeXml)
        {        
            while (this.Connect() == false)
            {
                Thread.Sleep(1000);
            }
            try
            {
                if (m_session != null && m_session.Connected == true)
                {
                    // create service request data
                    AddNodesItemCollection nodesToAdd = new AddNodesItemCollection();

                    // Create an ServerStatus node.
                    AddNodesItem node4 = new AddNodesItem();
                    node4.ParentNodeId = new NodeId(parentNodeID); //m_parentNodeId);
                    node4.ReferenceTypeId = ReferenceTypes.HasComponent;
                    node4.RequestedNewNodeId = null;
                    node4.BrowseName = new QualifiedName(nodeName);
                    node4.NodeClass = NodeClass.Variable;
                    node4.NodeAttributes = null;
                    node4.TypeDefinition = VariableTypeIds.ServerStatusType;
                    
                    //specify node attributes.
                    VariableAttributes node4Attribtues = new VariableAttributes();
                    node4Attribtues.DisplayName = nodeName;
                    node4Attribtues.Description = nodeName + " Description";
                    node4Attribtues.Value = new Variant(nodeXml);
                    node4Attribtues.DataType = (uint)BuiltInType.Double;
                    node4Attribtues.ValueRank = ValueRanks.Scalar;
                    node4Attribtues.ArrayDimensions = new UInt32Collection();
                    node4Attribtues.AccessLevel = AccessLevels.CurrentReadOrWrite;
                    node4Attribtues.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
                    node4Attribtues.MinimumSamplingInterval = 0;
                    node4Attribtues.Historizing = false;
                    node4Attribtues.WriteMask = (uint)AttributeWriteMask.None;
                    node4Attribtues.UserWriteMask = (uint)AttributeWriteMask.None;
                    node4Attribtues.SpecifiedAttributes = (uint)NodeAttributesMask.All;

                    node4.NodeAttributes = new ExtensionObject(node4Attribtues);
                    nodesToAdd.Add(node4);
                    
                    AddNodesResultCollection results = null;
                    DiagnosticInfoCollection diagnosticInfos;

                    RequestHeader requestHeader = new RequestHeader();
                    requestHeader.ReturnDiagnostics = (uint)DiagnosticsMasks.All;

                    Console.WriteLine(String.Format("Sending AddNodes request:"));

                    for (int ii = 0; ii < nodesToAdd.Count; ii++)
                    {
                        string nodeToadd = nodesToAdd[ii].RequestedNewNodeId == null ? "null" : nodesToAdd[ii].RequestedNewNodeId.ToString();
                        Console.WriteLine(String.Format("\tNode[{0}]: NodeId = {1}, BrowseName = {2}", ii, nodeToadd, nodesToAdd[ii].BrowseName));
                    }

                    m_session.AddNodes(
                        requestHeader,
                        nodesToAdd,
                        out results,
                        out diagnosticInfos);
                    
                    // Log service call
                    string logMessage = String.Format("AddNodes operation completed.");
                    Softing.Opc.Ua.Sdk.Trace.Instance.Log(TraceLevels.Information, TraceMasks.User1, "DAClient", logMessage);
                    Console.WriteLine(logMessage);
                    Console.WriteLine("Operation results: ");

                    for (int ii = 0; ii < results.Count; ii++)
                    {
                        Console.WriteLine(String.Format("\tNode[{0}]: StatusCode = {1}, AddedNodeId = {2}", ii, results[ii].StatusCode, results[ii].AddedNodeId));
                    }
                }
                else
                {
                    Console.WriteLine("Session not connected!");
                }
            }
            catch (Exception exception)
            {
                // Log Error
                string logMessage = String.Format("AddNodes Error : {0}.", exception.Message);
                Softing.Opc.Ua.Sdk.Trace.Instance.Log(TraceLevels.Error, TraceMasks.User1, "DAClient", logMessage);
                Console.WriteLine(logMessage);
            }
        }

        /// <summary>
        /// Invokes the DeleteNodes service.
        /// </summary>
        public void DeleteNodes(string nodeID)
        {
            try
            {
                if (m_session != null && m_session.Connected == true)
                {
                    // create service request data
                    DeleteNodesItemCollection nodesToDelete = new DeleteNodesItemCollection();

                    DeleteNodesItem node1 = new DeleteNodesItem();
                    node1.NodeId = new NodeId(nodeID);
                    node1.DeleteTargetReferences = false;

                    nodesToDelete.Add(node1);

                    StatusCodeCollection results = null;
                    DiagnosticInfoCollection diagnosticInfos;

                    RequestHeader requestHeader = new RequestHeader();
                    requestHeader.ReturnDiagnostics = (uint)DiagnosticsMasks.All;

                    Console.WriteLine(String.Format("Sending DeleteNodes request:"));

                    for (int ii = 0; ii < nodesToDelete.Count; ii++)
                    {
                        Console.WriteLine(String.Format("\tNode[{0}]: NodeId = {1}, DeleteTargetReferences = {2}", ii, nodesToDelete[ii].NodeId, nodesToDelete[ii].DeleteTargetReferences));
                    }

                    m_session.DeleteNodes(
                        requestHeader,
                        nodesToDelete,
                        out results,
                        out diagnosticInfos);

                    // Log service call
                    string logMessage = String.Format("DeleteNodes operation completed.");
                    Softing.Opc.Ua.Sdk.Trace.Instance.Log(TraceLevels.Information, TraceMasks.User1, "DAClient", logMessage);
                    Console.WriteLine(logMessage);

                    Console.WriteLine("Operation results: ");

                    for (int ii = 0; ii < results.Count; ii++)
                    {
                        Console.WriteLine(String.Format("\tNode[{0}]: StatusCode = {1}", ii, results[ii]));
                    }
                }
                else
                {
                    Console.WriteLine("Session not connected!");
                }
            }
            catch (Exception exception)
            {
                // Log Error
                string logMessage = String.Format("DeleteNodes Error : {0}.", exception.Message);
                Softing.Opc.Ua.Sdk.Trace.Instance.Log(TraceLevels.Error, TraceMasks.User1, "DAClient", logMessage);
                Console.WriteLine(logMessage);
            }
        }

        
        #endregion

        #region Private Methods

        /// <summary>
        /// Finds the endpoint that best matches the current settings.
        /// </summary>
        /// <param name="discoveryUrl">The discovery URL.</param>
        /// <param name="endpointIdx">The index of the endpoint to return from the list of server's endpoints.</param>
        /// <returns>The best available endpoint.</returns>
        private EndpointDescription SelectEndpoint(string discoveryUrl)
        {
            // needs to add the '/discovery' back onto non-UA TCP URLs.
            if (!discoveryUrl.StartsWith(Utils.UriSchemeOpcTcp))
            {
                if (!discoveryUrl.EndsWith("/discovery"))
                {
                    discoveryUrl += "/discovery";
                }
            }

            // parse the selected URL.
            Uri uri = new Uri(discoveryUrl);

            EndpointDescription selectedEndpoint = null;

            // Connect to the server's discovery endpoint and find the available configuration.
            using (DiscoveryClient client = DiscoveryClient.Create(uri))
            {
                EndpointDescriptionCollection endpoints = client.GetEndpoints(null);

                // select the endpoint without security
                for (int ii = 0; ii < endpoints.Count; ii++)
                {
                    EndpointDescription endpoint = endpoints[ii];

                    if (endpoint.SecurityMode == MessageSecurityMode.None)                    
                    {
                        selectedEndpoint = endpoint;
                        break;
                    }                    
                }                

                // pick the first available endpoint by default.
                if (selectedEndpoint == null && endpoints.Count > 0)
                {
                    selectedEndpoint = endpoints[0];
                }
            }
            
            // if a server is behind a firewall it may return URLs that are not accessible to the client.
            // This problem can be avoided by assuming that the domain in the URL used to call 
            // GetEndpoints can be used to access any of the endpoints. This code makes that conversion.
            // Note that the conversion only makes sense if discovery uses the same protocol as the endpoint.

            Uri endpointUrl = Utils.ParseUri(selectedEndpoint.EndpointUrl);

            if (endpointUrl != null && endpointUrl.Scheme == uri.Scheme)
            {
                UriBuilder builder = new UriBuilder(endpointUrl);
                builder.Host = uri.DnsSafeHost;
                builder.Port = uri.Port;
                selectedEndpoint.EndpointUrl = builder.ToString();
            }

            // return the selected endpoint.
            return selectedEndpoint;
        }

        /// <summary>
        /// Validate untrusted certificates
        /// </summary>
        private void CertificateValidator_CertificateValidation(CertificateValidator sender, CertificateValidationEventArgs e)
        {
            bool certificateAccepted = true;

            // ****
            // Implement a custom logic to decide if the certificate should be accepted or not and set the certificateAccepted accordingly.
            // The certificate can be retreived from the e.Certificate field
            // ***
            e.Accept = certificateAccepted;
        }

        #endregion

        #region Private Fields

        private ApplicationConfiguration m_configuration;
		private string ServerEndPoint = "http://18vj8v1:62549/AlarmsServer/None";

        private Session m_session;
        #endregion
    }
}
