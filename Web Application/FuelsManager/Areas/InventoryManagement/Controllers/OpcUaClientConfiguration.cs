namespace FuelsManager.Areas.Controllers
{
    using System.Runtime.Serialization;
    /// <summary>
    /// Stores custom configuration parameters
    /// </summary>
    [DataContract(Namespace = Opc.Ua.Namespaces.OpcUaConfig)]
    public class OpcUaClientConfiguration
    {
        #region Private Members
        private uint m_timerInterval;
        private uint m_clearCachedCertificatesInterval; 
        #endregion

        #region Constructors
        /// <summary>
        /// The default constructor
        /// </summary>
        public OpcUaClientConfiguration()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes the object during de-serialization
        /// </summary>
        [OnDeserializing()]
        private void Initialize(StreamingContext context)
        {
            Initialize();
        }

        /// <summary>
        /// Sets private members to default values.
        /// </summary>
        private void Initialize()
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The delay in seconds to allow a graceful shutdown
        /// </summary>
        [DataMember(Order = 1)]
        public uint TimerInterval
        {
            get { return m_timerInterval; }
            set { m_timerInterval = value; }
        }

        /// <summary>
        /// The interval to clear the list of cached trusted certificates
        /// </summary>
        [DataMember(Order = 2)]
        public uint ClearCachedCertificatesInterval
        {
            get { return m_clearCachedCertificatesInterval; }
            set { m_clearCachedCertificatesInterval = value; }
        }
        #endregion
    }
}