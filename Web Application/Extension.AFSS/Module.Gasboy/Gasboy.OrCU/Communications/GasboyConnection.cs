namespace FuelsManager.Afss.Module.Gasboy.OrCU.Communications
{
    using System;
    using System.Threading;

    using FMBusinessObjects.DataObjects;

    using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;
    using FuelsManager.Afss.Module.Gasboy.OrCU.GasboyBOS;

    internal class GasboyConnection : IDisposable
    {
        #region Attributes
        /// <summary>
        /// The is disposed.
        /// </summary>
        private bool isDisposed = false;

        private bool IsOwner = false;

        private GasboySession Session = null;

        private static readonly GasboySessionController SessionController = new GasboySessionController();

        #endregion Attributes

        #region Constructors/Destructors

        public GasboyConnection()
        {
            this.isDisposed = false;
        }

        #endregion Constructors/Destructors

        public GasboySession GetConnection(SecurityClass security, GasboyStation externalStation)
        {
            if (externalStation == null)
            {
                throw new ArgumentNullException("externalStation");
            }

            if (string.IsNullOrEmpty(externalStation.IpAddress))
            {
                throw new Exception("Station IP Address is required.");
            }

            if (!externalStation.SiteCode.HasValue)
            {
                throw new Exception("Station SiteCode is required.");
            }

            this.Session = GasboyConnection.SessionController.GetGasboySession(security, externalStation);

            this.IsOwner = (this.IsOwner) ? this.IsOwner : this.Session.ClaimOwnership();

            return this.Session;
        }

        public void CloseConnection()
        {
            if (this.IsOwner)
            {
                this.Session.ReleaseOwnership();
                GasboyConnection.SessionController.CloseGasboySession(this.Session);
            }
        }

        #region Disposable Pattern Implementation
        /// <summary>
        /// Disposes this Client Sync Provider instance 
        /// </summary>
        /// <param name="disposing">True if explicit finalization, false if through GC</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            try
            {
                if (disposing)
                {
                    this.CloseConnection();
                }

            }
            finally
            {
                this.isDisposed = true;
            }
        }

        /// <summary>
        /// Disposes this Client Sync Provider instance 
        /// </summary>
        public virtual void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion Disposable Pattern Implementation
    }
}
