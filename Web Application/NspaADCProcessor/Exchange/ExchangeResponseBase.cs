// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DownloadReferenceDataResponse.cs" company="Varec, Inc.">
//   
// </copyright>
// <summary>
//   Defines the DownloadReferenceDataResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ADC.Nspa.General
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public class ExchangeResponseBase : ExchangeBase
    {
		private List<string> internalErrorList = new List<string>();

        public bool Success { get; set; }
        
	    public List<string> ErrorList
	    {
		    get
		    {
			    return internalErrorList;
		    }
		    set
		    {
			    this.internalErrorList = value;
		    }
	    }
	   
    }
}
