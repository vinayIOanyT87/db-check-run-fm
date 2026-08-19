using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Data.SqlClient;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;
using FMBusinessObjects.LogClient;

namespace FMBusinessServices.ServiceClasses
{
    public class SitesInfoClass : ISitesInfo
    {
        #region private data members
        private ConsolidatedDAClass consolidatedDA;
        private SiteInfoDO siteInfoDO;
        #endregion

        #region Constructors
        public SitesInfoClass()
        {
            this.consolidatedDA = new ConsolidatedDAClass();
            this.Reset();
        }
        #endregion

        #region Public methods
        public void Reset()
        {
            this.siteInfoDO = new SiteInfoDO();
        }

        public SiteInfoDO RefreshSiteInfo(SecurityClass security)
        {
            this.CheckSecurity(security);
            this.Reset();

            SitesClass Sites = new SitesClass();

            StopWatch timer = new StopWatch(StopWatch.Appnames.ConsolidatedBLL, "SiteInfo.Refresh.EnumerateSitesInfo()");
            this.siteInfoDO.SiteCollection = Sites.EnumerateSitesInfo(security);
            timer.Stop();

            // Now get the SiteToSiteMaps
            this.GetSiteToSiteMaps(security);

            return this.siteInfoDO;
        }
        #endregion

        #region Private methods
        private void GetSiteToSiteMaps(SecurityClass security)
        {
            SiteToSiteMapClass map = new SiteToSiteMapClass();

            ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

            StopWatch timer = new StopWatch(StopWatch.Appnames.ConsolidatedBLL, "SiteInfo.GetSiteToSiteMaps.GetDataSet()");
            using (SqlCommand cmd = new SqlCommand())
            {
                map.EnumerateSQL(cmd);
                DataSet dataSet = ConsolidatedDA.GetDataSet(cmd, security);
                timer.Stop();

                if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
                {
                    DataTable table = dataSet.Tables[0];

                    if (table.Rows != null)
                    {
                        foreach (DataRow Row in table.Rows)
                        {
                            map = new SiteToSiteMapClass();
                            map.LoadObject(Row);
                            this.siteInfoDO.SiteToSiteMaps.Add(map);
                        }
                    }
                }
            }
        }

        private void CheckSecurity(SecurityClass Security)
        {
            if (Security == null)
            {
                throw new ArgumentNullException("Security");
            }
        }
        #endregion
    }
}