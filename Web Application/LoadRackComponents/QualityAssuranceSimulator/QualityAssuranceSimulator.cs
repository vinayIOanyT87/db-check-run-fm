/******************************************************************************

	FILE NAME:		QualityAssuranceSimulator.cs


	PURPOSE:			QualityAssuranceSimulatorClass


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------

*******************************************************************************/
using System;
using System.Diagnostics;
using FMCommon;
using ConsolidatedBLL;
using ConsolidatedDataObjects;

namespace QualityAssuranceSimulator
{
	/// <summary>
	/// Summary description for QualitySimulator
	/// </summary>
	public class QualityAssuranceSimulator : IQualityAssurance
	{
		EventLog	Log;
 
		public QualityAssuranceSimulator()
		{
			Log=new EventLog("Application",".","QualityAssuranceSimulator");
		}

		bool IQualityAssurance.GetTankCertification(
			SecurityClass	Security,
			int				TankIndex,
			int				ProductIndex)
		{
			object Certificate=(int) 1;

			try
			{
				TanksClass Tanks=new TanksClass();
				TankClass Tank=Tanks.Get(Security,TankIndex);
				Microsoft.Win32.RegistryKey Key=Microsoft.Win32.Registry.LocalMachine.CreateSubKey("Software\\Varec\\QualityAssuranceSimulator");
				if(Key != null)
				{
					Certificate = Key.GetValue(Tank.ID);
					if(Certificate == null
					|| !typeof(int).IsInstanceOfType(Certificate))
					{
						Certificate=(int) 1;
						Key.SetValue(Tank.ID,Certificate);
					}
				}
			}
			catch (Exception e)
			{
				Log.WriteEntry(e.Message,EventLogEntryType.Error);
			}

			return ((int) Certificate == 1) ? true : false;
		}

		bool IQualityAssurance.GetCertificateOfAnalysis(
			SecurityClass	Security,
			int				TankIndex,
			int				ProductIndex,
			int				OwnerIndex,
			int				BillToIndex,
			int				ShipToIndex,
			out FailedTestItem [] FailedTestItems)
		{
			FailedTestItems=new FailedTestItem[0];

			try
			{
				string [] FailedTestItemDescriptions=new string[0];

				TanksClass Tanks=new TanksClass();
				TankClass Tank=Tanks.Get(Security,TankIndex);
				ProductsClass Products=new ProductsClass();
				ProductClass Product=Products.Get(Security,ProductIndex);
				CompaniesClass Companies=new CompaniesClass();
				CompanyClass Owner=Companies.Get(Security,OwnerIndex);
				CompanyClass BillTo=Companies.Get(Security,BillToIndex);
				CompanyClass ShipTo=Companies.Get(Security,ShipToIndex);

				string Value=Tank.ID+" - "+Product.ID+" - "+Owner.ID+" - "+BillTo.ID+" - "+ShipTo.ID;

				Microsoft.Win32.RegistryKey Key=Microsoft.Win32.Registry.LocalMachine.CreateSubKey("Software\\Varec\\QualityAssuranceSimulator");
				if(Key != null)
				{
					FailedTestItemDescriptions = Key.GetValue(Value) as string[];
					if(FailedTestItemDescriptions == null
					|| !typeof(string []).IsInstanceOfType(FailedTestItemDescriptions))
					{
						FailedTestItemDescriptions=new string[0];
						Key.SetValue(Value,FailedTestItemDescriptions);
					}
					else
					{
						FailedTestItems=new FailedTestItem[FailedTestItemDescriptions.Length];
						int Index=0;
						foreach(string FailedTestItemDescription in FailedTestItemDescriptions)
							FailedTestItems[Index++].Description=FailedTestItemDescription;						
					}
				}
			}
			catch (Exception e)
			{
				Log.WriteEntry(e.Message,EventLogEntryType.Error);
			}

			return (FailedTestItems.Length == 0) ? true : false;
				
		}

		void IQualityAssurance.CreateCertificateOfAnalysis(
			SecurityClass	Security,
			int				TankIndex,
			int				ProductIndex,
			int				OwnerIndex,
			int				BillToIndex,
			int				ShipToIndex,
			string			BillOfLadingNumber,
			double			Quantity,
			string			EngineeringUnits,
			double			LoadingTemperature,
			string			OrderDocumentNumber,
			DateTime			LoadedDate,
			DateTime			ShipmentDate,
			int				CarrierIndex,
			string			CompartmentID,
			string			EquipmentID,
			string			CustomerPurchaseOrderNumber,
			string			Printer,
			bool				COAWaiver,
			string			UserIssuingWaiver,		
			out string		CertificateOfAnalysisID)
		{
			object ID=(int) 1;
			CertificateOfAnalysisID="1";

			try
			{
				Microsoft.Win32.RegistryKey Key=Microsoft.Win32.Registry.LocalMachine.CreateSubKey("Software\\Varec\\QualityAssuranceSimulator");
				if(Key != null)
				{
					ID = Key.GetValue("CertificateOfAnalysisID");
					if(ID == null
					|| !typeof(int).IsInstanceOfType(ID))
						ID=(int) 1;
					else
						ID=((int) ID)+1;

					CertificateOfAnalysisID=ID.ToString();
					Key.SetValue("CertificateOfAnalysisID",ID);
				}
			}
			catch (Exception e)
			{
				Log.WriteEntry(e.Message,EventLogEntryType.Error);
			}
		}

		bool IQualityAssurance.BlendComponentsCOA(
			SecurityClass	Security,
			int				ProductIndex)
		{
			object BlendComponentsCOA=(int) 1;

			try
			{
				ProductsClass Products=new ProductsClass();
				ProductClass Product=Products.Get(Security,ProductIndex);

				Microsoft.Win32.RegistryKey Key=Microsoft.Win32.Registry.LocalMachine.CreateSubKey("Software\\Varec\\QualityAssuranceSimulator");
				if(Key != null)
				{
					BlendComponentsCOA = Key.GetValue(Product.ID);
					if(BlendComponentsCOA == null
					|| !typeof(int).IsInstanceOfType(BlendComponentsCOA))
					{
						BlendComponentsCOA=(int) 1;
						Key.SetValue(Product.ID,BlendComponentsCOA);
					}
				}
			}
			catch (Exception e)
			{
				Log.WriteEntry(e.Message,EventLogEntryType.Error);
			}

			return ((int) BlendComponentsCOA == 1) ? true : false;
		}
	}
}
