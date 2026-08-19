namespace FMBusinessObjects.PIDXTransactions
{
    using System;

    using FMBusinessObjects.Constants;
    using FMBusinessObjects.Exceptions;

    class TcpConfig
	{
		#region Private Attributes
		private string hostname;
		private int port;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public TcpConfig ( )
		{
			this.Initialize ( );
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property sets and gets the host name attribute.
		/// </summary>
		public string HostName
		{
			get { return this.hostname; }
			set { this.hostname = value; }
		}

		/// <summary>
		/// This property sets and gets the port attribute.
		/// </summary>
		public int Port
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
		public void Validate ( )
		{
			if (string.IsNullOrEmpty(this.hostname))
			{
				throw new PIDXException ( PIDXConstants.ERR_MSG_027 );
			}

			if (this.port == -99)
			{
				throw new PIDXException ( PIDXConstants.ERR_MSG_028 );
			}
		}
		#endregion Private Methods

		#region Private Methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Initialize ( )
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