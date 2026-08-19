// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusProcessingUserControl.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the StatusProcessingUserControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataImportExportWizard
{
    using System.Windows.Forms;

    public partial class StatusProcessingUserControl : UserControl
    {
        #region Constructors

        public StatusProcessingUserControl()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// The update status.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void UpdateStatus(string message)
        {
            this.StatusTextBox.AppendText(string.Format("{0}", message));
        }

        /// <summary>
        /// The update status.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void UpdateStatusLine(string message)
        {
            this.StatusTextBox.AppendText(string.Format("{0}{1}", message, System.Environment.NewLine));
        }
        #endregion Public Methods
    }
}
