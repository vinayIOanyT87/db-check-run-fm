// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonSelectForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the PersonSelectForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Web;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

    using FMCore;

	/// <summary>
	///    Summary description for PersonSelectForm.
	/// </summary>
	public partial class PersonSelectForm : FMAutoSubmitFormBase
	{
		#region Constants and Fields
		protected PersonSelectContextClass PersonSelectContext;
		protected string SelectThisItemText;

		private int limit = -1;
		#endregion

		#region Methods
		protected void FindAllBtn_OnClick(object sender, EventArgs e)
		{
			this.PersonSelectContext.SearchString = null;
			this.FindTextBox.Text = string.Empty;
			this.UpdateView();
		}

		protected void FindBtn_OnClick(object sender, EventArgs e)
		{
			if (this.FindTextBox.Text.Length < 1)
			{
				this.PersonSelectContext.SearchString = null;
			}
			else
			{
				this.PersonSelectContext.SearchString = this.FindTextBox.Text.ToUpper();
			}

			this.UpdateView();
		}

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				this.SelectThisItemText = this.GetTranslatedText("Select this item");

				if (this.Page.IsPostBack == false)
				{
					this.PersonSelectContext = new PersonSelectContextClass();

					if (this.Request.GetQueryOrFormValue("Role") != null)
					{
						this.PersonSelectContext.Role = (PERSON_ROLE)Enum.Parse(typeof(PERSON_ROLE), this.Request.GetQueryOrFormValue("Role"));
					}

					if ( this.Request.GetQueryOrFormValue( "Null" ) != null )
					{
						this.PersonSelectContext.Null = Convert.ToBoolean( this.Request.GetQueryOrFormValue( "Null" ) );
					}

					if (this.Request.GetQueryOrFormValue("Unassigned") != null)
					{
						this.PersonSelectContext.Unassigned = Convert.ToBoolean(this.Request.GetQueryOrFormValue("Unassigned"));
					}

					if (this.Request.GetQueryOrFormValue("Mode") != null)
					{
						this.PersonSelectContext.Mode = this.Request.GetQueryOrFormValue("Mode");
					}

					if (!this.Security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA) || this.PersonSelectContext.Mode == "Unassign")
					{
						this.AddButton1.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					// Checks for the carrier ID link. This will be used to filter the
					// personnel data.
					if (this.Request.GetQueryOrFormValue("IDCarrierLink") != null)
					{
						this.PersonSelectContext.IDCarrierLink = this.Request.GetQueryOrFormValue("IDCarrierLink");
					}

					if (this.Request.GetQueryOrFormValue("SearchString") != null)
					{
						this.PersonSelectContext.SearchString = this.Request.GetQueryOrFormValue("SearchString");
						this.FindTextBox.Text = this.PersonSelectContext.SearchString;
					}

					if (this.Request.GetQueryOrFormValue("ExcludeGuid") != null)
					{
                        this.PersonSelectContext.ExcludeGuid = new Guid(this.Request.GetQueryOrFormValue("ExcludeGuid"));
					}

                    if (this.Request.GetQueryOrFormValue("HideHidden") != null)
                    {
                        this.PersonSelectContext.HideHidden = Convert.ToBoolean(this.Request.GetQueryOrFormValue("HideHidden"));
                    }

					this.Session["PersonSelectContext"] = this.PersonSelectContext;

					this.UpdateView();
				}
				else
				{
					this.PersonSelectContext = this.Session["PersonSelectContext"] as PersonSelectContextClass;
				}

				var personSelectContextClass = this.PersonSelectContext;

				if (personSelectContextClass?.Mode != null)
				{
					var form1 = (HtmlForm)this.FindControl("Form1");
					var okButton = new HtmlInputButton();

					okButton.Attributes.Add("value", this.GetTranslatedText("OK"));
					okButton.Attributes.Add("id", "OkButton");
					okButton.Attributes.Add("class", "formfieldtitle");
					okButton.Attributes.Add("onclick", "MultipleSelect()");
					okButton.Attributes.Add("style", "width:66px;Z-INDEX: 107; LEFT: 662px; POSITION: absolute; TOP: 8px");
					form1.Controls.Add(okButton);

					var cancelButton = new HtmlInputButton();
					cancelButton.Attributes.Add("value", this.GetTranslatedText("Cancel"));
					cancelButton.Attributes.Add("id", "CancelButton");
					cancelButton.Attributes.Add("class", "formfieldtitle");
					cancelButton.Attributes.Add("onclick", "NoSelect()");
					cancelButton.Attributes.Add("style", "width:66px;Z-INDEX: 107; LEFT: 758px; POSITION: absolute; TOP: 8px");
					form1.Controls.Add(cancelButton);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddButtonCommand(object sender, CommandEventArgs e)
		{
			var personArrayList = this.Session["PersonArrayList"] as ArrayList;

			if (personArrayList == null)
			{
				personArrayList = new ArrayList();
				this.Session["PersonArrayList"] = personArrayList;
			}

			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.GetByMemberAndProcessVariables(this.Security, this.Security.SiteGuid, false, false)
																);
			var person = new PersonClass(site);
			personArrayList.Add(person);
			var personSelectContextArrayList = this.Session["PersonSelectContextArrayList"] as ArrayList;

			if (personSelectContextArrayList == null)
			{
				personSelectContextArrayList = new ArrayList { this.Session["PersonSelectContext"] };
				this.Session["PersonSelectContextArrayList"] = personSelectContextArrayList;
			}
			else
			{
				personSelectContextArrayList.Add(this.Session["PersonSelectContext"]);
			}

			this.Redirect("PersonForm.aspx");
		}

		private bool CheckIfPersonInCollection(PersonCollectionClass personnelCollection, PersonClass person)
		{
			foreach (PersonClass checkPerson in personnelCollection)
			{
				if (checkPerson.ID == person.ID)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		///    This method will filter the personnel collection to only the persons
		///    associated to the carrier. It will return an empty collection if the
		///    carrier ID is present and there are no assoicated personnel.
		/// </summary>
		/// <param name="operatorCollection"></param>
		private void FilterByCarrierID(PersonCollectionClass operatorCollection)
		{
			if (this.PersonSelectContext.Unassigned)
			{
				var person = new PersonClass { ID = HttpUtility.HtmlEncode(this.GetTranslatedText("{Unassigned}")) };
				operatorCollection.Add(person);
			}
			else
			{
				var person = new PersonClass { IdentityGuid = Guid.Empty, ID = "", FirstName = "", MiddleName = "", LastName = "" };
				operatorCollection.Add(person);
			}

		    string carrierID = this.PersonSelectContext.IDCarrierLink;

		    if (!string.IsNullOrEmpty(carrierID))
		    {
		        Guid carrierGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(
		            x =>
		                x.GetIdentityGuid(this.Security, carrierID)
		            );

		        CompanyClass carrier = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
		            x =>
		                x.Get(this.Security, carrierGuid)
		            );
		        CompanyMapCollectionClass assignedPersonCollection = carrier?.AssignedPersonnelCollection;

		        if (assignedPersonCollection != null && (assignedPersonCollection.Count > 0))
		        {
		            foreach (CompanyMapClass assignedPerson in assignedPersonCollection)
		            {
		                bool bAdd = false;
		                if (string.IsNullOrEmpty(this.PersonSelectContext.SearchString))
		                {
		                    bAdd = true;
		                }
		                else
		                {
		                    string searchString = this.PersonSelectContext.SearchString.ToUpper();
		                    int index1 = assignedPerson.AssignedToID.ToUpper().IndexOf(searchString, StringComparison.Ordinal);
		                    int index2 = assignedPerson.AssignedToFirstName.ToUpper().IndexOf(searchString, StringComparison.Ordinal);
		                    int index3 = assignedPerson.AssignedToMiddleName.ToUpper().IndexOf(searchString, StringComparison.Ordinal);
		                    int index4 = assignedPerson.AssignedToLastName.ToUpper().IndexOf(searchString, StringComparison.Ordinal);

		                    if ((index1 > -1) || (index2 > -1) || (index3 > -1) || (index4 > -1))                                   
		                    {
		                        bAdd = true;
		                    }
		                }
		                if(bAdd)
		                {
		                    var person = new PersonClass
		                                 {
		                                     IdentityGuid = assignedPerson.AssignedToGuid,
		                                     ID = assignedPerson.AssignedToID,
		                                     FirstName = assignedPerson.AssignedToFirstName,
		                                     MiddleName = assignedPerson.AssignedToMiddleName,
		                                     LastName = assignedPerson.AssignedToLastName
		                                 };
		                    operatorCollection.Add(person);
		                }
		            }
		        }
		    }
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton1.Command				+= this.AddButtonCommand;
			this.PersonDataGrid.EditCommand		+= this.PersonDataGridEditCommand;
			this.PersonDataGrid.DeleteCommand	+= this.PersonDataGridDeleteCommand;
			this.PersonDataGrid.ItemDataBound	+= this.PersonDataGridItemDataBound;
			this.AddButton2.Command				+= this.AddButtonCommand;
		}

		private void PersonDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Get IdentityGuid
				TableCell identityGuidCell = e.Item.Cells[3];//bds

				FMChannelHelper.MakeCall<IPersonnel>(x =>  x.Purge(this.Security, Guid.Parse(identityGuidCell.Text)));
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void PersonDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				TableCell identityGuidCell = e.Item.Cells[3];//bds
				var personArrayList = this.Session["PersonArrayList"] as ArrayList;

				if (personArrayList == null)
				{
					personArrayList = new ArrayList();
					this.Session["PersonArrayList"] = personArrayList;
				}

				// Get Person
				PersonClass person = FMChannelHelper.MakeCall<IPersonnel, PersonClass>(
																	 x =>
																	 x.Get(this.Security, Guid.Parse(identityGuidCell.Text))
																);
				personArrayList.Add(person);

				var personSelectContextArrayList = this.Session["PersonSelectContextArrayList"] as ArrayList;

				if (personSelectContextArrayList == null)
				{
					personSelectContextArrayList = new ArrayList { this.Session["PersonSelectContext"] };
					this.Session["PersonSelectContextArrayList"] = personSelectContextArrayList;
				}
				else
				{
					personSelectContextArrayList.Add(this.Session["PersonSelectContext"]);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Redirect("PersonForm.aspx");
		}

		private void PersonDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex == -1)
			{
				if (e.Item.ItemType == ListItemType.Header)
				{
				    e.Item.Cells[0].Text = this.GetTranslatedText(this.PersonSelectContext.Mode ?? "Select");

					if (this.PersonDataGrid.Columns.Count > 0)
						this.PersonDataGrid.Columns[0].HeaderText = e.Item.Cells[0].Text;
				}
			}

			else
			{
				if (this.PersonSelectContext.Mode != null)
				{
					var select = new HtmlInputCheckBox();
					select.ID = "Select";
					e.Item.Cells[0].Controls.Add(select);

					e.Item.Cells[4].Text = e.Item.Cells[4].Text.Replace(" ", "&nbsp;");//bds
				}
				else
				{
					string id = string.Empty;

					// Leave hard space zero length string
					if (e.Item.Cells[4].Text != "&nbsp;")//bds
					{
						id = HttpUtility.HtmlDecode(e.Item.Cells[4].Text);//bds
					}

					string toolTip = ((e.Item.Cells[5].Text != "&nbsp;") ? e.Item.Cells[5].Text : string.Empty)//bds
									 + ((e.Item.Cells[6].Text != "&nbsp;") ? " " + e.Item.Cells[6].Text : string.Empty)//bds
									 + ((e.Item.Cells[7].Text != "&nbsp;") ? " " + e.Item.Cells[7].Text : string.Empty);//bds

					string cardNumber = e.Item.Cells[8].Text;//bds

					var select = new HtmlAnchor();
					select.ID = "Select";
                    select.HRef = HttpUtility.HtmlEncode("javascript:Select('" + HttpUtility.JavaScriptStringEncode(id ?? string.Empty) 
														+ "','" + HttpUtility.JavaScriptStringEncode(toolTip) 
														+ "','" 
														+ HttpUtility.JavaScriptStringEncode(cardNumber) + "')");
					Image im = new Image();
					im.ImageUrl = "../FMWebApp/Images/Select.gif";
					im.BorderWidth = 0;
					im.Style.Add("align", "absmiddle");
					select.Controls.Add(im);

					e.Item.Cells[0].Controls.Add(select);
				}

				Guid siteGuid = Guid.Parse(e.Item.Cells[2].Text);//bds
				Guid personGuid = Guid.Parse(e.Item.Cells[3].Text);//bds

				var deleteButton = (LinkButton)e.Item.FindControl("DeleteButton");

				if (deleteButton != null)
				{
					deleteButton.Enabled = (this.Security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA) && this.Security.SiteGuid == siteGuid
					                        && personGuid != Guid.Empty && this.PersonSelectContext.Mode != "Unassign");
					
					// Child record versions cannot be created or deleted directly. Their lifetime is controlled by the Entity-To-Site assignment only.
					if (deleteButton.Enabled)
					{
						var pc = (PersonCollectionClass)this.PersonDataGrid.DataSource;
						foreach (PersonClass p in pc)
						{
							if (p.IdentityGuid == personGuid)
							{
								if (p.IdentityGuid != p.MasterRecordGuid)
								{
									deleteButton.Enabled = false; 
								}

								break;
							}
						}
					}
				}

				var editButton = (LinkButton)e.Item.FindControl("EditButton");

				if (editButton != null)
				{
					editButton.Enabled = (this.PersonSelectContext.Mode != "Unassign" && personGuid != Guid.Empty);
				}
			}
		}

		private void UpdateView()
		{
			var personCollection = new PersonCollectionClass();
			this.FindTextBox.Text = this.PersonSelectContext.SearchString;

			if (!string.IsNullOrEmpty(this.PersonSelectContext.IDCarrierLink))
			{
				// Filter by the carrier ID if the the carrier ID is present
				// and contains a value.
				this.FilterByCarrierID(personCollection);
			}
			else
			{
				var limits = new EnumerationLimits();
				this.limit = limits.GetLimit(EnumerationLimits.EnumerationOptions.PERSON);

				if (this.PersonSelectContext.Role == PERSON_ROLE.MAX_PERSON_ROLE)
				{
					personCollection = FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);
				}
				else
				{
					if (this.FindTextBox.Text != string.Empty)
					{
						personCollection = FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(
								x =>
								x.EnumerateByRoleAndFilter(this.Security, this.PersonSelectContext.Role, this.FindTextBox.Text, null)
						);

					}
					else
					{
						personCollection = FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(
								x =>
								x.EnumerateByRoleAndFilter(this.Security, this.PersonSelectContext.Role, this.FindTextBox.Text, null)
						);

					}
				}

				if (personCollection.Count >= this.limit && this.limit > 0)
				{
					this.lblWarning.Text = "Results limited to first " + this.limit + " records.  Use filters to narrow search.";
					this.lblWarning.Visible = true;
				}
				else
				{
					this.lblWarning.Visible = false;
				}

				if (this.PersonSelectContext.Mode != null)
				{
					CompanyClass company = null;
					var companyArrayList = this.Session["CompanyArrayList"] as ArrayList;

					if (companyArrayList != null)
					{
						company = companyArrayList[companyArrayList.Count - 1] as CompanyClass;
					}

					if ("Assign" == this.PersonSelectContext.Mode)
					{
						var unassignedPersonnelCollection = new PersonCollectionClass();
                        
						foreach (PersonClass person in personCollection)
						{
                            if (person.AssignedCompaniesCount > 2)
                            {
                                continue;
                            }

                            bool assigned = false;
						    if (company != null)
						    {
						        foreach (CompanyMapClass assignedPerson in company.AssignedPersonnelCollection)
						        {
						            if (assignedPerson.AssignedToID == person.ID)
						            {
						                assigned = true;
						                break;
						            }
						        }
						    }

						    if (!assigned)
                            {
                                unassignedPersonnelCollection.Add(person);
                            }
						}                       
						personCollection = unassignedPersonnelCollection;                                            
					}
					else
					{
                        var assignedPersonnelCollection = new PersonCollectionClass();

                        foreach (PersonClass person in personCollection)
                        {
                            bool assigned = false;
                            if (company != null)
                            {
                                foreach (CompanyMapClass assignedPerson in company.AssignedPersonnelCollection)
                                {
                                    if (assignedPerson.AssignedToID == person.ID)
                                    {
                                        assigned = true;
                                        break;
                                    }
                                }
                            }

                            if (assigned)
                            {
                                assignedPersonnelCollection.Add(person);
                            }
                        }

                        personCollection = assignedPersonnelCollection;
					}
				}
			}

			if ( this.PersonSelectContext.Null )
			{
				var person = new PersonClass { ID = string.Empty };
				personCollection.Insert(0, person);
			}

            if (this.PersonSelectContext.ExcludeGuid != Guid.Empty)
			{
                var index = personCollection.FindIndex(x => x.IdentityGuid.Equals(this.PersonSelectContext.ExcludeGuid));
				if (index >= 0)
				{
					personCollection.RemoveAt(index);
				}
			}

			this.PersonDataGrid.DataSource = personCollection;
			this.PersonDataGrid.DataBind();
		}
		#endregion
	}

	[Serializable]
	public class PersonSelectContextClass
	{
		#region Constants and Fields
		public string IDCarrierLink;
		public string Mode;
		public PERSON_ROLE Role = PERSON_ROLE.MAX_PERSON_ROLE;
		public string SearchString;
		public bool Unassigned;
		public bool Null;
        /// <summary>
        /// This property is used to exclude the personnel you're configuring from the list of available records.
        /// We don't want to let a supervisor be his/her own supervisor. 
        /// </summary>
	    public Guid ExcludeGuid = Guid.Empty;

        /// <summary>
        /// If true, only personnel records not marked as hidden will be returned
        /// </summary>
        public bool HideHidden;

		#endregion
	}
}