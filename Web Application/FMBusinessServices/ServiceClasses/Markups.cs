using System.Security;
using System.ServiceModel;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System;
using System.Collections.Generic;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class MarkupsClass : IMarkups
	{
		#region Private data members
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the GST class.
		/// </summary>
		public MarkupsClass ( )
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Retrieves a collection of all Markups
		/// </summary>
		/// <returns>A collection containing all Markups</returns>
		public MarkupDOCollection GetAll ( SecurityClass security )
		{
			MarkupDOCollection markups = new MarkupDOCollection ( );

			try
			{
				MarkupDO markupDO = new MarkupDO ( );
				using (SqlCommand cmd = new SqlCommand())
				{
					markupDO.SelectMarkups(cmd);
					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

					if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
					{
						DataTable dataTable = dataSet.Tables[0];

						if (dataTable.Rows != null)
						{
							foreach (DataRow dataRow in dataTable.Rows)
							{
								MarkupDO markup = new MarkupDO();
								markup.Populate(dataRow);
								markups.Add(markup);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception ( "An error occurred attempting to retrieve Markups from the database.  " + ex.Message );
			}

			return markups;
		}

		/// <summary>
		/// Retrieves the companies assigned to a Markup
		/// </summary>
		/// <param name="markup">The Markup companies are assigned to</param>
		/// <returns>The companies assigned to the passed Markup</returns>
		public List<TaxCompanyMapDO> GetMarkupCompanies ( MarkupDO markup, SecurityClass security )
		{
			List<TaxCompanyMapDO> companyMapList = new List<TaxCompanyMapDO> ( );
			TaxCompanyMapDO companyMap = null;

			try
			{
				MarkupDO markupDO = new MarkupDO ( );
				using (SqlCommand cmd = new SqlCommand())
				{
					markupDO.SelectMarkupCompanies(cmd, markup);
					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

					if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
					{
						DataTable dataTable = dataSet.Tables[0];

						if (dataTable.Rows != null)
						{
							foreach (DataRow row in dataTable.Rows)
							{
								companyMap = new TaxCompanyMapDO();
								companyMap.CompanyGuid = DataObject.getValue<Guid>(row["CompanyGuid"], Guid.Empty);
								companyMap.CompanyID = DataObject.getValue<string>(row["ID"], "");

								companyMapList.Add(companyMap);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception ( "An error occurred attempting to retrieve Markup Companies from the database.  " + ex.Message );
			}

			companyMapList.Sort 
			( 
				delegate ( TaxCompanyMapDO class1, TaxCompanyMapDO class2 )
				{
					return ( Comparer<string>.Default.Compare ( class1.CompanyID, class2.CompanyID ) );
				} 
			);

			return companyMapList;
		}

		/// <summary>
		/// Removes a Markup from the database
		/// </summary>
		/// <param name="markup">The Markup to remove</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Remove(MarkupDO markup, SecurityClass security)
		{
			try
			{
				// Delete all the associated companies prior to deleting the Markup entry.
				TaxCompanyMapClass taxCompanyMap = new TaxCompanyMapClass ( );
				taxCompanyMap.DeleteAllAssociatedCompanies ( security, markup.IdentityGuid, TaxCompanyMapDO.TaxMapTypes.MARKUP_MAP );

				MarkupDO markupDO = new MarkupDO ( );
				using (SqlCommand cmd = new SqlCommand())
				{
					markupDO.Delete(cmd, markup);
					this.consolidatedDA.ExecuteQuery(security, cmd);
				}
			}
			catch (Exception ex)
			{
				throw new Exception ( "An error occured attempting to remove a Markup.  " + ex.Message );
			}
		}

		/// <summary>
		/// Saves changes to a Markup to the database
		/// </summary>
		/// <param name="markup">The Markup to save</param>
		/// <param name="security">Security</param>
		/// <param name="companyList">A list of associated companies to add</param>
		/// <param name="deletedCompanyList">A list of associated companies to remove</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Save(MarkupDO markup, SecurityClass security, List<TaxCompanyMapDO> companyList, List<TaxCompanyMapDO> deletedCompanyList)
		{
			try
			{
				MarkupDO markupDO = new MarkupDO ( );
				using (SqlCommand cmd = new SqlCommand())
				{
					markupDO.Update(cmd, markup, security.UserID);
					this.consolidatedDA.ExecuteQuery(security, cmd);

					// Insert any new company associations
					TaxCompanyMapClass taxCompanyMap = new TaxCompanyMapClass();

					if ((companyList != null) && (companyList.Count > 0))
					{
						taxCompanyMap.InsertMarkupAssociatedCompanies(security, companyList, markup.IdentityGuid);
					}

					// Remove company associations
					if ((deletedCompanyList != null) && (deletedCompanyList.Count > 0))
					{
						taxCompanyMap.DeleteMarkupAssociatedCompanies(security, deletedCompanyList, markup.IdentityGuid);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception ( "An error occurred attempting to save a Markup.  " + ex.Message );
			}
		}

		/// <summary>
		/// Adds the passed Markup to the database
		/// </summary>
		/// <param name="markup">The Markup to add</param>
		/// <param name="security">Security</param>
		/// <param name="companyList">A list of associated companies to add</param>
		/// <returns>The auto-generated guid of the newly added Markup</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(MarkupDO markup, SecurityClass security, List<TaxCompanyMapDO> companyList)
		{
			Guid markupGuid = Guid.Empty;

			try
			{
				MarkupDO markupDO = new MarkupDO ( );
				using (SqlCommand cmd = new SqlCommand())
				{
					markupDO.IdentityGuid = Guid.NewGuid();
					markupDO.Insert(cmd, markup, security.UserID);
					this.consolidatedDA.ExecuteQuery(security, cmd);
					markupGuid = markupDO.IdentityGuid;
				}

				// Insert company associations
				TaxCompanyMapClass taxCompanyMap = new TaxCompanyMapClass();
				if ((companyList != null) && (companyList.Count > 0) && (markupGuid != Guid.Empty))
				{
					taxCompanyMap.InsertMarkupAssociatedCompanies(security, companyList, markupGuid);
				}

			}
			catch (Exception ex)
			{
				throw new Exception ( "An error occurred attempting to add a Markup or Purchasing Unit exists.  " + ex.Message );
			}

			return markupGuid;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method returns True if the company assignment already exists.
		/// </summary>
		/// <param name="markup"></param>
		/// <param name="companyGuid"></param>
		/// <returns></returns>
		private bool CompanyAssignmentExists ( SecurityClass security, MarkupDO markup, Guid companyGuid )
		{
			bool exists = false;

			try
			{
				MarkupDO markupDO = new MarkupDO ( );
				using (SqlCommand cmd = new SqlCommand())
				{
					markupDO.CompanyAssignmentExists(cmd, markup, companyGuid);

					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

					if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
					{
						DataTable dataTable = dataSet.Tables[0];

						if ((dataTable.Rows != null) && (dataTable.Rows.Count > 0))
						{
							exists = true;
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception ( "Error determining if company assignment exists.  " + ex.Message );
			}

			return exists;
		}
		#endregion
	}
}