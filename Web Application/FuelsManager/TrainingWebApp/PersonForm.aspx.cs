/******************************************************************************
	FILE NAME:		PersonForm.aspx.cs
	PURPOSE:		Implementation of PersonForm

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.
		
	AUTHOR(S):
	VERSION:	7.4.0.0  Current version

	MODIFICATION HISTORY:
		Date:			By:					Reason:
		----------	-----------------	-------------------------------------------

*******************************************************************************/

namespace FuelsManager.TrainingWebApp
{
	using System;
	using System.Collections;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMWebApp;

	/// <summary>
	/// Summary description for PersonForm.
	/// </summary>
	public partial class PersonForm : FMAutoSubmitFormBase
	{
		private const string TrainingSummarySelectedPerson = "TrainingSummarySelectedPerson";

		public PersonClass Person = null;

		protected void Page_Load ( object sender, EventArgs e )
		{
			try
			{
				this.Session.Remove ( "Status" );

				this.GetSecurity ( );

				var personArrayList = this.Session[TrainingSummarySelectedPerson] as ArrayList;
				if (personArrayList == null)
				{
					throw new Exception ( "PersonArrayList not in session" );
				}

				this.Person = personArrayList[personArrayList.Count - 1] as PersonClass;

				if (this.Person == null)
				{
					throw new Exception("Person not in session");
				}

				if (!this.Page.IsPostBack)
				{
					if ((this.Security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
					     || this.Security.HasRight(RIGHT.MODIFY_PERSON_QUALIFICATIONS)
					     || this.Security.HasRight(RIGHT.MODIFY_PERSON_TRAINING))
					    && (this.Security.SiteGuid == this.Person.SiteGuid || this.Person.SiteGuid == Guid.Empty))
					{
						this.OK.Enabled = true;
					}
					else
					{
						this.OK.Enabled = false;
					}

					// set the header text
					this.MainHeaderLabel.Text += "-" + this.Person.LastName + "," + this.Person.FirstName + "," + this.Person.MiddleName;
				}

				if (this.Security.HasRight(RIGHT.VIEW_TRAINING_QUALIFICATIONS) ||
					this.Security.HasRight(RIGHT.MODIFY_PERSON_QUALIFICATIONS) ||
					this.Security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA))
				{
					this.tpQualificationsPage.HeaderText = this.GetTranslatedText("Qualifications");
				}
				else
				{
					this.tpQualificationsPage.Visible = false;
				}

				if (this.Security.HasRight(RIGHT.VIEW_TRAINING_QUALIFICATIONS) ||
					this.Security.HasRight(RIGHT.MODIFY_PERSON_TRAINING) ||
					this.Security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA))
				{
					this.tpTrainingPage.HeaderText = this.GetTranslatedText("Training");
				}
				else
				{
					this.tpTrainingPage.Visible = false;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler ( except );
				this.Response.End ( );
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit ( EventArgs e )
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent ( );
			base.OnInit ( e );
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent ( )
		{
			this.OK.Command += new System.Web.UI.WebControls.CommandEventHandler ( this.OkCommand );
			this.Cancel.Command += new System.Web.UI.WebControls.CommandEventHandler ( this.CancelCommand );

		}
		#endregion

		public void UpdateData ( )
		{
		}

		private void TransferToOriginatingForm ( )
		{
			var personArrayList = this.Session[TrainingSummarySelectedPerson] as ArrayList;

			if (personArrayList == null)
			{
				throw new Exception("PersonArrayList not in session");
			}

			personArrayList.RemoveAt ( personArrayList.Count - 1 );

			if (personArrayList.Count == 0)
			{
				this.Session.Remove ( TrainingSummarySelectedPerson );
			}

			if (this.Session["PersonSelectContextArrayList"] == null)
			{
				this.Redirect("TrainingSummary.aspx");
			}
			else
			{
				var personSelectContextArrayList = this.Session["PersonSelectContextArrayList"] as ArrayList;

				if (personSelectContextArrayList == null)
				{
					throw new Exception("PersonSelectContextArrayList not in session");
				}

				var personSelectContext = personSelectContextArrayList[personSelectContextArrayList.Count - 1] as PersonSelectContextClass;

				if (personSelectContext == null)
				{
					throw new Exception("PersonSelectContext not in session");
				}

				personSelectContextArrayList.RemoveAt ( personSelectContextArrayList.Count - 1 );

				if (personSelectContextArrayList.Count == 0)
				{
					this.Session.Remove ( "PersonSelectContextArrayList" );
				}

				string transferString = "PersonSelectForm.aspx?";

				if (personSelectContext.Role != PERSON_ROLE.MAX_PERSON_ROLE)
				{
					transferString += "Role=" + personSelectContext.Role.ToString ( ) + "&";
				}

				transferString += "Unassigned=" + personSelectContext.Unassigned.ToString ( ) + "&";

				if (personSelectContext.IDCarrierLink != null)
				{
					transferString += "IDCarrierLink=" + personSelectContext.IDCarrierLink + "&";
				}

				if (personSelectContext.Mode != null)
				{
					transferString += "Mode=" + personSelectContext.Mode + "&";
				}

				if (personSelectContext.SearchString != null)
				{
					transferString += "SearchString=" + personSelectContext.SearchString + "&";
				}

				this.Redirect(transferString);
			}
		}

		private void OkCommand ( object sender, System.Web.UI.WebControls.CommandEventArgs e )
		{
			try
			{
				if (this.Session["Status"] != null && (string) this.Session["Status"] == "Error")
				{
					return;
				}

				this.UpdateData ( );

				FMChannelHelper.MakeCall<IPersonnel>(
					personnel =>
						{
							if (!this.Person.IdentityGuid.IsEmpty())
							{
								personnel.Modify(this.Security, DATA_TYPE.CONFIG, this.Person);
							}
							else
							{
								personnel.Add(this.Security, this.Person);
							}
						});
			}
			catch (Exception except)
			{
				this.ErrorHandler ( except );
				return;
			}

			this.TransferToOriginatingForm ( );
		}

		private void CancelCommand ( object sender, System.Web.UI.WebControls.CommandEventArgs e )
		{
			this.TransferToOriginatingForm ( );
		}

		/// <summary>
		/// This method will either enable or disable controls.  It is called by
		/// the individual tabs associated to the person form.
		/// </summary>
		/// <param name="enable"></param>
		public void EnableControls ( bool enable )
		{
			var personArrayList = this.Session[TrainingSummarySelectedPerson] as ArrayList;

			if (personArrayList == null)
			{
				throw new Exception("PersonArrayList not in session");
			}

			var person = personArrayList[personArrayList.Count - 1] as PersonClass;

			if (person == null)
			{
				throw new Exception("Person not in session");
			}

			if (( this.Security.HasRight ( RIGHT.MODIFY_PERSONNEL_DATA ) ||
				this.Security.HasRight ( RIGHT.MODIFY_PERSON_QUALIFICATIONS ) ||
				this.Security.HasRight ( RIGHT.MODIFY_PERSON_TRAINING ) )
				&& (this.Security.SiteGuid == person.SiteGuid || person.SiteGuid == Guid.Empty))
			{
				this.OK.Enabled = enable;
			}

			this.Cancel.Enabled = enable;

			this.tcPersonTabs.HeaderEnabled = enable;
		}
	}

	public class PersonPageBase : FMUserControlBase
	{
		protected PersonClass Person
		{
			get
			{
				return ( (PersonForm) this.Page ).Person;
			}
		}

	}

}
