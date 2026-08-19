using System;
using System.Collections.Generic;
using System.Text;

namespace ConsolidatedDBTransactions
{
    internal class ApplicationSettings
    {
        private static volatile ApplicationSettings _instance = null;
        private string _dataSource = string.Empty;
        private string _initialCatalog = string.Empty;

        private ApplicationSettings()
        {
        }

        internal static ApplicationSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (typeof(ApplicationSettings))
                    {
                        if (_instance == null)
                            _instance = new ApplicationSettings();
                    }
                }

                return _instance;
            }
        }

        internal string DataSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }

        internal string InitialCatalog
        {
            get { return _initialCatalog; }
            set { _initialCatalog = value; }
        }
    }
}
