// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IATACodeSelectForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IATACodeSelectForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{ 
	using System;
	using System.Collections;
	using System.Data;
	using System.Web;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

    using FMCore;

	/// <summary>
	/// Allows a user to choose an IATACode
	/// </summary>
    public partial class IATACodeSelectForm : FMAutoSubmitFormBase
	{
		#region Constants and Fields

        protected IATACodeSelectContextClass IATACodeSelectContext = null;

		protected string SelectThisItemText = null;

		#endregion

		#region Methods

		protected void FindAllBtn_OnClick(object sender, EventArgs e)
		{
            this.IATACodeSelectContext.SearchString = null;
			this.FindTextBox.Text = "";
			this.UpdateView();
		}

		protected void FindBtn_OnClick(object sender, EventArgs e)
		{
			if (this.FindTextBox.Text.Length < 1)
			{
                this.IATACodeSelectContext.SearchString = null;
			}
			else
			{
                this.IATACodeSelectContext.SearchString = this.FindTextBox.Text.ToUpper();
			}

			this.UpdateView();
		}

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
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
                    this.IATACodeSelectContext = new IATACodeSelectContextClass();

					if (this.Request.GetQueryOrFormValue("All") != null)
					{
                        this.IATACodeSelectContext.All = Convert.ToBoolean(this.Request.GetQueryOrFormValue("All"));
					}

					if (this.Request.GetQueryOrFormValue("Null") != null)
					{
                        this.IATACodeSelectContext.Null = Convert.ToBoolean(this.Request.GetQueryOrFormValue("Null"));
					}

					if (this.Request.GetQueryOrFormValue("Unassigned") != null)
					{
                        this.IATACodeSelectContext.Unassigned = Convert.ToBoolean(this.Request.GetQueryOrFormValue("Unassigned"));
					}

					if (this.Request.GetQueryOrFormValue("Mode") != null)
					{
                        this.IATACodeSelectContext.Mode = this.Request.GetQueryOrFormValue("Mode");
					}

                    if (!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) || this.IATACodeSelectContext.Mode == "Unassign")
					{
						this.AddButton1.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					if (this.Request.GetQueryOrFormValue("IDLink") != null)
					{
                        this.IATACodeSelectContext.IDLink = this.Request.GetQueryOrFormValue("IDLink");

						// If no ShipTo is selected, don't let them add since there is no way for the 
                        // new IATACode to show up in the list.
                        if (this.IATACodeSelectContext.IDLink.Equals(""))
						{
							this.AddButton1.Enabled = false;
							this.AddButton2.Enabled = false;
						}
					}

					if (this.Request.GetQueryOrFormValue("SearchString") != null)
					{
                        this.IATACodeSelectContext.SearchString = this.Request.GetQueryOrFormValue("SearchString");
                        this.FindTextBox.Text = this.IATACodeSelectContext.SearchString;
					}

                    this.Session["IATACodeSelectContext"] = this.IATACodeSelectContext;

					this.UpdateView();
				}
				else
				{
                    this.IATACodeSelectContext = this.Session["IATACodeSelectContext"] as IATACodeSelectContextClass;
				}

                if (this.IATACodeSelectContext.Mode != null
                    && (this.IATACodeSelectContext.Mode == "Assign" || this.IATACodeSelectContext.Mode == "Unassign"))
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
            var IATACodeArrayList = this.Session["IATACodeArrayList"] as ArrayList;
            if (IATACodeArrayList == null)
			{
                IATACodeArrayList = new ArrayList();
                this.Session["IATACodeArrayList"] = IATACodeArrayList;
			}
			var iataCode = new IATACodeClass();


            IATACodeArrayList.Add(iataCode);

            if (this.Session["IATACodeSelectContextArrayList"] == null)
			{
                var IATACodeSelectContextArrayList = new ArrayList();
                IATACodeSelectContextArrayList.Add(this.Session["IATACodeSelectContext"]);
                this.Session["IATACodeSelectContextArrayList"] = IATACodeSelectContextArrayList;
			}
			else
			{
                (this.Session["IATACodeSelectContextArrayList"] as ArrayList).Add(this.Session["IATACodeSelectContext"]);
			}

			this.Redirect("IATACodeMainForm.aspx");
		}

        private void IATACodesDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Get Guid
				TableCell guidCell = e.Item.Cells[3];//bds
			    Guid guid = Guid.Parse(guidCell.Text);
			    FMChannelHelper.MakeCall<IIATACodes>(x => x.Purge(this.Security, guid));
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

        private void IATACodesDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				TableCell guidCell = e.Item.Cells[3];//bds

                var IATACodeArrayList = this.Session["IATACodeArrayList"] as ArrayList;
                if (IATACodeArrayList == null)
				{
                    IATACodeArrayList = new ArrayList();
                    this.Session["IATACodeArrayList"] = IATACodeArrayList;
				}

				// Get IATACode
				IATACodeClass iataCode = FMChannelHelper.MakeCall<IIATACodes, IATACodeClass>(
																	 x =>
																	 x.Get(this.Security, Guid.Parse(guidCell.Text))
																);

                IATACodeArrayList.Add(iataCode);

                if (this.Session["IATACodeSelectContextArrayList"] == null)
				{
                    var IATACodeSelectContextArrayList = new ArrayList();
                    IATACodeSelectContextArrayList.Add(this.Session["IATACodeSelectContext"]);
                    this.Session["IATACodeSelectContextArrayList"] = IATACodeSelectContextArrayList;
				}
				else
				{
                    (this.Session["IATACodeSelectContextArrayList"] as ArrayList).Add(this.Session["IATACodeSelectContext"]);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			this.Server.Transfer("IATACodeMainForm.aspx");
		}

        private void IATACodesDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex == -1)
			{
				if (e.Item.ItemType == ListItemType.Header)
				{
                    if (this.IATACodeSelectContext.Mode != null
                        && (this.IATACodeSelectContext.Mode == "Assign" || this.IATACodeSelectContext.Mode == "Unassign"))
					{
                        e.Item.Cells[0].Text = this.GetTranslatedText(this.IATACodeSelectContext.Mode);
					}
					else
					{
						e.Item.Cells[0].Text = this.GetTranslatedText("Select");
					}
				}
			}

			else
			{
                if (this.IATACodeSelectContext.Mode != null
                    && (this.IATACodeSelectContext.Mode == "Assign" || this.IATACodeSelectContext.Mode == "Unassign"))
				{
					var Select = new HtmlInputCheckBox();
					Select.ID = "Select";
					e.Item.Cells[0].Controls.Add(Select);

					e.Item.Cells[4].Text = e.Item.Cells[4].Text.Replace(" ", "&nbsp;");//bds
				}
				else
				{
					string ID = "";

					// Leave hard space zero length string
					if (e.Item.Cells[4].Text != "&nbsp;")//bds
					{
						ID = HttpUtility.HtmlDecode(e.Item.Cells[4].Text);//bds
					}

					string ToolTip = ((e.Item.Cells[6].Text != "&nbsp;") ? e.Item.Cells[6].Text : "")//bds
					                 + ((e.Item.Cells[7].Text != "&nbsp;") ? ", " + e.Item.Cells[7].Text : "")//bds
					                 + ((e.Item.Cells[8].Text != "&nbsp;") ? ", " + e.Item.Cells[8].Text : "")//bds
					                 + ((e.Item.Cells[9].Text != "&nbsp;") ? ", " + e.Item.Cells[9].Text : "");//bds

					string iataCodeName = string.Empty;
					if (e.Item.Cells[6].Text != "&nbsp;")//bds
					{
                        iataCodeName = e.Item.Cells[6].Text;//bds
					}

					var select = new HtmlAnchor();
					select.ID = "Select";
                    select.HRef = HttpUtility.HtmlEncode("javascript:Select('" + HttpUtility.JavaScriptStringEncode(ID) + "','" + HttpUtility.JavaScriptStringEncode(ToolTip) + "','" + HttpUtility.JavaScriptStringEncode(iataCodeName) + "')");
					select.InnerHtml = "<img src=\"../FMWebApp/Images/Select.gif\" border=\"0\" align=\"absmiddle\" alt='"
					                   + HttpUtility.HtmlEncode(this.SelectThisItemText) + "'>";

					e.Item.Cells[0].Controls.Add(select);
				}

				Guid SiteGuid = Guid.Parse(e.Item.Cells[2].Text);//bds
				Guid iataGuid = Guid.Parse(e.Item.Cells[3].Text);//bds

				var DeleteButton = (LinkButton)e.Item.FindControl("DeleteButton");
				if (DeleteButton != null)
				{
					DeleteButton.Enabled = (this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) && this.Security.SiteGuid == SiteGuid
                                            && this.IATACodeSelectContext.Mode != "Unassign" && iataGuid != Guid.Empty)
						                       ? true
						                       : false;

				}

				var EditButton = (LinkButton)e.Item.FindControl("EditButton");
			    if (EditButton != null)
			    {
			        EditButton.Enabled = ((this.IATACodeSelectContext.Mode != "Unassign") && (iataGuid != Guid.Empty)
			                              && (this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
			                                  || this.Security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)))
			            ? true
			            : false;
			    }
			}
		}

        /// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.IATACodesDataGrid.DeleteCommand +=
                new System.Web.UI.WebControls.DataGridCommandEventHandler(this.IATACodesDataGrid_DeleteCommand);
            this.IATACodesDataGrid.EditCommand +=
                new System.Web.UI.WebControls.DataGridCommandEventHandler(this.IATACodesDataGrid_EditCommand);
			this.AddButton1.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
            this.IATACodesDataGrid.ItemDataBound +=
                new System.Web.UI.WebControls.DataGridItemEventHandler(this.IATACodesDataGrid_ItemDataBound);
			this.AddButton2.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
		}

		/// <summary>
		///    This method will load a dataset and create a collection of IATACode
		///    objects. The reason this code is here is due to performance. Marshalling
        ///    a lot collection of IATACode object is much slower than marshalling
		///    a DataSet is a know object.
		/// </summary>
		/// <param name="dataSet"></param>
		/// <returns></returns>
        private IATACodeCollectionClass LoadIATACodeSelectData(DataSet dataSet)
		{
			var iataCodeCollection = new IATACodeCollectionClass();

			if (dataSet != null)
			{
				DataTable table = dataSet.Tables[0];

				while (table.Rows.Count != 0)
				{
					var iataCode = new IATACodeClass();

                    iataCode.Load(dataSet);
                    iataCodeCollection.Add(iataCode);

					table.Rows.RemoveAt(0);
				}
			}

            return iataCodeCollection;
		}

	    private void UpdateView()
	    {
	        int limit = -1;

            var IATACodeCollection = new IATACodeCollectionClass();

	        var limits = new EnumerationLimits();
	        limit = limits.GetLimit(EnumerationLimits.EnumerationOptions.DEFAULT);

	        this.FindTextBox.Text = this.IATACodeSelectContext.SearchString;

	        if (this.FindTextBox.Text != "")
	        {
                IATACodeCollection = FMChannelHelper.MakeCall<IIATACodes, IATACodeCollectionClass>(x => x.Enumerate(this.Security));
                //throw new NotImplementedException("IATACode Filtering not implemented");
	        }
	        else
	        {
                IATACodeCollection = FMChannelHelper.MakeCall<IIATACodes, IATACodeCollectionClass>(x => x.Enumerate(this.Security));
	        }

	        if (this.IATACodeSelectContext.Null)
	        {
	            var iataCode = new IATACodeClass();
	            iataCode.ID = "";
	            IATACodeCollection.Insert(0, iataCode);
	        }

	        if (this.IATACodeSelectContext.All)
	        {
	            var iataCode = new IATACodeClass();
	            iataCode.ID = HttpUtility.HtmlEncode(this.GetTranslatedText("{All}"));
	            IATACodeCollection.Insert(0, iataCode);
	        }

	        if (this.IATACodeSelectContext.Unassigned)
	        {
	            var iataCode = new IATACodeClass();
	            iataCode.ID = HttpUtility.HtmlEncode(this.GetTranslatedText("{Unassigned}"));
	            IATACodeCollection.Insert(0, iataCode);
	        }

            if (IATACodeCollection.Count >= limit && limit > 0)
	        {
	            this.lblWarning.Text = "Results limited to first " + limit + " records.  Use filters to narrow search.";
	            this.lblWarning.Visible = true;
	        }
	        else
	        {
	            this.lblWarning.Visible = false;
	        }

	        this.IATACodesDataGrid.DataSource = IATACodeCollection;
	        this.IATACodesDataGrid.DataBind();
	    }

	    #endregion
	}

	[Serializable]
	public class IATACodeSelectContextClass
	{
		#region Constants and Fields

		public bool All = false;

		public string IDLink = null;

		public string Mode = null;

		public bool Null = false;

		public string SearchString = null;

		public bool Unassigned = false;

	    #endregion
	}
}