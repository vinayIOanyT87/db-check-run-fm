/// <summary>
///   File name:	TempPasswordGenerator.cs
///   Purpose:	Temporary Password Generator
///	Comments:	
///	Author(s):	Whaylon Coleman
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:				By:						Reason:
///		----------		--------------------	----------------------------------
///		2012-07-09		W.Coleman				Provides an implementation of the ITempPasswordGenerator interface.
using System;
using System.Data;
using System.Data.SqlClient;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;
using FMBusinessObjects.ServiceRequests;
using System.Runtime.Serialization;

namespace FMBusinessServices.ServiceClasses
{
	/// <summary>
	/// Default ITempPasswordGenerator implementation class
	/// </summary>
	[DataContract]
	public class TempPasswordGenerator : ITempPasswordGenerator, IDependency
	{
		#region Private data members
		private ConsolidatedDAClass consolidatedDA = new ConsolidatedDAClass();
		private FMSecurityValidationBase _fmSecurityValidation = null;
		#endregion

		/// <summary>
		/// This is the default constructor for the TempPasswordGenerator class
		/// </summary>
		public TempPasswordGenerator()
		{

		}

		#region Implementation of the ITempPasswordGenerator interface
		/// <summary>
		/// This method will return a temporary password for the Forgotten Password functionality within FuelsManager.
		/// </summary>
		/// <returns>An 8 character string which representing a temporary password with the following criteria:
		/// <para>* At least 8 characters long</para>
		/// <para>* One symbol</para>
		/// <para>* At least one uppercase character</para>
		/// <para>* And at least one number</para>
		/// </returns>
		public string GenerateTemporaryPassword(SecurityClass security)
		{
			_fmSecurityValidation = new FMSecurityValidation();
			return _fmSecurityValidation.GenerateTemporaryPassword(security);
		}

		/// <summary>
		/// This method will return a user's password hint.
		/// </summary>
		/// <returns>A string which represents a password hint configured in the User Configuration section of FuelsManager.</returns>
		public string GetPasswordHint(SecurityClass security, SecurityLoginRequest loginRequest)
		{
			string retVal = string.Empty;

			if (security == null)
				throw new ArgumentNullException("Security");

			TempPasswordDO tempPasswordDo = new TempPasswordDO();
			DataSet dataSet = null;
			using (SqlCommand cmd = new SqlCommand())
			{
				tempPasswordDo.GetSelectCommand(cmd, loginRequest);
				dataSet = consolidatedDA.GetDataSet(cmd, security);
			}

			if ((dataSet != null) && (dataSet.Tables != null))
			{
				if (dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count > 0)
				{
					retVal = (string)dataSet.Tables[0].Rows[0]["PasswordHint"];
				}
			}
			return retVal;
		}
		#endregion

		#region Implementation of the IDependency interface
		/// <summary>
		/// This method provides an insert implementation of the IDependency interface
		/// </summary>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="Object">A BaseDataObject instance</param>
		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (Object == null)
				throw new ArgumentNullException("Object");
		}

		/// <summary>
		/// This method provides an update implementation of the IDependency interface
		/// </summary>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="Object">A BaseDataObject instance</param>
		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (Object == null)
				throw new ArgumentNullException("Object");
		}

		/// <summary>
		/// This method provides an purge implementation of the IDependency interface
		/// </summary>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="Object">A BaseDataObject instance</param>
		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (Object == null)
				throw new ArgumentNullException("Object");
		}
		#endregion

	}
}