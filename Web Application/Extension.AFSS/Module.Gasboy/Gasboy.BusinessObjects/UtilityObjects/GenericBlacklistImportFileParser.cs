using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuelsManager.Afss.Module.Gasboy.BusinessObjects.BusinessInterfaces;
using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;

namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.UtilityObjects
{
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	public class GSALockListImportFileParser : IBlacklistImportFileParser
	{
		private readonly int gsaCardNumberLength;

		public GSALockListImportFileParser()
		{
			this.gsaCardNumberLength = 0;
		}

		public GSALockListImportFileParser(int gsaCardNumberLength)
		{
			this.gsaCardNumberLength = gsaCardNumberLength;
		}

		public void ParseImportFile(string importFile, Action<GasboyBlacklistImportRecord> recordHandlerAction)
		{
			if (string.IsNullOrEmpty(importFile))
			{
				throw new ArgumentNullException("importFile", @"Import file must be specified.");
			}

			if (null != recordHandlerAction)
			{
				TextReader tr = new StreamReader(importFile, Encoding.Default);

				string singleLine = tr.ReadLine();

				while (!string.IsNullOrEmpty(singleLine))
				{
					if (singleLine.StartsWith(@"FLEET"))
					{
						continue;
					}

					var blackListCard = new GasboyBlacklistImportRecord()
					                    {
						                    CardNumber = singleLine.Substring(0, this.gsaCardNumberLength),
											EffectiveDateTime = TypeHelper.ConvertDateTimeOffset(singleLine.Substring(this.gsaCardNumberLength)),
					                    };

					recordHandlerAction.Invoke(blackListCard);

					singleLine = tr.ReadLine();
				}
			}
		
		}
	}
}
