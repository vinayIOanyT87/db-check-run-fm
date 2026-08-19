using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;

namespace DispatchPrototype
{
	public enum DispatchDependencyType
	{
		Requests,
		Equipment,
		Personnel
	}

	public class CallbackCargo
	{
		public SecurityClass security;
		public string rowVersion;
	}

	public class DispatchDataAccess
	{
		protected SqlConnection Connection = null;

		public delegate void OnDataUpdatedHandler ( );
		public event OnDataUpdatedHandler OnDataUpdated;
		public event OnDataUpdatedHandler OnEquipmentUpdated;
		public event OnDataUpdatedHandler OnPersonnelUpdated;

		public delegate void OnErrorHandler ( Exception except, bool bFatalError );
		public event OnErrorHandler OnError;

		Timer TransactionTimer;
		Timer EquipmentTimer;
		Timer PersonnelTimer;

		private int PollTime = 3000;

		public DispatchDataAccess ( SecurityClass security )
		{
			FMChannelFactory<IConfigurationSettings> clientConfigurationSettings = new FMChannelFactory<IConfigurationSettings>();
			IConfigurationSettings config = clientConfigurationSettings.CreateProxy();

			ConfigurationSettingDOClass configDO = config.GetByKey( security, ConfigurationSettingDOClass.Key_DISPATCH_PollTime );
			int? value = configDO.GetIntegerValue();

			if (value.HasValue)
			{
				PollTime = value.Value;
				PollTime *= 1000;
			}
		}

		void dependency_OnChange ( object sender )
		{
			DispatchTransactionsSR sr = (DispatchTransactionsSR) sender;

			// Check transverion
			FMChannelFactory<IDispatchTransactionsProcessor> clientDispatchTransactions = new FMChannelFactory<IDispatchTransactionsProcessor>();
			IDispatchTransactionsProcessor dispatchTransactionsProcessor = clientDispatchTransactions.CreateProxy();
			string topVer = ConvertToString(dispatchTransactionsProcessor.GetTransVersion( sr ));

			// If transversion indicates an update happened, invoke the callback.
			if (sr.RowVersion.CompareTo(topVer) < 0)
			{
				TransactionTimer = null;

				if (OnDataUpdated != null)
				{
					OnDataUpdated.Invoke();
				}
			}
			else
			{
				TransactionTimer.Change( 3000, Timeout.Infinite );
			}
		}

		void dependency_EquipmentOnChange ( object sender ) 
		{
			CallbackCargo cargo = (CallbackCargo)sender;

			string rowVersion = cargo.rowVersion;

			FMChannelFactory<IEquipments> clientEquipments = new FMChannelFactory<IEquipments>();
			IEquipments equipments = clientEquipments.CreateProxy();

			string latest = equipments.GetLatestRowVersionBySource( cargo.security );

			if (latest.CompareTo( cargo.rowVersion ) > 0)
			{
				EquipmentTimer = null;
				if (OnEquipmentUpdated != null)
				{
					OnEquipmentUpdated.Invoke();
				}
			}
			else
			{
				EquipmentTimer.Change( 3000, Timeout.Infinite );
			}
		}

		void dependency_PersonnelOnChange ( object sender )
		{
			CallbackCargo cargo = (CallbackCargo)sender;

			string rowVersion = cargo.rowVersion;

			FMChannelFactory<IPersonnel> clientPersonnel = new FMChannelFactory<IPersonnel>();
			IPersonnel personnel = clientPersonnel.CreateProxy();

			string latest = personnel.GetLatestRowVersionByRole( cargo.security, PERSON_ROLE.DRIVER_ROLE );

			if (latest.CompareTo( cargo.rowVersion ) > 0)
			{
				PersonnelTimer = null;
				if (OnPersonnelUpdated != null)
				{
					OnPersonnelUpdated.Invoke();
				}
			}
			else
			{
				PersonnelTimer.Change( 3000, Timeout.Infinite );
			}
		}

		private void InvokeError( Exception e, bool bFatalError )
		{
			if (OnError != null)
			{
				OnError.Invoke( e, bFatalError );
			}
			else
			{
				throw new ApplicationException( "Exception from SqlDependency processing" );
			}
		}

		public void ClearOnChange ( )
		{
			OnDataUpdated = null;
		}


