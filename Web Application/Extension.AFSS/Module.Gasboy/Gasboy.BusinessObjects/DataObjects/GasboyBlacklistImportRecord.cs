using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using FMBusinessObjects.DataObjects;
using FuelsManager.Afss.Module.Gasboy.BusinessObjects.Constants;

namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects
{
	public class GasboyBlacklistImportRecord
	{
		/// <summary>
		/// Constructor for a Gasboy Blacklist Import Record object.
		/// </summary>
		public GasboyBlacklistImportRecord()
		{
			this.DeviceName = string.Empty;
			this.CardNumber = string.Empty;
			this.EffectiveDateTime = null;
		}

		/// <summary>
		/// The Device Name of the Gasboy Device Entry
		/// </summary>
		[DataMember]
		[Required(ErrorMessage = "Device Name is required")]
		public string DeviceName { get; set; }

		/// <summary>
		/// The Card Number of the Gasboy Device Entry
		/// </summary>
		[DataMember]
		[Required(ErrorMessage = "Card Number is required")]
		public string CardNumber { get; set; }

		/// <summary>
		/// The blacklist date for the Gasboy Device Entry
		/// </summary>
		[DataMember]
		public DateTimeOffset? EffectiveDateTime { get; set; }
	}
}
