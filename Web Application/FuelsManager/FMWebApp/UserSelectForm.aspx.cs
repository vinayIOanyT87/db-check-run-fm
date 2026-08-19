// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserSelectForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the UserSelectForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Web;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

    using FMCore;

	/// <summary>
    ///    Summary description for UserSelectForm.
    /// </summary>
    public partial class UserSelectForm : FMAutoSubmitFormBase
    {
        #region Constants and Fields


        protected string SelectThisItemText = null;

        private int limit = -1;

        private string searchString = String.Empty;

        protected string SearchString { get { return this.searchString; } set { this.searchString = value; } }

        #endregion

        #region Methods

        protected void FindAllBtn_OnClick(object sender, EventArgs e)
        {
            this.SearchString = String.Empty;
            this.Session["UserSelectSearchString"] = this.SearchString;
            this.FindTextBox.Text = "";
            this.UpdateView();
        }

        protected void FindBtn_OnClick(object sender, EventArgs e)
        {
            if (this.FindTextBox.Text.Length < 1)
            {
                this.SearchString = String.Empty;
            }
            else
            {
                this.SearchString = this.FindTextBox.Text.ToUpper();
            }
            this.Session["UserSelectSearchString"] = this.SearchString;
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
                    if (this.Request.GetQueryOrFormValue("SearchString") != null)
                    {
                        this.SearchString = this.Request.GetQueryOrFormValue("SearchString");
                        this.FindTextBox.Text = this.SearchString;
                    }
                    this.Session["UserSelectSearchString"] = this.SearchString;
                    this.UpdateView();
                }
                else
                {
                    this.SearchString = this.Session["UserSelectSearchString"] as string;
                }
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }

 
 

        /// <summary>
        ///    Required method for Designer support - do not modify
        ///    the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.AddButton1.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
            this.UserDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.UserDataGrid_ItemDataBound);
            this.UserDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.UserDataGrid_EditCommand);
            this.UserDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.UserDataGrid_DeleteCommand);
            this.AddButton2.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
        }

        private void AddButton_Command(object sender, CommandEventArgs e)
        {

            this.Session["UserSelectSearchString"] = this.SearchString;

            this.Redirect("UserForm.aspx");
        }

        private void UserDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            try
            {
                // Get IdentityGuid
                TableCell identityGuidCell = e.Item.Cells[(int)EGridColumns.UserGuid];

                FMChannelHelper.MakeCall<IUsers>(
                                                                     x =>
                                                                     x.Purge(this.Security, Guid.Parse(identityGuidCell.Text))
                                                                );
                this.UpdateView();
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }

        private void UserDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
        {
            try
            {
                TableCell identityGuidCell = e.Item.Cells[(int)EGridColumns.UserGuid];
                this.Session[UserForm.SessionKeyUserGuid] = Guid.Parse(identityGuidCell.Text);

            }
            catch (Exception except)
            {   
                this.ErrorHandler(except);
                return;
            }

            this.Redirect("UserForm.aspx");
        }

        private enum EGridColumns
        {
            Select = 0,
            Edit,
            Delete,
            SiteGuid,
            UserGuid,
            ID,
            Name,
            EmailAddress
        }

        private void UserDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemIndex == -1)
            {
                if (e.Item.ItemType == ListItemType.Header)
                {
                    e.Item.Cells[(int)EGridColumns.Select].Text = this.GetTranslatedText("Select");
				if (this.UserDataGrid.Columns.Count > 0)
					this.UserDataGrid.Columns[0].HeaderText = e.Item.Cells[0].Text;
				}
            }

            else
            {
                string ID = "";

                // Leave hard space zero length string
                if (e.Item.Cells[(int)EGridColumns.ID].Text != "&nbsp;")
                {
                    ID = HttpUtility.HtmlDecode(e.Item.Cells[(int)EGridColumns.ID].Text);
                }

                string ToolTip = ((e.Item.Cells[(int)EGridColumns.Name].Text != "&nbsp;") ? e.Item.Cells[(int)EGridColumns.Name].Text : "")
                                    + ((e.Item.Cells[(int)EGridColumns.EmailAddress].Text != "&nbsp;") ? " " + e.Item.Cells[(int)EGridColumns.EmailAddress].Text : "");


                var Select = new HtmlAnchor();
                Select.ID = "Select";
                Select.HRef = HttpUtility.HtmlEncode("javascript:Select('" + HttpUtility.JavaScriptStringEncode(ID) + "')");

			Image im = new Image();
			im.ImageUrl = "../FMWebApp/Images/Select.gif";
			im.BorderWidth = 0;
			im.Style.Add("align", "absmiddle");
			Select.Controls.Add(im);

			e.Item.Cells[(int)EGridColumns.Select].Controls.Add(Select);

                Guid siteGuid = Guid.Parse(e.Item.Cells[(int)EGridColumns.SiteGuid].Text);
                Guid userGuid = Guid.Parse(e.Item.Cells[(int)EGridColumns.UserGuid].Text);

                var DeleteButton = (LinkButton)e.Item.FindControl("DeleteButton");
                if (DeleteButton != null)
                {
                    DeleteButton.Enabled = (this.Security.HasRight(RIGHT.MODIFY_USERS) && this.Security.SiteGuid == siteGuid
                                            && userGuid != Guid.Empty)
                                               ? true
                                               : false;

                    if (DeleteButton.Enabled)
                    {
                        var pc = (UserCollectionClass)this.UserDataGrid.DataSource;
                        foreach (UserClass p in pc)
                        {
                            if (p.IdentityGuid == userGuid)
                            {
                                DeleteButton.Enabled = false;
                                break;
                            }
                        }
                    }
                }
                var EditButton = (LinkButton)e.Item.FindControl("EditButton");
                if (EditButton != null)
                {
                    EditButton.Enabled = (userGuid != Guid.Empty) ? true : false;
                }
            }

        }

        private void UpdateView()
        {
            var UserCollection = new UserCollectionClass();

            if (UserCollection.Count >= this.limit && this.limit > 0)
            {
                this.lblWarning.Text = "Results limited to first " + this.limit + " records.  Use filters to narrow search.";
                this.lblWarning.Visible = true;
            }
            else
            {
                this.lblWarning.Visible = false;
            }

            if (this.FindTextBox.Text != "")
            {
                UserCollection = FMChannelHelper.MakeCall<IUsers, UserCollectionClass>(
                        x =>
                        x.EnumerateAndFilter(this.Security, this.FindTextBox.Text)
                );

            }
            else
            {
                UserCollection = FMChannelHelper.MakeCall<IUsers, UserCollectionClass>(
                        x =>
                        x.Enumerate(this.Security)
                );

            }

            var user = new UserClass();
            user.ID = string.Empty;
            UserCollection.Insert(0, user);
            this.UserDataGrid.DataSource = UserCollection;
            this.UserDataGrid.DataBind();
        }

        #endregion
    }

}