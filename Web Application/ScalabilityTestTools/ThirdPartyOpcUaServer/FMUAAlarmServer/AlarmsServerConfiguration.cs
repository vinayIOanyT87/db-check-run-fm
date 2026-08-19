/* ========================================================================
 * Copyright © 2011-2014 Softing Industrial Automation GmbH. 
 * All rights reserved.
 * 
 * The Software is subject to the Softing Industrial Automation GmbH’s 
 * license agreement, which can be found here:
 * http://www.softing.com/LicenseSIA.pdf
 * 
 * The Software is based on the OPC Foundation, Inc.’s software. This 
 * original OPC Foundation’s software can be found here:
 * http://www.opcfoundation.org
 * 
 * The original OPC Foundation’s software is subject to the OPC Foundation
 * MIT License 1.00, which can be found here:
 * http://opcfoundation.org/License/MIT/1.00/
 * 
 * ======================================================================*/

using System;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Collections.Generic;
using Softing.Opc.Ua.Sdk.Server;

namespace FMUAAlarmServer
{
    /// <summary>
    /// Stores the configuration of the node manager.
    /// </summary>
    [DataContract(Namespace = Namespaces.Alarms)]
    public class AlarmsServerConfiguration
    {
        #region Constructors
        /// <summary>
        /// The default constructor.
        /// </summary>
        public AlarmsServerConfiguration()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes the object during deserialization.
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
        /// The configuration parameter 1
        /// </summary>
        [DataMember(Order = 1)]
        public int ConfigParam1
        {
            get { return m_configParam1; }
            set { m_configParam1 = value; }
        }
        #endregion

        #region Private Members
        private int m_configParam1;
        #endregion
    }
}
