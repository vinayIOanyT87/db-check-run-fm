// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestSetResultForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.QualityControlWebApp
{
	using System;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;
    using FMCore;
	using FMWebApp;

	public partial class TestSetResultForm : FMAutoSubmitFormBase
	{
		#region Constants and Fields

		public TestSetEquipmentResultClass TestSetEquipmentResult = null;
		public TestSetTankResultClass TestSetTankResult = null;

		#endregion

		#region Public Methods and Operators

		public void ResetMainSummaryFilters()
		{
			this.TestSetResultGeneralPage.ResetMainSummaryFilters();
		}

		public bool UpdateData()
		{
			return this.TestSetResultGeneralPage.UpdateData();
		}

		#endregion

		#region Methods

		virtual protected void CancelCommand(object sender, CommandEventArgs e)
		{
			try
			{
				this.ResetMainSummaryFilters();
				this.Session.Remove(TestResults.TestsetResultGuid);
				this.Session.Remove(TestResults.TestsetResultAssetType);

				this.TransferBacktoCallingForm();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		virtual protected void OkCommand(object sender, CommandEventArgs e)
		{

            try
            {
                this.ResetMainSummaryFilters();
                if (this.UpdateData())
                {
                    this.Save();
                }
            }
            catch (Exception except)
            {
                ErrorHandler(except);
            }
        }


        protected void Save()
        {

			try
			{
				var assettype = (string)this.Session[TestResults.TestsetResultAssetType];

				if (assettype == this.GetTranslatedText("Tank"))
				{
					if (this.TestSetTankResult.IdentityGuid != Guid.Empty)
					{
						FMChannelHelper.MakeCall<ITestSetTankResults>(tResults => tResults.Modify(this.Security, this.TestSetTankResult));
					}
					else
					{
						this.TestSetTankResult.IdentityGuid = FMChannelHelper.MakeCall<ITestSetTankResults, Guid>(
							eResults => eResults.Add(this.Security, this.TestSetTankResult));
					}
					
					// call the scheduler to update the qc due date (IGO 11-Sep-2009)
					if (TESTSET_STATUS.Passed == this.TestSetTankResult.Status)
					{
						this.UpdateAppointment(this.TestSetTankResult.ResultTimeStamp);

						Guid testSetGuid =
							FMChannelHelper.MakeCall<ITestSets, Guid>(
								sets => sets.GetIdentityGuid(this.Security, this.TestSetTankResult.TestSetName));

						DateTimeOffset nextqcdate =
							FMChannelHelper.MakeCall<IAppointments, DateTimeOffset>(
								x =>
								x.GetNextQCDate(
									this.Security,
									this.TestSetTankResult.TankGuid,
                                    testSetGuid,
									this.GetTranslatedText("Tanks"),
									this.TestSetTankResult.ResultTimeStamp));

						TankClass tank = null;
							
						FMChannelHelper.MakeCall<ITanks>(
							tanks =>
								{
									tank = tanks.Get(this.Security, this.TestSetTankResult.TankGuid);
									tanks.Modify(this.Security, tank);// TODO: check to see if tank needs a value saved too
								});

						//get the next qc date for this particular test set
						DateTimeOffset nextTestSetQcDate = 
							FMChannelHelper.MakeCall<IAppointments, DateTimeOffset>(
								x =>
								x.GetNextQCDate(
									this.Security,
									this.TestSetTankResult.TankGuid,
									testSetGuid,
									this.GetTranslatedText("Tanks"),
									this.TestSetTankResult.ResultTimeStamp
									));

						// Update the appointment if there is one
						if (this.UpdateAppointment(nextTestSetQcDate) == false)
						{
							FMChannelHelper.MakeCall<IAppointments>(
								x =>
								x.UpdateAppointmentBasedOnDueDate(
									this.Security, testSetGuid, this.TestSetTankResult.TankGuid, false, nextTestSetQcDate));
						}

                        string alertText = string.Format("<script>alert('The next QC Due Date for {0} is {1}.');</script>",
                           tank.ID, nextqcdate.ToString("D"));

                        Response.Write(alertText);
                    }
				}
				else if (assettype == this.GetTranslatedText("Equipment"))
				{
					if (this.TestSetEquipmentResult.IdentityGuid != Guid.Empty)
					{
						FMChannelHelper.MakeCall<ITestSetEquipmentResults>(
							eResults => eResults.Modify(this.Security, this.TestSetEquipmentResult));
					}
					else
					{
                        this.TestSetEquipmentResult.IdentityGuid = FMChannelHelper.MakeCall<ITestSetEquipmentResults, Guid>(
							eResults => eResults.Add(this.Security, this.TestSetEquipmentResult));
					}

					// call the scheduler to update the qc due date (IGO 11-Sep-2009)
					if (TESTSET_STATUS.Passed == this.TestSetEquipmentResult.Status)
					{
						this.UpdateAppointment(this.TestSetEquipmentResult.ResultTimeStamp);

						Guid testSetGuid =
							FMChannelHelper.MakeCall<ITestSets, Guid>(
								sets => sets.GetIdentityGuid(this.Security, this.TestSetEquipmentResult.TestSetName));

						DateTimeOffset nextqcdate =
							FMChannelHelper.MakeCall<IAppointments, DateTimeOffset>(
								x =>
								x.GetNextQCDate(
									this.Security,
									this.TestSetEquipmentResult.EquipmentGuid,
                                    testSetGuid,
									this.GetTranslatedText("Equipment"),
									this.TestSetEquipmentResult.ResultTimeStamp));

						EquipmentClass equipment = null;
						FMChannelHelper.MakeCall<IEquipments>(
							equipments =>
								{
									equipment = equipments.Get(this.Security, this.TestSetEquipmentResult.EquipmentGuid);
									equipment._QCDate.Value = nextqcdate; //overall next qc date
									equipments.Modify(this.Security, equipment);
								});

						DateTimeOffset nextTestSetQcDate =
							FMChannelHelper.MakeCall<IAppointments, DateTimeOffset>(
								x =>
								x.GetNextQCDate(
									this.Security,
									this.TestSetEquipmentResult.EquipmentGuid,
									testSetGuid,
									this.GetTranslatedText("Equipment"),
									this.TestSetEquipmentResult.ResultTimeStamp));

						// Update the appointment if there is one
						if (this.UpdateAppointment(nextTestSetQcDate) == false)
						{
							FMChannelHelper.MakeCall<IAppointments>(
								x => x.UpdateAppointmentBasedOnDueDate(
									this.Security, testSetGuid, this.TestSetEquipmentResult.EquipmentGuid, true, nextTestSetQcDate));
						}

                        string alertText = string.Format("<script>alert('The next QC Due Date for {0} is {1}.');</script>",
                           equipment.ID, nextqcdate.ToString("D"));

                        Response.Write(alertText);

                    }
				}

				this.Session.Remove(TestResults.TestsetResultGuid);
				this.Session.Remove(TestResults.TestsetResultAssetType);

				this.TransferBacktoCallingForm();

			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		virtual protected void Page_Init(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (this.IsPostBack == false)
				{
					if (this.IsFromQueryWriter)
					{
						Guid entityGuid = Guid.Parse(this.Request.GetQueryOrFormValue("QUERYEDIT").Substring(1));
						char entityType = this.Request.GetQueryOrFormValue("QUERYEDIT")[0];

						if (entityType == 'E')
						{
							this.TestSetEquipmentResult = FMChannelHelper.MakeCall<ITestSetEquipmentResults, TestSetEquipmentResultClass>( x => x.Get( this.Security, entityGuid ) );
							this.Session[TestResults.TestSetResultsObject] = this.TestSetEquipmentResult;
							this.Session[TestResults.TestsetResultAssetType] = "Equipment";
							this.Session[TestResults.TestsetResultGuid] = this.TestSetEquipmentResult.IdentityGuid;
						}
						else
						{
							this.TestSetTankResult = FMChannelHelper.MakeCall<ITestSetTankResults, TestSetTankResultClass>( x => x.Get( this.Security, entityGuid ) );
							this.Session[TestResults.TestSetResultsObject] = this.TestSetTankResult;
							this.Session[TestResults.TestsetResultAssetType] = "Tank";
							this.Session[TestResults.TestsetResultGuid] = this.TestSetTankResult.IdentityGuid;
						}
					}

					if (this.Request.GetQueryOrFormValue("MODE").DefaultIfNull(string.Empty).Equals("ADD"))
					{
						this.Session[TestResults.TestSetResultsObject] = this.InitTestOptions();
					}
				}

				this.TestSetTankResult = this.Session[TestResults.TestSetResultsObject] as TestSetTankResultClass;
				if (this.TestSetTankResult == null)
				{
					this.TestSetEquipmentResult = this.Session[TestResults.TestSetResultsObject] as TestSetEquipmentResultClass;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		virtual protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (!this.Page.IsPostBack)
				{
					// Always disable OK button if user doesn't have have the correct rights. This fixes bug #6599.
					if (!this.Security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS) && !this.Security.HasRight(RIGHT.MODIFY_QUALITY_TESTS))
					{
						this.OK.Enabled = false;
					}

					// Check session for test set result to load
					if ((null != this.Session[TestResults.TestsetResultGuid]) && (null != this.Session[TestResults.TestsetResultAssetType]))
					{
						var selectedTestSetResultGuid = (Guid)this.Session[TestResults.TestsetResultGuid];

						if (selectedTestSetResultGuid != Guid.Empty)
						{
							var assetType = (string)this.Session[TestResults.TestsetResultAssetType];

							if (assetType == this.GetTranslatedText("Tank"))
							{
								this.TestSetTankResult =
									FMChannelHelper.MakeCall<ITestSetTankResults, TestSetTankResultClass>(
										x => x.Get(this.Security, selectedTestSetResultGuid));

								// Disable OK button if result not in pending state and user doesn't have modify rights. This fixes bug #6599.
								if (TESTSET_STATUS.Pending != this.TestSetTankResult.Status)
								{
									if (!this.Security.HasRight(RIGHT.MODIFY_QUALITY_TESTS))
									{
										this.OK.Enabled = false;
									}
								}
							}
							else if (assetType == this.GetTranslatedText("Equipment"))
							{
								this.TestSetEquipmentResult =
									FMChannelHelper.MakeCall<ITestSetEquipmentResults, TestSetEquipmentResultClass>(
										x => x.Get(this.Security, selectedTestSetResultGuid));

								// Disable OK button if result not in pending state and user doesn't have modify rights. This fixes bug #6599.
								if (TESTSET_STATUS.Pending != this.TestSetEquipmentResult.Status)
								{
									if (!this.Security.HasRight(RIGHT.MODIFY_QUALITY_TESTS))
									{
										this.OK.Enabled = false;
									}
								}
							}
						}
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.Response.End();
			}
		}

		protected virtual void Page_PreRenderComplete(object sender, EventArgs e)
		{
			try
			{
				if (!this.Page.IsPostBack)
				{
					// Once Test Set Results have been loaded remove from session.  If user navigates back to this page via
					// the main menu these Test Set Results should not be loaded.  Empty Test Set Results should be created.
					this.Session.Remove(TestResults.TestsetResultGuid);
					this.Session.Remove(TestResults.TestsetResultAssetType);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private object InitTestOptions()
		{
			TestSetClass testSet = null;
			Guid entityGuid = Guid.Empty;

			if (string.IsNullOrEmpty(this.Request.GetQueryOrFormValue("ENTITY")) == false)
			{
				entityGuid = Guid.Parse(this.Request.GetQueryOrFormValue("ENTITY"));
			}

			if (string.IsNullOrEmpty(this.Request.GetQueryOrFormValue("TEST")) == false)
			{
				Guid testSetGuid = Guid.Parse(this.Request.GetQueryOrFormValue("TEST"));
				testSet = FMChannelHelper.MakeCall<ITestSets, TestSetClass>(sets => sets.Get(this.Security, testSetGuid));
			}

			if (string.IsNullOrEmpty(this.Request.GetQueryOrFormValue("ASSETTYPE")) == false)
			{
				SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(sites => sites.Get(this.Security, this.Security.SiteGuid, false, false, false));

				DateTimeOffset siteTimeNow = TimeConverter.Now(site);

				this.Session[TestResults.TestsetResultGuid] = Guid.Empty;
				this.Session[TestResults.TestsetResultAssetType] = this.Request.GetQueryOrFormValue("ASSETTYPE");

				if (this.Request.GetQueryOrFormValue("ASSETTYPE").Equals("Equipment", StringComparison.OrdinalIgnoreCase))
				{
					EquipmentClass equipment =
						FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(x => x.Get(this.Security, entityGuid));

					var test = new TestSetEquipmentResultClass { SiteGuid = this.Security.SiteGuid };
					if (testSet != null)
					{
						test.TestSetName = testSet.ID;
					}

					test.EquipmentGuid = entityGuid;
					test.EquipmentID = equipment.ID;
					test.ResultTimeStamp = siteTimeNow;

					return test;
				}
				else
				{
					TankClass tank = FMChannelHelper.MakeCall<ITanks, TankClass>(tanks => tanks.Get(this.Security, entityGuid));

					var test = new TestSetTankResultClass { SiteGuid = this.Security.SiteGuid };

					if (testSet != null)
					{
						test.TestSetName = testSet.ID;
					}

					test.TankGuid = entityGuid;
					test.TankID = tank.ID;
					test.ResultTimeStamp = siteTimeNow;

					return test;
				}
			}

			return new TestSetEquipmentResultClass();
		}

		virtual protected void TransferBacktoCallingForm()
		{
			if (this.IsFromQueryWriter)
			{
				this.Redirect("..\\QueryWriterWebApp\\QueryResultsForm.aspx?Mode=Returning");
			}
			else if (string.IsNullOrEmpty(this.Request.GetQueryOrFormValue("TEST")))
			{
				this.Response.Write(string.Format("<script>window.location='TestResults.aspx?{0}';</script>", this.Security.CSRFTokenWithParamName));
			}
			else
			{
				this.Response.Write(string.Format("<script>window.location='../AppointmentWebApp/AppointmentSummary.aspx?MODE=GETTEST&{0}';</script>", this.Security.CSRFTokenWithParamName));
			}
		}

		private bool UpdateAppointment(DateTimeOffset newStartDate)
		{
			if (string.IsNullOrEmpty(this.Request.GetQueryOrFormValue("APPOINTMENT")) == false)
			{
				Guid appointmentGuid = Guid.Parse(this.Request.GetQueryOrFormValue("APPOINTMENT"));

				AppointmentClass appointment = null;

				FMChannelHelper.MakeCall<IAppointments>(
					appointments =>
						{
							appointment = appointments.Get(this.Security, appointmentGuid);

							if (appointment.IdentityGuid != Guid.Empty)
							{
								appointment.StartDate = newStartDate.ToString();
								appointments.Modify(this.Security, appointment);
							}
						});

				return appointment.IdentityGuid != Guid.Empty;
			}

			return false;
		}

		#endregion
	}

	public class TestSetResultPageBase : FMUserControlBase
	{
		#region Properties

		protected FMButton OkButton
		{
			get
			{
				return ((TestSetResultForm)this.Page).OK;
			}
		}

		protected TestSetEquipmentResultClass TestSetEquipmentResult
		{
			get
			{
				return ((TestSetResultForm)this.Page).TestSetEquipmentResult;
			}

			set
			{
				((TestSetResultForm)this.Page).TestSetEquipmentResult = value;
				this.Session[TestResults.TestSetResultsObject] = value;
			}
		}

		protected TestSetTankResultClass TestSetTankResult
		{
			get
			{
				return ((TestSetResultForm)this.Page).TestSetTankResult;
			}

			set
			{
				((TestSetResultForm)this.Page).TestSetTankResult = value;
				this.Session[TestResults.TestSetResultsObject] = value;
			}
		}

		#endregion
	}
}