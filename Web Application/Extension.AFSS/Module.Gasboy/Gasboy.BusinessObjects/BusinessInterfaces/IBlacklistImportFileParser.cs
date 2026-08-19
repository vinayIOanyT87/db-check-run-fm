using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;

namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.BusinessInterfaces
{
	public interface IBlacklistImportFileParser
	{
		void ParseImportFile(string importfile, Action<GasboyBlacklistImportRecord> recordHandlerAction);
	}
}
