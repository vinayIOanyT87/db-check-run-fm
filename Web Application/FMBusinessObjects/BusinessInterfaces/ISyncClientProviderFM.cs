using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Synchronization.Data;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.UtilityObjects;

namespace FMBusinessObjects.BusinessInterfaces
{
	public interface ISyncClientProviderFM
	{
		#region Events
		event EventHandler<SyncProgressEventArgs> SyncProgress;
		event EventHandler<ApplyChangeFailedEventArgs> ApplyChangeFailed;
		event EventHandler<ApplyingChangesEventArgs> ApplyingChanges;
		event EventHandler<ChangesAppliedEventArgs> ChangesApplied;
		event EventHandler<ChangesSelectedEventArgs> ChangesSelected;
		#endregion Events

		#region Properties
		SyncContextFM Context { get; set; }
		#endregion Properties

		#region Methods
		void Dispose();
		#endregion Methods
	}
}
