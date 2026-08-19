namespace FuelsManager.Afss.Module.Gasboy.OrCU.Communications
{
    using System;
    using System.Threading;

    using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;
    using FuelsManager.Afss.Module.Gasboy.OrCU.GasboyBOS;

    internal class GasboySession
    {
        private long IsOwned = 0;

        private readonly GasboyStation TargetStation = null;

        public string SessionID { get; set; }
        public int SiteCode { get; set; }
        
        public SiteOmatClassSoap Service { get; set; }

        public Guid StationGuid
        {
            get
            {
                return ((null != this.TargetStation) ? this.TargetStation.IdentityGuid : Guid.Empty);
            }
        }

        public bool ClaimOwnership()
        {
            return Interlocked.Exchange(ref this.IsOwned, 1) == 0;
        }

        public bool ReleaseOwnership()
        {
            return Interlocked.Exchange(ref this.IsOwned, 0) == 1;
        }

        public GasboySession(GasboyStation targetStation)
        {
            this.TargetStation = targetStation;
        }
    }
}
