using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FMNotificationBusinessObjects.DataObjects;
using FMNotificationBusinessServices.DataAccess;
using FMBusinessObjects.DataObjects;


namespace FMNotificationBusinessServices.UtilityObjects
{
    public class FileProcessor
    {

        #region Private Fields

        private string _site;
        private string _fileName;
        private string _data;
        private string _baseDir;

        #endregion

        #region Public Properties

        public string Site
        {
            get
            {
                return _site;
            }
            set
            {
                _site = value;
            }
        }

        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
            }
        }

        public string Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        #endregion

        #region Constructors

        public FileProcessor()
        {
            _baseDir = ConfigurationManager.AppSettings["BaseDirectory"];
        }

        public FileProcessor(string site, string fileName, string data)
        {
            _site = site;
            _fileName = fileName;
            _data = data;
            _baseDir = ConfigurationManager.AppSettings["BaseDirectory"];
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Creates a file out of the error summary data and places it in a site specific folder.
        /// </summary>
        /// <returns>Success or failure</returns>
        public bool ProcessFile()
        {
            try
            {
                ValidateData();
                string dir = GetDirectory();
                string path = Path.Combine(dir, _fileName);
                return WriteFile(path);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Gets the directory where a sites error summary files should be placed.
        /// </summary>
        /// <returns>The name of the directory.</returns>
        private string GetDirectory()
        {
            try
            {
                string directory = string.Empty;
                //mock security class
                SecurityClass sc = new SecurityClass();
                sc.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
                //replace this once the project is moved from RICE into the main app
                ErrorNotificationConfigDAL configDAL = new ErrorNotificationConfigDAL();
                ErrorNotificationConfig cfg = configDAL.GetErrorNotificationConfigBySite(_site,sc);
                directory = _baseDir + cfg.ErrorFolder + "\\";

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                return directory;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Writes the data to a file specified by the full file path.
        /// </summary>
        /// <param name="fullFilePath">Full path to file to be written.</param>
        /// <returns>Success or failure</returns>
        private bool WriteFile(string fullFilePath)
        {
            try
            {
                FileStream fs = File.Open(fullFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
                StreamWriter writer = new StreamWriter(fs);
                string reformatedData = _data.Replace("\n", Environment.NewLine);
                writer.Write(reformatedData);
                writer.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// validates field data used to process the error file.
        /// </summary>
        /// <returns>Success or failure</returns>
        private bool ValidateData()
        {
            try
            {
                //validate site information
                bool bEmptySite = string.IsNullOrEmpty(_site); //duplicate check?
                if (bEmptySite)
                    throw new Exception("Site not specified.");

                //validate that filename is valid
                bool bEmptyFileName = string.IsNullOrEmpty(_fileName); //duplicate check?
                bool bInvalidCharacters = _fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0;
                if (bEmptyFileName)
                    throw new Exception("File name is empty");
                if (bInvalidCharacters)
                    throw new Exception("File name contains invalid characters.");
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion
    }
}
