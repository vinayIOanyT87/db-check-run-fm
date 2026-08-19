// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanySelectForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CompanySelectForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Linq;
	using System.Web;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

    using FMCore;

	/// <summary>
	/// Allows a user to choose a company
	/// </summary>
	public partial class CompanySelectForm : FMAutoSubmitFormBase
	{
		#region Constants and Fields

		protected CompanySelectContextClass CompanySelectContext = null;

		protected bool InhibitStartupLoad = false;

		protected string SelectThisItemText = null;

		#endregion

		#region Methods

		protected void FindAllBtn_OnClick(object sender, EventArgs e)
		{
			this.CompanySelectContext.SearchString = null;
			this.FindTextBox.Text = "";
			this.UpdateView();
		}

		protected void FindBtn_OnClick(object sender, EventArgs e)
		{
			if (this.FindTextBox.Text.Length < 1)
			{
				this.CompanySelectContext.SearchString = null;
			}
			else
			{
				this.CompanySelectContext.SearchString = this.FindTextBox.Text.ToUpper();
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
					this.CompanySelectContext = new CompanySelectContextClass();

					if (this.Request.GetQueryOrFormValue("Role") != null)
					{
						this.CompanySelectContext.Role = (COMPANY_ROLE)Enum.Parse(typeof(COMPANY_ROLE), this.Request.GetQueryOrFormValue("Role"));
					}

					if (this.Request.GetQueryOrFormValue("All") != null)
					{
						this.CompanySelectContext.All = Convert.ToBoolean(this.Request.GetQueryOrFormValue("All"));
					}

					if (this.Request.GetQueryOrFormValue("Null") != null)
					{
						this.CompanySelectContext.Null = Convert.ToBoolean(this.Request.GetQueryOrFormValue("Null"));
					}

					if (this.Request.GetQueryOrFormValue("Unassigned") != null)
					{
						this.CompanySelectContext.Unassigned = Convert.ToBoolean(this.Request.GetQueryOrFormValue("Unassigned"));
					}

					// JS20100820 WI-14934
					if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey()))
					{
						if (this.Request.GetQueryOrFormValue("SubRole") != null)
						{
							this.CompanySelectContext.SubRole =
								(COMPANY_SUB_ROLE)Enum.Parse(typeof(COMPANY_SUB_ROLE), this.Request.GetQueryOrFormValue("SubRole"));
						}
					}

					if (this.Request.GetQueryOrFormValue("Inhibit") != null)
					{
						this.InhibitStartupLoad = Convert.ToBoolean(this.Request.GetQueryOrFormValue("Inhibit"));
					}

					if (this.Request.GetQueryOrFormValue("UseHierarchy") != null)
					{
						this.CompanySelectContext.UseHierarchy = Convert.ToBoolean(this.Request.GetQueryOrFormValue("UseHierarchy"));

						if (this.CompanySelectContext.UseHierarchy)
						{
                            switch (this.CompanySelectContext.Role)
                            {
                                case COMPANY_ROLE.OWNER:
								    if (this.Request.GetQueryOrFormValue("ManagerST") != null)
								    {
									    this.CompanySelectContext.ManagerST = Convert.ToString(this.Request.GetQueryOrFormValue("ManagerST"));
								    }
                                    break;
                                case COMPANY_ROLE.SHIPPER:
								    if (this.Request.GetQueryOrFormValue("ManagerST") != null 
                                        && this.Request.GetQueryOrFormValue("OwnerST") != null)
								    {
									    this.CompanySelectContext.ManagerST = Convert.ToString(this.Request.GetQueryOrFormValue("ManagerST"));
									    this.CompanySelectContext.OwnerST = Convert.ToString(this.Request.GetQueryOrFormValue("OwnerST"));
								    }
                                    break;
                                case COMPANY_ROLE.SUPPLIER:
								    if (this.Request.GetQueryOrFormValue("ManagerST") != null 
                                        && this.Request.GetQueryOrFormValue("OwnerST") != null)
								    {
									    this.CompanySelectContext.ManagerST = Convert.ToString(this.Request.GetQueryOrFormValue("ManagerST"));
									    this.CompanySelectContext.OwnerST = Convert.ToString(this.Request.GetQueryOrFormValue("OwnerST"));
								    }
                                    break;
                                case COMPANY_ROLE.CUSTOMER_BILLTO:
                                    if (this.Request.Params["ManagerST"] != null 
                                        && this.Request.Params["OwnerST"] != null 
                                        && this.Request.Params["ShipperST"] != null)
                                    {
                                        this.CompanySelectContext.ManagerST = Convert.ToString(this.Request.Params["ManagerST"]);
                                        this.CompanySelectContext.OwnerST = Convert.ToString(this.Request.Params["OwnerST"]);
                                        this.CompanySelectContext.ShipperST = Convert.ToString(this.Request.Params["ShipperST"]);
                                    }
                                    break;
                                case COMPANY_ROLE.CUSTOMER_SHIPTO:
								    if (this.Request.GetQueryOrFormValue("ManagerST") != null 
                                        && this.Request.GetQueryOrFormValue("OwnerST") != null
								        && this.Request.GetQueryOrFormValue("ShipperST") != null 
                                        && this.Request.GetQueryOrFormValue("BillToST") != null)
								    {
									    this.CompanySelectContext.ManagerST = Convert.ToString(this.Request.GetQueryOrFormValue("ManagerST"));
									    this.CompanySelectContext.OwnerST = Convert.ToString(this.Request.GetQueryOrFormValue("OwnerST"));
									    this.CompanySelectContext.ShipperST = Convert.ToString(this.Request.GetQueryOrFormValue("ShipperST"));
									    this.CompanySelectContext.BillToST = Convert.ToString(this.Request.GetQueryOrFormValue("BillToST"));
								    }
                                    break;
                            }
						}
					}

					if (this.Request.GetQueryOrFormValue("Map") != null)
					{
						try
						{
							this.CompanySelectContext.Map = (int)Enum.Parse(typeof(COMPANY_MAP_TYPE), this.Request.GetQueryOrFormValue("Map"));
							this.CompanySelectContext.MapType = typeof(COMPANY_MAP_TYPE);
						}
						catch
						{
							try
							{
								this.CompanySelectContext.Map = (int)Enum.Parse(typeof(PRODUCT_MAP_TYPE), this.Request.GetQueryOrFormValue("Map"));
								this.CompanySelectContext.MapType = typeof(PRODUCT_MAP_TYPE);
							}
							catch
							{
								try
								{
									this.CompanySelectContext.Map = (int)Enum.Parse(typeof(STRING_MAP_TYPE), this.Request.GetQueryOrFormValue("Map"));
									this.CompanySelectContext.MapType = typeof(STRING_MAP_TYPE);
								}
								catch
								{
									throw new Exception("Unknown Map Type");
								}
							}
						}
					}

					if (this.Request.GetQueryOrFormValue("Mode") != null)
					{
						this.CompanySelectContext.Mode = this.Request.GetQueryOrFormValue("Mode");
					}

					if (!this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA) || this.CompanySelectContext.Mode == "Unassign")
					{
						this.AddButton1.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					if (this.Request.GetQueryOrFormValue("IDLink") != null)
					{
						this.CompanySelectContext.IDLink = this.Request.GetQueryOrFormValue("IDLink");

						// If no ShipTo is selected, don't let them add since there is no way for the 
						// new company to show up in the list.
						if (this.CompanySelectContext.IDLink.Equals(""))
						{
							this.AddButton1.Enabled = false;
							this.AddButton2.Enabled = false;
						}
					}

					if (this.Request.GetQueryOrFormValue("SearchString") != null)
					{
						this.CompanySelectContext.SearchString = this.Request.GetQueryOrFormValue("SearchString");
						this.FindTextBox.Text = this.CompanySelectContext.SearchString;
					}

                    if (this.Request.GetQueryOrFormValue("HideHidden") != null)
                    {
                        this.CompanySelectContext.HideHidden = Convert.ToBoolean(this.Request.GetQueryOrFormValue("HideHidden"));
                    }

					this.Session["CompanySelectContext"] = this.CompanySelectContext;

					this.UpdateView();
				}
				else
				{
					this.CompanySelectContext = this.Session["CompanySelectContext"] as CompanySelectContextClass;
				}

				if (this.CompanySelectContext.Mode != null
				    && (this.CompanySelectContext.Mode == "Assign" || this.CompanySelectContext.Mode == "Unassign"))
				{
					var Form1 = (HtmlForm)this.FindControl("Form1");
					var OkButton = new HtmlInputButton();
					OkButton.Attributes.Add("value", this.GetTranslatedText("OK"));
					OkButton.Attributes.Add("id", "OkButton");
					OkButton.Attributes.Add("class", "formfieldtitle");
					OkButton.Attributes.Add("onclick", "MultipleSelect()");
					OkButton.Attributes.Add("style", "width:66px;Z-INDEX: 107; LEFT: 662px; POSITION: absolute; TOP: 8px");
					Form1.Controls.Add(OkButton);

					var CancelButton = new HtmlInputButton();
					CancelButton.Attributes.Add("value", this.GetTranslatedText("Cancel"));
					CancelButton.Attributes.Add("id", "CancelButton");
					CancelButton.Attributes.Add("class", "formfieldtitle");
					CancelButton.Attributes.Add("onclick", "NoSelect()");
					CancelButton.Attributes.Add("style", "width:66px;Z-INDEX: 107; LEFT: 758px; POSITION: absolute; TOP: 8px");
					Form1.Controls.Add(CancelButton);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddButton_Command(object sender, CommandEventArgs e)
		{
			var CompanyArrayList = this.Session["CompanyArrayList"] as ArrayList;
			if (CompanyArrayList == null)
			{
				CompanyArrayList = new ArrayList();
				this.Session["CompanyArrayList"] = CompanyArrayList;
			}

			var siteClass = FMChannelHelper.MakeCall<ISites, SiteClass>(
													sites => sites.Get(
														this.Security,
														this.Security.SiteGuid,
														getMemberSites: false,
														getSchedulesAndProcessVariables: false,
														bGetAssociatedAliases: false));

			var Company = new CompanyClass(siteClass);

			// WI#1694 (Kendall) - Add the role we are prompting for since role assignment cannot
			// be done now from the Company detail page.
			if (this.Request.GetQueryOrFormValue("Role") != null)
			{
				Company.RoleCollection.Add(
					new CompanyRoleMapClass { CompanyGuid = Company.IdentityGuid, Role = this.CompanySelectContext.Role });
			}

			CompanyArrayList.Add(Company);

			if (this.Session["CompanySelectContextArrayList"] == null)
			{
				var CompanySelectContextArrayList = new ArrayList();
				CompanySelectContextArrayList.Add(this.Session["CompanySelectContext"]);
				this.Session["CompanySelectContextArrayList"] = CompanySelectContextArrayList;
			}
			else
			{
				(this.Session["CompanySelectContextArrayList"] as ArrayList).Add(this.Session["CompanySelectContext"]);
			}

			this.Redirect("CompanyForm.aspx?Modal=true");
		}

		private void CompaniesDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Get Guid
				TableCell guidCell = e.Item.Cells[3];//bds

				FMChannelHelper.MakeCall<ICompanies>(
																	 x =>
																	 x.Purge(this.Security, Guid.Parse(guidCell.Text))
																);
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void CompaniesDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				TableCell guidCell = e.Item.Cells[3];//bds

				var CompanyArrayList = this.Session["CompanyArrayList"] as ArrayList;
				if (CompanyArrayList == null)
				{
					CompanyArrayList = new ArrayList();
					this.Session["CompanyArrayList"] = CompanyArrayList;
				}

				// Get Company
				CompanyClass Company = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(this.Security, Guid.Parse(guidCell.Text))
																);

				CompanyArrayList.Add(Company);

				if (this.Session["CompanySelectContextArrayList"] == null)
				{
					var CompanySelectContextArrayList = new ArrayList();
					CompanySelectContextArrayList.Add(this.Session["CompanySelectContext"]);
					this.Session["CompanySelectContextArrayList"] = CompanySelectContextArrayList;
				}
				else
				{
					(this.Session["CompanySelectContextArrayList"] as ArrayList).Add(this.Session["CompanySelectContext"]);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

            this.Server.Transfer("CompanyForm.aspx?Modal=true&"+Security.CSRFTokenWithParamName);
		}

		private void CompaniesDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex == -1)
			{
				if (e.Item.ItemType == ListItemType.Header)
				{
					if (this.CompanySelectContext.Mode != null
					    && (this.CompanySelectContext.Mode == "Assign" || this.CompanySelectContext.Mode == "Unassign"))
					{
						e.Item.Cells[0].Text = this.GetTranslatedText(this.CompanySelectContext.Mode);
					}
					else
					{
						e.Item.Cells[0].Text = this.GetTranslatedText("Select");
					}
					if (this.CompaniesDataGrid.Columns.Count > 0)
						this.CompaniesDataGrid.Columns[0].HeaderText = e.Item.Cells[0].Text;
				}
			}

			else
			{
					string ID = "";

					// Leave hard space zero length string
					if (e.Item.Cells[4].Text != "&nbsp;")//bds
					{
						ID = HttpUtility.HtmlDecode(e.Item.Cells[4].Text);//bds
					}

				if (this.CompanySelectContext.Mode != null
				    && (this.CompanySelectContext.Mode == "Assign" || this.CompanySelectContext.Mode == "Unassign"))
				{
					var Select = new HtmlInputCheckBox();
					Select.ID = "Select";
					e.Item.Cells[0].Controls.Add(Select);
					Select.Attributes.Add("Title", HttpUtility.JavaScriptStringEncode(this.CompaniesDataGrid.Columns[0].HeaderText + " " + ID));
					e.Item.Cells[4].Text = e.Item.Cells[4].Text.Replace(" ", "&nbsp;");//bds
				}
				else
				{
					string ToolTip = ((e.Item.Cells[6].Text != "&nbsp;") ? e.Item.Cells[6].Text : "")//bds
					                 + ((e.Item.Cells[7].Text != "&nbsp;") ? ", " + e.Item.Cells[7].Text : "")//bds
					                 + ((e.Item.Cells[8].Text != "&nbsp;") ? ", " + e.Item.Cells[8].Text : "")//bds
					                 + ((e.Item.Cells[9].Text != "&nbsp;") ? ", " + e.Item.Cells[9].Text : "");//bds

					string companyName = string.Empty;
					if (e.Item.Cells[6].Text != "&nbsp;")//bds
					{
						companyName = e.Item.Cells[6].Text;//bds
					}

					var select = new HtmlAnchor();
					select.ID = "Select";
					select.HRef = HttpUtility.HtmlEncode("javascript:Select('" + HttpUtility.JavaScriptStringEncode(ID) + "','" + HttpUtility.JavaScriptStringEncode(ToolTip) + "','" + HttpUtility.JavaScriptStringEncode(companyName) + "')");
					Image im = new Image();
					im.ImageUrl = "../FMWebApp/Images/Select.gif";
					im.BorderWidth = 0;
					im.Style.Add("align", "absmiddle");
					select.Controls.Add(im);

					e.Item.Cells[0].Controls.Add(select);
				}

				Guid SiteGuid = Guid.Parse(e.Item.Cells[2].Text);//bds
				Guid companyGuid = Guid.Parse(e.Item.Cells[3].Text);//bds

				var DeleteButton = (LinkButton)e.Item.FindControl("DeleteButton");
				if (DeleteButton != null)
				{
					DeleteButton.Enabled = (this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA) && this.Security.SiteGuid == SiteGuid
					                        && this.CompanySelectContext.Mode != "Unassign" && companyGuid != Guid.Empty)
						                       ? true
						                       : false;

					// Child record versions cannot be created or deleted directly. Their lifetime is controlled by the Entity-To-Site assignment only.
					if (DeleteButton.Enabled)
					{
						var cc = (CompanyCollectionClass)this.CompaniesDataGrid.DataSource;
						foreach (CompanyClass c in cc)
						{
							if (c.IdentityGuid == companyGuid)
							{
								if (c.IdentityGuid != c.MasterRecordGuid)
								{
									DeleteButton.Enabled = false;
								}

								break;
							}
						}
					}
				}

				var EditButton = (LinkButton)e.Item.FindControl("EditButton");
				if (EditButton != null)
				{
					EditButton.Enabled = ((this.CompanySelectContext.Mode != "Unassign") && (companyGuid != Guid.Empty)
					                      && (this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
					                          || this.Security.HasRight(RIGHT.VIEW_COMPANY_DATA)))
						                     ? true
						                     : false;
				}
			}
		}

		private void FilterOnAssociationToFootNote(ref CompanyCollectionClass companyCollection)
		{
			var footNote = this.Session["FootNote"] as FootNoteClass;
			if (footNote == null)
			{
				return;
			}

			ApplicationStringMapCollectionClass assignedApplicationStringMapCollection = null;

			if (this.CompanySelectContext.Role == COMPANY_ROLE.CUSTOMER_SHIPTO)
			{
				assignedApplicationStringMapCollection = footNote.FootNoteShipToMapCollection;
			}
			else if (this.CompanySelectContext.Role == COMPANY_ROLE.SHIPPER)
			{
				assignedApplicationStringMapCollection = footNote.FootNoteShipperMapCollection;
			}

			if (assignedApplicationStringMapCollection == null)
			{
				return;
			}

			if (this.CompanySelectContext.Mode == "Assign")
			{
				// Test for Assignment of {All}
				if (assignedApplicationStringMapCollection.Count == 1
				    && assignedApplicationStringMapCollection[0].AssignedToGuid == Guid.Empty)
				{
					companyCollection.Clear();
				}
				else
				{
					var unassignedCompanyCollection = new CompanyCollectionClass();
					{
						var company = new CompanyClass();
						company.ID = HttpUtility.HtmlEncode(this.GetTranslatedText("{All}"));
						unassignedCompanyCollection.Insert(0, company);
					}

					foreach (CompanyClass company in companyCollection)
					{
						bool assigned = false;
						foreach (ApplicationStringMapClass assignedApplicationStringMap in assignedApplicationStringMapCollection)
						{
							if (company.IdentityGuid == assignedApplicationStringMap.AssignedToGuid)
							{
								assigned = true;
								break;
							}
						}

						if (!assigned)
						{
							unassignedCompanyCollection.Add(company);
						}
					}

					companyCollection = unassignedCompanyCollection;
				}
			}
			else if (this.CompanySelectContext.Mode == "Unassign")
			{
				// Test for Assignment of {All}
				if (assignedApplicationStringMapCollection.Count == 1
				    && assignedApplicationStringMapCollection[0].AssignedToGuid == Guid.Empty)
				{
					companyCollection.Clear();
					var company = new CompanyClass();
					company.ID = HttpUtility.HtmlEncode(this.GetTranslatedText("{All}"));
					companyCollection.Insert(0, company);
				}
				else
				{
					var assignedCompanyCollection = new CompanyCollectionClass();

					foreach (CompanyClass company in companyCollection)
					{
						bool assigned = assignedApplicationStringMapCollection.Any(assignedApplicationStringMap => company.IdentityGuid == assignedApplicationStringMap.AssignedToGuid);

						if (assigned)
						{
							assignedCompanyCollection.Add(company);
						}
					}

					companyCollection = assignedCompanyCollection;
				}
			}
		}

		/// <summary>
		///    This method will create the carrier company collection based on the ShipTo
		///    ID and its authorized carriers. It will return true if the there is a linked
		///    ShipTo ID in session and the the ShipTo company is found. Otherwise, it will
		///    return false meaning that either there was not a matching ShipTo or there was
		///    not a link in the session.
		/// </summary>
		/// <param name="carrierCollection"></param>
		/// <returns></returns>
		private void FilterOnAssociationToShipTo(ref CompanyCollectionClass carrierCollection)
		{
			var AuthorizedCarrierCollection = new CompanyCollectionClass();

			if (this.CompanySelectContext.Role == COMPANY_ROLE.CARRIER && this.CompanySelectContext.IDLink != null)
			{
				string shipToID = this.CompanySelectContext.IDLink;

				if ((shipToID != null) && (shipToID.Length > 0))
				{
					Guid shipToGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(
																	 x =>
																	 x.GetIdentityGuid(base.Security,shipToID)
																);


					if (shipToGuid != Guid.Empty)
					{
						CompanyClass shipToCompany = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(base.Security, shipToGuid)
																);


						if (shipToCompany != null && shipToCompany.AuthorizedCarrierCollection.Any(c => c.AssignedID == "{All}"))
						{
                            // All carriers are Authorized
                            AuthorizedCarrierCollection = carrierCollection;
                        }
						else if (shipToCompany != null)
						{
							// need to filter list on assigned
                            foreach (CompanyClass Carrier in carrierCollection)
                            {
                                if (Carrier.ID.IndexOf("Unassigned") != -1)
                                {
                                    // Make "{Unassigned}" "" because TransactionDetail doesn't used "{Unassigned}"
                                    Carrier.ID = "";
                                    AuthorizedCarrierCollection.Add(Carrier);
                                    continue;
                                }

                                foreach (CompanyMapClass carrierMap in shipToCompany.AuthorizedCarrierCollection)
                                {
                                    if (Carrier.IdentityGuid == carrierMap.AssignedGuid)
                                    {
                                        AuthorizedCarrierCollection.Add(Carrier);
                                        break;
                                    }
                                }
                            }
						}
					}
				}
			}

			carrierCollection = AuthorizedCarrierCollection;
		}

		/// <summary>
		///    This method will filter the company collection based on the product authorized consumers.
		/// </summary>
		/// <param name="CompanyCollection"></param>
		private void FilterOnAuthorizedProducts(CompanyCollectionClass CompanyCollection)
		{
			var ProductArrayList = this.Session["ProductArrayList"] as ArrayList;
			if (ProductArrayList == null)
			{
				throw new Exception("ProductArrayList not in session");
			}

			var Product = ProductArrayList[ProductArrayList.Count - 1] as ProductClass;
			if (Product == null)
			{
				return;
			}

			foreach (ProductMapClass AuthorizedProduct in Product.AuthorizedCustomerCollection)
			{
				var Company = new CompanyClass();
				Company.IdentityGuid = AuthorizedProduct.AssignedToGuid;
				CompanyCollection.Remove(Company);
			}
		}

		/// <summary>
		///    This method will filter the company collection based on the company group.
		/// </summary>
		/// <param name="CompanyCollection"></param>
		private void FilterOnCompanyGroup(CompanyCollectionClass companyCollection)
		{
			var CompanyGroup = this.Session["CompanyGroup"] as CompanyGroupClass;

			if ("Assign" == this.CompanySelectContext.Mode)
			{
				foreach (CompanyMapClass CompanyMap in CompanyGroup.AssignedCompanyCollection)
				{
					var company = companyCollection.FindByMasterRecordGuid(CompanyMap.AssignedGuid);
					if (company != null)
					{
						companyCollection.Remove(company);
					}
				}
			}

			else
			{
				for (int nLoop = companyCollection.Count - 1; nLoop >= 0; --nLoop)
				{
					CompanyClass checkCompany = companyCollection[nLoop];

					if (this.IsCompanyAssignedInCollection(checkCompany, CompanyGroup.AssignedCompanyCollection) == false)
					{
						companyCollection.Remove(checkCompany);
					}
				}
			}
		}

		/// <summary>
		///    This method will filter the company collection based on the groups.
		/// </summary>
		/// <param name="CompanyCollection"></param>
		private void FilterOnUserGroup(CompanyCollectionClass companyCollection)
		{
			var Group = this.Session[GroupForm.SESSION_KEY_GROUP] as GroupClass;

			if (Group != null)
			{
				if ("Assign" == this.CompanySelectContext.Mode)
				{
					foreach (CompanyMapClass companyMap in Group.CompanyMapCollection)
					{
						var company = companyCollection.FindByMasterRecordGuid(companyMap.AssignedGuid);
						if (company != null)
						{
							companyCollection.Remove(company);
						}
					}
				}
				else
				{
					CompanyMapCollectionClass CheckCollection = Group.CompanyMapCollection;

					// Check for {All} entry
					if (CheckCollection.Count != 0)
					{
						CompanyMapClass companyMap = CheckCollection[0];

						if (companyMap.AssignedID == "{All}")
						{
							companyCollection.Clear();

							var Company = new CompanyClass();

							Company.IdentityGuid = companyMap.AssignedGuid;
							Company.ID = HttpUtility.HtmlEncode(this.GetTranslatedText("{All}"));
							Company.Name = companyMap.AssignedName;
							Company.Address1 = companyMap.AssignedAddress;
							Company.City = companyMap.AssignedCity;
							Company.State = companyMap.AssignedState;

							companyCollection.Add(Company);

							return;
						}
					}

					for (int nLoop = companyCollection.Count - 1; nLoop >= 0; --nLoop)
					{
						CompanyClass CheckCompany = companyCollection[nLoop];

						if (this.IsCompanyAssignedInCollection(CheckCompany, CheckCollection) == false)
						{
							companyCollection.Remove(CheckCompany);
						}
					}
				}
			}
		}

		/// <summary>
		///    This method will filter the company collection based on the mode set to Assign.
		/// </summary>
		/// <param name="CompanyCollection"></param>
		private void FilterOnModeAssignment(CompanyCollectionClass CompanyCollection)
		{
			CompanyClass Company = null;

			var CompanyArrayList = this.Session["CompanyArrayList"] as ArrayList;
			if (CompanyArrayList != null)
			{
				Company = CompanyArrayList[CompanyArrayList.Count - 1] as CompanyClass;
			}

			if (Company == null)
			{
				CompanyCollection.Clear();
				return;
			}

			if ("Assign" == this.CompanySelectContext.Mode)
			{
				if (this.CompanySelectContext.Role == COMPANY_ROLE.CARRIER)
				{
					foreach (CompanyMapClass CarrierMap in Company.AuthorizedCarrierCollection)
					{
						var Carrier = new CompanyClass();
						Carrier.IdentityGuid = CarrierMap.AssignedGuid;
						CompanyCollection.Remove(Carrier);
					}
				}

				else if (this.CompanySelectContext.Role == COMPANY_ROLE.CUSTOMER_SHIPTO)
				{
					foreach (CompanyMapClass ShipToMap in Company.CarrierCustomerShipToCollection)
					{
						var ShipTo = new CompanyClass();
						ShipTo.IdentityGuid = ShipToMap.AssignedToGuid;
						CompanyCollection.Remove(ShipTo);
					}
				}
			}
			else
			{
				CompanyMapCollectionClass CheckCollection = null;

				if (this.CompanySelectContext.Role != COMPANY_ROLE.CARRIER)
				{
					CheckCollection = Company.CarrierCustomerShipToCollection;

					for (int nLoop = CompanyCollection.Count - 1; nLoop >= 0; --nLoop)
					{
						CompanyClass CheckCompany = CompanyCollection[nLoop];

						if (this.IsCompanyAssignedToInCollection(CheckCompany, CheckCollection) == false)
						{
							CompanyCollection.Remove(CheckCompany);
						}
					}
				}
				else
				{
					CheckCollection = Company.AuthorizedCarrierCollection;

					for (int nLoop = CompanyCollection.Count - 1; nLoop >= 0; --nLoop)
					{
						CompanyClass CheckCompany = CompanyCollection[nLoop];

						if (this.IsCompanyAssignedInCollection(CheckCompany, CheckCollection) == false)
						{
							CompanyCollection.Remove(CheckCompany);
						}
					}
				}
			}
		}

		private CompanyCollectionClass FilterOnPIDXProfile()
		{
			this.FindTextBox.Text = this.CompanySelectContext.SearchString;

			CompanyCollectionClass CompanyCollection;

			if (this.FindTextBox.Text != "")
			{
				CompanyCollection = 
				FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
						x =>
						x.EnumerateByRoleAndFilter(this.Security,COMPANY_ROLE.CUSTOMER_SHIPTO, this.FindTextBox.Text,false)
				);

			}
			else
			{
				CompanyCollection = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
																	 x =>
																	 x.EnumerateByRole(this.Security, COMPANY_ROLE.CUSTOMER_SHIPTO, false, false)
																);

			}

			var PIDXProfile = this.Session["PIDXProfile"] as PIDXProfileClass;
			PIDXProfileCompanyMapCollectionClass PIDXProfileCompanyMapCollection = PIDXProfile.PIDXProfileCompanyMapCollection;

			var UnassignedCompanyCollection = new CompanyCollectionClass();
			foreach (CompanyClass Company in CompanyCollection)
			{
				UnassignedCompanyCollection.Add(Company);
			}

			return CompanyCollection;
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.CompaniesDataGrid.DeleteCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.CompaniesDataGrid_DeleteCommand);
			this.CompaniesDataGrid.EditCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.CompaniesDataGrid_EditCommand);
			this.AddButton1.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
			this.CompaniesDataGrid.ItemDataBound +=
				new System.Web.UI.WebControls.DataGridItemEventHandler(this.CompaniesDataGrid_ItemDataBound);
			this.AddButton2.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
		}

		private bool IsCompanyAssignedInCollection(CompanyClass Company, CompanyMapCollectionClass CompanyMaps)
		{
			foreach (CompanyMapClass CompanyMap in CompanyMaps)
			{
				if (CompanyMap.AssignedID == Company.ID)
				{
					return true;
				}
			}

			return false;
		}

		private bool IsCompanyAssignedToInCollection(CompanyClass Company, CompanyMapCollectionClass CompanyMaps)
		{
			foreach (CompanyMapClass CompanyMap in CompanyMaps)
			{
				if (CompanyMap.AssignedToID == Company.ID)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		///    This method will load a dataset and create a collection of company
		///    objects. The reason this code is here is due to performance. Marshalling
		///    a lot collection of company object is much slower than marshalling
		///    a DataSet is a know object.
		/// </summary>
		/// <param name="dataSet"></param>
		/// <returns></returns>
		private CompanyCollectionClass LoadCompanySelectData(DataSet dataSet)
		{
			var companyCollection = new CompanyCollectionClass();

			if (dataSet != null)
			{
				DataTable table = dataSet.Tables[0];

				while (table.Rows.Count != 0)
				{
					var company = new CompanyClass();

					company.LoadCompanySelectRole(dataSet);
					companyCollection.Add(company);

					table.Rows.RemoveAt(0);
				}
			}

			return companyCollection;
		}

		private void UpdateView()
		{
			if (this.InhibitStartupLoad == false)
			{
				int limit = -1;

				var CompanyCollection = new CompanyCollectionClass();

				var limits = new EnumerationLimits();
				limit = limits.GetLimit(EnumerationLimits.EnumerationOptions.COMPANY);

				this.FindTextBox.Text = this.CompanySelectContext.SearchString;

				if ("PIDXProfile" == this.CompanySelectContext.Mode)
				{
					CompanyCollection = this.FilterOnPIDXProfile();
				}
				else if ((this.CompanySelectContext.MapType == null) || (typeof(COMPANY_MAP_TYPE) == this.CompanySelectContext.MapType))
				{
					if (this.CompanySelectContext.UseHierarchy)
					{
						DataSet dataSet = FMChannelHelper.MakeCall<ICompanies, DataSet>(
																	 x =>
																	 x.EnumerateHierarchialCustomerFromRoleCompanySelect(
																	this.Security,
																	this.CompanySelectContext.Role,
																	this.CompanySelectContext.ManagerST,
																	this.CompanySelectContext.OwnerST,
																	this.CompanySelectContext.ShipperST,
																	this.CompanySelectContext.BillToST,
																	this.FindTextBox.Text,
                                                                    hideHiddenCompanies: this.CompanySelectContext.HideHidden)
												);

						CompanyCollection = this.LoadCompanySelectData(dataSet);
					}
					else if (this.FindTextBox.Text != "")
					{
						DataSet dataSet = null;

						if (FMChannelHelper.MakeCall<IHardwareKey,Boolean>(x => x.IsADFKey()))
						{
							dataSet = FMChannelHelper.MakeCall<ICompanies, DataSet>(
																	 x =>
																	 x.EnumerateByRoleAndFilterCompanySelectAndLoadType(
                                this.Security, this.CompanySelectContext.Role, this.FindTextBox.Text, true, hideHiddenCompanies: this.CompanySelectContext.HideHidden)
																);
						}
						else
						{
							dataSet = FMChannelHelper.MakeCall<ICompanies, DataSet>(
																	 x =>
																	 x.EnumerateByRoleAndFilterCompanySelect(
                                this.Security, this.CompanySelectContext.Role, this.FindTextBox.Text, hideHiddenCompanies: this.CompanySelectContext.HideHidden)
																);

						}

						CompanyCollection = this.LoadCompanySelectData(dataSet);
					}
					else
					{
						DataSet dataSet = null;

						if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey()))
						{
							dataSet = FMChannelHelper.MakeCall<ICompanies, DataSet>(
												x =>
                                                x.EnumerateCompanySelectRoleByLoadTypes(this.Security, this.CompanySelectContext.Role, true, hideHiddenCompanies: this.CompanySelectContext.HideHidden)
										);

						}
						else
						{
							dataSet = FMChannelHelper.MakeCall<ICompanies, DataSet>(
												x =>
                                                x.EnumerateCompanySelectRole(this.Security, this.CompanySelectContext.Role, hideHiddenCompanies: this.CompanySelectContext.HideHidden)
										);

						}

						CompanyCollection = this.LoadCompanySelectData(dataSet);
					}

					if (this.CompanySelectContext.Null)
					{
						var Company = new CompanyClass();
						Company.ID = "";
						CompanyCollection.Insert(0, Company);
					}

					if (this.CompanySelectContext.All)
					{
						var Company = new CompanyClass();
						Company.ID = HttpUtility.HtmlEncode(this.GetTranslatedText("{All}"));
						CompanyCollection.Insert(0, Company);
					}

					if (this.CompanySelectContext.Unassigned)
					{
						var Company = new CompanyClass();
						Company.ID = HttpUtility.HtmlEncode(this.GetTranslatedText("{Unassigned}"));
						CompanyCollection.Insert(0, Company);
					}

					if (this.CompanySelectContext.Role == COMPANY_ROLE.CARRIER
					    && (COMPANY_MAP_TYPE)this.CompanySelectContext.Map == COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP
					    && this.CompanySelectContext.IDLink != null)
					{
						this.FilterOnAssociationToShipTo(ref CompanyCollection);
					}
					else
					{
						if ((COMPANY_MAP_TYPE)this.CompanySelectContext.Map != COMPANY_MAP_TYPE.LOAD_MAX_COMPANY_MAP_TYPE)
						{
							if ((COMPANY_MAP_TYPE)this.CompanySelectContext.Map == COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP)
							{
								this.FilterOnModeAssignment(CompanyCollection);
							}
							else if ((COMPANY_MAP_TYPE)this.CompanySelectContext.Map == COMPANY_MAP_TYPE.COMPANY_GROUP_COMPANY_MAP)
							{
								this.FilterOnCompanyGroup(CompanyCollection);
							}
							else if ((COMPANY_MAP_TYPE)this.CompanySelectContext.Map == COMPANY_MAP_TYPE.USER_GROUP_COMPANY_MAP)
							{
								this.FilterOnUserGroup(CompanyCollection);
							}
						}
					}
				}

					// PRODUCT_COMPANY_MAP
				else if (typeof(PRODUCT_MAP_TYPE) == this.CompanySelectContext.MapType)
				{
					if (this.FindTextBox.Text != "")
					{
						DataSet dataSet = FMChannelHelper.MakeCall<ICompanies, DataSet>(
																	 x =>
																	 x.EnumerateByRoleAndFilterCompanySelect(
                            this.Security, this.CompanySelectContext.Role, this.FindTextBox.Text, hideHiddenCompanies: this.CompanySelectContext.HideHidden)
																);

						CompanyCollection = this.LoadCompanySelectData(dataSet);
					}
					else
					{
						DataSet dataSet = FMChannelHelper.MakeCall<ICompanies, DataSet>(
																	 x =>
                                                                     x.EnumerateCompanySelectRole(this.Security, this.CompanySelectContext.Role, hideHiddenCompanies: this.CompanySelectContext.HideHidden)
																);

						CompanyCollection = this.LoadCompanySelectData(dataSet);
					}

					this.FilterOnAuthorizedProducts(CompanyCollection);
				}
				else if (typeof(STRING_MAP_TYPE) == this.CompanySelectContext.MapType)
				{
					if (this.FindTextBox.Text != "")
					{
						DataSet dataSet = FMChannelHelper.MakeCall<ICompanies, DataSet>(
																	 x =>
																	 x.EnumerateByRoleAndFilterCompanySelect(
                            this.Security, this.CompanySelectContext.Role, this.FindTextBox.Text, hideHiddenCompanies: this.CompanySelectContext.HideHidden)
																);
						CompanyCollection = this.LoadCompanySelectData(dataSet);
					}
					else
					{
						DataSet dataSet = FMChannelHelper.MakeCall<ICompanies, DataSet>(
																	 x =>
                                                                     x.EnumerateCompanySelectRole(this.Security, this.CompanySelectContext.Role, hideHiddenCompanies: this.CompanySelectContext.HideHidden)
																);

						CompanyCollection = this.LoadCompanySelectData(dataSet);
					}

					this.FilterOnAssociationToFootNote(ref CompanyCollection);
				}

				// JS20100820 WI-14934 filter the collection based on role
				if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey()) &&
					this.CompanySelectContext.SubRole != COMPANY_SUB_ROLE.NO_SUBROLE)
				{
					var ADFCompanyCollection = new CompanyCollectionClass();

					foreach (CompanyClass company in CompanyCollection)
					{
						string companyType = null;
						switch (this.CompanySelectContext.Role)
						{
							case COMPANY_ROLE.CUSTOMER_BILLTO:
								companyType = company.CustomerBillToTypeID.ToUpper();
								break;
							case COMPANY_ROLE.CUSTOMER_SHIPTO:
								companyType = company.CustomerShipToTypeID.ToUpper();
								break;
						}

						if (!string.IsNullOrEmpty(companyType))
						{
							if (this.CompanySelectContext.SubRole == COMPANY_SUB_ROLE.ADF && companyType.Equals("ADF"))
							{
								ADFCompanyCollection.Add(company);
							}
							else if (this.CompanySelectContext.SubRole == COMPANY_SUB_ROLE.OTHER && companyType.Equals("OTHER"))
							{
								ADFCompanyCollection.Add(company);
							}
						}
					}

					CompanyCollection = ADFCompanyCollection;
				}

				if (CompanyCollection.Count >= limit && limit > 0)
				{
					this.lblWarning.Text = "Results limited to first " + limit + " records.  Use filters to narrow search.";
					this.lblWarning.Visible = true;
				}
				else
				{
					this.lblWarning.Visible = false;
				}

				this.CompaniesDataGrid.DataSource = CompanyCollection;
				this.CompaniesDataGrid.DataBind();
			}
			else
			{
				this.InhibitStartupLoad = false;
			}
		}

		#endregion
	}

	[Serializable]
	public class CompanySelectContextClass
	{
		#region Constants and Fields

		public bool All = false;

		public string BillToST = null;

		public string IDLink = null;

		public string ManagerST = null;

		public int Map = 0;

		public Type MapType = null;

		public string Mode = null;

		public bool Null = false;

		public string OwnerST = null;

		public COMPANY_ROLE Role = COMPANY_ROLE.MAX_COMPANY_ROLE;

		public string SearchString = null;

		public string ShipperST = null;

		public COMPANY_SUB_ROLE SubRole = COMPANY_SUB_ROLE.NO_SUBROLE;

		public bool Unassigned = false;

		public bool UseHierarchy = false;

        /// <summary>
        /// If true, companies that are marked as hidden will not be shown
        /// </summary>
	    public bool HideHidden = false;

	    #endregion
	}
}