using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FMBusinessObjects.Constants
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue" )]
	public enum FMEventLogEntryType
	{
		Warning = EventLogEntryType.Warning,
		Error = EventLogEntryType.Error
	}
}
