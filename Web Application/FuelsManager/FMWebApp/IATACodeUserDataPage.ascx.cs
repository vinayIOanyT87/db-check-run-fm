// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IATACodeUserDataPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Code behind for IATACodeUserDataPage.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.FMWebApp
{
    using System;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.DataObjects;

    /// <summary>
    /// Summary description for IATACodeUserDataPage.
    /// </summary>
    public partial class IATACodeUserDataPage : FMUserDataControlBase
    {
        protected IATACodeClass IATACode
        {
            get
            {
                return ((IATACodeMainForm)Page).IATACode;
            }
        }

        protected override System.Web.UI.WebControls.Table Table
        {
            get
            {
                return UserDataTable;
            }
        }

        protected override ENTITY_TYPE EntityType
        {
            get
            {
                IATACodeClass IATACode = new IATACodeClass();
                return IATACode.EntityType;
            }
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    int Index = 0;
                    foreach (UserDataFieldClass UserDataField in userDataFieldCollection)
                    {
                        if (UserDataField.UserDataType == USER_DATA_TYPE.TEXT)
                        {
                            TextBox ValueTextBox = (TextBox)UserDataTable.Rows[Index].Cells[2].Controls[0];
                            ValueTextBox.Text = IATACode.UserData[UserDataField.Number];
                        }
                        else
                        {
                            DropDownList ValueDropDownList =
                                (DropDownList)UserDataTable.Rows[Index].Cells[2].Controls[0];
                            ListItem Item = ValueDropDownList.Items.FindByText(IATACode.UserData[UserDataField.Number]);
                            if (Item == null)
                            {
                                ValueDropDownList.Items.Add(
                                    new ListItem(
                                        IATACode.UserData[UserDataField.Number],
                                        IATACode.UserData[UserDataField.Number]));
                                ValueDropDownList.SelectedIndex = ValueDropDownList.Items.Count - 1;
                            }
                            else ValueDropDownList.SelectedIndex = ValueDropDownList.Items.IndexOf(Item);
                        }
                        Index++;
                    }
                }
            }
            catch (Exception except)
            {
                ErrorHandler(except);
            }
        }

        #region Web Form Designer generated code

        protected override void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }

        #endregion

        public void UpdateData()
        {
            if (IATACode != null)
            {

                bool DontPerformRVCheck = (this.IATACode.IdentityGuid.Equals(Guid.Empty)
                                           || (this.IATACode.SiteGuid == this.Security.SiteGuid));
                int Index = 0;
                foreach (UserDataFieldClass UserDataField in userDataFieldCollection)
                {
                    if (DontPerformRVCheck)
                    {
                        if (UserDataField.UserDataType == USER_DATA_TYPE.TEXT)
                        {
                            TextBox ValueTextBox = (TextBox)UserDataTable.Rows[Index].Cells[2].Controls[0];
                            IATACode.UserData[UserDataField.Number] = ValueTextBox.Text;
                        }
                        else
                        {
                            DropDownList ValueDropDownList =
                                (DropDownList)UserDataTable.Rows[Index].Cells[2].Controls[0];
                            IATACode.UserData[UserDataField.Number] = ValueDropDownList.SelectedValue;
                        }
                    }
                    Index++;
                }
            }
        }
    }
}
