namespace FMBusinessObjects.PIDXTransactions
{
    using System;

    using FMBusinessObjects.Constants;
    using FMBusinessObjects.Exceptions;

    // ReSharper disable once InconsistentNaming
    public class AuthorizationDenyLA : AuthorizationDenyBase
	{
		#region Private attributes
		private string pidxrVersion;
		#endregion


		#region Constructor
		/// <summary>
		/// This is the default constructor for the authorization deny class.
		/// </summary>
		public AuthorizationDenyLA()
		{
			this.Initialize();
		}
		#endregion


		#region Properties
		public string PidxrVersion
		{
			get { return this.pidxrVersion; }
			set { this.pidxrVersion = value; }
		}
		#endregion

		#region Public override methods
		/// <summary>
		/// This method parses the deny response.
		/// </summary>
		/// <param name="response"></param>
		public override void Parse(string response)
		{
			if ((response != null) && (response.IndexOf("E!", StringComparison.Ordinal) >= 0))
			{
				throw new PIDXException(PIDXConstants.ERR_MSG_025, PIDXException.ErrorTypes.WARNING);
			}

			if ((response != null) && (response.Length >= 14))
			{
				this.ResponseType = response.Substring(0, 4);
				this.pidxrVersion = response.Substring(4, 4);
				this.SellerID = response.Substring(8, 3);
				this.DenyReasonCode = response.Substring(11, 3);
				this.DenyReason = (string) this.DenyReasonList[this.DenyReasonCode];

				int termIndex = response.IndexOf("R?", StringComparison.Ordinal);

				if (termIndex >= 0)
				{
					this.TerminatingString = response.Substring(termIndex);
				}
			}
			else
			{
				throw new PIDXException(PIDXConstants.ERR_MSG_024);
			}
		}
		#endregion




		#region Private methods
		/// <summary>
		/// This method initializes the object to its intial state.
		/// </summary>
		private void Initialize()
		{
			this.pidxrVersion = "";
		}
		#endregion

	}
}
