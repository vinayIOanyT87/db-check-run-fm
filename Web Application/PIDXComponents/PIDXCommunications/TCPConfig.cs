/// <summary>
/// File name:	TCPConfig.cs
/// Purpose:	
///	Comments:	Copyright (C) Varec, Inc. Norcross, GA, USA, 
///				2008.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Varec, Inc.
///	Author(s):	Ivan Orndorff
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:		By:				Reason:
///		----------	-------------	-------------------------------------------
///		23-Jan-08	I.Orndorff		1.0.0 - Initial Revision.
///		
/// </summary>
/// 

using System;
using System.Collections;
using System.Text;
using PIDXTransactions;

namespace PIDXCommunications
{
    class TCPConfig
    {
		#region Private Attributes
		private String hostname;
		private Int32 port;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public TCPConfig()
		{
            this.Initialize();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property sets and gets the host name attribute.
		/// </summary>
		public String HostName
		{
			get { return this.hostname; }
			set { this.hostname = value; }
		}

		/// <summary>
		/// This property sets and gets the port attribute.
		/// </summary>
		public Int32 Port
		{
			get { return this.port; }
			set { this.port = value; }
		}
		#endregion

        #region Private Methods
        /// <summary>
        /// This method validates for the TCPConfig fields.  It throws
        /// an exception if the validation fails.
        /// </summary>
        public void Validate()
        {
            if (this.hostname == null)
            {
                throw new PIDXException(PIDXConstants.ERR_MSG_027); 
            }

            if (this.port == -99)
            {
                throw new PIDXException(PIDXConstants.ERR_MSG_028);
            }
        }
        #endregion Private Methods

        #region Private Methods
        /// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
        private void Initialize()
		{
            this.hostname = null;
            this.port = -99;
		}
		#endregion

        #region Protected methods
        #endregion Protected methods

        #region Overrides
        #endregion Overrides
    }
}
