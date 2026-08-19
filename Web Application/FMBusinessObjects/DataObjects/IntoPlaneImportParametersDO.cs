namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;

    public class IntoPlaneImportParametersDO
    {
        #region Private Attributes

        private string managerFilter;
        private DateTime startDateFilter;
        private DateTime endDateFilter;
        private bool bUseTempGravVCFParams;
        private SortedList<string, IntoPlaneImportTempGravVcfParams> paramList;
        
        #endregion
        
        #region Construction
        public IntoPlaneImportParametersDO()
        {
            managerFilter = "";
            startDateFilter = DateTime.Now.Date;
            endDateFilter = startDateFilter;
            bUseTempGravVCFParams = true;
            paramList = new SortedList<string, IntoPlaneImportTempGravVcfParams>();
        }
        #endregion

        #region Public Properties
        public string ManagerFilter
        {
            get { return managerFilter; }
            set { managerFilter = value; }
        }
        public DateTime StartDateFilter
        {
            get { return startDateFilter; }
            set { startDateFilter = value; }
        }
        public DateTime EndDateFilter
        {
            get { return endDateFilter; }
            set { endDateFilter = value; }
        }
        public bool UseTempGravVCFParam
        {
            get { return bUseTempGravVCFParams; }
            set { bUseTempGravVCFParams = value; }
        }

        // Required for serilization to pass paramList from Web to FMBusinessServices
        public SortedList<string, IntoPlaneImportTempGravVcfParams> ParamList
        {
            get { return paramList; }
            set { paramList = value; }
        }
        #endregion

        #region Public Functions
        public void AddTempGravityVCFParam(string productID, IntoPlaneImportTempGravVcfParams param)
        {
            if(paramList == null)
            {
                paramList = new SortedList<string, IntoPlaneImportTempGravVcfParams>();
            }

            if(!paramList.ContainsKey(productID))
            {
                paramList.Add(productID, param);
            }
        }
        public IntoPlaneImportTempGravVcfParams GetTempGravityVCFParam(string productID)
        {
            if (paramList == null) return null;
            return (paramList.ContainsKey(productID) ? paramList[productID] : null);
        }
        #endregion
    }

}
