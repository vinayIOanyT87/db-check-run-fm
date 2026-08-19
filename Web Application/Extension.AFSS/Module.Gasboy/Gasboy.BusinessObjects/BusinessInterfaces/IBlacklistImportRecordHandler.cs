using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;

namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.BusinessInterfaces
{
	public interface IBlacklistImportRecordHandler
	{
		void ProcessImportRecord(GasboyBlacklistImportRecord importRecord);
	}
}
