using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.Interfaces
{
	/// <summary>
	/// Summary description for QualityAssuranceException.
	/// </summary>
	[System.Serializable]
	[CLSCompliant(false)]
	public class QualityAssuranceException : System.Exception, System.Runtime.Serialization.ISerializable
	{
		#region Attributes
		protected System.Collections.ArrayList results;
		#endregion Attributes

		#region Properties
		public System.Collections.ArrayList Results
		{
			get { return results; }
		}
		#endregion Properties

		public QualityAssuranceException()
		{

		}
		public QualityAssuranceException(System.Collections.ArrayList results)
		{
			this.results = results;
		}

		#region ISerializable Members
		protected QualityAssuranceException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			this.results = (System.Collections.ArrayList)info.GetValue("results", typeof(System.Collections.ArrayList));
		}

		[System.Security.SecurityCritical]
		override public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("results", this.results);
		}
		#endregion
	}

	public struct FailedTestItem
	{
		public string Description;
		public double Result;
		public double CustomerLowSpecification;
		public double CustomerHighSpecification;
	}

	/// <summary>
	/// Summary description for IQualityAssurance.
	/// </summary>
	public interface IQualityAssurance
	{
		//	throw QualityAssuranceException on error
		bool GetTankCertification
		(
			SecurityClass Security,
			Guid tankGuid,
			Guid productGuid
		);

		//	throw QualityAssuranceException on error
		bool GetCertificateOfAnalysis
		(
			SecurityClass Security,
			Guid tankGuid,
			Guid ProductIndex,
			Guid OwnerCompanyGuid,
			Guid BillToCompanyGuid,
			Guid ShipToCompanyGuid,
			out FailedTestItem[] FailedTestItems
		);

		//	throw QualityAssuranceException on error
		void CreateCertificateOfAnalysis
		(
			SecurityClass Security,
			Guid tankGuid,
			Guid productGuid,
			Guid ownerCompanyGuid,
			Guid billToCompanyGuid,
			Guid shipToCompanyGuid,
			string BillOfLadingNumber,
			double Quantity,
			string EngineeringUnits,
			double LoadingTemperature,
			string OrderDocumentNumber,
			DateTimeOffset LoadedDate,
			DateTimeOffset ShipmentDate,
			Guid carrierCompanyGuid,
			string CompartmentID,
			string EquipmentID,
			string CustomerPurchaseOrderNumber,
			string Printer,
			bool COAWaiver,
			string UserIssuingWaiver,
			out string CertificateOfAnalysisID
		);

		bool BlendComponentsCOA
		(
			SecurityClass Security,
			Guid productGuid
		);
	}
}
