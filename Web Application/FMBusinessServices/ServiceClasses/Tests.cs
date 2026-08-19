using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Web;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	using FMBusinessObjects.Exceptions;

	/// <summary>
	/// Summary description for TestsClass.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class TestsClass : ITests, IDependency
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public TestsClass()
		{
		}

		private void Validate(TestClass test)
		{
			if (test.ID.Trim().Length == 0)
				throw (new Exception("Name Required"));
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, TestClass test)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (test == null)
			{
				throw new ArgumentNullException("Test");
			}

			if (!security.HasRight(RIGHT.MODIFY_TEST_ITEMS))
				throw new FMInsufficientRightsException(); 

			if (GetIdentityGuid(security, test.ID) != Guid.Empty)
			{
				throw new Exception("Test Exists.");
			}

			Validate(test);

			test.CreatedDate = DateTimeOffset.Now;
			test.CreatedBy = security.UserID;
			test.UpdatedDate = test.CreatedDate;
			test.UpdatedBy = security.UserID;
			test.IdentityGuid = Guid.NewGuid();

			using (SqlCommand cmd = new SqlCommand())
			{
				test.InsertSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			// Create Entity to Site Map
			EntityToSiteMaps entityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapClass entityToSiteMap = new EntityToSiteMapClass(test);
			entityToSiteMaps.Add(security, entityToSiteMap, GetType().GUID);

			return test.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, TestClass test)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (test == null)
			{
				throw new ArgumentNullException("Test");
			}

			Validate(test);

			TestClass oldTest = Get(security, test.IdentityGuid);

			if (oldTest.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("Test Not Found"));
			}

			test.UpdatedDate = DateTimeOffset.Now;
			test.UpdatedBy = security.UserID;
			using (SqlCommand cmd = new SqlCommand())
			{
				test.UpdateSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			EntityToSiteMaps entityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(security, test.EntityType, test.IdentityGuid);

			if (test.SiteGuid != oldTest.SiteGuid)
			{
				// Purge from EntityToSiteMap
				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
					entityToSiteMaps.Purge(security, entityToSiteMap);

				// Create Entity to Site Map
				EntityToSiteMapClass newEntityToSiteMap = new EntityToSiteMapClass(test);
				entityToSiteMaps.Add(security, newEntityToSiteMap, GetType().GUID);
			}

			// Verify that new ID will not conflict with EntityToSiteMaps
			else
			{
				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					Guid siteGuid = security.SiteGuid;
					security.SiteGuid = entityToSiteMap.SiteGuid;
					Guid identityGuid = GetIdentityGuid(security, test.ID);
					security.SiteGuid = siteGuid;

					if (identityGuid != Guid.Empty
						&& identityGuid != entityToSiteMap.IdentityGuid)
						throw (new Exception("Test Exits"));
				}
			}


		}

		public bool IsAssociatedWithTestResult(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			TestClass test = new TestClass();
			test.IdentityGuid = identityGuid;

			if (identityGuid != Guid.Empty)
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					test.IsAssociatedWithTestResultSQL(cmd, security);
					DataSet set = ConsolidatedDA.GetDataSet(cmd, security);
					if (set == null)
					{
						throw new ArgumentNullException("set");
					}

					DataTable table = set.Tables[0];
					if (table.Rows.Count == 0)
					{
						return false;
					}

					DataRow row = table.Rows[0];

					return (int)row["CNT"] > 0;
				}
			}

			return false;
		}

		public TestClass Get(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			TestClass test = new TestClass();
			test.IdentityGuid = identityGuid;

			if (identityGuid != Guid.Empty)
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					test.SelectSQL(cmd, ContextUtil.IsInTransaction);
					test.Load(ConsolidatedDA.GetDataSet(cmd, security));
				}
			}

			return test;
		}

		public Guid GetIdentityGuid(SecurityClass security, string id)
		{
			TestClass test = null;

			if (security == null)
				throw new ArgumentNullException("Security");

			//   CheckSecurity(security);

			if (id == "{All}"
			|| id == "{Unassigned}"
			|| id == "{None}")
				return Guid.Empty;

			test = new TestClass();
			test.ID = id;
			using (SqlCommand cmd = new SqlCommand())
			{
				test.SelectByIDSQL(cmd, security, ContextUtil.IsInTransaction);
				test.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}
			return test.IdentityGuid;
		}

		private enum ValidateState
		{
			vsError,
			vsStart,
			vsGetFloat,
			vsGetNumber,
			vsGetValue,
			vsGetMaxFloat,
			vsGetMaxValue,
			vsGetRange,
			vsIsEllipsis,
			vsCheckForMatch
		};

		/// <summary>
		/// This method will check data against limit to see if it's valid. This method was
		/// converted from the original "CTest::mfnValidateResult()".
		/// </summary>
		/// <returns>
		/// true - successful, false - unsuccessful
		/// </returns>
		public bool ValidateTestResult(SecurityClass security, TestClass test, string data)
		{
			bool bfound = false;

			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (test == null)
			{
				throw new ArgumentNullException("Test");
			}

			if (data == null)
			{
				throw new ArgumentNullException("Data");
			}

			string validationrule = test.ValidationRule;
			char currentchar;
			ValidateState vscurrentstate = ValidateState.vsStart;
			string result = null;
			string minval = "";
			string maxval = "";

			int ruleposition = 0;
			int rulelength = validationrule.Length;

			while (rulelength > ruleposition &&
					!bfound &&
					ValidateState.vsError != vscurrentstate ||
					(rulelength == ruleposition && ValidateState.vsCheckForMatch == vscurrentstate) ||
					(rulelength == ruleposition && ValidateState.vsGetNumber == vscurrentstate) ||
					(rulelength == ruleposition && ValidateState.vsGetValue == vscurrentstate) ||
					(rulelength == ruleposition && ValidateState.vsGetFloat == vscurrentstate) ||
					(rulelength == ruleposition && ValidateState.vsGetMaxValue == vscurrentstate) ||
					(rulelength == ruleposition && ValidateState.vsGetMaxFloat == vscurrentstate))
			{
				switch (vscurrentstate)
				{
					case ValidateState.vsStart:
						currentchar = validationrule[ruleposition++];
						if ('*' == currentchar)
						{
							// Wild card value...pass by default
							bfound = true;
						}
						else if (('-' == currentchar) && char.IsDigit(validationrule[ruleposition]))
						{
							minval = "";
							maxval = "";
							result = "";
							result += currentchar;
							currentchar = validationrule[ruleposition++];
							result += currentchar;
							minval = result;
							vscurrentstate = ValidateState.vsGetNumber;
						}
						else if (('+' == currentchar) && char.IsDigit(validationrule[ruleposition]))
						{
							minval = "";
							maxval = "";
							result = "";
							currentchar = validationrule[ruleposition++];
							result += currentchar;
							minval = result;
							vscurrentstate = ValidateState.vsGetNumber;
						}
						else if (char.IsDigit(currentchar))
						{
							minval = "";
							maxval = "";
							result = "";
							result += currentchar;
							minval = result;
							vscurrentstate = ValidateState.vsGetNumber;
						}
						else if (char.IsLetter(currentchar))
						{
							minval = "";
							maxval = "";
							result = "";
							result += currentchar;
							minval = result;
							vscurrentstate = ValidateState.vsGetValue;
						}
						else if (' ' == currentchar)
						{
							//skip space
						}
						else //bad rule
						{
							vscurrentstate = ValidateState.vsError;
						}
						break;

					case ValidateState.vsGetNumber:
						if (rulelength == ruleposition)
						{
							vscurrentstate = ValidateState.vsCheckForMatch;
							break;
						}
						else
						{
							currentchar = validationrule[ruleposition++];
						}
						if (',' == currentchar)
						{
							vscurrentstate = ValidateState.vsCheckForMatch;
						}
						else if ('.' == currentchar)
						{
							vscurrentstate = ValidateState.vsIsEllipsis;
						}
						else if (char.IsDigit(currentchar))
						{
							result += currentchar;
							minval += currentchar;
							vscurrentstate = ValidateState.vsGetNumber;
						}
						else if (char.IsLetter(currentchar))
						{
							maxval = "";
							result += currentchar;
							minval += currentchar;
							vscurrentstate = ValidateState.vsGetValue;
						}
						else	// bad rule
						{
							vscurrentstate = ValidateState.vsError;
						}
						break;

					case ValidateState.vsGetValue:
						if (rulelength == ruleposition)
						{
							vscurrentstate = ValidateState.vsCheckForMatch;
							break;
						}
						else
						{
							currentchar = validationrule[ruleposition++];
						}
						if (',' == currentchar)
						{
							vscurrentstate = ValidateState.vsCheckForMatch;
						}
						else if ((char.IsLetter(currentchar)) || (char.IsDigit(currentchar)) || (' ' == currentchar))
						{
							result += currentchar;
							minval += currentchar;
							vscurrentstate = ValidateState.vsGetValue;
						}
						else	// bad rule
						{
							vscurrentstate = ValidateState.vsError;
						}
						break;

					case ValidateState.vsGetFloat:
						if (rulelength == ruleposition)
						{
							vscurrentstate = ValidateState.vsCheckForMatch;
							break;
						}
						else
						{
							currentchar = validationrule[ruleposition++];
						}
						if (',' == currentchar)
						{
							vscurrentstate = ValidateState.vsCheckForMatch;
						}
						else if ('.' == currentchar)
						{
							vscurrentstate = ValidateState.vsIsEllipsis;
						}
						else if (char.IsDigit(currentchar))
						{
							result += currentchar;
							minval += currentchar;
							vscurrentstate = ValidateState.vsGetFloat;
						}
						else	// bad rule
						{
							vscurrentstate = ValidateState.vsError;
						}
						break;

					case ValidateState.vsGetRange:
						currentchar = validationrule[ruleposition++];
						if (('-' == currentchar) && char.IsDigit(validationrule[ruleposition]))
						{
							result = maxval;
							result += currentchar;
							currentchar = validationrule[ruleposition++];
							result += currentchar;
							maxval = result;
							vscurrentstate = ValidateState.vsGetMaxValue;
						}
						else if (('+' == currentchar) && char.IsDigit(validationrule[ruleposition]))
						{
							result = maxval;
							currentchar = validationrule[ruleposition++];
							result += currentchar;
							maxval = result;
							vscurrentstate = ValidateState.vsGetMaxValue;
						}
						else if (char.IsDigit(currentchar))
						{
							result = maxval;
							result += currentchar;
							maxval = result;
							if (rulelength == ruleposition)
								vscurrentstate = ValidateState.vsCheckForMatch;
							else
								vscurrentstate = ValidateState.vsGetMaxValue;
						}
						else
						{
							vscurrentstate = ValidateState.vsError;
						}
						break;

					case ValidateState.vsGetMaxValue:
						if (rulelength == ruleposition)
						{
							vscurrentstate = ValidateState.vsCheckForMatch;
							break;
						}
						else
						{
							currentchar = validationrule[ruleposition++];
						}
						if (',' == currentchar)
						{
							vscurrentstate = ValidateState.vsCheckForMatch;
						}
						else if (('.' == currentchar) && char.IsDigit(validationrule[ruleposition]))
						{
							result += currentchar;
							maxval += currentchar;
							vscurrentstate = ValidateState.vsGetMaxFloat;
						}
						else if (char.IsDigit(currentchar))
						{
							result += currentchar;
							maxval += currentchar;
							vscurrentstate = ValidateState.vsGetMaxValue;
						}
						else	// bad rule
						{
							vscurrentstate = ValidateState.vsError;
						}
						break;

					case ValidateState.vsGetMaxFloat:
						if (rulelength == ruleposition)
						{
							vscurrentstate = ValidateState.vsCheckForMatch;
							break;
						}
						else
						{
							currentchar = validationrule[ruleposition++];
						}
						if (',' == currentchar)
						{
							vscurrentstate = ValidateState.vsCheckForMatch;
						}
						else if (char.IsDigit(currentchar))
						{
							result += currentchar;
							maxval += currentchar;
							vscurrentstate = ValidateState.vsGetMaxFloat;
						}
						else	// bad rule
						{
							vscurrentstate = ValidateState.vsError;
						}
						break;

					case ValidateState.vsIsEllipsis:
						currentchar = validationrule[ruleposition++];
						if ('.' == currentchar)
						{
							vscurrentstate = ValidateState.vsGetRange;
						}
						else if (char.IsDigit(currentchar))
						{
							result += '.';
							minval += '.';
							result += currentchar;
							minval += currentchar;

							vscurrentstate = ValidateState.vsGetFloat;
						}
						else	// bad rule
						{
							vscurrentstate = ValidateState.vsError;
						}
						break;

					case ValidateState.vsCheckForMatch:
						if (string.IsNullOrEmpty(maxval))		// there is not a range set
						{
							if (minval.ToUpper() == data.ToUpper())
							{
								bfound = true;
							}
							vscurrentstate = ValidateState.vsStart;
						}
						else
						{
							double dminval = System.Convert.ToDouble(minval);
							double dmaxval = System.Convert.ToDouble(maxval);
							double ddata;

							if (!string.IsNullOrEmpty(data) && double.TryParse(data, out ddata))
							{
								if ((ddata >= dminval) && (ddata <= dmaxval))
								{
									bfound = true;
								}
								else if ((ddata >= dmaxval) && (ddata <= dminval))
								{
									bfound = true;
								}
							}
							vscurrentstate = ValidateState.vsStart;
						}
						break;
				}
			}

			return bfound;
		}


		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid testGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			TestClass test = Get(security, testGuid);
			if (test.IdentityGuid == Guid.Empty)
			{
				return;
			}

			DependenciesClass dependencies = new DependenciesClass(security);
			dependencies.Purge(security, test);

			// Delete Entity to Site Map
			var entityToSiteMaps = new EntityToSiteMaps();
			var entityToSiteMapClass = new EntityToSiteMapClass(test.ID, ENTITY_TYPE.TEST, test.SiteGuid, test.IdentityGuid);
			entityToSiteMaps.Purge(security, entityToSiteMapClass);

			using (SqlCommand cmd = new SqlCommand())
			{
				test.PurgeSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

		}


		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (Object == null)
				throw new ArgumentNullException("Object");

		}

		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (Object == null)
				throw new ArgumentNullException("Object");

		}

		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (Object == null)
				throw new ArgumentNullException("Object");

		}

		public TestCollectionClass Enumerate(SecurityClass security, string filter, string order)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (!security.HasRight(RIGHT.VIEW_TEST_ITEMS)
				&& !security.HasRight(RIGHT.MODIFY_TEST_ITEMS)
				&& !security.HasRight(RIGHT.VIEW_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS))
			{
				throw new FMInsufficientRightsException();
			}

			TestClass test = new TestClass();
			using (SqlCommand cmd = new SqlCommand())
			{
				test.EnumerateSQL(cmd, security, filter, order);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				TestCollectionClass testCollection = new TestCollectionClass();
				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					test = new TestClass();
					test.Load(set);
					testCollection.Add(test);
					table.Rows.RemoveAt(0);
				}

				return testCollection;
			}
		}

		public TestCollectionClass EnumerateByTestSetGuid(SecurityClass security, Guid testSetGuid)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (!security.HasRight(RIGHT.VIEW_TEST_ITEMS)
				&& !security.HasRight(RIGHT.MODIFY_TEST_ITEMS)
				&& !security.HasRight(RIGHT.VIEW_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS))
			{
				throw new FMInsufficientRightsException();
			}

			TestClass test = new TestClass();
			using (SqlCommand cmd = new SqlCommand())
			{
				test.EnumerateByTestSetGuidSQL(cmd, security, testSetGuid);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				TestCollectionClass testCollection = new TestCollectionClass();
				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					test = new TestClass();
					test.Load(set);
					testCollection.Add(test);
					table.Rows.RemoveAt(0);
				}

				return testCollection;
			}
		}

	}
}
