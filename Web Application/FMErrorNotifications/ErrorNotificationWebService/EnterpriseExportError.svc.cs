using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using FMNotificationBusinessObjects.BusinessInterfaces;
using FMNotificationBusinessServices.UtilityObjects;

namespace FMErrorNotificationWebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class EnterpriseExportError : IEnterpriseExportError
    {
        /// <summary>
        /// Method that recieves the error summary data from an FMAEInterface import.
        /// </summary>
        /// <param name="site">ID of the site.</param>
        /// <param name="filename">Name of the file to be created.</param>
        /// <param name="data">The error summary information.</param>
        /// <returns></returns>
        public ReceiveErrorResponse ReceiveErrors(string site, string filename, string data)
        {
            var response = new ReceiveErrorResponse();
            try
            {
                
                //validate input
                if (String.IsNullOrEmpty(site) || String.IsNullOrEmpty(filename) || String.IsNullOrEmpty(data))
                {
                    string err = string.Empty;
                    if (String.IsNullOrEmpty(site))
                        err += "Site infomration is missing. ";
                    if (String.IsNullOrEmpty(filename))
                        err += "File name is missing. ";
                    if (String.IsNullOrEmpty(data))
                        err += "Error data is missing.";
                    response.OperationSuccessful = false;
                    response.OperationErrorText = err;
                }
                //process data
                var processor = new FileProcessor(site, filename, data);
                response.OperationSuccessful = processor.ProcessFile();
                if (!response.OperationSuccessful)
                    response.OperationErrorText = "The system was unable to save the error summary.";

                return response;
            }
            catch(Exception ex)
            {
                response.OperationSuccessful = false;
                response.OperationErrorText = ex.Message;
                return response;
            }
        }
    }
}
