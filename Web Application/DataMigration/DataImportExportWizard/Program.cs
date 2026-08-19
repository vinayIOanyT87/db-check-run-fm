// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataImportExportWizard
{
    using System;
    using System.Windows.Forms;

    using CmdLine;

    using DataImportExportWizard.Constants;
    using DataImportExportWizard.DataAccess;
    using DataImportExportWizard.InternalClasses;

    /// <summary>
    /// The program.
    /// </summary>
    public partial class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        [STAThread]
        static void Main(string[] args)
        {
            Options arguments = null;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.DoEvents();

            try
            {
                arguments = CommandLine.Parse<Options>();

                Start(arguments);

                arguments = null;
            }
            catch (CommandLineHelpException helpException)
            {
                // User asked for help
                MessageBox.Show(helpException.ArgumentHelp.GetHelpText(150));
                Environment.Exit(0);
            }
            catch (CommandLineException exception)
            {
                if (null != exception.ArgumentHelp)
                {
                    // Some other kind of command line error
                    MessageBox.Show(exception.ArgumentHelp.Message);
                    MessageBox.Show(exception.ArgumentHelp.GetHelpText(150));
                }
                else
                {
                    MessageBox.Show(exception.Message);
                }

                Environment.Exit(0);
            }

            // ConsoleApplication.RunProgram<CmdLineOptions>(Start);
        }

        /// <summary>
        /// The start.
        /// </summary>
        /// <param name="options">
        /// The options.
        /// </param>
        private static void Start(Options arguments)
        {
            if (arguments.CmdLineArgumentFound)
            {
                DataImportExportWizardOption.SetActionType(arguments.Action);

                if (DataImportExportWizardOption.Action.ToString().ToLower() != arguments.Action.ToLower())
                {
                    CommandArgumentHelp argumentHelp = new CommandArgumentHelp(typeof(Options), "Unrecognized Action.  Valid actions are EncryptOnly.");
                    throw new CommandLineException(argumentHelp);
                }

                DataImportExportWizardOption.SiteId = arguments.SiteId;
                DataImportExportWizardOption.InstanceName = arguments.InstanceName;
                DataImportExportWizardOption.DatabaseName = arguments.DatabaseName;
                DataImportExportWizardOption.Path = arguments.Path;
                DataImportExportWizardOption.FileName = arguments.File;
                DataImportExportWizardOption.BackupDatabaseFlag = arguments.BackupDatabaseFlag;
                DataImportExportWizardOption.ReEncryptFlag = arguments.ReEncryptFlag;
                DataImportExportWizardOption.ShowStatusFlag = arguments.ShowStatusFlag;

				object mainForm = null;
                ApplicationContext appContext = null;

                if (DataImportExportWizardOption.Action == DataImportExportActionType.EncryptOnly)
                {
                    // Allocate our main form.
                    mainForm = new ReEncryptPasswordForm();

                    // Force this to true, as if the -aes option was included.
                    DataImportExportWizardOption.ReEncryptFlag = true;
                }
                else
                {
                    if (!DataImportExportWizardOption.QuietFlag)
                    {
                        if (DataImportExportWizardOption.Action != DataImportExportActionType.Other)
                        {
                            mainForm = new DataImportExportWizardSheet();
                        }
                    }
                }

                if (null != mainForm)
                {
                    appContext = new AutoApplicationContext((Form)mainForm);

                    Application.Run(appContext);
                }
                else
                {
                    CommandArgumentHelp argumentHelp = new CommandArgumentHelp(typeof(Options), "Not enough command line arguments specified to perform selected action using the -q (quiet) option.");
                    throw new CommandLineException(argumentHelp);
                }
            }
        }
    }
}
