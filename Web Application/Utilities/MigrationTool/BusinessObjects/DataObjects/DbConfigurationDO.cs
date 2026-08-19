namespace MigrationToolBusinessObjects
{
    using System;

    [Serializable]
    public class DbConfigurationDO
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public DbConfigurationDO()
        {
            this.Init();
        }
        #endregion

        #region Properties
        public string SourceDbConnectionSecurity { get; set; }
        public string SourceDbConnectionTimeout { get; set; }
        public string SourceDbConnectionDbServer { get; set; }
        public string SourceDbConnectionDbName { get; set; }
        public string TargetDbConnectionSecurity { get; set; }
        public string TargetDbConnectionTimeout { get; set; }
        public string TargetDbConnectionDbServer { get; set; }
        public string TargetDbConnectionDbName { get; set; }

        public int SourceDbConnectTimeoutInt 
        { 
            get 
            { 
                if(string.IsNullOrEmpty(this.SourceDbConnectionTimeout))
                {
                    return 60;
                }

                int timeout;
                if(int.TryParse(this.SourceDbConnectionTimeout, out timeout) == false)
                {
                    return 60;
                }

                return timeout;
            }
        }

        public int TargetDbConnectTimeoutInt
        {
            get
            {
                if (string.IsNullOrEmpty(this.TargetDbConnectionTimeout))
                {
                    return 60;
                }

                int timeout;
                if (int.TryParse(this.TargetDbConnectionTimeout, out timeout) == false)
                {
                    return 60;
                }

                return timeout;
            }
        }

        public string SourceConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(this.SourceDbConnectionDbName)
                    || string.IsNullOrEmpty(this.SourceDbConnectionDbServer)
                    || string.IsNullOrEmpty(this.SourceDbConnectionSecurity))
                {
                    return string.Empty;
                }

                string sourceConn = this.SourceDbConnectionSecurity
                                    + ";database=" + this.SourceDbConnectionDbName
                                    + ";server=" + this.SourceDbConnectionDbServer
                                    + ";Connect Timeout=" + this.SourceDbConnectTimeoutInt;

                return sourceConn;
            }
        }

        public string TargetConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(this.TargetDbConnectionDbName)
                    || string.IsNullOrEmpty(this.TargetDbConnectionDbServer)
                    || string.IsNullOrEmpty(this.TargetDbConnectionSecurity))
                {
                    return string.Empty;
                }

                string targetConn = this.TargetDbConnectionSecurity
                                    + ";database=" + this.TargetDbConnectionDbName
                                    + ";server=" + this.TargetDbConnectionDbServer
                                    + ";Connect Timeout=" + this.TargetDbConnectTimeoutInt;

                return targetConn;
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method initializes the objects to its initial state.
        /// </summary>
        private void Init()
        {
            this.SourceDbConnectionDbName   = string.Empty;
            this.SourceDbConnectionDbServer = string.Empty;
            this.SourceDbConnectionSecurity = string.Empty;
            this.SourceDbConnectionTimeout  = "60";
            this.TargetDbConnectionDbName   = string.Empty;
            this.TargetDbConnectionDbServer = string.Empty;
            this.TargetDbConnectionSecurity = string.Empty;
            this.TargetDbConnectionTimeout  = "60";
        }
        #endregion
    }
}