		public DispatchTransactionsDO GetTransactions ( DispatchTransactionsSR sr )
		{
			if (TransactionTimer != null)
			{
				TransactionTimer.Change( Timeout.Infinite, Timeout.Infinite );
				TransactionTimer = null;
			}

			FMChannelFactory<IDispatchTransactionsProcessor> dispatchTransClient = new FMChannelFactory<IDispatchTransactionsProcessor>();
			IDispatchTransactionsProcessor dispatchTransProcessor = dispatchTransClient.CreateProxy();

			DispatchTransactionsDO results = dispatchTransProcessor.Process( sr );

			sr.RowVersion = GetLatestRowVersion( results.Transactions.Tables[0].Rows );

			TransactionTimer = new Timer( (TimerCallback)this.dependency_OnChange, sr, 3000, Timeout.Infinite );
	
			return results;
		}

		private string GetLatestRowVersion( DataRowCollection rows )
		{
			string rowVersion = string.Empty;

			foreach (DataRow row in rows)
			{
				string value = ConvertToString( (Byte[]) row["_RowVersion"] );

				if (value.CompareTo( rowVersion ) > 0)
				{
					rowVersion = value;
				}
			}

			return rowVersion;
		}


		private string ConvertToString( System.Byte[] rowVersion )
		{
			string result = string.Empty;

			foreach (byte b in rowVersion)
			{
				result += b.ToString( "X" );
			}

			return result;
		}

		private string GetLatestRowVersion<T>( List<T> rows )
		{
			string rowVersion = string.Empty;

			foreach (T row in rows)
			{
				string rowVer = ConvertToString((Byte[]) typeof( T ).GetProperty( "RowVersion" ).GetValue( row, null ));

				if (rowVer.CompareTo( rowVersion ) > 0)
				{
					rowVersion = rowVer;
				}
			}

			return rowVersion;
		}

		public DispatchTransactionsDO GetTransactionsNoUpdateConnection( DispatchTransactionsSR sr )
		{
			FMChannelFactory<IDispatchTransactionsProcessor> dispatchTransClient = new FMChannelFactory<IDispatchTransactionsProcessor> ( );
			IDispatchTransactionsProcessor dispatchTransProcessor = dispatchTransClient.CreateProxy ( );

			DispatchTransactionsDO results = dispatchTransProcessor.Process ( sr );
			return results;
		}

		public DispatchTransactionsDO GetTransactions ( DateTime BeginDate, DateTime EndDate )
		{
			DispatchTransactionsSR sr = new DispatchTransactionsSR ( );
			sr.BeginDate = BeginDate;
			sr.EndDate = EndDate;

			return GetTransactions ( sr );
		}

		public List<EquipmentClass> GetEquipmentNoUpdateConnection ( SecurityClass Security )
		{
			return InternalGetEquipment ( Security, false );
		}

		public List<EquipmentClass> GetEquipment ( SecurityClass Security )
		{
			return InternalGetEquipment ( Security, true );
		}

		private List<EquipmentClass> InternalGetEquipment ( SecurityClass Security, bool updateConnection )
		{
			FMChannelFactory<IEquipments> equipClient = new FMChannelFactory<IEquipments> ( );
			IEquipments equipments = equipClient.CreateProxy ( );

			List<EquipmentClass> equipmentCollection = (List<EquipmentClass>) equipments.EnumerateBySource ( Security );

			if (updateConnection)
			{
				string rowVersion = GetLatestRowVersion<EquipmentClass>( equipmentCollection );
				CallbackCargo cargo = new CallbackCargo() { security=Security, rowVersion=rowVersion };
				EquipmentTimer = new Timer( (TimerCallback)this.dependency_EquipmentOnChange, cargo, 3000, Timeout.Infinite );
			}

			return equipmentCollection;
		}

		public PersonCollectionClass GetPersonnelNoUpdateConnection ( SecurityClass Security )
		{
			return InternalGetPersonnel ( Security, false );
		}

		public PersonCollectionClass GetPersonnel ( SecurityClass Security )
		{
			return InternalGetPersonnel ( Security, true );
		}

		private PersonCollectionClass InternalGetPersonnel ( SecurityClass Security, bool updateConnection )
		{
			FMChannelFactory<IPersonnel> personnelClient = new FMChannelFactory<IPersonnel> ( );
			IPersonnel personnel = personnelClient.CreateProxy ( );

			PersonCollectionClass Persons = (PersonCollectionClass) personnel.EnumerateByRole ( Security, PERSON_ROLE.DRIVER_ROLE );

			if (updateConnection)
			{
				string rowVersion = GetLatestRowVersion<PersonClass>( Persons );
				CallbackCargo cargo = new CallbackCargo() { security=Security, rowVersion=rowVersion };
				PersonnelTimer = new Timer( (TimerCallback)this.dependency_PersonnelOnChange, cargo, 3000, Timeout.Infinite );
			}

			return Persons;
		}
	}
}
