// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NspaExchangeResponseDownloadReferenceData.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the DownloadReferenceDataResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ADC.Nspa.General
{
	using System.Collections.Generic;
	using System.Data;

	public class DownloadReferenceDataResponse : ExchangeResponseBase
	{
		public class EntityData
		{
			public string Name { get; set; }
			public byte[] Binary { get; set; }
		}

		public DownloadReferenceDataResponse()
        {
            this.ExchangeType = ExchangeType.DownloadReferenceData;
			this.DownloadFile = new DownloadFileInfo();
        }

		public List<EntityData> ExchangeData { get; set; }

		public DownloadFileInfo DownloadFile { get; set; }

    }
}
