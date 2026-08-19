// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationDataExportPage.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the MigrationDataExportPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMEntityImportWebApp
{
    using System;
    using System.IO;

    using FMBusinessObjects.DataObjects;

    using FuelsManager.FMWebApp;

	/// <summary>
    /// The migration data import export.
    /// </summary>
    public partial class MigrationDataExportPageDownload : FMFormBase
    {
        #region Properties
        /// <summary>
        /// Gets or sets the cached download filename.
        /// </summary>
        private string CachedDownloadFilename
        {
            get
            {
	            if (this.Session[MigrationDataExportPage.MigrationCachedDownloadFilename] != null 
					&& this.Session[MigrationDataExportPage.MigrationCachedDownloadFilename] is string)
                {
                    return (string)this.Session[MigrationDataExportPage.MigrationCachedDownloadFilename];
                }
	            return null;
            }
        }
        #endregion Properties

        #region Page Events and Overrides

        /// <summary>
        /// Overrides the virtual <code>OnInit</code> framework method.  Any special page specific initialization can be performed here.
        /// </summary>
        /// <param name="e">
        /// Event arguments of type <see cref="EventArgs"/> for the <code>OnInit</code> method.
        /// </param>
        protected override void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            this.InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        ///    Required method for Designer support - do not modify
        ///    the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // this.OK.Command += new System.Web.UI.WebControls.CommandEventHandler(this.OK_Command);
            // this.Cancel.Command += new System.Web.UI.WebControls.CommandEventHandler(this.Cancel_Command);
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <exception cref="Exception">
        /// Throws an exception for Insufficient Rights if the user does not have ANY View rights for Client, Server and Periodic Synchronization Settings.
        /// </exception>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.GetSecurity();

                if (!this.Security.HasRight(RIGHT.MIGRATION_PERFORM_IMPORT_EXPORT))
                {
                    throw new Exception("Insufficient Rights");
                }

                if (!this.Page.IsPostBack)
                {
                    string cachedFilename = this.CachedDownloadFilename;

                    if (!string.IsNullOrEmpty(cachedFilename))
                    {
                        this.Session.Remove(MigrationDataExportPage.MigrationCachedDownloadFilename);

                        // Don't cross the streams
	                    this.Response.ClearContent();
	                    this.Response.ClearHeaders();

	                    this.Response.AddHeader(
                            "Content-disposition", "attachment; filename=" + Path.GetFileName(cachedFilename));
	                    this.Response.Buffer = false;
	                    this.Response.ContentType = "application/octet-stream";
                        this.Response.AddHeader("cache-control", "private, max-age=0");
                        this.Response.AddHeader("Connection", "Keep-Alive");

                        // Read the memory stream and stream it back to the client in chunks
                        byte[] buffer = new byte[131072];

	                    using (var cachedFile = new FileStream(cachedFilename, FileMode.Open))
                        {
	                        int byteCount;
	                        while ((byteCount = cachedFile.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                this.Response.OutputStream.Write(buffer, 0, byteCount);
                                this.Response.Flush();
                            }
                        }

	                    // Complete request and stop more than the file from rendering to the client
                        this.Response.SuppressContent = true;
                    }
                }
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }
        #endregion Page Events and Overrides
    }
}