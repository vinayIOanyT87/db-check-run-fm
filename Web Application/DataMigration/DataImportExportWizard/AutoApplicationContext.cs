// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReEncryptApplicationContext.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ReEncryptApplicationContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataImportExportWizard
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Windows.Forms;

    using DataImportExportWizard.Constants;
    using DataImportExportWizard.Interfaces;
    using DataImportExportWizard.InternalClasses;
    using DataImportExportWizard.InternalClasses.LogClient;

    public class AutoApplicationContext : ApplicationContext
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private Logger loggerInstance;

        public delegate void ApplicationContextClosedEvent(object sender, EventArgs e);

        /// <summary>
        /// The loggerInstance.
        /// </summary>
        public Logger LoggerInstance
        {
            get
            {
                return this.loggerInstance;
            }

            set
            {
                this.loggerInstance = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoApplicationContext"/> class.
        /// </summary>
        public AutoApplicationContext(Form mainForm)
            : base(mainForm)
        {
            this.loggerInstance = new Logger(string.Format("{0}_AESEncrypt", StringConstants.ApplicationShortName));

            // Handle the ApplicationExit event to know when the application is exiting.
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);

            try
            {
                mainForm.FormClosed += this.mainForm_FormClosed;

                var form = mainForm as IMigrationForm;
                if (form != null)
                {
                    form.EnableAutoRun(DataImportExportWizardOption.QuietFlag);
                }
            }
            catch (IOException e)
            {
                MessageBox.Show(
                    @"An error occurred while attempting to re-encrypt persisted data." + @"The error is:"
                    + e.ToString());
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    @"An error occurred while attempting to re-encrypt persisted data." + @"The error is:"
                    + e.ToString());
            }
        }

        protected void mainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.ExitThread();
        }

        /// <summary>
        /// The on application exit.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnApplicationExit(object sender, EventArgs e)
        {

        }
   }
}