// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnterpriseExport.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the EnterpriseExport type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataImportExportWizard.InternalClasses
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EnterpriseExport : ImportExportBase
    {
        #region Attributes

        private string _SiteId = string.Empty;
        #endregion Attributes

        #region Static Properties
        #endregion Static Properties

        #region Constructors

        public EnterpriseExport(string serverName, string databaseName, string contentType)
            : base(serverName, databaseName, "keyfile")
        {
            
        }
        #endregion Constructors

        #region Properties
        #endregion Properties

        #region Static Methods
        #endregion Static Methods

        #region Public Methods
        public void BeginExport(string siteId)
        {
            
        }
        #endregion Public Methods

        #region Private Methods
        public void ExportData(string query, string fileName)
        {
            Process p = new System.Diagnostics.Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "BCP.exe";
            p.StartInfo.Arguments = string.Format("\"{0}\" queryout \"{1}\" -S \"{2}\" -T -c -k", query, fileName, this.ServerName);
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
        }

        private void CombineExportFiles()
        {
            
        }
        #endregion Private Methods
    }
}
