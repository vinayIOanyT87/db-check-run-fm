// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FootNoteForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FootNoteForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Globalization;
	using System.Web;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	///    Summary description for FootNoteForm.
	/// </summary>
	public partial class FootNoteForm : FMAutoSubmitFormBase
	{
		#region Enums

		private enum AssignmentType
		{
			ShipTo = 0,
			Shipper = 1,
			ShipToState = 2,
			Product = 3,
            AdditiveProfile = 4
		};

		#endregion

		#region Properties

		private string JavascriptStartup
		{
			get
			{
				string script = @"
				<script type='text/javascript'>
				<!--
					// Set Assign and Unassign Button values according to Data Dictionary
					var AssignButton=document.getElementById('FootNoteForm_AssignButton');
					if(AssignButton != null)
						AssignButton.value='" + ((FMFormBase)this.Page).GetTranslatedText("Assign") + @"';
					var UnassignButton=document.getElementById('FootNoteForm_UnassignButton');
					if(UnassignButton != null)
						UnassignButton.value='" + ((FMFormBase)this.Page).GetTranslatedText("Unassign") + @"';
				//-->
				</script>
				";
				return script;
			}
		}

		#endregion

		#region Methods

		protected void AssignEntitiesTextBoxTextChanged(object sender, EventArgs e)
		{
			try
			{
				string[] ids = this.AssignEntitiesTextBox.Text.Split('|');
				this.AssignEntitiesTextBox.Text = "";

				var type = (AssignmentType)Convert.ToInt32(this.TypeDropDownList.SelectedValue);

				var footNote = this.Session["FootNote"] as FootNoteClass;
				if (footNote == null)
				{
					return;
				}

				ApplicationStringMapCollectionClass assignedCollection;

				if (type == AssignmentType.ShipTo)
				{
					assignedCollection = footNote.FootNoteShipToMapCollection;
				}

				else if (type == AssignmentType.Shipper)
				{
					assignedCollection = footNote.FootNoteShipperMapCollection;
				}

				else if (type == AssignmentType.ShipToState)
				{
					assignedCollection = footNote.FootNoteShipToStateMapCollection;
				}

				else if (type == AssignmentType.Product)
				{
					assignedCollection = footNote.FootNoteProductMapCollection;
				}

                else if (type == AssignmentType.AdditiveProfile)
                {
                    assignedCollection = footNote.FootNoteAdditiveProfileMapCollection;
                }

                else
				{
					return;
				}

				// Remove {All}
				if (assignedCollection.Count == 1 && assignedCollection[0].AssignedToGuid == Guid.Empty)
				{
					assignedCollection.Clear();
				}

				if (type == AssignmentType.ShipTo || type == AssignmentType.Shipper)
				{
					foreach (string id in ids)
					{
						if (id == "|")
						{
							continue;
						}

					    var applicationStringMap = new ApplicationStringMapClass
					                               {
					                                   Type =
					                                       (type == AssignmentType.ShipTo)
					                                           ? STRING_MAP_TYPE.FOOT_NOTE_SHIPTO
					                                           : STRING_MAP_TYPE.FOOT_NOTE_SHIPPER
					                               };

					    if (id == this.GetTranslatedText("{All}"))
						{
							assignedCollection.Clear();
							applicationStringMap.AssignedToID = "{All}";
							applicationStringMap.AssignedToGuid = Guid.Empty;
							assignedCollection.Add(applicationStringMap);
							break;
						}
						
						CompanyClass company =
							FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
								companies => companies.Get(this.Security, companies.GetIdentityGuid(this.Security, id), false));

						applicationStringMap.AssignedToID = company.ID;
						applicationStringMap.AssignedToName = company.Name;
						applicationStringMap.AssignedToAddress = company.Address1;
						applicationStringMap.AssignedToCity = company.City;
						applicationStringMap.AssignedToState = company.State;
						applicationStringMap.AssignedToGuid = company.MasterRecordGuid;
						assignedCollection.Add(applicationStringMap);
					}
				}

				else if (type == AssignmentType.ShipToState)
				{
					foreach (string id in ids)
					{
						if (id == "|")
						{
							continue;
						}

					    var applicationStringMap = new ApplicationStringMapClass { Type = STRING_MAP_TYPE.FOOT_NOTE_SHIPTO_STATE };

					    if (id.Equals(this.GetTranslatedText("{All}")))
						{
							assignedCollection.Clear();
							applicationStringMap.AssignedToID = "{All}";
							applicationStringMap.AssignedToGuid = Guid.Empty;
							assignedCollection.Add(applicationStringMap);
							break;
						}


						applicationStringMap.AssignedToID = id;
						assignedCollection.Add(applicationStringMap);
					}
				}

				else if (type == AssignmentType.AdditiveProfile)
				{
					foreach (string id in ids)
					{
						if (id.Equals("|"))
						{
							continue;
						}

					    var applicationStringMap = new ApplicationStringMapClass { Type = STRING_MAP_TYPE.FOOT_NOTE_ADDITIVE_PROFILE };

					    if (id.Equals(this.GetTranslatedText("{All}")))
						{
							assignedCollection.Clear();
							applicationStringMap.AssignedToID = "{All}";
							applicationStringMap.AssignedToGuid = Guid.Empty;
							assignedCollection.Add(applicationStringMap);
							break;
						}
						
						
						AdditiveProfileClass additiveProfile =
							FMChannelHelper.MakeCall<IAdditiveProfiles, AdditiveProfileClass>(
								x => x.Get(this.Security, x.GetIdentityGuid(this.Security, id)));

						applicationStringMap.AssignedToID = additiveProfile.ID;
						applicationStringMap.AssignedToDescription = additiveProfile.Description;
                        applicationStringMap.AssignedToGuid = additiveProfile.IdentityGuid;
						assignedCollection.Add(applicationStringMap);
					}
				}

                else if (type.Equals(AssignmentType.Product))
                {
                    foreach (string id in ids)
                    {
                        if (id.Equals("|"))
                        {
                            continue;
                        }

                        var applicationStringMap = new ApplicationStringMapClass { Type = STRING_MAP_TYPE.FOOT_NOTE_PRODUCT };

                        if (id.Equals(this.GetTranslatedText("{All}")))
                        {
                            assignedCollection.Clear();
                            applicationStringMap.AssignedToID = "{All}";
                            applicationStringMap.AssignedToGuid = Guid.Empty;
                            assignedCollection.Add(applicationStringMap);
                            break;
                        }


                        ProductClass product =
                            FMChannelHelper.MakeCall<IProducts, ProductClass>(
                                x => x.GetByInfoAuthorizedCompanies(this.Security, x.GetIdentityGuid(this.Security, id), false, true));

                        applicationStringMap.AssignedToID = product.ID;
                        applicationStringMap.AssignedToCode = product.Code;
                        applicationStringMap.AssignedToDescription = product.Description;
                        applicationStringMap.AssignedToProductType = product.ProductType;
                        applicationStringMap.AssignedToGuid = product.MasterRecordGuid;
                        assignedCollection.Add(applicationStringMap);
                    }
                }

                this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
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

				if (!this.Page.IsPostBack)
				{
					FootNoteClass footNote;

					if (this.Session["IdentityGuid"] != null)
					{
						footNote =
							FMChannelHelper.MakeCall<IFootNotes, FootNoteClass>(
							    // ReSharper disable once AssignNullToNotNullAttribute
								x => x.Get(this.Security, Guid.Parse(this.Session["IdentityGuid"] as string)));
					}
					else
					{
						footNote = new FootNoteClass();
					}

					this.Session["FootNote"] = footNote;

					this.Name.Text = footNote.ID;

                    if (footNote.StartDate.HasValue)
                    {
                        this.startDate.Text = footNote.StartDate.Value.ToString(CultureInfo.CurrentCulture);
                    }
                    else
                    {
                        this.startDate.Text = "";
                    }

                    if (footNote.EndDate.HasValue)
                    {
                        this.endDate.Text = footNote.EndDate.Value.ToString(CultureInfo.CurrentCulture); 
                    }
                    else
                    {
                        this.endDate.Text = "";
                    }

                    // Populate the TypeDropDownList
					AssignmentType[] types =
						{
							AssignmentType.ShipTo, AssignmentType.Shipper, AssignmentType.ShipToState,
							AssignmentType.Product, AssignmentType.AdditiveProfile
						};

					foreach (AssignmentType type in types)
					{
						var item = new ListItem(this.AssignmentTypeID(type), ((int)type).ToString(CultureInfo.InvariantCulture));
						this.TypeDropDownList.Items.Add(item);
					}

					if ((!this.Security.HasRight(RIGHT.MODIFY_PRODUCTS) && !this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
					    || (this.Security.SiteGuid != footNote.SiteGuid && footNote.IdentityGuid != Guid.Empty))
					{
						this.OK.Enabled = false;
					}
					this.UpdateView();
				}

				this.Page.ClientScript.RegisterStartupScript(this.GetType(), "FootNoteFormScriptBlock", this.JavascriptStartup);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void TypeDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.UpdateView();
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );
			}
		}

		protected void UnassignEntitiesTextBoxTextChanged(object sender, EventArgs e)
		{
			try
			{
				var footNote = this.Session["FootNote"] as FootNoteClass;
				if (footNote == null)
				{
					return;
				}

				ApplicationStringMapCollectionClass assignedCollection;

				var type = (AssignmentType)Convert.ToInt32(this.TypeDropDownList.SelectedValue);

				if (type == AssignmentType.ShipTo)
				{
					assignedCollection = footNote.FootNoteShipToMapCollection;
				}
				else if (type == AssignmentType.Shipper)
				{
					assignedCollection = footNote.FootNoteShipperMapCollection;
				}
				else if (type == AssignmentType.ShipToState)
				{
					assignedCollection = footNote.FootNoteShipToStateMapCollection;
				}
				else if (type == AssignmentType.Product)
				{
					assignedCollection = footNote.FootNoteProductMapCollection;
				}
                else if (type == AssignmentType.AdditiveProfile)
                {
                    assignedCollection = footNote.FootNoteAdditiveProfileMapCollection;
                }
                else
				{
					return;
				}

				string[] ids = this.UnassignEntitiesTextBox.Text.Split('|');
				this.UnassignEntitiesTextBox.Text = "";

				foreach (string id in ids)
				{
					if (id == "|")
					{
						continue;
					}

					int index = 0;
					foreach (ApplicationStringMapClass applicationStringMap in assignedCollection)
					{
						string translatedText = this.GetTranslatedText(applicationStringMap.AssignedToID);

						if (translatedText == id)
						{
							assignedCollection.RemoveAt(index);
							break;
						}
						index++;
					}
				}

				this.UpdateView();
			}
			catch (Exception except)
			{
			    this.ErrorHandler(except);
			}
		}

		private void AssignedEntitiesDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			try
			{
				if (e.Item.ItemIndex != -1)
				{
					var type = (AssignmentType)Convert.ToInt32(this.TypeDropDownList.SelectedValue);

					var idLabel = (Label)e.Item.FindControl("IDLabel");

					var footNote = this.Session["FootNote"] as FootNoteClass;
					if (footNote == null)
					{
						return;
					}

					ApplicationStringMapCollectionClass assignedCollection;

					if (type == AssignmentType.ShipTo)
					{
						assignedCollection = footNote.FootNoteShipToMapCollection;
					}

					else if (type == AssignmentType.Shipper)
					{
						assignedCollection = footNote.FootNoteShipperMapCollection;
					}

					else if (type == AssignmentType.ShipToState)
					{
						assignedCollection = footNote.FootNoteShipToStateMapCollection;
					}

					else if (type == AssignmentType.Product)
					{
						assignedCollection = footNote.FootNoteProductMapCollection;
					}

                    else if (type == AssignmentType.AdditiveProfile)
                    {
                        assignedCollection = footNote.FootNoteAdditiveProfileMapCollection;
                    }

                    else
					{
						return;
					}

					ApplicationStringMapClass applicationStringMap = assignedCollection[e.Item.DataSetIndex];
					idLabel.Text = applicationStringMap.AssignedToID;

					if (idLabel.Text == "{All}")
					{
						idLabel.Text = HttpUtility.HtmlEncode(this.GetTranslatedText("{All}"));
					}

					idLabel.ToolTip = applicationStringMap.ToolTip;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AssignedEntitiesDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.AssignedEntitiesDataGrid.EditItemIndex > -1)
				{
					return;
				}

				this.AssignedEntitiesDataGrid.CurrentPageIndex = e.NewPageIndex;
				this.UpdateView();
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );
			}
		}

		private string AssignmentTypeID(AssignmentType type)
		{
			switch (type)
			{
				case AssignmentType.ShipTo:
					return "Ship To";

				case AssignmentType.Shipper:
					return "Shipper";

				case AssignmentType.ShipToState:
					return "State";

				case AssignmentType.Product:
					return "Product";

                case AssignmentType.AdditiveProfile:
                    return "Additive Profile";

				default:
					return "";
			}
		}

		private void CancelCommand(object sender, CommandEventArgs e)
		{
			this.Redirect("FootNotesForm.aspx");
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.OK.Command += this.OkCommand;
			this.Cancel.Command += this.CancelCommand;
			this.AssignedEntitiesDataGrid.PageIndexChanged += this.AssignedEntitiesDataGridPageIndexChanged;
			this.AssignedEntitiesDataGrid.ItemDataBound += this.AssignedEntitiesDataGridItemDataBound;
		}

		private void OkCommand(object sender, CommandEventArgs e)
		{
			try
			{
				var footNote = (FootNoteClass)this.Session["FootNote"];

				if (this.Name.Text == "")
				{
					var except = new Exception("ID is a required field.");
					this.ErrorHandler(except);
					return;
				}

				footNote.ID = this.Name.Text;

                if (string.IsNullOrEmpty(this.startDate.Text))
                {
                    footNote.StartDate = null;
                }
                else
                {
                    footNote.StartDate = this.startDate.DateTimeValue;
                }

                if (string.IsNullOrEmpty(this.endDate.Text))
                {
                    footNote.EndDate = null;
                }
                else
                {
                    footNote.EndDate = this.endDate.DateTimeValue; 
                }

                if (footNote.StartDate.HasValue &&
                    footNote.EndDate.HasValue &&
                    footNote.StartDate.Value > footNote.EndDate.Value)
                {
                    Exception except = new Exception("End Date must be later than Start Date.");
                    this.ErrorHandler(except);
                    return;
                }


				if (footNote.IdentityGuid != Guid.Empty)
				{
					FMChannelHelper.MakeCall<IFootNotes>(x => x.Modify(this.Security, footNote));
				}
				else
				{
					FMChannelHelper.MakeCall<IFootNotes, Guid>(x => x.Add(this.Security, footNote));
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Redirect("FootNotesForm.aspx");
		}

		private void UpdateView()
		{
			if (this.TypeDropDownList.SelectedValue == "")
			{
				return;
			}

			var footNote = this.Session["FootNote"] as FootNoteClass;
			if (footNote == null)
			{
				return;
			}

			var type = (AssignmentType)Convert.ToInt32(this.TypeDropDownList.SelectedValue);

			if (type == AssignmentType.ShipTo)
			{
				this.AssignedEntitiesDataGrid.DataSource = footNote.FootNoteShipToMapCollection;
			}

			else if (type == AssignmentType.Shipper)
			{
				this.AssignedEntitiesDataGrid.DataSource = footNote.FootNoteShipperMapCollection;
			}

			else if (type == AssignmentType.ShipToState)
			{
				this.AssignedEntitiesDataGrid.DataSource = footNote.FootNoteShipToStateMapCollection;
			}

			else if (type == AssignmentType.Product)
			{
				this.AssignedEntitiesDataGrid.DataSource = footNote.FootNoteProductMapCollection;
			}

            else if (type == AssignmentType.AdditiveProfile)
            {
                this.AssignedEntitiesDataGrid.DataSource = footNote.FootNoteAdditiveProfileMapCollection;
            }

            else
			{
				return;
			}

			var applicationStringMapCollectionClass = (ApplicationStringMapCollectionClass)this.AssignedEntitiesDataGrid.DataSource;
			if (applicationStringMapCollectionClass != null)
			{
				int count = applicationStringMapCollectionClass.Count;

				if ((count - 1) / this.AssignedEntitiesDataGrid.PageSize < this.AssignedEntitiesDataGrid.CurrentPageIndex)
				{
					this.AssignedEntitiesDataGrid.CurrentPageIndex = (count - 1) / this.AssignedEntitiesDataGrid.PageSize;
				}
			}

			this.AssignedEntitiesDataGrid.DataBind();
		}

		#endregion
	}
}